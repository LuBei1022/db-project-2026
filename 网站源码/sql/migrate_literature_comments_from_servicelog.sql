/*
  将旧 ServiceLog_List 中的文献评论迁移到 LiteratureComment。
  可重复执行：source_service_log_id/source_service_log_info_id 已建立唯一索引，脚本也有 NOT EXISTS 去重。
  执行前请先备份 ServiceLog_List、ServiceLogInfo_List、LiteratureComment。
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.LiteratureComment', N'U') IS NULL
BEGIN
    RAISERROR(N'请先执行 sql/upgrade_add_literature_comment.sql。', 16, 1);
    ROLLBACK TRANSACTION;
    RETURN;
END;

;WITH SourceRows AS
(
    SELECT
        s.id,
        s.info_,
        s.addtime,
        s.uptime,
        s.status,
        s.userid,
        REPLACE(s.info_, N'/LiteratureInfo.aspx?ID=', N'/LiteratureInfo.aspx?id=') AS normalized_info
    FROM dbo.ServiceLog_List s
    WHERE s.name LIKE N'[[]文献评论]%'
      AND s.info_ LIKE N'%/LiteratureInfo.aspx%'
),
Parsed AS
(
    SELECT
        sr.*,
        CHARINDEX(N'/LiteratureInfo.aspx?id=', sr.normalized_info) AS link_pos,
        CHARINDEX(N'评论内容：', sr.normalized_info) AS content_pos
    FROM SourceRows sr
),
ParsedId AS
(
    SELECT
        p.*,
        CASE WHEN p.link_pos > 0 THEN p.link_pos + LEN(N'/LiteratureInfo.aspx?id=') ELSE 0 END AS id_start
    FROM Parsed p
),
ParsedContent AS
(
    SELECT
        p.*,
        TRY_CONVERT(int, LEFT(SUBSTRING(p.normalized_info, p.id_start, 32), PATINDEX(N'%[^0-9]%', SUBSTRING(p.normalized_info, p.id_start, 32) + N'X') - 1)) AS parsed_literature_id,
        CASE
            WHEN p.content_pos > 0 THEN SUBSTRING(p.normalized_info, p.content_pos + LEN(N'评论内容：'), LEN(p.normalized_info))
            ELSE p.info_
        END AS parsed_content
    FROM ParsedId p
    WHERE p.id_start > 0
)
INSERT INTO dbo.LiteratureComment
(
    literature_id,
    canonical_literature_id,
    userid,
    parent_id,
    content,
    status,
    like_count,
    report_count,
    is_deleted,
    delete_time,
    reviewed_by,
    review_time,
    review_remark,
    source_service_log_id,
    source_service_log_info_id,
    addtime,
    updatetime
)
SELECT
    pc.parsed_literature_id,
    COALESCE(NULLIF(l.canonical_literature_id, 0), pc.parsed_literature_id),
    pc.userid,
    0,
    COALESCE(NULLIF(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(pc.parsed_content, N'<br/>', CHAR(13)+CHAR(10)), N'<br />', CHAR(13)+CHAR(10)), N'<br>', CHAR(13)+CHAR(10)), N'</br>', CHAR(13)+CHAR(10)))), N''), N'[旧评论内容解析失败，请查看 source_service_log_id]'),
    CASE WHEN pc.status IN (1,2) THEN 1 WHEN pc.status = -1 THEN 3 ELSE 0 END,
    0,
    0,
    CASE WHEN pc.status = -1 THEN 1 ELSE 0 END,
    CASE WHEN pc.status = -1 THEN pc.uptime ELSE NULL END,
    NULL,
    CASE WHEN pc.status IN (1,2) THEN pc.uptime ELSE NULL END,
    N'由 ServiceLog_List 迁移',
    pc.id,
    NULL,
    pc.addtime,
    pc.uptime
FROM ParsedContent pc
LEFT JOIN dbo.Literature l ON l.id = pc.parsed_literature_id
WHERE pc.parsed_literature_id IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM dbo.LiteratureComment c WHERE c.source_service_log_id = pc.id AND c.source_service_log_info_id IS NULL);

INSERT INTO dbo.LiteratureComment
(
    literature_id,
    canonical_literature_id,
    userid,
    parent_id,
    content,
    status,
    like_count,
    report_count,
    is_deleted,
    delete_time,
    reviewed_by,
    review_time,
    review_remark,
    source_service_log_id,
    source_service_log_info_id,
    addtime,
    updatetime
)
SELECT
    parent.literature_id,
    parent.canonical_literature_id,
    0,
    parent.id,
    COALESCE(NULLIF(sli.info_, N''), N'[旧管理员回复内容为空]'),
    CASE WHEN parent.status = 3 THEN 3 ELSE 1 END,
    0,
    0,
    parent.is_deleted,
    parent.delete_time,
    parent.reviewed_by,
    parent.review_time,
    N'由 ServiceLogInfo_List 管理员回复迁移',
    parent.source_service_log_id,
    sli.id,
    sli.addtime,
    sli.addtime
FROM dbo.ServiceLogInfo_List sli
INNER JOIN dbo.LiteratureComment parent
    ON parent.source_service_log_id = sli.ServiceLog_Id
   AND parent.source_service_log_info_id IS NULL
WHERE sli.type = 2
  AND NOT EXISTS (SELECT 1 FROM dbo.LiteratureComment c WHERE c.source_service_log_info_id = sli.id);

COMMIT TRANSACTION;

SELECT
    SUM(CASE WHEN parent_id=0 THEN 1 ELSE 0 END) AS migrated_comment_count,
    SUM(CASE WHEN parent_id<>0 THEN 1 ELSE 0 END) AS migrated_reply_count
FROM dbo.LiteratureComment
WHERE source_service_log_id IS NOT NULL;
