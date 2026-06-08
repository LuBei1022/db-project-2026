using System;

namespace Model
{
    public class ResourceCommentImg_list
    {
        private long _ResourceComment_Id;
        /// <summary>
        /// 
        /// </summary>
        public long ResourceComment_Id
        {
            set { _ResourceComment_Id = value; }
            get { return _ResourceComment_Id; }
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