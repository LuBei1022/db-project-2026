<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="js.ascx.cs" Inherits="Web.js" %>

 <script>
     function searchLogAdd(log_searchkey) {
         window.location.href = "/LiteratureSearch.aspx?keyword=" + encodeURIComponent((log_searchkey || "").trim());
     }
     function LoginShowFunc() {
         layui.use(['layer', 'form'], function () {
             var $ = layui.jquery, layer = layui.layer;
             var form = layui.form;
             layer.open({
                 type: 1,
                 skin: 'layui-layer-login',
                title: '文献管理系统账号',
                 shadeClose: true,
                 shade: 0.6,
                 area: ['600px', '520px'],
                 content: $('#login'),
                 success: function (layero, index) {
                     form.render();
                 }
             });
         })
     }
 </script>

        <script>
            function ImgGifShow() {
                $('.pe-li-gif').hover(function () {
                    let url = $(this).data('gif')
                    if (url) {
                        $(this).attr('src', url)
                    }
                }, function () {
                    let url = $(this).data('img')
                    $(this).attr('src', url)
                }
                )
            }
            ImgGifShow()
        </script>
