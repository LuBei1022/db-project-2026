<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Right.aspx.cs" Inherits="Web.admin.Right" %>
<%@ Register TagPrefix="LiteratureManager" TagName="Inc" Src="Inc.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="x-ua-compatible" content="ie=7" />
    <title>设置栏目默认页</title>
    <LiteratureManager:Inc ID="Inc1" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="container" id="cpcontainer">
    <asp:Panel ID="Default_Page" runat="server" Visible="false">
<asp:Repeater id="myRepeater" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
<ItemTemplate> 
<table width="100%" border="0" cellpadding="0" cellspacing="0" class="tb tb2 fixpadding">
  <tr class="title">
    <td width="90%" align="left"><%#Eval("popedom_name")%><%#GetIsHead(Eval("id").ToString())%></td>
    <td width="10%" align="left">&nbsp;</td>
  </tr>
<asp:Repeater id="myRepeater2" runat="server">
<ItemTemplate>
  <tr class="hover">
    	<td align="left"><%#Eval("popedom_name")%></td>
		<td align="center"><%#GetIsSet(DataBinder.Eval((Container.NamingContainer.NamingContainer as RepeaterItem).DataItem, "id").ToString(), Eval("id").ToString())%></td>
  </tr>
</ItemTemplate>
</asp:Repeater>

</table>
</ItemTemplate>
</asp:Repeater>
    </asp:Panel>
   
    </div>
    </form>
</body>
</html>
