<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceLog.aspx.cs" Inherits="Web.UserCenter.ServiceLog" %>

<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="left" Src="/UserCenter/left.ascx" %>

<!DOCTYPE html>
<html lang="en-US">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta name='robots' content='index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1' />
    <title>问题反馈-个人中心</title>
    <LiteratureManager:css ID="css" runat="server" />
    <style>
        .order-con a h4.status0 {
            color:#999999;
        }
         .order-con a h4.status1 {
            color:#ff0000;
        } .order-con a h4.status2 {
            color:#034d02;
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
                    <div class="accountR  order">
                        <div class="pe-item-tt">
                            <h4>问题反馈</h4>
                            <a href="/User/ServiceLogAdd">
                                <img src="/images/gongdan1.png">提交反馈</a>
                        </div>
                        <div class="order-list">
                            <div class="order-tt">
                                <div class="order-item"><span>反馈主题</span></div>
                                <div class="order-item"><span>更新时间</span></div>
                                <div class="order-item"><span>状态</span></div>
                                <div class="order-item"></div>
                            </div>
                            <div class="order-con pagedataList">
                                <asp:Repeater ID="DataList" runat="server">
                                    <ItemTemplate>
                                        <a href="/User/ServiceLog_<%#Eval("id").ToString() %>" class="data-item-page">
                                            <div class="order-item">
                                                <h4><%#Function.HtmlDiscode(Eval("name").ToString()) %></h4>
                                            </div>
                                            <div class="order-item">
                                                <h4><%#Function.ConvertTo<DateTime>(Eval("uptime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss") %></h4>
                                            </div>
                                            <div class="order-item">
                                                <h4 class="status<%#Eval("status").ToString() %>"><%#CommonFunc.GetServiceLogStatusNameFunc(Eval("status").ToString()) %></h4>
                                            </div>
                                            <div class="order-item">
                                                <img src="/images/xia.png" />
                                            </div>
                                        </a>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                            <Webdiyer:AspNetPager ID="AspNetPager1" Style="clear: both; padding-top: 20px;" runat="server" CssClass="web" CurrentPageButtonClass="current" FirstPageText="首页" PrevPageText="上一页" NextPageText="下一页" LastPageText="尾页"
                                ShowDisabledButtons="true" OnPageChanged="AspNetPager1_PageChanged" UrlPaging="true" PageIndexBoxType="DropDownList" ShowPageIndexBox="Never">
                            </Webdiyer:AspNetPager>
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
