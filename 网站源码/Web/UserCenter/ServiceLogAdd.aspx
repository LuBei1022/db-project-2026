<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceLogAdd.aspx.cs" Inherits="Web.UserCenter.ServiceLogAdd" %>

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
    <title>提交反馈-问题反馈-个人中心</title>
    <LiteratureManager:css ID="css" runat="server" />
    <style>
        .record-extare input {
            display: block;
            width: calc(100% - 40px);
            padding: 15px 20px;
            border: none;
            resize: none;
            background: #f3f3f3;
            border-radius: 10px;
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
                            <h4>提交反馈</h4>
                        </div>
                        <div class="record-form">
                            <asp:Panel ID="Main" runat="server">
                                <%=Function.KindEditor("info_", 1)%>
                                <form class="layui-form" runat="server">
                                    <div class="record-extare">
                                          <asp:TextBox ID="name" TextMode="SingleLine" runat="server" placeholder="反馈主题"></asp:TextBox>
                                    </div>
                                    <div class="record-extare">
                                          <asp:TextBox ID="info_" placeholder="请描述问题或建议" TextMode="MultiLine" runat="server" Style="visibility: hidden;"></asp:TextBox>
                                    </div>
                                    <div class="record-but">
                                        <asp:Button ID="Button3" Text="发布" CssClass="record-Button" runat="server" OnClick="OnClick_AddUp" OnClientClick="return OnClickAddUpFunc()" />
                                    </div>
                                </form>
                            </asp:Panel>
                            <div id="ok_html"></div>
                        </div>
                    </div>
                </div>
                <script>
                    function OnClickAddUpFunc() {
                        var isyes = true;
                        var Name_ = $("#name").val();
                        if (!(Name_ || "").trim()) {
                            isyes = false;
                            layer.alert('请填写反馈主题！', {
                                title: '提示'
                            }, function () {
                                layer.closeAll();
                            })
                        }
                        return isyes;
                    }
                </script>
            </div>
        </section>
    </div>
    <LiteratureManager:foot ID="foot" runat="server" />
</body>

</html>
