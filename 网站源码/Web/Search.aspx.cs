using LiteratureManager.Common;
using System;

namespace Web
{
    public partial class Search : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string keyword = Function.GetRequest("keyword");
            string categoryId = Function.GetRequest("categoryId");
            string year = Function.GetRequest("year");

            string redirectUrl = "/LiteratureSearch.aspx";
            string queryString = string.Empty;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                queryString += (queryString.Length == 0 ? "?" : "&") + "keyword=" + Server.UrlEncode(keyword);
            }

            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                queryString += (queryString.Length == 0 ? "?" : "&") + "categoryId=" + Server.UrlEncode(categoryId);
            }

            if (!string.IsNullOrWhiteSpace(year))
            {
                queryString += (queryString.Length == 0 ? "?" : "&") + "year=" + Server.UrlEncode(year);
            }

            Response.Redirect(redirectUrl + queryString, true);
        }
    }
}
