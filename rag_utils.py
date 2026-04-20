import os
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain_community.embeddings import HuggingFaceEmbeddings
from langchain_community.vectorstores import Chroma
from langchain_core.documents import Document

EMBEDDING_MODEL_NAME = "moka-ai/m3e-base"
embeddings = HuggingFaceEmbeddings(model_name=EMBEDDING_MODEL_NAME)

CHROMA_PERSIST_DIR = "./chroma_db"

def add_paper_to_vector_db(paper_id: int, full_text: str):
    #将论文全文切块、向量化，并存入 Chroma 数据库。

    if not full_text or len(full_text.strip()) == 0:
        print(f"论文 {paper_id} 文本为空，跳过向量化。")
        return False

    try:
        # 步骤 A：初始化文本切分器
        # chunk_size: 每块大约 500 个字符
        # chunk_overlap: 相邻两块重叠 50 个字符，防止切断上下文的逻辑连贯性
        text_splitter = RecursiveCharacterTextSplitter(
            chunk_size=500,
            chunk_overlap=50,
            separators=["\n\n", "\n", "。", "！", "？", " ", ""]
        )

        # 步骤 B：切分长文本
        texts = text_splitter.split_text(full_text)
        
        # 步骤 C：包装成 Document 对象，并绑定极其重要的 Metadata！
        # 这样我们以后检索出来的每一段话，都知道它属于数据库里的哪篇论文
        documents = []
        for text in texts:
            doc = Document(
                page_content=text,
                metadata={"paper_id": paper_id}  # 关联写在 MySQL 里的 ID
            )
            documents.append(doc)

        # 步骤 D：存入 Chroma 向量数据库
        # persist_directory 会让数据保存在本地磁盘，下次重启不用重新跑
        vector_db = Chroma.from_documents(
            documents=documents,
            embedding=embeddings,
            persist_directory=CHROMA_PERSIST_DIR
        )
        
        print(f"成功！论文 ID {paper_id} 已切分为 {len(texts)} 块并存入 Chroma。")
        return True

    except Exception as e:
        print(f"存入向量库失败，论文 ID {paper_id}，错误: {e}")
        return False