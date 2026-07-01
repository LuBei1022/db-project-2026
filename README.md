# 文献管理系统（数据库课程设计）

一个面向学术文献管理场景的数据库应用系统。以**规范化的关系数据库设计**为核心，
覆盖文献上传、审核、检索、下载、收藏、积分、通知等全生命周期业务；并在此基础上
扩展两项创新功能——基于 RAG 的语义检索与智能问答、基于知识图谱的文献关系可视化。


---

## 技术栈

| 层 | 技术 |
|---|---|
| 数据库 | **Microsoft SQL Server**（库名 `manage_db_final`）|
| 后端 | C# / .NET Framework，Model / BLL / DAL 三层架构 |
| 前端 | ASP.NET WebForms + JavaScript |
| RAG 扩展 | Python + Flask + LangChain + Chroma 向量库 + m3e 嵌入 + DeepSeek |
| 知识图谱扩展 | `Web/Inc/LiteratureGraph.ashx` + `vis-network`，基于 SQL Server 关系数据生成图数据 |

---

## 数据库设计（核心）

数据库是本项目的主体。采用 SQL Server，共 **58 张表、29 个外键约束、96 个索引**，
完整建库脚本见[`database/manage_db_schema.sql`](database/manage_db_schema.sql)，
表结构文档见 [`database/TABLE_STRUCTURE.md`](database/TABLE_STRUCTURE.md)，
提供操作演示与示例（教学），详见['操作演示与示例.zip']。

### 概念模型（主要实体）

文献（Literature）、作者（Author）、机构（Institution）、期刊（Journal）、会议（Conference）、
分类（LiteratureCategory）、标签（LiteratureTag）、用户（user_list）、管理员（admin）。

### 逻辑设计（关系模式，按职责分组）

| 类别 | 代表表 | 说明 |
|---|---|---|
| 核心实体 | `Literature`、`Author`、`Institution`、`Journal`、`Conference`、`LiteratureCategory`、`LiteratureTag`、`user_list`、`admin` | 每个独立实体一张表 |
| 关联/映射（多对多） | `LiteratureAuthorMap`（文献-作者）、`LiteratureTagMap`（文献-标签）、`LiteratureAuthorInstitutionMap`（文献-作者-机构） | 多对多拆为中间表 |
| 附属信息 | `LiteratureFile`（附件）、`AuthorInstitutionHistory`、`InstitutionAlias` | 一对多 / 历史记录 |
| 用户互动 | `LiteratureComment`（评论）、`LiteratureFavorite`（收藏）、`LiteratureLike`（点赞） | 记录用户与文献之间的互动 |
| 日志/流水 | `LiteratureDownloadLog`（下载）、`LiteratureExportLog`（导出）、`integrateLog_list`（积分流水）、`NoticeLog_List`（通知）、`LiteratureImportBatch` / `LiteratureImportError`（批量导入） | 过程型业务留痕 |
| 积分/支付 | `integrate_list`、`TopUpType_List`、`userpaylog_list`、`userpayloginfo_list` | 积分、充值与支付记录 |
| 系统/配置 | `popedom`（权限）、`tbl_class`（栏目）、`websiteinfo_list`（站点配置）、`SearchHot_List` 等 | 系统权限、栏目和基础配置 |

### 实体间联系

- **一对多**：用户 1—N 文献（`Literature.userid → user_list.id`）；分类 1—N 文献
  （`Literature.category_id → LiteratureCategory.id`）；文献 1—N 附件
  （`LiteratureFile.literature_id → Literature.id`）。
- **多对多**：文献 ↔ 作者（经 `LiteratureAuthorMap`）、文献 ↔ 标签（经 `LiteratureTagMap`），
  并用 `LiteratureAuthorInstitutionMap` 表达“文献-作者-机构”三元关系。
- **自引用**：机构层级 `Institution.parent_id → Institution.id`；分类树
  `LiteratureCategory.parent_id → LiteratureCategory.id`。


### 完整性约束与规范化

- **主键**：各表均设置主键，多数核心业务表以自增标识列（IDENTITY）作为主键。
- **外键**：29 个外键维护引用完整性（见 `database/foreign_keys.csv`）。
- **唯一约束**：如用户手机号 `user_list.tel`、规范化期刊/会议/机构名称、点赞收藏映射等。
- **默认值**：时间戳 `getdate()`、状态位 `status` 默认 `1` 等。
- **软删除/状态机**：用 `status` / `is_deleted` / 审核状态字段标记关键业务状态，保证可追溯。
- **索引**：索引覆盖主键、外键及高频检索字段（标题、作者、关键词、年份等）。
- **范式**：关系模式满足 **3NF**——独立实体单独建表、多对多拆中间表、消除传递依赖与冗余。

