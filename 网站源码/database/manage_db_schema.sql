/*
  Final initialization script for the literature management project.
  Execute this script once in an empty SQL Server database.
*/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

SET ANSI_PADDING ON;
GO

CREATE TABLE [dbo].[admin] (
    [id] int IDENTITY(1,1) NOT NULL,
    [username] nvarchar(50) NOT NULL,
    [password] nvarchar(50) NULL,
    [popedom] nvarchar(MAX) NULL,
    [lastloginip] nvarchar(50) NULL,
    [cityid] int NULL,
    [locks] int NULL,
    [code] nvarchar(50) NULL,
    [lastlogindate] datetime NULL DEFAULT (getdate())
);
GO

CREATE TABLE [dbo].[appeal_list] (
    [id] bigint IDENTITY(1,1) NOT NULL,
    [url] nvarchar(2500) NOT NULL,
    [info_] nvarchar(MAX) NOT NULL,
    [addtime] datetime NOT NULL,
    [status] int NOT NULL,
    [userid] int NOT NULL
);
GO

CREATE TABLE [dbo].[appealimg_list] (
    [appeal_id] bigint NOT NULL,
    [upload_pic_info] nvarchar(250) NOT NULL,
    [addtime] datetime NOT NULL,
    [orderid] int NOT NULL
);
GO

CREATE TABLE [dbo].[Author] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name_cn] nvarchar(100) NOT NULL,
    [name_en] nvarchar(200) NULL,
    [institution] nvarchar(300) NULL,
    [orcid] nvarchar(50) NULL,
    [email] nvarchar(200) NULL,
    [status] int NOT NULL DEFAULT ((1)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    [current_institution_id] int NULL,
    [current_institution_name] nvarchar(1000) NULL,
    [current_institution_literature_id] int NULL,
    [current_institution_sort_date] date NULL,
    [current_institution_precision] nvarchar(20) NULL,
    [identity_status] nvarchar(30) NULL,
    [merge_group_id] int NULL,
    [updatetime] datetime NOT NULL DEFAULT (getdate()),
    [merged_to_author_id] int NULL
);
GO

CREATE TABLE [dbo].[AuthorInstitutionHistory] (
    [id] int IDENTITY(1,1) NOT NULL,
    [author_id] int NOT NULL,
    [institution_name] nvarchar(500) NOT NULL,
    [is_current] int NOT NULL DEFAULT ((0)),
    [start_year] int NULL,
    [end_year] int NULL,
    [source_literature_id] int NULL,
    [source_type] nvarchar(50) NULL,
    [remark] nvarchar(500) NULL,
    [status] int NOT NULL DEFAULT ((1)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    [updatetime] datetime NOT NULL DEFAULT (getdate()),
    [institution_id] int NULL
);
GO

CREATE TABLE [dbo].[AuthorMergeLog] (
    [id] int IDENTITY(1,1) NOT NULL,
    [master_author_id] int NOT NULL,
    [duplicate_author_id] int NOT NULL,
    [admin_id] int NOT NULL DEFAULT ((0)),
    [remark] nvarchar(1000) NULL,
    [addtime] datetime NOT NULL DEFAULT (getdate())
);
GO

CREATE TABLE [dbo].[Conference] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name_cn] nvarchar(300) NOT NULL DEFAULT (N''),
    [name_en] nvarchar(500) NULL,
    [acronym] nvarchar(100) NULL,
    [normalized_name] nvarchar(500) NOT NULL,
    [organizer] nvarchar(500) NULL,
    [country] nvarchar(200) NULL,
    [city] nvarchar(200) NULL,
    [start_date] datetime NULL,
    [end_date] datetime NULL,
    [website] nvarchar(1000) NULL,
    [status] int NOT NULL DEFAULT ((1)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    [updatetime] datetime NOT NULL DEFAULT (getdate())
);
GO

CREATE TABLE [dbo].[cosfile_list] (
    [userid] int NOT NULL,
    [addtime] datetime NOT NULL,
    [up_filename] nvarchar(250) NOT NULL
);
GO

CREATE TABLE [dbo].[daoru_list] (
    [id] int IDENTITY(1,1) NOT NULL,
    [posttime] datetime NOT NULL,
    [r_info] nvarchar(500) NOT NULL,
    [status] int NOT NULL,
    [type] int NOT NULL
);
GO

CREATE TABLE [dbo].[daoruerr_list] (
    [info] nvarchar(500) NOT NULL,
    [filename] nvarchar(250) NOT NULL,
    [addtime] datetime NOT NULL,
    [daoruid] int NOT NULL
);
GO

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
    [istop] int NOT NULL
);
GO

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
    [url] nvarchar(2500) NULL
);
GO

CREATE TABLE [dbo].[Institution] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name_cn] nvarchar(300) NOT NULL DEFAULT (N''),
    [name_en] nvarchar(500) NULL,
    [normalized_name] nvarchar(500) NOT NULL,
    [alias_names] nvarchar(MAX) NULL,
    [country] nvarchar(200) NULL,
    [province] nvarchar(200) NULL,
    [city] nvarchar(200) NULL,
    [website] nvarchar(1000) NULL,
    [status] int NOT NULL DEFAULT ((1)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    [updatetime] datetime NOT NULL DEFAULT (getdate()),
    [parent_id] int NULL
);
GO

CREATE TABLE [dbo].[InstitutionAlias] (
    [id] int IDENTITY(1,1) NOT NULL,
    [institution_id] int NOT NULL,
    [alias_name] nvarchar(500) NOT NULL,
    [normalized_alias] nvarchar(500) NOT NULL,
    [language] nvarchar(50) NULL,
    [status] int NOT NULL DEFAULT ((1)),
    [addtime] datetime NOT NULL DEFAULT (getdate())
);
GO

CREATE TABLE [dbo].[integrate_list] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(250) NOT NULL,
    [orderid] int NOT NULL,
    [uptime] datetime NOT NULL,
    [addtime] datetime NOT NULL,
    [upload_pic_img] nvarchar(250) NULL,
    [about_] nvarchar(MAX) NULL,
    [num_integrate] int NOT NULL
);
GO

CREATE TABLE [dbo].[integrateExchangeLog_list] (
    [id] bigint IDENTITY(1,1) NOT NULL,
    [name] nvarchar(500) NOT NULL,
    [num_integrate] int NOT NULL,
    [codestr] nvarchar(250) NOT NULL,
    [addtime] datetime NOT NULL,
    [status] int NULL,
    [user_id] int NOT NULL,
    [upload_pic_img] nvarchar(250) NULL,
    [hexiaotime] nvarchar(250) NULL
);
GO

CREATE TABLE [dbo].[integrateLog_list] (
    [id] int IDENTITY(1,1) NOT NULL,
    [num_integrate] int NOT NULL,
    [type] int NOT NULL,
    [name] nvarchar(500) NOT NULL,
    [info_] nvarchar(MAX) NULL,
    [addtime] datetime NOT NULL,
    [user_id] int NOT NULL,
    [adminname] nvarchar(250) NULL,
    [orderpro_orderno] nvarchar(50) NULL,
    [literature_id] int NULL
);
GO

