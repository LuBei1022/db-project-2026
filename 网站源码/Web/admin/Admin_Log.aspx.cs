using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;

namespace Web.admin
{
    public partial class Admin_Log : System.Web.UI.Page
    {
        BLLBase<user_login> user_loginbll = new BLLBase<user_login>();
        public int MenuId = Function.ConvertTo<int>(Function.GetRequest("MenuId"), 0);
        public bool isLoading = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            if (!IsPostBack)
            {
                BindData();
            }
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        protected void BindData()
        {
            string Condition = " 1=1";


            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "user_login";
            //需要返回的列
            string strGetFields = " *";
            //排序的字段名
            string fldname = "id desc";
            //每页显示的记录数

            AspNetPager1.PageSize = 15;
            int page_Size = this.AspNetPager1.PageSize;
            //统计总记录数
            int intRecordCount = user_loginbll.GetCount(tblName, strWhere);
            if (intRecordCount > 0)
            {
                DivNull.Visible = false;
            }
            DataTable dt = user_loginbll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, strWhere);
            AspNetPager1.RecordCount = intRecordCount;
            AspNetPager1.AlwaysShow = true;
            if (dt != null && dt.Rows.Count > 0)
            {
                this.Repeater1.DataSource = dt.DefaultView;
                this.Repeater1.DataBind();
            }

        }
        protected void AspNetPager1_PageChanged(object src, EventArgs e)
        {
            BindData();
        }
    }

}