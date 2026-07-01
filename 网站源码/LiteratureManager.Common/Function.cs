using BLL;
using Model;
using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Xml.Serialization;

namespace LiteratureManager.Common
{
    public class Function
    {
        #region 检查后台是否登陆
        /// <summary>
        /// 检查后台是否登陆；
        /// </summary>
        public static void Check_AdminLogin()
        {

            if (Cookie.GetCookie("LMS_AdminID") == null || Cookie.GetCookie("LMS_AdminName") == null || Cookie.GetCookie("LMS_Popedom") == null || Cookie.GetCookie("LMS_AdminID") == "" || Cookie.GetCookie("LMS_AdminName") == "" || Cookie.GetCookie("LMS_Popedom") == "" || Cookie.GetCookie("LMS_Code") == "")
            {

                HttpContext.Current.Response.Write("<script>alert('请重新登录');top.location=('Login.aspx');</script>");
                HttpContext.Current.Response.End();
            }
            else
            {
                BLLBase<admin> adminbll = new BLLBase<admin>();
                admin admin = adminbll.SelectSingle("id", Function.ConvertTo<int>(Cookie.GetCookie("LMS_AdminID"), 0));
#pragma warning disable CS0472 // 由于此类型的值永不等于 "null"，该表达式的结果始终相同
                if (admin != null && admin.id != null && admin.id > 0
                    && string.Equals(admin.username, Cookie.GetCookie("LMS_AdminName"), StringComparison.Ordinal)
                    && string.Equals(admin.popedom, Cookie.GetCookie("LMS_Popedom"), StringComparison.Ordinal))
                {
                    if (!string.IsNullOrWhiteSpace(admin.code) && Function.MD5Encrypt(admin.code, ConfigurationManager.AppSettings["md5_key"]).Equals(Cookie.GetCookie("LMS_Code")))
                    {
#pragma warning disable CS0472 // 由于此类型的值永不等于 "null"，该表达式的结果始终相同
                        if (admin.locks != null && admin.locks == 1)
                        {
                            Cookie.ClearCookie("LMS_AdminID");
                            Cookie.ClearCookie("LMS_AdminName");
                            Cookie.ClearCookie("LMS_Popedom");
                            Cookie.ClearCookie("LMS_Code");
                            HttpContext.Current.Response.Write("<script>alert('管理员已被冻结，请联系相关人员解锁！');top.location=('Login.aspx');</script>");
                            HttpContext.Current.Response.End();
                        }
                        else
                        {
                            TimeSpan ts = DateTime.Now - ConvertTo<DateTime>(admin.lastlogindate, DateTime.MinValue);
                            if (ts.TotalMinutes >= 0 && ts.TotalMinutes < 60)
                            {
                                adminbll.Update("lastlogindate=getdate()", "id=" + admin.id);
                            }
                            else
                            {
                                Cookie.ClearCookie("LMS_AdminID");
                                Cookie.ClearCookie("LMS_AdminName");
                                Cookie.ClearCookie("LMS_Popedom");
                                Cookie.ClearCookie("LMS_Code");
                                HttpContext.Current.Response.Write("<script>alert('已停留一小时未进入新页面，已自动退出！');top.location=('Login.aspx');</script>");
                                HttpContext.Current.Response.End();
                            }
                        }
#pragma warning restore CS0472 // 由于此类型的值永不等于 "null"，该表达式的结果始终相同
                    }
                    else
                    {
                        Cookie.ClearCookie("LMS_AdminID");
                        Cookie.ClearCookie("LMS_AdminName");
                        Cookie.ClearCookie("LMS_Popedom");
                        Cookie.ClearCookie("LMS_Code");
                        HttpContext.Current.Response.Write("<script>alert('你已在其它地方登陆！');top.location=('Login.aspx');</script>");
                        HttpContext.Current.Response.End();
                    }
                }
                else
                {
                    Cookie.ClearCookie("LMS_AdminID");
                    Cookie.ClearCookie("LMS_AdminName");
                    Cookie.ClearCookie("LMS_Popedom");
                    Cookie.ClearCookie("LMS_Code");
                    HttpContext.Current.Response.Write("<script>alert('管理员缓存信息错误，请重新登录！');top.location=('Login.aspx');</script>");
                    HttpContext.Current.Response.End();
                }
#pragma warning restore CS0472 // 由于此类型的值永不等于 "null"，该表达式的结果始终相同
            }

        }
        #endregion



