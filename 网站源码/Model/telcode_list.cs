using System;

namespace Model
{
    public class telcode_list
    {

        private string _tel;
        /// <summary>
        /// 
        /// </summary>
        public string tel
        {
            set { _tel = value; }
            get { return _tel; }
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
        private string _code;
        /// <summary>
        /// 
        /// </summary>
        public string code
        {
            set { _code = value; }
            get { return _code; }
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
        private int _img_x;
        /// <summary>
        /// 
        /// </summary>
        public int img_x
        {
            set { _img_x = value; }
            get { return _img_x; }
        }
        private int _img_y;
        /// <summary>
        /// 
        /// </summary>
        public int img_y
        {
            set { _img_y = value; }
            get { return _img_y; }
        }

    }
}
