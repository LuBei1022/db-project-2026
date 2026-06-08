using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;

namespace Web.admin
{
    public partial class Admin_AppealList : System.Web.UI.Page
    {
        BLLBase<appeal_list> appeal_listbll = new BLLBase<appeal_list>();
        BLLBase<appealimg_list> appealimg_listbll = new BLLBase<appealimg_list>();
        string Action = Function.GetRequest("Action");
        public string MenuId = Function.GetRequest("MenuId");
        public int appeal_status = 0;
        public bool isLoading = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            switch (Action)
            {
                case "Edit":
                    EditFunc();
                    break;
                default:
                    BindData();
                    break;
            }
        }
        protected void EditFunc()
        {
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            appeal_list appeal_list = appeal_listbll.SelectSingle("Id=" + ID + " and status=" + appeal_status);
            if (appeal_list != null && appeal_list.id > 0)
            {
                AddUp.Visible = true;
                Main.Visible = false;
                Txt_Title.Text = "<font color=\"red\">反馈详情</font>";
                usetinfo.Text = CommonUserFunc.GetUserInfoHtml(appeal_list.userid.ToString());
                if (!string.IsNullOrWhiteSpace(appeal_list.url))
                {
                    url.Text = Function.HtmlDiscode(appeal_list.url);
                }
                if (!string.IsNullOrWhiteSpace(appeal_list.info_))
                {
                    info_.Text = Function.HtmlDiscode(appeal_list.info_);
                }
                addtime.Text = appeal_list.addtime.ToString("yyyy-MM-dd HH:mm:ss");

                DataTable appealimg_listdt = appealimg_listbll.GetDatatable("select upload_pic_info from appealimg_list where appeal_id=" + appeal_list.id + "  order by orderid asc,addtime asc");
                if (appealimg_listdt != null && appealimg_listdt.Rows.Count > 0)
                {
                    this.ImgList.DataSource = appealimg_listdt.DefaultView;
                    this.ImgList.DataBind();
                }
                appealimg_listdt.Dispose();

            }
        }

        /// <summary>
        /// 绑定反馈列表
        /// </summary>
        protected void BindData()
        {
            string Condition = " status=" + appeal_status;

            string SearchUserInfo_str = Function.GetRequest("SearchUserInfo");
            if (!string.IsNullOrWhiteSpace(SearchUserInfo_str))
            {
                Condition += " and userid in(select id from user_list where name like'%" + Function.HtmlEncode(SearchUserInfo_str) + "%' or tel like '%" + Function.HtmlEncode(SearchUserInfo_str) + "%')";
                SearchUserInfo.Text = SearchUserInfo_str;
            }
            string SearchKeyWords_str = Function.FormRequest("SearchKeyWords");
            if (!string.IsNullOrWhiteSpace(SearchKeyWords_str))
            {
                Condition += " and (url like'%" + Function.HtmlEncode(SearchKeyWords_str) + "%' or info_ like'%" + Function.HtmlEncode(SearchKeyWords_str) + "%')";
                SearchKeyWords.Text = SearchKeyWords_str;
            }

            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "appeal_list";
            //需要返回的列
            string strGetFields = " RANK()  OVER (order by addtime asc,id asc) AS xuhao,id, url, addtime, status, userid";
            //排序的字段名
            string fldname = "addtime desc,id desc";
            //每页显示的记录数

            AspNetPager1.PageSize = 15;
            int page_Size = this.AspNetPager1.PageSize;
            //统计总记录数
            int intRecordCount = appeal_listbll.GetCount(tblName, strWhere);
            if (intRecordCount > 0)
            {
                DivNull.Visible = false;
            }
            DataTable dt = appeal_listbll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, strWhere);
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

            Response.Redirect(Request.CurrentExecutionFilePath + where_);
        }
    }

}
