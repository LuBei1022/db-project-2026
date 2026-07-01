# 学术文献管理系统 README

本目录是学术文献管理系统网站源码。系统主体为 ASP.NET Web Forms + C# + SQL Server，面向学术文献的上传、检索、审核、下载、评论互动、作者机构维护、积分权益兑换和后台管理；同时在 Web 端接入了 PDF 自动解析、文献关系图谱、RAG 语义检索与智能问答入口。

当前网站包含：首页、文献检索、智能问答、文献/期刊汇总、学术资讯、文献投稿、消息通知、积分充值/权益兑换和用户中心。后台包含文献、分类、标签、作者、机构、期刊、会议、评论审核、导入批次、用户积分、内容配置、反馈工单、日志和管理员权限等管理页面。

## 目录结构

| 路径 | 说明 |
|---|---|
| `web.sln` | Visual Studio 解决方案入口。 |
| `Web/` | ASP.NET Web Forms 网站目录，IIS 站点物理路径应指向这里。 |
| `Web/admin/` | 管理后台页面，入口为 `/admin/Login.aspx`。 |
| `Web/UserCenter/` | 用户中心页面，包含账号、投稿、积分、通知、反馈等功能。 |
| `Web/Inc/` | 前台接口，包含 `UserCommon.ashx`、`RagApi.ashx`、`LiteratureGraph.ashx` 等。 |
| `Model/` | 数据库实体类，对应用户、文献、作者、机构、积分、通知等表。 |
| `BLL/` | 业务逻辑层，包含通用 `BLLBase<T>`。 |
| `DAL/` | 数据访问层，包含 `DALCommon<T>`、`DBHelper`。 |
| `LiteratureManager.Common/` | 登录、短信、积分、评论、上传策略、PDF 并发控制、文献关系同步、RAG 索引触发、微信支付等公共业务逻辑。 |
| `Common/` | 通用日志等基础工具类。 |
| `COSSTS/` | 腾讯云 COS STS 相关代码。 |
| `database/` | 数据库备份、完整结构 SQL、字段/索引/外键清单和表结构说明。 |
| `docs/` | 项目说明、测试清单、依赖说明和详细项目文档。 |
| `app.py` | 本机 PDF 元数据解析 HTTP 服务，默认监听 `127.0.0.1:5050`。 |
| `app_llm_pdf.py` | LLM 辅助 PDF 元数据解析服务，可替代 `app.py` 使用同一 `/upload` 接口。 |
| `pdf_parser.py` | PDF 元数据抽取核心脚本。 |
| `start_pdf_llm_service.ps1` | 启动 LLM PDF 解析服务的 PowerShell 脚本。 |
| `requirements.txt` | Python PDF 解析服务依赖。 |
| `uploads/` | Python 解析服务运行时上传/缓存目录。 |
| `packages/`、`bin/` | .NET 依赖包和已编译依赖。 |

## 技术环境

| 类型 | 内容 |
|---|---|
| Web 框架 | ASP.NET Web Forms |
| 后端语言 | C# |
| .NET 版本 | .NET Framework 4.6 |
| 数据库 | SQL Server / SQL Server Express |
| Web 宿主 | IIS，应用程序池使用 .NET CLR v4.0，Integrated 管道 |
| 数据访问 | `BLLBase<T>` + `DALCommon<T>` + `DBHelper` |
| 前端 | jQuery、Layui、AdminLTE、zTree、KindEditor、项目内 CSS/JS |
| .NET 依赖 | Newtonsoft.Json、NPOI、AspNetPager、UrlRewritingNet、Tencent COS SDK 等 |
| Python 依赖 | Python 3.9+、Flask、Werkzeug、pdfplumber、wordninja |
| 可选/外部服务 | Redis、互亿无线短信、微信支付、腾讯云 COS、兼容 RAG 服务 |

## 实际功能

### 前台用户端

