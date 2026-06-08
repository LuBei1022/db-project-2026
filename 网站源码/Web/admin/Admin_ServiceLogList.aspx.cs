using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Web.admin
{
    public partial class Admin_ServiceLogList : System.Web.UI.Page
    {
        BLLBase<ServiceLog_List> ServiceLog_ListBll = new BLLBase<ServiceLog_List>();
        BLLBase<ServiceLogStatus_List> ServiceLogStatus_Listbll = new BLLBase<ServiceLogStatus_List>();
        string Action = Function.GetRequest("Action");
        public string MenuId = Function.GetRequest("MenuId");
        public bool isLoading = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            Big_List();
            BindData();
            isLoading = true;
        }
        protected void Big_List()
        {
            SearchStatus.Items.Add(new ListItem("--请选择--", ""));
            DataTable ServiceLogStatus_Listdt = ServiceLogStatus_Listbll.GetDatatable("select id,name from ServiceLogStatus_List order by id asc");
            if (ServiceLogStatus_Listdt != null && ServiceLogStatus_Listdt.Rows.Count > 0)
            {
                foreach (DataRow item in ServiceLogStatus_Listdt.Rows)
                {
                    SearchStatus.Items.Add(new ListItem(Function.HtmlDiscode(item["name"].ToString()), item["id"].ToString()));
                }
            }
            ServiceLogStatus_Listdt.Dispose();
        }
        /// <summary>
        /// 绑定一级分类
        /// </summary>
        protected void BindData()
        {
            string Condition = " 1=1";

            int SearchStatus_ = Function.ConvertTo<int>(Function.GetRequest("SearchStatus"), -1);
            ServiceLogStatus_List ServiceLogStatus_List = ServiceLogStatus_Listbll.SelectSingle("id=" + SearchStatus_);
            if (ServiceLogStatus_List != null && ServiceLogStatus_List.id > -1)
            {
                Condition += " and status=" + ServiceLogStatus_List.id;
                SearchStatus.Text = SearchStatus_.ToString();
            }
            string SearchKeyWords_ = Function.GetRequest("SearchKeyWords");
            if (!string.IsNullOrWhiteSpace(SearchKeyWords_))
            {
                Condition += " and name like'%" + Function.HtmlEncode(SearchKeyWords_) + "%'";
                SearchKeyWords.Text = SearchKeyWords_;
            }

            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "ServiceLog_List";
            //需要返回的列
            string strGetFields = " RANK()  OVER (order by uptime asc,Id asc) AS xuhao,id, name, info_, addtime, status, userid, uptime";
            //排序的字段名
            string fldname = "uptime desc,Id desc";
            //每页显示的记录数

            AspNetPager1.PageSize = 15;
            int page_Size = this.AspNetPager1.PageSize;
            //统计总记录数
            int intRecordCount = ServiceLog_ListBll.GetCount(tblName, strWhere);
            if (intRecordCount > 0)
            {
                DivNull.Visible = false;
            }
            DataTable dt = ServiceLog_ListBll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, strWhere);
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

        protected void OnClick_Search(object sender, EventArgs e)
        {
            string where_ = "?MenuId=" + MenuId + "&btn=search";
            int SearchStatus_ = Function.ConvertTo<int>(Function.FormRequest("SearchStatus"), -1);
            ServiceLogStatus_List ServiceLogStatus_List = ServiceLogStatus_Listbll.SelectSingle("id=" + SearchStatus_);
            if (ServiceLogStatus_List != null && ServiceLogStatus_List.id > -1)
            {
                where_ += "&SearchStatus=" + ServiceLogStatus_List.id;
            }
            string SearchKeyWords_ = Function.FormRequest("SearchKeyWords");
            if (!string.IsNullOrWhiteSpace(SearchKeyWords_))
            {
                where_ += "&SearchKeyWords=" + Server.UrlEncode(SearchKeyWords_.Trim());
            }
            Response.Redirect(Request.CurrentExecutionFilePath + where_);
        }
    }

}