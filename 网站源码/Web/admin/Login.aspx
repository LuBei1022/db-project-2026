
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Web.admin.Login" %>

<!doctype html>
<html lang="zh-CN">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>文献管理后台登录</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link rel="stylesheet" href="css/index.css" />
    <link rel="stylesheet" href="css/overlayscrollbars.min.css" />
    <link rel="stylesheet" href="css/bootstrap-icons.min.css" />
    <link rel="stylesheet" href="css/adminlte.css" />
    <link rel="stylesheet" href="css/style.css" />
    <script src="js/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src="../Inc/jquery.validator.pack.js"></script>
    <link  href="../Inc/form_validate.css" rel="stylesheet" type="text/css" />
</head>
<style>
    body.login-page {
        min-height: 100vh;
        margin: 0;
        background:
            radial-gradient(circle at top left, rgba(59, 130, 246, 0.28), transparent 32%),
            radial-gradient(circle at bottom right, rgba(14, 165, 233, 0.22), transparent 28%),
            linear-gradient(135deg, #0f172a 0%, #102542 42%, #173b69 100%);
        background-attachment: fixed;
    }
    .login-shell {
        min-height: 100vh;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 32px 18px;
    }
    .login-bg {
        width: 100%;
        max-width: 1080px;
        display: grid;
        grid-template-columns: 1.1fr 0.9fr;
        border-radius: 28px;
        overflow: hidden;
        box-shadow: 0 30px 70px rgba(15, 23, 42, 0.28);
        background: rgba(255, 255, 255, 0.1);
        backdrop-filter: blur(12px);
        border: 1px solid rgba(255, 255, 255, 0.14);
    }
    .login-hero {
        padding: 64px 56px;
        color: #eef6ff;
        background: linear-gradient(160deg, rgba(255,255,255,0.08) 0%, rgba(255,255,255,0.02) 100%);
        position: relative;
    }
    .login-hero:before {
        content: "";
        position: absolute;
        inset: 24px;
        border: 1px solid rgba(255,255,255,0.10);
        border-radius: 24px;
        pointer-events: none;
    }
    .login-badge {
        display: inline-flex;
        align-items: center;
        gap: 10px;
        padding: 8px 14px;
        border-radius: 999px;
        background: rgba(255,255,255,0.12);
        color: #dbeafe;
        font-size: 13px;
        letter-spacing: 1px;
        margin-bottom: 24px;
    }
    .login-hero h1 {
        margin: 0 0 20px;
        font-size: 48px;
        line-height: 1.16;
        font-weight: 700;
        letter-spacing: -1px;
        max-width: 460px;
    }
    .login-hero p {
        margin: 0;
        font-size: 16px;
        line-height: 1.95;
        color: rgba(238, 246, 255, 0.8);
        max-width: 400px;
    }
    .login-note {
        margin-top: 44px;
        padding-top: 18px;
        border-top: 1px solid rgba(255,255,255,0.12);
        font-size: 13px;
        color: rgba(219, 234, 254, 0.72);
        letter-spacing: 0.5px;
    }
    .login-box {
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 36px;
        background: rgba(255,255,255,0.96);
    }
    .login-bg .login-box .card {
        width: 100%;
        max-width: 380px;
        margin: 0;
        border: none;
        border-radius: 24px;
        box-shadow: 0 18px 45px rgba(15, 23, 42, 0.12);
        overflow: hidden;
        background: #ffffff;
    }
    .login-bg .login-box .login-card-body {
        padding: 34px 32px 30px;
        background: #ffffff;
        border: none;
    }
    .login-box-msg {
        text-align: left;
        font-size: 28px;
        font-weight: 700;
        color: #0f172a;
        margin-bottom: 8px;
    }
    .login-subtitle {
        margin-bottom: 24px;
        color: #64748b;
        font-size: 14px;
    }
    .input-group {
        border-radius: 16px;
        overflow: hidden;
        border: 1px solid #d9e2ef;
        background: #f8fbff;
    }
    .input-group:focus-within {
        border-color: #3b82f6;
        box-shadow: 0 0 0 4px rgba(59, 130, 246, 0.12);
    }
    .input-group-text {
        border: none;
        background: transparent;
        color: #52749b;
        padding-left: 16px;
    }
    .form-control {
        border: none;
        background: transparent;
        height: 50px;
        color: #0f172a;
    }
    .form-control:focus {
        box-shadow: none;
        background: transparent;
    }
    .verification {
        width: 108px;
        display: flex;
        align-items: center;
        justify-content: center;
        background: #eef4fb;
        border-left: 1px solid #d9e2ef;
    }
    .verification img {
        cursor: pointer;
        max-width: 96px;
        height: 40px;
    }
    .btn.btn-primary {
        height: 50px;
        border: none;
        border-radius: 16px;
        background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
        font-size: 16px;
        font-weight: 600;
        box-shadow: 0 14px 28px rgba(37, 99, 235, 0.26);
    }
    .btn.btn-primary:hover {
        background: linear-gradient(135deg, #1d4ed8 0%, #1e40af 100%);
    }
        @media (max-width: 920px) {
            .login-bg {
                grid-template-columns: 1fr;
            }
            .login-hero {
                padding: 36px 28px 26px;
            }
            .login-hero h1 {
                font-size: 32px;
            }
            .login-box {
                padding: 24px;
            }
            .login-note {
                margin-top: 28px;
            }
        }
</style>
<body class="login-page">
    <div class="login-shell">
        <div class="login-bg">
            <div class="login-hero">
                <div class="login-badge">
                    <span class="bi bi-journal-text"></span>
                    文献管理后台
                </div>
                <h1>让知识沉淀更有序，让检索触达更高效。</h1>
                <p>登录后即可进入文献管理后台，继续处理审核、维护与日常管理工作。</p>
                <div class="login-note">Literature Management Console</div>
            </div>
            <div class="login-box">
                <div class="card">
                    <div class="card-body login-card-body">
                        <p class="login-box-msg">管理员登录</p>
                        <div class="login-subtitle">请输入后台账号、密码和验证码继续访问。</div>
                        <form id="loginform" runat="server">
                            <div class="input-group mb-3">
                                <div class="input-group-text"><span class="bi bi-person-fill"></span></div>
                                <asp:TextBox ID="user_name" placeholder="用户名" onfocus="this.placeholder=''" onblur="this.placeholder='用户名'" TextMode="SingleLine" runat="server" CssClass="form-control" reg="^.+$" tip="请输入后台登录用户名"></asp:TextBox>
                            </div>
                            <div class="input-group mb-3">
                                <div class="input-group-text"><span class="bi bi-lock-fill"></span></div>
                                <asp:TextBox ID="user_pwd" TextMode="Password" placeholder="密码" onfocus="this.placeholder=''" onblur="this.placeholder='密码'" runat="server" CssClass="form-control" reg="^.+$" tip="请输入用户名相对应的密码"></asp:TextBox>
                            </div>
                            <div class="input-group mb-3">
                                <div class="input-group-text"><span class="bi bi-shield-lock-fill"></span></div>
                                <asp:TextBox ID="Code" TextMode="SingleLine" placeholder="验证码" onfocus="this.placeholder=''" onblur="this.placeholder='验证码'" runat="server" CssClass="form-control" reg="^.+$" tip="请输入验证码"></asp:TextBox>
                                <div class="verification">
                                    <img src="" id="ImageKey" onclick="$('#ImageKey').attr('src', 'Code.aspx?dump=' + Math.random());" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-12">
                                    <div class="d-grid gap-2">
                                        <asp:Button ID="button" runat="server" Text="登录后台" CssClass="btn btn-primary" onfocus="this.blur()" OnClick="AdminLogin_Click" />
                                    </div>
                                </div>
                            </div>
                            <script type="text/javascript">
                                $("#ImageKey").attr("src", "Code.aspx?dump=" + Math.random());
                                $(function () {
                                    $("#body").css('height', $(window).height());
                                });

                                if (!('placeholder' in document.createElement('input'))) {

                                    $('input[placeholder],textarea[placeholder]').each(function () {
                                        var that = $(this),
                                            text = that.attr('placeholder');
                                        if (that.val() === "") {
                                            that.val(text).addClass('placeholder');
                                        }
                                        that.focus(function () {
                                            if (that.val() === text) {
                                                that.val("").removeClass('placeholder');
                                            }
                                        })
                                            .blur(function () {
                                                if (that.val() === "") {
                                                    that.val(text).addClass('placeholder');
                                                }
                                            })
                                            .closest('form').submit(function () {
                                                if (that.val() === text) {
                                                    that.val('');
                                                }
                                            });
                                    });
                                }
                            </script>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script src="js/overlayscrollbars.browser.es6.min.js"></script>
    <script src="js/popper.min.js"></script>
    <script src="js/bootstrap.min.js"></script>
    <script src="js/adminlte.js"></script>
</body>
</html>
