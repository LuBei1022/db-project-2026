<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="top.ascx.cs" Inherits="Web.top" %>
<%@ Register TagPrefix="LiteratureManager" TagName="code_tel" Src="/code_tel.ascx" %>
<style>
    .header .w1920 {
        gap: 18px;
    }
    .header-l {
        min-width: 0;
        flex: 1 1 auto;
    }
    .header-l ul {
        display: flex;
        flex-wrap: nowrap;
        gap: 0;
        white-space: nowrap;
    }
    .header-l ul li {
        margin-right: 22px;
    }
    .header-l ul li a {
        cursor: pointer;
        font-size: 14px;
    }
    .header .logo {
        margin-right: 24px;
        flex: 0 0 auto;
    }
    .header .logo a {
        display: inline-flex;
        align-items: center;
        gap: 10px;
        text-decoration: none;
    }
    .header .logo-mark {
        width: 36px;
        height: 36px;
        border-radius: 50%;
        position: relative;
        background: linear-gradient(135deg, #2b69b3 0%, #112c54 100%);
        flex: 0 0 36px;
        box-shadow: 0 10px 20px rgba(17, 44, 84, 0.18);
    }
    .header .logo-mark:before,
    .header .logo-mark:after {
        content: "";
        position: absolute;
        top: 9px;
        width: 9px;
        height: 16px;
        border: 2px solid #fff;
        border-top: 0;
        border-radius: 0 0 8px 8px;
    }
    .header .logo-mark:before {
        left: 9px;
        border-right: 0;
    }
    .header .logo-mark:after {
        right: 9px;
        border-left: 0;
    }
    .header .logo-mark i {
        position: absolute;
        left: 17px;
        top: 9px;
        width: 2px;
        height: 17px;
        background: #fff;
        border-radius: 2px;
    }
    .header .logo-text {
        display: flex;
        flex-direction: column;
        line-height: 1.1;
    }
    .header .logo-text strong {
        font-size: 17px;
        color: #182334;
        font-weight: 700;
        letter-spacing: 0.5px;
    }
    .header .logo-text span {
        margin-top: 3px;
        font-size: 10px;
        color: #6b7f96;
        text-transform: uppercase;
        letter-spacing: 1.1px;
    }
    .header-m {
        flex: 0 1 330px;
        max-width: 330px;
    }
    .header-r {
        flex: 0 0 auto;
    }
    .header-integrate-count {
        margin-left: 3px;
        color: #1d6fdc;
        font-weight: 700;
    }
    .header-integrate-item {
        display: inline-flex;
        align-items: center;
        flex-wrap: nowrap;
        white-space: nowrap;
    }
    .header-integrate-item a {
        display: inline-flex;
        align-items: center;
        white-space: nowrap;
    }
    .header-notice-link {
        position: relative;
    }
    .header-notice-dot {
        position: absolute;
        top: -3px;
        right: -7px;
        width: 8px;
        height: 8px;
        border-radius: 50%;
        background: #e53935;
        box-shadow: 0 0 0 2px #fff;
    }
    .header-integrate-plus {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        width: 18px;
        height: 18px;
        margin-left: 6px;
        border: 0;
        border-radius: 50%;
        background: #1d6fdc;
        color: #fff;
        font-weight: 700;
        line-height: 1;
        cursor: pointer;
        flex: 0 0 18px;
    }
    .lm-float-mask {
        position: fixed;
        inset: 0;
        z-index: 20000;
        display: none;
        align-items: center;
        justify-content: center;
        padding: 24px;
        background: rgba(18, 28, 40, .38);
        backdrop-filter: blur(10px);
        -webkit-backdrop-filter: blur(10px);
    }
    .lm-float-mask.show {
        display: flex;
    }
    .lm-float-panel {
        width: min(760px, calc(100vw - 40px));
        max-height: min(760px, calc(100vh - 60px));
        overflow: hidden;
        border-radius: 18px;
        background: #fff;
        box-shadow: 0 28px 90px rgba(13, 31, 52, .26);
    }
    .lm-float-head {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 18px 22px;
        border-bottom: 1px solid #edf1f5;
    }
    .lm-float-head h3 {
        margin: 0;
        font-size: 20px;
        color: #17283b;
    }
    .lm-float-close {
        border: 0;
        background: transparent;
        color: #667586;
        font-size: 24px;
        cursor: pointer;
    }
    .lm-float-body {
        padding: 20px 22px 24px;
        max-height: calc(100vh - 150px);
        overflow: auto;
    }
    .lm-topup-grid {
        display: grid;
        gap: 16px;
    }
    .lm-topup-summary {
        display: grid;
        grid-template-columns: repeat(3, minmax(0, 1fr));
        gap: 12px;
    }
    .lm-topup-summary .metric {
        padding: 14px 16px;
        border: 1px solid #e5edf5;
        border-radius: 12px;
        background: #f8fbff;
        color: #607084;
    }
    .lm-topup-summary .metric strong {
        display: block;
        margin-top: 6px;
        color: #153a68;
        font-size: 24px;
        line-height: 1.2;
    }
    .lm-topup-panel {
        padding: 18px;
        border: 1px solid #e8eef5;
        border-radius: 14px;
        background: #fff;
    }
    .lm-topup-panel h4 {
        margin: 0 0 8px;
        color: #17283b;
        font-size: 18px;
    }
    .lm-topup-panel p {
        margin: 0;
        color: #69788b;
        line-height: 1.8;
    }
    .lm-topup-options {
        display: flex;
        flex-wrap: wrap;
        gap: 10px;
        margin-top: 16px;
    }
    .lm-topup-option {
        min-width: 96px;
        height: 42px;
        padding: 0 16px;
        border: 1px solid #dbe7f2;
        border-radius: 10px;
        background: #f8fbff;
        color: #204a7b;
        cursor: pointer;
    }
    .lm-topup-option.current {
        border-color: #1d6fdc;
        background: #eaf3ff;
        color: #1d6fdc;
        font-weight: 700;
    }
    .lm-topup-custom {
        display: flex;
        gap: 12px;
        align-items: center;
        margin-top: 14px;
        flex-wrap: wrap;
    }
    .lm-topup-custom label {
        color: #25384d;
        font-weight: 600;
    }
    .lm-topup-custom input {
        width: 230px;
        height: 42px;
        border: 1px solid #d7e0ea;
        border-radius: 10px;
        padding: 0 14px;
    }
    .lm-topup-submit {
        display: flex;
        align-items: center;
        gap: 12px;
        margin-top: 16px;
        flex-wrap: wrap;
    }
    .lm-topup-submit button {
        min-width: 136px;
        height: 42px;
        border: 0;
        border-radius: 10px;
        background: #1d6fdc;
        color: #fff;
        cursor: pointer;
    }
    .lm-topup-submit button:disabled {
        opacity: .65;
        cursor: not-allowed;
    }
    .lm-topup-submit span {
        color: #7a8795;
        font-size: 13px;
    }
    .lm-topup-pay {
        display: none;
        padding: 18px;
        border: 1px solid #e8eef5;
        border-radius: 14px;
        background: #fbfdff;
        text-align: center;
    }
    .lm-topup-pay.show {
        display: block;
    }
    .lm-topup-qrcode {
        width: 220px;
        height: 220px;
        margin: 0 auto 14px;
        display: flex;
        align-items: center;
        justify-content: center;
        border: 1px solid #eef3f8;
        border-radius: 16px;
        background: #fff;
    }
    .lm-topup-qrcode img {
        max-width: 100%;
    }
    .lm-topup-status {
        margin-top: 10px;
        color: #2b4a6a;
    }
    .lm-topup-meta {
        margin-top: 10px;
        color: #6c7b8d;
        font-size: 13px;
        line-height: 1.9;
        word-break: break-all;
    }
    .lm-modal-notice-list {
        display: grid;
        gap: 18px;
    }
    .lm-modal-notice-section {
        display: grid;
        gap: 10px;
    }
    .lm-modal-notice-section-head {
        display: flex;
        align-items: center;
        justify-content: space-between;
        color: #1d1d1f;
        font-size: 15px;
        font-weight: 700;
    }
    .lm-modal-notice-section-head span {
        color: #86868b;
        font-size: 12px;
        font-weight: 400;
    }
    .lm-modal-notice-item {
        padding: 16px 18px;
        border: 1px solid #e5e5ea;
        border-radius: 18px;
        background: #fff;
    }
    .lm-modal-notice-section.likes .lm-modal-notice-item {
        padding: 13px 16px;
        background: #fbfbfd;
    }
    .lm-modal-notice-item h4 {
        margin: 0 0 8px;
        color: #17283b;
        font-size: 17px;
    }
    .lm-modal-notice-item .time {
        margin: 0 0 10px;
        color: #7b8795;
        font-size: 13px;
    }
    .lm-modal-notice-item .body {
        color: #34465a;
        line-height: 1.8;
    }
    .lm-modal-notice-item a {
        display: inline-block;
        margin-top: 10px;
        color: #1d6fdc;
    }
    .lm-modal-empty {
        padding: 42px 20px;
        text-align: center;
        color: #7b8795;
    }
    .lm-modal-more {
        display: inline-flex;
        margin-top: 16px;
        color: #1d6fdc;
    }
    @media (max-width: 1500px) {
        .header .logo-text strong {
            font-size: 16px;
        }
        .header .logo-text span {
            font-size: 9px;
        }
        .header-l ul li {
            margin-right: 18px;
        }
        .header-m {
            flex-basis: 280px;
            max-width: 280px;
        }
    }
    @media (max-width: 1280px) {
        .header .logo-text span {
            display: none;
        }
        .header-l ul li {
            margin-right: 14px;
        }
        .header-m {
            flex-basis: 240px;
            max-width: 240px;
        }
        .header-r .header-item {
            margin-right: 12px;
        }
    }
    @media (max-width: 768px) {
        .header .logo {
            margin-right: 12px;
        }
        .header .logo-text strong {
            font-size: 15px;
        }
        .header .logo-text span {
            display: none;
        }
        .lm-topup-summary {
            grid-template-columns: 1fr;
        }
        .lm-topup-custom input {
            width: 100%;
        }
    }
</style>
<style>
    body.ac:not(.lit-home) .header,
    body.front-glass .header {
        position: fixed !important;
        top: 20px !important;
        left: 0 !important;
        right: 0 !important;
        width: 100% !important;
        height: auto !important;
        z-index: 1000 !important;
        padding: 0 32px !important;
        box-sizing: border-box !important;
        background: transparent !important;
        border: 0 !important;
        box-shadow: none !important;
    }
    body.ac:not(.lit-home) .header.positioning,
    body.front-glass .header.positioning {
        top: 20px !important;
        background: transparent !important;
        box-shadow: none !important;
    }
    body.ac:not(.lit-home) .header .w1920,
    body.front-glass .header .w1920 {
        width: calc(100vw - 64px) !important;
        max-width: 1280px !important;
        min-height: 64px !important;
        height: 64px !important;
        margin: 0 auto !important;
        padding: 0 32px !important;
        border-radius: 32px !important;
        background: rgba(245,245,247,.86) !important;
        border: 1px solid rgba(0,0,0,.08) !important;
        box-shadow: none !important;
        backdrop-filter: saturate(180%) blur(20px) !important;
        -webkit-backdrop-filter: saturate(180%) blur(20px) !important;
    }
    body.ac:not(.lit-home) .header .logo,
    body.front-glass .header .logo {
        margin-right: 28px !important;
    }
    body.ac:not(.lit-home) .header .logo a,
    body.front-glass .header .logo a {
        gap: 8px !important;
    }
    body.ac:not(.lit-home) .header .logo-mark,
    body.front-glass .header .logo-mark {
        width: 32px !important;
        height: 32px !important;
        border-radius: 8px !important;
        background: #000 !important;
        box-shadow: none !important;
        display: inline-flex !important;
        align-items: center !important;
        justify-content: center !important;
    }
    body.ac:not(.lit-home) .header .logo-mark:before,
    body.ac:not(.lit-home) .header .logo-mark:after,
    body.front-glass .header .logo-mark:before,
    body.front-glass .header .logo-mark:after {
        display: none !important;
    }
    body.ac:not(.lit-home) .header .logo-mark i,
    body.front-glass .header .logo-mark i {
        position: static !important;
        width: auto !important;
        height: auto !important;
        background: transparent !important;
        color: #fff !important;
        font-size: 18px !important;
        line-height: 1 !important;
        font-style: normal !important;
        font-weight: 800 !important;
    }
    body.ac:not(.lit-home) .header .logo-text strong,
    body.front-glass .header .logo-text strong {
        color: #111827 !important;
        font-size: 21px !important;
        line-height: 1 !important;
        font-weight: 600 !important;
        letter-spacing: 0.231px !important;
    }
    body.ac:not(.lit-home) .header .logo-text span,
    body.front-glass .header .logo-text span {
        display: none !important;
    }
    body.ac:not(.lit-home) .header-l,
    body.front-glass .header-l {
        flex: 1 1 auto !important;
    }
    body.ac:not(.lit-home) .header-l ul,
    body.front-glass .header-l ul {
        display: flex !important;
        align-items: center !important;
        gap: 4px !important;
    }
    body.ac:not(.lit-home) .header-l ul li,
    body.front-glass .header-l ul li {
        margin-right: 0 !important;
        padding: 0 !important;
    }
    body.ac:not(.lit-home) .header-l ul li a,
    body.front-glass .header-l ul li a,
    body.ac:not(.lit-home) .header-item a,
    body.front-glass .header-item a {
        padding: 8px 10px !important;
        border-radius: 999px !important;
        color: #4b5563 !important;
        font-size: 12px !important;
        font-weight: 400 !important;
        line-height: 1 !important;
        letter-spacing: -0.12px !important;
        animation: none !important;
    }
    body.ac:not(.lit-home) .header-l ul li a:hover,
    body.front-glass .header-l ul li a:hover,
    body.ac:not(.lit-home) .header-item a:hover,
    body.front-glass .header-item a:hover {
        color: #000 !important;
        background: rgba(255,255,255,.5) !important;
    }
    body.ac:not(.lit-home) .header-m,
    body.front-glass .header-m {
        flex: 0 1 330px !important;
        max-width: 330px !important;
        height: 38px !important;
        margin-left: auto !important;
        padding-left: 16px !important;
        border-radius: 999px !important;
        background: #fff !important;
        border: 1px solid rgba(0,0,0,.08) !important;
        box-shadow: none !important;
        backdrop-filter: blur(16px) !important;
        -webkit-backdrop-filter: blur(16px) !important;
        order: initial !important;
    }
    body.ac:not(.lit-home) .header-m input,
    body.front-glass .header-m input {
        position: static !important;
        width: calc(100% - 40px) !important;
        height: 36px !important;
        padding: 0 !important;
        opacity: 1 !important;
        pointer-events: auto !important;
        transform: none !important;
        background: transparent !important;
        border: 0 !important;
        box-shadow: none !important;
        font-size: 14px !important;
    }
    body.ac:not(.lit-home) .header-m .sBut,
    body.front-glass .header-m .sBut {
        width: 36px !important;
        height: 36px !important;
        border-radius: 999px !important;
        border: 0 !important;
        background: #111827 !important;
        box-shadow: none !important;
    }
    body.ac:not(.lit-home) .header-m .sBut svg path,
    body.front-glass .header-m .sBut svg path {
        fill: #fff !important;
    }
    body.ac:not(.lit-home) .header-r,
    body.front-glass .header-r {
        display: flex !important;
        align-items: center !important;
        gap: 10px !important;
        flex: 0 0 auto !important;
    }
    body.ac:not(.lit-home) .header-r .header-item,
    body.front-glass .header-r .header-item {
        margin-right: 0 !important;
    }
    body.ac:not(.lit-home) .header-item svg path,
    body.front-glass .header-item svg path {
        fill: #6b7280 !important;
    }
    body.ac:not(.lit-home) .nav_but,
    body.front-glass .nav_but {
        display: none !important;
    }
    body.ac:not(.lit-home) .header-avatar,
    body.front-glass .header-avatar,
    body.ac:not(.lit-home) .header-avatar a,
    body.front-glass .header-avatar a,
    body.ac:not(.lit-home) .header-avatar img,
    body.front-glass .header-avatar img {
        width: 36px !important;
        height: 36px !important;
        border-radius: 999px !important;
    }
    @media (max-width: 1100px) {
        body.ac:not(.lit-home) .header-l ul,
        body.front-glass .header-l ul {
            display: none !important;
        }
    }
    @media (max-width: 760px) {
        body.ac:not(.lit-home) .header,
        body.front-glass .header {
            top: 10px !important;
            padding: 0 12px !important;
        }
        body.ac:not(.lit-home) .header .w1920,
        body.front-glass .header .w1920 {
            width: calc(100vw - 24px) !important;
            padding: 0 18px !important;
        }
        body.ac:not(.lit-home) .header .logo-text strong,
        body.front-glass .header .logo-text strong,
        body.ac:not(.lit-home) .header-r .header-item span,
        body.front-glass .header-r .header-item span,
        body.ac:not(.lit-home) .header-m,
        body.front-glass .header-m {
            display: none !important;
        }
    }

    /* Apple design final content layer. This lives in top.ascx so it wins over page-local styles. */
    body.ac:not(.lit-home) {
        font-family: "SF Pro Text", system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", "Microsoft YaHei", sans-serif !important;
        color: #1d1d1f !important;
        background: #f5f5f7 !important;
    }
    body.ac:not(.lit-home):before,
    body.ac:not(.lit-home):after {
        display: none !important;
    }
    body.ac:not(.lit-home) .middle {
        padding: 128px 32px 150px !important;
        min-height: 100vh !important;
        background: #f5f5f7 !important;
        box-sizing: border-box !important;
    }
    body.ac:not(.lit-home) .w1200 {
        width: 100% !important;
        max-width: 1280px !important;
        margin: 0 auto !important;
    }
    body.ac:not(.lit-home) .accountBox {
        display: grid !important;
        grid-template-columns: 240px minmax(0, 1fr) !important;
        gap: 32px !important;
        width: 100% !important;
        min-height: calc(100vh - 280px) !important;
        background: transparent !important;
        border: 0 !important;
        box-shadow: none !important;
    }
    body.ac:not(.lit-home) .accountL {
        position: sticky !important;
        top: 112px !important;
        width: auto !important;
        height: fit-content !important;
        padding: 24px !important;
        border: 1px solid #e0e0e0 !important;
        border-radius: 18px !important;
        background: #fff !important;
        box-shadow: none !important;
    }
    body.ac:not(.lit-home) .accountL:after {
        display: none !important;
    }
    body.ac:not(.lit-home) .accountL ul {
        display: grid !important;
        gap: 4px !important;
    }
    body.ac:not(.lit-home) .accountL ul li {
        padding: 0 !important;
    }
    body.ac:not(.lit-home) .accountL ul li a {
        padding: 12px 14px !important;
        border-radius: 9999px !important;
        background: transparent !important;
        color: #333 !important;
        font-size: 17px !important;
        font-weight: 400 !important;
        line-height: 1.47 !important;
        letter-spacing: -0.374px !important;
        box-shadow: none !important;
    }
    body.ac:not(.lit-home) .accountL ul li.current a,
    body.ac:not(.lit-home) .accountL ul li a:hover {
        background: #fafafc !important;
        color: #0066cc !important;
    }
    body.ac:not(.lit-home) .accountR {
        width: auto !important;
        margin-left: 0 !important;
        padding: 0 !important;
    }
    body.ac:not(.lit-home) .list-tt h4,
    body.ac:not(.lit-home) .accountR-tt,
    body.ac:not(.lit-home) .lit-upload-hero h1,
    body.ac:not(.lit-home) .lit-hero h1 {
        color: #1d1d1f !important;
        font-family: "SF Pro Display", system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif !important;
        font-size: clamp(40px, 5vw, 56px) !important;
        font-weight: 600 !important;
        line-height: 1.07 !important;
        letter-spacing: -0.28px !important;
    }
    body.ac:not(.lit-home) .list-class,
    body.ac:not(.lit-home) .status-lass,
    body.ac:not(.lit-home) .notice-tabs {
        margin: 0 0 32px !important;
        padding: 0 !important;
        background: transparent !important;
        border: 0 !important;
        box-shadow: none !important;
    }
    body.ac:not(.lit-home) .list-class ul,
    body.ac:not(.lit-home) .status-lass ul {
        display: flex !important;
        gap: 8px !important;
        flex-wrap: wrap !important;
    }
    body.ac:not(.lit-home) .list-class ul li,
    body.ac:not(.lit-home) .status-lass ul li {
        margin: 0 !important;
        padding: 0 !important;
    }
    body.ac:not(.lit-home) .list-class ul li a,
    body.ac:not(.lit-home) .status-lass ul li a,
    body.ac:not(.lit-home) .notice-tabs a {
        display: inline-flex !important;
        align-items: center !important;
        justify-content: center !important;
        min-height: 44px !important;
        padding: 0 18px !important;
        border-radius: 9999px !important;
        border: 1px solid #e0e0e0 !important;
        background: #fff !important;
        color: #333 !important;
        font-size: 14px !important;
        font-weight: 400 !important;
        line-height: 1.29 !important;
        letter-spacing: -0.224px !important;
    }
    body.ac:not(.lit-home) .list-class ul li.current a,
    body.ac:not(.lit-home) .list-class ul li a:hover,
    body.ac:not(.lit-home) .status-lass ul li.current a,
    body.ac:not(.lit-home) .status-lass ul li a:hover,
    body.ac:not(.lit-home) .notice-tabs a.current {
        border-color: #0066cc !important;
        color: #0066cc !important;
        background: #fff !important;
    }
    body.ac:not(.lit-home) .pe-item,
    body.ac:not(.lit-home) .points-card,
    body.ac:not(.lit-home) .lit-upload-card,
    body.ac:not(.lit-home) .lit-main,
    body.ac:not(.lit-home) .lit-side,
    body.ac:not(.lit-home) .lit-detail-card,
    body.ac:not(.lit-home) .notice-card,
    body.ac:not(.lit-home) .record-list,
    body.ac:not(.lit-home) .order-list,
    body.ac:not(.lit-home) .message-list,
    body.ac:not(.lit-home) .service-add-card,
    body.ac:not(.lit-home) .user-center-card,
    body.ac:not(.lit-home) .center-card {
        padding: 24px !important;
        border-radius: 18px !important;
        border: 1px solid #e0e0e0 !important;
        background: #fff !important;
        box-shadow: none !important;
        backdrop-filter: none !important;
        -webkit-backdrop-filter: none !important;
    }
    body.ac:not(.lit-home) .pe-list {
        display: grid !important;
        grid-template-columns: repeat(3, minmax(0, 1fr)) !important;
        gap: 24px !important;
        padding: 0 !important;
    }
    body.ac:not(.lit-home) .pe-li-item,
    body.ac:not(.lit-home) .exchange-item,
    body.ac:not(.lit-home) .message-item,
    body.ac:not(.lit-home) .record-item,
    body.ac:not(.lit-home) .order-con a {
        padding: 24px !important;
        border-radius: 18px !important;
        border: 1px solid #e0e0e0 !important;
        background: #fff !important;
        box-shadow: none !important;
        transition: transform .18s ease, border-color .18s ease !important;
    }
    body.ac:not(.lit-home) .pe-li-item:hover,
    body.ac:not(.lit-home) .exchange-item:hover,
    body.ac:not(.lit-home) .message-item:hover,
    body.ac:not(.lit-home) .record-item:hover,
    body.ac:not(.lit-home) .order-con a:hover {
        transform: translateY(-2px) !important;
        border-color: #c7c7cc !important;
    }
    body.ac:not(.lit-home) .pe-li-img,
    body.ac:not(.lit-home) .points .pe-li-img,
    body.ac:not(.lit-home) .exchange-img {
        width: 100% !important;
        height: auto !important;
        aspect-ratio: 16 / 9 !important;
        margin: 0 0 20px !important;
        border-radius: 8px !important;
        background: #fafafc !important;
        box-shadow: none !important;
        overflow: hidden !important;
    }
    body.ac:not(.lit-home) .pe-li-img img,
    body.ac:not(.lit-home) .points .pe-li-img img,
    body.ac:not(.lit-home) .exchange-img img {
        width: 100% !important;
        height: 100% !important;
        max-width: none !important;
        max-height: none !important;
        object-fit: contain !important;
        border-radius: 8px !important;
    }
    body.ac:not(.lit-home) .pe-li-item h4,
    body.ac:not(.lit-home) .exchange-text h4,
    body.ac:not(.lit-home) .message-text h4,
    body.ac:not(.lit-home) .lit-item h2 a {
        color: #1d1d1f !important;
        font-size: 17px !important;
        font-weight: 600 !important;
        line-height: 1.24 !important;
        letter-spacing: -0.374px !important;
    }
    body.ac:not(.lit-home) .data-box p,
    body.ac:not(.lit-home) .exchange-text p,
    body.ac:not(.lit-home) .message-text p,
    body.ac:not(.lit-home) .lit-abs,
    body.ac:not(.lit-home) .notice-body,
    body.ac:not(.lit-home) .lit-upload-title p,
    body.ac:not(.lit-home) .lit-upload-hero p,
    body.ac:not(.lit-home) .lit-hero p {
        color: #333 !important;
        font-size: 17px !important;
        font-weight: 400 !important;
        line-height: 1.47 !important;
        letter-spacing: -0.374px !important;
    }
    body.ac:not(.lit-home) .points .status-medium,
    body.ac:not(.lit-home) .status-but a,
    body.ac:not(.lit-home) .pe-item-tt a,
    body.ac:not(.lit-home) .lit-download-form button,
    body.ac:not(.lit-home) .lit-detail-actions a,
    body.ac:not(.lit-home) .lit-detail-actions span,
    body.ac:not(.lit-home) .lit-search-btn,
    body.ac:not(.lit-home) .lit-upload-actions .btn-primary,
    body.ac:not(.lit-home) #btnCreateTopUp,
    body.ac:not(.lit-home) .form-btn1,
    body.ac:not(.lit-home) .submit {
        display: inline-flex !important;
        align-items: center !important;
        justify-content: center !important;
        width: auto !important;
        min-width: 92px !important;
        height: 44px !important;
        padding: 0 22px !important;
        border: 0 !important;
        border-radius: 9999px !important;
        background: #0066cc !important;
        color: #fff !important;
        box-shadow: none !important;
        font-size: 17px !important;
        font-weight: 400 !important;
        line-height: 1 !important;
    }
    body.ac:not(.lit-home) .lit-detail-tools a,
    body.ac:not(.lit-home) .lit-detail-tools button,
    body.ac:not(.lit-home) .lit-upload-actions .btn-secondary,
    body.ac:not(.lit-home) #btnParsePdf,
    body.ac:not(.lit-home) .form-btn0,
    body.ac:not(.lit-home) .cancel {
        display: inline-flex !important;
        align-items: center !important;
        justify-content: center !important;
        min-height: 44px !important;
        padding: 0 22px !important;
        border-radius: 9999px !important;
        border: 1px solid #0066cc !important;
        background: #fff !important;
        color: #0066cc !important;
        box-shadow: none !important;
        font-size: 17px !important;
        font-weight: 400 !important;
    }
    body.ac:not(.lit-home) input[type="text"],
    body.ac:not(.lit-home) input[type="password"],
    body.ac:not(.lit-home) input[type="number"],
    body.ac:not(.lit-home) textarea,
    body.ac:not(.lit-home) select,
    body.ac:not(.lit-home) .layui-input,
    body.ac:not(.lit-home) .lit-upload-input,
    body.ac:not(.lit-home) .lit-upload-select,
    body.ac:not(.lit-home) .lit-upload-area {
        min-height: 44px !important;
        border-radius: 9999px !important;
        border: 1px solid rgba(0,0,0,.08) !important;
        background: #fff !important;
        color: #1d1d1f !important;
        box-shadow: none !important;
        font-size: 17px !important;
        font-weight: 400 !important;
        line-height: 1.47 !important;
        letter-spacing: -0.374px !important;
    }
    body.ac:not(.lit-home) textarea,
    body.ac:not(.lit-home) .lit-upload-area {
        border-radius: 18px !important;
    }
    body.ac:not(.lit-home) select,
    body.ac:not(.lit-home) .lit-upload-select,
    body.ac:not(.lit-home) .layui-form-select .layui-input {
        height: 54px !important;
        min-height: 54px !important;
        padding: 0 46px 0 22px !important;
        border-radius: 9999px !important;
        border: 1px solid rgba(0,0,0,.08) !important;
        background-color: #fff !important;
        background-image: url("data:image/svg+xml,%3Csvg width='14' height='9' viewBox='0 0 14 9' fill='none' xmlns='http://www.w3.org/2000/svg'%3E%3Cpath d='M1.5 1.75L7 7.25L12.5 1.75' stroke='%231d1d1f' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'/%3E%3C/svg%3E") !important;
        background-repeat: no-repeat !important;
        background-position: right 20px center !important;
        background-size: 14px 9px !important;
        color: #1d1d1f !important;
        font-family: "SF Pro Text", system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", "Microsoft YaHei", sans-serif !important;
        font-size: 17px !important;
        font-weight: 400 !important;
        line-height: 1.47 !important;
        letter-spacing: -0.374px !important;
        box-shadow: none !important;
        appearance: none !important;
        -webkit-appearance: none !important;
        -moz-appearance: none !important;
        cursor: pointer !important;
    }
    body.ac:not(.lit-home) select:hover,
    body.ac:not(.lit-home) .lit-upload-select:hover,
    body.ac:not(.lit-home) .layui-form-select .layui-input:hover {
        border-color: #c7c7cc !important;
    }
    body.ac:not(.lit-home) select:focus,
    body.ac:not(.lit-home) .lit-upload-select:focus,
    body.ac:not(.lit-home) .layui-form-select .layui-input:focus {
        outline: 2px solid #0071e3 !important;
        outline-offset: 2px !important;
        border-color: #0066cc !important;
    }
    body.ac:not(.lit-home) select::-ms-expand {
        display: none !important;
    }
    body.ac:not(.lit-home) select option,
    body.ac:not(.lit-home) .lit-upload-select option {
        min-height: 40px !important;
        padding: 10px 18px !important;
        background: #fff !important;
        color: #1d1d1f !important;
        font-family: "SF Pro Text", system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", "Microsoft YaHei", sans-serif !important;
        font-size: 17px !important;
        font-weight: 400 !important;
        line-height: 1.47 !important;
        letter-spacing: -0.374px !important;
    }
    body.ac:not(.lit-home) select option:checked,
    body.ac:not(.lit-home) .lit-upload-select option:checked {
        background: linear-gradient(#0066cc, #0066cc) !important;
        color: #fff !important;
    }
    body.ac:not(.lit-home) select option:hover,
    body.ac:not(.lit-home) .lit-upload-select option:hover {
        background: #f5f5f7 !important;
        color: #1d1d1f !important;
    }
    body.ac:not(.lit-home) select.apple-select-source {
        position: absolute !important;
        width: 1px !important;
        height: 1px !important;
        min-height: 1px !important;
        padding: 0 !important;
        opacity: 0 !important;
        pointer-events: none !important;
    }
    body.ac:not(.lit-home) .apple-select {
        position: relative;
        display: inline-block;
        min-width: 180px;
        vertical-align: middle;
        font-family: "SF Pro Text", system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", "Microsoft YaHei", sans-serif;
    }
    body.ac:not(.lit-home) .apple-select-trigger {
        width: 100%;
        min-height: 54px;
        display: inline-flex;
        align-items: center;
        justify-content: space-between;
        gap: 14px;
        padding: 0 20px 0 22px;
        border-radius: 9999px;
        border: 1px solid rgba(0,0,0,.08);
        background: #fff;
        color: #1d1d1f;
        font-size: 17px;
        font-weight: 400;
        line-height: 1.47;
        letter-spacing: -0.374px;
        box-shadow: none;
        cursor: pointer;
        box-sizing: border-box;
    }
    body.ac:not(.lit-home) .apple-select-trigger:focus {
        outline: 2px solid #0071e3;
        outline-offset: 2px;
        border-color: #0066cc;
    }
    body.ac:not(.lit-home) .apple-select-trigger svg {
        width: 14px;
        height: 9px;
        flex: 0 0 auto;
        transition: transform .18s ease;
    }
    body.ac:not(.lit-home) .apple-select.open .apple-select-trigger svg {
        transform: rotate(180deg);
    }
    body.ac:not(.lit-home) .apple-select-menu {
        position: absolute;
        left: 0;
        top: calc(100% + 8px);
        z-index: 12000;
        width: 100%;
        min-width: 220px;
        max-height: 280px;
        overflow: auto;
        padding: 8px;
        border-radius: 18px;
        border: 1px solid #e0e0e0;
        background: #fff;
        box-shadow: none;
        display: none;
        box-sizing: border-box;
    }
    body.ac:not(.lit-home) .apple-select.open .apple-select-menu {
        display: block;
    }
    body.ac:not(.lit-home) .apple-select-option {
        min-height: 40px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 12px;
        padding: 0 14px;
        border-radius: 12px;
        color: #1d1d1f;
        font-size: 17px;
        font-weight: 400;
        line-height: 1.47;
        letter-spacing: -0.374px;
        cursor: pointer;
        box-sizing: border-box;
    }
    body.ac:not(.lit-home) .apple-select-option:hover {
        background: #f5f5f7;
    }
    body.ac:not(.lit-home) .apple-select-option.selected {
        background: #f5f5f7;
        color: #0066cc;
        font-weight: 600;
    }
    body.ac:not(.lit-home) .apple-select-option.selected:after {
        content: "";
        width: 14px;
        height: 10px;
        flex: 0 0 auto;
        background: url("data:image/svg+xml,%3Csvg width='14' height='10' viewBox='0 0 14 10' fill='none' xmlns='http://www.w3.org/2000/svg'%3E%3Cpath d='M1.5 5.25L5.25 8.75L12.5 1.25' stroke='%230066cc' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'/%3E%3C/svg%3E") center / contain no-repeat;
    }
    body.ac:not(.lit-home) .apple-select-option.selected:hover {
        background: #f5f5f7;
        color: #0066cc;
    }
    body.ac:not(.lit-home) .lit-search-row .apple-select {
        flex: 0 0 auto;
    }
    body.ac:not(.lit-home) .layui-layer,
    body.ac:not(.lit-home) .lit-modal,
    body.ac:not(.lit-home) .topup-dialog-inner {
        border-radius: 18px !important;
        border: 1px solid #e0e0e0 !important;
        background: #fff !important;
        box-shadow: none !important;
    }
    @media (max-width: 1100px) {
        body.ac:not(.lit-home) .accountBox {
            grid-template-columns: 1fr !important;
        }
        body.ac:not(.lit-home) .accountL {
            position: static !important;
        }
        body.ac:not(.lit-home) .accountL ul {
            display: flex !important;
            flex-wrap: wrap !important;
        }
        body.ac:not(.lit-home) .pe-list {
            grid-template-columns: repeat(2, minmax(0, 1fr)) !important;
        }
    }
    @media (max-width: 700px) {
        body.ac:not(.lit-home) .middle {
            padding: 104px 16px 220px !important;
        }
        body.ac:not(.lit-home) .pe-list {
            grid-template-columns: 1fr !important;
        }
    }
    /* Home-style unified header */
    body.ac:not(.lit-home) .header,
    body.front-glass .header {
        top: 20px !important;
        padding: 0 32px !important;
        background: transparent !important;
        border: 0 !important;
        box-shadow: none !important;
    }
    body.ac:not(.lit-home) .header.positioning,
    body.front-glass .header.positioning {
        top: 20px !important;
        background: transparent !important;
        box-shadow: none !important;
    }
    body.ac:not(.lit-home) .header .w1920,
    body.front-glass .header .w1920 {
        display: flex !important;
        align-items: center !important;
        justify-content: space-between !important;
        width: calc(100vw - 64px) !important;
        max-width: 1280px !important;
        min-height: 64px !important;
        height: auto !important;
        margin: 0 auto !important;
        padding: 16px 32px !important;
        box-sizing: border-box !important;
        border-radius: 32px !important;
        border: 1px solid rgba(255,255,255,.45) !important;
        background: rgba(255,255,255,.58) !important;
        box-shadow: 0 18px 48px rgba(31,50,68,.14), inset 0 1px 0 rgba(255,255,255,.82) !important;
        backdrop-filter: blur(18px) !important;
        -webkit-backdrop-filter: blur(18px) !important;
    }
    body.ac:not(.lit-home) .header-l,
    body.front-glass .header-l {
        order: 1 !important;
        flex: 1 1 auto !important;
        display: flex !important;
        align-items: center !important;
        gap: 28px !important;
        min-width: 0 !important;
    }
    body.ac:not(.lit-home) .nav_but,
    body.front-glass .nav_but {
        display: none !important;
    }
    body.ac:not(.lit-home) .header-l ul,
    body.front-glass .header-l ul {
        display: flex !important;
        align-items: center !important;
        gap: 22px !important;
        min-width: 0 !important;
        margin: 0 !important;
        padding: 0 !important;
        white-space: nowrap !important;
        opacity: 1 !important;
        visibility: visible !important;
        transform: translateX(0) !important;
        transition: opacity .16s ease, transform .16s ease, visibility .16s ease !important;
    }
    body.ac:not(.lit-home) .header-l ul li,
    body.front-glass .header-l ul li {
        margin: 0 !important;
        padding: 0 !important;
    }
    body.ac:not(.lit-home) .header-l ul li:nth-child(n+6),
    body.front-glass .header-l ul li:nth-child(n+6) {
        display: none !important;
    }
    body.ac:not(.lit-home) .header-l ul li a,
    body.front-glass .header-l ul li a {
        display: inline-flex !important;
        align-items: center !important;
        height: 36px !important;
        padding: 8px 0 !important;
        border-radius: 999px !important;
        background: transparent !important;
        color: #4b5563 !important;
        font-family: "SF Pro Text", "PingFang SC", "Microsoft YaHei", system-ui, -apple-system, BlinkMacSystemFont, sans-serif !important;
        font-size: 14px !important;
        font-weight: 500 !important;
        line-height: 1 !important;
        letter-spacing: -0.12px !important;
    }
    body.ac:not(.lit-home) .header-l ul li a:hover,
    body.front-glass .header-l ul li a:hover {
        color: #000 !important;
        background: transparent !important;
    }
    body.ac:not(.lit-home) .header .logo,
    body.front-glass .header .logo {
        margin: 0 !important;
        flex: 0 0 auto !important;
    }
    body.ac:not(.lit-home) .header .logo a,
    body.front-glass .header .logo a {
        display: inline-flex !important;
        align-items: center !important;
        gap: 8px !important;
        color: #111827 !important;
    }
    body.ac:not(.lit-home) .header .logo-mark,
    body.front-glass .header .logo-mark {
        width: 32px !important;
        height: 32px !important;
        flex: 0 0 32px !important;
        border-radius: 8px !important;
        background: #000 !important;
        color: #fff !important;
        box-shadow: none !important;
    }
    body.ac:not(.lit-home) .header .logo-mark i,
    body.front-glass .header .logo-mark i {
        font-size: 18px !important;
        font-weight: 700 !important;
    }
    body.ac:not(.lit-home) .header .logo-text strong,
    body.front-glass .header .logo-text strong {
        color: #111827 !important;
        font-family: "SF Pro Display", "PingFang SC", "Microsoft YaHei", system-ui, -apple-system, BlinkMacSystemFont, sans-serif !important;
        font-size: 18px !important;
        font-weight: 700 !important;
        line-height: 1 !important;
        letter-spacing: -0.03em !important;
    }
    body.ac:not(.lit-home) .header .logo-text span,
    body.front-glass .header .logo-text span {
        display: none !important;
    }
    body.ac:not(.lit-home) .header-r,
    body.front-glass .header-r {
        order: 2 !important;
        margin-left: auto !important;
        display: flex !important;
        align-items: center !important;
        gap: 16px !important;
        flex: 0 0 auto !important;
    }
    body.ac:not(.lit-home) .header-r .header-item,
    body.front-glass .header-r .header-item {
        margin: 0 !important;
    }
    body.ac:not(.lit-home) .header-item a,
    body.front-glass .header-item a,
    body.ac:not(.lit-home) .header-login a,
    body.front-glass .header-login a {
        display: inline-flex !important;
        align-items: center !important;
        gap: 5px !important;
        height: 36px !important;
        padding: 8px 10px !important;
        border: 0 !important;
        border-radius: 999px !important;
        background: transparent !important;
        color: #4b5563 !important;
        font-family: "SF Pro Text", "PingFang SC", "Microsoft YaHei", system-ui, -apple-system, BlinkMacSystemFont, sans-serif !important;
        font-size: 14px !important;
        font-weight: 500 !important;
        line-height: 1 !important;
        letter-spacing: -0.12px !important;
        box-shadow: none !important;
    }
    body.ac:not(.lit-home) .header-item a:hover,
    body.front-glass .header-item a:hover,
    body.ac:not(.lit-home) .header-login a:hover,
    body.front-glass .header-login a:hover {
        color: #000 !important;
        background: rgba(255,255,255,.5) !important;
    }
    body.ac:not(.lit-home) .header-item svg path,
    body.front-glass .header-item svg path {
        fill: #6b7280 !important;
    }
    body.ac:not(.lit-home) .header-notice-dot,
    body.front-glass .header-notice-dot {
        top: 3px !important;
        right: 1px !important;
        width: 8px !important;
        height: 8px !important;
        box-shadow: none !important;
    }
    body.ac:not(.lit-home) .header-integrate-count,
    body.front-glass .header-integrate-count {
        color: #0066cc !important;
        font-weight: 700 !important;
    }
    body.ac:not(.lit-home) .header-integrate-plus,
    body.front-glass .header-integrate-plus {
        width: 22px !important;
        height: 22px !important;
        margin-left: 2px !important;
        margin-right: 4px !important;
        flex: 0 0 22px !important;
        vertical-align: middle !important;
        background: #0066cc !important;
        color: #fff !important;
        border: 0 !important;
        border-radius: 999px !important;
        box-shadow: none !important;
        font-size: 15px !important;
        font-weight: 600 !important;
        line-height: 22px !important;
        display: inline-flex !important;
        align-items: center !important;
        justify-content: center !important;
    }
    body.ac:not(.lit-home) .header-avatar,
    body.front-glass .header-avatar,
    body.ac:not(.lit-home) .header-login,
    body.front-glass .header-login {
        margin-left: 16px !important;
        padding-left: 16px !important;
        border-left: 1px solid #e5e7eb !important;
    }
    body.ac:not(.lit-home) .header-avatar a,
    body.front-glass .header-avatar a,
    body.ac:not(.lit-home) .header-avatar img,
    body.front-glass .header-avatar img {
        width: 36px !important;
        height: 36px !important;
        border-radius: 999px !important;
    }
    body.ac:not(.lit-home) .header-m,
    body.front-glass .header-m {
        order: 3 !important;
        position: relative !important;
        z-index: 4 !important;
        isolation: isolate !important;
        flex: 0 0 36px !important;
        width: 36px !important;
        max-width: 36px !important;
        height: 36px !important;
        margin: 0 0 0 12px !important;
        padding: 0 !important;
        border: 0 !important;
        border-radius: 999px !important;
        background: transparent !important;
        box-shadow: none !important;
        overflow: visible !important;
        transition: flex-basis .2s ease, max-width .2s ease, background .2s ease !important;
    }
    body.ac:not(.lit-home) .header-m:before,
    body.front-glass .header-m:before {
        content: "" !important;
        position: absolute !important;
        inset: -8px -12px !important;
        z-index: -1 !important;
        border-radius: 999px !important;
        opacity: 0 !important;
        transform: scaleX(.2) !important;
        transform-origin: right center !important;
        background: rgba(245,245,247,.76) !important;
        border: 1px solid rgba(255,255,255,.55) !important;
        box-shadow: 0 18px 42px rgba(31,50,68,.12), inset 0 1px 0 rgba(255,255,255,.86) !important;
        backdrop-filter: saturate(180%) blur(22px) !important;
        -webkit-backdrop-filter: saturate(180%) blur(22px) !important;
        transition: opacity .18s ease, transform .2s ease !important;
        pointer-events: none !important;
    }
    body.ac:not(.lit-home) .header-m:hover,
    body.ac:not(.lit-home) .header-m:focus-within,
    body.front-glass .header-m:hover,
    body.front-glass .header-m:focus-within {
        flex-basis: 360px !important;
        width: 360px !important;
        max-width: min(360px, 32vw) !important;
        padding-left: 18px !important;
        background: rgba(255,255,255,.72) !important;
        border: 1px solid rgba(0,0,0,.08) !important;
    }
    body.ac:not(.lit-home) .header-m:hover:before,
    body.ac:not(.lit-home) .header-m:focus-within:before,
    body.front-glass .header-m:hover:before,
    body.front-glass .header-m:focus-within:before {
        opacity: 1 !important;
        transform: scaleX(1) !important;
    }
    body.ac:not(.lit-home) .header .w1920:has(.header-m:hover) .header-l ul,
    body.ac:not(.lit-home) .header .w1920:has(.header-m:focus-within) .header-l ul,
    body.ac:not(.lit-home) .header .w1920.header-search-open .header-l ul,
    body.front-glass .header .w1920:has(.header-m:hover) .header-l ul,
    body.front-glass .header .w1920:has(.header-m:focus-within) .header-l ul {
        opacity: 0 !important;
        visibility: hidden !important;
        transform: translateX(-8px) !important;
        pointer-events: none !important;
    }
    body.ac:not(.lit-home) .header .w1920:has(.header-m:hover) .header-l,
    body.ac:not(.lit-home) .header .w1920:has(.header-m:focus-within) .header-l,
    body.ac:not(.lit-home) .header .w1920.header-search-open .header-l,
    body.front-glass .header .w1920:has(.header-m:hover) .header-l,
    body.front-glass .header .w1920:has(.header-m:focus-within) .header-l {
        flex: 0 1 auto !important;
    }
    body.ac:not(.lit-home) .header-m input,
    body.front-glass .header-m input {
        position: relative !important;
        z-index: 1 !important;
        width: 0 !important;
        height: 36px !important;
        padding: 0 !important;
        opacity: 0 !important;
        pointer-events: none !important;
        color: #1d1d1f !important;
        font-size: 14px !important;
        background: transparent !important;
        border: 0 !important;
        box-shadow: none !important;
        transition: opacity .16s ease, width .16s ease !important;
    }
    body.ac:not(.lit-home) .header-m:hover input,
    body.ac:not(.lit-home) .header-m:focus-within input,
    body.front-glass .header-m:hover input,
    body.front-glass .header-m:focus-within input {
        width: calc(100% - 42px) !important;
        opacity: 1 !important;
        pointer-events: auto !important;
    }
    body.ac:not(.lit-home) .header-m .sBut,
    body.front-glass .header-m .sBut {
        position: absolute !important;
        z-index: 2 !important;
        right: 0 !important;
        top: 0 !important;
        width: 36px !important;
        height: 36px !important;
        border: 0 !important;
        border-radius: 999px !important;
        background: transparent !important;
        color: #6b7280 !important;
        box-shadow: none !important;
    }
    body.ac:not(.lit-home) .header-m .sBut:hover,
    body.front-glass .header-m .sBut:hover {
        color: #000 !important;
        background: rgba(255,255,255,.5) !important;
    }
    body.ac:not(.lit-home) .header-m .sBut svg path,
    body.front-glass .header-m .sBut svg path {
        fill: currentColor !important;
    }
    @media (max-width: 860px) {
        body.ac:not(.lit-home) .header,
        body.front-glass .header {
            top: 10px !important;
            padding: 0 12px !important;
        }
        body.ac:not(.lit-home) .header .w1920,
        body.front-glass .header .w1920 {
            width: calc(100vw - 24px) !important;
            padding: 14px 18px !important;
        }
        body.ac:not(.lit-home) .header-r .header-item span,
        body.front-glass .header-r .header-item span,
        body.ac:not(.lit-home) .header-integrate-count,
        body.front-glass .header-integrate-count {
            display: none !important;
        }
        body.ac:not(.lit-home) .header-r,
        body.front-glass .header-r {
            gap: 8px !important;
        }
        body.ac:not(.lit-home) .header-l ul,
        body.front-glass .header-l ul {
            display: none !important;
        }
        body.ac:not(.lit-home) .header-m:hover,
        body.ac:not(.lit-home) .header-m:focus-within,
        body.front-glass .header-m:hover,
        body.front-glass .header-m:focus-within {
            flex-basis: 42vw !important;
            width: 42vw !important;
            max-width: 42vw !important;
        }
    }
    @media (max-width: 620px) {
        body.ac:not(.lit-home) .header .logo-text strong,
        body.front-glass .header .logo-text strong {
            display: none !important;
        }
        body.ac:not(.lit-home) .header-avatar,
        body.front-glass .header-avatar,
        body.ac:not(.lit-home) .header-login,
        body.front-glass .header-login {
            margin-left: 6px !important;
            padding-left: 8px !important;
        }
    }
</style>
<header class="header">
    <div class="w1920">
        <div class="header-l">
        <div class="nav_but">
          <div class="navBtn"><span></span><span></span><span></span></div>
        </div>
            <div class="logo">
                <a href="/">
                    <span class="logo-mark"><i>A</i></span>
                    <span class="logo-text">
                        <strong>Academic Portal</strong>
                        <span>Designed for Knowledge</span>
                    </span>
                </a>
            </div>
            <ul>
                <li><a href="/">&#39318;&#39029;</a></li>
                <li><a href="/LiteratureSearch.aspx">&#25991;&#29486;&#26816;&#32034;</a></li>
                <li><a href="/LiteratureQA.aspx">&#26234;&#33021;&#38382;&#31572;</a></li>
                <li><a href="/LiteratureVenue.aspx">&#25991;&#29486;/&#26399;&#21002;&#27719;&#24635;</a></li>
                <li><a href="<%=AcademicNewsHref %>">&#23398;&#26415;&#36164;&#35759;</a></li>
                <li><a <%=(IsLogin?"href=\"/User/LiteratureUpload\"":" class=\"loginBut\"") %>>&#25991;&#29486;&#25237;&#31295;</a></li>
                <%if (isTbClassLink)
                    {  %>
                <asp:Repeater ID="MenuClassList" runat="server">
                    <ItemTemplate>
                        <li><a href="<%#Function.HtmlDiscode(CommonFunc.GetTopHtmlHref(Eval("id").ToString(), "1")) %>"><%#Function.HtmlDiscode(Eval("classname").ToString()) %></a></li>
                    </ItemTemplate>
                </asp:Repeater>
                <%}
%>
            </ul>
        </div>
   <script>
       $('.navBtn').on('click', function () {
            if ($(this).hasClass('closeNavbtn')) {
                $(this).removeClass('closeNavbtn');
                $('.header-l ul').hide()
            } else {
                  $('.header-l ul').show()
                $(this).addClass('closeNavbtn');
            }
        })
       $(function () {
           var $headerShell = $('.header .w1920');
           var $topSearch = $('.header-m');
           $topSearch.on('mouseenter focusin', function () {
               $headerShell.addClass('header-search-open');
           });
           $topSearch.on('mouseleave focusout', function () {
               setTimeout(function () {
                   if (!$topSearch.is(':hover') && !$topSearch.find(':focus').length) {
                       $headerShell.removeClass('header-search-open');
                   }
               }, 80);
           });
       });
   </script>
        <%if (isSearch) {  %>
        <div class="header-m">
            <input type="text" id="top_searchkey" placeholder="搜索文献：标题 / 作者 / 关键词 / DOI" />
            <button class="sBut" id="top_searchbtn" onclick="return searchTopFunc()">
             <svg t="1766634326715" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="2802" id="mx_n_1766634326715" width="15" height="15"><path d="M806.272948 747.650963a455.884734 455.884734 0 1 0-58.60191 58.601911l217.664238 217.747126 58.60191-58.60191L806.355836 747.650963zM719.654849 192.134693Q828.901408 301.381253 828.901408 455.884734q0 154.503481-109.246559 263.75004Q610.408289 828.881334 455.904808 828.881334q-154.503481 0-263.75004-109.24656Q82.908208 610.388214 82.908208 455.884734q0-154.503481 109.24656-263.750041Q301.401328 82.888133 455.904808 82.888133q154.503481 0 263.750041 109.24656z" fill="#ffffff" p-id="2803"></path></svg>
            </button>
            <script>
                function searchTopFunc() {
                    $("#top_searchbtn").attr("disabled", "true");
                    var top_searchkey = ($("#top_searchkey").val() || "").trim();
                    $("#top_searchbtn").removeAttr("disabled");
                    if (!top_searchkey) {
                        window.location.href = "/LiteratureSearch.aspx";
                        return false;
                    }
                    window.location.href = "/LiteratureSearch.aspx?keyword=" + encodeURIComponent(top_searchkey);
                    return false;
                }
                $('#top_searchkey').bind('keypress', function (event) {
                    if (event.keyCode == 13) {
                        return searchTopFunc();
                    }
                });
            </script>
            <%if (isSearchHot)
                {  %>
            <div class="search" style="display: none;">
                <div class="search-1">
                    <h4>热搜榜</h4>
                    <div class="hot">
                        <asp:Repeater ID="SearchHotList" runat="server">
                            <ItemTemplate>
                                <a href="/LiteratureSearch.aspx?keyword=<%#Server.UrlEncode(Function.HtmlDiscode(Eval("name").ToString())) %>" class="sort-item"><span><%#Container.ItemIndex + 1 %></span><%#Function.HtmlDiscode(Eval("name").ToString()) %></a>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
            <script>
                $(function () {
                    $('body').bind('click', function (e) {
                        var target = $(e.target);
                        if (!target.is('.header-m input') && !target.is('.search') && !target.is('.search *')) {
                            $('.search').hide();
                        }
                    });
                    $('.header-m input').bind('click', function () {
                        $('.search').show();
                    });
                });
            </script>
            <%} %>
        </div>  
        <%} %>
        <div class="header-r">
            <div class="header-item">
                <a class="<%=(IsLogin?"header-notice-link":"loginBut header-notice-link") %>" <%=(IsLogin?"href=\"javascript:void(0);\" onclick=\"openHeaderNoticeModal();return false;\"":"") %>>
                <svg t="1766634189988" class="icon" viewBox="0 0 1149 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="1961" width="15" height="15"><path d="M622.543644 861.983508H1053.535398a95.775945 95.775945 0 0 0 95.775945-95.775946V95.775945a95.775945 95.775945 0 0 0-95.775945-95.775945H95.775945a95.775945 95.775945 0 0 0-95.775945 95.775945v670.431617a95.775945 95.775945 0 0 0 95.775945 95.775946h143.663918v114.06915a47.887973 47.887973 0 0 0 69.341785 42.811848L622.543644 861.983508zM95.775945 766.207562V95.775945h957.759453v670.431617H599.940521L335.215809 898.569919V766.207562H95.775945z m191.551891-263.383849a71.831959 71.831959 0 1 0 0-143.663918 71.831959 71.831959 0 0 0 0 143.663918z m287.327836 0a71.831959 71.831959 0 1 0 0-143.663918 71.831959 71.831959 0 0 0 0 143.663918z m287.327836 0a71.831959 71.831959 0 1 0 0-143.663918 71.831959 71.831959 0 0 0 0 143.663918z" fill="#666666" p-id="1962"></path></svg>
                <span>消息</span><%if (IsLogin && HeaderNoticeCount > 0) { %><i class="header-notice-dot"></i><% } %></a>
            </div>
            <div class="header-item header-integrate-item">
                <a <%=(IsLogin?"href=\"/User/IntegrateExchange\"":" class=\"loginBut\"") %>>
                <svg t="1766634151956" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="1724" width="15" height="15"><path d="M512 1024A512 512 0 1 0 512 0a512 512 0 0 0 0 1024z m301.738667-813.738667Q938.666667 335.36 938.666667 512q0 176.725333-124.928 301.738667Q688.64 938.666667 512 938.666667q-176.725333 0-301.738667-124.928Q85.333333 688.64 85.333333 512q0-176.725333 124.928-301.738667Q335.36 85.333333 512 85.333333q176.725333 0 301.738667 124.928zM581.888 432.896a17.066667 17.066667 0 0 1-16.213333-11.776l-37.461334-115.2a17.066667 17.066667 0 0 0-32.426666 0l-37.461334 115.2a17.066667 17.066667 0 0 1-16.213333 11.776H321.024a17.066667 17.066667 0 0 0-9.984 30.890667l97.962667 71.168a17.066667 17.066667 0 0 1 6.144 19.029333l-37.376 115.2a17.066667 17.066667 0 0 0 26.282666 19.029333l97.877334-71.168a17.066667 17.066667 0 0 1 20.138666 0l97.877334 71.168a17.066667 17.066667 0 0 0 26.282666-19.029333l-37.376-115.2a17.066667 17.066667 0 0 1 6.144-19.029333l97.962667-71.168a17.066667 17.066667 0 0 0-9.984-30.890667h-121.173333z" fill="#666666" p-id="1725"></path></svg>
                <span>&#31215;&#20998;<%if (IsLogin) { %><b class="header-integrate-count"><%=CurrentIntegrate %></b><% } %></span></a>
                <%if (IsLogin) { %><button type="button" class="header-integrate-plus" onclick="openHeaderTopupModal();return false;">+</button><% } %>
            </div>
            <div class="header-item">
                <a <%=(IsLogin?"href=\"/User/LiteratureUpload\"":" class=\"loginBut\"") %>>
                <svg t="1766634270242" class="icon" viewBox="0 0 1331 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="2435" width="18" height="18"><path d="M586.288124 14.012343l-190.786894 190.786894 67.53856 67.53856L572.36068 162.921514V667.839027h95.393447V162.921514l109.32089 109.416283 67.53856-67.53856-190.786893-190.786894a47.696723 47.696723 0 0 0-67.53856 0zM953.934467 95.478347h143.09017q59.23933 0 101.117054 41.973116 41.973117 41.877723 41.973117 101.117054v572.36068q0 59.23933-41.973117 101.117054-41.877723 41.973117-101.117054 41.973116h-953.934467q-59.23933 0-101.117053-41.973116Q0 870.263921 0 810.929197v-572.36068q0-59.23933 41.973117-101.117054Q83.755446 95.478347 143.09017 95.478347H286.18034v95.393447H143.09017q-19.746443 0-33.76928 13.927443Q95.393447 218.822074 95.393447 238.568517v572.36068q0 19.746443 13.927443 33.769281Q123.343727 858.625921 143.09017 858.625921h953.934467q19.746443 0 33.769281-13.927443Q1144.721361 830.675641 1144.721361 810.929197v-572.36068q0-19.746443-13.927443-33.76928Q1116.771081 190.871794 1097.024637 190.871794H953.934467V95.478347z" fill="#666666" p-id="2436"></path></svg>
                <span>上传</span></a>
            </div>
            <%if (IsLogin)
                {
%>
            <div class="header-avatar">
                <a href="/User/Center">
                    <img class="user-avatar-img" src="<%=(CommonUserFunc.GetUserAvatarFunc(user_list.upload_pic_avatar)) %>" style="display: block;" /></a>
            </div>
            <%}
                else
                { %>

            <div class="header-login"><a class="loginBut">登录</a></div>
            <%} %>
        </div>
    </div>
</header>
<%if (IsLogin) { %>
<script type="text/javascript" src="/js/qrcode.min.js"></script>
<div class="lm-float-mask" id="headerTopupModal" onclick="closeHeaderFloatModal(event, 'headerTopupModal')">
    <div class="lm-float-panel" onclick="event.stopPropagation();">
        <div class="lm-float-head">
            <h3>积分充值</h3>
            <button type="button" class="lm-float-close" onclick="hideHeaderModal('headerTopupModal')">×</button>
        </div>
        <div class="lm-float-body">
            <div class="lm-topup-grid">
                <div class="lm-topup-summary">
                    <div class="metric">当前积分<strong><%=CurrentIntegrate %></strong></div>
                    <div class="metric">兑换比例<strong>1 元 = <%=HeaderMoneyIntegrate %> 积分</strong></div>
                    <div class="metric">首充赠送<strong><%=(HeaderIntegrateDonate > 0 ? HeaderIntegrateDonate + "%" : "未开启") %></strong></div>
                </div>
                <div class="lm-topup-panel">
                    <h4>微信支付充值</h4>
                    <p>选择固定金额或输入自定义金额，支付完成后积分自动到账。</p>
                    <div class="lm-topup-options" id="headerTopupOptions"><%=HeaderTopUpOptionsHtml %></div>
                    <div class="lm-topup-custom">
                        <label for="headerCustomTopupMoney">自定义金额</label>
                        <input type="text" id="headerCustomTopupMoney" placeholder="请输入 1-1000 之间的整数金额" />
                    </div>
                    <div class="lm-topup-submit">
                        <button type="button" id="headerBtnCreateTopup">微信充值积分</button>
                        <span>支付成功后会自动刷新积分。</span>
                    </div>
                </div>
                <div class="lm-topup-pay" id="headerTopupPay">
                    <div class="lm-topup-qrcode" id="headerTopupQrCode"></div>
                    <div class="lm-topup-status" id="headerTopupStatus">请使用微信扫码完成支付。</div>
                    <div class="lm-topup-meta" id="headerTopupMeta"></div>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="lm-float-mask" id="headerNoticeModal" onclick="closeHeaderFloatModal(event, 'headerNoticeModal')">
    <div class="lm-float-panel" onclick="event.stopPropagation();">
        <div class="lm-float-head">
            <h3>通知消息</h3>
            <button type="button" class="lm-float-close" onclick="hideHeaderModal('headerNoticeModal')">×</button>
        </div>
        <div class="lm-float-body">
            <div class="lm-modal-notice-list">
                <%=HeaderNoticeHtml %>
            </div>
            <a class="lm-modal-more" href="/User/NoticeLog">查看全部通知</a>
        </div>
    </div>
</div>
<script>
    var headerTopupOrderNo = "";
    var headerTopupPollTimer = null;

    function headerFloatMessage(text) {
        if (window.layer && layer.msg) {
            layer.msg(text);
        } else {
            alert(text);
        }
    }

    function showHeaderModal(id) {
        var el = document.getElementById(id);
        if (el) el.classList.add('show');
    }
    function hideHeaderModal(id) {
        var el = document.getElementById(id);
        if (el) {
            el.classList.remove('show');
        }
        if (id === 'headerTopupModal') {
            stopHeaderTopupPolling();
        }
    }
    function closeHeaderFloatModal(event, id) {
        if (event.target && event.target.id === id) {
            hideHeaderModal(id);
        }
    }
    function openHeaderTopupModal() {
        showHeaderModal('headerTopupModal');
    }
    function openHeaderNoticeModal() {
        showHeaderModal('headerNoticeModal');
    }

    function stopHeaderTopupPolling() {
        if (headerTopupPollTimer) {
            window.clearInterval(headerTopupPollTimer);
            headerTopupPollTimer = null;
        }
    }

    function setHeaderTopupStatus(text, color) {
        var el = document.getElementById('headerTopupStatus');
        if (el) {
            el.innerHTML = text;
            el.style.color = color || '#2b4a6a';
        }
    }

    function getHeaderCustomTopupMoney() {
        var money = parseInt($('#headerCustomTopupMoney').val(), 10);
        return isNaN(money) ? 0 : money;
    }

    function renderHeaderTopupQr(codeUrl) {
        var qrBox = document.getElementById('headerTopupQrCode');
        if (!qrBox) {
            return;
        }
        qrBox.innerHTML = '';
        if (window.QRCode && codeUrl) {
            new QRCode(qrBox, {
                text: codeUrl,
                width: 220,
                height: 220
            });
        } else if (codeUrl) {
            qrBox.innerHTML = '<a href="' + codeUrl + '" target="_blank">打开支付链接</a>';
        }
    }

    function beginHeaderTopupPolling() {
        stopHeaderTopupPolling();
        headerTopupPollTimer = window.setInterval(function () {
            if (!headerTopupOrderNo) {
                return;
            }
            $.ajax({
                url: '/Inc/UserCommon.ashx',
                cache: false,
                data: JSON.stringify({
                    btn: 'QueryTopUpStatus',
                    out_trade_no: headerTopupOrderNo
                }),
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (!res) {
                        return;
                    }
                    if (res.status === 1) {
                        stopHeaderTopupPolling();
                        setHeaderTopupStatus('支付成功，正在刷新积分...', '#1b8a4b');
                        window.setTimeout(function () {
                            window.location.reload();
                        }, 800);
                    } else if (res.status === -1) {
                        stopHeaderTopupPolling();
                        setHeaderTopupStatus(res.info || '订单已关闭，请重新发起充值。', '#c0392b');
                    }
                }
            });
        }, 3000);
    }

    $(function () {
        var selectedId = 0;
        var selectedMoney = 0;

        $('#headerTopupOptions').on('click', '.lm-topup-option', function () {
            $('#headerTopupOptions .lm-topup-option').removeClass('current');
            $(this).addClass('current');
            selectedId = parseInt($(this).attr('data-id'), 10) || 0;
            selectedMoney = parseInt($(this).attr('data-money'), 10) || 0;
            $('#headerCustomTopupMoney').val('');
        });

        $('#headerCustomTopupMoney').on('input', function () {
            $('#headerTopupOptions .lm-topup-option').removeClass('current');
            selectedId = 0;
            selectedMoney = 0;
            this.value = this.value.replace(/[^\d]/g, '');
        });

        $('#headerBtnCreateTopup').on('click', function () {
            var customMoney = getHeaderCustomTopupMoney();
            var money = customMoney > 0 ? customMoney : selectedMoney;
            var idValue = customMoney > 0 ? 0 : selectedId;
            if (!money || money < 1 || money > 1000) {
                headerFloatMessage('请选择充值金额，或输入 1-1000 之间的整数金额。');
                return;
            }

            $('#headerBtnCreateTopup').prop('disabled', true);
            $.ajax({
                url: '/Inc/UserCommon.ashx',
                cache: false,
                data: JSON.stringify({
                    btn: 'AddTopUp',
                    money: customMoney > 0 ? money : 0,
                    typestr: 'wx',
                    idstr: idValue
                }),
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    $('#headerBtnCreateTopup').prop('disabled', false);
                    if (!res || res.status !== 1) {
                        headerFloatMessage((res && res.info) ? res.info : '创建充值订单失败。');
                        return;
                    }

                    headerTopupOrderNo = res.out_trade_no || '';
                    renderHeaderTopupQr(res.code_url);

                    var gift = parseInt(res.gift_amount || 0, 10);
                    var meta = [];
                    meta.push('订单号：' + headerTopupOrderNo);
                    meta.push('充值金额：' + res.money + ' 元');
                    meta.push('基础积分：' + (res.integrate_amount || 0));
                    if (gift > 0) {
                        meta.push('首充赠送：' + gift);
                    }
                    $('#headerTopupMeta').html(meta.join('<br />'));
                    $('#headerTopupPay').addClass('show');
                    setHeaderTopupStatus('请使用微信扫码支付，系统会自动查询订单状态。', '#2b4a6a');
                    beginHeaderTopupPolling();
                },
                error: function () {
                    $('#headerBtnCreateTopup').prop('disabled', false);
                    headerFloatMessage('请求异常，请稍后重试。');
                }
            });
        });
    });
</script>
<%} %>
<%if (!IsLogin)
    {  %>
<section id="login" style="display: none; background: #fff;">
    <div class="login-box">
        <div class="w1200">
            <div class="login_r">
                <div class="layui-form">
                    <div class="layui-form-tt">&#25991;&#29486;&#31995;&#32479;&#30331;&#24405;/&#27880;&#20876;</div>
                    <div class="layui-form-item">
                        <div class="layui-input-block">
                            <input type="text" id="toplogin-tel"  name="phone" placeholder="手机号码*"
                                class="layui-input">
                        </div>
                    </div>
                    <div class="layui-form-item  layui-form-f">
                        <div class="layui-input-block">
                            <input type="text" id="toplogin-code" name="text" placeholder="验证码*"
                                class="layui-input">
                        </div>
                        <button id="toplogin-codebtn" onclick="codeBtnFun()">发送验证码</button>
                    </div>

                    <div class="layui-form-agreement">
                        <input type="checkbox" id="toplogin-ischeckbox">
                        我已阅读
                        <asp:Repeater ID="LoginSingleData" runat="server">
                            <ItemTemplate>
                                <%#(Container.ItemIndex>0?"和":"") %><span class="service" data-id="<%#Eval("id").ToString() %>"><%#Function.HtmlDiscode(Eval("name").ToString()) %></span>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div class="layui-form-submit">
                        <button type="button" class="layui-btn" id="toplogin-loginbtn" onclick="LoginFunc('toplogin')">登录/注册</button>
		            	<input type="hidden" id="typeval" name="typeval" value="1"/>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        layui.use(['layer', 'form'], function () {
            var $ = layui.jquery, layer = layui.layer;
            var form = layui.form;
             var openForm = function() {
                 layer.open({
                        type: 1,
                        skin: 'layui-layer-login',
                        title: '文献管理系统账号',
                        shadeClose: true,
                        shade: 0.6,
                        area: ['600px', '520px'],
                        content: $('#login'),
                        success: function (layero, index) {
                        form.render();
                     }
                    });
             }
         $('.loginBut').on('click', openForm);
          $('.service').click(function () {
                let LoginSingle_Obj = this;
                let data_id = $(LoginSingle_Obj).data("id");
                if (parseInt(data_id) > 0) {
                    layer.open({
                        type: 1,
                        title: $(LoginSingle_Obj).html(),
                        shadeClose: true,
                        shade: 0.8,
                        content: $("#login-single-" + data_id).html(),
                    });
                }
            })
        })
      </script>
</section>


<LiteratureManager:code_tel ID="code_tel" runat="server" />


<script>

    var smsDebugMode = false;

    function requestLoginCode(img_x, img_y) {
        var typeval = $("#typeval").val();
        var tel = $("#toplogin-tel").val();
        var iscode = false;
        $.ajax({
            url: "/Inc/UserCommon.ashx",
            cache: true,
            async: false,
            data: "{btn:'addcode',tel:'" + escape(tel) + "',img_x:'" + img_x + "',img_y:'" + img_y + "',type:'" + typeval + "'}",
            dataType: "json",
            type: "POST",
            success: function (datas) {
                $("#toplogin-codebtn").removeAttr("disabled");
                if (datas.status == "1" || datas.status == 1) {
                    $("#img_x").val(img_x);
                    $("#img_y").val(img_y);
                    iscode = true;
                    if (smsDebugMode) {
                        var match = (datas.info || "").match(/(\d{6})/);
                        if (match && match[1]) {
                            $("#toplogin-code").val(match[1]);
                        }
                    }
                    layer.msg(datas.info, { icon: 1, time: 3500 });
                }
                else {
                    layer.msg(datas.info, { icon: 0 });
                }
            },
            error: function (err) {
                $("#toplogin-codebtn").removeAttr("disabled");
                layer.msg("获取验证码异常，请稍后再试！", { icon: 0 });
                console.log(JSON.stringify(err))
            }
        });
        if (iscode) {
            countTimeout();
        }
        return iscode;
    }

    function codeBtnFun() {
        var tel = $("#toplogin-tel").val();
        $("#toplogin-codebtn").attr("disabled", "true");
        if (tel != "") {
            let reg = /^1[0-9]{10}$/;
            if (!reg.test(tel)) {
                $("#toplogin-codebtn").removeAttr("disabled");
                layer.msg('手机号码格式错误', { icon: 0 });
            } else {
                if (smsDebugMode) {
                    requestLoginCode(1, 1);
                } else {
                    codePopupFunc();
                }
            }
        } else {
            $("#toplogin-codebtn").removeAttr("disabled");
            layer.msg('请输入手机号码', { icon: 0 });
        }
    }

    function LoginFunc(btn_name) {
        $("#" + btn_name + "-loginbtn").attr("disabled", "true");
        var ischeckbox = document.getElementById(btn_name + "-ischeckbox");
        if ($(ischeckbox).prop("checked") == true || $(ischeckbox).prop("checked") == "checked") {
            var isyes = true;
            var user_telcode = $("#"+btn_name+"-code").val();
            if (!(user_telcode || "").trim()) {
                isyes = false;
                $("#"+btn_name+"-code").focus();
            }

            var user_tel = $("#"+btn_name+"-tel").val();
            if (!(user_tel || "").trim()) {
                isyes = false;
                $("#"+btn_name+"-tel").focus();
            }
            var img_x = $("#img_x").val();
            var img_y = $("#img_y").val();
            var typeval = $("#typeval").val();

            if (isyes) {
                if (parseInt(typeval) > 0 && (img_x || "").trim() && (img_y || "").trim()) {
                    var param1_json = { // 提交数据
                        "btn": "UserLogin",
                        "code": user_telcode,
                        "tel": user_tel,
                        "img_x": img_x,
                        "img_y": img_y,
                        "type": typeval
                    }
                    $.ajax({
                        url: "/Inc/UserCommon.ashx",
                        cache: true,
                        async: false,
                        data: JSON.stringify(param1_json),
                        dataType: "json",
                        type: "POST",
                        success: function (datas) {
                            $("#" + btn_name + "-loginbtn").removeAttr("disabled");
                            if (datas.status == 1) {
                                location.reload(true);
                            } else {
                                layer.msg(datas.info, { icon: 0 });
                            }
                        },
                        error: function (err) {
                            $("#" + btn_name + "-loginbtn").removeAttr("disabled");
                            console.log(JSON.stringify(err))
                        }
                    });
                }
                else {
                    $("#" + btn_name + "-loginbtn").removeAttr("disabled");
                    layer.msg("参数异常！", { icon: 0 });
                }
            } else {
                $("#" + btn_name + "-loginbtn").removeAttr("disabled");
            }
        } else {
            $("#" + btn_name + "-loginbtn").removeAttr("disabled");
            layer.msg("请先阅读并同意协议！", { icon: 0 });
        }
    }
</script>

<asp:Repeater ID="LoginSingleData_" runat="server">
    <ItemTemplate>
        <div class="login-single" id="login-single-<%#Eval("id").ToString() %>">
            <div class="ser_text">
                <%#Function.Replace_Content(Eval("info_").ToString())%>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>
<style>
    .login-single {
        display: none;
    }
</style>
<%} %>
<script>
    layui.use('layer', function () {
        var $ = layui.jquery, layer = layui.layer;
        $(".tips").hover(function () {
            layer.tips($(this).data('text'), this);
        }, function () {
            layer.closeAll('tips')
        });
    })
