using System;

namespace Model
{
    public class ServiceLog_List
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
        private int _status;
        /// <summary>
        /// 
        /// </summary>
        public int status
        {
            set { _status = value; }
            get { return _status; }
        }
        private int _userid;
        /// <summary>
        /// 
        /// </summary>
        public int userid
        {
            set { _userid = value; }
            get { return _userid; }
        }
        private DateTime _uptime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime uptime
        {
            set { _uptime = value; }
            get { return _uptime; }
        }
        private string _looktime;
        /// <summary>
        /// 
        /// </summary>
        public string looktime
        {
            set { _looktime = value; }
            get { return _looktime; }
        }

    }
}
