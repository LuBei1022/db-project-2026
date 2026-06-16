using System;

namespace Model
{
    public class AuthorInstitutionHistory
    {
        public int id { get; set; }
        public int author_id { get; set; }
        public int? institution_id { get; set; }
        public string institution_name { get; set; }
        public int is_current { get; set; }
        public int? start_year { get; set; }
        public int? end_year { get; set; }
        public int? source_literature_id { get; set; }
        public string source_type { get; set; }
        public string remark { get; set; }
        public int status { get; set; }
        public DateTime addtime { get; set; }
        public DateTime updatetime { get; set; }
    }
}