</script>
<script>
    (function () {
        function buildAppleSelect(select) {
            if (!select || select.multiple || select.classList.contains('apple-select-source')) {
                return;
            }
            if (select.nextElementSibling && select.nextElementSibling.classList && select.nextElementSibling.classList.contains('apple-select')) {
                return;
            }

            select.classList.add('apple-select-source');

            var root = document.createElement('div');
            root.className = 'apple-select';
            root.style.minWidth = Math.max(select.offsetWidth || 0, 160) + 'px';

            var trigger = document.createElement('button');
            trigger.type = 'button';
            trigger.className = 'apple-select-trigger';
            trigger.innerHTML = '<span></span><svg viewBox="0 0 14 9" fill="none" aria-hidden="true"><path d="M1.5 1.75L7 7.25L12.5 1.75" stroke="#1d1d1f" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>';

            var menu = document.createElement('div');
            menu.className = 'apple-select-menu';

            function selectedText() {
                var opt = select.options[select.selectedIndex];
                return opt ? opt.text : '';
            }

            function sync() {
                trigger.querySelector('span').textContent = selectedText();
                Array.prototype.forEach.call(menu.children, function (item) {
                    item.classList.toggle('selected', parseInt(item.getAttribute('data-index'), 10) === select.selectedIndex);
                });
            }

            function rebuild() {
                menu.innerHTML = '';
                Array.prototype.forEach.call(select.options, function (opt, index) {
                    var item = document.createElement('div');
                    item.className = 'apple-select-option';
                    item.setAttribute('data-value', opt.value);
                    item.setAttribute('data-index', index);
                    item.textContent = opt.text;
                    item.onclick = function () {
                        select.selectedIndex = index;
                        sync();
                        root.classList.remove('open');
                        if (typeof Event === 'function') {
                            select.dispatchEvent(new Event('change', { bubbles: true }));
                        } else {
                            var evt = document.createEvent('HTMLEvents');
                            evt.initEvent('change', true, false);
                            select.dispatchEvent(evt);
                        }
                    };
                    menu.appendChild(item);
                });
                sync();
            }

            trigger.onclick = function (event) {
                event.stopPropagation();
                document.querySelectorAll('.apple-select.open').forEach(function (node) {
                    if (node !== root) node.classList.remove('open');
                });
                root.classList.toggle('open');
            };

            trigger.onkeydown = function (event) {
                if (event.key === 'Escape') {
                    root.classList.remove('open');
                }
            };

            root.appendChild(trigger);
            root.appendChild(menu);
            select.parentNode.insertBefore(root, select.nextSibling);
            rebuild();

            select.addEventListener('change', sync);
        }

        function initAppleSelects() {
            if (!document.body || !document.body.classList.contains('ac') || document.body.classList.contains('lit-home')) {
                return;
            }
            document.querySelectorAll('select').forEach(buildAppleSelect);
        }

        document.addEventListener('click', function () {
            document.querySelectorAll('.apple-select.open').forEach(function (node) {
                node.classList.remove('open');
            });
        });

        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', initAppleSelects);
        } else {
            initAppleSelects();
        }
    })();
</script>
