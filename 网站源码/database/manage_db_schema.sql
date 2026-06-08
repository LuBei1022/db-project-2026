IF DB_ID(N'manage_db') IS NULL CREATE DATABASE [manage_db];
GO
USE [manage_db];
GO
IF OBJECT_ID(N'dbo.admin', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[admin] (
    [id] int IDENTITY(1,1) NOT NULL,
    [username] nvarchar(50) NOT NULL,
    [password] nvarchar(50) NULL,
    [popedom] nvarchar(MAX) NULL,
    [lastloginip] nvarchar(50) NULL,
    [cityid] int NULL,
    [locks] int NULL,
    [code] nvarchar(50) NULL,
    [lastlogindate] datetime NULL DEFAULT (getdate()),
    CONSTRAINT [PK_Admin] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.appeal_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[appeal_list] (
    [id] bigint IDENTITY(1,1) NOT NULL,
    [url] nvarchar(2500) NOT NULL,
    [info_] nvarchar(MAX) NOT NULL,
    [addtime] datetime NOT NULL,
    [status] int NOT NULL,
    [userid] int NOT NULL,
    CONSTRAINT [PK_appeal_list] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.appealimg_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[appealimg_list] (
    [appeal_id] bigint NOT NULL,
    [upload_pic_info] nvarchar(250) NOT NULL,
    [addtime] datetime NOT NULL,
    [orderid] int NOT NULL
);
END
GO

IF OBJECT_ID(N'dbo.Author', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Author] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name_cn] nvarchar(100) NOT NULL,
    [name_en] nvarchar(200) NULL,
    [institution] nvarchar(300) NULL,
    [orcid] nvarchar(50) NULL,
    [email] nvarchar(200) NULL,
    [status] int NOT NULL DEFAULT ((1)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    CONSTRAINT [PK_Author] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.cosfile_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[cosfile_list] (
    [userid] int NOT NULL,
    [addtime] datetime NOT NULL,
    [up_filename] nvarchar(250) NOT NULL
);
END
GO

IF OBJECT_ID(N'dbo.daoru_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[daoru_list] (
    [id] int IDENTITY(1,1) NOT NULL,
    [posttime] datetime NOT NULL,
    [r_info] nvarchar(500) NOT NULL,
    [status] int NOT NULL,
    [type] int NOT NULL
);
END
GO

IF OBJECT_ID(N'dbo.daoruerr_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[daoruerr_list] (
    [info] nvarchar(500) NOT NULL,
    [filename] nvarchar(250) NOT NULL,
    [addtime] datetime NOT NULL,
    [daoruid] int NOT NULL
);
END
GO

IF OBJECT_ID(N'dbo.data_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[data_list] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(600) NULL,
    [tbclass_id] int NOT NULL DEFAULT ((0)),
    [upload_pic_img] nvarchar(50) NULL,
    [isshow] int NOT NULL DEFAULT ((0)),
    [addtime] datetime NOT NULL,
    [uptime] datetime NOT NULL,
    [datetime] datetime NOT NULL,
    [orderid] int NOT NULL,
    [info_] nvarchar(MAX) NULL,
    [keywords] nvarchar(MAX) NULL,
    [description] nvarchar(MAX) NULL,
    [title] nvarchar(MAX) NULL,
    [istop] int NOT NULL,
    CONSTRAINT [PK_data_list] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.GeneratedAssetRecord_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[GeneratedAssetRecord_list] (
    [id] bigint IDENTITY(1,1) NOT NULL,
    [userid] int NOT NULL,
    [addtime] datetime NOT NULL,
    [jobid] nvarchar(250) NOT NULL,
    [requestid] nvarchar(250) NOT NULL,
    [status] nvarchar(50) NULL,
    [ai_key] nvarchar(500) NULL,
    [ai_img] nvarchar(500) NULL,
    [type] int NOT NULL,
    [upload_pic_cover] nvarchar(250) NULL,
    [iscos] int NOT NULL,
    [upload_pic_cos] nvarchar(250) NULL,
    [isshow] int NULL,
    [istop] int NULL,
    [num_integrate] int NULL,
    [api_err] nvarchar(MAX) NULL,
    [cos_name] nvarchar(250) NULL,
    CONSTRAINT [PK_userai3dlog_list] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.indexsingle_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[indexsingle_list] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(600) NULL,
    [upload_pic_img] nvarchar(50) NULL,
    [upload_pic_m] nvarchar(250) NULL,
    [upload_pic_pc] nvarchar(250) NULL,
    [isshow] int NOT NULL DEFAULT ((0)),
    [addtime] datetime NOT NULL,
    [uptime] datetime NOT NULL,
    [orderid] int NOT NULL,
    [info_] nvarchar(MAX) NULL,
    [keywords] nvarchar(MAX) NULL,
    [description] nvarchar(MAX) NULL,
    [title] nvarchar(MAX) NULL,
    [istop] int NOT NULL,
    [istype] int NOT NULL,
    [url] nvarchar(2500) NULL,
    CONSTRAINT [PK_indexsingle_list] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.integrate_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[integrate_list] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(250) NOT NULL,
    [orderid] int NOT NULL,
    [uptime] datetime NOT NULL,
    [addtime] datetime NOT NULL,
    [upload_pic_img] nvarchar(250) NULL,
    [about_] nvarchar(MAX) NULL,
    [num_integrate] int NOT NULL,
    CONSTRAINT [PK_integrate_list] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.integrateExchangeLog_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[integrateExchangeLog_list] (
    [id] bigint IDENTITY(1,1) NOT NULL,
    [name] nvarchar(500) NOT NULL,
    [num_integrate] int NOT NULL,
    [codestr] nvarchar(250) NOT NULL,
    [addtime] datetime NOT NULL,
    [status] int NULL,
    [user_id] int NOT NULL,
    [upload_pic_img] nvarchar(250) NULL,
    [hexiaotime] nvarchar(250) NULL,
    CONSTRAINT [PK_integrateExchangeLog_list] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.integrateLog_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[integrateLog_list] (
    [id] int IDENTITY(1,1) NOT NULL,
    [num_integrate] int NOT NULL,
    [type] int NOT NULL,
    [name] nvarchar(500) NOT NULL,
    [info_] nvarchar(MAX) NULL,
    [addtime] datetime NOT NULL,
    [user_id] int NOT NULL,
    [adminname] nvarchar(250) NULL,
    [pro_id] bigint NULL,
    [prodata_id] bigint NULL,
    [orderpro_orderno] nvarchar(50) NULL,
    [literature_id] int NULL,
    CONSTRAINT [PK_integrateLog_list] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.integrateLogType_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[integrateLogType_list] (
    [id] int NOT NULL,
    [name] nvarchar(50) NOT NULL,
    [num_integrate] int NULL,
    CONSTRAINT [PK_integrateLogType_list] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.integratestatus_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[integratestatus_list] (
    [id] int NOT NULL,
    [name] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_integratestatus_list] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.IntegrationToken_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[IntegrationToken_list] (
    [id] int NOT NULL,
    [adddate] datetime NOT NULL,
    [access_token] nvarchar(250) NOT NULL,
    [expires_in] int NOT NULL
);
END
GO

IF OBJECT_ID(N'dbo.link_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[link_list] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(150) NULL,
    [isshow] int NOT NULL DEFAULT ((0)),
    [addtime] datetime NOT NULL,
    [uptime] datetime NOT NULL,
    [orderid] int NOT NULL,
    [url] nvarchar(600) NULL,
    [type] int NOT NULL,
    [upload_pic_icon] nvarchar(50) NULL
);
END
GO

IF OBJECT_ID(N'dbo.Literature', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Literature] (
    [id] int IDENTITY(1,1) NOT NULL,
    [title] nvarchar(500) NOT NULL,
    [subtitle] nvarchar(500) NULL,
    [doi] nvarchar(100) NULL,
    [keywords] nvarchar(500) NULL,
    [abstract_text] nvarchar(MAX) NULL,
    [source_type] nvarchar(50) NULL,
    [language] nvarchar(50) NULL,
    [publish_year] int NULL,
    [journal_name] nvarchar(300) NULL,
    [conference_name] nvarchar(300) NULL,
    [publisher] nvarchar(300) NULL,
    [volume] nvarchar(50) NULL,
    [issue] nvarchar(50) NULL,
    [pages] nvarchar(100) NULL,
    [category_id] int NOT NULL DEFAULT ((0)),
    [cover_pic] nvarchar(255) NULL,
    [external_url] nvarchar(500) NULL,
    [source_db] nvarchar(200) NULL,
    [remark] nvarchar(1000) NULL,
    [is_top] int NOT NULL DEFAULT ((0)),
    [status] int NOT NULL DEFAULT ((1)),
    [userid] int NOT NULL DEFAULT ((0)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    [updatetime] datetime NOT NULL DEFAULT (getdate()),
    [institution] nvarchar(500) NULL,
    [download_points] int NOT NULL DEFAULT ((0)),
    [reviewed_by] int NULL,
    [review_time] datetime NULL,
    [import_batch_id] int NULL,
    [canonical_literature_id] int NULL,
    CONSTRAINT [PK_Literature] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.LiteratureAuthorMap', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[LiteratureAuthorMap] (
    [id] int IDENTITY(1,1) NOT NULL,
    [literature_id] int NOT NULL,
    [author_id] int NOT NULL,
    [author_order] int NOT NULL DEFAULT ((1)),
    [is_corresponding] int NOT NULL DEFAULT ((0)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    CONSTRAINT [PK_LiteratureAuthorMap] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.LiteratureCategory', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[LiteratureCategory] (
    [id] int IDENTITY(1,1) NOT NULL,
    [parent_id] int NULL DEFAULT ((0)),
    [name] nvarchar(100) NOT NULL,
    [name_en] nvarchar(200) NULL,
    [code] nvarchar(100) NULL,
    [orderid] int NOT NULL DEFAULT ((0)),
    [status] int NOT NULL DEFAULT ((1)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    [updatetime] datetime NOT NULL DEFAULT (getdate()),
    CONSTRAINT [PK_LiteratureCategory] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.LiteratureDownloadLog', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[LiteratureDownloadLog] (
    [id] int IDENTITY(1,1) NOT NULL,
    [literature_id] int NOT NULL,
    [user_id] int NOT NULL,
    [literature_title] nvarchar(500) NULL,
    [file_url] nvarchar(255) NULL,
    [download_points] int NOT NULL DEFAULT ((0)),
    [literature_user_id] int NOT NULL DEFAULT ((0)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    CONSTRAINT [PK_LiteratureDownloadLog] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.LiteratureExportLog', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[LiteratureExportLog] (
    [id] int IDENTITY(1,1) NOT NULL,
    [export_name] nvarchar(200) NOT NULL,
    [export_type] nvarchar(50) NOT NULL,
    [file_name] nvarchar(255) NULL,
    [record_count] int NOT NULL DEFAULT ((0)),
    [userid] int NOT NULL DEFAULT ((0)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    CONSTRAINT [PK_LiteratureExportLog] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.LiteratureFavorite', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[LiteratureFavorite] (
    [id] int IDENTITY(1,1) NOT NULL,
    [literature_id] int NOT NULL,
    [userid] int NOT NULL,
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    CONSTRAINT [PK_LiteratureFavorite] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.LiteratureFile', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[LiteratureFile] (
    [id] int IDENTITY(1,1) NOT NULL,
    [literature_id] int NOT NULL,
    [file_type] nvarchar(50) NOT NULL,
    [file_name] nvarchar(255) NOT NULL,
    [file_path] nvarchar(255) NOT NULL,
    [file_size] bigint NULL,
    [mime_type] nvarchar(100) NULL,
    [orderid] int NOT NULL DEFAULT ((0)),
    [status] int NOT NULL DEFAULT ((1)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    CONSTRAINT [PK_LiteratureFile] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.LiteratureImportBatch', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[LiteratureImportBatch] (
    [id] int IDENTITY(1,1) NOT NULL,
    [batch_name] nvarchar(200) NOT NULL,
    [import_type] nvarchar(50) NOT NULL,
    [file_name] nvarchar(255) NULL,
    [status] int NOT NULL DEFAULT ((0)),
    [total_count] int NOT NULL DEFAULT ((0)),
    [success_count] int NOT NULL DEFAULT ((0)),
    [fail_count] int NOT NULL DEFAULT ((0)),
    [userid] int NOT NULL DEFAULT ((0)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    [finishtime] datetime NULL,
    CONSTRAINT [PK_LiteratureImportBatch] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.LiteratureImportError', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[LiteratureImportError] (
    [id] int IDENTITY(1,1) NOT NULL,
    [batch_id] int NOT NULL,
    [row_no] int NOT NULL,
    [title] nvarchar(500) NULL,
    [error_msg] nvarchar(1000) NOT NULL,
    [raw_data] nvarchar(MAX) NULL,
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    CONSTRAINT [PK_LiteratureImportError] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.LiteratureLike', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[LiteratureLike] (
    [id] int IDENTITY(1,1) NOT NULL,
    [literature_id] int NOT NULL,
    [userid] int NOT NULL,
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    CONSTRAINT [PK_LiteratureLike] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.LiteratureTag', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[LiteratureTag] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(100) NOT NULL,
    [orderid] int NOT NULL DEFAULT ((0)),
    [status] int NOT NULL DEFAULT ((1)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    CONSTRAINT [PK_LiteratureTag] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.LiteratureTagMap', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[LiteratureTagMap] (
    [id] int IDENTITY(1,1) NOT NULL,
    [literature_id] int NOT NULL,
    [tag_id] int NOT NULL,
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    CONSTRAINT [PK_LiteratureTagMap] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.LiteratureVenueProfile', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[LiteratureVenueProfile] (
    [id] int IDENTITY(1,1) NOT NULL,
    [venue_type] nvarchar(30) NOT NULL,
    [venue_name] nvarchar(500) NOT NULL,
    [introduction] nvarchar(MAX) NULL,
    [impact_factor] nvarchar(100) NULL,
    [jcr_quartile] nvarchar(100) NULL,
    [issn] nvarchar(100) NULL,
    [conference_level] nvarchar(100) NULL,
    [conference_cycle] nvarchar(100) NULL,
    [location] nvarchar(250) NULL,
    [website_url] nvarchar(500) NULL,
    [publisher] nvarchar(250) NULL,
    [remark] nvarchar(MAX) NULL,
    [status] int NOT NULL DEFAULT ((0)),
    [created_by] int NOT NULL DEFAULT ((0)),
    [updated_by] int NOT NULL DEFAULT ((0)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    [updatetime] datetime NOT NULL DEFAULT (getdate()),
    CONSTRAINT [PK__Literatu__3213E83FA0EA3D73] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.logincode_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[logincode_list] (
    [code] nvarchar(50) NOT NULL,
    [val] nchar(10) NOT NULL,
    [addtime] datetime NOT NULL,
    [ip_str] nvarchar(50) NOT NULL,
    [type] int NOT NULL,
    CONSTRAINT [PK_logincode_list] PRIMARY KEY ([code])
);
END
GO

IF OBJECT_ID(N'dbo.LoginSingle_List', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[LoginSingle_List] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(250) NOT NULL,
    [IsShow] int NOT NULL,
    [OrderId] int NOT NULL,
    [UpTime] datetime NOT NULL,
    [AddTime] datetime NOT NULL,
    [Info_] nvarchar(MAX) NULL,
    CONSTRAINT [PK_LoginSingle_List] PRIMARY KEY ([Id])
);
END
GO

IF OBJECT_ID(N'dbo.model_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[model_list] (
    [id] int IDENTITY(1,1) NOT NULL,
    [m_name] nvarchar(255) NULL,
    [m_url] nvarchar(255) NULL,
    [page_url] nvarchar(255) NULL,
    [orderid] int NULL,
    [addtime] datetime NULL,
    [upload_pic] nvarchar(50) NULL,
    CONSTRAINT [PK_model_list] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.NoticeLog_List', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[NoticeLog_List] (
    [id] int IDENTITY(1,1) NOT NULL,
    [info_] nvarchar(MAX) NULL,
    [type] int NOT NULL,
    [addtime] datetime NOT NULL,
    [userid] int NOT NULL,
    [looktime] nvarchar(50) NULL,
    [status] int NOT NULL,
    [url] nvarchar(500) NULL,
    [name] nvarchar(500) NOT NULL,
    CONSTRAINT [PK_NoticeLog_List] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.NoticeLogStatus_List', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[NoticeLogStatus_List] (
    [id] int NOT NULL,
    [name] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_NoticeLogStatus_List] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.NoticeLogType_List', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[NoticeLogType_List] (
    [id] int NOT NULL,
    [name] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_NoticeLogType_List] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.OptionColor_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[OptionColor_list] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(50) NOT NULL,
    [val] nvarchar(50) NOT NULL,
    [orderid] int NOT NULL,
    [uptime] datetime NOT NULL,
    [addtime] datetime NOT NULL,
    CONSTRAINT [PK_Color_List] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.OptionMaterial_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[OptionMaterial_list] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(50) NOT NULL,
    [orderid] int NOT NULL,
    [uptime] datetime NOT NULL,
    [addtime] datetime NOT NULL,
    CONSTRAINT [PK_ConsumableMaterial_List] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.popedom', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[popedom] (
    [id] int IDENTITY(1,1) NOT NULL,
    [popedom_name] nvarchar(50) NULL,
    [popedom_father] int NULL,
    [popedom_url] nvarchar(255) NULL,
    [orderid] int NULL,
    [ishead] int NULL,
    CONSTRAINT [PK_popedom] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.PromptTemplate_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[PromptTemplate_list] (
    [id] int IDENTITY(1,1) NOT NULL,
    [addtime] datetime NOT NULL,
    [name] nvarchar(250) NOT NULL,
    [uptime] datetime NULL
);
END
GO

IF OBJECT_ID(N'dbo.RenderStyle_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[RenderStyle_list] (
    [id] int IDENTITY(1,1) NOT NULL,
    [addtime] datetime NOT NULL,
    [name] nvarchar(250) NOT NULL,
    [uptime] datetime NOT NULL,
    [orderid] int NOT NULL,
    [upload_pic_img] nvarchar(250) NOT NULL
);
END
GO

IF OBJECT_ID(N'dbo.SearchHot_List', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[SearchHot_List] (
    [id] int IDENTITY(1,1) NOT NULL,
    [addtime] datetime NOT NULL,
    [name] nvarchar(500) NOT NULL,
    [url] nvarchar(2500) NOT NULL,
    [isshow] int NOT NULL,
    [orderid] int NOT NULL,
    [uptime] datetime NULL,
    [num_click] int NOT NULL
);
END
GO

IF OBJECT_ID(N'dbo.ServiceLog_List', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[ServiceLog_List] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(500) NOT NULL,
    [info_] nvarchar(MAX) NOT NULL,
    [addtime] datetime NOT NULL,
    [status] int NOT NULL,
    [userid] int NOT NULL,
    [uptime] datetime NOT NULL,
    [looktime] nvarchar(50) NULL,
    CONSTRAINT [PK_ServiceLog_List] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.ServiceLogInfo_List', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[ServiceLogInfo_List] (
    [id] int IDENTITY(1,1) NOT NULL,
    [ServiceLog_Id] int NOT NULL,
    [info_] nvarchar(MAX) NOT NULL,
    [type] int NOT NULL,
    [addtime] datetime NOT NULL,
    [adminname] nvarchar(250) NULL
);
END
GO

IF OBJECT_ID(N'dbo.ServiceLogStatus_List', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[ServiceLogStatus_List] (
    [id] int NOT NULL,
    [name] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_ServiceLogStatus_List] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.tbl_class', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[tbl_class] (
    [id] int IDENTITY(1,1) NOT NULL,
    [parentid] int NOT NULL,
    [children] nvarchar(MAX) NULL,
    [classname] nvarchar(250) NOT NULL,
    [model] int NOT NULL,
    [orderid] int NOT NULL,
    [isurl] int NOT NULL,
    [classurl] nvarchar(250) NULL,
    [about] nvarchar(MAX) NULL,
    [info_] nvarchar(MAX) NULL,
    [isshow] int NOT NULL DEFAULT ((0)),
    [adddate] datetime NOT NULL,
    [isfoot] int NOT NULL,
    [istop] int NOT NULL DEFAULT ((0)),
    [description] nvarchar(MAX) NULL,
    [keywords] nvarchar(MAX) NULL,
    [upload_pic_m] nvarchar(250) NULL,
    [title] nvarchar(MAX) NULL,
    [urlnamebtn] nvarchar(250) NOT NULL,
    [upload_pic_pc] nvarchar(250) NULL,
    CONSTRAINT [PK_tbl_class] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.telcode_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[telcode_list] (
    [tel] nvarchar(150) NOT NULL,
    [type] int NOT NULL,
    [code] nvarchar(50) NOT NULL,
    [addtime] datetime NOT NULL,
    [img_x] int NOT NULL,
    [img_y] int NOT NULL,
    CONSTRAINT [PK_telcode_list] PRIMARY KEY ([tel], [type], [code], [addtime], [img_x], [img_y])
);
END
GO

IF OBJECT_ID(N'dbo.TopUpType_List', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[TopUpType_List] (
    [id] int IDENTITY(1,1) NOT NULL,
    [money] int NOT NULL,
    [isshow] int NOT NULL,
    CONSTRAINT [PK_TopUpType_List] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.user_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[user_list] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(150) NULL,
    [tel] nvarchar(150) NOT NULL,
    [addtime] datetime NOT NULL,
    [uptime] datetime NULL,
    [isshow] int NOT NULL,
    [logintime] nvarchar(50) NULL,
    [loginip] nvarchar(50) NULL,
    [code] nvarchar(50) NULL,
    [email] nvarchar(250) NULL,
    [upload_pic_avatar] nvarchar(250) NULL,
    CONSTRAINT [PK_user_list] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.user_login', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[user_login] (
    [id] int IDENTITY(1,1) NOT NULL,
    [username] nvarchar(50) NULL,
    [time] datetime NULL,
    [ip] nvarchar(50) NULL,
    [password] nvarchar(50) NULL,
    [type] int NULL,
    [content] nvarchar(MAX) NULL,
    CONSTRAINT [PK_User_Login] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.userfile_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[userfile_list] (
    [userid] int NOT NULL,
    [addtime] datetime NOT NULL,
    [up_filename] nvarchar(250) NOT NULL
);
END
GO

IF OBJECT_ID(N'dbo.userimg_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[userimg_list] (
    [userid] int NOT NULL,
    [addtime] datetime NOT NULL,
    [upload_pic_img] nvarchar(250) NOT NULL
);
END
GO

IF OBJECT_ID(N'dbo.userpaylog_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[userpaylog_list] (
    [user_id] int NOT NULL,
    [out_trade_no] nvarchar(250) NOT NULL,
    [add_time] datetime NOT NULL,
    [pay_type] int NOT NULL,
    [pay_status] int NOT NULL,
    [up_time] nvarchar(250) NULL,
    [payer_total] decimal(18,2) NULL,
    CONSTRAINT [PK_userpaylog_list] PRIMARY KEY ([out_trade_no])
);
END
GO

IF OBJECT_ID(N'dbo.userpayloginfo_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[userpayloginfo_list] (
    [id] bigint IDENTITY(1,1) NOT NULL,
    [appid] nvarchar(250) NULL,
    [mchid] nvarchar(250) NULL,
    [out_trade_no] nvarchar(250) NOT NULL,
    [transaction_id] nvarchar(250) NOT NULL,
    [trade_type] nvarchar(50) NULL,
    [trade_state] nvarchar(50) NULL,
    [trade_state_desc] nvarchar(350) NULL,
    [bank_type] nvarchar(50) NULL,
    [success_time] nvarchar(150) NOT NULL,
    [payer_total] decimal(18,2) NULL,
    [pay_type] int NOT NULL,
    [add_time] datetime NOT NULL,
    [user_id] int NOT NULL
);
END
GO

IF OBJECT_ID(N'dbo.websiteinfo_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[websiteinfo_list] (
    [id] int NOT NULL,
    [companyname] nvarchar(250) NULL,
    [wangzhanbeian] nvarchar(250) NULL,
    [wangzhanbeianurl] nvarchar(250) NULL,
    [gonganbeian] nvarchar(250) NULL,
    [gonganbeianurl] nvarchar(250) NULL,
    [banquan] nvarchar(250) NULL,
    [upload_pic_logotop] nvarchar(50) NULL,
    [upload_pic_favicon] nvarchar(50) NULL,
    [title] nvarchar(500) NULL,
    [keywords] nvarchar(500) NULL,
    [description] nvarchar(500) NULL,
    [emailnum] nvarchar(250) NULL,
    [emailpasswd] nvarchar(250) NULL,
    [email_to] nvarchar(250) NULL,
    [smtpserverport] nvarchar(250) NULL,
    [host] nvarchar(250) NULL,
    [emailname] nvarchar(250) NULL,
    [upload_pic_indexbj] nvarchar(50) NULL,
    [upload_pic_indexbj_m] nvarchar(50) NULL,
    [info_IntegrateWithdrawal] nvarchar(MAX) NULL,
    [info_WorkflowInfo] nvarchar(MAX) NULL,
    [money_integrate] int NULL,
    [integrate_donate] int NULL,
    [integrate_buy] int NULL,
    [integrate_fare] int NULL,
    [integrate_allocation] int NULL,
    CONSTRAINT [PK_websiteinfo_list] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.WorkflowTaskComment_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[WorkflowTaskComment_list] (
    [id] bigint IDENTITY(1,1) NOT NULL,
    [userai3dlog_id] bigint NOT NULL,
    [user_id] int NOT NULL,
    [addtime] datetime NOT NULL,
    [info_] nvarchar(MAX) NULL,
    [isshow] int NOT NULL,
    [num_dianzan] int NOT NULL,
    [reviewtime] nvarchar(50) NULL,
    [about_] nvarchar(MAX) NULL,
    [num_msg] int NOT NULL,
    CONSTRAINT [PK_Ai3dMsg_list] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.WorkflowTaskCommentImage_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[WorkflowTaskCommentImage_list] (
    [Ai3dMsg_Id] bigint NOT NULL,
    [upload_pic_info] nvarchar(250) NOT NULL,
    [addtime] datetime NOT NULL,
    [orderid] int NOT NULL
);
END
GO

IF OBJECT_ID(N'dbo.WorkflowTaskLog_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[WorkflowTaskLog_list] (
    [id] bigint IDENTITY(1,1) NOT NULL,
    [userid] int NOT NULL,
    [addtime] datetime NOT NULL,
    [jobid] nvarchar(250) NOT NULL,
    [requestid] nvarchar(250) NULL,
    [ai_key] nvarchar(500) NULL,
    [ai_img] nvarchar(500) NULL,
    [type] int NOT NULL,
    [img_url] nvarchar(500) NULL,
    CONSTRAINT [PK_Ai3dLog_list] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.WorkflowTaskReaction_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[WorkflowTaskReaction_list] (
    [ai3dmsg_id] bigint NOT NULL,
    [user_id] int NOT NULL,
    [addtime] datetime NOT NULL,
    CONSTRAINT [PK_Ai3DMsgZan_List] PRIMARY KEY ([ai3dmsg_id], [user_id])
);
END
GO

IF OBJECT_ID(N'dbo.WorkflowTaskReply_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[WorkflowTaskReply_list] (
    [id] bigint IDENTITY(1,1) NOT NULL,
    [ai3dmsg_id] bigint NOT NULL,
    [user_id] int NOT NULL,
    [addtime] datetime NOT NULL,
    [info_] nvarchar(MAX) NULL,
    [isshow] int NOT NULL,
    [reviewtime] nvarchar(50) NULL,
    [about_] nvarchar(MAX) NULL,
    [msguser_id] int NOT NULL,
    CONSTRAINT [PK_Ai3DMsgReply_list] PRIMARY KEY ([id])
);
END
GO

IF OBJECT_ID(N'dbo.WorkflowTaskReplyImage_list', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[WorkflowTaskReplyImage_list] (
    [Ai3DMsgReply_Id] bigint NOT NULL,
    [upload_pic_info] nvarchar(250) NOT NULL,
    [addtime] datetime NOT NULL,
    [orderid] int NOT NULL
);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_integrateExchangeLog_list_integratestatus_list')
ALTER TABLE [dbo].[integrateExchangeLog_list] ADD CONSTRAINT [FK_integrateExchangeLog_list_integratestatus_list] FOREIGN KEY ([status]) REFERENCES [dbo].[integratestatus_list] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_integrateExchangeLog_list_user_list')
ALTER TABLE [dbo].[integrateExchangeLog_list] ADD CONSTRAINT [FK_integrateExchangeLog_list_user_list] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user_list] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_integrateLog_list_integrateLogType_list')
ALTER TABLE [dbo].[integrateLog_list] ADD CONSTRAINT [FK_integrateLog_list_integrateLogType_list] FOREIGN KEY ([type]) REFERENCES [dbo].[integrateLogType_list] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_integrateLog_list_Literature')
ALTER TABLE [dbo].[integrateLog_list] ADD CONSTRAINT [FK_integrateLog_list_Literature] FOREIGN KEY ([literature_id]) REFERENCES [dbo].[Literature] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_integrateLog_list_user_list')
ALTER TABLE [dbo].[integrateLog_list] ADD CONSTRAINT [FK_integrateLog_list_user_list] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user_list] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_Literature_LiteratureCategory')
ALTER TABLE [dbo].[Literature] ADD CONSTRAINT [FK_Literature_LiteratureCategory] FOREIGN KEY ([category_id]) REFERENCES [dbo].[LiteratureCategory] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_Literature_LiteratureImportBatch')
ALTER TABLE [dbo].[Literature] ADD CONSTRAINT [FK_Literature_LiteratureImportBatch] FOREIGN KEY ([import_batch_id]) REFERENCES [dbo].[LiteratureImportBatch] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_Literature_reviewed_by_admin')
ALTER TABLE [dbo].[Literature] ADD CONSTRAINT [FK_Literature_reviewed_by_admin] FOREIGN KEY ([reviewed_by]) REFERENCES [dbo].[admin] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_Literature_user_list')
ALTER TABLE [dbo].[Literature] ADD CONSTRAINT [FK_Literature_user_list] FOREIGN KEY ([userid]) REFERENCES [dbo].[user_list] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_LiteratureAuthorMap_Author')
ALTER TABLE [dbo].[LiteratureAuthorMap] ADD CONSTRAINT [FK_LiteratureAuthorMap_Author] FOREIGN KEY ([author_id]) REFERENCES [dbo].[Author] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_LiteratureAuthorMap_Literature')
ALTER TABLE [dbo].[LiteratureAuthorMap] ADD CONSTRAINT [FK_LiteratureAuthorMap_Literature] FOREIGN KEY ([literature_id]) REFERENCES [dbo].[Literature] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_LiteratureCategory_Parent')
ALTER TABLE [dbo].[LiteratureCategory] ADD CONSTRAINT [FK_LiteratureCategory_Parent] FOREIGN KEY ([parent_id]) REFERENCES [dbo].[LiteratureCategory] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_LiteratureDownloadLog_Literature')
ALTER TABLE [dbo].[LiteratureDownloadLog] ADD CONSTRAINT [FK_LiteratureDownloadLog_Literature] FOREIGN KEY ([literature_id]) REFERENCES [dbo].[Literature] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_LiteratureDownloadLog_uploader_user_list')
ALTER TABLE [dbo].[LiteratureDownloadLog] ADD CONSTRAINT [FK_LiteratureDownloadLog_uploader_user_list] FOREIGN KEY ([literature_user_id]) REFERENCES [dbo].[user_list] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_LiteratureDownloadLog_user_list')
ALTER TABLE [dbo].[LiteratureDownloadLog] ADD CONSTRAINT [FK_LiteratureDownloadLog_user_list] FOREIGN KEY ([user_id]) REFERENCES [dbo].[user_list] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_LiteratureExportLog_user_list')
ALTER TABLE [dbo].[LiteratureExportLog] ADD CONSTRAINT [FK_LiteratureExportLog_user_list] FOREIGN KEY ([userid]) REFERENCES [dbo].[user_list] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_LiteratureFavorite_Literature')
ALTER TABLE [dbo].[LiteratureFavorite] ADD CONSTRAINT [FK_LiteratureFavorite_Literature] FOREIGN KEY ([literature_id]) REFERENCES [dbo].[Literature] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_LiteratureFavorite_user_list')
ALTER TABLE [dbo].[LiteratureFavorite] ADD CONSTRAINT [FK_LiteratureFavorite_user_list] FOREIGN KEY ([userid]) REFERENCES [dbo].[user_list] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_LiteratureFile_Literature')
ALTER TABLE [dbo].[LiteratureFile] ADD CONSTRAINT [FK_LiteratureFile_Literature] FOREIGN KEY ([literature_id]) REFERENCES [dbo].[Literature] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_LiteratureImportBatch_user_list')
ALTER TABLE [dbo].[LiteratureImportBatch] ADD CONSTRAINT [FK_LiteratureImportBatch_user_list] FOREIGN KEY ([userid]) REFERENCES [dbo].[user_list] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_LiteratureImportError_LiteratureImportBatch')
ALTER TABLE [dbo].[LiteratureImportError] ADD CONSTRAINT [FK_LiteratureImportError_LiteratureImportBatch] FOREIGN KEY ([batch_id]) REFERENCES [dbo].[LiteratureImportBatch] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_LiteratureTagMap_Literature')
ALTER TABLE [dbo].[LiteratureTagMap] ADD CONSTRAINT [FK_LiteratureTagMap_Literature] FOREIGN KEY ([literature_id]) REFERENCES [dbo].[Literature] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_LiteratureTagMap_LiteratureTag')
ALTER TABLE [dbo].[LiteratureTagMap] ADD CONSTRAINT [FK_LiteratureTagMap_LiteratureTag] FOREIGN KEY ([tag_id]) REFERENCES [dbo].[LiteratureTag] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_NoticeLog_List_NoticeLogStatus_List')
ALTER TABLE [dbo].[NoticeLog_List] ADD CONSTRAINT [FK_NoticeLog_List_NoticeLogStatus_List] FOREIGN KEY ([status]) REFERENCES [dbo].[NoticeLogStatus_List] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_NoticeLog_List_NoticeLogType_List')
ALTER TABLE [dbo].[NoticeLog_List] ADD CONSTRAINT [FK_NoticeLog_List_NoticeLogType_List] FOREIGN KEY ([type]) REFERENCES [dbo].[NoticeLogType_List] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_NoticeLog_List_user_list')
ALTER TABLE [dbo].[NoticeLog_List] ADD CONSTRAINT [FK_NoticeLog_List_user_list] FOREIGN KEY ([userid]) REFERENCES [dbo].[user_list] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ServiceLog_List_ServiceLogStatus_List')
ALTER TABLE [dbo].[ServiceLog_List] ADD CONSTRAINT [FK_ServiceLog_List_ServiceLogStatus_List] FOREIGN KEY ([status]) REFERENCES [dbo].[ServiceLogStatus_List] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ServiceLogInfo_List_ServiceLog_List')
ALTER TABLE [dbo].[ServiceLogInfo_List] ADD CONSTRAINT [FK_ServiceLogInfo_List_ServiceLog_List] FOREIGN KEY ([ServiceLog_Id]) REFERENCES [dbo].[ServiceLog_List] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Author') AND name=N'IX_Author_NameCn')
CREATE INDEX [IX_Author_NameCn] ON [dbo].[Author] ([name_cn]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.cosfile_list') AND name=N'IX_cosfile_list_up_filename')
CREATE INDEX [IX_cosfile_list_up_filename] ON [dbo].[cosfile_list] ([up_filename]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.integrateExchangeLog_list') AND name=N'IX_integrateExchangeLog_list_UserStatus')
CREATE INDEX [IX_integrateExchangeLog_list_UserStatus] ON [dbo].[integrateExchangeLog_list] ([user_id], [status], [addtime]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.integrateLog_list') AND name=N'IX_integrateLog_list_Literature')
CREATE INDEX [IX_integrateLog_list_Literature] ON [dbo].[integrateLog_list] ([literature_id], [addtime]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.integrateLog_list') AND name=N'IX_integrateLog_list_UserAddtime')
CREATE INDEX [IX_integrateLog_list_UserAddtime] ON [dbo].[integrateLog_list] ([user_id], [addtime]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.integrateLogType_list') AND name=N'UX_integrateLogType_list_name')
CREATE UNIQUE INDEX [UX_integrateLogType_list_name] ON [dbo].[integrateLogType_list] ([name]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.integratestatus_list') AND name=N'UX_integratestatus_list_name')
CREATE UNIQUE INDEX [UX_integratestatus_list_name] ON [dbo].[integratestatus_list] ([name]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Literature') AND name=N'IX_Literature_Canonical')
CREATE INDEX [IX_Literature_Canonical] ON [dbo].[Literature] ([canonical_literature_id], [status], [category_id], [publish_year]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Literature') AND name=N'IX_Literature_Doi')
CREATE INDEX [IX_Literature_Doi] ON [dbo].[Literature] ([doi]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Literature') AND name=N'IX_Literature_ImportBatch')
CREATE INDEX [IX_Literature_ImportBatch] ON [dbo].[Literature] ([import_batch_id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Literature') AND name=N'IX_Literature_Review')
CREATE INDEX [IX_Literature_Review] ON [dbo].[Literature] ([reviewed_by], [review_time]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Literature') AND name=N'IX_Literature_StatusCategoryAddtime')
CREATE INDEX [IX_Literature_StatusCategoryAddtime] ON [dbo].[Literature] ([status], [category_id], [addtime]) INCLUDE ([title], [publish_year], [source_type], [is_top]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Literature') AND name=N'IX_Literature_StatusPublishYear')
CREATE INDEX [IX_Literature_StatusPublishYear] ON [dbo].[Literature] ([status], [publish_year]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.LiteratureAuthorMap') AND name=N'IX_LiteratureAuthorMap_Author')
CREATE INDEX [IX_LiteratureAuthorMap_Author] ON [dbo].[LiteratureAuthorMap] ([author_id], [literature_id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.LiteratureAuthorMap') AND name=N'UX_LiteratureAuthorMap_LitAuthor')
CREATE UNIQUE INDEX [UX_LiteratureAuthorMap_LitAuthor] ON [dbo].[LiteratureAuthorMap] ([literature_id], [author_id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.LiteratureDownloadLog') AND name=N'UX_LiteratureDownloadLog_UserLiterature')
CREATE UNIQUE INDEX [UX_LiteratureDownloadLog_UserLiterature] ON [dbo].[LiteratureDownloadLog] ([user_id], [literature_id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.LiteratureFavorite') AND name=N'UX_LiteratureFavorite_literature_user')
CREATE UNIQUE INDEX [UX_LiteratureFavorite_literature_user] ON [dbo].[LiteratureFavorite] ([literature_id], [userid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.LiteratureFavorite') AND name=N'UX_LiteratureFavorite_LiteratureUser')
CREATE UNIQUE INDEX [UX_LiteratureFavorite_LiteratureUser] ON [dbo].[LiteratureFavorite] ([literature_id], [userid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.LiteratureFile') AND name=N'IX_LiteratureFile_LiteratureStatus')
CREATE INDEX [IX_LiteratureFile_LiteratureStatus] ON [dbo].[LiteratureFile] ([literature_id], [status], [orderid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.LiteratureLike') AND name=N'UX_LiteratureLike_literature_user')
CREATE UNIQUE INDEX [UX_LiteratureLike_literature_user] ON [dbo].[LiteratureLike] ([literature_id], [userid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.LiteratureTag') AND name=N'IX_LiteratureTag_Name')
CREATE INDEX [IX_LiteratureTag_Name] ON [dbo].[LiteratureTag] ([name]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.LiteratureTagMap') AND name=N'IX_LiteratureTagMap_Tag')
CREATE INDEX [IX_LiteratureTagMap_Tag] ON [dbo].[LiteratureTagMap] ([tag_id], [literature_id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.LiteratureTagMap') AND name=N'UX_LiteratureTagMap_LitTag')
CREATE UNIQUE INDEX [UX_LiteratureTagMap_LitTag] ON [dbo].[LiteratureTagMap] ([literature_id], [tag_id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.LiteratureVenueProfile') AND name=N'UX_LiteratureVenueProfile_type_name')
CREATE UNIQUE INDEX [UX_LiteratureVenueProfile_type_name] ON [dbo].[LiteratureVenueProfile] ([venue_type], [venue_name]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.NoticeLog_List') AND name=N'IX_NoticeLog_List_UserStatusAddtime')
CREATE INDEX [IX_NoticeLog_List_UserStatusAddtime] ON [dbo].[NoticeLog_List] ([userid], [status], [addtime]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.NoticeLogStatus_List') AND name=N'UX_NoticeLogStatus_List_name')
CREATE UNIQUE INDEX [UX_NoticeLogStatus_List_name] ON [dbo].[NoticeLogStatus_List] ([name]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.NoticeLogType_List') AND name=N'UX_NoticeLogType_List_name')
CREATE UNIQUE INDEX [UX_NoticeLogType_List_name] ON [dbo].[NoticeLogType_List] ([name]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.telcode_list') AND name=N'IX_telcode_list_TelTypeAddtime')
CREATE INDEX [IX_telcode_list_TelTypeAddtime] ON [dbo].[telcode_list] ([tel], [type], [addtime]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.user_list') AND name=N'UX_user_list_tel')
CREATE UNIQUE INDEX [UX_user_list_tel] ON [dbo].[user_list] ([tel]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.userfile_list') AND name=N'IX_userfile_list_up_filename')
CREATE INDEX [IX_userfile_list_up_filename] ON [dbo].[userfile_list] ([up_filename]);
GO

