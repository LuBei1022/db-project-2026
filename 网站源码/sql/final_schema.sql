/*
  最终交付版数据库结构说明。
  完整历史结构基线仍以 database/manage_db_schema.sql 为准；
  本文件列出本次交付新增或必须重点保护的结构。
*/

-- 必须保护的文献核心表：
-- Literature, LiteratureCategory, Author, LiteratureAuthorMap, LiteratureFile,
-- LiteratureTag, LiteratureTagMap, LiteratureLike, LiteratureFavorite,
-- LiteratureDownloadLog, LiteratureExportLog, LiteratureImportBatch,
-- LiteratureImportError, LiteratureVenueProfile

-- 必须保护的用户/后台/积分/通知/日志表：
-- user_list, admin, user_login, telcode_list, integrateLog_list,
-- integrateExchangeLog_list, integrateLogType_list, integrate_list,
-- TopUpType_List, userpaylog_list, userpayloginfo_list,
-- NoticeLog_List, ServiceLog_List, ServiceLogInfo_List

-- 本次新增结构请执行：
-- 1. sql/upgrade_add_literature_comment.sql
-- 2. sql/migrate_literature_comments_from_servicelog.sql（有旧评论数据时执行）

-- 新表结构摘录：
IF OBJECT_ID(N'dbo.LiteratureComment', N'U') IS NULL
BEGIN
    PRINT N'LiteratureComment 尚未创建，请执行 upgrade_add_literature_comment.sql。';
END
ELSE
BEGIN
    EXEC sp_help N'dbo.LiteratureComment';
END;
