<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Center.aspx.cs" Inherits="Web.UserCenter.Center" %>
<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="left" Src="/UserCenter/left.ascx" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <title>&#20010;&#20154;&#20013;&#24515;</title>
    <LiteratureManager:css ID="css" runat="server" />
    <style>
        .lit-center-header {
            display: flex;
            justify-content: space-between;
            gap: 20px;
            align-items: center;
            padding: 26px 28px;
            border: 1px solid #e7edf4;
            border-radius: 20px;
            background: linear-gradient(135deg, #ffffff 0%, #f7fbff 100%);
            margin-bottom: 24px;
        }
        .lit-center-user {
            display: flex;
            align-items: center;
            gap: 18px;
        }
        .lit-center-avatar img {
            width: 72px;
            height: 72px;
            border-radius: 50%;
            object-fit: cover;
        }
        .lit-center-user h4 {
            margin: 0 0 10px;
            font-size: 26px;
            color: #1d2f42;
        }
        .lit-center-user p {
            margin: 0;
            color: #667788;
            line-height: 1.8;
        }
        .lit-center-actions {
            display: flex;
            gap: 12px;
            flex-wrap: wrap;
        }
        .lit-center-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 132px;
            height: 42px;
            border-radius: 999px;
            background: #1d6fdc;
            color: #fff;
            transition: transform .18s ease, box-shadow .18s ease, background-color .18s ease;
        }
        .lit-center-btn.secondary {
            background: #eef5ff;
            color: #1d6fdc;
        }
        .lit-center-btn:hover {
            transform: translateY(-3px);
            box-shadow: 0 12px 24px rgba(29, 111, 220, 0.16);
        }
        .lit-center-grid {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 16px;
            margin-bottom: 24px;
        }
        .lit-center-stat {
            padding: 22px;
            border: 1px solid #e7edf4;
            border-radius: 18px;
            background: #fff;
        }
        .lit-center-stat strong {
            display: block;
            margin-bottom: 8px;
            font-size: 30px;
            color: #173a62;
        }
        .lit-center-stat span {
            color: #67788a;
            font-size: 14px;
        }
        .lit-center-panel {
            padding: 24px 26px;
            border: 1px solid #e7edf4;
            border-radius: 20px;
            background: #fff;
            margin-bottom: 24px;
        }
        .lit-center-panel h4 {
            margin: 0 0 16px;
            font-size: 22px;
            color: #1d2f42;
        }
        .lit-center-note {
            color: #6c7c8c;
            line-height: 1.9;
            margin-bottom: 14px;
        }
        .lit-center-list {
            display: grid;
            gap: 14px;
        }
        .lit-center-item {
            display: block;
            padding: 18px 20px;
            border-radius: 16px;
            background: #f9fbfe;
            border: 1px solid #edf2f8;
            cursor: pointer;
            transition: transform .18s ease, box-shadow .18s ease, border-color .18s ease, background-color .18s ease;
        }
        .lit-center-item:hover {
            transform: translateY(-4px);
            border-color: #cfe0f3;
            background: #ffffff;
            box-shadow: 0 16px 34px rgba(28, 56, 88, 0.1);
        }
        .lit-center-item h5 {
            margin: 0 0 8px;
            font-size: 18px;
            color: #1d2f42;
            line-height: 1.6;
        }
        .lit-center-item p {
            margin: 0;
            color: #6c7c8c;
            line-height: 1.8;
        }
        .lit-center-empty {
            padding: 36px 20px;
            border: 1px dashed #d9e3ed;
            border-radius: 16px;
            background: #fbfdff;
            color: #7a8795;
            text-align: center;
        }
        @media (max-width: 1100px) {
            .lit-center-grid { grid-template-columns: repeat(2, minmax(0, 1fr)); }
            .lit-center-header { flex-direction: column; align-items: flex-start; }
        }
        @media (max-width: 720px) {
            .lit-center-grid { grid-template-columns: 1fr; }
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
                        <div class="lit-center-header">
                            <div class="lit-center-user">
                                <div class="lit-center-avatar"><img src="<%=(CommonUserFunc.GetUserAvatarFunc(user_list.upload_pic_avatar)) %>" /></div>
                                <div>
                                    <h4><%=Function.HtmlDiscode(user_list.name) %></h4>
                                    <p>&#36825;&#37324;&#38598;&#20013;&#23637;&#31034;&#20320;&#30340;&#25991;&#29486;&#25237;&#31295;&#12289;&#23457;&#26680;&#29366;&#24577;&#21644;&#31215;&#20998;&#27010;&#20917;&#12290;</p>
                                </div>
                            </div>
                            <div class="lit-center-actions">
                                <a class="lit-center-btn" href="/User/LiteratureUpload">&#25552;&#20132;&#25991;&#29486;</a>
                                <a class="lit-center-btn secondary" href="/User/IntegrateExchange">&#26597;&#30475;&#31215;&#20998;</a>
                            </div>
                        </div>

                        <div class="lit-center-grid">
                            <div class="lit-center-stat"><strong><%=TotalLiteratureCount %></strong><span>&#25105;&#30340;&#25237;&#31295;</span></div>
                            <div class="lit-center-stat"><strong><%=PendingLiteratureCount %></strong><span>&#24453;&#23457;&#26680;</span></div>
                            <div class="lit-center-stat"><strong><%=ApprovedLiteratureCount %></strong><span>&#24050;&#36890;&#36807;</span></div>
                            <div class="lit-center-stat"><strong><%=CommonUserFunc.GetUserIntegrateSumFunc(user_list.id,0) %></strong><span>&#24403;&#21069;&#31215;&#20998;</span></div>
                        </div>

                        <div class="lit-center-panel">
                            <h4>&#24453;&#23457;&#26680;&#25991;&#29486;</h4>
                            <div class="lit-center-note">&#36825;&#37324;&#26174;&#31034;&#20320;&#24050;&#25552;&#20132;&#12289;&#27491;&#22312;&#31561;&#24453;&#31649;&#29702;&#21592;&#23457;&#26680;&#30340;&#25991;&#29486;&#12290;&#23457;&#26680;&#36890;&#36807;&#21069;&#20165;&#20320;&#26412;&#20154;&#21487;&#26597;&#30475;&#35814;&#24773;&#65292;&#26242;&#19981;&#24320;&#25918;&#19979;&#36733;&#12290;</div>
                            <% if (!string.IsNullOrWhiteSpace(PendingLiteratureHtml)) { %>
                            <div class="lit-center-list"><%=PendingLiteratureHtml %></div>
                            <% } else { %>
                            <div class="lit-center-empty">&#24403;&#21069;&#27809;&#26377;&#24453;&#23457;&#26680;&#30340;&#25991;&#29486;&#12290;</div>
                            <% } %>
                        </div>

                        <div class="lit-center-panel">
                            <h4>&#26368;&#36817;&#25237;&#31295;</h4>
                            <div class="lit-center-note">&#20320;&#25552;&#20132;&#30340;&#25991;&#29486;&#38656;&#35201;&#21518;&#21488;&#23457;&#26680;&#36890;&#36807;&#21518;&#65292;&#25165;&#20250;&#22312;&#39318;&#39029;&#21644;&#25991;&#29486;&#26816;&#32034;&#39029;&#23545;&#22806;&#23637;&#31034;&#12290;</div>
                            <% if (!string.IsNullOrWhiteSpace(RecentLiteratureHtml)) { %>
                            <div class="lit-center-list"><%=RecentLiteratureHtml %></div>
                            <% } else { %>
                            <div class="lit-center-empty">&#20320;&#36824;&#27809;&#26377;&#25552;&#20132;&#36807;&#25991;&#29486;&#65292;&#28857;&#20987;&#19978;&#26041;&#8220;&#25552;&#20132;&#25991;&#29486;&#8221;&#21363;&#21487;&#24320;&#22987;&#25237;&#31295;&#12290;</div>
                            <% } %>
                        </div>

                        <div class="lit-center-panel">
                            <h4>&#24555;&#25463;&#20837;&#21475;</h4>
                            <div class="lit-center-list">
                                <a class="lit-center-item" href="/User/LiteratureUpload">
                                    <h5>&#32487;&#32493;&#25552;&#20132;&#25991;&#29486;</h5>
                                    <p>&#19978;&#20256; PDF&#65292;&#33258;&#21160;&#35299;&#26512;&#26631;&#39064;&#12289;&#20316;&#32773;&#12289;&#21333;&#20301;&#31561;&#20449;&#24687;&#65292;&#24182;&#25552;&#20132;&#21518;&#21488;&#23457;&#26680;&#12290;</p>
                                </a>
                                <a class="lit-center-item" href="/LiteratureSearch.aspx">
                                    <h5>&#36827;&#20837;&#25991;&#29486;&#26816;&#32034;</h5>
                                    <p>&#36890;&#36807;&#20851;&#38190;&#35789;&#12289;&#20316;&#32773;&#12289;DOI&#12289;&#26399;&#21002;&#25110;&#24180;&#20221;&#24555;&#36895;&#26816;&#32034;&#24179;&#21488;&#20869;&#20844;&#24320;&#25991;&#29486;&#12290;</p>
                                </a>
                                <a class="lit-center-item" href="/User/NoticeLog">
                                    <h5>&#26597;&#30475;&#31995;&#32479;&#36890;&#30693;</h5>
                                    <p>&#22312;&#36825;&#37324;&#26597;&#30475;&#23457;&#26680;&#32467;&#26524;&#12289;&#31215;&#20998;&#21464;&#26356;&#21644;&#25991;&#29486;&#22788;&#29702;&#25552;&#37266;&#12290;</p>
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>
    <LiteratureManager:foot ID="foot" runat="server" />
</body>
</html>
