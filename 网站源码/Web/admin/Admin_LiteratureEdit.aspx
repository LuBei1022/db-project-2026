<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_LiteratureEdit.aspx.cs" Inherits="Web.admin.Admin_LiteratureEdit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
</head>
<body>
    <%@ Register TagPrefix="LiteratureManager" TagName="Inc" Src="Inc.ascx" %>
    <%@ Register TagPrefix="LiteratureManager" TagName="class_menu" Src="class_menu.ascx" %>
    <% if (isLoading) { %>
    <LiteratureManager:Inc ID="Inc2" runat="server" />
    <LiteratureManager:class_menu ID="class_menu" runat="server" />

    <form id="form2" runat="server">
        <div class="app-content">
            <asp:Panel ID="AddUp" runat="server">
                <div class="container-fluid">
                    <div class="row">
                        <div class="col-md-8 offset-md-2">
                            <div class="card card-primary card-outline mb-4">
                                <div class="card-header">
                                    <div class="card-title">
                                        <asp:Label ID="Txt_Title" runat="server"></asp:Label>
                                    </div>
                                </div>
                                <%=duplicateMasterNoticeHtml %>
                                <div class="card-body">
                                    <div class="mb-6">
                                        <label class="form-label">&#25991;&#29486;&#26631;&#39064;<span>*</span></label>
                                        <asp:TextBox ID="title" runat="server" CssClass="txt form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="mb-6">
                                        <label class="form-label">&#21103;&#26631;&#39064;</label>
                                        <asp:TextBox ID="subtitle" runat="server" CssClass="txt form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="mb-6">
                                        <label class="form-label">&#20316;&#32773;</label>
                                        <asp:TextBox ID="author_names" runat="server" CssClass="txt form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="mb-6">
                                        <label class="form-label">&#20316;&#32773;&#21333;&#20301;</label>
                                        <asp:TextBox ID="institution" runat="server" CssClass="txt form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <label class="form-label">DOI</label>
                                            <asp:TextBox ID="doi" runat="server" CssClass="txt form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-md-6">
                                            <label class="form-label">&#19979;&#36733;&#31215;&#20998;</label>
                                            <asp:TextBox ID="download_points" runat="server" CssClass="txt form-control" Text="0"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="mb-6">
                                        <label class="form-label">&#21457;&#34920;&#24180;&#20221;</label>
                                        <asp:TextBox ID="publish_year" runat="server" CssClass="txt form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <label class="form-label">&#25991;&#29486;&#31867;&#22411;</label>
                                            <asp:DropDownList ID="source_type" runat="server" CssClass="form-control">
                                                <asp:ListItem Value="&#26399;&#21002;&#35770;&#25991;">&#26399;&#21002;&#35770;&#25991;</asp:ListItem>
                                                <asp:ListItem Value="&#20250;&#35758;&#35770;&#25991;">&#20250;&#35758;&#35770;&#25991;</asp:ListItem>
                                                <asp:ListItem Value="&#23398;&#20301;&#35770;&#25991;">&#23398;&#20301;&#35770;&#25991;</asp:ListItem>
                                                <asp:ListItem Value="&#19987;&#21033;">&#19987;&#21033;</asp:ListItem>
                                                <asp:ListItem Value="&#22270;&#20070;">&#22270;&#20070;</asp:ListItem>
                                                <asp:ListItem Value="&#25253;&#21578;">&#25253;&#21578;</asp:ListItem>
                                                <asp:ListItem Value="&#20854;&#20182;">&#20854;&#20182;</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-6">
                                            <label class="form-label">&#20998;&#31867;</label>
                                            <asp:DropDownList ID="category_id" runat="server" CssClass="form-control"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <label class="form-label">&#26399;&#21002;&#21517;&#31216;</label>
                                            <asp:TextBox ID="journal_name" runat="server" CssClass="txt form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-md-6">
                                            <label class="form-label">&#20250;&#35758;&#21517;&#31216;</label>
                                            <asp:TextBox ID="conference_name" runat="server" CssClass="txt form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-md-4">
                                            <label class="form-label">&#21367;</label>
                                            <asp:TextBox ID="volume" runat="server" CssClass="txt form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-md-4">
                                            <label class="form-label">&#26399;</label>
                                            <asp:TextBox ID="issue" runat="server" CssClass="txt form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-md-4">
                                            <label class="form-label">&#39029;&#30721;</label>
                                            <asp:TextBox ID="pages" runat="server" CssClass="txt form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <label class="form-label">&#20986;&#29256;&#31038;</label>
                                            <asp:TextBox ID="publisher" runat="server" CssClass="txt form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-md-6">
                                            <label class="form-label">&#35821;&#35328;</label>
                                            <asp:TextBox ID="language" runat="server" CssClass="txt form-control" Text="&#20013;&#25991;"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <label class="form-label">&#20851;&#38190;&#35789;</label>
                                    <asp:TextBox ID="keywords" runat="server" CssClass="txt form-control"></asp:TextBox>
                                </div>
                                <div class="card-body">
                                    <label class="form-label">&#26631;&#31614;</label>
                                    <asp:CheckBoxList ID="TagList" runat="server" RepeatColumns="4" RepeatDirection="Horizontal" CssClass="form-control" style="height:auto;"></asp:CheckBoxList>
                                    <div style="height: 10px;"></div>
                                    <label class="form-label">&#25163;&#21160;&#26631;&#31614;&#65288;&#29992;&#36887;&#21495;&#20998;&#38548;&#65289;</label>
                                    <asp:TextBox ID="tag_names" runat="server" CssClass="txt form-control"></asp:TextBox>
                                </div>
                                <div class="card-body">
                                    <label class="form-label">&#25688;&#35201;</label>
                                    <asp:TextBox ID="abstract_text" runat="server" CssClass="txt form-control" TextMode="MultiLine" Rows="8"></asp:TextBox>
                                </div>
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <label class="form-label">&#22806;&#37096;&#38142;&#25509;</label>
                                            <asp:TextBox ID="external_url" runat="server" CssClass="txt form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-md-6">
                                            <label class="form-label">&#26469;&#28304;&#24211;</label>
                                            <asp:TextBox ID="source_db" runat="server" CssClass="txt form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <label class="form-label">&#22791;&#27880;</label>
                                    <asp:TextBox ID="remark" runat="server" CssClass="txt form-control" TextMode="MultiLine" Rows="4"></asp:TextBox>
                                </div>
                                <div class="card-body">
                                    <label class="form-label">&#23553;&#38754;&#22270;</label>
                                    <asp:FileUpload ID="cover_pic" runat="server" accept="image/*" />
                                    <asp:HiddenField ID="cover_pic_old" runat="server" />
                                    <div class="div-group-img" style="margin-top: 10px;">
                                        <asp:Image ID="cover_pic_img" runat="server" ImageUrl="/admin/images/nophoto.gif" />
                                    </div>
                                    <div class="form-check" style="margin-top: 10px;">
                                        <asp:CheckBox ID="del_cover_pic" runat="server" Text="&#21024;&#38500;&#29616;&#26377;&#23553;&#38754;" />
                                    </div>
                                </div>
                                <div class="card-body">
                                    <label class="form-label">PDF &#38468;&#20214;</label>
                                    <asp:FileUpload ID="pdf_file" runat="server" accept=".pdf,.zip,.doc,.docx" />
                                    <input type="button" id="btnParsePdf" value="&#33258;&#21160;&#35299;&#26512;PDF" class="btn btn-secondary" style="margin-left: 10px;" />
                                    <span id="pdf_parse_status" style="margin-left: 10px; color: #666666;"></span>
                                    <asp:HiddenField ID="pdf_file_old" runat="server" />
                                    <asp:HiddenField ID="pdf_name_old" runat="server" />
                                    <div style="margin-top: 10px;">
                                        <asp:Label ID="pdf_file_name" runat="server"></asp:Label>
                                    </div>
                                    <div class="form-check" style="margin-top: 10px;">
                                        <asp:CheckBox ID="del_pdf_file" runat="server" Text="&#21024;&#38500;&#29616;&#26377;&#38468;&#20214;" />
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <div class="form-check">
                                                <asp:CheckBox ID="is_top" runat="server" Text="&#32622;&#39030;&#26174;&#31034;" />
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <label class="form-label">&#29366;&#24577;</label>
                                            <asp:DropDownList ID="status" runat="server" CssClass="form-control">
                                                <asp:ListItem Value="0">&#24453;&#23457;&#26680;</asp:ListItem>
                                                <asp:ListItem Value="1">&#23457;&#26680;&#36890;&#36807;</asp:ListItem>
                                                <asp:ListItem Value="2">&#23457;&#26680;&#39539;&#22238;</asp:ListItem>
                                                <asp:ListItem Value="3">&#37325;&#22797;&#25237;&#31295;&#24050;&#21512;&#24182;</asp:ListItem>
                                                <asp:ListItem Value="4">&#20803;&#25968;&#25454;&#20462;&#25913;&#24050;&#24212;&#29992;</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-footer">
                                    <asp:Button ID="Button3" Text=" &#20445; &#23384; " CssClass="btn btn-primary" runat="server" OnClick="OnClick_AddUp" />
                                    <input type="button" name="button" id="button" value=" &#36820; &#22238; " class="btn submit-but" onclick="history.go(-1)" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </form>
    <script type="text/javascript">
        (function () {
            function setStatus(text, color) {
                var el = document.getElementById("pdf_parse_status");
                if (!el) return;
                el.innerHTML = text || "";
                if (color) {
                    el.style.color = color;
                }
            }

            function setValue(id, value) {
                var el = document.getElementById(id);
                if (el && value) {
                    el.value = value;
                }
            }

            var btn = document.getElementById("btnParsePdf");
            if (!btn) return;

            btn.onclick = function () {
                var fileInput = document.getElementById("<%= pdf_file.ClientID %>");
                if (!fileInput || !fileInput.files || fileInput.files.length === 0) {
                    setStatus("&#35831;&#20808;&#36873;&#25321; PDF &#25991;&#20214;", "#d9534f");
                    return false;
                }

                var file = fileInput.files[0];
                var name = (file.name || "").toLowerCase();
                if (name.lastIndexOf(".pdf") !== name.length - 4) {
                    setStatus("&#30446;&#21069;&#20165;&#25903;&#25345;&#35299;&#26512; PDF &#25991;&#20214;", "#d9534f");
                    return false;
                }

                var formData = new FormData();
                formData.append("file", file);
                setStatus("&#27491;&#22312;&#35299;&#26512;&#20013;...", "#666666");

                var xhr = new XMLHttpRequest();
                xhr.open("POST", "PdfParse.ashx", true);
                xhr.onreadystatechange = function () {
                    if (xhr.readyState !== 4) return;

                    var data = null;
                    try {
                        data = JSON.parse(xhr.responseText || "{}");
                    } catch (e) {
                        data = null;
                    }

                    if (xhr.status !== 200 || !data || data.success !== true) {
                        setStatus((data && data.message) ? data.message : "PDF &#35299;&#26512;&#22833;&#36133;", "#d9534f");
                        return;
                    }

                    setValue("<%= title.ClientID %>", data.title);
                    setValue("<%= author_names.ClientID %>", data.author_names);
                    setValue("<%= institution.ClientID %>", data.institution);
                    setValue("<%= doi.ClientID %>", data.doi);
                    setValue("<%= publish_year.ClientID %>", data.publish_year);
                    setValue("<%= journal_name.ClientID %>", data.journal_name);
                    setValue("<%= conference_name.ClientID %>", data.conference_name);
                    setValue("<%= keywords.ClientID %>", data.keywords);
                    setValue("<%= abstract_text.ClientID %>", data.abstract_text);
                    setValue("<%= source_type.ClientID %>", data.source_type);
                    if (data.source_type) {
                        var sourceEl = document.getElementById("<%= source_type.ClientID %>");
                        if (sourceEl) {
                            sourceEl.value = data.source_type;
                        }
                    }
                    if (data.remark_append) {
                        var remarkEl = document.getElementById("<%= remark.ClientID %>");
                        if (remarkEl) {
                            if (remarkEl.value) {
                                remarkEl.value += "\r\n";
                            }
                            remarkEl.value += data.remark_append;
                        }
                    }
                    setStatus("PDF &#20449;&#24687;&#24050;&#33258;&#21160;&#22635;&#20889;", "#28a745");
                };
                xhr.send(formData);
                return false;
            };
        })();
    </script>
    <% } %>
</body>
</html>
