"""
RAG 核心：检索(Retrieval) + 生成(Generation)。

- add_paper_to_vector_db : 把一篇论文的文本切块、向量化、存入 Chroma（建索引）
- search_similar_texts   : 语义检索，返回最相关的若干段原文（R）
- answer_question        : 检索 + 调大模型生成回答（R + G，问答主入口）

嵌入模型用本地 m3e（免费、离线、中文友好）；生成用 DeepSeek（见 llm_utils.py）。
向量库里每段文本的 metadata.paper_id == SQL Server 的 Literature.id。
"""
import html
import re

from langchain_community.embeddings import HuggingFaceEmbeddings
from langchain_community.vectorstores import Chroma
from langchain_core.documents import Document

import config
import llm_utils

try:
    from pdf_parser import repair_common_pdf_word_splits
except Exception:
    repair_common_pdf_word_splits = None

EMBEDDING_MODEL_NAME = config.EMBEDDING_MODEL_NAME
CHROMA_PERSIST_DIR = config.CHROMA_PERSIST_DIR

# 嵌入模型惰性加载：第一次真正用到时才加载，避免一导入就吃内存/下载模型
_embeddings = None

OVERVIEW_QUERY = "abstract introduction method approach experiment results conclusion contribution"
STRUCTURE_QUESTION_HINTS = (
    "结构", "框架", "梳理", "总结", "概括", "主要研究", "主要内容", "贡献",
    "outline", "structure", "summary", "overview", "contribution",
)
SECTION_HEADING_RE = re.compile(
    r"^(?:\d+(?:\.\d+)*\.?\s+|[IVX]+\.\s+)?"
    r"(Abstract|Introduction|Related\s+Work|Background|Method|Methodology|Approach|"
    r"Model|Architecture|Experiment(?:s|al)?|Evaluation|Result(?:s)?|Discussion|"
    r"Conclusion|Acknowledg(?:e)?ments?|"
    r"摘要|引言|背景|相关工作|方法|实验|结果|讨论|结论)\b",
    re.IGNORECASE,
)
MAX_CONTEXT_CHARS = 16000
BODY_CHUNK_MAX_CHARS = 1200
BODY_CHUNK_MIN_CHARS = 180
SOURCE_EXCERPT_MAX_CHARS = 700
DISPLAY_SOURCE_LIMIT = 6
FORCED_MERGE_MAX_CHARS = 2200


def clean_rag_index_text(full_text: str) -> str:
    text = html.unescape(full_text or "")
    text = re.sub(r"ξξ_([A-Za-z]+)_ξξ", lambda match: match.group(1), text)
    text = text.replace("\xa0", " ")
    text = text.replace("\r\n", "\n").replace("\r", "\n")

    blocks = re.split(r"(\n\s*\n+)", text)
    cleaned_blocks = []
    for block in blocks:
        if not block:
            continue
        if re.fullmatch(r"\n\s*\n+", block):
            cleaned_blocks.append("\n\n")
            continue
        current = re.sub(r"[ \t]+", " ", block)
        if repair_common_pdf_word_splits is not None:
            current = repair_common_pdf_word_splits(current)
        current = re.sub(r"\s+([,.;:!?%)\]])", r"\1", current)
        current = re.sub(r"([(\[])\s+", r"\1", current)
        cleaned_blocks.append(current.strip())

    text = "".join(cleaned_blocks)
    text = re.sub(r"\n{3,}", "\n\n", text)
    return text.strip()


def get_embeddings():
    global _embeddings
    if _embeddings is None:
        _embeddings = HuggingFaceEmbeddings(model_name=EMBEDDING_MODEL_NAME)
    return _embeddings


def is_overview_question(question: str) -> bool:
    lowered = (question or "").lower()
    return any(hint.lower() in lowered for hint in STRUCTURE_QUESTION_HINTS)


def extract_section_outline(full_text: str, max_items: int = 24):
    outline = []
    seen = set()
    for raw in (full_text or "").splitlines():
        line = re.sub(r"\s+", " ", raw).strip()
        if not line or len(line) > 120:
            continue
        if not SECTION_HEADING_RE.search(line):
            continue
        if not line[0].isdigit() and line[0].islower():
            continue
        if len(line.split()) > 10 and not re.match(r"^\d+(?:\.\d+)*\.?\s+", line):
            continue
        if not re.match(r"^\d+(?:\.\d+)*\.?\s+", line) and re.search(r"\.\s+[A-Za-z]", line) and len(line.split()) > 3:
            continue
        if re.search(r"\b(as shown|achieves|improves|outperforms|combined with|average denotes|model average|quantitative|qualitative|presented|demonstrates|figure|table|we first)\b", line, re.IGNORECASE):
            continue
        key = line.lower()
        if key in seen:
            continue
        seen.add(key)
        outline.append(line)
        if len(outline) >= max_items:
            break
    return outline


