using LiteratureManager.Common;
using System;
using System.Configuration;
using System.Drawing;
using System.Web;

namespace Web.admin
{
    public partial class Code : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            string randomcode = GetCaptcha();
            this.CreateImage(randomcode);
        }

        /// <summary>
        ///
        /// </summary>
        protected string getcodeInit(int length)
        {
            string Vchar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            string[] VcArray = Vchar.Split(',');
            string VNum = ""; //由于字符串很短，就不用StringBuilder了
            int temp = -1; //记录上次随机数值，尽量避免生产几个一样的随机数
                           //采用一个简单的算法以保证生成随机数的不同
            Random rand = new Random();
            for (int i = 1; i < length + 1; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(VcArray.Length);
                if (temp != -1 && temp == t)
                {
                    i--;
                    continue;
                }
                temp = t;
                VNum += VcArray[t];
            }

            Session["yzmCode"] = VNum;
            CommonFunc.GetLoginCodeDelete();
            string NewGuidCode = Guid.NewGuid().ToString().Replace("-", "");
            if (CommonFunc.GetLoginCodeAdd(VNum.ToLower(), NewGuidCode))
            {
                HttpCookie cookie2 = new HttpCookie("BackstageCheckCode");
                cookie2.Value = Function.MD5Encrypt(NewGuidCode, ConfigurationManager.AppSettings["md5_key"]);
                HttpContext.Current.Response.Cookies.Add(cookie2);
                return VNum;
            }
            else
            {
                return "";
            }

        }
        /// <summary>
        /// 创建随机码图片
        /// </summary>
        /// <param name="randomcode">随机码</param>
        private void CreateImage(string checkCode)
        {
            if (checkCode == null || checkCode.Trim() == String.Empty)
            {
                return;
            }

            Bitmap image = new System.Drawing.Bitmap(96, 42);
            Graphics g = Graphics.FromImage(image);

            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的背景噪音线
                for (int i = 0; i < 6; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(GetRandomColor(CreateRandomSeed() + i)), x1, y1, x2, y2);
                }
                Font font = new System.Drawing.Font("Arial", 18, (System.Drawing.FontStyle.Bold));
                //写验证码
                for (int i = 1; i <= 8; i++)
                {
                    g.DrawString(checkCode.Substring(i - 1, 1), font,
                        new SolidBrush(GetRandomDarkerColor(CreateRandomSeed() + i)),
                        random.Next(image.Width / 10 * (i - 1), image.Width / 10 * i), random.Next(image.Height / 2 - 5));
                }
                //画图片的前景噪音点
                for (int i = 0; i < 300; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);

                    image.SetPixel(x, y, GetRandomColor(CreateRandomSeed() + i));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                //生成图片
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.ContentType = "image/gif";
                HttpContext.Current.Response.BinaryWrite(ms.ToArray());

                g.Dispose();
                image.Dispose();
            }
            catch
            {
                return;
            }
        }

        //验证码
        private static string captcha;

        /// <summary>
        /// Author:Johnny Wong
        /// Date:2013-10-28
        /// 生成4位长度的随机码
        /// </summary>
        /// <returns></returns>
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
            captcha = checkCode.Replace(" ", "");

            CommonFunc.GetLoginCodeDelete();
            string NewGuidCode = Guid.NewGuid().ToString().Replace("-", "");
            if (CommonFunc.GetLoginCodeAdd(captcha.ToLower(), NewGuidCode))
            {
                Cookie.SaveCookie("BackstageCheckCode", Function.MD5Encrypt(NewGuidCode, ConfigurationManager.AppSettings["md5_key"]), 0);
                return checkCode;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Author:Johnny Wong
        /// Date:2013-10-28
        /// 根据传入的值生成验证码图片
        /// </summary>
        /// <param name="checkCode">验证码</param>
        public static Bitmap CodeImage(string checkCode)
        {
            if (checkCode == null || checkCode.Trim() == String.Empty)
            {
                return null;
            }

            Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 12.3)), 44);
            Graphics g = Graphics.FromImage(image);

            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的背景噪音线
                for (int i = 0; i < 6; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(GetRandomColor(CreateRandomSeed() + i)), x1, y1, x2, y2);
                }
                Font font = new System.Drawing.Font("Arial", 18, (System.Drawing.FontStyle.Bold));
                //写验证码
                for (int i = 1; i <= 8; i++)
                {
                    g.DrawString(checkCode.Substring(i - 1, 1), font,
                        new SolidBrush(GetRandomDarkerColor(CreateRandomSeed() + i)),
                        random.Next(image.Width / 10 * (i - 1), image.Width / 10 * i), random.Next(image.Height / 2 - 5));
                }
                //画图片的前景噪音点
                for (int i = 0; i < 300; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);

                    image.SetPixel(x, y, GetRandomColor(CreateRandomSeed() + i));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                return image;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Author:Johnny Wong
        /// Date:2013-10-28
        /// 根据传入的值生成验证码图片
        /// </summary>
        /// <param name="checkCode">验证码</param>
        /// <param name="isRotate">是否带旋转</param>
        public static Bitmap CodeImage(string checkCode, bool isRotate)
        {
            if (isRotate)
            {
                if (checkCode == null || checkCode.Trim() == String.Empty)
                {
                    return null;
                }

                Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 12.3)), 44);
                Graphics g = Graphics.FromImage(image);

                try
                {
                    //生成随机生成器
                    Random random = new Random();
                    //清空图片背景色
                    g.Clear(Color.White);
                    //画图片的背景噪音线
                    for (int i = 0; i < 6; i++)
                    {
                        int x1 = random.Next(image.Width);
                        int x2 = random.Next(image.Width);
                        int y1 = random.Next(image.Height);
                        int y2 = random.Next(image.Height);
                        g.DrawLine(new Pen(GetRandomColor(CreateRandomSeed() + i)), x1, y1, x2, y2);
                    }
                    //画图片的边框线
                    g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                    //字体
                    Font font = new System.Drawing.Font("Arial", 18, (System.Drawing.FontStyle.Bold));
                    //写验证码
                    char[] chars = checkCode.ToCharArray();

                    StringFormat format = new StringFormat(StringFormatFlags.NoClip);
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    Point dot = new Point(0, 0);
                    for (int i = 1; i <= chars.Length; i++)
                    {
                        if (i == 1)
                        {
                            dot = new Point(random.Next(image.Width / 9, image.Width / 8), random.Next(13, image.Height - 8));
                        }
                        else
                        {
                            dot = new Point(random.Next(image.Width / 8), random.Next(13, image.Height - 8));
                        }
                        float angle = random.Next(-45, 46);
                        g.TranslateTransform(dot.X, dot.Y); //移动光标到指定位置   
                        g.RotateTransform(angle);
                        g.DrawString(chars[i - 1].ToString(), font,
                            new SolidBrush(GetRandomDarkerColor(CreateRandomSeed() + i)),
                            1, 1, format);
                        g.RotateTransform(-angle); //转回去
                        g.TranslateTransform(2, -dot.Y); //移动光标到指定位置
                    }
                    //画图片的前景噪音点
                    for (int i = 0; i < 300; i++)
                    {
                        int x = random.Next(image.Width);
                        int y = random.Next(image.Height);

                        image.SetPixel(x, y, GetRandomColor(CreateRandomSeed() + i));
                    }

                    return image;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return CodeImage(checkCode);
            }
        }

        /// <summary>
        /// Author:Johnny Wong
        /// Date:2013-10-28
        /// 获取随机颜色
        /// </summary>
        /// <returns></returns>
        private static System.Drawing.Color GetRandomColor()
        {
            Random random = new Random();
            return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256), random.Next(256));
        }

        /// <summary>
        /// Author:Johnny Wong
        /// Date:2013-10-28
        /// 获取随机颜色
        /// </summary>
        /// <param name="randomSeed">随机种子</param>
        /// <returns></returns>
        private static System.Drawing.Color GetRandomColor(int randomSeed)
        {
            Random random = new Random(randomSeed);
            return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256), random.Next(256));
        }

        /// <summary>
        /// Author:Johnny Wong
        /// Date:2013-10-28
        /// 获取随机加深颜色
        /// </summary>
        /// <returns></returns>
        private static System.Drawing.Color GetRandomDarkerColor()
        {
            return GetDarkerColor(GetRandomColor());
        }

        /// <summary>
        /// Author:Johnny Wong
        /// Date:2013-10-28
        /// 获取随机加深颜色
        /// <param name="randomSeed">随机种子</param>
        /// </summary>
        /// <returns></returns>
        private static Color GetRandomDarkerColor(int randomSeed)
        {
            Random random = new Random(randomSeed);
            return GetDarkerColor(GetRandomColor());
        }

        /// <summary>
        /// Author:Johnny Wong
        /// Date:2013-10-28
        /// 获取该颜色的加深颜色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static Color GetDarkerColor(Color color)
        {
            const int max = 255;
            int increase = new Random(Guid.NewGuid().GetHashCode()).Next(30, 255); //还可以根据需要调整此处的值

            int r = Math.Abs(Math.Min(color.R - increase, max));
            int g = Math.Abs(Math.Min(color.G - increase, max));
            int b = Math.Abs(Math.Min(color.B - increase, max));

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Author:Johnny Wong
        /// Date:2013-10-31
        /// 校验验证码(不区分大小写)
        /// </summary>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public static bool IsRight(string code)
        {
            return code.ToUpper() == captcha.ToUpper();
        }

        /// <summary>
        /// Author:Johnny Wong
        /// Date:2013-10-31
        /// 校验验证码
        /// </summary>
        /// <param name="code">验证码</param>
        /// <param name="matchCase">大小写区分</param>
        /// <returns></returns>
        public static bool IsRight(string code, bool matchCase)
        {
            if (matchCase)
            {
                return code == captcha;
            }
            return code.ToUpper() == captcha.ToUpper();
        }

        /// <summary>
        /// Author:Johnny Wong
        /// Time:2013-10-29
        /// 根据当前时间获取一个随机种子
        /// </summary>
        /// <returns></returns>
        private static int CreateRandomSeed()
        {
            long tc = DateTime.Now.Ticks;
            return Convert.ToInt32(tc.ToString().Substring(tc.ToString().Length - 9));
        }

        ///// <summary>
        ///// Author:Johnny Wong
        ///// Time:2013-10-28
        ///// 根据传入的时间获取一个随机种子
        ///// <param name="dt">传入时间</param>
        ///// </summary>
        ///// <returns></returns>
        //private static int CreateRandomSeed(DateTime dt)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(CreateRandomSeed(dt.Hour.ToString(), 2));
        //    sb.Append(CreateRandomSeed(dt.Minute.ToString(), 2));
        //    sb.Append(CreateRandomSeed(dt.Second.ToString(), 2));
        //    sb.Append(CreateRandomSeed(dt.Millisecond.ToString(), 3));
        //    return Convert.ToInt32(sb.ToString());
        //}

        ///// <summary>
        ///// Author:Johnny Wong
        ///// Time:2013-10-28
        ///// 根据当前时间获取一个随机种子
        ///// </summary>
        ///// <returns></returns>
        //private static int CreateRandomSeed()
        //{
        //    DateTime dt = DateTime.Now;
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(CreateRandomSeed(dt.Hour.ToString(), 2));
        //    sb.Append(CreateRandomSeed(dt.Minute.ToString(), 2));
        //    sb.Append(CreateRandomSeed(dt.Second.ToString(), 2));
        //    sb.Append(CreateRandomSeed(dt.Millisecond.ToString(), 3));
        //    return Convert.ToInt32(sb.ToString());
        //}

        ///// <summary>
        ///// Author:Johnny Wong
        ///// Time:2013-10-28
        ///// 拼接字符串(随机种子)
        ///// </summary>
        ///// <param name="time">传入参数</param>
        ///// <param name="length">字符串长度</param>
        ///// <returns></returns>
        //private static string CreateRandomSeed(string time, int length)
        //{
        //    while (time.Length < length)
        //    {
        //        time = "0" + time;
        //    }
        //    return time;
        //}
    }
}