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
        public DateTime addtime { get; set; }
    }
}
