<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceLogInfo.aspx.cs" Inherits="Web.UserCenter.ServiceLogInfo" %>

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
    <title>反馈记录-问题反馈-个人中心</title>
    <LiteratureManager:css ID="css" runat="server" />
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
                            <h4>反馈记录</h4>
                        </div>
                        <asp:Panel ID="Main" runat="server">
                            <div class="record-list">
                                <h4><%=Function.HtmlDiscode(ServiceLog_List.name) %></h4>
                                <%if (!string.IsNullOrWhiteSpace(ServiceLog_List.info_))
                                    {  %>
                                <div class="record-item">
                                    <div class="record-img">
                                        <img src="<%=upload_pic_avatar %>" />
                                    </div>
                                    <div class="record-text">
                                        <h4><%=user_name %> <span><%=ServiceLog_List.addtime.ToString("yyyy-MM-dd HH:mm:ss") %></span></h4>
                                        <div><%=Function.Replace_Content(ServiceLog_List.info_) %></div>
                                    </div>
                                </div>
                                <%} %>
                                <asp:Repeater ID="DataList" runat="server">
                                    <ItemTemplate>
                                        <div class="record-item">
                                            <div class="record-img">
                                                <img src="<%#(Eval("type").ToString()=="1"?upload_pic_avatar:(Eval("type").ToString()=="2"?"/images/kefu.jpg":"null")) %>" />
                                            </div>
                                            <div class="record-text">
                                                <h4><%#(Eval("type").ToString()=="1"?Function.HtmlDiscode(user_name):(Eval("type").ToString()=="2"?"客服":"null")) %> <span><%#Function.ConvertTo<DateTime>(Eval("addtime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss") %></span></h4>
                                                <div><%#Function.Replace_Content(Eval("info_").ToString()) %></div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                            <div class="record-form">
                                <%=Function.KindEditor("info_", 1)%>
                                <form class="layui-form" runat="server">
                                    <div class="record-extare">
                                        <asp:TextBox ID="info_" placeholder="继续补充反馈内容" TextMode="MultiLine" runat="server" Style="visibility: hidden;"></asp:TextBox>
                                    </div>
                                    <div class="record-but">
                                        <asp:Button ID="Button3" Text="发布" CssClass="record-Button" runat="server" OnClick="OnClick_AddUp" />
                                    </div>
                                </form>
                            </div>
                        </asp:Panel>
                        <div id="ok_html"></div>
                    </div>
                </div>

            </div>
        </section>
    </div>
    <LiteratureManager:foot ID="foot" runat="server" />
</body>

</html>
