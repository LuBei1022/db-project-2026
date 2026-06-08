using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteratureManager.Common
{
    public class WxPayRespModel
    {
        /// <summary>
        /// 预支付id
        /// </summary>
        public string prepay_id { set; get; }

        /// <summary>
        /// 如果prepay_id为null的话，这个就会有错误的编码
        /// 参考地址：https://pay.weixin.qq.com/wiki/doc/apiv3/Share/error_code.shtml
        /// </summary>
        public string code { set; get; }

        /// <summary>
        /// 如果prepay_id为null的话，这个就会有具体的错误信息和原因
        /// </summary>
        public string message { set; get; }

        /// <summary>
        /// 是否下单成功
        /// </summary>
        public bool success_union_order
        {
            get
            {
                if (string.IsNullOrEmpty(prepay_id) && !string.IsNullOrEmpty(code))
                {
                    return false;
                }
                return true;
            }
        }
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
        /// 返回成功的二维码链接
        /// </summary>
        public string code_url { get; set; }
    }
}
