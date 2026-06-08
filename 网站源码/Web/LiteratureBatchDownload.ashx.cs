using BLL;
using LiteratureManager.Common;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;

namespace Web
{
    public class LiteratureBatchDownload : IHttpHandler
    {
        private readonly BLLBase<Literature> literatureBll = new BLLBase<Literature>();
        private readonly BLLBase<LiteratureDownloadLog> downloadLogBll = new BLLBase<LiteratureDownloadLog>();
        private readonly BLLBase<integrateExchangeLog_list> exchangeLogBll = new BLLBase<integrateExchangeLog_list>();

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            user_list user = CommonUserFunc.GetUserLoginStatus();
            if (user == null || user.id <= 0)
            {
                WriteMessage(context, "\u8BF7\u5148\u767B\u5F55\u540E\u518D\u6279\u91CF\u4E0B\u8F7D PDF\uFF01");
                return;
            }

            List<int> ids = GetSelectedIds(context.Request.Form.GetValues("literature_ids"));
            if (ids.Count == 0)
            {
                WriteMessage(context, "\u8BF7\u5148\u9009\u62E9\u8981\u4E0B\u8F7D\u7684 PDF\uFF01");
                return;
            }
            if (ids.Count > UploadPolicy.MaxBatchDownloadFiles)
            {
                WriteMessage(context, "\u5355\u6B21\u6279\u91CF\u4E0B\u8F7D\u6700\u591A\u652F\u6301 " + UploadPolicy.MaxBatchDownloadFiles + " \u4E2A PDF\uFF01");
                return;
            }

            List<DownloadItem> items = LoadDownloadItems(context, ids);
            if (items.Count == 0)
            {
                WriteMessage(context, "\u9009\u4E2D\u6587\u732E\u6682\u65E0\u53EF\u4E0B\u8F7D\u7684 PDF\u9644\u4EF6\uFF01");
                return;
            }

            long totalBytes = items.Sum(item => item.FileSize);
            if (totalBytes > UploadPolicy.MaxBatchDownloadTotalBytes)
            {
                WriteMessage(context, "单次批量下载文件总大小不能超过 " + UploadPolicy.ToMbLabel(UploadPolicy.MaxBatchDownloadTotalBytes) + "！");
                return;
            }

            string payMethod = (context.Request.Form["pay_method"] ?? string.Empty).Trim();
            AuthorizeItems(context, user, items, payMethod);
            List<DownloadItem> pendingChargeItems = items.Where(item => !item.AlreadyPurchased && item.ChargedPoints > 0).ToList();
            int requiredPoints = pendingChargeItems.Sum(item => item.ChargedPoints);
            if (requiredPoints > 0)
            {
                int userPoints = CommonUserFunc.GetUserIntegrateSumFunc(user.id, 0);
                if (requiredPoints > userPoints)
                {
                    WriteMessage(context, "\u79EF\u5206\u4E0D\u8DB3\uFF0C\u6279\u91CF\u4E0B\u8F7D\u9700\u8981 " + requiredPoints + " \u79EF\u5206\uFF01");
                    return;
                }
            }

