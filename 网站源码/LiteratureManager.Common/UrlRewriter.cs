using BLL;
using Model;
using System;
using System.Text.RegularExpressions;
using System.Web;

namespace LiteratureManager.Common
{
    public class UrlRewriter : IHttpHandler
    {
        public void ProcessRequest(HttpContext Context)
        {
            string fallbackPath = @"~/err.aspx";

            try
            {
                BLLBase<tbl_class> tbl_classbll = new BLLBase<tbl_class>();
                BLLBase<data_list> data_listbll = new BLLBase<data_list>();
                BLLBase<indexsingle_list> indexsingle_listbll = new BLLBase<indexsingle_list>();

                string rawUrl = Context.Request.RawUrl ?? string.Empty;

                if (rawUrl.IndexOf("/LoginOut", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Context.Server.Execute(@"~/LoginOut.aspx");
                    return;
                }

                if (rawUrl.IndexOf("/User/", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    RewriteUserCenter(Context, rawUrl);
                    return;
                }

                if (rawUrl.IndexOf("/Search", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    string queryString = GetQueryString(rawUrl);
                    Context.Server.Execute(@"~/Search.aspx" + queryString);
                    return;
                }

                if (rawUrl.IndexOf("/Website/AdSingle_", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    string realPath = fallbackPath;
                    Match match = Regex.Match(rawUrl, @"^/Website/AdSingle_(\d+)$", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        int dataId = Function.ConvertTo<int>(match.Groups[1].Value, 0);
                        indexsingle_list item = indexsingle_listbll.SelectSingle("id=" + dataId + " and isshow=1");
                        if (item != null && item.id > 0)
                        {
                            realPath = @"~/WebsiteData/AdSingle.aspx?id=" + item.id;
                        }
                    }

                    Context.Server.Execute(realPath);
                    return;
                }

                if (rawUrl.IndexOf("/Website/Info_", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    string realPath = fallbackPath;
                    Match match = Regex.Match(rawUrl, @"^/Website/Info_(\d+)$", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        int dataId = Function.ConvertTo<int>(match.Groups[1].Value, 0);
                        data_list item = data_listbll.SelectSingle("id=" + dataId + " and isshow=1");
                        if (item != null && item.id > 0)
                        {
                            realPath = @"~/WebsiteData/NewsInfo.aspx?id=" + item.id;
                        }
                    }

                    Context.Server.Execute(realPath);
                    return;
                }

                if (rawUrl.IndexOf("/Website/", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    RewriteWebsite(Context, rawUrl, tbl_classbll, fallbackPath);
                    return;
                }

                Context.Server.Execute(rawUrl);
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "Error:" + ex.Message + "-" + ex.StackTrace);
                Context.Server.Execute(fallbackPath);
            }
        }

        private static void RewriteUserCenter(HttpContext context, string rawUrl)
        {
            string[] parts = rawUrl.Split(new[] { "?" }, StringSplitOptions.None);
            string queryString = parts.Length > 1 ? "?" + parts[1] : string.Empty;
            string route = parts[0].Replace("/User/", string.Empty);

            if (route == "ServiceLog")
            {
                context.Server.Execute(@"~/UserCenter/ServiceLog.aspx" + queryString);
                return;
            }

            string[] directPages =
            {
                "Account",
                "Center",
                "ServiceLogAdd",
                "NoticeLog",
                "Appeal",
                "MsgLog",
                "IntegrateExchange",
                "IntegrateExchangeLog",
                "IntegrateLog",
                "IntegrateWithdrawal",
                "LiteratureUpload"
            };

            foreach (string page in directPages)
            {
                if (string.Equals(route, page, StringComparison.OrdinalIgnoreCase))
                {
                    context.Server.Execute(@"~/UserCenter/" + page + ".aspx" + queryString);
                    return;
                }
            }

            Match serviceLogMatch = Regex.Match(route, @"^ServiceLog_(\d+)$", RegexOptions.IgnoreCase);
            if (serviceLogMatch.Success)
            {
                int id = Function.ConvertTo<int>(serviceLogMatch.Groups[1].Value, 0);
                context.Server.Execute(@"~/UserCenter/ServiceLogInfo.aspx?id=" + id);
                return;
            }

            context.Server.Execute(@"~/err.aspx");
        }

        private static void RewriteWebsite(HttpContext context, string rawUrl, BLLBase<tbl_class> tbl_classbll, string fallbackPath)
        {
            string[] parts = rawUrl.Split(new[] { "?" }, StringSplitOptions.None);
            string queryString = parts.Length > 1 ? "?" + parts[1] : string.Empty;
            string route = parts[0].Replace("/Website/", string.Empty);

            if (string.IsNullOrWhiteSpace(route))
            {
                context.Server.Execute(fallbackPath + queryString);
                return;
            }

            if (string.Equals(route, "news", StringComparison.OrdinalIgnoreCase))
            {
                context.Server.Execute(@"~/WebsiteData/News.aspx" + queryString);
                return;
            }

            tbl_class item = tbl_classbll.SelectSingle(
                "urlnamebtn='" + Function.HtmlEncode(route) + "' and isshow=1 and id in(" + Function.Decrypt(CommonFunc.GetChildrenId(360)) + ")");

            if (item == null || item.id <= 0)
            {
                context.Server.Execute(fallbackPath + queryString);
                return;
            }

            string target = CommonFunc.GetHtmlHrefUrl(item.model, item.id);
            if (string.IsNullOrWhiteSpace(target))
            {
                context.Server.Execute(fallbackPath + queryString);
                return;
            }

            if (!string.IsNullOrWhiteSpace(queryString) && queryString.StartsWith("?", StringComparison.Ordinal))
            {
                queryString = "&" + queryString.Substring(1);
            }

            context.Server.Execute(@"~/" + target + queryString);
        }

        private static string GetQueryString(string rawUrl)
        {
            string[] parts = rawUrl.Split(new[] { "?" }, StringSplitOptions.None);
            return parts.Length > 1 ? "?" + parts[1] : string.Empty;
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
