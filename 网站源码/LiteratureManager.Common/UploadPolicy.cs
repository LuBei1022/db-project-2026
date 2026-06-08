using System;
using System.Configuration;

namespace LiteratureManager.Common
{
    public static class UploadPolicy
    {
        public static int MaxPdfBytes
        {
            get { return GetMegabytes("UploadMaxPdfMb", 50); }
        }

        public static int MaxImageBytes
        {
            get { return GetMegabytes("UploadMaxImageMb", 10); }
        }

        public static int MaxAttachmentBytes
        {
            get { return GetMegabytes("UploadMaxAttachmentMb", 100); }
        }

        public static int MaxBatchFiles
        {
            get { return GetPositiveInt("UploadBatchMaxFiles", 20); }
        }

        public static long MaxBatchTotalBytes
        {
            get { return (long)GetMegabytes("UploadBatchMaxTotalMb", 200); }
        }

        public static int MaxBatchDownloadFiles
        {
            get { return GetPositiveInt("DownloadBatchMaxFiles", 10); }
        }

        public static long MaxBatchDownloadTotalBytes
        {
            get { return (long)GetMegabytes("DownloadBatchMaxTotalMb", 200); }
        }

        public static int MaxImportBytes
        {
            get { return GetMegabytes("ImportMaxMb", 10); }
        }

        public static int MaxImportRows
        {
            get { return GetPositiveInt("ImportMaxRows", 5000); }
        }

        public static int UserApiMaxRequestBytes
        {
            get { return GetKilobytes("UserApiMaxRequestKb", 64); }
        }

        public static int MaxCommentLength
        {
            get { return GetPositiveInt("CommentMaxLength", 2000); }
        }

        public static int SmsCooldownSeconds
        {
            get { return GetPositiveInt("SmsCooldownSeconds", 60); }
        }

        public static int CommentCooldownSeconds
        {
            get { return GetPositiveInt("CommentCooldownSeconds", 15); }
        }

        public static int ExternalRequestTimeoutMs
        {
            get { return GetPositiveInt("ExternalRequestTimeoutMs", 10000); }
        }

        public static int MaxPdfParseConcurrent
        {
            get { return GetPositiveInt("PdfParseMaxConcurrent", 2); }
        }

        public static bool RedisEnabled
        {
            get
            {
                bool configured;
                return bool.TryParse(ConfigurationManager.AppSettings["RedisEnabled"], out configured) && configured;
            }
        }

        public static string RedisHost
        {
            get { return ConfigurationManager.AppSettings["RedisHost"] ?? "127.0.0.1"; }
        }

        public static int RedisPort
        {
            get { return GetPositiveInt("RedisPort", 6379); }
        }

        public static int RedisDatabase
        {
            get { return GetNonNegativeInt("RedisDatabase", 0); }
        }

        public static string RedisPassword
        {
            get { return ConfigurationManager.AppSettings["RedisPassword"] ?? string.Empty; }
        }

        public static int RedisTimeoutMs
        {
            get { return GetPositiveInt("RedisTimeoutMs", 500); }
        }

        public static int PdfParseLeaseSeconds
        {
            get { return GetPositiveInt("PdfParseLeaseSeconds", 600); }
        }

        public static string RedisKeyPrefix
        {
            get
            {
                string value = ConfigurationManager.AppSettings["RedisKeyPrefix"];
                return string.IsNullOrWhiteSpace(value) ? "manage" : value.Trim();
            }
        }

        public static string ToMbLabel(long bytes)
        {
            return Math.Ceiling(bytes / 1024d / 1024d).ToString("0") + " MB";
        }

        private static int GetMegabytes(string key, int fallbackMb)
        {
            long bytes = (long)GetPositiveInt(key, fallbackMb) * 1024L * 1024L;
            return bytes > int.MaxValue ? int.MaxValue : (int)bytes;
        }

        private static int GetKilobytes(string key, int fallbackKb)
        {
            long bytes = (long)GetPositiveInt(key, fallbackKb) * 1024L;
            return bytes > int.MaxValue ? int.MaxValue : (int)bytes;
        }

        private static int GetPositiveInt(string key, int fallback)
        {
            int configured;
            return int.TryParse(ConfigurationManager.AppSettings[key], out configured) && configured > 0
                ? configured
                : fallback;
        }

        private static int GetNonNegativeInt(string key, int fallback)
        {
            int configured;
            return int.TryParse(ConfigurationManager.AppSettings[key], out configured) && configured >= 0
                ? configured
                : fallback;
        }
    }
}
