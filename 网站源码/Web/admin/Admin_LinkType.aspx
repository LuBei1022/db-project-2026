<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_LinkType.aspx.cs" Inherits="Web.admin.Admin_LinkType" %>
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

<form id="form1" runat="server">
    <!--begin::App Content-->
    <div class="app-content">
        <asp:Panel ID="Big" runat="server" Visible="false">
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
                                <div class="card-title">一级菜单管理</div>
                            </div>
                            <!--end::Header-->
                            <!--begin::Form-->
                            <form>
                                <!--begin::Body-->
                                <div class="card-body">
                                    <div class="mb-6  ">
                                        <label class="form-label">添加大类<span>*</span></label>
                                        <asp:TextBox ID="NameType" TextMode="SingleLine" runat="server" CssClass="form-control" reg="^.+$" tip="不能为空"></asp:TextBox>
                                    </div>
                                </div>
                                <!--end::Body-->
                                <!--begin::Footer-->
                                <div class="card-footer">
                                    <asp:Button ID="Button3" runat="server" OnClick="OnClick_BigClass" Text="确 定" CssClass="btn btn-primary" />
                                     <input type="button" name="button" id="button" value=" 返 回 " class="btn submit-but" onclick="history.go(-1)">
                                </div>
                                <!--end::Footer-->
                            </form>
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
        <asp:Panel ID="Small" runat="server" Visible="false">
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
                                <div class="card-title">二级菜单管理</div>
                            </div>
                            <!--end::Header-->
                            <!--begin::Form-->
                            <form>
                                <!--begin::Body-->
                                <div class="card-body">
                                    <div class="mb-3">
                                        <label for="validationCustom04" class="form-label">所属一级菜单<span>*</span></label>
                                        <asp:DropDownList ID="BigClassValue" runat="server" CssClass="form-control form-select" reg="^.+$" tip="一定要选择哟"></asp:DropDownList>
                                    </div></div>
                                    <div class="card-body">
                                    <div class="mb-6  ">
                                        <label class="form-label">二级菜单名称<span>*</span></label>
                                        <asp:TextBox ID="SmallTypeName" TextMode="SingleLine" runat="server" CssClass="form-control" reg="^.+$" tip="不能为空"></asp:TextBox>
                                    </div></div>
                                    <div class="card-body">
                                    <div class="mb-6  ">
                                        <label class="form-label">跳转链接地址<span>*</span></label>
                                        <asp:TextBox ID="SmallClassUrl" TextMode="SingleLine" runat="server" CssClass="form-control" reg="^.+$" tip="不能为空"></asp:TextBox>
                                    </div>
                                </div>
                                <!--end::Body-->
                                <!--begin::Footer-->
                                <div class="card-footer">
                                    <asp:Button ID="Button1" Text=" 保 存 " CssClass="btn btn-primary" runat="server" OnClick="OnClick_SmallClass" />
                                     <input type="button" name="button" id="button" value=" 返 回 " class="btn submit-but" onclick="history.go(-1)">
                                </div>
                                <!--end::Footer-->
                            </form>
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
        <asp:Panel ID="Main" runat="server" Visible="false">
            <!--begin::Container-->
            <div class="container-fluid">
                <div class="col-md-12">
                    <!-- /.card -->
                    <div class="card mb-12">
                        <div class="card-body p-0">
                            <table class="table table-sm">
                                <tbody>
                                    <asp:Repeater ID="myRepeater" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
                                        <itemtemplate>
                                            <tr>
                                                <th><%#Function.HtmlDiscode(Eval("popedom_name").ToString())%></th>
                                                <th>&nbsp;</th>
                                                <th class="textAlignC">
                                                    <div id='orderid_2_<%#Eval("id")%>'><span class="orderidVal"><%#Eval("orderid")%></span>&nbsp;&nbsp;<span style="cursor: pointer;" onclick="$('#orderid_2_<%#Eval("id")%>').hide();$('#orderid_1_<%#Eval("id")%>').show();">编辑</span></div>
                                                    <div id='orderid_1_<%#Eval("id")%>' style="display: none;">
                                                        <input type="text" id='orderid_3_<%#Eval("id")%>' value="<%#Eval("orderid")%>" class="txt" reg="^\d+$" tip="必须填写数字" name="orderid<%#Eval("id")%>" onkeypress="return event.keyCode>=48&&event.keyCode<=57||event.keyCode==46" onpaste="return !clipboardData.getData('text').match(/\D/)" style="ime-mode: Disabled; width: 60px;" ondragenter="return false">
                                                        <input type="button" class="btn btn-primary textAlignB" value="修改" onclick="$('td22<%#Eval("id")%>    ').hide();$('orderid_1_<%#Eval("id")%>').show();UpdateOrder(<%#Eval("id")%>, 'orderid', 'popedom', $('#orderid_3_<%#Eval("id")%>').val());" />
                                                    </div>
                                                </th>
                                                <th class="textAlignC"><a class="badge text-bg-success" href="Admin_LinkType.aspx?Action=UpBig&ID=<%#Eval("id")%>&MenuId=<%=Request.QueryString["MenuId"]%>">编辑</a>
                                                    <a class="badge text-bg-danger" data-href="Admin_LinkType.aspx?Action=Del&ID=<%#Eval("id")%>&MenuId=<%=Request.QueryString["MenuId"]%>&BackURL=<%#Function.GetEncodeURL()%>" onclick="DataDelFunc(this)">删除</a></th>
                                            </tr>
                                            <asp:Repeater ID="myRepeater2" runat="server">
                                                <itemtemplate>
                                                    <tr class="hover">
                                                        <td><%#Function.HtmlDiscode(Eval("popedom_name").ToString()) %></td>
                                                        <td>&nbsp;<%#Function.HtmlDiscode(Eval("popedom_url").ToString())%></td>
                                                        <td class="textAlignC">
                                                            <div id='orderid_2_<%#Eval("id")%>'><span class="orderidVal"><%#Eval("orderid")%></span>&nbsp;&nbsp;<span style="cursor: pointer;" onclick="$('#orderid_2_<%#Eval("id")%>').hide();$('#orderid_1_<%#Eval("id")%>').show();">编辑</span></div>
                                                            <div id='orderid_1_<%#Eval("id")%>' style="display: none;">
                                                                <input type="text" id='orderid_3_<%#Eval("id")%>' value="<%#Eval("orderid")%>" class="txt" reg="^\d+$" tip="必须填写数字" name="orderid<%#Eval("id")%>" onkeypress="return event.keyCode>=48&&event.keyCode<=57||event.keyCode==46" onpaste="return !clipboardData.getData('text').match(/\D/)" style="ime-mode: Disabled; width: 60px;" ondragenter="return false">
                                                                <input type="button" class="btn btn-primary textAlignB" value="修改" onclick="$('td22<%#Eval("id")%>    ').hide();$('orderid_1_<%#Eval("id")%>').show();UpdateOrder(<%#Eval("id")%>, 'orderid', 'popedom', $('#orderid_3_<%#Eval("id")%>').val());" />
                                                            </div>
                                                        </td>
                                                        <td class="textAlignC">
                                                            <a class="badge text-bg-success" href="Admin_LinkType.aspx?Action=UpSmall&ID=<%#Eval("ID")%>&ParentID=<%#Eval("popedom_father") %>&MenuId=<%=Request.QueryString["MenuId"]%>">编辑</a>
                                                            <a class="badge text-bg-danger" data-href="Admin_LinkType.aspx?Action=Del&ID=<%#Eval("ID")%>&MenuId=<%=Request.QueryString["MenuId"]%>&BackURL=<%#Function.GetEncodeURL()%>" onclick="DataDelFunc(this)">删除</a>
                                                        </td>
                                                    </tr>
                                                </itemtemplate>
                                            </asp:Repeater>
                                        </itemtemplate>
                                    </asp:Repeater>



                                </tbody>
                            </table>
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