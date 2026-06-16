using System;

namespace Model
{
    public class InstitutionAlias
    {
        public int id { get; set; }
        public int institution_id { get; set; }
        public string alias_name { get; set; }
        public string normalized_alias { get; set; }
        public string language { get; set; }
        public int status { get; set; }
        public DateTime addtime { get; set; }
    }
}
