/*
  学术文献管理系统 - 新增专用文献评论表
  执行时机：部署最终交付版代码前。
  状态约定：0=待审核，1=审核通过，2=驳回，3=软删除。
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.LiteratureComment', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LiteratureComment
    (
        id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_LiteratureComment PRIMARY KEY,
        literature_id int NOT NULL,
        canonical_literature_id int NULL,
        userid int NOT NULL,
        parent_id int NOT NULL CONSTRAINT DF_LiteratureComment_parent_id DEFAULT(0),
        content nvarchar(max) NOT NULL,
        status int NOT NULL CONSTRAINT DF_LiteratureComment_status DEFAULT(0),
        like_count int NOT NULL CONSTRAINT DF_LiteratureComment_like_count DEFAULT(0),
        report_count int NOT NULL CONSTRAINT DF_LiteratureComment_report_count DEFAULT(0),
        is_deleted int NOT NULL CONSTRAINT DF_LiteratureComment_is_deleted DEFAULT(0),
        delete_time datetime NULL,
        reviewed_by int NULL,
        review_time datetime NULL,
        review_remark nvarchar(500) NULL,
        source_service_log_id int NULL,
        source_service_log_info_id int NULL,
        addtime datetime NOT NULL CONSTRAINT DF_LiteratureComment_addtime DEFAULT(GETDATE()),
        updatetime datetime NOT NULL CONSTRAINT DF_LiteratureComment_updatetime DEFAULT(GETDATE())
    );
END;

IF COL_LENGTH('dbo.LiteratureComment', 'source_service_log_info_id') IS NULL
BEGIN
    ALTER TABLE dbo.LiteratureComment ADD source_service_log_info_id int NULL;
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_LiteratureComment_Literature_Status' AND object_id=OBJECT_ID(N'dbo.LiteratureComment'))
BEGIN
    CREATE INDEX IX_LiteratureComment_Literature_Status
    ON dbo.LiteratureComment(canonical_literature_id, literature_id, parent_id, status, is_deleted, addtime DESC);
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_LiteratureComment_User' AND object_id=OBJECT_ID(N'dbo.LiteratureComment'))
BEGIN
    CREATE INDEX IX_LiteratureComment_User
    ON dbo.LiteratureComment(userid, is_deleted, status, addtime DESC);
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'UX_LiteratureComment_SourceServiceLog' AND object_id=OBJECT_ID(N'dbo.LiteratureComment'))
BEGIN
    CREATE UNIQUE INDEX UX_LiteratureComment_SourceServiceLog
    ON dbo.LiteratureComment(source_service_log_id)
    WHERE source_service_log_id IS NOT NULL AND source_service_log_info_id IS NULL;
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'UX_LiteratureComment_SourceServiceLogInfo' AND object_id=OBJECT_ID(N'dbo.LiteratureComment'))
BEGIN
    CREATE UNIQUE INDEX UX_LiteratureComment_SourceServiceLogInfo
    ON dbo.LiteratureComment(source_service_log_info_id)
    WHERE source_service_log_info_id IS NOT NULL;
END;

COMMIT TRANSACTION;
