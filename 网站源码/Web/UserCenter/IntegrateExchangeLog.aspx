<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IntegrateExchangeLog.aspx.cs" Inherits="Web.UserCenter.IntegrateExchangeLog" %>

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
    <title>权益记录</title>
    <LiteratureManager:css ID="css" runat="server" />
    <style>
        body.exchange-log-page .pe-item.exchange {
            margin-top: 24px;
            padding: 32px !important;
            overflow: visible;
        }
        body.exchange-log-page .status-lass {
            margin: 0 0 24px !important;
        }
        body.exchange-log-page .pagedataList {
            display: flex !important;
            flex-direction: column;
            gap: 14px;
            padding: 0 !important;
        }
        body.exchange-log-page .exchange-item {
            display: grid !important;
            grid-template-columns: minmax(0, 1fr) auto;
            gap: 24px;
            align-items: center;
            min-height: 128px !important;
            padding: 18px 20px !important;
            border: 1px solid #e0e0e0 !important;
            border-radius: 18px !important;
            background: #fff !important;
            box-shadow: none !important;
            transform: none !important;
        }
        body.exchange-log-page .exchange-item:hover {
            border-color: #c7c7cc !important;
            transform: none !important;
        }
        body.exchange-log-page .exchange-l {
            display: grid !important;
            grid-template-columns: 96px minmax(0, 1fr);
            gap: 18px;
            align-items: center;
            min-width: 0;
        }
        body.exchange-log-page .exchange-img {
            width: 96px !important;
            height: 72px !important;
            aspect-ratio: auto !important;
            margin: 0 !important;
            border-radius: 11px !important;
            background: #fafafc !important;
            overflow: hidden;
        }
        body.exchange-log-page .exchange-img img {
            width: 100% !important;
            height: 100% !important;
            max-width: none !important;
            max-height: none !important;
            object-fit: contain !important;
            border-radius: 11px !important;
        }
        body.exchange-log-page .exchange-text {
            min-width: 0;
        }
        body.exchange-log-page .exchange-text h4 {
            margin: 0 0 8px !important;
            color: #1d1d1f !important;
            font-size: 17px !important;
            font-weight: 600 !important;
            line-height: 1.24 !important;
            letter-spacing: -0.374px !important;
        }
        body.exchange-log-page .exchange-text p {
            margin: 3px 0 !important;
            color: #333 !important;
            font-size: 14px !important;
            font-weight: 400 !important;
            line-height: 1.43 !important;
            letter-spacing: -0.224px !important;
            word-break: break-all;
        }
        body.exchange-log-page .exchange-text button {
            height: 28px;
            margin-left: 10px;
            padding: 0 12px;
            border: 1px solid #e0e0e0;
            border-radius: 9999px;
            background: #fff;
            color: #0066cc;
            font-size: 14px;
            cursor: pointer;
        }
        body.exchange-log-page .exchange-r {
            min-width: 72px;
            text-align: right;
        }
        body.exchange-log-page .exchange-r h4 {
            margin: 0 !important;
            color: #1d1d1f !important;
            font-size: 17px !important;
            font-weight: 600 !important;
            line-height: 1.24 !important;
            white-space: nowrap;
        }
        @media (max-width: 860px) {
            body.exchange-log-page .exchange-item {
                grid-template-columns: 1fr;
                gap: 14px;
            }
            body.exchange-log-page .exchange-r {
                text-align: left;
            }
        }
        @media (max-width: 620px) {
            body.exchange-log-page .pe-item.exchange {
                padding: 20px !important;
            }
            body.exchange-log-page .exchange-l {
                grid-template-columns: 1fr;
            }
            body.exchange-log-page .exchange-img {
                width: 100% !important;
                height: auto !important;
                aspect-ratio: 16 / 9 !important;
            }
        }
    </style>
</head>

