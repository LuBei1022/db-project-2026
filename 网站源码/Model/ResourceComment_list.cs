using System;

namespace Model
{
    public class ResourceComment_list
    {
        private long _id;
        /// <summary>
        /// 
        /// </summary>
        public long id
        {
            set { _id = value; }
            get { return _id; }
        }
        private long _Resource_id;
        /// <summary>
        /// 
        /// </summary>
        public long Resource_id
        {
            set { _Resource_id = value; }
            get { return _Resource_id; }
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
        private DateTime _addtime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime addtime
        {
            set { _addtime = value; }
            get { return _addtime; }
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
        private int _status;
        /// <summary>
        /// 
        /// </summary>
        public int status
        {
            set { _status = value; }
            get { return _status; }
        }
        private int _num_dianzan;
        /// <summary>
        /// 
        /// </summary>
        public int num_dianzan
        {
            set { _num_dianzan = value; }
            get { return _num_dianzan; }
        }
        private string _reviewtime;
        /// <summary>
        /// 
        /// </summary>
        public string reviewtime
        {
            set { _reviewtime = value; }
            get { return _reviewtime; }
        }
        private string _about_;
        /// <summary>
        /// 
        /// </summary>
        public string about_
        {
            set { _about_ = value; }
            get { return _about_; }
        }
        private int _num_msg;
        /// <summary>
        /// 
        /// </summary>
        public int num_msg
        {
            set { _num_msg = value; }
            get { return _num_msg; }
        }
    }
}