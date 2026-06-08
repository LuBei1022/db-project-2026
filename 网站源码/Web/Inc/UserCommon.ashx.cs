using LiteratureManager.Common;
using System;
using System.Text;
using System.Web;

namespace Web.Inc
{
    /// <summary>
    /// UserCommon 的摘要说明
    /// </summary>
    public class UserCommon : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json;charset=UTF-8";
            if (context.Request.ContentLength <= 0 || context.Request.ContentLength > UploadPolicy.UserApiMaxRequestBytes)
            {
                context.Response.StatusCode = 413;
                context.Response.Write("{\"status\":\"0\",\"info\":\"请求内容过大或为空\"}");
                return;
            }

            System.IO.Stream s = HttpContext.Current.Request.InputStream;
            byte[] b = new byte[context.Request.ContentLength];
            int bytesRead = 0;
            while (bytesRead < b.Length)
            {
                int current = s.Read(b, bytesRead, b.Length - bytesRead);
                if (current <= 0)
                {
                    break;
                }
                bytesRead += current;
            }
            if (bytesRead <= 0)
            {
                context.Response.Write("{\"status\":\"0\",\"info\":\"请求内容为空\"}");
                return;
            }
            string JsonStr = Encoding.UTF8.GetString(b, 0, bytesRead);
            if (!string.IsNullOrWhiteSpace(JsonStr) && JsonStr.Contains("btn"))
            {
                try
                {
                    System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    RJson RJson = Serializer.Deserialize<RJson>(JsonStr);
                    if (RJson != null)
                    {
                        switch (RJson.btn)
                        {
                            case "addcode":
                                context.Response.Write(CommonUserFunc.GetAddCodeFunc(RJson.tel, RJson.img_x, RJson.img_y, RJson.type));
                                break;
                            case "UserLogin":
                                context.Response.Write(CommonUserFunc.GetUserLoginFunc(RJson.tel, RJson.code, RJson.img_x, RJson.img_y, RJson.type));
                                break;
                            case "UpUserName":
                                context.Response.Write(CommonUserFunc.GetUpUserNameFunc(RJson.name, RJson.id));
                                break;
                            case "UpUserEmail":
                                context.Response.Write(CommonUserFunc.GetUpUserEmailFunc(RJson.email, RJson.id));
                                break;
                            case "UpUserTel":
                                context.Response.Write(CommonUserFunc.GetUpUserTelFunc(RJson.tel, RJson.code, RJson.img_x, RJson.img_y, RJson.type, RJson.id));
                                break;
                            case "AddTopUp":
                                context.Response.Write(CommonUserFunc.GetTaskAddTopUpFunc(RJson.money, RJson.typestr, RJson.idstr));
                                break;
                            case "QueryTopUpStatus":
                                context.Response.Write(CommonUserFunc.GetTopUpOrderStatusFunc(RJson.out_trade_no));
                                break;
                            case "DelUserImg":
                                context.Response.Write(CommonUserFunc.GetDelUserImgFunc(RJson.url));
                                break;
                            case "Drawdown":
                                context.Response.Write(CommonUserFunc.GetDrawdownFunc(RJson.id));
                                break;
                            case "AppealAdd":
                                context.Response.Write(CommonUserFunc.GetAppealAddFunc(RJson.url, RJson.info, RJson.ImgArr));
                                break;
                            case "IntegrateExchangeAdd":
                                context.Response.Write(CommonUserFunc.GetIntegrateExchangeAddFunc(RJson.id, RJson.num, RJson.user_id));
                                break;
                            case "LiteratureCommentAdd":
                                context.Response.Write(CommonUserFunc.GetLiteratureCommentAddFunc(RJson.id, RJson.info));
                                break;
                            case "LiteratureCommentDelete":
                                context.Response.Write(CommonUserFunc.GetLiteratureCommentDeleteFunc(RJson.id, RJson.comment_id));
                                break;
                            case "LiteratureReactionToggle":
                                context.Response.Write(CommonUserFunc.GetLiteratureReactionToggleFunc(RJson.id, RJson.action));
                                break;
                        }
                    }
                    else
                    {
                        context.Response.Write("{\"status\":\"0\",\"info\":\"请求参数异常！！\"}");
                    }
                }
                catch (Exception ex)
                {
                    ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
                    context.Response.Write("{\"status\":\"0\",\"info\":\"" + ex.Message + "\"}");
                }
            }
            else
            {
                context.Response.Write("{\"status\":\"0\",\"info\":\"请求类型异常！！！\"}");
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public class RJson
        {
            
            public string idstr { get; set; }
            public string money { get; set; }
            public string typestr { get; set; }
            public string out_trade_no { get; set; }
            public string reply_id { get; set; }
            public string[] ImgArr { get; set; }
            public string info { get; set; }
            public string url { get; set; }
            public string btn { get; set; }
            public string action { get; set; }
            public int comment_id { get; set; }
            public int id { get; set; }
            public int num { get; set; }
            public int user_id { get; set; }
            public string img_x { get; set; }
            public string img_y { get; set; }
            public string code { get; set; }
            public string type { get; set; }
            public string tel { get; set; }
            public string name { get; set; }
            public string email { get; set; }
        }
    }
}
