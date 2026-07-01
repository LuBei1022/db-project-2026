

using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

namespace LiteratureManager.Common
{
    public class CommonFunc
    {
        public static BLLBase<websiteinfo_list> websiteinfo_listbll = new BLLBase<websiteinfo_list>();
        public static BLLBase<logincode_list> logincode_listbll = new BLLBase<logincode_list>();
        public static BLLBase<tbl_class> tbl_classbll = new BLLBase<tbl_class>();
        public static BLLBase<userimg_list> userimg_listbll = new BLLBase<userimg_list>();
        public static BLLBase<userfile_list> userfile_listbll = new BLLBase<userfile_list>();
        public static BLLBase<ServiceLogStatus_List> ServiceLogStatus_Listbll = new BLLBase<ServiceLogStatus_List>();
        public static BLLBase<user_list> user_listbll = new BLLBase<user_list>();
        public static BLLBase<integrateLogType_list> integrateLogType_listbll = new BLLBase<integrateLogType_list>();
        public static BLLBase<cosfile_list> cosfile_listbll = new BLLBase<cosfile_list>();

        private class EmailMessageOptions
        {
            public string email_to { get; set; }
            public string subject { get; set; }
            public string body { get; set; }
            public string emailname { get; set; }
            public string emailnum { get; set; }
            public string emailpasswd { get; set; }
            public int smtpserverport { get; set; }
            public string host { get; set; }
        }        public static string MD5Key = ConfigurationManager.AppSettings["md5_key"];
        public static string A_UpLoad_Url = ConfigurationManager.AppSettings["website_url"] + "/A_UpLoad/upload_pic/";
        public static DataTable ExcelDataSource(string filepath, string sheetname)
        {

            //string strConn = "Provider=Microsoft.Jet.OleDb.4.0;" + "data source=" + filepath + ";Extended Properties='Excel 8.0; HDR=Yes; IMEX=1'";
            string strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + filepath + ";Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";
            OleDbConnection conn = new OleDbConnection(strConn);
            OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + sheetname + "]", strConn);
            DataTable dt = new DataTable();
            oada.Fill(dt);
            return dt;

        }
        public static void ToEmail(string Subject, string Body, string email_to)
        {
            try
            {
                websiteinfo_list websiteinfo_list = websiteinfo_listbll.SelectSingle("id=1");
                if (websiteinfo_list != null && !string.IsNullOrWhiteSpace(websiteinfo_list.host) && !string.IsNullOrWhiteSpace(websiteinfo_list.emailnum) && !string.IsNullOrWhiteSpace(websiteinfo_list.emailpasswd) && !string.IsNullOrWhiteSpace(websiteinfo_list.smtpserverport) && !string.IsNullOrWhiteSpace(websiteinfo_list.emailname))
                {
                    EmailMessageOptions emailOptions = new EmailMessageOptions();
                    if (string.IsNullOrWhiteSpace(email_to))
                    {
                        emailOptions.email_to = Function.HtmlDiscode(websiteinfo_list.email_to);
                    }
                    else
                    {
                        emailOptions.email_to = Function.HtmlDiscode(email_to);
                    }
                    emailOptions.subject = Subject;
                    emailOptions.body = Body;
                    emailOptions.emailname = Function.HtmlDiscode(websiteinfo_list.emailname);
                    emailOptions.emailnum = Function.HtmlDiscode(websiteinfo_list.emailnum);
                    emailOptions.emailpasswd = Function.HtmlDiscode(websiteinfo_list.emailpasswd);
                    emailOptions.smtpserverport = Function.ConvertTo<int>(websiteinfo_list.smtpserverport, 0);
                    emailOptions.host = Function.HtmlDiscode(websiteinfo_list.host);

                    ParameterizedThreadStart pts = new ParameterizedThreadStart(SayEmailFunc_);
                    Thread td2 = new Thread(pts);
                    td2.Start(emailOptions);
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "CommonFunc.ToOrderEmail:" + ex.Message + "-" + ex.StackTrace);
            }
        }
        public static void SayEmailFunc_(object emailjson_)
        {
            try
            {
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                EmailMessageOptions emailOptions = Function.ConvertTo<EmailMessageOptions>(emailjson_, null);
                if (emailOptions != null && !string.IsNullOrWhiteSpace(emailOptions.email_to) && !string.IsNullOrWhiteSpace(emailOptions.emailname) && !string.IsNullOrWhiteSpace(emailOptions.emailnum) && !string.IsNullOrWhiteSpace(emailOptions.emailpasswd) && emailOptions.smtpserverport > 0 && !string.IsNullOrWhiteSpace(emailOptions.host))
                {
                    try
                    {
                        // 创建 SmtpClient 对象用于发送邮件
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = Function.HtmlDiscode(emailOptions.host); // SMTP服务器地址
                        smtp.Port = emailOptions.smtpserverport; // SMTP服务器端口号，例如587（可能需要SSL/TLS）或25（非SSL）
                        smtp.Timeout = UploadPolicy.ExternalRequestTimeoutMs;
                        smtp.EnableSsl = true; // 如果使用SSL/TLS，则设置为true
                        //smtp.DeliveryMethod = SmtpDeliveryMethod.Network; // 使用网络发送邮件
                        smtp.UseDefaultCredentials = false; // 不使用默认的Windows账户认证
                        smtp.Credentials = new NetworkCredential(Function.HtmlDiscode(emailOptions.emailnum), Function.HtmlDiscode(emailOptions.emailpasswd)); // SMTP服务器认证信息
                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(Function.HtmlDiscode(emailOptions.emailnum), Function.HtmlDiscode(emailOptions.emailname));
                        string[] email_to_ = Function.HtmlDiscode(emailOptions.email_to).Split(',');
                        foreach (string item in email_to_)
                        {
                            mail.To.Add(new MailAddress(Function.HtmlDiscode(item)));
                        }
                        mail.Subject = Function.HtmlDiscode(emailOptions.subject); // 邮件主题
                        mail.BodyEncoding = Encoding.UTF8;
                        mail.IsBodyHtml = true;
                        mail.Body = Function.HtmlDiscode(emailOptions.body);// 邮件正文

                        smtp.Send(mail);

                    }
                    catch (SmtpException ex)
                    {
                        ImportDataLog.WriteLog(LogType.Error, "SMTP_err:" + ex.Message);

                    }
                    catch (Exception ex)
                    {
                        ImportDataLog.WriteLog(LogType.Error, "Error_err:" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "CommonFunc.SayEmailFunc_Error:" + ex.Message + "-" + ex.StackTrace);
            }
        }
        public static void Ok_Return(string title, string URL, int t)
        {
            string img = "";
            StringBuilder sb = new StringBuilder();
#pragma warning disable CS0472 // 由于此类型的值永不等于 "null"，该表达式的结果始终相同
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(URL) || t == null)
            {
                Function.Show_Msg("非法操作！", "");

            }
#pragma warning restore CS0472 // 由于此类型的值永不等于 "null"，该表达式的结果始终相同

            if (t == 0)   //正确
            {
                img = "OK.gif";
            }
            else if (t == 1)   //提示
            {
                img = "information.gif";
            }
            else if (t == 2)
            {
                img = "Failure.gif";   //错误显示图片不一样
            }
            else if (t == 3)
            {
                img = "warning.gif";   //警告显示图片不一样
            }


            sb.Append("<script>" + "\r\n");

            sb.Append("var seconds = 2;" + "\r\n");
            sb.Append("var defaultUrl = '" + URL + "';" + "\r\n");

            sb.Append("onload = function()" + "\r\n");
            sb.Append("{" + "\r\n");
            sb.Append("var ok_html =\"<link media='all' type='text/css' href='/admin/css/style.css' rel='stylesheet' /><div class='container' id='cpcontainer'><h3>操作提示</h3><div class='infobox'><table width='600' border='0' align='center' cellpadding='4' cellspacing='0'><tr><td width='154' rowspan='2' align='right'><img height='32' alt='information' src='/admin/images/" + img + "' width='32' border='0' style='margin-right:10px;' /></td><td width='430' align='left' class='infotitle2'>" + title + "</td></tr><tr><td align='left'>将在 <span id='spanSeconds'>2</span> 秒后跳转到第一个链接地址。</td></tr></table></div></div></div>\"\r\n");
            sb.Append("$('#ok_html').html(ok_html);\r\n");
            sb.Append("  if (defaultUrl == 'javascript:history.go(-1)' && window.history.length == 0)" + "\r\n");
            sb.Append("  {" + "\r\n");
            sb.Append("    document.getElementById('redirectionMsg').innerHTML = '';" + "\r\n");
            sb.Append("    return;" + "\r\n");
            sb.Append("  }" + "\r\n");

            sb.Append("}" + "\r\n");
            sb.Append("var aa = window.setInterval(redirection, 1000);" + "\r\n");
            sb.Append("function redirection()" + "\r\n");
            sb.Append("{" + "\r\n");
            sb.Append("  if (seconds <= 0)" + "\r\n");
            sb.Append("  {" + "\r\n");
            sb.Append("if(aa) window.clearInterval(aa);" + "\r\n");
            sb.Append("    return;" + "\r\n");
            sb.Append("  }" + "\r\n");
            sb.Append("  seconds --;" + "\r\n");
            sb.Append("  document.getElementById('spanSeconds').innerHTML = seconds;" + "\r\n");
            sb.Append("  if (seconds == 0) " + "\r\n");
            sb.Append("  {" + "\r\n");
            sb.Append("if(aa) window.clearInterval(aa);" + "\r\n");
            sb.Append("    location.href = defaultUrl;" + "\r\n");
            sb.Append("  }" + "\r\n");
            sb.Append("}" + "\r\n");
            sb.Append("</script>");

            HttpContext.Current.Response.Write(sb.ToString());
        }
        public static string GetTitle(string data_title, string tbclass_title)
        {
            string R_str = string.Empty;
            if (!string.IsNullOrWhiteSpace(data_title))
            {
                R_str = Function.HtmlDiscode(data_title).Trim();
            }
            if (string.IsNullOrWhiteSpace(R_str))
            {
                R_str = Function.HtmlDiscode(tbclass_title).Trim();
            }
            if (string.IsNullOrWhiteSpace(R_str))
            {
                websiteinfo_list websiteinfo_list = websiteinfo_listbll.SelectSingle("id=1");
                if (websiteinfo_list != null && websiteinfo_list.id > 0)
                {
                    R_str = Function.HtmlDiscode(websiteinfo_list.title);
                    if (string.IsNullOrWhiteSpace(R_str))
                    {
                        R_str = Function.HtmlDiscode(websiteinfo_list.companyname);
                    }
                }
            }
            return R_str;
        }
        public static string GetDescription(string data_description)
        {
            string R_str = string.Empty;
            if (!string.IsNullOrWhiteSpace(data_description))
            {
                R_str = Function.HtmlDiscode(data_description).Trim();
            }
            if (string.IsNullOrWhiteSpace(R_str))
            {
                websiteinfo_list websiteinfo_list = websiteinfo_listbll.SelectSingle("id=1");
                if (websiteinfo_list != null && websiteinfo_list.id > 0)
                {
                    R_str = Function.HtmlDiscode(websiteinfo_list.description);
                }
            }
            return R_str;
        }
        public static string GetKeywords(string data_keywords)
        {
            string R_str = string.Empty;
            if (!string.IsNullOrWhiteSpace(data_keywords))
            {
                R_str = Function.HtmlDiscode(data_keywords).Trim();
            }

            if (string.IsNullOrWhiteSpace(R_str))
            {
                websiteinfo_list websiteinfo_list = websiteinfo_listbll.SelectSingle("id=1");
                if (websiteinfo_list != null && websiteinfo_list.id > 0)
                {
                    R_str = Function.HtmlDiscode(websiteinfo_list.keywords);
                }
            }
            return R_str;
        }
        public static string GetTbClassTitle(tbl_class tbl_class)
        {
            string R_str = string.Empty;
            if (tbl_class != null && tbl_class.id > 0)
            {
                if (!string.IsNullOrWhiteSpace(tbl_class.classname))
                {
                    R_str = Function.HtmlDiscode(tbl_class.classname);
                }

                if (tbl_class.parentid != 360)
                {
                    tbl_class tbl_class_p = tbl_classbll.SelectSingle("id=" + tbl_class.parentid + " and isshow=1");
                    if (tbl_class_p != null && tbl_class_p.id > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(tbl_class_p.classname))
                        {
                            if (!string.IsNullOrWhiteSpace(R_str))
                            {
                                R_str += "-" + Function.HtmlDiscode(tbl_class_p.classname);
                            }
                            else
                            {
                                R_str += Function.HtmlDiscode(tbl_class_p.classname);
                            }

                        }
                    }
                }
            }
            websiteinfo_list websiteinfo_list = websiteinfo_listbll.SelectSingle("id=1");
            if (websiteinfo_list != null && websiteinfo_list.id > 0)
            {
                if (!string.IsNullOrWhiteSpace(websiteinfo_list.companyname))
                {
                    if (!string.IsNullOrWhiteSpace(R_str))
                    {
                        R_str += "-" + Function.HtmlDiscode(websiteinfo_list.companyname);
                    }
                    else
                    {
                        R_str += Function.HtmlDiscode(websiteinfo_list.companyname);
                    }

                }
            }
            return R_str;
        }
        public static string GetCaptcha()
        {
            int number;
            char code;
            string checkCode = String.Empty;

            Random random = new Random();

            for (int i = 0; i < 4; i++)
            {
                number = random.Next();

                if (number % 3 == 0)
                {
                    code = (char)('0' + (char)(number % 10));
                }
                else if (number % 3 == 1)
                {
                    code = (char)('a' + (char)(number % 26));
                }
                else
                {
                    code = (char)('A' + (char)(number % 26));
                }
                checkCode += " " + code.ToString();
            }
            checkCode = checkCode.Replace(" ", "");
            return checkCode.ToLower();
        }

