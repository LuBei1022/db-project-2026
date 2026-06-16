using System;

namespace Model
{
    public class LiteratureAuthorInstitutionMap
    {
        public int id { get; set; }
        public int literature_id { get; set; }
        public int author_id { get; set; }
        public int literature_author_map_id { get; set; }
        public int institution_id { get; set; }
        public string affiliation_text { get; set; }
        public int author_order { get; set; }
        public int institution_order { get; set; }
        public int is_current_for_author { get; set; }
        public string source_type { get; set; }
        public int is_confirmed { get; set; }
        public int? confirm_by { get; set; }
        public DateTime? confirm_time { get; set; }
        public DateTime addtime { get; set; }
        public DateTime updatetime { get; set; }
    }
}