- 首页门户：展示系统入口、最新文献、热门内容和检索入口。
- 文献检索：支持关键词、学科分类、文献类型、标签、期刊/会议等维度浏览；列表展示点赞、收藏、评论、PDF 下载等信息。
- 文献详情：展示标题、作者、机构、DOI、发表时间、期刊/会议、摘要、标签和 PDF；支持评论、点赞、收藏、下载。
- 批量下载：在检索页勾选多个 PDF，可使用积分或下载权益批量下载，受 `DownloadBatchMaxFiles`、`DownloadBatchMaxTotalMb` 限制。
- 智能问答：`/LiteratureQA.aspx` 通过 `/Inc/RagApi.ashx` 调用外部 RAG 服务完成文献语义检索和问答。
- 文献/期刊汇总：`/LiteratureVenue.aspx` 汇总展示文献、期刊、会议来源。
- 学术资讯与单页内容：通过 `WebsiteData/News.aspx`、`NewsInfo.aspx`、`Single.aspx`、`AdSingle.aspx` 展示后台配置的资讯和内容页。

### 用户中心

- 手机验证码登录/注册，登录协议弹窗展示。
- 账号信息维护：头像、昵称、手机号、邮箱和退出登录。
- 文献投稿：支持单篇 PDF 上传、手动补全文献元数据、PDF 自动解析填充、批量 PDF 解析与上传。
- 积分与权益：展示积分流水、微信充值、下载权益兑换和权益记录。
- 消息通知：查看系统通知、评论回复、点赞等消息。
- 反馈工单：提交问题反馈、查看反馈记录和详情。
- 内容反馈：可提交文字、图片或 PDF 附件反馈。

### 管理后台

- 管理员登录、权限菜单、管理员账号和密码维护。
- 文献管理：文献列表、详情、编辑、审核、上下架、分类、标签、PDF 解析填充、重复文献处理和状态日志。
- 批量导入：支持 CSV 元数据导入和多 PDF 解析导入；失败项写入 `LiteratureImportError`。
- 评论审核：后台审核、删除文献评论。
- 作者与机构：作者列表、作者详情、作者编辑、作者合并、机构维护、作者当前机构与历史机构维护。
- 期刊与会议：期刊、会议主数据维护。
- 积分体系：积分列表、积分流水、积分类型、下载权益兑换记录、充值套餐配置。
- 内容与配置：站点信息、邮箱配置、首页内容、底部链接、热门搜索、菜单配置。
- 服务记录：问题反馈、内容反馈、通知、日志、服务器信息等管理。

### 智能与关系数据

- PDF 元数据解析：后台和用户投稿页会调用 `/admin/PdfParse.ashx`，该接口优先请求本机 `http://127.0.0.1:5050/upload`，失败时会直接调用 `pdf_parser.py` 作为兜底。
- LLM PDF 解析：`app_llm_pdf.py` 使用 OpenAI 兼容 Chat Completions 接口抽取标题、作者、机构、DOI、日期、摘要、期刊/会议和作者-机构映射。
- 文献关系同步：文献保存后通过 `LiteratureRelationSync` 同步作者、机构、标签和 PDF 附件关系。
- 文献关系图谱：`/Inc/LiteratureGraph.ashx` 输出文献、作者、机构、分类、期刊/会议等节点和关系，供前端图谱视图使用。
- RAG 自动索引：已发布文献可由 `LiteratureRagSync` 调用外部 RAG 服务的 `/rag/index_paper` 触发重建索引。

## 主要页面与接口

| 路由 | 说明 |
|---|---|
| `/`、`/index.aspx` | 首页门户。 |
| `/LiteratureSearch.aspx` | 文献检索、学科浏览、批量下载。 |
| `/LiteratureInfo.aspx?id=...` | 文献详情、评论、点赞、收藏、下载。 |
| `/LiteratureQA.aspx` | RAG 语义检索与智能问答页面。 |
| `/LiteratureVenue.aspx` | 文献/期刊/会议汇总页面。 |
| `/Search.aspx` | 文献检索跳转页。 |
| `/User/Center` | 个人中心首页。 |
| `/User/LiteratureUpload` | 文献投稿、单篇/批量 PDF 上传。 |
| `/User/IntegrateLog` | 我的积分。 |
| `/User/IntegrateExchange` | 下载权益兑换。 |
| `/User/NoticeLog`、`/User/MsgLog` | 通知和评论回复。 |
| `/User/ServiceLog` | 问题反馈记录。 |
| `/admin/Login.aspx` | 后台登录。 |
| `/admin/Admin_LiteratureList.aspx` | 后台文献列表与审核。 |
| `/admin/Admin_LiteratureImport.aspx` | CSV / PDF 批量导入。 |
| `/admin/Admin_AuthorList.aspx`、`Admin_AuthorMerge.aspx` | 作者管理和作者合并。 |
| `/admin/Admin_InstitutionList.aspx` | 机构管理。 |
| `/admin/Admin_JournalList.aspx`、`Admin_ConferenceList.aspx` | 期刊和会议管理。 |
| `/admin/PdfParse.ashx` | PDF 解析接口，供前台投稿和后台编辑/导入使用。 |
| `/Inc/UserCommon.ashx` | 登录验证码、用户信息更新、充值、权益兑换、评论、点赞/收藏等 AJAX 接口。 |
| `/Inc/RagApi.ashx` | RAG 服务端中转接口。 |
| `/Inc/LiteratureGraph.ashx` | 文献关系图谱数据接口。 |
| `/LiteratureBatchDownload.ashx` | 文献 PDF 批量下载接口。 |
| `/wx_pay_notify.aspx` | 微信支付回调入口。 |