        public static string GetTbClassModelUrl(string tbclass_id)
        {
            string R_str = string.Empty;
            tbl_class tbl_class = tbl_classbll.SelectSingle("id=" + Function.ConvertTo<int>(tbclass_id, 0));
            if (tbl_class != null && tbl_class.id > 0)
            {
                R_str = "<a href=\"" + GetModelUrl(tbl_class.model.ToString()) + "?tbclass_id=" + tbl_class.id + "\" >" + Function.HtmlDiscode(tbl_class.classname) + "</a>";
            }
            return R_str;
        }

        public static string GetIntegrateLogTypeFunc(string type_id)
        {
            string R_str = string.Empty;
            integrateLogType_list integrateLogType_list = integrateLogType_listbll.SelectSingle("id=" + Function.ConvertTo<int>(type_id, 0));
            if (integrateLogType_list != null && integrateLogType_list.id > -1)
            {
                R_str = Function.HtmlDiscode(integrateLogType_list.name);
            }
            return R_str;
        }

        public static string GetServiceLogStatusNameFunc(string status_id)
        {
            string R_str = string.Empty;
            ServiceLogStatus_List ServiceLogStatus_List = ServiceLogStatus_Listbll.SelectSingle("id=" + Function.ConvertTo<int>(status_id, 0));
            if (ServiceLogStatus_List != null && ServiceLogStatus_List.id > -1)
            {
                R_str = Function.HtmlDiscode(ServiceLogStatus_List.name);
            }
            return R_str;
        }

