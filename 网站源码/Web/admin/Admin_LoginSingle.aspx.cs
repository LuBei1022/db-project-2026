using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;

namespace Web.admin
{
    public partial class Admin_LoginSingle : System.Web.UI.Page
    {
        BLLBase<LoginSingle_List> LoginSingle_ListBll = new BLLBase<LoginSingle_List>();
        string Action = Function.GetRequest("Action");
        public string MenuId = Function.GetRequest("MenuId");
        public bool isLoading = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            switch (Action)
            {
                case "Add":
                    AddFunc();
                    break;
                case "Edit":
                    EditFunc();
                    break;
                case "Del":
                    DelFunc();
                    break;
                default:
                    BindData();
                    break;
            }
        }
        protected void AddFunc()
        {
            AddUp.Visible = true;
            Main.Visible = false;
            Txt_Title.Text = "<font color=\"red\">添加</font>";
        }

        protected void EditFunc()
        {
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            LoginSingle_List LoginSingle_List = LoginSingle_ListBll.SelectSingle("Id=" + ID);
            if (LoginSingle_List != null && LoginSingle_List.Id > 0)
            {
                AddUp.Visible = true;
                Main.Visible = false;
                Txt_Title.Text = "<font color=\"red\">详情</font>";

                if (!string.IsNullOrWhiteSpace(LoginSingle_List.Name))
                {
                    Name.Text = Function.HtmlDiscode(LoginSingle_List.Name);
                }
                if (!string.IsNullOrWhiteSpace(LoginSingle_List.Info_))
                {
                    Info_.Text = Function.HtmlSqlDiscode(LoginSingle_List.Info_);
                }
            }
        }


        protected void DelFunc()
        {
            isLoading = false;
            AddUp.Visible = false;
            Main.Visible = false;
            string BackURL = Request.QueryString["BackURL"];
            if (string.IsNullOrWhiteSpace(BackURL))
            {
                BackURL = "Admin_LoginSingle.aspx?MenuId=" + MenuId;
            }

            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            LoginSingle_List LoginSingle_List = LoginSingle_ListBll.SelectSingle("id=" + ID);
            if (LoginSingle_List != null && LoginSingle_List.Id > 0)
            {
                AddUp.Visible = false;
                Main.Visible = false;
                if (LoginSingle_ListBll.Delete("Id", ID))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "登录注册协议《" + Function.HtmlDiscode(LoginSingle_List.Name) + "》删除成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "登录注册协议《" + Function.HtmlDiscode(LoginSingle_List.Name) + "》删除失败!", BackURL, 2);
                }
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "获取删除的参数错误!", BackURL, 1);
            }

        }

        /// <summary>
        /// 绑定登录注册协议
        /// </summary>
        protected void BindData()
        {
            string Condition = " 1=1";

            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "LoginSingle_List";
            //需要返回的列
            string strGetFields = " RANK()  OVER (order by OrderId asc,UpTime asc,Id asc) AS xuhao,Id,Name,IsShow,UpTime,OrderId";
            //排序的字段名
            string fldname = "OrderId desc,UpTime desc,Id desc";
            //每页显示的记录数

            AspNetPager1.PageSize = 15;
            int page_Size = this.AspNetPager1.PageSize;
            //统计总记录数
            int intRecordCount = LoginSingle_ListBll.GetCount(tblName, strWhere);
            if (intRecordCount > 0)
            {
                DivNull.Visible = false;
            }
            DataTable dt = LoginSingle_ListBll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, strWhere);
            AspNetPager1.RecordCount = intRecordCount;
            AspNetPager1.AlwaysShow = true;
            if (dt != null && dt.Rows.Count > 0)
            {
                this.Repeater1.DataSource = dt.DefaultView;
                this.Repeater1.DataBind();
            }
        }

        protected void AspNetPager1_PageChanged(object src, EventArgs e)
        {
            BindData();
        }

        protected void OnClick_AddUp(object sender, EventArgs e)
        {
            isLoading = false;
            AddUp.Visible = false;
            Main.Visible = false;
            string BackURL = Request.QueryString["BackURL"];
            if (string.IsNullOrWhiteSpace(BackURL))
            {
                BackURL = "Admin_LoginSingle.aspx?MenuId=" + MenuId;
            }
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            LoginSingle_List LoginSingle_List = new LoginSingle_List();
            if (Action == "Edit")
            {
                LoginSingle_List = LoginSingle_ListBll.SelectSingle("Id=" + ID);
                if (!(LoginSingle_List != null && LoginSingle_List.Id > 0))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "请求参数错误！", BackURL, 2);
                }
            }
            LoginSingle_List.Name = Function.HtmlEncode(Function.FormRequest("Name"));
            LoginSingle_List.Info_ = Function.HtmlSqlEncode(Function.FormRequest("Info_"));
            LoginSingle_List.UpTime = DateTime.Now;


            if (Action == "Add")
            {
                AddUp.Visible = false;
                int orderint = 0;
                DataTable orderdt = LoginSingle_ListBll.GetDatatable("select max(orderid) as num from LoginSingle_List where 1=1");
                if (orderdt != null && orderdt.Rows.Count > 0)
                {
                    orderint = Function.ConvertTo<int>(orderdt.Rows[0]["num"].ToString(), 0);
                }
                orderdt.Dispose();
                orderint++;
                LoginSingle_List.OrderId = orderint;
                LoginSingle_List.IsShow = 1;
                LoginSingle_List.AddTime = DateTime.Now;
                if (LoginSingle_ListBll.Add(LoginSingle_List, "Id") > 0)
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "登录注册协议《<font color=\"red\">" + Function.HtmlDiscode(LoginSingle_List.Name) + "</font>》 添加成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "登录注册协议《<font color=\"red\">" + Function.HtmlDiscode(LoginSingle_List.Name) + "</font>》 添加失败!", BackURL, 2);

                }
            }
            else if (Action == "Edit")
            {
                AddUp.Visible = false;
                string[] file = { "Id" };
                if (LoginSingle_ListBll.Update(file, LoginSingle_List))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "登录注册协议《<font color=\"red\">" + Function.HtmlDiscode(LoginSingle_List.Name) + "</font>》 修改成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "登录注册协议《<font color=\"red\">" + Function.HtmlDiscode(LoginSingle_List.Name) + "</font>》 修改失败!", BackURL, 2);
                }
            }
        }

        protected void OnClick_Search(object sender, EventArgs e)
        {
            Response.Redirect(Request.CurrentExecutionFilePath + "?SearchKeyWords=" + Server.UrlEncode(Function.FormRequest("SearchKeyWords")) + "&MenuId=" + MenuId);
        }
    }

}