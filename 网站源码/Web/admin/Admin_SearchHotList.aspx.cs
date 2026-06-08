using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;

namespace Web.admin
{
    public partial class Admin_SearchHotList : System.Web.UI.Page
    {
        BLLBase<SearchHot_List> SearchHot_ListBll = new BLLBase<SearchHot_List>();
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
            Txt_Title.Text = "<font color=\"red\">添加文献热搜关键词</font>";
        }

        protected void EditFunc()
        {
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            SearchHot_List SearchHot_List = SearchHot_ListBll.SelectSingle("Id=" + ID);
            if (SearchHot_List != null && SearchHot_List.id > 0)
            {
                AddUp.Visible = true;
                Main.Visible = false;
                Txt_Title.Text = "<font color=\"red\">文献热搜关键词详情</font>";

                if (!string.IsNullOrWhiteSpace(SearchHot_List.name))
                {
                    Name.Text = Function.HtmlDiscode(SearchHot_List.name);
                }
                if (!string.IsNullOrWhiteSpace(SearchHot_List.url))
                {
                    url.Text = Function.HtmlDiscode(SearchHot_List.url);
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
                BackURL = "Admin_SearchHotList.aspx?MenuId=" + MenuId;
            }

            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            SearchHot_List SearchHot_List = SearchHot_ListBll.SelectSingle("id=" + ID);
            if (SearchHot_List != null && SearchHot_List.id > 0)
            {
                AddUp.Visible = false;
                Main.Visible = false;
                if (SearchHot_ListBll.Delete("Id", ID))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "文献热搜关键词《" + Function.HtmlDiscode(SearchHot_List.name) + "》删除成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "文献热搜关键词《" + Function.HtmlDiscode(SearchHot_List.name) + "》删除失败!", BackURL, 2);
                }
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "获取删除的参数错误!", BackURL, 1);
            }

        }

        /// <summary>
        /// 绑定文献热搜关键词
        /// </summary>
        protected void BindData()
        {
            string Condition = " 1=1";

            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "SearchHot_List";
            //需要返回的列
            string strGetFields = " RANK()  OVER (order by orderid asc,uptime asc,Id asc) AS xuhao,Id,Name,uptime,url,isshow,orderid,num_click";
            //排序的字段名
            string fldname = "orderid desc,uptime desc,Id desc";
            //每页显示的记录数

            AspNetPager1.PageSize = 15;
            int page_Size = this.AspNetPager1.PageSize;
            //统计总记录数
            int intRecordCount = SearchHot_ListBll.GetCount(tblName, strWhere);
            if (intRecordCount > 0)
            {
                DivNull.Visible = false;
            }
            DataTable dt = SearchHot_ListBll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, strWhere);
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
                BackURL = "Admin_SearchHotList.aspx?MenuId=" + MenuId;
            }
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            SearchHot_List SearchHot_List = new SearchHot_List();
            if (Action == "Edit")
            {
                SearchHot_List = SearchHot_ListBll.SelectSingle("Id=" + ID);
                if (!(SearchHot_List != null && SearchHot_List.id > 0))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "请求参数错误！", BackURL, 2);
                }
            }
            SearchHot_List.name = Function.HtmlEncode(Function.FormRequest("Name"));
            SearchHot_List.url = Function.HtmlEncode(Function.FormRequest("url"));
            SearchHot_List.uptime = DateTime.Now;


            if (Action == "Add")
            {
                AddUp.Visible = false;
                int orderint = 0;
                DataTable orderdt = SearchHot_ListBll.GetDatatable("select max(orderid) as num from SearchHot_List where 1=1");
                if (orderdt != null && orderdt.Rows.Count > 0)
                {
                    orderint = Function.ConvertTo<int>(orderdt.Rows[0]["num"].ToString(), 0);
                }
                orderdt.Dispose();
                orderint++;
                SearchHot_List.orderid = orderint;
                SearchHot_List.isshow = 1;
                SearchHot_List.addtime = DateTime.Now;
                if (SearchHot_ListBll.Add(SearchHot_List, "Id") > 0)
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "文献热搜关键词《<font color=\"red\">" + Function.HtmlDiscode(SearchHot_List.name) + "</font>》 添加成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "文献热搜关键词《<font color=\"red\">" + Function.HtmlDiscode(SearchHot_List.name) + "</font>》 添加失败!", BackURL, 2);

                }
            }
            else if (Action == "Edit")
            {
                AddUp.Visible = false;
                string[] file = { "id" };
                if (SearchHot_ListBll.Update(file, SearchHot_List))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "文献热搜关键词《<font color=\"red\">" + Function.HtmlDiscode(SearchHot_List.name) + "</font>》 修改成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "文献热搜关键词《<font color=\"red\">" + Function.HtmlDiscode(SearchHot_List.name) + "</font>》 修改失败!", BackURL, 2);
                }
            }
        }

        protected void OnClick_Search(object sender, EventArgs e)
        {
            Response.Redirect(Request.CurrentExecutionFilePath + "?SearchKeyWords=" + Server.UrlEncode(Function.FormRequest("SearchKeyWords")) + "&MenuId=" + MenuId);
        }
    }

}
