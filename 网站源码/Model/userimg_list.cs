using System;

namespace Model
{
    public class userimg_list
    {
        private int _userid;
        /// <summary>
        /// 
        /// </summary>
        public int userid
        {
            set { _userid = value; }
            get { return _userid; }
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
    }
}
