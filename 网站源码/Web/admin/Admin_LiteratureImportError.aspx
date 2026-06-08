<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_LiteratureImportError.aspx.cs" Inherits="Web.admin.Admin_LiteratureImportError" %>
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
            <asp:Panel ID="Main" runat="server">
                <div class="container-fluid">
                    <div class="card mb-12">
                        <div class="card-header">
                            <div class="card-title">&#23548;&#20837;&#38169;&#35823;&#26126;&#32454;</div>
                        </div>
                        <div class="card-body p-0">
                            <table class="table table-sm">
                                <thead>
                                    <tr>
                                        <th>&#34892;&#21495;</th>
                                        <th>&#26631;&#39064;</th>
                                        <th>&#38169;&#35823;&#21407;&#22240;</th>
                                        <th>&#21407;&#22987;&#25968;&#25454;</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <tr class="hover">
                                                <td><%# Eval("row_no") %></td>
                                                <td><%# Function.HtmlDiscodeWeb(Eval("title").ToString()) %></td>
                                                <td><%# Function.HtmlDiscodeWeb(Eval("error_msg").ToString()) %></td>
                                                <td style="max-width: 460px; word-break: break-all;"><%# Function.HtmlDiscodeWeb(Eval("raw_data").ToString()) %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <asp:Panel ID="DivNull" runat="server" Visible="true">
                                        <tr>
                                            <td colspan="4" style="text-align: center;">&#26080;&#38169;&#35823;&#35760;&#24405;!</td>
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
            </asp:Panel>
        </div>
    </form>
    <% } %>
</body>
</html>
