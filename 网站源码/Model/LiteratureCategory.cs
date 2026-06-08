using System;

namespace Model
{
    public class LiteratureCategory
    {
        public int id { get; set; }
        public int? parent_id { get; set; }
        public string name { get; set; }
        public string name_en { get; set; }
        public string code { get; set; }
        public int orderid { get; set; }
        public int status { get; set; }
        public DateTime addtime { get; set; }
        public DateTime updatetime { get; set; }
    }
}
