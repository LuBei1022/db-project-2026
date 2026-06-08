<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IntegrateWithdrawal.aspx.cs" Inherits="Web.UserCenter.IntegrateWithdrawal" %>

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
    <title>收益提现</title>
    <LiteratureManager:css ID="css" runat="server" />
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
                            <h4>积分</h4>
                        </div>
                        <div class="list-class">
                            <ul>
                                <li>
                                    <a href="/User/IntegrateExchange">积分兑换</a>
                                </li>
                                <li class="current">
                                    <a>收益提现</a>
                                </li>
                                <li>
                                    <a href="/User/IntegrateLog">收支明细</a>
                                </li>
                                <li>
                                    <a href="/User/IntegrateExchangeLog">兑换记录</a>
                                </li>
                            </ul>
                            <%-- <div class="list-upload">
                           <p>当前收益:<span>100</span></p>
                     </div>--%>
                        </div>


                        <div class="des">
                             <%=Function.Replace_Content(websiteinfo_list.info_IntegrateWithdrawal) %>
                        </div>


                    </div>
                </div>

            </div>
        </section>
    </div>
    <LiteratureManager:foot ID="foot" runat="server" />
</body>

</html>
