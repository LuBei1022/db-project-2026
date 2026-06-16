<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_InstitutionList.aspx.cs" Inherits="Web.admin.Admin_InstitutionList" CodePage="65001" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <title>&#26426;&#26500;&#31649;&#29702;</title>
    <style type="text/css">
        .master-title { display:flex; align-items:flex-end; justify-content:space-between; gap:18px; margin-bottom:18px; }
        .master-title h1 { margin:0; font-size:30px; font-weight:800; color:#1d1d1f; }
        .master-title p { margin:6px 0 0; color:#6e6e73; }
        .master-toolbar { display:flex; gap:12px; align-items:center; flex-wrap:wrap; padding:16px; border-bottom:1px solid #edf2f7; }
        .master-toolbar input[type=text] { width:360px; max-width:100%; min-height:38px; border:1px solid #d7e0ea; border-radius:8px; padding:6px 12px; }
        .master-table th, .master-table td { vertical-align:top; padding:12px; }
        .master-main { font-weight:700; color:#111827; }
        .master-sub { color:#6b7280; font-size:13px; line-height:1.7; margin-top:4px; }
        .master-actions { display:flex; flex-wrap:wrap; gap:7px; }
        .master-actions a { display:inline-flex; align-items:center; min-height:28px; padding:0 10px; border-radius:8px; background:#eef4fb; border:1px solid #c8d6e5; color:#1f344d; text-decoration:none; }
        .master-actions a:hover { background:#dfeaf6; color:#10243a; text-decoration:none; }
        .master-empty { text-align:center; color:#6b7280; padding:34px; }
        .master-page { padding:14px 16px; display:flex; gap:8px; align-items:center; justify-content:flex-end; }
        .master-page a, .master-page span { min-width:34px; height:34px; line-height:34px; text-align:center; border-radius:8px; border:1px solid #d7e0ea; color:#1f344d; text-decoration:none; }
        .master-page span { background:#0066cc; color:#fff; border-color:#0066cc; }
        .master-form { padding:22px; }
        .master-grid { display:grid; grid-template-columns:repeat(2,minmax(0,1fr)); gap:16px; }
        .master-field { margin-bottom:16px; }
        .master-field label { display:block; margin-bottom:8px; color:#111827; font-weight:700; }
        .master-field input, .master-field textarea, .master-field select { width:100%; min-height:40px; border:1px solid #d7e0ea; border-radius:8px; padding:8px 12px; box-sizing:border-box; }
        .master-field textarea { min-height:110px; resize:vertical; }
        .master-hint { color:#6b7280; font-size:13px; margin-top:6px; line-height:1.7; }
        @media (max-width:760px){ .master-grid { grid-template-columns:1fr; } }
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
            <div class="master-title">
                <div>
                    <h1>&#26426;&#26500;&#31649;&#29702;</h1>
                    <p>&#32479;&#19968;&#31649;&#29702;&#20316;&#32773;&#26426;&#26500;&#12289;&#26426;&#26500;&#21035;&#21517;&#21644;&#35770;&#25991;&#26426;&#26500;&#20851;&#32852;&#12290;</p>
                </div>
                <div class="master-actions">
                    <a href="Admin_InstitutionList.aspx?Action=Add&MenuId=<%=MenuId %>">&#26032;&#22686;&#26426;&#26500;</a>
                    <a href="Admin_AuthorList.aspx?MenuId=1730">&#20316;&#32773;&#31649;&#29702;</a>
                </div>
            </div>
            <% if (IsEditMode) { %>
            <div class="card">
                <form method="post" action="Admin_InstitutionList.aspx?Action=<%=Server.UrlEncode(Action) %>&ID=<%=Id %>&MenuId=<%=Server.UrlEncode(MenuId) %>">
                    <div class="master-form">
                        <h3><%=PageTitle %></h3>
                        <div class="master-grid">
                            <div class="master-field">
                                <label>&#20013;&#25991;&#21517;</label>
                                <input type="text" name="name_cn" value="<%=Server.HtmlEncode(NameCn) %>" />
                            </div>
                            <div class="master-field">
                                <label>&#33521;&#25991;&#21517;</label>
                                <input type="text" name="name_en" value="<%=Server.HtmlEncode(NameEn) %>" />
                            </div>
                            <div class="master-field">
                                <label>&#19978;&#32423;&#26426;&#26500;</label>
                                <select name="parent_id"><%=ParentOptionsHtml %></select>
                                <div class="master-hint">&#29992;&#20110;&#34920;&#36798;&#23398;&#38498;&#12289;&#23454;&#39564;&#23460;&#19982;&#22823;&#23398;/&#30740;&#31350;&#26426;&#26500;&#30340;&#20174;&#23646;&#20851;&#31995;&#12290;</div>
                            </div>
                        </div>
                        <div class="master-field">
                            <label>&#26426;&#26500;&#21035;&#21517;</label>
                            <textarea name="alias_names"><%=Server.HtmlEncode(AliasNames) %></textarea>
                            <div class="master-hint">&#22810;&#20010;&#21035;&#21517;&#21487;&#29992;&#20998;&#21495;&#12289;&#31446;&#32447;&#25110;&#25442;&#34892;&#20998;&#38548;&#65292;&#29992;&#20110;&#21518;&#32493;&#33258;&#21160;&#21305;&#37197;&#12290;</div>
                        </div>
                        <div class="master-grid">
                            <div class="master-field"><label>&#22269;&#23478;/&#22320;&#21306;</label><input type="text" name="country" value="<%=Server.HtmlEncode(Country) %>" /></div>
                            <div class="master-field"><label>&#30465;&#20221;/&#24030;</label><input type="text" name="province" value="<%=Server.HtmlEncode(Province) %>" /></div>
                            <div class="master-field"><label>&#22478;&#24066;</label><input type="text" name="city" value="<%=Server.HtmlEncode(City) %>" /></div>
                            <div class="master-field"><label>&#23448;&#32593;</label><input type="text" name="website" value="<%=Server.HtmlEncode(Website) %>" /></div>
                        </div>
                        <div class="master-grid">
                            <div class="master-field">
                                <label>&#29366;&#24577;</label>
                                <select name="status">
                                    <option value="1" <%=Status == 1 ? "selected=\"selected\"" : "" %>>&#21551;&#29992;</option>
                                    <option value="0" <%=Status == 0 ? "selected=\"selected\"" : "" %>>&#20572;&#29992;</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="card-footer master-actions">
                        <button type="submit" class="btn btn-primary">&#20445;&#23384;</button>
                        <a href="Admin_InstitutionList.aspx?MenuId=<%=Server.UrlEncode(MenuId) %>">&#36820;&#22238;</a>
                    </div>
                </form>
            </div>
            <% } else { %>
            <div class="card mb-12">
                <form method="get" action="Admin_InstitutionList.aspx">
                    <input type="hidden" name="MenuId" value="<%=Server.HtmlEncode(MenuId) %>" />
                    <div class="master-toolbar">
                        <label>&#20851;&#38190;&#35789;</label>
                        <input type="text" name="Key" value="<%=Server.HtmlEncode(Key) %>" placeholder="&#26426;&#26500;&#21517;&#31216;&#12289;&#21035;&#21517;&#12289;&#22269;&#23478;&#12289;&#22478;&#24066;" />
                        <button type="submit" class="btn btn-primary">&#25628;&#32034;</button>
                        <a class="btn btn-secondary" href="Admin_InstitutionList.aspx?MenuId=<%=MenuId %>">&#37325;&#32622;</a>
                    </div>
                </form>
                <div class="card-body p-0">
                    <table class="table table-sm master-table">
                        <thead>
                            <tr>
                                <th style="width:70px;">ID</th>
                                <th>&#26426;&#26500;&#20449;&#24687;</th>
                                <th style="width:24%;">&#22320;&#21306;</th>
                                <th style="width:110px;">&#35770;&#25991;&#20851;&#32852;</th>
                                <th style="width:110px;">&#29366;&#24577;</th>
                                <th style="width:150px;">&#26356;&#26032;&#26102;&#38388;</th>
                                <th style="width:160px;">&#25805;&#20316;</th>
                            </tr>
                        </thead>
                        <tbody><%=ListHtml %></tbody>
                    </table>
                    <%=PagerHtml %>
                </div>
            </div>
            <% } %>
        </div>
    </div>
    <% } %>
</body>
</html>
