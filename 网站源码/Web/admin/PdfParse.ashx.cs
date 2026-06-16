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
                JObject parsedResult = ParsePdfMetadata(context, tempPath);
                if (parsedResult == null)
                {
                    WriteJson(context, new { success = false, message = "\u672A\u80FD\u4ECE PDF \u4E2D\u89E3\u6790\u51FA\u6587\u732E\u4FE1\u606F" });
                    return;
                }

                WriteJson(context, parsedResult);
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

        public JObject ParsePdfMetadata(HttpContext context, string pdfPath)
        {
            JObject parsed = TryParseByApp(pdfPath);
            if (parsed != null && ShouldMergePythonFallback(parsed))
            {
                JObject pythonParsed = TryParseByPython(context, pdfPath);
                MergeMissingParsedFields(parsed, pythonParsed);
            }
            if (parsed == null)
            {
                parsed = TryParseByPython(context, pdfPath);
            }
            if (parsed == null)
            {
                return null;
            }

            string title = GetJsonString(parsed, "title");
            string authorNames = JoinAuthorNames(parsed);
            string institution = NormalizeParsedInstitution(JoinArray(parsed, "institutions", "\uFF1B"));
            JArray authorDetails = BuildAuthorDetails(parsed, institution);
            string journalRaw = GetJsonString(parsed, "journal");
            string conferenceRaw = GetJsonString(parsed, "conference");
            string sourceTypeRaw = GetJsonString(parsed, "source_type");
            string doi = GetJsonString(parsed, "doi");
            string publishYear = GetFirstJsonString(parsed, "publish_year", "publication_year", "pub_year", "year");
            string publishMonth = GetFirstJsonString(parsed, "publish_month", "publication_month", "pub_month", "month");
            string publishDay = GetFirstJsonString(parsed, "publish_day", "publication_day", "pub_day", "day");
            string publishDatePrecision = GetFirstJsonString(parsed, "publish_date_precision", "publication_date_precision", "date_precision", "precision");
            ApplyPublishDateFallbacks(parsed, doi, ref publishYear, ref publishMonth, ref publishDay, ref publishDatePrecision);
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

            return JObject.FromObject(new
            {
                success = true,
                title = title,
                author_names = authorNames,
                authors = authorDetails,
                author_details = authorDetails,
                institution = institution,
                doi = doi,
                publish_year = publishYear,
                publish_month = publishMonth,
                publish_day = publishDay,
                publish_date_precision = publishDatePrecision,
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

        private bool ShouldMergePythonFallback(JObject parsed)
        {
            if (parsed == null)
            {
                return true;
            }

            bool missingDoi = !HasAnyJsonValue(parsed, "doi", "DOI");
            bool missingDate = !HasAnyJsonValue(parsed, "publish_year", "publication_year", "pub_year", "year", "publish_date", "publication_date", "published_date", "date");
            return missingDoi || missingDate;
        }

        private bool HasAnyJsonValue(JObject obj, params string[] keys)
        {
            if (obj == null || keys == null)
            {
                return false;
            }

            foreach (string key in keys)
            {
                string value = GetJsonString(obj, key);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return true;
                }
            }

            return false;
        }

        private void MergeMissingParsedFields(JObject target, JObject fallback)
        {
            if (target == null || fallback == null)
            {
                return;
            }

            foreach (JProperty prop in fallback.Properties())
            {
                JToken existing = target[prop.Name];
                if (existing == null || IsEmptyJsonToken(existing))
                {
                    target[prop.Name] = prop.Value.DeepClone();
                }
            }
        }

        private bool IsEmptyJsonToken(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null || token.Type == JTokenType.Undefined)
            {
                return true;
            }

            JArray array = token as JArray;
            if (array != null)
            {
                return array.Count == 0;
            }

            return string.IsNullOrWhiteSpace(token.ToString());
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

        private string GetFirstJsonString(JObject obj, params string[] keys)
        {
            if (obj == null || keys == null)
            {
                return string.Empty;
            }

            foreach (string key in keys)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                string value = GetJsonString(obj, key);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return string.Empty;
        }

        private void ApplyPublishDateFallbacks(JObject parsed, string doi, ref string year, ref string month, ref string day, ref string precision)
        {
            year = NormalizeYearValue(year);
            month = NormalizeMonthValue(month);
            day = NormalizeDayValue(day);
            precision = NormalizePrecisionValue(precision, year, month, day);

            if (!string.IsNullOrWhiteSpace(year))
            {
                return;
            }

            string dateText = GetFirstJsonString(
                parsed,
                "publish_date",
                "publication_date",
                "published_date",
                "published",
                "date",
                "date_published",
                "created",
                "updated");
            ApplyDateTextFallback(dateText, ref year, ref month, ref day, ref precision);

            if (!string.IsNullOrWhiteSpace(year))
            {
                return;
            }

            ApplyDoiDateFallback(doi, ref year, ref month, ref day, ref precision);
        }

        private void ApplyDateTextFallback(string dateText, ref string year, ref string month, ref string day, ref string precision)
        {
            string text = CleanParsedText(dateText);
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            Match match = Regex.Match(text, @"\b((?:19|20)\d{2})[-/.年]\s*(\d{1,2})(?:[-/.月]\s*(\d{1,2}))?", RegexOptions.IgnoreCase);
            if (match.Success && SetDateParts(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, ref year, ref month, ref day, ref precision))
            {
                return;
            }

            string monthPattern = @"(Jan(?:uary)?|Feb(?:ruary)?|Mar(?:ch)?|Apr(?:il)?|May|Jun(?:e)?|Jul(?:y)?|Aug(?:ust)?|Sep(?:t(?:ember)?)?|Oct(?:ober)?|Nov(?:ember)?|Dec(?:ember)?)";
            match = Regex.Match(text, monthPattern + @"\s+(\d{1,2},\s*)?((?:19|20)\d{2})", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string monthValue = MonthNameToNumber(match.Groups[1].Value);
                string dayValue = (match.Groups[2].Value ?? string.Empty).Trim(' ', ',');
                if (SetDateParts(match.Groups[3].Value, monthValue, dayValue, ref year, ref month, ref day, ref precision))
                {
                    return;
                }
            }

            match = Regex.Match(text, @"\b((?:19|20)\d{2})\s+" + monthPattern + @"(?:\s+(\d{1,2}))?\b", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string monthValue = MonthNameToNumber(match.Groups[2].Value);
                if (SetDateParts(match.Groups[1].Value, monthValue, match.Groups[3].Value, ref year, ref month, ref day, ref precision))
                {
                    return;
                }
            }

            match = Regex.Match(text, @"\b((?:19|20)\d{2})\b");
            if (match.Success)
            {
                year = match.Groups[1].Value;
                month = string.Empty;
                day = string.Empty;
                precision = "year";
            }
        }

        private void ApplyDoiDateFallback(string doi, ref string year, ref string month, ref string day, ref string precision)
        {
            string text = (doi ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            Match arxiv = Regex.Match(text, @"arxiv[.:/ ]+(\d{2})(\d{2})\.\d+");
            if (arxiv.Success)
            {
                int yy = Function.ConvertTo<int>(arxiv.Groups[1].Value, 0);
                int mm = Function.ConvertTo<int>(arxiv.Groups[2].Value, 0);
                int fullYear = yy >= 91 ? 1900 + yy : 2000 + yy;
                if (fullYear >= 1990 && fullYear <= 2100 && mm >= 1 && mm <= 12)
                {
                    year = fullYear.ToString();
                    month = mm.ToString();
                    day = string.Empty;
                    precision = "month";
                    return;
                }
            }

            Match yearOnly = Regex.Match(text, @"\b((?:19|20)\d{2})\b");
            if (yearOnly.Success)
            {
                year = yearOnly.Groups[1].Value;
                month = string.Empty;
                day = string.Empty;
                precision = "year";
            }
        }

        private bool SetDateParts(string yearValue, string monthValue, string dayValue, ref string year, ref string month, ref string day, ref string precision)
        {
            string normalizedYear = NormalizeYearValue(yearValue);
            string normalizedMonth = NormalizeMonthValue(monthValue);
            string normalizedDay = NormalizeDayValue(dayValue);
            if (string.IsNullOrWhiteSpace(normalizedYear))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(normalizedMonth))
            {
                int yearNumber = Function.ConvertTo<int>(normalizedYear, 0);
                int monthNumber = Function.ConvertTo<int>(normalizedMonth, 0);
                int dayNumber = Function.ConvertTo<int>(normalizedDay, 0);
                if (dayNumber > 0 && dayNumber > DateTime.DaysInMonth(yearNumber, monthNumber))
                {
                    normalizedDay = string.Empty;
                }
            }

            year = normalizedYear;
            month = normalizedMonth;
            day = string.IsNullOrWhiteSpace(month) ? string.Empty : normalizedDay;
            precision = NormalizePrecisionValue(string.Empty, year, month, day);
            return true;
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
            string text = CleanParsedText(value);
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

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

        private string NormalizePrecisionValue(string value, string year, string month, string day)
        {
            string text = (value ?? string.Empty).Trim().ToLowerInvariant();
            if (text == "day" || text == "month" || text == "year" || text == "unknown")
            {
                return text;
            }
            if (!string.IsNullOrWhiteSpace(day))
            {
                return "day";
            }
            if (!string.IsNullOrWhiteSpace(month))
            {
                return "month";
            }
            return string.IsNullOrWhiteSpace(year) ? "unknown" : "year";
        }

        private string MonthNameToNumber(string value)
        {
            string text = (value ?? string.Empty).Trim().ToLowerInvariant();
            if (text.Length < 3)
            {
                return string.Empty;
            }

            string key = text.Substring(0, 3);
            switch (key)
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

        private string JoinAuthorNames(JObject obj)
        {
            List<string> values = ReadAuthorNames(obj == null ? null : obj["authors"]);
            return string.Join(", ", values.ToArray());
        }

        private List<string> ReadAuthorNames(JToken token)
        {
            List<string> values = new List<string>();
            if (token == null)
            {
                return values;
            }

            if (token.Type == JTokenType.Array)
            {
                foreach (JToken item in token)
                {
                    string name = ReadAuthorName(item);
                    AddUnique(values, name);
                }
                return values;
            }

            AddUnique(values, CleanParsedText(token.ToString()));
            return values;
        }

        private string ReadAuthorName(JToken token)
        {
            if (token == null)
            {
                return string.Empty;
            }

            if (token.Type == JTokenType.Object)
            {
                JObject obj = (JObject)token;
                string name = GetObjectString(obj, "name");
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }

                string nameCn = GetObjectString(obj, "name_cn");
                string nameEn = GetObjectString(obj, "name_en");
                return !string.IsNullOrWhiteSpace(nameCn) ? nameCn : nameEn;
            }

            return CleanParsedText(token.ToString());
        }

        private JArray BuildAuthorDetails(JObject obj, string institution)
        {
            JArray result = new JArray();
            if (obj != null)
            {
                JToken detailsToken = obj["author_details"] ?? obj["author_affiliations"];
                if (detailsToken != null && detailsToken.Type == JTokenType.Array)
                {
                    foreach (JToken item in detailsToken)
                    {
                        JObject normalized = NormalizeAuthorDetail(item);
                        if (normalized != null)
                        {
                            result.Add(normalized);
                        }
                    }
                }
            }

            if (result.Count > 0)
            {
                return result;
            }

            List<string> authorNames = ReadAuthorNames(obj == null ? null : obj["authors"]);
            List<string> institutions = SplitInstitutionValues(institution);
            foreach (string authorName in authorNames)
            {
                List<string> affiliations = institutions.Count == 1 ? new List<string>(institutions) : new List<string>();
                result.Add(CreateAuthorDetail(authorName, affiliations, new List<string>(), affiliations.Count > 0 ? "single_institution" : "unmatched"));
            }

            return result;
        }

        private JObject NormalizeAuthorDetail(JToken token)
        {
            if (token == null)
            {
                return null;
            }

            if (token.Type != JTokenType.Object)
            {
                string simpleName = CleanParsedText(token.ToString());
                return string.IsNullOrWhiteSpace(simpleName) ? null : CreateAuthorDetail(simpleName, new List<string>(), new List<string>(), "unmatched");
            }

            JObject source = (JObject)token;
            string name = GetObjectString(source, "name");
            string nameCn = GetObjectString(source, "name_cn");
            string nameEn = GetObjectString(source, "name_en");
            if (string.IsNullOrWhiteSpace(name))
            {
                name = !string.IsNullOrWhiteSpace(nameCn) ? nameCn : nameEn;
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            List<string> affiliations = ReadCleanStringArray(source["affiliations"]);
            string affiliationText = GetObjectString(source, "affiliation_text");
            if (affiliations.Count == 0 && !string.IsNullOrWhiteSpace(affiliationText))
            {
                affiliations = SplitInstitutionValues(affiliationText);
            }

            List<string> markers = ReadCleanStringArray(source["markers"]);
            string mappingStatus = GetObjectString(source, "mapping_status");
            if (string.IsNullOrWhiteSpace(mappingStatus))
            {
                mappingStatus = affiliations.Count > 0 ? "matched" : "unmatched";
            }

            JObject detail = CreateAuthorDetail(name, affiliations, markers, mappingStatus);
            if (!string.IsNullOrWhiteSpace(nameCn))
            {
                detail["name_cn"] = nameCn;
            }
            if (!string.IsNullOrWhiteSpace(nameEn))
            {
                detail["name_en"] = nameEn;
            }
            return detail;
        }

        private JObject CreateAuthorDetail(string name, List<string> affiliations, List<string> markers, string mappingStatus)
        {
            string cleanName = CleanParsedText(name);
            string nameCn = ContainsChinese(cleanName) ? cleanName : string.Empty;
            string nameEn = string.IsNullOrWhiteSpace(nameCn) ? cleanName : string.Empty;

            JObject detail = new JObject();
            detail["name"] = cleanName;
            detail["name_cn"] = nameCn;
            detail["name_en"] = nameEn;
            detail["affiliations"] = ToJArray(affiliations);
            detail["affiliation_text"] = string.Join("\uFF1B", affiliations.ToArray());
            detail["markers"] = ToJArray(markers);
            detail["mapping_status"] = string.IsNullOrWhiteSpace(mappingStatus) ? "unmatched" : mappingStatus;
            return detail;
        }

        private string GetObjectString(JObject obj, string key)
        {
            if (obj == null || obj[key] == null)
            {
                return string.Empty;
            }

            return CleanParsedText(obj[key].ToString());
        }

        private List<string> ReadCleanStringArray(JToken token)
        {
            List<string> values = new List<string>();
            if (token == null)
            {
                return values;
            }

            if (token.Type == JTokenType.Array)
            {
                foreach (JToken item in token)
                {
                    AddUnique(values, CleanParsedText(item == null ? string.Empty : item.ToString()));
                }
                return values;
            }

            AddUnique(values, CleanParsedText(token.ToString()));
            return values;
        }

        private List<string> SplitInstitutionValues(string value)
        {
            List<string> values = new List<string>();
            string[] parts = (value ?? string.Empty).Split(new[] { ';', '\uFF1B', '|', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                AddUnique(values, CleanParsedText(part));
            }
            return values;
        }

        private JArray ToJArray(List<string> values)
        {
            JArray array = new JArray();
            foreach (string value in values ?? new List<string>())
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    array.Add(value);
                }
            }
            return array;
        }

        private void AddUnique(List<string> values, string value)
        {
            string current = CleanParsedText(value);
            if (string.IsNullOrWhiteSpace(current))
            {
                return;
            }

            foreach (string existing in values)
            {
                if (string.Equals(existing, current, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            values.Add(current);
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
