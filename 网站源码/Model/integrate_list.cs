using System;

namespace Model
{
    public class integrate_list
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
        private DateTime _addtime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime addtime
        {
            set { _addtime = value; }
            get { return _addtime; }
        }
        private string _upload_pic_img;
        /// <summary>
        /// 
        /// </summary>
        public string upload_pic_img
        {
            set { _upload_pic_img = value; }
            get { return _upload_pic_img; }
        }
        private string _about_;
        /// <summary>
        /// 
        /// </summary>
        public string about_
        {
            set { _about_ = value; }
            get { return _about_; }
        }
        private int _num_integrate;
        /// <summary>
        /// 
        /// </summary>
        public int num_integrate
        {
            set { _num_integrate = value; }
            get { return _num_integrate; }
        }
    }
}
