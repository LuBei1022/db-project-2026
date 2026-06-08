using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Text;
using System.Web;

namespace Web
{
    public partial class top : System.Web.UI.UserControl
    {
        BLLBase<websiteinfo_list> websiteinfo_listbll = new BLLBase<websiteinfo_list>();
        BLLBase<LoginSingle_List> LoginSingle_Listbll = new BLLBase<LoginSingle_List>();
        BLLBase<tbl_class> tbl_classbll = new BLLBase<tbl_class>();
        BLLBase<SearchHot_List> SearchHot_Listbll = new BLLBase<SearchHot_List>();
        BLLBase<NoticeLog_List> NoticeLog_ListBll = new BLLBase<NoticeLog_List>();
        BLLBase<TopUpType_List> TopUpType_ListBll = new BLLBase<TopUpType_List>();
        public websiteinfo_list websiteinfo_list = new websiteinfo_list();
        public user_list user_list = new user_list();
        public bool isTbClassLink = false;
        public bool IsLogin = false;
        public bool isSearchHot = false;
        public bool isSearch = true;
        public bool IsLiteratureHomePage = false;
        public int CurrentIntegrate = 0;
        public int HeaderNoticeCount = 0;
        public string HeaderNoticeHtml = string.Empty;
        public int HeaderMoneyIntegrate = 10;
        public int HeaderIntegrateDonate = 0;
        public string HeaderTopUpOptionsHtml = string.Empty;
        public string AcademicNewsHref = "/Website/news";

        private bool IsLiteratureHomeRequest()
        {
            string p = (Request.Url.LocalPath ?? string.Empty).Trim().ToLowerInvariant();
            if (p == "/" || string.Equals(p, "/index.aspx", StringComparison.Ordinal))
                return true;
            return p.EndsWith("/index.aspx", StringComparison.Ordinal);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                IsLiteratureHomePage = IsLiteratureHomeRequest();
                string LocalPath = Context.Request.Url.LocalPath.ToLower();
                isSearch = LocalPath != "/search";
                user_list = CommonUserFunc.GetUserLoginStatus();
                if (user_list != null && user_list.id > 0)
                {
                    IsLogin = true;
                    CurrentIntegrate = CommonUserFunc.GetUserIntegrateSumFunc(user_list.id, 0);
                    BindHeaderNotices(user_list.id);
                }

                isTbClassLink = false;
                isSearchHot = false;
                websiteinfo_list = websiteinfo_listbll.SelectSingle("id=1");
                if (websiteinfo_list != null)
                {
                    HeaderMoneyIntegrate = websiteinfo_list.money_integrate > 0 ? websiteinfo_list.money_integrate : 10;
                    HeaderIntegrateDonate = websiteinfo_list.integrate_donate;
                }
                if (IsLogin)
                {
                    BindHeaderTopUpOptions();
                }

                if (!IsLogin)
                {
                    DataTable LoginSingle_List_dt = LoginSingle_Listbll.GetDatatable("select id,name,info_ from LoginSingle_List where isshow=1 order by orderid asc,id asc");
                    if (LoginSingle_List_dt != null && LoginSingle_List_dt.Rows.Count > 0)
                    {
                        this.LoginSingleData.DataSource = LoginSingle_List_dt.DefaultView;
                        this.LoginSingleData.DataBind();
                        this.LoginSingleData_.DataSource = LoginSingle_List_dt.DefaultView;
                        this.LoginSingleData_.DataBind();
                    }
                    LoginSingle_List_dt.Dispose();
                }


                DataTable SearchHot_List_dt = SearchHot_Listbll.GetDatatable("select top 8 id,name,url from SearchHot_List where isshow=1 order by orderid desc,uptime desc,id desc");
                if (SearchHot_List_dt != null && SearchHot_List_dt.Rows.Count > 0)
                {
                    this.SearchHotList.DataSource = SearchHot_List_dt.DefaultView;
                    this.SearchHotList.DataBind();
                    isSearchHot = true;
                }
                SearchHot_List_dt.Dispose();

                if (!IsLiteratureHomePage)
                {
                    DataTable tbl_classdt = tbl_classbll.GetDatatable("select id,classname,model from tbl_class where isshow=1 and istop=1 and parentid=360 order by orderid asc,id asc");
                    if (tbl_classdt != null && tbl_classdt.Rows.Count > 0)
                    {
                        this.MenuClassList.DataSource = tbl_classdt.DefaultView;
                        this.MenuClassList.DataBind();
                        isTbClassLink = true;
                    }
                    tbl_classdt.Dispose();
                }

                AcademicNewsHref = GetAcademicNewsHref();
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "top.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
            }
        }

        private string GetAcademicNewsHref()
        {
            DataTable newsClassDt = tbl_classbll.GetDatatable("select top 1 id from tbl_class where isshow=1 and model=2 and id in(" + Function.Decrypt(CommonFunc.GetChildrenId(360)) + ") order by orderid asc,id asc");
            if (newsClassDt != null && newsClassDt.Rows.Count > 0)
            {
                int classId = Function.ConvertTo<int>(newsClassDt.Rows[0]["id"], 0);
                newsClassDt.Dispose();
                if (classId > 0)
                {
                    return CommonFunc.GetTopHtmlHref(classId.ToString(), "0");
                }
            }
            if (newsClassDt != null)
            {
                newsClassDt.Dispose();
            }

            return "/Website/news";
        }

