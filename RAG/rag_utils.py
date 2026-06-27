"""
RAG 核心：检索(Retrieval) + 生成(Generation)。

- add_paper_to_vector_db : 把一篇论文的文本切块、向量化、存入 Chroma（建索引）
- search_similar_texts   : 语义检索，返回最相关的若干段原文（R）
- answer_question        : 检索 + 调大模型生成回答（R + G，问答主入口）

嵌入模型用本地 m3e（免费、离线、中文友好）；生成用 DeepSeek（见 llm_utils.py）。
向量库里每段文本的 metadata.paper_id == SQL Server 的 Literature.id。
"""
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain_community.embeddings import HuggingFaceEmbeddings
from langchain_community.vectorstores import Chroma
from langchain_core.documents import Document

import config
import llm_utils

EMBEDDING_MODEL_NAME = config.EMBEDDING_MODEL_NAME
CHROMA_PERSIST_DIR = config.CHROMA_PERSIST_DIR

# 嵌入模型惰性加载：第一次真正用到时才加载，避免一导入就吃内存/下载模型
_embeddings = None


def get_embeddings():
    global _embeddings
    if _embeddings is None:
        _embeddings = HuggingFaceEmbeddings(model_name=EMBEDDING_MODEL_NAME)
    return _embeddings


def add_paper_to_vector_db(paper_id: int, full_text: str) -> bool:
    """将论文文本切块、向量化，并存入 Chroma 数据库。paper_id 用 Literature.id。"""
    if not full_text or len(full_text.strip()) == 0:
        print(f"论文 {paper_id} 文本为空，跳过向量化。")
        return False

    try:
        text_splitter = RecursiveCharacterTextSplitter(
            chunk_size=500,
            chunk_overlap=50,
            separators=["\n\n", "\n", "。", "！", "？", " ", ""],
        )
        texts = text_splitter.split_text(full_text)

        documents = [
            Document(page_content=t, metadata={"paper_id": int(paper_id)})
            for t in texts
        ]

        vector_db = get_vector_db()
        vector_db.add_documents(documents)
        # 新版 chromadb 会自动持久化；老版本调用 persist 兜底
        try:
            vector_db.persist()
        except Exception:
            pass

        print(f"成功！论文 ID {paper_id} 已切分为 {len(texts)} 块并存入 Chroma。")
        return True
    except Exception as e:
        print(f"存入向量库失败，论文 ID {paper_id}，错误: {e}")
        return False


def delete_paper_from_vector_db(paper_id: int) -> bool:
    """删除某篇论文的旧向量（重新索引前先清掉，避免重复）。库为空时静默跳过。"""
    try:
        vector_db = get_vector_db()
        # 先查这篇论文是否真的有旧向量，没有就不调 delete（避免空库报无害错误）
        existing = vector_db.get(where={"paper_id": int(paper_id)})
        if existing and existing.get("ids"):
            vector_db.delete(ids=existing["ids"])
        return True
    except Exception as e:
        # 这一步失败通常无害（多见于首次运行、库还是空的），降为提示即可
        print(f"(提示) 清理论文 {paper_id} 旧向量时跳过：{e}")
        return False


def get_vector_db():
    """加载（或新建）Chroma 向量库实例。"""
    return Chroma(
        persist_directory=CHROMA_PERSIST_DIR,
        embedding_function=get_embeddings(),
    )


def _build_filter(paper_id):
    """构造 Chroma 过滤条件，兼容新旧两种语法。"""
    if not paper_id:
        return None
    return {"paper_id": int(paper_id)}


def search_similar_texts(query: str, paper_id: int = None, k: int = None):
    """
    语义检索（RAG 的 R）。
    - paper_id 传入则只在该篇论文内部搜（单篇问答）；不传则全局搜。
    返回: [{"content": 段落文本, "paper_id": 所属论文id}, ...]
    """
    if not query or not query.strip():
        return []
    if k is None:
        k = config.RAG_TOP_K

    vector_db = get_vector_db()
    try:
        results = vector_db.similarity_search(
            query=query, k=k, filter=_build_filter(paper_id)
        )
        return [
            {"content": doc.page_content, "paper_id": doc.metadata.get("paper_id")}
            for doc in results
        ]
    except Exception as e:
        print(f"语义检索失败，错误: {e}")
        return []


def answer_question(question: str, paper_id: int = None, k: int = None):
    """
    智能问答主入口（RAG 的 R + G）。
    1) 按 paper_id 检索相关原文段落
    2) 把段落 + 问题交给 DeepSeek 生成回答
    返回: {"answer": 回答, "sources": [检索到的段落...]}
    """
    chunks = search_similar_texts(question, paper_id=paper_id, k=k)
    context = [c["content"] for c in chunks]
    answer = llm_utils.generate_answer(question, context)
    return {"answer": answer, "sources": chunks}
