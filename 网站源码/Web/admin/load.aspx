<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="load.aspx.cs" Inherits="Web.admin.load" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<style type="text/css">
.action{color:#3366cc;text-decoration:none;font-family:"微软雅黑","宋体";}
.overlay{position:fixed;top:0;right:0;bottom:0;left:0;z-index:998;width:100%;height:100%;_padding:0 20px 0 0;background:#f6f4f5;display:none;}
.showbox{position:fixed;top:0;left:50%;z-index:9999;margin-left:-80px; margin-top:150px;}
.showbox,.overlay{position:absolute;top:expression(eval(document.documentElement.scrollTop));}
#AjaxLoading{border:1px solid #8CBEDA;color:#37a;font-size:12px;font-weight:bold;}
#AjaxLoading div.loadingWord{width:180px;height:50px;line-height:50px;border:2px solid #D6E7F2;background:#fff;}
#AjaxLoading img{margin:10px 15px;float:left;display:inline;}
</style>
<script language="javascript" type="text/javascript" src="../Inc/jquery.min.js"></script>
<script type="text/javascript">
    $(document).ready(function () {
        window.location = ('<%=Request.QueryString["url"] %>');
    });
</script>
<body>
<div class="showbox" id="AjaxLoading">
    <div class="loadingWord"><img src="images/waiting.gif" alt="" />加载中，请稍候...</div>
</div>
</body>
</html>
