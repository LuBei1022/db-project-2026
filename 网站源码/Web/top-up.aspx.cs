using System;

namespace Web
{
    public partial class top_up : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("/User/IntegrateLog", true);
        }
    }
}
