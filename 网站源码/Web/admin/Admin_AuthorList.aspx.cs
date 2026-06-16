using BLL;
using LiteratureManager.Common;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Web.admin
{
    public partial class Admin_AuthorList : System.Web.UI.Page
    {
        private readonly BLLBase<Author> authorBll = new BLLBase<Author>();
        public bool isLoading = false;
        public string MenuId = Function.GetRequest("MenuId");
        public string Key = string.Empty;
        public string ListHtml = string.Empty;
        public string PagerHtml = string.Empty;
        private const int PageSize = 20;

        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            Key = Function.GetRequest("Key").Trim();
            BindData();
        }

        private void BindData()
        {
            string condition = BuildCondition();
            int pageIndex = Math.Max(1, Function.ConvertTo<int>(Function.GetRequest("Page"), 1));
            int total = GetScalarInt("select count(1) from Author a where " + condition);
            int startRow = (pageIndex - 1) * PageSize + 1;
            int endRow = pageIndex * PageSize;

            string sql = @"
select *
from
(
    select
        row_number() over(order by a.addtime desc,a.id desc) as row_no,
        a.id,
        a.name_cn,
        a.name_en,
        a.current_institution_name,
        a.current_institution_literature_id,
        a.current_institution_sort_date,
        a.current_institution_precision,
        a.institution,
        a.orcid,
        a.email,
        a.status,
        a.addtime,
        (
            select count(distinct m.literature_id)
            from LiteratureAuthorMap m
            inner join Literature l on l.id=m.literature_id
            where m.author_id=a.id and l.status<>-1
        ) as literature_count
    from Author a
    where " + condition + @"
) t
where t.row_no between " + startRow + " and " + endRow + @"
order by t.row_no";
            DataTable dt = authorBll.GetDatatable(sql);
            ListHtml = BuildListHtml(dt);
            PagerHtml = BuildPager(total, pageIndex);
            if (dt != null)
            {
                dt.Dispose();
            }
        }

        private string BuildCondition()
        {
            string condition = "a.status<>-1";
            if (!string.IsNullOrWhiteSpace(Key))
            {
                string safeKey = SqlLiteral(Function.HtmlEncode(Key));
                condition += @" and
(
    a.name_cn like N'%" + safeKey + @"%'
    or a.name_en like N'%" + safeKey + @"%'
    or a.current_institution_name like N'%" + safeKey + @"%'
    or a.institution like N'%" + safeKey + @"%'
    or exists
    (
        select 1
        from LiteratureAuthorInstitutionMap aim
        left join Institution i on i.id=aim.institution_id and i.status<>-1
        where aim.author_id=a.id
          and (
              aim.affiliation_text like N'%" + safeKey + @"%'
              or i.name_cn like N'%" + safeKey + @"%'
              or i.name_en like N'%" + safeKey + @"%'
          )
    )
    or exists
    (
        select 1
        from LiteratureAuthorMap m
        inner join Literature l on l.id=m.literature_id
        where m.author_id=a.id
          and l.status<>-1
          and l.title like N'%" + safeKey + @"%'
    )
)";
            }
            return condition;
        }

        private string BuildListHtml(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return "<tr><td colspan=\"6\" class=\"author-empty\">暂无作者数据</td></tr>";
            }

            StringBuilder html = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                int id = ToInt(row["id"]);
                string name = GetAuthorName(row);
                string currentInstitution = Decode(row["current_institution_name"]);
                if (string.IsNullOrWhiteSpace(currentInstitution))
                {
                    currentInstitution = Decode(row["institution"]);
                }
                int literatureCount = ToInt(row["literature_count"]);
                DateTime addtime = Function.ConvertTo<DateTime>(Convert.ToString(row["addtime"]), DateTime.MinValue);

                html.Append("<tr>");
                html.Append("<td>").Append(id).Append("</td>");
                html.Append("<td><div class=\"author-main\">").Append(Server.HtmlEncode(name)).Append("</div>");
                html.Append("<div class=\"author-sub\">中文名：").Append(Server.HtmlEncode(Decode(row["name_cn"]))).Append("　英文名：").Append(Server.HtmlEncode(Decode(row["name_en"]))).Append("</div>");
                html.Append("<div class=\"author-sub\">ORCID：").Append(Server.HtmlEncode(Decode(row["orcid"]))).Append("　邮箱：").Append(Server.HtmlEncode(Decode(row["email"]))).Append("</div></td>");
                html.Append("<td>");
                if (!string.IsNullOrWhiteSpace(currentInstitution))
                {
                    html.Append("<span class=\"author-pill\">当前：").Append(Server.HtmlEncode(currentInstitution)).Append("</span>");
                }
                html.Append(BuildHistoricalInstitutionPills(id, currentInstitution, literatureCount));
                if (string.IsNullOrWhiteSpace(currentInstitution) && literatureCount <= 0)
                {
                    html.Append("<span class=\"author-sub\">机构待补充</span>");
                }
                html.Append("</td>");
                html.Append("<td>").Append(literatureCount).Append("</td>");
                html.Append("<td>").Append(addtime == DateTime.MinValue ? "" : addtime.ToString("yyyy-MM-dd HH:mm")).Append("</td>");
                html.Append("<td><div class=\"author-actions\"><a href=\"Admin_AuthorInfo.aspx?ID=").Append(id).Append("&MenuId=").Append(Server.UrlEncode(MenuId)).Append("\">详情</a><a href=\"Admin_AuthorEdit.aspx?Action=Edit&ID=").Append(id).Append("&MenuId=").Append(Server.UrlEncode(MenuId)).Append("\">编辑</a></div></td>");
                html.Append("</tr>");
            }
            return html.ToString();
        }

        private string BuildHistoricalInstitutionPills(int authorId, string currentInstitution, int literatureCount)
        {
            List<string> currentKeys = BuildInstitutionKeys(currentInstitution);
            DataTable dt = authorBll.GetDatatable(@"
select top 8 institution_name, max(sort_date) as last_sort_date, count(distinct literature_id) as paper_count
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

            List<string> items = new List<string>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string institution = Decode(row["institution_name"]);
                    if (string.IsNullOrWhiteSpace(institution))
                    {
                        continue;
                    }
                    if (IsCurrentInstitution(institution, currentKeys))
                    {
                        continue;
                    }
                    if (!ContainsIgnoreCase(items, institution))
                    {
                        items.Add(institution);
                    }
                }
                dt.Dispose();
            }

            if (items.Count == 0 || (literatureCount <= 1 && !string.IsNullOrWhiteSpace(currentInstitution)))
            {
                return string.Empty;
            }

            StringBuilder html = new StringBuilder();
            foreach (string item in items)
            {
                html.Append("<span class=\"author-pill\">").Append(Server.HtmlEncode(item)).Append("</span>");
            }
            return html.ToString();
        }

        private string BuildPager(int total, int pageIndex)
        {
            int pageCount = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
            if (pageCount <= 1)
            {
                return string.Empty;
            }

            StringBuilder html = new StringBuilder();
            html.Append("<div class=\"author-page\">");
            for (int i = 1; i <= pageCount; i++)
            {
                if (i == pageIndex)
                {
                    html.Append("<span>").Append(i).Append("</span>");
                }
                else
                {
                    html.Append("<a href=\"Admin_AuthorList.aspx?MenuId=").Append(Server.UrlEncode(MenuId)).Append("&Key=").Append(Server.UrlEncode(Key)).Append("&Page=").Append(i).Append("\">").Append(i).Append("</a>");
                }
            }
            html.Append("</div>");
            return html.ToString();
        }

        private int GetScalarInt(string sql)
        {
            DataTable dt = authorBll.GetDatatable(sql);
            int value = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                value = ToInt(dt.Rows[0][0]);
                dt.Dispose();
            }
            return value;
        }

        private string GetAuthorName(DataRow row)
        {
            string cn = Decode(row["name_cn"]);
            string en = Decode(row["name_en"]);
            if (!string.IsNullOrWhiteSpace(cn))
            {
                return cn;
            }
            return string.IsNullOrWhiteSpace(en) ? "未命名作者" : en;
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

        private bool ContainsIgnoreCase(List<string> values, string value)
        {
            foreach (string item in values)
            {
                if (string.Equals(item, value, StringComparison.OrdinalIgnoreCase))
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

        private string SqlLiteral(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }
    }
}