CREATE TABLE [dbo].[integrateLogType_list] (
    [id] int NOT NULL,
    [name] nvarchar(50) NOT NULL,
    [num_integrate] int NULL
);
GO

CREATE TABLE [dbo].[integratestatus_list] (
    [id] int NOT NULL,
    [name] nvarchar(50) NOT NULL
);
GO

CREATE TABLE [dbo].[Journal] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name_cn] nvarchar(300) NOT NULL DEFAULT (N''),
    [name_en] nvarchar(500) NULL,
    [normalized_name] nvarchar(500) NOT NULL,
    [issn] nvarchar(100) NULL,
    [eissn] nvarchar(100) NULL,
    [publisher] nvarchar(500) NULL,
    [country] nvarchar(200) NULL,
    [subject] nvarchar(500) NULL,
    [website] nvarchar(1000) NULL,
    [status] int NOT NULL DEFAULT ((1)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    [updatetime] datetime NOT NULL DEFAULT (getdate())
);
GO

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
GO

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
    [journal_id] int NULL,
    [conference_id] int NULL,
    [publish_month] int NULL,
    [publish_day] int NULL,
    [publish_date] date NULL,
    [publish_date_precision] nvarchar(20) NULL
);
GO

CREATE TABLE [dbo].[LiteratureAuthorInstitutionMap] (
    [id] int IDENTITY(1,1) NOT NULL,
    [literature_id] int NOT NULL,
    [author_id] int NOT NULL,
    [literature_author_map_id] int NOT NULL DEFAULT ((0)),
    [institution_id] int NOT NULL,
    [affiliation_text] nvarchar(1000) NULL,
    [author_order] int NOT NULL DEFAULT ((0)),
    [institution_order] int NOT NULL DEFAULT ((0)),
    [is_current_for_author] int NOT NULL DEFAULT ((0)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    [source_type] nvarchar(50) NULL,
    [is_confirmed] int NOT NULL DEFAULT ((0)),
    [confirm_by] int NULL,
    [confirm_time] datetime NULL,
    [updatetime] datetime NOT NULL DEFAULT (getdate())
);
GO

CREATE TABLE [dbo].[LiteratureAuthorMap] (
    [id] int IDENTITY(1,1) NOT NULL,
    [literature_id] int NOT NULL,
    [author_id] int NOT NULL,
    [author_order] int NOT NULL DEFAULT ((1)),
    [is_corresponding] int NOT NULL DEFAULT ((0)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    [affiliation_text] nvarchar(500) NULL,
    [raw_author_text] nvarchar(300) NULL,
    [display_author_name] nvarchar(300) NULL,
    [author_name_raw] nvarchar(600) NULL,
    [is_confirmed] int NOT NULL DEFAULT ((0)),
    [confirm_by] int NULL,
    [confirm_time] datetime NULL,
    [identity_confidence] decimal(5,2) NULL
);
GO

CREATE TABLE [dbo].[LiteratureCategory] (
    [id] int IDENTITY(1,1) NOT NULL,
    [parent_id] int NULL DEFAULT ((0)),
    [name] nvarchar(100) NOT NULL,
    [name_en] nvarchar(200) NULL,
    [code] nvarchar(100) NULL,
    [orderid] int NOT NULL DEFAULT ((0)),
    [status] int NOT NULL DEFAULT ((1)),
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    [updatetime] datetime NOT NULL DEFAULT (getdate())
);
GO

CREATE TABLE [dbo].[LiteratureComment] (
    [id] int IDENTITY(1,1) NOT NULL,
    [literature_id] int NOT NULL,
    [canonical_literature_id] int NULL,
    [userid] int NOT NULL,
    [parent_id] int NOT NULL DEFAULT ((0)),
    [content] nvarchar(MAX) NOT NULL,
    [status] int NOT NULL DEFAULT ((0)),
    [like_count] int NOT NULL DEFAULT ((0)),
    [report_count] int NOT NULL DEFAULT ((0)),
    [is_deleted] int NOT NULL DEFAULT ((0)),
    [delete_time] datetime NULL,
    [reviewed_by] int NULL,
    [review_time] datetime NULL,
    [review_remark] nvarchar(500) NULL,
    [addtime] datetime NOT NULL DEFAULT (getdate()),
    [updatetime] datetime NOT NULL DEFAULT (getdate())
);
GO

CREATE TABLE [dbo].[LiteratureDownloadLog] (
    [id] int IDENTITY(1,1) NOT NULL,
    [literature_id] int NOT NULL,
    [user_id] int NOT NULL,
    [literature_title] nvarchar(500) NULL,
    [file_url] nvarchar(255) NULL,
    [download_points] int NOT NULL DEFAULT ((0)),
    [literature_user_id] int NOT NULL DEFAULT ((0)),
    [addtime] datetime NOT NULL DEFAULT (getdate())
);
GO

CREATE TABLE [dbo].[LiteratureExportLog] (
    [id] int IDENTITY(1,1) NOT NULL,
    [export_name] nvarchar(200) NOT NULL,
    [export_type] nvarchar(50) NOT NULL,
    [file_name] nvarchar(255) NULL,
    [record_count] int NOT NULL DEFAULT ((0)),
    [userid] int NOT NULL DEFAULT ((0)),
    [addtime] datetime NOT NULL DEFAULT (getdate())
);
GO

CREATE TABLE [dbo].[LiteratureFavorite] (
    [id] int IDENTITY(1,1) NOT NULL,
    [literature_id] int NOT NULL,
    [userid] int NOT NULL,
    [addtime] datetime NOT NULL DEFAULT (getdate())
);
GO

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
    [addtime] datetime NOT NULL DEFAULT (getdate())
);
GO

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
    [finishtime] datetime NULL
);
GO

CREATE TABLE [dbo].[LiteratureImportError] (
    [id] int IDENTITY(1,1) NOT NULL,
    [batch_id] int NOT NULL,
    [row_no] int NOT NULL,
    [title] nvarchar(500) NULL,
    [error_msg] nvarchar(1000) NOT NULL,
    [raw_data] nvarchar(MAX) NULL,
    [addtime] datetime NOT NULL DEFAULT (getdate())
);
GO

CREATE TABLE [dbo].[LiteratureLike] (
    [id] int IDENTITY(1,1) NOT NULL,
    [literature_id] int NOT NULL,
    [userid] int NOT NULL,
    [addtime] datetime NOT NULL DEFAULT (getdate())
);
GO

CREATE TABLE [dbo].[LiteratureStatusLog] (
    [id] int IDENTITY(1,1) NOT NULL,
    [literature_id] int NOT NULL,
    [old_status] int NULL,
    [new_status] int NULL,
    [reviewed_by] int NULL,
    [review_time] datetime NULL,
    [remark] nvarchar(1000) NULL,
    [addtime] datetime NOT NULL DEFAULT (getdate())
);
GO

