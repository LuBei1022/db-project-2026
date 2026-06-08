using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;

namespace Web.UserCenter
{
    public partial class ServiceLog : System.Web.UI.Page
    {
        BLLBase<ServiceLog_List> ServiceLog_ListBll = new BLLBase<ServiceLog_List>();
        public user_list user_list = new user_list();
        public static int PageIndex = 0;
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
                    ImportDataLog.WriteLog(LogType.Error, "ServiceLogAdd.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
                }
            }
        }

        protected void BindDataList()
        {
            string Condition = " userid=" + user_list.id;


            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "ServiceLog_List";
            //需要返回的列
            string strGetFields = " id, name, info_, addtime,uptime, status, userid";
            //排序的字段名
            string fldname = " uptime desc,addtime desc,id desc";


            //每页显示的记录数
            int page_Size = 20;
            //统计总记录数
            int intRecordCount = ServiceLog_ListBll.GetCount(tblName, strWhere);

            PageIndex = Function.ConvertTo<int>(Function.GetRequest("page"), 0);
            if (PageIndex > 0)
            {
            }
            else
            {
                PageIndex = 1;
            }

            DataTable dt = ServiceLog_ListBll.GetListByPage(tblName, strGetFields, fldname, page_Size, PageIndex, strWhere);
            if (dt != null && dt.Rows.Count > 0)
            {
                this.DataList.DataSource = dt.DefaultView;
                this.DataList.DataBind();
            }
        }
        protected void AspNetPager1_PageChanged(object src, EventArgs e)
        {
            BindDataList();
        }
    }
}