<body class="ac exchange-log-page">
    <LiteratureManager:top ID="top" runat="server" />
    <div class="middle">
        <section class="account">
            <div class="w1200">
                <div class="accountBox">
                    <LiteratureManager:left ID="left" runat="server" />
                    <div class="accountR">
                        <div class="list-tt">
                            <h4>文献权益</h4>
                        </div>
                        <div class="list-class">
                            <ul>
                                <li>
                                    <a href="/User/IntegrateExchange">下载权益兑换</a>
                                </li>
                                <li>
                                    <a href="/User/IntegrateLog">收支明细</a>
                                </li>
                                <li class="current">
                                    <a>权益记录</a>
                                </li>
                            </ul>
                        </div>
                        <div class="pe-item exchange ">
                            <div class="status-lass">
                                <ul>
                                    <li <%=(type_==0?" class=\"current\"":"") %>><a href="/User/IntegrateExchangeLog">全部</a></li>
                                    <li <%=(type_==1?" class=\"current\"":"") %>><a href="/User/IntegrateExchangeLog?type=1">待使用</a></li>
                                    <li <%=(type_==-1?" class=\"current\"":"") %>><a href="/User/IntegrateExchangeLog?type=-1">已使用</a></li>
                                </ul>
                            </div>
                            <div class="pagedataList" style="padding: 20px 0;">

                                <asp:Repeater ID="DataList" runat="server">
                                    <ItemTemplate>

                                        <div class="exchange-item">
                                            <div class="exchange-l">
                                                <div class="exchange-img">
                                                    <img src="<%#CommonFunc.GetWebUpload_Pic(Eval("upload_pic_img").ToString(), "/images/null.png") %>" />
                                                </div>
                                                <div class="exchange-text">
                                                    <h4><%#Function.HtmlDiscode(Eval("name").ToString()) %></h4>
                                                    <p>消耗积分: <%#Function.HtmlDiscode(Eval("num_integrate").ToString()) %></p>
                                                    <p>兑换时间:<%#Function.ConvertTo<DateTime>(Eval("addtime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss") %></p>
                                                    <p>
                                                        权益码: <%#Function.HtmlDiscode(Eval("codestr").ToString()) %>
                                            <button data-text="<%#Function.HtmlDiscode(Eval("codestr").ToString()) %>">复制</button>
                                                    </p>
                                                </div>
                                            </div>
                                            <div class="exchange-r">
                                                <h4 <%#Eval("status").ToString()=="-1"?" style=\"color: #999999;\"":"" %>><%#(Eval("status").ToString()=="1"?"待使用":(Eval("status").ToString()=="-1"?"已使用":"")) %></h4>
                                            </div>
                                        </div>

                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>

                            <div class="scroller-status" style="display: none; text-align: center;">
                                <div class="infinite-scroll-request loader-ellips jiazai" style="display: none;">
                                    <img src="/images/jiazai.gif" />
                                </div>
                                <p class="infinite-scroll-last" style="display: none;">End of content</p>
                                <p class="infinite-scroll-error" style="display: none;">No more pages to load</p>
                            </div>
                            <p class="pagination" style="display: none;">
                                <a class="pagination__next" href="?page=<%=(PageIndex+1) %><%=(type_==1||type_==-2)?"&type="+type_:"" %>">Next page</a>
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
                append: '.exchange-item', // 要追加的元素选择器
                status: '.scroller-status',
                hideNav: '.pagination',
                checkLastPage: true,
                history: false
                // 其他选项...
            });
        </script>
        <script>
            $(document).ready(function () {
                $('.exchange-text button').click(function () {
                    var textToCopy = $(this).data('text');
                    var tempInput = $('<input>');
                    $('body').append(tempInput);
                    tempInput.val(textToCopy).select();
                    document.execCommand('copy');
                    tempInput.remove();
                    layer.msg('文字已复制到剪贴板！');
                });
            });
        </script>
    </div>

    <LiteratureManager:foot ID="foot" runat="server" />
</body>

</html>
