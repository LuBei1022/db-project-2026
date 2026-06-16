# 学术文献管理系统说明

本项目为学术文献管理系统源码交付包，目录为 `数据库pj完整源码_最终交付版`。系统主体采用 ASP.NET Web Forms + C# + SQL Server，支持文献检索、文献详情、PDF 上传与下载、评论审核、点赞收藏、用户中心、积分充值兑换、作者机构管理和后台管理等功能。

## 目录结构

| 路径 | 说明 |
|---|---|
| `web.sln` | Visual Studio 解决方案入口。 |
| `Web/` | ASP.NET Web Forms 网站目录，IIS 站点物理路径应指向此目录。 |
| `Model/` | 数据库实体类。 |
| `BLL/` | 业务逻辑层，包含通用 `BLLBase<T>`。 |
| `DAL/` | 数据访问层，包含 `DALCommon<T>`、`DBHelper`。 |
| `LiteratureManager.Common/` | 用户登录、短信、积分、评论、上传解析等公共业务逻辑。 |
| `Common/` | 通用工具类。 |
| `COSSTS/` | 腾讯云 COS STS 相关代码。 |
| `database/` | 最终数据库备份、完整结构脚本、字段清单、索引清单、外键清单。 |
| `sql/` | 最终数据库结构脚本入口。 |
| `docs/` | 项目说明、测试清单和依赖说明文档。 |
| `app.py` | 本机 PDF 解析服务入口，默认监听 `127.0.0.1:5050`。 |
| `pdf_parser.py` | PDF 元数据解析脚本。 |
| `requirements.txt` | Python 依赖列表。 |

## 技术环境

| 类型 | 内容 |
|---|---|
| Web 框架 | ASP.NET Web Forms |
| 后端语言 | C# |
| .NET 版本 | .NET Framework 4.6 及以上 |
| 数据库 | SQL Server / SQL Server Express |
| 数据访问 | `BLLBase<T>` + `DALCommon<T>` + `DBHelper` |
| PDF 解析 | Python 3.9+，本机 HTTP 服务 |
| 可选组件 | Redis、腾讯云 COS、微信支付、短信服务 |
| 前端依赖 | jQuery、Layui、项目内 CSS/JS |

## 当前功能

- 用户注册、登录、退出。
- 短信验证码发送与校验。
- 用户中心。
- 文献检索、分类浏览、文献详情。
- 单篇 PDF 上传。
- 批量 CSV 元数据导入。
- 批量 PDF 自动解析导入。
- PDF 附件保存和下载。
- 文献点赞、收藏。
- 文献评论提交、展示、删除、后台审核。
- 文献分类、标签和附件维护。
- 作者信息、作者当前机构、历史机构和论文机构关系维护。
- 机构、期刊、会议资料维护。
- 积分流水、充值、兑换。
- 后台管理员登录。
- 后台文献审核。
- 后台日志和通知管理。

## 数据库部署

推荐将当前交付版部署到独立数据库，例如：

```text
manage_db_final
```

部署方式一：还原最终备份。

1. 在 SQL Server 中创建空数据库 `manage_db_final`。
2. 将 `database/manage_db_full.bak` 还原到 `manage_db_final`。
3. 如果还原时提示数据文件或日志文件被其他数据库占用，需要在 SSMS 的还原窗口进入 `Files / 文件` 页面，勾选重新定位文件，并将数据文件、日志文件改成独立文件名，例如：

```text
manage_db_final.mdf
manage_db_final_log.ldf
```

4. 还原后刷新数据库表，确认存在 `Literature`、`LiteratureComment`、`Author`、`Institution`、`Journal`、`Conference`、`LiteratureAuthorMap`、`LiteratureAuthorInstitutionMap` 等核心表。

部署方式二：从空数据库执行完整结构脚本。

1. 在 SQL Server 中创建空数据库 `manage_db_final`。
2. 在该数据库中执行 `database/manage_db_schema.sql` 创建当前版本完整表结构。
3. 根据需要导入初始化数据或使用 `database/manage_db_full.bak` 中的数据。

## SQL 与数据库文件说明

