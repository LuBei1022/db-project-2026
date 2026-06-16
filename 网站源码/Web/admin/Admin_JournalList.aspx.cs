using BLL;
using LiteratureManager.Common;
using Model;
using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace Web.admin
{
    public partial class Admin_JournalList : System.Web.UI.Page
    {
        private readonly BLLBase<Journal> bll = new BLLBase<Journal>();

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
        public string Issn = string.Empty;
        public string Eissn = string.Empty;
        public string Publisher = string.Empty;
        public string Country = string.Empty;
        public string Subject = string.Empty;
        public string Website = string.Empty;
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
            PageTitle = Action == "Edit" ? "\u7F16\u8F91\u671F\u520A" : "\u65B0\u589E\u671F\u520A";
            if (Action != "Edit")
            {
                return;
            }
            Journal model = bll.SelectSingle("id=" + Id + " and status<>-1");
            if (model == null || model.id <= 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u672A\u627E\u5230\u5BF9\u5E94\u671F\u520A\u8BB0\u5F55!", "Admin_JournalList.aspx?MenuId=" + MenuId, 1);
                return;
            }
            NameCn = Decode(model.name_cn);
            NameEn = Decode(model.name_en);
            Issn = Decode(model.issn);
            Eissn = Decode(model.eissn);
            Publisher = Decode(model.publisher);
            Country = Decode(model.country);
            Subject = Decode(model.subject);
            Website = Decode(model.website);
            Status = model.status;
        }

        private void BindList()
        {
            int pageSize = 15;
            int pageIndex = Math.Max(1, Function.ConvertTo<int>(Function.GetRequest("Page"), 1));
            string where = "j.status<>-1";
            string safeKey = EncodeSql(Key);
            if (!string.IsNullOrWhiteSpace(safeKey))
            {
                where += " and (j.name_cn like N'%" + safeKey + "%' or j.name_en like N'%" + safeKey + "%' or j.issn like N'%" + safeKey + "%' or j.eissn like N'%" + safeKey + "%' or j.publisher like N'%" + safeKey + "%')";
            }

            int count = Count("select count(1) from dbo.Journal j where " + where);
            int start = (pageIndex - 1) * pageSize + 1;
            int end = pageIndex * pageSize;
            string sql = @"
select *
from
(
    select row_number() over(order by j.updatetime desc,j.id desc) as rn,
           j.id,j.name_cn,j.name_en,j.issn,j.eissn,j.publisher,j.country,j.subject,j.status,j.updatetime,
           (select count(1) from dbo.Literature l where l.journal_id=j.id) as paper_count
    from dbo.Journal j
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
                return "<tr><td colspan=\"8\" class=\"master-empty\">\u6682\u65E0\u671F\u520A\u6570\u636E</td></tr>";
            }

            StringBuilder html = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                int id = ToInt(row["id"]);
                string nameCn = Decode(Convert.ToString(row["name_cn"]));
                string nameEn = Decode(Convert.ToString(row["name_en"]));
                string displayName = !string.IsNullOrWhiteSpace(nameCn) ? nameCn : nameEn;
                string issnText = JoinNonEmpty(Decode(Convert.ToString(row["issn"])), Decode(Convert.ToString(row["eissn"])));
                string meta = JoinNonEmpty(Decode(Convert.ToString(row["publisher"])), Decode(Convert.ToString(row["subject"])), Decode(Convert.ToString(row["country"])));

                html.Append("<tr>");
                html.Append("<td>").Append(id).Append("</td>");
                html.Append("<td><div class=\"master-main\">").Append(H(displayName)).Append("</div><div class=\"master-sub\">");
                if (!string.IsNullOrWhiteSpace(nameCn)) html.Append("\u4E2D\u6587\u540D: ").Append(H(nameCn)).Append(" &nbsp; ");
                if (!string.IsNullOrWhiteSpace(nameEn)) html.Append("\u82F1\u6587\u540D: ").Append(H(nameEn));
                html.Append("</div></td>");
                html.Append("<td>").Append(string.IsNullOrWhiteSpace(issnText) ? "\u6682\u65E0" : H(issnText)).Append("</td>");
                html.Append("<td>").Append(string.IsNullOrWhiteSpace(meta) ? "\u6682\u65E0" : H(meta)).Append("</td>");
                html.Append("<td>").Append(ToInt(row["paper_count"])).Append("</td>");
                html.Append("<td>").Append(GetStatusText(ToInt(row["status"]))).Append("</td>");
                html.Append("<td>").Append(ToDate(row["updatetime"]).ToString("yyyy-MM-dd HH:mm")).Append("</td>");
                html.Append("<td><div class=\"master-actions\">");
                html.Append("<a href=\"Admin_JournalList.aspx?Action=Edit&ID=").Append(id).Append("&MenuId=").Append(Server.UrlEncode(MenuId)).Append("\">\u7F16\u8F91</a>");
                html.Append("<a href=\"Admin_JournalList.aspx?Action=Del&ID=").Append(id).Append("&MenuId=").Append(Server.UrlEncode(MenuId)).Append("\" onclick=\"return confirm('\u786E\u8BA4\u505C\u7528\u8BE5\u671F\u520A?');\">\u505C\u7528</a>");
                html.Append("</div></td></tr>");
            }
            return html.ToString();
        }

        private void Save()
        {
            string backUrl = "Admin_JournalList.aspx?MenuId=" + MenuId;
            Journal model = Action == "Edit" ? bll.SelectSingle("id=" + Id + " and status<>-1") : new Journal();
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
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u671F\u520A\u4E2D\u6587\u540D\u6216\u82F1\u6587\u540D\u81F3\u5C11\u586B\u5199\u4E00\u9879!", backUrl, 2);
                return;
            }

            string duplicateWhere = "status<>-1 and normalized_name=N'" + EncodeSql(normalized) + "'";
            if (Action == "Edit") duplicateWhere += " and id<>" + Id;
            if (bll.Exists(duplicateWhere))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u5DF2\u5B58\u5728\u76F8\u540C\u671F\u520A!", backUrl, 2);
                return;
            }

            model.name_cn = Encode(nameCn);
            model.name_en = Encode(nameEn);
            model.normalized_name = normalized;
            model.issn = Encode(NormalizeInput(Function.FormRequest("issn")));
            model.eissn = Encode(NormalizeInput(Function.FormRequest("eissn")));
            model.publisher = Encode(NormalizeInput(Function.FormRequest("publisher")));
            model.country = Encode(NormalizeInput(Function.FormRequest("country")));
            model.subject = Encode(NormalizeInput(Function.FormRequest("subject")));
            model.website = Encode(NormalizeInput(Function.FormRequest("website")));
            model.status = Function.ConvertTo<int>(Function.FormRequest("status"), 1);
            model.updatetime = DateTime.Now;

            bool ok;
            if (Action == "Edit") ok = bll.Update(new[] { "id", "addtime" }, model);
            else
            {
                model.addtime = DateTime.Now;
                model.id = Function.ConvertTo<int>(bll.AddIdentity(model, "id"), 0);
                ok = model.id > 0;
            }
            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), ok ? "\u671F\u520A\u4FDD\u5B58\u6210\u529F!" : "\u671F\u520A\u4FDD\u5B58\u5931\u8D25!", backUrl, ok ? 0 : 2);
        }

        private void Delete()
        {
            string backUrl = "Admin_JournalList.aspx?MenuId=" + MenuId;
            bool ok = Id > 0 && bll.Update("status=-1,updatetime=GETDATE()", "id=" + Id);
            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), ok ? "\u671F\u520A\u5DF2\u505C\u7528!" : "\u671F\u520A\u505C\u7528\u5931\u8D25!", backUrl, ok ? 0 : 2);
        }

        private string BuildPager(int count, int pageSize, int pageIndex)
        {
            int pageCount = Math.Max(1, (int)Math.Ceiling(count / (double)pageSize));
            if (pageCount <= 1) return string.Empty;
            StringBuilder html = new StringBuilder("<div class=\"master-page\">");
            for (int i = 1; i <= pageCount; i++)
            {
                if (i == pageIndex) html.Append("<span>").Append(i).Append("</span>");
                else html.Append("<a href=\"Admin_JournalList.aspx?MenuId=").Append(Server.UrlEncode(MenuId)).Append("&Key=").Append(Server.UrlEncode(Key)).Append("&Page=").Append(i).Append("\">").Append(i).Append("</a>");
            }
            html.Append("</div>");
            return html.ToString();
        }

        private int Count(string sql)
        {
            DataTable dt = bll.GetDatatable(sql);
            return dt != null && dt.Rows.Count > 0 ? ToInt(dt.Rows[0][0]) : 0;
        }

        private string JoinNonEmpty(params string[] values)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string value in values)
            {
                if (string.IsNullOrWhiteSpace(value)) continue;
                if (sb.Length > 0) sb.Append(" / ");
                sb.Append(value);
            }
            return sb.ToString();
        }

        private string GetStatusText(int status) { return status == 1 ? "\u542F\u7528" : "\u505C\u7528"; }
        private string NormalizeInput(string value) { return Regex.Replace(Function.HtmlDiscode(value ?? string.Empty).Replace('\u00A0', ' '), @"\s+", " ").Trim(); }
        private string NormalizeKey(string value) { return NormalizeInput(value).ToLowerInvariant(); }
        private string Encode(string value) { return Function.HtmlEncode(value ?? string.Empty); }
        private string Decode(string value) { return Function.HtmlDiscode(value ?? string.Empty); }
        private string EncodeSql(string value) { return Function.HtmlEncode(value ?? string.Empty).Replace("'", "''"); }
        private string H(string value) { return Server.HtmlEncode(value ?? string.Empty); }
        private int ToInt(object value) { return Function.ConvertTo<int>(Convert.ToString(value), 0); }
        private DateTime ToDate(object value) { return Function.ConvertTo<DateTime>(Convert.ToString(value), DateTime.MinValue); }
    }
}
