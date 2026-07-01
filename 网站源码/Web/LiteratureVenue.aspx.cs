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
            string journalNameSql = VenueFieldSql("l.journal_name");
            string conferenceNameSql = VenueFieldSql("l.conference_name");
            string journalSql = "select N'journal' as venue_type, " + journalNameSql + " as venue_name, count(1) as lit_count from Literature l where l.status=1 and l.canonical_literature_id is null and " + journalNameSql + "<>N'' and " + ActiveVenueCondition("journal", "l", journalNameSql) + " group by " + journalNameSql;
            string conferenceSql = "select N'conference' as venue_type, " + conferenceNameSql + " as venue_name, count(1) as lit_count from Literature l where l.status=1 and l.canonical_literature_id is null and " + conferenceNameSql + "<>N'' and " + ActiveVenueCondition("conference", "l", conferenceNameSql) + " group by " + conferenceNameSql;
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
            string safeVenue = SqlLiteral(NormalizeVenueName(Venue));
            string fieldSql = VenueFieldSql("l." + field);
            string sql = @"
select top 100
    l.id,
    l.title,
    (select string_agg(coalesce(nullif(a.name_cn,N''),nullif(a.name_en,N''),N'未命名作者'),N', ') within group (order by m.author_order) from LiteratureAuthorMap m inner join Author a on a.id=m.author_id where m.literature_id=l.id) as author_names,
    l.publish_year,
    l.source_type,
    l.abstract_text
from Literature l
where l.status=1 and l.canonical_literature_id is null and " + fieldSql + @"=N'" + safeVenue + @"'
  and " + ActiveVenueCondition(Type, "l", fieldSql) + @"
