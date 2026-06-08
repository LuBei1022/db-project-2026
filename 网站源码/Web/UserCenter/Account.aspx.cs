using LiteratureManager.Common;
using Model;
using System;

namespace Web.UserCenter
{
    public partial class Account : System.Web.UI.Page
    {
        public user_list user_list = new user_list();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
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
                    ImportDataLog.WriteLog(LogType.Error, "Account.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
                }
            }
        }
    }
}