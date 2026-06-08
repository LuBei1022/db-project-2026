using LiteratureManager.Common;
using BLL;
using Model;
using System;
namespace Web.admin
{
    public partial class Admin_SiteInfo : System.Web.UI.Page
    {
        BLLBase<popedom> popedombll = new BLLBase<popedom>();
        public string MenuId = Function.GetRequest("MenuId");
        public bool isLoading = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Function.Check_AdminLogin();
                isLoading = true;
            }
        }


        public string GetUrl(string ishead)
        {
            string url = "";
            if (ishead != "" && ishead != null)
            {
                popedom popedom = popedombll.SelectSingle("id", Function.ConvertTo<int>(ishead, 0));
                if (popedom != null && popedom.id > 0)
                {
                    if (popedom.popedom_url != null && popedom.popedom_url.IndexOf("?") != -1)
                    {
                        url = popedom.popedom_url + "&MenuId=" + popedom.id;
                    }
                    else
                    {
                        url = popedom.popedom_url + "?MenuId=" + popedom.id;
                    }
                }
            }
            return url;
        }
        public string GetStr(string str)
        {
            if (str == "0")
            {
                return " class='navon'";
            }
            else
            {
                return "";
            }
        }

    }

}