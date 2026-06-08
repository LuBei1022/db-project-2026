using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;

namespace Web.admin
{
    public partial class Admin_IntegrateLogTypeList : System.Web.UI.Page
    {
        BLLBase<integrateLogType_list> integrateLogType_listBll = new BLLBase<integrateLogType_list>();
        private const string AllowedTypeIds = "1,3,4,6,10,11,12";
        private const string EditableTypeIds = "1,4,10";
        string Action = Function.GetRequest("Action");
        public string MenuId = Function.GetRequest("MenuId");
        public bool isLoading = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            switch (Action)
            {
                case "Edit":
                    EditFunc();
                    break;
                default:
                    BindData();
                    break;
            }
        }

        protected void EditFunc()
        {
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            integrateLogType_list integrateLogType_list = integrateLogType_listBll.SelectSingle("Id=" + ID + " and id in (" + EditableTypeIds + ")");
            if (integrateLogType_list != null && integrateLogType_list.id > 0)
            {
                AddUp.Visible = true;
                Main.Visible = false;
                Txt_Title.Text = "<font color=\"red\">积分类型详情</font>";

                if (!string.IsNullOrWhiteSpace(integrateLogType_list.name))
                {
                    IntegrateLogType_name.Text = Function.HtmlDiscode(integrateLogType_list.name);
                }
                num_integrate.Text = integrateLogType_list.num_integrate.ToString();

            }
        }

        /// <summary>
        /// 绑定积分类型
        /// </summary>
        protected void BindData()
        {
            string Condition = " id in (" + AllowedTypeIds + ")";

            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "integrateLogType_list";
            //需要返回的列
            string strGetFields = " RANK()  OVER (order by id desc) AS xuhao,Id,name,num_integrate";
            //排序的字段名
            string fldname = "id asc";
            //每页显示的记录数

            AspNetPager1.PageSize = 15;
            int page_Size = this.AspNetPager1.PageSize;
            //统计总记录数
            int intRecordCount = integrateLogType_listBll.GetCount(tblName, strWhere);
            if (intRecordCount > 0)
            {
                DivNull.Visible = false;
            }
            DataTable dt = integrateLogType_listBll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, strWhere);
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

        protected void OnClick_AddUp(object sender, EventArgs e)
        {
            isLoading = false;
            AddUp.Visible = false;
            Main.Visible = false;
            string BackURL = Request.QueryString["BackURL"];
            if (string.IsNullOrWhiteSpace(BackURL))
            {
                BackURL = "Admin_IntegrateLogTypeList.aspx?MenuId=" + MenuId;
            }
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            integrateLogType_list integrateLogType_list = new integrateLogType_list();
            if (Action == "Edit")
            {
                integrateLogType_list = integrateLogType_listBll.SelectSingle("Id=" + ID + " and id in (" + EditableTypeIds + ")");
                if (!(integrateLogType_list != null && integrateLogType_list.id > 0))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "请求参数错误！", BackURL, 2);
                }
                integrateLogType_list.num_integrate = Function.ConvertTo<int>(Function.FormRequest("num_integrate"), 0);
                AddUp.Visible = false;
                string[] file = { "id" };
                if (integrateLogType_listBll.Update(file, integrateLogType_list))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "积分类型《<font color=\"red\">" + Function.HtmlDiscode(integrateLogType_list.name) + "</font>》 修改成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "积分类型《<font color=\"red\">" + Function.HtmlDiscode(integrateLogType_list.name) + "</font>》 修改失败!", BackURL, 2);
                }
            }
        }

    }

}
