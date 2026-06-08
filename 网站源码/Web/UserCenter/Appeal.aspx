<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Appeal.aspx.cs" Inherits="Web.UserCenter.Appeal" %>

<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="UserJs" Src="/UserCenter/UserJs.ascx" %>
<!DOCTYPE html>
<html lang="en-US">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta name='robots' content='index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1' />
    <title>内容反馈</title>
    <LiteratureManager:css ID="css" runat="server" />
    <script src="/js/ajaxfileupload.js"></script>
    <style>
        .appeal-hidden-link { display: none; }
        .feedback-file-list { margin-top: 12px; display: grid; gap: 8px; }
        .feedback-file-item { display: flex; align-items: center; justify-content: space-between; gap: 12px; padding: 10px 12px; border: 1px solid #e6eaf0; border-radius: 8px; background: #fafbfc; color: #333; }
        .feedback-file-item span { overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
        .feedback-file-item button { border: 0; background: transparent; color: #c0392b; cursor: pointer; }
    </style>
</head>

<body class="front-glass" style="background: #ffff;">
    <div class="co-box layui-form">
        <div class="problem">
            <div class="form-div appeal-hidden-link">
                <label><span>*</span>原内容链接</label>
                <div class="layui-input-block">
                    <input type="text" id="appeal_url" value="<%=Server.HtmlEncode(defaultUrl) %>"/>
                </div>
            </div>
            <div class="form-div">
                <label><span>*</span>请描述反馈内容</label>
                <div class="layui-input-text">
                    <textarea placeholder="请描述您发现的问题或需要补充的情况，这将有助于我们尽快处理。" id="appeal_info"></textarea>
                </div>
               
            </div>
            <div class="form-div">
                <label class="copyrightItem" for="imagesUpload">
                    <input type="file" accept="image/*,.pdf,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.txt,.zip,.rar,.7z" name="imagesUpload" id="imagesUpload" style="display: none;"  onchange="imagesUploadFunc(this)">
                    <div class="copyrightBut">
                        <img src="/images/shangchuan1.png" />上传附件
                    </div>
                </label>
                <div class="inp-v">
                    <div class="imgBox">
                        <div class="imgList" id="imgList">
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-btn">
                <button type="button" class="form-btn0">取消</button>
                <button class="form-btn1" id="appealbtn" onclick="AppealAddFunc()">确定</button>
            </div>
        </div>
    </div>
    
    <LiteratureManager:UserJs ID="UserJs" runat="server" />
    <script>
        function AppealAddFunc(btn_name) {
            $("#appealbtn").attr("disabled", "true");
            var isyes = true;

            var appeal_info = $("#appeal_info").val();
            if (!(appeal_info || "").trim()) {
                isyes = false;
                $("#appeal_info").focus();
            }

            var appeal_url = $("#appeal_url").val() || window.location.href;
            if (!(appeal_url || "").trim()) {
                isyes = false;
                $("#appeal_url").focus();
            }

            var upload_pic_info = []
            $("input[name='upload_pic_info']").each(function () {
                upload_pic_info.push($(this).val())
            });
           
            var param1_json = { // 提交数据
                "btn": "AppealAdd",
                "url": appeal_url,
                "info": appeal_info,
                "ImgArr": upload_pic_info
            }
            $.ajax({
                url: "/Inc/UserCommon.ashx",
                cache: true,
                async: false,
                data: JSON.stringify(param1_json),
                dataType: "json",
                type: "POST",
                success: function (datas) {
                    $("#appealbtn").removeAttr("disabled");
                    if (datas.status == 1) {
                        layer.msg(datas.info, {
                            icon: 1,
                            time: 1500, // 显示3秒后自动关闭
                            end: function (layero, index) {
                                parent.layer.closeAll();
                            }
                        });
                    } else {
                        layer.msg(datas.info, { icon: 0 });
                    }
                },
                error: function (err) {
                    $("#appealbtn").removeAttr("disabled");
                    console.log(JSON.stringify(err))
                }
            });
        }
        // 多图
        function imagesUploadFunc(obj_this) {
            var img_num = document.getElementById("imgList").getElementsByClassName("upload-item").length + document.getElementById("imgList").getElementsByClassName("feedback-file-item").length;
            if (parseInt(img_num) < 5) {
                var x = document.getElementById("imagesUpload").value;
                if (x == "") {
                    /* layer.msg("请选择文件", { icon: 0 });*/
                    return false;
                } else {
                    $.ajaxFileUpload({
                        type: "post",
                        url: '/Inc/Upload_Img.aspx?btn=upload_feedback_file', //用于文件上传的服务器端请求地址
                        secureuri: false, //是否需要安全协议，一般设置为false
                        fileElementId: 'imagesUpload', //文件上传域的ID
                        dataType: 'json', //返回值类型 一般设置为json
                        success: function (data, status)  //服务器成功响应处理函数
                        {
                            if (data.error == 1) {
                                var isImage = /^(gif|jpg|jpeg|png)$/i.test(data.ext || "");
                                var displayName = data.name || data.url;
                                var html_img = isImage
                                    ? `<div class="upload-item"> 
                               <img src="`+ data.url + `" /> <input type="hidden" name="upload_pic_info" value="` + data.url + `">
                                 <div class="del" onclick="delUserImgFunc(this)" data-imgurl="` + data.url + `"><img src="/images/del.png" /></div>
                            </div>`
                                    : `<div class="feedback-file-item">
                                <span>` + displayName + `</span>
                                <input type="hidden" name="upload_pic_info" value="` + data.url + `">
                                <button type="button" onclick="delUserImgFunc(this)" data-imgurl="` + data.url + `">删除</button>
                            </div>`;
                                $('.imgList').append(html_img);
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
            }
            else {
                layer.msg('最多可上传5张图片！', { icon: 0 });
            }
            return true
        }


        $('.form-btn0').click(function () {
            parent.layer.closeAll();
        })

    </script>
</body>

</html>
