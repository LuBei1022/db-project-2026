using BLL;
using LiteratureManager.Common;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace Web.admin
{
    public partial class Admin_InstitutionList : System.Web.UI.Page
    {
        private readonly BLLBase<Institution> bll = new BLLBase<Institution>();
        private readonly BLLBase<InstitutionAlias> aliasBll = new BLLBase<InstitutionAlias>();

        public bool isLoading = false;
        public bool IsEditMode = false;
        public string Action = Function.GetRequest("Action");
        public string MenuId = Function.GetRequest("MenuId");
        public string Key = Function.GetRequest("Key");
        public string PageTitle = string.Empty;
        public string ListHtml = string.Empty;
        public string PagerHtml = string.Empty;
        public int Id = 0;
        public string NameCn = string.Empty;
        public string NameEn = string.Empty;
        public string AliasNames = string.Empty;
        public string Country = string.Empty;
        public string Province = string.Empty;
        public string City = string.Empty;
        public string Website = string.Empty;
        public int ParentId = 0;
        public string ParentOptionsHtml = string.Empty;
        public int Status = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            Id = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);

            if (Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                Save();
                return;
            }

            if (Action == "Del")
            {
                Delete();
                return;
            }

            if (Action == "Add" || Action == "Edit")
            {
                LoadForm();
                return;
            }

            BindList();
        }

        private void LoadForm()
        {
            IsEditMode = true;
            PageTitle = Action == "Edit" ? "\u7F16\u8F91\u673A\u6784" : "\u65B0\u589E\u673A\u6784";
            ParentOptionsHtml = BuildParentOptions(0);
            if (Action != "Edit")
            {
                return;
            }

            Institution model = bll.SelectSingle("id=" + Id + " and status<>-1");
            if (model == null || model.id <= 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u672A\u627E\u5230\u5BF9\u5E94\u673A\u6784\u8BB0\u5F55!", "Admin_InstitutionList.aspx?MenuId=" + MenuId, 1);
                return;
            }

            NameCn = Decode(model.name_cn);
            NameEn = Decode(model.name_en);
            AliasNames = Decode(model.alias_names);
            Country = Decode(model.country);
            Province = Decode(model.province);
            City = Decode(model.city);
            Website = Decode(model.website);
            ParentId = model.parent_id.HasValue ? model.parent_id.Value : 0;
            ParentOptionsHtml = BuildParentOptions(ParentId);
            Status = model.status;
        }

        private void BindList()
        {
            int pageSize = 15;
            int pageIndex = Math.Max(1, Function.ConvertTo<int>(Function.GetRequest("Page"), 1));
            string where = "i.status<>-1";
            string safeKey = EncodeSql(Key);
            if (!string.IsNullOrWhiteSpace(safeKey))
            {
                where += " and (i.name_cn like N'%" + safeKey + "%' or i.name_en like N'%" + safeKey + "%' or i.alias_names like N'%" + safeKey + "%' or i.country like N'%" + safeKey + "%' or i.city like N'%" + safeKey + "%' or exists(select 1 from dbo.Institution p where p.id=i.parent_id and p.status<>-1 and (p.name_cn like N'%" + safeKey + "%' or p.name_en like N'%" + safeKey + "%')))";
            }

            int count = Count("select count(1) from dbo.Institution i where " + where);
            int start = (pageIndex - 1) * pageSize + 1;
            int end = pageIndex * pageSize;
            string sql = @"
select *
from
(
    select row_number() over(order by i.updatetime desc,i.id desc) as rn,
           i.id,i.name_cn,i.name_en,i.alias_names,i.country,i.province,i.city,i.website,i.status,i.updatetime,
           coalesce(nullif(p.name_cn,N''), nullif(p.name_en,N''), N'') as parent_name,
           (select count(1) from dbo.LiteratureAuthorInstitutionMap m where m.institution_id=i.id) as paper_count
    from dbo.Institution i
    left join dbo.Institution p on p.id=i.parent_id and p.status<>-1
    where " + where + @"
) t
where t.rn between " + start + " and " + end + @"
order by t.rn";
            DataTable dt = bll.GetDatatable(sql);
            ListHtml = BuildRows(dt);
            PagerHtml = BuildPager(count, pageSize, pageIndex);
        }

        private string BuildRows(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
            {
                return "<tr><td colspan=\"7\" class=\"master-empty\">\u6682\u65E0\u673A\u6784\u6570\u636E</td></tr>";
            }

            StringBuilder html = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                string nameCn = Decode(Convert.ToString(row["name_cn"]));
                string nameEn = Decode(Convert.ToString(row["name_en"]));
                string displayName = !string.IsNullOrWhiteSpace(nameCn) ? nameCn : nameEn;
                string alias = Decode(Convert.ToString(row["alias_names"]));
                string parentName = Decode(Convert.ToString(row["parent_name"]));
                string area = JoinNonEmpty(Decode(Convert.ToString(row["country"])), Decode(Convert.ToString(row["province"])), Decode(Convert.ToString(row["city"])));
                int id = ToInt(row["id"]);

                html.Append("<tr>");
                html.Append("<td>").Append(id).Append("</td>");
                html.Append("<td><div class=\"master-main\">").Append(H(displayName)).Append("</div>");
                html.Append("<div class=\"master-sub\">");
                if (!string.IsNullOrWhiteSpace(nameCn)) html.Append("\u4E2D\u6587\u540D: ").Append(H(nameCn)).Append(" &nbsp; ");
                if (!string.IsNullOrWhiteSpace(nameEn)) html.Append("\u82F1\u6587\u540D: ").Append(H(nameEn));
                if (!string.IsNullOrWhiteSpace(alias)) html.Append("<br />\u522B\u540D: ").Append(H(alias));
                if (!string.IsNullOrWhiteSpace(parentName)) html.Append("<br />\u4E0A\u7EA7\u673A\u6784: ").Append(H(parentName));
                html.Append("</div></td>");
                html.Append("<td>").Append(string.IsNullOrWhiteSpace(area) ? "\u6682\u65E0" : H(area)).Append("</td>");
                html.Append("<td>").Append(ToInt(row["paper_count"])).Append("</td>");
                html.Append("<td>").Append(GetStatusText(ToInt(row["status"]))).Append("</td>");
                html.Append("<td>").Append(ToDate(row["updatetime"]).ToString("yyyy-MM-dd HH:mm")).Append("</td>");
                html.Append("<td><div class=\"master-actions\">");
                html.Append("<a href=\"Admin_InstitutionList.aspx?Action=Edit&ID=").Append(id).Append("&MenuId=").Append(Server.UrlEncode(MenuId)).Append("\">\u7F16\u8F91</a>");
                html.Append("<a href=\"Admin_InstitutionList.aspx?Action=Del&ID=").Append(id).Append("&MenuId=").Append(Server.UrlEncode(MenuId)).Append("\" onclick=\"return confirm('\u786E\u8BA4\u505C\u7528\u8BE5\u673A\u6784?');\">\u505C\u7528</a>");
                html.Append("</div></td>");
                html.Append("</tr>");
            }
            return html.ToString();
        }

        private void Save()
        {
            string backUrl = "Admin_InstitutionList.aspx?MenuId=" + MenuId;
            Institution model = Action == "Edit" ? bll.SelectSingle("id=" + Id + " and status<>-1") : new Institution();
            if (model == null)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u8BF7\u6C42\u53C2\u6570\u9519\u8BEF!", backUrl, 2);
                return;
            }

            string nameCn = NormalizeInput(Function.FormRequest("name_cn"));
            string nameEn = NormalizeInput(Function.FormRequest("name_en"));
            string normalized = NormalizeKey(!string.IsNullOrWhiteSpace(nameEn) ? nameEn : nameCn);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u673A\u6784\u4E2D\u6587\u540D\u6216\u82F1\u6587\u540D\u81F3\u5C11\u586B\u5199\u4E00\u9879!", backUrl, 2);
                return;
            }

            string duplicateWhere = "status<>-1 and normalized_name=N'" + EncodeSql(normalized) + "'";
            if (Action == "Edit") duplicateWhere += " and id<>" + Id;
            if (bll.Exists(duplicateWhere))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u5DF2\u5B58\u5728\u76F8\u540C\u673A\u6784\uFF0C\u8BF7\u5148\u68C0\u67E5\u662F\u5426\u9700\u8981\u5408\u5E76!", backUrl, 2);
                return;
            }

            int parentId = Function.ConvertTo<int>(Function.FormRequest("parent_id"), 0);
            if (Action == "Edit" && parentId == Id)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u4E0A\u7EA7\u673A\u6784\u4E0D\u80FD\u9009\u62E9\u81EA\u8EAB!", backUrl, 2);
                return;
            }

            model.parent_id = parentId > 0 ? (int?)parentId : null;
            model.name_cn = Encode(nameCn);
            model.name_en = Encode(nameEn);
            model.normalized_name = normalized;
            model.alias_names = Encode(NormalizeInput(Function.FormRequest("alias_names")));
            model.country = Encode(NormalizeInput(Function.FormRequest("country")));
            model.province = Encode(NormalizeInput(Function.FormRequest("province")));
            model.city = Encode(NormalizeInput(Function.FormRequest("city")));
            model.website = Encode(NormalizeInput(Function.FormRequest("website")));
            model.status = Function.ConvertTo<int>(Function.FormRequest("status"), 1);
            model.updatetime = DateTime.Now;

            bool ok;
            if (Action == "Edit")
            {
                ok = bll.Update(new[] { "id", "addtime" }, model);
            }
            else
            {
                model.addtime = DateTime.Now;
                model.id = Function.ConvertTo<int>(bll.AddIdentity(model, "id"), 0);
                ok = model.id > 0;
            }

            if (ok)
            {
                SyncAliases(model.id, Decode(model.alias_names));
            }

            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), ok ? "\u673A\u6784\u4FDD\u5B58\u6210\u529F!" : "\u673A\u6784\u4FDD\u5B58\u5931\u8D25!", backUrl, ok ? 0 : 2);
        }

        private void Delete()
        {
            string backUrl = "Admin_InstitutionList.aspx?MenuId=" + MenuId;
            if (Id <= 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u8BF7\u6C42\u53C2\u6570\u9519\u8BEF!", backUrl, 2);
                return;
            }

            bool ok = bll.Update("status=-1,updatetime=GETDATE()", "id=" + Id);
            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), ok ? "\u673A\u6784\u5DF2\u505C\u7528!" : "\u673A\u6784\u505C\u7528\u5931\u8D25!", backUrl, ok ? 0 : 2);
        }

        private void SyncAliases(int institutionId, string aliasText)
        {
            if (institutionId <= 0)
            {
                return;
            }

            aliasBll.Delete("institution_id=" + institutionId);
            foreach (string alias in SplitAlias(aliasText))
            {
                aliasBll.Add(new InstitutionAlias
                {
                    institution_id = institutionId,
                    alias_name = Encode(alias),
                    normalized_alias = NormalizeKey(alias),
                    language = ContainsChinese(alias) ? "zh" : "en",
                    status = 1,
                    addtime = DateTime.Now
                }, "id");
            }
        }

        private IEnumerable<string> SplitAlias(string value)
        {
            HashSet<string> exists = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string part in Regex.Split(value ?? string.Empty, @"[;\uFF1B|\r\n]+"))
            {
                string clean = NormalizeInput(part);
                if (!string.IsNullOrWhiteSpace(clean) && !exists.Contains(clean))
                {
                    exists.Add(clean);
                    yield return clean;
                }
            }
        }

        private string BuildPager(int count, int pageSize, int pageIndex)
        {
            int pageCount = Math.Max(1, (int)Math.Ceiling(count / (double)pageSize));
            if (pageCount <= 1)
            {
                return string.Empty;
            }

            StringBuilder html = new StringBuilder("<div class=\"master-page\">");
            for (int i = 1; i <= pageCount; i++)
            {
                if (i == pageIndex) html.Append("<span>").Append(i).Append("</span>");
                else html.Append("<a href=\"Admin_InstitutionList.aspx?MenuId=").Append(Server.UrlEncode(MenuId)).Append("&Key=").Append(Server.UrlEncode(Key)).Append("&Page=").Append(i).Append("\">").Append(i).Append("</a>");
            }
            html.Append("</div>");
            return html.ToString();
        }

        private string BuildParentOptions(int selectedId)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<option value=\"0\">无上级机构</option>");
            DataTable dt = bll.GetDatatable("select id,coalesce(nullif(name_cn,N''),nullif(name_en,N''),normalized_name) as name from dbo.Institution where status<>-1" + (Id > 0 ? " and id<>" + Id : "") + " order by name_en asc,name_cn asc,id asc");
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    int id = ToInt(row["id"]);
                    string selected = id == selectedId ? " selected=\"selected\"" : string.Empty;
                    html.Append("<option value=\"").Append(id).Append("\"").Append(selected).Append(">")
                        .Append(H(Decode(Convert.ToString(row["name"]))))
                        .Append("</option>");
                }
                dt.Dispose();
            }
            return html.ToString();
        }

        private int Count(string sql)
        {
            DataTable dt = bll.GetDatatable(sql);
            return dt != null && dt.Rows.Count > 0 ? ToInt(dt.Rows[0][0]) : 0;
        }

        private string JoinNonEmpty(params string[] values)
        {
            List<string> result = new List<string>();
            foreach (string value in values)
            {
                if (!string.IsNullOrWhiteSpace(value)) result.Add(value);
            }
            return string.Join(" / ", result.ToArray());
        }

        private string GetStatusText(int status)
        {
            return status == 1 ? "\u542F\u7528" : "\u505C\u7528";
        }

        private bool ContainsChinese(string value)
        {
            return Regex.IsMatch(value ?? string.Empty, @"[\u3400-\u9fff\uf900-\ufaff]");
        }

        private string NormalizeInput(string value)
        {
            return Regex.Replace(Function.HtmlDiscode(value ?? string.Empty).Replace('\u00A0', ' '), @"\s+", " ").Trim();
        }

        private string NormalizeKey(string value)
        {
            return NormalizeInput(value).ToLowerInvariant();
        }

        private string Encode(string value)
        {
            return Function.HtmlEncode(value ?? string.Empty);
        }

        private string Decode(string value)
        {
            return Function.HtmlDiscode(value ?? string.Empty);
        }

        private string EncodeSql(string value)
        {
            return Function.HtmlEncode(value ?? string.Empty).Replace("'", "''");
        }

        private string H(string value)
        {
            return Server.HtmlEncode(value ?? string.Empty);
        }

        private int ToInt(object value)
        {
            return Function.ConvertTo<int>(Convert.ToString(value), 0);
        }

        private DateTime ToDate(object value)
        {
            return Function.ConvertTo<DateTime>(Convert.ToString(value), DateTime.MinValue);
        }
    }
}
