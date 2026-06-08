using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Text;

namespace Web.admin
{
    public partial class Admin_LiteratureCommentList : System.Web.UI.Page
    {
        private readonly BLLBase<LiteratureComment> commentBll = new BLLBase<LiteratureComment>();
        private readonly BLLBase<Literature> literatureBll = new BLLBase<Literature>();
        private readonly BLLBase<user_list> userBll = new BLLBase<user_list>();

        public string MenuId = Function.GetRequest("MenuId");
        public string BackUrl = "Admin_LiteratureList.aspx?Mode=Approved";
        public string FilterTabsHtml = string.Empty;
        public string ListHtml = string.Empty;

        private int literatureIdFilter = 0;
        private int commentIdFilter = 0;
        private int statusFilter = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            literatureIdFilter = Function.ConvertTo<int>(Function.GetRequest("LiteratureId"), 0);
            commentIdFilter = Function.ConvertTo<int>(Function.GetRequest("CommentId"), 0);
            statusFilter = Function.ConvertTo<int>(Function.GetRequest("Status"), 0);
            string back = Function.GetRequest("BackURL");
            if (!string.IsNullOrWhiteSpace(back))
            {
                BackUrl = Server.UrlDecode(back);
            }
            else if (!string.IsNullOrWhiteSpace(MenuId))
            {
                BackUrl = "Admin_LiteratureList.aspx?Mode=Approved&MenuId=" + Server.UrlEncode(MenuId);
            }

            if (HandleAction())
            {
                return;
            }

            FilterTabsHtml = BuildFilterTabs();
            ListHtml = BuildList();
        }

        private bool HandleAction()
        {
            string action = Function.GetRequest("Action");
            int id = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            if (string.IsNullOrWhiteSpace(action) || id <= 0)
            {
                return false;
            }

            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string where = "id=" + id;
            if (action.Equals("Approve", StringComparison.OrdinalIgnoreCase))
            {
                commentBll.Update("status=1,is_deleted=0,reviewed_by=0,review_time='" + now + "',review_remark=N'后台审核通过',updatetime='" + now + "'", where);
                statusFilter = 1;
            }
            else if (action.Equals("Reject", StringComparison.OrdinalIgnoreCase))
            {
                commentBll.Update("status=2,is_deleted=0,reviewed_by=0,review_time='" + now + "',review_remark=N'后台审核驳回',updatetime='" + now + "'", where);
                statusFilter = 2;
            }
            else if (action.Equals("Delete", StringComparison.OrdinalIgnoreCase))
            {
                commentBll.Update("status=3,is_deleted=1,delete_time='" + now + "',reviewed_by=0,review_time='" + now + "',review_remark=N'后台删除',updatetime='" + now + "'", where);
                statusFilter = 3;
            }

            Response.Redirect(BuildBaseUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
            return true;
        }

        private string BuildList()
        {
            string condition = "c.parent_id=0";
            if (statusFilter == 3)
            {
                condition += " and c.is_deleted=1";
            }
            else
            {
                condition += " and c.is_deleted=0 and c.status=" + statusFilter;
            }
            if (literatureIdFilter > 0)
            {
                condition += " and (c.literature_id=" + literatureIdFilter + " or c.canonical_literature_id=" + literatureIdFilter + ")";
            }
            if (commentIdFilter > 0)
            {
                condition += " and c.id=" + commentIdFilter;
            }

            string sql = @"
select top 200
    c.id,
    c.literature_id,
    c.canonical_literature_id,
    c.userid,
    c.content,
    c.status,
    c.is_deleted,
    c.addtime,
    c.review_time,
    c.review_remark,
    l.title,
    u.name as user_name,
    u.tel as user_tel
from LiteratureComment c
left join Literature l on l.id=c.literature_id
left join user_list u on u.id=c.userid
where " + condition + @"
order by c.addtime desc,c.id desc";
            DataTable dt = commentBll.GetDatatable(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                if (dt != null)
                {
                    dt.Dispose();
                }
                return "<div class=\"empty\">当前筛选条件下暂无文献评论。</div>";
            }

            StringBuilder html = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                int id = Function.ConvertTo<int>(Convert.ToString(row["id"]), 0);
                int literatureId = Function.ConvertTo<int>(Convert.ToString(row["literature_id"]), 0);
                int status = Function.ConvertTo<int>(Convert.ToString(row["status"]), 0);
                int isDeleted = Function.ConvertTo<int>(Convert.ToString(row["is_deleted"]), 0);
                string userName = GetDisplayUserName(Convert.ToString(row["user_name"]), Convert.ToString(row["user_tel"]), Function.ConvertTo<int>(Convert.ToString(row["userid"]), 0));
                string title = Function.HtmlDiscode(Convert.ToString(row["title"]));
                string content = Function.HtmlDiscode(Convert.ToString(row["content"]));
                DateTime addtime = Function.ConvertTo<DateTime>(Convert.ToString(row["addtime"]), DateTime.MinValue);

                html.Append("<article class=\"comment-card\"><div class=\"comment-title\">");
                html.Append(Server.HtmlEncode(string.IsNullOrWhiteSpace(title) ? "文献记录不存在或已删除" : title));
                html.Append("</div><div class=\"comment-meta\"><span>ID: ");
                html.Append(id);
                html.Append("</span><span>文献ID: <a target=\"_blank\" href=\"Admin_LiteratureInfo.aspx?ID=");
                html.Append(literatureId);
                html.Append("&MenuId=");
                html.Append(Server.UrlEncode(MenuId));
                html.Append("\">");
                html.Append(literatureId);
                html.Append("</a></span><span>用户: ");
                html.Append(Server.HtmlEncode(userName));
                html.Append("</span><span>提交时间: ");
                html.Append(addtime == DateTime.MinValue ? string.Empty : addtime.ToString("yyyy-MM-dd HH:mm"));
                html.Append("</span><span>状态: ");
                html.Append(GetStatusText(status, isDeleted));
                html.Append("</span></div><div class=\"comment-content\">");
                html.Append(Server.HtmlEncode(content).Replace("\r\n", "\n").Replace("\n", "<br />"));
                html.Append("</div><div class=\"comment-actions\">");
                AppendActionLinks(html, id, status, isDeleted);
                html.Append("</div></article>");
            }
            dt.Dispose();
            return html.ToString();
        }

