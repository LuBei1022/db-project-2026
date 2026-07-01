namespace LiteratureManager.Common
{
    public static class LiteratureStatus
    {
        public const int Deleted = -1;
        public const int PendingReview = 0;
        public const int Published = 1;
        public const int Rejected = 2;
        public const int DuplicateMerged = 3;
        public const int MetadataApplied = 4;

        public static bool IsPublic(int status)
        {
            return status == Published;
        }

        public static bool IsReviewQueueStatus(int status)
        {
            return status == PendingReview || status == MetadataApplied;
        }

        public static string GetText(int status)
        {
            switch (status)
            {
                case Deleted:
                    return "已删除";
                case PendingReview:
                    return "待审核";
                case Published:
                    return "审核通过";
                case Rejected:
                    return "审核驳回";
                case DuplicateMerged:
                    return "重复投稿已合并";
                case MetadataApplied:
                    return "元数据修改已应用";
                default:
                    return "未知状态";
            }
        }
    }
}
