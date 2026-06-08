using System;

namespace Model
{
    public class ResourceSearchLog_list
    {
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
        private string _keyword;
        /// <summary>
        /// 
        /// </summary>
        public string keyword
        {
            set { _keyword = value; }
            get { return _keyword; }
        }
        private int _isshow;
        /// <summary>
        /// 
        /// </summary>
        public int isshow
        {
            set { _isshow = value; }
            get { return _isshow; }
        }
        private string _codestr;
        /// <summary>
        /// 
        /// </summary>
        public string codestr
        {
            set { _codestr = value; }
            get { return _codestr; }
        }
    }
}
