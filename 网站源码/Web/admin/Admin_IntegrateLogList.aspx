<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_IntegrateLogList.aspx.cs" Inherits="Web.admin.Admin_IntegrateLogList" %>

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
                                <label class="col-form-label">关键词</label>
                                <div class="col-form-input">
                                    <asp:TextBox ID="SearchKeyWords" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                             <div class="cardItem">
                                <label class="col-form-label">用户昵称/手机号码</label>
                                <div class="col-form-input">
                                    <asp:TextBox ID="SearchUserInfo" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                             <div class="cardItem">
                                <label class="col-form-label">类型</label>
                                <div class="col-form-input">
                                    <asp:DropDownList ID="SearchType" runat="server" Style="width: auto;" CssClass="form-control form-select" ></asp:DropDownList>
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
                                        <th>会员信息</th>
                                        <th>标题</th>
                                        <th>详情</th>
                                        <th>积分</th>
                                        <th>类型</th>
                                        <th>时间</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <itemtemplate>
                                            <tr class="hover">
                                                 <td><%#Eval("xuhao").ToString()%></td>
                                                <td><%#CommonUserFunc.GetUserInfoHtml(Eval("user_id").ToString())%></td>
                                                <td><%#Function.HtmlDiscodeWeb(Eval("name").ToString())%></td>
                                                <td><%#Function.HtmlDiscodeWeb(Eval("info_").ToString())%></td>
                                                <td><%#Function.HtmlDiscodeWeb(Eval("num_integrate").ToString())%></td>
                                                <td><%#CommonFunc.GetIntegrateLogTypeFunc(Eval("type").ToString())%></td>
                                                <td><%#Function.ConvertTo<DateTime>(Eval("addtime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss")%></td>
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