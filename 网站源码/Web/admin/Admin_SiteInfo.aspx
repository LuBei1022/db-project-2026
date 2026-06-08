<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_SiteInfo.aspx.cs" Inherits="Web.admin.Admin_SiteInfo" %>
<%@ Register TagPrefix="LiteratureManager" TagName="Jsmenu" Src="Jsmenu.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="Inc" Src="Inc.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="class_menu" Src="class_menu.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<LiteratureManager:Inc ID="Inc1" runat="server" />
      <script type="text/javascript" src="js/jquery.ztree.core.min.js"></script>
    <link rel="stylesheet" href="css/zTreeStyle.css" type="text/css" />
    <LiteratureManager:Jsmenu ID="jsmenu1" runat="server" />
</head>
<body style="margin: 0px" scroll="no">
<%if (isLoading)
    {  %>
<LiteratureManager:class_menu ID="class_menu" runat="server" />
<form id="form2" runat="server" style="margin-block-end: 1em;">
<!--begin::App Content-->
            <!--begin::Container-->
            <table cellpadding="0" cellspacing="0" width="100%" height="100%">
  <tr>
    <td valign="top" id="tree" class="ztree"></td>
    <td valign="top" width="100%"  style="height: calc(100vh - 57px - 65px);">
	<iframe src="Admin_Null.aspx" id="menu_info" name="menu_info" onload="" width="100%" height="100%" frameborder="0" scrolling="yes" style="overflow: visible;"></iframe>
	</td>
  </tr>
</table>
            <!--end::Container-->
<script type="text/JavaScript">
    $.ajaxSetup({ cache: false });
    $("#leftmenu").load("Left.aspx?menuid=43");
    function LoadMenu(url, id) {
        $('#topmenu li').removeClass("navon");
        $('#' + id).addClass("navon");
        if (url == "") {
            parent.main.location = "Right.aspx?MenuID=" + id + "&act=default_page";
        }
        else {
            parent.main.location = "load.aspx?url=" + escape(url);
            //parent.main.location = url;
        }
        $("#leftmenu").load("Left.aspx?menuid=" + id + "");
    };
</script>
<!--end::App Content-->
    </form>
<%} %>
    </body>
</html>
