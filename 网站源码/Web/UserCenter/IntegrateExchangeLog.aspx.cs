using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;

namespace Web.UserCenter
{
    public partial class IntegrateExchangeLog : System.Web.UI.Page
    {
        BLLBase<integrateExchangeLog_list> integrateExchangeLog_listbll = new BLLBase<integrateExchangeLog_list>();
        public user_list user_list = new user_list();
        public static int PageIndex = 0;
        public int intRecordCount = 0;
        public int type_ = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    type_ = 0;
                    user_list = CommonUserFunc.GetUserLoginStatus();
                    if (user_list != null && user_list.id > 0)
                    {
                        string Condition = " user_id=" + user_list.id;

                        type_ = Function.ConvertTo<int>(Function.GetRequest("type"), 0);
                        if (type_ == 1 || type_ == -1)
                        {
                            Condition += " and status=" + type_;
                        }
                        else
                        {
                            type_ = 0;
                        }

                        ViewState["strWhere"] = Condition;
                        string strWhere = ViewState["strWhere"].ToString();

                        //表或视图名
                        string tblName = "integrateExchangeLog_list";
                        //需要返回的列
                        string strGetFields = " id, name, num_integrate, codestr, addtime, status, user_id,upload_pic_img";
                        //排序的字段名
                        string fldname = " addtime desc,id desc";


                        //每页显示的记录数
                        int page_Size = 20;
                        //统计总记录数
                        intRecordCount = integrateExchangeLog_listbll.GetCount(tblName, strWhere);

                        PageIndex = Function.ConvertTo<int>(Function.GetRequest("page"), 0);
                        if (PageIndex > 0)
                        {
                        }
                        else
                        {
                            PageIndex = 1;
                        }

                        DataTable dt = integrateExchangeLog_listbll.GetListByPage(tblName, strGetFields, fldname, page_Size, PageIndex, strWhere);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            this.DataList.DataSource = dt.DefaultView;
                            this.DataList.DataBind();
                        }
                    }
                    else
                    {
                        Response.Redirect("/");
                    }
                }
                catch (Exception ex)
                {
                    ImportDataLog.WriteLog(LogType.Error, "IntegrateExchangeLog.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
                }
            }
        }
    }
}