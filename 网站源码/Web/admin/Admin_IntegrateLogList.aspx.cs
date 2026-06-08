using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Web.admin
{
    public partial class Admin_IntegrateLogList : System.Web.UI.Page
    {
        BLLBase<integrateLog_list> integrateLog_listbll = new BLLBase<integrateLog_list>();
        BLLBase<integrateLogType_list> integrateLogType_listbll = new BLLBase<integrateLogType_list>();
        private const string AllowedTypeIds = "1,3,4,6,10,11,12";
        public string MenuId = Function.GetRequest("MenuId");
        public bool isLoading = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            Big_List();
            BindData();
        }
        protected void Big_List()
        {
            SearchType.Items.Add(new ListItem("--请选择--", ""));
            DataTable integrateLogType_listdt = integrateLogType_listbll.GetDatatable("select id,name from integrateLogType_list where id in (" + AllowedTypeIds + ") order by id asc");
            if (integrateLogType_listdt != null && integrateLogType_listdt.Rows.Count > 0)
            {
                foreach (DataRow item in integrateLogType_listdt.Rows)
                {
                    SearchType.Items.Add(new ListItem(Function.HtmlDiscode(item["name"].ToString()), item["id"].ToString()));
                }
            }
            integrateLogType_listdt.Dispose();
        }
        /// <summary>
        /// 绑定积分流水
        /// </summary>
        protected void BindData()
        {
            string Condition = " type in (" + AllowedTypeIds + ")";


            string SearchUserInfo_str = Function.GetRequest("SearchUserInfo");
            if (!string.IsNullOrWhiteSpace(SearchUserInfo_str))
            {
                Condition += " and user_id in(select id from user_list where name like'%" + Function.HtmlEncode(SearchUserInfo_str) + "%' or tel like '%" + Function.HtmlEncode(SearchUserInfo_str) + "%')";
                SearchUserInfo.Text = SearchUserInfo_str;
            }
            string SearchKeyWords_str = Function.FormRequest("SearchKeyWords");
            if (!string.IsNullOrWhiteSpace(SearchKeyWords_str))
            {
                Condition += " and (name like'%" + Function.HtmlEncode(SearchKeyWords_str) + "%' or info_ like'%" + Function.HtmlEncode(SearchKeyWords_str) + "%')";
                SearchKeyWords.Text = SearchKeyWords_str;
            }

            int SearchType_ = Function.ConvertTo<int>(Function.GetRequest("SearchType"), 0);
            integrateLogType_list integrateLogType_list = integrateLogType_listbll.SelectSingle("id=" + SearchType_);
            if (integrateLogType_list != null && !string.IsNullOrWhiteSpace(integrateLogType_list.name))
            {
                Condition += " and type=" + integrateLogType_list.id;
                SearchType.Text = SearchType_.ToString();
            }

            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "integrateLog_list";
            //需要返回的列
            string strGetFields = " RANK()  OVER (order by addtime asc,id asc) AS xuhao,id, num_integrate, type, name, info_, addtime, user_id, adminname";
            //排序的字段名
            string fldname = "addtime desc,id desc";
            //每页显示的记录数

            AspNetPager1.PageSize = 15;
            int page_Size = this.AspNetPager1.PageSize;
            //统计总记录数
            int intRecordCount = integrateLog_listbll.GetCount(tblName, strWhere);
            if (intRecordCount > 0)
            {
                DivNull.Visible = false;
            }
            DataTable dt = integrateLog_listbll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, strWhere);
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
            string where_ = "?btn=search&MenuId=" + MenuId;
            string SearchUserInfo_ = Function.FormRequest("SearchUserInfo");
            if (!string.IsNullOrWhiteSpace(SearchUserInfo_))
            {
                where_ += "&SearchUserInfo=" + Server.UrlEncode(SearchUserInfo_.Trim());
            }

            string SearchKeyWords_ = Function.FormRequest("SearchKeyWords");
            if (!string.IsNullOrWhiteSpace(SearchKeyWords_))
            {
                where_ += "&SearchKeyWords=" + Server.UrlEncode(SearchKeyWords_.Trim());
            }
            int SearchType_ = Function.ConvertTo<int>(Function.FormRequest("SearchType"), 0);
            integrateLogType_list integrateLogType_list = integrateLogType_listbll.SelectSingle("id=" + SearchType_);
            if (integrateLogType_list != null && !string.IsNullOrWhiteSpace(integrateLogType_list.name))
            {
                where_ += "&SearchType=" + integrateLogType_list.id;
            }
            Response.Redirect(Request.CurrentExecutionFilePath + where_);
        }
    }

}
