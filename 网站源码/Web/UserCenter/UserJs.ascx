<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserJs.ascx.cs" Inherits="Web.UserCenter.UserJs" %>

<script>
    function delUserImgFunc(obj_this) {
        $(obj_this).closest(".upload-item,.feedback-file-item").remove();
        var del_imgurl = $(obj_this).data("imgurl");
        var param1_json = { // 提交数据
            "btn": "DelUserImg",
            "url": del_imgurl
        }
        $.ajax({
            url: "/Inc/UserCommon.ashx",
            cache: true,
            async: false,
            data: JSON.stringify(param1_json),
            dataType: "json",
            type: "POST",
            success: function (datas) {
                console.log('datas', datas)
            },
            error: function (err) {
                console.log(JSON.stringify(err))
            }
        });
    }


    function delImgFunc(obj_this) {
        $(obj_this).parent(".upload-item").remove();
    }


    function renderSize(filesize) {
        if (filesize == null || filesize === '') {
            return "0 Bytes";
        }
        var unitArr = ["Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];
        var index = 0;
        var srcsize = parseFloat(filesize);
        index = Math.floor(Math.log(srcsize) / Math.log(1024));
        var size = srcsize / Math.pow(1024, index);
        size = size.toFixed(2); // 保留两位小数
        return size + " " + unitArr[index];
    }
</script>
