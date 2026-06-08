namespace Model
{
    /// <summary>
    /// 门户模板配置。
    /// 字段名需要与数据库列保持一致，通用 DAL 会按属性名生成 SQL。
    /// </summary>
    public class model_list
    {
        private int _id;
        public int id
        {
            set { _id = value; }
            get { return _id; }
        }

        private string _m_name;
        public string m_name
        {
            set { _m_name = value; }
            get { return _m_name; }
        }

        private string _m_url;
        public string m_url
        {
            set { _m_url = value; }
            get { return _m_url; }
        }

        private string _page_url;
        public string page_url
        {
            set { _page_url = value; }
            get { return _page_url; }
        }

        private int _orderid;
        public int orderid
        {
            set { _orderid = value; }
            get { return _orderid; }
        }

        private string _upload_pic;
        public string upload_pic
        {
            set { _upload_pic = value; }
            get { return _upload_pic; }
        }
    }
}
