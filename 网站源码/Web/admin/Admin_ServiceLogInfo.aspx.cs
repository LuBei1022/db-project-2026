using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Web.admin
{
    public partial class Admin_ServiceLogInfo : System.Web.UI.Page
    {
        BLLBase<ServiceLog_List> ServiceLog_ListBll = new BLLBase<ServiceLog_List>();
        BLLBase<ServiceLogInfo_List> ServiceLogInfo_Listbll = new BLLBase<ServiceLogInfo_List>();
        BLLBase<user_list> user_listbll = new BLLBase<user_list>();
        public string MenuId = Function.GetRequest("MenuId");
        public ServiceLog_List ServiceLog_List = new ServiceLog_List();
        public bool isLoading = false;
        public string upload_pic_avatar = string.Empty;
        public string user_name = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            ServiceLog_List = ServiceLog_ListBll.SelectSingle("id=" + Function.ConvertTo<int>(Function.GetRequest("id"), 0));
            if (ServiceLog_List != null && ServiceLog_List.id > 0)
            {

                user_list user_list = user_listbll.SelectSingle("id=" + ServiceLog_List.userid);
                if (user_list != null && user_list.id > 0)
                {
                    upload_pic_avatar = CommonUserFunc.GetUserAvatarFunc(user_list.upload_pic_avatar);
                    user_name = (!string.IsNullOrWhiteSpace(user_list.name) ? Function.HtmlDiscode(user_list.name) : "我");
                }

                DataTable ServiceLogInfo_Listdt = ServiceLogInfo_Listbll.GetDatatable("select info_, type, addtime, adminname from ServiceLogInfo_List where ServiceLog_Id=" + ServiceLog_List.id + " order by addtime asc");
                if (ServiceLogInfo_Listdt != null && ServiceLogInfo_Listdt.Rows.Count > 0)
                {
                    this.DataList.DataSource = ServiceLogInfo_Listdt.DefaultView;
                    this.DataList.DataBind();
                }
                ServiceLogInfo_Listdt.Dispose();

                isLoading = true;
            }
        }

        protected void OnClick_AddUp(object sender, EventArgs e)
        {
            isLoading = false;
            AddUp.Visible = false;
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);

            string BackURL = Request.QueryString["BackURL"];
            if (string.IsNullOrWhiteSpace(BackURL))
            {
                BackURL = "Admin_ServiceLogInfo.aspx?MenuId=" + MenuId + "&id=" + Function.ConvertTo<int>(Function.GetRequest("id"), 0);
            }
            ServiceLog_List = ServiceLog_ListBll.SelectSingle("id=" + Function.ConvertTo<int>(Function.GetRequest("id"), 0));
            if (ServiceLog_List != null && ServiceLog_List.id > 0)
            {
                user_list user_list = user_listbll.SelectSingle("id=" + ServiceLog_List.userid);
                if (user_list != null && user_list.id > 0)
                {
                    string form_info_ = Function.HtmlSqlEncode(Function.FormRequest("info_"));
                    if (!string.IsNullOrWhiteSpace(form_info_))
                    {
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("insert into ServiceLogInfo_List(");
                        strSql.Append("ServiceLog_Id, info_, type, addtime, adminname)");
                        strSql.Append(" values (");
                        strSql.Append(" @ServiceLog_Id, @info_, @type, @addtime, @adminname)");
                        strSql.Append(";select @@IDENTITY");
                        SqlParameter[] parameters = {
                        new SqlParameter("@ServiceLog_Id", SqlDbType.Int),
                          new SqlParameter("@info_",SqlDbType.NVarChar,-1),
                          new SqlParameter("@type",SqlDbType.Int),
                          new SqlParameter("@addtime",SqlDbType.DateTime),
                          new SqlParameter("@adminname",SqlDbType.NVarChar,250)
                                    };
                        parameters[0].Value = ServiceLog_List.id;
                        parameters[1].Value = form_info_;
                        parameters[2].Value = 2;
                        parameters[3].Value = DateTime.Now;
                        parameters[4].Value = Cookie.GetCookie("LMS_AdminName");
                        string sql = "UPDATE ServiceLog_List SET status=1, uptime = GETDATE() WHERE id=" + ServiceLog_List.id;
                        sql += "INSERT INTO NoticeLog_List (name, type, addtime, userid, status, url,info_)  VALUES ('您提交的服务工单已经有最新回复咯！', 1, GETDATE(), " + ServiceLog_List.userid + ", 0, '/User/ServiceLog_" + ServiceLog_List.id + "','您的服务工单（标题：[" + ServiceLog_List.name + "]）已回复，点击查看详情。')";
                        int addid = ServiceLogInfo_Listbll.Add_R_Id_(parameters, strSql, sql);
                        if (addid > 0)
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "回复工单成功!", BackURL, 0);
                        }
                        else
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "回复工单失败!", BackURL, 2);
                        }
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "回复不能为空！", BackURL, 2);
                    }
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "提交人错误！", "Admin_ServiceLogList.aspx?MenuId=" + MenuId, 2);
                }
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "请求参数错误！", "Admin_ServiceLogList.aspx?MenuId=" + MenuId, 2);
            }
        }
    }
}