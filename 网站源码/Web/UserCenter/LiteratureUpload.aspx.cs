using LiteratureManager.Common;
using BLL;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace Web.UserCenter
{
    public partial class LiteratureUpload : System.Web.UI.Page
    {
        private readonly BLLBase<Literature> literatureBll = new BLLBase<Literature>();
        private readonly BLLBase<LiteratureCategory> categoryBll = new BLLBase<LiteratureCategory>();
        private const string DuplicateSubmissionRemarkPrefix = "[重复投稿]关联文献ID:";

        protected void Page_Load(object sender, EventArgs e)
        {
            user_list user = CommonUserFunc.GetUserLoginStatus();
            if (user == null || user.id <= 0)
            {
                Response.Redirect("/");
                return;
            }

            if (!IsPostBack)
            {
                BindCategories();
            }
        }

        private void BindCategories()
        {
            category_id.Items.Clear();
            category_id.Items.Add(new System.Web.UI.WebControls.ListItem("\u672A\u5206\u7C7B", "0"));
            batch_category_id.Items.Clear();
            batch_category_id.Items.Add(new System.Web.UI.WebControls.ListItem("\u672A\u5206\u7C7B", "0"));
            DataTable dt = categoryBll.GetDatatable("select id,name from LiteratureCategory where status=1 order by orderid asc,id asc");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    category_id.Items.Add(new System.Web.UI.WebControls.ListItem(row["name"].ToString(), row["id"].ToString()));
                    batch_category_id.Items.Add(new System.Web.UI.WebControls.ListItem(row["name"].ToString(), row["id"].ToString()));
                }
            }
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            user_list user = CommonUserFunc.GetUserLoginStatus();
            if (user == null || user.id <= 0)
            {
                Function.Show_Msg("\u767B\u5F55\u72B6\u6001\u5F02\u5E38\uFF01", "/");
                return;
            }

            if (string.IsNullOrWhiteSpace(title.Text))
            {
                Function.Show_Msg("\u6587\u732E\u6807\u9898\u4E0D\u80FD\u4E3A\u7A7A\uFF01", "/User/LiteratureUpload");
                return;
            }

            if (!pdf_file.HasFile)
            {
                Function.Show_Msg("\u8BF7\u4E0A\u4F20 PDF \u9644\u4EF6\u540E\u518D\u63D0\u4EA4\uFF01", "/User/LiteratureUpload");
                return;
            }

            if (!IsAllowedPdf(pdf_file.PostedFile))
            {
                Function.Show_Msg("PDF 文件格式不正确或超过 " + UploadPolicy.ToMbLabel(UploadPolicy.MaxPdfBytes) + " 限制！", "/User/LiteratureUpload");
                return;
            }

            Literature literature = new Literature();
            literature.title = Function.HtmlEncode(title.Text.Trim());
            literature.institution = LiteratureRelationSync.EncodeForColumn(institution.Text, 500);
            literature.doi = Function.HtmlEncode(doi.Text.Trim());
            literature.download_points = 0;
            literature.source_type = Function.HtmlEncode(source_type.SelectedValue);
            literature.category_id = Function.ConvertTo<int>(category_id.SelectedValue, 0);
            literature.journal_name = Function.HtmlEncode(journal_name.Text.Trim());
            literature.conference_name = Function.HtmlEncode(conference_name.Text.Trim());
            literature.volume = Function.HtmlEncode(volume.Text.Trim());
            literature.issue = Function.HtmlEncode(issue.Text.Trim());
            literature.pages = Function.HtmlEncode(pages.Text.Trim());
            literature.publisher = Function.HtmlEncode(publisher.Text.Trim());
            string publishDateError = ApplyPublicationDate(literature, publish_year.Text, publish_month.Text, publish_day.Text);
            if (!string.IsNullOrWhiteSpace(publishDateError))
            {
                Function.Show_Msg(publishDateError, "/User/LiteratureUpload");
                return;
            }
            literature.keywords = Function.HtmlEncode(keywords.Text.Trim());
            literature.abstract_text = Function.HtmlEncode(abstract_text.Text.Trim());
            literature.cover_pic = string.Empty;
            Literature userDuplicate = FindDuplicateLiterature(title.Text.Trim(), doi.Text.Trim(), user.id);
            if (userDuplicate != null && userDuplicate.id > 0)
            {
                HandleOwnDuplicateSingleUpload(userDuplicate);
                return;
            }

            Literature platformDuplicate = FindDuplicateLiterature(title.Text.Trim(), doi.Text.Trim(), 0);
            int platformDuplicateMasterId = GetCanonicalLiteratureId(platformDuplicate);

            string uploadedPdfPath = SaveUploadFile(pdf_file, "upload_file");
            string uploadedPdfName = Path.GetFileName(pdf_file.FileName);
            literature.external_url = string.Empty;
            literature.source_db = string.Empty;
            literature.remark = Function.HtmlEncode("\u7528\u6237\u524D\u53F0\u63D0\u4EA4\u5F85\u5BA1\u6838");
            if (platformDuplicateMasterId > 0)
            {
                literature.canonical_literature_id = platformDuplicateMasterId;
                literature.remark = Function.HtmlEncode(DuplicateSubmissionRemarkPrefix + platformDuplicateMasterId + "\uFF1B\u7528\u6237\u63D0\u4EA4\u5E73\u53F0\u5DF2\u5B58\u5728\u6587\u732E\uFF0C\u5BA1\u6838\u901A\u8FC7\u540E\u5171\u7528\u539F\u6587\u732E\u8BE6\u60C5\u9875\u3002");
            }
            literature.is_top = 0;
            literature.status = 0;
            literature.userid = user.id;
            literature.addtime = DateTime.Now;
            literature.updatetime = DateTime.Now;

            int literatureId = Convert.ToInt32(literatureBll.AddIdentity(literature, "id"));
            if (literatureId > 0)
            {
                literature.id = literatureId;
                LiteratureRelationSync.Sync(literature, author_names.Text.Trim(), string.Empty, uploadedPdfPath, uploadedPdfName, author_details_payload.Value);
                LiteratureVenueProfileSync.EnsureForLiterature(literature);
                if (literature.status == 1)
                {
                    LiteratureRagSync.QueueReindex(literature.id);
                }
                string successMessage = platformDuplicateMasterId > 0
                    ? "\u5E73\u53F0\u5DF2\u5B58\u5728\u8FD9\u7BC7\u6587\u732E\uFF0C\u672C\u6B21\u63D0\u4EA4\u5DF2\u8FDB\u5165\u540E\u53F0\u5BA1\u6838\uFF0C\u5BA1\u6838\u901A\u8FC7\u540E\u5C06\u5171\u7528\u5DF2\u6709\u8BE6\u60C5\u9875\u3002"
                    : "\u6587\u732E\u5DF2\u63D0\u4EA4\uFF0C\u8BF7\u7B49\u5F85\u540E\u53F0\u5BA1\u6838\u901A\u8FC7\u540E\u5C55\u793A\uFF01";
                Function.Show_Msg(successMessage, "/User/LiteratureUpload?graph=1");
            }
            else
            {
                Function.Show_Msg("\u6587\u732E\u63D0\u4EA4\u5931\u8D25\uFF0C\u8BF7\u7A0D\u540E\u518D\u8BD5\uFF01", "/User/LiteratureUpload");
            }
        }

        protected void ButtonBatchUpload_Click(object sender, EventArgs e)
        {
            user_list user = CommonUserFunc.GetUserLoginStatus();
            if (user == null || user.id <= 0)
            {
                Function.Show_Msg("\u767B\u5F55\u72B6\u6001\u5F02\u5E38\uFF01", "/");
                return;
            }

            List<HttpPostedFile> batchFiles = GetBatchPostedFiles();
            if (batchFiles.Count == 0)
            {
                Function.Show_Msg("\u8BF7\u9009\u62E9\u9700\u8981\u6279\u91CF\u4E0A\u4F20\u7684 PDF \u6587\u4EF6\uFF01", "/User/LiteratureUpload");
                return;
            }

            if (batchFiles.Count > UploadPolicy.MaxBatchFiles)
            {
                Function.Show_Msg("单次批量上传最多支持 " + UploadPolicy.MaxBatchFiles + " 个 PDF！", "/User/LiteratureUpload");
                return;
            }

            long totalBytes = 0;
            foreach (HttpPostedFile postedFile in batchFiles)
            {
                if (!IsAllowedPdf(postedFile))
                {
                    Function.Show_Msg("批量上传中包含格式不正确或超过 " + UploadPolicy.ToMbLabel(UploadPolicy.MaxPdfBytes) + " 的 PDF！", "/User/LiteratureUpload");
                    return;
                }
                totalBytes += postedFile.ContentLength;
            }
            if (totalBytes > UploadPolicy.MaxBatchTotalBytes)
            {
                Function.Show_Msg("单次批量上传文件总大小不能超过 " + UploadPolicy.ToMbLabel(UploadPolicy.MaxBatchTotalBytes) + "！", "/User/LiteratureUpload");
                return;
            }

            int successCount = 0;
            int failCount = 0;
            int duplicateOwnCount = 0;
            int duplicatePendingCount = 0;
            Dictionary<string, BatchParsedPdf> parsedMap = GetBatchParsedMap(batch_parse_payload.Value);
            foreach (HttpPostedFile postedFile in batchFiles)
            {
                if (postedFile == null || postedFile.ContentLength <= 0)
                {
                    continue;
                }

                string extension = Path.GetExtension(postedFile.FileName).ToLower();
                if (extension != ".pdf")
                {
                    failCount++;
                    continue;
                }

                string uploadedPdfName = Path.GetFileName(postedFile.FileName);
                string titleFromFile = Path.GetFileNameWithoutExtension(uploadedPdfName);
                if (string.IsNullOrWhiteSpace(titleFromFile))
                {
                    titleFromFile = "\u672A\u547D\u540D PDF \u6587\u732E";
                }

                BatchParsedPdf parsed = FindParsedPdf(parsedMap, uploadedPdfName);
                string parsedTitle = parsed == null ? string.Empty : (parsed.title ?? string.Empty);
                string parsedAuthorNames = parsed == null ? string.Empty : (parsed.author_names ?? string.Empty);
                string parsedInstitution = parsed == null ? string.Empty : (parsed.institution ?? string.Empty);
                string parsedDoi = parsed == null ? string.Empty : (parsed.doi ?? string.Empty);
                string parsedSourceType = parsed == null ? string.Empty : (parsed.source_type ?? string.Empty);
                string parsedCategoryId = parsed == null ? string.Empty : (parsed.category_id ?? string.Empty);
                string parsedJournalName = parsed == null ? string.Empty : (parsed.journal_name ?? string.Empty);
                string parsedConferenceName = parsed == null ? string.Empty : (parsed.conference_name ?? string.Empty);
                string parsedPublishYear = parsed == null ? string.Empty : (parsed.publish_year ?? string.Empty);
                string parsedVolume = parsed == null ? string.Empty : (parsed.volume ?? string.Empty);
                string parsedIssue = parsed == null ? string.Empty : (parsed.issue ?? string.Empty);
                string parsedPages = parsed == null ? string.Empty : (parsed.pages ?? string.Empty);
                string parsedPublisher = parsed == null ? string.Empty : (parsed.publisher ?? string.Empty);
                string parsedKeywords = parsed == null ? string.Empty : (parsed.keywords ?? string.Empty);
                string parsedAbstract = parsed == null ? string.Empty : (parsed.abstract_text ?? string.Empty);
                string parsedPublishMonth = parsed == null ? string.Empty : (parsed.publish_month ?? string.Empty);
                string parsedPublishDay = parsed == null ? string.Empty : (parsed.publish_day ?? string.Empty);
                string parsedAuthorDetails = parsed == null || parsed.author_details == null ? string.Empty : parsed.author_details.ToString(Formatting.None);

                Literature literature = new Literature();
                literature.title = Function.HtmlEncode(string.IsNullOrWhiteSpace(parsedTitle) ? titleFromFile.Trim() : parsedTitle.Trim());
                literature.institution = LiteratureRelationSync.EncodeForColumn(parsedInstitution, 500);
                literature.doi = Function.HtmlEncode(parsedDoi.Trim());
                literature.download_points = 0;
                literature.source_type = Function.HtmlEncode(string.IsNullOrWhiteSpace(parsedSourceType) ? batch_source_type.SelectedValue : parsedSourceType.Trim());
                literature.category_id = Function.ConvertTo<int>(string.IsNullOrWhiteSpace(parsedCategoryId) ? batch_category_id.SelectedValue : parsedCategoryId, 0);
                literature.journal_name = Function.HtmlEncode(parsedJournalName.Trim());
                literature.conference_name = Function.HtmlEncode(parsedConferenceName.Trim());
                literature.volume = Function.HtmlEncode(parsedVolume.Trim());
                literature.issue = Function.HtmlEncode(parsedIssue.Trim());
                literature.pages = Function.HtmlEncode(parsedPages.Trim());
                literature.publisher = Function.HtmlEncode(parsedPublisher.Trim());
                string parsedPublishError = ApplyPublicationDate(literature, parsedPublishYear, parsedPublishMonth, parsedPublishDay);
                if (!string.IsNullOrWhiteSpace(parsedPublishError))
                {
                    failCount++;
                    continue;
                }
                literature.keywords = Function.HtmlEncode(parsedKeywords.Trim());
                literature.abstract_text = Function.HtmlEncode(parsedAbstract.Trim());
                literature.cover_pic = string.Empty;
                string duplicateTitle = Function.HtmlDiscode(literature.title);
                Literature userDuplicate = FindDuplicateLiterature(duplicateTitle, parsedDoi.Trim(), user.id);
                if (userDuplicate != null && userDuplicate.id > 0)
                {
                    duplicateOwnCount++;
                    continue;
                }

                Literature platformDuplicate = FindDuplicateLiterature(duplicateTitle, parsedDoi.Trim(), 0);
                int platformDuplicateMasterId = GetCanonicalLiteratureId(platformDuplicate);
                string uploadedPdfPath = SaveUploadFile(postedFile, "upload_file");

                literature.external_url = string.Empty;
                literature.source_db = string.Empty;
                literature.remark = Function.HtmlEncode("\u7528\u6237\u524D\u53F0\u6279\u91CF\u63D0\u4EA4\u5F85\u5BA1\u6838");
                if (platformDuplicateMasterId > 0)
                {
                    literature.canonical_literature_id = platformDuplicateMasterId;
                    literature.remark = Function.HtmlEncode(DuplicateSubmissionRemarkPrefix + platformDuplicateMasterId + "\uFF1B\u7528\u6237\u6279\u91CF\u63D0\u4EA4\u5E73\u53F0\u5DF2\u5B58\u5728\u6587\u732E\uFF0C\u5BA1\u6838\u901A\u8FC7\u540E\u5171\u7528\u539F\u6587\u732E\u8BE6\u60C5\u9875\u3002");
                }
                literature.is_top = 0;
                literature.status = 0;
                literature.userid = user.id;
                literature.addtime = DateTime.Now;
                literature.updatetime = DateTime.Now;

                int literatureId = Convert.ToInt32(literatureBll.AddIdentity(literature, "id"));
                if (literatureId > 0)
                {
                    literature.id = literatureId;
                    LiteratureRelationSync.Sync(literature, parsedAuthorNames, string.Empty, uploadedPdfPath, uploadedPdfName, parsedAuthorDetails);
                    LiteratureVenueProfileSync.EnsureForLiterature(literature);
                    if (literature.status == 1)
                    {
                        LiteratureRagSync.QueueReindex(literature.id);
                    }
                    if (platformDuplicateMasterId > 0)
                    {
                        duplicatePendingCount++;
                    }
                    successCount++;
                }
                else
                {
                    failCount++;
                }
            }

            if (successCount > 0)
            {
                string message = "\u6279\u91CF\u4E0A\u4F20\u5B8C\u6210\uFF0C\u6210\u529F\u63D0\u4EA4 " + successCount + " \u4E2A PDF";
                if (duplicateOwnCount > 0)
                {
                    message += "\uFF0C" + duplicateOwnCount + " \u4E2A\u4E3A\u60A8\u5DF2\u4E0A\u4F20\u7684\u91CD\u590D\u6587\u732E\uFF0C\u5DF2\u8DF3\u8FC7";
                }
                if (duplicatePendingCount > 0)
                {
                    message += "\uFF0C" + duplicatePendingCount + " \u4E2A\u4E3A\u5E73\u53F0\u5DF2\u6709\u6587\u732E\uFF0C\u5DF2\u4F5C\u4E3A\u91CD\u590D\u6295\u7A3F\u8FDB\u5165\u5F85\u5BA1\u6838";
                }
                if (failCount > 0)
                {
                    message += "\uFF0C" + failCount + " \u4E2A\u6587\u4EF6\u672A\u6210\u529F";
                }
                message += "\uFF0C\u8BF7\u7B49\u5F85\u540E\u53F0\u5BA1\u6838\uFF01";
                Function.Show_Msg(message, "/User/LiteratureUpload?graph=1");
            }
            else
            {
                string message = "\u672A\u80FD\u6210\u529F\u63D0\u4EA4 PDF\uFF0C\u8BF7\u786E\u8BA4\u6587\u4EF6\u683C\u5F0F\u540E\u91CD\u8BD5\uFF01";
                if (duplicateOwnCount > 0)
                {
                    message = "\u672C\u6B21\u9009\u62E9\u7684 PDF \u5747\u4E3A\u60A8\u5DF2\u4E0A\u4F20\u8FC7\u7684\u6587\u732E\uFF0C\u540C\u4E00\u7528\u6237\u4E0D\u80FD\u91CD\u590D\u4E0A\u4F20\u540C\u4E00\u7BC7\u6587\u7AE0\u3002";
                }
                Function.Show_Msg(message, "/User/LiteratureUpload");
            }
        }

        private void HandleOwnDuplicateSingleUpload(Literature duplicate)
        {
            int targetId = GetCanonicalLiteratureId(duplicate);
            string url = "/LiteratureInfo.aspx?id=" + (targetId > 0 ? targetId : duplicate.id);
            Function.Show_Msg("\u60A8\u5DF2\u4E0A\u4F20\u8FC7\u8FD9\u7BC7\u6587\u732E\uFF0C\u540C\u4E00\u7528\u6237\u4E0D\u80FD\u91CD\u590D\u4E0A\u4F20\u540C\u4E00\u7BC7\u6587\u7AE0\u3002", url);
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
            literature.publish_date = new DateTime(year, 12, 31);
            literature.publish_date_precision = "year";

            if (month > 0)
            {
                int maxDay = DateTime.DaysInMonth(year, month);
                if (day > maxDay)
                {
                    return "发表日期超过该月份最大天数。";
                }
                literature.publish_date = new DateTime(year, month, maxDay);
                literature.publish_date_precision = "month";
                if (day > 0)
                {
                    literature.publish_day = day;
                    literature.publish_date = new DateTime(year, month, day);
                    literature.publish_date_precision = "day";
                }
            }

            return string.Empty;
        }

        private Literature FindDuplicateLiterature(string rawTitle, string rawDoi, int userId)
        {
            string scopeCondition = userId > 0 ? "status<>-1 and userid=" + userId : "status in(1,3)";
            string doiKey = NormalizeDoi(rawDoi);
            if (!string.IsNullOrWhiteSpace(doiKey))
            {
                Literature byDoi = SelectDuplicateCandidate(scopeCondition + " and LOWER(REPLACE(REPLACE(LTRIM(RTRIM(ISNULL(doi,''))),'https://doi.org/',''),'http://dx.doi.org/',''))='" + EscapeSql(doiKey) + "'");
                if (byDoi != null && byDoi.id > 0)
                {
                    return byDoi;
                }
            }

            string titleKey = NormalizeTitle(rawTitle);
            if (string.IsNullOrWhiteSpace(titleKey))
            {
                return null;
            }

            return SelectDuplicateCandidate(scopeCondition + " and LOWER(REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(ISNULL(title,''))),' ',''),CHAR(9),''),N'\u3000',''))='" + EscapeSql(titleKey) + "'");
        }

        private Literature SelectDuplicateCandidate(string where)
        {
            DataTable dt = literatureBll.GetDatatable("select top 1 id from Literature where " + where + " order by case when canonical_literature_id is null then 0 else 1 end, case when status=1 then 0 when status=3 then 1 else 2 end, id asc");
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    int id = Function.ConvertTo<int>(dt.Rows[0]["id"].ToString(), 0);
                    if (id > 0)
                    {
                        return literatureBll.SelectSingle("id=" + id);
                    }
                }
                return null;
            }
            finally
            {
                if (dt != null)
                {
                    dt.Dispose();
                }
            }
        }

        private int GetCanonicalLiteratureId(Literature literature)
        {
            if (literature == null || literature.id <= 0)
            {
                return 0;
            }
            if (literature.canonical_literature_id.HasValue && literature.canonical_literature_id.Value > 0)
            {
                return literature.canonical_literature_id.Value;
            }
            int mergedId = GetDuplicateMasterId(literature.remark);
            return mergedId > 0 ? mergedId : literature.id;
        }

        private int GetDuplicateMasterId(string remark)
        {
            string cleanRemark = Function.HtmlDiscode(remark ?? string.Empty).Trim();
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

        private string NormalizeDoi(string value)
        {
            string text = Function.HtmlDiscode(value ?? string.Empty).Trim().ToLowerInvariant();
            text = text.Replace("https://doi.org/", string.Empty).Replace("http://dx.doi.org/", string.Empty);
            return Regex.Replace(text, @"\s+", string.Empty);
        }

        private string NormalizeTitle(string value)
        {
            string text = Function.HtmlEncode(Function.HtmlDiscode(value ?? string.Empty).Trim()).ToLowerInvariant();
            return Regex.Replace(text, @"[\s\u3000]+", string.Empty);
        }

        private string EscapeSql(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }

        private List<HttpPostedFile> GetBatchPostedFiles()
        {
            List<HttpPostedFile> files = new List<HttpPostedFile>();
            IList<HttpPostedFile> postedFiles = Request.Files.GetMultiple("batch_pdf_files");
            if (postedFiles != null)
            {
                foreach (HttpPostedFile file in postedFiles)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        files.Add(file);
                    }
                }
            }

            if (files.Count > 0)
            {
                return files;
            }

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFile file = Request.Files[i];
                if (file != null && file.ContentLength > 0 && string.Equals(Request.Files.GetKey(i), "batch_pdf_files", StringComparison.OrdinalIgnoreCase))
                {
                    files.Add(file);
                }
            }
            return files;
        }

        private bool IsAllowedPdf(HttpPostedFile postedFile)
        {
            return postedFile != null
                && postedFile.ContentLength > 0
                && postedFile.ContentLength <= UploadPolicy.MaxPdfBytes
                && string.Equals(Path.GetExtension(postedFile.FileName), ".pdf", StringComparison.OrdinalIgnoreCase);
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

        private string SaveUploadFile(HttpPostedFile postedFile, string folderName)
        {
            string extension = Path.GetExtension(postedFile.FileName).ToLower();
            string dateFolder = DateTime.Now.ToString("yyyyMMdd") + "/";
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff") + "_" + Guid.NewGuid().ToString("N").Substring(0, 8) + extension;
            string relativePath = dateFolder + fileName;
            string baseDirectory = Server.MapPath("../A_UpLoad/" + folderName + "/");
            string saveDirectory = Path.Combine(baseDirectory, dateFolder);
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
            postedFile.SaveAs(Path.Combine(baseDirectory, relativePath));
            return relativePath.Replace("\\", "/");
        }

        private Dictionary<string, BatchParsedPdf> GetBatchParsedMap(string payload)
        {
            Dictionary<string, BatchParsedPdf> map = new Dictionary<string, BatchParsedPdf>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(payload))
            {
                return map;
            }

            try
            {
                List<BatchParsedPdf> parsedList = JsonConvert.DeserializeObject<List<BatchParsedPdf>>(payload);
                if (parsedList == null)
                {
                    return map;
                }

                foreach (BatchParsedPdf item in parsedList)
                {
                    if (item == null || !item.success || string.IsNullOrWhiteSpace(item.file_name))
                    {
                        continue;
                    }

                    string key = Path.GetFileName(item.file_name);
                    if (!map.ContainsKey(key))
                    {
                        map.Add(key, item);
                    }
                }
            }
            catch
            {
            }
            return map;
        }

        private BatchParsedPdf FindParsedPdf(Dictionary<string, BatchParsedPdf> parsedMap, string fileName)
        {
            if (parsedMap == null || parsedMap.Count == 0 || string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            BatchParsedPdf parsed = null;
            parsedMap.TryGetValue(Path.GetFileName(fileName), out parsed);
            return parsed;
        }

        private class BatchParsedPdf
        {
            public bool success { get; set; }
            public string file_name { get; set; }
            public string title { get; set; }
            public string author_names { get; set; }
            public string institution { get; set; }
            public string doi { get; set; }
            public string publish_year { get; set; }
            public string publish_month { get; set; }
            public string publish_day { get; set; }
            public string journal_name { get; set; }
            public string conference_name { get; set; }
            public string volume { get; set; }
            public string issue { get; set; }
            public string pages { get; set; }
            public string publisher { get; set; }
            public string keywords { get; set; }
            public string abstract_text { get; set; }
            public string source_type { get; set; }
            public string category_id { get; set; }
            public JArray author_details { get; set; }
        }
    }
}