        public static string GetTbClassNameFunc(string tbclass_id)
        {
            string R_str = string.Empty;
            tbl_class tbl_class = tbl_classbll.SelectSingle("id=" + Function.ConvertTo<int>(tbclass_id, 0));
            if (tbl_class != null && tbl_class.id > 0)
            {
                R_str = Function.HtmlDiscode(tbl_class.classname);
            }
            return R_str;
        }

        /// <summary>
        /// 前端的栏目跳转连接；
        /// </summary>
        public static string GetTopHtmlHref(string id_str, string type)
        {
            string R_str = "javascript:void(0);";
            string wherestr = string.Empty;
            if (type == "1")
            {
                wherestr = " and istop=1 ";
            }
            else if (type == "2")
            {
                wherestr = " and isfoot=1 ";
            }

            int id_ = Function.ConvertTo<int>(id_str, 0);
            if (id_ > 0)
            {
                tbl_class tbl_class = tbl_classbll.SelectSingle("id=" + id_ + " and isshow=1");
                if (tbl_class != null && tbl_class.id > 0)
                {
                    if (tbl_class.isurl == 2)
                    {
                        if (!string.IsNullOrWhiteSpace(tbl_class.classurl))
                        {
                            string classurl = Function.HtmlDiscode(tbl_class.classurl);
                            if (classurl.ToLower().IndexOf("http") == 0)
                            {
                                R_str = classurl + "\" target=\"_blank";
                            }
                            else
                            {
                                R_str = classurl;
                            }
                        }
                        else
                        {
                            R_str = "javascript:void(0);";
                        }

                    }
                    else
                    {
                        if (tbl_class.model.Equals(1))
                        {
                            DataTable tbl_classdt = tbl_classbll.GetDatatable("select * from tbl_class where parentid=" + tbl_class.id + " and isshow=1 " + wherestr + "  order by orderid asc");


                            if (tbl_classdt != null && tbl_classdt.Rows.Count > 0)
                            {
                                if (tbl_classdt.Rows[0]["model"].ToString() == "1")
                                {
                                    int pid = Function.ConvertTo<int>(tbl_classdt.Rows[0]["id"].ToString(), 0);
                                    DataTable tbl_classdt_ = tbl_classbll.GetDatatable("select * from tbl_class where  parentid=" + pid + " and isshow=1 " + wherestr + " order by orderid asc");


                                    if (tbl_classdt_ != null && tbl_classdt_.Rows.Count > 0)
                                    {
                                        R_str = GetHtmlHref(Function.ConvertTo<int>(tbl_classdt_.Rows[0]["id"].ToString(), 0), wherestr, Function.ConvertTo<int>(tbl_classdt.Rows[0]["id"].ToString(), 0));
                                    }
                                    else
                                    {
                                        R_str = GetHtmlHref(Function.ConvertTo<int>(tbl_classdt.Rows[0]["id"].ToString(), 0), wherestr, tbl_class.id);
                                    }
                                }
                                else
                                {
                                    R_str = GetHtmlHref(Function.ConvertTo<int>(tbl_classdt.Rows[0]["id"].ToString(), 0), wherestr, tbl_class.id);
                                }
                            }
                            else
                            {
                                R_str = "javascript:void(0);";
                            }
                            tbl_classdt.Dispose();
                        }
                        else
                        {
                            R_str = GetHtmlHref(Function.ConvertTo<int>(tbl_class.id, 0), wherestr, Function.ConvertTo<int>(tbl_class.id, 0));
                        }
                    }
                }
            }
            return Function.HtmlDiscode(R_str);
        }