        public static T DESerializer<T>(string strXML) where T : class
        {

            try
            {
                using (StringReader sr = new StringReader(strXML))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return serializer.Deserialize(sr) as T;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("将XML转换成实体对象异常", ex);
            }
        }

        public static string RandomNum(int num)
        {
            int[] arr = getRandomNum(num, 0, 9); //从1至20中取出6个互不相同的随机数
            int i = 0;
            string temp = "";
            while (i <= arr.Length - 1)
            {
                temp += arr[i].ToString();
                i++;
            }
            return temp; //显示在label1中
        }
        public static int[] getRandomNum(int num, int minValue, int maxValue)
        {
            Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
            int[] arrNum = new int[num];
            int tmp = 0;
            for (int i = 0; i <= num - 1; i++)
            {
                tmp = ra.Next(minValue, maxValue); //随机取数
                arrNum[i] = getNum(arrNum, tmp, minValue, maxValue, ra); //取出值赋到数组中
            }
            return arrNum;
        }
        public static int getNum(int[] arrNum, int tmp, int minValue, int maxValue, Random ra)
        {
            int n = 0;
            while (n <= arrNum.Length - 1)
            {
                if (arrNum[n] == tmp) //利用循环判断是否有重复
                {
                    tmp = ra.Next(minValue, maxValue); //重新随机获取。
                    getNum(arrNum, tmp, minValue, maxValue, ra);//递归:如果取出来的数字和已取得的数字有重复就重新随机获取。
                }
                n++;
            }
            return tmp;
        }

        /// <summary>
        /// Request读取值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        #region Request读取值
        public static string CheckURLSqlstr(string str)
        {
            if (str == null)
            {
                str = "";
            }
            else
            {
                str = Regex.Replace(str, "execiframe", " ", RegexOptions.IgnoreCase);
            }
            return str;
        }

        public static string CheckFormSqlstr(string str)
        {
            if (str == null)
            {
                str = "";
            }
            else
            {
                str = Regex.Replace(str.Trim(), "execiframe", " ", RegexOptions.IgnoreCase);
            }
            return str;
        }

        public static string FormRequest(string RequestName)
        {
            string str = CheckFormSqlstr(HttpContext.Current.Request.Form[RequestName]);
            return str;
        }
        public static string GetRequest(string RequestName)
        {
            string str = CheckURLSqlstr(Convert.ToString(HttpContext.Current.Request.QueryString[RequestName]));
            return str;
        }
        #endregion

        #region 返回的数组
        /// <summary>
        /// 返回的数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] ret_Power(string str)
        {
            string[] Dic = str.Split(',');

            return Dic;
        }
        #endregion

        #region 截取字符串

        /// <summary>
        /// 判断字符串长度 true 为超出限制
        /// </summary>
        public static bool CutStrBool(string stringToSub, int length)
        {
            Regex regex = new Regex("[\u4e00-\u9fa5]+", RegexOptions.Compiled);
            char[] stringChar = stringToSub.ToCharArray();
            StringBuilder sb = new StringBuilder();
            int nLength = 0;
            bool isCut = false;
            for (int i = 0; i < stringChar.Length; i++)
            {
                if (regex.IsMatch((stringChar[i]).ToString()))
                {
                    sb.Append(stringChar[i]);
                    nLength += 2;
                }
                else
                {
                    sb.Append(stringChar[i]);
                    nLength = nLength + 1;
                }

                if (nLength > length)
                {
                    isCut = true;
                    break;
                }
            }
            return isCut;

        }
        /// <summary>
        /// 截取字符串
        /// </summary>
        public static string CutStr(string stringToSub, int length)
        {
            Regex regex = new Regex("[\u4e00-\u9fa5]+", RegexOptions.Compiled);
            char[] stringChar = stringToSub.ToCharArray();
            StringBuilder sb = new StringBuilder();
            int nLength = 0;
            bool isCut = false;
            for (int i = 0; i < stringChar.Length; i++)
            {
                if (regex.IsMatch((stringChar[i]).ToString()))
                {
                    sb.Append(stringChar[i]);
                    nLength += 2;
                }
                else
                {
                    sb.Append(stringChar[i]);
                    nLength = nLength + 1;
                }

                if (nLength > length)
                {
                    isCut = true;
                    break;
                }
            }
            if (isCut)
                return sb.ToString() + "...";
            else
                return sb.ToString();
        }
        #endregion

