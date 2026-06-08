<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="News.aspx.cs" Inherits="Web.Website.News" %>

<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<!DOCTYPE html>
<html lang="zh-CN">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta name='robots' content='index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1' />
    <title><%=CommonFunc.GetTitle("", tbclass_title) %></title>
    <meta name="description" content="<%=CommonFunc.GetDescription(tbl_class.description) %>" />
    <LiteratureManager:css ID="css" runat="server" />
    <style>
        body.ac .middle {
            padding: 128px 32px 170px !important;
            min-height: 100vh;
            box-sizing: border-box;
        }
        .zixun {
            position: relative;
            width: 100% !important;
            height: auto !important;
            max-width: 896px;
            margin: 36px auto 72px;
            background-image: none !important;
            background: transparent !important;
            overflow: visible;
            text-align: center;
        }
        .zixun:before {
            content: "Academic Insights";
            display: inline-flex;
            align-items: center;
            justify-content: center;
            margin-bottom: 22px;
            padding: 9px 16px;
            border-radius: 999px;
            background: rgba(255,255,255,.46);
            border: 1px solid rgba(255,255,255,.42);
            box-shadow: inset 0 1px 0 rgba(255,255,255,.78), 0 12px 28px rgba(31,50,68,.06);
            color: #6b7280;
            font-size: 12px;
            font-weight: 700;
            letter-spacing: .16em;
            text-transform: uppercase;
            backdrop-filter: blur(16px);
            -webkit-backdrop-filter: blur(16px);
        }
        .zixun:after {
            content: "";
            position: absolute;
            left: 50%;
            top: 58%;
            z-index: -1;
            width: min(620px, 76vw);
            height: min(260px, 36vw);
            transform: translate(-50%, -50%);
            border-radius: 999px;
            background: radial-gradient(circle, rgba(147,197,253,.24) 0%, rgba(167,243,208,.18) 42%, transparent 72%);
            filter: blur(36px);
            pointer-events: none;
        }
        .zixun .w1920 {
            width: auto !important;
            max-width: none !important;
            height: auto !important;
            padding: 0 !important;
            display: block !important;
        }
        .zixun .w1920 h4 {
            max-width: 880px;
            margin: 0 auto;
            color: #111827 !important;
            font-size: clamp(48px, 7vw, 72px) !important;
            line-height: 1.08 !important;
            font-weight: 800 !important;
            letter-spacing: -0.055em !important;
        }
        .zixun-box {
            padding: 0 !important;
        }
        .zixun-box > .w1920 {
            width: 100%;
            max-width: 1280px !important;
            padding: 0 !important;
            margin: 0 auto !important;
        }
        .zixun-tt {
            display: flex;
            align-items: flex-end;
            justify-content: space-between;
            gap: 24px;
            padding: 0 !important;
            margin: 0 0 24px;
        }
        .zixun-tt h4 {
            position: relative;
            margin: 0;
            color: #111827;
            font-size: 12px;
            line-height: 1.2;
            font-weight: 800;
            letter-spacing: .18em;
            text-transform: uppercase;
        }
        .zixun-tt h4:after {
            content: "Browse the latest platform notes, research methods, and literature workflows.";
            display: block;
            margin-top: 10px;
            color: #6b7280;
            font-size: 15px;
            line-height: 1.7;
            font-weight: 400;
            letter-spacing: 0;
            text-transform: none;
        }
        .zixunList {
            margin-top: 0 !important;
            display: grid !important;
            grid-template-columns: repeat(4, minmax(0, 1fr)) !important;
            gap: 24px !important;
        }
        .zixunList .data-item-box {
            position: relative;
            min-height: 292px;
            display: flex !important;
            flex-direction: column;
            overflow: hidden;
            border-radius: 24px !important;
            border: 1px solid rgba(255,255,255,.34) !important;
            background: rgba(255,255,255,.42) !important;
            box-shadow: 0 18px 48px rgba(31,50,68,.08), inset 0 1px 0 rgba(255,255,255,.72) !important;
            backdrop-filter: blur(16px);
            -webkit-backdrop-filter: blur(16px);
            transition: transform .22s ease, box-shadow .22s ease, border-color .22s ease;
        }
        .zixunList .data-item-box:hover {
            transform: translateY(-6px);
            border-color: rgba(255,255,255,.72) !important;
            box-shadow: 0 28px 72px rgba(31,50,68,.14), inset 0 1px 0 rgba(255,255,255,.86) !important;
        }
        .zixun-img {
            margin: 16px 16px 0;
            padding: 0 !important;
            height: auto;
            overflow: hidden;
            border-radius: 18px !important;
            background: rgba(255,255,255,.32) !important;
            box-shadow: inset 0 1px 0 rgba(255,255,255,.65);
        }
        .zixun-img img {
            width: 100% !important;
            height: auto !important;
            aspect-ratio: 16 / 9;
            display: block;
            object-fit: cover;
            border-radius: 18px !important;
            transition: transform .45s ease, filter .45s ease;
        }
        .zixunList .data-item-box:hover .zixun-img img {
            transform: scale(1.04);
            filter: saturate(1.06);
        }
        .zixunList a h4 {
            flex: 1 1 auto;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
            margin: 20px 20px 10px !important;
            padding: 0 !important;
            color: #111827 !important;
            font-size: 18px !important;
            line-height: 1.35 !important;
            font-weight: 800 !important;
            letter-spacing: -0.02em;
        }
        .zixunList a p {
            margin: 0 20px 22px !important;
            padding: 0 !important;
            color: #9ca3af !important;
            font-size: 13px !important;
            line-height: 1.2 !important;
            font-weight: 600;
        }
        .zixunList a p:before {
            content: "Published ";
            color: #6b7280;
            font-weight: 500;
        }
        .web {
            margin: 34px 0 0 !important;
            text-align: center;
        }
        .web a,
        .web span,
        .web input,
        .web select {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 36px;
            height: 36px;
            margin: 0 4px;
            padding: 0 12px;
            border-radius: 999px !important;
            border: 1px solid rgba(255,255,255,.42) !important;
            background: rgba(255,255,255,.42) !important;
            color: #4b5563 !important;
            box-shadow: inset 0 1px 0 rgba(255,255,255,.7);
            font-size: 13px;
        }
        .web .current {
            background: #007aff !important;
            border-color: #007aff !important;
            color: #fff !important;
        }
        @media (max-width: 1200px) {
            .zixunList {
                grid-template-columns: repeat(3, minmax(0, 1fr)) !important;
            }
        }
        @media (max-width: 900px) {
            body.ac .middle {
                padding: 104px 16px 220px !important;
            }
            .zixun {
                margin: 30px auto 48px;
            }
            .zixun .w1920 h4 {
                font-size: 42px !important;
            }
            .zixunList {
                grid-template-columns: repeat(2, minmax(0, 1fr)) !important;
            }
        }
        @media (max-width: 620px) {
            .zixunList {
                grid-template-columns: 1fr !important;
            }
            .zixun-tt {
                display: block;
            }
        }
        /* Apple skill refinement: quiet parchment canvas, white utility cards, one blue accent */
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
            padding: 128px 0 150px !important;
            background: #f5f5f7 !important;
        }
        body.apple-content-page .zixun {
            max-width: 980px;
            margin: 52px auto 64px;
            padding: 0 24px;
        }
        body.apple-content-page .zixun:before,
        body.apple-content-page .zixun:after {
            display: none !important;
        }
        body.apple-content-page .zixun .w1920 h4 {
            max-width: 820px;
            color: #1d1d1f !important;
            font-family: "SF Pro Display", system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif !important;
            font-size: clamp(40px, 5vw, 56px) !important;
            font-weight: 600 !important;
            line-height: 1.07 !important;
            letter-spacing: -0.28px !important;
        }
        body.apple-content-page .zixun-box > .w1920 {
            max-width: 1440px !important;
            padding: 0 24px !important;
        }
        body.apple-content-page .zixun-tt {
            max-width: 980px;
            margin: 0 auto 32px;
            display: block;
            text-align: center;
        }
        body.apple-content-page .zixun-tt h4 {
            color: #1d1d1f;
            font-family: "SF Pro Display", system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
            font-size: 40px;
            font-weight: 600;
            line-height: 1.1;
            letter-spacing: 0;
            text-transform: none;
        }
        body.apple-content-page .zixun-tt h4:after {
            max-width: 640px;
            margin: 14px auto 0;
            color: #7a7a7a;
            font-size: 17px;
            font-weight: 400;
            line-height: 1.47;
            letter-spacing: -0.374px;
        }
        body.apple-content-page .zixunList {
            grid-template-columns: repeat(4, minmax(0, 1fr)) !important;
            gap: 24px !important;
        }
        body.apple-content-page .zixunList .data-item-box {
            min-height: 336px;
            border-radius: 18px !important;
            border: 1px solid #e0e0e0 !important;
            background: #fff !important;
            box-shadow: none !important;
            backdrop-filter: none !important;
            -webkit-backdrop-filter: none !important;
            transition: transform .18s ease, border-color .18s ease;
        }
        body.apple-content-page .zixunList .data-item-box:hover {
            transform: translateY(-2px);
            border-color: #c7c7cc !important;
            box-shadow: none !important;
        }
        body.apple-content-page .zixun-img {
            margin: 24px 24px 0;
            border-radius: 8px !important;
            background: #fafafc !important;
            box-shadow: none !important;
        }
        body.apple-content-page .zixun-img img {
            border-radius: 8px !important;
            aspect-ratio: 1 / 1;
            object-fit: cover;
        }
        body.apple-content-page .zixunList a h4 {
            margin: 20px 24px 8px !important;
            color: #1d1d1f !important;
            font-size: 17px !important;
            font-weight: 600 !important;
            line-height: 1.24 !important;
            letter-spacing: -0.374px !important;
        }
        body.apple-content-page .zixunList a p {
            margin: 0 24px 24px !important;
            color: #7a7a7a !important;
            font-size: 14px !important;
            font-weight: 400 !important;
            line-height: 1.43 !important;
            letter-spacing: -0.224px !important;
        }
        body.apple-content-page .zixunList a p:before {
            content: "";
        }
        body.apple-content-page .web a,
        body.apple-content-page .web span,
        body.apple-content-page .web input,
        body.apple-content-page .web select {
            border: 1px solid #e0e0e0 !important;
            background: #fff !important;
            color: #333 !important;
            box-shadow: none !important;
        }
        body.apple-content-page .web .current {
            background: #0066cc !important;
            border-color: #0066cc !important;
            color: #fff !important;
        }
        /* Final DESIGN.md typography pass for long Chinese page copy */
        body.apple-content-page .zixun {
            max-width: 1040px !important;
            margin: 42px auto 56px !important;
            padding: 0 24px !important;
        }
        body.apple-content-page .zixun .w1920 h4 {
            max-width: 960px !important;
            margin: 0 auto !important;
            color: #1d1d1f !important;
            font-family: "SF Pro Display", "PingFang SC", "Microsoft YaHei", system-ui, -apple-system, BlinkMacSystemFont, sans-serif !important;
            font-size: clamp(30px, 3.2vw, 40px) !important;
            font-weight: 600 !important;
            line-height: 1.18 !important;
            letter-spacing: 0 !important;
            text-align: center !important;
        }
        body.apple-content-page .zixun-tt {
            margin-bottom: 28px !important;
        }
        body.apple-content-page .zixun-tt h4 {
            color: #1d1d1f !important;
            font-family: "SF Pro Display", "PingFang SC", "Microsoft YaHei", system-ui, -apple-system, BlinkMacSystemFont, sans-serif !important;
            font-size: 34px !important;
            font-weight: 600 !important;
            line-height: 1.2 !important;
            letter-spacing: 0 !important;
        }
        body.apple-content-page .zixun-tt h4:after {
            max-width: 620px !important;
            margin-top: 12px !important;
            color: #7a7a7a !important;
            font-family: "SF Pro Text", "PingFang SC", "Microsoft YaHei", system-ui, -apple-system, BlinkMacSystemFont, sans-serif !important;
            font-size: 17px !important;
            font-weight: 400 !important;
            line-height: 1.47 !important;
            letter-spacing: -0.224px !important;
        }
        body.apple-content-page .zixunList a h4,
        body.apple-content-page .zixunList a p {
            font-family: "SF Pro Text", "PingFang SC", "Microsoft YaHei", system-ui, -apple-system, BlinkMacSystemFont, sans-serif !important;
        }
        @media (max-width: 900px) {
            body.apple-content-page .zixun .w1920 h4 {
                font-size: 30px !important;
                line-height: 1.22 !important;
            }
            body.apple-content-page .zixun-tt h4 {
                font-size: 28px !important;
            }
        }
    </style>
