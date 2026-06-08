using System;

namespace Model
{
    public class ResourceCommentReplyImg_list
    {
        private long _ResourceCommentReply_Id;
        /// <summary>
        /// 
        /// </summary>
        public long ResourceCommentReply_Id
        {
            set { _ResourceCommentReply_Id = value; }
            get { return _ResourceCommentReply_Id; }
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