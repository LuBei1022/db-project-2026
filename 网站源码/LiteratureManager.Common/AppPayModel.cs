using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteratureManager.Common
{
    public class AppPayModel
    {
        /// <summary>
        /// 预支付id
        /// </summary>
        public string prepay_id { set; get; }

        /// <summary>
        /// 签名
        /// </summary>
        public string signature { set; get; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp { set; get; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        public string noncestr { set; get; }
    }
    public class NativePayModel
    {
        /// <summary>
        /// 预支付id
        /// </summary>
        public string prepay_id { set; get; }

        /// <summary>
        /// 签名
        /// </summary>
        public string signature { set; get; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp { set; get; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        public string noncestr { set; get; }
    }
}
