using LiteratureManager.Common;
using BLL;
using Model;
using System;
namespace Web.admin
{
    public partial class class_menu : System.Web.UI.UserControl
    {
        BLLBase<popedom> popedombll = new BLLBase<popedom>();
        public string R_Menu = string.Empty;
        public string R_MenuStr = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            popedom popedom = popedombll.SelectSingle("id=" + Function.ConvertTo<int>(Function.GetRequest("MenuId"), 0) + " and id in(" + Cookie.GetCookie("LMS_Popedom") + ")");
            if (popedom != null && popedom.id > 0)
            {
                R_Menu = Function.HtmlDiscode(popedom.popedom_name);
                if (popedom.popedom_father > 0)
                {
                    popedom popedom_p = popedombll.SelectSingle("id=" + popedom.popedom_father + " and id in(" + Cookie.GetCookie("LMS_Popedom") + ")");
                    if (popedom_p != null && popedom_p.id > 0)
                    {
                        R_MenuStr = "<li class=\"breadcrumb-item\"><a>" + Function.HtmlDiscode(popedom_p.popedom_name) + "</a></li><li class=\"breadcrumb-item active\" aria-current=\"page\">" + R_Menu + "</li>";
                    }
                }
            }
            else
            {

            }
        }
    }
}