            string zipPath = CreateZipFile(context, items);
            try
            {
                StringBuilder chargeSql = new StringBuilder();
                foreach (DownloadItem item in items)
                {
                    if (!item.AlreadyPurchased)
                    {
                        AppendDownloadLogSql(chargeSql, item.Literature, user.id, item.ChargedPoints, true, item.CouponId, item.RelativePath);
                    }
                }
                if (chargeSql.Length > 0 && !downloadLogBll.Sql_D(chargeSql.ToString()))
                {
                    WriteMessage(context, "\u79EF\u5206\u6216\u514D\u8D39\u4E0B\u8F7D\u5238\u72B6\u6001\u5DF2\u53D8\u66F4\uFF0C\u8BF7\u5237\u65B0\u540E\u91CD\u8BD5\uFF01");
                    return;
                }

                SendZip(context, zipPath);
            }
            finally
            {
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }
            }
        }

        private List<int> GetSelectedIds(string[] values)
        {
            List<int> ids = new List<int>();
            if (values == null)
            {
                return ids;
            }

            foreach (string value in values)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                foreach (string part in value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int id = Function.ConvertTo<int>(part.Trim(), 0);
                    if (id > 0 && !ids.Contains(id))
                    {
                        ids.Add(id);
                    }
                    if (ids.Count > UploadPolicy.MaxBatchDownloadFiles)
                    {
                        return ids;
                    }
                }
            }
            return ids;
        }

        private List<DownloadItem> LoadDownloadItems(HttpContext context, List<int> ids)
        {
            string idList = string.Join(",", ids);
            string sql = @"
select
    l.*,
    f.file_path,
    f.file_name
from Literature l
inner join LiteratureFile f on f.literature_id=l.id and f.status=1
where l.status=1 and l.canonical_literature_id is null and l.id in (" + idList + @")
and f.id=(select top 1 id from LiteratureFile where literature_id=l.id and status=1 order by orderid asc,id asc)
order by l.is_top desc,l.addtime desc,l.id desc";

            DataTable dt = literatureBll.GetDatatable(sql);
            List<DownloadItem> items = new List<DownloadItem>();
            if (dt == null || dt.Rows.Count == 0)
            {
                return items;
            }

            string uploadRoot = context.Server.MapPath("~/A_UpLoad/upload_file/");
            foreach (DataRow row in dt.Rows)
            {
                string relativePath = Function.HtmlDiscode(Convert.ToString(row["file_path"])).Replace("\\", "/").TrimStart('/');
                string fullPath = Path.GetFullPath(Path.Combine(uploadRoot, relativePath.Replace("/", "\\")));
                if (!fullPath.StartsWith(Path.GetFullPath(uploadRoot), StringComparison.OrdinalIgnoreCase) || !File.Exists(fullPath))
                {
                    continue;
                }

                Literature literature = new Literature();
                literature.id = Function.ConvertTo<int>(Convert.ToString(row["id"]), 0);
                literature.title = Convert.ToString(row["title"]);
                literature.download_points = Function.ConvertTo<int>(Convert.ToString(row["download_points"]), 0);
                literature.userid = Function.ConvertTo<int>(Convert.ToString(row["userid"]), 0);
                literature.status = Function.ConvertTo<int>(Convert.ToString(row["status"]), 0);

                items.Add(new DownloadItem
                {
                    Literature = literature,
                    RelativePath = relativePath,
                    FullPath = fullPath,
                    FileName = Function.HtmlDiscode(Convert.ToString(row["file_name"])),
                    FileSize = new FileInfo(fullPath).Length
                });
            }
            return items;
        }

        private void AuthorizeItems(HttpContext context, user_list user, List<DownloadItem> items, string payMethod)
        {
            foreach (DownloadItem item in items)
            {
                LiteratureDownloadLog log = downloadLogBll.SelectSingle("user_id=" + user.id + " and literature_id=" + item.Literature.id);
                item.AlreadyPurchased = log != null && log.id > 0;
                bool isUploader = item.Literature.userid == user.id;
                item.ChargedPoints = item.AlreadyPurchased || isUploader ? 0 : Math.Max(0, item.Literature.download_points);
            }

            if (!string.Equals(payMethod, "coupon", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            List<long> couponIds = GetAvailableCouponIds(user.id);
            int couponIndex = 0;
            foreach (DownloadItem item in items.Where(i => !i.AlreadyPurchased && i.ChargedPoints > 0))
            {
                if (couponIndex >= couponIds.Count)
                {
                    break;
                }

                item.CouponId = couponIds[couponIndex];
                item.ChargedPoints = 0;
                couponIndex++;
            }
        }

        private List<long> GetAvailableCouponIds(int userId)
        {
            DataTable dt = exchangeLogBll.GetDatatable("select id from integrateExchangeLog_list where user_id=" + userId + " and status=1 and name like N'%\u514D\u8D39\u4E0B\u8F7D%' order by id asc");
            List<long> ids = new List<long>();
            if (dt == null)
            {
                return ids;
            }
            foreach (DataRow row in dt.Rows)
            {
                long id = Function.ConvertTo<long>(Convert.ToString(row["id"]), 0);
                if (id > 0)
                {
                    ids.Add(id);
                }
            }
            return ids;
        }

        private void AppendDownloadLogSql(StringBuilder sql, Literature literature, int userId, int chargedPoints, bool grantUploaderPoints, long couponId, string pdfFile)
        {
            string safeTitle = Function.HtmlEncode(Function.HtmlDiscode(literature.title)).Replace("'", "''");
            AppendDownloadAuthorizationStart(sql, userId, chargedPoints, couponId);
            sql.Append("INSERT INTO LiteratureDownloadLog(literature_id,user_id,literature_title,file_url,download_points,literature_user_id,addtime) VALUES (");
            sql.Append(literature.id);
            sql.Append(",");
            sql.Append(userId);
            sql.Append(",N'");
            sql.Append(safeTitle);
            sql.Append("',N'");
            sql.Append(Function.HtmlEncode(pdfFile).Replace("'", "''"));
            sql.Append("',");
            sql.Append(chargedPoints);
            sql.Append(",");
            sql.Append(literature.userid);
            sql.Append(",GETDATE());");

            if (couponId > 0)
            {
                sql.Append(";UPDATE integrateExchangeLog_list SET status=-1,hexiaotime='");
                sql.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sql.Append("' WHERE id=");
                sql.Append(couponId);
                sql.Append(" AND user_id=");
                sql.Append(userId);
                sql.Append(" AND status=1");
            }

            if (chargedPoints > 0)
            {
                sql.Append(";INSERT INTO integrateLog_list (num_integrate, type, name, info_, addtime, user_id) VALUES (-");
                sql.Append(chargedPoints);
                sql.Append(",4,N'\u6587\u732E\u6279\u91CF\u4E0B\u8F7D',N'\u6279\u91CF\u4E0B\u8F7D\u6587\u732E\u300A");
                sql.Append(safeTitle);
                sql.Append("\u300B\u6263\u9664");
                sql.Append(chargedPoints);
                sql.Append("\u79EF\u5206',GETDATE(),");
                sql.Append(userId);
                sql.Append(")");

                if (grantUploaderPoints && literature.userid > 0 && literature.userid != userId)
                {
                    sql.Append(";INSERT INTO integrateLog_list (num_integrate, type, name, info_, addtime, user_id) VALUES (");
                    sql.Append(chargedPoints);
                    sql.Append(",5,N'\u6587\u732E\u88AB\u6279\u91CF\u4E0B\u8F7D',N'\u60A8\u7684\u6587\u732E\u300A");
                    sql.Append(safeTitle);
                    sql.Append("\u300B\u88AB\u4E0B\u8F7D\uFF0C\u83B7\u5F97");
                    sql.Append(chargedPoints);
                    sql.Append("\u79EF\u5206',GETDATE(),");
                    sql.Append(literature.userid);
                    sql.Append(")");
                }
            }

            AppendDownloadAuthorizationEnd(sql, chargedPoints, couponId);
        }

        private void AppendDownloadAuthorizationStart(StringBuilder sql, int userId, int chargedPoints, long couponId)
        {
            if (chargedPoints > 0)
            {
                sql.Append("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; IF ((SELECT ISNULL(SUM(num_integrate),0) FROM integrateLog_list WITH (UPDLOCK,HOLDLOCK) WHERE user_id=");
                sql.Append(userId);
                sql.Append(") >= ");
                sql.Append(chargedPoints);
                sql.Append(") BEGIN ");
            }
            else if (couponId > 0)
            {
                sql.Append("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; IF EXISTS(SELECT 1 FROM integrateExchangeLog_list WITH (UPDLOCK,HOLDLOCK) WHERE id=");
                sql.Append(couponId);
                sql.Append(" AND user_id=");
                sql.Append(userId);
                sql.Append(" AND status=1) BEGIN ");
            }
        }

        private void AppendDownloadAuthorizationEnd(StringBuilder sql, int chargedPoints, long couponId)
        {
            if (chargedPoints > 0 || couponId > 0)
            {
                sql.Append("; END ELSE BEGIN RAISERROR(N'\u4E0B\u8F7D\u652F\u4ED8\u72B6\u6001\u5DF2\u53D8\u66F4',16,1); END; ");
            }
        }

        private string CreateZipFile(HttpContext context, List<DownloadItem> items)
        {
            string zipRoot = context.Server.MapPath("~/A_UpLoad/upload_file/temp/");
            if (!Directory.Exists(zipRoot))
            {
                Directory.CreateDirectory(zipRoot);
            }
            CleanupExpiredZipFiles(zipRoot);
            string zipPath = Path.Combine(zipRoot, "literature-pdf-" + Guid.NewGuid().ToString("N") + ".zip");
            try
            {
                using (FileStream stream = File.Create(zipPath))
                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create, false, Encoding.UTF8))
                {
                    HashSet<string> usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (DownloadItem item in items)
                    {
                        string entryName = GetZipEntryName(item, usedNames);
                        ZipArchiveEntry entry = archive.CreateEntry(entryName, CompressionLevel.Fastest);
                        using (Stream entryStream = entry.Open())
                        using (FileStream fileStream = File.OpenRead(item.FullPath))
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    }
                }
                return zipPath;
            }
            catch
            {
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }
                throw;
            }
        }

        private void CleanupExpiredZipFiles(string zipRoot)
        {
            try
            {
                foreach (string file in Directory.GetFiles(zipRoot, "literature-pdf-*.zip"))
                {
                    if (File.GetLastWriteTimeUtc(file) < DateTime.UtcNow.AddHours(-1))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch
            {
                // Cleanup is best effort and must not block the current download.
            }
        }

        private void SendZip(HttpContext context, string zipPath)
        {
            FileInfo file = new FileInfo(zipPath);
            context.Response.Clear();
            context.Response.ContentType = "application/zip";
            context.Response.AddHeader("Content-Disposition", "attachment; filename=\"literature-pdf-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip\"");
            context.Response.AddHeader("Content-Length", file.Length.ToString());
            using (FileStream fileStream = File.OpenRead(zipPath))
            {
                fileStream.CopyTo(context.Response.OutputStream);
            }
            context.Response.Flush();
            context.ApplicationInstance.CompleteRequest();
        }

        private string GetZipEntryName(DownloadItem item, HashSet<string> usedNames)
        {
            string title = Function.HtmlDiscode(item.Literature.title);
            string fileName = string.IsNullOrWhiteSpace(item.FileName) ? Path.GetFileName(item.RelativePath) : item.FileName;
            string baseName = string.IsNullOrWhiteSpace(title) ? Path.GetFileNameWithoutExtension(fileName) : title;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                baseName = baseName.Replace(c, '_');
            }
            if (string.IsNullOrWhiteSpace(baseName))
            {
                baseName = "literature-" + item.Literature.id;
            }

            string entryName = baseName + ".pdf";
            int index = 2;
            while (usedNames.Contains(entryName))
            {
                entryName = baseName + "-" + index + ".pdf";
                index++;
            }
            usedNames.Add(entryName);
            return entryName;
        }

        private void WriteMessage(HttpContext context, string message)
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.Write("<script>alert('" + HttpUtility.JavaScriptStringEncode(message) + "');history.back();</script>");
            context.ApplicationInstance.CompleteRequest();
        }

        private class DownloadItem
        {
            public Literature Literature { get; set; }
            public string RelativePath { get; set; }
            public string FullPath { get; set; }
            public string FileName { get; set; }
            public long FileSize { get; set; }
            public bool AlreadyPurchased { get; set; }
            public int ChargedPoints { get; set; }
            public long CouponId { get; set; }
        }
    }
}
