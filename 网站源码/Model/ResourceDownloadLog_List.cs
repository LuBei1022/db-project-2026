using System;

namespace Model
{
    public class ResourceDownloadLog_List
    {
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
        private string _resource_name;
        /// <summary>
        /// 
        /// </summary>
        public string resource_name
        {
            set { _resource_name = value; }
            get { return _resource_name; }
        }
        private string _resource_data_name;
        /// <summary>
        /// 
        /// </summary>
        public string resource_data_name
        {
            set { _resource_data_name = value; }
            get { return _resource_data_name; }
        }
        private string _file_url;
        /// <summary>
        /// 
        /// </summary>
        public string file_url
        {
            set { _file_url = value; }
            get { return _file_url; }
        }
        private string _file_type;
        /// <summary>
        /// 
        /// </summary>
        public string file_type
        {
            set { _file_type = value; }
            get { return _file_type; }
        }
        private long _resource_id;
        /// <summary>
        /// 
        /// </summary>
        public long resource_id
        {
            set { _resource_id = value; }
            get { return _resource_id; }
        }
        private long _resource_data_id;
        /// <summary>
        /// 
        /// </summary>
        public long resource_data_id
        {
            set { _resource_data_id = value; }
            get { return _resource_data_id; }
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
        private int _resource_user_id;
        /// <summary>
        /// 
        /// </summary>
        public int resource_user_id
        {
            set { _resource_user_id = value; }
            get { return _resource_user_id; }
        }


    }
}