using LiteratureManager.Common;
using BLL;
using Model;
using System;

namespace Web.admin
{
    public partial class Index : System.Web.UI.Page
    {

        public BLLBase<websiteinfo_list> websiteinfo_listbll = new BLLBase<websiteinfo_list>();
        public websiteinfo_list websiteinfo_list = new websiteinfo_list();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Function.Check_AdminLogin();
                websiteinfo_list = websiteinfo_listbll.SelectSingle("id=1");
                if (!(websiteinfo_list != null && websiteinfo_list.id > 0))
                {
                    websiteinfo_list = new websiteinfo_list();
                }
            }
        }
    }
}