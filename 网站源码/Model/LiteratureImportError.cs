using System;

namespace Model
{
    public class LiteratureImportError
    {
        public int id { get; set; }
        public int batch_id { get; set; }
        public int row_no { get; set; }
        public string title { get; set; }
        public string error_msg { get; set; }
        public string raw_data { get; set; }
        public DateTime addtime { get; set; }
    }
}
