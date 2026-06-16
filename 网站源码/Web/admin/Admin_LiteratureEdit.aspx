<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_LiteratureEdit.aspx.cs" Inherits="Web.admin.Admin_LiteratureEdit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <style type="text/css">
        .lit-author-affiliation-editor { display:grid; gap:10px; }
        .lit-author-affiliation-row { display:grid; grid-template-columns:minmax(160px,220px) minmax(180px,260px) minmax(260px,1fr); gap:10px; align-items:start; }
        .lit-author-affiliation-row input,
        .lit-author-affiliation-row textarea { width:100%; border:1px solid #d7e0ea; border-radius:8px; padding:8px 10px; box-sizing:border-box; }
        .lit-author-affiliation-row textarea { min-height:42px; resize:vertical; }
        .lit-author-affiliation-tools { display:flex; gap:8px; align-items:center; margin-bottom:10px; }
        .lit-author-affiliation-hint { color:#6b7280; font-size:13px; line-height:1.7; margin-top:8px; }
        .lit-master-row { display:flex; gap:8px; align-items:center; }
        .lit-master-row .txt { flex:1; min-width:0; }
        .lit-master-row a { white-space:nowrap; }
        @media (max-width:760px) {
            .lit-author-affiliation-row { grid-template-columns:1fr; }
            .lit-master-row { align-items:stretch; flex-direction:column; }
        }
    </style>
</head>
<body>
    <%@ Register TagPrefix="LiteratureManager" TagName="Inc" Src="Inc.ascx" %>
    <%@ Register TagPrefix="LiteratureManager" TagName="class_menu" Src="class_menu.ascx" %>
    <% if (isLoading) { %>
    <LiteratureManager:Inc ID="Inc2" runat="server" />
    <LiteratureManager:class_menu ID="class_menu" runat="server" />

    <form id="form2" runat="server">
        <asp:HiddenField ID="author_details_payload" runat="server" />
        <asp:HiddenField ID="journal_id_payload" runat="server" />
        <asp:HiddenField ID="conference_id_payload" runat="server" />
        <datalist id="institutionMasterList"><%=InstitutionDatalistHtml %></datalist>
        <datalist id="journalMasterList"><%=JournalDatalistHtml %></datalist>
        <datalist id="conferenceMasterList"><%=ConferenceDatalistHtml %></datalist>
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
                                    <label class="form-label">论文作者机构归属</label>
                                    <div class="lit-author-affiliation-tools">
                                        <button type="button" class="btn btn-secondary" onclick="renderAdminAuthorDetailsFromNames()">根据作者姓名生成/刷新作者行</button>
                                    </div>
                                    <div id="authorAffiliationEditor" class="lit-author-affiliation-editor"><%=AuthorAffiliationEditorHtml %></div>
                                    <div class="lit-author-affiliation-hint">这里维护的是“当前这篇论文中每位作者对应的机构”。中间输入框只是从机构库快捷追加，右侧机构框可以直接输入新机构；保存后会同步到作者管理页的论文机构记录，不会覆盖该作者在其他论文中的机构。</div>
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
                                    <div class="row">
                                        <div class="col-md-4">
                                            <label class="form-label">发表年份</label>
                                            <asp:TextBox ID="publish_year" runat="server" CssClass="txt form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-md-4">
                                            <label class="form-label">发表月份</label>
                                            <asp:TextBox ID="publish_month" runat="server" CssClass="txt form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-md-4">
                                            <label class="form-label">发表日期</label>
                                            <asp:TextBox ID="publish_day" runat="server" CssClass="txt form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="lit-author-affiliation-hint">如果原文只有年份，可只填年份；如果能确认月份或日期，请一并填写，用于作者当前机构和论文排序。</div>
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
                                            <div class="lit-master-row">
                                                <asp:TextBox ID="journal_name" runat="server" CssClass="txt form-control"></asp:TextBox>
                                                <a class="btn btn-secondary" href="Admin_JournalList.aspx?MenuId=1732" target="_blank">&#26399;&#21002;&#24211;</a>
                                            </div>
                                            <div class="lit-author-affiliation-hint">&#21487;&#30452;&#25509;&#36755;&#20837;&#65292;&#20063;&#21487;&#20174;&#26399;&#21002;&#24211;&#20505;&#36873;&#20013;&#36873;&#25321;&#12290;</div>
                                        </div>
                                        <div class="col-md-6">
                                            <label class="form-label">&#20250;&#35758;&#21517;&#31216;</label>
                                            <div class="lit-master-row">
                                                <asp:TextBox ID="conference_name" runat="server" CssClass="txt form-control"></asp:TextBox>
                                                <a class="btn btn-secondary" href="Admin_ConferenceList.aspx?MenuId=1733" target="_blank">&#20250;&#35758;&#24211;</a>
                                            </div>
                                            <div class="lit-author-affiliation-hint">&#21487;&#30452;&#25509;&#36755;&#20837;&#65292;&#20063;&#21487;&#20174;&#20250;&#35758;&#24211;&#20505;&#36873;&#20013;&#36873;&#25321;&#12290;</div>
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
                                    <asp:Button ID="Button3" Text=" &#20445; &#23384; " CssClass="btn btn-primary" runat="server" OnClientClick="return collectAdminAuthorDetails();" OnClick="OnClick_AddUp" />
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
        var adminInstitutionOptions = <%=InstitutionOptionsJson %>;
        var adminJournalOptions = <%=JournalOptionsJson %>;
        var adminConferenceOptions = <%=ConferenceOptionsJson %>;

        function normalizeMasterOptionName(value) {
            return String(value || "").replace(/\s+/g, " ").trim().toLowerCase();
        }

        function findMasterOption(options, value) {
            var key = normalizeMasterOptionName(value);
            if (!key || !options) return null;
            for (var i = 0; i < options.length; i++) {
                if (normalizeMasterOptionName(options[i].name) === key) {
                    return options[i];
                }
            }
            return null;
        }

        function syncJournalMasterSelection() {
            var input = document.getElementById("<%= journal_name.ClientID %>");
            var hidden = document.getElementById("<%= journal_id_payload.ClientID %>");
            if (!input || !hidden) return;
            var item = findMasterOption(adminJournalOptions, input.value);
            hidden.value = item ? item.id : "";
        }

        function syncConferenceMasterSelection() {
            var input = document.getElementById("<%= conference_name.ClientID %>");
            var hidden = document.getElementById("<%= conference_id_payload.ClientID %>");
            if (!input || !hidden) return;
            var item = findMasterOption(adminConferenceOptions, input.value);
            hidden.value = item ? item.id : "";
        }

        function appendAuthorInstitutionFromPicker(picker) {
            if (!picker) return;
            var row = picker.closest ? picker.closest(".lit-author-affiliation-row") : null;
            if (!row) return;
            var value = picker.value.replace(/\s+/g, " ").trim();
            if (!value) return;
            var option = findMasterOption(adminInstitutionOptions, value);
            if (option && option.name) {
                value = option.name;
            }
            var textarea = row.querySelector("[data-author-affiliation]");
            if (!textarea) return;
            var existing = splitAdminAffiliations(textarea.value);
            var exists = false;
            for (var i = 0; i < existing.length; i++) {
                if (normalizeMasterOptionName(existing[i]) === normalizeMasterOptionName(value)) {
                    exists = true;
                    break;
                }
            }
            if (!exists) {
                existing.push(value);
                textarea.value = existing.join("; ");
            }
        }

        function bindAuthorInstitutionPickers() {
            var editor = document.getElementById("authorAffiliationEditor");
            if (!editor) return;
            var pickers = editor.querySelectorAll("[data-author-affiliation-picker]");
            for (var i = 0; i < pickers.length; i++) {
                if (pickers[i].getAttribute("data-bound") === "1") continue;
                pickers[i].setAttribute("data-bound", "1");
                pickers[i].onchange = function () {
                    appendAuthorInstitutionFromPicker(this);
                };
            }
        }

        function escapeAdminAuthorHtml(value) {
            return String(value || "").replace(/[&<>"']/g, function (ch) {
                return ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", "\"": "&quot;", "'": "&#39;" })[ch];
            });
        }

        function containsAdminChinese(value) {
            return /[\u3400-\u9fff\uf900-\ufaff]/.test(value || "");
        }

        function splitAdminAffiliations(value) {
            var parts = String(value || "").split(/[;；|\n\r]+/);
            var result = [];
            for (var i = 0; i < parts.length; i++) {
                var current = parts[i].replace(/\s+/g, " ").trim();
                if (current && result.indexOf(current) < 0) {
                    result.push(current);
                }
            }
            return result;
        }

        function splitAdminAuthorNames(value) {
            var parts = String(value || "").split(/[,，;；、|]+/);
            var result = [];
            for (var i = 0; i < parts.length; i++) {
                var current = parts[i].replace(/\s+/g, " ").trim();
                if (current && result.indexOf(current) < 0) {
                    result.push(current);
                }
            }
            return result;
        }

        function normalizeAdminAuthorDetails(details) {
            var result = [];
            if (!details || !details.length) return result;
            for (var i = 0; i < details.length; i++) {
                var item = details[i];
                var name = "";
                var affiliations = [];
                if (typeof item === "string") {
                    name = item;
                } else if (item) {
                    name = item.name || item.name_cn || item.name_en || "";
                    if (item.affiliations && item.affiliations.length) {
                        for (var j = 0; j < item.affiliations.length; j++) {
                            var aff = String(item.affiliations[j] || "").replace(/\s+/g, " ").trim();
                            if (aff && affiliations.indexOf(aff) < 0) affiliations.push(aff);
                        }
                    }
                    if (affiliations.length === 0 && item.affiliation_text) {
                        affiliations = splitAdminAffiliations(item.affiliation_text);
                    }
                }
                name = String(name || "").replace(/\s+/g, " ").trim();
                if (!name) continue;
                result.push({
                    author_id: item.author_id || 0,
                    name: name,
                    name_cn: containsAdminChinese(name) ? name : "",
                    name_en: containsAdminChinese(name) ? "" : name,
                    affiliations: affiliations,
                    affiliation_text: affiliations.join("; "),
                    mapping_status: affiliations.length ? "matched" : "unmatched"
                });
            }
            return result;
        }

        function renderAdminAuthorDetails(details) {
            var editor = document.getElementById("authorAffiliationEditor");
            if (!editor) return;
            var normalized = normalizeAdminAuthorDetails(details);
            editor.innerHTML = "";
            if (!normalized.length) {
                editor.innerHTML = "<div class=\"lit-author-affiliation-hint\">暂无作者机构归属，请先填写作者或解析 PDF。</div>";
                setAdminAuthorDetailsPayload([]);
                return;
            }
            for (var i = 0; i < normalized.length; i++) {
                var row = document.createElement("div");
                row.className = "lit-author-affiliation-row";
                row.setAttribute("data-author-id", normalized[i].author_id || 0);
                row.innerHTML =
                    "<input type=\"text\" data-author-name=\"1\" value=\"" + escapeAdminAuthorHtml(normalized[i].name) + "\" placeholder=\"作者姓名\" />" +
                    "<input type=\"text\" data-author-affiliation-picker=\"1\" list=\"institutionMasterList\" placeholder=\"从机构库选择（可选）\" />" +
                    "<textarea data-author-affiliation=\"1\" placeholder=\"可直接输入该作者在本文中的机构；多个机构用分号分隔\">" + escapeAdminAuthorHtml(normalized[i].affiliation_text || "") + "</textarea>";
                editor.appendChild(row);
            }
            bindAuthorInstitutionPickers();
            setAdminAuthorDetailsPayload(normalized);
        }

        function setAdminAuthorDetailsPayload(details) {
            var input = document.getElementById("<%= author_details_payload.ClientID %>");
            if (!input) return;
            try {
                input.value = details && details.length ? JSON.stringify(details) : "";
            } catch (e) {
                input.value = "";
            }
        }

        function readAdminAuthorDetailsFromEditor() {
            var editor = document.getElementById("authorAffiliationEditor");
            var details = [];
            if (!editor) return details;
            var rows = editor.querySelectorAll(".lit-author-affiliation-row");
            for (var i = 0; i < rows.length; i++) {
                var nameEl = rows[i].querySelector("[data-author-name]");
                var affEl = rows[i].querySelector("[data-author-affiliation]");
                var name = nameEl ? nameEl.value.replace(/\s+/g, " ").trim() : "";
                if (!name) continue;
                var affiliations = splitAdminAffiliations(affEl ? affEl.value : "");
                details.push({
                    author_id: parseInt(rows[i].getAttribute("data-author-id") || "0", 10) || 0,
                    name: name,
                    name_cn: containsAdminChinese(name) ? name : "",
                    name_en: containsAdminChinese(name) ? "" : name,
                    affiliations: affiliations,
                    affiliation_text: affiliations.join("; "),
                    mapping_status: affiliations.length ? "matched" : "unmatched"
                });
            }
            return details;
        }

        function collectAdminAuthorDetails() {
            var editor = document.getElementById("authorAffiliationEditor");
            if (!editor) return true;
            var details = readAdminAuthorDetailsFromEditor();
            if (!details.length) {
                var authorInput = document.getElementById("<%= author_names.ClientID %>");
                var names = splitAdminAuthorNames(authorInput ? authorInput.value : "");
                for (var j = 0; j < names.length; j++) {
                    details.push({
                        name: names[j],
                        name_cn: containsAdminChinese(names[j]) ? names[j] : "",
                        name_en: containsAdminChinese(names[j]) ? "" : names[j],
                        affiliations: [],
                        affiliation_text: "",
                        mapping_status: "unmatched"
                    });
                }
            }
            var authorNamesInput = document.getElementById("<%= author_names.ClientID %>");
            if (authorNamesInput && details.length) {
                var namesForSubmit = [];
                for (var n = 0; n < details.length; n++) {
                    if (details[n].name) namesForSubmit.push(details[n].name);
                }
                authorNamesInput.value = namesForSubmit.join(", ");
            }
            setAdminAuthorDetailsPayload(details);
            return true;
        }

        function renderAdminAuthorDetailsFromNames() {
            var authorInput = document.getElementById("<%= author_names.ClientID %>");
            var institutionInput = document.getElementById("<%= institution.ClientID %>");
            var names = splitAdminAuthorNames(authorInput ? authorInput.value : "");
            var defaultInstitution = institutionInput ? institutionInput.value.replace(/\s+/g, " ").trim() : "";
            var currentDetails = normalizeAdminAuthorDetails(readAdminAuthorDetailsFromEditor());
            var currentMap = {};
            for (var j = 0; j < currentDetails.length; j++) {
                currentMap[currentDetails[j].name.toLowerCase()] = currentDetails[j];
            }
            var details = [];
            for (var i = 0; i < names.length; i++) {
                var current = currentMap[names[i].toLowerCase()];
                var affiliations = current ? current.affiliations : [];
                if (!current && names.length === 1 && defaultInstitution) {
                    affiliations = splitAdminAffiliations(defaultInstitution);
                }
                details.push({
                    author_id: current ? (current.author_id || 0) : 0,
                    name: names[i],
                    affiliations: affiliations,
                    affiliation_text: affiliations.join("; ")
                });
            }
            renderAdminAuthorDetails(details);
            var editor = document.getElementById("authorAffiliationEditor");
            if (editor && names.length > 1) {
                var hint = document.createElement("div");
                hint.className = "lit-author-affiliation-hint";
                hint.innerHTML = "已根据作者姓名生成作者行。多作者论文不会自动套用整篇机构字段，请逐一确认机构归属。";
                editor.insertBefore(hint, editor.firstChild);
            }
            return false;
        }

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
                    setValue("<%= publish_month.ClientID %>", data.publish_month);
                    setValue("<%= publish_day.ClientID %>", data.publish_day);
                    setValue("<%= journal_name.ClientID %>", data.journal_name);
                    setValue("<%= conference_name.ClientID %>", data.conference_name);
                    syncJournalMasterSelection();
                    syncConferenceMasterSelection();
                    setValue("<%= keywords.ClientID %>", data.keywords);
                    setValue("<%= abstract_text.ClientID %>", data.abstract_text);
                    setValue("<%= source_type.ClientID %>", data.source_type);
                    setAdminAuthorDetailsPayload(data.author_details || data.authors || []);
                    renderAdminAuthorDetails(data.author_details || data.authors || []);
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

        (function () {
            var journalInput = document.getElementById("<%= journal_name.ClientID %>");
            if (journalInput) {
                journalInput.oninput = syncJournalMasterSelection;
                journalInput.onchange = syncJournalMasterSelection;
            }
            var conferenceInput = document.getElementById("<%= conference_name.ClientID %>");
            if (conferenceInput) {
                conferenceInput.oninput = syncConferenceMasterSelection;
                conferenceInput.onchange = syncConferenceMasterSelection;
            }
            bindAuthorInstitutionPickers();
            syncJournalMasterSelection();
            syncConferenceMasterSelection();
        })();
    </script>
    <% } %>
</body>
</html>
