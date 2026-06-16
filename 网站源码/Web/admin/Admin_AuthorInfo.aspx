<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_AuthorInfo.aspx.cs" Inherits="Web.admin.Admin_AuthorInfo" CodePage="65001" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <title>作者详情</title>
    <style type="text/css">
        .author-title { display:flex; align-items:flex-end; justify-content:space-between; gap:18px; margin-bottom:18px; }
        .author-title h1 { margin:0; font-size:30px; font-weight:800; color:#1d1d1f; }
        .author-title p { margin:6px 0 0; color:#6e6e73; }
        .author-actions { display:flex; flex-wrap:wrap; gap:8px; }
        .author-actions a { display:inline-flex; align-items:center; min-height:32px; padding:0 12px; border-radius:8px; background:#eef4fb; border:1px solid #c8d6e5; color:#1f344d; text-decoration:none; }
        .author-section { margin-bottom:18px; }
        .author-section h3 { margin:0; padding:16px 18px; border-bottom:1px solid #edf2f7; font-size:18px; color:#111827; }
        .author-grid { display:grid; grid-template-columns:repeat(2,minmax(0,1fr)); gap:12px; padding:18px; }
        .author-field { padding:14px; border:1px solid #edf2f7; border-radius:12px; background:#fbfbfd; }
        .author-field span { display:block; color:#6b7280; font-size:13px; margin-bottom:6px; }
        .author-field strong { color:#111827; }
        .author-pill { display:inline-flex; min-height:28px; align-items:center; padding:0 10px; border-radius:999px; background:#eef4fb; color:#1f344d; margin:0 6px 6px 0; }
        .author-history, .author-paper-group { padding:0 18px 18px; }
        .author-history table, .author-paper-group table { width:100%; }
        .author-history th, .author-history td, .author-paper-group th, .author-paper-group td { padding:10px; border-bottom:1px solid #edf2f7; vertical-align:top; }
        .author-paper-group h4 { margin:18px 0 10px; color:#111827; }
        .author-paper-title { color:#0066cc; font-weight:700; text-decoration:none; }
        .author-paper-title:hover { text-decoration:underline; }
        .author-empty { color:#6b7280; padding:18px; }
    </style>
</head>
<body>
    <%@ Register TagPrefix="LiteratureManager" TagName="Inc" Src="Inc.ascx" %>
    <%@ Register TagPrefix="LiteratureManager" TagName="class_menu" Src="class_menu.ascx" %>
    <% if (isLoading) { %>
    <LiteratureManager:Inc ID="Inc2" runat="server" />
    <LiteratureManager:class_menu ID="class_menu" runat="server" />
    <div class="app-content">
        <div class="container-fluid">
            <div class="author-title">
                <div>
                    <h1><%=Server.HtmlEncode(AuthorName) %></h1>
                    <p>作者详情、机构历史与论文关联。</p>
                </div>
                <div class="author-actions">
                    <a href="Admin_AuthorEdit.aspx?Action=Edit&ID=<%=AuthorId %>&MenuId=<%=Server.UrlEncode(MenuId) %>">编辑作者</a>
                    <a href="Admin_AuthorList.aspx?MenuId=<%=Server.UrlEncode(MenuId) %>">返回列表</a>
                </div>
            </div>
            <div class="card author-section">
                <h3>基本信息</h3>
                <div class="author-grid">
                    <%=BasicHtml %>
                </div>
            </div>
            <div class="card author-section">
                <h3>当前所属机构</h3>
                <div style="padding:18px;"><%=CurrentInstitutionHtml %></div>
            </div>
            <div class="card author-section">
                <h3>历史所属机构</h3>
                <div class="author-history"><%=HistoryHtml %></div>
            </div>
            <div class="card author-section">
                <h3>论文关联关系</h3>
                <div class="author-paper-group"><%=PaperHtml %></div>
            </div>
        </div>
    </div>
    <% } %>
</body>
</html>
