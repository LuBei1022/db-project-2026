using System;

namespace Model
{
    public class ResourceCommentLike_List
    {
        private long _comment_id;
        /// <summary>
        /// 
        /// </summary>
        public long comment_id
        {
            set { _comment_id = value; }
            get { return _comment_id; }
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
    }
}
