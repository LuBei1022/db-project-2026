using System;

namespace Model
{
    public class Conference
    {
        public int id { get; set; }
        public string name_cn { get; set; }
        public string name_en { get; set; }
        public string acronym { get; set; }
        public string normalized_name { get; set; }
        public string organizer { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string website { get; set; }
        public int status { get; set; }
        public DateTime addtime { get; set; }
        public DateTime updatetime { get; set; }
    }
}
