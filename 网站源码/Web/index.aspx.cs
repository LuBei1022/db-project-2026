using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Web
{
    public partial class index : System.Web.UI.Page
    {
        private readonly BLLBase<Literature> literatureBll = new BLLBase<Literature>();
        private readonly BLLBase<LiteratureCategory> categoryBll = new BLLBase<LiteratureCategory>();
        private readonly BLLBase<Author> authorBll = new BLLBase<Author>();

        public int literatureCount = 0;
        public int categoryCount = 0;
        public int yearCount = 0;
        public int authorCount = 0;
        public string LiteratureSparklinePath = "M0 40 Q20 10 40 30 T80 10 L100 20";
        public bool IsLogin = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user_list user = CommonUserFunc.GetUserLoginStatus();
                IsLogin = user != null && user.id > 0;
                BindStatistics();
                BindFeaturedLiterature();
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "index.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
            }
        }

        private void BindStatistics()
        {
            literatureCount = literatureBll.GetCount("Literature", "status=1 and canonical_literature_id is null");
            categoryCount = categoryBll.GetCount("LiteratureCategory", "status=1");
            DataTable yearDt = literatureBll.GetDatatable("select distinct publish_year from Literature where status=1 and canonical_literature_id is null and publish_year is not null");
            yearCount = yearDt == null ? 0 : yearDt.Rows.Count;
            authorCount = authorBll.GetCount("Author", "status=1");
            BindSparkline();
            if (yearDt != null)
            {
                yearDt.Dispose();
            }
        }

        private void BindSparkline()
        {
            DataTable dt = literatureBll.GetDatatable(@"
select convert(char(7), addtime, 120) as month_key, count(1) as total_count
from Literature
where status=1 and canonical_literature_id is null and addtime >= dateadd(month,-11,dateadd(day,1-day(getdate()),convert(date,getdate())))
group by convert(char(7), addtime, 120)
order by month_key asc");

            int[] values = new int[12];
            DateTime startMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-11);
            Dictionary<string, int> monthMap = new Dictionary<string, int>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string key = Convert.ToString(row["month_key"]);
                    int total = Function.ConvertTo<int>(Convert.ToString(row["total_count"]), 0);
                    if (!monthMap.ContainsKey(key))
                    {
                        monthMap.Add(key, total);
                    }
                }
                dt.Dispose();
            }

            for (int i = 0; i < values.Length; i++)
            {
                string key = startMonth.AddMonths(i).ToString("yyyy-MM");
                values[i] = monthMap.ContainsKey(key) ? monthMap[key] : 0;
            }

            LiteratureSparklinePath = BuildSparklinePath(values, 100, 40);
        }

        private string BuildSparklinePath(int[] values, int width, int height)
        {
            if (values == null || values.Length == 0)
            {
                return "M0 40 Q20 10 40 30 T80 10 L100 20";
            }

            int max = 0;
            int min = int.MaxValue;
            foreach (int value in values)
            {
                if (value > max) max = value;
                if (value < min) min = value;
            }

            if (max == min)
            {
                int mid = height / 2;
                return "M0 " + mid + " L" + width + " " + mid;
            }

            StringBuilder path = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                double x = values.Length == 1 ? 0 : (double)i * width / (values.Length - 1);
                double ratio = (double)(values[i] - min) / (max - min);
                double y = height - (ratio * (height - 6)) - 3;
                path.Append(i == 0 ? "M" : " L");
                path.Append(Math.Round(x, 1).ToString("0.#"));
                path.Append(" ");
                path.Append(Math.Round(y, 1).ToString("0.#"));
            }

            return path.ToString();
        }

        private void BindFeaturedLiterature()
        {
            DataTable dt = literatureBll.GetDatatable(@"
select top 3
    l.id,
    l.title,
    (select string_agg(coalesce(nullif(a.name_cn,N''),nullif(a.name_en,N''),N'未命名作者'),N'，') within group (order by m.author_order) from LiteratureAuthorMap m inner join Author a on a.id=m.author_id where m.literature_id=l.id) as author_names,
    l.publish_year,
    l.source_type
from Literature l
where l.status=1 and l.canonical_literature_id is null
order by l.is_top desc,l.updatetime desc,l.id desc");
            if (dt != null && dt.Rows.Count > 0)
            {
                FeaturedLiteratureList.DataSource = dt.DefaultView;
                FeaturedLiteratureList.DataBind();
            }
        }

        public string GetLiteratureCardMeta(object authorObj, object yearObj, object sourceTypeObj)
        {
            StringBuilder sb = new StringBuilder();
            string author = Function.HtmlDiscode(authorObj == null ? string.Empty : authorObj.ToString());
            string year = yearObj == null ? string.Empty : yearObj.ToString();
            string sourceType = Function.HtmlDiscode(sourceTypeObj == null ? string.Empty : sourceTypeObj.ToString());
            if (!string.IsNullOrWhiteSpace(author))
            {
                sb.Append(author);
            }
            if (!string.IsNullOrWhiteSpace(year))
            {
                if (sb.Length > 0) sb.Append(" | ");
                sb.Append(year);
            }
            if (!string.IsNullOrWhiteSpace(sourceType))
            {
                if (sb.Length > 0) sb.Append(" | ");
                sb.Append(sourceType);
            }
            return sb.ToString();
        }

        public string GetLiteratureSummary(object abstractObj)
        {
            string text = Function.HtmlDiscode(abstractObj == null ? string.Empty : abstractObj.ToString());
            if (string.IsNullOrWhiteSpace(text))
            {
                return "\u6682\u65E0\u6458\u8981";
            }
            return text.Length > 90 ? text.Substring(0, 90) + "..." : text;
        }

        public string GetShortAuthor(object authorObj)
        {
            string author = Function.HtmlDiscode(authorObj == null ? string.Empty : authorObj.ToString());
            if (string.IsNullOrWhiteSpace(author))
            {
                return "Academic Team";
            }

            string[] parts = author.Split(new[] { '，', ',', ';', '；' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[0].Trim() : author.Trim();
        }

    }
}
