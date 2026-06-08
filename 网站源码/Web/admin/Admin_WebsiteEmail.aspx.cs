using LiteratureManager.Common;
using BLL;
using Model;
using System;

namespace Web.admin
{
    public partial class Admin_WebsiteEmail : System.Web.UI.Page
    {
        BLLBase<websiteinfo_list> websiteinfo_listbll = new BLLBase<websiteinfo_list>();
        public int MenuId = Function.ConvertTo<int>(Function.GetRequest("MenuId"), 0);
        public bool isLoading = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            EditFunc();
        }


        protected void EditFunc()
        {
            websiteinfo_list websiteinfo_list = websiteinfo_listbll.SelectSingle(" id=1");
            if (websiteinfo_list != null && websiteinfo_list.id > 0)
            {
                Txt_Title.Text = "<font color=\"red\">邮箱推送配置修改</font>";
                if (!string.IsNullOrWhiteSpace(websiteinfo_list.emailname))
                {
                    emailname.Text = Function.HtmlDiscode(websiteinfo_list.emailname);
                }
                if (!string.IsNullOrWhiteSpace(websiteinfo_list.emailnum))
                {
                    emailnum.Text = Function.HtmlDiscode(websiteinfo_list.emailnum);
                }
                if (!string.IsNullOrWhiteSpace(websiteinfo_list.emailpasswd))
                {
                    emailpasswd.Text = Function.HtmlDiscode(websiteinfo_list.emailpasswd);
                }
                if (!string.IsNullOrWhiteSpace(websiteinfo_list.email_to))
                {
                    email_to.Text = Function.HtmlDiscode(websiteinfo_list.email_to);
                }
                if (!string.IsNullOrWhiteSpace(websiteinfo_list.smtpserverport))
                {
                    smtpserverport.Text = Function.HtmlDiscode(websiteinfo_list.smtpserverport);
                }
                if (!string.IsNullOrWhiteSpace(websiteinfo_list.host))
                {
                    host.Text = Function.HtmlDiscode(websiteinfo_list.host);
                }
            }
        }



        protected void OnClick_AddUp(object sender, EventArgs e)
        {
            isLoading = false;
            string BackURL = Request.QueryString["BackURL"];
            if (string.IsNullOrWhiteSpace(BackURL))
            {
                BackURL = "Admin_WebsiteEmail.aspx?MenuId=" + MenuId;
            }
            websiteinfo_list websiteinfo_list = websiteinfo_listbll.SelectSingle("id=1");
            bool isadd = false;
            if (!(websiteinfo_list != null && websiteinfo_list.id > 0))
            {
                websiteinfo_list = new websiteinfo_list();
                isadd = true;
            }
            websiteinfo_list.email_to = Function.HtmlEncode(Function.FormRequest("email_to"));
            websiteinfo_list.emailpasswd = Function.HtmlEncode(Function.FormRequest("emailpasswd"));
            websiteinfo_list.smtpserverport = Function.HtmlEncode(Function.FormRequest("smtpserverport"));
            websiteinfo_list.host = Function.HtmlEncode(Function.FormRequest("host"));
            websiteinfo_list.emailnum = Function.HtmlEncode(Function.FormRequest("emailnum"));
            websiteinfo_list.emailname = Function.HtmlEncode(Function.FormRequest("emailname"));
            if (isadd)
            {
                websiteinfo_list.id = 1;
                if (websiteinfo_listbll.Add(websiteinfo_list))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "邮箱推送配置修改成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "邮箱推送配置修改失败!", BackURL, 2);

                }
            }
            else
            {
                string[] file = { "id" };
                if (websiteinfo_listbll.Update(file, websiteinfo_list))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "邮箱推送配置修改成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "邮箱推送配置修改失败!", BackURL, 2);
                }
            }
        }

    }

}