<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="foot.ascx.cs" Inherits="Web.foot" %>
<%@ Register TagPrefix="LiteratureManager" TagName="js" Src="/js.ascx" %>
<footer>
    <div class="w1920 portal-foot-row">
        <div class="footer-l portal-foot-links">
            <a href="javascript:void(0);" data-footer-modal="about">关于</a>
            <a href="javascript:void(0);" data-footer-modal="contact">联系我们</a>
            <a href="javascript:void(0);" data-footer-modal="privacy">隐私</a>
            <a href="javascript:void(0);" data-footer-modal="terms">条款</a>
            <a href="javascript:void(0);" data-footer-modal="support">支持</a>
        </div>
        <p class="portal-foot-copy">© 2026 Academic Portal. Designed for Knowledge.</p>
        <div class="footer-r portal-foot-social">
            <% if (FooterGitHubVisible) { %>
            <a aria-label="GitHub" href="<%=Server.HtmlEncode(FooterGitHubHref) %>" target="_blank" rel="noopener noreferrer"><svg width="18" height="18" viewBox="0 0 24 24" fill="currentColor"><path d="M12 .5a12 12 0 0 0-3.8 23.4c.6.1.8-.2.8-.6v-2.1c-3.3.7-4-1.4-4-1.4-.5-1.2-1.2-1.6-1.2-1.6-1-.7.1-.7.1-.7 1.1.1 1.7 1.2 1.7 1.2 1 .1.6 2.7 3.5 1.9.1-.7.4-1.2.7-1.5-2.6-.3-5.4-1.3-5.4-5.9 0-1.3.5-2.4 1.2-3.2-.1-.3-.5-1.6.1-3.2 0 0 1-.3 3.3 1.2a11.5 11.5 0 0 1 6 0c2.3-1.5 3.3-1.2 3.3-1.2.6 1.6.2 2.9.1 3.2.8.8 1.2 1.9 1.2 3.2 0 4.6-2.8 5.6-5.4 5.9.4.4.8 1 .8 2v3c0 .4.2.7.8.6A12 12 0 0 0 12 .5Z"/></svg></a>
            <% } %>
        </div>
    </div>
</footer>
<div class="portal-footer-modal" aria-hidden="true">
    <div class="portal-footer-modal-backdrop" data-footer-close></div>
    <section class="portal-footer-modal-panel" role="dialog" aria-modal="true" aria-labelledby="portalFooterModalTitle">
        <button type="button" class="portal-footer-modal-close" data-footer-close aria-label="关闭">×</button>
        <p class="portal-footer-modal-kicker">Academic Portal</p>
        <h2 id="portalFooterModalTitle"></h2>
        <div class="portal-footer-modal-body"></div>
    </section>
</div>
<div class="portal-footer-modal-source" hidden>
    <article data-footer-content="about" data-title="关于"><%=FooterAboutContent %></article>
    <article data-footer-content="contact" data-title="联系我们"><%=FooterContactContent %></article>
    <article data-footer-content="privacy" data-title="隐私"><%=FooterPrivacyContent %></article>
    <article data-footer-content="terms" data-title="条款"><%=FooterTermsContent %></article>
    <article data-footer-content="support" data-title="支持"><%=FooterSupportContent %></article>
</div>
<div style="display:none;">
    <asp:Repeater ID="MenuClassList" runat="server"></asp:Repeater>
    <asp:Repeater ID="ImgLinkList" runat="server"></asp:Repeater>
</div>

<div class="rightBox">
    <div class="returnB">
        <div class="frightItem" id="btt">
            <img src="/images/shang.png" />
        </div>
    </div>
