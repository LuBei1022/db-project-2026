using LiteratureManager.Common;
using Model;
using System;

namespace Web.UserCenter
{
    public partial class Appeal : System.Web.UI.Page
    {
        public user_list user_list = new user_list();
        public string defaultUrl = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    defaultUrl = Function.HtmlDiscode(Function.GetRequest("url"));
                    user_list = CommonUserFunc.GetUserLoginStatus();
                    if (user_list != null && user_list.id > 0)
                    {

                    }
                    else
                    {
                        Response.Redirect("/");
                    }
                }
                catch (Exception ex)
                {
                    ImportDataLog.WriteLog(LogType.Error, "Appeal.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
                }
            }
        }
    }
}
