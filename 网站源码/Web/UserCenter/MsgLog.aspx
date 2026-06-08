<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MsgLog.aspx.cs" Inherits="Web.UserCenter.MsgLog" %>
<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="left" Src="/UserCenter/left.ascx" %>
<!DOCTYPE html>
<html lang="zh-CN">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta name='robots' content='index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1' />
    <title>评论回复</title>
    <LiteratureManager:css ID="css" runat="server" />
    <style>
        .notice-tabs ul { display: flex; gap: 14px; padding: 0; margin: 0 0 24px; list-style: none; }
        .notice-tabs li a, .notice-tabs li span {
            display: inline-block;
            padding: 10px 18px;
            border-radius: 999px;
            background: #f3f6fa;
            color: #425466;
        }
        .notice-tabs li.current a, .notice-tabs li.current span {
            background: #1d6fdc;
            color: #ffffff;
        }
        .notice-empty {
            padding: 48px 24px;
            border: 1px dashed #d6e0ea;
            border-radius: 16px;
            text-align: center;
            color: #7a8795;
            background: #fbfdff;
        }
    </style>
</head>

<body class="ac">
    <LiteratureManager:top ID="top" runat="server" />
    <div class="middle">
        <section class="account">
            <div class="w1200">
                <div class="accountBox">
                    <LiteratureManager:left ID="left" runat="server" />
                    <div class="accountR">
                        <div class="list-tt">
                            <h4>通知消息</h4>
                        </div>
                        <div class="pe-item status">
                            <div class="notice-tabs">
                                <ul>
                                    <li class="current"><span>互动提醒（0）</span></li>
                                    <li><a href="/User/NoticeLog">系统通知（<%=NoticeCount %>）</a></li>
                                </ul>
                            </div>
                            <div class="notice-empty">
                                当前版本暂未开放前台文献评论互动提醒。后续如接入文献评论、审核反馈或协作批注，会在这里集中展示。
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </section>
    </div>
    <LiteratureManager:foot ID="foot" runat="server" />
</body>

</html>
