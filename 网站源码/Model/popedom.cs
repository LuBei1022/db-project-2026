namespace Model
{
    public class popedom
    {
        private int? _id;
        /// <summary>
        /// 
        /// </summary>
        public int? id
        {
            set { _id = value; }
            get { return _id; }
        }


        private string _popedom_name;
        /// <summary>
        /// 
        /// </summary>
        public string popedom_name
        {
            set { _popedom_name = value; }
            get { return _popedom_name; }
        }
        private int? _popedom__father;
        /// <summary>
        /// 
        /// </summary>
        public int? popedom_father
        {
            set { _popedom__father = value; }
            get { return _popedom__father; }
        }
        private string _popedom_url;
        /// <summary>
        /// 
        /// </summary>
        public string popedom_url
        {
            set { _popedom_url = value; }
            get { return _popedom_url; }
        }



        private int? _orderid;
        /// <summary>
        /// 
        /// </summary>
        public int? orderid
        {
            set { _orderid = value; }
            get { return _orderid; }
        }

        private int? _ishead;
        /// <summary>
        /// 
        /// </summary>
        public int? ishead
        {
            set { _ishead = value; }
            get { return _ishead; }
        }

    }
}
