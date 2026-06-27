using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Web.admin
{
    public partial class Admin_LiteratureEdit : System.Web.UI.Page
    {
        private readonly BLLBase<Literature> literatureBll = new BLLBase<Literature>();
        private readonly BLLBase<LiteratureCategory> categoryBll = new BLLBase<LiteratureCategory>();
        private readonly BLLBase<LiteratureTag> tagBll = new BLLBase<LiteratureTag>();
        private readonly BLLBase<Journal> journalBll = new BLLBase<Journal>();
        private readonly BLLBase<Conference> conferenceBll = new BLLBase<Conference>();
        private readonly BLLBase<NoticeLog_List> noticeLogBll = new BLLBase<NoticeLog_List>();
        private readonly string Action = Function.GetRequest("Action");
        private const string DuplicateSubmissionRemarkPrefix = "[重复投稿]关联文献ID:";
        private const string MetadataRevisionRemarkPrefix = "[元数据修改]原文献ID:";
        public int MenuId = Function.ConvertTo<int>(Function.GetRequest("MenuId"), 0);
        public bool isLoading = false;
        public string duplicateMasterNoticeHtml = string.Empty;
        public string AuthorAffiliationEditorHtml = string.Empty;
        public string InstitutionDatalistHtml = string.Empty;
        public string JournalDatalistHtml = string.Empty;
        public string ConferenceDatalistHtml = string.Empty;
        public string InstitutionOptionsJson = "[]";
        public string JournalOptionsJson = "[]";
        public string ConferenceOptionsJson = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            LoadMasterDataOptions();
            ConfigureMasterDataControls();
            if (!IsPostBack)
            {
                BindCategoryDropDown();
                BindTagList();
                if (Action == "Edit")
                {
                    EditFunc();
                }
                else
                {
                    Txt_Title.Text = "\u6DFB\u52A0\u6587\u732E";
                }
            }
        }

        protected void BindCategoryDropDown()
        {
            category_id.Items.Clear();
            category_id.Items.Add(new System.Web.UI.WebControls.ListItem("\u672A\u5206\u7C7B", "0"));
            DataTable dt = categoryBll.GetDatatable("select id,name from LiteratureCategory where status=1 order by orderid asc,id asc");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    category_id.Items.Add(new System.Web.UI.WebControls.ListItem(row["name"].ToString(), row["id"].ToString()));
                }
            }
        }

        protected void BindTagList()
        {
            TagList.Items.Clear();
            DataTable dt = tagBll.GetDatatable("select id,name from LiteratureTag where status=1 order by orderid asc,id asc");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    TagList.Items.Add(new System.Web.UI.WebControls.ListItem(row["name"].ToString(), row["id"].ToString()));
                }
            }
        }

        private void ConfigureMasterDataControls()
        {
            journal_name.Attributes["list"] = "journalMasterList";
            journal_name.Attributes["oninput"] = "syncJournalMasterSelection()";
            journal_name.Attributes["onchange"] = "syncJournalMasterSelection()";
            conference_name.Attributes["list"] = "conferenceMasterList";
            conference_name.Attributes["oninput"] = "syncConferenceMasterSelection()";
            conference_name.Attributes["onchange"] = "syncConferenceMasterSelection()";
        }

        private void LoadMasterDataOptions()
        {
            List<MasterOption> institutions = LoadInstitutionOptions();
            List<MasterOption> journals = LoadNamedOptions("Journal", "name_cn", "name_en", "status=1");
            List<MasterOption> conferences = LoadNamedOptions("Conference", "name_cn", "name_en", "status=1", "acronym");

            InstitutionDatalistHtml = BuildDatalistHtml(institutions);
            JournalDatalistHtml = BuildDatalistHtml(journals);
            ConferenceDatalistHtml = BuildDatalistHtml(conferences);
            InstitutionOptionsJson = BuildOptionsJson(institutions);
            JournalOptionsJson = BuildOptionsJson(journals);
            ConferenceOptionsJson = BuildOptionsJson(conferences);
        }

        private List<MasterOption> LoadInstitutionOptions()
        {
            List<MasterOption> result = new List<MasterOption>();
            try
            {
                DataTable dt = literatureBll.GetDatatable("select top 500 id,name_cn,name_en,alias_names from Institution where status=1 order by updatetime desc,id desc");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        int id = Function.ConvertTo<int>(Convert.ToString(row["id"]), 0);
                        AddMasterOption(result, id, Function.HtmlDiscode(Convert.ToString(row["name_cn"])));
                        AddMasterOption(result, id, Function.HtmlDiscode(Convert.ToString(row["name_en"])));
                        foreach (string alias in SplitMasterNames(Function.HtmlDiscode(Convert.ToString(row["alias_names"]))))
                        {
                            AddMasterOption(result, id, alias);
                        }
                    }
                    dt.Dispose();
                }
            }
            catch
            {
            }
            return result;
        }

        private List<MasterOption> LoadNamedOptions(string tableName, string cnColumn, string enColumn, string where, string extraColumn = "")
        {
            List<MasterOption> result = new List<MasterOption>();
            try
            {
                string fields = "id," + cnColumn + "," + enColumn + (string.IsNullOrWhiteSpace(extraColumn) ? "" : "," + extraColumn);
                DataTable dt = literatureBll.GetDatatable("select top 500 " + fields + " from " + tableName + " where " + where + " order by updatetime desc,id desc");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        int id = Function.ConvertTo<int>(Convert.ToString(row["id"]), 0);
                        AddMasterOption(result, id, Function.HtmlDiscode(Convert.ToString(row[cnColumn])));
                        AddMasterOption(result, id, Function.HtmlDiscode(Convert.ToString(row[enColumn])));
                        if (!string.IsNullOrWhiteSpace(extraColumn))
                        {
                            AddMasterOption(result, id, Function.HtmlDiscode(Convert.ToString(row[extraColumn])));
                        }
                    }
                    dt.Dispose();
                }
            }
            catch
            {
            }
            return result;
        }

        private void AddMasterOption(List<MasterOption> options, int id, string name)
        {
            string clean = NormalizePlainText(name);
            if (id <= 0 || string.IsNullOrWhiteSpace(clean))
            {
                return;
            }
            string key = NormalizeMasterName(clean);
            for (int i = 0; i < options.Count; i++)
            {
                if (NormalizeMasterName(options[i].name) == key)
                {
                    return;
                }
            }
            options.Add(new MasterOption { id = id, name = clean });
        }

        private string BuildDatalistHtml(List<MasterOption> options)
        {
            StringBuilder html = new StringBuilder();
            foreach (MasterOption option in options)
            {
                html.Append("<option value=\"").Append(Server.HtmlEncode(option.name)).Append("\"></option>");
            }
            return html.ToString();
        }

        private string BuildOptionsJson(List<MasterOption> options)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(options).Replace("<", "\\u003c").Replace(">", "\\u003e").Replace("&", "\\u0026");
        }

        protected void EditFunc()
        {
            Txt_Title.Text = "\u7F16\u8F91\u6587\u732E";
            int id = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            Literature literature = literatureBll.SelectSingle("id=" + id + " and status<>-1");
            if (literature == null || literature.id <= 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u672A\u627E\u5230\u5BF9\u5E94\u7684\u6587\u732E\u8BB0\u5F55!", "Admin_LiteratureList.aspx?MenuId=" + MenuId, 1);
                return;
            }

            duplicateMasterNoticeHtml = GetDuplicateMasterNoticeHtml(literature.remark);
            title.Text = Function.HtmlDiscode(literature.title);
            subtitle.Text = Function.HtmlDiscode(literature.subtitle);
            author_names.Text = LiteratureRelationSync.GetAuthorNames(literature.id);
            institution.Text = Function.HtmlDiscode(literature.institution);
            AuthorAffiliationEditorHtml = BuildAuthorAffiliationEditorHtml(literature.id);
            author_details_payload.Value = BuildAuthorDetailsJson(literature.id);
            doi.Text = Function.HtmlDiscode(literature.doi);
            download_points.Text = literature.download_points.ToString();
            publish_year.Text = literature.publish_year.HasValue ? literature.publish_year.Value.ToString() : string.Empty;
            publish_month.Text = literature.publish_month.HasValue ? literature.publish_month.Value.ToString() : string.Empty;
            publish_day.Text = literature.publish_day.HasValue ? literature.publish_day.Value.ToString() : string.Empty;
            SetDropDownValue(source_type, literature.source_type, "\u5176\u4ED6");
            SetDropDownValue(category_id, literature.category_id.ToString(), "0");
            journal_name.Text = Function.HtmlDiscode(literature.journal_name);
            journal_id_payload.Value = literature.journal_id.HasValue ? literature.journal_id.Value.ToString() : string.Empty;
            conference_name.Text = Function.HtmlDiscode(literature.conference_name);
            conference_id_payload.Value = literature.conference_id.HasValue ? literature.conference_id.Value.ToString() : string.Empty;
            volume.Text = Function.HtmlDiscode(literature.volume);
            issue.Text = Function.HtmlDiscode(literature.issue);
            pages.Text = Function.HtmlDiscode(literature.pages);
            publisher.Text = Function.HtmlDiscode(literature.publisher);
            language.Text = Function.HtmlDiscode(literature.language);
            keywords.Text = Function.HtmlDiscode(literature.keywords);
            string mappedTagNames = LiteratureRelationSync.GetTagNames(literature.id);
            tag_names.Text = mappedTagNames;
            BindSelectedTags(Function.HtmlEncode(mappedTagNames));
            abstract_text.Text = Function.HtmlDiscode(literature.abstract_text);
            external_url.Text = Function.HtmlDiscode(literature.external_url);
            source_db.Text = Function.HtmlDiscode(literature.source_db);
            remark.Text = Function.HtmlDiscode(literature.remark);
            is_top.Checked = literature.is_top == 1;
            SetDropDownValue(status, literature.status.ToString(), "0");

            cover_pic_old.Value = literature.cover_pic;
            if (!string.IsNullOrWhiteSpace(literature.cover_pic))
            {
                cover_pic_img.ImageUrl = Function.GetAdminUpload_Pic(literature.cover_pic);
            }

            LiteratureFile primaryFile = LiteratureRelationSync.GetPrimaryFile(literature.id);
            string currentPdfFile = primaryFile != null && primaryFile.id > 0 ? primaryFile.file_path : string.Empty;
            string currentPdfName = primaryFile != null && primaryFile.id > 0 ? primaryFile.file_name : string.Empty;
            pdf_file_old.Value = currentPdfFile;
            pdf_name_old.Value = currentPdfName;
            if (!string.IsNullOrWhiteSpace(currentPdfName))
            {
                pdf_file_name.Text = "\u5F53\u524D\u9644\u4EF6\uFF1A" + Function.HtmlDiscode(currentPdfName);
            }
        }

        protected void OnClick_AddUp(object sender, EventArgs e)
        {
            isLoading = false;
            string backUrl = Request.QueryString["BackURL"];
            if (string.IsNullOrWhiteSpace(backUrl))
            {
                backUrl = "Admin_LiteratureList.aspx?MenuId=" + MenuId;
            }

            if (string.IsNullOrWhiteSpace(title.Text))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u6587\u732E\u6807\u9898\u4E0D\u80FD\u4E3A\u7A7A!", backUrl, 2);
                return;
            }

            int id = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            Literature literature = Action == "Edit" ? literatureBll.SelectSingle("id=" + id + " and status<>-1") : new Literature();
            if (Action == "Edit" && (literature == null || literature.id <= 0))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u7F16\u8F91\u7684\u6587\u732E\u4E0D\u5B58\u5728!", backUrl, 2);
                return;
            }

            int oldStatus = Action == "Edit" ? literature.status : -1;
            string safeTitle = Function.HtmlEncode(title.Text.Trim());
            string where = "title='" + safeTitle + "' and status<>-1";
            if (Action == "Edit")
            {
                where += " and id not in(" + id + ")";
            }
            bool isSpecialReviewRecord = Action == "Edit" && (GetDuplicateMasterId(literature.remark) > 0 || GetMetadataRevisionMasterId(literature.remark) > 0 || literature.status == 3 || literature.status == 4);
            if (!isSpecialReviewRecord && literatureBll.Exists(where))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u6587\u732E\u6807\u9898\u300A" + Function.HtmlDiscode(safeTitle) + "\u300B\u5DF2\u5B58\u5728!", backUrl, 2);
                return;
            }

            literature.title = safeTitle;
            literature.subtitle = Function.HtmlEncode(subtitle.Text.Trim());
            literature.institution = LiteratureRelationSync.EncodeForColumn(institution.Text, 500);
            literature.doi = Function.HtmlEncode(doi.Text.Trim());
            literature.download_points = Function.ConvertTo<int>(download_points.Text.Trim(), 0);
            if (literature.download_points < 0)
            {
                literature.download_points = 0;
            }
            literature.keywords = Function.HtmlEncode(keywords.Text.Trim());
            literature.abstract_text = Function.HtmlEncode(abstract_text.Text.Trim());
            literature.source_type = Function.HtmlEncode(source_type.SelectedValue);
            literature.language = Function.HtmlEncode(language.Text.Trim());
            string publishDateError = ApplyPublicationDate(literature, publish_year.Text, publish_month.Text, publish_day.Text);
            if (!string.IsNullOrWhiteSpace(publishDateError))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), publishDateError, backUrl, 2);
                return;
            }
            literature.journal_name = Function.HtmlEncode(journal_name.Text.Trim());
            literature.journal_id = ResolveJournalId(journal_name.Text.Trim(), journal_id_payload.Value);
            literature.conference_name = Function.HtmlEncode(conference_name.Text.Trim());
            literature.conference_id = ResolveConferenceId(conference_name.Text.Trim(), conference_id_payload.Value);
            literature.publisher = Function.HtmlEncode(publisher.Text.Trim());
            literature.volume = Function.HtmlEncode(volume.Text.Trim());
            literature.issue = Function.HtmlEncode(issue.Text.Trim());
            literature.pages = Function.HtmlEncode(pages.Text.Trim());
            literature.category_id = Function.ConvertTo<int>(category_id.SelectedValue, 0);
            string submittedAuthorNames = author_names.Text.Trim();
            string submittedTagNames = BuildTagNames();
            literature.is_top = is_top.Checked ? 1 : 0;
            literature.status = Function.ConvertTo<int>(status.SelectedValue, 0);
            literature.external_url = Function.HtmlEncode(external_url.Text.Trim());
            literature.source_db = Function.HtmlEncode(source_db.Text.Trim());
            literature.remark = BuildRemark(literature.status, remark.Text);
            int duplicateMasterId = Action == "Edit" ? GetDuplicateMasterId(literature.remark) : 0;
            int metadataMasterId = Action == "Edit" ? GetMetadataRevisionMasterId(literature.remark) : 0;
            bool applyMetadataRevision = metadataMasterId > 0 && literature.status == 1;
            if (duplicateMasterId > 0 && literature.status == 1)
            {
                literature.status = 3;
                literature.remark = Function.HtmlEncode("\u91CD\u590D\u6295\u7A3F\u5BA1\u6838\u901A\u8FC7\uFF0C\u5171\u7528\u6587\u732EID:" + duplicateMasterId + "\u7684\u8BE6\u60C5\u9875");
            }
            if (applyMetadataRevision)
            {
                literature.status = 0;
            }
            UpdateReviewAudit(literature, oldStatus);
            if (Action != "Edit")
            {
                literature.userid = 0;
            }
            if (Action != "Edit")
            {
                literature.addtime = DateTime.Now;
            }
            literature.updatetime = DateTime.Now;

            if (cover_pic.HasFile && cover_pic.PostedFile.ContentLength > UploadPolicy.MaxImageBytes)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "封面图片不能超过 " + UploadPolicy.ToMbLabel(UploadPolicy.MaxImageBytes) + "！", backUrl, 2);
                return;
            }
            if (pdf_file.HasFile && pdf_file.PostedFile.ContentLength > UploadPolicy.MaxAttachmentBytes)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "文献附件不能超过 " + UploadPolicy.ToMbLabel(UploadPolicy.MaxAttachmentBytes) + "！", backUrl, 2);
                return;
            }

            HandleCoverUpload(literature);
            string submittedPdfFile;
            string submittedPdfName;
            HandlePdfUpload(out submittedPdfFile, out submittedPdfName);

            bool success;
            if (Action == "Edit")
            {
                success = literatureBll.Update(new[] { "id" }, literature);
            }
            else
            {
                int savedId = Convert.ToInt32(literatureBll.AddIdentity(literature, "id"));
                literature.id = savedId;
                success = savedId > 0;
            }

            if (success)
            {
                LiteratureRelationSync.Sync(literature, submittedAuthorNames, submittedTagNames, submittedPdfFile, submittedPdfName, author_details_payload.Value);
                if (literature.status == 1)
                {
                    LiteratureVenueProfileSync.EnsureForLiterature(literature);
                    LiteratureRagSync.QueueReindex(literature.id);
                }
                if (applyMetadataRevision && !ApplyMetadataRevisionFromEdit(literature, metadataMasterId))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u5143\u6570\u636E\u4FEE\u6539\u4FDD\u5B58\u6210\u529F\uFF0C\u4F46\u5E94\u7528\u5230\u539F\u6587\u732E\u5931\u8D25\uFF0C\u8BF7\u91CD\u8BD5!", backUrl, 2);
                    return;
                }
                if (Action == "Edit" && oldStatus == 1 && literature.status == 1)
                {
                    AddAdminEditNotice(literature);
                }
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u6587\u732E\u300A" + Function.HtmlDiscode(literature.title) + "\u300B\u4FDD\u5B58\u6210\u529F!", backUrl, 0);
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u6587\u732E\u300A" + Function.HtmlDiscode(literature.title) + "\u300B\u4FDD\u5B58\u5931\u8D25!", backUrl, 2);
            }
        }

        private void AddAdminEditNotice(Literature literature)
        {
            if (literature == null || literature.userid <= 0)
            {
                return;
            }

            NoticeLog_List notice = new NoticeLog_List();
            notice.userid = literature.userid;
            notice.type = 1;
            notice.status = 0;
            notice.addtime = DateTime.Now;
            notice.url = "/LiteratureInfo.aspx?id=" + literature.id;
            notice.name = Function.HtmlEncode("[文献管理] 文献信息已更新");
            notice.info_ = Function.HtmlEncode("管理员更新了您的文献《" + Function.HtmlDiscode(literature.title) + "》的元数据或附件信息，请进入详情页查看。");
            noticeLogBll.Add(notice, "id");
        }

        private string ApplyPublicationDate(Literature literature, string yearText, string monthText, string dayText)
        {
            int year = Function.ConvertTo<int>((yearText ?? string.Empty).Trim(), 0);
            int month = Function.ConvertTo<int>((monthText ?? string.Empty).Trim(), 0);
            int day = Function.ConvertTo<int>((dayText ?? string.Empty).Trim(), 0);

            if (year <= 0)
            {
                if (month > 0 || day > 0)
                {
                    return "填写发表月份或日期时必须同时填写发表年份。";
                }
                literature.publish_year = null;
                literature.publish_month = null;
                literature.publish_day = null;
                literature.publish_date = null;
                literature.publish_date_precision = "unknown";
                return string.Empty;
            }

            if (year < 1000 || year > 9999)
            {
                return "发表年份格式不正确。";
            }
            if (month < 0 || month > 12)
            {
                return "发表月份必须在 1-12 之间。";
            }
            if (month == 0 && day > 0)
            {
                return "填写发表日期时必须同时填写发表月份。";
            }
            if (day < 0 || day > 31)
            {
                return "发表日期格式不正确。";
            }

            literature.publish_year = year;
            literature.publish_month = month > 0 ? (int?)month : null;
            literature.publish_day = null;
            literature.publish_date_precision = "year";
            literature.publish_date = new DateTime(year, 12, 31);

            if (month > 0)
            {
                int maxDay = DateTime.DaysInMonth(year, month);
                if (day > maxDay)
                {
                    return "发表日期超过该月份最大天数。";
                }
                literature.publish_date_precision = "month";
                literature.publish_date = new DateTime(year, month, maxDay);
                if (day > 0)
                {
                    literature.publish_day = day;
                    literature.publish_date_precision = "day";
                    literature.publish_date = new DateTime(year, month, day);
                }
            }

            return string.Empty;
        }

        private string BuildAuthorAffiliationEditorHtml(int literatureId)
        {
            DataTable dt = literatureBll.GetDatatable(@"
select
    m.author_id,
    coalesce(nullif(a.name_cn,N''),nullif(a.name_en,N'')) as author_name,
    m.affiliation_text,
    (
        select string_agg(coalesce(nullif(i.name_cn,N''),nullif(i.name_en,N''),nullif(aim.affiliation_text,N'')),N'；') within group (order by aim.institution_order, aim.id)
        from LiteratureAuthorInstitutionMap aim
        left join Institution i on i.id=aim.institution_id and i.status<>-1
        where aim.literature_author_map_id=m.id
           or (isnull(aim.literature_author_map_id,0)=0 and aim.literature_id=m.literature_id and aim.author_id=m.author_id)
    ) as institution_names
from LiteratureAuthorMap m
inner join Author a on a.id=m.author_id
where m.literature_id=" + literatureId + @"
order by m.author_order asc,m.id asc");
            if (dt == null || dt.Rows.Count == 0)
            {
                return "<div class=\"lit-author-affiliation-hint\">暂无作者机构归属，请先填写作者或解析 PDF。</div>";
            }

            StringBuilder html = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                string name = Function.HtmlDiscode(Convert.ToString(row["author_name"]));
                string affiliation = Function.HtmlDiscode(Convert.ToString(row["institution_names"]));
                if (string.IsNullOrWhiteSpace(affiliation))
                {
                    affiliation = Function.HtmlDiscode(Convert.ToString(row["affiliation_text"]));
                }
                html.Append("<div class=\"lit-author-affiliation-row\" data-author-id=\"");
                html.Append(Function.ConvertTo<int>(Convert.ToString(row["author_id"]), 0));
                html.Append("\"><input type=\"text\" data-author-name=\"1\" value=\"");
                html.Append(Server.HtmlEncode(name));
                html.Append("\" placeholder=\"作者姓名\" /><input type=\"text\" data-author-affiliation-picker=\"1\" list=\"institutionMasterList\" placeholder=\"从机构库选择（可选）\" /><textarea data-author-affiliation=\"1\" placeholder=\"可直接输入该作者在本文中的机构；多个机构用分号分隔\">");
                html.Append(Server.HtmlEncode(affiliation));
                html.Append("</textarea></div>");
            }
            dt.Dispose();
            return html.ToString();
        }

        private string BuildAuthorDetailsJson(int literatureId)
        {
            DataTable dt = literatureBll.GetDatatable(@"
select
    m.author_id,
    coalesce(nullif(a.name_cn,N''),nullif(a.name_en,N'')) as author_name,
    a.name_cn,
    a.name_en,
    m.affiliation_text,
    (
        select string_agg(coalesce(nullif(i.name_cn,N''),nullif(i.name_en,N''),nullif(aim.affiliation_text,N'')),N'；') within group (order by aim.institution_order, aim.id)
        from LiteratureAuthorInstitutionMap aim
        left join Institution i on i.id=aim.institution_id and i.status<>-1
        where aim.literature_author_map_id=m.id
           or (isnull(aim.literature_author_map_id,0)=0 and aim.literature_id=m.literature_id and aim.author_id=m.author_id)
    ) as institution_names
from LiteratureAuthorMap m
inner join Author a on a.id=m.author_id
where m.literature_id=" + literatureId + @"
order by m.author_order asc,m.id asc");
            List<Dictionary<string, object>> details = new List<Dictionary<string, object>>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string name = Function.HtmlDiscode(Convert.ToString(row["author_name"]));
                    string affiliationText = Function.HtmlDiscode(Convert.ToString(row["institution_names"]));
                    if (string.IsNullOrWhiteSpace(affiliationText))
                    {
                        affiliationText = Function.HtmlDiscode(Convert.ToString(row["affiliation_text"]));
                    }
                    List<string> affiliations = SplitAffiliations(affiliationText);
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    item["author_id"] = Function.ConvertTo<int>(Convert.ToString(row["author_id"]), 0);
                    item["name"] = name;
                    item["name_cn"] = Function.HtmlDiscode(Convert.ToString(row["name_cn"]));
                    item["name_en"] = Function.HtmlDiscode(Convert.ToString(row["name_en"]));
                    item["affiliations"] = affiliations;
                    item["affiliation_text"] = string.Join("; ", affiliations.ToArray());
                    item["mapping_status"] = affiliations.Count > 0 ? "matched" : "unmatched";
                    details.Add(item);
                }
                dt.Dispose();
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(details);
        }

        private List<string> SplitAffiliations(string value)
        {
            List<string> values = new List<string>();
            foreach (string part in (value ?? string.Empty).Split(new[] { ';', '\uFF1B', '|', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string current = part.Trim();
                if (!string.IsNullOrWhiteSpace(current) && !values.Contains(current))
                {
                    values.Add(current);
                }
            }
            return values;
        }

        private int? ResolveJournalId(string journalName, string submittedId)
        {
            string cleanName = NormalizePlainText(journalName);
            if (string.IsNullOrWhiteSpace(cleanName))
            {
                return null;
            }

            int selectedId = Function.ConvertTo<int>(submittedId, 0);
            Journal selected = selectedId > 0 ? journalBll.SelectSingle("id=" + selectedId + " and status<>-1") : null;
            if (selected != null && selected.id > 0)
            {
                return selected.id;
            }

            string normalized = NormalizeMasterName(cleanName);
            Journal exists = journalBll.SelectSingle("status<>-1 and normalized_name=N'" + EncodeSql(normalized) + "'");
            if (exists != null && exists.id > 0)
            {
                return exists.id;
            }

            bool chinese = ContainsChinese(cleanName);
            Journal model = new Journal
            {
                name_cn = chinese ? Function.HtmlEncode(cleanName) : string.Empty,
                name_en = chinese ? string.Empty : Function.HtmlEncode(cleanName),
                normalized_name = normalized,
                issn = string.Empty,
                eissn = string.Empty,
                publisher = Function.HtmlEncode(publisher.Text.Trim()),
                country = string.Empty,
                subject = string.Empty,
                website = string.Empty,
                status = 1,
                addtime = DateTime.Now,
                updatetime = DateTime.Now
            };
            int id = Function.ConvertTo<int>(journalBll.AddIdentity(model, "id"), 0);
            return id > 0 ? (int?)id : null;
        }

        private int? ResolveConferenceId(string conferenceName, string submittedId)
        {
            string cleanName = NormalizePlainText(conferenceName);
            if (string.IsNullOrWhiteSpace(cleanName))
            {
                return null;
            }

            int selectedId = Function.ConvertTo<int>(submittedId, 0);
            Conference selected = selectedId > 0 ? conferenceBll.SelectSingle("id=" + selectedId + " and status<>-1") : null;
            if (selected != null && selected.id > 0)
            {
                return selected.id;
            }

            string normalized = NormalizeMasterName(cleanName);
            Conference exists = conferenceBll.SelectSingle("status<>-1 and normalized_name=N'" + EncodeSql(normalized) + "'");
            if (exists != null && exists.id > 0)
            {
                return exists.id;
            }

            bool chinese = ContainsChinese(cleanName);
            Conference model = new Conference
            {
                name_cn = chinese ? Function.HtmlEncode(cleanName) : string.Empty,
                name_en = chinese ? string.Empty : Function.HtmlEncode(cleanName),
                acronym = string.Empty,
                normalized_name = normalized,
                organizer = Function.HtmlEncode(publisher.Text.Trim()),
                country = string.Empty,
                city = string.Empty,
                start_date = null,
                end_date = null,
                website = string.Empty,
                status = 1,
                addtime = DateTime.Now,
                updatetime = DateTime.Now
            };
            int id = Function.ConvertTo<int>(conferenceBll.AddIdentity(model, "id"), 0);
            return id > 0 ? (int?)id : null;
        }

        private IEnumerable<string> SplitMasterNames(string value)
        {
            foreach (string part in Regex.Split(value ?? string.Empty, @"[;\uFF1B|\r\n]+"))
            {
                string clean = NormalizePlainText(part);
                if (!string.IsNullOrWhiteSpace(clean))
                {
                    yield return clean;
                }
            }
        }

        private string NormalizePlainText(string value)
        {
            return Regex.Replace(Function.HtmlDiscode(value ?? string.Empty).Replace('\u00A0', ' '), @"\s+", " ").Trim();
        }

        private string NormalizeMasterName(string value)
        {
            return NormalizePlainText(value).ToLowerInvariant();
        }

        private bool ContainsChinese(string value)
        {
            return Regex.IsMatch(value ?? string.Empty, @"[\u3400-\u9fff\uf900-\ufaff]");
        }

        private string EncodeSql(string value)
        {
            return Function.HtmlEncode(value ?? string.Empty).Replace("'", "''");
        }

        private void HandleCoverUpload(Literature literature)
        {
            string oldPath = cover_pic_old.Value;
            literature.cover_pic = oldPath;
            if (del_cover_pic.Checked)
            {
                DeleteUploadFile(oldPath, "upload_pic");
                literature.cover_pic = string.Empty;
            }

            if (cover_pic.HasFile)
            {
                string newPath = SaveUploadFile(cover_pic, "upload_pic");
                if (!string.IsNullOrWhiteSpace(newPath))
                {
                    DeleteUploadFile(oldPath, "upload_pic");
                    literature.cover_pic = newPath;
                }
            }
        }

        private string BuildRemark(int currentStatus, string inputRemark)
        {
            string cleanRemark = (inputRemark ?? string.Empty).Trim();
            if (currentStatus == 1 && IsSystemReviewRemark(cleanRemark))
            {
                cleanRemark = "\u5BA1\u6838\u901A\u8FC7";
            }
            else if (currentStatus == 2 && IsSystemReviewRemark(cleanRemark))
            {
                cleanRemark = "\u8BF7\u4FEE\u6539\u540E\u91CD\u65B0\u63D0\u4EA4\u5BA1\u6838";
            }

            return Function.HtmlEncode(cleanRemark);
        }

        private void UpdateReviewAudit(Literature literature, int oldStatus)
        {
            if (literature.status == 1 || literature.status == 2 || literature.status == 3)
            {
                if (oldStatus != literature.status || !literature.reviewed_by.HasValue || !literature.review_time.HasValue)
                {
                    literature.reviewed_by = Function.ConvertTo<int>(Cookie.GetCookie("LMS_AdminID"), 0);
                    literature.review_time = DateTime.Now;
                }
            }
            else
            {
                literature.reviewed_by = null;
                literature.review_time = null;
            }
        }

        private bool IsSystemReviewRemark(string cleanRemark)
        {
            return string.IsNullOrWhiteSpace(cleanRemark)
                || cleanRemark == "\u7528\u6237\u524D\u53F0\u63D0\u4EA4\u5F85\u5BA1\u6838"
                || cleanRemark == "\u5BA1\u6838\u901A\u8FC7"
                || cleanRemark == "\u8BF7\u4FEE\u6539\u540E\u91CD\u65B0\u63D0\u4EA4\u5BA1\u6838";
        }

        private string GetDuplicateMasterNoticeHtml(string remarkValue)
        {
            int masterId = GetDuplicateMasterId(remarkValue);
            if (masterId <= 0)
            {
                int metadataMasterId = GetMetadataRevisionMasterId(remarkValue);
                if (metadataMasterId <= 0)
                {
                    return string.Empty;
                }
                return "<div class=\"alert alert-info\" style=\"margin: 12px 0;\">\u8FD9\u662F\u5143\u6570\u636E\u4FEE\u6539\u5F85\u5BA1\u6838\u8BB0\u5F55\uFF0C\u5BA1\u6838\u901A\u8FC7\u540E\u4F1A\u5E94\u7528\u5230\u539F\u6587\u732E\u3002 <a class=\"btn btn-sm btn-info\" href=\"Admin_LiteratureInfo.aspx?MenuId=" + MenuId + "&ID=" + metadataMasterId + "&BackURL=" + Function.GetEncodeURL() + "\">\u67E5\u770B\u539F\u6587\u732E</a></div>";
            }

            return "<div class=\"alert alert-info\" style=\"margin: 12px 0;\">\u8FD9\u662F\u91CD\u590D\u6295\u7A3F\u5F85\u5BA1\u6838\u8BB0\u5F55\uFF0C\u5BA1\u6838\u901A\u8FC7\u540E\u5C06\u5171\u7528\u5DF2\u6709\u6587\u732E\u8BE6\u60C5\u9875\u3002 <a class=\"btn btn-sm btn-info\" href=\"Admin_LiteratureInfo.aspx?MenuId=" + MenuId + "&ID=" + masterId + "&BackURL=" + Function.GetEncodeURL() + "\">\u67E5\u770B\u5DF2\u6709\u76F8\u540C\u6587\u732E</a></div>";
        }

        private int GetDuplicateMasterId(string remarkValue)
        {
            string cleanRemark = Function.HtmlDiscode(remarkValue ?? string.Empty).Trim();
            if (!cleanRemark.StartsWith(DuplicateSubmissionRemarkPrefix, StringComparison.Ordinal))
            {
                return 0;
            }

            string idText = cleanRemark.Substring(DuplicateSubmissionRemarkPrefix.Length);
            int separatorIndex = idText.IndexOfAny(new[] { '\uFF1B', ';', ' ', '\r', '\n' });
            if (separatorIndex >= 0)
            {
                idText = idText.Substring(0, separatorIndex);
            }
            return Function.ConvertTo<int>(idText, 0);
        }

        private int GetMetadataRevisionMasterId(string remarkValue)
        {
            string cleanRemark = Function.HtmlDiscode(remarkValue ?? string.Empty).Trim();
            if (!cleanRemark.StartsWith(MetadataRevisionRemarkPrefix, StringComparison.Ordinal))
            {
                return 0;
            }

            string idText = cleanRemark.Substring(MetadataRevisionRemarkPrefix.Length);
            int separatorIndex = idText.IndexOfAny(new[] { '\uFF1B', ';', ' ', '\r', '\n' });
            if (separatorIndex >= 0)
            {
                idText = idText.Substring(0, separatorIndex);
            }
            return Function.ConvertTo<int>(idText, 0);
        }

        private bool ApplyMetadataRevisionFromEdit(Literature revision, int masterId)
        {
            Literature master = literatureBll.SelectSingle("id=" + masterId + " and status<>-1");
            if (master == null || master.id <= 0)
            {
                return false;
            }

            master.title = revision.title;
            master.subtitle = revision.subtitle;
            master.institution = revision.institution;
            master.doi = revision.doi;
            master.keywords = revision.keywords;
            master.abstract_text = revision.abstract_text;
            master.source_type = revision.source_type;
            master.language = revision.language;
            master.publish_year = revision.publish_year;
            master.publish_month = revision.publish_month;
            master.publish_day = revision.publish_day;
            master.publish_date = revision.publish_date;
            master.publish_date_precision = revision.publish_date_precision;
            master.journal_name = revision.journal_name;
            master.journal_id = revision.journal_id;
            master.conference_name = revision.conference_name;
            master.conference_id = revision.conference_id;
            master.publisher = revision.publisher;
            master.volume = revision.volume;
            master.issue = revision.issue;
            master.pages = revision.pages;
            master.updatetime = DateTime.Now;

            if (!literatureBll.Update(new[] { "id" }, master))
            {
                return false;
            }
            LiteratureVenueProfileSync.EnsureForLiterature(master);

            string revisionAuthors = LiteratureRelationSync.GetAuthorNames(revision.id);
            string masterTags = LiteratureRelationSync.GetTagNames(master.id);
            LiteratureRelationSync.SyncMetadata(master, revisionAuthors, masterTags, BuildAuthorDetailsJson(revision.id));

            int adminId = Function.ConvertTo<int>(Cookie.GetCookie("LMS_AdminID"), 0);
            string updateRevisionSql = "status=4,reviewed_by=" + adminId + ",review_time=GETDATE(),updatetime=GETDATE(),remark=N'\u5143\u6570\u636E\u4FEE\u6539\u5DF2\u5BA1\u6838\u901A\u8FC7\u5E76\u5E94\u7528\u5230\u6587\u732EID:" + masterId + "\u3002'";
            bool revisionUpdated = literatureBll.Update(updateRevisionSql, "id=" + revision.id);
            if (revisionUpdated)
            {
                LiteratureRagSync.QueueReindex(master.id);
            }
            return revisionUpdated;
        }

        private void HandlePdfUpload(out string pdfFilePath, out string pdfFileName)
        {
            string oldPath = pdf_file_old.Value;
            string oldName = pdf_name_old.Value;
            pdfFilePath = oldPath;
            pdfFileName = oldName;
            if (del_pdf_file.Checked)
            {
                DeleteUploadFile(oldPath, "upload_file");
                pdfFilePath = string.Empty;
                pdfFileName = string.Empty;
            }

            if (pdf_file.HasFile)
            {
                string newPath = SaveUploadFile(pdf_file, "upload_file");
                if (!string.IsNullOrWhiteSpace(newPath))
                {
                    DeleteUploadFile(oldPath, "upload_file");
                    pdfFilePath = newPath;
                    pdfFileName = Path.GetFileName(pdf_file.FileName);
                }
            }
        }

        private string SaveUploadFile(System.Web.UI.WebControls.FileUpload upload, string folderName)
        {
            string extension = Path.GetExtension(upload.FileName).ToLower();
            string dateFolder = DateTime.Now.ToString("yyyyMMdd") + "/";
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff") + extension;
            string relativePath = dateFolder + fileName;
            string baseDirectory = Server.MapPath("../A_UpLoad/" + folderName + "/");
            string saveDirectory = Path.Combine(baseDirectory, dateFolder);
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
            upload.SaveAs(Path.Combine(baseDirectory, relativePath));
            return relativePath.Replace("\\", "/");
        }

        private void DeleteUploadFile(string relativePath, string folderName)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return;
            }

            string fullPath = Server.MapPath("../A_UpLoad/" + folderName + "/" + relativePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        private void SetDropDownValue(System.Web.UI.WebControls.DropDownList ddl, string value, string fallback)
        {
            if (ddl.Items.FindByValue(value) != null)
            {
                ddl.SelectedValue = value;
            }
            else if (ddl.Items.FindByValue(fallback) != null)
            {
                ddl.SelectedValue = fallback;
            }
        }

        private void BindSelectedTags(string tagNames)
        {
            string[] values = (tagNames ?? string.Empty).Split(new[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (System.Web.UI.WebControls.ListItem item in TagList.Items)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (item.Text.Equals(values[i].Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
        }

        private string BuildTagNames()
        {
            StringBuilder sb = new StringBuilder();
            foreach (System.Web.UI.WebControls.ListItem item in TagList.Items)
            {
                if (item.Selected)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(",");
                    }
                    sb.Append(item.Text.Trim());
                }
            }

            string manualTags = tag_names.Text.Trim();
            if (!string.IsNullOrWhiteSpace(manualTags))
            {
                string[] arr = manualTags.Split(new[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < arr.Length; i++)
                {
                    string current = arr[i].Trim();
                    if (string.IsNullOrWhiteSpace(current))
                    {
                        continue;
                    }
                    if (("," + sb.ToString() + ",").IndexOf("," + current + ",", StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(",");
                        }
                        sb.Append(current);
                    }
                }
            }
            return sb.ToString();
        }

        private class MasterOption
        {
            public int id { get; set; }
            public string name { get; set; }
        }
    }
}