        private void BindHeaderTopUpOptions()
        {
            StringBuilder html = new StringBuilder();
            DataTable dt = TopUpType_ListBll.GetDatatable("select id,money from TopUpType_List where isshow=1 order by money asc,id asc");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    int id = Function.ConvertTo<int>(Convert.ToString(row["id"]), 0);
                    int money = Function.ConvertTo<int>(Convert.ToString(row["money"]), 0);
                    if (id <= 0 || money <= 0)
                    {
                        continue;
                    }

                    html.Append("<button type=\"button\" class=\"lm-topup-option\" data-id=\"")
                        .Append(id)
                        .Append("\" data-money=\"")
                        .Append(money)
                        .Append("\">")
                        .Append(money)
                        .Append(" 元</button>");
                }
            }
            if (dt != null)
            {
                dt.Dispose();
            }

            HeaderTopUpOptionsHtml = html.ToString();
        }

        private void BindHeaderNotices(int userId)
        {
            string baseCondition = " userid=" + userId + " and (isnull(name,'') like '%文献%' or isnull(info_,'') like '%文献%' or isnull(name,'') like '%积分%' or isnull(info_,'') like '%积分%' or isnull(name,'') like '%赞&收藏%' or isnull(url,'') like '/Literature%' or isnull(url,'') like '/User/Integrate%')";
            string praiseCondition = baseCondition + " and isnull(name,'') like '%赞&收藏%'";
            string importantCondition = baseCondition + " and isnull(name,'') not like '%赞&收藏%'";
            string condition = baseCondition;
            HeaderNoticeCount = NoticeLog_ListBll.GetCount("NoticeLog_List", condition);

            DataTable importantDt = NoticeLog_ListBll.GetDatatable("select top 6 id,name,info_,addtime,url from NoticeLog_List where " + importantCondition + " order by addtime desc,id desc");
            DataTable praiseDt = NoticeLog_ListBll.GetDatatable("select top 8 id,name,info_,addtime,url from NoticeLog_List where " + praiseCondition + " order by addtime desc,id desc");
            StringBuilder html = new StringBuilder();
            html.Append(BuildHeaderNoticeSection("重要通知", "审核、积分、评论与系统处理消息", importantDt, false));
            html.Append(BuildHeaderNoticeSection("赞&收藏", "点赞和收藏消息单独收纳", praiseDt, true));
            if (importantDt != null)
            {
                importantDt.Dispose();
            }
            if (praiseDt != null)
            {
                praiseDt.Dispose();
            }
            HeaderNoticeHtml = html.ToString();
        }

        private string BuildHeaderNoticeSection(string sectionTitle, string subtitle, DataTable dt, bool isPraise)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<section class=\"lm-modal-notice-section");
            html.Append(isPraise ? " likes" : string.Empty);
            html.Append("\"><div class=\"lm-modal-notice-section-head\">");
            html.Append(HttpUtility.HtmlEncode(sectionTitle));
            html.Append("<span>");
            html.Append(HttpUtility.HtmlEncode(subtitle));
            html.Append("</span></div>");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string title = Function.HtmlDiscode(Convert.ToString(row["name"]));
                    if (string.IsNullOrWhiteSpace(title))
                    {
                        title = "系统通知";
                    }
                    string body = Function.HtmlDiscode(Convert.ToString(row["info_"]));
                    if (string.IsNullOrWhiteSpace(body))
                    {
                        body = "暂无详细说明。";
                    }
                    DateTime addtime = Function.ConvertTo<DateTime>(Convert.ToString(row["addtime"]), DateTime.MinValue);
                    string url = Function.HtmlDiscode(Convert.ToString(row["url"])).Trim();
                    if (string.IsNullOrWhiteSpace(url) && (title.Contains("服务工单") || body.Contains("服务工单")))
                    {
                        url = "/User/ServiceLog";
                    }
                    bool canOpen = url.StartsWith("/Literature", StringComparison.OrdinalIgnoreCase)
                        || url.StartsWith("/User/Integrate", StringComparison.OrdinalIgnoreCase)
                        || url.StartsWith("/User/ServiceLog", StringComparison.OrdinalIgnoreCase)
                        || url.StartsWith("/LiteratureSearch", StringComparison.OrdinalIgnoreCase);

                    html.Append("<div class=\"lm-modal-notice-item\">");
                    html.Append("<h4>");
                    html.Append(HttpUtility.HtmlEncode(title));
                    html.Append("</h4>");
                    if (addtime != DateTime.MinValue)
                    {
                        html.Append("<p class=\"time\">");
                        html.Append(addtime.ToString("yyyy-MM-dd HH:mm:ss"));
                        html.Append("</p>");
                    }
                    html.Append("<div class=\"body\">");
                    html.Append(HttpUtility.HtmlEncode(body));
                    html.Append("</div>");
                    if (canOpen)
                    {
                        html.Append("<a href=\"");
                        html.Append(HttpUtility.HtmlAttributeEncode(url));
                        html.Append("\">查看详情</a>");
                    }
                    html.Append("</div>");
                }
            }
            else
            {
                html.Append("<div class=\"lm-modal-empty\">");
                html.Append(isPraise ? "暂无点赞或收藏消息" : "暂无重要通知消息");
                html.Append("</div>");
            }
            html.Append("</section>");
            return html.ToString();
        }
    }
}