order by l.is_top desc,l.publish_year desc,l.addtime desc,l.id desc";
            DataTable dt = literatureBll.GetDatatable(sql);
            int count = dt != null ? dt.Rows.Count : 0;
            SelectedVenueTitleHtml = Server.HtmlEncode(NormalizeVenueName(Venue));
            SelectedVenueSummaryHtml = (Type == "journal" ? "&#26399;&#21002;" : "&#20250;&#35758;") + " · &#24403;&#21069;&#26174;&#31034; " + count + " &#31687;&#20844;&#24320;&#25991;&#29486;";
            SelectedVenueSummaryHtml = SelectedVenueSummaryHtml.Replace(" \u8DEF ", " &middot; ");
            VenueInfoHtml = BuildVenueInfoHtml(field, safeVenue, count);
            LiteratureListHtml = BuildLiteratureListHtml(dt);
            if (dt != null)
            {
                dt.Dispose();
            }
        }

        private string BuildVenueInfoHtml(string field, string safeVenue, int visibleCount)
        {
            VenueMasterInfo masterInfo = GetVenueMasterInfo(field, safeVenue);
            string condition = "status=1 and canonical_literature_id is null and " + VenueFieldSql(field) + "=N'" + safeVenue + "' and " + ActiveVenueCondition(Type, string.Empty, VenueFieldSql(field));
            int totalCount = GetScalarInt("select count(1) from Literature where " + condition);
            string minYear = GetScalarText("select cast(min(publish_year) as nvarchar(20)) from Literature where " + condition + " and publish_year is not null");
            string maxYear = GetScalarText("select cast(max(publish_year) as nvarchar(20)) from Literature where " + condition + " and publish_year is not null");
            string publisher = FirstNonEmpty(masterInfo.Publisher, GetTopValue("publisher", condition));
            string sourceDb = GetTopValue("source_db", condition);
            string sourceTypes = GetTopGroupedValues("source_type", condition, 4);
            string tags = GetTopTags(field, safeVenue, 6);
            string yearRange = !string.IsNullOrWhiteSpace(minYear) && !string.IsNullOrWhiteSpace(maxYear) ? (minYear == maxYear ? maxYear : minYear + " - " + maxYear) : "&#26242;&#26080;";
            string introduction = string.Empty;
            string issnOrLocation = Type == "journal"
                ? masterInfo.Issn
                : JoinMeta(masterInfo.Cycle, masterInfo.Location);
            string websiteUrl = masterInfo.Website;

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
            AppendInfoItem(html, Type == "journal" ? "ISSN / EISSN" : "&#20250;&#35758;&#26085;&#26399;/&#22320;&#28857;", SafeHtml(issnOrLocation));
            AppendInfoItem(html, "&#23448;&#32593;", BuildWebsiteHtml(websiteUrl));
            html.Append("</div></section>");
            return html.ToString();
        }

        private VenueMasterInfo GetVenueMasterInfo(string field, string safeVenue)
        {
            return Type == "journal" ? GetJournalMasterInfo(field, safeVenue) : GetConferenceMasterInfo(field, safeVenue);
        }

        private VenueMasterInfo GetConferenceMasterInfo(string field, string safeVenue)
        {
            string sql = @"
select top 1 c.organizer,c.country,c.city,c.start_date,c.end_date,c.website
from Literature l
inner join Conference c on c.id=l.conference_id and c.status<>-1
where l.status=1 and l.canonical_literature_id is null
  and " + VenueFieldSql("l." + field) + @"=N'" + safeVenue + @"'
order by c.status desc,c.updatetime desc,c.id desc";
            DataTable dt = literatureBll.GetDatatable(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                if (dt != null)
                {
                    dt.Dispose();
                }
                dt = literatureBll.GetDatatable(@"
select top 1 c.organizer,c.country,c.city,c.start_date,c.end_date,c.website
from Conference c
where c.status<>-1 and (c.acronym=N'" + safeVenue + @"' or c.name_cn=N'" + safeVenue + @"' or c.name_en=N'" + safeVenue + @"')
order by c.status desc,c.updatetime desc,c.id desc");
            }

            VenueMasterInfo info = new VenueMasterInfo();
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                info.Publisher = Function.HtmlDiscode(Convert.ToString(row["organizer"]));
                info.Location = JoinMeta(Function.HtmlDiscode(Convert.ToString(row["country"])), Function.HtmlDiscode(Convert.ToString(row["city"])));
                info.Cycle = BuildDateRange(row["start_date"], row["end_date"]);
                info.Website = Function.HtmlDiscode(Convert.ToString(row["website"]));
                dt.Dispose();
            }
            return info;
        }

        private VenueMasterInfo GetJournalMasterInfo(string field, string safeVenue)
        {
            string sql = @"
select top 1 j.publisher,j.country,j.issn,j.eissn,j.website
from Literature l
inner join Journal j on j.id=l.journal_id and j.status<>-1
where l.status=1 and l.canonical_literature_id is null
  and " + VenueFieldSql("l." + field) + @"=N'" + safeVenue + @"'
order by j.status desc,j.updatetime desc,j.id desc";
            DataTable dt = literatureBll.GetDatatable(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                if (dt != null)
                {
                    dt.Dispose();
                }
                dt = literatureBll.GetDatatable(@"
select top 1 j.publisher,j.country,j.issn,j.eissn,j.website
from Journal j
where j.status<>-1 and (j.name_cn=N'" + safeVenue + @"' or j.name_en=N'" + safeVenue + @"')
order by j.status desc,j.updatetime desc,j.id desc");
            }

            VenueMasterInfo info = new VenueMasterInfo();
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                info.Publisher = Function.HtmlDiscode(Convert.ToString(row["publisher"]));
                info.Location = Function.HtmlDiscode(Convert.ToString(row["country"]));
                info.Issn = JoinMeta(Function.HtmlDiscode(Convert.ToString(row["issn"])), Function.HtmlDiscode(Convert.ToString(row["eissn"])));
                info.Website = Function.HtmlDiscode(Convert.ToString(row["website"]));
                dt.Dispose();
            }
            return info;
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
            string fieldSql = VenueFieldSql("l." + field);
            string sql = "select top " + top + " t.name,count(1) as num from Literature l inner join LiteratureTagMap m on m.literature_id=l.id inner join LiteratureTag t on t.id=m.tag_id where l.status=1 and l.canonical_literature_id is null and t.status<>-1 and " + fieldSql + "=N'" + safeVenue + "' and " + ActiveVenueCondition(Type, "l", fieldSql) + " group by t.name order by num desc,t.name asc";
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
            string cleanUrl = url.Trim();
            string href = cleanUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || cleanUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                ? cleanUrl
                : "https://" + cleanUrl;
            return "<a href=\"" + Server.HtmlEncode(href) + "\" target=\"_blank\" rel=\"noopener\">" + Server.HtmlEncode(cleanUrl) + "</a>";
        }

        private string NormalizeType(string rawType)
        {
            string value = (rawType ?? string.Empty).Trim().ToLowerInvariant();
            return value == "journal" || value == "conference" ? value : "all";
        }

        private string ActiveVenueCondition(string venueType, string literatureAlias, string venueNameSql)
        {
            if (venueType == "journal")
            {
                string journalId = LiteratureColumn(literatureAlias, "journal_id");
                return @"exists(
                        select 1 from Journal j
                        where j.status<>-1
                          and (
                              j.id=" + journalId + @"
                              or " + VenueFieldSql("j.name_cn") + @"=" + venueNameSql + @"
                              or " + VenueFieldSql("j.name_en") + @"=" + venueNameSql + @"
                          )
                    )";
            }

            if (venueType == "conference")
            {
                string conferenceId = LiteratureColumn(literatureAlias, "conference_id");
                return @"exists(
                        select 1 from Conference c
                        where c.status<>-1
                          and (
                              c.id=" + conferenceId + @"
                              or " + VenueFieldSql("c.acronym") + @"=" + venueNameSql + @"
                              or " + VenueFieldSql("c.name_cn") + @"=" + venueNameSql + @"
                              or " + VenueFieldSql("c.name_en") + @"=" + venueNameSql + @"
                          )
                    )";
            }

            return "1=1";
        }

        private string LiteratureColumn(string literatureAlias, string column)
        {
            return string.IsNullOrWhiteSpace(literatureAlias) ? column : literatureAlias + "." + column;
        }

        private string NormalizeVenueName(string value)
        {
            string text = Function.HtmlDiscode(value ?? string.Empty)
                .Replace("&nbsp;", " ")
                .Replace('\u00A0', ' ')
                .Replace('\u2002', ' ')
                .Replace('\u2003', ' ');
            return System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
        }

        private string VenueFieldSql(string field)
        {
            return "LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(isnull(" + field + ",N''),N'&nbsp;',N' '),NCHAR(160),N' '),NCHAR(12288),N' ')))";
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

        private string FirstNonEmpty(params string[] values)
        {
            foreach (string value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
            return string.Empty;
        }

        private string BuildDateRange(object start, object end)
        {
            DateTime startDate = Function.ConvertTo<DateTime>(Convert.ToString(start), DateTime.MinValue);
            DateTime endDate = Function.ConvertTo<DateTime>(Convert.ToString(end), DateTime.MinValue);
            if (startDate == DateTime.MinValue && endDate == DateTime.MinValue)
            {
                return string.Empty;
            }
            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
            {
                return startDate.ToString("yyyy-MM-dd") + " - " + endDate.ToString("yyyy-MM-dd");
            }
            return (startDate != DateTime.MinValue ? startDate : endDate).ToString("yyyy-MM-dd");
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

        private class VenueMasterInfo
        {
            public string Publisher { get; set; }
            public string Cycle { get; set; }
            public string Location { get; set; }
            public string Website { get; set; }
            public string Issn { get; set; }
        }
    }
}
