using System;

namespace Model
{
    public class daoru_list
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
        private DateTime _posttime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime posttime
        {
            set { _posttime = value; }
            get { return _posttime; }
        }

        private string _r_info;
        /// <summary>
        /// 
        /// </summary>
        public string r_info
        {
            set { _r_info = value; }
            get { return _r_info; }
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
        private int _type;
        /// <summary>
        /// 
        /// </summary>
        public int type
        {
            set { _type = value; }
            get { return _type; }
        }
    }
}
