using System.Configuration;

namespace LiteratureManager.Common
{
    public class WxPayConst
    {
        private static string GetAppSetting(string key, string fallback)
        {
            string value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        /// <summary>
        /// 直连商户申请的公众号或移动应用appid。
        /// </summary>
        public static string appid => GetAppSetting("wxpay_appid", "wx499d14b1bce1bb81");

        /// <summary>
        /// AppSecret，app端加密解密使用
        /// </summary>
        public static string AppSecret => GetAppSetting("wxpay_appsecret", "");

        /// <summary>
        /// APIv3 密钥
        /// </summary>
        public static string APIV3Key => GetAppSetting("wxpay_apiv3key", "huh8y7uYYTnju8Gftr5CFDe5ggykiu8y");

        /// <summary>
        /// 微信支付商户号
        /// </summary>
        public static string mchid => GetAppSetting("wxpay_mchid", "1277407001");

        /// <summary>
        /// 微信支付证书序列号
        /// </summary>
        public static string serialNo => GetAppSetting("wxpay_serial_no", "54B3D4654927ED6C71CF1C9B7714392608DB4891");

        /// <summary>
        /// PKCS8 base64 私钥，不带 BEGIN/END 头
        /// </summary>
        public static string privateKey => GetAppSetting("wxpay_private_key", @"MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC71wmZoQa0u3ZI
UTkVjIY+Ikq2VyeS0tc4ggYfdH97aHmdBD+tLxb+5nQpgpj2tlfP96A+CQknrTlq
lJkthHiEPcc3o8LHUOBrnEATGMGhWqcWaAXT0iwMs7JiP4ZEnbpdiEVVaHXQb246
lXamuc//YV4g0EdlrIek1pQ+x56Lt0S5YJ2R8dTC42z+o+JKOAl6ADOxVoHam1Bt
2ciBUgyzgbdBfKKiaP95CSibbz2Nsv35v8sHMGUQTtXSH56/6+D6esqsjqMWEC2X
xFjTa0sBEU1ER+W/rW6gnH6iVlnfqS/ozmQ7ouhCA2X1jCN7QELkjQgm07ArK2fc
63Sr4lPzAgMBAAECggEBAI9WRpMfF5juvBHvORpCg2YCkPSXiTE3BtiuD7INGgK3
9KVmypiDKI8WmGncqJELD/M4yOTNzFikjP3RqxnazoRLCCxjII0sIDS9pP9tksRo
ArzMYDYFWWvP7D2gr/rISaB6Dj5gWhbWEU1PJJ2RiTEdwdBUX0cs4s1cmP9XIJsH
zag/bCYPntzNYR8EICEubULY8sfPkWzdUmcZdfS6QyonRzs3wC+4aNqtn7hTzC9g
4TnvX3ynH++8EtIDKK8SVNLern0DWSlmUra9KDJMggH8wlWoZZKm25mN0s1msSjX
igV5chx9aF/2Lnkzztpmsotjc+SZWHPbjT1sxK+Dn9ECgYEA5oXqu59TGtWl9cYX
lpMX133HgLnYJJVl1tVU78KpH5u2AC+QBWw/5jlAL/UtAkt+jOVXl6skfHpRCxQZ
GqF/vXj0n0UPzOJt8o4OL9lMr+X+DSByLapD7zTJ4QSi7iilSp5Zq+6tVMT0ANOM
kooEDTslPWHJ/UIcffV3x6/cCB0CgYEA0JmBwbDKijx1y5meL5Mi8m6G8x0oQb+e
yUvL4eWDA/ueX9cSTZVe1a7voLSn3woTL416XZ8nNcvP7u7ZsNZH1eAqkEpmaHet
gyNuEeIL6Oli8O1DR4MRkAumfQB6rd1c7kOrwEQdvDVEzeFlN/FXAfro6BSm8C2p
qjqyVNgwr08CgYBBSZI2eyQtSG8NUYIiuhwZgtz63yeRDOLf2mGI2gfOFOqR2Oag
Mo1SZcqBCp4ptTntK5MIOIdH3diQG6wUd8LW5afaZ9PWwhZDiOoJsTBf7PJrw1Gz
DzLYT4oReZ+vdcGChaB96kYa6QD2LvP0GLSXDrY4UTbEzHGHVvsKigr2HQKBgCG1
DHCd/ryDtI0nz5Xkcrs1/Px/86dcLW5dnx5rldYo7JiUClMbFe31jKctSgsSd7Mx
a1qBgzaALqNvWSHoHkeDJ52VSCMNY65TJVAidMY4IFLKJBsEJOxf1ZFRaIF7ya5+
pNw2pY9qFCooh9CYYPTi9Iu7+pXI6yekwHhRTtylAoGBALEV26SN/9wxEodCRQ8D
+M38SGysGvC45hmEW0r5Rhiv+r9RjHkeBey9WNm97VGgQsuKZNnaQ2vlR4wY2PhO
2DMuRCevZisnwKpvjUKg7QHlTTPidU6TpqLgBUhy9by+/aEJQE7FjvSG6DVfkGrt
tuxGZwdGxQXD5Q7qFeqHzfsx");
    }
}
