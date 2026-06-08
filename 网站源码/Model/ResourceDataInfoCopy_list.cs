using System;

namespace Model
{
    public class ResourceDataInfoCopy_list
    {
        private long _id;
        /// <summary>
        /// 
        /// </summary>
        public long id
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
        private string _upload_pic_cover;
        /// <summary>
        /// 
        /// </summary>
        public string upload_pic_cover
        {
            set { _upload_pic_cover = value; }
            get { return _upload_pic_cover; }
        }
        private long? _ResourceDataCopy_id;
        /// <summary>
        /// 
        /// </summary>
        public long? ResourceDataCopy_id
        {
            set { _ResourceDataCopy_id = value; }
            get { return _ResourceDataCopy_id; }
        }
        private decimal _time_h;
        /// <summary>
        /// 
        /// </summary>
        public decimal time_h
        {
            set { _time_h = value; }
            get { return _time_h; }
        }
        private decimal _diameter;
        /// <summary>
        /// 
        /// </summary>
        public decimal diameter
        {
            set { _diameter = value; }
            get { return _diameter; }
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
        private int _status;
        /// <summary>
        /// 
        /// </summary>
        public int status
        {
            set { _status = value; }
            get { return _status; }
        }
        private int _user_id;
        /// <summary>
        /// 
        /// </summary>
        public int user_id
        {
            set { _user_id = value; }
            get { return _user_id; }
        }
    }
}