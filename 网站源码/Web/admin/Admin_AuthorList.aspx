<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_AuthorList.aspx.cs" Inherits="Web.admin.Admin_AuthorList" CodePage="65001" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <title>作者管理</title>
    <style type="text/css">
        .author-title { display:flex; align-items:flex-end; justify-content:space-between; gap:18px; margin-bottom:18px; }
        .author-title h1 { margin:0; font-size:30px; font-weight:800; color:#1d1d1f; }
        .author-title p { margin:6px 0 0; color:#6e6e73; }
        .author-toolbar { display:flex; gap:12px; align-items:center; flex-wrap:wrap; padding:16px; border-bottom:1px solid #edf2f7; }
        .author-toolbar input[type=text] { width:360px; max-width:100%; min-height:38px; border:1px solid #d7e0ea; border-radius:8px; padding:6px 12px; }
        .author-table th, .author-table td { vertical-align:top; padding:12px; }
        .author-main { font-weight:700; color:#111827; }
        .author-sub { color:#6b7280; font-size:13px; line-height:1.7; margin-top:4px; }
        .author-pill { display:inline-flex; min-height:24px; align-items:center; padding:0 8px; border-radius:999px; background:#eef4fb; color:#1f344d; font-size:12px; margin:0 4px 4px 0; }
        .author-actions { display:flex; flex-wrap:wrap; gap:7px; }
        .author-actions a { display:inline-flex; align-items:center; min-height:28px; padding:0 10px; border-radius:8px; background:#eef4fb; border:1px solid #c8d6e5; color:#1f344d; text-decoration:none; }
        .author-actions a:hover { background:#dfeaf6; color:#10243a; text-decoration:none; }
        .author-empty { text-align:center; color:#6b7280; padding:34px; }
        .author-page { padding:14px 16px; display:flex; gap:8px; align-items:center; justify-content:flex-end; }
        .author-page a, .author-page span { min-width:34px; height:34px; line-height:34px; text-align:center; border-radius:8px; border:1px solid #d7e0ea; color:#1f344d; text-decoration:none; }
        .author-page span { background:#0066cc; color:#fff; border-color:#0066cc; }
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
                    <h1>作者管理</h1>
                    <p>统一维护作者基础信息、机构历史和论文关联关系。</p>
                </div>
                <div class="author-actions">
                    <a href="Admin_AuthorEdit.aspx?Action=Add&MenuId=<%=MenuId %>">新增作者</a>
                </div>
            </div>
            <div class="card mb-12">
                <form method="get" action="Admin_AuthorList.aspx">
                    <input type="hidden" name="MenuId" value="<%=Server.HtmlEncode(MenuId) %>" />
                    <div class="author-toolbar">
                        <label>关键词</label>
                        <input type="text" name="Key" value="<%=Server.HtmlEncode(Key) %>" placeholder="作者姓名、机构、论文标题" />
                        <button type="submit" class="btn btn-primary">搜索</button>
                        <a class="btn btn-secondary" href="Admin_AuthorList.aspx?MenuId=<%=MenuId %>">重置</a>
                    </div>
                </form>
                <div class="card-body p-0">
                    <table class="table table-sm author-table">
                        <thead>
                            <tr>
                                <th style="width:70px;">ID</th>
                                <th>作者信息</th>
                                <th style="width:28%;">机构概览</th>
                                <th style="width:110px;">论文数</th>
                                <th style="width:150px;">创建时间</th>
                                <th style="width:160px;">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <%=ListHtml %>
                        </tbody>
                    </table>
                    <%=PagerHtml %>
                </div>
            </div>
        </div>
    </div>
    <% } %>
</body>
</html>
