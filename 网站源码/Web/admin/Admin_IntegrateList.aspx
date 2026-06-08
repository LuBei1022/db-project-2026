<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_IntegrateList.aspx.cs" Inherits="Web.admin.Admin_IntegrateList" %>

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
            <script src="js/JScript.js"></script>
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
                                    <label class="form-label">文献下载权益名称<span>*</span></label>
                                    <asp:TextBox ID="name" TextMode="SingleLine" runat="server" CssClass="txt form-control" reg="^.+$" tip="不能为空"></asp:TextBox>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="mb-6 ">
                                    <label class="form-label">兑换所需积分<span>*</span></label>
                                    <asp:TextBox ID="num_integrate" TextMode="SingleLine" runat="server" CssClass="txt form-control" reg="^.+$" tip="不能为空"></asp:TextBox>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="mb-3">
                                    <label class="form-label">封面图标</label>
                                    <div class="form-but">
                                        <div class="btn-img">
                                            <label for="upload_pic_img" class="btn btn-success">
                                                选择图片<asp:FileUpload ID="upload_pic_img" runat="server" accept="image/*" class="btn btn-sm btn-primary btnimg" value="" />
                                            </label>
                                        </div>
                                        <div class="div-group-img">
                                            <asp:Image ID="upload_pic_img_img" Target="_blank" runat="server" ImageUrl="/admin/images/nophoto.gif" />
                                            <asp:HiddenField ID="upload_pic_img_old" runat="server" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="mb-3">
                                    <label class="form-label">权益说明</label>
                                    <asp:TextBox ID="about_" TextMode="MultiLine" runat="server" CssClass="txt form-control"></asp:TextBox>
                                </div>
                            </div>
                            <!--end::Body-->
                            <!--begin::Footer-->
                            <div class="card-footer">
                                <asp:Button ID="Button3" Text=" 保 存 " CssClass="btn btn-primary" runat="server" OnClick="OnClick_AddUp" />
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
             <script>
                        $(function () {
                            $("#upload_pic_img").uploadPreview({
                                Img: "upload_pic_img_img", Width: 100, Height: 100, ImgType: ["gif", "jpeg", "jpg", "bmp", "png"], Callback: function () {

                                }
                            });
                        });
             </script>
        </asp:Panel>
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
                                <asp:Button ID="Button2" runat="server" OnClick="OnClick_Search" Text="搜索" CssClass="btn btn-success" />
                            </div>
                            <div class="cardItem" style="float: right;">
                                <a href="?Action=Add&MenuId=<%=MenuId %>&BackURL=<%=Function.GetEncodeURL()%>" class="btn btn-primary">添加</a>
                            </div>
                        </div>
                        <div class="card-body p-0">
                            <table class="table table-sm">
                                <thead>
                                    <tr>
                                        <th>序号</th>
                                        <th>图片</th>
                                        <th>下载权益</th>
                                        <th>所需积分</th>
                                        <th>编辑时间</th>
                                        <th class="textAlignC">排序ID</th>
                                        <th>操作</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <itemtemplate>
                                            <tr class="hover">
                                                 <td><%#Eval("xuhao").ToString()%></td>
                                                 <td>
                                                    <img src="<%#Function.GetAdminUpload_Pic(Eval("upload_pic_img").ToString())%>" height="20" style="border: 1px solid #cccccc" class="tooltip_img"></td>
                                                <td><%#Function.HtmlDiscodeWeb(Eval("name").ToString())%></td>
                                                <td><%#Function.HtmlDiscodeWeb(Eval("num_integrate").ToString())%></td>
                                                <td><%#Function.ConvertTo<DateTime>(Eval("uptime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd HH:mm")%></td>
                                                <td class="textAlignC">
                                                    <div id='orderid_2_<%#Eval("id")%>'><span class="orderidVal"><%#Eval("orderid")%></span>&nbsp;&nbsp;<span style="cursor: pointer;" onclick="$('#orderid_2_<%#Eval("id")%>').hide();$('#orderid_1_<%#Eval("id")%>').show();">编辑</span></div>
                                                    <div id='orderid_1_<%#Eval("id")%>' style="display: none;">
                                                        <input type="text" id='orderid_3_<%#Eval("id")%>' value="<%#Eval("OrderId")%>" class="txt" reg="^\d+$" tip="必须填写数字" name="orderid<%#Eval("id")%>" onkeypress="return event.keyCode>=48&&event.keyCode<=57||event.keyCode==46" onpaste="return !clipboardData.getData('text').match(/\D/)" style="ime-mode: Disabled; width: 60px;" ondragenter="return false">
                                                        <input type="button" class="btn btn-primary textAlignB" value="修改" onclick="$('td22<%#Eval("id")%>    ').hide();$('orderid_1_<%#Eval("id")%>').show();UpdateOrder(<%#Eval("id")%>, 'orderid', 'integrate_list', $('#orderid_3_<%#Eval("id")%>').val());" />
                                                    </div>
                                                </td>
                                                <td >
                                                     <a class="badge text-bg-success" href='?Action=Edit&MenuId=<%#MenuId %>&ID=<%#Eval("id")%>&BackURL=<%=Function.GetEncodeURL() %>'>编辑</a>
                                                    <a class="badge text-bg-danger" data-href='?Action=Del&MenuId=<%#MenuId %>&ID=<%#Eval("id")%>&BackURL=<%=Function.GetEncodeURL() %>' onclick="DataDelFunc(this)">删除</a>
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
