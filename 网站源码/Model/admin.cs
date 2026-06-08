using System;

namespace Model
{
    public class admin
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


        private string _username;
        /// <summary>
        /// 
        /// </summary>
        public string username
        {
            set { _username = value; }
            get { return _username; }
        }
        private string _password;
        /// <summary>
        /// 
        /// </summary>
        public string password
        {
            set { _password = value; }
            get { return _password; }
        }
        private string _popedom;
        /// <summary>
        /// 
        /// </summary>
        public string popedom
        {
            set { _popedom = value; }
            get { return _popedom; }
        }

        private string _lastloginip;
        /// <summary>
        /// 
        /// </summary>
        public string lastloginip
        {
            set { _lastloginip = value; }
            get { return _lastloginip; }
        }

        private int _cityid;
        /// <summary>
        /// 
        /// </summary>
        public int cityid
        {
            set { _cityid = value; }
            get { return _cityid; }
        }
        private int _locks;
        /// <summary>
        /// 
        /// </summary>
        public int locks
        {
            set { _locks = value; }
            get { return _locks; }
        }
        private string _code;
        /// <summary>
        /// 
        /// </summary>
        public string code
        {
            set { _code = value; }
            get { return _code; }
        }
        private DateTime _lastlogindate;
        /// <summary>
        /// 
        /// </summary>
        public DateTime lastlogindate
        {
            set { _lastlogindate = value; }
            get { return _lastlogindate; }
        }

    }
}
