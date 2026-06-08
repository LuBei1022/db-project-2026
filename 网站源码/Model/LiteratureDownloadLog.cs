using System;

namespace Model
{
    public class LiteratureDownloadLog
    {
        public int id { get; set; }
        public int literature_id { get; set; }
        public int user_id { get; set; }
        public string literature_title { get; set; }
        public string file_url { get; set; }
        public int download_points { get; set; }
        public int literature_user_id { get; set; }
        public DateTime addtime { get; set; }
    }
}
