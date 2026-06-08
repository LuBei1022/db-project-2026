using System;

namespace Model
{
    public class userfile_list
    {
        private int _userid;
        /// <summary>
        /// 
        /// </summary>
        public int userid
        {
            set { _userid = value; }
            get { return _userid; }
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
        private string _up_filename;
        /// <summary>
        /// 
        /// </summary>
        public string up_filename
        {
            set { _up_filename = value; }
            get { return _up_filename; }
        }
    }
}
