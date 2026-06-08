<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_LiteratureVenueProfile.aspx.cs" Inherits="Web.admin.Admin_LiteratureVenueProfile" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server"></head>
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
                    <div class="col-md-10 offset-md-1">
                        <div class="card card-primary card-outline mb-4">
                            <div class="card-header"><div class="card-title"><asp:Label ID="Txt_Title" runat="server"></asp:Label></div></div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-4">
                                        <label class="form-label">&#31867;&#22411;</label>
                                        <asp:DropDownList ID="venue_type" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="journal">&#26399;&#21002;</asp:ListItem>
                                            <asp:ListItem Value="conference">&#20250;&#35758;</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-md-8">
                                        <label class="form-label">&#21517;&#31216;<span>*</span></label>
                                        <asp:TextBox ID="venue_name" runat="server" CssClass="txt form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="mt-3">
                                    <label class="form-label">&#31616;&#20171;</label>
                                    <asp:TextBox ID="introduction" runat="server" TextMode="MultiLine" CssClass="txt form-control" Style="height:120px;"></asp:TextBox>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-md-3"><label class="form-label">&#24433;&#21709;/&#24341;&#29992;&#22240;&#23376;</label><asp:TextBox ID="impact_factor" runat="server" CssClass="txt form-control"></asp:TextBox></div>
                                    <div class="col-md-3"><label class="form-label">&#20998;&#21306;/&#31561;&#32423;</label><asp:TextBox ID="jcr_quartile" runat="server" CssClass="txt form-control"></asp:TextBox></div>
                                    <div class="col-md-3"><label class="form-label">ISSN</label><asp:TextBox ID="issn" runat="server" CssClass="txt form-control"></asp:TextBox></div>
                                    <div class="col-md-3"><label class="form-label">&#20250;&#35758;&#31561;&#32423;</label><asp:TextBox ID="conference_level" runat="server" CssClass="txt form-control"></asp:TextBox></div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-md-3"><label class="form-label">&#20250;&#35758;&#21608;&#26399;</label><asp:TextBox ID="conference_cycle" runat="server" CssClass="txt form-control"></asp:TextBox></div>
                                    <div class="col-md-3"><label class="form-label">&#22320;&#28857;</label><asp:TextBox ID="location" runat="server" CssClass="txt form-control"></asp:TextBox></div>
                                    <div class="col-md-3"><label class="form-label">&#23448;&#32593;</label><asp:TextBox ID="website_url" runat="server" CssClass="txt form-control"></asp:TextBox></div>
                                    <div class="col-md-3"><label class="form-label">&#20986;&#29256;/&#20027;&#21150;&#26041;</label><asp:TextBox ID="publisher" runat="server" CssClass="txt form-control"></asp:TextBox></div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-md-9"><label class="form-label">&#22791;&#27880;</label><asp:TextBox ID="remark" runat="server" CssClass="txt form-control"></asp:TextBox></div>
                                    <div class="col-md-3">
                                        <label class="form-label">&#29366;&#24577;</label>
                                        <asp:DropDownList ID="status" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="1">&#24050;&#32500;&#25252;</asp:ListItem>
                                            <asp:ListItem Value="0">&#24453;&#32500;&#25252;</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="card-footer">
                                <asp:Button ID="Button3" Text=" &#20445; &#23384; " CssClass="btn btn-primary" runat="server" OnClick="OnClick_AddUp" />
                                <input type="button" value=" &#36820; &#22238; " class="btn submit-but" onclick="history.go(-1)">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="Main" runat="server">
            <div class="container-fluid">
                <div class="card mb-12">
                    <div class="card-header cardList">
                        <div class="cardItem"><label class="col-form-label">&#21517;&#31216;</label><div class="col-form-input"><asp:TextBox ID="SearchKeyWords" runat="server" CssClass="form-control"></asp:TextBox></div></div>
                        <div class="cardItem"><label class="col-form-label">&#31867;&#22411;</label><div class="col-form-input"><asp:DropDownList ID="SearchType" runat="server" CssClass="form-control"><asp:ListItem Value="">&#20840;&#37096;</asp:ListItem><asp:ListItem Value="journal">&#26399;&#21002;</asp:ListItem><asp:ListItem Value="conference">&#20250;&#35758;</asp:ListItem></asp:DropDownList></div></div>
                        <div class="cardItem"><label class="col-form-label">&#29366;&#24577;</label><div class="col-form-input"><asp:DropDownList ID="SearchStatus" runat="server" CssClass="form-control"><asp:ListItem Value="">&#20840;&#37096;</asp:ListItem><asp:ListItem Value="0">&#24453;&#32500;&#25252;</asp:ListItem><asp:ListItem Value="1">&#24050;&#32500;&#25252;</asp:ListItem></asp:DropDownList></div></div>
                        <div class="cardItem"><asp:Button ID="Button2" runat="server" OnClick="OnClick_Search" Text="&#25628;&#32034;" CssClass="btn btn-success" /></div>
                        <div class="cardItem" style="float:right;"><a href="?Action=Add&MenuId=<%=MenuId %>&BackURL=<%=Function.GetEncodeURL()%>" class="btn btn-primary">&#28155;&#21152;</a></div>
                    </div>
                    <div class="card-body p-0">
                        <table class="table table-sm">
                            <thead><tr><th>ID</th><th>&#31867;&#22411;</th><th>&#21517;&#31216;</th><th>&#24433;&#21709;/&#31561;&#32423;</th><th>&#20998;&#21306;/ISSN</th><th>&#29366;&#24577;</th><th>&#26356;&#26032;&#26102;&#38388;</th><th>&#25805;&#20316;</th></tr></thead>
                            <tbody>
                                <asp:Repeater ID="Repeater1" runat="server">
                                    <ItemTemplate>
                                        <tr class="hover">
                                            <td><%# Eval("id") %></td>
                                            <td><%# GetTypeText(Eval("venue_type")) %></td>
                                            <td><%# Function.HtmlDiscodeWeb(Eval("venue_name").ToString()) %></td>
                                            <td><%# GetImpactText(Eval("impact_factor"), Eval("conference_level")) %></td>
                                            <td><%# GetQuartileText(Eval("jcr_quartile"), Eval("issn")) %></td>
                                            <td><%# GetStatusText(Eval("status")) %></td>
                                            <td><%# Function.ConvertTo<DateTime>(Eval("updatetime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd HH:mm") %></td>
                                            <td><a class="badge text-bg-success" href='?Action=Edit&MenuId=<%#MenuId %>&ID=<%#Eval("id")%>&BackURL=<%=Function.GetEncodeURL() %>'>&#32534;&#36753;</a></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <asp:Panel ID="DivNull" runat="server" Visible="true"><tr><td colspan="8" style="text-align:center;">&#26080;&#30456;&#20851;&#25968;&#25454;!</td></tr></asp:Panel>
                            </tbody>
                        </table>
                        <div class="msdn"><div></div><Webdiyer:AspNetPager ID="AspNetPager1" runat="server" CurrentPageButtonClass="current" FirstPageText="Home" PrevPageText="Prev" NextPageText="Next" LastPageText="End" ShowDisabledButtons="true" OnPageChanged="AspNetPager1_PageChanged" UrlPaging="true" PageIndexBoxClass="input_page" PageIndexBoxType="TextBox" SubmitButtonClass="go" SubmitButtonText="GO" ShowPageIndexBox="Always"></Webdiyer:AspNetPager></div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</form>
<% } %>
</body>
</html>
