using BLL;
using LiteratureManager.Common;
using Model;
using System;
using System.Data;
using System.Text;
using System.Web;

namespace Web
{
    public partial class LiteratureVenue : System.Web.UI.Page
    {
        private readonly BLLBase<Literature> literatureBll = new BLLBase<Literature>();
        private readonly BLLBase<LiteratureVenueProfile> profileBll = new BLLBase<LiteratureVenueProfile>();
        public string Type = "all";
        public string ActiveType = "all";
        public string Venue = string.Empty;
        public string VenueListTitleHtml = string.Empty;
        public string VenueListHtml = string.Empty;
        public string SelectedVenueTitleHtml = string.Empty;
        public string SelectedVenueSummaryHtml = string.Empty;
        public string VenueInfoHtml = string.Empty;
        public string LiteratureListHtml = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            Type = NormalizeType(Function.GetRequest("type"));
            ActiveType = Type;
            Venue = Function.HtmlDiscode(Function.GetRequest("venue"));
            BindVenuePage();
        }

        public string GetTypeClass(string type)
        {
            return string.Equals(ActiveType, type, StringComparison.OrdinalIgnoreCase)
                || (type == "all" && string.IsNullOrWhiteSpace(ActiveType))
                ? "current"
                : string.Empty;
        }

        private void BindVenuePage()
        {
            DataTable venueDt = literatureBll.GetDatatable(GetVenueSql());
            if (string.IsNullOrWhiteSpace(Venue) && venueDt != null && venueDt.Rows.Count > 0)
            {
                Venue = Function.HtmlDiscode(Convert.ToString(venueDt.Rows[0]["venue_name"]));
                Type = Convert.ToString(venueDt.Rows[0]["venue_type"]);
            }

            VenueListHtml = BuildVenueListHtml(venueDt);
            VenueListTitleHtml = GetListTitle(venueDt);
            BuildSelectedLiteratureHtml();
            if (venueDt != null)
            {
                venueDt.Dispose();
            }
        }

        private string GetVenueSql()
        {
            string journalSql = "select N'journal' as venue_type, LTRIM(RTRIM(journal_name)) as venue_name, count(1) as lit_count from Literature where status=1 and canonical_literature_id is null and LTRIM(RTRIM(isnull(journal_name,N'')))<>N'' group by LTRIM(RTRIM(journal_name))";
            string conferenceSql = "select N'conference' as venue_type, LTRIM(RTRIM(conference_name)) as venue_name, count(1) as lit_count from Literature where status=1 and canonical_literature_id is null and LTRIM(RTRIM(isnull(conference_name,N'')))<>N'' group by LTRIM(RTRIM(conference_name))";
            string source;
            if (Type == "journal")
            {
                source = journalSql;
            }
            else if (Type == "conference")
            {
                source = conferenceSql;
            }
            else
            {
                source = journalSql + " union all " + conferenceSql;
            }
            return "select * from (" + source + ") v order by lit_count desc, venue_name asc";
        }

        private string BuildVenueListHtml(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return "<div class=\"venue-empty\">&#26242;&#26080;&#26399;&#21002;/&#20250;&#35758;&#20998;&#32452;</div>";
            }

