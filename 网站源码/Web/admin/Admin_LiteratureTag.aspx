<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_LiteratureTag.aspx.cs" Inherits="Web.admin.Admin_LiteratureTag" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
</head>
<body>
<%@ Register TagPrefix="LiteratureManager" TagName="Inc" Src="Inc.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="class_menu" Src="class_menu.ascx" %>
<% if (isLoading) { %>
<LiteratureManager:Inc ID="Inc2" runat="server" />
<LiteratureManager:class_menu ID="class_menu" runat="server" />

<form id="form2" runat="server">
    <div class="app-content">
        <asp:Panel ID="AddUp" runat="server" Visible="false">
            <div class="container-fluid">
                <div class="row">
                    <div class="col-md-6 offset-md-3">
                        <div class="card card-primary card-outline mb-4">
                            <div class="card-header">
                                <div class="card-title">
                                    <asp:Label ID="Txt_Title" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="mb-6">
                                    <label class="form-label">&#26631;&#31614;&#21517;&#31216;<span>*</span></label>
                                    <asp:TextBox ID="name" runat="server" CssClass="txt form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-6">
                                        <label class="form-label">&#25490;&#24207;</label>
                                        <asp:TextBox ID="orderid" runat="server" CssClass="txt form-control" Text="0"></asp:TextBox>
                                    </div>
                                    <div class="col-md-6">
                                        <label class="form-label">&#29366;&#24577;</label>
                                        <asp:DropDownList ID="status" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="1">&#21551;&#29992;</asp:ListItem>
                                            <asp:ListItem Value="0">&#20572;&#29992;</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="card-footer">
                                <asp:Button ID="Button3" Text=" &#20445; &#23384; " CssClass="btn btn-primary" runat="server" OnClick="OnClick_AddUp" />
                                <input type="button" name="button" id="button" value=" &#36820; &#22238; " class="btn submit-but" onclick="history.go(-1)">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="Main" runat="server">
            <div class="container-fluid">
                <div class="col-md-12">
                    <div class="card mb-12">
                        <div class="card-header cardList">
                            <div class="cardItem">
                                <label class="col-form-label">&#26631;&#31614;&#21517;&#31216;</label>
                                <div class="col-form-input">
                                    <asp:TextBox ID="SearchKeyWords" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="cardItem">
                                <asp:Button ID="Button2" runat="server" OnClick="OnClick_Search" Text="&#25628;&#32034;" CssClass="btn btn-success" />
                            </div>
                            <div class="cardItem" style="float: right;">
                                <a href="?Action=Add&MenuId=<%=MenuId %>&BackURL=<%=Function.GetEncodeURL()%>" class="btn btn-primary">&#28155;&#21152;</a>
                            </div>
                        </div>
                        <div class="card-body p-0">
                            <table class="table table-sm">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>&#26631;&#31614;&#21517;&#31216;</th>
                                        <th>&#25490;&#24207;</th>
                                        <th>&#29366;&#24577;</th>
                                        <th>&#21019;&#24314;&#26102;&#38388;</th>
                                        <th>&#25805;&#20316;</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <tr class="hover">
                                                <td><%# Eval("id") %></td>
                                                <td><%# Function.HtmlDiscodeWeb(Eval("name").ToString()) %></td>
                                                <td><%# Eval("orderid") %></td>
                                                <td><%# GetStatusText(Eval("status")) %></td>
                                                <td><%# Function.ConvertTo<DateTime>(Eval("addtime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd HH:mm") %></td>
                                                <td>
                                                    <a class="badge text-bg-success" href='?Action=Edit&MenuId=<%#MenuId %>&ID=<%#Eval("id")%>&BackURL=<%=Function.GetEncodeURL() %>'>&#32534;&#36753;</a>
                                                    <a class="badge text-bg-danger" data-href='?Action=Del&MenuId=<%#MenuId %>&ID=<%#Eval("id")%>&BackURL=<%=Function.GetEncodeURL() %>' onclick="DataDelFunc(this)">&#21024;&#38500;</a>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <asp:Panel ID="DivNull" runat="server" Visible="true">
                                        <tr>
                                            <td colspan="6" style="text-align: center;">&#26080;&#30456;&#20851;&#25968;&#25454;!</td>
                                        </tr>
                                    </asp:Panel>
                                </tbody>
                            </table>
                            <div class="msdn">
                                <div></div>
                                <Webdiyer:AspNetPager ID="AspNetPager1" runat="server" CurrentPageButtonClass="current" FirstPageText="Home" PrevPageText="Prev" NextPageText="Next" LastPageText="End"
                                    ShowDisabledButtons="true" OnPageChanged="AspNetPager1_PageChanged" UrlPaging="true" PageIndexBoxClass="input_page" PageIndexBoxType="TextBox" SubmitButtonClass="go" SubmitButtonText="GO" ShowPageIndexBox="Always">
                                </Webdiyer:AspNetPager>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</form>
<% } %>
</body>
</html>