CREATE TABLE [dbo].[LiteratureTag] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(100) NOT NULL,
    [orderid] int NOT NULL DEFAULT ((0)),
    [status] int NOT NULL DEFAULT ((1)),
    [addtime] datetime NOT NULL DEFAULT (getdate())
);
GO

CREATE TABLE [dbo].[LiteratureTagMap] (
    [id] int IDENTITY(1,1) NOT NULL,
    [literature_id] int NOT NULL,
    [tag_id] int NOT NULL,
    [addtime] datetime NOT NULL DEFAULT (getdate())
);
GO

CREATE TABLE [dbo].[logincode_list] (
    [code] nvarchar(50) NOT NULL,
    [val] nchar(10) NOT NULL,
    [addtime] datetime NOT NULL,
    [ip_str] nvarchar(50) NOT NULL,
    [type] int NOT NULL
);
GO

CREATE TABLE [dbo].[LoginSingle_List] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(250) NOT NULL,
    [IsShow] int NOT NULL,
    [OrderId] int NOT NULL,
    [UpTime] datetime NOT NULL,
    [AddTime] datetime NOT NULL,
    [Info_] nvarchar(MAX) NULL
);
GO

CREATE TABLE [dbo].[MasterNameChangeLog] (
    [id] int IDENTITY(1,1) NOT NULL,
    [entity_type] nvarchar(50) NOT NULL,
    [entity_id] int NOT NULL,
    [old_name_cn] nvarchar(500) NULL,
    [new_name_cn] nvarchar(500) NULL,
    [old_name_en] nvarchar(500) NULL,
    [new_name_en] nvarchar(500) NULL,
    [old_normalized_name] nvarchar(500) NULL,
    [new_normalized_name] nvarchar(500) NULL,
    [addtime] datetime NOT NULL DEFAULT (getdate())
);
GO

CREATE TABLE [dbo].[NoticeLog_List] (
    [id] int IDENTITY(1,1) NOT NULL,
    [info_] nvarchar(MAX) NULL,
    [type] int NOT NULL,
    [addtime] datetime NOT NULL,
    [userid] int NOT NULL,
    [looktime] nvarchar(50) NULL,
    [status] int NOT NULL,
    [url] nvarchar(500) NULL,
    [name] nvarchar(500) NOT NULL
);
GO

CREATE TABLE [dbo].[NoticeLogStatus_List] (
    [id] int NOT NULL,
    [name] nvarchar(50) NOT NULL
);
GO

CREATE TABLE [dbo].[NoticeLogType_List] (
    [id] int NOT NULL,
    [name] nvarchar(50) NOT NULL
);
GO

CREATE TABLE [dbo].[popedom] (
    [id] int IDENTITY(1,1) NOT NULL,
    [popedom_name] nvarchar(50) NULL,
    [popedom_father] int NULL,
    [popedom_url] nvarchar(255) NULL,
    [orderid] int NULL,
    [ishead] int NULL
);
GO

CREATE TABLE [dbo].[popedomhead] (
    [id] int IDENTITY(1,1) NOT NULL,
    [adminid] int NULL,
    [headid] int NULL,
    [popedomid] int NULL
);
GO

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
GO

CREATE TABLE [dbo].[ServiceLog_List] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(500) NOT NULL,
    [info_] nvarchar(MAX) NOT NULL,
    [addtime] datetime NOT NULL,
    [status] int NOT NULL,
    [userid] int NOT NULL,
    [uptime] datetime NOT NULL,
    [looktime] nvarchar(50) NULL
);
GO

CREATE TABLE [dbo].[ServiceLogInfo_List] (
    [id] int IDENTITY(1,1) NOT NULL,
    [ServiceLog_Id] int NOT NULL,
    [info_] nvarchar(MAX) NOT NULL,
    [type] int NOT NULL,
    [addtime] datetime NOT NULL,
    [adminname] nvarchar(250) NULL
);
GO

CREATE TABLE [dbo].[ServiceLogStatus_List] (
    [id] int NOT NULL,
    [name] nvarchar(50) NOT NULL
);
GO

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
    [upload_pic_pc] nvarchar(250) NULL
);
GO

CREATE TABLE [dbo].[telcode_list] (
    [tel] nvarchar(150) NOT NULL,
    [type] int NOT NULL,
    [code] nvarchar(50) NOT NULL,
    [addtime] datetime NOT NULL,
    [img_x] int NOT NULL,
    [img_y] int NOT NULL
);
GO

CREATE TABLE [dbo].[TopUpType_List] (
    [id] int IDENTITY(1,1) NOT NULL,
    [money] int NOT NULL,
    [isshow] int NOT NULL
);
GO

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
    [upload_pic_avatar] nvarchar(250) NULL
);
GO

CREATE TABLE [dbo].[user_login] (
    [id] int IDENTITY(1,1) NOT NULL,
    [username] nvarchar(50) NULL,
    [time] datetime NULL,
    [ip] nvarchar(50) NULL,
    [password] nvarchar(50) NULL,
    [type] int NULL,
    [content] nvarchar(MAX) NULL
);
GO

CREATE TABLE [dbo].[userfile_list] (
    [userid] int NOT NULL,
    [addtime] datetime NOT NULL,
    [up_filename] nvarchar(250) NOT NULL
);
GO

CREATE TABLE [dbo].[userimg_list] (
    [userid] int NOT NULL,
    [addtime] datetime NOT NULL,
    [upload_pic_img] nvarchar(250) NOT NULL
);
GO

CREATE TABLE [dbo].[userpaylog_list] (
    [user_id] int NOT NULL,
    [out_trade_no] nvarchar(250) NOT NULL,
    [add_time] datetime NOT NULL,
    [pay_type] int NOT NULL,
    [pay_status] int NOT NULL,
    [up_time] nvarchar(250) NULL,
    [payer_total] decimal(18,2) NULL
);
GO

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
GO

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
    [money_integrate] int NULL,
    [integrate_donate] int NULL,
    [integrate_buy] int NULL,
    [integrate_fare] int NULL,
    [integrate_allocation] int NULL
);
GO

ALTER TABLE [dbo].[admin] ADD CONSTRAINT [PK_Admin] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[appeal_list] ADD CONSTRAINT [PK_appeal_list] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[appealimg_list] ADD CONSTRAINT [IX_appealimg_list] UNIQUE NONCLUSTERED ([upload_pic_info] ASC);
GO

ALTER TABLE [dbo].[Author] ADD CONSTRAINT [PK_Author] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[AuthorInstitutionHistory] ADD CONSTRAINT [PK_AuthorInstitutionHistory] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[AuthorMergeLog] ADD CONSTRAINT [PK_AuthorMergeLog] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[Conference] ADD CONSTRAINT [PK_Conference] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[data_list] ADD CONSTRAINT [PK_data_list] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[indexsingle_list] ADD CONSTRAINT [PK_indexsingle_list] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[Institution] ADD CONSTRAINT [PK_Institution] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[InstitutionAlias] ADD CONSTRAINT [PK_InstitutionAlias] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[integrate_list] ADD CONSTRAINT [PK_integrate_list] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[integrateExchangeLog_list] ADD CONSTRAINT [PK_integrateExchangeLog_list] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[integrateExchangeLog_list] ADD CONSTRAINT [IX_integrateExchangeLog_list] UNIQUE NONCLUSTERED ([codestr] ASC);
GO

