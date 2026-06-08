<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LiteratureVenue.aspx.cs" Inherits="Web.LiteratureVenue" %>

<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <title>&#25991;&#29486;/&#26399;&#21002;&#27719;&#24635;</title>
    <LiteratureManager:css ID="css" runat="server" />
    <style>
        .venue-wrap { max-width: 1280px; margin: 0 auto; padding: 30px 20px 60px; }
        .venue-hero { border: 1px solid #dbe7f4; border-radius: 24px; background: linear-gradient(135deg, #eef6ff 0%, #fbfdff 100%); padding: 30px; margin-bottom: 22px; }
        .venue-hero h1 { margin: 0 0 10px; color: #16324f; font-size: 32px; }
        .venue-hero p { margin: 0; color: #5b6b7d; line-height: 1.8; }
        .venue-tabs { display: flex; gap: 10px; flex-wrap: wrap; margin: 18px 0 0; }
        .venue-tabs a { display: inline-flex; align-items: center; min-height: 38px; padding: 0 16px; border-radius: 999px; border: 1px solid #d9e8fb; background: #fff; color: #1d6fdc; }
        .venue-tabs a.current, .venue-tabs a:hover { background: #1d6fdc; color: #fff; }
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
        @media (max-width: 960px) {
            .venue-grid { grid-template-columns: 1fr; }
            .venue-list { max-height: none; }
            .venue-info-grid { grid-template-columns: repeat(2, minmax(0, 1fr)); }
        }
        @media (max-width: 640px) {
            .venue-info-grid { grid-template-columns: 1fr; }
        }
    </style>
</head>
<body class="ac" style="background: #f6f8fb;">
    <LiteratureManager:top ID="top" runat="server" />
    <div class="middle">
        <div class="venue-wrap">
            <div class="venue-hero">
                <h1>&#25991;&#29486;/&#26399;&#21002;&#27719;&#24635;</h1>
                <p>&#24179;&#21488;&#20250;&#33258;&#21160;&#27719;&#24635;&#24050;&#20844;&#24320;&#25991;&#29486;&#30340;&#26399;&#21002;&#21644;&#20250;&#35758;&#20449;&#24687;&#65292;&#24182;&#23558;&#23545;&#24212;&#25991;&#29486;&#24402;&#20837;&#21508;&#33258;&#30340;&#26469;&#28304;&#20998;&#32452;&#12290;</p>
                <div class="venue-tabs">
                    <a href="/LiteratureVenue.aspx" class="<%=GetTypeClass("all") %>">&#20840;&#37096;</a>
                    <a href="/LiteratureVenue.aspx?type=journal" class="<%=GetTypeClass("journal") %>">&#26399;&#21002;</a>
                    <a href="/LiteratureVenue.aspx?type=conference" class="<%=GetTypeClass("conference") %>">&#20250;&#35758;</a>
                    <a href="/LiteratureSearch.aspx">&#25991;&#29486;&#26816;&#32034;</a>
                </div>
            </div>

            <div class="venue-grid">
                <div class="venue-panel">
                    <div class="venue-panel-head"><%=VenueListTitleHtml %></div>
                    <div class="venue-list">
                        <%=VenueListHtml %>
                    </div>
                </div>
                <div class="venue-panel venue-main">
                    <div class="venue-summary">
                        <h2><%=SelectedVenueTitleHtml %></h2>
                        <p><%=SelectedVenueSummaryHtml %></p>
                    </div>
                    <%=VenueInfoHtml %>
                    <div class="venue-items">
                        <%=LiteratureListHtml %>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <LiteratureManager:foot ID="foot" runat="server" />
</body>
</html>
