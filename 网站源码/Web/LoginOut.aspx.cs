using LiteratureManager.Common;
using System;

namespace Web
{
    public partial class LoginOut : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Cookie.ClearCookie("user_id");
            Cookie.ClearCookie("user_tel");
            Response.Redirect("/");
        }
    }
}