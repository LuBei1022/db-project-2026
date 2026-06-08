using System;

namespace Model
{
    public class Author
    {
        public int id { get; set; }
        public string name_cn { get; set; }
        public string name_en { get; set; }
        public string institution { get; set; }
        public string orcid { get; set; }
        public string email { get; set; }
        public int status { get; set; }
        public DateTime addtime { get; set; }
    }
}