</div>
<script>
    var btn = document.getElementById('btt');
    var timer = null;
    var isTop = true;
    window.onscroll = function () {
        if (!isTop) {
            clearInterval(timer);
        }
        isTop = false;
        if ($(document).scrollTop() < 61) {
            $(".rightBox").hide(500);
            $('.header').removeClass('positioning')
            $('.middle').css('padding-top', '0px')
        } else {
            $(".rightBox").show(500);
            $('.header').addClass('positioning')
            $('.middle').css('padding-top', '61px')
        }
    }

    btn.onclick = function () {
        timer = setInterval(function () {
            var osTop = document.documentElement.scrollTop || document.body.scrollTop;
            var ispeed = Math.floor(-osTop / 7);
            document.documentElement.scrollTop = document.body.scrollTop = osTop + ispeed;
            if (osTop == 0) {
                clearInterval(timer);
            }
            isTop = true;
        }, 30);
    };
    (function () {
        var modal = document.querySelector('.portal-footer-modal');
        if (!modal) return;
        var titleNode = modal.querySelector('#portalFooterModalTitle');
        var bodyNode = modal.querySelector('.portal-footer-modal-body');
        var sourceRoot = document.querySelector('.portal-footer-modal-source');
        function openFooterModal(key) {
            var source = sourceRoot && sourceRoot.querySelector('[data-footer-content="' + key + '"]');
            if (!source) return;
            titleNode.textContent = source.getAttribute('data-title') || '';
            bodyNode.innerHTML = source.innerHTML || '<p>暂无内容。</p>';
            modal.classList.add('is-open');
            modal.setAttribute('aria-hidden', 'false');
            document.documentElement.classList.add('portal-modal-lock');
        }
        function closeFooterModal() {
            modal.classList.remove('is-open');
            modal.setAttribute('aria-hidden', 'true');
            document.documentElement.classList.remove('portal-modal-lock');
        }
        document.addEventListener('click', function (event) {
            var trigger = event.target.closest && event.target.closest('[data-footer-modal]');
            if (trigger) {
                event.preventDefault();
                openFooterModal(trigger.getAttribute('data-footer-modal'));
                return;
            }
            if (event.target.closest && event.target.closest('[data-footer-close]')) {
                event.preventDefault();
                closeFooterModal();
            }
        });
        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape' && modal.classList.contains('is-open')) {
                closeFooterModal();
            }
        });
    })();
   </script>

