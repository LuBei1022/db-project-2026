<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="class_menu.ascx.cs" Inherits="Web.admin.class_menu" %>
<%if (!string.IsNullOrWhiteSpace(R_Menu))
    {  %>
<!--begin::App Content Header-->
<div class="app-content-header">
    <!--begin::Container-->
    <div class="container-fluid">
        <!--begin::Row-->
        <div class="row">
            <div class="col-sm-6">
                <h3 class="mb-0" style="display: flex;align-items: center;"><a href="javascript:window.document.location.reload()" target="main" >
                    <img src="images/Refresh.png" style="width:1em;margin-right:0.5em;"/></a><%=R_Menu %></h3>
            </div>
            <%if (!string.IsNullOrWhiteSpace(R_MenuStr))
                {  %>
            <div class="col-sm-6">
                <ol class="breadcrumb float-sm-end">
                    <%=R_MenuStr %>
                </ol>
            </div>
            <%} %>
        </div>
        <!--end::Row-->
    </div>
    <!--end::Container-->
</div>
<!--end::App Content Header-->
<%} %>