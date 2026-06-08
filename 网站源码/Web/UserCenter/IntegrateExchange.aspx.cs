using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;

namespace Web.UserCenter
{
    public partial class IntegrateExchange : System.Web.UI.Page
    {
        BLLBase<integrate_list> integrate_listbll = new BLLBase<integrate_list>();
        public user_list user_list = new user_list();
        public static int PageIndex = 0;
        public int intRecordCount = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    user_list = CommonUserFunc.GetUserLoginStatus();
                    if (user_list != null && user_list.id > 0)
                    {
                        BindDataList();
                    }
                    else
                    {
                        Response.Redirect("/");
                    }
                }
                catch (Exception ex)
                {
                    ImportDataLog.WriteLog(LogType.Error, "IntegrateExchange.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
                }
            }
        }

        protected void BindDataList()
        {
            string Condition = " 1=1";


            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "integrate_list";
            //需要返回的列
            string strGetFields = " id, name, orderid, uptime, addtime, upload_pic_img, about_, num_integrate";
            //排序的字段名
            string fldname = " orderid desc,uptime desc,addtime desc,id desc";


            //每页显示的记录数
            int page_Size = 20;
            //统计总记录数
            intRecordCount = integrate_listbll.GetCount(tblName, strWhere);

            PageIndex = Function.ConvertTo<int>(Function.GetRequest("page"), 0);
            if (PageIndex > 0)
            {
            }
            else
            {
                PageIndex = 1;
            }

            DataTable dt = integrate_listbll.GetListByPage(tblName, strGetFields, fldname, page_Size, PageIndex, strWhere);
            if (dt != null && dt.Rows.Count > 0)
            {
                this.DataList.DataSource = dt.DefaultView;
                this.DataList.DataBind();
            }
        }
    }
}