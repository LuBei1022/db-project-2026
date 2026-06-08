using System;

namespace Web.UserCenter
{
    public partial class left : System.Web.UI.UserControl
    {
        public string Url = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            Url = (Context.Request.Url.LocalPath ?? string.Empty).Replace("/User/", "").Trim('/');
        }

        protected string GetClassHtml(string btn)
        {
            string currentClass = string.Empty;
            string[] buttonKeys = btn.Split(',');
            foreach (string item in buttonKeys)
            {
                if (string.Equals(item, Url, StringComparison.OrdinalIgnoreCase))
                {
                    currentClass = "current";
                    break;
                }

                if (item.Contains("_") && Url.Contains("_"))
                {
                    string[] itemParts = item.Split('_');
                    string[] urlParts = Url.Split('_');
                    if (string.Equals(itemParts[0], urlParts[0], StringComparison.OrdinalIgnoreCase))
                    {
                        currentClass = "current";
                        break;
                    }
                }
            }
            return currentClass;
        }
    }
}
