using System;

namespace Model
{
    public class LiteratureComment
    {
        public int id { get; set; }
        public int literature_id { get; set; }
        public int? canonical_literature_id { get; set; }
        public int userid { get; set; }
        public int parent_id { get; set; }
        public string content { get; set; }
        public int status { get; set; }
        public int like_count { get; set; }
        public int report_count { get; set; }
        public int is_deleted { get; set; }
        public DateTime? delete_time { get; set; }
        public int? reviewed_by { get; set; }
        public DateTime? review_time { get; set; }
        public string review_remark { get; set; }
        public int? source_service_log_id { get; set; }
        public int? source_service_log_info_id { get; set; }
        public DateTime addtime { get; set; }
        public DateTime updatetime { get; set; }
    }
}
