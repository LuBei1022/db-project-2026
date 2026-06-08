using System;

namespace Model
{
    public class logincode_list
    {
        private string _code;
        /// <summary>
        /// 
        /// </summary>
        public string code
        {
            set { _code = value; }
            get { return _code; }
        }
        private string _val;
        /// <summary>
        /// 
        /// </summary>
        public string val
        {
            set { _val = value; }
            get { return _val; }
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

        private string _ip_str;
        /// <summary>
        /// 
        /// </summary>
        public string ip_str
        {
            set { _ip_str = value; }
            get { return _ip_str; }
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
    }
}
