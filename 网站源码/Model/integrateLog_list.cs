using System;

namespace Model
{
    public class integrateLog_list
    {

        private int _id;
        /// <summary>
        /// 
        /// </summary>
        public int id
        {
            set { _id = value; }
            get { return _id; }
        }
        private int _num_integrate;
        /// <summary>
        /// 
        /// </summary>
        public int num_integrate
        {
            set { _num_integrate = value; }
            get { return _num_integrate; }
        }
        private int _type;
        /// <summary>
        /// 
        /// </summary>
        public int type
        {
            set { _type = value; }
            get { return _type; }
        }
        private string _name;
        /// <summary>
        /// 
        /// </summary>
        public string name
        {
            set { _name = value; }
            get { return _name; }
        }
        private string _info_;
        /// <summary>
        /// 
        /// </summary>
        public string info_
        {
            set { _info_ = value; }
            get { return _info_; }
        }
        private DateTime _addtime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime addtime
        {
            set { _addtime = value; }
            get { return _addtime; }
        }
        private int _user_id;
        /// <summary>
        /// 
        /// </summary>
        public int user_id
        {
            set { _user_id = value; }
            get { return _user_id; }
        }
        private string _adminname;
        /// <summary>
        /// 
        /// </summary>
        public string adminname
        {
            set { _adminname = value; }
            get { return _adminname; }
        }
        private long? _resource_id;
        /// <summary>
        /// 
        /// </summary>
        public long? resource_id
        {
            set { _resource_id = value; }
            get { return _resource_id; }
        }
        private long? _resource_data_id;
        /// <summary>
        /// 
        /// </summary>
        public long? resource_data_id
        {
            set { _resource_data_id = value; }
            get { return _resource_data_id; }
        }

        private int? _literature_id;
        /// <summary>
        /// Normalized literature reference for point flows.
        /// </summary>
        public int? literature_id
        {
            set { _literature_id = value; }
            get { return _literature_id; }
        }

    }
}
