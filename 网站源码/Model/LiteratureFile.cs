using System;

namespace Model
{
    public class LiteratureFile
    {
        public int id { get; set; }
        public int literature_id { get; set; }
        public string file_type { get; set; }
        public string file_name { get; set; }
        public string file_path { get; set; }
        public long? file_size { get; set; }
        public string mime_type { get; set; }
        public int orderid { get; set; }
        public int status { get; set; }
        public DateTime addtime { get; set; }
    }
}
