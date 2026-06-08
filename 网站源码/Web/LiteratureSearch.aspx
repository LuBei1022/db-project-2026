<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LiteratureSearch.aspx.cs" Inherits="Web.LiteratureSearch" %>

<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<script runat="server">
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (string.Equals(Request["view"], "browse", StringComparison.OrdinalIgnoreCase))
        {
            string rawCategory = (Request["category_id"] ?? string.Empty).Trim();
            string redirectUrl = "/LiteratureSearch.aspx";
            if (!string.IsNullOrWhiteSpace(rawCategory) && !string.Equals(rawCategory, "all", StringComparison.OrdinalIgnoreCase))
            {
                redirectUrl += "?category_id=" + Server.UrlEncode(rawCategory);
            }
            Response.Redirect(redirectUrl, true);
        }
    }

    protected string RenderSelectedCategoryOverviewInline()
    {
        if (!HasCategoryFilter || selectedCategoryId <= 0)
        {
            return string.Empty;
        }

        BLL.BLLBase<Model.Literature> literatureBllInline = new BLL.BLLBase<Model.Literature>();
        BLL.BLLBase<Model.LiteratureCategory> categoryBllInline = new BLL.BLLBase<Model.LiteratureCategory>();
        System.Data.DataTable categoryDt = categoryBllInline.GetDatatable("select top 1 name from LiteratureCategory where status=1 and id=" + selectedCategoryId);
        if (categoryDt == null || categoryDt.Rows.Count <= 0)
        {
            if (categoryDt != null)
            {
                categoryDt.Dispose();
            }
            return string.Empty;
        }

        string categoryName = LiteratureManager.Common.Function.HtmlDiscode(Convert.ToString(categoryDt.Rows[0]["name"]));
        categoryDt.Dispose();
        string condition = "status=1 and category_id=" + selectedCategoryId;
        string minYear = GetOverviewScalarText(literatureBllInline, "select cast(min(publish_year) as nvarchar(20)) from Literature where " + condition + " and publish_year is not null");
        string maxYear = GetOverviewScalarText(literatureBllInline, "select cast(max(publish_year) as nvarchar(20)) from Literature where " + condition + " and publish_year is not null");
        int authorCount = LiteratureManager.Common.Function.ConvertTo<int>(GetOverviewScalarText(literatureBllInline, "select count(distinct m.author_id) from LiteratureAuthorMap m inner join Literature l on l.id=m.literature_id where l." + condition), 0);
        string sourceTypes = GetOverviewTopGroupedValues(literatureBllInline, "source_type", condition, 4);
        string sourceDbs = GetOverviewTopGroupedValues(literatureBllInline, "source_db", condition, 4);
        string topTags = GetOverviewTopTags(literatureBllInline, selectedCategoryId, 6);
        string topVenues = GetOverviewTopVenues(literatureBllInline, selectedCategoryId, 4);
        string yearRange = !string.IsNullOrWhiteSpace(minYear) && !string.IsNullOrWhiteSpace(maxYear) ? (minYear == maxYear ? maxYear : minYear + " - " + maxYear) : "暂无";
        string latestYear = !string.IsNullOrWhiteSpace(maxYear) ? maxYear : "暂无";

        System.Text.StringBuilder html = new System.Text.StringBuilder();
        html.Append("<section class=\"venue-info-card\"><div class=\"venue-info-intro\"><h3>学科概览</h3><p>");
        html.Append(Server.HtmlEncode(categoryName));
        html.Append(" 汇总了平台中同一学科方向的公开文献，便于快速观察该方向的年份分布、来源类型、核心标签与常见投稿来源。</p></div><div class=\"venue-info-grid\">");
        AppendOverviewInfoItem(html, "平台文献数", totalCount + " 篇");
        AppendOverviewInfoItem(html, "年份范围", Server.HtmlEncode(yearRange));
        AppendOverviewInfoItem(html, "作者规模", authorCount + " 位");
        AppendOverviewInfoItem(html, "文献类型", SafeOverviewHtml(sourceTypes));
        AppendOverviewInfoItem(html, "高频标签", SafeOverviewHtml(topTags));
        AppendOverviewInfoItem(html, "常见期刊/会议", SafeOverviewHtml(topVenues));
        AppendOverviewInfoItem(html, "主要来源库", SafeOverviewHtml(sourceDbs));
        AppendOverviewInfoItem(html, "最近年份", Server.HtmlEncode(latestYear));
        html.Append("</div></section>");
        return html.ToString();
    }

    private string GetOverviewScalarText(BLL.BLLBase<Model.Literature> bll, string sql)
    {
        System.Data.DataTable dt = bll.GetDatatable(sql);
        string value = string.Empty;
        if (dt != null && dt.Rows.Count > 0)
        {
            value = LiteratureManager.Common.Function.HtmlDiscode(Convert.ToString(dt.Rows[0][0]));
            dt.Dispose();
        }
        return value;
    }

    private string GetOverviewTopGroupedValues(BLL.BLLBase<Model.Literature> bll, string field, string condition, int top)
    {
        System.Data.DataTable dt = bll.GetDatatable("select top " + top + " " + field + ",count(1) as num from Literature where " + condition + " and LTRIM(RTRIM(isnull(" + field + ",N'')))<>N'' group by " + field + " order by num desc," + field + " asc");
        string result = BuildOverviewNameCountList(dt, field);
        if (dt != null)
        {
            dt.Dispose();
        }
        return result;
    }

    private string GetOverviewTopTags(BLL.BLLBase<Model.Literature> bll, int categoryId, int top)
    {
        System.Data.DataTable dt = bll.GetDatatable("select top " + top + " t.name,count(1) as num from Literature l inner join LiteratureTagMap m on m.literature_id=l.id inner join LiteratureTag t on t.id=m.tag_id where l.status=1 and l.category_id=" + categoryId + " and t.status<>-1 group by t.name order by num desc,t.name asc");
        string result = BuildOverviewNameCountList(dt, "name");
        if (dt != null)
        {
            dt.Dispose();
        }
        return result;
    }

    private string GetOverviewTopVenues(BLL.BLLBase<Model.Literature> bll, int categoryId, int top)
    {
        string sql = "select top " + top + " venue_name,count(1) as num from (select LTRIM(RTRIM(journal_name)) as venue_name from Literature where status=1 and category_id=" + categoryId + " and LTRIM(RTRIM(isnull(journal_name,N'')))<>N'' union all select LTRIM(RTRIM(conference_name)) as venue_name from Literature where status=1 and category_id=" + categoryId + " and LTRIM(RTRIM(isnull(conference_name,N'')))<>N'') v group by venue_name order by num desc,venue_name asc";
        System.Data.DataTable dt = bll.GetDatatable(sql);
        string result = BuildOverviewNameCountList(dt, "venue_name");
        if (dt != null)
        {
            dt.Dispose();
        }
        return result;
    }

    private string BuildOverviewNameCountList(System.Data.DataTable dt, string nameField)
    {
        if (dt == null || dt.Rows.Count == 0)
        {
            return string.Empty;
        }
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (System.Data.DataRow row in dt.Rows)
        {
            string name = LiteratureManager.Common.Function.HtmlDiscode(Convert.ToString(row[nameField]));
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
            sb.Append(LiteratureManager.Common.Function.ConvertTo<int>(Convert.ToString(row["num"]), 0));
        }
        return sb.ToString();
    }

    private void AppendOverviewInfoItem(System.Text.StringBuilder html, string label, string value)
    {
        html.Append("<div><span>");
        html.Append(label);
        html.Append("</span><strong>");
        html.Append(string.IsNullOrWhiteSpace(value) ? "暂无" : value);
        html.Append("</strong></div>");
    }

    private string SafeOverviewHtml(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? "暂无" : Server.HtmlEncode(value);
    }