        private void AppendActionLinks(StringBuilder html, int id, int status, int isDeleted)
        {
            if (isDeleted == 1)
            {
                return;
            }
            if (status != 1)
            {
                html.Append("<a class=\"btn btn-success btn-sm\" href=\"");
                html.Append(BuildActionUrl(id, "Approve"));
                html.Append("\">审核通过</a>");
            }
            if (status != 2)
            {
                html.Append("<a class=\"btn btn-warning btn-sm\" href=\"");
                html.Append(BuildActionUrl(id, "Reject"));
                html.Append("\">驳回</a>");
            }
            html.Append("<a class=\"btn btn-danger btn-sm\" onclick=\"return confirm('确认软删除该评论？');\" href=\"");
            html.Append(BuildActionUrl(id, "Delete"));
            html.Append("\">删除</a>");
        }

        private string BuildFilterTabs()
        {
            StringBuilder html = new StringBuilder();
            AppendFilterTab(html, 0, "待审核");
            AppendFilterTab(html, 1, "已通过");
            AppendFilterTab(html, 2, "已驳回");
            AppendFilterTab(html, 3, "已删除");
            return html.ToString();
        }

        private void AppendFilterTab(StringBuilder html, int status, string text)
        {
            html.Append("<a");
            if (statusFilter == status)
            {
                html.Append(" class=\"active\"");
            }
            html.Append(" href=\"");
            html.Append(BuildBaseUrl(status));
            html.Append("\">");
            html.Append(Server.HtmlEncode(text));
            html.Append("</a>");
        }

        private string BuildActionUrl(int id, string action)
        {
            return BuildBaseUrl(statusFilter) + "&Action=" + Server.UrlEncode(action) + "&ID=" + id;
        }

        private string BuildBaseUrl()
        {
            return BuildBaseUrl(statusFilter);
        }

        private string BuildBaseUrl(int status)
        {
            StringBuilder url = new StringBuilder("Admin_LiteratureCommentList.aspx?Status=");
            url.Append(status);
            if (!string.IsNullOrWhiteSpace(MenuId))
            {
                url.Append("&MenuId=");
                url.Append(Server.UrlEncode(MenuId));
            }
            if (literatureIdFilter > 0)
            {
                url.Append("&LiteratureId=");
                url.Append(literatureIdFilter);
            }
            if (commentIdFilter > 0)
            {
                url.Append("&CommentId=");
                url.Append(commentIdFilter);
            }
            if (!string.IsNullOrWhiteSpace(BackUrl))
            {
                url.Append("&BackURL=");
                url.Append(Server.UrlEncode(BackUrl));
            }
            return url.ToString();
        }

        private string GetDisplayUserName(string name, string tel, int userId)
        {
            name = Function.HtmlDiscode(name);
            tel = Function.HtmlDiscode(tel);
            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }
            if (!string.IsNullOrWhiteSpace(tel))
            {
                return tel.Length > 4 ? "用户 " + tel.Substring(tel.Length - 4) : "用户 " + tel;
            }
            return userId > 0 ? "用户 " + userId : "匿名用户";
        }

        private string GetStatusText(int status, int isDeleted)
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
    }
}
