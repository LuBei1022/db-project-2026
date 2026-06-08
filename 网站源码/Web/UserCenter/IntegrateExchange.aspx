<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IntegrateExchange.aspx.cs" Inherits="Web.UserCenter.IntegrateExchange" %>

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
    <title>下载权益兑换</title>
    <LiteratureManager:css ID="css" runat="server" />
    <style>
        body.integrate-exchange-page {
            background: #f5f5f7;
        }
        body.integrate-exchange-page .accountBox {
            align-items: flex-start;
        }
        body.integrate-exchange-page .accountR {
            min-width: 0;
        }
        body.integrate-exchange-page .pe-item.points {
            margin-top: 24px;
            padding: 32px !important;
            overflow: visible;
        }
        body.integrate-exchange-page .points .pe-list {
            display: grid !important;
            grid-template-columns: repeat(auto-fit, minmax(280px, 1fr)) !important;
            gap: 24px !important;
            align-items: stretch;
            padding: 0 !important;
        }
        body.integrate-exchange-page .points .pe-li-item {
            display: flex !important;
            flex-direction: column;
            min-width: 0 !important;
            width: auto !important;
            min-height: 0 !important;
            padding: 24px !important;
            box-sizing: border-box;
            border: 1px solid #e0e0e0 !important;
            border-radius: 18px !important;
            background: #fff !important;
            box-shadow: none !important;
            overflow: hidden;
        }
        body.integrate-exchange-page .points .pe-li-item:hover {
            transform: none !important;
            border-color: #c7c7cc !important;
        }
        body.integrate-exchange-page .points .pe-li-item .pe-li-img {
            display: flex;
            align-items: center;
            justify-content: center;
            width: 100% !important;
            height: auto !important;
            aspect-ratio: 16 / 9;
            margin: 0 0 24px !important;
            border-radius: 11px !important;
            background: #fafafc !important;
            overflow: hidden;
        }
        body.integrate-exchange-page .points .pe-li-item .pe-li-img img {
            width: 100% !important;
            height: 100% !important;
            max-width: none !important;
            max-height: none !important;
            object-fit: contain;
            border-radius: 11px !important;
        }
        body.integrate-exchange-page .points .pe-li-item h4 {
            margin: 0 0 8px !important;
            padding: 0 !important;
            color: #1d1d1f !important;
            font-size: 21px !important;
            font-weight: 600 !important;
            line-height: 1.19 !important;
            letter-spacing: 0.231px !important;
            overflow: visible !important;
            display: block !important;
        }
        body.integrate-exchange-page .points .pe-li-item .data-box {
            flex: 1 1 auto;
            min-width: 0;
        }
        body.integrate-exchange-page .points .pe-li-item .data-box p {
            min-height: 74px !important;
            margin: 0 !important;
            color: #333 !important;
            font-size: 17px !important;
            font-weight: 400 !important;
            line-height: 1.47 !important;
            letter-spacing: -0.374px !important;
        }
        body.integrate-exchange-page .points .status-but {
            display: flex !important;
            align-items: center !important;
            justify-content: space-between !important;
            gap: 16px !important;
            margin-top: 28px !important;
            padding-top: 0 !important;
        }
        body.integrate-exchange-page .points .status-short {
            flex: 0 0 auto;
            display: inline-flex !important;
            align-items: center;
            color: #1d1d1f !important;
            font-size: 17px !important;
            font-weight: 600 !important;
            line-height: 1.24 !important;
            letter-spacing: -0.374px !important;
        }
        body.integrate-exchange-page .points .status-short svg {
            width: 18px;
            height: 18px;
            margin-right: 4px;
        }
        body.integrate-exchange-page .points .status-medium {
            flex: 0 0 auto;
            min-width: 118px !important;
            height: 44px !important;
            padding: 0 20px !important;
            border-radius: 9999px !important;
            background: #0066cc !important;
            color: #fff !important;
            font-size: 17px !important;
            font-weight: 400 !important;
            line-height: 1 !important;
        }
        body.integrate-exchange-page .points .status-medium img {
            width: 17px !important;
            height: 12px !important;
            margin-right: 7px !important;
            filter: brightness(0) invert(1);
        }
        body.integrate-exchange-page .scroller-status {
            padding: 16px 0 0;
            color: #7a7a7a;
            font-size: 14px;
            letter-spacing: -0.224px;
        }
        @media (max-width: 1180px) {
            body.integrate-exchange-page .points .pe-list {
                grid-template-columns: repeat(2, minmax(260px, 1fr)) !important;
            }
        }
        @media (max-width: 760px) {
            body.integrate-exchange-page .pe-item.points {
                padding: 20px !important;
            }
            body.integrate-exchange-page .points .pe-list {
                grid-template-columns: 1fr !important;
            }
            body.integrate-exchange-page .points .status-but {
                align-items: flex-start !important;
                flex-direction: column;
            }
        }
    </style>
