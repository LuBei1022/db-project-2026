using System;

namespace Model
{
    public class daoruerr_list
    {

        private string _info;
        /// <summary>
        /// 
        /// </summary>
        public string info
        {
            set { _info = value; }
            get { return _info; }
        }

        private string _filename;
        /// <summary>
        /// 
        /// </summary>
        public string filename
        {
            set { _filename = value; }
            get { return _filename; }
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
        private int _daoruid;
        /// <summary>
        /// 
        /// </summary>
        public int daoruid
        {
            set { _daoruid = value; }
            get { return _daoruid; }
        }
    }
}