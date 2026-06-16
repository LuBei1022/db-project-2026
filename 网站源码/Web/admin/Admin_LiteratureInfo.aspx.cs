using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Web.admin
{
    public partial class Admin_LiteratureInfo : System.Web.UI.Page
    {
        private readonly BLLBase<Literature> literatureBll = new BLLBase<Literature>();
        private readonly BLLBase<LiteratureCategory> categoryBll = new BLLBase<LiteratureCategory>();
        private readonly BLLBase<LiteratureComment> literatureCommentBll = new BLLBase<LiteratureComment>();
        private readonly BLLBase<user_list> userBll = new BLLBase<user_list>();
        public bool isLoading = false;
        public int LiteratureId = 0;
        public string MenuId = Function.GetRequest("MenuId");
        public string BackUrl = "Admin_LiteratureList.aspx?Mode=Approved";
        public string TitleHtml = "文献详情";
        public string MetaHtml = string.Empty;
        public string InfoGridHtml = string.Empty;
        public int LikeCount = 0;
        public int FavoriteCount = 0;
        public int CommentCount = 0;
        public int DownloadPoints = 0;
        public string PdfHtml = string.Empty;
        public string CommentHtml = string.Empty;
        public string TrendLabelsJson = "[]";
        public string TrendLikesJson = "[]";
        public string TrendFavoritesJson = "[]";
        public string TrendCommentsJson = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            LiteratureId = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            string back = Function.GetRequest("BackURL");
            if (!string.IsNullOrWhiteSpace(back))
            {
                BackUrl = Server.UrlDecode(back);
            }
            else if (!string.IsNullOrWhiteSpace(MenuId))
            {
                BackUrl = "Admin_LiteratureList.aspx?Mode=Approved&MenuId=" + MenuId;
            }

            Literature literature = literatureBll.SelectSingle("id=" + LiteratureId + " and status<>-1");
            if (literature == null || literature.id <= 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "未找到对应的文献记录！", BackUrl, 1);
                return;
            }

            BindInfo(literature);
        }

        private void BindInfo(Literature literature)
        {
            string title = Function.HtmlDiscode(literature.title);
            string authors = LiteratureRelationSync.GetAuthorNames(literature.id);
            string tags = LiteratureRelationSync.GetTagNames(literature.id);
            LikeCount = GetScalarCount("select count(1) from LiteratureLike where literature_id=" + literature.id);
            FavoriteCount = GetScalarCount("select count(1) from LiteratureFavorite where literature_id=" + literature.id);
            DownloadPoints = literature.download_points;
            CommentCount = GetScalarCount(GetCommentSql(literature.id, "count(1)", string.Empty));

            TitleHtml = Server.HtmlEncode(title);
            MetaHtml = Server.HtmlEncode(authors) + " | " + Server.HtmlEncode(literature.publish_year.HasValue ? literature.publish_year.Value.ToString() : "年份暂无") + " | " + Server.HtmlEncode(Function.HtmlDiscode(literature.source_type));

            StringBuilder grid = new StringBuilder();
            AppendInfo(grid, "文献ID", literature.id.ToString());
            AppendInfo(grid, "作者", authors);
            AppendInfo(grid, "分类", GetCategoryName(literature.category_id));
            AppendInfo(grid, "标签", tags);
            AppendInfo(grid, "DOI", Function.HtmlDiscode(literature.doi));
            AppendInfo(grid, "期刊", Function.HtmlDiscode(literature.journal_name));
            AppendInfo(grid, "会议", Function.HtmlDiscode(literature.conference_name));
            AppendInfo(grid, "出版社", Function.HtmlDiscode(literature.publisher));
            AppendInfo(grid, "卷期页", JoinParts(Function.HtmlDiscode(literature.volume), Function.HtmlDiscode(literature.issue), Function.HtmlDiscode(literature.pages)));
            AppendInfo(grid, "作者单位", Function.HtmlDiscode(literature.institution));
            AppendInfoHtml(grid, "\u4F5C\u8005\u673A\u6784\u5BF9\u5E94", GetAuthorInstitutionHtml(literature.id));
            AppendInfo(grid, "关键词", Function.HtmlDiscode(literature.keywords));
            AppendInfo(grid, "摘要", Function.HtmlDiscode(literature.abstract_text));
            AppendInfo(grid, "合并投稿记录", GetMergedSubmissionSummary(literature.id));
            InfoGridHtml = grid.ToString();
            PdfHtml = GetPdfHtml(literature.id);
            CommentHtml = GetCommentHtml(literature.id);

            BindTrend(literature.id);
        }

        private string GetPdfHtml(int literatureId)
        {
            LiteratureFile file = LiteratureRelationSync.GetPrimaryFile(literatureId);
            if (file == null || file.id <= 0 || string.IsNullOrWhiteSpace(file.file_path))
            {
                return "<div class=\"lit-pdf-empty\">&#26242;&#26080; PDF &#38468;&#20214;</div>";
            }

            string filePath = Function.HtmlDiscode(file.file_path).Replace("\\", "/").TrimStart('/');
            string fileName = Function.HtmlDiscode(file.file_name);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "PDF";
            }
            string url = "../A_UpLoad/upload_file/" + filePath;
            StringBuilder html = new StringBuilder();
            html.Append("<div class=\"lit-pdf-head\"><strong>");
            html.Append(Server.HtmlEncode(fileName));
            html.Append("</strong><div><a class=\"btn btn-info\" target=\"_blank\" href=\"");
            html.Append(Server.HtmlEncode(url));
            html.Append("\">&#26032;&#31383;&#21475;&#26597;&#30475;</a><a class=\"btn btn-secondary\" href=\"");
            html.Append(Server.HtmlEncode(url));
            html.Append("\" download=\"");
            html.Append(Server.HtmlEncode(fileName));
            html.Append("\">&#19979;&#36733;&#38468;&#20214;</a></div></div>");
            html.Append("<iframe class=\"lit-pdf-frame\" src=\"");
            html.Append(Server.HtmlEncode(url));
            html.Append("\"></iframe>");
            return html.ToString();
        }

        private void BindTrend(int literatureId)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<string> labels = new List<string>();
            List<int> likes = new List<int>();
            List<int> favorites = new List<int>();
            List<int> comments = new List<int>();
            DateTime startDate = DateTime.Today.AddDays(-13);
            Dictionary<string, int> likeMap = GetDailyTrend("LiteratureLike", "addtime", literatureId, startDate);
            Dictionary<string, int> favoriteMap = GetDailyTrend("LiteratureFavorite", "addtime", literatureId, startDate);
            Dictionary<string, int> commentMap = GetDailyCommentTrend(literatureId, startDate);
            for (int i = 0; i < 14; i++)
            {
                DateTime day = startDate.AddDays(i);
                string key = day.ToString("yyyy-MM-dd");
                labels.Add(day.ToString("MM-dd"));
                likes.Add(likeMap.ContainsKey(key) ? likeMap[key] : 0);
                favorites.Add(favoriteMap.ContainsKey(key) ? favoriteMap[key] : 0);
                comments.Add(commentMap.ContainsKey(key) ? commentMap[key] : 0);
            }
            TrendLabelsJson = serializer.Serialize(labels);
            TrendLikesJson = serializer.Serialize(likes);
            TrendFavoritesJson = serializer.Serialize(favorites);
            TrendCommentsJson = serializer.Serialize(comments);
        }

        private Dictionary<string, int> GetDailyTrend(string tableName, string dateField, int literatureId, DateTime startDate)
        {
            Dictionary<string, int> map = new Dictionary<string, int>();
            string sql = "select convert(varchar(10)," + dateField + ",120) as day_key,count(1) as num from " + tableName + " where literature_id=" + literatureId + " and " + dateField + ">='" + startDate.ToString("yyyy-MM-dd") + "' group by convert(varchar(10)," + dateField + ",120)";
            DataTable dt = literatureBll.GetDatatable(sql);
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    map[Convert.ToString(row["day_key"])] = Function.ConvertTo<int>(Convert.ToString(row["num"]), 0);
                }
                dt.Dispose();
            }
            return map;
        }

        private Dictionary<string, int> GetDailyCommentTrend(int literatureId, DateTime startDate)
        {
            Dictionary<string, int> map = new Dictionary<string, int>();
            string sql = GetCommentSql(literatureId, "convert(varchar(10),addtime,120) as day_key,count(1) as num", " and addtime>='" + startDate.ToString("yyyy-MM-dd") + "' group by convert(varchar(10),addtime,120)");
            DataTable dt = literatureBll.GetDatatable(sql);
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    map[Convert.ToString(row["day_key"])] = Function.ConvertTo<int>(Convert.ToString(row["num"]), 0);
                }
                dt.Dispose();
            }
            return map;
        }

        private int GetScalarCount(string sql)
        {
            DataTable dt = literatureBll.GetDatatable(sql);
            int count = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                count = Function.ConvertTo<int>(Convert.ToString(dt.Rows[0][0]), 0);
                dt.Dispose();
            }
            return count;
        }
        private string GetCommentSql(int literatureId, string fields, string suffix)
        {
            return @"select " + fields + @"
from LiteratureComment
where parent_id=0
  and is_deleted=0
  and status=1
  and (canonical_literature_id=" + literatureId + @" or literature_id=" + literatureId + @")" + suffix;
        }

        private string GetCommentHtml(int literatureId)
        {
            DataTable commentDt = literatureCommentBll.GetDatatable(GetCommentSql(literatureId, "top 50 id,content as comment_text,addtime,userid", " order by updatetime desc, addtime desc, id desc"));
            int count = commentDt != null ? commentDt.Rows.Count : 0;
            StringBuilder html = new StringBuilder();
            html.Append("<div class=\"lit-admin-comments\">");
            if (count == 0)
            {
                html.Append("<div class=\"lit-admin-comment-empty\">&#26242;&#26080;&#20844;&#24320;&#35780;&#35770;</div>");
            }
            else
            {
                foreach (DataRow row in commentDt.Rows)
                {
                    int userId = Function.ConvertTo<int>(Convert.ToString(row["userid"]), 0);
                    int commentId = Function.ConvertTo<int>(Convert.ToString(row["id"]), 0);
                    user_list commentUser = userBll.SelectSingle("id=" + userId);
                    string userName = GetDisplayUserName(commentUser, userId);
                    DateTime addtime = Function.ConvertTo<DateTime>(Convert.ToString(row["addtime"]), DateTime.MinValue);
                    html.Append("<article class=\"lit-admin-comment\"><div class=\"lit-admin-comment-head\"><strong>");
                    html.Append(Server.HtmlEncode(userName));
                    html.Append("</strong><span>");
                    html.Append(addtime == DateTime.MinValue ? string.Empty : addtime.ToString("yyyy-MM-dd HH:mm"));
                    html.Append("</span><a href=\"Admin_LiteratureCommentList.aspx?MenuId=");
                    html.Append(MenuId);
                    html.Append("&LiteratureId=");
                    html.Append(literatureId);
                    html.Append("&CommentId=");
                    html.Append(commentId);
                    html.Append("&BackURL=");
                    html.Append(Function.GetEncodeURL());
                    html.Append("\">&#31649;&#29702;&#35780;&#35770;</a>");
                    html.Append("</div><div class=\"lit-admin-comment-text\">");
                    html.Append(FormatPublicText(Function.HtmlDiscode(Convert.ToString(row["comment_text"]))));
                    html.Append("</div>");
                    html.Append(GetLiteratureCommentReplyHtml(commentId));
                    html.Append("</article>");
                }
            }
            html.Append("</div>");
            if (commentDt != null)
            {
                commentDt.Dispose();
            }
            return html.ToString();
        }
        private string GetLiteratureCommentReplyHtml(int parentCommentId)
        {
            DataTable replyDt = literatureCommentBll.GetDatatable("select content,addtime,userid from LiteratureComment where parent_id=" + parentCommentId + " and is_deleted=0 and status=1 order by addtime asc,id asc");
            if (replyDt == null || replyDt.Rows.Count == 0)
            {
                if (replyDt != null)
                {
                    replyDt.Dispose();
                }
                return string.Empty;
            }
            StringBuilder html = new StringBuilder();
            html.Append("<div class=\"lit-admin-replies\">");
            foreach (DataRow reply in replyDt.Rows)
            {
                int userId = Function.ConvertTo<int>(Convert.ToString(reply["userid"]), 0);
                user_list replyUser = userId > 0 ? userBll.SelectSingle("id=" + userId) : null;
                string displayName = userId > 0 ? GetDisplayUserName(replyUser, userId) : "\u7BA1\u7406\u5458";
                DateTime addtime = Function.ConvertTo<DateTime>(Convert.ToString(reply["addtime"]), DateTime.MinValue);
                html.Append("<div class=\"lit-admin-reply\"><strong>");
                html.Append(Server.HtmlEncode(displayName));
                html.Append("</strong><span>");
                html.Append(addtime == DateTime.MinValue ? string.Empty : addtime.ToString("yyyy-MM-dd HH:mm"));
                html.Append("</span><p>");
                html.Append(FormatPublicText(Function.HtmlDiscode(Convert.ToString(reply["content"]))));
                html.Append("</p></div>");
            }
            html.Append("</div>");
            replyDt.Dispose();
            return html.ToString();
        }
        private string GetDisplayUserName(user_list user, int userId)
        {
            if (user != null && user.id > 0)
            {
                string name = Function.HtmlDiscode(user.name);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }
                string tel = Function.HtmlDiscode(user.tel);
                if (!string.IsNullOrWhiteSpace(tel))
                {
                    return tel.Length > 4 ? "\u7528\u6237 " + tel.Substring(tel.Length - 4) : "\u7528\u6237 " + tel;
                }
            }
            return userId > 0 ? "\u7528\u6237 " + userId : "\u533F\u540D\u7528\u6237";
        }
        private string FormatPublicText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "&#26242;&#26080;&#20869;&#23481;";
            }
            return Server.HtmlEncode(text).Replace("\r\n", "\n").Replace("\n", "<br />");
        }

        private string GetCategoryName(int categoryId)
        {
            LiteratureCategory category = categoryBll.SelectSingle("id=" + categoryId);
            if (category != null && category.id > 0)
            {
                return Function.HtmlDiscode(category.name);
            }
            return "未分类";
        }

        private string GetAuthorInstitutionHtml(int literatureId)
        {
            DataTable dt = literatureBll.GetDatatable(@"
select
    m.author_order,
    coalesce(nullif(a.name_cn,N''),nullif(a.name_en,N''),N'未命名作者') as author_name,
    m.affiliation_text,
    (
        select string_agg(coalesce(nullif(i.name_cn,N''),nullif(i.name_en,N''),nullif(aim.affiliation_text,N'')),N'；') within group (order by aim.institution_order, aim.id)
        from LiteratureAuthorInstitutionMap aim
        left join Institution i on i.id=aim.institution_id and i.status<>-1
        where aim.literature_author_map_id=m.id
           or (isnull(aim.literature_author_map_id,0)=0 and aim.literature_id=m.literature_id and aim.author_id=m.author_id)
    ) as institution_names
from LiteratureAuthorMap m
inner join Author a on a.id=m.author_id
where m.literature_id=" + literatureId + @"
order by m.author_order,m.id");
            if (dt == null || dt.Rows.Count <= 0)
            {
                if (dt != null)
                {
                    dt.Dispose();
                }
                return string.Empty;
            }

            StringBuilder html = new StringBuilder();
            html.Append("<div class=\"lit-admin-author-affiliations\">");
            foreach (DataRow row in dt.Rows)
            {
                string authorName = Function.HtmlDiscode(Convert.ToString(row["author_name"]));
                string institutionNames = Function.HtmlDiscode(Convert.ToString(row["institution_names"]));
                string affiliationText = Function.HtmlDiscode(Convert.ToString(row["affiliation_text"]));
                if (string.IsNullOrWhiteSpace(institutionNames))
                {
                    institutionNames = affiliationText;
                }
                if (string.IsNullOrWhiteSpace(institutionNames))
                {
                    institutionNames = "\u672A\u5339\u914D\u673A\u6784";
                }

                html.Append("<div><strong>");
                html.Append(Server.HtmlEncode(authorName));
                html.Append("</strong><span>");
                html.Append(Server.HtmlEncode(institutionNames));
                html.Append("</span></div>");
            }
            html.Append("</div>");
            dt.Dispose();
            return html.ToString();
        }

        private void AppendInfo(StringBuilder grid, string label, string value)
        {
            grid.Append("<div class=\"label\">");
            grid.Append(Server.HtmlEncode(label));
            grid.Append("</div><div class=\"value\">");
            grid.Append(Server.HtmlEncode(string.IsNullOrWhiteSpace(value) ? "暂无" : value));
            grid.Append("</div>");
        }

        private void AppendInfoHtml(StringBuilder grid, string label, string htmlValue)
        {
            grid.Append("<div class=\"label\">");
            grid.Append(Server.HtmlEncode(label));
            grid.Append("</div><div class=\"value\">");
            grid.Append(string.IsNullOrWhiteSpace(htmlValue) ? "&#26242;&#26080;" : htmlValue);
            grid.Append("</div>");
        }

        private string GetMergedSubmissionSummary(int masterId)
        {
            DataTable dt = literatureBll.GetDatatable(@"
select top 20
    l.id,
    l.title,
    l.status,
    l.addtime,
    u.name as user_name
from Literature l
left join user_list u on u.id=l.userid
where l.status<>-1 and l.canonical_literature_id=" + masterId + @"
order by l.addtime desc,l.id desc");
            if (dt == null || dt.Rows.Count <= 0)
            {
                return string.Empty;
            }

            List<string> rows = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                int id = Function.ConvertTo<int>(Convert.ToString(row["id"]), 0);
                string userName = Function.HtmlDiscode(Convert.ToString(row["user_name"]));
                string title = Function.HtmlDiscode(Convert.ToString(row["title"]));
                DateTime addtime = Function.ConvertTo<DateTime>(Convert.ToString(row["addtime"]), DateTime.MinValue);
                rows.Add("ID " + id + " / " + (string.IsNullOrWhiteSpace(userName) ? "用户未知" : userName) + " / " + (addtime == DateTime.MinValue ? "时间未知" : addtime.ToString("yyyy-MM-dd HH:mm")) + " / " + title);
            }
            dt.Dispose();
            return string.Join("；", rows.ToArray());
        }

        private string JoinParts(params string[] parts)
        {
            List<string> values = new List<string>();
            foreach (string part in parts)
            {
                if (!string.IsNullOrWhiteSpace(part))
                {
                    values.Add(part);
                }
            }
            return values.Count == 0 ? "暂无" : string.Join(" / ", values.ToArray());
        }
    }
}


