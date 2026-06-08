using System;

namespace Model
{
    public class LiteratureFavorite
    {
        public int id { get; set; }
        public int literature_id { get; set; }
        public int userid { get; set; }
        public DateTime addtime { get; set; }
    }
}