        public static string GetHtmlHref(int id_, string wherestr, int pid_)
        {
            string R_str = "javascript:void(0);";
            if (id_ > 0)
            {
                tbl_class tbl_class = tbl_classbll.SelectSingle("id=" + id_ + " and isshow=1" + wherestr);
                if (tbl_class != null && tbl_class.id > 0)
                {
                    if (tbl_class.isurl == 2)
                    {
                        string classurl = tbl_class.classurl;
                        if (!string.IsNullOrWhiteSpace(classurl))
                        {
                            if (classurl.ToLower().IndexOf("http") == 0)
                            {
                                R_str = classurl + "\" target=\"_blank";
                            }
                            else
                            {
                                R_str = classurl;
                            }
                        }
                        else
                        {
                            R_str = "javascript:void(0);";
                        }
                    }
                    else
                    {
                        R_str = "/Website/" + Function.HtmlDiscode(tbl_class.urlnamebtn);
                        //R_str = GetHtmlHrefUrl(tbl_class.model, "?mid=" + tbl_class.id);
                    }
                }
            }
            return R_str;
        }
        public static string GetModelUrl(string id)
        {
            int modelId = Function.ConvertTo<int>(id, 0);
            switch (modelId)
            {
                case 2:
                    return "/WebsiteData/News.aspx";
                case 3:
                    return "/WebsiteData/Single.aspx";
                default:
                    return string.Empty;
            }
        }

