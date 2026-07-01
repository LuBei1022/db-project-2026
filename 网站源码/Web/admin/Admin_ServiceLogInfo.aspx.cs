using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace Web.admin
{
    public partial class Admin_ServiceLogInfo : System.Web.UI.Page
    {
        BLLBase<ServiceLog_List> ServiceLog_ListBll = new BLLBase<ServiceLog_List>();
        BLLBase<ServiceLogInfo_List> ServiceLogInfo_Listbll = new BLLBase<ServiceLogInfo_List>();
        BLLBase<user_list> user_listbll = new BLLBase<user_list>();
        BLLBase<LiteratureComment> LiteratureCommentBll = new BLLBase<LiteratureComment>();
        public string MenuId = Function.GetRequest("MenuId");
        public ServiceLog_List ServiceLog_List = new ServiceLog_List();
        public bool isLoading = false;
        public string upload_pic_avatar = string.Empty;
        public string user_name = string.Empty;
        public string BackUrl = string.Empty;
        public string LiteratureCommentAuditHtml = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            BackUrl = GetBackUrl();
            ServiceLog_List = ServiceLog_ListBll.SelectSingle("id=" + Function.ConvertTo<int>(Function.GetRequest("id"), 0));
            if (ServiceLog_List != null && ServiceLog_List.id > 0)
            {
                if (HandleLiteratureCommentAuditAction())
                {
                    return;
                }

                user_list user_list = user_listbll.SelectSingle("id=" + ServiceLog_List.userid);
                if (user_list != null && user_list.id > 0)
                {
                    upload_pic_avatar = CommonUserFunc.GetUserAvatarFunc(user_list.upload_pic_avatar);
                    user_name = (!string.IsNullOrWhiteSpace(user_list.name) ? Function.HtmlDiscode(user_list.name) : "我");
                }

                DataTable ServiceLogInfo_Listdt = ServiceLogInfo_Listbll.GetDatatable("select info_, type, addtime, adminname from ServiceLogInfo_List where ServiceLog_Id=" + ServiceLog_List.id + " order by addtime asc");
                if (ServiceLogInfo_Listdt != null && ServiceLogInfo_Listdt.Rows.Count > 0)
                {
                    this.DataList.DataSource = ServiceLogInfo_Listdt.DefaultView;
                    this.DataList.DataBind();
                }
                ServiceLogInfo_Listdt.Dispose();

                LiteratureCommentAuditHtml = BuildLiteratureCommentAuditHtml();
                isLoading = true;
            }
        }

        private bool HandleLiteratureCommentAuditAction()
        {
            string action = Function.GetRequest("CommentAction");
            if (string.IsNullOrWhiteSpace(action))
            {
                return false;
            }

            int commentId = Function.ConvertTo<int>(Function.GetRequest("CommentId"), 0);
            int ticketCommentId = ExtractLiteratureCommentId();
            if (commentId <= 0)
            {
                commentId = ticketCommentId;
            }
            if (commentId <= 0 || (ticketCommentId > 0 && ticketCommentId != commentId))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "未识别到对应评论，无法审核！", BuildServiceLogInfoUrl(), 2);
                return true;
            }

            LiteratureComment comment = LiteratureCommentBll.SelectSingle("id=" + commentId);
            if (comment == null || comment.id <= 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "评论记录不存在或已被删除！", BuildServiceLogInfoUrl(), 2);
                return true;
            }

            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string message;
            string auditInfo;
            if (action.Equals("Approve", StringComparison.OrdinalIgnoreCase))
            {
                LiteratureCommentBll.Update("status=1,is_deleted=0,reviewed_by=0,review_time='" + now + "',review_remark=N'后台工单审核通过',updatetime='" + now + "'", "id=" + commentId);
                message = "评论审核通过，前台已可展示。";
                auditInfo = "评论审核结果：审核通过。";
            }
            else if (action.Equals("Reject", StringComparison.OrdinalIgnoreCase))
            {
                LiteratureCommentBll.Update("status=2,is_deleted=0,reviewed_by=0,review_time='" + now + "',review_remark=N'后台工单审核驳回',updatetime='" + now + "'", "id=" + commentId);
                message = "评论已驳回，前台不会展示。";
                auditInfo = "评论审核结果：已驳回。";
            }
            else if (action.Equals("Delete", StringComparison.OrdinalIgnoreCase))
            {
                LiteratureCommentBll.Update("status=3,is_deleted=1,delete_time='" + now + "',reviewed_by=0,review_time='" + now + "',review_remark=N'后台工单删除评论',updatetime='" + now + "'", "id=" + commentId);
                message = "评论已删除，前台不会展示。";
                auditInfo = "评论审核结果：已删除。";
            }
            else
            {
                return false;
            }

            AddAuditServiceLogInfo(auditInfo);
            ServiceLog_ListBll.Update("status=1,uptime='" + now + "'", "id=" + ServiceLog_List.id);
            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), message, BuildCleanServiceLogInfoUrl(), 0);
            return true;
        }

        private string BuildLiteratureCommentAuditHtml()
        {
            if (!IsLiteratureCommentTicket())
            {
                return string.Empty;
            }

            int commentId = ExtractLiteratureCommentId();
            if (commentId <= 0)
            {
                return "<div class=\"comment-audit-panel\"><strong>文献评论审核</strong><p>未能从工单内容中识别评论ID，请到文献评论审核页按文献筛选处理。</p></div>";
            }

            LiteratureComment comment = LiteratureCommentBll.SelectSingle("id=" + commentId);
            if (comment == null || comment.id <= 0)
            {
                return "<div class=\"comment-audit-panel\"><strong>文献评论审核</strong><p>评论记录不存在或已被删除。</p></div>";
            }

            StringBuilder html = new StringBuilder();
            html.Append("<div class=\"comment-audit-panel\"><div class=\"comment-audit-title\">文献评论审核</div>");
            html.Append("<div class=\"comment-audit-status\">评论ID：");
            html.Append(comment.id);
            html.Append("　当前状态：<strong>");
            html.Append(Server.HtmlEncode(GetLiteratureCommentStatusText(comment.status, comment.is_deleted)));
            html.Append("</strong></div><div class=\"comment-audit-actions\">");
            if (!(comment.status == 1 && comment.is_deleted == 0))
            {
                html.Append("<a class=\"btn btn-success\" href=\"");
                html.Append(BuildCommentAuditActionUrl(comment.id, "Approve"));
                html.Append("\">审核通过</a>");
            }
            if (!(comment.status == 2 && comment.is_deleted == 0))
            {
                html.Append("<a class=\"btn btn-warning\" href=\"");
                html.Append(BuildCommentAuditActionUrl(comment.id, "Reject"));
                html.Append("\">驳回</a>");
            }
            if (comment.is_deleted == 0 && comment.status != 3)
            {
                html.Append("<a class=\"btn btn-danger\" onclick=\"return confirm('确认删除这条评论吗？');\" href=\"");
                html.Append(BuildCommentAuditActionUrl(comment.id, "Delete"));
                html.Append("\">删除评论</a>");
            }
            html.Append("</div></div>");
            return html.ToString();
        }

        private string GetBackUrl()
        {
            string backUrl = Request.QueryString["BackURL"];
            if (!string.IsNullOrWhiteSpace(backUrl))
            {
                backUrl = Server.UrlDecode(backUrl);
            }

            if (string.IsNullOrWhiteSpace(backUrl)
                || backUrl.IndexOf("Admin_ServiceLogInfo.aspx", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                backUrl = "Admin_ServiceLogList.aspx?MenuId=" + Server.UrlEncode(MenuId);
            }
            return backUrl;
        }

        private bool IsLiteratureCommentTicket()
        {
            string name = Function.HtmlDiscode(ServiceLog_List == null ? string.Empty : ServiceLog_List.name);
            return name.StartsWith("[文献评论]", StringComparison.OrdinalIgnoreCase)
                || ExtractLiteratureCommentId() > 0;
        }

        private int ExtractLiteratureCommentId()
        {
            string info = GetTicketInfoText();
            Match match = Regex.Match(info ?? string.Empty, @"评论ID\s*[:：]\s*(\d+)", RegexOptions.IgnoreCase);
            int parsedCommentId = match.Success ? Function.ConvertTo<int>(match.Groups[1].Value, 0) : 0;
            int literatureId = ExtractTicketNumber(info, "文献ID");
            return ResolveLiteratureCommentId(parsedCommentId, literatureId);
        }

        private string GetTicketInfoText()
        {
            return Function.HtmlDiscode(ServiceLog_List == null ? string.Empty : ServiceLog_List.info_);
        }

        private int ExtractTicketNumber(string info, string label)
        {
            Match match = Regex.Match(info ?? string.Empty, label + @"\s*[:：]\s*(\d+)", RegexOptions.IgnoreCase);
            return match.Success ? Function.ConvertTo<int>(match.Groups[1].Value, 0) : 0;
        }

        private string ExtractTicketCommentText(string info)
        {
            Match match = Regex.Match(info ?? string.Empty, @"评论内容\s*[:：]\s*(?<content>[\s\S]*)", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups["content"].Value.Trim() : string.Empty;
        }

        private int ResolveLiteratureCommentId(int parsedCommentId, int literatureId)
        {
            if (literatureId <= 0)
            {
                return parsedCommentId;
            }

            if (parsedCommentId > 0)
            {
                LiteratureComment parsedComment = LiteratureCommentBll.SelectSingle("id=" + parsedCommentId);
                if (parsedComment != null
                    && parsedComment.id > 0
                    && (parsedComment.literature_id == literatureId
                        || (parsedComment.canonical_literature_id.HasValue && parsedComment.canonical_literature_id.Value == literatureId)))
                {
                    return parsedCommentId;
                }
            }

            int matchedCommentId = FindLiteratureCommentIdByTicket(literatureId, true);
            if (matchedCommentId <= 0)
            {
                matchedCommentId = FindLiteratureCommentIdByTicket(literatureId, false);
            }
            return matchedCommentId > 0 ? matchedCommentId : parsedCommentId;
        }

        private int FindLiteratureCommentIdByTicket(int literatureId, bool requireContent)
        {
            if (ServiceLog_List == null || literatureId <= 0)
            {
                return 0;
            }

            string condition = "parent_id=0 and (literature_id=" + literatureId + " or canonical_literature_id=" + literatureId + ")";
            if (ServiceLog_List.userid > 0)
            {
                condition += " and userid=" + ServiceLog_List.userid;
            }

            string content = ExtractTicketCommentText(GetTicketInfoText());
            if (requireContent)
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return 0;
                }
                string safeContent = Function.HtmlEncode(content).Replace("'", "''");
                condition += " and content=N'" + safeContent + "'";
            }

            string addtime = ServiceLog_List.addtime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string sql = "select top 1 id from LiteratureComment where " + condition
                + " order by abs(datediff(second,addtime,'" + addtime + "')),id desc";
            DataTable dt = LiteratureCommentBll.GetDatatable(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                if (dt != null)
                {
                    dt.Dispose();
                }
                return 0;
            }

            int id = Function.ConvertTo<int>(Convert.ToString(dt.Rows[0]["id"]), 0);
            dt.Dispose();
            return id;
        }

        private string BuildCommentAuditActionUrl(int commentId, string action)
        {
            StringBuilder url = new StringBuilder("Admin_ServiceLogInfo.aspx?ID=");
            url.Append(ServiceLog_List.id);
            if (!string.IsNullOrWhiteSpace(MenuId))
            {
                url.Append("&MenuId=");
                url.Append(Server.UrlEncode(MenuId));
            }
            url.Append("&CommentAction=");
            url.Append(Server.UrlEncode(action));
            url.Append("&CommentId=");
            url.Append(commentId);
            string backUrl = Request.QueryString["BackURL"];
            if (!string.IsNullOrWhiteSpace(backUrl))
            {
                url.Append("&BackURL=");
                url.Append(Server.UrlEncode(backUrl));
            }
            return url.ToString();
        }

        private string BuildServiceLogInfoUrl()
        {
            StringBuilder url = new StringBuilder("Admin_ServiceLogInfo.aspx?ID=");
            url.Append(ServiceLog_List.id);
            if (!string.IsNullOrWhiteSpace(MenuId))
            {
                url.Append("&MenuId=");
                url.Append(Server.UrlEncode(MenuId));
            }
            string backUrl = Request.QueryString["BackURL"];
            if (!string.IsNullOrWhiteSpace(backUrl))
            {
                url.Append("&BackURL=");
                url.Append(Server.UrlEncode(backUrl));
            }
            return url.ToString();
        }

        private string BuildCleanServiceLogInfoUrl()
        {
            StringBuilder url = new StringBuilder("Admin_ServiceLogInfo.aspx?ID=");
            url.Append(ServiceLog_List.id);
            if (!string.IsNullOrWhiteSpace(MenuId))
            {
                url.Append("&MenuId=");
                url.Append(Server.UrlEncode(MenuId));
            }
            return url.ToString();
        }

        private void AddAuditServiceLogInfo(string text)
        {
            ServiceLogInfo_List item = new ServiceLogInfo_List();
            item.ServiceLog_Id = ServiceLog_List.id;
            item.info_ = Function.HtmlEncode(text);
            item.type = 2;
            item.addtime = DateTime.Now;
            item.adminname = Cookie.GetCookie("LMS_AdminName");
            ServiceLogInfo_Listbll.Add(item, "id");
        }

        private string GetLiteratureCommentStatusText(int status, int isDeleted)
        {
            if (isDeleted == 1 || status == 3)
            {
                return "已删除";
            }
            if (status == 1)
            {
                return "已通过";
            }
            if (status == 2)
            {
                return "已驳回";
            }
            return "待审核";
        }

        protected void OnClick_AddUp(object sender, EventArgs e)
        {
            isLoading = false;
            AddUp.Visible = false;
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);

            string BackURL = GetBackUrl();
            ServiceLog_List = ServiceLog_ListBll.SelectSingle("id=" + Function.ConvertTo<int>(Function.GetRequest("id"), 0));
            if (ServiceLog_List != null && ServiceLog_List.id > 0)
            {
                user_list user_list = user_listbll.SelectSingle("id=" + ServiceLog_List.userid);
                if (user_list != null && user_list.id > 0)
                {
                    string form_info_ = Function.HtmlSqlEncode(Function.FormRequest("info_"));
                    if (!string.IsNullOrWhiteSpace(form_info_))
                    {
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("insert into ServiceLogInfo_List(");
                        strSql.Append("ServiceLog_Id, info_, type, addtime, adminname)");
                        strSql.Append(" values (");
                        strSql.Append(" @ServiceLog_Id, @info_, @type, @addtime, @adminname)");
                        strSql.Append(";select @@IDENTITY");
                        SqlParameter[] parameters = {
                        new SqlParameter("@ServiceLog_Id", SqlDbType.Int),
                          new SqlParameter("@info_",SqlDbType.NVarChar,-1),
                          new SqlParameter("@type",SqlDbType.Int),
                          new SqlParameter("@addtime",SqlDbType.DateTime),
                          new SqlParameter("@adminname",SqlDbType.NVarChar,250)
                                    };
                        parameters[0].Value = ServiceLog_List.id;
                        parameters[1].Value = form_info_;
                        parameters[2].Value = 2;
                        parameters[3].Value = DateTime.Now;
                        parameters[4].Value = Cookie.GetCookie("LMS_AdminName");
                        string sql = "UPDATE ServiceLog_List SET status=1, uptime = GETDATE() WHERE id=" + ServiceLog_List.id;
                        sql += "INSERT INTO NoticeLog_List (name, type, addtime, userid, status, url,info_)  VALUES ('您提交的服务工单已经有最新回复咯！', 1, GETDATE(), " + ServiceLog_List.userid + ", 0, '/User/ServiceLog_" + ServiceLog_List.id + "','您的服务工单（标题：[" + ServiceLog_List.name + "]）已回复，点击查看详情。')";
                        int addid = ServiceLogInfo_Listbll.Add_R_Id_(parameters, strSql, sql);
                        if (addid > 0)
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "回复工单成功!", BackURL, 0);
                        }
                        else
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "回复工单失败!", BackURL, 2);
                        }
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "回复不能为空！", BackURL, 2);
                    }
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "提交人错误！", "Admin_ServiceLogList.aspx?MenuId=" + MenuId, 2);
                }
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "请求参数错误！", "Admin_ServiceLogList.aspx?MenuId=" + MenuId, 2);
            }
        }
    }
}
