using System;

namespace Model
{
    public class LiteratureAuthorMap
    {
        public int id { get; set; }
        public int literature_id { get; set; }
        public int author_id { get; set; }
        public int author_order { get; set; }
        public int is_corresponding { get; set; }
        public string affiliation_text { get; set; }
        public string raw_author_text { get; set; }
        public string display_author_name { get; set; }
        public string author_name_raw { get; set; }
        public int is_confirmed { get; set; }
        public int? confirm_by { get; set; }
        public DateTime? confirm_time { get; set; }
        public decimal? identity_confidence { get; set; }
        public DateTime addtime { get; set; }
    }
}
