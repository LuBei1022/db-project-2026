using LiteratureManager.Common;
using BLL;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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

                        if (literatureBll.Exists("title='" + Function.HtmlEncode(title) + "' and status<>-1"))
                        {
                            throw new Exception("\u6587\u732E\u6807\u9898\u5DF2\u5B58\u5728");
                        }

                        Literature literature = BuildLiterature(row, batch.id);
                        int literatureId = Convert.ToInt32(literatureBll.AddIdentity(literature, "id"));
                        if (literatureId <= 0)
                        {
                            throw new Exception("\u6570\u636E\u5E93\u5199\u5165\u5931\u8D25");
                        }

                        literature.id = literatureId;
                        LiteratureRelationSync.Sync(literature, GetValue(row, "author_names"), GetValue(row, "tag_names"), string.Empty, string.Empty);
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
            literature.publish_year = ParseNullableInt(GetValue(row, "publish_year"));
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
