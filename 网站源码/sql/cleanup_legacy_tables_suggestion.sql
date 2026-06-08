/*
  旧业务表清理建议脚本。本文件只记录建议，不执行 DROP。
  所有旧表处理必须先完成备份、引用复扫、测试环境验证和人工确认。
*/

-- 高风险：当前仍被用户中心、通知、日志或审核使用，不能删除。
-- ServiceLog_List
-- ServiceLogInfo_List
-- NoticeLog_List
-- integrateLog_list
-- integrateExchangeLog_list
-- userpaylog_list
-- userpayloginfo_list

-- 中风险：旧 AI / 3D / Workflow 业务残留表，只能在确认旧页面和接口已隔离后迁移到归档库或保留只读备份。
-- GeneratedAssetRecord_list
-- WorkflowTaskComment_list
-- WorkflowTaskCommentImage_list
-- WorkflowTaskLog_list
-- WorkflowTaskReaction_list
-- WorkflowTaskReply_list
-- WorkflowTaskReplyImage_list
-- PromptTemplate_list
-- RenderStyle_list
-- IntegrationToken_list
-- OptionColor_list
-- OptionMaterial_list

-- 中风险：Resource 旧资源模块相关表在当前项目 schema 中不完整，但 Model 和旧代码仍有残留。
-- Resource_list
-- ResourceData_list
-- ResourceComment_list
-- ResourceLike_List
-- ResourceCollect_List
-- ResourceDownloadLog_List
-- ResourceSearchLog_list
-- 以及 ResourceClass/ResourceData/ResourceHome/ResourceMetadata/ResourceFormat 相关表

-- 低风险候选：前置报告中标记“全项目无引用”的旧 Model/表名，可在备份和测试确认后列入删除候选。
-- 本交付版不在 SQL 中执行 DROP，避免破坏线上历史数据。
