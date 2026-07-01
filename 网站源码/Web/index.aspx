<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Web.index" %>

<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta name='robots' content='index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1' />
    <title>&#23398;&#26415;&#25991;&#29486;&#31649;&#29702;&#31995;&#32479;</title>
    <LiteratureManager:css ID="css" runat="server" />
    <style>
        .lit-home { min-height: 100vh; font-family: Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif; color: #111827; background: rgba(249,250,251,.5); }
        .lit-hidden-shell > header, .lit-hidden-shell > footer, .lit-hidden-shell > .rightBox { display: none !important; }
        .portal-page { position: relative; min-height: 100vh; overflow: hidden; padding: 128px 32px 170px; isolation: isolate; background: #f5f5f7; }
        .portal-aurora { position: fixed; inset: 0; z-index: 0; pointer-events: none; overflow: hidden; }
        .portal-aurora span {
            position: absolute;
            display: block;
            border-radius: 9999px;
            filter: blur(70px);
            opacity: .72;
            will-change: transform, opacity;
        }
        .portal-aurora span:nth-child(1) {
            width: 58vw;
            height: 58vw;
            left: -22vw;
            top: 2vh;
            background: rgba(56, 189, 248, .5);
            animation: auroraOne 9s ease-in-out infinite alternate;
        }
        .portal-aurora span:nth-child(2) {
            width: 54vw;
            height: 54vw;
            right: -18vw;
            top: -16vh;
            background: rgba(52, 211, 153, .52);
            animation: auroraTwo 11s ease-in-out infinite alternate;
        }
        .portal-aurora span:nth-child(3) {
            width: 52vw;
            height: 52vw;
            left: 30vw;
            bottom: -24vh;
            background: rgba(168, 85, 247, .42);
            animation: auroraThree 12s ease-in-out infinite alternate;
        }
        .portal-aurora span:nth-child(4) {
            width: 34vw;
            height: 34vw;
            right: 10vw;
            bottom: 12vh;
            background: rgba(255, 255, 255, .72);
            filter: blur(48px);
            animation: auroraGlow 6s ease-in-out infinite;
        }
        @keyframes auroraOne {
            0% { transform: translate3d(0, 0, 0) scale(.95) rotate(0deg); opacity: .42; }
            100% { transform: translate3d(28vw, 18vh, 0) scale(1.2) rotate(18deg); opacity: .86; }
        }
        @keyframes auroraTwo {
            0% { transform: translate3d(0, 0, 0) scale(1.06) rotate(0deg); opacity: .46; }
            100% { transform: translate3d(-30vw, 26vh, 0) scale(.88) rotate(-16deg); opacity: .88; }
        }
        @keyframes auroraThree {
            0% { transform: translate3d(0, 0, 0) scale(.9) rotate(0deg); opacity: .34; }
            100% { transform: translate3d(-22vw, -28vh, 0) scale(1.18) rotate(14deg); opacity: .76; }
        }
        @keyframes auroraGlow {
            0%, 100% { transform: translate3d(0, 0, 0) scale(.76); opacity: .18; }
            50% { transform: translate3d(-18vw, -10vh, 0) scale(1.32); opacity: .82; }
        }
        .portal-page:before, .portal-page:after, .portal-mesh { content: ""; position: fixed; inset: -18vh -12vw; z-index: 0; pointer-events: none; }
        .portal-page:before {
            background:
                radial-gradient(ellipse at 18% 28%, rgba(220,252,231,.58) 0%, rgba(220,252,231,.28) 24%, rgba(220,252,231,0) 52%),
                radial-gradient(ellipse at 82% 18%, rgba(219,234,254,.66) 0%, rgba(219,234,254,.32) 26%, rgba(219,234,254,0) 56%),
                radial-gradient(ellipse at 58% 82%, rgba(243,232,255,.54) 0%, rgba(243,232,255,.22) 28%, rgba(243,232,255,0) 58%);
            filter: blur(8px);
            opacity: 1;
            animation: auraDrift 24s ease-in-out infinite alternate;
        }
        .portal-page:after {
            z-index: 1;
            background:
                linear-gradient(180deg, rgba(255,255,255,.34), rgba(245,245,247,.08) 46%, rgba(255,255,255,.36)),
                radial-gradient(ellipse at 50% 30%, rgba(255,255,255,.34), rgba(255,255,255,0) 62%);
            backdrop-filter: blur(34px) saturate(130%);
            -webkit-backdrop-filter: blur(34px) saturate(130%);
        }
        .portal-mesh {
            inset: 10vh 8vw auto auto;
            z-index: 1;
            width: min(560px, 44vw);
            height: min(560px, 44vw);
            border-radius: 9999px;
            background: rgba(255,255,255,.42);
            filter: blur(70px);
            animation: auraPulse 16s ease-in-out infinite;
        }
        .portal-container { position: relative; z-index: 2; }
        @keyframes auraDrift {
            0% { transform: translate3d(-1.5vw, -1vh, 0) scale(1); }
            50% { transform: translate3d(1vw, 1.4vh, 0) scale(1.03); }
            100% { transform: translate3d(2vw, -.8vh, 0) scale(1.01); }
        }
        @keyframes auraPulse {
            0%, 100% { opacity: .34; transform: translate3d(0, 0, 0) scale(.94); }
            50% { opacity: .72; transform: translate3d(-8vw, 10vh, 0) scale(1.12); }
        }
        @media (prefers-reduced-motion: reduce) {
            .portal-page:before,
            .portal-mesh,
            .portal-aurora span {
                animation: none !important;
            }
        }
        .portal-container { max-width: 1280px; margin: 0 auto; }
        .glass { background: rgba(255,255,255,.4); backdrop-filter: blur(16px); -webkit-backdrop-filter: blur(16px); border: 1px solid rgba(255,255,255,.3); }
        .portal-nav {
            position: fixed;
            left: 50%;
            right: auto;
            top: 20px;
            z-index: 80;
            display: flex;
            align-items: center;
            justify-content: space-between;
            width: calc(100vw - 64px);
            max-width: 1280px;
            transform: translateX(-50%);
            padding: 16px 32px;
            border-radius: 32px;
            background: rgba(255,255,255,.58);
            box-shadow: 0 18px 48px rgba(31,50,68,.14), inset 0 1px 0 rgba(255,255,255,.82);
            backdrop-filter: blur(18px);
            -webkit-backdrop-filter: blur(18px);
        }
        .portal-brand { display: flex; align-items: center; gap: 8px; color: #111827; }
        .portal-brand-mark { width: 32px; height: 32px; border-radius: 8px; display: flex; align-items: center; justify-content: center; background: #000; color: #fff; font-size: 18px; font-weight: 700; }
        .portal-brand strong { font-size: 18px; line-height: 1; letter-spacing: -0.03em; }
        .portal-nav-actions { display: flex; align-items: center; gap: 16px; }
        .portal-nav-btn { border: 0; background: transparent; color: #4b5563; padding: 8px 12px; border-radius: 999px; font-size: 14px; font-weight: 500; cursor: pointer; transition: color .18s ease, background .18s ease; }
        .portal-nav-btn:hover { color: #000; background: rgba(255,255,255,.5); }
        .portal-icon-group { display: flex; align-items: center; gap: 12px; margin-left: 16px; padding-left: 16px; border-left: 1px solid #e5e7eb; }
        .portal-icon-btn { position: relative; width: 36px; height: 36px; border: 0; border-radius: 999px; display: inline-flex; align-items: center; justify-content: center; color: #6b7280; background: transparent; cursor: pointer; transition: color .18s ease, background .18s ease; }
        .portal-icon-btn:hover { color: #000; background: rgba(255,255,255,.5); }
        .portal-dot { position: absolute; top: 4px; right: 5px; width: 8px; height: 8px; border-radius: 999px; background: #ef4444; }
        .portal-hero { max-width: 896px; margin: 0 auto 128px; text-align: center; }
        .portal-hero h1 { margin: 0 0 24px; color: #111827; font-size: clamp(48px, 7vw, 72px); line-height: 1.1; font-weight: 700; letter-spacing: -0.055em; }
        .portal-hero h1 span { color: #9ca3af; }
        .portal-hero p { max-width: 672px; margin: 0 auto 48px; color: #6b7280; font-size: 20px; line-height: 1.55; }
        .portal-search-wrap { position: relative; max-width: 576px; margin: 0 auto 32px; }
        .portal-search-icon { position: absolute; left: 24px; top: 50%; transform: translateY(-50%); color: #9ca3af; pointer-events: none; }
        .portal-search { width: 100%; height: 64px; box-sizing: border-box; padding: 0 24px 0 64px; border-radius: 16px; border: 1px solid rgba(255,255,255,.3); outline: 0; color: #111827; font-size: 18px; box-shadow: 0 20px 25px -5px rgba(59,130,246,.05), 0 8px 10px -6px rgba(59,130,246,.05); transition: box-shadow .18s ease, border-color .18s ease; }
        .portal-search:focus { box-shadow: 0 0 0 2px rgba(59,130,246,.2), 0 20px 25px -5px rgba(59,130,246,.05); }
        .portal-primary { border: 0; display: inline-flex; align-items: center; justify-content: center; padding: 16px 40px; border-radius: 16px; background: #007aff; color: #fff; font-weight: 600; box-shadow: 0 10px 15px -3px rgba(59,130,246,.2), 0 4px 6px -4px rgba(59,130,246,.2); cursor: pointer; transition: transform .18s ease, background .18s ease; }
        .portal-primary:hover { transform: scale(1.05); background: #0066d6; }
        .portal-grid { display: grid; grid-template-columns: repeat(4, minmax(0, 1fr)); gap: 24px; margin-bottom: 96px; }
        .portal-card { min-height: 220px; box-sizing: border-box; padding: 24px; border-radius: 24px; opacity: 0; transform: translateY(20px); animation: riseIn .5s ease forwards; }
        .portal-card:nth-child(2) { animation-delay: .1s; }
        .portal-card:nth-child(3) { animation-delay: .2s; }
        .portal-card:nth-child(4) { animation-delay: .3s; }
        @keyframes riseIn { to { opacity: 1; transform: translateY(0); } }
        .portal-card-title { margin: 0 0 24px; color: #111827; font-size: 12px; line-height: 1.2; font-weight: 700; letter-spacing: .2em; text-transform: uppercase; }
        .stat-card { display: flex; flex-direction: column; justify-content: space-between; }
        .stat-title { color: #6b7280; }
        .stat-grid { display: grid; grid-template-columns: repeat(2, minmax(0,1fr)); gap: 16px; margin: 16px 0; }
        .stat-item strong, .stat-bottom strong { display: block; color: #111827; font-size: 30px; line-height: 1; font-weight: 700; letter-spacing: -0.03em; }
        .stat-item span, .stat-bottom span { display: block; margin-top: 5px; color: #6b7280; font-size: 12px; }
        .stat-row { display: flex; align-items: flex-end; justify-content: space-between; margin-top: auto; }
        .spark { width: 96px; height: 48px; color: #3b82f6; }
        .quick-list { display: grid; gap: 8px; }
        .quick-link { width: 100%; border: 0; display: flex; align-items: center; gap: 16px; padding: 12px; border-radius: 16px; background: transparent; text-align: left; cursor: pointer; color: #1f2937; transition: background .18s ease; }
        .quick-link:hover { background: rgba(255,255,255,.4); }
        .quick-icon { width: 40px; height: 40px; flex: 0 0 40px; border-radius: 12px; display: flex; align-items: center; justify-content: center; background: rgba(255,255,255,.5); box-shadow: 0 1px 2px rgba(0,0,0,.05); color: #374151; }
        .quick-link strong { display: block; color: #1f2937; font-size: 14px; font-weight: 500; }
        .quick-link small { display: block; margin-top: 2px; color: #9ca3af; font-size: 10px; letter-spacing: .16em; text-transform: uppercase; }
        .quick-chevron { margin-left: auto; opacity: 0; color: #9ca3af; transition: opacity .18s ease; }
        .quick-link:hover .quick-chevron { opacity: 1; }
        .discovery-list { display: grid; gap: 24px; }
        .discovery-item { display: block; color: inherit; cursor: pointer; }
        .discovery-text { min-width: 0; display: flex; flex-direction: column; justify-content: center; padding: 2px 0; }
        .discovery-text strong { display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; color: #111827; font-size: 14px; line-height: 1.35; font-weight: 600; transition: color .18s ease; }
        .discovery-item:hover strong { color: #2563eb; }
        .discovery-text span { margin-top: 4px; color: #6b7280; font-size: 11px; }
        .discovery-text small { color: #9ca3af; font-size: 10px; font-weight: 500; text-transform: uppercase; }
        .qa-card { position: relative; overflow: hidden; display: flex; flex-direction: column; justify-content: space-between; min-height: 220px; color: inherit; text-decoration: none; padding: 28px; transition: transform .18s ease, border-color .18s ease, background .18s ease; }
        .qa-card:hover { transform: translateY(-3px); border-color: rgba(59,130,246,.34); background: rgba(255,255,255,.52); }
        .qa-card-top { display: flex; align-items: center; justify-content: space-between; gap: 16px; }
        .qa-card .portal-card-title { margin: 0; }
        .qa-mark { width: 52px; height: 52px; border-radius: 16px; display: inline-flex; align-items: center; justify-content: center; color: #fff; background: #111827; box-shadow: 0 16px 32px rgba(17,24,39,.16); }
        .qa-status { display: inline-flex; align-items: center; gap: 6px; color: #059669; font-size: 11px; font-weight: 700; letter-spacing: .08em; text-transform: uppercase; }
        .qa-status:before { content: ""; width: 7px; height: 7px; border-radius: 999px; background: #10b981; box-shadow: 0 0 0 4px rgba(16,185,129,.12); }
        .qa-card h4 { margin: 24px 0 8px; color: #111827; font-size: 22px; line-height: 1.18; font-weight: 800; letter-spacing: 0; }
        .qa-card p { margin: 0; color: #5b6472; font-size: 13px; line-height: 1.7; }
        .qa-pills { display: flex; flex-wrap: wrap; gap: 8px; margin-top: 18px; }
        .qa-pill { display: inline-flex; align-items: center; min-height: 26px; padding: 0 10px; border-radius: 999px; color: #2563eb; background: rgba(59,130,246,.1); font-size: 11px; font-weight: 700; }
        .qa-action { display: flex; align-items: center; justify-content: space-between; margin-top: 24px; color: #111827; font-size: 13px; font-weight: 800; }
        .qa-arrow { width: 34px; height: 34px; border-radius: 999px; display: inline-flex; align-items: center; justify-content: center; color: #fff; background: #2563eb; transition: transform .18s ease, background .18s ease; }
        .qa-card:hover .qa-arrow { transform: translateX(3px); background: #1d4ed8; }
        .portal-footer {
            position: fixed !important;
            left: 50% !important;
            right: auto !important;
            bottom: 20px !important;
            width: calc(100vw - 64px) !important;
            max-width: 1280px;
            transform: translateX(-50%) !important;
            z-index: 80 !important;
            border: 1px solid rgba(255,255,255,.45);
            border-radius: 28px;
            background: rgba(255,255,255,.58) !important;
            box-shadow: 0 18px 48px rgba(31,50,68,.14), inset 0 1px 0 rgba(255,255,255,.82);
            backdrop-filter: blur(18px);
            -webkit-backdrop-filter: blur(18px);
            padding: 10px 24px;
        }
        .portal-footer-row { max-width: 1280px; margin: 0 auto; display: flex; align-items: center; justify-content: space-between; gap: 24px; white-space: nowrap; }
        .portal-footer-links { display: flex; align-items: center; gap: 22px; color: #6b7280; font-size: 12px; font-weight: 500; }
        .portal-footer-links a { color: #6b7280; }
        .portal-social { display: flex; align-items: center; gap: 14px; }
        .portal-social a { display: inline-flex; align-items: center; justify-content: center; color: #9ca3af; padding: 4px; transition: color .18s ease; }
        .portal-social a:hover { color: #000; }
        .portal-copy { margin: 0; color: #9ca3af; text-align: center; font-size: 10px; font-weight: 600; letter-spacing: .18em; text-transform: uppercase; white-space: nowrap; }
        .lit-home footer:not(.portal-footer) { display: none; }
        @media (max-width: 1100px) { .portal-grid { grid-template-columns: repeat(2, minmax(0, 1fr)); } }
        @media (max-width: 760px) {
            .portal-page { padding: 104px 16px 230px; }
            .portal-nav { width: calc(100vw - 24px); top: 10px; padding: 14px 18px; }
            .portal-nav-actions { display: none; }
            .portal-hero { margin-bottom: 72px; }
            .portal-hero h1 { font-size: 46px; }
            .portal-hero p { font-size: 17px; }
            .portal-grid { grid-template-columns: 1fr; }
            .portal-footer-row { flex-direction: column; white-space: normal; }
            .portal-footer-links { flex-wrap: wrap; justify-content: center; }
            .portal-footer { width: calc(100vw - 24px) !important; bottom: 10px !important; padding: 10px 14px; }
        }
    </style>
</head>

<body class="ac lit-home">
    <div class="lit-hidden-shell">
        <LiteratureManager:top ID="top" runat="server" />
    </div>
    <div class="portal-page">
        <div class="portal-aurora" aria-hidden="true">
            <span></span>
            <span></span>
            <span></span>
            <span></span>
        </div>
        <div class="portal-mesh"></div>
        <div class="portal-container">
            <header class="portal-nav glass">
                <a class="portal-brand" href="/">
                    <span class="portal-brand-mark">A</span>
                    <strong>Academic Portal</strong>
                </a>
                <nav class="portal-nav-actions">
                    <button type="button" class="portal-nav-btn" onclick="<%=IsLogin?"openHeaderNoticeModal();return false;":"$('.loginBut').first().click();return false;" %>">&#28040;&#24687;</button>
                    <button type="button" class="portal-nav-btn" onclick="<%=IsLogin?"openHeaderTopupModal();return false;":"$('.loginBut').first().click();return false;" %>">&#31215;&#20998;</button>
                    <button type="button" class="portal-nav-btn" onclick="<%=IsLogin?"window.location.href='/User/LiteratureUpload';return false;":"$('.loginBut').first().click();return false;" %>">&#19978;&#20256;</button>
                    <div class="portal-icon-group">
                        <button type="button" class="portal-icon-btn" onclick="<%=IsLogin?"window.location.href='/User/Center';return false;":"$('.loginBut').first().click();return false;" %>" aria-label="user">
                            <svg width="20" height="20" viewBox="0 0 24 24" fill="none"><path d="M20 21a8 8 0 0 0-16 0M12 13a5 5 0 1 0 0-10 5 5 0 0 0 0 10Z" stroke="currentColor" stroke-width="2" stroke-linecap="round"/></svg>
                            <%if (IsLogin) { %><span class="portal-dot"></span><% } %>
                        </button>
                        <button type="button" class="portal-icon-btn" onclick="document.getElementById('home_keyword').focus();return false;" aria-label="search">
                            <svg width="20" height="20" viewBox="0 0 24 24" fill="none"><path d="M10.8 18.2a7.4 7.4 0 1 1 0-14.8 7.4 7.4 0 0 1 0 14.8Zm5.6-1.8 4.2 4.2" stroke="currentColor" stroke-width="2" stroke-linecap="round"/></svg>
                        </button>
                    </div>
                </nav>
            </header>

            <section class="portal-hero">
                <h1>Unlocking the World&apos;s <br />Knowledge, <span>Redefined.</span></h1>
                <p>Your gateway to advanced academic literature, research, and collaborative discovery, elevated by premium design.</p>
                <div class="portal-search-wrap">
                    <span class="portal-search-icon">
                        <svg width="20" height="20" viewBox="0 0 24 24" fill="none"><path d="M10.8 18.2a7.4 7.4 0 1 1 0-14.8 7.4 7.4 0 0 1 0 14.8Zm5.6-1.8 4.2 4.2" stroke="currentColor" stroke-width="2" stroke-linecap="round"/></svg>
                    </span>
                    <input class="portal-search glass" type="text" id="home_keyword" placeholder="&#25628;&#32034;&#26631;&#39064;&#12289;&#20316;&#32773;&#25110;&#20027;&#39064;..." />
                </div>
                <button type="button" class="portal-primary" onclick="window.location.href='/LiteratureSearch.aspx'">&#27983;&#35272;&#20840;&#37096;&#30740;&#31350;</button>
            </section>

            <section class="portal-grid">
                <div class="portal-card glass stat-card">
                    <h3 class="portal-card-title stat-title">&#25968;&#25454;&#27010;&#35272;</h3>
                    <div class="stat-grid">
                        <div class="stat-item"><strong><%=literatureCount %>+</strong><span>&#24050;&#32034;&#24341;&#25991;&#31456;</span></div>
                        <div class="stat-item"><strong><%=authorCount %>+</strong><span>&#23398;&#32773;&#20837;&#39547;</span></div>
                    </div>
                    <div class="stat-row">
                        <div class="stat-bottom"><strong><%=categoryCount %>+</strong><span>&#23398;&#31185;&#31867;&#30446;</span></div>
                        <svg class="spark" viewBox="0 0 100 40" fill="none"><path d="<%=LiteratureSparklinePath %>" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"/></svg>
                    </div>
                </div>

                <div class="portal-card glass">
                    <h3 class="portal-card-title">&#24555;&#36895;&#20837;&#21475;</h3>
                    <div class="quick-list">
                        <a class="quick-link" href="/LiteratureSearch.aspx"><span class="quick-icon"><svg width="20" height="20" viewBox="0 0 24 24" fill="none"><path d="M10.8 18.2a7.4 7.4 0 1 1 0-14.8 7.4 7.4 0 0 1 0 14.8Zm5.6-1.8 4.2 4.2" stroke="currentColor" stroke-width="2" stroke-linecap="round"/></svg></span><span><strong>&#36827;&#20837;&#25991;&#29486;&#26816;&#32034;&#39029;&#38754;</strong></span><span class="quick-chevron">›</span></a>
                        <a class="quick-link" href="/Website/news"><span class="quick-icon"><svg width="20" height="20" viewBox="0 0 24 24" fill="none"><path d="M4 19.5V5a2 2 0 0 1 2-2h12v18H6a2 2 0 0 1-2-1.5Z" stroke="currentColor" stroke-width="2"/><path d="M8 7h6M8 11h6" stroke="currentColor" stroke-width="2" stroke-linecap="round"/></svg></span><span><strong>&#27983;&#35272;&#23398;&#26415;&#36164;&#35759;</strong></span><span class="quick-chevron">›</span></a>
                        <a class="quick-link <%=IsLogin?"":"loginBut" %>" <%=IsLogin?"href=\"/User/LiteratureUpload\"":"href=\"javascript:void(0);\"" %>><span class="quick-icon"><svg width="20" height="20" viewBox="0 0 24 24" fill="none"><path d="M12 16V4m0 0 4 4m-4-4-4 4M4 16v3a1 1 0 0 0 1 1h14a1 1 0 0 0 1-1v-3" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg></span><span><strong>&#19978;&#20256;&#25991;&#29486;</strong></span><span class="quick-chevron">›</span></a>
                        <a class="quick-link <%=IsLogin?"":"loginBut" %>" <%=IsLogin?"href=\"/User/IntegrateExchange\"":"href=\"javascript:void(0);\"" %>><span class="quick-icon"><svg width="20" height="20" viewBox="0 0 24 24" fill="none"><path d="M20 12v8H4v-8m16 0H4m16 0h-7m-9 0h7m1 0V8m0 4v8M7.5 8A2.5 2.5 0 1 1 12 6.5 2.5 2.5 0 1 1 16.5 8" stroke="currentColor" stroke-width="2" stroke-linecap="round"/></svg></span><span><strong>&#26597;&#30475;&#31215;&#20998;&#19982;&#20817;&#25442;</strong></span><span class="quick-chevron">›</span></a>
                    </div>
                </div>

                <div class="portal-card glass">
                    <h3 class="portal-card-title">&#26368;&#26032;&#21457;&#29616;</h3>
                    <div class="discovery-list">
                        <asp:Repeater ID="FeaturedLiteratureList" runat="server">
                            <ItemTemplate>
                                <a class="discovery-item" href="/LiteratureInfo.aspx?id=<%# Eval("id") %>">
                                    <span class="discovery-text">
                                        <strong><%# Function.HtmlDiscode(Eval("title").ToString()) %></strong>
                                        <span>By <%# GetShortAuthor(Eval("author_names")) %></span>
                                        <small><%# GetLiteratureCardMeta(Eval("author_names"), Eval("publish_year"), Eval("source_type")) %></small>
                                    </span>
                                </a>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <a class="portal-card glass qa-card" href="/LiteratureQA.aspx">
                    <div class="qa-card-top">
                        <h3 class="portal-card-title">&#26234;&#33021;&#38382;&#31572;</h3>
                        <span class="qa-status">RAG</span>
                    </div>
                    <div>
                        <span class="qa-mark" aria-hidden="true">
                            <svg width="28" height="28" viewBox="0 0 24 24" fill="none"><path d="M4 6.5A3.5 3.5 0 0 1 7.5 3h9A3.5 3.5 0 0 1 20 6.5v6A3.5 3.5 0 0 1 16.5 16H12l-4.6 4.2A.8.8 0 0 1 6 19.6V16A3.5 3.5 0 0 1 4 12.5v-6Z" stroke="currentColor" stroke-width="1.8" stroke-linejoin="round"/><path d="M8 8h8M8 11.5h5" stroke="currentColor" stroke-width="1.8" stroke-linecap="round"/></svg>
                        </span>
                        <h4>&#25991;&#29486;&#26234;&#33021;&#38382;&#31572;</h4>
                        <p>&#36830;&#25509;&#24050;&#32034;&#24341;&#25991;&#29486;&#65292;&#24555;&#36895;&#26597;&#25214;&#31572;&#26696;&#19982;&#20381;&#25454;&#12290;</p>
                        <div class="qa-pills">
                            <span class="qa-pill">&#25688;&#35201;</span>
                            <span class="qa-pill">DOI</span>
                            <span class="qa-pill">&#20316;&#32773;</span>
                        </div>
                    </div>
                    <div class="qa-action">
                        <span>&#36827;&#20837; RAG &#38382;&#31572;</span>
                        <span class="qa-arrow" aria-hidden="true">&#8594;</span>
                    </div>
                </a>
            </section>

            <footer class="portal-footer">
                <div class="portal-footer-row">
                    <div class="portal-footer-links">
                        <a href="javascript:void(0);" data-footer-modal="about">&#20851;&#20110;</a>
                        <a href="javascript:void(0);" data-footer-modal="contact">&#32852;&#31995;&#25105;&#20204;</a>
                        <a href="javascript:void(0);" data-footer-modal="privacy">&#38544;&#31169;</a>
                        <a href="javascript:void(0);" data-footer-modal="terms">&#26465;&#27454;</a>
                        <a href="javascript:void(0);" data-footer-modal="support">&#25903;&#25345;</a>
                    </div>
                    <div class="portal-copy">© 2026 Academic Portal. Designed for Knowledge.</div>
                    <div class="portal-social">
                        <a aria-label="GitHub" href="https://github.com/LuBei1022/db-project-2026/" target="_blank" rel="noopener noreferrer"><svg width="18" height="18" viewBox="0 0 24 24" fill="currentColor"><path d="M12 .5a12 12 0 0 0-3.8 23.4c.6.1.8-.2.8-.6v-2.1c-3.3.7-4-1.4-4-1.4-.5-1.2-1.2-1.6-1.2-1.6-1-.7.1-.7.1-.7 1.1.1 1.7 1.2 1.7 1.2 1 .1.6 2.7 3.5 1.9.1-.7.4-1.2.7-1.5-2.6-.3-5.4-1.3-5.4-5.9 0-1.3.5-2.4 1.2-3.2-.1-.3-.5-1.6.1-3.2 0 0 1-.3 3.3 1.2a11.5 11.5 0 0 1 6 0c2.3-1.5 3.3-1.2 3.3-1.2.6 1.6.2 2.9.1 3.2.8.8 1.2 1.9 1.2 3.2 0 4.6-2.8 5.6-5.4 5.9.4.4.8 1 .8 2v3c0 .4.2.7.8.6A12 12 0 0 0 12 .5Z"/></svg></a>
                    </div>
                </div>
            </footer>
        </div>
    </div>
    <div class="lit-hidden-shell">
        <LiteratureManager:foot ID="foot" runat="server" />
    </div>
    <script type="text/javascript">
        function goHomeLiteratureSearch() {
            var keyword = ($("#home_keyword").val() || "").trim();
            if (!keyword) {
                window.location.href = "/LiteratureSearch.aspx";
                return;
            }
            window.location.href = "/LiteratureSearch.aspx?keyword=" + encodeURIComponent(keyword);
        }
        $("#home_keyword").on("keypress", function (e) {
            if (e.keyCode === 13) {
                goHomeLiteratureSearch();
            }
        });
    </script>
</body>

</html>
