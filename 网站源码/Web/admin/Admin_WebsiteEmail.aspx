<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_WebsiteEmail.aspx.cs" Inherits="Web.admin.Admin_WebsiteEmail" %>
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
            <script src="/admin/js/JScript.js"></script>
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
                                        <label class="form-label">邮箱号（发件）<span>*</span></label>
                                        <asp:TextBox ID="emailnum" TextMode="SingleLine" runat="server" CssClass="txt form-control" autocomplete="off" reg="^.+$" tip="不能为空"></asp:TextBox>
                                    </div>
                                </div>
                            <div class="card-body">
                                    <div class="mb-6 ">
                                        <label class="form-label">发件名称<span>*</span></label>
                                        <asp:TextBox ID="emailname" TextMode="SingleLine" runat="server" CssClass="txt form-control" autocomplete="off" reg="^.+$" tip="不能为空"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="mb-6 ">
                                        <label class="form-label">密码（发件）<span>*</span></label>
                                        <asp:TextBox ID="emailpasswd" TextMode="SingleLine" runat="server" CssClass="txt form-control" autocomplete="off" reg="^.+$" tip="不能为空"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="mb-6 ">
                                        <label class="form-label">邮箱号（收件）<span>*</span></label>
                                        <asp:TextBox ID="email_to" TextMode="SingleLine" runat="server" CssClass="txt form-control" autocomplete="off" reg="^.+$" tip="不能为空"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="mb-6 ">
                                        <label class="form-label">端口<span>*</span></label>
                                        <asp:TextBox ID="smtpserverport" TextMode="SingleLine" runat="server" CssClass="txt form-control" autocomplete="off" reg="^.+$" tip="不能为空"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="mb-6 ">
                                        <label class="form-label">Smtp服务器<span>*</span></label>
                                        <asp:TextBox ID="host" TextMode="SingleLine" runat="server" CssClass="txt form-control" autocomplete="off" reg="^.+$" tip="不能为空"></asp:TextBox>
                                    </div>
                                </div>
                            <!--end::Body-->
                            <!--begin::Footer-->
                            <div class="card-footer">
                                <asp:Button ID="Button3" Text=" 保 存 " CssClass="btn btn-primary" runat="server" OnClick="OnClick_AddUp"/>
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
    </div>
    <!--end::App Content-->
</form>
<%} %>
    </body>
</html>