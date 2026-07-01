<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_LiteratureList.aspx.cs" Inherits="Web.admin.Admin_LiteratureList" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <style type="text/css">
        .lit-page-title { display: flex; align-items: flex-end; justify-content: space-between; gap: 18px; margin-bottom: 18px; }
        .lit-page-title h1 { margin: 0; font-size: 30px; font-weight: 800; color: #1d1d1f; }
        .lit-page-title p { margin: 6px 0 0; color: #6e6e73; }
        .lit-monitor { margin-bottom: 18px; padding: 20px; border: 1px solid #e5e5ea; border-radius: 18px; background: #fff; }
        .lit-monitor-head { display: flex; align-items: center; justify-content: space-between; gap: 18px; margin-bottom: 16px; }
        .lit-monitor-head strong { display: block; color: #1d1d1f; font-size: 20px; }
        .lit-monitor-head span { color: #6e6e73; }
        .lit-monitor-head a { color: #0066cc; }
        .lit-monitor-stats { display: grid; grid-template-columns: repeat(4, minmax(0, 1fr)); gap: 12px; margin-bottom: 18px; }
        .lit-monitor-stats div { padding: 16px; border: 1px solid #e5e5ea; border-radius: 14px; background: #fbfbfd; }
        .lit-monitor-stats span { display: block; color: #86868b; font-size: 13px; }
        .lit-monitor-stats strong { display: block; margin-top: 8px; color: #1d1d1f; font-size: 26px; }
        .lit-tag-summary { padding-top: 2px; }
        .lit-tag-summary > strong { display: block; margin-bottom: 10px; color: #1d1d1f; }
        .lit-tag-summary div { display: flex; flex-wrap: wrap; gap: 8px; }
        .lit-tag-summary span { display: inline-flex; align-items: center; gap: 6px; min-height: 30px; padding: 0 10px; border-radius: 999px; background: #f5f5f7; color: #1d1d1f; }
        .lit-tag-summary em { color: #0066cc; font-style: normal; font-weight: 700; }
        .lit-select-cell { text-align: center; width: 42px; }
        .lit-select-cell input { width: 16px; height: 16px; }
        .lit-row-stats { display: inline-flex; flex-direction: column; align-items: stretch; gap: 4px; }
        .lit-row-stats span, .lit-row-stats a { display: inline-flex; justify-content: space-between; gap: 8px; min-width: 72px; padding: 2px 7px; border-radius: 6px; background: #f5f7fa; color: #1d1d1f; font-size: 12px; line-height: 1.45; text-decoration: none; }
        .lit-row-stats a:hover { background: #e8eef6; color: #10243a; text-decoration: none; }
        .lit-admin-table { table-layout: fixed; margin-bottom: 0; }
        .lit-admin-table th { white-space: nowrap; vertical-align: middle; }
        .lit-admin-table td { vertical-align: top; padding: 14px 12px; }
        .cardHeader .btn.lit-toolbar-btn, .cardHeader button.lit-toolbar-btn { background: #eef4fb !important; border: 1px solid #c8d6e5 !important; color: #1f344d !important; box-shadow: none; }
        .cardHeader .btn.lit-toolbar-btn:hover, .cardHeader .btn.lit-toolbar-btn:focus, .cardHeader button.lit-toolbar-btn:hover, .cardHeader button.lit-toolbar-btn:focus { background: #dfeaf6 !important; border-color: #adc0d4 !important; color: #10243a !important; }
        .lit-col-check { width: 44px; }
        .lit-col-no { width: 52px; }
        .lit-col-cover { width: 58px; }
        .lit-col-main { width: 42%; }
        .lit-col-category { width: 120px; }
        .lit-col-year { width: 72px; }
        .lit-col-type { width: 86px; }
        .lit-col-source { width: 92px; }
        .lit-col-status { width: 86px; }
        .lit-col-stats { width: 150px; }
        .lit-col-time { width: 132px; }
        .lit-col-actions { width: 150px; }
        .lit-cover-thumb { width: 34px; height: 44px; object-fit: cover; border: 1px solid #d2d2d7; border-radius: 4px; background: #f5f5f7; }
        .lit-main-title { color: #111827; font-weight: 700; line-height: 1.55; word-break: break-word; }
        .lit-main-authors { margin-top: 6px; color: #4b5563; font-size: 13px; line-height: 1.6; word-break: break-word; }
        .lit-main-mobile-meta { display: none; margin-top: 8px; color: #6b7280; font-size: 12px; }
        .lit-status-pill { display: inline-flex; align-items: center; min-height: 26px; padding: 0 9px; border-radius: 999px; background: #e8f5ee; color: #168449; font-size: 12px; white-space: nowrap; }
        .lit-source-pill { display: inline-flex; align-items: center; min-height: 26px; padding: 0 9px; border-radius: 999px; background: #eef4fb; color: #1f344d; font-size: 12px; white-space: nowrap; }
        .lit-source-user { background: #fff4e5; color: #9a5b00; }
        .lit-source-import { background: #e8f5ee; color: #168449; }
        .lit-source-admin { background: #eef4fb; color: #1f344d; }
        .lit-row-stats { gap: 4px; align-items: stretch; }
        .lit-actions-inline { display: flex; flex-wrap: wrap; gap: 7px; justify-content: flex-start; }
        .lit-actions-inline a.lit-action-btn { display: inline-flex; align-items: center; min-height: 26px; padding: 0 10px; border-radius: 8px; background: #eef4fb !important; border: 1px solid #c8d6e5 !important; color: #1f344d !important; font-weight: 500; line-height: 1; }
        .lit-actions-inline a.lit-action-btn:hover { background: #dfeaf6 !important; color: #10243a !important; text-decoration: none; }
        .lit-time { color: #1f2937; line-height: 1.6; white-space: nowrap; }
        @media (max-width: 1200px) {
            .lit-monitor-stats { grid-template-columns: repeat(2, minmax(0, 1fr)); }
            .lit-col-category, .lit-col-year, .lit-col-type, .lit-col-source, .lit-col-status { display: none; }
            .lit-main-mobile-meta { display: block; }
            .lit-col-main { width: 50%; }
        }
    </style>
</head>
<body>
    <%@ Register TagPrefix="LiteratureManager" TagName="Inc" Src="Inc.ascx" %>
    <%@ Register TagPrefix="LiteratureManager" TagName="class_menu" Src="class_menu.ascx" %>
    <% if (isLoading) { %>
    <LiteratureManager:Inc ID="Inc2" runat="server" />
    <LiteratureManager:class_menu ID="class_menu" runat="server" />

    <form id="form2" runat="server">
        <div class="app-content">
            <asp:Panel ID="Main" runat="server">
                <div class="container-fluid">
                    <div class="lit-page-title">
                        <div>
                            <h1><%=ListTitle %></h1>
                            <p><%=ListSubtitle %></p>
                        </div>
                    </div>
                    <%=ApprovedDashboardHtml %>
                    <div class="col-md-12">
                        <div class="card mb-12">
                            <div class="card-header cardList">
                                <div class="cardItem">
                                    <label class="col-form-label">&#20851;&#38190;&#35789;</label>
                                    <div class="col-form-input">
                                        <asp:TextBox ID="SearchKeyWords" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="cardItem">
                                    <label class="col-form-label">&#20998;&#31867;</label>
                                    <div class="col-form-input">
                                        <asp:DropDownList ID="SearchCategoryId" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="cardItem">
                                    <label class="col-form-label">&#26631;&#31614;</label>
                                    <div class="col-form-input">
                                        <asp:DropDownList ID="SearchTagId" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="cardItem">
                                    <asp:Button ID="Button2" runat="server" OnClick="OnClick_Search" Text="&#25628;&#32034;" CssClass="btn lit-toolbar-btn" />
                                </div>
                                <div class="cardItem" style="float: right;">
                                    <button type="button" class="btn lit-toolbar-btn" style="margin-right: 10px;" onclick="selectAllAdminLiterature()">&#20840;&#36873;</button>
                                </div>
                                <div class="cardItem" style="float: right;">
                                    <asp:Button ID="BatchPdfExportButton" runat="server" OnClick="OnClick_BatchPdfExport" Text="&#25209;&#37327;&#23548;&#20986;PDF" CssClass="btn lit-toolbar-btn" style="margin-right: 10px;" OnClientClick="return validateLiteratureBatchExport();" />
                                </div>
                                <div class="cardItem" style="float: right;">
                                    <asp:Button ID="BatchExportButton" runat="server" OnClick="OnClick_BatchExport" Text="&#25209;&#37327;&#23548;&#20986;CSV" CssClass="btn lit-toolbar-btn" style="margin-right: 10px;" OnClientClick="return validateLiteratureBatchExport();" />
                                </div>
                            </div>
                            <div class="card-body p-0">
                                <table class="table table-sm lit-admin-table">
                                    <colgroup>
                                        <col class="lit-col-check" />
                                        <col class="lit-col-no" />
                                        <col class="lit-col-cover" />
                                        <col class="lit-col-main" />
                                        <col class="lit-col-category" />
                                        <col class="lit-col-year" />
                                        <col class="lit-col-type" />
                                        <col class="lit-col-source" />
                                        <col class="lit-col-status" />
                                        <col class="lit-col-stats" />
                                        <col class="lit-col-time" />
                                        <col class="lit-col-actions" />
                                    </colgroup>
                                    <thead>
                                        <tr>
                                            <th class="lit-select-cell"><input type="checkbox" id="litSelectAll" onclick="toggleAdminLiteratureSelection(this)" /></th>
                                            <th>&#24207;&#21495;</th>
                                            <th>&#23553;&#38754;</th>
                                            <th>&#25991;&#29486;&#20449;&#24687;</th>
                                            <th>&#20998;&#31867;</th>
                                            <th>&#24180;&#20221;</th>
                                            <th>&#31867;&#22411;</th>
                                            <th>&#26469;&#28304;</th>
                                            <th>&#23457;&#26680;&#29366;&#24577;</th>
                                            <th>&#20114;&#21160;</th>
                                            <th>&#19978;&#20256;&#26102;&#38388;</th>
                                            <th class="textAlignC">&#25805;&#20316;</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="Repeater1" runat="server">
                                            <ItemTemplate>
                                                <tr class="hover">
                                                    <td class="lit-select-cell"><input type="checkbox" name="lit_ids" value="<%# Eval("id") %>" /></td>
                                                    <td><%# Eval("xuhao") %></td>
                                                    <td><img class="lit-cover-thumb" src="<%# GetCoverUrl(Eval("cover_pic")) %>" /></td>
                                                    <td>
                                                        <div class="lit-main-title"><%# Function.HtmlDiscodeWeb(Eval("title").ToString()) %></div>
                                                        <div class="lit-main-authors"><%# Function.HtmlDiscodeWeb(Eval("author_names").ToString()) %></div>
                                                        <div class="lit-main-mobile-meta"><%# GetCategoryName(Eval("category_id")) %> · <%# Eval("publish_year") %> · <%# Function.HtmlDiscodeWeb(Eval("source_type").ToString()) %> · <%# GetSourceText(Eval("userid"), Eval("import_batch_id")) %> · <%# GetStatusText(Eval("status")) %></div>
                                                    </td>
                                                    <td><%# GetCategoryName(Eval("category_id")) %></td>
                                                <td><%# Eval("publish_year") %></td>
                                                <td><%# Function.HtmlDiscodeWeb(Eval("source_type").ToString()) %></td>
                                                <td><%# GetSourceBadgeHtml(Eval("userid"), Eval("import_batch_id")) %></td>
                                                <td><span class="lit-status-pill"><%# GetStatusText(Eval("status")) %></span></td>
                                                <td><%# GetInteractionStatsHtml(Eval("id"), Eval("like_count"), Eval("favorite_count"), Eval("comment_count")) %></td>
                                                    <td><div class="lit-time"><%# Function.ConvertTo<DateTime>(Eval("addtime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd") %><br /><%# Function.ConvertTo<DateTime>(Eval("addtime").ToString(),DateTime.MinValue).ToString("HH:mm:ss") %></div></td>
                                                    <td class="textAlignC">
                                                        <div class="lit-actions-inline"><%# GetOperationHtml(Eval("id"), Eval("remark"), Eval("canonical_literature_id"), Eval("status")) %></div>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <asp:Panel ID="DivNull" runat="server" Visible="true">
                                            <tr>
                                                <td colspan="12" style="text-align: center;">&#26080;&#30456;&#20851;&#25968;&#25454;!</td>
                                            </tr>
                                        </asp:Panel>
                                    </tbody>
                                </table>
                                <div class="msdn">
                                    <div></div>
                                    <Webdiyer:AspNetPager ID="AspNetPager1" runat="server" CurrentPageButtonClass="current" FirstPageText="Home" PrevPageText="Prev" NextPageText="Next" LastPageText="End"
                                        ShowDisabledButtons="true" OnPageChanged="AspNetPager1_PageChanged" UrlPaging="true" PageIndexBoxClass="input_page" PageIndexBoxType="TextBox" SubmitButtonClass="go" SubmitButtonText="GO" ShowPageIndexBox="Always">
                                    </Webdiyer:AspNetPager>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </form>
    <script type="text/javascript">
        function toggleAdminLiteratureSelection(source) {
            var items = document.getElementsByName("lit_ids");
            for (var i = 0; i < items.length; i++) {
                items[i].checked = source.checked;
            }
        }
        function selectAllAdminLiterature() {
            var items = document.getElementsByName("lit_ids");
            var shouldCheck = false;
            for (var i = 0; i < items.length; i++) {
                if (!items[i].checked) {
                    shouldCheck = true;
                    break;
                }
            }
            var source = document.getElementById("litSelectAll");
            if (source) {
                source.checked = shouldCheck;
            }
            for (var j = 0; j < items.length; j++) {
                items[j].checked = shouldCheck;
            }
        }
        function validateLiteratureBatchExport() {
            var items = document.getElementsByName("lit_ids");
            for (var i = 0; i < items.length; i++) {
                if (items[i].checked) {
                    return true;
                }
            }
            alert("&#35831;&#20808;&#36873;&#25321;&#35201;&#23548;&#20986;&#30340;&#25991;&#29486;");
            return false;
        }
    </script>
    <% } %>
</body>
</html>
