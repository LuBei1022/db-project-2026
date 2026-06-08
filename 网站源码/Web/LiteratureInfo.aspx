<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LiteratureInfo.aspx.cs" Inherits="Web.LiteratureInfo" %>

<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <title><%=pageTitle %></title>
    <LiteratureManager:css ID="css" runat="server" />
    <style>
        .lit-detail-wrap { max-width: 1100px; margin: 0 auto; padding: 30px 20px 60px; }
        .lit-detail-card { background: #fff; border: 1px solid #ebeff4; border-radius: 24px; padding: 34px; }
        .lit-owner-card { border-color: #cfe0f6; box-shadow: 0 18px 50px rgba(29, 111, 220, .08); }
        .lit-detail-card h1 { margin: 0 0 16px; font-size: 34px; line-height: 1.45; color: #16283d; }
        .lit-owner-card h1 { display: flex; align-items: center; gap: 12px; flex-wrap: wrap; }
        .lit-owner-badge { display: inline-flex; align-items: center; min-height: 30px; padding: 0 12px; border-radius: 999px; background: #eef5ff; color: #1d6fdc; font-size: 14px; font-weight: 700; }
        .lit-detail-meta { color: #617286; font-size: 15px; margin-bottom: 22px; line-height: 1.9; }
        .lit-detail-grid { display: grid; grid-template-columns: 160px 1fr; gap: 10px 18px; margin-bottom: 26px; }
        .lit-detail-grid .label { color: #6f7d8d; }
        .lit-detail-grid .value { color: #223548; }
        .lit-detail-section { margin-top: 26px; }
        .lit-detail-section h3 { margin: 0 0 12px; font-size: 20px; color: #1b2b3b; }
        .lit-detail-section p { margin: 0; color: #49596b; line-height: 1.9; white-space: pre-wrap; }
        .lit-detail-actions { display: flex; align-items: center; gap: 12px; flex-wrap: wrap; margin-bottom: 12px; }
        .lit-detail-tools { display: flex; align-items: center; gap: 12px; flex-wrap: wrap; margin: 16px 0 12px; }
        .lit-detail-actions a, .lit-detail-actions span { display: inline-flex; align-items: center; justify-content: center; min-height: 44px; padding: 0 18px; border-radius: 10px; background: #1d6fdc; color: #fff; box-sizing: border-box; }
        .lit-detail-tools a.secondary { background: #eef5ff; color: #1d6fdc; }
        .lit-detail-tools a, .lit-detail-tools button { display: inline-flex; align-items: center; justify-content: center; min-height: 44px; padding: 0 18px; border: 0; border-radius: 10px; background: #eef5ff; color: #1d6fdc; cursor: pointer; font: inherit; box-sizing: border-box; }
        .lit-detail-tools button:hover, .lit-detail-tools a:hover { background: #dfeeff; }
        .lit-detail-tools .lit-reaction-btn { gap: 8px; border: 1px solid #d9e8fb; border-radius: 999px; background: #fff; color: #0066cc; }
        .lit-detail-tools .lit-reaction-btn em { min-width: 22px; padding: 2px 8px; border-radius: 999px; background: #f5f5f7; color: #1d1d1f; font-style: normal; font-size: 13px; }
        .lit-detail-tools .lit-reaction-btn.active { background: #0066cc; border-color: #0066cc; color: #fff; }
        .lit-detail-tools .lit-reaction-btn.active em { background: rgba(255,255,255,.2); color: #fff; }
        .lit-detail-actions span.disabled { background: #edf1f6; color: #8b98a8; cursor: not-allowed; }
        .lit-download-form { display: inline-flex; flex-wrap: nowrap; align-items: center; gap: 10px; margin: 0; padding: 10px; border: 1px solid #e2e9f3; border-radius: 16px; background: #f9fbfe; vertical-align: middle; }
        .lit-download-form .lit-pay-title { color: #23364a; font-weight: 700; white-space: nowrap; }
        .lit-download-form .lit-pay-option { display: inline-flex; align-items: center; gap: 8px; padding: 9px 12px; border: 1px solid #dce6f2; border-radius: 12px; background: #fff; color: #23364a; cursor: pointer; }
        .lit-download-form .lit-pay-option input { margin: 0; }
        .lit-download-form .lit-pay-option em { color: #1d6fdc; font-style: normal; font-weight: 700; }
        .lit-download-form .lit-pay-option.unavailable { color: #93a0ae; background: #f0f3f7; cursor: not-allowed; }
        .lit-download-form .lit-pay-option.unavailable em { color: #93a0ae; }
        .lit-download-form button { border: 0; padding: 10px 18px; border-radius: 12px; background: #122235; color: #fff; cursor: pointer; transition: transform .18s ease, box-shadow .18s ease; }
        .lit-download-form button:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(18, 34, 53, .18); }
        .lit-detail-tip { margin-top: 10px; color: #8b98a8; font-size: 13px; line-height: 1.8; }
        .lit-status-notice { margin: 16px 0 22px; padding: 14px 16px; border-radius: 14px; background: #fff8e8; border: 1px solid #f1dfb8; color: #8a6724; line-height: 1.8; }
        .lit-owner-panel { margin: 18px 0 22px; border: 1px solid #d8e6f8; border-radius: 18px; background: #f7fbff; padding: 20px; }
        .lit-owner-panel-head { display: flex; align-items: center; justify-content: space-between; gap: 16px; margin-bottom: 16px; }
        .lit-owner-panel-head span { display: block; color: #7b8794; font-size: 13px; margin-bottom: 4px; }
        .lit-owner-panel-head strong { color: #172b40; font-size: 22px; }
        .lit-owner-panel-head a { display: inline-flex; align-items: center; justify-content: center; min-height: 38px; padding: 0 14px; border-radius: 10px; background: #1d6fdc; color: #fff; }
        .lit-owner-panel-actions { display: flex; align-items: center; gap: 10px; flex-wrap: wrap; }
        .lit-owner-panel-actions button { display: inline-flex; align-items: center; justify-content: center; min-height: 38px; padding: 0 14px; border: 0; border-radius: 10px; background: #122235; color: #fff; cursor: pointer; font: inherit; }
        .lit-owner-stats { display: grid; grid-template-columns: repeat(4, minmax(0, 1fr)); gap: 12px; }
        .lit-owner-stats div { border: 1px solid #e3edf8; border-radius: 12px; background: #fff; padding: 12px; }
        .lit-owner-stats span { display: block; color: #7b8794; font-size: 13px; margin-bottom: 6px; }
        .lit-owner-stats strong { color: #172b40; font-size: 15px; }
        .lit-owner-status { font-style: normal; }
        .lit-owner-status.ok { color: #168449; }
        .lit-owner-status.pending { color: #a06a00; }
        .lit-owner-status.reject { color: #c33b32; }
        .lit-owner-status.merged { color: #1d6fdc; }
        .lit-owner-panel p { margin: 14px 0 0; color: #526174; line-height: 1.8; }
        .lit-owner-audit { margin: 22px 0; border: 1px solid #e2e9f3; border-radius: 16px; background: #fbfdff; padding: 18px; }
        .lit-owner-audit h3 { margin: 0 0 8px; font-size: 18px; color: #172b40; }
        .lit-owner-audit p { margin: 0; color: #66758a; line-height: 1.8; }
        .lit-owner-audit p.ok { color: #168449; }
        .lit-owner-meta-modal { width: min(780px, 100%); max-height: calc(100vh - 72px); }
        .lit-owner-meta-modal form { display: flex; flex-direction: column; flex: 1 1 auto; min-height: 0; }
        .lit-owner-meta-body { max-height: none; overflow-y: auto; overscroll-behavior: contain; padding-bottom: 18px; flex: 1 1 auto; min-height: 0; }
        .lit-owner-meta-grid { display: grid; grid-template-columns: repeat(2, minmax(0, 1fr)); gap: 12px; }
        .lit-owner-meta-field { display: block; margin-bottom: 12px; color: #526174; font-weight: 700; }
        .lit-owner-meta-field span { display: block; margin-bottom: 7px; }
        .lit-owner-meta-field input, .lit-owner-meta-field textarea { width: 100%; box-sizing: border-box; border: 1px solid #dce5ef; border-radius: 10px; padding: 11px 12px; color: #172b40; font: inherit; line-height: 1.6; }
        .lit-owner-meta-field textarea { min-height: 120px; resize: vertical; }
        .lit-modal-mask { display: none; position: fixed; inset: 0; z-index: 1000; background: rgba(15, 28, 44, .48); align-items: center; justify-content: center; padding: 20px; }
        .lit-modal-mask.show { display: flex; }
        body.lit-modal-open { overflow: hidden; }
        .lit-modal { width: min(560px, 100%); max-height: calc(100vh - 56px); background: #fff; border-radius: 18px; box-shadow: 0 26px 80px rgba(16, 32, 54, .22); overflow: hidden; display: flex; flex-direction: column; }
        .lit-modal-head { display: flex; align-items: center; justify-content: space-between; padding: 18px 22px; border-bottom: 1px solid #edf1f5; }
        .lit-modal-head h3 { margin: 0; font-size: 20px; color: #172b40; }
        .lit-modal-close { border: 0; background: transparent; font-size: 24px; line-height: 1; cursor: pointer; color: #7b8794; }
        .lit-modal-body { padding: 22px; overflow-y: auto; overscroll-behavior: contain; flex: 1 1 auto; min-height: 0; }
        .lit-modal-body textarea { width: 100%; min-height: 150px; box-sizing: border-box; border: 1px solid #dce5ef; border-radius: 12px; padding: 14px; resize: vertical; font-size: 15px; line-height: 1.7; }
        .lit-modal-foot { display: flex; justify-content: flex-end; gap: 12px; padding: 14px 22px 22px; border-top: 1px solid #edf1f5; background: #fff; flex: 0 0 auto; }
        .lit-modal-foot button { border: 0; border-radius: 10px; padding: 10px 18px; cursor: pointer; }
        .lit-modal-foot .cancel { background: #edf1f6; color: #526174; }
        .lit-modal-foot .submit { background: #1d6fdc; color: #fff; }
        .lit-modal-tip { margin-top: 10px; color: #7b8794; line-height: 1.7; }
        .lit-citation-modal { width: min(760px, 100%); max-height: min(760px, 92vh); }
        .lit-citation-body { max-height: 620px; overflow: auto; }
        .lit-citation-tabs { display: flex; gap: 8px; border-bottom: 1px solid #e7edf5; margin-bottom: 18px; }
        .lit-citation-tabs button { border: 0; background: transparent; color: #526174; padding: 0 18px 12px; cursor: pointer; font: inherit; border-bottom: 2px solid transparent; }
        .lit-citation-tabs button.active { color: #1d6fdc; border-bottom-color: #1d6fdc; }
        .lit-citation-panel { display: none; }
        .lit-citation-panel.active { display: block; }
        .lit-citation-row { display: grid; grid-template-columns: 86px 1fr; gap: 14px; align-items: start; margin-bottom: 12px; }
        .lit-citation-label { color: #526174; font-weight: 700; padding-top: 18px; text-align: right; }
        .lit-citation-card { position: relative; border: 1px solid #e2e8f0; border-radius: 12px; background: #fff; padding: 18px 108px 18px 20px; color: #172b40; line-height: 1.75; min-height: 62px; }
        .lit-citation-copy { position: absolute; right: 16px; top: 16px; border: 0; background: #eef5ff; color: #1d6fdc; border-radius: 9px; padding: 7px 12px; cursor: pointer; }
        .lit-citation-copy:hover { background: #dfeeff; }
        .lit-citation-note { margin: 0 0 16px 86px; color: #7b8794; line-height: 1.7; }
        .lit-citation-pre { white-space: pre-wrap; word-break: break-word; font-family: Consolas, "Courier New", monospace; }
        .lit-comments-section { margin-top: 34px; padding-top: 30px; border-top: 1px solid #e8edf4; }
        .lit-comments-head { display: flex; align-items: flex-end; justify-content: space-between; gap: 18px; margin-bottom: 18px; }
        .lit-comments-head h3 { margin: 4px 0 0; font-size: 22px; color: #172b40; }
        .lit-comments-kicker { color: #8b98a8; font-size: 12px; font-weight: 700; letter-spacing: .14em; }
        .lit-comments-count { color: #6f7d8d; font-size: 14px; white-space: nowrap; }
        .lit-comment-list { display: grid; gap: 14px; }
        .lit-comment-item { border: 1px solid #e6edf6; border-radius: 18px; background: linear-gradient(180deg, #fbfdff 0%, #fff 100%); padding: 18px; box-shadow: 0 12px 32px rgba(22, 40, 61, .04); }
        .lit-comment-main { display: flex; align-items: flex-start; gap: 14px; }
        .lit-comment-avatar { width: 44px; height: 44px; border-radius: 50%; object-fit: cover; flex: 0 0 44px; background: #eef3f8; }
        .lit-comment-body { min-width: 0; flex: 1; }
        .lit-comment-author-row { display: flex; align-items: center; justify-content: space-between; gap: 12px; }
        .lit-comment-author { color: #172b40; font-size: 16px; font-weight: 700; }
        .lit-comment-delete { border: 0; border-radius: 999px; background: #f5f5f7; color: #0066cc; padding: 6px 12px; cursor: pointer; font: inherit; font-size: 13px; }
        .lit-comment-delete:hover { background: #e8f1ff; }
        .lit-comment-time { margin-top: 3px; color: #8b98a8; font-size: 13px; }
        .lit-comment-text { margin-top: 10px; color: #405064; line-height: 1.8; }
        .lit-comment-replies { display: grid; gap: 10px; margin: 15px 0 0; padding-left: 14px; border-left: 3px solid #d7e8ff; }
        .lit-comment-reply { border: 1px solid #edf2f8; border-radius: 14px; background: rgba(238, 245, 255, .55); padding: 12px 14px; }
        .lit-comment-reply-head { display: flex; align-items: center; justify-content: space-between; gap: 12px; margin-bottom: 6px; }
        .lit-comment-reply-head strong { color: #1d6fdc; }
        .lit-comment-reply-head span { color: #8b98a8; font-size: 13px; white-space: nowrap; }
        .lit-comment-reply-text { color: #405064; line-height: 1.75; }
        .lit-comments-empty { border: 1px dashed #d9e3ef; border-radius: 18px; background: #fbfdff; padding: 24px; color: #728197; line-height: 1.75; }
        .lit-comments-empty strong { display: block; color: #172b40; font-size: 16px; margin-bottom: 6px; }
        .lit-comments-empty p { margin: 0; }
        @media (max-width: 760px) {
            .lit-detail-card { padding: 24px; }
            .lit-detail-card h1 { font-size: 28px; }
            .lit-detail-grid { grid-template-columns: 1fr; }
            .lit-detail-actions, .lit-detail-tools { align-items: stretch; }
            .lit-detail-actions a, .lit-detail-actions span, .lit-detail-tools a, .lit-detail-tools button { width: 100%; }
            .lit-download-form { width: 100%; flex-wrap: wrap; }
            .lit-download-form .lit-pay-title { width: 100%; }
            .lit-owner-panel-head { align-items: flex-start; flex-direction: column; }
            .lit-owner-panel-head a, .lit-owner-panel-actions, .lit-owner-panel-actions button { width: 100%; }
            .lit-owner-stats { grid-template-columns: 1fr; }
            .lit-owner-meta-grid { grid-template-columns: 1fr; }
            .lit-comments-head { align-items: flex-start; flex-direction: column; }
            .lit-comments-count { white-space: normal; }
            .lit-comment-main { gap: 10px; }
            .lit-comment-avatar { width: 38px; height: 38px; flex-basis: 38px; }
            .lit-citation-row { grid-template-columns: 1fr; }
            .lit-citation-label { text-align: left; padding-top: 0; }
            .lit-citation-note { margin-left: 0; }
            .lit-citation-card { padding-right: 20px; padding-bottom: 58px; }
            .lit-citation-copy { top: auto; right: 14px; bottom: 14px; }
        }
    </style>
</head>
<body class="ac" style="background: #f6f8fb;">
    <LiteratureManager:top ID="top" runat="server" />
    <div class="middle">
        <div class="lit-detail-wrap">
            <div class="<%=detailCardClass %>">
                <h1><%=title %><%=ownerBadgeHtml %></h1>
                <div class="lit-detail-meta"><%=metaLine %></div>
                <%=ownerPanelHtml %>
                <%=statusNoticeHtml %>
                <div class="lit-detail-actions">
                    <%=pdfLinkHtml %>
                </div>
                <div class="lit-detail-tools">
                    <%=reactionToolsHtml %>
                    <button type="button" onclick="openLiteratureCitation()">&#24341;&#29992;</button>
                    <button type="button" onclick="openLiteratureCommentEntry()">&#21457;&#34920;&#35780;&#35770;</button>
                    <button type="button" onclick="openLiteratureAppealEntry()">&#20869;&#23481;&#21453;&#39304; / &#29256;&#26435;&#30003;&#35785;</button>
                    <a href="/LiteratureSearch.aspx" class="secondary">&#36820;&#22238;&#26816;&#32034;</a>
                </div>
                <div class="lit-detail-tip">
                    <%=detailTipHtml %>
                </div>
                <%=ownerMetaAuditHtml %>

                <div class="lit-detail-section">
                    <h3>&#22522;&#26412;&#20449;&#24687;</h3>
                    <div class="lit-detail-grid">
                        <div class="label">DOI</div><div class="value"><%=doi %></div>
                        <div class="label">&#20316;&#32773;&#21333;&#20301;</div><div class="value"><%=institution %></div>
                                    <div class="label">&#26399;&#21002;</div><div class="value"><%=journalName %></div>
                        <div class="label">&#20250;&#35758;</div><div class="value"><%=conferenceName %></div>
                        <div class="label">&#20851;&#38190;&#35789;</div><div class="value"><%=keywords %></div>
                        <div class="label">&#26631;&#31614;</div><div class="value"><%=tags %></div>
                        <div class="label">&#39029;&#30721;</div><div class="value"><%=pages %></div>
                        <div class="label">&#20986;&#29256;&#31038;</div><div class="value"><%=publisher %></div>
                        <div class="label">&#26469;&#28304;&#24211;</div><div class="value"><%=sourceDb %></div>
                        <div class="label">&#19979;&#36733;&#31215;&#20998;</div><div class="value"><%=downloadPointsText %></div>
                        <div class="label">&#22806;&#37096;&#38142;&#25509;</div><div class="value"><%=externalLinkHtml %></div>
                    </div>
                </div>

                <div class="lit-detail-section">
                    <h3>&#25688;&#35201;</h3>
                    <p><%=abstractText %></p>
                </div>

                <div class="lit-detail-section">
                    <h3>&#22791;&#27880;</h3>
                    <p><%=remark %></p>
                </div>
                <%=commentSectionHtml %>
            </div>
        </div>
    </div>
    <%=citationModalHtml %>
    <%=ownerMetadataModalHtml %>
    <div class="lit-modal-mask" id="literatureCommentModal">
        <div class="lit-modal">
            <div class="lit-modal-head">
                <h3>&#21457;&#34920;&#35780;&#35770;</h3>
                <button type="button" class="lit-modal-close" onclick="closeLiteratureComment()">×</button>
            </div>
            <div class="lit-modal-body">
                <textarea id="literature_comment_info" placeholder="写下你对这篇文献的评论、补充说明或阅读建议。评论提交后将先进入审核。"></textarea>
                <div class="lit-modal-tip">&#31649;&#29702;&#21592;&#23457;&#26680;&#36890;&#36807;&#21518;&#23558;&#20250;&#20844;&#24320;&#23637;&#31034;&#65292;&#35831;&#21247;&#21253;&#21547;&#25935;&#24863;&#20869;&#23481;</div>
            </div>
            <div class="lit-modal-foot">
                <button type="button" class="cancel" onclick="closeLiteratureComment()">&#21462;&#28040;</button>
                <button type="button" class="submit" id="literatureCommentSubmit" onclick="submitLiteratureComment()">&#25552;&#20132;</button>
            </div>
        </div>
    </div>
    <LiteratureManager:foot ID="foot" runat="server" />
    <script type="text/javascript">
        var currentLiteratureId = <%=literatureId %>;
        var currentLiteratureUrl = "<%=Server.UrlEncode(currentPageUrl) %>";
        var literatureUserLoggedIn = <%=IsLogin ? "true" : "false" %>;

        function requireLiteratureLogin() {
            if (literatureUserLoggedIn) {
                return true;
            }
            $(".loginBut:first").click();
            return false;
        }

        function openLiteratureAppealEntry() {
            if (requireLiteratureLogin()) {
                openLiteratureAppeal();
            }
        }

        function openLiteratureCommentEntry() {
            if (requireLiteratureLogin()) {
                openLiteratureComment();
            }
        }

        function toggleLiteratureReaction(button, action) {
            if (!requireLiteratureLogin()) {
                return;
            }
            var $button = $(button);
            if ($button.data("loading")) {
                return;
            }
            $button.data("loading", true).attr("disabled", "disabled");
            $.ajax({
                url: "/Inc/UserCommon.ashx",
                cache: false,
                async: true,
                data: JSON.stringify({
                    btn: "LiteratureReactionToggle",
                    id: currentLiteratureId,
                    action: action
                }),
                dataType: "json",
                type: "POST",
                success: function (data) {
                    $button.data("loading", false).removeAttr("disabled");
                    if (data && data.status == 1) {
                        var selected = !!data.selected;
                        $button.toggleClass("active", selected);
                        $button.find("span").text(action === "like" ? (selected ? "已点赞" : "点赞") : (selected ? "已收藏" : "收藏"));
                        if (typeof data.like_count !== "undefined") {
                            $("#litLikeCount").text(data.like_count);
                        }
                        if (typeof data.favorite_count !== "undefined") {
                            $("#litFavoriteCount").text(data.favorite_count);
                        }
                        if (window.layer) {
                            layer.msg(data.info || "操作成功", { icon: 1 });
                        }
                    } else if (data && data.status == -1) {
                        $(".loginBut:first").click();
                    } else {
                        var message = data && data.info ? data.info : "操作失败，请稍后再试";
                        if (window.layer) {
                            layer.msg(message, { icon: 0 });
                        } else {
                            alert(message);
                        }
                    }
                },
                error: function () {
                    $button.data("loading", false).removeAttr("disabled");
                    if (window.layer) {
                        layer.msg("操作失败，请稍后再试", { icon: 0 });
                    } else {
                        alert("操作失败，请稍后再试");
                    }
                }
            });
        }

        function openLiteratureCitation() {
            $("#literatureCitationModal").addClass("show");
            document.body.classList.add("lit-modal-open");
        }

        function closeLiteratureCitation() {
            $("#literatureCitationModal").removeClass("show");
            closeModalScrollLockIfNoneOpen();
        }

        function openOwnerMetadataModal() {
            $("#ownerMetadataModal").addClass("show");
            document.body.classList.add("lit-modal-open");
        }

        function closeOwnerMetadataModal() {
            $("#ownerMetadataModal").removeClass("show");
            closeModalScrollLockIfNoneOpen();
        }

        function closeModalScrollLockIfNoneOpen() {
            if ($(".lit-modal-mask.show").length === 0) {
                document.body.classList.remove("lit-modal-open");
            }
        }

        function switchLiteratureCitationTab(tabName) {
            $(".lit-citation-tabs button").removeClass("active");
            $(".lit-citation-tabs button[data-tab='" + tabName + "']").addClass("active");
            $(".lit-citation-panel").removeClass("active");
            $("#litCitationPanel" + tabName).addClass("active");
        }

        function copyLiteratureCitation(id) {
            var elem = document.getElementById(id);
            if (!elem) {
                return;
            }
            var text = elem.innerText || elem.textContent || "";
            var done = function () {
                if (window.layer) {
                    layer.msg("已复制引用信息", { icon: 1 });
                }
            };
            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(text).then(done, function () {
                    fallbackCopyLiteratureCitation(text, done);
                });
            } else {
                fallbackCopyLiteratureCitation(text, done);
            }
        }

        function fallbackCopyLiteratureCitation(text, done) {
            var textarea = document.createElement("textarea");
            textarea.value = text;
            textarea.style.position = "fixed";
            textarea.style.left = "-9999px";
            document.body.appendChild(textarea);
            textarea.focus();
            textarea.select();
            try {
                document.execCommand("copy");
                if (done) {
                    done();
                }
            } finally {
                document.body.removeChild(textarea);
            }
        }

        function openLiteratureAppeal() {
            var url = "/User/Appeal?url=" + currentLiteratureUrl;
            if (window.layer && layer.open) {
                layer.open({
                    type: 2,
                    title: "内容反馈 / 版权申诉",
                    area: ["680px", "620px"],
                    content: url
                });
            } else {
                window.location.href = url;
            }
        }

        function openLiteratureComment() {
            $("#literatureCommentModal").addClass("show");
            document.body.classList.add("lit-modal-open");
            $("#literature_comment_info").focus();
        }

        function closeLiteratureComment() {
            $("#literatureCommentModal").removeClass("show");
            closeModalScrollLockIfNoneOpen();
        }

        function submitLiteratureComment() {
            var info = ($("#literature_comment_info").val() || "").trim();
            if (!info) {
                if (window.layer) {
                    layer.msg("请先填写评论内容", { icon: 0 });
                } else {
                    alert("请先填写评论内容");
                }
                return;
            }

            $("#literatureCommentSubmit").attr("disabled", "disabled");
            $.ajax({
                url: "/Inc/UserCommon.ashx",
                cache: false,
                async: true,
                data: JSON.stringify({
                    btn: "LiteratureCommentAdd",
                    id: currentLiteratureId,
                    info: info
                }),
                dataType: "json",
                type: "POST",
                success: function (data) {
                    $("#literatureCommentSubmit").removeAttr("disabled");
                    if (data && data.status == 1) {
                        $("#literature_comment_info").val("");
                        closeLiteratureComment();
                        if (window.layer) {
                            layer.msg(data.info, { icon: 1 });
                        } else {
                            alert(data.info);
                        }
                    } else if (data && data.status == -1) {
                        closeLiteratureComment();
                        $(".loginBut:first").click();
                    } else {
                        var message = data && data.info ? data.info : "提交失败，请稍后再试";
                        if (window.layer) {
                            layer.msg(message, { icon: 0 });
                        } else {
                            alert(message);
                        }
                    }
                },
                error: function () {
                    $("#literatureCommentSubmit").removeAttr("disabled");
                    if (window.layer) {
                        layer.msg("提交失败，请稍后再试", { icon: 0 });
                    } else {
                        alert("提交失败，请稍后再试");
                    }
                }
            });
        }

        function deleteLiteratureComment(commentId) {
            if (!requireLiteratureLogin()) {
                return;
            }
            var doDelete = function () {
                $.ajax({
                    url: "/Inc/UserCommon.ashx",
                    cache: false,
                    async: true,
                    data: JSON.stringify({
                        btn: "LiteratureCommentDelete",
                        id: currentLiteratureId,
                        comment_id: commentId
                    }),
                    dataType: "json",
                    type: "POST",
                    success: function (data) {
                        if (data && data.status == 1) {
                            var $item = $(".lit-comment-item[data-comment-id='" + commentId + "']");
                            $item.slideUp(180, function () { $(this).remove(); });
                            var $count = $("#litCommentCount");
                            var count = parseInt($count.text(), 10) || 0;
                            $count.text(Math.max(0, count - 1));
                            if (window.layer) {
                                layer.msg(data.info || "评论已删除", { icon: 1 });
                            }
                        } else if (data && data.status == -1) {
                            $(".loginBut:first").click();
                        } else {
                            var message = data && data.info ? data.info : "删除失败，请稍后再试";
                            if (window.layer) {
                                layer.msg(message, { icon: 0 });
                            } else {
                                alert(message);
                            }
                        }
                    },
                    error: function () {
                        if (window.layer) {
                            layer.msg("删除失败，请稍后再试", { icon: 0 });
                        } else {
                            alert("删除失败，请稍后再试");
                        }
                    }
                });
            };
            if (window.layer && layer.confirm) {
                layer.confirm("确定删除这条评论吗？", { title: "删除评论" }, function (index) {
                    layer.close(index);
                    doDelete();
                });
            } else if (confirm("确定删除这条评论吗？")) {
                doDelete();
            }
        }
    </script>
</body>
</html>
