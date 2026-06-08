<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Account.aspx.cs" Inherits="Web.UserCenter.Account" %>

<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="left" Src="/UserCenter/left.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="code_tel" Src="/code_tel.ascx" %>

<!DOCTYPE html>
<html lang="en-US">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta name='robots' content='index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1' />
    <title>账号信息</title>
    <LiteratureManager:css ID="css" runat="server" />
    <script src="/js/ajaxfileupload.js"></script>
    <style>
        .xinxi-nr {
            cursor: pointer;
        }
    </style>
</head>

<body class="ac">
    <LiteratureManager:top ID="top" runat="server" />
    <div class="middle">
        <section class="account">
            <div class="w1200">
                <div class="accountBox">
                    <LiteratureManager:left ID="left" runat="server" />
                    <div class="accountR">
                        <div class="account-tt">
                            <h4>账号设置</h4>
                        </div>
                        <div class="xinxi">
                            <div class="xinxi-item">
                                <div class="xinxi-tt">头像</div>
                                <label class="uploadItem flex-box" for="imagesUpload">
                                    <div class="xinxi-nr">
                                        <img class="user-avatar-img" id="upload_pic_avatar" src="<%=CommonUserFunc.GetUserAvatarFunc(user_list.upload_pic_avatar) %>" />
                                    </div>
                                    <div class="xinxi-x">
                                        <input type="file" accept="image/*" name="imagesUpload" id="imagesUpload" style="display: none;" onchange="imagesUploadFunc(this)">
                                        <div class="xinxi-img">
                                            <img src="/images/xia.png" />
                                        </div>
                                    </div>
                                </label>
                            </div>
                            <div class="xinxi-item">
                                <div class="xinxi-tt">昵称</div>
                                <div class="up_user_name  flex-box">
                                    <div class="xinxi-nr">
                                        <h4 id="user_name"><%=Function.HtmlDiscode(user_list.name) %></h4>
                                    </div>
                                    <div class="xinxi-x">
                                        <div class="xinxi-img ">
                                            <img src="/images/xia.png" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="xinxi-item">
                                <div class="xinxi-tt">手机</div>
                                <div class="up_user_tel flex-box">
                                    <div class="xinxi-nr">
                                        <h4 id="user_tel"><%=Function.HtmlDiscode(user_list.tel) %></h4>
                                    </div>
                                    <div class="xinxi-x">
                                        <div class="xinxi-img">
                                            <img src="/images/xia.png" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="xinxi-item">
                                <div class="xinxi-tt">邮箱</div>
                                <div class="up_user_email flex-box">
                                    <div class="xinxi-nr">
                                        <h4 id="user_email"><%=Function.HtmlDiscode(user_list.email) %></h4>
                                    </div>
                                    <div class="xinxi-x">
                                        <div class="xinxi-img">
                                            <img src="/images/xia.png" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="xinxi-item" id="LoginOut" style="cursor: pointer;">
                                <div class="xinxi-tt">退出登录</div>
                                <div class="flex-box xinxi-img" >
                                <!-- <a href="/LoginOut"  class="flex-box xinxi-img" > -->
                                        <div class="xinxi-img">
                                            <img src="/images/xia.png" />
                                        </div>
                                    </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <script>
                function codeBtnFun() {
                    var tel = $("#toplogin-tel").val();
                    $("#toplogin-codebtn").attr("disabled", "true");
                    if (!(tel || "").trim()) {
                        $("#toplogin-codebtn").removeAttr("disabled");
                        layer.msg('请输入新手机号码！', { icon: 0 });
                    } else {
                        if (tel != $("#user_tel").html()) {
                            let reg = /^1[0-9]{10}$/;
                            if (!reg.test(tel)) {
                                $("#toplogin-codebtn").removeAttr("disabled");
                                layer.msg('新手机号码格式错误', { icon: 0 });
                            } else {
                                codePopupFunc();
                            }
                        } else {
                            $("#toplogin-codebtn").removeAttr("disabled");
                            layer.msg('请输入新手机号码', { icon: 0 });
                        }
                    }
                }
                function imagesUploadFunc(obj_this) {
                    var x = document.getElementById("imagesUpload").value;
                    if (x == "") {
                       /* layer.msg("请选择文件", { icon: 0 });*/
                        return false;
                    } else {
                        $.ajaxFileUpload({
                            type: "post",
                            url: '/Inc/uptouxiang.aspx', //用于文件上传的服务器端请求地址
                            secureuri: false, //是否需要安全协议，一般设置为false
                            fileElementId: 'imagesUpload', //文件上传域的ID
                            dataType: 'json', //返回值类型 一般设置为json
                            success: function (data, status)  //服务器成功响应处理函数
                            {
                                if (data.error == 1) {
                                    $(".user-avatar-img").attr("src", data.url);
                                } else {
                                    layer.msg(data.message, { icon: 0 });
                                }
                            },
                            error: function (data, status, e)//服务器响应失败处理函数
                            {
                                console.log("err_e", e);
                            }
                        })
                    }
                    return true
                } 
                layui.use('layer', function () {
                    var $ = layui.jquery, layer = layui.layer;
                    $('.up_user_name').click(function () {
                        var layer_up_user_name = layer.open({
                            type: 1,
                            title: '昵称*',
                            shadeClose: true,
                            shade: 0.8,
                            content: ` <div class="inp" ><input id="up_user_name" type="text" placeholder="请输入昵称" value="` + $("#user_name").html() + `" /></div>`,
                            btn: ['取消', '保存'],
                            btn1: function (index, layero) {
                                layer.close(layer_up_user_name);
                                return false;
                            },
                            btn2: function (index, layero) {
                                var up_user_name = $("#up_user_name").val();
                                console.log('up_user_name', up_user_name);
                                if (!(up_user_name || "").trim()) {
                                    $("#up_user_name").focus();
                                    layer.msg('请输入昵称', { icon: 0 });
                                } else {
                                    var param1_json = { // 提交数据
                                        "btn": "UpUserName",
                                        "name": up_user_name,
                                        "id": <%=user_list.id%>,
                                        }
                                        $.ajax({
                                            url: "/Inc/UserCommon.ashx",
                                            cache: true,
                                            async: false,
                                            data: JSON.stringify(param1_json),
                                            dataType: "json",
                                            type: "POST",
                                            success: function (datas) {
                                                if (datas.status == 1) {
                                                    $("#user_name").html(up_user_name);
                                                    layer.close(layer_up_user_name);
                                                } else {
                                                    layer.msg(datas.info, { icon: 0 });
                                                }
                                            },
                                            error: function (err) {
                                                console.log(JSON.stringify(err))
                                            }
                                        });
                                    }
                                    return false;
                                }
                            });
                        });
                    $('.up_user_tel').click(function () {
                        var layer_up_user_tel = layer.open({
                            type: 1,
                            title: '手机*',
                            shadeClose: true,
                            shade: 0.8,
                            content: ` 
                          <div class="inp" ><input id="toplogin-tel" type="text" placeholder="请输入新手机号" /></div>
                           <div class="inp inp_code"><input type="text" id="toplogin-code" placeholder="请输入验证码"><button id="toplogin-codebtn" onclick="codeBtnFun()">获取验证码</button>
			<input type="hidden" id="typeval" name="typeval" value="2"/></div>
                          `,
                            btn: ['取消', '保存'],
                            btn1: function (index, layero) {
                                layer.close(layer_up_user_tel);
                                return false;

                            },
                            btn2: function (index, layero) {
                                var up_user_tel = $("#toplogin-tel").val();
                                var up_user_code = $("#toplogin-code").val();
                                var img_x = $("#img_x").val();
                                var img_y = $("#img_y").val();
                                var typeval = $("#typeval").val();

                                if (!(up_user_tel || "").trim()) {
                                    $("#toplogin-tel").focus();
                                    layer.msg('请输入新手机号!', { icon: 0 });
                                } else {
                                    var param1_json = { // 提交数据
                                        "btn": "UpUserTel",
                                        "tel": up_user_tel,
                                        "code": up_user_code,
                                        "img_x": img_x,
                                        "img_y": img_y,
                                        "type": typeval,
                                        "id": <%=user_list.id%>,
                                    }
                                    $.ajax({
                                        url: "/Inc/UserCommon.ashx",
                                        cache: true,
                                        async: false,
                                        data: JSON.stringify(param1_json),
                                        dataType: "json",
                                        type: "POST",
                                        success: function (datas) {
                                            if (datas.status == 1) {
                                                $("#user_tel").html(up_user_tel);
                                                layer.close(layer_up_user_tel);
                                            } else {
                                                layer.msg(datas.info, { icon: 0 });
                                            }
                                        },
                                        error: function (err) {
                                            console.log(JSON.stringify(err))
                                        }
                                    });
                                }
                                return false;
                            }
                      });
                  })
                    $('.up_user_email').click(function () {
                        var layer_up_user_email = layer.open({
                            type: 1,
                            title: '邮箱*',
                            shadeClose: true,
                            shade: 0.8,
                            content: ` <div class="inp" ><input id="up_user_email" type="text" placeholder="请输入邮箱" value="` + $("#user_email").html() + `" /></div>`,
                            btn: ['取消', '保存'],
                            btn1: function (index, layero) {
                                layer.close(layer_up_user_email);
                                return false;

                            },
                            btn2: function (index, layero) {
                                var up_user_email = $("#up_user_email").val();
                                if (!(up_user_email || "").trim()) {
                                    $("#up_user_email").focus();
                                    layer.msg('请输入邮箱', { icon: 0 });
                                } else {
                                    var emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
                                    if (!emailRegex.test(up_user_email)) {
                                        $("#up_user_email").focus();
                                        layer.msg('邮箱格式错误！', { icon: 0 });
                                    } else {
                                        var param1_json = { // 提交数据
                                            "btn": "UpUserEmail",
                                            "email": up_user_email,
                                            "id": <%=user_list.id%>,
                                        }
                                        $.ajax({
                                            url: "/Inc/UserCommon.ashx",
                                            cache: true,
                                            async: false,
                                            data: JSON.stringify(param1_json),
                                            dataType: "json",
                                            type: "POST",
                                            success: function (datas) {
                                                if (datas.status == 1) {
                                                    $("#user_email").html(up_user_email);
                                                    layer.close(layer_up_user_email);
                                                } else {
                                                    layer.msg(datas.info, { icon: 0 });
                                                }
                                            },
                                            error: function (err) {
                                                console.log(JSON.stringify(err))
                                            }
                                        });
                                    }
                                }
                                return false;
                            }
                        });
                    })
                    $('#LoginOut').click(function(){
                         let up_jifen = layer.open({
                            type: 1,
                            skin: 'layui-layer-jifen',
                            title: false,
                            shadeClose: true,
                            shade: 0.6,
                            area: ['270px', '160px'],
                            content: `<div class="failure-box"><p>是否退出登录？</p></div>`,
                            btn: ['否', '是'],
                            btn1: function (index, layero) {
                                layer.close(up_jifen);
                                return false;
                            },
                            btn2: function (index, layero) {
                                 layer.close(up_jifen);
                                 window.location.href = "/LoginOut";
                                  return false;
                                 }
                                });
                            })
                        })
            </script>
        </section>
    </div>
    <LiteratureManager:code_tel ID="code_tel" runat="server" />
    <LiteratureManager:foot ID="foot" runat="server" />
</body>
</html>
