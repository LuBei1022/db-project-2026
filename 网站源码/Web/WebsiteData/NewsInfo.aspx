<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewsInfo.aspx.cs" Inherits="Web.WebsiteData.NewsInfo" %>

<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<!DOCTYPE html>
<html lang="zh-CN">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta name='robots' content='index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1' />
    <title><%=CommonFunc.GetTitle("", data_title) %></title>
    <meta name="description" content="<%=CommonFunc.GetDescription(data_list.description) %>" />
    <LiteratureManager:css ID="css" runat="server" />
    <style>
        body.ac .middle {
            padding: 128px 32px 170px !important;
            min-height: 100vh;
            box-sizing: border-box;
        }
        .zixun-xq {
            padding-top: 36px !important;
        }
        .zixun-xq .w1200 {
            width: 100%;
            max-width: 1280px;
            margin: 0 auto !important;
            display: grid !important;
            grid-template-columns: minmax(0, 1fr) 320px;
            gap: 24px;
            align-items: stretch;
        }
        .zixun-l,
        .zixun-r {
            width: auto !important;
            border-radius: 28px !important;
            background: rgba(255,255,255,.42) !important;
            border: 1px solid rgba(255,255,255,.34) !important;
            box-shadow: 0 18px 48px rgba(31,50,68,.08), inset 0 1px 0 rgba(255,255,255,.72) !important;
            backdrop-filter: blur(16px);
            -webkit-backdrop-filter: blur(16px);
        }
        .zixun-l[style] {
            width: auto !important;
        }
        .zixun-xq-tt {
            display: block !important;
            padding: 46px 54px 28px !important;
            border-bottom: 1px solid rgba(229,231,235,.75);
        }
        .zixun-xq-text {
            width: 100% !important;
        }
        .zixun-xq-text:before {
            content: "Academic Insights";
            display: inline-flex;
            margin-bottom: 18px;
            padding: 8px 14px;
            border-radius: 999px;
            background: rgba(255,255,255,.5);
            border: 1px solid rgba(255,255,255,.44);
            color: #6b7280;
            font-size: 11px;
            font-weight: 800;
            letter-spacing: .16em;
            text-transform: uppercase;
        }
        .zixun-xq-text h4 {
            max-width: 860px;
            margin: 0 0 18px !important;
            color: #111827 !important;
            font-size: clamp(36px, 5vw, 58px) !important;
            line-height: 1.12 !important;
            font-weight: 800 !important;
            letter-spacing: -0.05em !important;
        }
        .zixun-xq-text p {
            color: #9ca3af !important;
            font-size: 14px !important;
            font-weight: 700;
        }
        .zixun-xq-tt > a {
            display: inline-flex !important;
            align-items: center;
            justify-content: center;
            width: auto !important;
            min-width: 96px;
            height: 44px;
            margin-top: 26px;
            padding: 0 18px !important;
            border-radius: 999px !important;
            background: #007aff !important;
            color: #fff !important;
            box-shadow: 0 10px 15px -3px rgba(59,130,246,.2);
        }
        .zixun-xq-con {
            padding: 38px 54px !important;
            color: #374151;
            font-size: 16px;
            line-height: 1.95;
        }
        .zixun-xq-con p {
            color: #374151 !important;
            font-size: 16px !important;
            line-height: 1.95 !important;
        }
        .zixun-xq-con img {
            max-width: 100%;
            height: auto;
            border-radius: 22px;
            box-shadow: 0 18px 48px rgba(31,50,68,.1);
        }
        .zixun-xq-but {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 16px;
            padding: 26px 54px 46px !important;
            border-top: 1px solid rgba(229,231,235,.75) !important;
        }
        .zixun-xq-but div {
            min-height: 72px;
            padding: 16px 18px;
            border-radius: 18px;
            background: rgba(255,255,255,.38);
            border: 1px solid rgba(255,255,255,.34);
            color: #9ca3af !important;
            font-size: 12px !important;
            line-height: 1.5 !important;
        }
        .zixun-xq-but div a {
            display: block !important;
            margin: 8px 0 0 !important;
            color: #111827 !important;
            font-size: 14px;
            line-height: 1.55;
            font-weight: 700;
        }
        .zixun-r {
            padding: 24px;
            position: sticky;
            top: 112px;
        }
        .zixun-r h4 {
            margin: 0 0 18px;
            color: #111827 !important;
            font-size: 12px !important;
            font-weight: 800;
            letter-spacing: .18em;
            text-transform: uppercase;
        }
        .zixun-r .zixunList {
            display: grid !important;
            gap: 16px;
            margin-top: 0 !important;
        }
        .zixun-r .zixunList a {
            overflow: hidden;
            border-radius: 18px !important;
            background: rgba(255,255,255,.38) !important;
            border: 1px solid rgba(255,255,255,.34) !important;
        }
        .zixun-r .zixun-img {
            border-radius: 14px !important;
            overflow: hidden;
        }
        .zixun-r .zixun-img img {
            height: auto !important;
            aspect-ratio: 16 / 9;
            border-radius: 14px !important;
        }
        .zixun-r .zixunList h4 {
            padding: 0 14px !important;
            margin: 12px 0 6px !important;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
            color: #111827 !important;
            font-size: 14px !important;
            line-height: 1.45 !important;
            letter-spacing: 0;
            text-transform: none;
        }
        .zixun-r .zixunList p {
            padding: 0 14px 14px !important;
            color: #9ca3af !important;
            font-size: 12px !important;
        }
        @media (max-width: 1024px) {
            .zixun-xq .w1200 {
                grid-template-columns: 1fr;
            }
            .zixun-r {
                position: static;
            }
        }
        @media (max-width: 760px) {
            body.ac .middle {
                padding: 104px 16px 220px !important;
            }
            .zixun-xq-tt,
            .zixun-xq-con,
            .zixun-xq-but {
                padding-left: 24px !important;
                padding-right: 24px !important;
            }
            .zixun-xq-but {
                grid-template-columns: 1fr;
            }
        }
        /* Apple skill refinement for article reading surface */
        body.apple-content-page {
            font-family: "SF Pro Text", system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", "Microsoft YaHei", sans-serif !important;
            color: #1d1d1f !important;
            background: #f5f5f7 !important;
        }
        body.apple-content-page:before,
        body.apple-content-page:after {
            display: none !important;
        }
        body.apple-content-page .middle {
            padding: 128px 24px 150px !important;
            background: #f5f5f7 !important;
        }
        body.apple-content-page .zixun-xq {
            padding-top: 48px !important;
        }
        body.apple-content-page .zixun-xq .w1200 {
            max-width: 1180px;
            grid-template-columns: minmax(0, 1fr) 300px;
            align-items: stretch !important;
        }
        body.apple-content-page .zixun-l,
        body.apple-content-page .zixun-r {
            border-radius: 18px !important;
            background: #fff !important;
            border: 1px solid #e0e0e0 !important;
            box-shadow: none !important;
            backdrop-filter: none !important;
            -webkit-backdrop-filter: none !important;
        }
        body.apple-content-page .zixun-xq-tt {
            padding: 64px 80px 32px !important;
        }
        body.apple-content-page .zixun-xq-text:before {
            background: #fafafc;
            border: 1px solid #f0f0f0;
            color: #7a7a7a;
            box-shadow: none;
        }
        body.apple-content-page .zixun-xq-text h4 {
            color: #1d1d1f !important;
            font-family: "SF Pro Display", system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif !important;
            font-size: clamp(40px, 5vw, 56px) !important;
            font-weight: 600 !important;
            line-height: 1.07 !important;
            letter-spacing: -0.28px !important;
        }
        body.apple-content-page .zixun-xq-text p {
            color: #7a7a7a !important;
            font-size: 14px !important;
            font-weight: 400 !important;
            line-height: 1.43 !important;
            letter-spacing: -0.224px !important;
        }
        body.apple-content-page .zixun-xq-tt > a {
            height: 44px;
            border-radius: 9999px !important;
            background: #0066cc !important;
            color: #fff !important;
            box-shadow: none !important;
            font-size: 17px;
            font-weight: 400;
        }
        body.apple-content-page .zixun-xq-tt > a:active,
        body.apple-content-page .zixun-xq-but div a:active,
        body.apple-content-page .zixun-r .zixunList a:active {
            transform: scale(.95);
        }
        body.apple-content-page .zixun-xq-con {
            max-width: 820px;
            padding: 44px 80px !important;
            color: #1d1d1f;
            font-size: 17px;
            font-weight: 400;
            line-height: 1.47;
            letter-spacing: -0.374px;
        }
        body.apple-content-page .zixun-xq-con p {
            color: #1d1d1f !important;
            font-size: 17px !important;
            font-weight: 400 !important;
            line-height: 1.47 !important;
            letter-spacing: -0.374px !important;
            margin-bottom: 24px !important;
        }
        body.apple-content-page .consult-enhance {
            max-width: 820px;
            margin: 0 80px 48px;
            padding: 28px;
            border: 1px solid #e0e0e0;
            border-radius: 18px;
            background: #fafafc;
        }
        body.apple-content-page .consult-enhance-head span {
            display: inline-flex;
            margin-bottom: 12px;
            color: #0066cc;
            font-size: 13px;
            font-weight: 600;
            line-height: 1.3;
        }
        body.apple-content-page .consult-enhance h5 {
            margin: 0 0 10px;
            color: #1d1d1f;
            font-family: "SF Pro Display", "PingFang SC", "Microsoft YaHei", system-ui, -apple-system, BlinkMacSystemFont, sans-serif;
            font-size: 24px;
            font-weight: 600;
            line-height: 1.25;
            letter-spacing: 0;
        }
        body.apple-content-page .consult-enhance p {
            margin: 0;
            color: #424245;
            font-size: 15px;
            line-height: 1.8;
            letter-spacing: 0;
        }
        body.apple-content-page .consult-block {
            margin-top: 24px;
            padding-top: 22px;
            border-top: 1px solid #e5e5e5;
        }
        body.apple-content-page .consult-block h6 {
            margin: 0 0 12px;
            color: #1d1d1f;
            font-size: 17px;
            font-weight: 600;
            line-height: 1.35;
        }
        body.apple-content-page .consult-block ul {
            display: grid;
            gap: 10px;
            margin: 0;
            padding: 0;
            list-style: none;
        }
        body.apple-content-page .consult-block li {
            position: relative;
            padding-left: 18px;
            color: #424245;
            font-size: 15px;
            line-height: 1.72;
        }
        body.apple-content-page .consult-block li:before {
            content: "";
            position: absolute;
            left: 0;
            top: .76em;
            width: 6px;
            height: 6px;
            border-radius: 50%;
            background: #0066cc;
        }
        body.apple-content-page .consult-note {
            margin-top: 24px;
            padding: 18px 20px;
            border-radius: 14px;
            background: #fff;
            border: 1px solid #f0f0f0;
        }
        body.apple-content-page .consult-note strong {
            display: block;
            margin-bottom: 8px;
            color: #1d1d1f;
            font-size: 15px;
            font-weight: 600;
        }
        body.apple-content-page .consult-note p {
            color: #6e6e73;
            font-size: 14px;
            line-height: 1.7;
        }
        body.apple-content-page .zixun-xq-con img {
            border-radius: 8px;
            box-shadow: rgba(0, 0, 0, .22) 3px 5px 30px 0;
        }
        body.apple-content-page .zixun-xq-but {
            padding: 24px 80px 56px !important;
        }
        body.apple-content-page .zixun-xq-but div {
            background: #fafafc;
            border: 1px solid #f0f0f0;
            border-radius: 18px;
            box-shadow: none;
            color: #7a7a7a !important;
        }
        body.apple-content-page .zixun-xq-but div a {
            color: #0066cc !important;
            font-size: 17px;
            font-weight: 400;
            line-height: 1.47;
        }
        body.apple-content-page .zixun-r {
            padding: 24px;
        }
        body.apple-content-page .zixun-r h4 {
            color: #1d1d1f !important;
            font-size: 21px !important;
            font-weight: 600;
            line-height: 1.19;
            letter-spacing: .231px;
            text-transform: none;
        }
        body.apple-content-page .zixun-r .zixunList a {
            border-radius: 18px !important;
            background: #fafafc !important;
            border: 1px solid #f0f0f0 !important;
            box-shadow: none !important;
        }
        body.apple-content-page .zixun-r .zixun-img,
        body.apple-content-page .zixun-r .zixun-img img {
            border-radius: 8px !important;
        }
        body.apple-content-page .zixun-r .zixunList h4 {
            color: #1d1d1f !important;
            font-size: 17px !important;
            font-weight: 600 !important;
            line-height: 1.24 !important;
            letter-spacing: -0.374px !important;
        }
        body.apple-content-page .zixun-r .zixunList p {
            color: #7a7a7a !important;
            font-size: 14px !important;
            line-height: 1.43 !important;
            letter-spacing: -0.224px !important;
        }
        @media (max-width: 1024px) {
            body.apple-content-page .zixun-xq .w1200 {
                grid-template-columns: 1fr;
            }
        }
        @media (max-width: 760px) {
            body.apple-content-page .middle {
                padding: 104px 16px 220px !important;
            }
            body.apple-content-page .zixun-xq-tt,
            body.apple-content-page .zixun-xq-con,
            body.apple-content-page .zixun-xq-but {
                padding-left: 24px !important;
                padding-right: 24px !important;
            }
            body.apple-content-page .consult-enhance {
                margin: 0 24px 36px;
                padding: 22px;
            }
            body.apple-content-page .consult-enhance h5 {
                font-size: 21px;
            }
        }
        /* Final DESIGN.md pass: related news is a compact sidebar list, not a multi-column card grid */
        body.apple-content-page .zixun-r {
            position: sticky !important;
            top: 112px !important;
            align-self: stretch !important;
            padding: 28px !important;
            border-radius: 18px !important;
            border: 1px solid #e0e0e0 !important;
            background: #fff !important;
            box-shadow: none !important;
        }
        body.apple-content-page .zixun-r > h4 {
            margin: 0 0 20px !important;
            color: #1d1d1f !important;
            font-family: "SF Pro Display", "PingFang SC", "Microsoft YaHei", system-ui, -apple-system, BlinkMacSystemFont, sans-serif !important;
            font-size: 21px !important;
            font-weight: 600 !important;
            line-height: 1.19 !important;
            letter-spacing: .231px !important;
        }
        body.apple-content-page .zixun-r .zixunList {
            display: flex !important;
            flex-direction: column !important;
            gap: 12px !important;
            margin: 0 !important;
            padding: 0 !important;
        }
        body.apple-content-page .zixun-r .zixunList a {
            display: grid !important;
            grid-template-columns: 72px minmax(0, 1fr) !important;
            grid-template-rows: auto auto !important;
            column-gap: 14px !important;
            row-gap: 6px !important;
            align-items: center !important;
            min-height: 88px !important;
            padding: 12px !important;
            border-radius: 18px !important;
            border: 1px solid #f0f0f0 !important;
            background: #fafafc !important;
            box-shadow: none !important;
            transition: background .18s ease, border-color .18s ease, transform .18s ease !important;
        }
        body.apple-content-page .zixun-r .zixunList a:hover {
            transform: translateY(-1px) !important;
            border-color: #e0e0e0 !important;
            background: #fff !important;
        }
        body.apple-content-page .zixun-r .zixun-img {
            grid-column: 1 !important;
            grid-row: 1 / 3 !important;
            width: 72px !important;
            height: 72px !important;
            margin: 0 !important;
            border-radius: 11px !important;
            background: #fff !important;
            overflow: hidden !important;
        }
        body.apple-content-page .zixun-r .zixun-img img {
            width: 100% !important;
            height: 100% !important;
            aspect-ratio: auto !important;
            object-fit: cover !important;
            border-radius: 11px !important;
            box-shadow: none !important;
        }
        body.apple-content-page .zixun-r .zixunList a h4 {
            grid-column: 2 !important;
            grid-row: 1 !important;
            display: -webkit-box !important;
            -webkit-line-clamp: 2 !important;
            -webkit-box-orient: vertical !important;
            overflow: hidden !important;
            margin: 0 !important;
            padding: 0 !important;
            color: #1d1d1f !important;
            font-family: "SF Pro Text", "PingFang SC", "Microsoft YaHei", system-ui, -apple-system, BlinkMacSystemFont, sans-serif !important;
            font-size: 14px !important;
            font-weight: 600 !important;
            line-height: 1.29 !important;
            letter-spacing: -0.224px !important;
            text-transform: none !important;
        }
        body.apple-content-page .zixun-r .zixunList a p {
            grid-column: 2 !important;
            grid-row: 2 !important;
            margin: 0 !important;
            padding: 0 !important;
            color: #7a7a7a !important;
            font-family: "SF Pro Text", "PingFang SC", "Microsoft YaHei", system-ui, -apple-system, BlinkMacSystemFont, sans-serif !important;
            font-size: 12px !important;
            font-weight: 400 !important;
            line-height: 1.3 !important;
            letter-spacing: -0.12px !important;
        }
        @media (max-width: 1024px) {
            body.apple-content-page .zixun-r {
                position: static !important;
            }
            body.apple-content-page .zixun-r .zixunList {
                display: grid !important;
                grid-template-columns: repeat(2, minmax(0, 1fr)) !important;
            }
        }
        @media (max-width: 620px) {
            body.apple-content-page .zixun-r .zixunList {
                grid-template-columns: 1fr !important;
            }
        }
    </style>
