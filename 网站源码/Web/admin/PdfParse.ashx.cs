using LiteratureManager.Common;
using BLL;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Web.admin
{
    public class PdfParse : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;

            if (!IsAuthorizedRequest())
            {
                WriteJson(context, new { success = false, message = "\u8BF7\u5148\u767B\u5F55\u540E\u518D\u4F7F\u7528 PDF \u89E3\u6790" });
                return;
            }

            HttpPostedFile file = context.Request.Files["file"];
            if (file == null || file.ContentLength <= 0)
            {
                WriteJson(context, new { success = false, message = "\u8BF7\u5148\u4E0A\u4F20 PDF \u6587\u4EF6" });
                return;
            }

            if (!Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                WriteJson(context, new { success = false, message = "\u76EE\u524D\u4EC5\u652F\u6301 PDF \u89E3\u6790" });
                return;
            }

            if (file.ContentLength > UploadPolicy.MaxPdfBytes)
            {
                WriteJson(context, new { success = false, message = "PDF 文件不能超过 " + UploadPolicy.ToMbLabel(UploadPolicy.MaxPdfBytes) });
                return;
            }

            IDisposable parseLease = PdfParseConcurrencyGate.TryEnter();
            if (parseLease == null)
            {
                WriteJson(context, new { success = false, message = "当前解析任务较多，请稍后重试" });
                return;
            }

            string tempPath = null;
            try
            {
                tempPath = SaveTempFile(context, file);
                JObject parsed = TryParseByApp(tempPath) ?? TryParseByPython(context, tempPath);
                if (parsed == null)
                {
                    WriteJson(context, new { success = false, message = "\u672A\u80FD\u4ECE PDF \u4E2D\u89E3\u6790\u51FA\u6587\u732E\u4FE1\u606F" });
                    return;
                }

                string title = GetJsonString(parsed, "title");
                string authorNames = JoinArray(parsed, "authors", ", ");
                string institution = NormalizeParsedInstitution(JoinArray(parsed, "institutions", "\uFF1B"));
                string journalRaw = GetJsonString(parsed, "journal");
                string conferenceRaw = GetJsonString(parsed, "conference");
                string sourceTypeRaw = GetJsonString(parsed, "source_type");
                string doi = GetJsonString(parsed, "doi");
                string publishYear = GetJsonString(parsed, "publish_year");
                string volume = GetJsonString(parsed, "volume");
                string issue = GetJsonString(parsed, "issue");
                string pages = GetJsonString(parsed, "pages");
                string pageCount = GetJsonString(parsed, "page_count");
                string publisher = GetJsonString(parsed, "publisher");
                string keywords = JoinArray(parsed, "keywords", ", ");
                string abstractText = GetJsonString(parsed, "abstract");

                string journalName = string.Empty;
                string conferenceName = string.Empty;
                string sourceType = "\u5176\u4ED6";
                if (!string.IsNullOrWhiteSpace(journalRaw))
                {
                    journalName = journalRaw;
                    sourceType = "\u671F\u520A\u8BBA\u6587";
                }
                else if (!string.IsNullOrWhiteSpace(conferenceRaw))
                {
                    if (LooksLikeJournal(conferenceRaw))
                    {
                        journalName = conferenceRaw;
                        sourceType = "\u671F\u520A\u8BBA\u6587";
                    }
                    else
                    {
                        conferenceName = conferenceRaw;
                        sourceType = "\u4F1A\u8BAE\u8BBA\u6587";
                    }
                }

                if (!string.IsNullOrWhiteSpace(sourceTypeRaw))
                {
                    if (sourceTypeRaw.Contains("\u671F\u520A"))
                    {
                        sourceType = "\u671F\u520A\u8BBA\u6587";
                    }
                    else if (sourceTypeRaw.Contains("\u4F1A\u8BAE"))
                    {
                        sourceType = "\u4F1A\u8BAE\u8BBA\u6587";
                    }
                }

                if (string.IsNullOrWhiteSpace(journalName) && !string.IsNullOrWhiteSpace(conferenceName))
                {
                    journalName = string.Empty;
                }

                WriteJson(context, new
                {
                    success = true,
                    title = title,
                    author_names = authorNames,
                    institution = institution,
                    doi = doi,
                    publish_year = publishYear,
                    volume = volume,
                    issue = issue,
                    pages = pages,
                    page_count = pageCount,
                    publisher = publisher,
                    journal_name = journalName,
                    conference_name = conferenceName,
                    keywords = keywords,
                    abstract_text = abstractText,
                    source_type = sourceType,
                    remark_append = string.IsNullOrWhiteSpace(institution) ? string.Empty : "\u4F5C\u8005\u5355\u4F4D\uFF1A" + institution
                });
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "PdfParse.ashx_Error:" + ex.Message + "-" + ex.StackTrace);
                WriteJson(context, new { success = false, message = "\u89E3\u6790\u5931\u8D25\uFF1A" + ex.Message });
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(tempPath) && File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
                parseLease.Dispose();
            }
        }

        private bool IsAuthorizedRequest()
        {
            if (IsAdminAuthenticated())
            {
                return true;
            }

            user_list user = CommonUserFunc.GetUserLoginStatus();
            return user != null && user.id > 0;
        }

        private bool IsAdminAuthenticated()
        {
            string adminId = LiteratureManager.Common.Cookie.GetCookie("LMS_AdminID");
            string adminName = LiteratureManager.Common.Cookie.GetCookie("LMS_AdminName");
            string popedom = LiteratureManager.Common.Cookie.GetCookie("LMS_Popedom");
            string adminCode = LiteratureManager.Common.Cookie.GetCookie("LMS_Code");
            if (string.IsNullOrWhiteSpace(adminId) || string.IsNullOrWhiteSpace(adminName) || string.IsNullOrWhiteSpace(popedom) || string.IsNullOrWhiteSpace(adminCode))
            {
                return false;
            }

            BLLBase<Model.admin> adminBll = new BLLBase<Model.admin>();
            Model.admin adminModel = adminBll.SelectSingle("id", Function.ConvertTo<int>(adminId, 0));
            if (adminModel == null || adminModel.id <= 0 || adminModel.locks == 1 || string.IsNullOrWhiteSpace(adminModel.code)
                || !string.Equals(adminModel.username, adminName, StringComparison.Ordinal)
                || !string.Equals(adminModel.popedom, popedom, StringComparison.Ordinal))
            {
                return false;
            }

            return Function.MD5Encrypt(adminModel.code, System.Configuration.ConfigurationManager.AppSettings["md5_key"]).Equals(adminCode);
        }

        private string SaveTempFile(HttpContext context, HttpPostedFile file)
        {
            string baseDirectory = context.Server.MapPath("~/A_UpLoad/upload_file/temp/");
            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }

            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N").Substring(0, 6) + ".pdf";
            string fullPath = Path.Combine(baseDirectory, fileName);
            file.SaveAs(fullPath);
            return fullPath;
        }

        private JObject TryParseByApp(string pdfPath)
        {
            try
            {
                using (WebClient client = new TimeoutWebClient(UploadPolicy.ExternalRequestTimeoutMs))
                {
                    client.Encoding = Encoding.UTF8;
                    byte[] response = client.UploadFile("http://127.0.0.1:5050/upload", "POST", pdfPath);
                    string json = Encoding.UTF8.GetString(response);
                    JObject obj = JObject.Parse(json);
                    if (obj["extracted"] != null && obj["error"] == null)
                    {
                        return obj["extracted"] as JObject;
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        private JObject TryParseByPython(HttpContext context, string pdfPath)
        {
            string webRoot = context.Server.MapPath("~/");
            string projectRoot = Path.GetFullPath(Path.Combine(webRoot, ".."));
            string pythonExe = File.Exists(@"E:\tools\python.exe") ? @"E:\tools\python.exe" : "python";
            string script = "import json,sys;from pdf_parser import extract_paper_info;data=extract_paper_info(sys.argv[1]) or {};print(json.dumps(data, ensure_ascii=True))";
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = pythonExe,
                Arguments = "-c \"" + script.Replace("\"", "\\\"") + "\" \"" + pdfPath + "\"",
                WorkingDirectory = projectRoot,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };
            startInfo.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8";
            startInfo.EnvironmentVariables["PYTHONUTF8"] = "1";

            using (Process process = Process.Start(startInfo))
            {
                System.Threading.Tasks.Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                System.Threading.Tasks.Task<string> errorTask = process.StandardError.ReadToEndAsync();
                if (!process.WaitForExit(UploadPolicy.ExternalRequestTimeoutMs * 2))
                {
                    try
                    {
                        process.Kill();
                    }
                    catch
                    {
                    }
                    throw new TimeoutException("PDF 解析超时，请稍后重试");
                }

                string output = outputTask.GetAwaiter().GetResult();
                string error = errorTask.GetAwaiter().GetResult();

                if (!string.IsNullOrWhiteSpace(output))
                {
                    return JObject.Parse(output);
                }

                if (!string.IsNullOrWhiteSpace(error))
                {
                    if (error.IndexOf("No module named", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        throw new Exception("Python \u89E3\u6790\u73AF\u5883\u7F3A\u5C11\u4F9D\u8D56\uFF1A" + error.Trim());
                    }
                    throw new Exception(error);
                }
            }

            return null;
        }

        private sealed class TimeoutWebClient : WebClient
        {
            private readonly int timeoutMs;

            public TimeoutWebClient(int timeoutMs)
            {
                this.timeoutMs = timeoutMs;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);
                request.Timeout = timeoutMs;
                HttpWebRequest httpRequest = request as HttpWebRequest;
                if (httpRequest != null)
                {
                    httpRequest.ReadWriteTimeout = timeoutMs;
                }
                return request;
            }
        }

        private bool LooksLikeJournal(string value)
        {
            string text = (value ?? string.Empty).ToLower();
            return text.Contains("journal")
                || text.Contains("transactions")
                || text.Contains("review")
                || text.Contains("\u671F\u520A")
                || text.Contains("\u5B66\u62A5");
        }

        private string GetJsonString(JObject obj, string key)
        {
            JToken token = obj[key];
            return token == null ? string.Empty : CleanParsedText(token.ToString());
        }

        private string JoinArray(JObject obj, string key, string separator)
        {
            JToken token = obj[key];
            if (token == null)
            {
                return string.Empty;
            }

            if (token.Type == JTokenType.Array)
            {
                List<string> values = new List<string>();
                foreach (JToken item in token)
                {
                    string raw = item == null ? string.Empty : item.ToString();
                    bool hadGarbledMarker = HasGarbledMarker(raw);
                    string current = CleanParsedText(raw);
                    if (hadGarbledMarker && CountReadableChars(current) < 6)
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(current) && !values.Contains(current))
                    {
                        values.Add(current);
                    }
                }
                return string.Join(separator, values.ToArray());
            }

            return CleanParsedText(token.ToString());
        }

        private bool HasGarbledMarker(string value)
        {
            return Regex.IsMatch(value ?? string.Empty, @"\uFFFD|\?{2,}|\(cid:\d+\)");
        }

        private string CleanParsedText(string value)
        {
            string text = (value ?? string.Empty)
                .Replace('\u0000', ' ')
                .Replace('\uFFFD', ' ')
                .Replace('\u25A0', ' ')
                .Replace('\u25A1', ' ')
                .Replace('\u25CA', ' ')
                .Replace('\u25C6', ' ')
                .Replace('\u25C7', ' ');

            text = Regex.Replace(text, @"\(cid:\d+\)", " ");
            text = Regex.Replace(text, @"\?{2,}", " ");

            StringBuilder builder = new StringBuilder(text.Length);
            foreach (char current in text)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(current);
                if (category == UnicodeCategory.Control && current != '\r' && current != '\n' && current != '\t')
                {
                    builder.Append(' ');
                    continue;
                }

                if (category == UnicodeCategory.PrivateUse
                    || category == UnicodeCategory.Surrogate
                    || category == UnicodeCategory.OtherNotAssigned)
                {
                    builder.Append(' ');
                    continue;
                }

                builder.Append(current);
            }

            text = Regex.Replace(builder.ToString().Replace('\u00A0', ' '), @"[ \t]+", " ");
            text = Regex.Replace(text, @"\s+([,.;:!?])", "$1");
            text = Regex.Replace(text, @"([,;:]){2,}", "$1");
            text = text.Trim(' ', ',', ';', ':', '-');

            return IsReadableParsedText(text) ? text : string.Empty;
        }

        private bool IsReadableParsedText(string value)
        {
            string text = Regex.Replace(value ?? string.Empty, @"\s+", string.Empty);
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            int readable = CountReadableChars(text);

            if (Regex.IsMatch(text, @"^\d+(?:[-/.]\d+)?$"))
            {
                return true;
            }

            return readable >= 2 && readable / (double)text.Length >= 0.35;
        }

        private int CountReadableChars(string value)
        {
            int readable = 0;
            foreach (char current in value ?? string.Empty)
            {
                if (char.IsLetterOrDigit(current) || (current >= '\u4E00' && current <= '\u9FFF'))
                {
                    readable++;
                }
            }

            return readable;
        }

        private string NormalizeParsedInstitution(string value)
        {
            string text = Regex.Replace((value ?? string.Empty).Replace('\u00A0', ' '), @"\s+", " ").Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            string[] items = text.Split(new[] { ';', '\uFF1B', '|', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> values = new List<string>();
            foreach (string item in items)
            {
                string current = Regex.Replace(item, @"\s+", " ").Trim();
                if (IsBadInstitutionCandidate(current))
                {
                    continue;
                }

                if (current.Length > 260)
                {
                    current = current.Substring(0, 260).Trim();
                }

                if (!values.Contains(current))
                {
                    values.Add(current);
                }
            }

            return string.Join("\uFF1B", values.ToArray());
        }

        private bool IsBadInstitutionCandidate(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length < 6)
            {
                return true;
            }

            string lower = value.ToLower();
            if (lower.Contains("final published version")
                || lower.Contains("proceedings is available")
                || lower.Contains("abstract")
                || lower.Contains("keywords")
                || lower.Contains("index terms")
                || lower.Contains("introduction"))
            {
                return true;
            }

            return value.Length > 260 && !LooksLikeAffiliation(value);
        }

        private bool LooksLikeAffiliation(string value)
        {
            string lower = (value ?? string.Empty).ToLower();
            return lower.Contains("university")
                || lower.Contains("institute")
                || lower.Contains("institution")
                || lower.Contains("college")
                || lower.Contains("school")
                || lower.Contains("department")
                || lower.Contains("faculty")
                || lower.Contains("laboratory")
                || lower.Contains("center")
                || lower.Contains("centre")
                || lower.Contains("academy")
                || lower.Contains("hospital")
                || lower.Contains("research")
                || lower.Contains("\u5927\u5B66")
                || lower.Contains("\u5B66\u9662")
                || lower.Contains("\u7814\u7A76")
                || lower.Contains("\u5B9E\u9A8C\u5BA4")
                || lower.Contains("\u4E2D\u5FC3")
                || lower.Contains("\u533B\u9662");
        }

        private void WriteJson(HttpContext context, object data)
        {
            context.Response.Write(JsonConvert.SerializeObject(data));
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
