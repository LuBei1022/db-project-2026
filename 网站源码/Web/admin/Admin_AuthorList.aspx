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
        .author-actions a.author-delete { background:#fff1f2; border-color:#fecdd3; color:#b91c1c; }
        .author-actions a.author-delete:hover { background:#ffe4e6; color:#991b1b; }
        .author-empty { text-align:center; color:#6b7280; padding:34px; }
        .author-page { padding:14px 16px; display:flex; gap:8px; align-items:center; justify-content:flex-end; }
        .author-page a, .author-page span { min-width:34px; height:34px; line-height:34px; text-align:center; border-radius:8px; border:1px solid #d7e0ea; color:#1f344d; text-decoration:none; }
        .author-page span { background:#0066cc; color:#fff; border-color:#0066cc; }
    </style>
</head>
<body>
    <%@ Register TagPrefix="LiteratureManager" TagName="Inc" Src="Inc.ascx" %>
    <%@ Register TagPrefix="LiteratureManager" TagName="class_menu" Src="class_menu.ascx" %>
    <% HandleAuthorDeleteRequest(); %>
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
    <script type="text/javascript">
        (function () {
            var menuId = "<%=System.Web.HttpUtility.JavaScriptStringEncode(MenuId)%>";
            var key = "<%=System.Web.HttpUtility.JavaScriptStringEncode(Key)%>";
            var page = "<%=System.Web.HttpUtility.JavaScriptStringEncode(Request.QueryString["Page"] ?? string.Empty)%>";
            var rows = document.querySelectorAll(".author-table tbody tr");
            for (var i = 0; i < rows.length; i++) {
                var row = rows[i];
                if (!row.cells || row.cells.length < 6) {
                    continue;
                }
                var id = parseInt((row.cells[0].textContent || "").replace(/\s+/g, ""), 10);
                if (!id) {
                    continue;
                }
                var paperCount = parseInt((row.cells[3].textContent || "").replace(/\s+/g, ""), 10) || 0;
                if (paperCount !== 0) {
                    continue;
                }
                var actions = row.querySelector(".author-actions");
                if (!actions || actions.querySelector(".author-delete")) {
                    continue;
                }
                var link = document.createElement("a");
                link.className = "author-delete";
                link.href = "Admin_AuthorList.aspx?Action=Del&ID=" + encodeURIComponent(id)
                    + "&MenuId=" + encodeURIComponent(menuId)
                    + "&Key=" + encodeURIComponent(key)
                    + "&Page=" + encodeURIComponent(page);
                link.onclick = function () {
                    return confirm("确认删除该作者？删除后会移除该作者的论文作者关系和机构历史。若只是同一作者，请优先使用合并。");
                };
                link.appendChild(document.createTextNode("删除"));
                actions.appendChild(link);
            }
        })();
    </script>
    <script runat="server">
        private void HandleAuthorDeleteRequest()
        {
            if (!string.Equals(Request.QueryString["Action"], "Del", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            int authorId = LiteratureManager.Common.Function.ConvertTo<int>(Request.QueryString["ID"], 0);
            string backUrl = BuildAuthorListBackUrl();
            if (authorId <= 0)
            {
                WriteAuthorDeleteResult("请求参数错误!", backUrl, 2);
                return;
            }

            BLL.BLLBase<Model.Author> authorBll = new BLL.BLLBase<Model.Author>();
            Model.Author author = authorBll.SelectSingle("id=" + authorId + " and status<>-1");
            if (author == null || author.id <= 0)
            {
                WriteAuthorDeleteResult("未找到对应作者记录!", backUrl, 1);
                return;
            }

            try
            {
                DeleteAuthorAndRelations(authorId);
            }
            catch (Exception ex)
            {
                LiteratureManager.Common.ImportDataLog.WriteLog(LiteratureManager.Common.LogType.Error, "Admin_AuthorList_Delete:" + ex.Message + "-" + ex.StackTrace);
                WriteAuthorDeleteResult("作者删除失败：" + ex.Message, backUrl, 2);
                return;
            }

            WriteAuthorDeleteResult("作者已删除，相关论文作者关系和机构历史已清理。", backUrl, 0);
        }

        private void DeleteAuthorAndRelations(int authorId)
        {
            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(DAL.DBHelper.ConnectionString))
            {
                connection.Open();
                using (System.Data.SqlClient.SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int activePaperCount = GetAuthorActivePaperCount(transaction, authorId);
                        if (activePaperCount > 0)
                        {
                            throw new InvalidOperationException("该作者仍关联 " + activePaperCount + " 篇论文，不能删除。请先处理论文作者关系或使用合并功能。");
                        }
                        ExecuteAuthorDeleteSql(transaction, "delete from dbo.LiteratureAuthorInstitutionMap where author_id=@author_id", authorId);
                        ExecuteAuthorDeleteSql(transaction, "delete from dbo.LiteratureAuthorMap where author_id=@author_id", authorId);
                        ExecuteAuthorDeleteSql(transaction, "update dbo.AuthorInstitutionHistory set status=-1,is_current=0,updatetime=getdate() where author_id=@author_id", authorId);
                        int affected = ExecuteAuthorDeleteSql(transaction, @"
update dbo.Author
set status=-1,
    identity_status=N'deleted',
    current_institution_id=null,
    current_institution_name=null,
    current_institution_literature_id=null,
    current_institution_sort_date=null,
    current_institution_precision=N'unknown',
    updatetime=getdate()
where id=@author_id and status<>-1", authorId);
                        if (affected <= 0)
                        {
                            throw new InvalidOperationException("作者不存在或已删除。");
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private int GetAuthorActivePaperCount(System.Data.SqlClient.SqlTransaction transaction, int authorId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(@"
select count(distinct m.literature_id)
from dbo.LiteratureAuthorMap m
inner join dbo.Literature l on l.id=m.literature_id
where m.author_id=@author_id and l.status<>-1", transaction.Connection, transaction))
            {
                command.Parameters.Add("@author_id", System.Data.SqlDbType.Int).Value = authorId;
                object value = command.ExecuteScalar();
                return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
            }
        }

        private int ExecuteAuthorDeleteSql(System.Data.SqlClient.SqlTransaction transaction, string sql, int authorId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql, transaction.Connection, transaction))
            {
                command.Parameters.Add("@author_id", System.Data.SqlDbType.Int).Value = authorId;
                return command.ExecuteNonQuery();
            }
        }

        private string BuildAuthorListBackUrl()
        {
            string url = "Admin_AuthorList.aspx?MenuId=" + Server.UrlEncode(MenuId);
            if (!string.IsNullOrWhiteSpace(Key))
            {
                url += "&Key=" + Server.UrlEncode(Key);
            }
            string page = Request.QueryString["Page"] ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(page))
            {
                url += "&Page=" + Server.UrlEncode(page);
            }
            return url;
        }

        private void WriteAuthorDeleteResult(string message, string backUrl, int status)
        {
            LiteratureManager.Common.Function.Ok_Return(LiteratureManager.Common.Cookie.GetCookie("LMS_AdminName"), message, backUrl, status);
            Response.End();
        }
    </script>
</body>
</html>