| 文件 | 用途 | 是否部署需要 |
|---|---|---|
| `database/manage_db_full.bak` | 当前版本数据库备份 | 是 |
| `database/manage_db_schema.sql` | 当前版本完整建表脚本 | 按部署方式需要 |
| `database/table_columns.csv` | 数据库字段清单 | 参考 |
| `database/indexes.csv` | 索引清单 | 参考 |
| `database/foreign_keys.csv` | 外键清单 | 参考 |
| `sql/final_schema.sql` | 指向当前完整结构脚本的入口说明 | 参考 |

## Web.config 关键配置

当前数据库连接配置位于 `Web/Web.config`：

```xml
<connectionStrings>
  <add name="SQLCONNECTIONSTRING"
       connectionString="data source=(local)\SQLEXPRESS; Initial Catalog=manage_db_final;User ID=sa;Password=123456;"
       providerName="System.Data.SqlClient"/>
</connectionStrings>
```

正式部署时需要根据实际 SQL Server 地址、数据库名、账号和密码调整。

站点地址配置：

```xml
<add key="website_url" value="http://localhost:8081/"/>
```

正式部署到其他域名或端口时，将该值改为实际访问地址。

Redis 默认配置：

```xml
<add key="RedisEnabled" value="true"/>
```

Redis 按原有方式保持运行即可。重启 PDF 解析服务不需要重启 Redis。

## PDF 解析服务

后台单篇上传和批量 PDF 导入会调用本机 PDF 解析服务：

```text
http://127.0.0.1:5050/upload
```

启动方式：

```powershell
cd E:\数据库pj完整源码_最终交付版
E:\tools\python.exe app.py
```

`app.py` 内部调用 `pdf_parser.py` 的 `extract_paper_info` 完成标题、作者、机构、DOI、发表日期等元数据解析。修改 `pdf_parser.py` 后需要重启 `app.py`，让服务加载最新解析逻辑。

## IIS 部署

1. 打开 IIS 管理器。
2. 新建应用程序池，.NET CLR 选择 v4.0，托管管道选择 Integrated。
3. 新建网站，物理路径指向 `E:\数据库pj完整源码_最终交付版\Web`。
4. 绑定未被占用的端口，例如 `8081`。
5. 给网站目录和 ASP.NET 临时编译目录授予应用程序池身份的读取和写入权限。
6. 浏览器访问 `http://localhost:8081/` 验证站点。

## 核心数据表

| 模块 | 数据表 |
|---|---|
| 用户与登录 | `user_list`、`user_login`、`telcode_list`、`logincode_list`、`admin`、`popedom`、`popedomhead`、`LoginSingle_List` |
| 文献主体 | `Literature`、`LiteratureCategory`、`LiteratureFile`、`LiteratureTag`、`LiteratureTagMap`、`LiteratureVenueProfile` |
| 作者与机构 | `Author`、`Institution`、`InstitutionAlias`、`AuthorInstitutionHistory`、`LiteratureAuthorMap`、`LiteratureAuthorInstitutionMap` |
| 期刊会议 | `Journal`、`Conference` |
| 互动功能 | `LiteratureComment`、`LiteratureLike`、`LiteratureFavorite`、`LiteratureDownloadLog`、`LiteratureExportLog` |
| 批量导入 | `LiteratureImportBatch`、`LiteratureImportError` |
| 积分充值 | `integrate_list`、`integrateLog_list`、`integrateLogType_list`、`integrateExchangeLog_list`、`integratestatus_list`、`IntegrationToken_list`、`TopUpType_List`、`userpaylog_list`、`userpayloginfo_list` |
| 通知、反馈与服务记录 | `NoticeLog_List`、`NoticeLogStatus_List`、`NoticeLogType_List`、`ServiceLog_List`、`ServiceLogInfo_List`、`ServiceLogStatus_List`、`appeal_list`、`appealimg_list` |
| 内容配置与文件 | `websiteinfo_list`、`indexsingle_list`、`data_list`、`link_list`、`SearchHot_List`、`tbl_class`、`model_list`、`cosfile_list`、`userfile_list`、`userimg_list` |
| 导入与基础数据 | `daoru_list`、`daoruerr_list` |

## 运行检查

- 网站首页可以打开。
- 普通用户可以登录、上传文献、查看详情、下载 PDF。
- 后台管理员可以登录、审核文献、维护作者/机构/期刊/会议、审核评论。
- 批量 PDF 导入可以先解析预览，再由管理员调整后导入。
- PDF 解析服务监听 `127.0.0.1:5050`。
- Redis 按配置保持运行。


