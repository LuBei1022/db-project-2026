<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_AuthorEdit.aspx.cs" Inherits="Web.admin.Admin_AuthorEdit" CodePage="65001" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <title>编辑作者</title>
    <style type="text/css">
        .author-edit-wrap { max-width:920px; margin:0 auto; }
        .author-edit-title { margin-bottom:18px; }
        .author-edit-title h1 { margin:0; font-size:30px; font-weight:800; color:#1d1d1f; }
        .author-edit-title p { margin:6px 0 0; color:#6e6e73; }
        .author-form { padding:22px; }
        .author-grid { display:grid; grid-template-columns:repeat(2,minmax(0,1fr)); gap:16px; }
        .author-field { margin-bottom:16px; }
        .author-field label { display:block; margin-bottom:8px; color:#111827; font-weight:700; }
        .author-field input, .author-field select, .author-field textarea { width:100%; min-height:40px; border:1px solid #d7e0ea; border-radius:8px; padding:8px 12px; box-sizing:border-box; }
        .author-field textarea { min-height:150px; resize:vertical; }
        .author-hint { color:#6b7280; font-size:13px; margin-top:6px; line-height:1.7; }
        .author-actions { display:flex; gap:10px; padding:18px 22px; border-top:1px solid #edf2f7; }
        .author-actions a { display:inline-flex; align-items:center; min-height:38px; padding:0 14px; border-radius:8px; background:#eef4fb; border:1px solid #c8d6e5; color:#1f344d; text-decoration:none; }
        @media (max-width:760px){ .author-grid { grid-template-columns:1fr; } }
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
            <div class="author-edit-wrap">
                <div class="author-edit-title">
                    <h1><%=PageTitle %></h1>
                    <p>维护作者基础资料。当前机构由该作者最后发表论文中的机构自动计算，论文机构归属请在具体论文编辑页维护。</p>
                </div>
                <div class="card">
                    <form method="post" action="Admin_AuthorEdit.aspx?Action=<%=Server.UrlEncode(Action) %>&ID=<%=AuthorId %>&MenuId=<%=Server.UrlEncode(MenuId) %>">
                        <div class="author-form">
                            <div class="author-grid">
                                <div class="author-field">
                                    <label>中文名</label>
                                    <input type="text" name="name_cn" value="<%=Server.HtmlEncode(NameCn) %>" />
                                </div>
                                <div class="author-field">
                                    <label>英文名</label>
                                    <input type="text" name="name_en" value="<%=Server.HtmlEncode(NameEn) %>" />
                                </div>
                            </div>
                            <div class="author-field">
                                <label>当前所属机构</label>
                                <input type="text" value="<%=Server.HtmlEncode(CurrentInstitution) %>" readonly="readonly" />
                                <div class="author-hint">只读字段，根据该作者最后发表论文中的作者-机构关系自动计算。</div>
                                <%=CurrentInstitutionSourceHtml %>
                            </div>
                            <div class="author-grid">
                                <div class="author-field">
                                    <label>ORCID</label>
                                    <input type="text" name="orcid" value="<%=Server.HtmlEncode(Orcid) %>" />
                                </div>
                                <div class="author-field">
                                    <label>邮箱</label>
                                    <input type="text" name="email" value="<%=Server.HtmlEncode(Email) %>" />
                                </div>
                            </div>
                            <div class="author-field">
                                <label>作者身份确认状态</label>
                                <select name="identity_status">
                                    <option value="confirmed" <%=IdentityStatus == "confirmed" ? "selected=\"selected\"" : "" %>>已确认</option>
                                    <option value="unconfirmed" <%=IdentityStatus == "unconfirmed" || IdentityStatus == "auto" ? "selected=\"selected\"" : "" %>>待确认</option>
                                    <option value="merged" <%=IdentityStatus == "merged" ? "selected=\"selected\"" : "" %>>已合并</option>
                                    <option value="split_needed" <%=IdentityStatus == "split_needed" ? "selected=\"selected\"" : "" %>>需拆分</option>
                                </select>
                                <div class="author-hint">用于标记重名作者、误合并或误拆分的人工核对状态，不影响论文原始关联。</div>
                            </div>
                            <div class="author-field">
                                <label>状态</label>
                                <select name="status">
                                    <option value="1" <%=Status == 1 ? "selected=\"selected\"" : "" %>>启用</option>
                                    <option value="0" <%=Status == 0 ? "selected=\"selected\"" : "" %>>停用</option>
                                </select>
                            </div>
                        </div>
                        <div class="author-actions">
                            <button type="submit" class="btn btn-primary">保存</button>
                            <a href="<%=BackUrl %>">返回</a>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
    <% } %>
</body>
</html>
