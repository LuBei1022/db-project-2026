<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_AppealList.aspx.cs" Inherits="Web.admin.Admin_AppealList" %>

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
        <asp:Panel ID="AddUp" runat="server" Visible="false">
            <!--begin::Container-->
            <div class="container-fluid">
                <!--begin::Row-->
                <div class="row">
                    <!--begin::Col-->
                    <div class="col-md-6 offset-md-3">
                        <!--begin::Quick Example-->
                        <div class="card card-primary card-outline mb-4">
                            <!--begin::Header-->
                            <div class="card-header">
                                <div class="card-title">
                                 <asp:Label ID="Txt_Title" runat="server"></asp:Label>
                                </div>
                            </div>
                            <!--end::Header-->
                            <!--begin::Form-->
                            <!--begin::Body-->
                            <div class="card-body">
                                <div class="mb-6 ">
                                    <label class="form-label">反馈用户</label>
                                   <div class="form-control"> <asp:Label ID="usetinfo" runat="server"></asp:Label></div>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="mb-6 ">
                                    <label class="form-label">反馈日期</label>
                                    <div class="form-control"><asp:Label ID="addtime" runat="server"></asp:Label></div>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="mb-6 ">
                                    <label class="form-label">原内容链接</label>
                                    <div class="form-control"><asp:Label ID="url" runat="server"></asp:Label></div>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="mb-6 ">
                                    <label class="form-label">反馈内容</label>
                                    <div class="form-control"><asp:Label ID="info_" runat="server"></asp:Label></div>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="mb-3">
                                    <label class="form-label">相关照片</label>
                                    <div class="form-but">
                                       <asp:Repeater ID="ImgList" runat="server">
                                                    <ItemTemplate>
                                                        <div class="upload-item">
                                                            <img src="<%#Function.GetAdminUpload_Pic(Eval("upload_pic_info").ToString())%>" />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                    </div>
                                </div>
                            </div>
                            <!--end::Body-->
                            <!--begin::Footer-->
                            <div class="card-footer">
                                <input type="button" name="button" id="button" value=" 返 回 " class="btn submit-but" onclick="history.go(-1)">
                            </div>
                            <!--end::Footer-->
                            <!--end::Form-->
                        </div>
                        <!--end::Quick Example-->
                    </div>
                    <!--end::Col-->
                </div>
                <!--end::Row-->
            </div>
            <!--end::Container-->
        </asp:Panel>
        <asp:Panel ID="Main" runat="server">
            <!--begin::Container-->
            <div class="container-fluid">
                <div class="col-md-12">
                    <!-- /.card -->
                    <div class="card mb-12">
                        <div class="card-header cardList">
                            <div class="cardItem">
                                <label class="col-form-label">链接/反馈内容</label>
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
                                <asp:Button ID="Button2" runat="server" OnClick="OnClick_Search" Text="搜索" CssClass="btn btn-success" />
                            </div>
                        </div>
                        <div class="card-body p-0">
                            <table class="table table-sm">
                                <thead>
                                    <tr>
                                        <th>序号</th>
                                        <th>反馈用户</th>
                                        <th>原内容链接</th>
                                        <th>反馈时间</th>
                                        <th class="textAlignC">操作</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <itemtemplate>
                                            <tr class="hover">
                                                 <td><%#Eval("xuhao").ToString()%></td>
                                                <td><%#CommonUserFunc.GetUserInfoHtml(Eval("userid").ToString())%></td>
                                                <td><%#Function.HtmlDiscode(Eval("url").ToString())%></td>
                                                <td><%#Function.ConvertTo<DateTime>(Eval("addtime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss")%></td>
                                                <td class="textAlignC">
                                                     <a class="badge text-bg-success" href='?Action=Edit&MenuId=<%#MenuId %>&ID=<%#Eval("Id")%>&BackURL=<%=Function.GetEncodeURL() %>'>查看</a>
                                                </td>
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
