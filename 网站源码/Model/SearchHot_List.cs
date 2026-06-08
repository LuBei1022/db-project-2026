using System;

namespace Model
{
    public class SearchHot_List
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
        private DateTime _addtime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime addtime
        {
            set { _addtime = value; }
            get { return _addtime; }
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
        private string _url;
        /// <summary>
        /// 
        /// </summary>
        public string url
        {
            set { _url = value; }
            get { return _url; }
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
        private int _orderid;
        /// <summary>
        /// 
        /// </summary>
        public int orderid
        {
            set { _orderid = value; }
            get { return _orderid; }
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
        private int _num_click;
        /// <summary>
        /// 
        /// </summary>
        public int num_click
        {
            set { _num_click = value; }
            get { return _num_click; }
        }
    }
}
