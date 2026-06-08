using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Configuration;
using System.Data;
using System.Web;

namespace Web.admin
{
    public partial class Login : System.Web.UI.Page
    {
        BLLBase<Model.admin> adminbll = new BLLBase<Model.admin>();
        BLLBase<user_login> user_loginbll = new BLLBase<user_login>();
        BLLBase<logincode_list> logincode_listbll = new BLLBase<logincode_list>();
        public bool loading = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                logincode_listbll.Delete("addtime < DATEADD(MINUTE, -5, GETDATE())");

                loading = false;
                if (Cookie.GetCookie("LMS_AdminID") != null && Cookie.GetCookie("LMS_AdminName") != null && Cookie.GetCookie("LMS_Popedom") != null && Cookie.GetCookie("LMS_AdminID") != "" && Cookie.GetCookie("LMS_AdminName") != "" && Cookie.GetCookie("LMS_Popedom") != "")
                {
                    Response.Redirect("index.aspx");
                }
                else
                {
                    loading = true;
                }
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex is HttpRequestValidationException)
            {
                Response.Write("请您输入合法字符串。");
                Server.ClearError(); // 如果不ClearError()这个异常会继续传到Application_Error()。
            }
        }

        protected void AdminLogin_Click(object sender, EventArgs e)
        {
            string user_name = Function.HtmlEncode(Function.FormRequest("user_name"));
            string user_pwd = Function.MD5(Function.FormRequest("user_pwd"), 32);
            string user_code = Function.FormRequest("Code").ToLower();
            string BackstageCheckCode_ = Cookie.GetCookie("BackstageCheckCode");

            bool iscode = false;
            if (!string.IsNullOrWhiteSpace(user_code) && !string.IsNullOrWhiteSpace(BackstageCheckCode_))
            {
                logincode_list logincode_list = logincode_listbll.SelectSingle("code='" + Function.MD5Decrypt(BackstageCheckCode_, ConfigurationManager.AppSettings["md5_key"]) + "' and val='" + user_code + "' and addtime >= DATEADD(MINUTE, -5, GETDATE())");
                if (logincode_list != null && !string.IsNullOrWhiteSpace(logincode_list.val))
                {
                    iscode = true;
                }
            }
            if (iscode)
            {
                CommonFunc.GetLoginCodeDelete();
                Cookie.ClearCookie("BackstageCheckCode");
                Model.admin admin = adminbll.SelectSingle("username='" + user_name + "'");
                if (admin != null && admin.id > 0)
                {
                    DataTable user_login_bt = user_loginbll.GetDatatable("select * from user_login where datediff(HOUR,time,getdate())<=3 and username='" + user_name + "' and content like '%登录失败！ 原因：%'");

                    if (!(user_login_bt != null && user_login_bt.Rows.Count > 2))
                    {
                        if (admin.password.Equals(user_pwd))
                        {

                            if (admin.locks == 1)
                            {
                                Function.Show_Msg("该用户已被冻结，请联系管理员！", "login.aspx");
                            }
                            else
                            {
                                string logincode = GetCaptcha();
                                Cookie.SaveCookie("LMS_AdminID", admin.id.ToString(), 0);
                                Cookie.SaveCookie("LMS_AdminName", admin.username, 0);
                                Cookie.SaveCookie("LMS_Popedom", admin.popedom, 0);
                                Cookie.SaveCookie("LMS_Code", Function.MD5Encrypt(logincode, ConfigurationManager.AppSettings["md5_key"]), 0);
                                if (admin.username.ToUpper() != "SYSADMIN")
                                {
                                    try
                                    {
                                        user_login user_login = new user_login();
                                        user_login.username = user_name;
                                        user_login.time = DateTime.Now;
                                        user_login.ip = Function.GetClientIP();
                                        user_login.password = user_pwd;
                                        user_login.content = user_name + "登录成功！";
                                        user_loginbll.Add(user_login, "id");
                                    }
                                    catch (Exception)
                                    {
                                        throw;
                                    }

                                }
                                user_loginbll.Delete("datediff(HOUR,time,getdate())<=3 and username='" + user_name + "' and content like '%登录失败！ 原因：%'");
                                adminbll.Update("lastlogindate=getdate(),code='" + logincode + "'", "id=" + admin.id);
                                Response.Redirect("index.aspx");
                            }
                        }
                        else
                        {
                            if (admin.username.ToUpper() != "SYSADMIN")
                            {
                                try
                                {
                                    user_login user_login = new user_login();
                                    user_login.username = user_name;
                                    user_login.time = DateTime.Now;
                                    user_login.ip = Function.GetClientIP();
                                    user_login.password = Function.HtmlEncode(Function.FormRequest("user_pwd"));
                                    user_login.content = "<font color=red>" + user_name + "登录失败！ 原因：密码<font color=green>[" + user_name + "]</font>输入有误！</font>')";
                                    user_loginbll.Add(user_login, "id");
                                }
                                catch (Exception)
                                {

                                    throw;
                                }
                            }
                            int num = 0;
                            DataTable user_login_bt_ = user_loginbll.GetDatatable("select * from user_login where datediff(HOUR,time,getdate())<=3 and username='" + user_name + "' and content like '%登录失败！ 原因：%'");
                            if (user_login_bt_ != null)
                            {
                                num = user_login_bt_.Rows.Count;
                            }

                            Function.Show_Msg("密码有误，3个小时内登录失败次数达到3次帐号会被禁用，已错" + num + "次！", "login.aspx");
                        }
                    }
                    else
                    {
                        user_login user_login = new user_login();
                        user_login.username = user_name;
                        user_login.time = DateTime.Now;
                        user_login.ip = Function.GetClientIP();
                        user_login.password = Function.HtmlEncode(Function.FormRequest("user_pwd"));
                        user_login.content = "<font color=red>" + user_name + "登录失败！ 原因：用户名<font color=green>[" + user_name + "]</font>3个小时内登录失败次数达到3次已被禁用！</font>')";
                        user_loginbll.Add(user_login, "id");
                        adminbll.Update("locks=1", "id=" + admin.id);

                        Function.Show_Msg("3个小时内登录失败次数达到3次帐号已被禁用！", "login.aspx");
                    }
                }
                else
                {
                    if (user_name.ToUpper() != "SYSADMIN")
                    {
                        try
                        {
                            user_login user_login = new user_login();
                            user_login.username = user_name;
                            user_login.time = DateTime.Now;
                            user_login.ip = Function.GetClientIP();
                            user_login.password = Function.HtmlEncode(Function.FormRequest("user_pwd"));
                            user_login.content = "<font color=red>" + user_name + "登录失败！ 原因：用户名<font color=green>[" + user_name + "]</font>输入有误！</font>')";
                            user_loginbll.Add(user_login, "id");
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    int num = 0;
                    DataTable user_login_bt_ = user_loginbll.GetDatatable("select * from user_login where datediff(HOUR,time,getdate())<=3 and username='" + user_name + "' and content like '%登录失败！ 原因：%'");
                    if (user_login_bt_ != null)
                    {
                        num = user_login_bt_.Rows.Count;
                    }

                    Function.Show_Msg("密码有误，3个小时内登录失败次数达到3次帐号会被禁用，已错" + num + "次！", "login.aspx");
                }
            }
            else
            {
                CommonFunc.GetLoginCodeDelete();
                Cookie.ClearCookie("BackstageCheckCode");
                if (user_name.ToUpper() != "SYSADMIN")
                {
                    try
                    {
                        user_login user_login = new user_login();
                        user_login.username = user_name;
                        user_login.time = DateTime.Now;
                        user_login.ip = Function.GetClientIP();
                        user_login.password = Function.HtmlEncode(Function.FormRequest("user_pwd"));
                        user_login.content = "<font color=red>" + user_name + "登录失败！ 原因：验证码<font color=green>[" + user_code + "]</font>输入有误！</font>'";
                        user_loginbll.Add(user_login, "id");
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                int num = 0;
                DataTable user_login_bt_ = user_loginbll.GetDatatable("select * from user_login where datediff(HOUR,time,getdate())<=3 and username='" + user_name + "' and content like '%登录失败！ 原因：%'");
                if (user_login_bt_ != null)
                {
                    num = user_login_bt_.Rows.Count;
                }

                Function.Show_Msg("验证码错误，3个小时内登录失败次数达到3次帐号会被禁用，已错" + num + "次！", "login.aspx");
            }

        }


        public string GetCaptcha()
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
    }



}