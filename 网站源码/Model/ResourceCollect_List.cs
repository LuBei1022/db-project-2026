using System;

namespace Model
{
    public class ResourceCollect_List
    {
        private long _resource_id;
        /// <summary>
        /// 
        /// </summary>
        public long resource_id
        {
            set { _resource_id = value; }
            get { return _resource_id; }
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
