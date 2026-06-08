using System;

namespace Model
{
    public class ResourceFormatTag_List
    {

        private int _Id;
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            set { _Id = value; }
            get { return _Id; }
        }
        private string _Name;
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            set { _Name = value; }
            get { return _Name; }
        }
        private int _OrderId;
        /// <summary>
        /// 
        /// </summary>
        public int OrderId
        {
            set { _OrderId = value; }
            get { return _OrderId; }
        }
        private DateTime _UpTime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime UpTime
        {
            set { _UpTime = value; }
            get { return _UpTime; }
        }
        private DateTime _AddTime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime AddTime
        {
            set { _AddTime = value; }
            get { return _AddTime; }
        }
        private string _Upload_Pic_Img;
        /// <summary>
        /// 
        /// </summary>
        public string Upload_Pic_Img
        {
            set { _Upload_Pic_Img = value; }
            get { return _Upload_Pic_Img; }
        }
    }
}
