<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoticeLog.aspx.cs" Inherits="Web.UserCenter.NoticeLog" %>

<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="left" Src="/UserCenter/left.ascx" %>

<!DOCTYPE html>
<html lang="zh-CN">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta name='robots' content='index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1' />
    <title>消息通知-个人中心</title>
    <LiteratureManager:css ID="css" runat="server" />
    <style>
        .notice-tabs ul { display: flex; gap: 14px; padding: 0; margin: 0 0 24px; list-style: none; }
        .notice-tabs li a, .notice-tabs li span {
            display: inline-block;
            padding: 10px 18px;
            border-radius: 999px;
            background: #f3f6fa;
            color: #425466;
        }
        .notice-tabs li.current a, .notice-tabs li.current span {
            background: #1d6fdc;
            color: #ffffff;
        }
        .notice-list { display: grid; gap: 16px; }
        .notice-card {
            padding: 20px 22px;
            border: 1px solid #e7edf4;
            border-radius: 16px;
            background: #ffffff;
        }
        .notice-card h4 {
            margin: 0 0 8px;
            font-size: 18px;
            color: #1f2d3d;
        }
        .notice-meta {
            margin: 0 0 12px;
            color: #7a8795;
            font-size: 13px;
        }
        .notice-body {
            color: #314357;
            line-height: 1.9;
            white-space: pre-line;
        }
        .notice-link {
            display: inline-block;
            margin-top: 14px;
            color: #1d6fdc;
        }
        .notice-empty {
            padding: 48px 24px;
            border: 1px dashed #d6e0ea;
            border-radius: 16px;
            text-align: center;
            color: #7a8795;
            background: #fbfdff;
        }
    </style>
</head>

<body class="ac">
    <LiteratureManager:top ID="top" runat="server" />
    <div class="middle">
        <section class="account">
            <div class="w1200">
                <div class="accountBox">
                    <LiteratureManager:left ID="left" runat="server" />
                    <div class="accountR ">
                        <div class="list-tt">
                            <h4>通知消息</h4>
                        </div>
                        <div class="pe-item status">
                            <div class="notice-tabs">
                                <ul>
                                    <li><a href="/User/MsgLog">互动提醒（<%=ReplyCount %>）</a></li>
                                    <li class="current"><span>系统通知（<%=intRecordCount %>）</span></li>
                                </ul>
                            </div>
                            <div class="notice-list pagedataList">
                                <asp:Repeater ID="DataList" runat="server">
                                    <ItemTemplate>
                                        <div class="notice-card data-item-page">
                                            <h4><%# GetNoticeTitle(Eval("name")) %></h4>
                                            <p class="notice-meta"><%# GetNoticeTime(Eval("addtime")) %></p>
                                            <div class="notice-body"><%# GetNoticeBody(Eval("info_")) %></div>
                                            <%# GetNoticeLink(Eval("url")) %>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <% if (!HasData) { %>
                                <div class="notice-empty">
                                    暂无可展示的系统通知。后续审核结果、积分变更与文献处理提醒会显示在这里。
                                </div>
                                <% } %>
                            </div>
                            <div class="scroller-status" style="display: none; text-align: center;">
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
                    </div>
                </div>
            </div>
        </section>
        <script src="/js/infinite-scroll.pkgd.min.js"></script>
        <script type="text/javascript">
            let infScroll = new InfiniteScroll('.pagedataList', {
                // 这里配置无限滚动的选项，例如：
                path: '.pagination__next', // 下一页的路径
                append: '.data-item-page', // 要追加的元素选择器
                status: '.scroller-status',
                hideNav: '.pagination',
                checkLastPage: true,
                history: false
                // 其他选项...
            });
        </script>
    </div>
    <LiteratureManager:foot ID="foot" runat="server" />
</body>

</html>