def build_overview_chunk(full_text: str) -> str:
    text = re.sub(r"\n{3,}", "\n\n", (full_text or "").strip())
    if not text:
        return ""
    outline = extract_section_outline(text)
    prefix_start = 0
    for pattern in (r"\bAbstract\b", r"\b1\s+Introduction\b", r"\bIntroduction\b", r"摘要", r"引言"):
        match = re.search(pattern, text, re.IGNORECASE)
        if match and match.start() < 5000:
            prefix_start = match.start()
            break
    prefix = text[prefix_start:prefix_start + 4200]
    parts = []
    if outline:
        parts.append("Section outline:\n" + "\n".join(outline))
    parts.append("Paper opening and abstract:\n" + prefix)
    return "\n\n".join(parts).strip()


def is_section_heading_line(text: str) -> bool:
    line = re.sub(r"\s+", " ", (text or "").strip())
    if not line or len(line) > 120:
        return False
    if not SECTION_HEADING_RE.search(line):
        return False
    if not line[0].isdigit() and line[0].islower():
        return False
    if len(line.split()) > 10 and not re.match(r"^\d+(?:\.\d+)*\.?\s+", line):
        return False
    if not re.match(r"^\d+(?:\.\d+)*\.?\s+", line) and re.search(r"\.\s+[A-Za-z]", line) and len(line.split()) > 3:
        return False
    if re.search(r"\b(as shown|achieves|improves|outperforms|combined with|average denotes|model average|quantitative|qualitative|presented|demonstrates|figure|table|we first)\b", line, re.IGNORECASE):
        return False
    return True


def split_sentences(text: str):
    normalized = re.sub(r"\s+", " ", (text or "").strip())
    if not normalized:
        return []
    return [
        part.strip()
        for part in re.split(r"(?<=[。！？!?])\s+|(?<=[.!?])\s+(?=[A-Z0-9])", normalized)
        if part and part.strip()
    ]


def append_chunk(chunks, text: str):
    chunk = re.sub(r"[ \t]+", " ", (text or "").strip())
    chunk = re.sub(r"\n{3,}", "\n\n", chunk)
    if len(chunk) >= BODY_CHUNK_MIN_CHARS or is_section_heading_line(chunk.split("\n", 1)[0]):
        chunks.append(chunk)


def ends_with_sentence_boundary(text: str) -> bool:
    stripped = re.sub(r"\s+", " ", (text or "").strip())
    if not stripped:
        return True
    if is_section_heading_line(stripped):
        return False
    return bool(re.search(r"[。！？!?。.!?][\"'”’)\]]*$", stripped))


def should_force_merge(current: str) -> bool:
    text = (current or "").strip()
    if not text:
        return False
    if len(text) < BODY_CHUNK_MIN_CHARS:
        return True
    return not ends_with_sentence_boundary(text)


def split_long_paragraph(paragraph: str):
    sentences = split_sentences(paragraph)
    if not sentences:
        return []
    chunks = []
    current = ""
    for sentence in sentences:
        if not current:
            current = sentence
            continue
        merged_len = len(current) + 1 + len(sentence)
        if merged_len <= BODY_CHUNK_MAX_CHARS or (should_force_merge(current) and merged_len <= FORCED_MERGE_MAX_CHARS):
            current += " " + sentence
        else:
            append_chunk(chunks, current)
            current = sentence
    append_chunk(chunks, current)
    return chunks


def build_body_chunks(full_text: str):
    paragraphs = [
        re.sub(r"[ \t]+", " ", part).strip()
        for part in re.split(r"\n\s*\n+", full_text or "")
        if part and part.strip()
    ]
    chunks = []
    current = ""
    pending_heading = ""

    for paragraph in paragraphs:
        if is_section_heading_line(paragraph):
            if current:
                append_chunk(chunks, current)
                current = ""
            pending_heading = paragraph
            continue

        if pending_heading:
            paragraph = pending_heading + "\n" + paragraph
            pending_heading = ""

        parts = split_long_paragraph(paragraph) if len(paragraph) > BODY_CHUNK_MAX_CHARS else [paragraph]
        for part in parts:
            if not current:
                current = part
                continue
            merged_len = len(current) + 2 + len(part)
            force_merge = should_force_merge(current) and merged_len <= FORCED_MERGE_MAX_CHARS
            if merged_len <= BODY_CHUNK_MAX_CHARS or force_merge:
                current += (" " if force_merge else "\n\n") + part
            else:
                append_chunk(chunks, current)
                current = part

    if pending_heading:
        if current:
            current += "\n\n" + pending_heading
        else:
            current = pending_heading
    append_chunk(chunks, current)
    return chunks


