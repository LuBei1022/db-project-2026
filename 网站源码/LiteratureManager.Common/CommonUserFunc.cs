
using BLL;
using DAL;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace LiteratureManager.Common
{
    public class CommonUserFunc
    {
        public static BLLBase<user_list> user_listbll = new BLLBase<user_list>();
        public static BLLBase<telcode_list> telcode_listbll = new BLLBase<telcode_list>();
        public static BLLBase<userimg_list> userimg_listbll = new BLLBase<userimg_list>();
        public static BLLBase<appeal_list> appeal_listbll = new BLLBase<appeal_list>();
        public static BLLBase<NoticeLog_List> NoticeLog_Listbll = new BLLBase<NoticeLog_List>();
        public static BLLBase<integrateLogType_list> integrateLogType_listbll = new BLLBase<integrateLogType_list>();
        public static BLLBase<integrate_list> integrate_listbll = new BLLBase<integrate_list>();
        public static BLLBase<integrateExchangeLog_list> integrateExchangeLog_listbll = new BLLBase<integrateExchangeLog_list>();
        public static BLLBase<integrateLog_list> integrateLog_listbll = new BLLBase<integrateLog_list>();
        public static BLLBase<ServiceLog_List> ServiceLog_Listbll = new BLLBase<ServiceLog_List>();
        public static BLLBase<Literature> LiteratureBll = new BLLBase<Literature>();
        public static BLLBase<LiteratureComment> LiteratureCommentBll = new BLLBase<LiteratureComment>();
        public static BLLBase<LiteratureLike> LiteratureLikeBll = new BLLBase<LiteratureLike>();
        public static BLLBase<LiteratureFavorite> LiteratureFavoriteBll = new BLLBase<LiteratureFavorite>();
        public static BLLBase<TopUpType_List> TopUpType_Listbll = new BLLBase<TopUpType_List>(); 
        public static BLLBase<websiteinfo_list> websiteinfo_listbll = new BLLBase<websiteinfo_list>();
        public static string A_UpLoad_Url = ConfigurationManager.AppSettings["website_url"] + "/A_UpLoad/upload_pic/";
        public static string upload_file_Url = ConfigurationManager.AppSettings["website_url"] + "/A_UpLoad/upload_file/";
        public static string MD5Key = ConfigurationManager.AppSettings["md5_key"];
        public static string fileStorageBaseUrl = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["file_storage_base_url"])
            ? upload_file_Url
            : ConfigurationManager.AppSettings["file_storage_base_url"];

        public static int GetNoticeLogNum(int user_id)
        {
            int num = 0;
            DataTable NoticeLog_List_dt = NoticeLog_Listbll.GetDatatable("select count(1) as num from NoticeLog_List where userid=" + user_id);
            if (NoticeLog_List_dt != null && NoticeLog_List_dt.Rows.Count > 0)
            {
                num = Function.ConvertTo<int>(NoticeLog_List_dt.Rows[0]["num"].ToString(), 0);
            }
            NoticeLog_List_dt.Dispose();
            return num;
        }
        public static string GetUpUserEmailFunc(string email, int userid)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                user_list user_list = GetUserLoginStatus();
                if (user_list != null && user_list.id > 0 && userid == user_list.id)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        if (user_listbll.Update("email='" + Function.HtmlEncode(email) + "'", "id=" + user_list.id))
                        {
                            str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = "保存成功！" });
                        }
                        else
                        {
                            str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "保存失败！" });
                        }
                    }
                    else
                    {
                        str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "*为必填项！" });
                    }
                }
                else
                {
                    str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = -1, ["info"] = "登录状态异常！" });
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
            return str_;
        }

        public static string GetUpUserNameFunc(string name, int userid)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                user_list user_list = GetUserLoginStatus();
                if (user_list != null && user_list.id > 0 && userid == user_list.id)
                {
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        if (user_listbll.Update("name='" + Function.HtmlEncode(name) + "'", "id=" + user_list.id))
                        {
                            str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = "保存成功！" });
                        }
                        else
                        {
                            str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "保存失败！" });
                        }
                    }
                    else
                    {
                        str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "*为必填项！" });
                    }
                }
                else
                {
                    str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = -1, ["info"] = "登录状态异常！" });
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
            return str_;
        }

        public static string IsTelCode(string tel, string code, int type, int img_x, int img_y)
        {
            string err = "校验验证码异常";
            telcode_list telcode_list = telcode_listbll.SelectSingle("tel='" + Function.HtmlEncode(tel) + "' and type=" + type + " and code='" + code + "' and img_x=" + img_x + " and img_y=" + img_y);
            if (telcode_list != null)
            {
                if (telcode_list.addtime.AddMinutes(5) > DateTime.Now)
                {
                    if (telcode_listbll.Sql_D("DELETE FROM telcode_list WHERE tel='" + Function.HtmlEncode(tel) + "' and type=" + type))
                    {
                        err = "ok";
                    }
                    else
                    {
                        err = "核销短信验证码失败";
                    }
                }
                else
                {
                    err = "短信验证码已失效";
                }
            }
            else
            {
                err = "短信验证码错误";
            }
            return err;
        }
        public static string GetUserLoginFunc(string tel, string code, string img_x, string img_y, string type)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                user_list user_list = GetUserLoginStatus();
                if (user_list != null && user_list.id > 0)
                {
                    str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = "已登录，自动跳转！" });
                }
                else
                {
                    string err = IsTelCode(tel, code, Function.ConvertTo<int>(type, 0), Function.ConvertTo<int>(img_x, 0), Function.ConvertTo<int>(img_y, 0));

                    if (err == "ok")
                    {
                        user_list user_list_model = user_listbll.SelectSingle("tel='" + Function.HtmlEncode(tel) + "'");
                        if (user_list_model != null && user_list_model.id > 0)
                        {
                            if (user_list_model.isshow == 1)
                            {
                                string logincode = CommonFunc.GetCaptcha();
                                Cookie.SaveCookie("user_id", user_list_model.id.ToString(), 0);
                                Cookie.SaveCookie("user_tel", user_list_model.tel, 0);
                                Cookie.SaveCookie("user_code", Function.MD5Encrypt(logincode, MD5Key), 0);
                                if (user_listbll.Update("logintime=getdate(),loginip='" + Function.HtmlEncode(Function.GetClientIP()) + "',code='" + logincode + "'", "id=" + user_list_model.id))
                                {
                                    str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = "登录成功！" });
                                }
                                else
                                {
                                    str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "登录异常，请稍后再试！" });
                                }
                            }
                            else
                            {
                                str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "账号已锁定，请联系网站相关人员处理！" });
                            }
                        }
                        else
                        {

                            StringBuilder strSql = new StringBuilder();
                            strSql.Append("insert into user_list(");
                            strSql.Append("tel, isshow,addtime, uptime, loginip, logintime, code)");
                            strSql.Append(" values (");
                            strSql.Append(" @tel, @isshow,@addtime, @uptime, @loginip, @logintime, @code)");
                            strSql.Append(";select @@IDENTITY");
                            SqlParameter[] parameters = {
                        new SqlParameter("@tel", SqlDbType.NVarChar,150),
                          new SqlParameter("@isshow",SqlDbType.Int),
                          new SqlParameter("@addtime",SqlDbType.DateTime),
                          new SqlParameter("@uptime",SqlDbType.DateTime),
                          new SqlParameter("@loginip",SqlDbType.NVarChar,50),
                          new SqlParameter("@logintime",SqlDbType.NVarChar,50),
                          new SqlParameter("@code",SqlDbType.NVarChar,50)
                                    };

                            string logincode = CommonFunc.GetCaptcha();
                            parameters[0].Value = Function.HtmlEncode(tel);
                            parameters[1].Value = 1;
                            parameters[2].Value = DateTime.Now;
                            parameters[3].Value = DateTime.Now;
                            parameters[4].Value = Function.GetClientIP();
                            parameters[5].Value = DateTime.Now.ToString();
                            parameters[6].Value = logincode;

                            int? num_integrate = 5;
                            integrateLogType_list integrateLogType_list = integrateLogType_listbll.SelectSingle("id=1");
                            if (integrateLogType_list != null && integrateLogType_list.id > 0 && integrateLogType_list.num_integrate > 0)
                            {
                                num_integrate = integrateLogType_list.num_integrate;
                            }
                            string sql = "INSERT INTO integrateLog_list (num_integrate, type, name, info_, addtime, user_id) VALUES (" + num_integrate + ",1,'新用户注册','',GETDATE(),LiteratureManagerteshu)";

                            int add_user_id = user_listbll.Add_R_Id_(parameters, strSql, sql);
                            if (add_user_id > 0)
                            {
                                Cookie.SaveCookie("user_id", add_user_id.ToString(), 0);
                                Cookie.SaveCookie("user_tel", Function.HtmlEncode(tel), 0);
                                Cookie.SaveCookie("user_code", Function.MD5Encrypt(logincode, MD5Key), 0);
                                str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = "注册成功！", ["url"] = "/User/UserInfo" });
                            }
                            else
                            {
                                str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "注册失败！" });
                            }
                        }
                    }
                    else
                    {
                        str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = err });
                    }
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
            return str_;
        }
        public static string GetUpUserTelFunc(string tel, string code, string img_x, string img_y, string type, int userid)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                user_list user_list = GetUserLoginStatus();
                if (user_list != null && user_list.id > 0 && userid == user_list.id)
                {
                    if (!string.IsNullOrWhiteSpace(tel) && !string.IsNullOrWhiteSpace(code))
                    {
                        if (tel == user_list.tel)
                        {
                            str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "亲，新填手机号和原来一样！" });
                        }
                        else
                        {
                            string err = IsTelCode(tel, code, Function.ConvertTo<int>(type, 0), Function.ConvertTo<int>(img_x, 0), Function.ConvertTo<int>(img_y, 0));
                            if (err == "ok")
                            {
                                user_list user_list_model = user_listbll.SelectSingle("tel='" + Function.HtmlEncode(tel) + "'");
                                if (user_list_model != null && user_list_model.id > 0 && user_list_model.id != user_list.id)
                                {
                                    str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "新手机号已绑定在其他账号下！" });
                                }
                                else
                                {
                                    if (user_listbll.Update("tel='" + Function.HtmlEncode(tel) + "'", "id=" + user_list.id))
                                    {

                                        Cookie.SaveCookie("user_tel", tel, 0);
                                        str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = "保存成功！" });
                                    }
                                    else
                                    {
                                        str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "保存失败！" });
                                    }
                                }
                            }
                            else
                            {
                                str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = err });
                            }
                        }
                    }
                    else
                    {
                        str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "*为必填项！" });
                    }
                }
                else
                {
                    str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = -1, ["info"] = "登录状态异常！" });
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
            return str_;
        }
        public class returnParameters
        {
            /// <summary>
            /// 返回结果【Success/Error】
            /// </summary>
            public bool result { get; set; }
            /// <summary>
            /// 描述
            /// </summary>
            public string errmsg { get; set; }
            /// <summary>
            /// 返回成功的链接
            /// </summary>
            public string code_url { get; set; }
        }


        public static string GetTaskAddTopUpFunc(string money_str,string typestr, string id_str)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                string wxConfigError = ValidateWxPayConfig();
                if (!string.IsNullOrWhiteSpace(wxConfigError))
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = wxConfigError });
                }

                user_list user_list = GetUserLoginStatus();
                if (user_list != null && user_list.id > 0 )
                {
                    int money = Function.ConvertTo<int>(money_str, 0);
                    int id = Function.ConvertTo<int>(id_str, 0);
                    TopUpType_List TopUpType_List = TopUpType_Listbll.SelectSingle("id=" + id + " and isshow=1");
                    int TopUpType_money = 0;
                    if (money > 0) {
                        TopUpType_money = money;
                    } else if (TopUpType_List != null && TopUpType_List.money > 0) {
                        TopUpType_money = TopUpType_List.money;
                    }
                    if (TopUpType_money > 0)
                    {
                        if (typestr == "wx")
                        {
                            var orderNumber = $"{DateTime.Now:yyyyMMddHHmmssff}{CodeHelper.CreateNumCode(3)}";
                            var helper = new WxPayHelper(WxPayConst.appid, WxPayConst.mchid, WxPayConst.serialNo, WxPayConst.privateKey);
                            var notify_url = ConfigurationManager.AppSettings["wx_notify_url"];
                            if (string.IsNullOrWhiteSpace(notify_url))
                            {
                                notify_url = (ConfigurationManager.AppSettings["website_url"] ?? string.Empty).TrimEnd('/') + "/wx_pay_notify.aspx";
                            }

                            LiteratureManager.Common.returnParameters result = helper.NativeGetprepay("文献积分充值：" + TopUpType_money + "元", TopUpType_money, orderNumber, notify_url, Function.GetClientIP(), "integrate_topup").GetAwaiter().GetResult();
                            if (result != null && result.result && !string.IsNullOrWhiteSpace(result.code_url))
                            {
                                SavePendingTopUpOrder(user_list.id, orderNumber, 1, TopUpType_money);
                                websiteinfo_list websiteinfo = GetWebsiteInfoConfig();
                                int integrateAmount = GetRechargeIntegrateAmount(TopUpType_money, websiteinfo);
                                int giftAmount = GetRechargeGiftAmount(user_list.id, TopUpType_money, websiteinfo);
                                str_ = serializer.Serialize(new Dictionary<string, object>
                                {
                                    ["status"] = 1,
                                    ["code_url"] = result.code_url,
                                    ["money"] = TopUpType_money,
                                    ["out_trade_no"] = orderNumber,
                                    ["payImg"] = "/images/pay1.png",
                                    ["payType"] = "微信支付",
                                    ["integrate_amount"] = integrateAmount,
                                    ["gift_amount"] = giftAmount
                                });
                            }
                            else
                            {
                                str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = result == null ? "微信下单失败，请稍后再试" : result.errmsg });
                            }
                        }
                        else
                        {
                            str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "当前仅支持微信扫码充值" });
                        }
                    }
                    else
                    {
                        str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "充值金额为必选项！" });
                    }
                }
                else
                {
                    str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = -1, ["info"] = "登录状态异常！" });
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
                string friendlyInfo = "微信支付下单失败，请检查商户配置或服务器网络。";
                if (ex.Message.IndexOf("发送请求时出错", StringComparison.OrdinalIgnoreCase) >= 0
                    || ex.Message.IndexOf("Unable to connect", StringComparison.OrdinalIgnoreCase) >= 0
                    || ex.Message.IndexOf("连接", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    friendlyInfo = "微信支付通道暂时不可用，请检查服务器是否可访问微信支付接口，或核对商户号、AppID、证书序列号和私钥。";
                }
                str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = friendlyInfo });
            }
            return str_;
        }

        public static string GetTopUpOrderStatusFunc(string out_trade_no)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "订单状态查询失败" });
            try
            {
                string wxConfigError = ValidateWxPayConfig();
                if (!string.IsNullOrWhiteSpace(wxConfigError))
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = wxConfigError });
                }

                user_list user_list = GetUserLoginStatus();
                if (user_list == null || user_list.id <= 0)
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = -1, ["info"] = "请先登录！" });
                }

                out_trade_no = Function.HtmlEncode(out_trade_no ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(out_trade_no))
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "订单号不能为空" });
                }

                DataTable orderDt = integrateLog_listbll.GetDatatable("select top 1 user_id,pay_type,pay_status,payer_total from userpaylog_list where out_trade_no='" + out_trade_no + "'");
                if (orderDt == null || orderDt.Rows.Count <= 0 || Function.ConvertTo<int>(orderDt.Rows[0]["user_id"].ToString(), 0) != user_list.id)
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "未找到对应充值订单" });
                }

                int payType = Function.ConvertTo<int>(orderDt.Rows[0]["pay_type"].ToString(), 0);
                if (Function.ConvertTo<int>(orderDt.Rows[0]["pay_status"].ToString(), 0) == 1)
                {
                    decimal paid = Function.ConvertTo<decimal>(orderDt.Rows[0]["payer_total"].ToString(), 0);
                    int integrateAmount = GetRechargeIntegrateAmount(paid, GetWebsiteInfoConfig());
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = "充值已到账", ["integrate_amount"] = integrateAmount });
                }

                if (payType != 1)
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "暂不支持查询该支付方式" });
                }

                var helper = new WxPayHelper(WxPayConst.appid, WxPayConst.mchid, WxPayConst.serialNo, WxPayConst.privateKey);
                WxPayStatusRespModel payModel = helper.QueryOrder(out_trade_no).GetAwaiter().GetResult();
                if (payModel != null && "SUCCESS".Equals(payModel.trade_state, StringComparison.OrdinalIgnoreCase))
                {
                    decimal paid = 0;
                    if (payModel.amount != null)
                    {
                        paid = Function.ConvertTo<decimal>(payModel.amount.payer_total.ToString(), 0) / 100m;
                    }
                    else
                    {
                        paid = Function.ConvertTo<decimal>(orderDt.Rows[0]["payer_total"].ToString(), 0);
                    }

                    bool finalized = FinalizeTopUpOrder(
                        user_list.id,
                        out_trade_no,
                        payType,
                        paid,
                        payModel.transaction_id,
                        payModel.trade_type,
                        payModel.trade_state,
                        payModel.trade_state_desc,
                        payModel.bank_type,
                        payModel.success_time
                    );

                    if (finalized)
                    {
                        websiteinfo_list websiteinfo = GetWebsiteInfoConfig();
                        int integrateAmount = GetRechargeIntegrateAmount(paid, websiteinfo);
                        int giftAmount = GetRechargeGiftAmount(user_list.id, paid, websiteinfo, out_trade_no, true);
                        return serializer.Serialize(new Dictionary<string, object>
                        {
                            ["status"] = 1,
                            ["info"] = giftAmount > 0 ? "充值成功，积分已到账并赠送首充奖励" : "充值成功，积分已到账",
                            ["integrate_amount"] = integrateAmount,
                            ["gift_amount"] = giftAmount
                        });
                    }
                }

                string tradeState = payModel == null ? string.Empty : payModel.trade_state;
                if (string.IsNullOrWhiteSpace(tradeState) || "NOTPAY".Equals(tradeState, StringComparison.OrdinalIgnoreCase) || "USERPAYING".Equals(tradeState, StringComparison.OrdinalIgnoreCase))
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = 2, ["info"] = "订单待支付" });
                }

                return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = string.IsNullOrWhiteSpace(payModel.trade_state_desc) ? "支付未完成" : payModel.trade_state_desc });
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "GetTopUpOrderStatusFunc_Error:" + ex.Message + "-" + ex.StackTrace);
                str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "订单查询失败，请检查微信支付配置或服务器网络。" });
            }
            return str_;
        }

        private static string ValidateWxPayConfig()
        {
            if (string.IsNullOrWhiteSpace(WxPayConst.appid)
                || string.IsNullOrWhiteSpace(WxPayConst.mchid)
                || string.IsNullOrWhiteSpace(WxPayConst.serialNo)
                || string.IsNullOrWhiteSpace(WxPayConst.privateKey))
            {
                return "微信支付尚未配置完整，请先在 Web.config 中填写你的商户 AppID、商户号、证书序列号和私钥。";
            }
            return string.Empty;
        }

        private static websiteinfo_list GetWebsiteInfoConfig()
        {
            websiteinfo_list websiteinfo = websiteinfo_listbll.SelectSingle("id=1");
            if (websiteinfo == null)
            {
                websiteinfo = new websiteinfo_list();
                websiteinfo.money_integrate = 10;
                websiteinfo.integrate_donate = 0;
            }
            if (websiteinfo.money_integrate <= 0)
            {
                websiteinfo.money_integrate = 10;
            }
            return websiteinfo;
        }

        private static int GetRechargeIntegrateAmount(decimal payerTotal, websiteinfo_list websiteinfo)
        {
            int ratio = websiteinfo == null || websiteinfo.money_integrate <= 0 ? 10 : websiteinfo.money_integrate;
            return Convert.ToInt32(Math.Round(payerTotal * ratio, MidpointRounding.AwayFromZero));
        }

        private static int GetRechargeGiftAmount(int userId, decimal payerTotal, websiteinfo_list websiteinfo, string currentOrderNo = "", bool includeCurrentOrder = false)
        {
            if (websiteinfo == null || websiteinfo.integrate_donate <= 0)
            {
                return 0;
            }

            string sql = "select count(1) as num from userpaylog_list where user_id=" + userId + " and pay_status=1";
            if (!includeCurrentOrder && !string.IsNullOrWhiteSpace(currentOrderNo))
            {
                sql += " and out_trade_no<>'" + Function.HtmlEncode(currentOrderNo) + "'";
            }
            DataTable dt = integrateLog_listbll.GetDatatable(sql);
            int successCount = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                successCount = Function.ConvertTo<int>(dt.Rows[0]["num"].ToString(), 0);
            }
            if (successCount > 0)
            {
                return 0;
            }

            int baseIntegrate = GetRechargeIntegrateAmount(payerTotal, websiteinfo);
            return Convert.ToInt32(Math.Round(baseIntegrate * websiteinfo.integrate_donate / 100m, MidpointRounding.AwayFromZero));
        }

        private static void SavePendingTopUpOrder(int userId, string outTradeNo, int payType, decimal payerTotal)
        {
            string safeOrderNo = Function.HtmlEncode(outTradeNo);
            DataTable dt = integrateLog_listbll.GetDatatable("select top 1 out_trade_no from userpaylog_list where out_trade_no='" + safeOrderNo + "'");
            if (dt != null && dt.Rows.Count > 0)
            {
                integrateLog_listbll.Sql_D("update userpaylog_list set user_id=" + userId + ",pay_type=" + payType + ",payer_total=" + payerTotal.ToString("0.##") + ",add_time=getdate() where out_trade_no='" + safeOrderNo + "'");
                return;
            }

            integrateLog_listbll.Sql_D("insert into userpaylog_list(user_id,out_trade_no,add_time,pay_type,pay_status,payer_total) values(" + userId + ",'" + safeOrderNo + "',getdate()," + payType + ",0," + payerTotal.ToString("0.##") + ")");
        }

        private static bool FinalizeTopUpOrder(int userId, string outTradeNo, int payType, decimal payerTotal, string transactionId, string tradeType, string tradeState, string tradeStateDesc, string bankType, string successTime)
        {
            string safeOrderNo = Function.HtmlEncode(outTradeNo);
            DataTable orderDt = integrateLog_listbll.GetDatatable("select top 1 user_id,pay_status from userpaylog_list where out_trade_no='" + safeOrderNo + "'");
            if (orderDt == null || orderDt.Rows.Count <= 0)
            {
                return false;
            }

            if (Function.ConvertTo<int>(orderDt.Rows[0]["user_id"].ToString(), 0) != userId)
            {
                return false;
            }

            if (Function.ConvertTo<int>(orderDt.Rows[0]["pay_status"].ToString(), 0) == 1)
            {
                return true;
            }

            websiteinfo_list websiteinfo = GetWebsiteInfoConfig();
            int integrateAmount = GetRechargeIntegrateAmount(payerTotal, websiteinfo);
            int giftAmount = GetRechargeGiftAmount(userId, payerTotal, websiteinfo, safeOrderNo, false);

            StringBuilder sql = new StringBuilder();
            sql.Append("update userpaylog_list set pay_status=1,up_time='").Append(Function.HtmlEncode(successTime)).Append("',payer_total=").Append(payerTotal.ToString("0.##")).Append(" where out_trade_no='").Append(safeOrderNo).Append("' and pay_status=0;");
            sql.Append("if not exists(select 1 from userpayloginfo_list where out_trade_no='").Append(safeOrderNo).Append("' and transaction_id='").Append(Function.HtmlEncode(transactionId)).Append("') ");
            sql.Append("insert into userpayloginfo_list(appid,mchid,out_trade_no,transaction_id,trade_type,trade_state,trade_state_desc,bank_type,success_time,payer_total,pay_type,add_time,user_id) values(");
            sql.Append("N'").Append(Function.HtmlEncode(WxPayConst.appid)).Append("',");
            sql.Append("N'").Append(Function.HtmlEncode(WxPayConst.mchid)).Append("',");
            sql.Append("N'").Append(safeOrderNo).Append("',");
            sql.Append("N'").Append(Function.HtmlEncode(transactionId)).Append("',");
            sql.Append("N'").Append(Function.HtmlEncode(tradeType)).Append("',");
            sql.Append("N'").Append(Function.HtmlEncode(tradeState)).Append("',");
            sql.Append("N'").Append(Function.HtmlEncode(tradeStateDesc)).Append("',");
            sql.Append("N'").Append(Function.HtmlEncode(bankType)).Append("',");
            sql.Append("N'").Append(Function.HtmlEncode(successTime)).Append("',");
            sql.Append(payerTotal.ToString("0.##")).Append(",");
            sql.Append(payType).Append(",getdate(),").Append(userId).Append(");");
            sql.Append("if not exists(select 1 from integrateLog_list where user_id=").Append(userId).Append(" and type=11 and orderpro_orderno='").Append(safeOrderNo).Append("') ");
            sql.Append("insert into integrateLog_list(num_integrate,type,name,info_,addtime,user_id,orderpro_orderno) values(")
                .Append(integrateAmount).Append(",11,N'积分充值',N'微信支付充值 ")
                .Append(payerTotal.ToString("0.##")).Append(" 元，到账 ")
                .Append(integrateAmount).Append(" 积分。',getdate(),")
                .Append(userId).Append(",N'").Append(safeOrderNo).Append("');");

            if (giftAmount > 0)
            {
                sql.Append("if not exists(select 1 from integrateLog_list where user_id=").Append(userId).Append(" and type=12 and orderpro_orderno='").Append(safeOrderNo).Append("_gift') ");
                sql.Append("insert into integrateLog_list(num_integrate,type,name,info_,addtime,user_id,orderpro_orderno) values(")
                    .Append(giftAmount).Append(",12,N'充值赠送',N'首充赠送 ")
                    .Append(giftAmount).Append(" 积分。',getdate(),")
                    .Append(userId).Append(",N'").Append(safeOrderNo).Append("_gift');");
            }

            return integrateLog_listbll.Sql_D(sql.ToString());
        }
        public static string GetDelUserImgFunc(string del_imgurl)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                user_list user_list = GetUserLoginStatus();
                if (user_list != null && user_list.id > 0)
                {
                    if (!string.IsNullOrWhiteSpace(del_imgurl))
                    {
                        del_imgurl = del_imgurl.Replace("/A_UpLoad/upload_pic/", "");
                        userimg_list userimg_list = userimg_listbll.SelectSingle("upload_pic_img='" + del_imgurl + "' and userid=" + user_list.id);
                        if (userimg_list != null && userimg_list.userid > 0)
                        {
                            try
                            {
                                userimg_listbll.Delete("upload_pic_img='" + userimg_list.upload_pic_img + "' and userid=" + user_list.id);
                            }
                            catch (Exception ex)
                            {
                                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
                            }

                            try
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + userimg_list.upload_pic_img);
                            }
                            catch (Exception ex)
                            {
                                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
                            }
                            str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = "删除图片记录成功！" });
                        }
                        else
                        {
                            str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "图片记录异常！" });
                        }
                    }
                    else
                    {
                        str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "删除参数异常！" });
                    }
                }
                else
                {
                    str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = -1, ["info"] = "登录状态异常！" });
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
            return str_;
        }

        public static string GetLiteratureCommentAddFunc(int literatureId, string info)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                user_list user = GetUserLoginStatus();
                if (user == null || user.id <= 0)
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = -1, ["info"] = "请先登录！" });
                }

                if (literatureId <= 0 || string.IsNullOrWhiteSpace(info))
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "*为必填项！" });
                }

                if (info.Trim().Length > UploadPolicy.MaxCommentLength)
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "评论内容不能超过 " + UploadPolicy.MaxCommentLength + " 个字符！" });
                }

                if (!PdfParseConcurrencyGate.TryAcquireThrottle("comment:user:" + user.id, UploadPolicy.CommentCooldownSeconds))
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "提交过于频繁，请稍后再试！" });
                }

                Literature literature = ResolveCanonicalLiterature(literatureId);
                if (literature == null || literature.id <= 0)
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "文献参数异常！" });
                }

                DateTime now = DateTime.Now;
                string title = Function.HtmlDiscode(literature.title);
                string cleanInfo = info.Trim();
                string safeInfo = Function.HtmlEncode(cleanInfo);
                string pageUrl = "/LiteratureInfo.aspx?id=" + literature.id;

                LiteratureComment comment = new LiteratureComment();
                comment.literature_id = literature.id;
                comment.canonical_literature_id = literature.id;
                comment.userid = user.id;
                comment.parent_id = 0;
                comment.content = safeInfo;
                comment.status = 0;
                comment.like_count = 0;
                comment.report_count = 0;
                comment.is_deleted = 0;
                comment.delete_time = null;
                comment.reviewed_by = null;
                comment.review_time = null;
                comment.review_remark = null;
                comment.addtime = now;
                comment.updatetime = now;

                if (LiteratureCommentBll.Add(comment, "id") > 0)
                {
                    string body = "<!DOCTYPE html><html><head><meta charset='utf-8' /></head><body style='font-size:16px;line-height:1.8;'>";
                    body += "<h2>收到一条新的文献评论</h2>";
                    body += "<p><strong>评论者：</strong>" + Function.HtmlDiscode(user.name) + " / " + Function.HtmlDiscode(user.tel) + "</p>";
                    body += "<p><strong>文献标题：</strong>" + title + "</p>";
                    body += "<p><strong>文献链接：</strong>" + pageUrl + "</p>";
                    body += "<p><strong>评论内容：</strong><br/>" + Function.HtmlDiscode(safeInfo).Replace("\n", "<br/>") + "</p>";
                    body += "</body></html>";
                    CommonFunc.ToEmail("温馨提示：收到一条新的文献评论~", body, "");
                    AddLiteratureCommentNotice(literature, user);
                    str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = "评论已提交，我们会尽快审核处理。" });
                }
                else
                {
                    str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "提交失败，请稍后再试！" });
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "GetLiteratureCommentAddFunc:" + ex.Message + "-" + ex.StackTrace);
            }
            return str_;
        }

        public static string GetLiteratureCommentDeleteFunc(int literatureId, int commentId)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                user_list user = GetUserLoginStatus();
                if (user == null || user.id <= 0)
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = -1, ["info"] = "请先登录！" });
                }

                if (literatureId <= 0 || commentId <= 0)
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "评论参数异常！" });
                }

                int canonicalId = ResolveCanonicalLiteratureId(literatureId);
                if (canonicalId <= 0)
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "文献参数异常！" });
                }

                LiteratureComment comment = LiteratureCommentBll.SelectSingle(
                    "id=" + commentId + " and userid=" + user.id + " and is_deleted=0 and parent_id=0 and (canonical_literature_id=" + canonicalId + " or literature_id=" + canonicalId + ")");
                if (comment != null && comment.id > 0)
                {
                    bool ok = LiteratureCommentBll.Update(
                        "is_deleted=1,status=3,delete_time='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',updatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'",
                        "id=" + commentId + " and userid=" + user.id);
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = ok ? 1 : 0, ["info"] = ok ? "评论已删除" : "删除失败，请稍后再试。" });
                }

                string pageUrl = "/LiteratureInfo.aspx?id=" + canonicalId;
                string safeUrl = Function.HtmlEncode(pageUrl).Replace("'", "''");
                ServiceLog_List oldComment = ServiceLog_Listbll.SelectSingle("id=" + commentId + " and userid=" + user.id + " and status<>-1 and name like N'[[]文献评论]%' and info_ like N'%" + safeUrl + "%'");
                if (oldComment == null || oldComment.id <= 0)
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "只能删除您自己的评论。" });
                }

                bool oldOk = ServiceLog_Listbll.Update("status=-1,uptime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'", "id=" + commentId + " and userid=" + user.id);
                str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = oldOk ? 1 : 0, ["info"] = oldOk ? "评论已删除" : "删除失败，请稍后再试。" });
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "GetLiteratureCommentDeleteFunc:" + ex.Message + "-" + ex.StackTrace);
            }
            return str_;
        }

        public static string GetLiteratureReactionToggleFunc(int literatureId, string action)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                user_list user = GetUserLoginStatus();
                if (user == null || user.id <= 0)
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = -1, ["info"] = "请先登录！" });
                }

                action = (action ?? string.Empty).Trim().ToLowerInvariant();
                if (literatureId <= 0 || (action != "like" && action != "favorite"))
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "操作参数异常，请稍后再试！" });
                }

                Literature literature = ResolveCanonicalLiterature(literatureId);
                if (literature == null || literature.id <= 0)
                {
                    return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "文献参数异常！" });
                }
                literatureId = literature.id;

                bool isLike = action == "like";
                bool selected;
                if (isLike)
                {
                    LiteratureLike exists = LiteratureLikeBll.SelectSingle("literature_id=" + literatureId + " and userid=" + user.id);
                    if (exists != null && exists.id > 0)
                    {
                        LiteratureLikeBll.Delete("literature_id=" + literatureId + " and userid=" + user.id);
                        selected = false;
                    }
                    else
                    {
                        LiteratureLike item = new LiteratureLike();
                        item.literature_id = literatureId;
                        item.userid = user.id;
                        item.addtime = DateTime.Now;
                        LiteratureLikeBll.Add(item, "id");
                        selected = true;
                        AddLiteratureReactionNotice(literature, user, true);
                    }
                }
                else
                {
                    LiteratureFavorite exists = LiteratureFavoriteBll.SelectSingle("literature_id=" + literatureId + " and userid=" + user.id);
                    if (exists != null && exists.id > 0)
                    {
                        LiteratureFavoriteBll.Delete("literature_id=" + literatureId + " and userid=" + user.id);
                        selected = false;
                    }
                    else
                    {
                        LiteratureFavorite item = new LiteratureFavorite();
                        item.literature_id = literatureId;
                        item.userid = user.id;
                        item.addtime = DateTime.Now;
                        LiteratureFavoriteBll.Add(item, "id");
                        selected = true;
                        AddLiteratureReactionNotice(literature, user, false);
                    }
                }

                int likeCount = GetLiteratureReactionCount(true, literatureId);
                int favoriteCount = GetLiteratureReactionCount(false, literatureId);
                string info = selected ? (isLike ? "已点赞" : "已收藏") : (isLike ? "已取消点赞" : "已取消收藏");
                str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = info, ["selected"] = selected, ["like_count"] = likeCount, ["favorite_count"] = favoriteCount });
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "GetLiteratureReactionToggleFunc:" + ex.Message + "-" + ex.StackTrace);
            }
            return str_;
        }

        private static int ResolveCanonicalLiteratureId(int literatureId)
        {
            Literature literature = ResolveCanonicalLiterature(literatureId);
            return literature != null ? literature.id : 0;
        }

        private static Literature ResolveCanonicalLiterature(int literatureId)
        {
            if (literatureId <= 0)
            {
                return null;
            }

            Literature literature = LiteratureBll.SelectSingle("id=" + literatureId + " and status in(1,3)");
            if (literature == null || literature.id <= 0)
            {
                return null;
            }

            if (literature.canonical_literature_id.HasValue
                && literature.canonical_literature_id.Value > 0
                && literature.canonical_literature_id.Value != literature.id)
            {
                return LiteratureBll.SelectSingle("id=" + literature.canonical_literature_id.Value + " and status=1");
            }

            return literature.status == 1 ? literature : null;
        }

        private static int GetLiteratureReactionCount(bool isLike, int literatureId)
        {
            BLLBase<LiteratureLike> likeBll = LiteratureLikeBll;
            BLLBase<LiteratureFavorite> favoriteBll = LiteratureFavoriteBll;
            string table = isLike ? "LiteratureLike" : "LiteratureFavorite";
            DataTable dt = isLike
                ? likeBll.GetDatatable("select count(1) as num from " + table + " where literature_id=" + literatureId)
                : favoriteBll.GetDatatable("select count(1) as num from " + table + " where literature_id=" + literatureId);
            int count = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                count = Function.ConvertTo<int>(Convert.ToString(dt.Rows[0]["num"]), 0);
            }
            if (dt != null)
            {
                dt.Dispose();
            }
            return count;
        }

        private static void AddLiteratureReactionNotice(Literature literature, user_list actor, bool isLike)
        {
            if (literature == null || actor == null || literature.userid <= 0 || literature.userid == actor.id)
            {
                return;
            }

            string title = Function.HtmlDiscode(literature.title);
            string actorName = string.IsNullOrWhiteSpace(actor.name) ? actor.tel : actor.name;
            string actionText = isLike ? "点赞" : "收藏";
            NoticeLog_List notice = new NoticeLog_List();
            notice.userid = literature.userid;
            notice.type = 1;
            notice.status = 0;
            notice.addtime = DateTime.Now;
            notice.url = "/LiteratureInfo.aspx?id=" + literature.id;
            notice.name = Function.HtmlEncode("[赞&收藏] 文献被" + actionText);
            notice.info_ = Function.HtmlEncode(Function.HtmlDiscode(actorName) + actionText + "了您的文献《" + title + "》。");
            NoticeLog_Listbll.Add(notice, "id");
        }

        private static void AddLiteratureCommentNotice(Literature literature, user_list actor)
        {
            if (literature == null || actor == null || literature.userid <= 0 || literature.userid == actor.id)
            {
                return;
            }

            string title = Function.HtmlDiscode(literature.title);
            string actorName = string.IsNullOrWhiteSpace(actor.name) ? actor.tel : actor.name;
            NoticeLog_List notice = new NoticeLog_List();
            notice.userid = literature.userid;
            notice.type = 1;
            notice.status = 0;
            notice.addtime = DateTime.Now;
            notice.url = "/LiteratureInfo.aspx?id=" + literature.id;
            notice.name = Function.HtmlEncode("[文献评论] 文献收到新评论");
            notice.info_ = Function.HtmlEncode(Function.HtmlDiscode(actorName) + "评论了您的文献《" + title + "》，评论通过后台审核后会公开展示。");
            NoticeLog_Listbll.Add(notice, "id");
        }
        public static string GetAppealAddFunc(string url, string info, string[] ImgArr)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                user_list user_list = GetUserLoginStatus();
                if (user_list != null && user_list.id > 0)
                {
                    if (!string.IsNullOrWhiteSpace(url) && !string.IsNullOrWhiteSpace(info))
                    {
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("insert into appeal_list(");
                        strSql.Append("url, info_, addtime, status, userid)");
                        strSql.Append(" values (");
                        strSql.Append(" @url, @info_, @addtime, @status, @userid)");
                        strSql.Append(";select @@IDENTITY");
                        SqlParameter[] parameters = {
                        new SqlParameter("@url", SqlDbType.NVarChar,2500),
                          new SqlParameter("@info_",SqlDbType.NVarChar,-1),
                          new SqlParameter("@addtime",SqlDbType.DateTime),
                          new SqlParameter("@status",SqlDbType.Int),
                          new SqlParameter("@userid",SqlDbType.Int)
                                    };

                        parameters[0].Value = Function.HtmlEncode(url);
                        parameters[1].Value = Function.HtmlEncode(info);
                        DateTime DateTimeNow = DateTime.Now;
                        parameters[2].Value = DateTimeNow;
                        parameters[3].Value = 0;
                        parameters[4].Value = user_list.id;

                        string sql = string.Empty;
                        string del_img = string.Empty;
                        if (ImgArr != null && ImgArr.Length > 0)
                        {
                            int index_ = 1;
                            foreach (string item in ImgArr)
                            {
                                string item_ = item.Replace("/A_UpLoad/upload_pic/", "");
                                if (CommonFunc.GetImgBool(item_))
                                {
                                    sql += "(LiteratureManagerteshu, '" + item_ + "', GETDATE(), " + index_ + "),";
                                    del_img += "ξLiteratureManagerξDELETE FROM userimg_list WHERE upload_pic_img='" + item_ + "' and userid=" + user_list.id;
                                    index_++;
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(sql))
                        {
                            sql = "INSERT INTO appealimg_list (appeal_id, upload_pic_info, addtime, orderid) VALUES " + sql.Substring(0, sql.Length - 1) + del_img;
                        }

                        int addid = appeal_listbll.Add_R_Id_(parameters, strSql, sql);
                        if (addid > 0)
                        {
                            str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = "提交成功！" });
                            try
                            {
                                string Body = "<!DOCTYPE html>";
                                Body += "<html>";
                                Body += "<head>";
                                Body += "<meta charset='utf-8' />";
                                Body += "<title></title>";
                                Body += "</head>";
                                Body += "<body style='font-size: 18px;' id='body'>";
                                Body += " <div id='wrap' style=' width: 750px; margin: 0 auto;'>";

                                Body += "<article >";


                                Body += "<div  style=' padding: 30px 30px 10px 30px;'>";

                                //Body += "<p style='color: #888; font-size: 26px; line-height: 1.4; height: 32px; font-weight: lighter;'>" + info_str + "</p>";


                                Body += "<div  style=' display: flex; justify-content: flex-start;align-items: flex-start; border-bottom: 1px #eee solid; padding-bottom: 30px; margin-bottom: 30px;'>";
                                Body += " <div style=' width: 150px; text-align: justify; color: #888; font-size: 26px; line-height: 1.4; height: 32px;  overflow: hidden; font-weight: lighter;'>申述者：</div>";
                                Body += " <div  style='width: calc(100% - 150px); color: #222;  font-size: 26px; line-height: 1.4; display: flex; justify-content: flex-start; align-items: flex-start;'>" + Function.HtmlDiscode(user_list.name) + " / " + Function.HtmlDiscode(user_list.tel) + "</div> ";
                                Body += " </div>";


                                Body += "<div  style=' display: flex; justify-content: flex-start;align-items: flex-start; border-bottom: 1px #eee solid; padding-bottom: 30px; margin-bottom: 30px;'>";
                                Body += " <div style=' width: 150px; text-align: justify; color: #888; font-size: 26px; line-height: 1.4; height: 32px;  overflow: hidden; font-weight: lighter;'>申述日期：</div>";
                                Body += " <div  style='width: calc(100% - 150px); color: #222;  font-size: 26px; line-height: 1.4; display: flex; justify-content: flex-start; align-items: flex-start;'>" + DateTimeNow.ToString("yyyy-MM-dd HH:mm:ss") + "</div> ";
                                Body += " </div>";


                                Body += "<div  style=' display: flex; justify-content: flex-start;align-items: flex-start; border-bottom: 1px #eee solid; padding-bottom: 30px; margin-bottom: 30px;'>";
                                Body += " <div style=' width: 150px; text-align: justify; color: #888; font-size: 26px; line-height: 1.4; height: 32px;  overflow: hidden; font-weight: lighter;'>原文献链接：</div>";
                                Body += " <div  style='width: calc(100% - 150px); color: #222;  font-size: 26px; line-height: 1.4; display: flex; justify-content: flex-start; align-items: flex-start;'>" + Function.HtmlDiscode(url) + "</div> ";
                                Body += " </div>";


                                Body += "<div  style=' display: flex; justify-content: flex-start;align-items: flex-start; border-bottom: 1px #eee solid; padding-bottom: 30px; margin-bottom: 30px;'>";
                                Body += " <div style=' width: 150px; text-align: justify; color: #888; font-size: 26px; line-height: 1.4; height: 32px;  overflow: hidden; font-weight: lighter;'>问题描述：</div>";
                                Body += " <div  style='width: calc(100% - 150px); color: #222;  font-size: 26px; line-height: 1.4; display: flex; justify-content: flex-start; align-items: flex-start;'>" + Function.HtmlDiscode(info) + "</div> ";
                                Body += " </div>";






                                Body += " </div> ";

                                Body += "</article> ";
                                Body += "</div> ";
                                Body += "</body> ";
                                Body += "</html>";

                                CommonFunc.ToEmail("新版权申述来啦，烦请尽快处理哟~", Body, "");
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                        else
                        {
                            str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "提交失败，请稍后再试！" });
                        }
                    }
                    else
                    {
                        str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "*为必填项！" });
                    }
                }
                else
                {
                    str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = -1, ["info"] = "请先登录！" });
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
            return str_;
        }

        public static int GetUserIntegrateSumFunc(int user_id, int type)
        {
            int num_integrate = 0;
            DataTable integrateLog_list_dt = integrateLog_listbll.GetDatatable("select sum(num_integrate) as num from integrateLog_list where user_id=" + user_id + (type == 1 ? " and num_integrate>0" : (type == -1 ? " and num_integrate<0" : "")));
            if (integrateLog_list_dt != null && integrateLog_list_dt.Rows.Count > 0)
            {
                num_integrate = Function.ConvertTo<int>(integrateLog_list_dt.Rows[0]["num"].ToString(), 0);
            }
            integrateLog_list_dt.Dispose();
            return num_integrate;
        }

        public static string GetIntegrateExchangeAddFunc(int integrate_id, int num_integrate, int user_id)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                user_list user_list = GetUserLoginStatus();
                if (user_list != null && user_list.id > 0 && user_list.id == user_id)
                {
                    integrate_list integrate_list = integrate_listbll.SelectSingle("id=" + integrate_id + " and num_integrate=" + num_integrate);
                    if (integrate_list != null && integrate_list.id > 0)
                    {
                        if (integrate_list.num_integrate > 0)
                        {
                            string codestr = Function.HtmlEncode(Guid.NewGuid().ToString().Replace("-", ""));
                            string safeName = (integrate_list.name ?? string.Empty).Replace("'", "''");
                            string safeImage = (integrate_list.upload_pic_img ?? string.Empty).Replace("'", "''");
                            string sql = "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; "
                                + "IF ((SELECT ISNULL(SUM(num_integrate),0) FROM integrateLog_list WITH (UPDLOCK,HOLDLOCK) WHERE user_id=" + user_list.id + ") >= " + integrate_list.num_integrate + ") BEGIN "
                                + "INSERT INTO integrateExchangeLog_list (name, num_integrate, codestr, addtime, status, user_id,upload_pic_img) VALUES (N'" + safeName + "'," + integrate_list.num_integrate + ", '" + codestr + "', GETDATE(), 1, " + user_list.id + ",N'" + safeImage + "');"
                                + "INSERT INTO integrateLog_list (num_integrate, type, name, info_, addtime, user_id) VALUES (-" + integrate_list.num_integrate + ", 6,N'兑换文献下载权益成功', N'您消耗了" + integrate_list.num_integrate + "积分兑换了文献下载权益《" + safeName + "》', GETDATE(), " + user_list.id + ");"
                                + "INSERT INTO NoticeLog_List (name, type, addtime, userid, status, url,info_) VALUES (N'文献下载权益兑换成功，点击查看详情。', 6, GETDATE(), " + user_list.id + ", 0, '/User/IntegrateExchangeLog',N'您消耗了" + integrate_list.num_integrate + "积分，成功兑换了文献下载权益《" + safeName + "》，请在权益记录中查看。');"
                                + " END ELSE BEGIN RAISERROR('insufficient points',16,1); END";

                            if (integrateExchangeLog_listbll.Sql_D(sql))
                            {
                                str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = "兑换成功！" });
                            }
                            else
                            {
                                str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "积分不足或请求冲突，请刷新后重试！" });
                            }
                        }
                        else
                        {
                            str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "积分不足！" });
                        }
                    }
                    else
                    {
                        str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "下载权益异常！" });
                    }
                }
                else
                {
                    str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = -1, ["info"] = "请先登录！" });
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
            return str_;
        }

        public static string GetSmsCodeFunc(string tel, int type, int cookie_x, int cookie_y)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                if (!string.IsNullOrWhiteSpace(tel))
                {
                    if (!Regex.IsMatch(tel.Trim(), @"^1\d{10}$"))
                    {
                        return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "请输入正确的手机号码" });
                    }

                    if (type == 1 || type == 2)
                    {
                        telcode_list previousCode = telcode_listbll.SelectSingle("tel='" + Function.HtmlEncode(tel.Trim()) + "' and type=" + type);
                        if (previousCode != null && previousCode.addtime.AddSeconds(UploadPolicy.SmsCooldownSeconds) > DateTime.Now)
                        {
                            return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "验证码发送过于频繁，请稍后再试！" });
                        }

                        string requestIp = Function.GetClientIP() ?? string.Empty;
                        if (!PdfParseConcurrencyGate.TryAcquireThrottle("sms:phone:" + tel.Trim(), UploadPolicy.SmsCooldownSeconds)
                            || !PdfParseConcurrencyGate.TryAcquireThrottle("sms:ip:" + requestIp, UploadPolicy.SmsCooldownSeconds))
                        {
                            return serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "验证码发送过于频繁，请稍后再试！" });
                        }

                        Random rad = new Random();
                        int mobile_code = rad.Next(100000, 1000000);
                        bool isDebugMode = "true".Equals(ConfigurationManager.AppSettings["SmsDebugMode"], StringComparison.OrdinalIgnoreCase);
                        if (isDebugMode)
                        {
                            bool iscode = SaveTelCode(tel, type, cookie_x, cookie_y, mobile_code.ToString());
                            if (iscode)
                            {
                                str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = "当前为本地调试模式，验证码：" + mobile_code });
                            }
                            else
                            {
                                str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "本地验证码保存失败，请重新发送" });
                            }
                            return str;
                        }

                        string account = ConfigurationManager.AppSettings["SmsAccount"];
                        string password = ConfigurationManager.AppSettings["SmsPassword"];
                        string content = "您的验证码是：" + mobile_code + "，5分钟有效期。请勿泄露给其他人。";
                        string postStrTpl = "account={0}&password={1}&mobile={2}&content={3}";
                        UTF8Encoding encoding = new UTF8Encoding();
                        byte[] postData = encoding.GetBytes(string.Format(postStrTpl, account, password, tel, content));
                        string PostUrl = ConfigurationManager.AppSettings["SmsPostUrl"];
                        HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(PostUrl);
                        string smsProxyUrl = ConfigurationManager.AppSettings["SmsProxyUrl"];
                        if (!string.IsNullOrWhiteSpace(smsProxyUrl))
                        {
                            Uri proxyUri;
                            if (!Uri.TryCreate(smsProxyUrl, UriKind.Absolute, out proxyUri)
                                || (proxyUri.Scheme != Uri.UriSchemeHttp && proxyUri.Scheme != Uri.UriSchemeHttps))
                            {
                                throw new ConfigurationErrorsException("SmsProxyUrl配置无效");
                            }

                            myRequest.Proxy = new WebProxy(proxyUri);
                        }
                        myRequest.Method = "POST";
                        myRequest.ContentType = "application/x-www-form-urlencoded";
                        myRequest.ContentLength = postData.Length;
                        myRequest.Timeout = UploadPolicy.ExternalRequestTimeoutMs;
                        myRequest.ReadWriteTimeout = UploadPolicy.ExternalRequestTimeoutMs;

                        Stream newStream = myRequest.GetRequestStream();
                        // Send the data.
                        newStream.Write(postData, 0, postData.Length);
                        newStream.Flush();
                        newStream.Close();

                        HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                        if (myResponse.StatusCode == HttpStatusCode.OK)
                        {
                            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);

                            //Response.Write(reader.ReadToEnd());

                            string res = reader.ReadToEnd();
                            int len1 = res.IndexOf("</code>");
                            int len2 = res.IndexOf("<code>");
                            string code = res.Substring((len2 + 6), (len1 - len2 - 6));
                            //Response.Write(code);
                            int len3 = res.IndexOf("</msg>");
                            int len4 = res.IndexOf("<msg>");
                            string msg = res.Substring((len4 + 5), (len3 - len4 - 5));

                            if (code == "2")
                            {
                                bool iscode = SaveTelCode(tel, type, cookie_x, cookie_y, mobile_code.ToString());
                                if (iscode)
                                {
                                    str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = "发送短信成功" });
                                }
                                else
                                {
                                    str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "获取验证码短信发送失败，请重新发送" });
                                }
                            }
                            else
                            {
                                str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = msg });
                            }
                        }
                        else
                        {
                            str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "短信验证码请求异常" });
                        }
                    }
                    else
                    {
                        str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "请求短信验证码异常" });
                    }
                }
                else
                {
                    str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "请输入手机号码" });
                }
            }
            catch (Exception ex)
            {
                str = serializer.Serialize(new Dictionary<string, object>
                {
                    ["status"] = 0,
                    ["info"] = ex.Message
                });
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
            return str;
        }

        private static bool SaveTelCode(string tel, int type, int cookie_x, int cookie_y, string mobileCode)
        {
            bool iscode = false;
            telcode_list telcode_list = telcode_listbll.SelectSingle("tel='" + Function.HtmlEncode(tel) + "' and type=" + type);
            if (telcode_list != null)
            {
                telcode_list.addtime = DateTime.Now;
                telcode_list.code = mobileCode;
                telcode_list.tel = Function.HtmlEncode(tel);
                telcode_list.type = type;
                telcode_list.img_x = cookie_x;
                telcode_list.img_y = cookie_y;
                string[] up_ = { "type" };
                if (telcode_listbll.Update(up_, telcode_list))
                {
                    iscode = true;
                }
            }
            else
            {
                telcode_list = new telcode_list();
                telcode_list.addtime = DateTime.Now;
                telcode_list.code = mobileCode;
                telcode_list.tel = Function.HtmlEncode(tel);
                telcode_list.type = type;
                telcode_list.img_x = cookie_x;
                telcode_list.img_y = cookie_y;
                if (telcode_listbll.Add(telcode_list))
                {
                    iscode = true;
                }
            }
            return iscode;
        }

        public static string GetAddCodeFunc(string tel, string img_x, string img_y, string type)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                if (string.IsNullOrWhiteSpace(tel))
                {
                    str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "请输入手机号码" });
                }
                else
                {
                    int cookie_x = Function.ConvertTo<int>(img_x, 0);
                    int cookie_y = Function.ConvertTo<int>(img_y, 0);
                    if (cookie_x > 0 && cookie_y > 0)
                    {
                        str = GetSmsCodeFunc(tel, Function.ConvertTo<int>(type, 0), cookie_x, cookie_y);
                    }
                    else
                    {
                        str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "发送短信验证参数异常，请刷新页面再试一次！" });
                    }
                }
            }
            catch (Exception ex)
            {
                str = serializer.Serialize(new Dictionary<string, object>
                {
                    ["status"] = 0,
                    ["info"] = ex.Message
                });
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
            return str;
        }
        public static string GetUserInfoHtml(string user_id)
        {
            string R_str = string.Empty;
            user_list user_list = user_listbll.SelectSingle("id=" + Function.ConvertTo<int>(user_id, 0));
            if (user_list != null && user_list.id > -1)
            {
                R_str = "<img src=\"" + GetUserAvatarFunc(user_list.upload_pic_avatar) + "\" height=\"20\" style=\"border: 1px solid #cccccc\" class=\"tooltip_img\">" + Function.HtmlDiscode(user_list.name) + " / " + Function.HtmlDiscode(user_list.tel);
            }
            return R_str;
        }
        public static string GetUserAvatarFunc(string upload_pic_avatar)
        {
            string avatar_img = "/images/touxiang1.png";
            if (CommonFunc.GetImgBool(upload_pic_avatar))
            {
                avatar_img = A_UpLoad_Url + upload_pic_avatar;
            }
            return avatar_img;
        }
        public static user_list GetUserLoginStatus()
        {
            //未登录
            user_list user_list_model = new user_list();
            user_list user_list = user_listbll.SelectSingle("id", Function.ConvertTo<int>(Cookie.GetCookie("user_id"), 0));
            if (user_list != null && user_list.id > 0 && string.Equals(user_list.tel, Cookie.GetCookie("user_tel"), StringComparison.Ordinal))
            {
                if (!string.IsNullOrWhiteSpace(user_list.code) && Function.MD5Encrypt(user_list.code, MD5Key).Equals(Cookie.GetCookie("user_code")))
                {
                    if (user_list.isshow == 1)
                    {
                        try
                        {
                            TimeSpan ts = DateTime.Now - Function.ConvertTo<DateTime>(user_list.logintime, DateTime.MinValue);
                            if (ts.TotalMinutes >= 0 && ts.TotalMinutes < 60)
                            {
                                user_list_model = user_list;
                                user_listbll.Update("logintime=getdate(),loginip='" + Function.HtmlEncode(Function.GetClientIP()) + "'", "id=" + user_list.id);
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
            }
            if (!(user_list_model != null && user_list_model.id > 0))
            {
                Cookie.ClearCookie("user_id");
                Cookie.ClearCookie("user_tel");
                Cookie.ClearCookie("user_code");
            }
            return user_list_model;
        }
    }
}

