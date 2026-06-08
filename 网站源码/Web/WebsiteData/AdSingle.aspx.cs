using LiteratureManager.Common;
using BLL;
using Model;
using System;

namespace Web.WebsiteData
{
    public partial class AdSingle : System.Web.UI.Page
    {
        BLLBase<indexsingle_list> indexsingle_listbll = new BLLBase<indexsingle_list>();
        public string banner = string.Empty;
        public bool webisyes = false;
        public indexsingle_list indexsingle_list = new indexsingle_list();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    banner = string.Empty;
                    webisyes = false;
                    indexsingle_list = indexsingle_listbll.SelectSingle("id=" + Function.ConvertTo<int>(Function.GetRequest("id"), 0) + " and isshow=1");
                    if (indexsingle_list != null && indexsingle_list.id > 0)
                    {
                        banner = CommonFunc.GetBannerImg(indexsingle_list.upload_pic_pc, indexsingle_list.upload_pic_m);

                        webisyes = true;
                    }
                }
                catch (Exception ex)
                {
                    ImportDataLog.WriteLog(LogType.Error, "AdSingle.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
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