        public static string GetModelName(int id)
        {
            switch (id)
            {
                case 1:
                    return "栏目分组";
                case 2:
                    return "新闻列表";
                case 3:
                    return "单页内容";
                default:
                    return "未配置";
            }
        }


        public static bool GetLoginCodeAdd(string VNum, string NewGuidCode)
        {
            logincode_list logincode_list = new logincode_list();
            logincode_list.code = NewGuidCode;
            logincode_list.val = VNum;
            logincode_list.addtime = DateTime.Now;
            logincode_list.ip_str = Function.GetClientIP();
            logincode_list.type = 1;
            return logincode_listbll.Add(logincode_list);
        }
        public static void GetLoginCodeDelete()
        {
            string BackstageCheckCode_ = Cookie.GetCookie("BackstageCheckCode");
            if (!string.IsNullOrWhiteSpace(BackstageCheckCode_))
            {
                logincode_listbll.Delete("code='" + Function.MD5Decrypt(BackstageCheckCode_, MD5Key) + "'");
            }
        }
        public static string Replace_Content(string info, string info_m)
        {
            return Function.Replace_Content(Replace_Content(info, info_m, GetIsWeb()));
        }
        public static string Replace_Content(string info, string info_m, bool isweb)
        {
            string info_pc_ = string.Empty;
            string info_m_ = string.Empty;
            if (!string.IsNullOrWhiteSpace(info) && !string.IsNullOrWhiteSpace(info_m))
            {
                info_pc_ = info;
                info_m_ = info_m;
            }
            else if (string.IsNullOrWhiteSpace(info) && string.IsNullOrWhiteSpace(info_m))
            {

            }
            else
            {
                if (!string.IsNullOrWhiteSpace(info))
                {
                    info_pc_ = info;
                    info_m_ = info_pc_;
                }
                else
                {
                    info_m_ = info_m;
                    info_pc_ = info_m_;
                }
            }

            string info_ = string.Empty;
            if (isweb)
            {
                info_ = info_pc_;
            }
            else
            {
                info_ = info_m_;
            }
            return Function.Replace_Content(info_);
        }


