<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LiteratureUpload.aspx.cs" Inherits="Web.UserCenter.LiteratureUpload" %>
<%@ Register TagPrefix="LiteratureManager" TagName="css" Src="/css.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="top" Src="/top.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="foot" Src="/foot.ascx" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <title>&#25991;&#29486;&#25237;&#31295;</title>
    <LiteratureManager:css ID="css" runat="server" />
    <style>
        .lit-upload-page { padding: 38px 0 72px; background: #f5f8fc; min-height: calc(100vh - 130px); }
        .lit-upload-shell { width: 1120px; max-width: calc(100vw - 40px); margin: 0 auto; }
        .lit-upload-hero { margin-bottom: 18px; }
        .lit-upload-hero h1 { margin: 0 0 10px; color: #17283b; font-size: 34px; letter-spacing: 0; }
        .lit-upload-hero p { margin: 0; max-width: 760px; color: #5f7085; line-height: 1.9; }
        .lit-mode-switch { display: inline-flex; align-items: center; gap: 6px; margin: 0 0 20px; padding: 4px; border-radius: 999px; background: #fff; border: 1px solid #e0e0e0; }
        .lit-mode-switch button { min-width: 128px; height: 40px; border: 0; border-radius: 999px; background: transparent; color: #333; cursor: pointer; font-size: 14px; }
        .lit-mode-switch button.active { background: #0066cc; color: #fff; }
        .lit-upload-panel { display: none; }
        .lit-upload-panel.active { display: block; }
        .lit-upload-card { background: #fff; border: 1px solid #e3ebf5; border-radius: 16px; padding: 30px; box-shadow: 0 18px 45px rgba(18, 42, 70, .06); }
        .lit-upload-title { margin-bottom: 20px; padding-bottom: 16px; border-bottom: 1px solid #edf2f7; }
        .lit-upload-title h4 { margin: 0 0 8px; font-size: 22px; color: #1a2d42; }
        .lit-upload-title p { margin: 0; color: #6a7788; line-height: 1.8; }
        .lit-upload-row { margin-bottom: 18px; }
        .lit-upload-row label { display: block; margin-bottom: 8px; color: #314357; font-weight: 600; }
        .lit-upload-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
        .lit-upload-input, .lit-upload-area, .lit-upload-select { width: 100%; border: 1px solid #d7e0ea; border-radius: 12px; padding: 10px 14px; font-size: 14px; box-sizing: border-box; }
        .lit-upload-area { min-height: 140px; resize: vertical; }
        .lit-upload-actions { margin-top: 24px; }
        .lit-upload-actions .btn { display: inline-block; min-width: 132px; height: 44px; line-height: 44px; text-align: center; border-radius: 12px; border: none; cursor: pointer; }
        .lit-upload-actions .btn-primary { background: #1d6fdc; color: #fff; }
        .lit-upload-actions .btn-secondary { background: #eef5ff; color: #1d6fdc; margin-left: 12px; }
        .lit-upload-hint { color: #7a8795; font-size: 13px; margin-top: 8px; }
        .lit-batch-drop { display: grid; gap: 12px; padding: 24px; border: 1px dashed #d2d2d7; border-radius: 18px; background: #fafafc; }
        .lit-batch-drop strong { color: #1d1d1f; font-size: 17px; line-height: 1.24; }
        .lit-batch-drop input[type="file"] { width: 100%; color: #333; }
        .lit-batch-drop input[type="button"] { width: auto; min-width: 92px; }
        .lit-batch-file-actions { display: flex; align-items: center; gap: 12px; flex-wrap: wrap; }
        .lit-batch-file-actions button { min-height: 40px; padding: 0 18px; border: 1px solid #0066cc; border-radius: 999px; background: #fff; color: #0066cc; cursor: pointer; }
        .lit-batch-file-actions .clear { border-color: #d2d2d7; color: #333; }
        .lit-hidden-file { display: none; }
        .lit-batch-options { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
        .lit-batch-detail-list { display: grid; gap: 18px; margin-top: 22px; }
        .lit-batch-detail-card { border: 1px solid #e0e0e0; border-radius: 18px; background: #fff; padding: 22px; }
        .lit-batch-detail-head { display: flex; align-items: center; justify-content: space-between; gap: 14px; margin-bottom: 18px; }
        .lit-batch-detail-file { min-width: 0; color: #1d1d1f; font-weight: 700; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
        .lit-batch-detail-state { color: #7a7a7a; font-size: 13px; white-space: nowrap; }
        .lit-batch-detail-state.ok { color: #1f8f4d; }
        .lit-batch-detail-state.err { color: #d9534f; }
        #parseStatus { margin-left: 12px; color: #6a7788; }
        @media (max-width: 860px) {
            .lit-upload-page { padding-top: 24px; }
            .lit-upload-hero h1 { font-size: 28px; }
            .lit-upload-card { padding: 22px; }
            .lit-upload-grid { grid-template-columns: 1fr; }
            .lit-upload-actions .btn-secondary { margin-left: 0; margin-top: 10px; }
        }
        body.ac .middle {
            padding: 128px 32px 170px !important;
        }
        .lit-upload-page {
            padding: 36px 0 0 !important;
            min-height: auto !important;
            background: transparent !important;
        }
        .lit-upload-shell {
            width: 1280px !important;
            max-width: 100% !important;
        }
        .lit-upload-hero {
            max-width: 896px !important;
            margin: 36px auto 64px !important;
            text-align: center !important;
            background: transparent !important;
            border: 0 !important;
            box-shadow: none !important;
        }
        .lit-upload-hero h1 {
            margin: 0 0 18px !important;
            color: #111827 !important;
            font-size: clamp(48px, 7vw, 72px) !important;
            line-height: 1.1 !important;
            font-weight: 800 !important;
            letter-spacing: -0.055em !important;
        }
        .lit-upload-hero p {
            max-width: 720px !important;
            margin: 0 auto !important;
            color: #6b7280 !important;
            font-size: 18px !important;
            line-height: 1.65 !important;
        }
        .lit-mode-switch {
            margin: 0 auto 22px !important;
            background: #fff !important;
            border-color: #e0e0e0 !important;
            box-shadow: none !important;
        }
        .lit-mode-switch button {
            color: #333 !important;
            font-weight: 600 !important;
        }
        .lit-mode-switch button.active {
            background: #0066cc !important;
            color: #fff !important;
        }
        .lit-upload-card {
            max-width: 1080px !important;
            margin: 0 auto 32px !important;
            padding: 36px !important;
            border-radius: 24px !important;
            background: rgba(255,255,255,.4) !important;
            border: 1px solid rgba(255,255,255,.3) !important;
            box-shadow: 0 18px 48px rgba(31,50,68,.08), inset 0 1px 0 rgba(255,255,255,.65) !important;
            backdrop-filter: blur(16px) !important;
            -webkit-backdrop-filter: blur(16px) !important;
        }
        .lit-upload-title {
            margin-bottom: 28px !important;
            padding-bottom: 24px !important;
            border-bottom: 1px solid rgba(229,231,235,.75) !important;
        }
        .lit-upload-title h4 {
            margin: 0 0 14px !important;
            color: #111827 !important;
            font-size: 12px !important;
            line-height: 1.2 !important;
            font-weight: 800 !important;
            letter-spacing: .18em !important;
            text-transform: uppercase !important;
        }
        .lit-upload-title p {
            color: #6b7280 !important;
            font-size: 14px !important;
            line-height: 1.85 !important;
        }
        .lit-upload-row label {
            color: #111827 !important;
            font-size: 13px !important;
            font-weight: 800 !important;
            letter-spacing: .02em !important;
        }
        .lit-upload-input,
        .lit-upload-area,
        .lit-upload-select {
            min-height: 56px !important;
            border-radius: 16px !important;
            background: rgba(255,255,255,.48) !important;
            border: 1px solid rgba(255,255,255,.45) !important;
            box-shadow: inset 0 1px 0 rgba(255,255,255,.8), 0 10px 24px rgba(31,50,68,.04) !important;
            color: #111827 !important;
            font-size: 15px !important;
        }
        .lit-upload-area {
            min-height: 160px !important;
        }
        .lit-upload-actions {
            display: flex !important;
            align-items: center !important;
            gap: 14px !important;
            margin-top: 30px !important;
        }
        .lit-upload-actions .btn,
        #btnParseBatchPdf,
        #btnParsePdf {
            min-width: 140px !important;
            height: 52px !important;
            line-height: 52px !important;
            padding: 0 22px !important;
            border-radius: 16px !important;
            font-weight: 700 !important;
            box-sizing: border-box !important;
        }
        .lit-upload-actions .btn-primary {
            background: #0066cc !important;
            color: #fff !important;
            box-shadow: 0 10px 15px -3px rgba(59,130,246,.2), 0 4px 6px -4px rgba(59,130,246,.2) !important;
        }
        .lit-upload-actions .btn-secondary,
        #btnParseBatchPdf,
        #btnParsePdf {
            margin-left: 0 !important;
            background: rgba(255,255,255,.48) !important;
            border: 1px solid rgba(255,255,255,.45) !important;
            color: #111827 !important;
            box-shadow: inset 0 1px 0 rgba(255,255,255,.8) !important;
        }
        .lit-upload-hint {
            color: #6b7280 !important;
        }
        .lit-batch-drop {
            border-color: #e0e0e0 !important;
            border-radius: 18px !important;
            background: #fafafc !important;
            box-shadow: none !important;
        }
        @media (max-width: 760px) {
            body.ac .middle {
                padding: 104px 16px 220px !important;
            }
            .lit-upload-hero {
                margin-bottom: 44px !important;
            }
            .lit-upload-hero h1 {
                font-size: 42px !important;
            }
            .lit-upload-card {
                padding: 24px !important;
            }
            .lit-upload-actions {
                flex-direction: column !important;
                align-items: stretch !important;
            }
            .lit-batch-options {
                grid-template-columns: 1fr !important;
            }
            .lit-batch-detail-head {
                align-items: flex-start !important;
                flex-direction: column !important;
            }
        }
    </style>
</head>
<body class="ac">
    <LiteratureManager:top ID="top" runat="server" />
    <div class="middle">
        <section class="lit-upload-page">
            <div class="lit-upload-shell">
                <div class="lit-upload-hero">
                    <h1>&#25991;&#29486;&#25237;&#31295;</h1>
                    <div class="lit-mode-switch" aria-label="上传模式">
                        <button type="button" class="active" data-upload-mode="single" onclick="switchUploadMode('single')">单篇上传</button>
                        <button type="button" data-upload-mode="batch" onclick="switchUploadMode('batch')">批量上传</button>
                    </div>
                    <p>&#19978;&#20256; PDF &#38468;&#20214;&#24182;&#22635;&#20889;&#25991;&#29486;&#20449;&#24687;&#65292;&#31995;&#32479;&#21487;&#33258;&#21160;&#35299;&#26512;&#26631;&#39064;&#12289;&#20316;&#32773;&#12289;&#21333;&#20301;&#12289;&#25688;&#35201;&#31561;&#20869;&#23481;&#65292;&#25552;&#20132;&#21518;&#30001;&#31649;&#29702;&#21592;&#23457;&#26680;&#20844;&#24320;&#23637;&#31034;&#12290;</p>
                </div>
                <form id="form1" runat="server">
                    <asp:HiddenField ID="batch_parse_payload" runat="server" />
                            <div class="lit-upload-card lit-upload-panel active" id="singleUploadPanel">
                                <div class="lit-upload-title">
                                    <h4>&#22522;&#26412;&#20449;&#24687;</h4>
                                    <p>&#21069;&#21488;&#29992;&#25143;&#21482;&#33021;&#25552;&#20132;&#25991;&#29486;&#22522;&#26412;&#20449;&#24687;&#19982; PDF &#38468;&#20214;&#65292;&#38656;&#32463;&#21518;&#21488;&#23457;&#26680;&#36890;&#36807;&#21518;&#25165;&#20250;&#22312;&#39318;&#39029;&#19982;&#26816;&#32034;&#39029;&#23637;&#31034;&#12290;</p>
                                    <p>&#19979;&#36733;&#31215;&#20998;&#30001;&#31649;&#29702;&#21592;&#23457;&#26680;&#26102;&#35774;&#32622;&#65292;&#24744;&#26080;&#38656;&#22635;&#20889;&#12290;</p>
                                </div>

                                <div class="lit-upload-row">
                                    <label>&#25991;&#29486;&#26631;&#39064; *</label>
                                    <asp:TextBox ID="title" runat="server" CssClass="lit-upload-input"></asp:TextBox>
                                </div>

                                <div class="lit-upload-grid">
                                    <div class="lit-upload-row">
                                        <label>&#20316;&#32773;</label>
                                        <asp:TextBox ID="author_names" runat="server" CssClass="lit-upload-input"></asp:TextBox>
                                    </div>
                                    <div class="lit-upload-row">
                                        <label>&#20316;&#32773;&#21333;&#20301;</label>
                                        <asp:TextBox ID="institution" runat="server" CssClass="lit-upload-input"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="lit-upload-row">
                                    <label>DOI</label>
                                    <asp:TextBox ID="doi" runat="server" CssClass="lit-upload-input"></asp:TextBox>
                                </div>

                                <div class="lit-upload-grid">
                                    <div class="lit-upload-row">
                                        <label>&#25991;&#29486;&#31867;&#22411;</label>
                                        <asp:DropDownList ID="source_type" runat="server" CssClass="lit-upload-select">
                                            <asp:ListItem Value="&#26399;&#21002;&#35770;&#25991;">&#26399;&#21002;&#35770;&#25991;</asp:ListItem>
                                            <asp:ListItem Value="&#20250;&#35758;&#35770;&#25991;">&#20250;&#35758;&#35770;&#25991;</asp:ListItem>
                                            <asp:ListItem Value="&#23398;&#20301;&#35770;&#25991;">&#23398;&#20301;&#35770;&#25991;</asp:ListItem>
                                            <asp:ListItem Value="&#19987;&#21033;">&#19987;&#21033;</asp:ListItem>
                                            <asp:ListItem Value="&#22270;&#20070;">&#22270;&#20070;</asp:ListItem>
                                            <asp:ListItem Value="&#25253;&#21578;">&#25253;&#21578;</asp:ListItem>
                                            <asp:ListItem Value="&#20854;&#20182;">&#20854;&#20182;</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="lit-upload-row">
                                        <label>&#20998;&#31867;</label>
                                        <asp:DropDownList ID="category_id" runat="server" CssClass="lit-upload-select"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="lit-upload-grid">
                                    <div class="lit-upload-row">
                                        <label>&#26399;&#21002;&#21517;&#31216;</label>
                                        <asp:TextBox ID="journal_name" runat="server" CssClass="lit-upload-input"></asp:TextBox>
                                    </div>
                                    <div class="lit-upload-row">
                                        <label>&#20250;&#35758;&#21517;&#31216;</label>
                                        <asp:TextBox ID="conference_name" runat="server" CssClass="lit-upload-input"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="lit-upload-grid">
                                    <div class="lit-upload-row">
                                        <label>&#21457;&#34920;&#24180;&#20221;</label>
                                        <asp:TextBox ID="publish_year" runat="server" CssClass="lit-upload-input"></asp:TextBox>
                                    </div>
                                    <div class="lit-upload-row">
                                        <label>&#20851;&#38190;&#35789;</label>
                                        <asp:TextBox ID="keywords" runat="server" CssClass="lit-upload-input"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="lit-upload-grid">
                                    <div class="lit-upload-row">
                                        <label>&#21367;</label>
                                        <asp:TextBox ID="volume" runat="server" CssClass="lit-upload-input"></asp:TextBox>
                                    </div>
                                    <div class="lit-upload-row">
                                        <label>&#26399;</label>
                                        <asp:TextBox ID="issue" runat="server" CssClass="lit-upload-input"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="lit-upload-grid">
                                    <div class="lit-upload-row">
                                        <label>&#39029;&#30721;</label>
                                        <asp:TextBox ID="pages" runat="server" CssClass="lit-upload-input" placeholder="例如 12-20"></asp:TextBox>
                                    </div>
                                    <div class="lit-upload-row">
                                        <label>&#20986;&#29256;&#31038;</label>
                                        <asp:TextBox ID="publisher" runat="server" CssClass="lit-upload-input"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="lit-upload-row">
                                    <label>&#25688;&#35201;</label>
                                    <asp:TextBox ID="abstract_text" runat="server" TextMode="MultiLine" CssClass="lit-upload-area"></asp:TextBox>
                                </div>

                                <div class="lit-upload-row">
                                    <label>PDF &#38468;&#20214; *</label>
                                     <asp:FileUpload ID="pdf_file" runat="server" accept=".pdf" />
                                     <div class="lit-upload-hint">
                                        &#25903;&#25345;&#19978;&#20256; PDF &#25991;&#20214;&#65292;&#21333;&#20010;&#25991;&#20214;&#19981;&#36229;&#36807; <%= UploadPolicy.ToMbLabel(UploadPolicy.MaxPdfBytes) %>&#12290;&#22914;&#26524; PDF &#20869;&#21547;&#20803;&#25968;&#25454;&#65292;&#21487;&#20808;&#28857;&#20987;&#33258;&#21160;&#35299;&#26512;&#65292;&#20877;&#34917;&#20805;&#26680;&#23545;&#20449;&#24687;&#12290;
                                    </div>
                                    <div class="lit-upload-hint">
                                        <input type="button" id="btnParsePdf" value="&#33258;&#21160;&#35299;&#26512;PDF" class="btn btn-secondary" />
                                        <span id="parseStatus"></span>
                                    </div>
                                </div>

                                <div class="lit-upload-actions">
                                    <asp:Button ID="ButtonSubmit" runat="server" CssClass="btn btn-primary" Text="&#25552;&#20132;&#23457;&#26680;" OnClick="ButtonSubmit_Click" />
                                    <a href="/LiteratureSearch.aspx" class="btn btn-secondary">&#36820;&#22238;&#26816;&#32034;</a>
                                </div>
                            </div>

                            <div class="lit-upload-card lit-upload-panel" id="batchUploadPanel">
                                <div class="lit-upload-title">
                                    <h4>PDF 批量上传</h4>
                                    <p>一次选择多个 PDF，系统会逐个解析并按解析结果生成待审核文献记录；解析失败的文件仍会按文件名提交，后台审核时可继续补全。</p>
                                </div>
                                <div class="lit-batch-drop">
                                    <strong>选择多个 PDF 文件</strong>
                                    <input id="batch_pdf_files" name="batch_pdf_files" type="file" multiple="multiple" accept=".pdf,application/pdf" />
                                    <input id="batch_pdf_files_more" class="lit-hidden-file" type="file" multiple="multiple" accept=".pdf,application/pdf" />
                                    <div class="lit-batch-file-actions">
                                        <button type="button" onclick="document.getElementById('batch_pdf_files_more').click();">继续添加 PDF</button>
                                        <button type="button" class="clear" onclick="clearBatchFiles();">清空列表</button>
                                        <span id="batchFileCount">已选择 0 个 PDF</span>
                                    </div>
                                    <div class="lit-upload-hint">PDF 可以分多次、从不同文件夹添加；单个文件不超过 <%= UploadPolicy.ToMbLabel(UploadPolicy.MaxPdfBytes) %>，单次最多 <%= UploadPolicy.MaxBatchFiles %> 个、总计不超过 <%= UploadPolicy.ToMbLabel(UploadPolicy.MaxBatchTotalBytes) %>。每个文件会独立保存为一篇待审核文献，下载积分默认为 0。</div>
                                    <div class="lit-upload-hint">
                                        <input type="button" id="btnParseBatchPdf" value="解析PDF" class="btn btn-secondary" />
                                        <span id="batchParseStatus"></span>
                                    </div>
                                </div>
                                <div class="lit-batch-options">
                                    <div class="lit-upload-row">
                                        <label>批量文献类型</label>
                                        <asp:DropDownList ID="batch_source_type" runat="server" CssClass="lit-upload-select">
                                            <asp:ListItem Value="期刊论文">期刊论文</asp:ListItem>
                                            <asp:ListItem Value="会议论文">会议论文</asp:ListItem>
                                            <asp:ListItem Value="学位论文">学位论文</asp:ListItem>
                                            <asp:ListItem Value="专利">专利</asp:ListItem>
                                            <asp:ListItem Value="图书">图书</asp:ListItem>
                                            <asp:ListItem Value="报告">报告</asp:ListItem>
                                            <asp:ListItem Value="其他">其他</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="lit-upload-row">
                                        <label>批量分类</label>
                                        <asp:DropDownList ID="batch_category_id" runat="server" CssClass="lit-upload-select"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="lit-batch-detail-list" id="batchDetailList"></div>
                                <div class="lit-upload-actions">
                                    <asp:Button ID="ButtonBatchUpload" runat="server" CssClass="btn btn-primary" Text="批量提交审核" OnClientClick="collectBatchDetails();" OnClick="ButtonBatchUpload_Click" />
                                </div>
                            </div>
                </form>
            </div>
        </section>
    </div>
    <LiteratureManager:foot ID="foot" runat="server" />
    <script type="text/javascript">
        function switchUploadMode(mode) {
            var singlePanel = document.getElementById("singleUploadPanel");
            var batchPanel = document.getElementById("batchUploadPanel");
            var buttons = document.querySelectorAll(".lit-mode-switch button");
            for (var i = 0; i < buttons.length; i++) {
                buttons[i].className = buttons[i].getAttribute("data-upload-mode") === mode ? "active" : "";
            }
            if (singlePanel) singlePanel.className = mode === "single" ? "lit-upload-card lit-upload-panel active" : "lit-upload-card lit-upload-panel";
            if (batchPanel) batchPanel.className = mode === "batch" ? "lit-upload-card lit-upload-panel active" : "lit-upload-card lit-upload-panel";
        }

        function collectBatchDetails() {
            var payloadInput = document.getElementById("<%= batch_parse_payload.ClientID %>");
            var cards = document.querySelectorAll(".lit-batch-detail-card");
            var items = [];
            for (var i = 0; i < cards.length; i++) {
                items.push({
                    success: true,
                    file_name: cards[i].getAttribute("data-file-name") || "",
                    title: getBatchField(cards[i], "title"),
                    author_names: getBatchField(cards[i], "author_names"),
                    institution: getBatchField(cards[i], "institution"),
                    doi: getBatchField(cards[i], "doi"),
                    source_type: getBatchField(cards[i], "source_type"),
                    category_id: getBatchField(cards[i], "category_id"),
                    journal_name: getBatchField(cards[i], "journal_name"),
                    conference_name: getBatchField(cards[i], "conference_name"),
                    publish_year: getBatchField(cards[i], "publish_year"),
                    volume: getBatchField(cards[i], "volume"),
                    issue: getBatchField(cards[i], "issue"),
                    pages: getBatchField(cards[i], "pages"),
                    publisher: getBatchField(cards[i], "publisher"),
                    keywords: getBatchField(cards[i], "keywords"),
                    abstract_text: getBatchField(cards[i], "abstract_text")
                });
            }
            if (payloadInput) {
                payloadInput.value = JSON.stringify(items);
            }
            return true;
        }

        function getBatchField(card, name) {
            var el = card.querySelector("[data-batch-field='" + name + "']");
            return el ? el.value : "";
        }

        function clearBatchFiles() {
            if (window.resetBatchFileStore) {
                window.resetBatchFileStore();
            }
        }

        (function () {
            function setText(id, value) {
                var el = document.getElementById(id);
                if (el && value) {
                    el.value = value;
                }
            }

            function setStatus(text, color) {
                var el = document.getElementById("parseStatus");
                if (!el) return;
                el.innerHTML = text || "";
                if (color) {
                    el.style.color = color;
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
                if (file.size > <%= UploadPolicy.MaxPdfBytes %>) {
                    setStatus("PDF 文件不能超过 <%= UploadPolicy.ToMbLabel(UploadPolicy.MaxPdfBytes) %>", "#d9534f");
                    return false;
                }

                var formData = new FormData();
                formData.append("file", file);
                setStatus("&#27491;&#22312;&#35299;&#26512;&#20013;...", "#666666");

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

                    if (xhr.status !== 200 || !data || data.success !== true) {
                        setStatus((data && data.message) ? data.message : "PDF &#35299;&#26512;&#22833;&#36133;", "#d9534f");
                        return;
                    }

                    setText("<%= title.ClientID %>", data.title);
                    setText("<%= author_names.ClientID %>", data.author_names);
                    setText("<%= institution.ClientID %>", data.institution);
                    setText("<%= doi.ClientID %>", data.doi);
                    setText("<%= publish_year.ClientID %>", data.publish_year);
                    setText("<%= journal_name.ClientID %>", data.journal_name);
                    setText("<%= conference_name.ClientID %>", data.conference_name);
                    setText("<%= volume.ClientID %>", data.volume);
                    setText("<%= issue.ClientID %>", data.issue);
                    setText("<%= pages.ClientID %>", data.pages);
                    setText("<%= publisher.ClientID %>", data.publisher);
                    setText("<%= keywords.ClientID %>", data.keywords);
                    setText("<%= abstract_text.ClientID %>", data.abstract_text);
                    if (data.source_type) {
                        var source = document.getElementById("<%= source_type.ClientID %>");
                        if (source) {
                            source.value = data.source_type;
                        }
                    }
                    setStatus("PDF &#20449;&#24687;&#24050;&#33258;&#21160;&#22635;&#20889;", "#28a745");
                };
                xhr.send(formData);
                return false;
            };
        })();

        (function () {
            var btn = document.getElementById("btnParseBatchPdf");
            var fileInput = document.getElementById("batch_pdf_files");
            var moreFileInput = document.getElementById("batch_pdf_files_more");
            var payloadInput = document.getElementById("<%= batch_parse_payload.ClientID %>");
            var list = document.getElementById("batchDetailList");
            var status = document.getElementById("batchParseStatus");
            if (!btn || !fileInput || !payloadInput) return;
            var sourceOptions = document.getElementById("<%= batch_source_type.ClientID %>") ? document.getElementById("<%= batch_source_type.ClientID %>").innerHTML : "";
            var categoryOptions = document.getElementById("<%= batch_category_id.ClientID %>") ? document.getElementById("<%= batch_category_id.ClientID %>").innerHTML : "";
            var fileCount = document.getElementById("batchFileCount");
            var batchTransfer = window.DataTransfer ? new DataTransfer() : null;
            var maxBatchFiles = <%= UploadPolicy.MaxBatchFiles %>;
            var maxBatchTotalBytes = <%= UploadPolicy.MaxBatchTotalBytes %>;
            var maxPdfBytes = <%= UploadPolicy.MaxPdfBytes %>;

            function setBatchStatus(text, color) {
                if (!status) return;
                status.innerHTML = text || "";
                if (color) status.style.color = color;
            }

            function getBatchLimitError(files) {
                var totalBytes = 0;
                if (files.length > maxBatchFiles) {
                    return "单次最多选择 " + maxBatchFiles + " 个 PDF";
                }
                for (var i = 0; i < files.length; i++) {
                    if (files[i].size > maxPdfBytes) {
                        return "单个 PDF 不能超过 <%= UploadPolicy.ToMbLabel(UploadPolicy.MaxPdfBytes) %>";
                    }
                    totalBytes += files[i].size;
                }
                if (totalBytes > maxBatchTotalBytes) {
                    return "文件总大小不能超过 <%= UploadPolicy.ToMbLabel(UploadPolicy.MaxBatchTotalBytes) %>";
                }
                return "";
            }

            function escapeHtml(value) {
                return String(value || "").replace(/[&<>"']/g, function (ch) {
                    return ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", "\"": "&quot;", "'": "&#39;" })[ch];
                });
            }

            function fieldHtml(label, name, value, area) {
                if (area) {
                    return "<div class=\"lit-upload-row\"><label>" + label + "</label><textarea class=\"lit-upload-area\" data-batch-field=\"" + name + "\">" + escapeHtml(value) + "</textarea></div>";
                }
                return "<div class=\"lit-upload-row\"><label>" + label + "</label><input type=\"text\" class=\"lit-upload-input\" data-batch-field=\"" + name + "\" value=\"" + escapeHtml(value) + "\" /></div>";
            }

            function renderItem(file, state, className) {
                if (!list) return null;
                var row = document.createElement("div");
                var fileName = file.name || "";
                row.className = "lit-batch-detail-card";
                row.setAttribute("data-file-name", fileName);
                row.innerHTML =
                    "<div class=\"lit-batch-detail-head\"><div class=\"lit-batch-detail-file\"></div><span class=\"lit-batch-detail-state\"></span></div>" +
                    fieldHtml("文献标题 *", "title", fileName.replace(/\\.pdf$/i, ""), false) +
                    "<div class=\"lit-upload-grid\">" +
                    fieldHtml("作者", "author_names", "", false) +
                    fieldHtml("作者单位", "institution", "", false) +
                    "</div>" +
                    fieldHtml("DOI", "doi", "", false) +
                    "<div class=\"lit-upload-grid\"><div class=\"lit-upload-row\"><label>文献类型</label><select class=\"lit-upload-select\" data-batch-field=\"source_type\">" + sourceOptions + "</select></div><div class=\"lit-upload-row\"><label>分类</label><select class=\"lit-upload-select\" data-batch-field=\"category_id\">" + categoryOptions + "</select></div></div>" +
                    "<div class=\"lit-upload-grid\">" +
                    fieldHtml("期刊名称", "journal_name", "", false) +
                    fieldHtml("会议名称", "conference_name", "", false) +
                    "</div>" +
                    "<div class=\"lit-upload-grid\">" +
                    fieldHtml("发表年份", "publish_year", "", false) +
                    fieldHtml("关键词", "keywords", "", false) +
                    "</div>" +
                    "<div class=\"lit-upload-grid\">" +
                    fieldHtml("卷", "volume", "", false) +
                    fieldHtml("期", "issue", "", false) +
                    "</div>" +
                    "<div class=\"lit-upload-grid\">" +
                    fieldHtml("页码", "pages", "", false) +
                    fieldHtml("出版社", "publisher", "", false) +
                    "</div>" +
                    fieldHtml("摘要", "abstract_text", "", true);
                row.querySelector(".lit-batch-detail-file").innerText = fileName;
                setCardState(row, state, className);
                list.appendChild(row);
                return row;
            }

            function setCardState(row, state, className) {
                var stateEl = row ? row.querySelector(".lit-batch-detail-state") : null;
                if (!stateEl) return;
                stateEl.innerText = state || "";
                stateEl.className = className ? "lit-batch-detail-state " + className : "lit-batch-detail-state";
            }

            function setCardField(row, name, value) {
                var el = row ? row.querySelector("[data-batch-field='" + name + "']") : null;
                if (el && value) {
                    el.value = value;
                }
            }

            function applyParsedData(row, data) {
                setCardField(row, "title", data.title);
                setCardField(row, "author_names", data.author_names);
                setCardField(row, "institution", data.institution);
                setCardField(row, "doi", data.doi);
                setCardField(row, "publish_year", data.publish_year);
                setCardField(row, "journal_name", data.journal_name);
                setCardField(row, "conference_name", data.conference_name);
                setCardField(row, "volume", data.volume);
                setCardField(row, "issue", data.issue);
                setCardField(row, "pages", data.pages);
                setCardField(row, "publisher", data.publisher);
                setCardField(row, "keywords", data.keywords);
                setCardField(row, "abstract_text", data.abstract_text);
                setCardField(row, "source_type", data.source_type);
            }

            function parseOne(file, row, done) {
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
                        setCardState(row, "已解析", "ok");
                        applyParsedData(row, data);
                        data.file_name = file.name || "";
                        done(data);
                        return;
                    }
                    setCardState(row, (data && data.message) ? data.message : "解析失败", "err");
                    done({ file_name: file.name || "", success: false });
                };
                xhr.send(formData);
            }

            btn.onclick = function () {
                var files = fileInput.files || [];
                if (files.length === 0) {
                    setBatchStatus("请先选择 PDF 文件", "#d9534f");
                    return false;
                }
                var limitError = getBatchLimitError(files);
                if (limitError) {
                    setBatchStatus(limitError, "#d9534f");
                    return false;
                }
                if (list) list.innerHTML = "";
                payloadInput.value = "";
                renderBatchCards(files);
                btn.disabled = true;
                setBatchStatus("正在解析 0 / " + files.length + "...", "#666666");

                var index = 0;
                var results = [];
                function next() {
                    if (index >= files.length) {
                        payloadInput.value = JSON.stringify(results);
                        btn.disabled = false;
                        setBatchStatus("批量解析完成：" + results.filter(function (item) { return item.success === true; }).length + " / " + files.length + " 个成功", "#28a745");
                        return;
                    }

                    var file = files[index];
                    var name = (file.name || "").toLowerCase();
                    var row = list.querySelectorAll(".lit-batch-detail-card")[index] || renderItem(file, "等待解析", "");
                    if (name.lastIndexOf(".pdf") !== name.length - 4) {
                        setCardState(row, "非 PDF", "err");
                        results.push({ file_name: file.name || "", success: false });
                        index++;
                        next();
                        return;
                    }

                    setCardState(row, "解析中", "");
                    setBatchStatus("正在解析 " + (index + 1) + " / " + files.length + "...", "#666666");
                    parseOne(file, row, function (data) {
                        results.push(data);
                        index++;
                        next();
                    });
                }
                next();
                return false;
            };

            fileInput.onchange = function () {
                addBatchFiles(fileInput.files || [], true);
            };

            if (moreFileInput) {
                moreFileInput.onchange = function () {
                    addBatchFiles(moreFileInput.files || [], false);
                    moreFileInput.value = "";
                };
            }

            window.resetBatchFileStore = function () {
                if (batchTransfer) {
                    batchTransfer = new DataTransfer();
                    fileInput.files = batchTransfer.files;
                } else {
                    fileInput.value = "";
                }
                payloadInput.value = "";
                renderBatchCards([]);
                updateBatchFileCount();
                setBatchStatus("", "");
            };

            function renderBatchCards(files) {
                if (!list) return;
                list.innerHTML = "";
                for (var i = 0; i < files.length; i++) {
                    renderItem(files[i], "待解析", "");
                }
                collectBatchDetails();
            }

            function addBatchFiles(files, replaceCurrent) {
                if (!files || files.length === 0) {
                    updateBatchFileCount();
                    return;
                }

                if (batchTransfer) {
                    if (replaceCurrent && batchTransfer.files.length === 0) {
                        batchTransfer = new DataTransfer();
                    }

                    var exists = {};
                    for (var i = 0; i < batchTransfer.files.length; i++) {
                        exists[getFileKey(batchTransfer.files[i])] = true;
                    }
                    for (var j = 0; j < files.length; j++) {
                        var key = getFileKey(files[j]);
                        if (!exists[key]) {
                            batchTransfer.items.add(files[j]);
                            exists[key] = true;
                        }
                    }
                    fileInput.files = batchTransfer.files;
                }

                payloadInput.value = "";
                renderBatchCards(fileInput.files || files);
                updateBatchFileCount();
                setBatchStatus("", "");
            }

            function getFileKey(file) {
                return [file.name, file.size, file.lastModified].join("|");
            }

            function updateBatchFileCount() {
                if (fileCount) {
                    var count = fileInput.files ? fileInput.files.length : 0;
                    fileCount.innerText = "已选择 " + count + " 个 PDF";
                }
            }

            updateBatchFileCount();
        })();
    </script>
</body>
</html>
