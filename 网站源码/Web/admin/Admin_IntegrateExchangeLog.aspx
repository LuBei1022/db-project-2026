<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_IntegrateExchangeLog.aspx.cs" Inherits="Web.admin.Admin_IntegrateExchangeLog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
</head>
<body>
<%@ Register TagPrefix="LiteratureManager" TagName="Inc" Src="Inc.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="class_menu" Src="class_menu.ascx" %>
<%if (isLoading)
    {  %>
<LiteratureManager:Inc ID="Inc2" runat="server" />
<LiteratureManager:class_menu ID="class_menu" runat="server" />

<form id="form2" runat="server">
    <!--begin::App Content-->
    <div class="app-content">
        <asp:Panel ID="Main" runat="server">
            <!--begin::Container-->
            <div class="container-fluid">
                <div class="col-md-12">
                    <!-- /.card -->
                    <div class="card mb-12">
                        <div class="card-header cardList">
                            <div class="cardItem">
                                <label class="col-form-label">下载权益</label>
                                <div class="col-form-input">
                                    <asp:TextBox ID="SearchKeyWords" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="cardItem">
                                <label class="col-form-label">权益码</label>
                                <div class="col-form-input">
                                    <asp:TextBox ID="SearchCode" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                             <div class="cardItem">
                                <label class="col-form-label">用户昵称/手机号码</label>
                                <div class="col-form-input">
                                    <asp:TextBox ID="SearchUserInfo" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                             <div class="cardItem">
                                <label class="col-form-label">状态</label>
                                <div class="col-form-input">
                                    <asp:DropDownList ID="SearchStatus" runat="server" Style="width: auto;" CssClass="form-control form-select" ></asp:DropDownList>
                                </div>
                            </div>
                            <div class="cardItem">
                                <asp:Button ID="Button2" runat="server" OnClick="OnClick_Search" Text="搜索" CssClass="btn btn-success" />
                            </div>
                        </div>
                        <div class="card-body p-0">
                            <table class="table table-sm">
                                <thead>
                                    <tr>
                                        <th>序号</th>
                                        <th>图片</th>
                                        <th>兑换者</th>
                                        <th>下载权益</th>
                                        <th>消耗积分</th>
                                        <th>权益码</th>
                                        <th>兑换时间</th>
                                        <th>状态</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <itemtemplate>
                                            <tr class="hover">
                                                 <td><%#Eval("xuhao").ToString()%></td>
                                                  <td>
                                                    <img src="<%#Function.GetAdminUpload_Pic(Eval("upload_pic_img").ToString())%>" height="20" style="border: 1px solid #cccccc" class="tooltip_img"></td>
                                                <td><%#CommonUserFunc.GetUserInfoHtml(Eval("user_id").ToString())%></td>
                                                <td><%#Function.HtmlDiscodeWeb(Eval("name").ToString())%></td>
                                                <td><%#Function.HtmlDiscodeWeb(Eval("num_integrate").ToString())%></td>
                                                <td><%#Function.HtmlDiscodeWeb(Eval("codestr").ToString())%></td>
                                                <td><%#Function.ConvertTo<DateTime>(Eval("addtime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss")%></td>
                                                 <td><%#(Eval("status").ToString()=="1"?"待使用":(Eval("status").ToString()=="-1"?"已使用":""))%></td>
                                            </tr>
                                        </itemtemplate>
                                    </asp:Repeater>
                                    <asp:Panel ID="DivNull" runat="server" Visible="true">
                                        <tr>
                                            <td colspan="10" style="text-align: center;">无相关数据!</td>
                                        </tr>
                                    </asp:Panel>
                                </tbody>
                            </table>
                            <div class="msdn">
                                <div></div>
                                <Webdiyer:AspNetPager ID="AspNetPager1" runat="server" CurrentPageButtonClass="current" FirstPageText="Home" PrevPageText="Prev" NextPageText="Next" LastPageText="End"
                                    ShowDisabledButtons="true" OnPageChanged="AspNetPager1_PageChanged" UrlPaging="true" PageIndexBoxClass="input_page" PageIndexBoxType="TextBox" SubmitButtonClass="go" SubmitButtonText="GO" ShowPageIndexBox="Always">
                                </Webdiyer:AspNetPager>
                            </div>
                        </div>
                        <!-- /.card-body -->
                    </div>
                    <!-- /.card -->
                </div>
            </div>
            <!--end::Container-->
        </asp:Panel>
    </div>
    <!--end::App Content-->
</form>
<%} %>
    </body>
</html>
