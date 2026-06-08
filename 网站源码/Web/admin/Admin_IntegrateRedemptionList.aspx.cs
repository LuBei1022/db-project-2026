using LiteratureManager.Common;
using BLL;
using Model;
using System;

namespace Web.admin
{
    public partial class Admin_IntegrateRedemptionList : System.Web.UI.Page
    {
        BLLBase<integrateExchangeLog_list> integrateExchangeLog_listbll = new BLLBase<integrateExchangeLog_list>();
        public int MenuId = Function.ConvertTo<int>(Function.GetRequest("MenuId"), 0);
        public bool isLoading = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
        }
        protected void OnClick_AddUp(object sender, EventArgs e)
        {
            isLoading = false;
            string BackURL = Request.QueryString["BackURL"];
            if (string.IsNullOrWhiteSpace(BackURL))
            {
                BackURL = "Admin_IntegrateRedemptionList.aspx?MenuId=" + MenuId;
            }
            string codestr_ = Function.HtmlEncode(Function.FormRequest("codestr"));
            if (!string.IsNullOrWhiteSpace(codestr_))
            {
                integrateExchangeLog_list integrateExchangeLog_list = integrateExchangeLog_listbll.SelectSingle("codestr='" + Function.HtmlEncode(codestr_) + "'");
                if (integrateExchangeLog_list != null && integrateExchangeLog_list.id > 0)
                {
                    if (integrateExchangeLog_list.status == 1)
                    {
                        if (integrateExchangeLog_listbll.Update("status=-1,hexiaotime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'", "id=" + integrateExchangeLog_list.id))
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "下载权益《" + codestr_ + "》使用登记成功!", BackURL, 0);
                        }
                        else
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "下载权益《" + codestr_ + "》使用登记失败，请稍后再试!", BackURL, 2);
                        }
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "该权益码已使用!", BackURL, 2);
                    }
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "权益码错误!", BackURL, 2);
                }
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "权益码不能为空!", BackURL, 2);
            }
        }

    }

}
