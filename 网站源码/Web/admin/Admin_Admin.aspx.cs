using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Web.admin
{
    public partial class Admin_Admin : System.Web.UI.Page
    {
        BLLBase<Model.admin> adminbll = new BLLBase<Model.admin>();
        BLLBase<user_login> user_loginbll = new BLLBase<user_login>();
        BLLBase<popedom> popedombll = new BLLBase<popedom>();
        public string MenuId = Function.GetRequest("MenuId");
        string Action = Function.GetRequest("Action");
        public bool isLoading = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                isLoading = true;
                Function.Check_AdminLogin();
                switch (Action)
                {
                    case "Add":
                        AddUserFunc();
                        break;
                    case "EditUser":
                        EditUserFunc();
                        break;
                    case "DelUser":
                        DelUserFunc();
                        break;
                    case "LockUser":
                        LockUserFunc();
                        break;
                    case "EditPopedom":
                        EditPopedomFunc();
                        break;
                    case "UnLockUser":
                        UnLockUserFunc();
                        break;
                    default:
                        BindData();
                        break;
                }
            }
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        protected void BindData()
        {
            string sql = "";
            Main.Visible = true;

            if (Cookie.GetCookie("LMS_AdminName").ToString().ToUpper() == "SYSADMIN")
            {
                sql = "select * from admin";
            }
            else if (Cookie.GetCookie("LMS_AdminName").ToString().ToUpper() == "ADMIN" || Cookie.GetCookie("LMS_AdminName").ToString().ToUpper() == "YILIAN")
            {
                sql = "select * from admin where [username]<>'SYSADMIN'";
            }
            else
            {
                sql = "select * from admin where [username]<>'SYSADMIN' and [username]<>'ADMIN' and [username]<>'YILIAN'";
            }
            DataTable admindt = adminbll.GetDatatable(sql);


            if (admindt != null && admindt.Rows.Count > 0)
            {


                this.myRepeater.DataSource = admindt.DefaultView;
                this.myRepeater.DataBind();

            }

        }


        #region 权限相关函数
        /// <summary>
        /// 绑定数据
        /// </summary>
        protected void Popedom_BindData()
        {

            string sql = "";
            if (Cookie.GetCookie("LMS_AdminName").ToString().ToUpper() == "SYSADMIN" || Cookie.GetCookie("LMS_AdminName").ToString().ToUpper() == "YILIAN" || Cookie.GetCookie("LMS_AdminName").ToString().ToUpper() == "ADMIN")
            {
                sql = "select * from popedom where popedom_father=0 order by orderid asc";
            }
            else
            {
                sql = "select * from popedom where popedom_father=0 and id in(" + Cookie.GetCookie("LMS_Popedom") + ") order by orderid asc";
                //sql = "select * from popedom where popedom_father=0 order by orderid asc";
            }

            Popedom.Visible = true;
            DataTable popedomdt = popedombll.GetDatatable(sql);
            if (popedomdt != null && popedomdt.Rows.Count > 0)
            {
                this.myRepeater1.DataSource = popedomdt.DefaultView;
                this.myRepeater1.DataBind();
            }
            popedomdt.Dispose();
        }


        protected void MenuPopedom_BindData()
        {

        }

        //Repeater显示两列第二种方法
        //int index = 0; // 全局字段 
        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // if (index % 2 == 0 && index > 0)
            // {
            //     e.Item.Controls.Add(new LiteralControl(" </tr> <tr>"));
            // }
            // index++;


            //内部myRepeater2
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                int Id = Function.ConvertTo<int>(((DataRowView)e.Item.DataItem).Row["id"], 0);

                Repeater myRepeater2 = (Repeater)e.Item.FindControl("myRepeater2");

                if (myRepeater2 != null)
                {
                    string SqlGetMinMenu = "";
                    if (Cookie.GetCookie("LMS_AdminName").ToString().ToUpper() == "SYSADMIN" || Cookie.GetCookie("LMS_AdminName").ToString().ToUpper() == "YILIAN" || Cookie.GetCookie("LMS_AdminName").ToString().ToUpper() == "ADMIN")
                    {
                        SqlGetMinMenu = "select * from popedom where popedom_father=" + Id + " order by orderid asc";
                    }
                    else
                    {
                        //SqlGetMinMenu = "select * from popedom where popedom_father=" + Id + " order by orderid asc";
                        SqlGetMinMenu = "select * from popedom where popedom_father=" + Id + " and id in(" + Cookie.GetCookie("LMS_Popedom") + ") order by orderid asc";
                    }

                    DataTable popedomdt = popedombll.GetDatatable(SqlGetMinMenu);
                    if (popedomdt != null && popedomdt.Rows.Count > 0)
                    {
                        myRepeater2.DataSource = popedomdt.DefaultView;
                        myRepeater2.DataBind();
                    }
                    popedomdt.Dispose();
                }
            }
        }


        protected string GetChecked(string BoxID)
        {
            int CheckBoxID = Convert.ToInt16(BoxID);
            string Popedom_Str = "";
            int AdminID = Function.ConvertTo<int>(Function.GetRequest("AdminID"), 0);
            Model.admin admin = adminbll.SelectSingle("id", AdminID);
            if (admin != null && admin.id > 0)
            {
                if (!string.IsNullOrWhiteSpace(admin.popedom))
                {
                    Popedom_Str = Function.HtmlDiscode(admin.popedom);
                }
            }

            string[] a = Function.ret_Power(Popedom_Str);
            for (int i = 0; i < a.Length; i++)
            {
                if (CheckBoxID.ToString() == a[i].ToString())
                {
                    return ("checked");
                }
            }
            return "";
        }

        protected void OnClick_Popedom(object sender, EventArgs e)
        {
            isLoading = false;
            Popedom.Visible = false;
            string PopedomStr = "";
            int AdminID = Function.ConvertTo<int>(Function.GetRequest("AdminID"), 0);
            DataTable popedomdt = popedombll.GetDatatable("select * from popedom where popedom_father = 0");
            if (popedomdt != null && popedomdt.Rows.Count > 0)
            {
                foreach (DataRow item in popedomdt.Rows)
                {
                    string pstr = Function.FormRequest("checkbox" + item["id"]);
                    if (!string.IsNullOrWhiteSpace(pstr.Replace(",", "")))
                    {
                        if (string.IsNullOrWhiteSpace(PopedomStr))
                        {
                            PopedomStr = PopedomStr + pstr;
                        }
                        else
                        {
                            PopedomStr = PopedomStr + "," + pstr;
                        }

                    }

                }
            }
            adminbll.Update(" popedom ='" + PopedomStr + "'", "id=" + AdminID);
            if (Cookie.GetCookie("LMS_AdminID") != "" && Cookie.GetCookie("LMS_AdminID") != null)
            {
                Model.admin admin = adminbll.SelectSingle("id", Function.ConvertTo<int>(Cookie.GetCookie("LMS_AdminID"), 0));
                if (admin != null && admin.id > 0)
                {
                    Cookie.SaveCookie("LMS_Popedom", admin.popedom, 24);
                }
            }


            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "权限设置成功,点击更新系统缓存才能生效!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 0);
        }
        #endregion




        public string GetString(string UserName, int IsLock, int id)
        {
            string url = "<a class=\"badge text-bg-success\" href='Admin_Admin.aspx?MenuId=" + MenuId + "&Action=EditUser&AdminID=" + id + "'>编辑</a> ";


            url = url + "<a class=\"badge text-bg-primary\" href='Admin_Admin.aspx?MenuId=" + MenuId + "&Action=EditPopedom&AdminID=" + id + "'>编辑权限</a> ";


            if (IsLock == 1)
            {
                url = url + "<a class=\"badge\" href='Admin_Admin.aspx?MenuId=" + MenuId + "&Action=UnLockUser&AdminID=" + id + "' style='color:green'>解锁</a> ";
            }
            else
            {
                url = url + "<a class=\"badge text-bg-warning\" href='Admin_Admin.aspx?MenuId=" + MenuId + "&Action=LockUser&AdminID=" + id + "' style='color:green'>锁定</a> ";
            }

            url = url + "<a class=\"badge text-bg-danger\" data-href='Admin_Admin.aspx?Action=DelUser&MenuID=" + MenuId + "&AdminID=" + id + "' onclick=\"DataDelFunc(this)\">删除</a> ";

            return url;

        }


        protected string GetMenu(string id)
        {
            int child_id = Convert.ToInt16(id);


            return "";

        }


        protected void AddUserFunc()
        {
            AddUser.Visible = true;
            Title.Text = "添加管理员";
            Button3.Text = " 添 加 ";
        }

        protected void EditUserFunc()
        {
            AddUser.Visible = true;
            Title.Text = "修改密码";
            int AdminID = Function.ConvertTo<int>(Function.GetRequest("AdminID"), 0);
            Model.admin admin = adminbll.SelectSingle("id", AdminID);
            if (admin != null && admin.id > 0)
            {
                Admin_Name.Text = Function.HtmlDiscode(admin.username);
                Admin_Name.Enabled = false;
            }
        }


        protected void DelUserFunc()
        {
            isLoading = false;
            AddUser.Visible = false;
            string AdminID = Function.GetRequest("AdminID");

            if (AdminID == Cookie.GetCookie("LMS_AdminID").ToString())
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "失败，不可以删除自己!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 2);
            }
            else
            {
                if (Cookie.GetCookie("LMS_AdminName").ToString().ToUpper() == "SYSADMIN")
                {
                    if (adminbll.Delete("id", AdminID))
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "管理员删除成功!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 0);
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "管理员删除失败!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 2);
                    }
                }
                else
                {
                    if (adminbll.Delete("id=" + AdminID + " and [username]<>'SYSADMIN'"))
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "管理员删除成功!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 0);
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "管理员删除失败!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 2);
                    }
                }


            }
        }


        protected void LockUserFunc()
        {
            isLoading = false;
            AddUser.Visible = false;
            string AdminID = Function.GetRequest("AdminID");
            if (AdminID == Cookie.GetCookie("LMS_AdminID").ToString())
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "失败，不可以锁定自己!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 2);
            }
            else
            {
                if (adminbll.Update("locks=1", "id=" + AdminID))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "成功锁定该用户!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "锁定该用户失败!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 2);
                }

            }
        }


        protected void UnLockUserFunc()
        {
            isLoading = false;
            AddUser.Visible = false;
            string AdminID = Function.GetRequest("AdminID");
            Model.admin admin = adminbll.SelectSingle("id=" + AdminID);
            if (admin != null)
            {
                if (adminbll.Update("locks=0", "id=" + admin.id))
                {
                    try
                    {
                        user_loginbll.Delete("datediff(HOUR,time,getdate())<=3 and username='" + admin.username + "' and content like '%登录失败！ 原因：%'");
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "成功解锁该用户!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "解锁该用户失败!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 2);
                }
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "解锁用户不存在!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 2);
            }


        }


        protected void EditPopedomFunc()
        {
            Popedom.Visible = true;
            Popedom_BindData();
        }


        protected void OnClick_AddUp(object sender, EventArgs e)
        {
            isLoading = false;
            string Admin_Name = Function.FormRequest("Admin_Name");
            string AdminID = Function.GetRequest("AdminID");
            string pwd = Function.FormRequest("Admin_Pwd");
            string pwd1 = Function.FormRequest("Admin_Pwd1");
            string Admin_Pwd = Function.MD5(pwd, 32);


            if (pwd != pwd1)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "" + Admin_Name + "两次密码输入不一致!", "javascript:history.go(-1)", 2);
            }

            if (Action == "Add")
            {
                AddUser.Visible = false;
                Model.admin admin_model = adminbll.SelectSingle("username='" + Admin_Name + "'");
#pragma warning disable CS0472 // 由于此类型的值永不等于 "null"，该表达式的结果始终相同
                if (admin_model != null && admin_model.id != null && admin_model.id > 0)
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "管理员帐号《" + Admin_Name + "》已存在!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 2);
                }
                else
                {

                    Model.admin admin = new Model.admin();
                    admin.username = Admin_Name;
                    admin.password = Admin_Pwd;
                    admin.lastlogindate = DateTime.Now;
                    admin.popedom = Cookie.GetCookie("LMS_Popedom");
                    admin.locks = 0;
                    if (adminbll.Add(admin, "id") > 0)
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "管理员" + Admin_Name + "添加成功!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 0);
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "管理员" + Admin_Name + "添加失败!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 2);
                    }
                }
#pragma warning restore CS0472 // 由于此类型的值永不等于 "null"，该表达式的结果始终相同

            }
            else if (Action == "EditUser")
            {
                if (pwd != "" && pwd != null)
                {
                    AddUser.Visible = false;
                    if (adminbll.Update("[password]='" + Admin_Pwd + "'", "id=" + AdminID))
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "" + Admin_Name + "密码修改成功!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 0);
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "" + Admin_Name + "密码修改失败!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 2);
                    }

                }
                else
                {
                    AddUser.Visible = false;
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "" + Admin_Name + "密码修改失败!", "Admin_Admin.aspx?MenuId=" + MenuId + "", 2);
                }
            }

        }

        public string GetLock(string IsLock)
        {
            string url = "";
            if (IsLock == "1")
            {
                url = " <font color=\"red\">[已锁定]</font>";
            }
            return url;
        }
    }


}