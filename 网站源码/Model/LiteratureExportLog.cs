using System;

namespace Model
{
    public class LiteratureExportLog
    {
        public int id { get; set; }
        public string export_name { get; set; }
        public string export_type { get; set; }
        public string file_name { get; set; }
        public int record_count { get; set; }
        public int userid { get; set; }
        public DateTime addtime { get; set; }
    }
}