---

## 代码结构

```
db-project-2026/
├─ 网站源码/                 主网站（ASP.NET 解决方案 web.sln）
│  ├─ Web/                   前台页面 + 接口（index / LiteratureSearch / LiteratureInfo /
│  │                          LiteratureVenue / LiteratureQA(智能问答) / admin 后台 / User 用户中心）
│  │  └─ Inc/                RagApi.ashx(RAG 中转)、LiteratureGraph.ashx(知识图谱数据)
│  ├─ Model / BLL / DAL      实体 / 业务逻辑 / 数据访问 三层
│  ├─ LiteratureManager.Common  公共库
│  └─ database/              建库脚本、表结构文档、外键/索引清单、数据库备份(.bak)
│
├─ RAG/                      RAG 智能问答后端（Python 微服务）
│  ├─ app.py                 Flask 接口服务（5051 端口）
│  ├─ rag_utils.py           切块 / 向量化 / 检索 / 问答
│  ├─ llm_utils.py           DeepSeek 调用封装
│  ├─ db_utils.py            连 SQL Server 读 Literature / LiteratureFile
│  ├─ pdf_parser.py          PDF 全文提取（OCR 兜底、单词粘连修复）
│  ├─ backfill_index.py      批量回填建索引；config.py 配置；test_*/demo_* 自测与演示
│  └─ chroma_db/             向量库（生成物）
│
├─ graph1.py                 知识图谱构建脚本（→ Neo4j）
└─ 表结构ER (1).drawio        数据库 ER 图
```

---

## 主要功能

文献全生命周期管理（核心业务）：用户注册登录、文献投稿上传（PDF + 元数据）、管理员审核、
多条件检索（标题/作者/关键词/DOI/分类/年份）、PDF 自动解析元数据、积分下载、收藏、
分类与标签管理、批量导入导出、通知消息、操作日志留痕。

两项创新扩展：

1. **RAG 语义检索与智能问答**：论文全文切块向量化存入 Chroma，用户在「智能问答」页面
   搜论文并自然语言提问，系统语义检索原文段落后交 DeepSeek 生成回答并附原文依据。
   每段文本绑定的 `paper_id` 即 `Literature.id`，与数据库一一对应。
   链路：`浏览器 → LiteratureQA.aspx → Inc/RagApi.ashx(服务端中转) → Python /rag/ask → SQL Server + Chroma + DeepSeek`。
2. **知识图谱可视化**：提取「文献-作者-机构-关键词」关系，`Inc/LiteratureGraph.ashx` 输出图数据、
   前端渲染；`graph1.py` 可导入 Neo4j 做深度图查询。

---

## 运行方式

### 数据库

在 SQL Server 中还原 `database/manage_db_full.bak`，或执行
`database/manage_db_schema.sql` 建库（库名 `manage_db_final`）。

### 主网站

Visual Studio 打开 `网站源码/web.sln`，在 `Web/Web.config` 配置 `SQLCONNECTIONSTRING`，
重新生成并运行（正式部署用 IIS，详见 [`网站源码/README.md`](网站源码/README.md)）。

上传/批量导入会调用 PDF 元数据解析服务（`网站源码/app.py`，端口 5050）：

```bash
cd 网站源码 && python app.py
```

> 系统有两个独立 Python 服务：`网站源码/app.py`(5050) 解析上传文献的元数据；
> `RAG/app.py`(5051) 提供智能问答。下面的 RAG 服务即指后者。

### RAG 服务（智能问答依赖）

```bash
cd RAG
pip install -r requirements.txt          # 首次会下载 m3e 模型
cp .env.example .env                      # 填 DeepSeek Key、SQL Server 连接、PDF 目录
python backfill_index.py                  # 建索引（重建用 --rebuild）
python app.py                             # 启动问答服务（5051 端口）
```

`Web.config` 的 `rag_service_url` 需与 RAG 服务端口一致；两者通常同机经 localhost 互通。
详见 [`RAG/RAG_README.md`](RAG/RAG_README.md)。

---

## 说明

- `.env`（含 API Key）与 `chroma_db/` 等生成物不纳入版本管理。
- RAG 服务与网站共用同一套 SQL Server；PDF 全文索引需 PDF 可读，缺失时自动降级为「标题+摘要」索引。
