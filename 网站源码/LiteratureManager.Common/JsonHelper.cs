using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace LiteratureManager.Common
{
    public static class JsonHelper
    {
        //将json字符串转化成实体类
        public static T FromJsonTo<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {


                T jsonObject = (T)ser.ReadObject(ms);
                return jsonObject;
            }
        }
        /// <summary>
        /// 转化为json字符串，默认的时间格式
        /// </summary>
        /// <param name="obj">要被转化的对象</param>
        /// <returns>json字符串</returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateFormatString = "yyyy-MM-dd HH:mm:ss" });
        }
        /// <summary>
        /// json字符串转化为相应的类型
        /// </summary>
        /// <typeparam name="T">转化后的类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns>转化后的类型</returns>
        public static T ToObject<T>(this string json)
        {
            return json == null ? default(T) : JsonConvert.DeserializeObject<T>(json);
        }
    }
}
