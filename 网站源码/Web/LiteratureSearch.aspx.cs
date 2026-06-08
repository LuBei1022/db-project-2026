using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Web
{
    public partial class LiteratureSearch : System.Web.UI.Page
    {
        private readonly BLLBase<Literature> literatureBll = new BLLBase<Literature>();
        private readonly BLLBase<LiteratureCategory> categoryBll = new BLLBase<LiteratureCategory>();
        private readonly BLLBase<integrateExchangeLog_list> exchangeLogBll = new BLLBase<integrateExchangeLog_list>();
        public string keyword = string.Empty;
        public int selectedCategoryId = 0;
        public bool HasCategoryFilter = false;
        public int selectedYear = 0;
        public int totalCount = 0;
        public int categoryCount = 0;
        public string pagerHtml = string.Empty;
        public bool IsBrowseView = false;
        public int availableCouponCount = 0;
        public string BrowseCategoryTitleHtml = string.Empty;
        public string BrowseCategorySummaryHtml = string.Empty;
        public string BrowseCategoryInfoHtml = string.Empty;
        public string BrowseLiteratureListHtml = string.Empty;
        public string BrowseCategoryListTitleHtml = string.Empty;
        public string BrowseCategoryListHtml = string.Empty;
        private int currentUserId = 0;
        private readonly Dictionary<int, string> browsePreviewMap = new Dictionary<int, string>();
        private readonly Dictionary<int, int> categoryCountMap = new Dictionary<int, int>();

        protected void Page_Load(object sender, EventArgs e)
        {
            user_list currentUser = CommonUserFunc.GetUserLoginStatus();
            currentUserId = currentUser != null && currentUser.id > 0 ? currentUser.id : 0;
            availableCouponCount = currentUserId > 0 ? exchangeLogBll.GetCount("integrateExchangeLog_list", "user_id=" + currentUserId + " and status=1 and name like N'%\u514D\u8D39\u4E0B\u8F7D%'") : 0;
            keyword = Function.GetRequest("keyword");
            ParseCategoryFilter();
            selectedYear = Function.ConvertTo<int>(Function.GetRequest("publish_year"), 0);
            if (string.Equals(Function.GetRequest("view"), "browse", StringComparison.OrdinalIgnoreCase))
            {
                RedirectLegacyBrowseView();
                return;
            }
            IsBrowseView = false;
            if (!IsPostBack)
            {
                BindCategories();
                BindYears();
                if (IsBrowseView)
                {
                    BindBrowseCategories();
                    SearchPanel.Visible = false;
                    BrowsePanel.Visible = true;
                }
                else
                {
                    BindData();
                    SearchPanel.Visible = true;
                    BrowsePanel.Visible = false;
                }
            }
        }

        private void BindCategories()
        {
            DataTable dt = categoryBll.GetDatatable("select id,name from LiteratureCategory where status=1 order by orderid asc,id asc");
            BuildCategoryCountMap();
            if (dt != null)
            {
                CategoryList.DataSource = dt.DefaultView;
                CategoryList.DataBind();
                CategoryNavList.DataSource = dt.DefaultView;
                CategoryNavList.DataBind();
            }
        }

        private void BuildCategoryCountMap()
        {
            categoryCountMap.Clear();
            DataTable dt = literatureBll.GetDatatable("select category_id,count(1) as total_count from Literature where status=1 and canonical_literature_id is null group by category_id");
            if (dt == null)
            {
                return;
            }

            foreach (DataRow row in dt.Rows)
            {
                int categoryId = Function.ConvertTo<int>(Convert.ToString(row["category_id"]), 0);
                int count = Function.ConvertTo<int>(Convert.ToString(row["total_count"]), 0);
                if (categoryId > 0 && !categoryCountMap.ContainsKey(categoryId))
                {
                    categoryCountMap.Add(categoryId, count);
                }
            }
            dt.Dispose();
        }

        private void BindYears()
        {
            DataTable dt = literatureBll.GetDatatable("select distinct publish_year from Literature where status=1 and canonical_literature_id is null and publish_year is not null order by publish_year desc");
            if (dt != null)
            {
                YearList.DataSource = dt.DefaultView;
                YearList.DataBind();
            }
        }

        private void ParseCategoryFilter()
        {
            string raw = (Function.GetRequest("category_id") ?? string.Empty).Trim();
            HasCategoryFilter = false;
            selectedCategoryId = 0;
            if (string.IsNullOrWhiteSpace(raw) || string.Equals(raw, "all", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (raw.StartsWith("cat:", StringComparison.OrdinalIgnoreCase))
            {
                raw = raw.Substring(4);
            }

            selectedCategoryId = Function.ConvertTo<int>(raw, 0);
            HasCategoryFilter = true;
        }

        private void BindData()
        {
            string where = BuildWhere();
            int pageIndex = Function.ConvertTo<int>(Function.GetRequest("page"), 1);
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }

            int pageSize = 10;
            totalCount = Function.ConvertTo<int>(literatureBll.GetDatatable("select count(1) as total_count from Literature l where " + where).Rows[0]["total_count"].ToString(), 0);
            int startRow = (pageIndex - 1) * pageSize + 1;
            int endRow = pageIndex * pageSize;
            string purchasedSql = currentUserId > 0
                ? "case when exists(select 1 from LiteratureDownloadLog d where d.literature_id=l.id and d.user_id=" + currentUserId + ") then 1 else 0 end"
                : "0";
            string sql = @"
select *
from
(
    select
        l.id,
        l.title,
        (select string_agg(a.name_cn,N'，') within group (order by m.author_order) from LiteratureAuthorMap m inner join Author a on a.id=m.author_id where m.literature_id=l.id) as author_names,
        l.institution,
        l.publish_year,
        l.source_type,
        l.download_points,
        l.userid,
        (select string_agg(t.name,N'，') from LiteratureTagMap m inner join LiteratureTag t on t.id=m.tag_id where m.literature_id=l.id and t.status<>-1) as tag_names,
        l.abstract_text,
        (select top 1 f.file_path from LiteratureFile f where f.literature_id=l.id and f.status=1 order by f.orderid asc,f.id asc) as pdf_file,
        (select count(1) from LiteratureLike lk where lk.literature_id=l.id) as like_count,
        (select count(1) from LiteratureFavorite fav where fav.literature_id=l.id) as favorite_count,
        ((select count(1) from LiteratureComment lc where lc.parent_id=0 and lc.is_deleted=0 and lc.status=1 and (lc.canonical_literature_id=l.id or lc.literature_id=l.id))
         + (select count(1) from ServiceLog_List s where s.name like N'%文献评论%' and s.info_ like N'%/LiteratureInfo.aspx?id=' + cast(l.id as nvarchar(20)) + N'%' and s.status in (1,2) and not exists(select 1 from LiteratureComment lc2 where lc2.source_service_log_id=s.id))) as comment_count,
        " + purchasedSql + @" as already_purchased,
        row_number() over(order by l.is_top desc,l.addtime desc,l.id desc) as row_no
    from Literature l
    where " + where + @"
) t
where t.row_no between " + startRow + " and " + endRow + @"
order by t.row_no";
            DataTable dt = literatureBll.GetDatatable(sql);
            EmptyPanel.Visible = dt == null || dt.Rows.Count <= 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                LiteratureRepeater.DataSource = dt.DefaultView;
                LiteratureRepeater.DataBind();
            }

            pagerHtml = BuildPager(pageIndex, pageSize);
            BuildSelectedCategoryOverview();
        }

        private void RedirectLegacyBrowseView()
        {
            StringBuilder url = new StringBuilder("/LiteratureSearch.aspx");
            if (HasCategoryFilter)
            {
                url.Append("?category_id=");
                url.Append(Server.UrlEncode(GetCategoryOptionValue(selectedCategoryId)));
            }
            Response.Redirect(url.ToString(), true);
        }

        private void BuildSelectedCategoryOverview()
        {
            BrowseCategoryInfoHtml = string.Empty;
            if (!HasCategoryFilter || selectedCategoryId <= 0)
            {
                return;
            }

            DataTable dt = categoryBll.GetDatatable("select top 1 name from LiteratureCategory where status=1 and id=" + selectedCategoryId);
            if (dt == null || dt.Rows.Count <= 0)
            {
                if (dt != null)
                {
                    dt.Dispose();
                }
                return;
            }

            string categoryName = Function.HtmlDiscode(Convert.ToString(dt.Rows[0]["name"]));
            dt.Dispose();
            BrowseCategoryInfoHtml = BuildBrowseCategoryInfoHtml(categoryName, totalCount);
        }

        private void BindBrowseCategories()
        {
            string sql = "select c.id,c.name,c.orderid,count(l.id) as lit_count from LiteratureCategory c left join Literature l on l.category_id=c.id and l.status=1 and l.canonical_literature_id is null where c.status=1 group by c.id,c.name,c.orderid order by c.orderid asc,c.id asc";
            DataTable dt = categoryBll.GetDatatable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                categoryCount = dt.Rows.Count;
                if (selectedCategoryId <= 0)
                {
                    selectedCategoryId = Function.ConvertTo<int>(dt.Rows[0]["id"].ToString(), 0);
                }
                HasCategoryFilter = selectedCategoryId > 0;
                BrowseCategoryListTitleHtml = "学科分组 · " + categoryCount;
                BrowseCategoryListHtml = BuildBrowseCategoryListHtml(dt);
                BuildSelectedBrowseCategory(dt);
                BrowseEmptyPanel.Visible = false;
            }
            else
            {
                BrowseCategoryTitleHtml = "暂无学科";
                BrowseCategorySummaryHtml = "当前还没有可浏览的学科分类。";
                BrowseCategoryInfoHtml = string.Empty;
                BrowseLiteratureListHtml = string.Empty;
                BrowseCategoryListTitleHtml = "学科分组 · 0";
                BrowseCategoryListHtml = "<div class=\"venue-empty\">暂无学科分组</div>";
                BrowseEmptyPanel.Visible = true;
            }
        }

        private string BuildBrowseCategoryListHtml(DataTable categoryTable)
        {
            if (categoryTable == null || categoryTable.Rows.Count <= 0)
            {
                return "<div class=\"venue-empty\">暂无学科分组</div>";
            }

            StringBuilder html = new StringBuilder();
            foreach (DataRow row in categoryTable.Rows)
            {
                int categoryId = Function.ConvertTo<int>(Convert.ToString(row["id"]), 0);
                string categoryName = Function.HtmlDiscode(Convert.ToString(row["name"]));
                int count = Function.ConvertTo<int>(Convert.ToString(row["lit_count"]), 0);
                string currentClass = categoryId == selectedCategoryId ? " current" : string.Empty;
                html.Append("<a class=\"venue-card");
                html.Append(currentClass);
                html.Append("\" href=\"/LiteratureSearch.aspx?view=browse&category_id=");
                html.Append(Server.UrlEncode(GetCategoryOptionValue(categoryId)));
                html.Append("\"><strong>");
                html.Append(Server.HtmlEncode(categoryName));
                html.Append("</strong><span>学科 · ");
                html.Append(count);
                html.Append(" 篇文献</span></a>");
            }
            return html.ToString();
        }

        private void BuildSelectedBrowseCategory(DataTable categoryTable)
        {
            DataRow selectedRow = null;
            foreach (DataRow row in categoryTable.Rows)
            {
                int categoryId = Function.ConvertTo<int>(row["id"].ToString(), 0);
                if (categoryId == selectedCategoryId)
                {
                    selectedRow = row;
                    break;
                }
            }

            if (selectedRow == null)
            {
                selectedRow = categoryTable.Rows[0];
                selectedCategoryId = Function.ConvertTo<int>(selectedRow["id"].ToString(), 0);
            }

            string categoryName = Function.HtmlDiscode(Convert.ToString(selectedRow["name"]));
            int litCount = Function.ConvertTo<int>(Convert.ToString(selectedRow["lit_count"]), 0);
            BrowseCategoryTitleHtml = Server.HtmlEncode(categoryName);
            BrowseCategorySummaryHtml = "学科 · 当前显示 " + litCount + " 篇公开文献";
            BrowseCategoryInfoHtml = BuildBrowseCategoryInfoHtml(categoryName, litCount);
            BrowseLiteratureListHtml = BuildBrowseLiteratureListHtml();
        }

        private string BuildBrowseCategoryInfoHtml(string categoryName, int litCount)
        {
            string condition = "status=1 and canonical_literature_id is null and category_id=" + selectedCategoryId;
            string minYear = GetScalarText("select cast(min(publish_year) as nvarchar(20)) from Literature where " + condition + " and publish_year is not null");
            string maxYear = GetScalarText("select cast(max(publish_year) as nvarchar(20)) from Literature where " + condition + " and publish_year is not null");
            int authorCount = GetScalarInt("select count(distinct m.author_id) from LiteratureAuthorMap m inner join Literature l on l.id=m.literature_id where l." + condition);
            string sourceTypes = GetTopGroupedValues("source_type", condition, 4);
            string sourceDbs = GetTopGroupedValues("source_db", condition, 4);
            string topTags = GetTopTagsByCategory(6);
            string topVenues = GetTopVenuesByCategory(4);
            string yearRange = !string.IsNullOrWhiteSpace(minYear) && !string.IsNullOrWhiteSpace(maxYear) ? (minYear == maxYear ? maxYear : minYear + " - " + maxYear) : "暂无";
            string latestYear = !string.IsNullOrWhiteSpace(maxYear) ? maxYear : "暂无";

            StringBuilder html = new StringBuilder();
            html.Append("<section class=\"venue-info-card\"><div class=\"venue-info-intro\"><h3>学科概览</h3><p>");
            html.Append(Server.HtmlEncode(categoryName));
            html.Append(" 汇总了平台中同一学科方向的公开文献，便于快速观察该方向的年份分布、来源类型、核心标签与常见投稿来源。</p></div><div class=\"venue-info-grid\">");
            AppendBrowseInfoItem(html, "平台文献数", litCount + " 篇");
            AppendBrowseInfoItem(html, "年份范围", Server.HtmlEncode(yearRange));
            AppendBrowseInfoItem(html, "作者规模", authorCount + " 位");
            AppendBrowseInfoItem(html, "文献类型", SafeHtml(sourceTypes));
            AppendBrowseInfoItem(html, "高频标签", SafeHtml(topTags));
            AppendBrowseInfoItem(html, "常见期刊/会议", SafeHtml(topVenues));
            AppendBrowseInfoItem(html, "主要来源库", SafeHtml(sourceDbs));
            AppendBrowseInfoItem(html, "最近年份", Server.HtmlEncode(latestYear));
            html.Append("</div></section>");
            return html.ToString();
        }

        private string BuildBrowseLiteratureListHtml()
        {
            if (selectedCategoryId <= 0)
            {
                return "<div class=\"lit-empty\">请选择一个学科查看文献。</div>";
            }

            string sql = @"
select top 100
    l.id,
    l.title,
    (select string_agg(a.name_cn,N'，') within group (order by m.author_order) from LiteratureAuthorMap m inner join Author a on a.id=m.author_id where m.literature_id=l.id) as author_names,
    l.institution,
    l.publish_year,
    l.source_type,
    l.abstract_text,
    (select string_agg(t.name,N'，') from LiteratureTagMap m inner join LiteratureTag t on t.id=m.tag_id where m.literature_id=l.id and t.status<>-1) as tag_names,
    (select count(1) from LiteratureLike lk where lk.literature_id=l.id) as like_count,
    (select count(1) from LiteratureFavorite fav where fav.literature_id=l.id) as favorite_count,
    ((select count(1) from LiteratureComment lc where lc.parent_id=0 and lc.is_deleted=0 and lc.status=1 and (lc.canonical_literature_id=l.id or lc.literature_id=l.id))
     + (select count(1) from ServiceLog_List s where s.name like N'%文献评论%' and s.info_ like N'%/LiteratureInfo.aspx?id=' + cast(l.id as nvarchar(20)) + N'%' and s.status in (1,2) and not exists(select 1 from LiteratureComment lc2 where lc2.source_service_log_id=s.id))) as comment_count
from Literature l
where l.status=1 and l.canonical_literature_id is null and l.category_id=" + selectedCategoryId + @"
order by l.is_top desc,l.publish_year desc,l.addtime desc,l.id desc";
            DataTable dt = literatureBll.GetDatatable(sql);
            if (dt == null || dt.Rows.Count <= 0)
            {
                return "<div class=\"lit-empty\">该学科下暂时还没有公开文献。</div>";
            }

            StringBuilder html = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                string title = Function.HtmlDiscode(Convert.ToString(row["title"]));
                int id = Function.ConvertTo<int>(Convert.ToString(row["id"]), 0);
                html.Append("<article class=\"venue-item\"><h3><a href=\"/LiteratureInfo.aspx?id=");
                html.Append(id);
                html.Append("\">");
                html.Append(Server.HtmlEncode(title));
                html.Append("</a></h3><div class=\"venue-meta\">");
                html.Append(Server.HtmlEncode(GetMeta(row["author_names"], row["institution"], row["publish_year"], row["source_type"])));
                html.Append("</div><div class=\"lit-tags\">");
                html.Append(GetTagHtml(row["tag_names"]));
                html.Append("</div><div class=\"venue-abstract\">");
                html.Append(Server.HtmlEncode(GetSummary(row["abstract_text"])));
                html.Append("</div><div class=\"lit-social-stats\"><span>点赞 <strong>");
                html.Append(Function.ConvertTo<int>(Convert.ToString(row["like_count"]), 0));
                html.Append("</strong></span><span>收藏 <strong>");
                html.Append(Function.ConvertTo<int>(Convert.ToString(row["favorite_count"]), 0));
                html.Append("</strong></span><span>评论 <strong>");
                html.Append(Function.ConvertTo<int>(Convert.ToString(row["comment_count"]), 0));
                html.Append("</strong></span></div><div class=\"lit-actions\"><a href=\"/LiteratureInfo.aspx?id=");
                html.Append(id);
                html.Append("\">查看详情</a></div></article>");
            }
            dt.Dispose();
            return html.ToString();
        }

        private void BuildBrowsePreviewMap(DataTable categoryTable)
        {
            browsePreviewMap.Clear();
            if (categoryTable == null || categoryTable.Rows.Count <= 0)
            {
                return;
            }

            foreach (DataRow row in categoryTable.Rows)
            {
                int categoryId = Function.ConvertTo<int>(row["id"].ToString(), 0);
                if (categoryId <= 0)
                {
                    continue;
                }

                DataTable previewDt = literatureBll.GetDatatable("select top 3 id,title from Literature where status=1 and canonical_literature_id is null and category_id=" + categoryId + " order by is_top desc,addtime desc,id desc");
                if (previewDt == null || previewDt.Rows.Count <= 0)
                {
                    browsePreviewMap[categoryId] = "<div class=\"lit-browse-preview-empty\">该学科下暂时还没有公开文献，可先查看其他分类或稍后再试。</div>";
                    continue;
                }

                StringBuilder sb = new StringBuilder();
                int index = 1;
                foreach (DataRow previewRow in previewDt.Rows)
                {
                    string title = Function.HtmlDiscode(previewRow["title"].ToString());
                    int literatureId = Function.ConvertTo<int>(previewRow["id"].ToString(), 0);
                    sb.Append("<a href=\"/LiteratureInfo.aspx?id=");
                    sb.Append(literatureId);
                    sb.Append("\"><em>0");
                    sb.Append(index);
                    sb.Append("</em>");
                    sb.Append(Server.HtmlEncode(title));
                    sb.Append("</a>");
                    index++;
                }
                browsePreviewMap[categoryId] = sb.ToString();
            }
        }

        private string BuildWhere()
        {
            string where = "l.status=1 and l.canonical_literature_id is null";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string safeKeyword = SqlLiteral(Function.HtmlEncode(keyword.Trim()));
                where += " and (l.title like N'%" + safeKeyword + "%' or l.institution like N'%" + safeKeyword + "%' or l.keywords like N'%" + safeKeyword + "%' or l.doi like N'%" + safeKeyword + "%' or l.journal_name like N'%" + safeKeyword + "%' or l.conference_name like N'%" + safeKeyword + "%' or exists(select 1 from LiteratureAuthorMap m inner join Author a on a.id=m.author_id where m.literature_id=l.id and a.name_cn like N'%" + safeKeyword + "%') or exists(select 1 from LiteratureTagMap tm inner join LiteratureTag t on t.id=tm.tag_id where tm.literature_id=l.id and t.name like N'%" + safeKeyword + "%'))";
            }
            if (HasCategoryFilter)
            {
                where += " and l.category_id=" + selectedCategoryId;
            }
            if (selectedYear > 0)
            {
                where += " and l.publish_year=" + selectedYear;
            }
            return where;
        }

        private string SqlLiteral(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }

        private string BuildPager(int pageIndex, int pageSize)
        {
            if (totalCount <= pageSize)
            {
                return string.Empty;
            }

            int pageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
            StringBuilder sb = new StringBuilder();
            if (pageIndex > 1)
            {
                sb.Append("<a href=\"");
                sb.Append(GetPageUrl(pageIndex - 1));
                sb.Append("\">\u4E0A\u4E00\u9875</a>");
            }
            sb.Append("\u7B2C ");
            sb.Append(pageIndex);
            sb.Append(" / ");
            sb.Append(pageCount);
            sb.Append(" \u9875");
            if (pageIndex < pageCount)
            {
                sb.Append(" <a href=\"");
                sb.Append(GetPageUrl(pageIndex + 1));
                sb.Append("\">\u4E0B\u4E00\u9875</a>");
            }
            return sb.ToString();
        }

        private string GetPageUrl(int page)
        {
            StringBuilder sb = new StringBuilder("/LiteratureSearch.aspx?page=" + page);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sb.Append("&keyword=" + Server.UrlEncode(keyword));
            }
            if (HasCategoryFilter)
            {
                sb.Append("&category_id=" + Server.UrlEncode(GetCategoryOptionValue(selectedCategoryId)));
            }
            if (selectedYear > 0)
            {
                sb.Append("&publish_year=" + selectedYear);
            }
            return sb.ToString();
        }

        public string GetMeta(object authorObj, object institutionObj, object yearObj, object sourceTypeObj)
        {
            StringBuilder sb = new StringBuilder();
            string author = Function.HtmlDiscode(authorObj == null ? string.Empty : authorObj.ToString());
            string institution = Function.HtmlDiscode(institutionObj == null ? string.Empty : institutionObj.ToString());
            string year = yearObj == null ? string.Empty : yearObj.ToString();
            string sourceType = Function.HtmlDiscode(sourceTypeObj == null ? string.Empty : sourceTypeObj.ToString());
            if (!string.IsNullOrWhiteSpace(author))
            {
                sb.Append(author);
            }
            if (!string.IsNullOrWhiteSpace(institution))
            {
                if (sb.Length > 0) sb.Append(" | ");
                sb.Append(institution);
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

        public string GetSummary(object abstractObj)
        {
            string text = Function.HtmlDiscode(abstractObj == null ? string.Empty : abstractObj.ToString());
            if (string.IsNullOrWhiteSpace(text))
            {
                return "\u6682\u65E0\u6458\u8981";
            }
            return text.Length > 220 ? text.Substring(0, 220) + "..." : text;
        }

        public string GetTagHtml(object tagObj)
        {
            string value = Function.HtmlDiscode(tagObj == null ? string.Empty : tagObj.ToString());
            if (string.IsNullOrWhiteSpace(value))
            {
                return "<span class=\"lit-tag\">\u672A\u8BBE\u7F6E\u6807\u7B7E</span>";
            }
            StringBuilder sb = new StringBuilder();
            string[] arr = value.Split(new[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in arr)
            {
                sb.Append("<span class=\"lit-tag\">");
                sb.Append(Server.HtmlEncode(item.Trim()));
                sb.Append("</span>");
            }
            return sb.ToString();
        }

        public string GetBatchCheckbox(object idObj, object pdfFileObj, object pointsObj, object purchasedObj, object userIdObj)
        {
            string pdfFile = pdfFileObj == null ? string.Empty : pdfFileObj.ToString();
            if (string.IsNullOrWhiteSpace(pdfFile))
            {
                return string.Empty;
            }
            int id = Function.ConvertTo<int>(idObj == null ? "0" : idObj.ToString(), 0);
            if (id <= 0)
            {
                return string.Empty;
            }
            int points = Function.ConvertTo<int>(pointsObj == null ? "0" : pointsObj.ToString(), 0);
            int purchased = Function.ConvertTo<int>(purchasedObj == null ? "0" : purchasedObj.ToString(), 0);
            int uploaderId = Function.ConvertTo<int>(userIdObj == null ? "0" : userIdObj.ToString(), 0);
            bool isUploader = currentUserId > 0 && uploaderId == currentUserId;
            int chargedPoints = isUploader ? 0 : Math.Max(0, points);
            string costText = isUploader
                ? "\u6295\u7A3F\u8005\u514D\u79EF\u5206"
                : purchased > 0
                ? "\u5DF2\u8D2D\u4E70"
                : (points <= 0 ? "\u514D\u79EF\u5206" : points + " \u79EF\u5206");
            return "<label class=\"lit-select-pdf\"><input type=\"checkbox\" class=\"lit-pdf-check\" name=\"literature_ids\" value=\"" + id + "\" data-points=\"" + chargedPoints + "\" data-purchased=\"" + purchased + "\" /> PDF <span>" + costText + "</span></label>";
        }

        public string GetPdfLink(object idObj, object pdfFileObj)
        {
            string pdfFile = pdfFileObj == null ? string.Empty : pdfFileObj.ToString();
            if (string.IsNullOrWhiteSpace(pdfFile))
            {
                return string.Empty;
            }
            int id = Function.ConvertTo<int>(idObj == null ? "0" : idObj.ToString(), 0);
            if (id <= 0)
            {
                return string.Empty;
            }
            return "<a href=\"/LiteratureInfo.aspx?id=" + id + "&action=download\">\u4E0B\u8F7D PDF</a>";
        }

        private int GetScalarInt(string sql)
        {
            return Function.ConvertTo<int>(GetScalarText(sql), 0);
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

        private string GetTopTagsByCategory(int top)
        {
            string sql = "select top " + top + " t.name,count(1) as num from Literature l inner join LiteratureTagMap m on m.literature_id=l.id inner join LiteratureTag t on t.id=m.tag_id where l.status=1 and l.canonical_literature_id is null and l.category_id=" + selectedCategoryId + " and t.status<>-1 group by t.name order by num desc,t.name asc";
            DataTable dt = literatureBll.GetDatatable(sql);
            string result = BuildNameCountList(dt, "name");
            if (dt != null)
            {
                dt.Dispose();
            }
            return result;
        }

        private string GetTopVenuesByCategory(int top)
        {
            string sql = @"
select top " + top + @" venue_name,count(1) as num
from
(
                select LTRIM(RTRIM(journal_name)) as venue_name from Literature where status=1 and canonical_literature_id is null and category_id=" + selectedCategoryId + @" and LTRIM(RTRIM(isnull(journal_name,N'')))<>N''
    union all
                select LTRIM(RTRIM(conference_name)) as venue_name from Literature where status=1 and canonical_literature_id is null and category_id=" + selectedCategoryId + @" and LTRIM(RTRIM(isnull(conference_name,N'')))<>N''
) v
group by venue_name
order by num desc,venue_name asc";
            DataTable dt = literatureBll.GetDatatable(sql);
            string result = BuildNameCountList(dt, "venue_name");
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

        private void AppendBrowseInfoItem(StringBuilder html, string label, string value)
        {
            html.Append("<div><span>");
            html.Append(label);
            html.Append("</span><strong>");
            html.Append(string.IsNullOrWhiteSpace(value) ? "暂无" : value);
            html.Append("</strong></div>");
        }

        private string SafeHtml(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "暂无" : Server.HtmlEncode(value);
        }

        public string GetAllCategoryNavUrl()
        {
            return IsBrowseView ? "/LiteratureSearch.aspx?view=browse" : "/LiteratureSearch.aspx";
        }

        public string GetCategoryNavUrl(object categoryIdObj)
        {
            string categoryValue = GetCategoryOptionValue(categoryIdObj);
            return IsBrowseView
                ? "/LiteratureSearch.aspx?view=browse&category_id=" + Server.UrlEncode(categoryValue)
                : "/LiteratureSearch.aspx?category_id=" + Server.UrlEncode(categoryValue);
        }

        public string GetCategoryNavInner(object categoryIdObj, object nameObj)
        {
            string name = Function.HtmlDiscode(nameObj == null ? string.Empty : nameObj.ToString());
            int categoryId = Function.ConvertTo<int>(categoryIdObj == null ? "0" : categoryIdObj.ToString(), 0);
            int count = categoryCountMap.ContainsKey(categoryId) ? categoryCountMap[categoryId] : 0;
            if (!IsBrowseView)
            {
                return Server.HtmlEncode(name);
            }
            return "<strong>" + Server.HtmlEncode(name) + "</strong><span>&#23398;&#31185; · " + count + " &#31687;&#25991;&#29486;</span>";
        }

        public string GetCategorySummary(object nameObj, object countObj)
        {
            string name = Function.HtmlDiscode(nameObj == null ? string.Empty : nameObj.ToString());
            int count = Function.ConvertTo<int>(countObj == null ? "0" : countObj.ToString(), 0);
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "未命名分类";
            }
            return "当前学科下已收录 " + count + " 篇公开文献，可继续按关键词、年份和作者进行细化检索。";
        }

        public string GetBrowsePreviewHtml(object categoryIdObj)
        {
            int categoryId = Function.ConvertTo<int>(categoryIdObj == null ? "0" : categoryIdObj.ToString(), 0);
            if (categoryId > 0 && browsePreviewMap.ContainsKey(categoryId))
            {
                return browsePreviewMap[categoryId];
            }
            return "<div class=\"lit-browse-preview-empty\">该学科下暂时还没有公开文献，可先查看其他分类或稍后再试。</div>";
        }

        public string GetCategoryOptionValue(object categoryIdObj)
        {
            int categoryId = Function.ConvertTo<int>(categoryIdObj == null ? "0" : categoryIdObj.ToString(), 0);
            return "cat:" + categoryId;
        }

        public string GetCategorySelectedAttr(object categoryIdObj)
        {
            int categoryId = Function.ConvertTo<int>(categoryIdObj == null ? "0" : categoryIdObj.ToString(), 0);
            return HasCategoryFilter && selectedCategoryId == categoryId ? "selected=\"selected\"" : string.Empty;
        }

        public string GetCategoryCurrentClass(object categoryIdObj)
        {
            int categoryId = Function.ConvertTo<int>(categoryIdObj == null ? "0" : categoryIdObj.ToString(), 0);
            return (HasCategoryFilter || IsBrowseView) && selectedCategoryId == categoryId ? "current" : string.Empty;
        }
    }
}
