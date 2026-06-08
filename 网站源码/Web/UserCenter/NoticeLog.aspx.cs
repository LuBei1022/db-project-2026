using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Web;

namespace Web.UserCenter
{
    public partial class NoticeLog : System.Web.UI.Page
    {
        BLLBase<NoticeLog_List> NoticeLog_ListBll = new BLLBase<NoticeLog_List>();
        public user_list user_list = new user_list();
        public static int PageIndex = 0;
        public int intRecordCount = 0;
        public int ReplyCount = 0;
        public bool HasData = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    user_list = CommonUserFunc.GetUserLoginStatus();
                    if (user_list != null && user_list.id > 0)
                    {
                        ReplyCount = 0;
                        BindDataList();
                    }
                    else
                    {
                        Response.Redirect("/");
                    }
                }
                catch (Exception ex)
                {
                    ImportDataLog.WriteLog(LogType.Error, "ServiceLogAdd.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
                }
            }
        }

        protected void BindDataList()
        {
            string Condition = " userid=" + user_list.id + " and (isnull(name,'') like '%文献%' or isnull(info_,'') like '%文献%' or isnull(name,'') like '%积分%' or isnull(info_,'') like '%积分%' or isnull(url,'') like '/Literature%' or isnull(url,'') like '/User/Integrate%')";


            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "NoticeLog_List";
            //需要返回的列
            string strGetFields = " id, info_, type, addtime, userid, looktime, status, url, name";
            //排序的字段名
            string fldname = " addtime desc,id desc";


            //每页显示的记录数
            int page_Size = 20;
            //统计总记录数
            intRecordCount = NoticeLog_ListBll.GetCount(tblName, strWhere);

            PageIndex = Function.ConvertTo<int>(Function.GetRequest("page"), 0);
            if (PageIndex > 0)
            {
            }
            else
            {
                PageIndex = 1;
            }

            DataTable dt = NoticeLog_ListBll.GetListByPage(tblName, strGetFields, fldname, page_Size, PageIndex, strWhere);
            if (dt != null && dt.Rows.Count > 0)
            {
                HasData = true;
                this.DataList.DataSource = dt.DefaultView;
                this.DataList.DataBind();
            }
        }

        protected string GetNoticeTitle(object value)
        {
            string title = Function.HtmlDiscode(Convert.ToString(value));
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "系统通知";
            }
            return HttpUtility.HtmlEncode(title);
        }

        protected string GetNoticeTime(object value)
        {
            DateTime time = Function.ConvertTo<DateTime>(Convert.ToString(value), DateTime.MinValue);
            if (time == DateTime.MinValue)
            {
                return string.Empty;
            }
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        protected string GetNoticeBody(object value)
        {
            string body = Function.HtmlDiscode(Convert.ToString(value));
            if (string.IsNullOrWhiteSpace(body))
            {
                body = "暂无详细说明。";
            }
            return HttpUtility.HtmlEncode(body);
        }

        protected string GetNoticeLink(object value)
        {
            string url = Function.HtmlDiscode(Convert.ToString(value)).Trim();
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            if (!(url.StartsWith("/Literature", StringComparison.OrdinalIgnoreCase)
                || url.StartsWith("/User/Integrate", StringComparison.OrdinalIgnoreCase)
                || url.StartsWith("/User/ServiceLog", StringComparison.OrdinalIgnoreCase)
                || url.StartsWith("/LiteratureSearch", StringComparison.OrdinalIgnoreCase)))
            {
                return string.Empty;
            }

            return "<a class=\"notice-link\" href=\"" + HttpUtility.HtmlAttributeEncode(url) + "\">查看详情</a>";
        }
    }
}