        public static string HtmlInputValueEncode(string theString)
        {
            if (!string.IsNullOrWhiteSpace(theString))
            {
                theString = HtmlDiscode(theString);
                theString = theString.Replace("\"", "&quot;");
                theString = theString.Replace("<", "&lt;");
                theString = theString.Replace(">", "&gt;");
            }
            return HtmlSqlEncode(theString);
        }
        public static string HtmlInputValueDiscode(string theString)
        {
            if (!string.IsNullOrWhiteSpace(theString))
            {
                theString = theString.Replace("&quot;", "\"");
                theString = theString.Replace("&lt;", "<");
                theString = theString.Replace("&gt;", ">");
            }
            return HtmlSqlEncode(theString);
        }

        /// <summary>
        /// 替换html中的特殊字符
        /// </summary>
        /// <param name="theString">需要进行替换的文本。</param>
        /// <returns>替换完的文本。</returns>
        public static string HtmlEncode(string theString)
        {
            if (!string.IsNullOrWhiteSpace(theString))
            {
                theString = theString.Replace(" ", "&emsp;");
                theString = theString.Replace(" ", "&nbsp;");
                theString = theString.Replace(" ", "&nbsp;");
                theString = theString.Replace(" ", "&ensp;");
                theString = theString.Replace("'", "&#039;");
                theString = theString.Replace("\"", "&quot;");
                theString = theString.Replace("<", "&lt;");
                theString = theString.Replace(">", "&gt;");
                theString = theString.Replace("\r\n", "<br/>");
                theString = theString.Replace("\r", "<br/>");
                theString = theString.Replace("\n", "<br/>");
            }
            return HtmlSqlEncode(theString);
        }

        public static string HtmlSqlEncode(string theString)
        {
            if (!string.IsNullOrWhiteSpace(theString))
            {
                theString = theString.Replace("'", "&#039;");
                theString = ReplaceSqlKeywordToken(theString, "and");
                theString = ReplaceSqlKeywordToken(theString, "exec");
                theString = ReplaceSqlKeywordToken(theString, "insert");
                theString = ReplaceSqlKeywordToken(theString, "select");
                theString = ReplaceSqlKeywordToken(theString, "delete");
                theString = ReplaceSqlKeywordToken(theString, "update");
                theString = ReplaceSqlKeywordToken(theString, "count");
                theString = theString.Replace("*", "ξξ_*_ξξ");
                theString = theString.Replace("%", "ξξ_%_ξξ");
                theString = ReplaceSqlKeywordToken(theString, "chr");
                theString = ReplaceSqlKeywordToken(theString, "mid");
                theString = ReplaceSqlKeywordToken(theString, "master");
                theString = ReplaceSqlKeywordToken(theString, "truncate");
                theString = ReplaceSqlKeywordToken(theString, "char");
                theString = ReplaceSqlKeywordToken(theString, "declare");
                theString = ReplaceSqlKeywordToken(theString, "or");
            }
            return theString;
        }

        private static string ReplaceSqlKeywordToken(string value, string keyword)
        {
            return Regex.Replace(value, "\\b" + Regex.Escape(keyword) + "\\b", "ξξ_" + keyword + "_ξξ", RegexOptions.IgnoreCase);
        }

        private static string RemoveSqlKeywordTokens(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            return Regex.Replace(value, "(?:ξξ|§§|§)_([A-Za-z*%]+)_(?:ξξ|§§|§)", "$1");
        }
        public static string GetAdminUpload_Pic(string imgurl)
        {
            if (string.IsNullOrWhiteSpace(imgurl))
            {
                return "images/nophoto.gif";
            }
            else
            {
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath("../A_UpLoad/upload_pic/" + imgurl));
                if (file.Exists)
                {
                    return "../A_UpLoad/upload_pic/" + imgurl;
                }
                else
                {
                    return "images/nophoto.gif";
                }
            }
        }

