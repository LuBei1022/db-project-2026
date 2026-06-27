using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace LiteratureManager.Common
{
    public static class LiteratureRagSync
    {
        public static void QueueReindex(int literatureId)
        {
            if (literatureId <= 0 || !IsEnabled())
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    Reindex(literatureId);
                }
                catch (Exception ex)
                {
                    ImportDataLog.WriteLog(LogType.Error, "LiteratureRagSync.QueueReindex:" + ex.Message);
                }
            });
        }

        public static bool Reindex(int literatureId)
        {
            if (literatureId <= 0 || !IsEnabled())
            {
                return false;
            }

            string baseUrl = GetServiceUrl();
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return false;
            }

            string url = baseUrl.TrimEnd('/') + "/rag/index_paper";
            string json = "{\"paper_id\":" + literatureId + "}";
            byte[] payload = Encoding.UTF8.GetBytes(json);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Accept = "application/json";
            request.Timeout = UploadPolicy.ExternalRequestTimeoutMs;
            request.ReadWriteTimeout = UploadPolicy.ExternalRequestTimeoutMs;
            request.ContentLength = payload.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(payload, 0, payload.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;
            }
        }

        private static bool IsEnabled()
        {
            string value = ConfigurationManager.AppSettings["rag_auto_index_enabled"];
            return string.IsNullOrWhiteSpace(value) || !"false".Equals(value.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private static string GetServiceUrl()
        {
            string value = ConfigurationManager.AppSettings["rag_service_url"];
            return string.IsNullOrWhiteSpace(value) ? "http://127.0.0.1:5051" : value.Trim();
        }
    }
}
