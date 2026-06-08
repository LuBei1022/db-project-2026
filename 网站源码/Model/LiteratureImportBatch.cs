using System;

namespace Model
{
    public class LiteratureImportBatch
    {
        public int id { get; set; }
        public string batch_name { get; set; }
        public string import_type { get; set; }
        public string file_name { get; set; }
        public int status { get; set; }
        public int total_count { get; set; }
        public int success_count { get; set; }
        public int fail_count { get; set; }
        public int userid { get; set; }
        public DateTime addtime { get; set; }
        public DateTime? finishtime { get; set; }
    }
}
