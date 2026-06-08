using System;

namespace Model
{
    public class ServiceLogInfo_List
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

        private int _ServiceLog_Id;
        /// <summary>
        /// 
        /// </summary>
        public int ServiceLog_Id
        {
            set { _ServiceLog_Id = value; }
            get { return _ServiceLog_Id; }
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
        private string _adminname;
        /// <summary>
        /// 
        /// </summary>
        public string adminname
        {
            set { _adminname = value; }
            get { return _adminname; }
        }
    }
}