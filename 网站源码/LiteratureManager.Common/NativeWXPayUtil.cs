using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static NPOI.HSSF.Util.HSSFColor;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Runtime;

namespace LiteratureManager.Common
{
    public class NativeWXPayUtil
    {
        public readonly static string appid = "wx499d14b1bce1bb81"// 这里填写你自己的appid
        // 商户号
        public readonly static string mch_id = "1277407001";// 这里填写你自己的商户号
        // 商户秘钥
        public readonly static string partnerkey = “”// 这里填写你自己的商户秘钥
        // 回调地址
        public readonly static string notifyurl = "https://your-domain.example.com/wx_pay_notify.aspx";// 待替换为微信支付商家平台回调地址
        // Native下单接口
        public readonly static string url = "https://api.mch.weixin.qq.com/v3/pay/transactions/native";

        #region  Native请求的需要的参数
        /// <summary>
        ///Native支付需要的类
        /// </summary>
        public class bodyModel
        {
            /// <summary>
            /// 商户号
            /// </summary>
            public string mchid { get; set; }
            /// <summary>
            /// 公众账号ID
            /// </summary>
            public string appid { get; set; }
            /// <summary>
            /// 商品描述
            /// </summary>
            public string description { get; set; }
            /// <summary>
            /// 商户订单号
            /// </summary>
            public string out_trade_no { get; set; }
            /// <summary>
            /// 支付结束时间
            /// </summary>
            public string time_expire { get; set; }
            /// <summary>
            /// 商户数据包
            /// </summary>
            public string attach { get; set; }
            /// <summary>
            /// 商户回调地址
            /// </summary>
            public string notify_url { get; set; }
            /// <summary>
            /// 订单金额
            /// </summary>
            public object amount { get; set; }
            /// <summary>
            /// 景信息
            /// </summary>
            public object scene_info { get; set; }
        }
        public class Wxamount
        {
            /// <summary>
            /// 总金额 单位为分
            /// </summary>
            public int total { get; set; }
            /// <summary>
            /// 货币类型 固定传：CNY，代表人民币
            /// </summary>
            public string currency { get; set; }

        }

      
        #endregion

        #region 请求返回的参数

       

        #endregion

        

        /// <summary>
        /// 构造签名串
        /// </summary>
        /// <param name="method">HTTP请求方式（全大写）</param>
        /// <param name="body">API接口请求参数的json字符串</param>
        /// <param name="uri">API接口的相对路径</param>
        /// <returns></returns>
        protected string BuildAuthAsync(string method, string body, string uri, string _mchID, string _serialNo, string _apiCertPath, string _certPwd)
        {
            var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            string nonce = Path.GetRandomFileName();

            string message = $"{method}\n{uri}\n{timestamp}\n{nonce}\n{body}\n";
            string signature = RequestSign(message, _apiCertPath, _certPwd);
            return $"mchid=\"{_mchID}\",nonce_str=\"{nonce}\",timestamp=\"{timestamp}\",serial_no=\"{_serialNo}\",signature=\"{signature}\"";
        }

        /// <summary>
        /// 生成签名
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected string RequestSign(string message, string _apiCertPath, string _certPwd)
        {
            //加载证书 _apiCertPath API证书物理路径 _certPwd API证书密码（默认是商户号）
            X509Certificate2 cer = new X509Certificate2(_apiCertPath, _certPwd, X509KeyStorageFlags.Exportable);
            if (cer != null)
            {
                RSA rsa = cer.GetRSAPrivateKey();  //获取私钥
                //查看在不同平台上的具体类型
                byte[] data = Encoding.UTF8.GetBytes(message);
                return Convert.ToBase64String(rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
            }
            else
            {
                return "";
            }
        }

        /// 生成订单号
        /// </summary>
        /// <returns></returns>
        private static string getRandomTime()
        {
            Random rd = new Random();//用于生成随机数
            string DateStr = DateTime.Now.ToString("yyyyMMddHHmmssMM");//日期
            string str = DateStr + rd.Next(10000).ToString().PadLeft(4, '0');//带日期的随机数
            return str;
        }
    }
}