</head>

<body class="ac apple-content-page">
    <LiteratureManager:top ID="top" runat="server" />
    <div class="middle">

        <section class="zixun-xq">
            <div class="w1200">
                <div class="zixun-l" style="<%=(istop?"":" width:100%;")%>">
                    <div class="zixun-xq-tt">
                        <div class="zixun-xq-text">
                            <h4><%=Function.HtmlDiscode(data_list.name) %></h4>
                            <p><%=data_list.datetime.ToString("yyyy-MM-dd") %></p>
                        </div>
                        <a href="<%= Function.HtmlDiscode(CommonFunc.GetTopHtmlHref(data_list.tbclass_id.ToString(), "0")) %>">返回</a>
                    </div>
                    <div class="zixun-xq-con">
                        <%=Function.Replace_Content(data_list.info_) %>
                    </div>
                    <div class="zixun-xq-but">
                        <div>上一篇 <a <%=!string.IsNullOrWhiteSpace(prev_href)?"href=\""+prev_href.Replace("/Website/Info_", "/WebsiteData/NewsInfo.aspx?id=")+"\"":"" %>><%=prev_name %></a></div>
                        <div>下一篇 <a <%=!string.IsNullOrWhiteSpace(nex_href)?"href=\""+nex_href.Replace("/Website/Info_", "/WebsiteData/NewsInfo.aspx?id=")+"\"":"" %>><%=nex_name %></a></div>
                    </div>
                </div>
                <%if (istop)
                    {  %>
                <div class="zixun-r">
                    <h4>相关资讯</h4>
                    <div class="zixunList">
                         <asp:Repeater ID="TopNewsList" runat="server">
                <ItemTemplate>
                        <a href="/WebsiteData/NewsInfo.aspx?id=<%#Eval("id").ToString() %>">
                            <div class="zixun-img">
                                <img src="<%#CommonFunc.GetWebUpload_Pic(Eval("upload_pic_img").ToString(), "/images/null.png") %>" /></div>
                            <h4><%#Function.HtmlDiscode(Eval("name").ToString()) %></h4>
                                <p><%#Function.ConvertTo<DateTime>(Eval("datetime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd") %></p>
                        </a>
                      </ItemTemplate>
            </asp:Repeater>
                    </div>
                </div>
                <%} %>
            </div>
        </section>
    </div>
    <LiteratureManager:foot ID="foot" runat="server" />
</body>

</html>
