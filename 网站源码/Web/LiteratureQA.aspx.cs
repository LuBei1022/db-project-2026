using System;

namespace Web
{
    public partial class LiteratureQA : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 页面本身是静态外壳，所有数据通过 /Inc/RagApi.ashx 异步获取。
        }
    }
}
