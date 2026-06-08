using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;

namespace Web.admin
{
    public partial class Admin_LiteratureImportError : System.Web.UI.Page
    {
        private readonly BLLBase<LiteratureImportError> errorBll = new BLLBase<LiteratureImportError>();
        public string MenuId = Function.GetRequest("MenuId");
        public bool isLoading = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            BindData();
        }

        protected void BindData()
        {
            int batchId = Function.ConvertTo<int>(Function.GetRequest("BatchId"), 0);
            string where = "batch_id=" + batchId;
            ViewState["strWhere"] = where;

            string tblName = "LiteratureImportError";
            string strGetFields = "id,batch_id,row_no,title,error_msg,raw_data,addtime";
            string fldname = "row_no asc,id asc";

            AspNetPager1.PageSize = 20;
            int intRecordCount = errorBll.GetCount(tblName, where);
            DivNull.Visible = intRecordCount <= 0;
            DataTable dt = errorBll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, where);
            AspNetPager1.RecordCount = intRecordCount;
            AspNetPager1.AlwaysShow = true;
            if (dt != null && dt.Rows.Count > 0)
            {
                Repeater1.DataSource = dt.DefaultView;
                Repeater1.DataBind();
            }
        }

        protected void AspNetPager1_PageChanged(object src, EventArgs e)
        {
            BindData();
        }
    }
}
