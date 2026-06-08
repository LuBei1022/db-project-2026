var iMaxFilesize = 1048576 * 100; 
var sResultFileSize = '';
function bytesToSize(bytes) {
    var sizes = ['Bytes', 'KB', 'MB'];
    if (bytes == 0) return 'n/a';
    var i = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)));
    return (bytes / Math.pow(1024, i)).toFixed(1) + ' ' + sizes[i];
};
function fileSelected(btn_str) {
    document.getElementById('error' + btn_str).style.display = 'none';
    document.getElementById('warnsize' + btn_str).style.display = 'none';
    var oFile = document.getElementById('uploadify' + btn_str).files[0];
    if (!oFile) {
        document.getElementById('error' + btn_str).style.display = 'block';
        document.getElementById('warnsize' + btn_str).style.display = 'none';
        document.getElementById('fileinfo' + btn_str).style.display = 'none';
        document.getElementById('filename' + btn_str).innerHTML = '';
        document.getElementById('filesize' + btn_str).innerHTML = '';
        document.getElementById('filetype' + btn_str).innerHTML = '';
        $("#file_name" + btn_str).val('');
        $("#file_size" + btn_str).val('');
        $("#file_type" + btn_str).val('');
        document.getElementById('uploadify' + btn_str).outerHTML = document.getElementById('uploadify' + btn_str).outerHTML;
        return;
    } else {
        //var rFilter = /^(audio\/mp4|video\/mp4)$/i;
        //if (!rFilter.test(oFile.type)) {
        //    document.getElementById('error' + btn_str).style.display = 'block';
        //    document.getElementById('warnsize' + btn_str).style.display = 'none';
        //    document.getElementById('fileinfo' + btn_str).style.display = 'none';
        //    document.getElementById('filename' + btn_str).innerHTML = '';
        //    document.getElementById('filesize' + btn_str).innerHTML = '';
        //    document.getElementById('filetype' + btn_str).innerHTML = '';
        //    $("#file_name" + btn_str).val('');
        //    $("#file_size" + btn_str).val('');
        //    $("#file_type" + btn_str).val('');
        //    document.getElementById('uploadify' + btn_str).outerHTML = document.getElementById('uploadify' + btn_str).outerHTML;
        //    return;
        //}
        if (oFile.size > iMaxFilesize) {
            document.getElementById('warnsize' + btn_str).style.display = 'block';
            document.getElementById('error' + btn_str).style.display = 'none';
            document.getElementById('fileinfo' + btn_str).style.display = 'none';
            document.getElementById('filename' + btn_str).innerHTML = '';
            document.getElementById('filesize' + btn_str).innerHTML = '';
            document.getElementById('filetype' + btn_str).innerHTML = '';
            $("#file_name" + btn_str).val('');
            $("#file_size" + btn_str).val('');
            $("#file_type" + btn_str).val('');
            document.getElementById('uploadify' + btn_str).outerHTML = document.getElementById('uploadify' + btn_str).outerHTML;
            return;
        }
    }
    

    var oReader = new FileReader();
    oReader.onload = function (e) {
        sResultFileSize = bytesToSize(oFile.size);
        document.getElementById('fileinfo' + btn_str).style.display = 'block';
        document.getElementById('filename' + btn_str).innerHTML = 'Name: ' + oFile.name;
        document.getElementById('filesize' + btn_str).innerHTML = 'Size: ' + sResultFileSize;
        document.getElementById('filetype' + btn_str).innerHTML = 'Type: ' + oFile.type;
        $("#file_name" + btn_str).val(oFile.name);
        $("#file_size" + btn_str).val(sResultFileSize);
        $("#file_type" + btn_str).val(oFile.type);
        document.getElementById('error' + btn_str).style.display = 'none';
        document.getElementById('warnsize' + btn_str).style.display = 'none';
    };
    oReader.readAsDataURL(oFile);
}