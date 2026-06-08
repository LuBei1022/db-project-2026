using LiteratureManager.Common;
using BLL;
using Model;
using System;

namespace Web.WebsiteData
{
    public partial class Single : System.Web.UI.Page
    {
        BLLBase<tbl_class> tbl_classbll = new BLLBase<tbl_class>();
        public int mid = 0;
        public bool webisyes = false;
        public tbl_class tbl_class = new tbl_class();
        public string tbclass_title = string.Empty;
        public string banner = string.Empty;
        public string SingleTitle
        {
            get
            {
                string title = Function.HtmlDiscode(tbl_class.about ?? string.Empty).Trim();
                return title.EndsWith("页面") ? title.Substring(0, title.Length - 2).Trim() : title;
            }
        }
        public string SingleIntro
        {
            get
            {
                string intro = Function.HtmlDiscode(tbl_class.description ?? string.Empty).Trim();
                return string.IsNullOrWhiteSpace(intro) ? "在这里查看平台说明、服务规则和使用支持，帮助你更顺畅地完成文献检索、投稿、下载与协作。" : intro;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    webisyes = false;
                    banner = string.Empty;
                    mid = Function.ConvertTo<int>(Function.GetRequest("mid"), 0);
                    tbl_class = tbl_classbll.SelectSingle("id=" + mid + " and isshow=1 and model =3 and id in(" + Function.Decrypt(CommonFunc.GetChildrenId(360)) + ")");
                    if (tbl_class != null && tbl_class.id > 0 && tbl_class.parentid > 0)
                    {
                        tbclass_title = CommonFunc.GetTbClassTitle(tbl_class);
                        banner = CommonFunc.GetBannerImg(tbl_class.upload_pic_pc, tbl_class.upload_pic_m);

                        webisyes = true;
                    }
                }
                catch (Exception ex)
                {
                    ImportDataLog.WriteLog(LogType.Error, "Single.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
                }
                if (!webisyes)
                {
                    Response.Redirect("/err");
                    Response.End();
                }
            }
        }
    }
}
