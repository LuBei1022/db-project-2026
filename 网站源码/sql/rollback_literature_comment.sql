/*
  LiteratureComment 回滚辅助脚本。
  目的：在代码回退到旧 ServiceLog_List 评论实现前，把新表中没有旧来源 ID 的顶级评论回写为服务日志。
  注意：脚本不 DROP LiteratureComment，避免误删新评论数据。
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.LiteratureComment', N'U') IS NULL
BEGIN
    RAISERROR(N'LiteratureComment 表不存在，无需回滚。', 10, 1);
    COMMIT TRANSACTION;
    RETURN;
END;

INSERT INTO dbo.ServiceLog_List(name, info_, addtime, status, userid, uptime)
SELECT
    N'[文献评论] ' + COALESCE(NULLIF(l.title, N''), N'未命名文献'),
    N'文献链接：/LiteratureInfo.aspx?id=' + CAST(COALESCE(c.canonical_literature_id, c.literature_id) AS nvarchar(20))
        + N'<br/>文献标题：' + COALESCE(NULLIF(l.title, N''), N'未命名文献')
        + N'<br/>评论内容：' + c.content,
    c.addtime,
    CASE WHEN c.status=1 THEN 1 WHEN c.status=3 OR c.is_deleted=1 THEN -1 ELSE 0 END,
    c.userid,
    c.updatetime
FROM dbo.LiteratureComment c
LEFT JOIN dbo.Literature l ON l.id = c.literature_id
WHERE c.parent_id=0
  AND c.source_service_log_id IS NULL
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.ServiceLog_List s
      WHERE s.name LIKE N'[[]文献评论]%'
        AND s.userid=c.userid
        AND s.addtime=c.addtime
        AND s.info_ LIKE N'%/LiteratureInfo.aspx?id=' + CAST(COALESCE(c.canonical_literature_id, c.literature_id) AS nvarchar(20)) + N'%'
  );

COMMIT TRANSACTION;

-- 如确需完全移除新表，请在完成备份、代码回退和人工确认后手工执行：
-- DROP TABLE dbo.LiteratureComment;
