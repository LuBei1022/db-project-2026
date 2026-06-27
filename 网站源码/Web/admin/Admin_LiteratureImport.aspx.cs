using LiteratureManager.Common;
using BLL;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Web.admin
{
    public partial class Admin_LiteratureImport : System.Web.UI.Page
    {
        private readonly BLLBase<LiteratureImportBatch> batchBll = new BLLBase<LiteratureImportBatch>();
        private readonly BLLBase<LiteratureImportError> errorBll = new BLLBase<LiteratureImportError>();
        private readonly BLLBase<Literature> literatureBll = new BLLBase<Literature>();
        private readonly BLLBase<LiteratureCategory> categoryBll = new BLLBase<LiteratureCategory>();
        public string MenuId = Function.GetRequest("MenuId");
        public bool isLoading = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            if (!IsPostBack)
            {
                BindData();
            }
        }

        protected void BindData()
        {
            DataTable dt = batchBll.GetDatatable("select top 20 * from LiteratureImportBatch order by id desc");
            DivNull.Visible = dt == null || dt.Rows.Count <= 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                Repeater1.DataSource = dt.DefaultView;
                Repeater1.DataBind();
            }
        }

        protected void OnClick_Import(object sender, EventArgs e)
        {
            isLoading = false;
            string importMode = (Request.Form["import_mode"] ?? "csv").Trim().ToLowerInvariant();
            if (importMode == "pdf")
            {
                HandlePdfImport();
                return;
            }

            HandleCsvImport();
        }

        private void HandleCsvImport()
        {
            if (!import_file.HasFile)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u8BF7\u5148\u9009\u62E9\u5F85\u5BFC\u5165\u7684 CSV \u6587\u4EF6!", Request.RawUrl, 2);
                return;
            }

            if (!Path.GetExtension(import_file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u76EE\u524D\u4EC5\u652F\u6301 CSV \u6279\u91CF\u5BFC\u5165!", Request.RawUrl, 2);
                return;
            }

            if (import_file.PostedFile.ContentLength > UploadPolicy.MaxImportBytes)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "CSV 文件不能超过 " + UploadPolicy.ToMbLabel(UploadPolicy.MaxImportBytes) + "！", Request.RawUrl, 2);
                return;
            }

            string savedPath = SaveImportFile(import_file);
            LiteratureImportBatch batch = new LiteratureImportBatch
            {
                batch_name = "\u6587\u732E\u6279\u91CF\u5BFC\u5165_" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                import_type = "CSV",
                file_name = Function.HtmlEncode(Path.GetFileName(import_file.FileName)),
                status = 0,
                total_count = 0,
                success_count = 0,
                fail_count = 0,
                userid = 0,
                addtime = DateTime.Now,
                finishtime = null
            };

            int batchId = Convert.ToInt32(batchBll.AddIdentity(batch, "id"));
            if (batchId <= 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u5BFC\u5165\u6279\u6B21\u521B\u5EFA\u5931\u8D25!", Request.RawUrl, 2);
                return;
            }

            batch.id = batchId;
            ImportCsv(savedPath, batch);
            batch.finishtime = DateTime.Now;
            batch.status = batch.fail_count > 0 ? 2 : 1;
            batchBll.Update(new[] { "id" }, batch);

            string message = batch.fail_count > 0
                ? "\u5BFC\u5165\u5B8C\u6210\uFF0C\u6210\u529F " + batch.success_count + " \u6761\uFF0C\u5931\u8D25 " + batch.fail_count + " \u6761\u3002"
                : "\u5BFC\u5165\u5B8C\u6210\uFF0C\u5171\u6210\u529F\u5BFC\u5165 " + batch.success_count + " \u6761\u6587\u732E\u3002";
            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), message, "Admin_LiteratureImport.aspx?MenuId=" + MenuId, 0);
        }

        private void HandlePdfImport()
        {
            List<HttpPostedFile> pdfFiles = GetImportPostedFiles();
            if (pdfFiles.Count == 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u8BF7\u5148\u9009\u62E9\u5F85\u5BFC\u5165\u7684 PDF \u6587\u4EF6!", Request.RawUrl, 2);
                return;
            }

            if (pdfFiles.Count > UploadPolicy.MaxBatchFiles)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u5355\u6B21 PDF \u6279\u91CF\u5BFC\u5165\u6700\u591A\u652F\u6301 " + UploadPolicy.MaxBatchFiles + " \u4E2A\u6587\u4EF6!", Request.RawUrl, 2);
                return;
            }

            long totalBytes = 0;
            foreach (HttpPostedFile postedFile in pdfFiles)
            {
                if (!IsAllowedPdf(postedFile))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "PDF \u5BFC\u5165\u4E2D\u5305\u542B\u683C\u5F0F\u4E0D\u6B63\u786E\u6216\u8D85\u8FC7 " + UploadPolicy.ToMbLabel(UploadPolicy.MaxPdfBytes) + " \u7684\u6587\u4EF6!", Request.RawUrl, 2);
                    return;
                }
                totalBytes += postedFile.ContentLength;
            }

            if (totalBytes > UploadPolicy.MaxBatchTotalBytes)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u5355\u6B21 PDF \u6279\u91CF\u5BFC\u5165\u603B\u5927\u5C0F\u4E0D\u80FD\u8D85\u8FC7 " + UploadPolicy.ToMbLabel(UploadPolicy.MaxBatchTotalBytes) + "!", Request.RawUrl, 2);
                return;
            }

            List<JObject> parsedItems = ReadPdfPreviewItems();
            if (parsedItems.Count == 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "PDF \u6A21\u5F0F\u8BF7\u5148\u70B9\u51FB\u300C\u89E3\u6790\u9884\u89C8\u300D\uFF0C\u786E\u8BA4\u6216\u4FEE\u6539\u540E\u518D\u5BFC\u5165\u3002", Request.RawUrl, 2);
                return;
            }

            LiteratureImportBatch batch = new LiteratureImportBatch
            {
                batch_name = "\u6587\u732E PDF \u6279\u91CF\u5BFC\u5165_" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                import_type = "PDF",
                file_name = BuildBatchFileName(pdfFiles),
                status = 0,
                total_count = 0,
                success_count = 0,
                fail_count = 0,
                userid = 0,
                addtime = DateTime.Now,
                finishtime = null
            };

            int batchId = Convert.ToInt32(batchBll.AddIdentity(batch, "id"));
            if (batchId <= 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u5BFC\u5165\u6279\u6B21\u521B\u5EFA\u5931\u8D25!", Request.RawUrl, 2);
                return;
            }

            batch.id = batchId;
            ImportPdfFiles(pdfFiles, parsedItems, batch);
            batch.finishtime = DateTime.Now;
            batch.status = batch.fail_count > 0 ? 2 : 1;
            batchBll.Update(new[] { "id" }, batch);

            string message = batch.fail_count > 0
                ? "\u5BFC\u5165\u5B8C\u6210\uFF0C\u6210\u529F " + batch.success_count + " \u4E2A PDF\uFF0C\u5931\u8D25 " + batch.fail_count + " \u4E2A\u3002"
                : "\u5BFC\u5165\u5B8C\u6210\uFF0C\u5171\u6210\u529F\u5BFC\u5165 " + batch.success_count + " \u4E2A PDF\u3002";
            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), message, "Admin_LiteratureImport.aspx?MenuId=" + MenuId, 0);
        }

        private void ImportPdfFiles(List<HttpPostedFile> pdfFiles, List<JObject> parsedItems, LiteratureImportBatch batch)
        {
            int rowNo = 0;
            HashSet<int> usedPayloadIndexes = new HashSet<int>();
            foreach (HttpPostedFile postedFile in pdfFiles)
            {
                rowNo++;
                batch.total_count++;
                string originalFileName = Path.GetFileName(postedFile.FileName);
                string savedRelativePath = string.Empty;
                string parsedTitle = string.Empty;

                try
                {
                    savedRelativePath = SaveLiteraturePdfFile(postedFile);
                    JObject parsed = FindPdfPreviewItem(parsedItems, usedPayloadIndexes, originalFileName, rowNo - 1);

                    if (parsed == null)
                    {
                        throw new Exception("\u672A\u627E\u5230\u8BE5 PDF \u7684\u89E3\u6790\u9884\u89C8\u7ED3\u679C\uFF0C\u8BF7\u5148\u89E3\u6790\u5E76\u786E\u8BA4");
                    }
                    if (!IsPdfPreviewSuccess(parsed))
                    {
                        throw new Exception("\u8BE5 PDF \u9884\u89C8\u89E3\u6790\u672A\u6210\u529F\uFF0C\u8BF7\u91CD\u65B0\u89E3\u6790\u540E\u518D\u5BFC\u5165");
                    }

                    parsedTitle = GetJsonValue(parsed, "title");
                    Literature literature = BuildLiteratureFromParsed(parsed, batch.id, Path.GetFileNameWithoutExtension(originalFileName));
                    Literature duplicate = FindDuplicateLiterature(Function.HtmlDiscode(literature.title), GetJsonValue(parsed, "doi"));
                    if (duplicate != null && duplicate.id > 0)
                    {
                        throw new Exception(BuildDuplicateMessage(duplicate));
                    }

                    int literatureId = Convert.ToInt32(literatureBll.AddIdentity(literature, "id"));
                    if (literatureId <= 0)
                    {
                        throw new Exception("\u6570\u636E\u5E93\u5199\u5165\u5931\u8D25");
                    }

                    literature.id = literatureId;
                    LiteratureRelationSync.Sync(
                        literature,
                        GetJsonValue(parsed, "author_names"),
                        GetJsonValue(parsed, "keywords"),
                        savedRelativePath,
                        originalFileName,
                        GetAuthorDetailsJson(parsed));
                    LiteratureVenueProfileSync.EnsureForLiterature(literature);
                    if (literature.status == 1)
                    {
                        LiteratureRagSync.QueueReindex(literature.id);
                    }
                    batch.success_count++;
                }
                catch (Exception ex)
                {
                    batch.fail_count++;
                    string title = string.IsNullOrWhiteSpace(parsedTitle) ? originalFileName : parsedTitle;
                    SaveImportError(batch.id, rowNo, title, ex.Message, BuildPdfRawData(originalFileName, savedRelativePath));
                }
            }
        }

        private void ImportCsv(string fullPath, LiteratureImportBatch batch)
        {
            using (StreamReader reader = new StreamReader(fullPath, Encoding.UTF8, true))
            {
                string headerLine = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(headerLine))
                {
                    SaveImportError(batch.id, 1, string.Empty, "\u5BFC\u5165\u6587\u4EF6\u4E3A\u7A7A", "{}");
                    batch.total_count = 1;
                    batch.fail_count = 1;
                    return;
                }

                List<string> headers = ParseCsvLine(headerLine);
                int rowNo = 1;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    if (batch.total_count >= UploadPolicy.MaxImportRows)
                    {
                        batch.fail_count++;
                        SaveImportError(batch.id, rowNo + 1, string.Empty, "导入记录数超过 " + UploadPolicy.MaxImportRows + " 行限制", "{}");
                        break;
                    }

                    rowNo++;
                    batch.total_count++;
                    List<string> values = ParseCsvLine(line);
                    Dictionary<string, string> row = BuildRow(headers, values);
                    string title = GetValue(row, "title");
                    try
                    {
                        if (string.IsNullOrWhiteSpace(title))
                        {
                            throw new Exception("\u7F3A\u5C11 title \u5B57\u6BB5");
                        }

                        Literature duplicate = FindDuplicateLiterature(title, GetValue(row, "doi"));
                        if (duplicate != null && duplicate.id > 0)
                        {
                            throw new Exception(BuildDuplicateMessage(duplicate));
                        }

                        Literature literature = BuildLiterature(row, batch.id);
                        int literatureId = Convert.ToInt32(literatureBll.AddIdentity(literature, "id"));
                        if (literatureId <= 0)
                        {
                            throw new Exception("\u6570\u636E\u5E93\u5199\u5165\u5931\u8D25");
                        }

                        literature.id = literatureId;
                        LiteratureRelationSync.Sync(literature, GetValue(row, "author_names"), GetValue(row, "tag_names"), string.Empty, string.Empty);
                        if (literature.status == 1)
                        {
                            LiteratureRagSync.QueueReindex(literature.id);
                        }
                        batch.success_count++;
                    }
                    catch (Exception ex)
                    {
                        batch.fail_count++;
                        SaveImportError(batch.id, rowNo, title, ex.Message, JsonConvert.SerializeObject(row));
                    }
                }
            }
        }

        private Literature BuildLiterature(Dictionary<string, string> row, int batchId)
        {
            Literature literature = new Literature();
            literature.title = Function.HtmlEncode(GetValue(row, "title"));
            literature.subtitle = Function.HtmlEncode(GetValue(row, "subtitle"));
            literature.institution = LiteratureRelationSync.EncodeForColumn(GetValue(row, "institution"), 500);
            literature.doi = Function.HtmlEncode(GetValue(row, "doi"));
            literature.keywords = Function.HtmlEncode(GetValue(row, "keywords"));
            literature.abstract_text = Function.HtmlEncode(GetValue(row, "abstract_text"));
            literature.source_type = Function.HtmlEncode(DefaultIfEmpty(GetValue(row, "source_type"), "\u671F\u520A\u8BBA\u6587"));
            literature.language = Function.HtmlEncode(DefaultIfEmpty(GetValue(row, "language"), "\u4E2D\u6587"));
            string publishDateError = ApplyPublicationDate(literature, GetValue(row, "publish_year"), GetValue(row, "publish_month"), GetValue(row, "publish_day"));
            if (!string.IsNullOrWhiteSpace(publishDateError))
            {
                throw new Exception(publishDateError);
            }
            literature.journal_name = Function.HtmlEncode(GetValue(row, "journal_name"));
            literature.conference_name = Function.HtmlEncode(GetValue(row, "conference_name"));
            literature.publisher = Function.HtmlEncode(GetValue(row, "publisher"));
            literature.volume = Function.HtmlEncode(GetValue(row, "volume"));
            literature.issue = Function.HtmlEncode(GetValue(row, "issue"));
            literature.pages = Function.HtmlEncode(GetValue(row, "pages"));
            literature.category_id = ResolveCategoryId(row);
            literature.cover_pic = string.Empty;
            literature.external_url = Function.HtmlEncode(GetValue(row, "external_url"));
            literature.source_db = Function.HtmlEncode(GetValue(row, "source_db"));
            literature.remark = Function.HtmlEncode(GetValue(row, "remark"));
            literature.is_top = ParseInt(GetValue(row, "is_top"), 0);
            literature.status = ParseInt(GetValue(row, "status"), 1);
            literature.userid = 0;
            literature.import_batch_id = batchId;
            if (literature.status == 1 || literature.status == 2)
            {
                literature.reviewed_by = Function.ConvertTo<int>(Cookie.GetCookie("LMS_AdminID"), 0);
                literature.review_time = DateTime.Now;
            }
            literature.addtime = DateTime.Now;
            literature.updatetime = DateTime.Now;
            return literature;
        }

        private Literature BuildLiteratureFromParsed(JObject parsed, int batchId, string titleFallback)
        {
            string title = DefaultIfEmpty(GetJsonValue(parsed, "title"), titleFallback);
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new Exception("\u672A\u80FD\u89E3\u6790\u51FA\u6587\u732E\u6807\u9898");
            }

            Literature literature = new Literature();
            literature.title = Function.HtmlEncode(title.Trim());
            literature.subtitle = string.Empty;
            literature.institution = LiteratureRelationSync.EncodeForColumn(GetJsonValue(parsed, "institution"), 500);
            string doiValue = GetFirstJsonValue(parsed, "doi", "DOI");
            literature.doi = Function.HtmlEncode(doiValue);
            literature.keywords = Function.HtmlEncode(GetJsonValue(parsed, "keywords"));
            literature.abstract_text = Function.HtmlEncode(GetJsonValue(parsed, "abstract_text"));
            literature.source_type = Function.HtmlEncode(DefaultIfEmpty(GetJsonValue(parsed, "source_type"), "\u671F\u520A\u8BBA\u6587"));
            literature.language = ContainsChinese(title + GetJsonValue(parsed, "abstract_text")) ? "\u4E2D\u6587" : "\u82F1\u6587";
            string publishYear;
            string publishMonth;
            string publishDay;
            ResolveParsedPublishDate(parsed, doiValue, title, out publishYear, out publishMonth, out publishDay);
            string publishDateError = ApplyPublicationDate(literature, publishYear, publishMonth, publishDay);
            if (!string.IsNullOrWhiteSpace(publishDateError))
            {
                throw new Exception(publishDateError);
            }
            literature.journal_name = Function.HtmlEncode(GetJsonValue(parsed, "journal_name"));
            literature.conference_name = Function.HtmlEncode(GetJsonValue(parsed, "conference_name"));
            literature.publisher = Function.HtmlEncode(GetJsonValue(parsed, "publisher"));
            literature.volume = Function.HtmlEncode(GetJsonValue(parsed, "volume"));
            literature.issue = Function.HtmlEncode(GetJsonValue(parsed, "issue"));
            literature.pages = Function.HtmlEncode(GetJsonValue(parsed, "pages"));
            literature.category_id = 0;
            literature.cover_pic = string.Empty;
            literature.download_points = 0;
            literature.external_url = string.Empty;
            literature.source_db = string.Empty;
            literature.remark = string.Empty;
            literature.is_top = 0;
            literature.status = 1;
            literature.userid = 0;
            literature.import_batch_id = batchId;
            literature.reviewed_by = Function.ConvertTo<int>(Cookie.GetCookie("LMS_AdminID"), 0);
            literature.review_time = DateTime.Now;
            literature.addtime = DateTime.Now;
            literature.updatetime = DateTime.Now;
            return literature;
        }

        private int ResolveCategoryId(Dictionary<string, string> row)
        {
            int categoryId = ParseInt(GetValue(row, "category_id"), 0);
            if (categoryId > 0)
            {
                return categoryId;
            }

            string categoryName = GetValue(row, "category_name");
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return 0;
            }

            LiteratureCategory category = categoryBll.SelectSingle("name='" + Function.HtmlEncode(categoryName) + "' and status<>-1");
            if (category == null || category.id <= 0)
            {
                throw new Exception("\u5206\u7C7B\u300A" + categoryName + "\u300B\u4E0D\u5B58\u5728");
            }
            return category.id;
        }

        private string SaveImportFile(System.Web.UI.WebControls.FileUpload upload)
        {
            string folder = Server.MapPath("~/A_UpLoad/upload_file/import/");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string saveName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Path.GetFileName(upload.FileName);
            string fullPath = Path.Combine(folder, saveName);
            upload.SaveAs(fullPath);
            return fullPath;
        }

        private List<HttpPostedFile> GetImportPostedFiles()
        {
            List<HttpPostedFile> files = new List<HttpPostedFile>();
            IList<HttpPostedFile> postedFiles = Request.Files.GetMultiple(import_file.UniqueID);
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
                if (file != null && file.ContentLength > 0)
                {
                    files.Add(file);
                }
            }
            return files;
        }

        private List<JObject> ReadPdfPreviewItems()
        {
            List<JObject> items = new List<JObject>();
            string payload = pdf_parse_payload.Value;
            if (string.IsNullOrWhiteSpace(payload))
            {
                return items;
            }

            try
            {
                JArray array = JArray.Parse(payload);
                foreach (JToken token in array)
                {
                    JObject item = token as JObject;
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
            }
            catch
            {
            }

            return items;
        }

        private JObject FindPdfPreviewItem(List<JObject> items, HashSet<int> usedIndexes, string fileName, int fallbackIndex)
        {
            if (items == null || items.Count == 0)
            {
                return null;
            }

            string cleanFileName = Path.GetFileName(fileName ?? string.Empty);
            for (int i = 0; i < items.Count; i++)
            {
                if (usedIndexes.Contains(i))
                {
                    continue;
                }

                string payloadFileName = Path.GetFileName(GetJsonValue(items[i], "file_name"));
                if (string.Equals(payloadFileName, cleanFileName, StringComparison.OrdinalIgnoreCase))
                {
                    usedIndexes.Add(i);
                    return items[i];
                }
            }

            if (fallbackIndex >= 0 && fallbackIndex < items.Count && !usedIndexes.Contains(fallbackIndex))
            {
                usedIndexes.Add(fallbackIndex);
                return items[fallbackIndex];
            }

            return null;
        }

        private bool IsPdfPreviewSuccess(JObject item)
        {
            JToken token = item == null ? null : item["success"];
            if (token == null)
            {
                return false;
            }
            if (token.Type == JTokenType.Boolean)
            {
                return token.Value<bool>();
            }
            return string.Equals(token.ToString(), "true", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsAllowedPdf(HttpPostedFile postedFile)
        {
            return postedFile != null
                && postedFile.ContentLength > 0
                && postedFile.ContentLength <= UploadPolicy.MaxPdfBytes
                && string.Equals(Path.GetExtension(postedFile.FileName), ".pdf", StringComparison.OrdinalIgnoreCase);
        }

        private string SaveLiteraturePdfFile(HttpPostedFile postedFile)
        {
            string extension = Path.GetExtension(postedFile.FileName).ToLower();
            string dateFolder = DateTime.Now.ToString("yyyyMMdd") + "/";
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff") + "_" + Guid.NewGuid().ToString("N").Substring(0, 8) + extension;
            string relativePath = dateFolder + fileName;
            string baseDirectory = Server.MapPath("~/A_UpLoad/upload_file/");
            string saveDirectory = Path.Combine(baseDirectory, dateFolder);
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            postedFile.SaveAs(Path.Combine(baseDirectory, relativePath));
            return relativePath.Replace("\\", "/");
        }

        private string BuildBatchFileName(List<HttpPostedFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return string.Empty;
            }

            if (files.Count == 1)
            {
                return Function.HtmlEncode(Path.GetFileName(files[0].FileName));
            }

            return Function.HtmlEncode(files.Count + "\u4E2A PDF \u6587\u4EF6");
        }

        private string BuildPdfRawData(string originalFileName, string savedRelativePath)
        {
            Dictionary<string, string> raw = new Dictionary<string, string>();
            raw["file_name"] = originalFileName ?? string.Empty;
            raw["file_path"] = savedRelativePath ?? string.Empty;
            return JsonConvert.SerializeObject(raw);
        }

        private string GetJsonValue(JObject obj, string key)
        {
            if (obj == null || obj[key] == null)
            {
                return string.Empty;
            }
            return Function.HtmlDiscode(obj[key].ToString()).Trim();
        }

        private string GetFirstJsonValue(JObject obj, params string[] keys)
        {
            if (obj == null || keys == null)
            {
                return string.Empty;
            }

            foreach (string key in keys)
            {
                string value = GetJsonValue(obj, key);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return string.Empty;
        }

        private string GetAuthorDetailsJson(JObject parsed)
        {
            if (parsed == null)
            {
                return string.Empty;
            }

            JToken token = parsed["author_details"] ?? parsed["authors"];
            return token == null ? string.Empty : token.ToString(Formatting.None);
        }

        private void ResolveParsedPublishDate(JObject parsed, string doi, string title, out string year, out string month, out string day)
        {
            year = NormalizeYearValue(GetFirstJsonValue(parsed, "publish_year", "publication_year", "pub_year", "year"));
            month = NormalizeMonthValue(GetFirstJsonValue(parsed, "publish_month", "publication_month", "pub_month", "month"));
            day = NormalizeDayValue(GetFirstJsonValue(parsed, "publish_day", "publication_day", "pub_day", "day"));
            if (!string.IsNullOrWhiteSpace(year))
            {
                return;
            }

            string dateText = GetFirstJsonValue(parsed, "publish_date", "publication_date", "published_date", "published", "date", "date_published", "created", "updated");
            if (TryParseDateText(dateText, out year, out month, out day))
            {
                return;
            }

            string combined = string.Join(" ",
                new[]
                {
                    doi,
                    GetJsonValue(parsed, "conference_name"),
                    GetJsonValue(parsed, "journal_name"),
                    GetJsonValue(parsed, "source_db"),
                    title
                });
            if (TryParseDateText(combined, out year, out month, out day))
            {
                return;
            }

            if (TryParseArxivDateFromDoi(doi, out year, out month))
            {
                day = string.Empty;
                return;
            }

            year = string.Empty;
            month = string.Empty;
            day = string.Empty;
        }

        private bool TryParseDateText(string text, out string year, out string month, out string day)
        {
            year = string.Empty;
            month = string.Empty;
            day = string.Empty;
            string source = Function.HtmlDiscode(text ?? string.Empty);
            if (string.IsNullOrWhiteSpace(source))
            {
                return false;
            }

            Match match = Regex.Match(source, @"\b((?:19|20)\d{2})[-/.年]\s*(\d{1,2})(?:[-/.月]\s*(\d{1,2}))?", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return SetDateParts(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, out year, out month, out day);
            }

            string monthPattern = @"(Jan(?:uary)?|Feb(?:ruary)?|Mar(?:ch)?|Apr(?:il)?|May|Jun(?:e)?|Jul(?:y)?|Aug(?:ust)?|Sep(?:t(?:ember)?)?|Oct(?:ober)?|Nov(?:ember)?|Dec(?:ember)?)";
            match = Regex.Match(source, monthPattern + @"\s+(\d{1,2},\s*)?((?:19|20)\d{2})", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return SetDateParts(match.Groups[3].Value, MonthNameToNumber(match.Groups[1].Value), (match.Groups[2].Value ?? string.Empty).Trim(' ', ','), out year, out month, out day);
            }

            match = Regex.Match(source, @"\b((?:19|20)\d{2})\s+" + monthPattern + @"(?:\s+(\d{1,2}))?\b", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return SetDateParts(match.Groups[1].Value, MonthNameToNumber(match.Groups[2].Value), match.Groups[3].Value, out year, out month, out day);
            }

            match = Regex.Match(source, @"\b((?:19|20)\d{2})\b");
            if (match.Success)
            {
                year = match.Groups[1].Value;
                return true;
            }

            return false;
        }

        private bool TryParseArxivDateFromDoi(string doi, out string year, out string month)
        {
            year = string.Empty;
            month = string.Empty;
            Match match = Regex.Match((doi ?? string.Empty).ToLowerInvariant(), @"arxiv[.:/ ]+(\d{2})(\d{2})\.\d+");
            if (!match.Success)
            {
                return false;
            }

            int yy = Function.ConvertTo<int>(match.Groups[1].Value, 0);
            int mm = Function.ConvertTo<int>(match.Groups[2].Value, 0);
            int fullYear = yy >= 91 ? 1900 + yy : 2000 + yy;
            if (fullYear < 1990 || fullYear > 2100 || mm < 1 || mm > 12)
            {
                return false;
            }

            year = fullYear.ToString();
            month = mm.ToString();
            return true;
        }

        private bool SetDateParts(string yearValue, string monthValue, string dayValue, out string year, out string month, out string day)
        {
            year = NormalizeYearValue(yearValue);
            month = NormalizeMonthValue(monthValue);
            day = NormalizeDayValue(dayValue);
            if (string.IsNullOrWhiteSpace(year))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(month))
            {
                day = string.Empty;
                return true;
            }

            int y = Function.ConvertTo<int>(year, 0);
            int m = Function.ConvertTo<int>(month, 0);
            int d = Function.ConvertTo<int>(day, 0);
            if (d > 0 && d > DateTime.DaysInMonth(y, m))
            {
                day = string.Empty;
            }
            return true;
        }

        private Literature FindDuplicateLiterature(string rawTitle, string rawDoi)
        {
            string doiKey = NormalizeDoi(rawDoi);
            string titleKey = NormalizeTitle(rawTitle);
            if (string.IsNullOrWhiteSpace(doiKey) && string.IsNullOrWhiteSpace(titleKey))
            {
                return null;
            }

            DataTable dt = literatureBll.GetDatatable(@"
select id,title,doi,canonical_literature_id,status
from Literature
where status<>-1 and (isnull(title,N'')<>N'' or isnull(doi,N'')<>N'')
order by case when canonical_literature_id is null then 0 else 1 end,
         case when status=1 then 0 when status=0 then 1 when status=3 then 2 else 3 end,
         id asc");
            try
            {
                if (dt == null)
                {
                    return null;
                }

                foreach (DataRow row in dt.Rows)
                {
                    int id = Function.ConvertTo<int>(Convert.ToString(row["id"]), 0);
                    if (id <= 0)
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(doiKey) && NormalizeDoi(Convert.ToString(row["doi"])) == doiKey)
                    {
                        return literatureBll.SelectSingle("id=" + id);
                    }

                    if (!string.IsNullOrWhiteSpace(titleKey) && NormalizeTitle(Convert.ToString(row["title"])) == titleKey)
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

        private string BuildDuplicateMessage(Literature duplicate)
        {
            if (duplicate == null || duplicate.id <= 0)
            {
                return "\u6587\u732E\u5DF2\u5B58\u5728\uFF0C\u5DF2\u8DF3\u8FC7\u5BFC\u5165";
            }

            string title = Function.HtmlDiscode(duplicate.title ?? string.Empty).Trim();
            return "\u6587\u732E\u5DF2\u5B58\u5728\uFF0C\u5DF2\u8DF3\u8FC7\u5BFC\u5165\uFF08ID:" + duplicate.id + (string.IsNullOrWhiteSpace(title) ? string.Empty : "\uFF0C\u6807\u9898\uFF1A" + title) + "\uFF09";
        }

        private string NormalizeDoi(string value)
        {
            string text = DecodeCompareText(value).ToLowerInvariant();
            text = text.Replace("https://doi.org/", string.Empty)
                .Replace("http://doi.org/", string.Empty)
                .Replace("http://dx.doi.org/", string.Empty)
                .Replace("doi:", string.Empty)
                .Replace("doi：", string.Empty);
            return Regex.Replace(text, @"[\s\u3000]+", string.Empty).Trim(' ', '.', ';', ',');
        }

        private string NormalizeTitle(string value)
        {
            string text = DecodeCompareText(value).Normalize(NormalizationForm.FormKC).ToLowerInvariant();
            return Regex.Replace(text, @"[^a-z0-9\u4e00-\u9fff]+", string.Empty);
        }

        private string DecodeCompareText(string value)
        {
            string text = Function.HtmlDiscode(value ?? string.Empty);
            text = HttpUtility.HtmlDecode(text) ?? string.Empty;
            text = text.Replace('\u00A0', ' ').Replace("&nbsp;", " ");
            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        private string EscapeSql(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }

        private string NormalizeYearValue(string value)
        {
            Match match = Regex.Match(value ?? string.Empty, @"(?:19|20)\d{2}");
            if (!match.Success)
            {
                return string.Empty;
            }

            int year = Function.ConvertTo<int>(match.Value, 0);
            return year >= 1900 && year <= 2100 ? year.ToString() : string.Empty;
        }

        private string NormalizeMonthValue(string value)
        {
            string text = Function.HtmlDiscode(value ?? string.Empty).Trim();
            string fromName = MonthNameToNumber(text);
            if (!string.IsNullOrWhiteSpace(fromName))
            {
                return fromName;
            }

            Match match = Regex.Match(text, @"\d{1,2}");
            if (!match.Success)
            {
                return string.Empty;
            }

            int month = Function.ConvertTo<int>(match.Value, 0);
            return month >= 1 && month <= 12 ? month.ToString() : string.Empty;
        }

        private string NormalizeDayValue(string value)
        {
            Match match = Regex.Match(value ?? string.Empty, @"\d{1,2}");
            if (!match.Success)
            {
                return string.Empty;
            }

            int day = Function.ConvertTo<int>(match.Value, 0);
            return day >= 1 && day <= 31 ? day.ToString() : string.Empty;
        }

        private string MonthNameToNumber(string value)
        {
            string text = (value ?? string.Empty).Trim().ToLowerInvariant();
            if (text.Length < 3)
            {
                return string.Empty;
            }

            switch (text.Substring(0, 3))
            {
                case "jan": return "1";
                case "feb": return "2";
                case "mar": return "3";
                case "apr": return "4";
                case "may": return "5";
                case "jun": return "6";
                case "jul": return "7";
                case "aug": return "8";
                case "sep": return "9";
                case "oct": return "10";
                case "nov": return "11";
                case "dec": return "12";
                default: return string.Empty;
            }
        }

        private bool ContainsChinese(string value)
        {
            foreach (char current in value ?? string.Empty)
            {
                if ((current >= '\u4E00' && current <= '\u9FFF') ||
                    (current >= '\u3400' && current <= '\u4DBF') ||
                    (current >= '\uF900' && current <= '\uFAFF'))
                {
                    return true;
                }
            }

            return false;
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
                    return "\u586B\u5199\u53D1\u8868\u6708\u4EFD\u6216\u65E5\u671F\u65F6\u5FC5\u987B\u540C\u65F6\u586B\u5199\u53D1\u8868\u5E74\u4EFD\u3002";
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
                return "\u53D1\u8868\u5E74\u4EFD\u683C\u5F0F\u4E0D\u6B63\u786E\u3002";
            }
            if (month < 0 || month > 12)
            {
                return "\u53D1\u8868\u6708\u4EFD\u5FC5\u987B\u5728 1-12 \u4E4B\u95F4\u3002";
            }
            if (month == 0 && day > 0)
            {
                return "\u586B\u5199\u53D1\u8868\u65E5\u671F\u65F6\u5FC5\u987B\u540C\u65F6\u586B\u5199\u53D1\u8868\u6708\u4EFD\u3002";
            }
            if (day < 0 || day > 31)
            {
                return "\u53D1\u8868\u65E5\u671F\u683C\u5F0F\u4E0D\u6B63\u786E\u3002";
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
                    return "\u53D1\u8868\u65E5\u671F\u8D85\u8FC7\u8BE5\u6708\u4EFD\u6700\u5927\u5929\u6570\u3002";
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

        private void SaveImportError(int batchId, int rowNo, string title, string errorMessage, string rawData)
        {
            LiteratureImportError error = new LiteratureImportError
            {
                batch_id = batchId,
                row_no = rowNo,
                title = Function.HtmlEncode(title),
                error_msg = Function.HtmlEncode(errorMessage),
                raw_data = Function.HtmlEncode(rawData),
                addtime = DateTime.Now
            };
            errorBll.Add(error, "id");
        }

        private Dictionary<string, string> BuildRow(List<string> headers, List<string> values)
        {
            Dictionary<string, string> row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < headers.Count; i++)
            {
                string key = headers[i].Trim();
                string value = i < values.Count ? values[i] : string.Empty;
                if (!row.ContainsKey(key))
                {
                    row.Add(key, value == null ? string.Empty : value.Trim());
                }
            }
            return row;
        }

        private List<string> ParseCsvLine(string line)
        {
            List<string> fields = new List<string>();
            if (line == null)
            {
                return fields;
            }

            MatchCollection matches = Regex.Matches(line, "(?:^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
            foreach (Match match in matches)
            {
                string value = match.Value;
                if (value.StartsWith(","))
                {
                    value = value.Substring(1);
                }
                value = value.Trim();
                if (value.StartsWith("\"") && value.EndsWith("\"") && value.Length >= 2)
                {
                    value = value.Substring(1, value.Length - 2).Replace("\"\"", "\"");
                }
                fields.Add(value);
            }
            return fields;
        }

        private string GetValue(Dictionary<string, string> row, string key)
        {
            return row.ContainsKey(key) ? row[key] : string.Empty;
        }

        private string DefaultIfEmpty(string value, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
        }

        private int ParseInt(string value, int defaultValue)
        {
            return Function.ConvertTo<int>(value, defaultValue);
        }

        private int? ParseNullableInt(string value)
        {
            int result = ParseInt(value, 0);
            return result > 0 ? (int?)result : null;
        }

        public string GetBatchStatus(object statusObj)
        {
            int status = Function.ConvertTo<int>(statusObj, 0);
            switch (status)
            {
                case 1:
                    return "\u5168\u90E8\u6210\u529F";
                case 2:
                    return "\u90E8\u5206\u5931\u8D25";
                default:
                    return "\u5904\u7406\u4E2D";
            }
        }
    }
}