ALTER TABLE [dbo].[integrateLog_list] ADD CONSTRAINT [PK_integrateLog_list] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[integrateLogType_list] ADD CONSTRAINT [PK_integrateLogType_list] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[integratestatus_list] ADD CONSTRAINT [PK_integratestatus_list] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[Journal] ADD CONSTRAINT [PK_Journal] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[Literature] ADD CONSTRAINT [PK_Literature] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[LiteratureAuthorInstitutionMap] ADD CONSTRAINT [PK_LiteratureAuthorInstitutionMap] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[LiteratureAuthorMap] ADD CONSTRAINT [PK_LiteratureAuthorMap] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[LiteratureCategory] ADD CONSTRAINT [PK_LiteratureCategory] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[LiteratureComment] ADD CONSTRAINT [PK_LiteratureComment] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[LiteratureDownloadLog] ADD CONSTRAINT [PK_LiteratureDownloadLog] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[LiteratureExportLog] ADD CONSTRAINT [PK_LiteratureExportLog] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[LiteratureFavorite] ADD CONSTRAINT [PK_LiteratureFavorite] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[LiteratureFile] ADD CONSTRAINT [PK_LiteratureFile] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[LiteratureImportBatch] ADD CONSTRAINT [PK_LiteratureImportBatch] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[LiteratureImportError] ADD CONSTRAINT [PK_LiteratureImportError] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[LiteratureLike] ADD CONSTRAINT [PK_LiteratureLike] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[LiteratureStatusLog] ADD CONSTRAINT [PK_LiteratureStatusLog] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[LiteratureTag] ADD CONSTRAINT [PK_LiteratureTag] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[LiteratureTagMap] ADD CONSTRAINT [PK_LiteratureTagMap] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[logincode_list] ADD CONSTRAINT [PK_logincode_list] PRIMARY KEY CLUSTERED ([code] ASC);
GO

ALTER TABLE [dbo].[LoginSingle_List] ADD CONSTRAINT [PK_LoginSingle_List] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

ALTER TABLE [dbo].[MasterNameChangeLog] ADD CONSTRAINT [PK_MasterNameChangeLog] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[NoticeLog_List] ADD CONSTRAINT [PK_NoticeLog_List] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[NoticeLogStatus_List] ADD CONSTRAINT [PK_NoticeLogStatus_List] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[NoticeLogType_List] ADD CONSTRAINT [PK_NoticeLogType_List] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[popedom] ADD CONSTRAINT [PK_popedom] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[popedomhead] ADD CONSTRAINT [PK_popedomhead] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[ServiceLog_List] ADD CONSTRAINT [PK_ServiceLog_List] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[ServiceLogStatus_List] ADD CONSTRAINT [PK_ServiceLogStatus_List] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[tbl_class] ADD CONSTRAINT [PK_tbl_class] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[telcode_list] ADD CONSTRAINT [PK_telcode_list] PRIMARY KEY CLUSTERED ([tel] ASC, [type] ASC, [code] ASC, [addtime] ASC, [img_x] ASC, [img_y] ASC);
GO

ALTER TABLE [dbo].[TopUpType_List] ADD CONSTRAINT [PK_TopUpType_List] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[TopUpType_List] ADD CONSTRAINT [IX_TopUpType_List] UNIQUE NONCLUSTERED ([money] ASC);
GO

ALTER TABLE [dbo].[user_list] ADD CONSTRAINT [PK_user_list] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[user_login] ADD CONSTRAINT [PK_User_Login] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[userpaylog_list] ADD CONSTRAINT [PK_userpaylog_list] PRIMARY KEY CLUSTERED ([out_trade_no] ASC);
GO

ALTER TABLE [dbo].[websiteinfo_list] ADD CONSTRAINT [PK_websiteinfo_list] PRIMARY KEY CLUSTERED ([id] ASC);
GO

ALTER TABLE [dbo].[Literature] WITH CHECK ADD CONSTRAINT [CK_Literature_publish_date_precision] CHECK ([publish_date_precision] IS NULL OR ([publish_date_precision]=N'unknown' OR [publish_date_precision]=N'year' OR [publish_date_precision]=N'month' OR [publish_date_precision]=N'day'));
ALTER TABLE [dbo].[Literature] CHECK CONSTRAINT [CK_Literature_publish_date_precision];
GO

ALTER TABLE [dbo].[Literature] WITH CHECK ADD CONSTRAINT [CK_Literature_publish_day] CHECK ([publish_day] IS NULL OR [publish_day]>=(1) AND [publish_day]<=(31));
ALTER TABLE [dbo].[Literature] CHECK CONSTRAINT [CK_Literature_publish_day];
GO

ALTER TABLE [dbo].[Literature] WITH CHECK ADD CONSTRAINT [CK_Literature_publish_month] CHECK ([publish_month] IS NULL OR [publish_month]>=(1) AND [publish_month]<=(12));
ALTER TABLE [dbo].[Literature] CHECK CONSTRAINT [CK_Literature_publish_month];
GO

CREATE NONCLUSTERED INDEX [IX_Author_CurrentInstitution] ON [dbo].[Author] ([current_institution_literature_id] ASC, [current_institution_sort_date] DESC) INCLUDE ([current_institution_name]);
GO

CREATE NONCLUSTERED INDEX [IX_Author_NameCn] ON [dbo].[Author] ([name_cn] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_AuthorInstitutionHistory_author] ON [dbo].[AuthorInstitutionHistory] ([author_id] ASC, [status] ASC, [is_current] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_AuthorInstitutionHistory_literature] ON [dbo].[AuthorInstitutionHistory] ([source_literature_id] ASC, [author_id] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_Conference_normalized] ON [dbo].[Conference] ([normalized_name] ASC) WHERE ([status]<>(-1) AND [normalized_name]<>N'');
GO

CREATE NONCLUSTERED INDEX [IX_cosfile_list_up_filename] ON [dbo].[cosfile_list] ([up_filename] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Institution_ParentId] ON [dbo].[Institution] ([parent_id] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_Institution_normalized] ON [dbo].[Institution] ([normalized_name] ASC) WHERE ([status]<>(-1) AND [normalized_name]<>N'');
GO

CREATE NONCLUSTERED INDEX [IX_InstitutionAlias_normalized] ON [dbo].[InstitutionAlias] ([normalized_alias] ASC, [status] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_integrateExchangeLog_list_UserStatus] ON [dbo].[integrateExchangeLog_list] ([user_id] ASC, [status] ASC, [addtime] DESC);
GO