<style>
    .footer-l p span {
        color: #999999;
    }
    .portal-modal-lock {
        overflow: hidden !important;
    }
    .portal-footer-modal {
        position: fixed;
        inset: 0;
        z-index: 5000;
        display: none;
        align-items: center;
        justify-content: center;
        padding: 88px 24px;
        box-sizing: border-box;
    }
    .portal-footer-modal.is-open {
        display: flex;
    }
    .portal-footer-modal-backdrop {
        position: absolute;
        inset: 0;
        background: rgba(245,245,247,.42);
        backdrop-filter: saturate(180%) blur(22px);
        -webkit-backdrop-filter: saturate(180%) blur(22px);
    }
    .portal-footer-modal-panel {
        position: relative;
        z-index: 1;
        width: min(820px, calc(100vw - 48px));
        max-height: min(720px, calc(100vh - 136px));
        overflow: auto;
        padding: 48px 56px 56px;
        box-sizing: border-box;
        border: 1px solid rgba(0,0,0,.08);
        border-radius: 18px;
        background: rgba(255,255,255,.9);
        color: #1d1d1f;
        box-shadow: none;
    }
    .portal-footer-modal-close {
        position: absolute;
        right: 24px;
        top: 20px;
        width: 36px;
        height: 36px;
        border: 0;
        border-radius: 9999px;
        background: #f5f5f7;
        color: #6e6e73;
        font-size: 24px;
        line-height: 36px;
        cursor: pointer;
    }
    .portal-footer-modal-kicker {
        margin: 0 0 14px !important;
        color: #7a7a7a !important;
        font-size: 12px !important;
        font-weight: 600 !important;
        line-height: 1 !important;
        letter-spacing: .12em !important;
        text-transform: uppercase;
    }
    .portal-footer-modal-panel h2 {
        margin: 0 0 28px !important;
        color: #1d1d1f !important;
        font-family: "SF Pro Display", "PingFang SC", "Microsoft YaHei", system-ui, -apple-system, BlinkMacSystemFont, sans-serif !important;
        font-size: clamp(30px, 3.2vw, 40px) !important;
        font-weight: 600 !important;
        line-height: 1.1 !important;
        letter-spacing: 0 !important;
    }
    .portal-footer-modal-body {
        padding-top: 28px;
        border-top: 1px solid #e0e0e0;
    }
    .portal-footer-modal-body,
    .portal-footer-modal-body p,
    .portal-footer-modal-body div,
    .portal-footer-modal-body li {
        color: #1d1d1f !important;
        font-size: 17px !important;
        font-weight: 400 !important;
        line-height: 1.47 !important;
        letter-spacing: -0.374px !important;
    }
    .portal-footer-modal-body p {
        margin: 0 0 16px !important;
    }
    .portal-footer-modal-body h1,
    .portal-footer-modal-body h2,
    .portal-footer-modal-body h3,
    .portal-footer-modal-body h4 {
        margin: 24px 0 12px !important;
        color: #1d1d1f !important;
        font-size: 21px !important;
        font-weight: 600 !important;
        line-height: 1.19 !important;
        letter-spacing: .231px !important;
    }
    .portal-footer-modal-body a {
        color: #0066cc !important;
    }
    .portal-footer-modal-body img {
        max-width: 100% !important;
        height: auto !important;
        border-radius: 8px !important;
    }
    body.ac:not(.lit-home) footer,
    body.front-glass footer {
        position: fixed !important;
        left: 50% !important;
        right: auto !important;
        bottom: 20px !important;
        width: calc(100vw - 64px) !important;
        max-width: 1280px !important;
        height: auto !important;
        min-height: 56px !important;
        transform: translateX(-50%) !important;
        z-index: 900 !important;
        padding: 10px 24px !important;
        box-sizing: border-box !important;
        border: 1px solid rgba(0,0,0,.08) !important;
        border-radius: 28px !important;
        background: rgba(245,245,247,.86) !important;
        box-shadow: none !important;
        backdrop-filter: saturate(180%) blur(20px) !important;
        -webkit-backdrop-filter: saturate(180%) blur(20px) !important;
    }
    body.ac:not(.lit-home) footer .portal-foot-row,
    body.front-glass footer .portal-foot-row {
        width: 100% !important;
        min-height: 36px !important;
        padding: 0 !important;
        display: flex !important;
        align-items: center !important;
        justify-content: space-between !important;
        gap: 24px !important;
        white-space: nowrap !important;
    }
    body.ac:not(.lit-home) .portal-foot-links,
    body.front-glass .portal-foot-links,
    body.ac:not(.lit-home) .portal-foot-social,
    body.front-glass .portal-foot-social {
        display: flex !important;
        align-items: center !important;
        gap: 22px !important;
        flex-wrap: nowrap !important;
    }
    body.ac:not(.lit-home) .portal-foot-links a,
    body.front-glass .portal-foot-links a {
        color: #6b7280 !important;
        font-size: 12px !important;
        font-weight: 500 !important;
        line-height: 1 !important;
    }
    body.ac:not(.lit-home) .portal-foot-copy,
    body.front-glass .portal-foot-copy {
        margin: 0 !important;
        color: #9ca3af !important;
        text-align: center !important;
        font-size: 10px !important;
        font-weight: 600 !important;
        letter-spacing: .18em !important;
        text-transform: uppercase !important;
        white-space: nowrap !important;
    }
    body.ac:not(.lit-home) .portal-foot-social a,
    body.front-glass .portal-foot-social a {
        display: inline-flex !important;
        align-items: center !important;
        justify-content: center !important;
        color: #9ca3af !important;
        font-weight: 800 !important;
        line-height: 1 !important;
    }
    @media (max-width: 760px) {
        .portal-footer-modal {
            padding: 72px 16px;
        }
        .portal-footer-modal-panel {
            width: calc(100vw - 32px);
            max-height: calc(100vh - 112px);
            padding: 40px 24px 44px;
        }
        body.ac:not(.lit-home) footer,
        body.front-glass footer {
            width: calc(100vw - 24px) !important;
            bottom: 10px !important;
            padding: 10px 14px !important;
        }
        body.ac:not(.lit-home) footer .portal-foot-row,
        body.front-glass footer .portal-foot-row {
            flex-wrap: wrap !important;
            justify-content: center !important;
            white-space: normal !important;
        }
    }
</style>

<LiteratureManager:js ID="js" runat="server" />
