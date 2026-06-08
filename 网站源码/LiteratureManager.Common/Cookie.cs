using System;
using System.Web;

namespace LiteratureManager.Common
{
    /// <summary> 
    /// Cookie操作类 
    /// </summary> 
    public class Cookie
    {
        /// <summary> 
        /// 保存一个Cookie 
        /// </summary> 
        /// <param name="CookieName">Cookie名称</param> 
        /// <param name="CookieValue">Cookie值</param> 
        /// <param name="CookieTime">Cookie过期时间(小时),0为关闭页面失效</param> 
        public static void SaveCookie(string CookieName, string CookieValue, double CookieTime)
        {
            //设定cookie 域名.
            //string domain = string.Empty;
            //if (HttpContext.Current.Request.Params["HTTP_HOST"] != null)
            //{
            //    domain = Function.MD5(HttpContext.Current.Request.Params["HTTP_HOST"].ToString(), 16);
            //}

            HttpCookie myCookie = new HttpCookie(CookieName);
            DateTime now = DateTime.Now;
            myCookie.Value = HttpUtility.UrlEncode(CookieValue);
            SetSecurityFlags(myCookie);

            if (CookieTime != 0)
            {
                //有两种方法，第一方法设置Cookie时间的话，关闭浏览器不会自动清除Cookie 
                //第二方法不设置Cookie时间的话，关闭浏览器会自动清除Cookie ,但是有效期 
                //多久还未得到证实。 
                myCookie.Expires = now.AddDays(CookieTime);
                if (HttpContext.Current.Response.Cookies[CookieName] != null)
                    HttpContext.Current.Response.Cookies.Remove(CookieName);
                HttpContext.Current.Response.Cookies.Add(myCookie);
            }
            else
            {
                if (HttpContext.Current.Response.Cookies[CookieName] != null)
                    HttpContext.Current.Response.Cookies.Remove(CookieName);
                HttpContext.Current.Response.Cookies.Add(myCookie);
            }
        }
        /// <summary> 
        /// 取得CookieValue 
        /// </summary> 
        /// <param name="CookieName">Cookie名称</param> 
        /// <returns>Cookie的值</returns> 
        public static string GetCookie(string CookieName)
        {

            //设定cookie 域名.
            string domain = string.Empty;
            if (HttpContext.Current.Request.Params["HTTP_HOST"] != null)
            {
                domain = Function.MD5(HttpContext.Current.Request.Params["HTTP_HOST"].ToString(), 16);
            }


            HttpCookie myCookie = new HttpCookie(CookieName);
            myCookie = HttpContext.Current.Request.Cookies[CookieName];

            if (myCookie != null)
            {
                return HttpUtility.UrlDecode(myCookie.Value);
            }
            else
            {
                return "";
            }
        }
        /// <summary> 
        /// 清除CookieValue 
        /// </summary> 
        /// <param name="CookieName">Cookie名称</param> 
        public static void ClearCookie(string CookieName)
        {
            HttpCookie myCookie = new HttpCookie(CookieName);
            DateTime now = DateTime.Now;

            myCookie.Expires = now.AddYears(-2);
            SetSecurityFlags(myCookie);

            HttpContext.Current.Response.Cookies.Add(myCookie);
        }

        private static void SetSecurityFlags(HttpCookie cookie)
        {
            cookie.HttpOnly = true;
            cookie.Path = "/";
            cookie.Secure = HttpContext.Current.Request.IsSecureConnection;
        }
    }

}
