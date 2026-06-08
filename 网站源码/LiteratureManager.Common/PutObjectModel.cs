
using COSXML;
using COSXML.Auth;
using COSXML.Common;
using COSXML.CosException;
using COSXML.Model.Object;
using COSXML.Model.Tag;
using System;
using System.Configuration;
namespace LiteratureManager.Common
{
    public class PutObjectModel
    {
        public static string bucket => GetSetting("qcloud_bucket");
        public static string region => GetSetting("qcloud_region");
        public static string secretId => GetSetting("qcloud_secret_id");
        public static string secretKey => GetSetting("qcloud_secret_key");

        private static string GetSetting(string key)
        {
            return (ConfigurationManager.AppSettings[key] ?? string.Empty).Trim();
        }

        private static bool HasBaseConfig
        {
            get
            {
                return !string.IsNullOrWhiteSpace(bucket)
                    && !string.IsNullOrWhiteSpace(region)
                    && !string.IsNullOrWhiteSpace(secretId)
                    && !string.IsNullOrWhiteSpace(secretKey);
            }
        }

        /// 判断对象是否存在
        public static string DoesObjectExist(string key)
        {
            string isok = "";
            if (!HasBaseConfig)
            {
                return isok;
            }
            CosXmlConfig config = new CosXmlConfig.Builder().SetRegion(region).Build();

            long durationSecond = 600;          //每次请求签名有效时长，单位为秒
            QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(secretId,
              secretKey, durationSecond);

            CosXml cosXml = new CosXmlServer(config, qCloudCredentialProvider);
            try
            {
                DoesObjectExistRequest request = new DoesObjectExistRequest(bucket, key);
                //执行请求
                if (cosXml.DoesObjectExist(request))
                {
                    isok = "ok";
                }
            }
            catch (COSXML.CosException.CosClientException clientEx)
            {
                isok = clientEx.Message;
            }
            catch (COSXML.CosException.CosServerException serverEx)
            {
                isok = serverEx.GetInfo();
            }
            return isok;
        }

        /// 删除对象
        public static void DeleteObject(string key)
        {
            if (!HasBaseConfig)
            {
                return;
            }
            string isok = "";
            CosXmlConfig config = new CosXmlConfig.Builder().SetRegion(region).Build();

            long durationSecond = 600;          //每次请求签名有效时长，单位为秒
            QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(secretId,
              secretKey, durationSecond);

            CosXml cosXml = new CosXmlServer(config, qCloudCredentialProvider);
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest(bucket, key);
                //执行请求
                DeleteObjectResult result = cosXml.DeleteObject(request);
                //请求成功
                if (result.GetResultInfo().Contains("200 OK"))
                {
                    isok = "ok";
                }
            }
            catch (COSXML.CosException.CosClientException clientEx)
            {
                isok = clientEx.Message;
            }
            catch (COSXML.CosException.CosServerException serverEx)
            {
                isok = serverEx.GetInfo();
            }
        }
    }
}

