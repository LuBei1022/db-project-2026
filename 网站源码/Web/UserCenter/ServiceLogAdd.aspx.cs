using LiteratureManager.Common;
using BLL;
using Model;
using System;

namespace Web.UserCenter
{
    public partial class ServiceLogAdd : System.Web.UI.Page
    {
        BLLBase<ServiceLog_List> ServiceLog_ListBll = new BLLBase<ServiceLog_List>();
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
                    ImportDataLog.WriteLog(LogType.Error, "ServiceLogAdd.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
                }
            }
        }
        protected void OnClick_AddUp(object sender, EventArgs e)
        {
            Main.Visible = false;
            user_list = CommonUserFunc.GetUserLoginStatus();
            if (user_list != null && user_list.id > 0)
            {
                ServiceLog_List ServiceLog_List = new ServiceLog_List();
                ServiceLog_List.name = Function.HtmlEncode(Function.FormRequest("name"));
                ServiceLog_List.info_ = Function.HtmlSqlEncode(Function.FormRequest("info_"));
                ServiceLog_List.addtime = DateTime.Now;
                ServiceLog_List.uptime = ServiceLog_List.addtime;
                ServiceLog_List.status = 0;
                ServiceLog_List.userid = user_list.id;
                if (ServiceLog_ListBll.Add(ServiceLog_List, "id") > 0)
                {
                    CommonFunc.Ok_Return("反馈已成功提交，我们会尽快查看并回复。", "/User/ServiceLog", 0);
                }
                else
                {
                    CommonFunc.Ok_Return("反馈提交失败，请稍后再试！", "/User/ServiceLog", 2);
                }
            }
            else
            {
                Function.Show_Msg("登录状态异常！", "/");
            }
        }
    }
}