## 数据库部署

推荐部署到独立数据库，例如：

```text
manage_db_final
```

方式一：还原备份。

1. 在 SQL Server 中创建空数据库 `manage_db_final`。
2. 将 `database/manage_db_full.bak` 还原到该数据库。
3. 如果还原时提示数据文件或日志文件被占用，在 SSMS 的还原窗口进入 `Files / 文件` 页面，勾选重新定位文件，并改成独立文件名，例如：

```text
manage_db_final.mdf
manage_db_final_log.ldf
```

方式二：执行结构脚本。

1. 在 SQL Server 中创建空数据库 `manage_db_final`。
2. 执行 `database/manage_db_schema.sql` 创建完整表、约束、索引、外键、存储过程和触发器。
3. 根据需要导入初始化数据，或使用备份中的数据。

数据库目录文件说明：

| 文件 | 用途 |
|---|---|
| `database/manage_db_full.bak` | 当前交付版数据库完整备份。 |
| `database/manage_db_schema.sql` | 当前交付版完整建库脚本。 |
| `database/TABLE_STRUCTURE.md` | 表结构说明。 |
| `database/table_columns.csv` | 字段清单。 |
| `database/indexes.csv` | 索引清单。 |
| `database/foreign_keys.csv` | 外键清单。 |

## 核心数据表

| 模块 | 数据表 |
|---|---|
| 用户与登录 | `user_list`、`user_login`、`telcode_list`、`logincode_list`、`admin`、`popedom`、`popedomhead`、`LoginSingle_List` |
| 文献主体 | `Literature`、`LiteratureCategory`、`LiteratureFile`、`LiteratureTag`、`LiteratureTagMap`、`LiteratureStatusLog` |
| 作者与机构 | `Author`、`Institution`、`InstitutionAlias`、`AuthorInstitutionHistory`、`LiteratureAuthorMap`、`LiteratureAuthorInstitutionMap` |
| 期刊会议 | `Journal`、`Conference` |
| 互动与下载 | `LiteratureComment`、`LiteratureLike`、`LiteratureFavorite`、`LiteratureDownloadLog`、`LiteratureExportLog` |
| 批量导入 | `LiteratureImportBatch`、`LiteratureImportError` |
| 积分与权益 | `integrate_list`、`integrateLog_list`、`integrateLogType_list`、`integrateExchangeLog_list`、`integratestatus_list`、`TopUpType_List`、`userpaylog_list`、`userpayloginfo_list` |
| 通知、反馈与工单 | `NoticeLog_List`、`NoticeLogStatus_List`、`NoticeLogType_List`、`ServiceLog_List`、`ServiceLogInfo_List`、`ServiceLogStatus_List`、`appeal_list`、`appealimg_list` |
| 内容配置与文件 | `websiteinfo_list`、`indexsingle_list`、`data_list`、`link_list`、`SearchHot_List`、`tbl_class`、`cosfile_list`、`userfile_list`、`userimg_list` |
| 历史导入兼容 | `daoru_list`、`daoruerr_list` |

## Web.config 关键配置

配置文件位于 `Web/Web.config`。

数据库连接：

```xml
<connectionStrings>
  <add name="SQLCONNECTIONSTRING"
       connectionString="data source=(local)\SQLEXPRESS; Initial Catalog=manage_db_final;User ID=sa;Password=123456;"
       providerName="System.Data.SqlClient"/>
</connectionStrings>
```

部署时，需要替换为实际 SQL Server 地址、数据库名、账号和密码。

常用站点与服务配置：

