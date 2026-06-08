<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="left.ascx.cs" Inherits="Web.UserCenter.left" %>
<%@ Register TagPrefix="LiteratureManager" TagName="UserJs" Src="/UserCenter/UserJs.ascx" %>
<div class="accountL">
    <ul>
        <li  class="<%=GetClassHtml("Account") %>"><a href="/User/Account">账号信息</a></li>
        <li class="<%=GetClassHtml("ServiceLog,ServiceLog_,ServiceLogInfo,ServiceLogAdd") %>"><a href="/User/ServiceLog">问题反馈</a></li>
        <li class="<%=GetClassHtml("IntegrateExchange,IntegrateWithdrawal,IntegrateLog,IntegrateExchangeLog") %>"><a href="/User/IntegrateExchange">我的积分</a></li>
        <li class="<%=GetClassHtml("NoticeLog,MsgLog") %>"><a href="/User/NoticeLog">通知消息</a></li>
    </ul>
</div>

    <LiteratureManager:UserJs ID="UserJs" runat="server" />
