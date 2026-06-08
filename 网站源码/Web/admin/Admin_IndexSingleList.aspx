<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_IndexSingleList.aspx.cs" Inherits="Web.admin.Admin_IndexSingleList" %>

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
            <%=Function.KindEditor("info_", 1)%>
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
                                    <label class="form-label">所属栏目：<span> <asp:Literal ID="tbclass_name" runat="server"></asp:Literal></span></label>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="mb-6 ">
                                    <label class="form-label">标题<span>*</span></label>
                                    <asp:TextBox ID="name" TextMode="SingleLine" runat="server" CssClass="txt form-control" reg="^.+$" tip="不能为空"></asp:TextBox>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="mb-3">
                                    <label class="form-label">主图</label>
                                    <div class="form-but">
                                        <div class="btn-img">
                                            <label for="upload_pic_img" class="btn btn-success">
                                                选择图片<asp:FileUpload ID="upload_pic_img" runat="server" accept="image/*" class="btn btn-sm btn-primary btnimg" value="" />
                                            </label>
                                            <span class="help-block" style="display: block;">（建议尺寸：100px*100px）</span>
                                        </div>
                                        <div class="div-group-img">
                                            <asp:Image ID="upload_pic_img_img" Target="_blank" runat="server" ImageUrl="/admin/images/nophoto.gif" />
                                            <asp:HiddenField ID="upload_pic_img_Old" runat="server" />
                                            <div class="form-radio">
                                                <div class="form-check">
                                                    <input type="radio" name="del_upload_pic_img" id="del_upload_pic_img_0" value="0" runat="server" class="form-check-input" />
                                                    <label class="form-check-label" for="del_upload_pic_img_0">修改图片</label>
                                                </div>
                                                <div class="form-check">
                                                    <input type="radio" name="del_upload_pic_img" id="del_upload_pic_img_1" value="1" runat="server" class="form-check-input">
                                                    <label class="form-check-label" for="del_upload_pic_img_1">删除图片</label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="mb-3">
                                    <label class="form-label">区块类型</label>
                                    <div class="form-but">
                                        <div class="div-group-img">
                                            <div class="form-radio">
                                                <div class="form-check">
                                                    <input type="radio" name="istype" id="istype1" value="1" runat="server" onclick="isurlfunc(this)" class="form-check-input isradio" />
                                                    <label class="form-check-label" for="istype1">单图文</label>
                                                </div>
                                                <div class="form-check">
                                                    <input type="radio" name="istype" id="istype2" value="2" runat="server" onclick="isurlfunc(this)" class="form-check-input isradio">
                                                    <label class="form-check-label" for="istype2">外链</label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div> 
                            <div class="card-body  ClassURL_tr ClassURL_tr_1">
                                <div class="mb-3">
                                    <label class="form-label">Banner</label>
                                    <div class="form-but">
                                        <div class="btn-img">
                                            <label for="upload_pic_pc" class="btn btn-success">
                                                选择图片<asp:FileUpload ID="upload_pic_pc" runat="server" accept="image/*" class="btn btn-sm btn-primary btnimg" value="" />
                                            </label>
                                            <span class="help-block" style="display: block;">（建议尺寸：1920px*300px）</span>
                                        </div>
                                        <div class="div-group-img">
                                            <asp:Image ID="upload_pic_pc_img" Target="_blank" runat="server" ImageUrl="/admin/images/nophoto.gif" />
                                            <asp:HiddenField ID="upload_pic_pc_Old" runat="server" />
                                            <div class="form-radio">
                                                <div class="form-check">
                                                    <input type="radio" name="del_upload_pic_pc" id="del_upload_pic_pc_0" value="0" runat="server" class="form-check-input" />
                                                    <label class="form-check-label" for="del_upload_pic_pc_0">修改图片</label>
                                                </div>
                                                <div class="form-check">
                                                    <input type="radio" name="del_upload_pic_pc" id="del_upload_pic_pc_1" value="1" runat="server" class="form-check-input">
                                                    <label class="form-check-label" for="del_upload_pic_pc_1">删除图片</label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="card-body  ClassURL_tr ClassURL_tr_1">
                                <div class="mb-6 ">
                                    <label class="form-label">页面描述</label>
                                    <asp:TextBox ID="description" TextMode="SingleLine" runat="server" CssClass="txt form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="card-body  ClassURL_tr ClassURL_tr_1">
                                <div class="mb-3">
                                    <label class="form-label">详情</label>
                                    <asp:TextBox ID="info_" TextMode="MultiLine" runat="server" Style="width: 100%; height: 450px; visibility: hidden;"></asp:TextBox>
                                </div>
                            </div>
                            <div class="card-body ClassURL_tr ClassURL_tr_2">
                                <div class="mb-6 ">
                                    <label class="form-label">外链</label>
                                    <asp:TextBox ID="url" TextMode="SingleLine" runat="server" CssClass="txt form-control"></asp:TextBox>
                                </div>
                            </div>
                            <!--end::Body-->
                            <!--begin::Footer-->
                            <div class="card-footer">
                                <asp:Button ID="Button3" Text=" 保 存 " CssClass="btn btn-primary" runat="server" OnClick="OnClick_AddUp"  OnClientClick="return OnClickAddUpFunc()" />
                                <input type="button" name="button" id="button" value=" 返 回 " class="btn submit-but" onclick="history.go(-1)">
                            </div>
                            <!--end::Footer-->
                            <!--end::Form-->
                        </div>
                        <!--end::Quick Example-->
                    </div>
                    <!--end::Col-->
                    <script>
                        $(function () {
                            $("#upload_pic_img").uploadPreview({
                                Img: "upload_pic_img_img", Width: 100, Height: 100, ImgType: ["gif", "jpeg", "jpg", "bmp", "png"], Callback: function () {

                                }
                            });
                            $("#upload_pic_pc").uploadPreview({
                                Img: "upload_pic_pc_img", Width: 100, Height: 100, ImgType: ["gif", "jpeg", "jpg", "bmp", "png"], Callback: function () {

                                }
                            });
                        });
                    </script>
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
                                <label class="col-form-label">标题</label>
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
                                        <th>ID</th>
                                        <th>标题</th>
                                        <th>图片</th>
                                        <th>编辑时间</th>
                                        <th class="textAlignC">显示状态</th>
                                        <th class="textAlignC">是否推荐</th>
                                        <th class="textAlignC">排序ID</th>
                                        <th>操作</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <itemtemplate>
                                            <tr class="hover">
                                                 <td><%#Eval("id").ToString()%></td>
                                                <td><%#Function.HtmlDiscodeWeb(Eval("name").ToString())%></td>
                                                 <td>
                                                    <img src="<%#Function.GetAdminUpload_Pic(Eval("upload_pic_img").ToString())%>" height="20" style="border: 1px solid #cccccc" class="tooltip_img"></td>
                                                <td><%#Function.ConvertTo<DateTime>(Eval("uptime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd HH:mm")%></td>
                                                <td class="textAlignC">
                                                    <img class="img_show" style="width: 14px; height: 14px; margin-top: 6px;" src="<%#Function.GetAdminIsShow(Eval("isshow").ToString(),"1")%>"
                                                        id='isshow<%#Eval("id")%>' border="0" style="cursor: pointer;" onclick="IsYes(<%#Eval("id")%>,'isshow','indexsingle_list')" />
                                                </td>
                                                <td class="textAlignC">
                                                    <img class="img_show" style="width: 14px; height: 14px; margin-top: 6px;" src="<%#Function.GetAdminIsShow(Eval("istop").ToString(),"1")%>"
                                                        id='istop<%#Eval("id")%>' border="0" style="cursor: pointer;" onclick="IsYes(<%#Eval("id")%>,'istop','indexsingle_list')" />
                                                </td>
                                                <td class="textAlignC">
                                                    <div id='orderid_2_<%#Eval("id")%>'><span class="orderidVal"><%#Eval("orderid")%></span>&nbsp;&nbsp;<span style="cursor: pointer;" onclick="$('#orderid_2_<%#Eval("id")%>').hide();$('#orderid_1_<%#Eval("id")%>').show();">编辑</span></div>
                                                    <div id='orderid_1_<%#Eval("id")%>' style="display: none;">
                                                        <input type="text" id='orderid_3_<%#Eval("id")%>' value="<%#Eval("orderid")%>" class="txt" reg="^\d+$" tip="必须填写数字" name="orderid<%#Eval("id")%>" onkeypress="return event.keyCode>=48&&event.keyCode<=57||event.keyCode==46" onpaste="return !clipboardData.getData('text').match(/\D/)" style="ime-mode: Disabled; width: 60px;" ondragenter="return false">
                                                        <input type="button" class="btn btn-primary textAlignB" value="修改" onclick="$('td22<%#Eval("id")%>    ').hide();$('orderid_1_<%#Eval("id")%>').show();UpdateOrder(<%#Eval("id")%>, 'orderid', 'indexsingle_list', $('#orderid_3_<%#Eval("id")%>').val());" />
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
