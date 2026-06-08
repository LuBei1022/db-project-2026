using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LiteratureManager.Common
{
    public class WeChatPaySignature
    {
        private static string apiKey = "your_api_key"; // 替换为你的APIv3密钥

        public static string GenerateSignature(string method, string url, Dictionary<string, string> queryParameters, string timestamp, string nonce)
        {
            // 构造待签名数据
            var signData = new Dictionary<string, string>
        {
            { "method", method },
            { "url", url },
            { "timestamp", timestamp },
            { "nonce", nonce },
            // 添加其他必要的参数...
        };

            // 合并queryParameters到signData中
            foreach (var kvp in queryParameters)
            {
                signData[kvp.Key] = kvp.Value;
            }

            // 对参数按照key=value的格式，并按照参数名ASCII码从小到大排序（字典序）
            var sortedParams = signData.OrderBy(kvp => kvp.Key).ToList();
            var stringA = string.Join("&", sortedParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var stringSignTemp = $"{method}\n{url}\n{timestamp}\n{nonce}\n{stringA}\n";

            // HMAC-SHA256签名
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(apiKey)))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(stringSignTemp));
                return BitConverter.ToString(hashmessage).Replace("-", "").ToLower(); // 转换为16进制字符串并返回
            }
        }
    }
}
