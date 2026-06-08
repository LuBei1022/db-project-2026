using System;

namespace Model
{
    public class link_list
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
        private int _isshow;
        /// <summary>
        /// 
        /// </summary>
        public int isshow
        {
            set { _isshow = value; }
            get { return _isshow; }
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
        private int _orderid;
        /// <summary>
        /// 
        /// </summary>
        public int orderid
        {
            set { _orderid = value; }
            get { return _orderid; }
        }
        private string _url;
        /// <summary>
        /// 
        /// </summary>
        public string url
        {
            set { _url = value; }
            get { return _url; }
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
        private string _upload_pic_icon;
        /// <summary>
        /// 
        /// </summary>
        public string upload_pic_icon
        {
            set { _upload_pic_icon = value; }
            get { return _upload_pic_icon; }
        }
    }
}
