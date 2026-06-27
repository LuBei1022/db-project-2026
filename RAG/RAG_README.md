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

> 说明：所有 Python 后端文件都在仓库的 `RAG/` 子目录里，下面命令请先 `cd RAG` 再执行。
> 前端文件（ASP.NET 页面/接口）在 `网站源码/Web/` 里，见文末「前端」一节。

## 怎么跑起来

```bash
# 0. 进入 RAG 目录
cd RAG

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

## 关于 PDF

如果本机暂时没有 PDF 文件，回填脚本会**自动降级**，用每篇论文的
「标题 + 摘要 + 关键词」建索引——问答照样能用，只是答得没全文那么细。


1. 在 `.env` 里把 `PDF_BASE_DIR` 设成那个目录；
2. 重新跑 `python backfill_index.py --rebuild`，即可升级到「全文问答」。

（需要问同学的就一句话：**PDF 文件存在哪个目录，`LiteratureFile.file_path` 里存的是绝对路径还是相对路径**。）

## 前端（ASP.NET 页面，已完成）

前端已按网站原有风格写好，可直接合并进网站项目。新增/改动的文件：

| 文件 | 作用 |
|---|---|
| `网站源码/Web/LiteratureQA.aspx` (+ `.cs` / `.designer.cs`) | 智能问答页面：搜论文 → 选中 → 提问 → 显示回答和原文依据 |
| `网站源码/Web/Inc/RagApi.ashx` (+ `.cs`) | 中转接口，服务端转发到 Python RAG 服务（同源，无跨域问题） |
| `网站源码/Web/top.ascx` | 导航栏新增「智能问答」入口 |
| `网站源码/Web/Web.config` | 新增 `rag_service_url` 配置（Python 服务地址） |
| `网站源码/Web/Web.csproj` | 登记以上新文件 |

数据流：浏览器 → `LiteratureQA.aspx`(页面) → `Inc/RagApi.ashx`(C#中转) → Python `app.py` → 返回。
因为是服务端中转，不存在跨域(CORS)问题。

### 前端怎么用

1. 用 Visual Studio 打开网站项目，**重新生成(Rebuild)** 一次（让新增的 .cs 编译进 Web.dll）。
2. 确保 Python RAG 服务在跑：`python app.py`（默认 5050 端口）。
3. **端口要对齐**：`Web.config` 里 `rag_service_url` 的端口要和 `app.py` 实际监听的端口一致。
   - 若 `app.py` 跑在 5050，保持默认即可；
   - 若改成别的端口（如 5051），就把 `Web.config` 里的 `rag_service_url` 一起改成 `http://localhost:5051`。
4. 启动网站，点导航栏「智能问答」即可使用。

> 前提：网站和 Python 服务跑在同一台机器（用 localhost 互通）。若分开部署，把
> `rag_service_url` 改成 Python 服务所在机器的地址即可。