</head>

<body class="ac integrate-exchange-page">
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
                                <li class="current">
                                    <a>下载权益兑换</a>
                                </li>
                                <li>
                                    <a href="/User/IntegrateLog">收支明细</a>
                                </li>
                                <li>
                                    <a href="/User/IntegrateExchangeLog">权益记录</a>
                                </li>
                            </ul>
                        </div>

                        <div class="pe-item points">
                            <div class="pe-list pagedataList">

                                <asp:Repeater ID="DataList" runat="server">
                                    <ItemTemplate>
                                        <div class="pe-li-item">
                                            <a class="pe-li-img">
                                                <img src="<%#CommonFunc.GetWebUpload_Pic(Eval("upload_pic_img").ToString(), "/images/null.png") %>" />
                                            </a>
                                            <h4><%#Function.HtmlDiscode(Eval("name").ToString()) %></h4>
                                            <div class="data-box">
                                                <p><%#Function.HtmlDiscodeWeb(Eval("about_").ToString()) %></p>
                                            </div>
                                            <div class="status-but">
                                            <div class="status-short"><svg t="1766634151956" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="1724" width="16" height="16"><path d="M512 1024A512 512 0 1 0 512 0a512 512 0 0 0 0 1024z m301.738667-813.738667Q938.666667 335.36 938.666667 512q0 176.725333-124.928 301.738667Q688.64 938.666667 512 938.666667q-176.725333 0-301.738667-124.928Q85.333333 688.64 85.333333 512q0-176.725333 124.928-301.738667Q335.36 85.333333 512 85.333333q176.725333 0 301.738667 124.928zM581.888 432.896a17.066667 17.066667 0 0 1-16.213333-11.776l-37.461334-115.2a17.066667 17.066667 0 0 0-32.426666 0l-37.461334 115.2a17.066667 17.066667 0 0 1-16.213333 11.776H321.024a17.066667 17.066667 0 0 0-9.984 30.890667l97.962667 71.168a17.066667 17.066667 0 0 1 6.144 19.029333l-37.376 115.2a17.066667 17.066667 0 0 0 26.282666 19.029333l97.877334-71.168a17.066667 17.066667 0 0 1 20.138666 0l97.877334 71.168a17.066667 17.066667 0 0 0 26.282666-19.029333l-37.376-115.2a17.066667 17.066667 0 0 1 6.144-19.029333l97.962667-71.168a17.066667 17.066667 0 0 0-9.984-30.890667h-121.173333z" fill="#232323" p-id="1725"></path></svg><%#Eval("num_integrate").ToString() %></div>
                                                <div class="status-medium" data-id="<%#Eval("id").ToString() %>" data-integrate="<%#Eval("num_integrate").ToString() %>">
                                                    <img src="/images/duihuan.png">兑换权益
                                                </div>
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
                append: '.pe-li-item', // 要追加的元素选择器
                status: '.scroller-status',
                hideNav: '.pagination',
                checkLastPage: true,
                history: false
                // 其他选项...
            });
        </script>

        <script>
            layui.use('layer', function () {
                var $ = layui.jquery, layer = layui.layer;
                $('.points  .status-medium').click(function () {
                    var jifen_id = $(this).data("id");
                    var jifen_integrate = $(this).data("integrate");
                    if (parseInt(jifen_id) > 0 && parseInt(jifen_integrate) > 0) {
                        let up_jifen = layer.open({
                            type: 1,
                            skin: 'layui-layer-jifen',
                            title: false,
                            shadeClose: true,
                            shade: 0.6,
                            area: ['270px', '160px'],
                            content: `<div class="failure-box"><p>确定消耗<span>` + jifen_integrate +`</span>积分兑换此下载权益吗？</p></div>`,
                            btn: ['否', '是'],
                            btn1: function (index, layero) {
                                //取消
                                layer.close(up_jifen);
                                return false;

                            },
                            btn2: function (index, layero) {
                                var param1_json = { // 提交数据
                                    "btn": "IntegrateExchangeAdd",
                                    "num": jifen_integrate,
                                    "id": jifen_id,
                                    "user_id": <%=user_list.id%>
                                        }
                                $.ajax({
                                    url: "/Inc/UserCommon.ashx",
                                    cache: true,
                                    async: false,
                                    data: JSON.stringify(param1_json),
                                    dataType: "json",
                                    type: "POST",
                                    success: function (datas) {
                                        if (datas.status == 1) {
                                            layer.close(up_jifen);
                                            layer.msg(datas.info, { icon: 1 });
                                        } else {
                                            layer.msg(datas.info, { icon: 0 });
                                        }
                                    },
                                    error: function (err) {
                                        console.log(JSON.stringify(err))
                                    }
                                });
                                return false;
                            }
                        });
                    }
                })
            })

        </script>
    </div>
    <LiteratureManager:foot ID="foot" runat="server" />
</body>

</html>
