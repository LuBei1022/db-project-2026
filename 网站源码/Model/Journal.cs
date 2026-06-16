using System;

namespace Model
{
    public class Journal
    {
        public int id { get; set; }
        public string name_cn { get; set; }
        public string name_en { get; set; }
        public string normalized_name { get; set; }
        public string issn { get; set; }
        public string eissn { get; set; }
        public string publisher { get; set; }
        public string country { get; set; }
        public string subject { get; set; }
        public string website { get; set; }
        public int status { get; set; }
        public DateTime addtime { get; set; }
        public DateTime updatetime { get; set; }
    }
}
