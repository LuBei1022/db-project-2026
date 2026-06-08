using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web
{
    public partial class wx_pay_notify : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        //public static string AesGcmDecrypt(string associatedData, string nonce, string ciphertext, string apiV3Key)
        //{
            //var cipher = new GcmBlockCipher(new AesEngine());
            //var parameters = new AeadParameters(new KeyParameter(Encoding.UTF8.GetBytes(apiV3Key)), 128,
            //                                    Encoding.UTF8.GetBytes(nonce),
            //                                    Encoding.UTF8.GetBytes(associatedData));
            //cipher.Init(false, parameters);
            //var data = Convert.FromBase64String(ciphertext);
            //var plaintext = new byte[cipher.GetOutputSize(data.Length)];
            //var len = cipher.ProcessBytes(data, 0, data.Length, plaintext, 0);
            //cipher.DoFinal(plaintext, len);
            //return Encoding.UTF8.GetString(plaintext).TrimEnd('\0');
        //}
    }
}