        public static string GetWebUpload_Pic(string imgurl, string noimg)
        {
            if (imgurl == "" || imgurl == null)
            {
                return noimg;
            }
            else
            {
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/A_UpLoad/upload_pic/" + imgurl));
                if (file.Exists)
                {
                    return "/A_UpLoad/upload_pic/" + imgurl;
                }
                else
                {
                    return noimg;
                }
            }
        }


        public static string GetAdminIsShow(object isshow, object equals)
        {
            if (isshow.Equals(equals))
            {
                return "images/yes.gif";
            }
            else
            {
                return "images/no.gif";
            }
        }
        /// <summary>
        /// 输出硬盘文件，提供下载
        /// </summary>
        /// <param name="_Request">Page.Request对象</param>
        /// <param name="_Response">Page.Response对象</param>
        /// <param name="_fileName">下载文件名</param>
        /// <param name="_fullPath">带文件名下载路径</param>
        /// <param name="_speed">每秒允许下载的字节数</param>
        /// <returns>返回是否成功</returns>
        public static bool ResponseFile(HttpRequest _Request, HttpResponse _Response, string _fileName, string _titfileName, string _fullPath, long _speed)
        {
            string newfileName = _titfileName;
            try
            {
                FileStream myFile = new FileStream(_fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(myFile);
                try
                {
                    _Response.AddHeader("Accept-Ranges", "bytes");
                    _Response.Buffer = false;
                    long fileLength = myFile.Length;
                    long startBytes = 0;
                    int pack = 10240; //10K bytes
                    //int sleep = 200;   //每秒5次   即5*10K bytes每秒
                    int sleep = (int)Math.Floor((decimal)1000 * pack / _speed) + 1;
                    if (_Request.Headers["Range"] != null)
                    {
                        _Response.StatusCode = 206;
                        string[] range = _Request.Headers["Range"].Split(new char[] { '=', '-' });
                        startBytes = Convert.ToInt64(range[1]);
                    }
                    _Response.AddHeader("Content-Length", (fileLength - startBytes).ToString());
                    if (startBytes != 0)
                    {
                        _Response.AddHeader("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength - 1, fileLength));
                    }
                    _Response.AddHeader("Connection", "Keep-Alive");
                    _Response.ContentType = "application/octet-stream";
                    _Response.Charset = "UTF-8";
                    _Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    _Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(newfileName, System.Text.Encoding.UTF8));
                    br.BaseStream.Seek(startBytes, SeekOrigin.Begin);
                    int maxCount = (int)Math.Floor((decimal)(fileLength - startBytes) / pack) + 1;
                    for (int i = 0; i < maxCount; i++)
                    {
                        if (_Response.IsClientConnected)
                        {
                            _Response.BinaryWrite(br.ReadBytes(pack));
                            Thread.Sleep(sleep);
                        }
                        else
                        {
                            i = maxCount;
                        }
                    }
                    _Response.End();
                }
                catch
                {
                    return false;
                }
                finally
                {
                    br.Close();
                    myFile.Close();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 恢复html中的特殊字符
        /// </summary>
        /// <param name="theString">需要恢复的文本。</param>
        /// <returns>恢复好的文本。</returns>
        public static string HtmlDiscode(string theString)
        {
            if (!string.IsNullOrWhiteSpace(theString))
            {
                theString = RemoveSqlKeywordTokens(theString);
                theString = theString.Replace("ξξ_", "");
                theString = theString.Replace("_ξξ", "");
                theString = theString.Replace("&emsp;", " ");
                theString = theString.Replace("&nbsp;", " ");
                theString = theString.Replace("&ensp;", " ");
                theString = theString.Replace("&#039;", "'");
                theString = theString.Replace("&quot;", "\"");
                theString = theString.Replace("&lt;", "<");
                theString = theString.Replace("&gt;", ">");
                theString = theString.Replace("<br/>", "\r\n");
            }
            return theString;
        }
        public static string HtmlSqlDiscode(string theString)
        {
            theString = RemoveSqlKeywordTokens(theString);
            theString = theString.Replace("ξξ_", "");
            theString = theString.Replace("_ξξ", "");
            theString = theString.Replace("&#039;", "'");
            return theString;
        }
        public static string HtmlDiscodeWeb(string theString)
        {
            if (!string.IsNullOrWhiteSpace(theString))
            {
                theString = RemoveSqlKeywordTokens(theString);
                theString = theString.Replace("ξξ_", "");
                theString = theString.Replace("_ξξ", "");
                theString = theString.Replace("&emsp;", " ");
                theString = theString.Replace("&nbsp;", " ");
                theString = theString.Replace("&ensp;", " ");
                theString = theString.Replace("&#039;", "'");
                theString = theString.Replace("&quot;", "\"");
                theString = theString.Replace("&lt;", "<");
                theString = theString.Replace("&gt;", ">");
            }
            return theString;
        }

        /// <summary>
        /// 文件路劲的替换
        /// </summary>
        /// <param name="theString"></param>
        /// <returns></returns>
        public static string Replace_Content(string theString)
        {
            if (!string.IsNullOrWhiteSpace(theString))
            {
                if (theString.IndexOf("../A_UpLoad/") >= 0)
                {
                    theString = theString.Replace("../A_UpLoad/", "/A_UpLoad/");
                }
                if (theString.IndexOf("../A_UpLoadTool/") >= 0)
                {
                    theString = theString.Replace("../A_UpLoadTool/", "/A_UpLoadTool/");
                }
                if (theString.IndexOf("ξξ_") >= 0)
                {
                    theString = RemoveSqlKeywordTokens(theString);
                    theString = theString.Replace("ξξ_", "");
                }
                if (theString.IndexOf("_ξξ") >= 0)
                {
                    theString = theString.Replace("_ξξ", "");
                }
            }
            return theString;
        }



        /// <summary>
        /// 文件路劲改为后台路劲
        /// </summary>
        /// <param name="theString"></param>
        /// <returns></returns>
        public static string U_Replace_Content(string theString)
        {
            if (theString.IndexOf("A_UpLoad/") >= 0)
            {
                theString = theString.Replace("A_UpLoad/", "../A_UpLoad/");
            }
            return theString;
        }
        //// <summary>
        /// DEC 加密过程
        /// </summary>
        /// <param name="pToEncrypt">被加密的字符串</param>
        /// <param name="sKey">密钥（只支持8个字节的密钥）</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string pToEncrypt)
        {
            //访问数据加密标准(DES)算法的加密服务提供程序 (CSP) 版本的包装对象
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes("lms2026a");　//建立加密对象的密钥和偏移量
            des.IV = ASCIIEncoding.ASCII.GetBytes("lms2026a");　 //原文使用ASCIIEncoding.ASCII方法的GetBytes方法

            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);//把字符串放到byte数组中

            MemoryStream ms = new MemoryStream();//创建其支持存储区为内存的流　
            //定义将数据流链接到加密转换的流
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            //上面已经完成了把加密后的结果放到内存中去

            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        /// <summary>
        /// DEC 解密过程
        /// </summary>
        /// <param name="pToDecrypt">被解密的字符串</param>
        /// <param name="sKey">密钥（只支持8个字节的密钥，同前面的加密密钥相同）</param>
        /// <returns>返回被解密的字符串</returns>
        public static string Decrypt(string pToDecrypt)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            des.Key = ASCIIEncoding.ASCII.GetBytes("lms2026a");　//建立加密对象的密钥和偏移量，此值重要，不能修改
            des.IV = ASCIIEncoding.ASCII.GetBytes("lms2026a"); //只支持8个字节的密钥
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            //建立StringBuild对象，createDecrypt使用的是流对象，必须把解密后的文本变成流对象
            StringBuilder ret = new StringBuilder();

            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }
        /// <summary>
        /// 字符加密MD5
        /// </summary>
        /// <param name="str"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        #region 字符加密MD5

        public static string MD5(string str, int code)
        {

            if (code == 16)
            {
#pragma warning disable CS0618 // 类型或成员已过时
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower().Substring(8, 16);
#pragma warning restore CS0618 // 类型或成员已过时
            }

            if (code == 32)
            {
#pragma warning disable CS0618 // 类型或成员已过时
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
#pragma warning restore CS0618 // 类型或成员已过时
            }

            return "00000000000000000000000000000000";
        }

        #endregion

        #region 字符加密解密
        //加密算法
        public static string MD5Encrypt(string pToEncrypt, string Key)
        {
            if (string.IsNullOrWhiteSpace(Key))
            {
                Key = "96041408";//加密密钥必须为8位
            }
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            des.Key = ASCIIEncoding.ASCII.GetBytes(Key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(Key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();

        }

        //解密算法
        public static string MD5Decrypt(string pToDecrypt, string Key)
        {
            if (string.IsNullOrWhiteSpace(Key))
            {
                Key = "96041408";//加密密钥必须为8位
            }
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(Key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(Key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            return System.Text.Encoding.ASCII.GetString(ms.ToArray());

        }
        #endregion



        #region 提示信息！操作回滚！
        /// <summary>
        /// 提示信息！操作回滚！
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="code"></param>
        public static void Show_Msg(string msg, string url)
        {
            if (url.ToUpper() == "BACK")
            {
                HttpContext.Current.Response.Write("<script>alert(\"" + msg + "\");history.back();</script>");
                HttpContext.Current.Response.End();
            }
            else if (url.ToUpper() == "CLOSE")
            {

                HttpContext.Current.Response.Write("<script>alert(\"" + msg + "\");window.opener=null;window.open('','_self','');window.close();</script>");
                HttpContext.Current.Response.End();
            }
            else
            {
                HttpContext.Current.Response.Write("<script>alert(\"" + msg + "\");window.location=('" + url + "');</script>");
                HttpContext.Current.Response.End();
            }
        }
        #endregion


        #region 防止外部提交
        /// <summary>
        /// 防止外部提交
        /// </summary>
        public static void IsSelfRefer()
        {
            string sHttp_Referer = "";
            string Server_Name = "";
            sHttp_Referer = Convert.ToString(HttpContext.Current.Request.ServerVariables["HTTP_REFERER"]);
            Server_Name = Convert.ToString(HttpContext.Current.Request.ServerVariables["SERVER_NAME"]);
            if (sHttp_Referer != null)
            {
                if (!(Function.CutString(sHttp_Referer, 7, Server_Name.Length) == Server_Name || Function.CutString(sHttp_Referer, 8, Server_Name.Length) == Server_Name || Function.CutString(sHttp_Referer, 6, Server_Name.Length) == Server_Name))
                {
                    HttpContext.Current.Response.Write("禁止外部提交数据!");
                    HttpContext.Current.Response.End();
                }
            }
            else
            {
                HttpContext.Current.Response.Write("禁止外部提交数据!");
                HttpContext.Current.Response.End();
            }
        }

        /// <summary>
        /// 从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex, int length)
        {
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length = length * -1;
                    if (startIndex - length < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        startIndex = startIndex - length;
                    }
                }


                if (startIndex > str.Length)
                {
                    return "";
                }


            }
            else
            {
                if (length < 0)
                {
                    return "";
                }
                else
                {
                    if (length + startIndex > 0)
                    {
                        length = length + startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        return "";
                    }
                }
            }

            if (str.Length - startIndex < length)
            {
                length = str.Length - startIndex;
            }

            return str.Substring(startIndex, length);
        }
        #endregion

        #region 获取URL
        /// <summary>
        /// 获取完整URL(加密)
        /// </summary>
        public static string GetEncodeURL()
        {
            return HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.Url.ToString());
        }
        /// <summary>
        /// 获取完整URL(不加密)
        /// </summary>
        public static string GetDecodeURL()
        {
            return HttpContext.Current.Request.Url.ToString();
        }
        #endregion

        #region 返回执行结果
        ///// <summary>
        ///// 返回执行结果
        ///// </summary>
        public static void Ok_Return(string UserName, string title, string URL, int t)
        {
            string img = "";
            string Ip = "";
            StringBuilder sb = new StringBuilder();
#pragma warning disable CS0168 // 声明了变量，但从未使用过
            try
            {
                Ip = GetClientIP();
                if (Cookie.GetCookie("LMS_AdminName").ToUpper() != "SYSADMIN")
                {
                    user_login user_login = new user_login();
                    BLLBase<user_login> user_loginbll = new BLLBase<user_login>();
                    user_login.username = Function.HtmlEncode(UserName);
                    user_login.time = DateTime.Now;
                    user_login.ip = Function.HtmlEncode(Ip);
                    user_login.content = Function.HtmlEncode(title);
                    user_loginbll.Add(user_login, "id");

                }
            }
            catch (Exception ex)
            {

                throw;
            }
#pragma warning restore CS0168 // 声明了变量，但从未使用过


#pragma warning disable CS0472 // 由于此类型的值永不等于 "null"，该表达式的结果始终相同
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(URL) || t == null)
            {
                Show_Msg("非法操作！", "");

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

            sb.Append("<link media='all' type='text/css' href='/admin/css/style.css' rel='stylesheet' /><div class='container' id='cpcontainer'><h3>操作提示</h3><div class='infobox'>");
            sb.Append("<table width='600' border='0' align='center' cellpadding='4' cellspacing='0'><tr><td width='154' rowspan='2' align='right'>");
            sb.Append("<img height='32' alt='information' src='/admin/images/" + img + "' width='32' border='0' style='margin-right:10px;' /></td><td width='430' align='left' class='infotitle2'>" + title + "</td></tr><tr>");
            sb.Append("<td align='left'>将在 <span id='spanSeconds'>2</span> 秒后跳转到第一个链接地址。</td></tr></table></div></div></div>");
            sb.Append("<script>" + "\r\n");
            sb.Append("var seconds = 2;" + "\r\n");
            sb.Append("var defaultUrl = '" + URL + "';" + "\r\n");

            sb.Append("onload = function()" + "\r\n");
            sb.Append("{" + "\r\n");
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
        #endregion
        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            return result;
        }
        public static string KindEditor(string str_name, int num)
        {
            StringBuilder sb = new StringBuilder();
            if (num == 1)
            {
                sb.Append("	<link rel=\"stylesheet\" href=\"../A_UpLoadTool/themes/default/default.css\" />" + "\r\n");
                sb.Append("	<script charset=\"utf-8\" src=\"../A_UpLoadTool/kindeditor.js\"></script>" + "\r\n");
                sb.Append("	<script charset=\"utf-8\" src=\"../A_UpLoadTool/lang/zh_CN.js\"></script>" + "\r\n");
            }
            sb.Append("	<script>" + "\r\n");
            sb.Append("		KindEditor.ready(function(K) {" + "\r\n");
            sb.Append("var " + str_name + " = K.create('#" + str_name + "', {" + "\r\n");
            sb.Append("	cssPath : '../A_UpLoadTool/plugins/code/prettify.css'," + "\r\n");
            sb.Append("	uploadJson : '../A_UpLoadTool/tool/upload_json.aspx'," + "\r\n");
            sb.Append("	fileManagerJson : '../A_UpLoadTool/tool/file_manager_json.ashx'," + "\r\n");
            sb.Append("	allowFileManager : false," + "\r\n");
            sb.Append("	newlineTag :  'br'," + "\r\n");
            sb.Append("	urlType :  'relative'," + "\r\n");
            sb.Append("	filterMode : false," + "\r\n");
            sb.Append("	afterCreate : function() {" + "\r\n");
            sb.Append("		var self = this;" + "\r\n");
            sb.Append("		K.ctrl(document, 13, function() {" + "\r\n");
            sb.Append("self.sync();" + "\r\n");
            sb.Append("K('form[name=" + str_name + "]')[0].submit();" + "\r\n");
            sb.Append("		});" + "\r\n");
            sb.Append("		K.ctrl(self.edit.doc, 13, function() {" + "\r\n");
            sb.Append("self.sync();" + "\r\n");
            sb.Append("K('form[name=" + str_name + "]')[0].submit();" + "\r\n");
            sb.Append("		});" + "\r\n");
            sb.Append("	}" + "\r\n");
            sb.Append("});" + "\r\n");
            sb.Append("		});" + "\r\n");
            sb.Append("	</script>");
            return sb.ToString();
        }


        /// <summary>
        /// 字符串转化
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(object val, T defaultVal)
        {
            if (Convert.IsDBNull(val) || val == null)
                return defaultVal;
            else
            {
                try
                {
                    return (T)Convert.ChangeType(val, typeof(T));
                }
                catch
                {
                    return defaultVal;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool FileDelete(string path)
        {
            bool ret = false;
            FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(path));
            if (file.Exists)
            {
                file.Delete();
                ret = true;
            }
            return ret;
        }


    }

}
