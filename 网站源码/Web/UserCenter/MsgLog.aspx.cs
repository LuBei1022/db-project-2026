using LiteratureManager.Common;
using Model;
using System;

namespace Web.UserCenter
{
    public partial class MsgLog : System.Web.UI.Page
    {
        public user_list user_list = new user_list();
        public int NoticeCount = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    user_list = CommonUserFunc.GetUserLoginStatus();
                    if (user_list != null && user_list.id > 0)
                    {
                        NoticeCount = CommonUserFunc.GetNoticeLogNum(user_list.id);
                    }
                    else
                    {
                        Response.Redirect("/");
                    }
                }
                catch (Exception ex)
                {
                    ImportDataLog.WriteLog(LogType.Error, "MsgLog.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
                }
            }
        }
    }
}
