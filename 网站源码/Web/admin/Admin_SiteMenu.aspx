<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_SiteMenu.aspx.cs" Inherits="Web.admin.Admin_SiteMenu" %>
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
                                    所属栏目：<span style="color: #f8ac59; margin-right: 50px;"><%=GetMenuHtml() %></span> 
                                </div>
                            </div>
                            <!--end::Header-->
                            <!--begin::Form-->
                            <!--begin::Body-->
                            <div class="card-body">
                                <div class="mb-6 ">
                                    <label class="form-label">栏目名称<span>*</span></label>
                                    <asp:TextBox ID="name" TextMode="SingleLine" runat="server" CssClass="txt form-control" reg="^.+$" tip="不能为空"></asp:TextBox>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="mb-6 ">
                                    <label class="form-label">网页链接名<span>*</span></label>
                                    <asp:TextBox ID="urlnamebtn" TextMode="SingleLine" runat="server" CssClass="txt form-control" reg="^.+$" tip="不能为空"></asp:TextBox>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="mb-3">
                                    <label class="form-label">栏目类型</label>
                                    <div class="form-but">
                                        <div class="div-group-img">
                                            <div class="form-radio">
                                                 <asp:Repeater ID="Repeater2" runat="server">
                                        <ItemTemplate>
                                            <div class="form-check" style="width:100%;">
                                                    <label class="form-check-label" for="model_<%#Eval("id") %>">
                                                        <input type="radio" class="form-check-input isradio" id="model_<%#Eval("id").ToString() %>" name="model" value="<%#Eval("id").ToString() %>" <%#GetModelChecked(Eval("id").ToString()) %> />
                                                        <%#Function.HtmlDiscodeWeb(Eval("m_name").ToString()) %></label><img src="<%#Function.GetAdminUpload_Pic(Eval("upload_pic").ToString())%>" height="20" style="border: 1px solid #cccccc" class="tooltip_img">
                                                </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="card-body">
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
                            <div class="card-body">
                                <div class="mb-3">
                                    <label class="form-label">简介</label>
                                    <asp:TextBox ID="about" TextMode="MultiLine" runat="server" CssClass="txt form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="mb-3">
                                    <label class="form-label">是/否外链</label>
                                    <div class="form-but">
                                        <div class="div-group-img">
                                            <div class="form-radio">
                                                <div class="form-check">
                                                    <label class="form-check-label" for="isurl1">
                                                    <input type="radio" name="isurl" id="isurl1" value="1" runat="server" onclick="isurlfunc(this)" class="form-check-input isradio" />否</label>
                                                </div>
                                                <div class="form-check">
                                                    <label class="form-check-label" for="isurl2">
                                                    <input type="radio" name="isurl" id="isurl2" value="2" runat="server" onclick="isurlfunc(this)" class="form-check-input isradio">是</label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="card-body ClassURL_tr ClassURL_tr_2">
                                <div class="mb-6 ">
                                    <label class="form-label">外链网址</label>
                                    <asp:TextBox ID="ClassURL" TextMode="SingleLine" runat="server" CssClass="txt form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="card-body ClassURL_tr ClassURL_tr_1">
                                <div class="mb-6 ">
                                    <label class="form-label">页面描述</label>
                                    <asp:TextBox ID="description" TextMode="SingleLine" runat="server" CssClass="txt form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="card-body ClassURL_tr ClassURL_tr_1">
                                <div class="mb-3">
                                    <label class="form-label">详情(Web)</label>
                                    <asp:TextBox ID="info_" TextMode="MultiLine" runat="server" Style="width: 100%; height: 450px; visibility: hidden;"></asp:TextBox>
                                </div>
                            </div>
                            <!--end::Body-->
                            <!--begin::Footer-->
                            <div class="card-footer">
                                <asp:Button ID="Button3" Text=" 保 存 " CssClass="btn btn-primary" runat="server" OnClick="OnClick_AddUp" OnClientClick="return OnClickAddUpFunc()" />
                                <input type="button" name="button" id="button" value=" 返 回 " class="btn submit-but" onclick="history.go(-1)">
                            </div>
                            <!--end::Footer-->
                            <!--end::Form-->
                        </div>
                        <!--end::Quick Example-->
                    </div>
                    <!--end::Col-->
                    <script>
                        function OnClickAddUpFunc() {
                            var isyes = false;
                            $("input[name='model']").each(function () {
                                if ($(this).prop("checked") == true || $(this).prop("checked") == "checked") {
                                    isyes = true;
                                }
                            });
                            if (!isyes) {
                                layer.alert('请选择栏目类型！', {
                                    title: '提示'
                                }, function () {
                                    layer.closeAll();
                                })
                            }
                            if (isyes) {
                                isyes = false;
                                $("input[name='isurl']").each(function () {
                                    if ($(this).prop("checked") == true || $(this).prop("checked") == "checked") {
                                        isyes = true;
                                    }
                                });
                                if (!isyes) {
                                    layer.alert('请选择是/否外链！', {
                                        title: '提示'
                                    }, function () {
                                        layer.closeAll();
                                    })
                                }
                            }
                            return isyes;
                        }

                        $(function () {
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
                                <span>所属栏目：<span style="color: #f8ac59; margin-right: 50px;"><%=GetMenuHtml() %></span> </span>
                            </div>
                            <div class="cardItem" style="float: right;">
                                <%=GetTool(Request.QueryString["ParentId"])%>
                            </div>

                        </div>
                        <div class="card-body p-0">
                            <table class="table table-sm">
                                <thead>
                                    <tr>
                                       <th>选中</th>
                                        <th>ID</th>
                                        <th>栏目名称</th>
                                        <th>模板配置</th>
                                        <th>Banner</th>
                                        <th class="textAlignC">显示状态</th>
                                        <th class="textAlignC">菜单排序ID</th>
                                        <th class="textAlignC">是否头部显示</th>
                                        <th class="textAlignC">是否底部显示</th>
                                        <th class="textAlignC">预览</th>
                                        <th>操作</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <itemtemplate>
                                            <tr class="hover">
                                                <td><input name="id" type="checkbox" value="<%#Eval("id")%>" class="form-check-input checkbox"></td>
                                                <td><%#Eval("id").ToString()%></td>
                                                <td><a href="Admin_SiteMenu.aspx?MenuId=<%=Request.QueryString["MenuId"]%>&ParentID=<%#Eval("id")%>"><%#Function.HtmlDiscodeWeb(Eval("ClassName").ToString())%><%#GetCount(Eval("ID").ToString())%></a></td>
                                                <td><%#GetModel(Eval("Model").ToString())%></td>
                                                 <td>
                                                    <img src="<%#Function.GetAdminUpload_Pic(Eval("upload_pic_pc").ToString())%>" height="20" style="border: 1px solid #cccccc" class="tooltip_img"></td>
                                                <td class="textAlignC">
                                                    <img class="img_show" style="width: 14px; height: 14px; margin-top: 6px;" src="<%#Function.GetAdminIsShow(Eval("isshow").ToString(),"1")%>"
                                                        id='isshow<%#Eval("id")%>' border="0" style="cursor: pointer;" onclick="IsYes(<%#Eval("id")%>,'isshow','tbl_class')" />
                                                </td>
                                                <td class="textAlignC">
                                                    <div id='orderid_2_<%#Eval("id")%>'><span class="orderidVal"><%#Eval("orderid")%></span>&nbsp;&nbsp;<span style="cursor: pointer;" onclick="$('#orderid_2_<%#Eval("id")%>').hide();$('#orderid_1_<%#Eval("id")%>').show();">编辑</span></div>
                                                    <div id='orderid_1_<%#Eval("id")%>' style="display: none;">
                                                        <input type="text" id='orderid_3_<%#Eval("id")%>' value="<%#Eval("orderid")%>" class="txt" reg="^\d+$" tip="必须填写数字" name="orderid<%#Eval("id")%>" onkeypress="return event.keyCode>=48&&event.keyCode<=57||event.keyCode==46" onpaste="return !clipboardData.getData('text').match(/\D/)" style="ime-mode: Disabled; width: 60px;" ondragenter="return false">
                                                        <input type="button" class="btn btn-primary textAlignB" value="修改" onclick="$('td22<%#Eval("id")%>    ').hide();$('orderid_1_<%#Eval("id")%>').show();UpdateOrder(<%#Eval("id")%>, 'orderid', 'tbl_class', $('#orderid_3_<%#Eval("id")%>').val());" />
                                                    </div>
                                                </td>
                                                <td class="textAlignC">
                                                    <img class="img_show" style="width: 14px; height: 14px; margin-top: 6px;" src="<%#Function.GetAdminIsShow(Eval("istop").ToString(),"1")%>"
                                                        id='istop<%#Eval("id")%>'  border="0" style="cursor: pointer;" onclick="IsYes(<%#Eval("id")%>,'istop','tbl_class')" />
                                                </td>
                                                  <td class="textAlignC">
                                                    <img class="img_show" style="width: 14px; height: 14px; margin-top: 6px;" src="<%#Function.GetAdminIsShow(Eval("isfoot").ToString(),"1")%>"
                                                        id='isfoot<%#Eval("id")%>'  border="0" style="cursor: pointer;" onclick="IsYes(<%#Eval("id")%>,'isfoot','tbl_class')" />
                                                </td>
                                                 <td class="textAlignC"><a href="<%# Eval("isurl").ToString()=="0"?Function.HtmlDiscode(Eval("classurl").ToString()):Function.HtmlDiscode(CommonFunc.GetTopHtmlHref(Eval("id").ToString(),"0")) %>" target="_blank">（查看页面）</a></td>
                                                <td ><%#GetOperation(Eval("ParentId").ToString(), Eval("Id").ToString())%>
                                                </td>
                                            </tr>
                                        </itemtemplate>
                                    </asp:Repeater>
                                    <asp:Panel ID="DivNull" runat="server" Visible="true">
                                        <tr>
                                            <td colspan="20" style="text-align: center;">无相关数据!</td>
                                        </tr>
                                    </asp:Panel>
                                </tbody>
                            </table>
                            <div class="msdn">
                                
                                <div class="msdn-item">
                                     <label for="chkall" class="chkall">
                                        <input name="chkall" type="checkbox" id="chkall" value="all" class=" form-check-input checkbox" onclick="CheckAll(this.form)" />
                                        全选
                                        </label>

                                        <div class="btn-sm btn-div btn-danger" >
                                            <i class="iconfont icon-cuocha_kuai size12"></i>
                                            <asp:Button ID="submitid" runat="server" OnClick="DelSelect_Click" Text="清除所选栏目" CssClass="btn text-bg-danger" />
                                        </div>
                                        <input name="BackURL" type="hidden" id="BackURL" value="<%=LiteratureManager.Common.Function.GetDecodeURL()%>" />
                                         <input name="ParentId" type="hidden" id="ParentId" value="<%=Function.GetRequest("ParentId")%>" />
                                </div>
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
