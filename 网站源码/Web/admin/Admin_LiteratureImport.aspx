<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_LiteratureImport.aspx.cs" Inherits="Web.admin.Admin_LiteratureImport" %>
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
        <asp:HiddenField ID="pdf_parse_payload" runat="server" />
        <div class="app-content">
            <asp:Panel ID="Main" runat="server">
                <div class="container-fluid">
                    <div class="card mb-4">
                        <div class="card-header">
                            <div class="card-title">&#25209;&#37327;&#23548;&#20837;&#25991;&#29486;</div>
                        </div>
                        <div class="card-body">
                            <div class="mb-3">
                                <label class="form-label">&#23548;&#20837;&#27169;&#24335;</label>
                                <label style="margin-left: 12px;"><input type="radio" name="import_mode" value="csv" checked="checked" onclick="setImportMode('csv')" /> CSV &#20803;&#25968;&#25454;</label>
                                <label style="margin-left: 18px;"><input type="radio" name="import_mode" value="pdf" onclick="setImportMode('pdf')" /> PDF &#33258;&#21160;&#35299;&#26512;</label>
                            </div>
                            <div class="mb-3">
                                <label class="form-label" id="importFileLabel">CSV / PDF &#25991;&#20214;</label>
                                <asp:FileUpload ID="import_file" runat="server" accept=".csv,.pdf,application/pdf" AllowMultiple="true" />
                                <button type="button" id="btnParseImportPdf" class="btn btn-secondary" style="margin-left: 10px; display: none;">&#35299;&#26512;&#39044;&#35272;</button>
                                <asp:Button ID="ButtonImport" runat="server" Text=" &#24320; &#22987; &#23548; &#20837; " CssClass="btn btn-primary" OnClick="OnClick_Import" OnClientClick="return beforeImportSubmit();" style="margin-left: 10px;" />
                                <span id="pdfParseStatus" style="margin-left: 10px; color: #666;"></span>
                            </div>
                            <div id="pdfPreviewPanel" style="display: none; margin-bottom: 16px;">
                                <div style="font-weight: 700; margin-bottom: 8px;">PDF &#35299;&#26512;&#32467;&#26524;&#65288;&#21487;&#22312;&#23548;&#20837;&#21069;&#20462;&#25913;&#65289;</div>
                                <div id="pdfPreviewList"></div>
                            </div>
                            <div class="alert alert-info" style="margin-bottom: 0;">
                                <div><strong>&#23548;&#20837;&#35828;&#26126;</strong></div>
                                <div style="margin-top: 8px;">CSV &#27169;&#24335;&#65306;&#31532;&#19968;&#34892;&#20026;&#34920;&#22836;&#65292;&#25353;&#34920;&#22836;&#25209;&#37327;&#23548;&#20837;&#20803;&#25968;&#25454;&#12290;</div>
                                <div style="margin-top: 8px;">PDF &#27169;&#24335;&#65306;&#21487;&#19968;&#27425;&#36873;&#25321;&#22810;&#20010; PDF&#65292;&#35831;&#20808;&#28857;&#20987;&#35299;&#26512;&#39044;&#35272;&#65292;&#30830;&#35748;&#25110;&#20462;&#25913;&#20803;&#25968;&#25454;&#12289;&#20316;&#32773;&#26426;&#26500;&#20851;&#31995;&#21518;&#20877;&#23548;&#20837;&#12290;</div>
                                <div style="margin-top: 8px;">CSV &#25512;&#33616;&#23383;&#27573;&#65306;<code>title,subtitle,author_names,institution,doi,keywords,abstract_text,source_type,language,publish_year,publish_month,publish_day,journal_name,conference_name,publisher,volume,issue,pages,category_id,category_name,tag_names,external_url,source_db,remark,status,is_top</code></div>
                            </div>
                        </div>
                    </div>

                    <div class="card mb-12">
                        <div class="card-header">
                            <div class="card-title">&#26368;&#36817;&#23548;&#20837;&#25209;&#27425;</div>
                        </div>
                        <div class="card-body p-0">
                            <table class="table table-sm">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>&#25209;&#27425;&#21517;&#31216;</th>
                                        <th>&#25991;&#20214;&#21517;</th>
                                        <th>&#24635;&#25968;</th>
                                        <th>&#25104;&#21151;</th>
                                        <th>&#22833;&#36133;</th>
                                        <th>&#29366;&#24577;</th>
                                        <th>&#23548;&#20837;&#26102;&#38388;</th>
                                        <th class="textAlignC">&#25805;&#20316;</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <tr class="hover">
                                                <td><%# Eval("id") %></td>
                                                <td><%# Function.HtmlDiscodeWeb(Eval("batch_name").ToString()) %></td>
                                                <td><%# Function.HtmlDiscodeWeb(Eval("file_name").ToString()) %></td>
                                                <td><%# Eval("total_count") %></td>
                                                <td><%# Eval("success_count") %></td>
                                                <td><%# Eval("fail_count") %></td>
                                                <td><%# GetBatchStatus(Eval("status")) %></td>
                                                <td><%# Function.ConvertTo<DateTime>(Eval("addtime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss") %></td>
                                                <td class="textAlignC">
                                                    <a class="badge text-bg-info" href='Admin_LiteratureImportError.aspx?MenuId=<%=MenuId %>&BatchId=<%# Eval("id") %>'>&#38169;&#35823;&#26126;&#32454;</a>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <asp:Panel ID="DivNull" runat="server" Visible="true">
                                        <tr>
                                            <td colspan="9" style="text-align: center;">&#26080;&#23548;&#20837;&#35760;&#24405;!</td>
                                        </tr>
                                    </asp:Panel>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </form>
    <script src="/js/literature-text-normalizer.js" type="text/javascript"></script>
    <script type="text/javascript">
        function normalizeAbstractText(value) {
            return window.LiteratureTextNormalizer ? window.LiteratureTextNormalizer.normalizeAbstract(value) : (value || "");
        }

        function setImportMode(mode) {
            var fileInput = document.getElementById("<%= import_file.ClientID %>");
            var label = document.getElementById("importFileLabel");
            var parseBtn = document.getElementById("btnParseImportPdf");
            var preview = document.getElementById("pdfPreviewPanel");
            var payload = document.getElementById("<%= pdf_parse_payload.ClientID %>");
            var status = document.getElementById("pdfParseStatus");
            if (!fileInput) {
                return;
            }
            if (mode === "pdf") {
                fileInput.setAttribute("accept", ".pdf,application/pdf");
                if (parseBtn) {
                    parseBtn.style.display = "";
                }
                if (preview) {
                    preview.style.display = "";
                }
                if (label) {
                    label.innerHTML = "PDF &#25991;&#20214;";
                }
            } else {
                fileInput.setAttribute("accept", ".csv");
                if (parseBtn) {
                    parseBtn.style.display = "none";
                }
                if (preview) {
                    preview.style.display = "none";
                }
                if (payload) {
                    payload.value = "";
                }
                if (status) {
                    status.innerHTML = "";
                }
                if (label) {
                    label.innerHTML = "CSV &#25991;&#20214;";
                }
            }
        }

        function beforeImportSubmit() {
            var mode = getImportMode();
            if (mode !== "pdf") {
                return true;
            }

            var payload = document.getElementById("<%= pdf_parse_payload.ClientID %>");
            var cards = document.querySelectorAll(".admin-pdf-preview-card");
            if (!cards || cards.length === 0) {
                alert("&#35831;&#20808;&#28857;&#20987;&#35299;&#26512;&#39044;&#35272;&#65292;&#30830;&#35748;&#21518;&#20877;&#23548;&#20837; PDF");
                return false;
            }
            for (var i = 0; i < cards.length; i++) {
                if (cards[i].getAttribute("data-parse-ok") !== "true") {
                    alert("&#26377; PDF &#26410;&#25104;&#21151;&#35299;&#26512;&#65292;&#35831;&#37325;&#26032;&#35299;&#26512;&#25110;&#31227;&#38500;&#21518;&#20877;&#23548;&#20837;");
                    return false;
                }
            }
            collectPdfPreviewPayload();
            if (!payload || !payload.value) {
                alert("PDF &#35299;&#26512;&#39044;&#35272;&#32467;&#26524;&#20026;&#31354;&#65292;&#35831;&#20808;&#35299;&#26512;&#24182;&#30830;&#35748;");
                return false;
            }
            return true;
        }

        function getImportMode() {
            var radios = document.getElementsByName("import_mode");
            for (var i = 0; i < radios.length; i++) {
                if (radios[i].checked) {
                    return radios[i].value;
                }
            }
            return "csv";
        }

        function setPdfParseStatus(text, color) {
            var status = document.getElementById("pdfParseStatus");
            if (!status) return;
            status.innerHTML = text || "";
            if (color) {
                status.style.color = color;
            }
        }

        function escapeHtml(value) {
            return String(value || "").replace(/[&<>"']/g, function (ch) {
                return ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", "\"": "&quot;", "'": "&#39;" })[ch];
            });
        }

        function containsChineseClient(value) {
            return /[\u3400-\u9fff\uf900-\ufaff]/.test(value || "");
        }

        function splitAffiliationValues(value) {
            var parts = String(value || "").split(/[;；|\n\r]+/);
            var result = [];
            for (var i = 0; i < parts.length; i++) {
                var current = parts[i].replace(/\s+/g, " ").trim();
                if (/^(未匹配|待匹配|无|none|unmatched)$/i.test(current)) {
                    continue;
                }
                if (current && result.indexOf(current) < 0) {
                    result.push(current);
                }
            }
            return result;
        }

        function splitAuthorNameValues(value) {
            var normalized = String(value || "").replace(/\s+(?:and|&)\s+/gi, ", ");
            var parts = normalized.split(/[,，;；、|\n\r]+/);
            var result = [];
            for (var i = 0; i < parts.length; i++) {
                var current = parts[i]
                    .replace(/\s+/g, " ")
                    .replace(/\s*(?:\d+(?:\s*[,，]\s*\d+)*|[*†‡§¶#]+)\s*$/g, "")
                    .trim();
                if (!/[A-Za-z\u4e00-\u9fff]/.test(current)) {
                    continue;
                }
                if (current && result.indexOf(current) < 0) {
                    result.push(current);
                }
            }
            return result;
        }

        function namesToUnmatchedAuthorDetails(value) {
            var names = splitAuthorNameValues(value);
            var result = [];
            for (var i = 0; i < names.length; i++) {
                result.push({
                    name: names[i],
                    name_cn: containsChineseClient(names[i]) ? names[i] : "",
                    name_en: containsChineseClient(names[i]) ? "" : names[i],
                    affiliations: [],
                    affiliation_text: "",
                    markers: [],
                    mapping_status: "unmatched"
                });
            }
            return result;
        }

        function normalizeAuthorDetails(details) {
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
                        affiliations = splitAffiliationValues(item.affiliation_text);
                    }
                }
                name = String(name || "").replace(/\s+/g, " ").trim();
                if (!name) continue;
                result.push({
                    name: name,
                    name_cn: containsChineseClient(name) ? name : "",
                    name_en: containsChineseClient(name) ? "" : name,
                    affiliations: affiliations,
                    affiliation_text: affiliations.join("; "),
                    markers: [],
                    mapping_status: affiliations.length ? "matched" : "unmatched"
                });
            }
            return result;
        }

        function pickAuthorDetailsFromParse(data) {
            if (!data) return [];
            var details = normalizeAuthorDetails(data.author_details);
            if (details.length) return details;

            var authors = normalizeAuthorDetails(data.authors);
            if (authors.length) return authors;

            return namesToUnmatchedAuthorDetails(data.author_names);
        }

        function formatAuthorDetailsText(details) {
            var normalized = normalizeAuthorDetails(details);
            var lines = [];
            for (var i = 0; i < normalized.length; i++) {
                lines.push(normalized[i].name + " => " + (normalized[i].affiliation_text || "未匹配"));
            }
            return lines.join("\n");
        }

        function parseAuthorDetailsText(value) {
            var lines = String(value || "").split(/\r?\n/);
            var result = [];
            for (var i = 0; i < lines.length; i++) {
                var line = lines[i].trim();
                if (!line) continue;
                var match = line.match(/^(.*?)\s*(?:=>|->|：|:)\s*(.*)$/);
                var name = match ? match[1].trim() : line;
                var affiliationText = match ? match[2].trim() : "";
                if (!name) continue;
                var affiliations = splitAffiliationValues(affiliationText);
                result.push({
                    name: name,
                    name_cn: containsChineseClient(name) ? name : "",
                    name_en: containsChineseClient(name) ? "" : name,
                    affiliations: affiliations,
                    affiliation_text: affiliations.join("; "),
                    markers: [],
                    mapping_status: affiliations.length ? "matched" : "unmatched"
                });
            }
            return result;
        }

        function previewFieldHtml(label, name, value, area) {
            if (area) {
                return "<div style=\"margin-bottom:8px;\"><label style=\"display:block;font-weight:600;margin-bottom:4px;\">" + label + "</label><textarea data-pdf-field=\"" + name + "\" style=\"width:100%;min-height:68px;border:1px solid #d4dbe6;border-radius:6px;padding:8px;\">" + escapeHtml(value) + "</textarea></div>";
            }
            return "<div style=\"margin-bottom:8px;\"><label style=\"display:block;font-weight:600;margin-bottom:4px;\">" + label + "</label><input type=\"text\" data-pdf-field=\"" + name + "\" value=\"" + escapeHtml(value) + "\" style=\"width:100%;border:1px solid #d4dbe6;border-radius:6px;padding:8px;\" /></div>";
        }

        function renderPdfPreviewCard(file) {
            var list = document.getElementById("pdfPreviewList");
            if (!list) return null;
            var card = document.createElement("div");
            var fileName = file.name || "";
            card.className = "admin-pdf-preview-card";
            card.setAttribute("data-file-name", fileName);
            card.setAttribute("data-parse-ok", "false");
            card.style.cssText = "margin-bottom:14px;padding:14px;border:1px solid #d7e1ec;border-radius:8px;background:#fff;";
            card.innerHTML =
                "<div style=\"display:flex;justify-content:space-between;gap:12px;margin-bottom:10px;\"><strong class=\"pdf-file-title\"></strong><span class=\"pdf-card-state\">待解析</span></div>" +
                previewFieldHtml("文献标题 *", "title", fileName.replace(/\\.pdf$/i, ""), false) +
                "<div style=\"display:grid;grid-template-columns:1fr 1fr;gap:12px;\">" +
                previewFieldHtml("作者", "author_names", "", false) +
                previewFieldHtml("作者单位", "institution", "", false) +
                "</div>" +
                previewFieldHtml("作者与机构匹配（每行：作者 => 机构；多个机构用分号分隔）", "author_details_text", "", true) +
                "<div style=\"display:grid;grid-template-columns:1fr 1fr;gap:12px;\">" +
                previewFieldHtml("DOI", "doi", "", false) +
                previewFieldHtml("文献类型", "source_type", "", false) +
                "</div>" +
                "<div style=\"display:grid;grid-template-columns:1fr 1fr;gap:12px;\">" +
                previewFieldHtml("期刊名称", "journal_name", "", false) +
                previewFieldHtml("会议名称", "conference_name", "", false) +
                "</div>" +
                "<div style=\"display:grid;grid-template-columns:repeat(4,1fr);gap:12px;\">" +
                previewFieldHtml("发表年份", "publish_year", "", false) +
                previewFieldHtml("发表月份", "publish_month", "", false) +
                previewFieldHtml("发表日期", "publish_day", "", false) +
                previewFieldHtml("关键词", "keywords", "", false) +
                "</div>" +
                "<div style=\"display:grid;grid-template-columns:repeat(4,1fr);gap:12px;\">" +
                previewFieldHtml("卷", "volume", "", false) +
                previewFieldHtml("期", "issue", "", false) +
                previewFieldHtml("页码", "pages", "", false) +
                previewFieldHtml("出版社", "publisher", "", false) +
                "</div>" +
                previewFieldHtml("摘要", "abstract_text", "", true);
            card.querySelector(".pdf-file-title").innerText = fileName;
            list.appendChild(card);
            return card;
        }

        function setPreviewState(card, text, ok) {
            var el = card ? card.querySelector(".pdf-card-state") : null;
            if (!el) return;
            el.innerText = text || "";
            el.style.color = ok ? "#168449" : "#b42318";
        }

        function setPreviewField(card, name, value) {
            var el = card ? card.querySelector("[data-pdf-field='" + name + "']") : null;
            if (el && value !== undefined && value !== null) {
                el.value = value;
            }
        }

        function applyParsedPreview(card, data) {
            var authorDetails = pickAuthorDetailsFromParse(data);
            try {
                card.setAttribute("data-author-details", JSON.stringify(authorDetails));
            } catch (e) {
                card.setAttribute("data-author-details", "[]");
            }
            card.setAttribute("data-parse-ok", "true");
            setPreviewField(card, "title", data.title || "");
            setPreviewField(card, "author_names", data.author_names || "");
            setPreviewField(card, "institution", data.institution || "");
            setPreviewField(card, "doi", data.doi || "");
            setPreviewField(card, "source_type", data.source_type || "");
            setPreviewField(card, "journal_name", data.journal_name || "");
            setPreviewField(card, "conference_name", data.conference_name || "");
            setPreviewField(card, "publish_year", data.publish_year || "");
            setPreviewField(card, "publish_month", data.publish_month || "");
            setPreviewField(card, "publish_day", data.publish_day || "");
            setPreviewField(card, "volume", data.volume || "");
            setPreviewField(card, "issue", data.issue || "");
            setPreviewField(card, "pages", data.pages || "");
            setPreviewField(card, "publisher", data.publisher || "");
            setPreviewField(card, "keywords", data.keywords || "");
            setPreviewField(card, "abstract_text", normalizeAbstractText(data.abstract_text || ""));
            setPreviewField(card, "author_details_text", formatAuthorDetailsText(authorDetails));
        }

        function getPreviewField(card, name) {
            var el = card ? card.querySelector("[data-pdf-field='" + name + "']") : null;
            return el ? el.value : "";
        }

        function collectPdfPreviewPayload() {
            var payload = document.getElementById("<%= pdf_parse_payload.ClientID %>");
            var cards = document.querySelectorAll(".admin-pdf-preview-card");
            var items = [];
            for (var i = 0; i < cards.length; i++) {
                items.push({
                    success: cards[i].getAttribute("data-parse-ok") === "true",
                    file_name: cards[i].getAttribute("data-file-name") || "",
                    title: getPreviewField(cards[i], "title"),
                    author_names: getPreviewField(cards[i], "author_names"),
                    institution: getPreviewField(cards[i], "institution"),
                    doi: getPreviewField(cards[i], "doi"),
                    source_type: getPreviewField(cards[i], "source_type"),
                    journal_name: getPreviewField(cards[i], "journal_name"),
                    conference_name: getPreviewField(cards[i], "conference_name"),
                    publish_year: getPreviewField(cards[i], "publish_year"),
                    publish_month: getPreviewField(cards[i], "publish_month"),
                    publish_day: getPreviewField(cards[i], "publish_day"),
                    volume: getPreviewField(cards[i], "volume"),
                    issue: getPreviewField(cards[i], "issue"),
                    pages: getPreviewField(cards[i], "pages"),
                    publisher: getPreviewField(cards[i], "publisher"),
                    keywords: getPreviewField(cards[i], "keywords"),
                    abstract_text: getPreviewField(cards[i], "abstract_text"),
                    author_details: parseAuthorDetailsText(getPreviewField(cards[i], "author_details_text"))
                });
            }
            if (payload) {
                payload.value = JSON.stringify(items);
            }
        }

        (function () {
            var fileInput = document.getElementById("<%= import_file.ClientID %>");
            var parseBtn = document.getElementById("btnParseImportPdf");
            var list = document.getElementById("pdfPreviewList");
            var payload = document.getElementById("<%= pdf_parse_payload.ClientID %>");
            if (!fileInput || !parseBtn) return;

            fileInput.onchange = function () {
                if (payload) payload.value = "";
                if (list) list.innerHTML = "";
                setPdfParseStatus("", "");
            };

            parseBtn.onclick = function () {
                var files = fileInput.files || [];
                if (!files.length) {
                    setPdfParseStatus("&#35831;&#20808;&#36873;&#25321; PDF &#25991;&#20214;", "#b42318");
                    return false;
                }
                if (list) list.innerHTML = "";
                if (payload) payload.value = "";
                parseBtn.disabled = true;
                var nextIndex = 0;
                var finishedCount = 0;
                var activeCount = 0;
                var batchConcurrency = 3;
                var okCount = 0;

                function updatePdfParseProgress() {
                    setPdfParseStatus("正在解析 " + finishedCount + " / " + files.length + "，进行中 " + activeCount + " / " + batchConcurrency + "...", "#666");
                }

                function finishPdfParse() {
                    parseBtn.disabled = false;
                    collectPdfPreviewPayload();
                    setPdfParseStatus("&#35299;&#26512;&#23436;&#25104;&#65306;" + okCount + " / " + files.length + " &#20010;&#25104;&#21151;&#65292;&#35831;&#30830;&#35748;&#21518;&#23548;&#20837;", okCount === files.length ? "#168449" : "#b42318");
                }

                function launchNext() {
                    if (finishedCount >= files.length) {
                        finishPdfParse();
                        return;
                    }

                    while (activeCount < batchConcurrency && nextIndex < files.length) {
                        (function (currentIndex) {
                            var file = files[currentIndex];
                            var card = renderPdfPreviewCard(file);
                            var name = (file.name || "").toLowerCase();
                            nextIndex++;

                            if (name.lastIndexOf(".pdf") !== name.length - 4) {
                                setPreviewState(card, "非 PDF", false);
                                finishedCount++;
                                launchNext();
                                return;
                            }

                            activeCount++;
                            setPreviewState(card, "解析中", true);
                            updatePdfParseProgress();
                            var formData = new FormData();
                            formData.append("file", file);
                            var xhr = new XMLHttpRequest();
                            xhr.open("POST", "/admin/PdfParse.ashx", true);
                            xhr.onreadystatechange = function () {
                                if (xhr.readyState !== 4) return;
                                var data = null;
                                try {
                                    data = JSON.parse(xhr.responseText || "{}");
                                } catch (e) {
                                    data = null;
                                }
                                if (xhr.status === 200 && data && data.success === true) {
                                    applyParsedPreview(card, data);
                                    setPreviewState(card, "已解析", true);
                                    okCount++;
                                } else {
                                    setPreviewState(card, (data && data.message) ? data.message : "解析失败", false);
                                }
                                activeCount--;
                                finishedCount++;
                                launchNext();
                            };
                            xhr.send(formData);
                        })(nextIndex);
                    }

                    if (finishedCount < files.length) {
                        updatePdfParseProgress();
                    }
                }

                launchNext();
                return false;
            };
        })();

        setImportMode("csv");
    </script>
    <% } %>
</body>
</html>
