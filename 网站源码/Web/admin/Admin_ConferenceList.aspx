<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_ConferenceList.aspx.cs" Inherits="Web.admin.Admin_ConferenceList" CodePage="65001" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <title>&#20250;&#35758;&#31649;&#29702;</title>
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
        .master-empty { text-align:center; color:#6b7280; padding:34px; }
        .master-page { padding:14px 16px; display:flex; gap:8px; align-items:center; justify-content:flex-end; }
        .master-page a, .master-page span { min-width:34px; height:34px; line-height:34px; text-align:center; border-radius:8px; border:1px solid #d7e0ea; color:#1f344d; text-decoration:none; }
        .master-page span { background:#0066cc; color:#fff; border-color:#0066cc; }
        .master-form { padding:22px; }
        .master-grid { display:grid; grid-template-columns:repeat(2,minmax(0,1fr)); gap:16px; }
        .master-field { margin-bottom:16px; }
        .master-field label { display:block; margin-bottom:8px; color:#111827; font-weight:700; }
        .master-field input, .master-field select { width:100%; min-height:40px; border:1px solid #d7e0ea; border-radius:8px; padding:8px 12px; box-sizing:border-box; }
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
                    <h1>&#20250;&#35758;&#31649;&#29702;</h1>
                    <p>&#32479;&#19968;&#31649;&#29702;&#20250;&#35758;&#20027;&#25968;&#25454;&#65292;&#24182;&#29992;&#20110;&#35770;&#25991;&#20250;&#35758;&#20851;&#32852;&#12290;</p>
                </div>
                <div class="master-actions">
                    <a href="Admin_ConferenceList.aspx?Action=Add&MenuId=<%=MenuId %>">&#26032;&#22686;&#20250;&#35758;</a>
                </div>
            </div>
            <% if (IsEditMode) { %>
            <div class="card">
                <form method="post" action="Admin_ConferenceList.aspx?Action=<%=Server.UrlEncode(Action) %>&ID=<%=Id %>&MenuId=<%=Server.UrlEncode(MenuId) %>">
                    <div class="master-form">
                        <h3><%=PageTitle %></h3>
                        <div class="master-grid">
                            <div class="master-field"><label>&#20013;&#25991;&#21517;</label><input type="text" name="name_cn" value="<%=Server.HtmlEncode(NameCn) %>" /></div>
                            <div class="master-field"><label>&#33521;&#25991;&#21517;</label><input type="text" name="name_en" value="<%=Server.HtmlEncode(NameEn) %>" /></div>
                            <div class="master-field"><label>&#31616;&#31216;</label><input type="text" name="acronym" value="<%=Server.HtmlEncode(Acronym) %>" /></div>
                            <div class="master-field"><label>&#20027;&#21150;&#26041;</label><input type="text" name="organizer" value="<%=Server.HtmlEncode(Organizer) %>" /></div>
                            <div class="master-field"><label>&#22269;&#23478;/&#22320;&#21306;</label><input type="text" name="country" value="<%=Server.HtmlEncode(Country) %>" /></div>
                            <div class="master-field"><label>&#22478;&#24066;</label><input type="text" name="city" value="<%=Server.HtmlEncode(City) %>" /></div>
                            <div class="master-field"><label>&#24320;&#22987;&#26085;&#26399;</label><input type="date" name="start_date" value="<%=Server.HtmlEncode(StartDate) %>" /></div>
                            <div class="master-field"><label>&#32467;&#26463;&#26085;&#26399;</label><input type="date" name="end_date" value="<%=Server.HtmlEncode(EndDate) %>" /></div>
                            <div class="master-field"><label>&#23448;&#32593;</label><input type="text" name="website" value="<%=Server.HtmlEncode(Website) %>" /></div>
                            <div class="master-field"><label>&#29366;&#24577;</label><select name="status"><option value="1" <%=Status == 1 ? "selected=\"selected\"" : "" %>>&#21551;&#29992;</option><option value="0" <%=Status == 0 ? "selected=\"selected\"" : "" %>>&#20572;&#29992;</option></select></div>
                        </div>
                    </div>
                    <div class="card-footer master-actions">
                        <button type="submit" class="btn btn-primary">&#20445;&#23384;</button>
                        <a href="Admin_ConferenceList.aspx?MenuId=<%=Server.UrlEncode(MenuId) %>">&#36820;&#22238;</a>
                    </div>
                </form>
            </div>
            <% } else { %>
            <div class="card mb-12">
                <form method="get" action="Admin_ConferenceList.aspx">
                    <input type="hidden" name="MenuId" value="<%=Server.HtmlEncode(MenuId) %>" />
                    <div class="master-toolbar">
                        <label>&#20851;&#38190;&#35789;</label>
                        <input type="text" name="Key" value="<%=Server.HtmlEncode(Key) %>" placeholder="&#20250;&#35758;&#21517;&#31216;&#12289;&#31616;&#31216;&#12289;&#20027;&#21150;&#26041;&#12289;&#22478;&#24066;" />
                        <button type="submit" class="btn btn-primary">&#25628;&#32034;</button>
                        <a class="btn btn-secondary" href="Admin_ConferenceList.aspx?MenuId=<%=MenuId %>">&#37325;&#32622;</a>
                    </div>
                </form>
                <div class="card-body p-0">
                    <table class="table table-sm master-table">
                        <thead><tr><th style="width:70px;">ID</th><th>&#20250;&#35758;&#20449;&#24687;</th><th style="width:16%;">&#31616;&#31216;</th><th style="width:20%;">&#22320;&#28857;/&#26102;&#38388;</th><th style="width:100px;">&#35770;&#25991;&#25968;</th><th style="width:100px;">&#29366;&#24577;</th><th style="width:150px;">&#26356;&#26032;&#26102;&#38388;</th><th style="width:160px;">&#25805;&#20316;</th></tr></thead>
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