        public static bool GetIsWeb()
        {
            bool isweb = true;
            string u = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
            Regex b = new Regex(@"android.+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |ipad|maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(di|rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))))
            {
                isweb = false;
            }
            return isweb;
        }
        public static string GetBannerImg(string img_url_pc, string img_url_m)
        {
            string str = GetBannerImg(img_url_pc, img_url_m, GetIsWeb());
            return str;
        }

        public static string GetBannerImg(string img_url_pc, string img_url_m, bool isweb)
        {
            string str = string.Empty;
            bool is_bj_img_pc = GetImgBool(img_url_pc);
            bool is_bj_img_m = GetImgBool(img_url_m);
            if (is_bj_img_pc || is_bj_img_m)
            {
                if (isweb)
                {
                    if (is_bj_img_pc)
                    {
                        str = GetWebUpload_Pic(img_url_pc, "/images/null.png");
                    }
                    else if (is_bj_img_m)
                    {
                        str = GetWebUpload_Pic(img_url_m, "/images/null.png");
                    }
                }
                else
                {
                    if (is_bj_img_m)
                    {
                        str = GetWebUpload_Pic(img_url_m, "/images/null.png");
                    }
                    else if (is_bj_img_pc)
                    {
                        str = GetWebUpload_Pic(img_url_pc, "/images/null.png");
                    }
                }
            }
            return str;
        }
        public static string GetWebUpload_Pic(string imgurl, string noimg)
        {
            if (GetImgBool(imgurl))
            {
                return "/A_UpLoad/upload_pic/" + imgurl;
            }
            else
            {
                return noimg;
            }
        }
        public static string GetHtmlHrefUrl(int model_, int tbclass_id)
        {
            string R_str = string.Empty;
            if (model_ == 2)
            {
                R_str = "/WebsiteData/News.aspx?mid=" + tbclass_id;
            }
            else if (model_ == 3)
            {
                R_str = "/WebsiteData/Single.aspx?mid=" + tbclass_id;
            }

            return R_str;
        }
        public static bool GetCosFileBool(string up_filename)
        {
            bool isyes = false;
            try
            {

                if (!string.IsNullOrWhiteSpace(up_filename))
                {
                    string isok = PutObjectModel.DoesObjectExist(up_filename);
                    if (isok == "ok")
                    {
                        isyes = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "CommonFunc.GetCosFileBool:" + ex.Message + "\\n" + ex.StackTrace);
            }
            return isyes;
        }

        public static void DeleteTrackedUploadRecord(string upFilename)
        {
            string safeFileName = EscapeSqlLiteral(upFilename);
            if (string.IsNullOrWhiteSpace(safeFileName))
            {
                return;
            }

            userfile_listbll.Delete("up_filename='" + safeFileName + "'");
            cosfile_listbll.Delete("up_filename='" + safeFileName + "'");
        }

        public static string EscapeSqlLiteral(string value)
        {
            return (value ?? string.Empty).Replace("'", "''").Trim();
        }

        public static bool GetFileBool(string up_filename)
        {
            bool isyes = false;
            try
            {

                if (!string.IsNullOrWhiteSpace(up_filename))
                {
                    string filePath = HttpContext.Current.Server.MapPath(@"/A_UpLoad/upload_file/" + up_filename);
                    System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                    if (file.Exists)
                    {
                        isyes = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "CommonFunc.GetFileBool:" + ex.Message + "\\n" + ex.StackTrace);
            }
            return isyes;
        }
        public static bool GetImgBool(string upload_pic)
        {
            bool isyes = false;
            try
            {

                if (!string.IsNullOrWhiteSpace(upload_pic))
                {
                    string filePath = HttpContext.Current.Server.MapPath(@"/A_UpLoad/upload_pic/" + upload_pic);
                    System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                    if (file.Exists)
                    {
                        isyes = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "CommonFunc.cs_GetImgBool:" + ex.Message + "\\n" + ex.StackTrace);
            }
            return isyes;
        }
        public static string classid_str = "";
        public static string GetChildrenId(int id)
        {
            string nav_str = "";
            string yi = "";
            DataTable dt = GetClassList("");
            DataRow[] drs = dt.Select("parentid= " + id);

            foreach (DataRow dr in drs)
            {
                int classid = int.Parse(dr["id"].ToString());
                yi = yi + classid + ",";
                nav_str = GetId(classid, dt) + yi;
            }
            classid_str = "";
            return Function.Encrypt(nav_str + id);
        }

        //绑定子分类
        public static string GetId(int cid, DataTable dt)
        {
            DataRow[] drs = dt.Select("parentid= " + cid);

            foreach (DataRow dr in drs)
            {
                int classid = int.Parse(dr["id"].ToString());
                classid_str = dr["id"] + "," + GetId(classid, dt);
            }
            return classid_str;
        }


        public static DataTable GetClassList(string strWhere)
        {
            string strsql = "select * from tbl_class where isshow=1 ";

            if (!string.IsNullOrWhiteSpace(strWhere))
            {
                strsql += strsql;
            }

            strsql += " order by orderid asc";
            return tbl_classbll.GetDatatable(strsql);
        }


    }
}





