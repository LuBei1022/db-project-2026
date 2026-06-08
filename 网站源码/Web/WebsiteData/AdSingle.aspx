<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdSingle.aspx.cs" Inherits="Web.WebsiteData.AdSingle" %>

<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<!DOCTYPE html>
<html lang="en-US">

<head>
   <meta charset="UTF-8">
   <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
   <meta name='robots' content='index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1' />
   <title><%=CommonFunc.GetTitle("", indexsingle_list.name) %></title>
    <meta name="description" content="<%=CommonFunc.GetDescription(indexsingle_list.description) %>" />
    <LiteratureManager:css ID="css" runat="server" />
</head>

<body class="ac">
  <LiteratureManager:top ID="top" runat="server" />
   <div class="middle">
        <section class="imageText">
            <div class="w1200" <%=!string.IsNullOrWhiteSpace(banner)?" style=\"background-image: url("+banner+");\"":"" %>>
               <h4><%=Function.HtmlDiscode(indexsingle_list.name) %></h4>
            </div>
      </section>
        <section class="imageText-con">
            <div class="w1200">
               <%=Function.Replace_Content(indexsingle_list.info_) %>
            </div>
      </section>

   </div>
   <LiteratureManager:foot ID="foot" runat="server" />
</body>

</html>
