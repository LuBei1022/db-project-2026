using LiteratureManager.Common;
using BLL;
using Model;
using System;

namespace Web.UserCenter
{
    public partial class IntegrateWithdrawal : System.Web.UI.Page
    {
        BLLBase<websiteinfo_list> websiteinfo_listbll = new BLLBase<websiteinfo_list>();
        public websiteinfo_list websiteinfo_list = new websiteinfo_list();
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("/User/IntegrateExchange", true);

            try
            {
                websiteinfo_list = websiteinfo_listbll.SelectSingle("id=1");

            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "IntegrateWithdrawal.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
            }
        }
    }
}
