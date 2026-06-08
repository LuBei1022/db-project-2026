<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="top-up.aspx.cs" Inherits="Web.top_up" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <meta http-equiv="refresh" content="0;url=/User/IntegrateLog" />
    <title>积分充值跳转中</title>
    <style>
        body {
            margin: 0;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            background: linear-gradient(180deg, #f5f8fc 0%, #eef3f9 100%);
            font-family: "Microsoft YaHei", sans-serif;
            color: #203247;
        }
        .redirect-card {
            width: min(460px, calc(100vw - 32px));
            padding: 32px 28px;
            border-radius: 24px;
            background: #fff;
            border: 1px solid #e5edf6;
            box-shadow: 0 24px 60px rgba(17, 39, 65, 0.08);
            text-align: center;
        }
        .redirect-card h1 {
            margin: 0 0 12px;
            font-size: 28px;
            color: #17385f;
        }
        .redirect-card p {
            margin: 0;
            font-size: 15px;
            line-height: 1.9;
            color: #5f7184;
        }
        .redirect-card a {
            color: #1d6fdc;
            text-decoration: none;
        }
    </style>
</head>
<body>
    <div class="redirect-card">
        <h1>正在跳转到积分页</h1>
        <p>积分充值与权益兑换已统一到用户中心处理。如果页面没有自动跳转，请点击 <a href="/User/IntegrateLog">进入我的积分</a>。</p>
    </div>
</body>
</html>
