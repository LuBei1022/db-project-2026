using System;

namespace Model
{
    public class Author
    {
        public int id { get; set; }
        public string name_cn { get; set; }
        public string name_en { get; set; }
        public string institution { get; set; }
        public int? current_institution_id { get; set; }
        public string current_institution_name { get; set; }
        public int? current_institution_literature_id { get; set; }
        public DateTime? current_institution_sort_date { get; set; }
        public string current_institution_precision { get; set; }
        public string identity_status { get; set; }
        public int? merge_group_id { get; set; }
        public string orcid { get; set; }
        public string email { get; set; }
        public int status { get; set; }
        public DateTime addtime { get; set; }
        public DateTime updatetime { get; set; }
    }
}
