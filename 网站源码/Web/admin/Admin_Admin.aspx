<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_Admin.aspx.cs" Inherits="Web.admin.Admin_Admin" %>

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
    <style type="text/css">
        /*密码强度*/
        .pw-strength {
            clear: both;
            position: relative;
            width: 180px;
            margin-top: 2em;
        }

        .pw-bar {
            background: url(images/pwd-1.png) no-repeat;
            height: 14px;
            overflow: hidden;
            width: 179px;
        }

        .pw-bar-on {
            background: url(images/pwd-2.png) no-repeat;
            width: 0px;
            height: 14px;
            top: 0;
            position: absolute;
            transition: width .5s ease-in;
            -moz-transition: width .5s ease-in;
            -webkit-transition: width .5s ease-in;
            -o-transition: width .5s ease-in;
        }

        .pw-weak .pw-defule {
            width: 0px;
        }

        .pw-weak .pw-bar-on {
            width: 60px;
        }

        .pw-medium .pw-bar-on {
            width: 120px;
        }

        .pw-strong .pw-bar-on {
            width: 179px;
        }

        .pw-txt {
            padding-top: 2px;
            width: 180px;
            overflow: hidden;
        }

            .pw-txt span {
                color: #707070;
                float: left;
                font-size: 12px;
                text-align: center;
                width: 58px;
            }
    </style>
    <script type="text/javascript">
        $(function () {
            $('#Admin_Pwd').keyup(function () {
                var strongRegex = new RegExp("^(?=.{8,})(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*\\W).*$", "g");
                var mediumRegex = new RegExp("^(?=.{7,})(((?=.*[A-Z])(?=.*[a-z]))|((?=.*[A-Z])(?=.*[0-9]))|((?=.*[a-z])(?=.*[0-9]))).*$", "g");
                var enoughRegex = new RegExp("(?=.{6,}).*", "g");

                if (false == enoughRegex.test($(this).val())) {
                    $('#level').removeClass('pw-weak');
                    $('#level').removeClass('pw-medium');
                    $('#level').removeClass('pw-strong');
                    $('#level').addClass(' pw-defule');
                    //密码小于六位的时候，密码强度图片都为灰色 
                }
                else if (strongRegex.test($(this).val())) {
                    $('#level').removeClass('pw-weak');
                    $('#level').removeClass('pw-medium');
                    $('#level').removeClass('pw-strong');
                    $('#level').addClass(' pw-strong');
                    //密码为八位及以上并且字母数字特殊字符三项都包括,强度最强 
                }
                else if (mediumRegex.test($(this).val())) {
                    $('#level').removeClass('pw-weak');
                    $('#level').removeClass('pw-medium');
                    $('#level').removeClass('pw-strong');
                    $('#level').addClass(' pw-medium');
                    //密码为七位及以上并且字母、数字、特殊字符三项中有两项，强度是中等 
                }
                else {
                    $('#level').removeClass('pw-weak');
                    $('#level').removeClass('pw-medium');
                    $('#level').removeClass('pw-strong');
                    $('#level').addClass('pw-weak');
                    //如果密码为6为及以下，就算字母、数字、特殊字符三项都包括，强度也是弱的 
                }
                return true;
            });
        })
    </script>
    <script language="javascript">
        function unselectall() {
            if (document.myform.chkAll.checked) {
                document.myform.chkAll.checked = document.myform.chkAll.checked & 0;
            }
        }

        function CheckAll(form) {
            for (var i = 0; i < form.elements.length; i++) {
                var e = form.elements[i];
                if (e.Name != "chkAll" && e.disabled != true)
                    e.checked = form.chkAll.checked;
            }
        }

        function GetPopedom(name, fathername) {
            if (name.checked == true) {
                fathername.checked = true;
            }
        }

        function GetPopedom1(fathername, id) {
            if (fathername.checked == false) {
                $(".b_checkbox" + id).attr("checked", false);
            } else {
                $(".b_checkbox" + id).attr("checked", true);
            }
        }
    </script>
    <form id="form2" runat="server">
        <!--begin::App Content-->
        <div class="app-content">
            <asp:Panel ID="AddUser" runat="server" Visible="false">
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
                                        <asp:Label ID="Title" runat="server"></asp:Label>
                                    </div>
                                </div>
                                <!--end::Header-->
                                <!--begin::Form-->
                                <!--begin::Body-->
                                <div class="card-body">
                                    <div class="mb-6 ">
                                        <label class="form-label">管理员名称<span>*</span></label>
                                        <asp:TextBox ID="Admin_Name" TextMode="SingleLine" runat="server" CssClass="txt form-control" reg="^\w+$" strlen="4,50" tip="由数字、26个英文字母或者下划线组成的字符串"></asp:TextBox>
                                    </div>
                                </div>
                                <!--end::Body-->
                                <!--begin::Body-->
                                <div class="card-body">
                                    <div class="mb-6 ">
                                        <label class="form-label">密码<span>*</span></label>
                                        <asp:TextBox ID="Admin_Pwd" TextMode="Password" runat="server" CssClass="txt form-control" reg="^.+$" tip="不能为空"></asp:TextBox>
                                        <div id="level" class="pw-strength  pw-defule">
                                            <div class="pw-bar"></div>
                                            <div class="pw-bar-on"></div>
                                            <div class="pw-txt">
                                                <span>弱</span>
                                                <span>中</span>
                                                <span>强</span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <!--end::Body-->
                                <!--begin::Body-->
                                <div class="card-body">
                                    <div class="mb-6 ">
                                        <label class="form-label">确认密码<span>*</span></label>
                                        <asp:TextBox ID="Admin_Pwd1" TextMode="Password" runat="server" CssClass="txt form-control" reg="^.+$" tip="不能为空"></asp:TextBox>
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
                <script>
                    function AddUpFunc() {
                        var isyes = false;
                        var txt = "网络繁忙，请稍后再试！";
                        var paw = $("#Admin_Pwd").val();
                        var paw_q = $("#Admin_Pwd1").val();
                        if (paw == "" || paw.replace(/\s/g, "") == "") {
                            txt = "密码不能为空！";
                            isyes = false;
                        } else {
                            var strongRegex = new RegExp("^(?=.{8,})(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*\\W).*$", "g");
                            var mediumRegex = new RegExp("^(?=.{7,})(((?=.*[A-Z])(?=.*[a-z]))|((?=.*[A-Z])(?=.*[0-9]))|((?=.*[a-z])(?=.*[0-9]))).*$", "g");
                            var enoughRegex = new RegExp("(?=.{6,}).*", "g");

                            if (false == enoughRegex.test(paw)) {
                                txt = "密码最低6位数！";
                                isyes = false;
                            }
                            else if (strongRegex.test(paw)) {
                                if (paw == paw_q) {
                                    isyes = true;
                                } else {
                                    txt = "俩次输入的密码不一致！";
                                    isyes = false;
                                }
                            }
                            else if (mediumRegex.test(paw)) {
                                if (paw == paw_q) {
                                    isyes = true;
                                } else {
                                    txt = "俩次输入的密码不一致！";
                                    isyes = false;
                                }
                            }
                            else {
                                txt = "密码安全过低！";
                                isyes = false;
                            }
                        }
                        if (!isyes) {
                            layer.alert(txt, {
                                title: '提示'
                            }, function () {
                                layer.closeAll();
                            })
                            return false;
                        }
                    }
                </script>
            </asp:Panel>
            <asp:Panel ID="Main" runat="server" Visible="false">
                <!--begin::Container-->
                <div class="container-fluid">
                    <div class="col-md-12">
                        <!-- /.card -->
                        <div class="card mb-12">
                            <div class="card-body p-0">
                                <table class="table table-sm">
                                    <thead>
                                        <tr>
                                            <th>序号</th>
                                            <th>用户名</th>
                                            <th>密码</th>
                                            <th>最后登录IP</th>
                                            <th>操作</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="myRepeater" runat="server">
                                            <ItemTemplate>
                                                <tr class="hover">
                                                    <td><%#Eval("id")%></td>
                                                    <td><%#Eval("UserName")%><%#GetLock(Eval("Locks").ToString()) %></td>
                                                    <td><%#Eval("PassWord")%></td>
                                                    <td><%#Eval("LastLoginIp")%></td>
                                                    <td><%#GetString(Eval("UserName").ToString(),Convert.ToInt16(Eval("Locks")), Convert.ToInt16(Eval("id")))%></td>
                                                </tr>
                                            </ItemTemplate>
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
            <asp:Panel ID="Popedom" runat="server" Visible="false">
                <div class="container-div">
                    <div class="container-fluid">
                        <div class="card card-primary card-outline">
                            <div class="card-header">
                                <div class="card-title">修改管理员权限</div>
                            </div>
                        </div>
                    </div>
                </div>
                <asp:Repeater ID="myRepeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
                    <ItemTemplate>
                        <!--begin::Container-->
                        <div class="container-fluid" style="margin: 1em 0;">
                            <div class="col-md-12">
                                <!-- /.card -->
                                <div class="card mb-12">
                                    <div class="card-body p-0">
                                        <table class="table table-sm">
                                            <thead>
                                                <tr>
                                                    <th style="text-align: left;">
                                                        <input id="checkbox<%#Eval("id")%>" type="checkbox" class="form-check-input a_checkbox<%#Eval("id")%>" name="checkbox<%#Eval("id")%>" value='<%#Eval("id")%>' onclick='GetPopedom1(this,<%#Eval("id")%>)' <%#GetChecked(Eval("id").ToString())%> /><%#Eval("popedom_name")%>
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr class="hover">
                                                    <td align="left" style="text-align: left;">
                                                        <asp:Repeater ID="myRepeater2" runat="server">
                                                            <ItemTemplate>
                                                                <input id="checkbox<%#Eval("popedom_father")%>" name="checkbox<%#Eval("popedom_father")%>" type="checkbox" class="form-check-input b_checkbox<%#Eval("popedom_father")%>" value='<%#Eval("id")%>' onclick='GetPopedom(this,$("#checkbox"+<%#Eval("popedom_father")%>)[0])' <%#GetChecked(Eval("id").ToString())%> /><%#Eval("popedom_name")%>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                    <!-- /.card-body -->
                                </div>
                                <!-- /.card -->
                            </div>
                        </div>
                        <!--end::Container-->
                    </ItemTemplate>
                </asp:Repeater>
                <div class="container-fluid">
                    <!--begin::Footer-->
                    <div class="card-footer">
                        <asp:Button ID="Button2" Text=" 保 存 " CssClass="btn btn-primary" runat="server" OnClick="OnClick_Popedom" />
                        <input type="button" name="button" id="button" value=" 返 回 " class="btn submit-but" onclick="history.go(-1)">
                    </div>
                    <!--end::Footer-->
                </div>
            </asp:Panel>
        </div>
        <!--end::App Content-->
    </form>
    <%} %>
</body>
</html>
