using System;

namespace Model
{
    public class Institution
    {
        public int id { get; set; }
        public int? parent_id { get; set; }
        public string name_cn { get; set; }
        public string name_en { get; set; }
        public string normalized_name { get; set; }
        public string alias_names { get; set; }
        public string country { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string website { get; set; }
        public int status { get; set; }
        public DateTime addtime { get; set; }
        public DateTime updatetime { get; set; }
    }
}
