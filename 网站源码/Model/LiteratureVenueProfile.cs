using System;

namespace Model
{
    public class LiteratureVenueProfile
    {
        public int id { get; set; }
        public string venue_type { get; set; }
        public string venue_name { get; set; }
        public string introduction { get; set; }
        public string impact_factor { get; set; }
        public string jcr_quartile { get; set; }
        public string issn { get; set; }
        public string conference_level { get; set; }
        public string conference_cycle { get; set; }
        public string location { get; set; }
        public string website_url { get; set; }
        public string publisher { get; set; }
        public string remark { get; set; }
        public int status { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public DateTime addtime { get; set; }
        public DateTime updatetime { get; set; }
    }
}