</head>

<body class="ac apple-content-page">
    <LiteratureManager:top ID="top" runat="server" />
    <div class="middle">
        <section class="zixun" style="background-image: url(<%=GetNewsBannerImage() %>);">
            <div class="w1920">
                <h4><%=Function.HtmlDiscode(tbl_class.about) %></h4>
            </div>
        </section>
        <section class="zixun-box">
            <div class="w1920">
                <div class="zixun-tt">
                    <h4><%=Function.HtmlDiscode(tbl_class.classname) %></h4>
                </div>
                <div class="zixunList" id="data-list">

                    <asp:Repeater ID="DataList" runat="server">
                        <ItemTemplate>
                            <a class="data-item-box" target="_blank" href="/WebsiteData/NewsInfo.aspx?id=<%#Eval("id").ToString() %>">
                                <div class="zixun-img">
                                    <img src="<%#GetNewsCardImage(Eval("upload_pic_img"), Container.ItemIndex) %>" alt="<%#Function.HtmlDiscode(Eval("name").ToString()) %>" />
                                </div>
                                <h4><%#Function.HtmlDiscode(Eval("name").ToString()) %></h4>
                                <p><%#Function.ConvertTo<DateTime>(Eval("datetime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd") %></p>
                            </a>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>
                <Webdiyer:AspNetPager ID="AspNetPager1" Style="clear: both; padding-top: 20px;" runat="server" CssClass="web" CurrentPageButtonClass="current" FirstPageText="首页" PrevPageText="上一页" NextPageText="下一页" LastPageText="尾页"
                    ShowDisabledButtons="true" OnPageChanged="AspNetPager1_PageChanged" UrlPaging="true" PageIndexBoxType="DropDownList" ShowPageIndexBox="Never">
                </Webdiyer:AspNetPager>
                <div class="scroller-status" style="display: none;text-align: center;" >
                    <div class="infinite-scroll-request loader-ellips jiazai" style="display: none;">
                        <img src="/images/jiazai.gif" />
                    </div>
                    <p class="infinite-scroll-last" style="display: none;">End of content</p>
                    <p class="infinite-scroll-error" style="display: none;">No more pages to load</p>
                </div>
                <p class="pagination" style="display: none;">
                    <a class="pagination__next" href="?page=<%=(PageIndex+1) %>">Next page</a>
                </p>
            </div>
        </section>
    </div>
    <LiteratureManager:foot ID="foot" runat="server" />
    <script src="/js/infinite-scroll.pkgd.min.js"></script>
    <script type="text/javascript">
        let infScroll = new InfiniteScroll('.zixunList', {
            // 这里配置无限滚动的选项，例如：
            path: '.pagination__next', // 下一页的路径
            append: '.data-item-box', // 要追加的元素选择器
            status: '.scroller-status',
            hideNav: '.pagination',
            checkLastPage: true,
            history: false
            // 其他选项...
        });
    </script>
</body>

</html>
