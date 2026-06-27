"""
统一配置：所有可变参数集中在这里，全部从环境变量读取（找不到就用默认值）。
本地开发时把这些写进同目录下的 .env 文件即可，不要把真实 key 提交到 git。
"""
import os

# 关掉 Chroma 的匿名遥测，避免控制台刷一堆无害的 "Failed to send telemetry" 提示。
# 必须在 chromadb 被导入之前设置，所以放在最顶上。
os.environ.setdefault("ANONYMIZED_TELEMETRY", "False")
os.environ.setdefault("CHROMA_TELEMETRY_IMPL", "none")

# 尝试加载 .env 文件（python-dotenv 没装也不报错）。
# 显式指定本文件同目录下的 .env，这样无论从哪个目录运行都能读到。
try:
    from dotenv import load_dotenv
    load_dotenv(os.path.join(os.path.dirname(os.path.abspath(__file__)), ".env"))
except Exception:
    pass


# ============ DeepSeek / 大模型 ============
# DeepSeek 用的是 OpenAI 兼容接口，所以下面这些字段和 OpenAI 一样。
# 想换成通义千问(Qwen)也只要改 BASE_URL 和 MODEL 两个值即可。
LLM_API_KEY = os.getenv("LLM_API_KEY", "")              # 必填：你的 DeepSeek API Key
LLM_BASE_URL = os.getenv("LLM_BASE_URL", "https://api.deepseek.com")
LLM_MODEL = os.getenv("LLM_MODEL", "deepseek-v4-flash")
LLM_TIMEOUT = int(os.getenv("LLM_TIMEOUT", "60"))


# ============ SQL Server（真网站的数据库） ============
# 默认值取自 网站源码/Web/Web.config 里的 SQLCONNECTIONSTRING。
# data source=(local)\SQLEXPRESS; Initial Catalog=manage_db_final; User ID=sa; Password=123456
SQLSERVER_HOST = os.getenv("SQLSERVER_HOST", r"localhost\SQLEXPRESS")
SQLSERVER_PORT = int(os.getenv("SQLSERVER_PORT", "1433"))
SQLSERVER_USER = os.getenv("SQLSERVER_USER", "sa")
SQLSERVER_PASSWORD = os.getenv("SQLSERVER_PASSWORD", "123456")
SQLSERVER_DB = os.getenv("SQLSERVER_DB", "manage_db_final")


# ============ 向量库 / 嵌入模型 ============
EMBEDDING_MODEL_NAME = os.getenv("EMBEDDING_MODEL_NAME", "moka-ai/m3e-base")
CHROMA_PERSIST_DIR = os.getenv("CHROMA_PERSIST_DIR", "./chroma_db")

# 检索召回多少段原文喂给大模型
RAG_TOP_K = int(os.getenv("RAG_TOP_K", "4"))


# ============ PDF 文件根目录 ============
# LiteratureFile.file_path 里如果存的是相对路径（如 /upload/xxx.pdf），
# 回填脚本会把它拼到这个根目录下去找文件。
# 等你从同学那拿到 PDF 存放位置后，把这里改成那个目录即可。
PDF_BASE_DIR = os.getenv("PDF_BASE_DIR", "")
