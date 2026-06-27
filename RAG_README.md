# RAG 智能问答模块说明

文献管理系统的「语义检索 + 智能问答」功能。独立的 Python 微服务，
ASP.NET 网站只需通过 HTTP 调它的接口即可接入。

## 它做什么

把论文文本切块、用 m3e 模型向量化存进 Chroma 向量库（**检索 R**）；
用户提问时，先检索出最相关的几段原文，再交给 DeepSeek 生成回答（**生成 G**）。
向量库里每段文本绑定的 `paper_id` 就是 SQL Server `Literature.id`，和网站完全对应。

## 文件结构

| 文件 | 作用 |
|---|---|
| `config.py` | 统一配置，全部从 `.env` 读 |
| `llm_utils.py` | DeepSeek 调用封装（生成 G） |
| `db_utils.py` | 连 SQL Server，读 `Literature` / `LiteratureFile` |
| `rag_utils.py` | 切块 / 向量化 / 检索 / `answer_question` 问答主入口 |
| `app.py` | Flask 接口服务（端口 5050） |
| `backfill_index.py` | 一次性回填脚本：把已有论文灌进向量库 |
| `pdf_parser.py` | PDF 全文提取（已有，回填全文时用） |

## 怎么跑起来

```bash
# 1. 装依赖
pip install -r requirements.txt

# 2. 配置：复制 .env.example 为 .env，填入 DeepSeek API Key 和数据库信息
cp .env.example .env

# 3. 建索引（把数据库里的论文灌进向量库）
python backfill_index.py
#    重复运行时用：python backfill_index.py --rebuild

# 4. 启动问答服务
python app.py
```

服务起在 `http://localhost:5050`。

## 接口

| 方法 | 路径 | 说明 |
|---|---|---|
| GET | `/health` | 健康检查 |
| GET | `/rag/search_paper?title=xxx` | 按标题搜论文，返回含 `id` 的列表（供选论文） |
| POST | `/rag/ask` | 问答：body `{ "paper_id": 12, "question": "本文方法是什么?" }` → `{answer, sources}` |
| GET | `/rag/search?q=xxx&paper_id=12` | 纯检索，只返回相关原文段落，不生成回答 |

`/rag/ask` 不传 `paper_id` 就是跨全部论文的全局问答；传了就是单篇问答。

## 关于 PDF（重要）

如果本机暂时没有 PDF 文件，回填脚本会**自动降级**，用每篇论文的
「标题 + 摘要 + 关键词」建索引——问答照样能用，只是答得没全文那么细。

等从负责文件的同学那拿到 PDF 存放目录后：
1. 在 `.env` 里把 `PDF_BASE_DIR` 设成那个目录；
2. 重新跑 `python backfill_index.py --rebuild`，即可升级到「全文问答」。

（需要问同学的就一句话：**PDF 文件存在哪个目录，`LiteratureFile.file_path` 里存的是绝对路径还是相对路径**。）

## 给队友（接前端）的提示

ASP.NET 端建议照搬知识图谱的做法：加一个 `Inc/RagAsk.ashx` 中转接口，
浏览器把问题发给它，它再用 `HttpClient` 转发到 `http://localhost:5050/rag/ask`，
拿到 JSON 回传给页面。首页导航 `top.ascx` 加一项「智能问答」链到新页面即可。
（本次只交付 Python 后端，前端集成留到后面。）
