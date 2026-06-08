<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Single.aspx.cs" Inherits="Web.WebsiteData.Single" %>
<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<!DOCTYPE html>
<html lang="en-US">

<head>
   <meta charset="UTF-8">
   <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
   <meta name='robots' content='index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1' />
   <title><%=CommonFunc.GetTitle("", tbclass_title) %></title>
    <meta name="description" content="<%=CommonFunc.GetDescription(tbl_class.description) %>" />
    <LiteratureManager:css ID="css" runat="server" />
    <style>
        body.apple-single-page {
            background: #f5f5f7 !important;
            color: #1d1d1f !important;
            font-family: "SF Pro Text", "PingFang SC", "Microsoft YaHei", system-ui, -apple-system, BlinkMacSystemFont, sans-serif !important;
        }
        body.apple-single-page .middle {
            min-height: 100vh;
            padding: 132px 24px 150px !important;
            box-sizing: border-box;
            background: #f5f5f7 !important;
        }
        body.apple-single-page .imageText,
        body.apple-single-page .imageText-con {
            width: 100%;
            padding: 0 !important;
            margin: 0 !important;
            background: transparent !important;
        }
        body.apple-single-page .imageText .w1200 {
            position: relative;
            width: 100% !important;
            max-width: 1120px !important;
            min-height: 248px !important;
            margin: 0 auto 20px !important;
            padding: 48px 56px !important;
            box-sizing: border-box;
            display: flex;
            align-items: center;
            border-radius: 18px !important;
            border: 1px solid #e0e0e0 !important;
            background-color: #ffffff !important;
            background-size: cover !important;
            background-position: center !important;
            overflow: hidden;
            box-shadow: none !important;
        }
        body.apple-single-page .imageText .w1200:before {
            content: "";
            position: absolute;
            inset: 0;
            background: rgba(255,255,255,.92);
            pointer-events: none;
        }
        body.apple-single-page .imageText .w1200:after {
            content: "ACADEMIC PORTAL";
            position: absolute;
            left: 56px;
            top: 40px;
            padding: 8px 14px;
            border-radius: 9999px;
            border: 1px solid #f0f0f0;
            background: #fafafc;
            color: #7a7a7a;
            font-size: 12px;
            font-weight: 600;
            line-height: 1;
            letter-spacing: .12em;
            text-transform: uppercase;
        }
        body.apple-single-page .single-hero-inner {
            position: relative;
            z-index: 1;
            max-width: 760px;
            padding-top: 34px;
        }
        body.apple-single-page .imageText h4 {
            margin: 0 0 18px !important;
            color: #1d1d1f !important;
            font-family: "SF Pro Display", "PingFang SC", "Microsoft YaHei", system-ui, -apple-system, BlinkMacSystemFont, sans-serif !important;
            font-size: clamp(30px, 3.2vw, 40px) !important;
            font-weight: 600 !important;
            line-height: 1.1 !important;
            letter-spacing: 0 !important;
            text-shadow: none !important;
        }
        body.apple-single-page .single-lead {
            max-width: 720px;
            margin: 0 !important;
            color: #6e6e73 !important;
            font-size: 21px !important;
            font-weight: 400 !important;
            line-height: 1.38 !important;
            letter-spacing: .011em !important;
        }
        body.apple-single-page .single-content-shell {
            width: 100%;
            max-width: 1120px;
            margin: 0 auto;
            display: grid;
            grid-template-columns: 280px minmax(0, 1fr);
            gap: 20px;
            align-items: start;
        }
        body.apple-single-page .single-aside {
            border: 1px solid #e0e0e0;
            border-radius: 18px;
            background: #fff;
            padding: 24px;
            box-sizing: border-box;
        }
        body.apple-single-page .single-aside h5,
        body.apple-single-page .single-aside strong {
            display: block;
            margin: 0 0 16px;
            color: #1d1d1f;
            font-size: 17px;
            font-weight: 600;
            line-height: 1.24;
            letter-spacing: -0.374px;
        }
        body.apple-single-page .single-aside a,
        body.apple-single-page .single-aside span {
            display: flex;
            align-items: center;
            min-height: 42px;
            margin-top: 8px;
            padding: 0 14px;
            border-radius: 9999px;
            background: #f5f5f7;
            color: #0066cc;
            font-size: 14px;
            line-height: 1.29;
            letter-spacing: -0.224px;
            text-decoration: none;
        }
        body.apple-single-page .single-meta-grid {
            display: grid;
            grid-template-columns: 1fr;
            gap: 10px;
            margin-top: 22px;
        }
        body.apple-single-page .single-meta {
            padding: 14px;
            border-radius: 11px;
            background: #fafafc;
            color: #333;
            font-size: 14px;
            line-height: 1.43;
            letter-spacing: -0.224px;
        }
        body.apple-single-page .imageText-con .w1200 {
            width: 100% !important;
            max-width: none !important;
            margin: 0 !important;
            padding: 48px 56px 56px !important;
            box-sizing: border-box;
            border-radius: 18px !important;
            border: 1px solid #e0e0e0 !important;
            background: #fff !important;
            box-shadow: none !important;
        }
        body.apple-single-page .imageText-con .w1200:before {
            content: "内容说明";
            display: block;
            margin: 0 0 22px;
            color: #1d1d1f;
            font-family: "SF Pro Display", "PingFang SC", "Microsoft YaHei", system-ui, -apple-system, BlinkMacSystemFont, sans-serif;
            font-size: 28px;
            font-weight: 600;
            line-height: 1.14;
            letter-spacing: .196px;
        }
        body.apple-single-page .imageText-con h1,
        body.apple-single-page .imageText-con h2,
        body.apple-single-page .imageText-con h3,
        body.apple-single-page .imageText-con h4 {
            margin: 0 0 14px !important;
            color: #1d1d1f !important;
            font-family: "SF Pro Display", "PingFang SC", "Microsoft YaHei", system-ui, -apple-system, BlinkMacSystemFont, sans-serif !important;
            font-size: 21px !important;
            font-weight: 600 !important;
            line-height: 1.19 !important;
            letter-spacing: .231px !important;
        }
        body.apple-single-page .imageText-con p,
        body.apple-single-page .imageText-con li,
        body.apple-single-page .imageText-con div {
            color: #1d1d1f !important;
            font-size: 17px !important;
            font-weight: 400 !important;
            line-height: 1.47 !important;
            letter-spacing: -0.374px !important;
        }
        body.apple-single-page .imageText-con p {
            margin: 0 0 18px !important;
        }
        body.apple-single-page .imageText-con ul,
        body.apple-single-page .imageText-con ol {
            margin: 20px 0 !important;
            padding-left: 24px !important;
        }
        body.apple-single-page .imageText-con img {
            max-width: 100% !important;
            height: auto !important;
            margin: 24px 0 !important;
            border-radius: 8px !important;
            box-shadow: rgba(0, 0, 0, .18) 3px 5px 30px 0;
        }
        @media (max-width: 760px) {
            body.apple-single-page .middle {
                padding: 116px 16px 220px !important;
            }
            body.apple-single-page .single-content-shell {
                grid-template-columns: 1fr;
            }
            body.apple-single-page .imageText .w1200,
            body.apple-single-page .imageText-con .w1200 {
                padding-left: 24px !important;
                padding-right: 24px !important;
            }
            body.apple-single-page .imageText .w1200:after {
                left: 24px;
                top: 28px;
            }
            body.apple-single-page .imageText .w1200 {
                min-height: 240px !important;
                padding-top: 82px !important;
                padding-bottom: 36px !important;
            }
            body.apple-single-page .single-lead {
                font-size: 17px !important;
            }
        }
    </style>
</head>

<body class="ac apple-single-page">
  <LiteratureManager:top ID="top" runat="server" />
   <div class="middle">
        <section class="imageText">
            <div class="w1200" <%=!string.IsNullOrWhiteSpace(banner)?" style=\"background-image: url("+banner+");\"":"" %>>
                <div class="single-hero-inner">
                   <h4><%=SingleTitle %></h4>
                   <p class="single-lead"><%=SingleIntro %></p>
                </div>
            </div>
      </section>
        <div class="single-content-shell">
            <aside class="single-aside">
                <h5>页面导航</h5>
                <a href="/LiteratureSearch">文献检索</a>
                <a href="/Website/news">学术资讯</a>
                <a href="/User/ServiceLog">服务支持</a>
                <div class="single-meta-grid">
                    <div class="single-meta">栏目：<%=SingleTitle %></div>
                    <div class="single-meta">风格：简洁、清晰、可阅读</div>
                </div>
            </aside>
            <section class="imageText-con">
                <div class="w1200">
                   <%=Function.Replace_Content(tbl_class.info_) %>
                </div>
          </section>
        </div>

   </div>
   <LiteratureManager:foot ID="foot" runat="server" />
</body>

</html>