def build_source_excerpt(content: str, max_chars: int = SOURCE_EXCERPT_MAX_CHARS) -> str:
    text = re.sub(r"[ \t]+", " ", (content or "").strip())
    text = re.sub(r"\n{3,}", "\n\n", text)
    if len(text) <= max_chars:
        return text

    cut = text[:max_chars].rstrip()
    boundary_candidates = [cut.rfind(mark) for mark in ("。", "！", "？", ".", "!", "?")]
    boundary = max(boundary_candidates)
    if boundary >= int(max_chars * 0.45):
        return cut[:boundary + 1]

    newline = cut.rfind("\n")
    if newline >= int(max_chars * 0.45):
        return cut[:newline].rstrip()

    return cut.rstrip(" ,;:") + "..."


def prepare_sources_for_display(chunks):
    sources = []
    for chunk in chunks[:DISPLAY_SOURCE_LIMIT]:
        item = dict(chunk)
        item["raw_chars"] = len(item.get("content") or "")
        item["content"] = build_source_excerpt(item.get("content") or "")
        sources.append(item)
    return sources


def add_paper_to_vector_db(paper_id: int, full_text: str) -> bool:
    """将论文文本切块、向量化，并存入 Chroma 数据库。paper_id 用 Literature.id。"""
    if not full_text or len(full_text.strip()) == 0:
        print(f"论文 {paper_id} 文本为空，跳过向量化。")
        return False

    try:
        full_text = clean_rag_index_text(full_text)
        texts = build_body_chunks(full_text)

        documents = []
        overview = build_overview_chunk(full_text)
        if overview:
            documents.append(
                Document(
                    page_content=overview,
                    metadata={"paper_id": int(paper_id), "chunk_type": "overview"},
                )
            )
        documents.extend(
            Document(
                page_content=t,
                metadata={"paper_id": int(paper_id), "chunk_type": "body", "chunk_index": index},
            )
            for index, t in enumerate(texts)
        )

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
            {
                "content": doc.page_content,
                "paper_id": doc.metadata.get("paper_id"),
                "chunk_type": doc.metadata.get("chunk_type", "body"),
            }
            for doc in results
        ]
    except Exception as e:
        print(f"语义检索失败，错误: {e}")
        return []


def get_overview_chunks(paper_id: int, limit: int = 2):
    if not paper_id:
        return []
    try:
        vector_db = get_vector_db()
        existing = vector_db.get(where={"paper_id": int(paper_id)}, include=["documents", "metadatas"])
        documents = existing.get("documents") or []
        metadatas = existing.get("metadatas") or []
        chunks = []
        for content, metadata in zip(documents, metadatas):
            metadata = metadata or {}
            if metadata.get("chunk_type") == "overview":
                chunks.append({
                    "content": content,
                    "paper_id": metadata.get("paper_id"),
                    "chunk_type": "overview",
                })
                if len(chunks) >= limit:
                    break
        return chunks
    except Exception as e:
        print(f"读取论文概览块失败，错误: {e}")
        return []


def dedupe_chunks(chunks):
    deduped = []
    seen = set()
    total_chars = 0
    for chunk in chunks:
        content = (chunk.get("content") or "").strip()
        if not content:
            continue
        key = re.sub(r"\s+", " ", content[:220].lower())
        if key in seen:
            continue
        seen.add(key)
        if total_chars + len(content) > MAX_CONTEXT_CHARS and deduped:
            break
        total_chars += len(content)
        deduped.append(chunk)
    return deduped


def answer_question(question: str, paper_id: int = None, k: int = None):
    """
    智能问答主入口（RAG 的 R + G）。
    1) 按 paper_id 检索相关原文段落
    2) 把段落 + 问题交给 DeepSeek 生成回答
    返回: {"answer": 回答, "sources": [检索到的段落...]}
    """
    if k is None:
        k = config.RAG_TOP_K
    chunks = []
    if paper_id and is_overview_question(question):
        chunks.extend(get_overview_chunks(paper_id))
        chunks.extend(search_similar_texts(OVERVIEW_QUERY, paper_id=paper_id, k=max(k, 8)))
        chunks.extend(search_similar_texts(question, paper_id=paper_id, k=max(k, 6)))
    else:
        chunks.extend(search_similar_texts(question, paper_id=paper_id, k=k))
    chunks = dedupe_chunks(chunks)
    context = [c["content"] for c in chunks]
    answer = llm_utils.generate_answer(question, context, is_overview=is_overview_question(question))
    return {"answer": answer, "sources": prepare_sources_for_display(chunks)}
