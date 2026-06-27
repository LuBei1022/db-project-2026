using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Web.Inc
{
    /// <summary>
    /// RAG 智能问答中转接口。
    /// 浏览器请求本接口（同源，无跨域问题），由它在服务端转发到 Python RAG 服务。
    ///
    /// 用法：
    ///   GET  /Inc/RagApi.ashx?action=search&title=关键词   → 转发到 {rag}/rag/search_paper
    ///   POST /Inc/RagApi.ashx?action=ask  (JSON: paper_id, question) → 转发到 {rag}/rag/ask
    ///
    /// Python 服务地址在 Web.config 的 appSettings["rag_service_url"] 配置，
    /// 默认 http://localhost:5050 。
    /// </summary>
    public class RagApi : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json;charset=UTF-8";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            string ragBase = (ConfigurationManager.AppSettings["rag_service_url"] ?? "http://localhost:5050").TrimEnd('/');
            string action = (context.Request["action"] ?? string.Empty).Trim().ToLowerInvariant();

            try
            {
                if (action == "search")
                {
                    string title = context.Request["title"] ?? string.Empty;
                    string url = ragBase + "/rag/search_paper?title=" + HttpUtility.UrlEncode(title, Encoding.UTF8);
                    context.Response.Write(ForwardGet(url));
                }
                else if (action == "ask")
                {
                    string body;
                    using (StreamReader reader = new StreamReader(context.Request.InputStream, Encoding.UTF8))
                    {
                        body = reader.ReadToEnd();
                    }
                    string url = ragBase + "/rag/ask";
                    context.Response.Write(ForwardPost(url, body));
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Write("{\"error\":\"unknown action，需要 action=search 或 action=ask\"}");
                }
            }
            catch (WebException wex)
            {
                // Python 服务返回了错误状态码：把它的响应体原样透传，方便前端显示
                string detail = ReadWebExceptionBody(wex);
                context.Response.Write(string.IsNullOrEmpty(detail)
                    ? "{\"error\":\"无法连接 RAG 服务，请确认 Python 服务(app.py)已启动。\"}"
                    : detail);
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"error\":" + JsonString("中转失败: " + ex.Message) + "}");
            }
        }

        private static string ForwardGet(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.Timeout = 100000;
            req.ReadWriteTimeout = 100000;
            return ReadResponse(req);
        }

        private static string ForwardPost(string url, string jsonBody)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/json; charset=utf-8";
            req.Timeout = 100000;          // 大模型生成可能要几十秒
            req.ReadWriteTimeout = 100000;
            byte[] data = Encoding.UTF8.GetBytes(jsonBody ?? string.Empty);
            req.ContentLength = data.Length;
            using (Stream s = req.GetRequestStream())
            {
                s.Write(data, 0, data.Length);
            }
            return ReadResponse(req);
        }

        private static string ReadResponse(HttpWebRequest req)
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            using (StreamReader reader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private static string ReadWebExceptionBody(WebException wex)
        {
            try
            {
                if (wex.Response != null)
                {
                    using (StreamReader reader = new StreamReader(wex.Response.GetResponseStream(), Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        private static string JsonString(string value)
        {
            StringBuilder sb = new StringBuilder("\"");
            foreach (char c in value ?? string.Empty)
            {
                switch (c)
                {
                    case '\"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default: sb.Append(c); break;
                }
            }
            sb.Append("\"");
            return sb.ToString();
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
