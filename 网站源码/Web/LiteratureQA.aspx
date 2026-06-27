<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LiteratureQA.aspx.cs" Inherits="Web.LiteratureQA" CodePage="65001" %>

<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <title>智能问答</title>
    <LiteratureManager:css ID="css" runat="server" />
    <style>
        .qa-wrap { max-width: 980px; margin: 0 auto; padding: 30px 20px 60px; }
        .qa-hero { background: linear-gradient(135deg, #eef6ff 0%, #f9fbff 100%); border: 1px solid #dbe7f4; border-radius: 24px; padding: 30px 32px; margin-bottom: 24px; }
        .qa-hero h1 { font-size: 30px; margin: 0 0 10px; color: #16324f; }
        .qa-hero p { margin: 0; color: #5b6b7d; font-size: 15px; }
        .qa-card { background: #fff; border: 1px solid #ebeff4; border-radius: 20px; padding: 24px; margin-bottom: 20px; }
        .qa-step { font-size: 13px; color: #8493a4; margin-bottom: 10px; }
        .qa-row { display: flex; gap: 12px; flex-wrap: wrap; }
        .qa-row input { flex: 1 1 360px; height: 46px; border-radius: 12px; border: 1px solid #cdd9e5; padding: 0 16px; font-size: 15px; background: #fff; }
        .qa-btn { height: 46px; padding: 0 26px; border: none; border-radius: 12px; background: #1d6fdc; color: #fff; font-size: 15px; cursor: pointer; }
        .qa-btn:disabled { background: #a9c4e8; cursor: not-allowed; }
        .qa-results { margin-top: 16px; display: flex; flex-direction: column; gap: 10px; }
        .qa-paper { border: 1px solid #e6ecf3; border-radius: 12px; padding: 12px 14px; cursor: pointer; transition: all .15s; }
        .qa-paper:hover { border-color: #1d6fdc; background: #f5f9ff; }
        .qa-paper .t { font-size: 15px; color: #1b2a3a; margin: 0 0 4px; }
        .qa-paper .m { font-size: 12px; color: #8493a4; }
        .qa-selected { display: none; align-items: center; gap: 10px; border: 2px solid #1d6fdc; background: #f0f6ff; border-radius: 12px; padding: 12px 14px; margin-bottom: 18px; color: #16324f; font-size: 15px; }
        .qa-selected b { font-weight: 500; }
        .qa-ask-area { display: none; }
        .qa-answer-area { display: none; border-top: 1px solid #eef2f6; margin-top: 20px; padding-top: 18px; }
        .qa-answer-label { font-size: 13px; color: #8493a4; margin-bottom: 8px; }
        .qa-answer { font-size: 16px; line-height: 1.8; color: #22303f; white-space: pre-wrap; }
        .qa-sources { margin-top: 18px; }
        .qa-source { background: #f6f8fb; border-radius: 10px; padding: 10px 12px; font-size: 13px; color: #5b6b7d; line-height: 1.6; margin-top: 8px; word-break: break-word; }
        .qa-hint { color: #8493a4; font-size: 14px; padding: 6px 0; }
        .qa-loading { color: #1d6fdc; font-size: 14px; padding: 6px 0; }
        .qa-error { color: #d8432a; font-size: 14px; padding: 6px 0; }
    </style>
</head>
<body class="ac" style="background: #f6f8fb;">
    <LiteratureManager:top ID="top" runat="server" />

    <div class="qa-wrap">
        <div class="qa-hero">
            <h1>语义检索与智能问答</h1>
            <p>先搜索一篇论文，再用自然语言对它提问，系统基于论文全文内容（RAG）为你作答。</p>
        </div>

        <div class="qa-card">
            <div class="qa-step">第一步 · 搜索论文</div>
            <div class="qa-row">
                <input type="text" id="paperKeyword" placeholder="输入论文标题关键词，如：protein design" onkeydown="if(event.keyCode==13)qaSearch()" />
                <button type="button" class="qa-btn" id="searchBtn" onclick="qaSearch()">搜索</button>
            </div>
            <div class="qa-results" id="paperResults"></div>
        </div>

        <div class="qa-card">
            <div class="qa-selected" id="selectedBox">
                <span>已选定：</span><b id="selectedTitle"></b>
            </div>

            <div class="qa-ask-area" id="askArea">
                <div class="qa-step">第二步 · 提问</div>
                <div class="qa-row">
                    <input type="text" id="question" placeholder="例如：这篇论文主要研究什么？用了什么方法？" onkeydown="if(event.keyCode==13)qaAsk()" />
                    <button type="button" class="qa-btn" id="askBtn" onclick="qaAsk()">提问</button>
                </div>
            </div>

            <div class="qa-hint" id="askHint">请先在上方搜索并选择一篇论文。</div>

            <div class="qa-answer-area" id="answerArea">
                <div class="qa-answer-label">回答</div>
                <div class="qa-answer" id="answerText"></div>
                <div class="qa-sources" id="sourcesBox"></div>
            </div>
        </div>
    </div>

    <LiteratureManager:foot ID="foot" runat="server" />

    <script type="text/javascript">
        var qaPaperId = null;
        var API = "/Inc/RagApi.ashx";

        function el(id) { return document.getElementById(id); }
        function esc(s) { var d = document.createElement("div"); d.textContent = (s == null ? "" : s); return d.innerHTML; }
        function decodeEntities(s) { var d = document.createElement("div"); d.innerHTML = (s == null ? "" : s); return d.textContent; }

        function qaSearch() {
            var kw = (el("paperKeyword").value || "").trim();
            var box = el("paperResults");
            if (!kw) { box.innerHTML = '<div class="qa-error">请输入论文关键词。</div>'; return; }
            el("searchBtn").disabled = true;
            box.innerHTML = '<div class="qa-loading">正在搜索…</div>';
            fetch(API + "?action=search&title=" + encodeURIComponent(kw))
                .then(function (r) { return r.json(); })
                .then(function (data) {
                    el("searchBtn").disabled = false;
                    if (data.error) { box.innerHTML = '<div class="qa-error">' + esc(data.error) + '</div>'; return; }
                    var papers = data.papers || [];
                    if (!papers.length) { box.innerHTML = '<div class="qa-hint">没找到匹配的论文，换个关键词试试。</div>'; return; }
                    var html = "";
                    for (var i = 0; i < papers.length; i++) {
                        var p = papers[i];
                        var title = decodeEntities(p.title || "未命名");
                        var meta = [];
                        if (p.publish_year) meta.push(p.publish_year);
                        if (p.journal_name) meta.push(decodeEntities(p.journal_name));
                        html += '<div class="qa-paper" onclick="qaSelect(' + p.id + ', this)" data-title="' + esc(title) + '">'
                              + '<p class="t">' + esc(title) + '</p>'
                              + '<p class="m">编号 ' + p.id + (meta.length ? ' · ' + esc(meta.join(" · ")) : '') + '</p></div>';
                    }
                    box.innerHTML = html;
                })
                .catch(function () {
                    el("searchBtn").disabled = false;
                    box.innerHTML = '<div class="qa-error">搜索失败，请确认 RAG 服务已启动。</div>';
                });
        }

        function qaSelect(id, node) {
            qaPaperId = id;
            var title = node.getAttribute("data-title");
            el("selectedTitle").innerHTML = title;
            el("selectedBox").style.display = "flex";
            el("askArea").style.display = "block";
            el("askHint").style.display = "none";
            el("answerArea").style.display = "none";
            el("question").focus();
        }

        function qaAsk() {
            if (!qaPaperId) { return; }
            var q = (el("question").value || "").trim();
            if (!q) { return; }
            el("askBtn").disabled = true;
            el("answerArea").style.display = "block";
            el("answerText").textContent = "";
            el("sourcesBox").innerHTML = "";
            el("answerText").innerHTML = '<span class="qa-loading">正在思考中，请稍候…</span>';
            fetch(API + "?action=ask", {
                method: "POST",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify({ paper_id: qaPaperId, question: q })
            })
                .then(function (r) { return r.json(); })
                .then(function (data) {
                    el("askBtn").disabled = false;
                    if (data.error) { el("answerText").innerHTML = '<span class="qa-error">' + esc(data.error) + '</span>'; return; }
                    el("answerText").textContent = data.answer || "（无回答）";
                    var sources = data.sources || [];
                    if (sources.length) {
                        var sh = '<div class="qa-answer-label">回答依据（来自论文原文）</div>';
                        for (var i = 0; i < sources.length; i++) {
                            sh += '<div class="qa-source">' + esc(sources[i].content) + '</div>';
                        }
                        el("sourcesBox").innerHTML = sh;
                    }
                })
                .catch(function () {
                    el("askBtn").disabled = false;
                    el("answerText").innerHTML = '<span class="qa-error">请求失败，请确认 RAG 服务已启动。</span>';
                });
        }
    </script>
</body>
</html>
