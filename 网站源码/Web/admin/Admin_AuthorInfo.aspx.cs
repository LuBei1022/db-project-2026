using BLL;
using LiteratureManager.Common;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Web.admin
{
    public partial class Admin_AuthorInfo : System.Web.UI.Page
    {
        private readonly BLLBase<Author> authorBll = new BLLBase<Author>();
        public bool isLoading = false;
        public string MenuId = Function.GetRequest("MenuId");
        public int AuthorId = 0;
        public string AuthorName = string.Empty;
        public string BasicHtml = string.Empty;
        public string CurrentInstitutionHtml = string.Empty;
        public string HistoryHtml = string.Empty;
        public string PaperHtml = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            AuthorId = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            Author author = authorBll.SelectSingle("id=" + AuthorId + " and status<>-1");
            if (author == null || author.id <= 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "未找到对应作者记录", "Admin_AuthorList.aspx?MenuId=" + MenuId, 1);
                return;
            }

            BindAuthor(author);
        }

        private void BindAuthor(Author author)
        {
            AuthorName = GetAuthorName(author);
            BasicHtml = BuildBasicHtml(author);
            CurrentInstitutionHtml = BuildCurrentInstitutionHtml(author);
            HistoryHtml = BuildHistoryHtml(author.id, Decode(author.current_institution_name));
            PaperHtml = BuildPaperHtml(author.id);
        }

        private string BuildBasicHtml(Author author)
        {
            StringBuilder html = new StringBuilder();
            AppendField(html, "作者ID", author.id.ToString());
            AppendField(html, "中文名", Decode(author.name_cn));
            AppendField(html, "英文名", Decode(author.name_en));
            AppendField(html, "邮箱", Decode(author.email));
            AppendField(html, "ORCID", Decode(author.orcid));
            AppendField(html, "身份状态", string.IsNullOrWhiteSpace(author.identity_status) ? "auto" : author.identity_status);
            AppendField(html, "状态", author.status == 1 ? "启用" : "停用");
            AppendField(html, "创建时间", author.addtime == DateTime.MinValue ? "" : author.addtime.ToString("yyyy-MM-dd HH:mm"));
            return html.ToString();
        }

        private string BuildCurrentInstitutionHtml(Author author)
        {
            string current = Decode(author.current_institution_name);
            if (string.IsNullOrWhiteSpace(current))
            {
                current = Decode(author.institution);
            }
            if (string.IsNullOrWhiteSpace(current))
            {
                return "<div class=\"author-empty\">暂无当前机构。当前机构会根据该作者最后发表论文中的机构自动计算。</div>";
            }

            StringBuilder html = new StringBuilder();
            html.Append("<span class=\"author-pill\">").Append(Server.HtmlEncode(current)).Append("</span>");
            if (author.current_institution_literature_id.HasValue && author.current_institution_literature_id.Value > 0)
            {
                string title = GetScalarText("select title from Literature where id=" + author.current_institution_literature_id.Value);
                html.Append("<div class=\"author-sub\" style=\"margin-top:8px;\">来源论文：<a class=\"author-paper-title\" href=\"");
                html.Append(Server.HtmlEncode(BuildAdminLiteratureInfoUrl(author.current_institution_literature_id.Value)));
                html.Append("\">");
                html.Append(Server.HtmlEncode(Decode(title)));
                html.Append("</a>");
                if (author.current_institution_sort_date.HasValue)
                {
                    html.Append("　排序日期：").Append(author.current_institution_sort_date.Value.ToString("yyyy-MM-dd"));
                    html.Append("（").Append(Server.HtmlEncode(string.IsNullOrWhiteSpace(author.current_institution_precision) ? "unknown" : author.current_institution_precision)).Append("）");
                }
                html.Append("</div>");
            }
            return html.ToString();
        }

        private string BuildHistoryHtml(int authorId, string currentInstitution)
        {
            List<string> currentKeys = BuildInstitutionKeys(currentInstitution);
            DataTable dt = authorBll.GetDatatable(@"
select
    institution_name,
    min(sort_date) as first_sort_date,
    max(sort_date) as last_sort_date,
    count(distinct literature_id) as paper_count
from
(
    select
        aim.literature_id,
        coalesce(nullif(i.name_cn,N''), nullif(i.name_en,N''), nullif(aim.affiliation_text,N'')) as institution_name,
        coalesce(l.publish_date, case when l.publish_year between 1000 and 9999 then datefromparts(l.publish_year,12,31) end, convert(date,l.addtime)) as sort_date
    from LiteratureAuthorInstitutionMap aim
    inner join Literature l on l.id=aim.literature_id and l.status<>-1
    left join Institution i on i.id=aim.institution_id and i.status<>-1
    where aim.author_id=" + authorId + @"
) q
where ltrim(rtrim(isnull(institution_name,N'')))<>N''
group by institution_name
order by max(sort_date) desc, institution_name asc");

            if (dt == null || dt.Rows.Count == 0)
            {
                return "<div class=\"author-empty\">暂无论文机构记录。</div>";
            }

            StringBuilder html = new StringBuilder();
            int rows = 0;
            html.Append("<table><thead><tr><th>机构</th><th>论文数</th><th>首次出现</th><th>最近出现</th></tr></thead><tbody>");
            foreach (DataRow row in dt.Rows)
            {
                string institution = Decode(row["institution_name"]);
                if (string.IsNullOrWhiteSpace(institution) || IsCurrentInstitution(institution, currentKeys))
                {
                    continue;
                }
                rows++;
                html.Append("<tr><td>").Append(Server.HtmlEncode(institution)).Append("</td>");
                html.Append("<td>").Append(ToInt(row["paper_count"])).Append("</td>");
                html.Append("<td>").Append(FormatDate(row["first_sort_date"])).Append("</td>");
                html.Append("<td>").Append(FormatDate(row["last_sort_date"])).Append("</td></tr>");
            }
            html.Append("</tbody></table>");
            dt.Dispose();

            if (rows == 0)
            {
                return "<div class=\"author-empty\">该作者目前只有当前机构记录，暂无不同的历史/论文机构。</div>";
            }
            return html.ToString();
        }

        private string BuildPaperHtml(int authorId)
        {
            DataTable dt = authorBll.GetDatatable(@"
select *
from
(
    select
        l.id,
        l.title,
        l.publish_year,
        l.publish_month,
        l.publish_day,
        l.publish_date,
        l.publish_date_precision,
        l.source_type,
        l.status,
        l.addtime,
        m.author_order,
        coalesce(
            nullif(
                stuff((
                    select N'；' + coalesce(nullif(i.name_cn,N''), nullif(i.name_en,N''), nullif(aim.affiliation_text,N''))
                    from LiteratureAuthorInstitutionMap aim
                    left join Institution i on i.id=aim.institution_id and i.status<>-1
                    where aim.literature_author_map_id=m.id
                       or (isnull(aim.literature_author_map_id,0)=0 and aim.literature_id=m.literature_id and aim.author_id=m.author_id)
                    order by aim.institution_order, aim.id
                    for xml path(''), type
                ).value('.','nvarchar(max)'),1,1,N''),
                N''
            ),
            nullif(m.affiliation_text,N''),
            N''
        ) as affiliation_text
    from LiteratureAuthorMap m
    inner join Literature l on l.id=m.literature_id
    where m.author_id=" + authorId + @" and l.status<>-1
) q
order by coalesce(q.publish_date, case when q.publish_year between 1000 and 9999 then datefromparts(q.publish_year,12,31) end, convert(date,q.addtime)) desc, q.id desc");
            if (dt == null || dt.Rows.Count == 0)
            {
                return "<div class=\"author-empty\">暂无关联论文。</div>";
            }

            StringBuilder html = new StringBuilder();
            html.Append("<table><thead><tr><th>论文标题</th><th style=\"width:110px;\">发表时间</th><th style=\"width:120px;\">本文机构</th><th style=\"width:90px;\">作者序</th><th style=\"width:110px;\">状态</th></tr></thead><tbody>");
            foreach (DataRow row in dt.Rows)
            {
                int literatureId = ToInt(row["id"]);
                string affiliation = Decode(row["affiliation_text"]);
                html.Append("<tr><td><a class=\"author-paper-title\" href=\"").Append(Server.HtmlEncode(BuildAdminLiteratureInfoUrl(literatureId))).Append("\">").Append(Server.HtmlEncode(Decode(row["title"]))).Append("</a>");
                html.Append(" <a href=\"Admin_LiteratureEdit.aspx?Action=Edit&ID=").Append(literatureId).Append("&MenuId=").Append(Server.UrlEncode(MenuId)).Append("\" style=\"margin-left:8px;\">后台编辑</a></td>");
                html.Append("<td>").Append(Server.HtmlEncode(FormatPublishDate(row))).Append("</td>");
                html.Append("<td>").Append(Server.HtmlEncode(string.IsNullOrWhiteSpace(affiliation) ? "未匹配机构" : affiliation)).Append("</td>");
                html.Append("<td>").Append(ToInt(row["author_order"])).Append("</td>");
                html.Append("<td>").Append(GetLiteratureStatusText(ToInt(row["status"]))).Append("</td></tr>");
            }
            html.Append("</tbody></table>");
            dt.Dispose();
            return html.ToString();
        }

        private void AppendField(StringBuilder html, string label, string value)
        {
            html.Append("<div class=\"author-field\"><span>").Append(Server.HtmlEncode(label)).Append("</span><strong>").Append(Server.HtmlEncode(value ?? string.Empty)).Append("</strong></div>");
        }

        private string FormatPublishDate(DataRow row)
        {
            int year = ToInt(row["publish_year"]);
            int month = ToInt(row["publish_month"]);
            int day = ToInt(row["publish_day"]);
            if (year <= 0)
            {
                return "暂无";
            }
            if (month <= 0)
            {
                return year.ToString();
            }
            if (day <= 0)
            {
                return year.ToString("0000") + "-" + month.ToString("00");
            }
            return year.ToString("0000") + "-" + month.ToString("00") + "-" + day.ToString("00");
        }

        private string BuildAdminLiteratureInfoUrl(int literatureId)
        {
            return "Admin_LiteratureInfo.aspx?MenuId=" + Server.UrlEncode(MenuId)
                + "&ID=" + literatureId
                + "&BackURL=" + Function.GetEncodeURL();
        }

        private string FormatDate(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "";
            }
            DateTime date;
            return DateTime.TryParse(Convert.ToString(value), out date) ? date.ToString("yyyy-MM-dd") : "";
        }

        private string GetLiteratureStatusText(int status)
        {
            switch (status)
            {
                case 1: return "已通过";
                case 2: return "已驳回";
                case 3: return "已合并";
                case 4: return "已应用";
                default: return "待审核";
            }
        }

        private string GetAuthorName(Author author)
        {
            string cn = Decode(author.name_cn);
            string en = Decode(author.name_en);
            if (!string.IsNullOrWhiteSpace(cn))
            {
                return cn;
            }
            return string.IsNullOrWhiteSpace(en) ? "未命名作者" : en;
        }

        private string GetScalarText(string sql)
        {
            DataTable dt = authorBll.GetDatatable(sql);
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToString(dt.Rows[0][0]);
                }
                return string.Empty;
            }
            finally
            {
                if (dt != null)
                {
                    dt.Dispose();
                }
            }
        }

        private List<string> BuildInstitutionKeys(string value)
        {
            List<string> keys = new List<string>();
            foreach (string part in (value ?? string.Empty).Split(new[] { ';', '；', '|', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string key = NormalizeKey(part);
                if (!string.IsNullOrWhiteSpace(key) && !keys.Contains(key))
                {
                    keys.Add(key);
                }
            }
            return keys;
        }

        private bool IsCurrentInstitution(string value, List<string> currentKeys)
        {
            string key = NormalizeKey(value);
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }
            foreach (string currentKey in currentKeys)
            {
                if (key == currentKey || key.Contains(currentKey) || currentKey.Contains(key))
                {
                    return true;
                }
            }
            return false;
        }

        private string NormalizeKey(string value)
        {
            return (Function.HtmlDiscode(value ?? string.Empty)).Replace('\u00A0', ' ').Trim().ToLowerInvariant();
        }

        private string Decode(object value)
        {
            return Function.HtmlDiscode(Convert.ToString(value ?? string.Empty));
        }

        private int ToInt(object value)
        {
            return Function.ConvertTo<int>(Convert.ToString(value), 0);
        }
    }
}