            StringBuilder html = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                string venueType = Convert.ToString(row["venue_type"]);
                string venueName = Function.HtmlDiscode(Convert.ToString(row["venue_name"]));
                string currentClass = string.Equals(venueType, Type, StringComparison.OrdinalIgnoreCase) && string.Equals(venueName, Venue, StringComparison.OrdinalIgnoreCase) ? " current" : string.Empty;
                html.Append("<a class=\"venue-card");
                html.Append(currentClass);
                html.Append("\" href=\"/LiteratureVenue.aspx?type=");
                html.Append(HttpUtility.UrlEncode(venueType));
                html.Append("&venue=");
                html.Append(HttpUtility.UrlEncode(venueName));
                html.Append("\"><strong>");
                html.Append(Server.HtmlEncode(venueName));
                html.Append("</strong><span>");
                html.Append(venueType == "journal" ? "&#26399;&#21002;" : "&#20250;&#35758;");
                html.Append(" · ");
                html.Append(Function.ConvertTo<int>(Convert.ToString(row["lit_count"]), 0));
                html.Append(" &#31687;&#25991;&#29486;</span></a>");
            }
            return html.ToString();
        }

        private string GetListTitle(DataTable dt)
        {
            int count = dt != null ? dt.Rows.Count : 0;
            if (ActiveType == "journal")
            {
                return "&#26399;&#21002;&#20998;&#32452; · " + count;
            }
            if (ActiveType == "conference")
            {
                return "&#20250;&#35758;&#20998;&#32452; · " + count;
            }
            return "&#26399;&#21002;/&#20250;&#35758;&#20998;&#32452; · " + count;
        }

        private void BuildSelectedLiteratureHtml()
        {
            if (string.IsNullOrWhiteSpace(Venue) || (Type != "journal" && Type != "conference"))
            {
                SelectedVenueTitleHtml = "&#26242;&#26410;&#36873;&#25321;&#26469;&#28304;";
                SelectedVenueSummaryHtml = "&#35831;&#20174;&#24038;&#20391;&#36873;&#25321;&#19968;&#20010;&#26399;&#21002;&#25110;&#20250;&#35758;&#26597;&#30475;&#23545;&#24212;&#25991;&#29486;&#12290;";
                VenueInfoHtml = string.Empty;
                LiteratureListHtml = "<div class=\"venue-empty\">&#26242;&#26080;&#25991;&#29486;</div>";
                return;
            }

            string field = Type == "journal" ? "journal_name" : "conference_name";
            string safeVenue = SqlLiteral(Venue.Trim());
            string sql = @"
select top 100
    l.id,
    l.title,
    (select string_agg(a.name_cn,N', ') within group (order by m.author_order) from LiteratureAuthorMap m inner join Author a on a.id=m.author_id where m.literature_id=l.id) as author_names,
    l.publish_year,
    l.source_type,
    l.abstract_text
from Literature l
where l.status=1 and l.canonical_literature_id is null and LTRIM(RTRIM(isnull(l." + field + @",N'')))=N'" + safeVenue + @"'
order by l.is_top desc,l.publish_year desc,l.addtime desc,l.id desc";
            DataTable dt = literatureBll.GetDatatable(sql);
            int count = dt != null ? dt.Rows.Count : 0;
            SelectedVenueTitleHtml = Server.HtmlEncode(Venue);
            SelectedVenueSummaryHtml = (Type == "journal" ? "&#26399;&#21002;" : "&#20250;&#35758;") + " · &#24403;&#21069;&#26174;&#31034; " + count + " &#31687;&#20844;&#24320;&#25991;&#29486;";
            VenueInfoHtml = BuildVenueInfoHtml(field, safeVenue, count);
            LiteratureListHtml = BuildLiteratureListHtml(dt);
            if (dt != null)
            {
                dt.Dispose();
            }
        }

        private string BuildVenueInfoHtml(string field, string safeVenue, int visibleCount)
        {
            LiteratureVenueProfile profile = profileBll.SelectSingle("status<>-1 and venue_type=N'" + SqlLiteral(Type) + "' and venue_name=N'" + safeVenue + "'");
            string condition = "status=1 and canonical_literature_id is null and LTRIM(RTRIM(isnull(" + field + ",N'')))=N'" + safeVenue + "'";
            int totalCount = GetScalarInt("select count(1) from Literature where " + condition);
            string minYear = GetScalarText("select cast(min(publish_year) as nvarchar(20)) from Literature where " + condition + " and publish_year is not null");
            string maxYear = GetScalarText("select cast(max(publish_year) as nvarchar(20)) from Literature where " + condition + " and publish_year is not null");
            string publisher = ProfileValue(profile, "publisher", GetTopValue("publisher", condition));
            string sourceDb = GetTopValue("source_db", condition);
            string sourceTypes = GetTopGroupedValues("source_type", condition, 4);
            string tags = GetTopTags(field, safeVenue, 6);
            string yearRange = !string.IsNullOrWhiteSpace(minYear) && !string.IsNullOrWhiteSpace(maxYear) ? (minYear == maxYear ? maxYear : minYear + " - " + maxYear) : "&#26242;&#26080;";
            string introduction = ProfileValue(profile, "introduction", string.Empty);

            StringBuilder html = new StringBuilder();
            html.Append("<section class=\"venue-info-card\"><div class=\"venue-info-intro\"><h3>&#26469;&#28304;&#31616;&#20171;</h3><p>");
            html.Append(!string.IsNullOrWhiteSpace(introduction) ? Server.HtmlEncode(introduction) : (Type == "journal"
                ? "&#35813;&#26399;&#21002;&#20998;&#32452;&#30001;&#24179;&#21488;&#26681;&#25454;&#24050;&#20844;&#24320;&#25991;&#29486;&#30340;&#26399;&#21002;&#23383;&#27573;&#33258;&#21160;&#27719;&#24635;&#29983;&#25104;&#65292;&#20415;&#20110;&#36861;&#36394;&#21516;&#26399;&#21002;&#30340;&#30740;&#31350;&#20027;&#39064;&#21644;&#25991;&#29486;&#20998;&#24067;&#12290;"
                : "&#35813;&#20250;&#35758;&#20998;&#32452;&#30001;&#24179;&#21488;&#26681;&#25454;&#24050;&#20844;&#24320;&#25991;&#29486;&#30340;&#20250;&#35758;&#23383;&#27573;&#33258;&#21160;&#27719;&#24635;&#29983;&#25104;&#65292;&#20415;&#20110;&#26597;&#30475;&#21516;&#20250;&#35758;&#30340;&#30456;&#20851;&#35770;&#25991;&#21644;&#30740;&#31350;&#36235;&#21183;&#12290;"));
            html.Append("</p></div><div class=\"venue-info-grid\">");
            AppendInfoItem(html, "&#24179;&#21488;&#25991;&#29486;&#25968;", totalCount + " &#31687;");
            AppendInfoItem(html, "&#24180;&#20221;&#33539;&#22260;", yearRange);
            AppendInfoItem(html, Type == "journal" ? "&#20986;&#29256;&#26041;" : "&#20027;&#21150;/&#20986;&#29256;&#26041;", SafeHtml(publisher));
            AppendInfoItem(html, "&#26469;&#28304;&#24211;", SafeHtml(sourceDb));
            AppendInfoItem(html, "&#25991;&#29486;&#31867;&#22411;", SafeHtml(sourceTypes));
            AppendInfoItem(html, "&#30456;&#20851;&#26631;&#31614;", SafeHtml(tags));
            AppendInfoItem(html, Type == "journal" ? "&#24433;&#21709;/&#24341;&#29992;&#22240;&#23376;" : "&#20250;&#35758;&#31561;&#32423;/&#24433;&#21709;&#21147;", SafeHtml(Type == "journal" ? ProfileValue(profile, "impact_factor", string.Empty) : ProfileValue(profile, "conference_level", string.Empty)));
            AppendInfoItem(html, Type == "journal" ? "ISSN / &#20998;&#21306;" : "&#20250;&#35758;&#21608;&#26399;/&#22320;&#28857;", SafeHtml(Type == "journal" ? JoinMeta(ProfileValue(profile, "issn", string.Empty), ProfileValue(profile, "jcr_quartile", string.Empty)) : JoinMeta(ProfileValue(profile, "conference_cycle", string.Empty), ProfileValue(profile, "location", string.Empty))));
            AppendInfoItem(html, "&#23448;&#32593;", BuildWebsiteHtml(ProfileValue(profile, "website_url", string.Empty)));
            html.Append("</div></section>");
            return html.ToString();
        }

        private void AppendInfoItem(StringBuilder html, string label, string value)
        {
            html.Append("<div><span>");
            html.Append(label);
            html.Append("</span><strong>");
            html.Append(string.IsNullOrWhiteSpace(value) ? "&#26242;&#26080;" : value);
            html.Append("</strong></div>");
        }

        private string BuildLiteratureListHtml(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return "<div class=\"venue-empty\">&#35813;&#26399;&#21002;/&#20250;&#35758;&#19979;&#26242;&#26080;&#20844;&#24320;&#25991;&#29486;</div>";
            }

            StringBuilder html = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                string title = Function.HtmlDiscode(Convert.ToString(row["title"]));
                string authors = Function.HtmlDiscode(Convert.ToString(row["author_names"]));
                string year = Convert.ToString(row["publish_year"]);
                string sourceType = Function.HtmlDiscode(Convert.ToString(row["source_type"]));
                string abstractText = Function.HtmlDiscode(Convert.ToString(row["abstract_text"]));
                int id = Function.ConvertTo<int>(Convert.ToString(row["id"]), 0);

                html.Append("<article class=\"venue-item\"><h3><a href=\"/LiteratureInfo.aspx?id=");
                html.Append(id);
                html.Append("\">");
                html.Append(Server.HtmlEncode(title));
                html.Append("</a></h3><div class=\"venue-meta\">");
                html.Append(Server.HtmlEncode(JoinMeta(authors, year, sourceType)));
                html.Append("</div><div class=\"venue-abstract\">");
                html.Append(Server.HtmlEncode(TrimText(abstractText, 220)));
                html.Append("</div></article>");
            }
            return html.ToString();
        }

        private int GetScalarInt(string sql)
        {
            string value = GetScalarText(sql);
            return Function.ConvertTo<int>(value, 0);
        }

        private string GetScalarText(string sql)
        {
            DataTable dt = literatureBll.GetDatatable(sql);
            string value = string.Empty;
            if (dt != null && dt.Rows.Count > 0)
            {
                value = Function.HtmlDiscode(Convert.ToString(dt.Rows[0][0]));
                dt.Dispose();
            }
            return value;
        }

        private string GetTopValue(string field, string condition)
        {
            return GetScalarText("select top 1 " + field + " from Literature where " + condition + " and LTRIM(RTRIM(isnull(" + field + ",N'')))<>N'' group by " + field + " order by count(1) desc," + field + " asc");
        }

        private string GetTopGroupedValues(string field, string condition, int top)
        {
            DataTable dt = literatureBll.GetDatatable("select top " + top + " " + field + ",count(1) as num from Literature where " + condition + " and LTRIM(RTRIM(isnull(" + field + ",N'')))<>N'' group by " + field + " order by num desc," + field + " asc");
            string result = BuildNameCountList(dt, field);
            if (dt != null)
            {
                dt.Dispose();
            }
            return result;
        }

        private string GetTopTags(string field, string safeVenue, int top)
        {
            string sql = "select top " + top + " t.name,count(1) as num from Literature l inner join LiteratureTagMap m on m.literature_id=l.id inner join LiteratureTag t on t.id=m.tag_id where l.status=1 and l.canonical_literature_id is null and t.status<>-1 and LTRIM(RTRIM(isnull(l." + field + ",N'')))=N'" + safeVenue + "' group by t.name order by num desc,t.name asc";
            DataTable dt = literatureBll.GetDatatable(sql);
            string result = BuildNameCountList(dt, "name");
            if (dt != null)
            {
                dt.Dispose();
            }
            return result;
        }

        private string BuildNameCountList(DataTable dt, string nameField)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                string name = Function.HtmlDiscode(Convert.ToString(row[nameField]));
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }
                if (sb.Length > 0)
                {
                    sb.Append(" / ");
                }
                sb.Append(name);
                sb.Append(" ");
                sb.Append(Function.ConvertTo<int>(Convert.ToString(row["num"]), 0));
            }
            return sb.ToString();
        }

        private string SafeHtml(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "&#26242;&#26080;" : Server.HtmlEncode(value);
        }

        private string BuildWebsiteHtml(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return "&#26242;&#26080;";
            }
            string safeUrl = Server.HtmlEncode(url);
            return "<a href=\"" + safeUrl + "\" target=\"_blank\" rel=\"noopener\">" + safeUrl + "</a>";
        }

        private string ProfileValue(LiteratureVenueProfile profile, string field, string fallback)
        {
            if (profile == null || profile.id <= 0)
            {
                return fallback;
            }
            string value = string.Empty;
            switch (field)
            {
                case "introduction": value = profile.introduction; break;
                case "impact_factor": value = profile.impact_factor; break;
                case "jcr_quartile": value = profile.jcr_quartile; break;
                case "issn": value = profile.issn; break;
                case "conference_level": value = profile.conference_level; break;
                case "conference_cycle": value = profile.conference_cycle; break;
                case "location": value = profile.location; break;
                case "website_url": value = profile.website_url; break;
                case "publisher": value = profile.publisher; break;
            }
            value = Function.HtmlDiscode(value);
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }

        private string NormalizeType(string rawType)
        {
            string value = (rawType ?? string.Empty).Trim().ToLowerInvariant();
            return value == "journal" || value == "conference" ? value : "all";
        }

        private string JoinMeta(params string[] values)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string value in values)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }
                if (sb.Length > 0)
                {
                    sb.Append(" | ");
                }
                sb.Append(value.Trim());
            }
            return sb.Length == 0 ? "\u6682\u65E0\u5143\u6570\u636E" : sb.ToString();
        }

        private string TrimText(string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "\u6682\u65E0\u6458\u8981";
            }
            value = value.Trim();
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }

        private string SqlLiteral(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }
    }
}
