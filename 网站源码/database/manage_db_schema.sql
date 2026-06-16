/* Generated from manage_db_final */

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
    [updatetime] datetime NOT NULL DEFAULT (getdate())
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

CREATE TABLE [dbo].[IntegrationToken_list] (
    [id] int NOT NULL,
    [adddate] datetime NOT NULL,
    [access_token] nvarchar(250) NOT NULL,
    [expires_in] int NOT NULL
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
    [updatetime] datetime NOT NULL DEFAULT (getdate())
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

CREATE TABLE [dbo].[model_list] (
    [id] int IDENTITY(1,1) NOT NULL,
    [m_name] nvarchar(255) NULL,
    [m_url] nvarchar(255) NULL,
    [page_url] nvarchar(255) NULL,
    [orderid] int NULL,
    [addtime] datetime NULL,
    [upload_pic] nvarchar(50) NULL
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

CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128) NOT NULL,
    [principal_id] int NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int NULL,
    [definition] varbinary(MAX) NULL
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


