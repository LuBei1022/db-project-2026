<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Inc.ascx.cs" Inherits="Web.admin.Inc" %>
   <link rel="shortcut icon" type="image/x-icon" href="/images/favicon.ico">
<script src="js/jquery-1.8.3.min.js"></script>
<script type="text/javascript" src="../Inc/jquery.validator.pack.js"></script>
<link href="../Inc/form_validate.css" rel="stylesheet" type="text/css" />
<link href="css/page.css" rel="stylesheet" />
<link rel="stylesheet" href="css/adminlte.css" />
<link rel="stylesheet" href="css/layui.css" />
<link rel="stylesheet" href="css/style.css" />
<link rel="stylesheet" href="css/style_new.css" />
<script src="js/script.js"></script>
<script src="js/layui.js"></script>
<script type="text/javascript">
    //隐藏函数
    function IsYes(id, action, tb) {
        var obj = $("#" + action + id)
        $.ajax({
            url: "Ajax.ashx?action=" + action + "&tb=" + tb + "&id=" + id,
            cache: false,
            success: function (html) {
                obj.attr("src", html);
            }
        });
    }
    function IsYess(id, action, tb) {
        var obj = $("#" + action + "s" + id)
        $.ajax({
            url: "Ajax.ashx?action=" + action + "&tb=" + tb + "&id=" + id,
            cache: false,
            success: function (html) {
                obj.attr("src", html);
            }
        });
    }
    //排序提交处理
    function UpdateOrder(id, action, tb, val) {
        $("#" + action + "_2_" + id).show();
        $("#" + action + "_1_" + id).hide();
        $("#" + action + "_2_" + id + " ." + action + "Val").html('<img src=\"images/load_small.gif\" style=\"width:10px;height:10px;\" />');
        $.ajax({
            url: "Ajax.ashx?action=" + action + "&tb=" + tb + "&val=" + val + "&id=" + id,
            cache: false,
            success: function (html) {
                $("#" + action + "_2_" + id + " ." + action + "Val").html(html);
                $("#" + action + "_1_" + id).hide();
                $("#" + action + "_2_" + id).show();
            }
        });
    }

    function isurlfunc(obj) {
        $(".ClassURL_tr").hide();
        $(".ClassURL_tr_" + $(obj).val()).css("display", "block");
    }

    $(document).ready(function () {
        var obj_val_ = 0;
        $(".isradio").each(function () {
            if ($(this).prop("checked") == true || $(this).prop("checked") == "checked") {
                obj_val_ = $(this).val();
            }
        })

        if (obj_val_ && parseInt(obj_val_) > 0) {
            $(".ClassURL_tr").hide();
            $(".ClassURL_tr_" + obj_val_).css("display", "block");
        }
    });

    function DataDelFunc(obj_this) {
        layer.confirm('您确定要进行删除操作吗？', {
            title: '提示',
            btn: ['确定', '取消']
        }, function () {
            window.location.href = $(obj_this).data("href");
        }, function () {
            layer.closeAll();
        });
    }
    function PopupFunc(obj_this) {
        var data_name = $(obj_this).data("name");
        var data_url = $(obj_this).data("url");
        var index = layer.open({
            type: 2,
            id: 'maxminId',
            title: data_name,
            shadeClose: true,
            shade: 0.8,
            maxmin: false, //开启最大化最小化按钮
            area: ['100%', '100%'],
            content: data_url,
            end: function () {
               /* window.location.reload();*/
            }
        });
    }

    $(function () {
        var x = 10;
        var y = 20;
        $(".tooltip_img").mouseover(function (e) {
            var tooltip = "<div id='tooltip' style='position:absolute;display:none;border:1px solid #ccc;'><img src='" + this.src + "' width='150' alt='预览图'/><\/div>"; //创建 div 元素
            $("body").append(tooltip);	//把它追加到文档中 
            $("#tooltip")
                .css({
                    "top": (e.pageY + y) + "px",
                    "left": (e.pageX + x) + "px"
                }).show("fast");	  //设置x坐标和y坐标，并且显示
        }).mouseout(function () {
            $("#tooltip").remove();	 //移除 
        }).mousemove(function (e) {
            $("#tooltip")
                .css({
                    "top": (e.pageY + y) + "px",
                    "left": (e.pageX + x) + "px"
                });
        });
    })
</script>


