<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_LiteratureCommentList.aspx.cs" Inherits="Web.admin.Admin_LiteratureCommentList" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <title>文献评论审核</title>
    <link href="css/adminlte.min.css" rel="stylesheet" />
    <link href="css/style.css" rel="stylesheet" />
    <style>
        body { background:#f5f6f8; padding:20px; }
        .comment-audit { max-width:1180px; margin:0 auto; }
        .comment-audit-head { display:flex; justify-content:space-between; align-items:center; margin-bottom:16px; gap:12px; }
        .comment-audit-head h1 { font-size:22px; margin:0; color:#1f2937; }
        .comment-audit-tabs { display:flex; gap:8px; flex-wrap:wrap; margin-bottom:16px; }
        .comment-audit-tabs a { padding:7px 12px; border:1px solid #d0d7de; border-radius:4px; background:#fff; color:#374151; text-decoration:none; }
        .comment-audit-tabs a.active { background:#0d6efd; border-color:#0d6efd; color:#fff; }
        .comment-card { background:#fff; border:1px solid #e5e7eb; border-radius:6px; padding:16px; margin-bottom:12px; box-shadow:0 1px 2px rgba(15,23,42,.04); }
        .comment-meta { display:flex; flex-wrap:wrap; gap:12px; font-size:13px; color:#6b7280; margin-bottom:10px; }
        .comment-title { font-size:15px; font-weight:600; color:#111827; margin-bottom:10px; }
        .comment-content { color:#1f2937; line-height:1.7; white-space:normal; }
        .comment-actions { display:flex; gap:8px; flex-wrap:wrap; margin-top:14px; }
        .comment-actions a { text-decoration:none; }
        .empty { background:#fff; border:1px dashed #cbd5e1; padding:30px; text-align:center; color:#64748b; border-radius:6px; }
    </style>
</head>
<body>
    <div class="comment-audit">
        <div class="comment-audit-head">
            <h1>文献评论审核</h1>
            <a class="btn btn-secondary" href="<%=BackUrl %>">返回</a>
        </div>
        <div class="comment-audit-tabs">
            <%=FilterTabsHtml %>
        </div>
        <%=ListHtml %>
    </div>
</body>
</html>
