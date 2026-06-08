using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Web.admin
{
    public partial class Admin_IntegrateExchangeLog : System.Web.UI.Page
    {
        BLLBase<integrateExchangeLog_list> integrateExchangeLog_listbll = new BLLBase<integrateExchangeLog_list>();
        BLLBase<integratestatus_list> integratestatus_listbll = new BLLBase<integratestatus_list>();
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
            SearchStatus.Items.Add(new ListItem("--请选择--", ""));
            DataTable integratestatus_listdt = integratestatus_listbll.GetDatatable("select id,name from integratestatus_list order by id asc");
            if (integratestatus_listdt != null && integratestatus_listdt.Rows.Count > 0)
            {
                foreach (DataRow item in integratestatus_listdt.Rows)
                {
                    SearchStatus.Items.Add(new ListItem(Function.HtmlDiscode(item["name"].ToString()), item["id"].ToString()));
                }
            }
            integratestatus_listdt.Dispose();
        }
        /// <summary>
        /// 绑定权益记录
        /// </summary>
        protected void BindData()
        {
            string Condition = " 1=1";
            string SearchCode_str = Function.FormRequest("SearchCode");
            if (!string.IsNullOrWhiteSpace(SearchCode_str))
            {
                Condition += " and codestr like'%" + Function.HtmlEncode(SearchCode_str) + "%'";
                SearchCode.Text = SearchCode_str;
            }

            string SearchUserInfo_str = Function.GetRequest("SearchUserInfo");
            if (!string.IsNullOrWhiteSpace(SearchUserInfo_str))
            {
                Condition += " and user_id in(select id from user_list where name like'%" + Function.HtmlEncode(SearchUserInfo_str) + "%' or tel like '%" + Function.HtmlEncode(SearchUserInfo_str) + "%')";
                SearchUserInfo.Text = SearchUserInfo_str;
            }
            string SearchKeyWords_str = Function.FormRequest("SearchKeyWords");
            if (!string.IsNullOrWhiteSpace(SearchKeyWords_str))
            {
                Condition += " and name like'%" + Function.HtmlEncode(SearchKeyWords_str) + "%'";
                SearchKeyWords.Text = SearchKeyWords_str;
            }

            int SearchStatus_ = Function.ConvertTo<int>(Function.GetRequest("SearchStatus"), 0);
            integratestatus_list integratestatus_list = integratestatus_listbll.SelectSingle("id=" + SearchStatus_);
            if (integratestatus_list != null && !string.IsNullOrWhiteSpace(integratestatus_list.name))
            {
                Condition += " and status=" + integratestatus_list.id;
                SearchStatus.Text = SearchStatus_.ToString();
            }

            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "integrateExchangeLog_list";
            //需要返回的列
            string strGetFields = " RANK()  OVER (order by addtime asc,id asc) AS xuhao,id, name, num_integrate, codestr, addtime, status, user_id, upload_pic_img";
            //排序的字段名
            string fldname = "addtime desc,id desc";
            //每页显示的记录数

            AspNetPager1.PageSize = 15;
            int page_Size = this.AspNetPager1.PageSize;
            //统计总记录数
            int intRecordCount = integrateExchangeLog_listbll.GetCount(tblName, strWhere);
            if (intRecordCount > 0)
            {
                DivNull.Visible = false;
            }
            DataTable dt = integrateExchangeLog_listbll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, strWhere);
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
            string SearchCode_ = Function.FormRequest("SearchCode");
            if (!string.IsNullOrWhiteSpace(SearchCode_))
            {
                where_ += "&SearchCode=" + Server.UrlEncode(SearchCode_.Trim());
            }
            string SearchKeyWords_ = Function.FormRequest("SearchKeyWords");
            if (!string.IsNullOrWhiteSpace(SearchKeyWords_))
            {
                where_ += "&SearchKeyWords=" + Server.UrlEncode(SearchKeyWords_.Trim());
            }
            int SearchStatus_ = Function.ConvertTo<int>(Function.FormRequest("SearchStatus"), 0);
            integratestatus_list integratestatus_list = integratestatus_listbll.SelectSingle("id=" + SearchStatus_);
            if (integratestatus_list != null && !string.IsNullOrWhiteSpace(integratestatus_list.name))
            {
                where_ += "&SearchStatus=" + integratestatus_list.id;
            }
            Response.Redirect(Request.CurrentExecutionFilePath + where_);
        }
    }

}
