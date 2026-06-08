<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="err.aspx.cs" Inherits="Web.err" %>

<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<!DOCTYPE html>
<html lang="en-US">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta name='robots' content='index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1' />
    <title>抱歉，沒有找到您请求的页面</title>
    <LiteratureManager:css ID="css" runat="server" />
    <style>
body { background: #a8a9a9; font-family: 'Arial','Microsoft Yahei'; }
p { margin: 0; color: #fff; line-height: 30px; text-align: center; font-size: 18px; }
.lose-en { font-size: 14px; }
.lose-main { background: url("/images/lose-bg.png") 0 0 no-repeat; background-size: 100%; min-height: 540px; }
.lose-box { width: 440px; height: 450px; position: absolute; left: 50%; top: 50%; margin: -225px 0 0 -220px; }
.lose-box img { margin-bottom: 50px; }
.lose-box a { width: 218px; height: 50px; display: block; margin: 50px auto 0 auto; border: solid 1px #fff; border-radius: 3px; text-align: center; line-height: 50px; color: #fff; text-decoration: none; transition: all ease-out .4s; }
.lose-box a:hover { background: #fff; color: #004f67; border-color: transparent; }
</style>
</head>

<body class="ac">
    <LiteratureManager:top ID="top" runat="server" />
    <div class="middle">
       <div class="lose-main">
		<div class="lose-box">
			<img src="/images/pic1.png">
			<p>Sorry, didn't find the page you requet.</p>
			<p class="lose-en">抱歉，沒有找到您请求的页面</p>
			<a href="/">返回主页</a>
		</div>
	</div>
    </div>
    <LiteratureManager:foot ID="foot" runat="server" />
</body>

</html>
