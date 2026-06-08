using LiteratureManager.Common;
using System;

namespace Web.admin
{
    public partial class Inc : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            Function.IsSelfRefer();

            //判断权限
            string MenuId = "";
            MenuId = Request.QueryString["MenuId"];
        }
    }
}