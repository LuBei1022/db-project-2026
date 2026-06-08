using System;

namespace Model
{
    public class NoticeLog_List
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
        private string _info_;
        /// <summary>
        /// 
        /// </summary>
        public string info_
        {
            set { _info_ = value; }
            get { return _info_; }
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

        private DateTime _addtime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime addtime
        {
            set { _addtime = value; }
            get { return _addtime; }
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
        private string _looktime;
        /// <summary>
        /// 
        /// </summary>
        public string looktime
        {
            set { _looktime = value; }
            get { return _looktime; }
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
        private string _url;
        /// <summary>
        /// 
        /// </summary>
        public string url
        {
            set { _url = value; }
            get { return _url; }
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
    }
}
