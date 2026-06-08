<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="code_tel.ascx.cs" Inherits="Web.code_tel" %>

<style>
    .code_popup {
	position: fixed;
	top: 0;
	left: 0;
	width: 100%;
	height: 100%;
	z-index: 9999999999999;
	display: none;
}.ys-popup .popup-bg, .code_popup .popup-bg {
    position: absolute;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    z-index: 0;
    background: rgba(0, 0, 0, .6);
    backdrop-filter: blur(2px);
}.ys-popup .popup-box, .code_popup .popup-box {
    position: absolute;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
    pointer-events: none;
    z-index: 1;
}.ys-popup .popup-content, .code_popup .popup-content {
    background: #fff;
    color: #333;
    width: 80vw;
    max-width: 600px;
    height: auto;
    border-radius: 10px;
    position: relative;
    padding: 20px;
    pointer-events: all;
}.ys-popup .popup-content .popup-neirong, .code_popup .popup-content .popup-neirong {
    height: calc(100% - 61px);
    overflow-y: scroll;
    font-size: 14px;
    line-height: 1.5;
}
</style>
<div class="code_popup" id="code_popup">
    <div class="popup-bg" id="code_popupbg1"></div>
    <div class="popup-box">
        <div class="popup-content" style="width: auto;">
            <div class="popup-neirong" style="width: 310px; overflow: hidden;">
                <div class="container">
                    <div id="captcha" style="position: relative"></div>
                    <div id="msgtxt"></div>
                </div>
            </div>
        </div>
    </div>
</div>
<input type="hidden" id="img_x" value="0" />
<input type="hidden" id="img_y" value="0" />

<script>
    function setCookie(cname, cvalue, exdays) {
        var str = cname + "=" + cvalue;
        if (exdays != 0) {
            var d = new Date();
            d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
            str += "; expires=" + d.toGMTString();
        }
        document.cookie = str;
    }
    function getCookie(cname) {
        var name = cname + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i].trim();
            if (c.indexOf(name) == 0) { return c.substring(name.length, c.length); }
        }
        return "";
    }

    function clearCookie(name) {
        setCookie(name, "", -1);
    } 
</script>


<script type="text/javascript" src="/js/jigsaw.js"></script>
<script type="text/javascript">



    function codePopupFunc() {
        $("#code_popup").show();
        $("body").addClass("overflow");
    }

    function cleanMsg() {
        document.getElementById('msgtxt').innerHTML = ''
    }

    jigsaw.init({
        el: document.getElementById('captcha'),
        onSuccess: function () {
            var img_x = getCookie("img_x");
            var img_y = getCookie("img_y");
            if (parseInt(img_x) > 0 && parseInt(img_y) > 0) {
                clearCookie("img_y");
                clearCookie("img_x");
                $("#toplogin-codebtn").removeAttr("disabled");
                this.reset();
                $("#code_popup").hide();
                $("body").removeClass("overflow");
                requestLoginCode(img_x, img_y);
            } else {
                layer.msg('异常，请再试一次！', { icon: 0 });
            }
        },
        onFail: cleanMsg,
        onRefresh: cleanMsg
    })


    var flag = 1;
    var i = 60;
    function countTimeout() {
        i = i - 1;
        $("#toplogin-codebtn").html(i + "s");
        $("#toplogin-codebtn").attr("disabled", "true");
        if (i == 0) {
            $("#toplogin-codebtn").removeAttr("disabled");
            $("#toplogin-codebtn").html("发送验证码");
            flag = 1;
            i = 60;
            return;
        }
        setTimeout('countTimeout()', 1000);
    }
</script>
