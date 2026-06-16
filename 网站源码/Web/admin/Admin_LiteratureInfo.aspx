<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_LiteratureInfo.aspx.cs" Inherits="Web.admin.Admin_LiteratureInfo" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <style type="text/css">
        .lit-info-title { display: flex; align-items: flex-start; justify-content: space-between; gap: 18px; margin-bottom: 18px; }
        .lit-info-title h1 { margin: 0; color: #1d1d1f; font-size: 28px; line-height: 1.4; }
        .lit-info-title p { margin: 8px 0 0; color: #6e6e73; }
        .lit-info-actions { display: flex; gap: 10px; flex-wrap: wrap; }
        .lit-info-card { margin-bottom: 18px; padding: 20px; border: 1px solid #e5e5ea; border-radius: 18px; background: #fff; }
        .lit-info-stats { display: grid; grid-template-columns: repeat(4, minmax(0, 1fr)); gap: 12px; margin-bottom: 18px; }
        .lit-info-stats div { padding: 16px; border: 1px solid #e5e5ea; border-radius: 14px; background: #fbfbfd; }
        .lit-info-stats span { display: block; color: #86868b; font-size: 13px; }
        .lit-info-stats strong { display: block; margin-top: 8px; color: #1d1d1f; font-size: 26px; }
        .lit-info-chart { height: 260px; }
        .lit-info-chart canvas { width: 100%; height: 100%; }
        .lit-info-grid { display: grid; grid-template-columns: 140px 1fr; gap: 10px 16px; }
        .lit-info-grid .label { color: #86868b; }
        .lit-info-grid .value { color: #1d1d1f; word-break: break-word; }
        .lit-admin-author-affiliations { display: grid; gap: 8px; }
        .lit-admin-author-affiliations div { display: grid; grid-template-columns: minmax(120px, 220px) 1fr; gap: 10px; padding: 8px 0; border-bottom: 1px dashed #e5e5ea; }
        .lit-admin-author-affiliations div:last-child { border-bottom: 0; }
        .lit-admin-author-affiliations strong { color: #1d1d1f; }
        .lit-admin-author-affiliations span { color: #515864; line-height: 1.7; }
        .lit-info-section-title { margin: 0 0 14px; color: #1d1d1f; font-size: 18px; font-weight: 700; }
        .lit-pdf-head { display: flex; align-items: center; justify-content: space-between; gap: 14px; margin-bottom: 14px; }
        .lit-pdf-head strong { color: #1d1d1f; }
        .lit-pdf-head div { display: flex; gap: 10px; flex-wrap: wrap; }
        .lit-pdf-frame { width: 100%; height: 620px; border: 1px solid #e5e5ea; border-radius: 14px; background: #f5f5f7; }
        .lit-pdf-empty { padding: 26px; border: 1px dashed #d2d2d7; border-radius: 14px; color: #86868b; background: #fbfbfd; }
        .lit-admin-comments { display: grid; gap: 12px; }
        .lit-admin-comment { padding: 14px; border: 1px solid #e5e5ea; border-radius: 12px; background: #fbfbfd; }
        .lit-admin-comment-head { display: flex; align-items: center; gap: 10px; flex-wrap: wrap; margin-bottom: 8px; }
        .lit-admin-comment-head strong { color: #1d1d1f; }
        .lit-admin-comment-head span { color: #86868b; font-size: 12px; }
        .lit-admin-comment-head a { margin-left: auto; color: #245d92; }
        .lit-admin-comment-text { color: #1d1d1f; line-height: 1.7; }
        .lit-admin-comment-empty { padding: 18px; border: 1px dashed #d2d2d7; border-radius: 12px; color: #86868b; background: #fbfbfd; }
        .lit-admin-replies { margin-top: 10px; padding-left: 12px; border-left: 3px solid #dfeaf6; display: grid; gap: 8px; }
        .lit-admin-reply { color: #4b5563; }
        .lit-admin-reply span { margin-left: 8px; color: #86868b; font-size: 12px; }
        .lit-admin-reply p { margin: 4px 0 0; line-height: 1.6; }
        @media (max-width: 900px) {
            .lit-info-title { flex-direction: column; }
            .lit-info-stats { grid-template-columns: repeat(2, minmax(0, 1fr)); }
            .lit-info-grid { grid-template-columns: 1fr; }
        }
    </style>
</head>
<body>
    <%@ Register TagPrefix="LiteratureManager" TagName="Inc" Src="Inc.ascx" %>
    <%@ Register TagPrefix="LiteratureManager" TagName="class_menu" Src="class_menu.ascx" %>
    <% if (isLoading) { %>
    <LiteratureManager:Inc ID="Inc2" runat="server" />
    <LiteratureManager:class_menu ID="class_menu" runat="server" />

    <form id="form2" runat="server">
        <div class="app-content">
            <div class="container-fluid">
                <div class="lit-info-title">
                    <div>
                        <h1><%=TitleHtml %></h1>
                        <p><%=MetaHtml %></p>
                    </div>
                    <div class="lit-info-actions">
                        <a class="btn btn-primary" href="Admin_LiteratureEdit.aspx?Action=Edit&MenuId=<%=MenuId %>&ID=<%=LiteratureId %>&BackURL=<%=Function.GetEncodeURL() %>">&#32534;&#36753;&#25991;&#29486;</a>
                        <a class="btn btn-secondary" href="<%=BackUrl %>">&#36820;&#22238;&#21015;&#34920;</a>
                    </div>
                </div>

                <div class="lit-info-card">
                    <div class="lit-info-stats">
                        <div><span>&#28857;&#36190;&#25968;</span><strong><%=LikeCount %></strong></div>
                        <div><span>&#25910;&#34255;&#25968;</span><strong><%=FavoriteCount %></strong></div>
                        <div><span>&#20844;&#24320;&#35780;&#35770;</span><strong><%=CommentCount %></strong></div>
                        <div><span>&#19979;&#36733;&#31215;&#20998;</span><strong><%=DownloadPoints %></strong></div>
                    </div>
                    <h3 class="lit-info-section-title">&#26368;&#36817; 14 &#22825;&#20114;&#21160;&#36235;&#21183;</h3>
                    <div class="lit-info-chart"><canvas id="literatureDetailTrendChart"></canvas></div>
                </div>

                <div class="lit-info-card">
                    <h3 class="lit-info-section-title">&#22522;&#26412;&#20449;&#24687;</h3>
                    <div class="lit-info-grid">
                        <%=InfoGridHtml %>
                    </div>
                </div>

                <div class="lit-info-card">
                    <h3 class="lit-info-section-title">PDF &#38468;&#20214;</h3>
                    <%=PdfHtml %>
                </div>

                <div class="lit-info-card" id="comments">
                    <h3 class="lit-info-section-title">&#35780;&#35770;&#35814;&#24773;</h3>
                    <%=CommentHtml %>
                </div>
            </div>
        </div>
    </form>
    <script type="text/javascript">
        (function () {
            var canvas = document.getElementById("literatureDetailTrendChart");
            if (!canvas) return;
            var labels = <%=TrendLabelsJson %>;
            var series = [
                { name: "\u70b9\u8d5e", color: "#0066cc", data: <%=TrendLikesJson %> },
                { name: "\u6536\u85cf", color: "#34c759", data: <%=TrendFavoritesJson %> },
                { name: "\u8bc4\u8bba", color: "#ff9500", data: <%=TrendCommentsJson %> }
            ];
            var ctx = canvas.getContext("2d");
            function draw() {
                var rect = canvas.parentNode.getBoundingClientRect();
                canvas.width = rect.width * window.devicePixelRatio;
                canvas.height = rect.height * window.devicePixelRatio;
                ctx.setTransform(window.devicePixelRatio, 0, 0, window.devicePixelRatio, 0, 0);
                var w = rect.width, h = rect.height, left = 46, right = 18, top = 24, bottom = 38;
                ctx.clearRect(0, 0, w, h);
                var max = 1;
                series.forEach(function (s) { s.data.forEach(function (v) { if (v > max) max = v; }); });
                ctx.strokeStyle = "#e5e5ea";
                ctx.lineWidth = 1;
                for (var i = 0; i < 4; i++) {
                    var y = top + (h - top - bottom) * i / 3;
                    ctx.beginPath(); ctx.moveTo(left, y); ctx.lineTo(w - right, y); ctx.stroke();
                }
                function xAt(i) { return left + (w - left - right) * i / Math.max(1, labels.length - 1); }
                function yAt(v) { return top + (h - top - bottom) * (1 - v / max); }
                series.forEach(function (s) {
                    ctx.strokeStyle = s.color; ctx.lineWidth = 2; ctx.beginPath();
                    s.data.forEach(function (v, i) {
                        var x = xAt(i), y = yAt(v);
                        if (i === 0) ctx.moveTo(x, y); else ctx.lineTo(x, y);
                    });
                    ctx.stroke();
                    ctx.fillStyle = s.color;
                    s.data.forEach(function (v, i) { ctx.beginPath(); ctx.arc(xAt(i), yAt(v), 3, 0, Math.PI * 2); ctx.fill(); });
                });
                ctx.fillStyle = "#86868b"; ctx.font = "12px Arial";
                labels.forEach(function (label, i) {
                    if (i % 2 === 0 || i === labels.length - 1) ctx.fillText(label, xAt(i) - 15, h - 12);
                });
                var legendX = left;
                series.forEach(function (s) {
                    ctx.fillStyle = s.color; ctx.fillRect(legendX, 4, 10, 10);
                    ctx.fillStyle = "#1d1d1f"; ctx.fillText(s.name, legendX + 14, 13);
                    legendX += 58;
                });
            }
            draw();
            window.addEventListener("resize", draw);
        })();
    </script>
    <% } %>
</body>
</html>
