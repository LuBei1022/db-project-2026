using LiteratureManager.Common;
using System;

namespace Web.admin
{
    public partial class Admin_ServerInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();

        }
    }
}