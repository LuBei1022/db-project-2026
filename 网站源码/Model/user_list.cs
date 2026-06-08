using System;

namespace Model
{
    public class user_list
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
        private string _name;
        /// <summary>
        /// 
        /// </summary>
        public string name
        {
            set { _name = value; }
            get { return _name; }
        }
        private string _tel;
        /// <summary>
        /// 
        /// </summary>
        public string tel
        {
            set { _tel = value; }
            get { return _tel; }
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
        private DateTime _uptime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime uptime
        {
            set { _uptime = value; }
            get { return _uptime; }
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
        private string _logintime;
        /// <summary>
        /// 
        /// </summary>
        public string logintime
        {
            set { _logintime = value; }
            get { return _logintime; }
        }
        private string _loginip;
        /// <summary>
        /// 
        /// </summary>
        public string loginip
        {
            set { _loginip = value; }
            get { return _loginip; }
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
        private string _email;
        /// <summary>
        /// 
        /// </summary>
        public string email
        {
            set { _email = value; }
            get { return _email; }
        }
        private string _upload_pic_avatar;
        /// <summary>
        /// 
        /// </summary>
        public string upload_pic_avatar
        {
            set { _upload_pic_avatar = value; }
            get { return _upload_pic_avatar; }
        }
    }
}
