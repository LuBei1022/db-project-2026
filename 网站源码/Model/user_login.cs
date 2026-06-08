using System;

namespace Model
{
    public class user_login
    {
        private int? _id;
        /// <summary>
        /// 
        /// </summary>
        public int? id
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
        private DateTime _time;
        /// <summary>
        /// 
        /// </summary>
        public DateTime time
        {
            set { _time = value; }
            get { return _time; }
        }

        private string _ip;
        /// <summary>
        /// 
        /// </summary>
        public string ip
        {
            set { _ip = value; }
            get { return _ip; }
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

        private int? _type;
        /// <summary>
        /// 
        /// </summary>
        public int? type
        {
            set { _type = value; }
            get { return _type; }
        }
        private string _content;
        /// <summary>
        /// 
        /// </summary>
        public string content
        {
            set { _content = value; }
            get { return _content; }
        }


    }
}
