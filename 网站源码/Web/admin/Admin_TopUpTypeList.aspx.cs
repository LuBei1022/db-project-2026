using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;

namespace Web.admin
{
    public partial class Admin_TopUpTypeList : System.Web.UI.Page
    {
        BLLBase<TopUpType_List> TopUpType_ListBll = new BLLBase<TopUpType_List>();
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
            Txt_Title.Text = "<font color=\"red\">添加充值类型</font>";
        }

        protected void EditFunc()
        {
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            TopUpType_List TopUpType_List = TopUpType_ListBll.SelectSingle("Id=" + ID);
            if (TopUpType_List != null && TopUpType_List.id > 0)
            {
                AddUp.Visible = true;
                Main.Visible = false;
                Txt_Title.Text = "<font color=\"red\">充值类型详情</font>";

                money.Text = TopUpType_List.money.ToString();
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
                BackURL = "Admin_TopUpTypeList.aspx?MenuId=" + MenuId;
            }

            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            TopUpType_List TopUpType_List = TopUpType_ListBll.SelectSingle("id=" + ID);
            if (TopUpType_List != null)
            {
                AddUp.Visible = false;
                Main.Visible = false;
                if (TopUpType_ListBll.Delete("Id", ID))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "充值类型《" + TopUpType_List.money + "》删除成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "充值类型《" + TopUpType_List.money + "》删除失败!", BackURL, 2);
                }
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "获取删除的参数错误!", BackURL, 1);
            }

        }

        /// <summary>
        /// 绑定充值类型
        /// </summary>
        protected void BindData()
        {
            string Condition = " 1=1";

            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "TopUpType_List";
            //需要返回的列
            string strGetFields = " Id,money,IsShow";
            //排序的字段名
            string fldname = "money asc,Id desc";
            //每页显示的记录数

            AspNetPager1.PageSize = 15;
            int page_Size = this.AspNetPager1.PageSize;
            //统计总记录数
            int intRecordCount = TopUpType_ListBll.GetCount(tblName, strWhere);
            if (intRecordCount > 0)
            {
                DivNull.Visible = false;
            }
            DataTable dt = TopUpType_ListBll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, strWhere);
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
                BackURL = "Admin_TopUpTypeList.aspx?MenuId=" + MenuId;
            }
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            TopUpType_List TopUpType_List = new TopUpType_List();
            if (Action == "Edit")
            {
                TopUpType_List = TopUpType_ListBll.SelectSingle("Id=" + ID);
                if (!(TopUpType_List != null))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "请求参数错误！", BackURL, 2);
                }
            }
            TopUpType_List.money = Function.ConvertTo<int>(Function.FormRequest("money"), 0);


            if (Action == "Add")
            {
                AddUp.Visible = false;
                TopUpType_List.isshow = 1;
                if (TopUpType_ListBll.Add(TopUpType_List, "Id") > 0)
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "充值类型《<font color=\"red\">" + TopUpType_List.money + "</font>》 添加成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "充值类型《<font color=\"red\">" + TopUpType_List.money + "</font>》 添加失败!", BackURL, 2);

                }
            }
            else if (Action == "Edit")
            {
                AddUp.Visible = false;
                string[] file = { "id" };
                if (TopUpType_ListBll.Update(file, TopUpType_List))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "充值类型《<font color=\"red\">" + TopUpType_List.money + "</font>》 修改成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "充值类型《<font color=\"red\">" + TopUpType_List.money + "</font>》 修改失败!", BackURL, 2);
                }
            }
        }

        protected void OnClick_Search(object sender, EventArgs e)
        {
            Response.Redirect(Request.CurrentExecutionFilePath + "?SearchKeyWords=" + Server.UrlEncode(Function.FormRequest("SearchKeyWords")) + "&MenuId=" + MenuId);
        }
    }

}