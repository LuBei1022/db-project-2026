<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_AuthorMerge.aspx.cs" Inherits="Web.admin.Admin_AuthorMerge" CodePage="65001" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>作者合并</title>
    <style type="text/css">
        .merge-wrap { max-width: 760px; margin: 28px auto; padding: 24px; background: #fff; border: 1px solid #d7e0ea; border-radius: 8px; }
        .merge-wrap h1 { margin: 0 0 12px; font-size: 24px; color: #111827; }
        .merge-tip { color: #6b7280; line-height: 1.7; margin-bottom: 18px; }
        .merge-row { margin-bottom: 14px; }
        .merge-row label { display: block; font-weight: 700; margin-bottom: 6px; }
        .merge-row input, .merge-row select, .merge-row textarea { width: 100%; min-height: 38px; border: 1px solid #c8d6e5; border-radius: 6px; padding: 6px 10px; box-sizing: border-box; }
        .merge-row textarea { min-height: 86px; }
        .merge-preview { background: #f8fafc; border: 1px solid #edf2f7; border-radius: 8px; padding: 12px; margin: 12px 0; color: #374151; }
        .merge-actions { display: flex; gap: 10px; margin-top: 18px; }
        .merge-actions button, .merge-actions a { min-height: 36px; padding: 0 14px; border-radius: 6px; border: 1px solid #0066cc; background: #0066cc; color: #fff; text-decoration: none; display: inline-flex; align-items: center; }
        .merge-actions a { background: #fff; color: #1f344d; border-color: #c8d6e5; }
    </style>
</head>
<body>
    <%@ Register TagPrefix="LiteratureManager" TagName="Inc" Src="Inc.ascx" %>
    <%@ Register TagPrefix="LiteratureManager" TagName="class_menu" Src="class_menu.ascx" %>
    <% if (isLoading) { %>
    <LiteratureManager:Inc ID="Inc2" runat="server" />
    <LiteratureManager:class_menu ID="class_menu" runat="server" />
    <div class="app-content">
        <div class="merge-wrap">
            <h1>作者合并</h1>
            <div class="merge-tip">以主作者 ID 为准，将重复作者的论文关系、机构历史和当前机构信息迁移到主作者。合并后重复作者会标记为 merged/status=-1，前台不再展示。</div>
            <form method="post" action="Admin_AuthorMerge.aspx?MenuId=<%=Server.UrlEncode(MenuId) %>">
                <div class="merge-row">
                    <label>主作者</label>
                    <select name="master_author_id">
                        <%=RenderMergeAuthorOptionsHtml(MasterAuthorId, "请选择主作者") %>
                    </select>
                </div>
                <div class="merge-row">
                    <label>重复作者</label>
                    <select name="duplicate_author_id">
                        <%=RenderMergeAuthorOptionsHtml(DuplicateAuthorId, "请选择重复作者") %>
                    </select>
                </div>
                <div class="merge-row">
                    <label>备注</label>
                    <textarea name="remark"><%=Server.HtmlEncode(Remark) %></textarea>
                </div>
                <div class="merge-preview"><%=PreviewHtml %></div>
                <div class="merge-actions">
                    <button type="submit">确认合并</button>
                    <a href="Admin_AuthorList.aspx?MenuId=<%=Server.UrlEncode(MenuId) %>">返回列表</a>
                </div>
            </form>
        </div>
    </div>
    <% } %>
    <script runat="server">
        private string RenderMergeAuthorOptionsHtml(int selectedId, string placeholder)
        {
            BLL.BLLBase<Model.Author> authorBll = new BLL.BLLBase<Model.Author>();
            System.Collections.Generic.List<Model.Author> authors = authorBll.SelectList(null, "status<>-1", "name_cn asc,name_en asc,id asc");
            System.Text.StringBuilder html = new System.Text.StringBuilder();
            html.Append("<option value=\"0\"");
            if (selectedId <= 0)
            {
                html.Append(" selected=\"selected\"");
            }
            html.Append(">").Append(Server.HtmlEncode(placeholder)).Append("</option>");

            foreach (Model.Author author in authors)
            {
                if (author == null || author.id <= 0)
                {
                    continue;
                }

                html.Append("<option value=\"").Append(author.id).Append("\"");
                if (author.id == selectedId)
                {
                    html.Append(" selected=\"selected\"");
                }
                html.Append(">").Append(Server.HtmlEncode(BuildMergeAuthorOptionLabel(author))).Append("</option>");
            }

            return html.ToString();
        }

        private string BuildMergeAuthorOptionLabel(Model.Author author)
        {
            string cn = LiteratureManager.Common.Function.HtmlDiscode(author.name_cn);
            string en = LiteratureManager.Common.Function.HtmlDiscode(author.name_en);
            string name = !string.IsNullOrWhiteSpace(cn) ? cn : en;
            return (string.IsNullOrWhiteSpace(name) ? "未命名作者" : name) + " / ID " + author.id;
        }
    </script>
</body>
</html>
