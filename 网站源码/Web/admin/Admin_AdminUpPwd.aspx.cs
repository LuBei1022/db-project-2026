using LiteratureManager.Common;
using BLL;
using System;

namespace Web.admin
{
    public partial class Admin_AdminUpPwd : System.Web.UI.Page
    {
        public string MenuId = Function.GetRequest("MenuId");
        BLLBase<Model.admin> adminbll = new BLLBase<Model.admin>();
        public bool isLoading = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                isLoading = true;
                Function.Check_AdminLogin();
                EditUserFunc();
            }
        }
        protected void EditUserFunc()
        {
            Title.Text = "修改密码";
            int AdminID = Function.ConvertTo<int>(Cookie.GetCookie("LMS_AdminID"), 0);
            Model.admin admin = adminbll.SelectSingle("id", AdminID);
            if (admin != null && admin.id > 0)
            {
                Admin_Name.Text = Function.HtmlDiscode(admin.username);
                Admin_Name.Enabled = false;
            }
        }
        protected void OnClick_AddUp(object sender, EventArgs e)
        {
            isLoading = false;
            string Admin_Name = Function.FormRequest("Admin_Name");
            string AdminID = Cookie.GetCookie("LMS_AdminID");
            string pwd = Function.FormRequest("Admin_Pwd");
            string pwd1 = Function.FormRequest("Admin_Pwd1");
            string Admin_Pwd = Function.MD5(pwd, 32);
            if (pwd != pwd1)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "" + Admin_Name + "两次密码输入不一致!", "javascript:history.go(-1)", 2);
            }
            else
            {
                if (pwd != "" && pwd != null)
                {
                    if (adminbll.Update("[password]='" + Admin_Pwd + "'", "id=" + AdminID))
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "" + Admin_Name + "密码修改成功!", "Admin_AdminUpPwd.aspx?MenuId=" + MenuId + "", 0);
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "" + Admin_Name + "密码修改失败!", "Admin_AdminUpPwd.aspx?MenuId=" + MenuId + "", 2);
                    }

                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "" + Admin_Name + "密码修改失败!", "Admin_AdminUpPwd.aspx?MenuId=" + MenuId + "", 2);
                }
            }

        }
    }
}