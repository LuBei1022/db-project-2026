
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace LiteratureManager.Common
{
    public class HttpWebResponseUtility
    {
        private static readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
        public static string selectAPI(string postUrl, string jsonStr, Encoding dataEncode, DataTable table, string ContentTypeStr)
        {
            string url = postUrl;
            string result = "";//返回结果
            try
            {
                Encoding encoding = Encoding.UTF8;
                HttpWebResponse response;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);//webrequest请求api地址

                request.Accept = "text/html,application/xhtml+xml,*/*";
                request.ContentType = ContentTypeStr;
                //请求方式
                request.Method = "POST";
                //头文件

                if (table != null && table.Rows.Count > 0)
                {
                    foreach (DataRow item in table.Rows)
                    {
                        request.Headers.Add(item["Name"].ToString(), item["Val"].ToString());
                    }
                }
                try
                {
                    byte[] payload = dataEncode.GetBytes(jsonStr);//将URL编码后的字符串转化为字节             
                    request.ContentLength = payload.Length;//设置请求的 ContentLength 
                    Stream stream = request.GetRequestStream();//获得请求流
                    stream.Write(payload, 0, payload.Length);//将请求参数写入流
                    stream.Close(); // 关闭请求流
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    response = (HttpWebResponse)ex.Response;
                }
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), dataEncode))
                {
                    result = reader.ReadToEnd();
                    reader.Close();
                }
                if (response.StatusCode != HttpStatusCode.OK)//未成功格式化数据，返回响应码
                {
                    result = "Exception:" + result;
                }
                return result;
            }
            catch (WebException ex)
            {
                return "Exception:" + ex.Message;
            }
        }
        //url请求地址 可包含参数 ？&
        public static string GetQueryPostparamsService(string url)
        {
            string result = "";
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "Get";
            //req.Headers.Add()
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
      

        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        public static string GetCreateHttpResponse(string url, int? timeout, string userAgent = null, CookieCollection cookies = null)
        {
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = DefaultUserAgent;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            HttpWebResponse webreponse = request.GetResponse() as HttpWebResponse;
            try
            {
                using (StreamReader reader = new StreamReader(webreponse.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }

            catch (Exception exp)
            {
                return exp.ToString();
            }
        }
        /// <summary>
        /// 生成Http Post入参
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public static string PostHttpNewXml(string xmlstring, string postUrl, string Host, Dictionary<string, string> headers,string ContentType)
        {
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(postUrl);
            //Request.CookieContainer = "";  
            Request.Method = "POST";
            Request.ContentType = ContentType;
            if (headers != null && headers.Count > 0)
            {
                foreach (string key in headers.Keys)
                {
                    Request.Headers[key] = headers[key];
                }
            }
            Request.AllowAutoRedirect = true;
            if (!string.IsNullOrWhiteSpace(Host))
            {
                Request.Host = Host;
            }
            //    string strXML = "XMLDATA=<book><author>Irina</author><title>Piano Fort A</title><price>4.95</price></book>";  
            string strXML = xmlstring;
            byte[] data = Encoding.UTF8.GetBytes(strXML);
            Stream newStream = Request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();
            HttpWebResponse response = (HttpWebResponse)Request.GetResponse();
            Stream stream = null;
            stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8, true);
            string file = reader.ReadToEnd();
            return file;
        }

        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        public static string PostCreateHttpResponse(string url, IDictionary<string, string> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies)
        {
            ServicePointManager.Expect100Continue = false;

            HttpWebRequest httpWebRequest = null;

            HttpWebResponse httpWebResponse = null;
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentNullException("url");
                }
                if (requestEncoding == null)
                {
                    throw new ArgumentNullException("requestEncoding");
                }
                ////如果是发送HTTPS请求  
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);

                    httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
                    httpWebRequest.KeepAlive = false;
                    httpWebRequest.ProtocolVersion = HttpVersion.Version10;

                }
                else
                {
                    httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
                }
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";

                if (!string.IsNullOrEmpty(userAgent))
                {
                    httpWebRequest.UserAgent = userAgent;
                }
                else
                {
                    httpWebRequest.UserAgent = DefaultUserAgent;
                }

                if (timeout.HasValue)
                {
                    httpWebRequest.Timeout = timeout.Value;
                }
                if (cookies != null)
                {
                    httpWebRequest.CookieContainer = new CookieContainer();
                    httpWebRequest.CookieContainer.Add(cookies);
                }
                //如果需要POST数据  
                if (!(parameters == null || parameters.Count == 0))
                {
                    StringBuilder buffer = new StringBuilder();
                    int i = 0;
                    foreach (string key in parameters.Keys)
                    {
                        if (i > 0)
                        {
                            buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                        }
                        else
                        {
                            buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        }
                        i++;
                    }
                    byte[] data = requestEncoding.GetBytes(buffer.ToString());
                    using (Stream stream = httpWebRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, requestEncoding);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();

                httpWebRequest.Abort();
                httpWebResponse.Close();


                return html;
            }
            catch (Exception ex)
            {

                if (httpWebRequest != null)
                {
                    httpWebRequest.Abort();
                }
                if (httpWebResponse != null)
                {
                    httpWebResponse.Close();
                }
                return ex.Message;
            }
        }

        /// <summary>
        /// json转urlencode
        /// </summary>
        /// <returns></returns>
        public static string JsonUrlEncode(string json)
        {
            Dictionary<string, object> dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, object> item in dic)
            {
                builder.Append(GetFormDataContent(item, ""));
            }
            return builder.ToString().TrimEnd('&');
        }

        /// <summary>
        /// 递归转formdata
        /// </summary>
        /// <param name="item"></param>
        /// <param name="preStr"></param>
        /// <returns></returns>
        private static string GetFormDataContent(KeyValuePair<string, object> item, string preStr)
        {
            StringBuilder builder = new StringBuilder();
            if (string.IsNullOrEmpty(item.Value?.ToString()))
            {
                builder.AppendFormat("{0}={1}", string.IsNullOrEmpty(preStr) ? item.Key : (preStr + "[" + item.Key + "]"), System.Web.HttpUtility.UrlEncode((item.Value == null ? "" : item.Value.ToString()).ToString()));
                builder.Append("&");
            }
            else
            {
                //如果是数组
                if (item.Value.GetType().Name.Equals("JArray"))
                {
                    var children = JsonConvert.DeserializeObject<List<object>>(item.Value.ToString());
                    for (int j = 0; j < children.Count; j++)
                    {
                        Dictionary<string, object> childrendic = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(children[j]));
                        foreach (var row in childrendic)
                        {
                            builder.Append(GetFormDataContent(row, string.IsNullOrEmpty(preStr) ? (item.Key + "[" + j + "]") : (preStr + "[" + item.Key + "][" + j + "]")));
                        }
                    }

                }
                //如果是对象
                else if (item.Value.GetType().Name.Equals("JObject"))
                {
                    Dictionary<string, object> children = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.Value.ToString());
                    foreach (var row in children)
                    {
                        builder.Append(GetFormDataContent(row, string.IsNullOrEmpty(preStr) ? item.Key : (preStr + "[" + item.Key + "]")));
                    }
                }
                //字符串、数字等
                else
                {
                    builder.AppendFormat("{0}={1}", string.IsNullOrEmpty(preStr) ? item.Key : (preStr + "[" + item.Key + "]"), System.Web.HttpUtility.UrlEncode((item.Value == null ? "" : item.Value.ToString()).ToString()));
                    builder.Append("&");
                }
            }

            return builder.ToString();
        }

        public static string PostRequest(string postUrl, string paramData, string token, string ContentTypeStr)
        {
            string ret = string.Empty;
            try
            {
                Encoding dataEncode = Encoding.GetEncoding("utf-8");
                if (ContentTypeStr == "application/x-www-form-urlencoded")
                {
                    paramData = JsonUrlEncode(paramData);
                }
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化

                if (postUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                }
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(postUrl);

                webReq.KeepAlive = false;
                webReq.ProtocolVersion = HttpVersion.Version10;
                //webReq.ProtocolVersion = HttpVersion.Version11;
                webReq.Method = "POST";
                webReq.Timeout = 1000000;
                // ServicePointManager.CheckCertificateRevocationList = true;
                webReq.UserAgent = DefaultUserAgent;

                webReq.ContentType = ContentTypeStr;// "application/json";//application/x-www-form-urlencoded
                if (!string.IsNullOrWhiteSpace(token))
                {
                    webReq.Headers.Add("token", token);
                }

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数

                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), dataEncode);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
                webReq.Abort();

            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }

            return ret;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }
    }
}
