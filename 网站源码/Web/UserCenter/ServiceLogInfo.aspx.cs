using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Web.UserCenter
{
    public partial class ServiceLogInfo : System.Web.UI.Page
    {
        BLLBase<ServiceLog_List> ServiceLog_ListBll = new BLLBase<ServiceLog_List>();
        BLLBase<ServiceLogInfo_List> ServiceLogInfo_Listbll = new BLLBase<ServiceLogInfo_List>();
        public user_list user_list = new user_list();
        public ServiceLog_List ServiceLog_List = new ServiceLog_List();
        public bool webisyes = false;
        public string upload_pic_avatar = string.Empty;
        public string user_name = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    upload_pic_avatar = string.Empty;
                    user_name = string.Empty;
                    webisyes = false;
                    user_list = CommonUserFunc.GetUserLoginStatus();
                    if (user_list != null && user_list.id > 0)
                    {
                        ServiceLog_List = ServiceLog_ListBll.SelectSingle("id=" + Function.ConvertTo<int>(Function.GetRequest("id"), 0) + " and userid=" + user_list.id);
                        if (ServiceLog_List != null && ServiceLog_List.id > 0)
                        {
                            upload_pic_avatar = CommonUserFunc.GetUserAvatarFunc(user_list.upload_pic_avatar);
                            user_name = (!string.IsNullOrWhiteSpace(user_list.name) ? Function.HtmlDiscode(user_list.name) : "我");

                            DataTable ServiceLogInfo_Listdt = ServiceLogInfo_Listbll.GetDatatable("select info_, type, addtime, adminname from ServiceLogInfo_List where ServiceLog_Id=" + ServiceLog_List.id + " order by addtime asc");
                            if (ServiceLogInfo_Listdt != null && ServiceLogInfo_Listdt.Rows.Count > 0)
                            {
                                this.DataList.DataSource = ServiceLogInfo_Listdt.DefaultView;
                                this.DataList.DataBind();
                            }
                            ServiceLogInfo_Listdt.Dispose();

                            if (ServiceLog_List.status == 1)
                            {
                                ServiceLog_ListBll.Update("status=2,looktime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'", "id=" + ServiceLog_List.id + " and status=1");
                            }
                            webisyes = true;
                        }
                    }
                    if (!webisyes)
                    {
                        Response.Redirect("/err");
                        Response.End();
                    }
                }
                catch (Exception ex)
                {
                    ImportDataLog.WriteLog(LogType.Error, "ServiceLogAdd.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
                }
            }
        }


        protected void OnClick_AddUp(object sender, EventArgs e)
        {
            Main.Visible = false;

            user_list = CommonUserFunc.GetUserLoginStatus();
            if (user_list != null && user_list.id > 0)
            {
                ServiceLog_List = ServiceLog_ListBll.SelectSingle("id=" + Function.ConvertTo<int>(Function.GetRequest("id"), 0) + " and userid=" + user_list.id);
                if (ServiceLog_List != null && ServiceLog_List.id > 0)
                {
                    string form_info_ = Function.HtmlSqlEncode(Function.FormRequest("info_"));
                    if (!string.IsNullOrWhiteSpace(form_info_))
                    {
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("insert into ServiceLogInfo_List(");
                        strSql.Append("ServiceLog_Id, info_, type, addtime)");
                        strSql.Append(" values (");
                        strSql.Append(" @ServiceLog_Id, @info_, @type, @addtime)");
                        strSql.Append(";select @@IDENTITY");
                        SqlParameter[] parameters = {
                        new SqlParameter("@ServiceLog_Id", SqlDbType.Int),
                          new SqlParameter("@info_",SqlDbType.NVarChar,-1),
                          new SqlParameter("@type",SqlDbType.Int),
                          new SqlParameter("@addtime",SqlDbType.DateTime)
                                    };
                        parameters[0].Value = ServiceLog_List.id;
                        parameters[1].Value = form_info_;
                        parameters[2].Value = 1;
                        parameters[3].Value = DateTime.Now;
                        string sql = "UPDATE ServiceLog_List SET status=0, uptime = GETDATE() WHERE id=" + ServiceLog_List.id;
                        int addid = ServiceLogInfo_Listbll.Add_R_Id_(parameters, strSql, sql);
                        if (addid > 0)
                        {
                            CommonFunc.Ok_Return("继续提问提交成功咯！我们会尽快查看，然后第一时间给您回复哒，请您耐心等待~", "/User/ServiceLog_" + ServiceLog_List.id, 0);
                        }
                        else
                        {
                            CommonFunc.Ok_Return("继续提问提交失败，请稍后再试！", "/User/ServiceLog_" + ServiceLog_List.id, 2);
                        }
                    }
                    else
                    {
                        CommonFunc.Ok_Return("提问不能为空，自动跳转！", "/User/ServiceLog_" + ServiceLog_List.id, 2);
                    }
                }
                else
                {
                    CommonFunc.Ok_Return("提交参数异常，自动跳转！", "/User/ServiceLog", 2);
                }
            }
            else
            {
                Function.Show_Msg("登录状态异常！", "/");
            }
        }
    }
}