| Key | 当前用途 |
|---|---|
| `website_url` | 站点访问地址，默认 `http://localhost:8081/`。 |
| `UploadMaxPdfMb`、`UploadMaxImageMb`、`UploadMaxAttachmentMb` | 单个 PDF、图片、附件上传大小限制。 |
| `UploadBatchMaxFiles`、`UploadBatchMaxTotalMb` | 批量 PDF 上传数量和总大小限制。 |
| `DownloadBatchMaxFiles`、`DownloadBatchMaxTotalMb` | 批量下载数量和总大小限制。 |
| `ImportMaxMb`、`ImportMaxRows` | CSV 导入大小和行数限制。 |
| `CommentMaxLength`、`CommentCooldownSeconds` | 评论长度和冷却时间。 |
| `SmsPostUrl`、`SmsAccount`、`SmsPassword`、`SmsProxyUrl`、`SmsDebugMode` | 手机验证码短信服务配置。 |
| `RedisEnabled`、`RedisHost`、`RedisPort`、`RedisDatabase`、`RedisKeyPrefix` | Redis 限流与 PDF 解析并发控制配置。 |
| `PdfParseMaxConcurrent`、`PdfParseLeaseSeconds` | PDF 解析并发数量和租约时间。 |
| `rag_service_url`、`rag_auto_index_enabled` | RAG 服务地址和发布后自动索引开关。 |
| `file_storage_base_url`、`qcloud_*` | 腾讯云 COS 对象存储配置。 |
| `wxpay_*`、`wx_notify_url` | 微信支付和回调配置。 |

## PDF 解析服务

网站中的单篇投稿、后台文献编辑和批量 PDF 导入都会调用：

```text
POST http://127.0.0.1:5050/upload
```

解析服务启动：

```powershell
cd PATH_TO_PJ
python app.py
```

如需使用指定 Python：

```powershell
PATH\python.exe app.py
```


## RAG 智能问答服务

当前交付目录包含 Web 端问答页面、中转接口和自动索引触发逻辑，但不包含独立的 `RAG/` 后端服务目录。启用智能问答时，需要另行部署一个兼容以下接口的服务，并在 `Web/Web.config` 中配置：

```xml
<add key="rag_service_url" value="http://127.0.0.1:5051"/>
<add key="rag_auto_index_enabled" value="true"/>
```

兼容接口：

```text
GET  /rag/search_paper?title=关键词
POST /rag/ask
POST /rag/index_paper
```

调用关系：

```text
浏览器 -> /LiteratureQA.aspx -> /Inc/RagApi.ashx -> rag_service_url
文献审核/发布 -> LiteratureRagSync -> rag_service_url/rag/index_paper
```


## IIS 部署

1. 打开 IIS 管理器。
2. 新建应用程序池，.NET CLR 选择 v4.0，托管管道选择 Integrated。
3. 新建网站，物理路径指向 `PATH_TO_PJ`。
4. 绑定未被占用的端口，例如 `8081`。
5. 给 `Web/`、`Web/A_UpLoad/`、`uploads/` 和 ASP.NET 临时编译目录授予应用程序池身份的读取/写入权限。
6. 确认 SQL Server 连接、Redis、PDF 解析服务和可选 RAG 服务可访问。
7. 浏览器访问 `http://localhost:8081/` 验证站点首页。

## 运行检查

- 首页 `/` 可以打开，顶部导航与搜索框显示正常。
- 普通用户可以用手机验证码登录/注册。
- `/User/LiteratureUpload` 可以手动投稿、单篇 PDF 解析、批量 PDF 解析上传。
- `/LiteratureSearch.aspx` 可以检索、按分类浏览、查看详情、点赞、收藏、评论、下载和批量下载。
- `/LiteratureVenue.aspx` 可以查看文献、期刊、会议汇总。
- 后台 `/admin/Login.aspx` 可以登录，文献审核、作者机构、期刊会议、评论审核、导入批次、积分权益和内容配置可访问。
- PDF 解析服务监听 `127.0.0.1:5050`，`/admin/PdfParse.ashx` 能返回文献元数据。
- 如启用 RAG，`rag_service_url` 监听 `127.0.0.1:5051`，`/LiteratureQA.aspx` 可以检索文献并提问。
- Redis 按 `Web.config` 配置运行；如关闭 Redis，需要确认 PDF 解析并发控制仍能按本机锁工作。


