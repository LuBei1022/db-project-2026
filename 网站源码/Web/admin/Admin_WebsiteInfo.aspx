<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_WebsiteInfo.aspx.cs" Inherits="Web.admin.Admin_WebsiteInfo" %>

<%@ Register TagPrefix="LiteratureManager" TagName="Inc" Src="Inc.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="class_menu" Src="class_menu.ascx" %>
<%if (isLoading)
    {  %>
<LiteratureManager:Inc ID="Inc1" runat="server" />
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
                        <div class="card-body">
                            <div class="mb-6 ">
                                <label class="form-label">版权所有<span>*</span></label>
                                <asp:TextBox ID="companyname" TextMode="SingleLine" runat="server" CssClass="txt form-control" reg="^.+$" tip="不能为空"></asp:TextBox>
                            </div>
                        </div>
                        <div class="card-body">
                            <div class="mb-6 ">
                                <label class="form-label">底部版权信息</label>
                                <asp:TextBox ID="banquan" TextMode="SingleLine" runat="server" CssClass="txt form-control"></asp:TextBox>
                            </div>
                        </div>
                        <!--end::Body-->
                        <!--begin::Footer-->
                        <div class="card-footer">
                            <asp:Button ID="Button3" Text=" 保 存 " CssClass="btn btn-primary" runat="server" OnClientClick="return AddUpFunc()" OnClick="OnClick_AddUp" />
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