</script>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <title>&#25991;&#29486;&#26816;&#32034;</title>
    <LiteratureManager:css ID="css" runat="server" />
    <style>
        .lit-wrap { max-width: 1400px; margin: 0 auto; padding: 30px 20px 60px; }
        .lit-hero { background: linear-gradient(135deg, #eef6ff 0%, #f9fbff 100%); border: 1px solid #dbe7f4; border-radius: 24px; padding: 32px; margin-bottom: 26px; }
        .lit-hero h1 { font-size: 34px; margin: 0 0 12px; color: #16324f; }
        .lit-hero p { margin: 0 0 22px; color: #5b6b7d; font-size: 15px; }
        .lit-search-row { display: flex; gap: 14px; flex-wrap: wrap; }
        .lit-search-row input, .lit-search-row select { height: 44px; border-radius: 12px; border: 1px solid #cdd9e5; padding: 0 14px; font-size: 14px; background: #fff; }
        .lit-search-key { flex: 1 1 420px; }
        .lit-search-btn { height: 44px; padding: 0 22px; border: none; border-radius: 12px; background: #1d6fdc; color: #fff; font-size: 14px; cursor: pointer; }
        .lit-grid { display: grid; grid-template-columns: 280px minmax(0, 1fr); gap: 22px; }
        .lit-side, .lit-main { background: #fff; border-radius: 20px; border: 1px solid #ebeff4; }
        .lit-side { padding: 22px; }
        .lit-side h3 { margin: 0 0 14px; font-size: 18px; color: #1b2a3a; }
        .lit-side a { display: block; padding: 10px 12px; border-radius: 10px; color: #49596b; margin-bottom: 6px; }
        .lit-side a.current, .lit-side a:hover { background: #eef5ff; color: #1d6fdc; }
        .lit-browse-mode .lit-wrap { max-width: 1280px; margin: 0 auto; padding: 30px 20px 60px; }
        .lit-browse-mode .lit-wrap > .lit-grid { display: none; }
        .venue-hero { border: 1px solid #dbe7f4; border-radius: 24px; background: linear-gradient(135deg, #eef6ff 0%, #fbfdff 100%); padding: 30px; margin-bottom: 22px; }
        .venue-hero h1 { margin: 0 0 10px; color: #16324f; font-size: 32px; }
        .venue-hero p { margin: 0; color: #5b6b7d; line-height: 1.8; }
        .venue-grid { display: grid; grid-template-columns: 340px minmax(0, 1fr); gap: 22px; }
        .venue-panel { background: #fff; border: 1px solid #ebeff4; border-radius: 20px; overflow: hidden; }
        .venue-panel-head { padding: 18px 20px; border-bottom: 1px solid #edf1f5; color: #172b40; font-size: 18px; font-weight: 700; }
        .venue-list { max-height: 760px; overflow: auto; padding: 12px; }
        .venue-card { display: block; padding: 13px 14px; border-radius: 14px; color: #25384f; margin-bottom: 8px; }
        .venue-card:hover, .venue-card.current { background: #eef5ff; color: #1d6fdc; }
        .venue-card strong { display: block; line-height: 1.5; }
        .venue-card span { display: block; margin-top: 5px; color: #7a8795; font-size: 13px; }
        .venue-main { padding: 0; }
        .venue-summary { padding: 20px 24px; border-bottom: 1px solid #edf1f5; }
        .venue-summary h2 { margin: 0 0 8px; color: #172b40; font-size: 24px; line-height: 1.45; }
        .venue-summary p { margin: 0; color: #6f7d8d; }
        .venue-info-card { margin: 18px 24px 6px; padding: 18px; border: 1px solid #e5edf6; border-radius: 16px; background: #fbfdff; }
        .venue-info-intro h3 { margin: 0 0 8px; color: #172b40; font-size: 18px; }
        .venue-info-intro p { margin: 0 0 16px; color: #526174; line-height: 1.8; }
        .venue-info-grid { display: grid; grid-template-columns: repeat(4, minmax(0, 1fr)); gap: 10px; }
        .venue-info-grid div { padding: 12px; border: 1px solid #e7edf4; border-radius: 12px; background: #fff; min-width: 0; }
        .venue-info-grid span { display: block; margin-bottom: 6px; color: #7a8795; font-size: 12px; }
        .venue-info-grid strong { display: block; color: #1f344d; font-size: 14px; line-height: 1.55; word-break: break-word; }
        .venue-items { padding: 4px 24px 24px; }
        .venue-item { padding: 20px 0; border-bottom: 1px solid #edf1f5; }
        .venue-item:last-child { border-bottom: 0; }
        .venue-item h3 { margin: 0 0 8px; font-size: 20px; line-height: 1.45; }
        .venue-item h3 a { color: #15283d; }
        .venue-meta { color: #617286; line-height: 1.8; font-size: 14px; }
        .venue-abstract { margin-top: 8px; color: #4c5c6c; line-height: 1.8; font-size: 14px; }
        .venue-empty { padding: 46px 24px; text-align: center; color: #7e8b99; }
        .lit-browse-mode .lit-hero { padding: 30px; margin-bottom: 22px; }
        .lit-browse-tabs { display: flex; gap: 10px; flex-wrap: wrap; margin: 18px 0 0; }
        .lit-browse-tabs a { display: inline-flex; align-items: center; min-height: 38px; padding: 0 16px; border-radius: 999px; border: 1px solid #d9e8fb; background: #fff; color: #1d6fdc; }
        .lit-browse-tabs a.current, .lit-browse-tabs a:hover { background: #1d6fdc; color: #fff; }
        .lit-browse-mode .lit-grid { display: grid; grid-template-columns: 340px minmax(0, 1fr); gap: 22px; }
        .lit-browse-mode .lit-side, .lit-browse-mode .lit-main { background: #fff; border: 1px solid #ebeff4; border-radius: 20px; overflow: hidden; }
        .lit-browse-mode .lit-side { padding: 0; }
        .lit-browse-mode .lit-side h3 { padding: 18px 20px; margin: 0; border-bottom: 1px solid #edf1f5; color: #172b40; font-size: 18px; font-weight: 700; }
        .lit-browse-mode .lit-side-list { max-height: 760px; overflow: auto; padding: 12px; }
        .lit-browse-mode .lit-side a { display: block; padding: 13px 14px; border-radius: 14px; color: #25384f; margin-bottom: 8px; }
        .lit-browse-mode .lit-side a.current, .lit-browse-mode .lit-side a:hover { background: #eef5ff; color: #1d6fdc; }
        .lit-browse-mode .lit-side a strong { display: block; line-height: 1.5; }
        .lit-browse-mode .lit-side a span { display: block; margin-top: 5px; color: #7a8795; font-size: 13px; }
        .lit-main { padding: 10px 0; }
        .lit-browse-mode .lit-main { padding: 0; }
        .lit-topbar { padding: 12px 24px 6px; color: #728196; font-size: 14px; }
        .lit-batchbar { display: flex; align-items: center; justify-content: space-between; gap: 16px; margin: 8px 24px 14px; padding: 14px 18px; border: 1px solid #e0e0e0; border-radius: 18px; background: #fafafc; color: #1d1d1f; }
        .lit-batchbar-left, .lit-batchbar-right { display: flex; align-items: center; gap: 12px; flex-wrap: wrap; }
        .lit-batchbar label { display: inline-flex; align-items: center; gap: 7px; cursor: pointer; }
        .lit-batch-cost { color: #333; font-size: 14px; }
        .lit-batchbar select { height: 38px; border: 1px solid #d2d2d7; border-radius: 999px; padding: 0 14px; background: #fff; color: #1d1d1f; }
        .lit-batchbar button { min-height: 40px; border: 0; border-radius: 999px; padding: 0 18px; background: #0066cc; color: #fff; cursor: pointer; }
        .lit-select-pdf { display: inline-flex; align-items: center; gap: 8px; margin: 0 0 8px; color: #333; font-size: 14px; }
        .lit-item { padding: 20px 24px; border-top: 1px solid #edf1f5; }
        .lit-item:first-child { border-top: none; }
        .lit-item h2 { margin: 0 0 10px; font-size: 22px; line-height: 1.4; }
        .lit-item h2 a { color: #15283d; }
        .lit-meta { color: #617286; font-size: 14px; margin-bottom: 10px; }
        .lit-tags { margin-bottom: 12px; }
        .lit-tag { display: inline-block; padding: 4px 10px; border-radius: 999px; background: #eef5ff; color: #215da8; font-size: 12px; margin: 0 8px 8px 0; }
        .lit-abs { color: #4c5c6c; line-height: 1.8; font-size: 14px; }
        .lit-social-stats { display: flex; align-items: center; gap: 8px; flex-wrap: wrap; margin-top: 14px; color: #6e6e73; font-size: 13px; }
        .lit-social-stats span { display: inline-flex; align-items: center; min-height: 28px; padding: 0 10px; border: 1px solid #e5e5ea; border-radius: 999px; background: #fff; }
        .lit-social-stats strong { margin-left: 5px; color: #1d1d1f; font-weight: 700; }
        .lit-actions { margin-top: 14px; }
        .lit-actions a { display: inline-block; margin-right: 16px; color: #1d6fdc; font-size: 14px; }
        .lit-empty { padding: 60px 24px; text-align: center; color: #7e8b99; }
        .lit-pager { padding: 20px 24px 28px; color: #5b6b7d; }
        .lit-pager a { margin-right: 16px; color: #1d6fdc; }
        .lit-browse-summary { padding: 20px 24px; border-bottom: 1px solid #edf1f5; }
        .lit-browse-summary h2 { margin: 0 0 8px; color: #172b40; font-size: 24px; line-height: 1.45; }
        .lit-browse-summary p { margin: 0; color: #6f7d8d; line-height: 1.8; }
        .lit-browse-info { margin: 18px 24px 6px; padding: 18px; border: 1px solid #e5edf6; border-radius: 16px; background: #fbfdff; }
        .lit-browse-info h3 { margin: 0 0 8px; color: #172b40; font-size: 18px; }
        .lit-browse-info p { margin: 0 0 16px; color: #526174; line-height: 1.8; }
        .lit-browse-info-grid { display: grid; grid-template-columns: repeat(4, minmax(0, 1fr)); gap: 10px; }
        .lit-browse-info-grid div { padding: 12px; border: 1px solid #e7edf4; border-radius: 12px; background: #fff; min-width: 0; }
        .lit-browse-info-grid span { display: block; margin-bottom: 6px; color: #7a8795; font-size: 12px; }
        .lit-browse-info-grid strong { display: block; color: #1f344d; font-size: 14px; line-height: 1.55; word-break: break-word; }
        .lit-browse-items { padding: 4px 24px 24px; }
        .lit-browse-mode .lit-item { padding: 20px 0; border-top: 0; border-bottom: 1px solid #edf1f5; }
        .lit-browse-mode .lit-item:last-child { border-bottom: 0; }
        .lit-browse-mode .lit-item h2 { margin: 0 0 8px; font-size: 20px; line-height: 1.45; }
        .lit-browse-mode .lit-item h2 a { color: #15283d; }
        .lit-browse-mode .lit-meta { color: #617286; line-height: 1.8; font-size: 14px; margin-bottom: 8px; }
        .lit-browse-mode .lit-tags { margin-bottom: 8px; }
        .lit-browse-mode .lit-abs { color: #4c5c6c; line-height: 1.8; font-size: 14px; }
        .lit-browse-mode .lit-social-stats { margin-top: 12px; }
        .lit-browse-mode .lit-actions { margin-top: 12px; }
        @media (max-width: 960px) {
            .lit-grid { grid-template-columns: 1fr; }
            .venue-grid { grid-template-columns: 1fr; }
            .venue-list { max-height: none; }
            .venue-info-grid { grid-template-columns: repeat(2, minmax(0, 1fr)); }
            .lit-hero { padding: 24px; }
            .lit-hero h1 { font-size: 28px; }
            .lit-batchbar { align-items: stretch; flex-direction: column; }
            .lit-batchbar-left, .lit-batchbar-right { align-items: stretch; flex-direction: column; }
            .lit-browse-info-grid { grid-template-columns: repeat(2, minmax(0, 1fr)); }
        }
        @media (max-width: 640px) {
            .lit-browse-info-grid { grid-template-columns: 1fr; }
            .venue-info-grid { grid-template-columns: 1fr; }
        }
    </style>
</head>
<body class="ac <%= IsBrowseView ? "lit-browse-mode" : "" %>" style="background: #f6f8fb;">
    <LiteratureManager:top ID="top" runat="server" />
    <div class="middle">
        <div class="lit-wrap">
            <% if (!IsBrowseView) { %>
            <div class="lit-hero">
                <h1><%= IsBrowseView ? "&#23398;&#31185;&#25991;&#29486;&#27983;&#35272;" : "&#23398;&#26415;&#25991;&#29486;&#26816;&#32034;" %></h1>
                <p><%= IsBrowseView ? "&#25353;&#23398;&#31185;&#20998;&#31867;&#24555;&#36895;&#27983;&#35272;&#25991;&#29486;&#36164;&#28304;&#65292;&#20808;&#36827;&#20837;&#30456;&#24212;&#23398;&#31185;&#20877;&#32467;&#21512;&#24180;&#20221;&#12289;&#20851;&#38190;&#35789;&#36827;&#34892;&#31614;&#36873;&#12290;" : "&#25903;&#25345;&#25353;&#26631;&#39064;&#12289;&#20316;&#32773;&#12289;&#21333;&#20301;&#12289;&#20851;&#38190;&#35789;&#12289;DOI &#21644;&#26399;&#21002;/&#20250;&#35758;&#21517;&#31216;&#26816;&#32034;&#12290;" %></p>
                <div class="lit-search-row">
                    <input type="text" id="search_keyword" class="lit-search-key" value="<%=keyword %>" placeholder="&#36755;&#20837;&#26631;&#39064;&#12289;&#20316;&#32773;&#12289;&#20851;&#38190;&#35789;&#25110; DOI" />
                    <select id="search_category">
                        <option value="all" <%= !HasCategoryFilter ? "selected=\"selected\"" : "" %>>&#20840;&#37096;&#20998;&#31867;</option>
                        <asp:Repeater ID="CategoryList" runat="server">
                            <ItemTemplate>
                                <option value="<%# GetCategoryOptionValue(Eval("id")) %>" <%# GetCategorySelectedAttr(Eval("id")) %>><%# Function.HtmlDiscode(Eval("name").ToString()) %></option>
                            </ItemTemplate>
                        </asp:Repeater>
                    </select>
                    <select id="search_year">
                        <option value="0">&#20840;&#37096;&#24180;&#20221;</option>
                        <asp:Repeater ID="YearList" runat="server">
                            <ItemTemplate>
                                <option value="<%# Eval("publish_year") %>" <%# selectedYear.ToString()==Eval("publish_year").ToString()?"selected=\"selected\"":"" %>><%# Eval("publish_year") %></option>
                            </ItemTemplate>
                        </asp:Repeater>
                    </select>
                    <button type="button" class="lit-search-btn" onclick="literatureSearchSubmit()">&#24320;&#22987;&#26816;&#32034;</button>
                </div>
            </div>
            <% } %>
            <% if (IsBrowseView) { %>
            <div class="venue-hero">
                <h1>&#23398;&#31185;&#27983;&#35272;</h1>
                <p>&#24179;&#21488;&#20250;&#33258;&#21160;&#27719;&#24635;&#24050;&#20844;&#24320;&#25991;&#29486;&#30340;&#23398;&#31185;&#20998;&#31867;&#65292;&#24182;&#23558;&#23545;&#24212;&#25991;&#29486;&#24402;&#20837;&#21508;&#33258;&#30340;&#23398;&#31185;&#20998;&#32452;&#12290;</p>
            </div>
            <div class="venue-grid">
                <div class="venue-panel">
                    <div class="venue-panel-head"><%=BrowseCategoryListTitleHtml %></div>
                    <div class="venue-list">
                        <%=BrowseCategoryListHtml %>
                    </div>
                </div>
                <div class="venue-panel venue-main">
                    <div class="venue-summary">
                        <h2><%=BrowseCategoryTitleHtml %></h2>
                        <p><%=BrowseCategorySummaryHtml %></p>
                    </div>
                    <%=BrowseCategoryInfoHtml %>
                    <div class="venue-items">
                        <%=BrowseLiteratureListHtml %>
                    </div>
                </div>
            </div>
            <% } %>
            <div class="lit-grid">
                <div class="lit-side">
                    <h3><%= IsBrowseView ? "&#23398;&#31185;&#20998;&#32452; · " + categoryCount : "&#28909;&#38376;&#20998;&#31867;" %></h3>
                    <div class="lit-side-list">
                        <% if (!IsBrowseView) { %>
                        <a href="<%=GetAllCategoryNavUrl() %>" class="<%= !HasCategoryFilter ? "current" : "" %>">&#20840;&#37096;&#25991;&#29486;</a>
                        <% } %>
                        <asp:Repeater ID="CategoryNavList" runat="server">
                            <ItemTemplate>
                                <a href="<%# GetCategoryNavUrl(Eval("id")) %>" class="<%# GetCategoryCurrentClass(Eval("id")) %>"><%# GetCategoryNavInner(Eval("id"), Eval("name")) %></a>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="lit-main">
                    <asp:Panel ID="BrowsePanel" runat="server" Visible="false">
                        <div class="lit-browse-summary">
                            <h2><%=BrowseCategoryTitleHtml %></h2>
                            <p><%=BrowseCategorySummaryHtml %></p>
                        </div>
                        <%=BrowseCategoryInfoHtml %>
                        <div class="lit-browse-items">
                            <%=BrowseLiteratureListHtml %>
                        </div>
                        <asp:Panel ID="BrowseEmptyPanel" runat="server" Visible="false">
                            <div class="lit-empty">&#26242;&#26080;&#21487;&#27983;&#35272;&#30340;&#23398;&#31185;&#20998;&#31867;</div>
                        </asp:Panel>
                    </asp:Panel>
                    <asp:Panel ID="SearchPanel" runat="server" Visible="true">
                        <form id="batchDownloadForm" method="post" action="/LiteratureBatchDownload.ashx" onsubmit="return submitBatchDownload();">
                            <div class="lit-topbar">&#20849;&#25214;&#21040; <strong><%=totalCount %></strong> &#31687;&#25991;&#29486;</div>
                            <%=RenderSelectedCategoryOverviewInline() %>
                            <div class="lit-batchbar">
                                <div class="lit-batchbar-left">
                                    <label><input type="checkbox" id="litSelectAll" onclick="toggleLiteratureSelection(this)" /> 选择本页可下载 PDF</label>
                                    <span id="litSelectedCount">已选择 0 篇</span>
                                    <span id="litBatchCost" class="lit-batch-cost">预计消耗 0 积分</span>
                                </div>
                                <div class="lit-batchbar-right">
                                    <select name="pay_method" id="batchPayMethod" onchange="updateLiteratureSelectedCount()">
                                        <option value="points">积分下载</option>
                                        <option value="coupon">优先使用免费下载券</option>
                                    </select>
                                    <button type="submit">批量下载 PDF</button>
                                </div>
                            </div>
                            <asp:Repeater ID="LiteratureRepeater" runat="server">
                                <ItemTemplate>
                                    <div class="lit-item">
                                        <%# GetBatchCheckbox(Eval("id"), Eval("pdf_file"), Eval("download_points"), Eval("already_purchased"), Eval("userid")) %>
                                        <h2><a href="/LiteratureInfo.aspx?id=<%# Eval("id") %>"><%# Function.HtmlDiscode(Eval("title").ToString()) %></a></h2>
                                        <div class="lit-meta"><%# GetMeta(Eval("author_names"), Eval("institution"), Eval("publish_year"), Eval("source_type")) %></div>
                                        <div class="lit-tags"><%# GetTagHtml(Eval("tag_names")) %></div>
                                        <div class="lit-abs"><%# GetSummary(Eval("abstract_text")) %></div>
                                        <div class="lit-social-stats">
                                            <span>点赞 <strong><%# Eval("like_count") %></strong></span>
                                            <span>收藏 <strong><%# Eval("favorite_count") %></strong></span>
                                            <span>评论 <strong><%# Eval("comment_count") %></strong></span>
                                        </div>
                                        <div class="lit-actions">
                                            <a href="/LiteratureInfo.aspx?id=<%# Eval("id") %>">&#26597;&#30475;&#35814;&#24773;</a>
                                            <%# GetPdfLink(Eval("id"), Eval("pdf_file")) %>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </form>
                        <asp:Panel ID="EmptyPanel" runat="server" Visible="false">
                            <div class="lit-empty">&#26242;&#26080;&#31526;&#21512;&#26465;&#20214;&#30340;&#25991;&#29486;&#35760;&#24405;</div>
                        </asp:Panel>
                        <div class="lit-pager"><%=pagerHtml %></div>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>
    <LiteratureManager:foot ID="foot" runat="server" />
    <script type="text/javascript">
        function literatureSearchSubmit() {
            var url = "/LiteratureSearch.aspx?";
            var keyword = ($("#search_keyword").val() || "").trim();
            var categoryId = $("#search_category").val() || "all";
            var year = $("#search_year").val() || "0";
            if (keyword) {
                url += "keyword=" + encodeURIComponent(keyword) + "&";
            }
            if (categoryId !== "all") {
                url += "category_id=" + encodeURIComponent(categoryId) + "&";
            }
            if (year !== "0") {
                url += "publish_year=" + encodeURIComponent(year) + "&";
            }
            window.location.href = url.replace(/[&?]$/, "");
        }
        $("#search_keyword").on("keypress", function (e) {
            if (e.keyCode === 13) {
                literatureSearchSubmit();
            }
        });
        function updateLiteratureSelectedCount() {
            var checked = $(".lit-pdf-check:checked");
            var count = checked.length;
            var paidCount = 0;
            var pointTotal = 0;
            var paidPoints = [];
            checked.each(function () {
                var points = parseInt($(this).attr("data-points") || "0", 10) || 0;
                var purchased = parseInt($(this).attr("data-purchased") || "0", 10) || 0;
                if (purchased === 0 && points > 0) {
                    paidCount++;
                    pointTotal += points;
                    paidPoints.push(points);
                }
            });
            $("#litSelectedCount").text("已选择 " + count + " 篇");
            var payMethod = $("#batchPayMethod").val() || "points";
            if (payMethod === "coupon") {
                var couponCount = <%=availableCouponCount %>;
                var useCoupon = Math.min(couponCount, paidCount);
                var lackPoints = 0;
                for (var i = useCoupon; i < paidPoints.length; i++) {
                    lackPoints += paidPoints[i];
                }
                var message = "预计消耗 " + useCoupon + " 张下载卡";
                if (lackPoints > 0) {
                    message += "，不足部分 " + lackPoints + " 积分";
                }
                message += "（可用 " + couponCount + " 张）";
                $("#litBatchCost").text(message);
            } else {
                $("#litBatchCost").text("预计消耗 " + pointTotal + " 积分");
            }
        }
        function toggleLiteratureSelection(source) {
            $(".lit-pdf-check").prop("checked", source.checked);
            updateLiteratureSelectedCount();
        }
        function submitBatchDownload() {
            if ($(".lit-pdf-check:checked").length === 0) {
                if (window.layer) {
                    layer.msg("请先选择要下载的 PDF", { icon: 0 });
                } else {
                    alert("请先选择要下载的 PDF");
                }
                return false;
            }
            return true;
        }
        $(document).on("change", ".lit-pdf-check", updateLiteratureSelectedCount);
    </script>
</body>
</html>
