using System;

namespace Model
{
    public class appealimg_list
    {
        private long _appeal_id;
        /// <summary>
        /// 
        /// </summary>
        public long appeal_id
        {
            set { _appeal_id = value; }
            get { return _appeal_id; }
        }
        private string _upload_pic_info;
        /// <summary>
        /// 
        /// </summary>
        public string upload_pic_info
        {
            set { _upload_pic_info = value; }
            get { return _upload_pic_info; }
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
        private int _orderid;
        /// <summary>
        /// 
        /// </summary>
        public int orderid
        {
            set { _orderid = value; }
            get { return _orderid; }
        }
    }
}