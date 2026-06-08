<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IntegrateLog.aspx.cs" Inherits="Web.UserCenter.IntegrateLog" %>
<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="left" Src="/UserCenter/left.ascx" %>

<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <meta name="robots" content="index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1" />
    <title>&#6211;&#30340;&#31215;&#20998;</title>
    <LiteratureManager:css ID="css" runat="server" />
    <script type="text/javascript" src="/js/jquery-1.11.3.min.js"></script>
    <script type="text/javascript" src="/js/qrcode.min.js"></script>
    <style>
        .points-card { margin-bottom: 24px; padding: 24px; border: 1px solid #ebeff4; border-radius: 18px; background: linear-gradient(135deg, #f7fbff 0%, #ffffff 100%); }
        .points-card h5 { margin: 0 0 10px; font-size: 20px; color: #1a2d42; }
        .points-card p { margin: 0; color: #607084; line-height: 1.8; }
        .points-metrics { display: flex; gap: 14px; margin-top: 16px; flex-wrap: wrap; }
        .points-metrics .metric { min-width: 160px; padding: 14px 16px; border-radius: 14px; background: #fff; border: 1px solid #e5edf5; }
        .points-metrics .metric strong { display: block; font-size: 26px; color: #153a68; }
        .topup-panel { margin: 20px 0 26px; padding: 22px; border: 1px solid #e8eef5; border-radius: 18px; background: #fff; }
        .topup-panel h5 { margin: 0 0 8px; font-size: 18px; color: #1a2d42; }
        .topup-panel p { margin: 0; color: #69788b; line-height: 1.8; }
        .topup-options { display: flex; flex-wrap: wrap; gap: 12px; margin-top: 18px; }
        .topup-option { min-width: 108px; height: 44px; padding: 0 18px; border: 1px solid #dbe7f2; border-radius: 12px; background: #f8fbff; color: #204a7b; cursor: pointer; }
        .topup-option.current { border-color: #1d6fdc; background: #eaf3ff; color: #1d6fdc; }
        .topup-custom { display: flex; gap: 12px; align-items: center; margin-top: 16px; flex-wrap: wrap; }
        .topup-custom input { width: 220px; height: 42px; border: 1px solid #d7e0ea; border-radius: 12px; padding: 0 14px; }
        .topup-submit { margin-top: 18px; display: flex; gap: 12px; align-items: center; flex-wrap: wrap; }
        .topup-submit button { min-width: 136px; height: 44px; border: none; border-radius: 12px; background: #1d6fdc; color: #fff; cursor: pointer; }
        .topup-submit span { color: #7a8795; font-size: 13px; }
        .topup-dialog { display: none; position: fixed; left: 0; top: 0; right: 0; bottom: 0; background: rgba(17, 24, 39, 0.46); z-index: 9999; }
        .topup-dialog.active { display: block; }
        .topup-dialog-inner { width: 420px; max-width: calc(100vw - 24px); margin: 8vh auto 0; background: #fff; border-radius: 22px; padding: 28px; box-shadow: 0 30px 80px rgba(11, 31, 54, 0.24); }
        .topup-dialog-head { display: flex; justify-content: space-between; align-items: center; margin-bottom: 18px; }
        .topup-dialog-head h4 { margin: 0; font-size: 22px; color: #1a2d42; }
        .topup-close { border: none; background: transparent; font-size: 24px; color: #8a98a8; cursor: pointer; }
        .topup-paybox { text-align: center; padding: 18px 0 8px; }
        .topup-qrcode { width: 220px; height: 220px; margin: 0 auto 16px; display: flex; align-items: center; justify-content: center; border: 1px solid #eef3f8; border-radius: 18px; background: #fbfdff; }
        .topup-qrcode img { max-width: 100%; }
        .topup-order-meta { margin-top: 12px; font-size: 13px; color: #6c7b8d; line-height: 1.9; word-break: break-all; }
        .topup-status { margin-top: 12px; font-size: 14px; color: #2b4a6a; }
        @media (max-width: 860px) {
            .topup-custom { align-items: flex-start; }
            .topup-custom input { width: 100%; }
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
                    <div class="accountR">
                        <div class="list-tt">
                            <h4>&#25991;&#29486;&#26435;&#30410;</h4>
                        </div>
                        <div class="list-class">
                            <ul>
                                <li>
                                    <a href="/User/IntegrateExchange">&#19979;&#36733;&#26435;&#30410;&#20817;&#25442;</a>
                                </li>
                                <li class="current">
                                    <a href="/User/IntegrateLog">&#25910;&#25903;&#26126;&#32454;</a>
                                </li>
                                <li>
                                    <a href="/User/IntegrateExchangeLog">&#26435;&#30410;&#35760;&#24405;</a>
                                </li>
                            </ul>
                            <div class="jifen-but">
                                <div class="list-upload">
                                    <p>&#32047;&#35745;&#33719;&#24471;: <span><%=huoqu_num_integrate %></span></p>
                                </div>
                                <div class="list-upload">
                                    <p>&#32047;&#35745;&#28040;&#32791;: <span><%=xiaohao_num_integrate %></span></p>
                                </div>
                            </div>
                        </div>

                        <div class="points-card">
                            <h5>&#31215;&#20998;&#20313;&#39069;</h5>
                            <p>&#25991;&#29486;&#19979;&#36733;&#12289;&#26435;&#30410;&#20817;&#25442;&#21644;&#31449;&#20869;&#22870;&#21169;&#37117;&#20351;&#29992;&#21516;&#19968;&#22871;&#31215;&#20998;&#20307;&#31995;&#12290;&#29616;&#22312;&#25903;&#25345;&#24494;&#20449;&#25195;&#30721;&#20805;&#20540;&#65292;&#25903;&#20184;&#25104;&#21151;&#21518;&#31215;&#20998;&#20250;&#33258;&#21160;&#21040;&#36134;&#12290;</p>
                            <div class="points-metrics">
                                <div class="metric">
                                    <span>&#24403;&#21069;&#31215;&#20998;</span>
                                    <strong><%=current_integrate %></strong>
                                </div>
                                <div class="metric">
                                    <span>&#20817;&#25442;&#27604;&#20363;</span>
                                    <strong>1 &#20803; = <%=money_integrate %> &#31215;&#20998;</strong>
                                </div>
                                <div class="metric">
                                    <span>&#39318;&#20805;&#36192;&#36865;</span>
                                    <strong><%=(integrate_donate>0?integrate_donate + "%":"&#26410;&#24320;&#21551;") %></strong>
                                </div>
                            </div>
                        </div>

                        <div class="topup-panel">
                            <h5>&#24494;&#20449;&#25903;&#20184;&#20805;&#20540;</h5>
                            <p>&#36873;&#25321;&#22266;&#23450;&#37329;&#39069;&#25110;&#36755;&#20837;&#33258;&#23450;&#20041;&#37329;&#39069;&#65292;&#31995;&#32479;&#20250;&#29983;&#25104;&#24494;&#20449;&#25903;&#20184;&#20108;&#32500;&#30721;&#12290;&#25903;&#20184;&#23436;&#25104;&#21518;&#39029;&#38754;&#20250;&#33258;&#21160;&#26597;&#35810;&#32467;&#26524;&#24182;&#21047;&#26032;&#31215;&#20998;&#12290;</p>
                            <div class="topup-options" id="topupOptions"><%=TopUpOptionsHtml %></div>
                            <div class="topup-custom">
                                <label for="customTopUpMoney">&#33258;&#23450;&#20041;&#37329;&#39069;</label>
                                <input type="text" id="customTopUpMoney" placeholder="&#35831;&#36755;&#20837; 1-1000 &#20043;&#38388;&#30340;&#25972;&#25968;&#37329;&#39069;" />
                            </div>
                            <div class="topup-submit">
                                <button type="button" id="btnCreateTopUp">&#24494;&#20449;&#20805;&#20540;&#31215;&#20998;</button>
                                <span>&#25903;&#20184;&#25104;&#21151;&#21518;&#21487;&#33719;&#24471;&#22522;&#30784;&#31215;&#20998;&#65292;&#33509;&#24320;&#21551;&#39318;&#20805;&#36192;&#36865;&#65292;&#36824;&#20250;&#33258;&#21160;&#34917;&#21457;&#22870;&#21169;&#31215;&#20998;&#12290;</span>
                            </div>
                        </div>

                        <div class="pe-item status">
                            <div class="status-lass">
                                <ul>
                                    <li <%=(type_==0?" class=\"current\"":"") %>><a href="/User/IntegrateLog">&#20840;&#37096;</a></li>
                                    <li <%=(type_==1?" class=\"current\"":"") %>><a href="/User/IntegrateLog?type=1">&#24050;&#33719;&#21462;</a></li>
                                    <li <%=(type_==-1?" class=\"current\"":"") %>><a href="/User/IntegrateLog?type=-1">&#24050;&#28040;&#32791;</a></li>
                                </ul>
                            </div>
                            <div class="message-list pagedataList">
                                <asp:Repeater ID="DataList" runat="server">
                                    <ItemTemplate>
                                        <div class="message-item">
                                            <div class="message-text">
                                                <h4><%#Function.HtmlDiscode(Eval("name").ToString()) %></h4>
                                                <h6><%#Function.HtmlDiscode(Eval("info_").ToString()) %></h6>
                                                <p>&#33719;&#21462;&#26102;&#38388;&#65306;<%#Function.ConvertTo<DateTime>(Eval("addtime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss") %></p>
                                            </div>
                                            <div class="message-xiala">
                                                <p><%#Function.HtmlDiscode(Eval("num_integrate").ToString()) %></p>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>

                        <div class="scroller-status" style="display: none; text-align: center;">
                            <div class="infinite-scroll-request loader-ellips jiazai" style="display: none;">
                                <img src="/images/jiazai.gif" />
                            </div>
                            <p class="infinite-scroll-last" style="display: none;">End of content</p>
                            <p class="infinite-scroll-error" style="display: none;">No more pages to load</p>
                        </div>
                        <p class="pagination" style="display: none;">
                            <a class="pagination__next" href="?page=<%=(PageIndex+1) %><%=(type_==1||type_==-1)?"&type="+type_:"" %>">Next page</a>
                        </p>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="topup-dialog" id="topupDialog">
        <div class="topup-dialog-inner">
            <div class="topup-dialog-head">
                <h4>&#24494;&#20449;&#25195;&#30721;&#25903;&#20184;</h4>
                <button type="button" class="topup-close" id="btnCloseTopupDialog">&times;</button>
            </div>
            <div class="topup-paybox">
                <div class="topup-qrcode" id="topupQrCode"></div>
                <div class="topup-status" id="topupStatus">&#35831;&#20351;&#29992;&#24494;&#20449;&#25195;&#25551;&#20108;&#32500;&#30721;&#23436;&#25104;&#25903;&#20184;&#12290;</div>
                <div class="topup-order-meta" id="topupMeta"></div>
            </div>
        </div>
    </div>

    <LiteratureManager:foot ID="foot" runat="server" />
    <script type="text/javascript" src="/js/infinite-scroll.pkgd.min.js"></script>
    <script type="text/javascript">
        var infScroll = new InfiniteScroll('.pagedataList', {
            path: '.pagination__next',
            append: '.message-item',
            status: '.scroller-status',
            hideNav: '.pagination',
            checkLastPage: true,
            history: false
        });

        (function () {
            var selectedId = 0;
            var selectedMoney = 0;
            var currentOrderNo = "";
            var pollTimer = null;
            var dialog = document.getElementById("topupDialog");
            var qrBox = document.getElementById("topupQrCode");
            var statusEl = document.getElementById("topupStatus");
            var metaEl = document.getElementById("topupMeta");

            function setDialogVisible(visible) {
                if (!dialog) {
                    return;
                }
                dialog.className = visible ? "topup-dialog active" : "topup-dialog";
                if (!visible && pollTimer) {
                    window.clearInterval(pollTimer);
                    pollTimer = null;
                }
            }

            function setStatus(text, color) {
                if (!statusEl) {
                    return;
                }
                statusEl.innerHTML = text || "";
                if (color) {
                    statusEl.style.color = color;
                }
            }

            function getCustomMoney() {
                var input = document.getElementById("customTopUpMoney");
                if (!input) {
                    return 0;
                }
                var value = parseInt(input.value, 10);
                if (!isNaN(value) && value >= 1 && value <= 1000) {
                    return value;
                }
                return 0;
            }

            function renderQr(codeUrl) {
                if (!qrBox) {
                    return;
                }
                qrBox.innerHTML = "";
                new QRCode(qrBox, {
                    text: codeUrl,
                    width: 220,
                    height: 220
                });
            }

            function beginPolling() {
                if (pollTimer) {
                    window.clearInterval(pollTimer);
                }
                pollTimer = window.setInterval(function () {
                    if (!currentOrderNo) {
                        return;
                    }
                    $.ajax({
                        url: "/Inc/UserCommon.ashx",
                        cache: false,
                        data: JSON.stringify({
                            btn: "QueryTopUpStatus",
                            out_trade_no: currentOrderNo
                        }),
                        dataType: "json",
                        type: "POST",
                        success: function (res) {
                            if (!res) {
                                return;
                            }
                            if (res.status === 1) {
                                var extra = "";
                                if (parseInt(res.gift_amount || 0, 10) > 0) {
                                    extra = "&#65292;&#21478;&#36192;&#36865; " + res.gift_amount + " &#31215;&#20998;";
                                }
                                setStatus("&#25903;&#20184;&#25104;&#21151;&#65292;&#24050;&#21040;&#36134; " + (res.integrate_amount || 0) + " &#31215;&#20998;" + extra + "&#12290;&#39029;&#38754;&#21363;&#23558;&#21047;&#26032;&#12290;", "#1a8f49");
                                if (pollTimer) {
                                    window.clearInterval(pollTimer);
                                    pollTimer = null;
                                }
                                window.setTimeout(function () { window.location.reload(); }, 1800);
                            } else if (res.status === 2) {
                                setStatus("&#35746;&#21333;&#24453;&#25903;&#20184;&#65292;&#35831;&#23436;&#25104;&#24494;&#20449;&#25195;&#30721;&#21518;&#31561;&#24453;&#31995;&#32479;&#30830;&#35748;&#12290;", "#2b4a6a");
                            } else if (res.status === -1) {
                                setStatus(res.info || "&#30331;&#24405;&#24050;&#22833;&#25928;&#65292;&#35831;&#37325;&#26032;&#30331;&#24405;&#12290;", "#d9534f");
                                if (pollTimer) {
                                    window.clearInterval(pollTimer);
                                    pollTimer = null;
                                }
                            } else if (res.info) {
                                setStatus(res.info, "#d9534f");
                            }
                        }
                    });
                }, 3000);
            }

            $("#topupOptions").on("click", ".topup-option", function () {
                $("#topupOptions .topup-option").removeClass("current");
                $(this).addClass("current");
                selectedId = parseInt($(this).attr("data-id"), 10) || 0;
                selectedMoney = parseInt($(this).attr("data-money"), 10) || 0;
                $("#customTopUpMoney").val("");
            });

            $("#customTopUpMoney").on("input", function () {
                $("#topupOptions .topup-option").removeClass("current");
                selectedId = 0;
                selectedMoney = 0;
                this.value = this.value.replace(/[^\d]/g, "");
            });

            $("#btnCreateTopUp").on("click", function () {
                var customMoney = getCustomMoney();
                var money = customMoney > 0 ? customMoney : selectedMoney;
                var idValue = customMoney > 0 ? 0 : selectedId;
                if (!money || money < 1 || money > 1000) {
                    alert("&#35831;&#36873;&#25321;&#20805;&#20540;&#37329;&#39069;&#65292;&#25110;&#36755;&#20837; 1-1000 &#20043;&#38388;&#30340;&#25972;&#25968;&#37329;&#39069;&#12290;");
                    return;
                }

                $("#btnCreateTopUp").prop("disabled", true);
                $.ajax({
                    url: "/Inc/UserCommon.ashx",
                    cache: false,
                    data: JSON.stringify({
                        btn: "AddTopUp",
                        money: customMoney > 0 ? money : 0,
                        typestr: "wx",
                        idstr: idValue
                    }),
                    dataType: "json",
                    type: "POST",
                    success: function (res) {
                        $("#btnCreateTopUp").prop("disabled", false);
                        if (!res || res.status !== 1) {
                            alert((res && res.info) ? res.info : "&#21019;&#24314;&#20805;&#20540;&#35746;&#21333;&#22833;&#36133;&#12290;");
                            return;
                        }

                        currentOrderNo = res.out_trade_no || "";
                        renderQr(res.code_url);

                        var gift = parseInt(res.gift_amount || 0, 10);
                        var meta = [];
                        meta.push("&#35746;&#21333;&#21495;&#65306;" + currentOrderNo);
                        meta.push("&#20805;&#20540;&#37329;&#39069;&#65306;" + res.money + " &#20803;");
                        meta.push("&#22522;&#30784;&#31215;&#20998;&#65306;" + (res.integrate_amount || 0));
                        if (gift > 0) {
                            meta.push("&#39318;&#20805;&#36192;&#36865;&#65306;" + gift);
                        }
                        metaEl.innerHTML = meta.join("<br />");
                        setStatus("&#35831;&#20351;&#29992;&#24494;&#20449;&#25195;&#30721;&#25903;&#20184;&#65292;&#31995;&#32479;&#20250;&#33258;&#21160;&#36718;&#35810;&#35746;&#21333;&#29366;&#24577;&#12290;", "#2b4a6a");
                        setDialogVisible(true);
                        beginPolling();
                    },
                    error: function () {
                        $("#btnCreateTopUp").prop("disabled", false);
                        alert("&#35831;&#27714;&#24322;&#24120;&#65292;&#35831;&#31245;&#21518;&#37325;&#35797;&#12290;");
                    }
                });
            });

            $("#btnCloseTopupDialog").on("click", function () {
                setDialogVisible(false);
            });

            $(dialog).on("click", function (e) {
                if (e.target === dialog) {
                    setDialogVisible(false);
                }
            });
        })();
    </script>
</body>
</html>
