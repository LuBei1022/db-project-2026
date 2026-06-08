using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class TopUpType_List
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
        private int _money;
        /// <summary>
        /// 
        /// </summary>
        public int money
        {
            set { _money = value; }
            get { return _money; }
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
    }
}
