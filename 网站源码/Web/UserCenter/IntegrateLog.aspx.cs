using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Text;

namespace Web.UserCenter
{
    public partial class IntegrateLog : System.Web.UI.Page
    {
        private readonly BLLBase<integrateLog_list> integrateLog_listbll = new BLLBase<integrateLog_list>();
        private readonly BLLBase<TopUpType_List> topUpTypeBll = new BLLBase<TopUpType_List>();
        private readonly BLLBase<websiteinfo_list> websiteinfoBll = new BLLBase<websiteinfo_list>();

        public user_list user_list = new user_list();
        public static int PageIndex = 0;
        public int intRecordCount = 0;
        public int huoqu_num_integrate = 0;
        public int xiaohao_num_integrate = 0;
        public int type_ = 0;
        public int current_integrate = 0;
        public int money_integrate = 10;
        public int integrate_donate = 0;
        public string TopUpOptionsHtml = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            try
            {
                type_ = 0;
                xiaohao_num_integrate = 0;
                huoqu_num_integrate = 0;
                user_list = CommonUserFunc.GetUserLoginStatus();
                if (user_list == null || user_list.id <= 0)
                {
                    Response.Redirect("/");
                    return;
                }

                current_integrate = CommonUserFunc.GetUserIntegrateSumFunc(user_list.id, 0);
                xiaohao_num_integrate = CommonUserFunc.GetUserIntegrateSumFunc(user_list.id, -1);
                huoqu_num_integrate = CommonUserFunc.GetUserIntegrateSumFunc(user_list.id, 1);

                websiteinfo_list websiteinfo = websiteinfoBll.SelectSingle("id=1");
                if (websiteinfo != null)
                {
                    money_integrate = websiteinfo.money_integrate > 0 ? websiteinfo.money_integrate : 10;
                    integrate_donate = websiteinfo.integrate_donate;
                }
                TopUpOptionsHtml = BuildTopUpOptions();

                string condition = " user_id=" + user_list.id;

                type_ = Function.ConvertTo<int>(Function.GetRequest("type"), 0);
                if (type_ == 1)
                {
                    condition += " and num_integrate>0";
                }
                else if (type_ == -1)
                {
                    condition += " and num_integrate<0";
                }
                else
                {
                    type_ = 0;
                }

                ViewState["strWhere"] = condition;
                string strWhere = ViewState["strWhere"].ToString();
                string tblName = "integrateLog_list";
                string strGetFields = " id, num_integrate, type, name, info_, addtime, user_id";
                string fldname = " addtime desc,id desc";

                const int pageSize = 20;
                intRecordCount = integrateLog_listbll.GetCount(tblName, strWhere);

                PageIndex = Function.ConvertTo<int>(Function.GetRequest("page"), 0);
                if (PageIndex <= 0)
                {
                    PageIndex = 1;
                }

                DataTable dt = integrateLog_listbll.GetListByPage(tblName, strGetFields, fldname, pageSize, PageIndex, strWhere);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataList.DataSource = dt.DefaultView;
                    DataList.DataBind();
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "IntegrateLog.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
            }
        }

        private string BuildTopUpOptions()
        {
            StringBuilder html = new StringBuilder();
            DataTable dt = topUpTypeBll.GetDatatable("select id,money from TopUpType_List where isshow=1 order by money asc,id asc");
            if (dt == null || dt.Rows.Count <= 0)
            {
                return html.ToString();
            }

            foreach (DataRow row in dt.Rows)
            {
                int id = Function.ConvertTo<int>(row["id"].ToString(), 0);
                int money = Function.ConvertTo<int>(row["money"].ToString(), 0);
                if (id <= 0 || money <= 0)
                {
                    continue;
                }

                html.Append("<button type=\"button\" class=\"topup-option\" data-id=\"")
                    .Append(id)
                    .Append("\" data-money=\"")
                    .Append(money)
                    .Append("\">")
                    .Append(money)
                    .Append(" 元</button>");
            }

            return html.ToString();
        }
    }
}
