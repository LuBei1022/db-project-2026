using System;

namespace Model
{
    public class Literature
    {
        public int id { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public string institution { get; set; }
        public string doi { get; set; }
        public string keywords { get; set; }
        public string abstract_text { get; set; }
        public string source_type { get; set; }
        public string language { get; set; }
        public int? publish_year { get; set; }
        public int? publish_month { get; set; }
        public int? publish_day { get; set; }
        public DateTime? publish_date { get; set; }
        public string publish_date_precision { get; set; }
        public string journal_name { get; set; }
        public int? journal_id { get; set; }
        public string conference_name { get; set; }
        public int? conference_id { get; set; }
        public string publisher { get; set; }
        public string volume { get; set; }
        public string issue { get; set; }
        public string pages { get; set; }
        public int category_id { get; set; }
        public string cover_pic { get; set; }
        public int download_points { get; set; }
        public string external_url { get; set; }
        public string source_db { get; set; }
        public string remark { get; set; }
        public int is_top { get; set; }
        public int status { get; set; }
        public int userid { get; set; }
        public int? reviewed_by { get; set; }
        public DateTime? review_time { get; set; }
        public int? import_batch_id { get; set; }
        public int? canonical_literature_id { get; set; }
        public DateTime addtime { get; set; }
        public DateTime updatetime { get; set; }
    }
}