CREATE NONCLUSTERED INDEX [IX_integrateLog_list_Literature] ON [dbo].[integrateLog_list] ([literature_id] ASC, [addtime] DESC);
GO

CREATE NONCLUSTERED INDEX [IX_integrateLog_list_UserAddtime] ON [dbo].[integrateLog_list] ([user_id] ASC, [addtime] DESC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_integrateLogType_list_name] ON [dbo].[integrateLogType_list] ([name] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_integratestatus_list_name] ON [dbo].[integratestatus_list] ([name] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_Journal_normalized] ON [dbo].[Journal] ([normalized_name] ASC) WHERE ([status]<>(-1) AND [normalized_name]<>N'');
GO

CREATE NONCLUSTERED INDEX [IX_Literature_Canonical] ON [dbo].[Literature] ([canonical_literature_id] ASC, [status] ASC, [category_id] ASC, [publish_year] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Literature_Doi] ON [dbo].[Literature] ([doi] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Literature_ImportBatch] ON [dbo].[Literature] ([import_batch_id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Literature_PublishDate] ON [dbo].[Literature] ([publish_date] DESC, [publish_year] DESC, [id] DESC);
GO

CREATE NONCLUSTERED INDEX [IX_Literature_Review] ON [dbo].[Literature] ([reviewed_by] ASC, [review_time] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Literature_StatusCategoryAddtime] ON [dbo].[Literature] ([status] ASC, [category_id] ASC, [addtime] ASC) INCLUDE ([title], [publish_year], [source_type], [is_top]);
GO

CREATE NONCLUSTERED INDEX [IX_Literature_StatusPublishYear] ON [dbo].[Literature] ([status] ASC, [publish_year] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_LitAuthorInstitution_AuthorLiterature] ON [dbo].[LiteratureAuthorInstitutionMap] ([author_id] ASC, [literature_id] ASC, [institution_order] ASC, [id] ASC) INCLUDE ([institution_id], [affiliation_text], [is_current_for_author]);
GO

CREATE NONCLUSTERED INDEX [IX_LiteratureAuthorInstitutionMap_author] ON [dbo].[LiteratureAuthorInstitutionMap] ([author_id] ASC, [institution_id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_LiteratureAuthorInstitutionMap_literature] ON [dbo].[LiteratureAuthorInstitutionMap] ([literature_id] ASC, [author_order] ASC, [institution_order] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_LiteratureAuthorMap_Author] ON [dbo].[LiteratureAuthorMap] ([author_id] ASC, [literature_id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_LiteratureAuthorMap_AuthorLiterature] ON [dbo].[LiteratureAuthorMap] ([author_id] ASC, [literature_id] ASC, [author_order] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_LiteratureAuthorMap_LitAuthor] ON [dbo].[LiteratureAuthorMap] ([literature_id] ASC, [author_id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_LiteratureComment_Literature_Status] ON [dbo].[LiteratureComment] ([canonical_literature_id] ASC, [literature_id] ASC, [parent_id] ASC, [status] ASC, [is_deleted] ASC, [addtime] DESC);
GO

CREATE NONCLUSTERED INDEX [IX_LiteratureComment_User] ON [dbo].[LiteratureComment] ([userid] ASC, [is_deleted] ASC, [status] ASC, [addtime] DESC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_LiteratureDownloadLog_UserLiterature] ON [dbo].[LiteratureDownloadLog] ([user_id] ASC, [literature_id] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_LiteratureFavorite_literature_user] ON [dbo].[LiteratureFavorite] ([literature_id] ASC, [userid] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_LiteratureFavorite_LiteratureUser] ON [dbo].[LiteratureFavorite] ([literature_id] ASC, [userid] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_LiteratureFile_LiteratureStatus] ON [dbo].[LiteratureFile] ([literature_id] ASC, [status] ASC, [orderid] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_LiteratureLike_literature_user] ON [dbo].[LiteratureLike] ([literature_id] ASC, [userid] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_LiteratureTag_Name] ON [dbo].[LiteratureTag] ([name] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_LiteratureTagMap_Tag] ON [dbo].[LiteratureTagMap] ([tag_id] ASC, [literature_id] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_LiteratureTagMap_LitTag] ON [dbo].[LiteratureTagMap] ([literature_id] ASC, [tag_id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_NoticeLog_List_UserStatusAddtime] ON [dbo].[NoticeLog_List] ([userid] ASC, [status] ASC, [addtime] DESC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_NoticeLogStatus_List_name] ON [dbo].[NoticeLogStatus_List] ([name] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_NoticeLogType_List_name] ON [dbo].[NoticeLogType_List] ([name] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_popedomhead_admin_popedom] ON [dbo].[popedomhead] ([adminid] ASC, [popedomid] ASC) WHERE ([adminid] IS NOT NULL AND [popedomid] IS NOT NULL);
GO

CREATE NONCLUSTERED INDEX [IX_telcode_list_TelTypeAddtime] ON [dbo].[telcode_list] ([tel] ASC, [type] ASC, [addtime] DESC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_user_list_tel] ON [dbo].[user_list] ([tel] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_userfile_list_up_filename] ON [dbo].[userfile_list] ([up_filename] ASC);
GO

ALTER TABLE [dbo].[Institution] WITH CHECK ADD CONSTRAINT [FK_Institution_Parent] FOREIGN KEY([parent_id]) REFERENCES [dbo].[Institution] ([id]);
ALTER TABLE [dbo].[Institution] CHECK CONSTRAINT [FK_Institution_Parent];
GO

ALTER TABLE [dbo].[integrateExchangeLog_list] WITH CHECK ADD CONSTRAINT [FK_integrateExchangeLog_list_integratestatus_list] FOREIGN KEY([status]) REFERENCES [dbo].[integratestatus_list] ([id]);
ALTER TABLE [dbo].[integrateExchangeLog_list] CHECK CONSTRAINT [FK_integrateExchangeLog_list_integratestatus_list];
GO

ALTER TABLE [dbo].[integrateExchangeLog_list] WITH CHECK ADD CONSTRAINT [FK_integrateExchangeLog_list_user_list] FOREIGN KEY([user_id]) REFERENCES [dbo].[user_list] ([id]);
ALTER TABLE [dbo].[integrateExchangeLog_list] CHECK CONSTRAINT [FK_integrateExchangeLog_list_user_list];
GO

ALTER TABLE [dbo].[integrateLog_list] WITH CHECK ADD CONSTRAINT [FK_integrateLog_list_integrateLogType_list] FOREIGN KEY([type]) REFERENCES [dbo].[integrateLogType_list] ([id]);
ALTER TABLE [dbo].[integrateLog_list] CHECK CONSTRAINT [FK_integrateLog_list_integrateLogType_list];
GO

ALTER TABLE [dbo].[integrateLog_list] WITH CHECK ADD CONSTRAINT [FK_integrateLog_list_Literature] FOREIGN KEY([literature_id]) REFERENCES [dbo].[Literature] ([id]);
ALTER TABLE [dbo].[integrateLog_list] CHECK CONSTRAINT [FK_integrateLog_list_Literature];
GO

ALTER TABLE [dbo].[integrateLog_list] WITH CHECK ADD CONSTRAINT [FK_integrateLog_list_user_list] FOREIGN KEY([user_id]) REFERENCES [dbo].[user_list] ([id]);
ALTER TABLE [dbo].[integrateLog_list] CHECK CONSTRAINT [FK_integrateLog_list_user_list];
GO

ALTER TABLE [dbo].[Literature] WITH CHECK ADD CONSTRAINT [FK_Literature_LiteratureCategory] FOREIGN KEY([category_id]) REFERENCES [dbo].[LiteratureCategory] ([id]);
ALTER TABLE [dbo].[Literature] CHECK CONSTRAINT [FK_Literature_LiteratureCategory];
GO

ALTER TABLE [dbo].[Literature] WITH CHECK ADD CONSTRAINT [FK_Literature_LiteratureImportBatch] FOREIGN KEY([import_batch_id]) REFERENCES [dbo].[LiteratureImportBatch] ([id]);
ALTER TABLE [dbo].[Literature] CHECK CONSTRAINT [FK_Literature_LiteratureImportBatch];
GO

ALTER TABLE [dbo].[Literature] WITH CHECK ADD CONSTRAINT [FK_Literature_reviewed_by_admin] FOREIGN KEY([reviewed_by]) REFERENCES [dbo].[admin] ([id]);
ALTER TABLE [dbo].[Literature] CHECK CONSTRAINT [FK_Literature_reviewed_by_admin];
GO

ALTER TABLE [dbo].[Literature] WITH CHECK ADD CONSTRAINT [FK_Literature_user_list] FOREIGN KEY([userid]) REFERENCES [dbo].[user_list] ([id]);
ALTER TABLE [dbo].[Literature] CHECK CONSTRAINT [FK_Literature_user_list];
GO

ALTER TABLE [dbo].[LiteratureAuthorMap] WITH CHECK ADD CONSTRAINT [FK_LiteratureAuthorMap_Author] FOREIGN KEY([author_id]) REFERENCES [dbo].[Author] ([id]);
ALTER TABLE [dbo].[LiteratureAuthorMap] CHECK CONSTRAINT [FK_LiteratureAuthorMap_Author];
GO

ALTER TABLE [dbo].[LiteratureAuthorMap] WITH CHECK ADD CONSTRAINT [FK_LiteratureAuthorMap_Literature] FOREIGN KEY([literature_id]) REFERENCES [dbo].[Literature] ([id]);
ALTER TABLE [dbo].[LiteratureAuthorMap] CHECK CONSTRAINT [FK_LiteratureAuthorMap_Literature];
GO

ALTER TABLE [dbo].[LiteratureCategory] WITH CHECK ADD CONSTRAINT [FK_LiteratureCategory_Parent] FOREIGN KEY([parent_id]) REFERENCES [dbo].[LiteratureCategory] ([id]);
ALTER TABLE [dbo].[LiteratureCategory] CHECK CONSTRAINT [FK_LiteratureCategory_Parent];
GO

ALTER TABLE [dbo].[LiteratureDownloadLog] WITH CHECK ADD CONSTRAINT [FK_LiteratureDownloadLog_Literature] FOREIGN KEY([literature_id]) REFERENCES [dbo].[Literature] ([id]);
ALTER TABLE [dbo].[LiteratureDownloadLog] CHECK CONSTRAINT [FK_LiteratureDownloadLog_Literature];
GO

ALTER TABLE [dbo].[LiteratureDownloadLog] WITH CHECK ADD CONSTRAINT [FK_LiteratureDownloadLog_uploader_user_list] FOREIGN KEY([literature_user_id]) REFERENCES [dbo].[user_list] ([id]);
ALTER TABLE [dbo].[LiteratureDownloadLog] CHECK CONSTRAINT [FK_LiteratureDownloadLog_uploader_user_list];
GO

ALTER TABLE [dbo].[LiteratureDownloadLog] WITH CHECK ADD CONSTRAINT [FK_LiteratureDownloadLog_user_list] FOREIGN KEY([user_id]) REFERENCES [dbo].[user_list] ([id]);
ALTER TABLE [dbo].[LiteratureDownloadLog] CHECK CONSTRAINT [FK_LiteratureDownloadLog_user_list];
GO

ALTER TABLE [dbo].[LiteratureExportLog] WITH CHECK ADD CONSTRAINT [FK_LiteratureExportLog_user_list] FOREIGN KEY([userid]) REFERENCES [dbo].[user_list] ([id]);
ALTER TABLE [dbo].[LiteratureExportLog] CHECK CONSTRAINT [FK_LiteratureExportLog_user_list];
GO

ALTER TABLE [dbo].[LiteratureFavorite] WITH CHECK ADD CONSTRAINT [FK_LiteratureFavorite_Literature] FOREIGN KEY([literature_id]) REFERENCES [dbo].[Literature] ([id]);
ALTER TABLE [dbo].[LiteratureFavorite] CHECK CONSTRAINT [FK_LiteratureFavorite_Literature];
GO

ALTER TABLE [dbo].[LiteratureFavorite] WITH CHECK ADD CONSTRAINT [FK_LiteratureFavorite_user_list] FOREIGN KEY([userid]) REFERENCES [dbo].[user_list] ([id]);
ALTER TABLE [dbo].[LiteratureFavorite] CHECK CONSTRAINT [FK_LiteratureFavorite_user_list];
GO

ALTER TABLE [dbo].[LiteratureFile] WITH CHECK ADD CONSTRAINT [FK_LiteratureFile_Literature] FOREIGN KEY([literature_id]) REFERENCES [dbo].[Literature] ([id]);
ALTER TABLE [dbo].[LiteratureFile] CHECK CONSTRAINT [FK_LiteratureFile_Literature];
GO

ALTER TABLE [dbo].[LiteratureImportBatch] WITH CHECK ADD CONSTRAINT [FK_LiteratureImportBatch_user_list] FOREIGN KEY([userid]) REFERENCES [dbo].[user_list] ([id]);
ALTER TABLE [dbo].[LiteratureImportBatch] CHECK CONSTRAINT [FK_LiteratureImportBatch_user_list];
GO

ALTER TABLE [dbo].[LiteratureImportError] WITH CHECK ADD CONSTRAINT [FK_LiteratureImportError_LiteratureImportBatch] FOREIGN KEY([batch_id]) REFERENCES [dbo].[LiteratureImportBatch] ([id]);
ALTER TABLE [dbo].[LiteratureImportError] CHECK CONSTRAINT [FK_LiteratureImportError_LiteratureImportBatch];
GO

ALTER TABLE [dbo].[LiteratureTagMap] WITH CHECK ADD CONSTRAINT [FK_LiteratureTagMap_Literature] FOREIGN KEY([literature_id]) REFERENCES [dbo].[Literature] ([id]);
ALTER TABLE [dbo].[LiteratureTagMap] CHECK CONSTRAINT [FK_LiteratureTagMap_Literature];
GO

ALTER TABLE [dbo].[LiteratureTagMap] WITH CHECK ADD CONSTRAINT [FK_LiteratureTagMap_LiteratureTag] FOREIGN KEY([tag_id]) REFERENCES [dbo].[LiteratureTag] ([id]);
ALTER TABLE [dbo].[LiteratureTagMap] CHECK CONSTRAINT [FK_LiteratureTagMap_LiteratureTag];
GO

ALTER TABLE [dbo].[NoticeLog_List] WITH CHECK ADD CONSTRAINT [FK_NoticeLog_List_NoticeLogStatus_List] FOREIGN KEY([status]) REFERENCES [dbo].[NoticeLogStatus_List] ([id]);
ALTER TABLE [dbo].[NoticeLog_List] CHECK CONSTRAINT [FK_NoticeLog_List_NoticeLogStatus_List];
GO

ALTER TABLE [dbo].[NoticeLog_List] WITH CHECK ADD CONSTRAINT [FK_NoticeLog_List_NoticeLogType_List] FOREIGN KEY([type]) REFERENCES [dbo].[NoticeLogType_List] ([id]);
ALTER TABLE [dbo].[NoticeLog_List] CHECK CONSTRAINT [FK_NoticeLog_List_NoticeLogType_List];
GO

ALTER TABLE [dbo].[NoticeLog_List] WITH CHECK ADD CONSTRAINT [FK_NoticeLog_List_user_list] FOREIGN KEY([userid]) REFERENCES [dbo].[user_list] ([id]);
ALTER TABLE [dbo].[NoticeLog_List] CHECK CONSTRAINT [FK_NoticeLog_List_user_list];
GO

ALTER TABLE [dbo].[ServiceLog_List] WITH CHECK ADD CONSTRAINT [FK_ServiceLog_List_ServiceLogStatus_List] FOREIGN KEY([status]) REFERENCES [dbo].[ServiceLogStatus_List] ([id]);
ALTER TABLE [dbo].[ServiceLog_List] CHECK CONSTRAINT [FK_ServiceLog_List_ServiceLogStatus_List];
GO

ALTER TABLE [dbo].[ServiceLogInfo_List] WITH CHECK ADD CONSTRAINT [FK_ServiceLogInfo_List_ServiceLog_List] FOREIGN KEY([ServiceLog_Id]) REFERENCES [dbo].[ServiceLog_List] ([id]);
ALTER TABLE [dbo].[ServiceLogInfo_List] CHECK CONSTRAINT [FK_ServiceLogInfo_List_ServiceLog_List];
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO


CREATE PROCEDURE dbo.PROC_CLEAN_SOFT_DELETED_DATA
    @RetentionDays int = 0
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @cutoff datetime;
    SET @cutoff = DATEADD(day, -1 * ISNULL(@RetentionDays, 0), GETDATE());

    DECLARE @DeletedLiterature TABLE(id int PRIMARY KEY);
    INSERT INTO @DeletedLiterature(id)
    SELECT id
    FROM dbo.Literature
    WHERE status = -1 AND ISNULL(updatetime, addtime) <= @cutoff;

    DECLARE @DeletedJournal TABLE(id int PRIMARY KEY);
    INSERT INTO @DeletedJournal(id)
    SELECT id FROM dbo.Journal WHERE status = -1 AND ISNULL(updatetime, addtime) <= @cutoff;

    DECLARE @DeletedConference TABLE(id int PRIMARY KEY);
    INSERT INTO @DeletedConference(id)
    SELECT id FROM dbo.Conference WHERE status = -1 AND ISNULL(updatetime, addtime) <= @cutoff;

    DECLARE @DeletedInstitution TABLE(id int PRIMARY KEY);
    INSERT INTO @DeletedInstitution(id)
    SELECT id FROM dbo.Institution WHERE status = -1 AND ISNULL(updatetime, addtime) <= @cutoff;

    BEGIN TRANSACTION;

    UPDATE dbo.integrateLog_list SET literature_id = NULL WHERE literature_id IN (SELECT id FROM @DeletedLiterature);
    DELETE FROM dbo.LiteratureTagMap WHERE literature_id IN (SELECT id FROM @DeletedLiterature);
    DELETE FROM dbo.LiteratureFavorite WHERE literature_id IN (SELECT id FROM @DeletedLiterature);
    DELETE FROM dbo.LiteratureLike WHERE literature_id IN (SELECT id FROM @DeletedLiterature);
    DELETE FROM dbo.LiteratureDownloadLog WHERE literature_id IN (SELECT id FROM @DeletedLiterature);
    DELETE FROM dbo.LiteratureFile WHERE literature_id IN (SELECT id FROM @DeletedLiterature);
    DELETE FROM dbo.LiteratureAuthorInstitutionMap WHERE literature_id IN (SELECT id FROM @DeletedLiterature);
    DELETE FROM dbo.LiteratureAuthorMap WHERE literature_id IN (SELECT id FROM @DeletedLiterature);
    DELETE FROM dbo.AuthorInstitutionHistory WHERE source_literature_id IN (SELECT id FROM @DeletedLiterature);
    ;WITH CommentTree AS
    (
        SELECT id
        FROM dbo.LiteratureComment
        WHERE literature_id IN (SELECT id FROM @DeletedLiterature)
        UNION ALL
        SELECT child.id
        FROM dbo.LiteratureComment child
        INNER JOIN CommentTree parent ON parent.id = child.parent_id
    )
    DELETE FROM dbo.LiteratureComment
    WHERE id IN (SELECT id FROM CommentTree)
    OPTION (MAXRECURSION 0);
    DELETE FROM dbo.Literature WHERE id IN (SELECT id FROM @DeletedLiterature);

    UPDATE dbo.Literature SET journal_id = NULL WHERE journal_id IN (SELECT id FROM @DeletedJournal);
    DELETE FROM dbo.Journal WHERE id IN (SELECT id FROM @DeletedJournal);

    UPDATE dbo.Literature SET conference_id = NULL WHERE conference_id IN (SELECT id FROM @DeletedConference);
    DELETE FROM dbo.Conference WHERE id IN (SELECT id FROM @DeletedConference);

    UPDATE dbo.Institution SET parent_id = NULL WHERE parent_id IN (SELECT id FROM @DeletedInstitution);
    UPDATE dbo.Author
    SET current_institution_id = NULL,
        current_institution_name = NULL,
        current_institution_literature_id = NULL,
        current_institution_sort_date = NULL,
        current_institution_precision = N'unknown',
        updatetime = GETDATE()
    WHERE current_institution_id IN (SELECT id FROM @DeletedInstitution);
    UPDATE dbo.AuthorInstitutionHistory SET institution_id = NULL, updatetime = GETDATE() WHERE institution_id IN (SELECT id FROM @DeletedInstitution);
    DELETE FROM dbo.LiteratureAuthorInstitutionMap WHERE institution_id IN (SELECT id FROM @DeletedInstitution);
    DELETE FROM dbo.InstitutionAlias WHERE institution_id IN (SELECT id FROM @DeletedInstitution);
    DELETE FROM dbo.Institution WHERE id IN (SELECT id FROM @DeletedInstitution);

    COMMIT TRANSACTION;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO


CREATE PROCEDURE dbo.PROCE_SQL2005PAGECHANGE
    @TableName nvarchar(500),
    @ReFieldsStr nvarchar(500) = N'*',
    @OrderString nvarchar(500),
    @WhereString nvarchar(1000) = N'',
    @PageSize int,
    @PageIndex int = 1,
    @TotalRecord int OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF @PageIndex < 1 SET @PageIndex = 1;
    IF @PageSize < 1 SET @PageSize = 20;

    DECLARE @whereSql nvarchar(max);
    DECLARE @sql nvarchar(max);
    DECLARE @countSql nvarchar(max);
    DECLARE @startRow int;
    DECLARE @endRow int;

    SET @whereSql = CASE WHEN LTRIM(RTRIM(ISNULL(@WhereString, N''))) = N'' THEN N'' ELSE N' WHERE ' + @WhereString END;
    SET @countSql = N'SELECT @TotalRecord = COUNT(1) FROM ' + @TableName + @whereSql;
    EXEC sp_executesql @countSql, N'@TotalRecord int OUTPUT', @TotalRecord OUTPUT;

    SET @startRow = (@PageIndex - 1) * @PageSize + 1;
    SET @endRow = @PageIndex * @PageSize;
    SET @sql = N'
SELECT ' + @ReFieldsStr + N'
FROM
(
    SELECT ROW_NUMBER() OVER(ORDER BY ' + @OrderString + N') AS row_no, ' + @ReFieldsStr + N'
    FROM ' + @TableName + @whereSql + N'
) page_data
WHERE row_no BETWEEN ' + CAST(@startRow AS nvarchar(20)) + N' AND ' + CAST(@endRow AS nvarchar(20)) + N'
ORDER BY row_no';
    EXEC (@sql);
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO


CREATE PROCEDURE dbo.SP_PageList
    @tblName varchar(255),
    @strGetFields varchar(2000) = '*',
    @fldName varchar(500) = 'id',
    @PageSize int = 20,
    @PageIndex int = 1,
    @strWhere varchar(4000) = '',
    @distinct varchar(50) = '',
    @doCount bit = 0
AS
BEGIN
    SET NOCOUNT ON;
    IF @PageIndex < 1 SET @PageIndex = 1;
    IF @PageSize < 1 SET @PageSize = 20;

    DECLARE @whereSql nvarchar(max);
    DECLARE @sql nvarchar(max);
    DECLARE @startRow int;
    DECLARE @endRow int;
    SET @whereSql = CASE WHEN LTRIM(RTRIM(ISNULL(@strWhere, ''))) = '' THEN N'' ELSE N' WHERE ' + CONVERT(nvarchar(max), @strWhere) END;

    IF @doCount = 1
    BEGIN
        SET @sql = N'SELECT COUNT(1) AS TotalCount FROM ' + CONVERT(nvarchar(max), @tblName) + @whereSql;
        EXEC (@sql);
        RETURN;
    END;

    SET @startRow = (@PageIndex - 1) * @PageSize + 1;
    SET @endRow = @PageIndex * @PageSize;
    SET @sql = N'
SELECT ' + CONVERT(nvarchar(max), @strGetFields) + N'
FROM
(
    SELECT ROW_NUMBER() OVER(ORDER BY ' + CONVERT(nvarchar(max), @fldName) + N') AS row_no, ' + CONVERT(nvarchar(max), @strGetFields) + N'
    FROM ' + CONVERT(nvarchar(max), @tblName) + @whereSql + N'
) page_data
WHERE row_no BETWEEN ' + CAST(@startRow AS nvarchar(20)) + N' AND ' + CAST(@endRow AS nvarchar(20)) + N'
ORDER BY row_no';
    EXEC (@sql);
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO


CREATE TRIGGER dbo.TR_Conference_Name_Log
ON dbo.Conference
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.MasterNameChangeLog(entity_type, entity_id, old_name_cn, new_name_cn, old_name_en, new_name_en, old_normalized_name, new_normalized_name, addtime)
    SELECT N'Conference', i.id, d.name_cn, i.name_cn, d.name_en, i.name_en, d.normalized_name, i.normalized_name, GETDATE()
    FROM inserted i
    INNER JOIN deleted d ON d.id = i.id
    WHERE ISNULL(i.name_cn,N'')<>ISNULL(d.name_cn,N'')
       OR ISNULL(i.name_en,N'')<>ISNULL(d.name_en,N'')
       OR ISNULL(i.normalized_name,N'')<>ISNULL(d.normalized_name,N'');
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO


CREATE TRIGGER dbo.TR_Institution_Name_Log
ON dbo.Institution
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.MasterNameChangeLog(entity_type, entity_id, old_name_cn, new_name_cn, old_name_en, new_name_en, old_normalized_name, new_normalized_name, addtime)
    SELECT N'Institution', i.id, d.name_cn, i.name_cn, d.name_en, i.name_en, d.normalized_name, i.normalized_name, GETDATE()
    FROM inserted i
    INNER JOIN deleted d ON d.id = i.id
    WHERE ISNULL(i.name_cn,N'')<>ISNULL(d.name_cn,N'')
       OR ISNULL(i.name_en,N'')<>ISNULL(d.name_en,N'')
       OR ISNULL(i.normalized_name,N'')<>ISNULL(d.normalized_name,N'');
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO


CREATE TRIGGER dbo.TR_Journal_Name_Log
ON dbo.Journal
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.MasterNameChangeLog(entity_type, entity_id, old_name_cn, new_name_cn, old_name_en, new_name_en, old_normalized_name, new_normalized_name, addtime)
    SELECT N'Journal', i.id, d.name_cn, i.name_cn, d.name_en, i.name_en, d.normalized_name, i.normalized_name, GETDATE()
    FROM inserted i
    INNER JOIN deleted d ON d.id = i.id
    WHERE ISNULL(i.name_cn,N'')<>ISNULL(d.name_cn,N'')
       OR ISNULL(i.name_en,N'')<>ISNULL(d.name_en,N'')
       OR ISNULL(i.normalized_name,N'')<>ISNULL(d.normalized_name,N'');
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO


CREATE TRIGGER dbo.TR_Literature_Status_Log
ON dbo.Literature
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.LiteratureStatusLog(literature_id, old_status, new_status, reviewed_by, review_time, remark, addtime)
    SELECT i.id, d.status, i.status, i.reviewed_by, i.review_time, i.remark, GETDATE()
    FROM inserted i
    INNER JOIN deleted d ON d.id = i.id
    WHERE ISNULL(i.status, -999) <> ISNULL(d.status, -999);
END;
GO

