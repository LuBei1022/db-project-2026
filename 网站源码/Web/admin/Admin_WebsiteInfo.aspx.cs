using LiteratureManager.Common;
using BLL;
using Model;
using System;

namespace Web.admin
{
    public partial class Admin_WebsiteInfo : System.Web.UI.Page
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
                Txt_Title.Text = "<font color=\"red\">修改</font>";
                if (!string.IsNullOrWhiteSpace(websiteinfo_list.companyname))
                {
                    companyname.Text = Function.HtmlDiscode(websiteinfo_list.companyname);
                }

                if (!string.IsNullOrWhiteSpace(websiteinfo_list.banquan))
                {
                    banquan.Text = Function.HtmlDiscode(websiteinfo_list.banquan);
                }
            }
        }

        protected void OnClick_AddUp(object sender, EventArgs e)
        {
            isLoading = false;
            string BackURL = Request.QueryString["BackURL"];
            if (string.IsNullOrWhiteSpace(BackURL))
            {
                BackURL = "Admin_WebsiteInfo.aspx?MenuId=" + MenuId;
            }

            websiteinfo_list websiteinfo_list = websiteinfo_listbll.SelectSingle("id=1");
            bool isadd = false;
            if (!(websiteinfo_list != null && websiteinfo_list.id > 0))
            {
                websiteinfo_list = new websiteinfo_list();
                isadd = true;
            }

            websiteinfo_list.banquan = Function.HtmlEncode(Function.FormRequest("banquan"));
            websiteinfo_list.companyname = Function.HtmlEncode(Function.FormRequest("companyname"));

            if (isadd)
            {
                websiteinfo_list.id = 1;
                if (websiteinfo_listbll.Add(websiteinfo_list))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "网站基本信息配置修改成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "网站基本信息修改失败!", BackURL, 2);
                }
            }
            else
            {
                string[] file = { "id" };
                if (websiteinfo_listbll.Update(file, websiteinfo_list))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "网站基本信息修改成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "网站基本信息修改失败!", BackURL, 2);
                }
            }
        }
    }
}
