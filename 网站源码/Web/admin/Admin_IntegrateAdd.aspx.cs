using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.IO;
using System.Threading;

namespace Web.admin
{
    public partial class Admin_IntegrateAdd : System.Web.UI.Page
    {
        BLLBase<daoru_list> daoru_listbll = new BLLBase<daoru_list>();
        BLLBase<daoruerr_list> daoruerr_listbll = new BLLBase<daoruerr_list>();
        BLLBase<integrateLog_list> integrateLog_listbll = new BLLBase<integrateLog_list>();
        BLLBase<user_list> user_listbll = new BLLBase<user_list>();
        public string MenuId = Function.GetRequest("MenuId");
        public bool isLoading = false;
        public int type = 1;
        public string file_name = string.Empty;
        public string AdminName = Cookie.GetCookie("LMS_AdminName");
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
            string Condition = " type=" + type;

            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "daoru_list";
            //需要返回的列
            string strGetFields = " RANK()  OVER (order by id asc) AS xuhao,*";
            //排序的字段名
            string fldname = "id desc";
            //每页显示的记录数

            AspNetPager1.PageSize = 15;
            int page_Size = this.AspNetPager1.PageSize;
            //统计总记录数
            int intRecordCount = daoru_listbll.GetCount(tblName, strWhere);
            if (intRecordCount > 0)
            {
                DivNull.Visible = false;
            }
            DataTable dt = daoru_listbll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, strWhere);
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
            try
            {
                isLoading = false;
                Main.Visible = false;
                string BackURL = Request.QueryString["BackURL"];
                if (string.IsNullOrWhiteSpace(BackURL))
                {
                    BackURL = "Admin_IntegrateAdd.aspx?MenuId=" + MenuId;
                }

                string user_tel_ = Function.HtmlEncode(Function.FormRequest("user_tel"));
                int num_integrate_ = Function.ConvertTo<int>(Function.FormRequest("num_integrate"), 0);
                if (!string.IsNullOrWhiteSpace(user_tel_) && (num_integrate_ > 0 || num_integrate_ < 0))
                {
                    user_list user_list = user_listbll.SelectSingle("tel='" + user_tel_ + "'");
                    if (user_list != null && user_list.id > 0)
                    {
                        if (user_list.isshow == 1)
                        {
                            string sql = "INSERT INTO integrateLog_list (num_integrate, type, name, info_, addtime, user_id,adminname) VALUES (" + num_integrate_ + ",3,'系统录入','',GETDATE()," + user_list.id + ",'" + Cookie.GetCookie("LMS_AdminName") + "')";
                            sql += "ξLiteratureManagerξINSERT INTO daoru_list (posttime, r_info, status, type) VALUES (GETDATE(), '给用户《" + Function.HtmlDiscode(user_list.name) + " / " + user_list.tel + "》录入 " + num_integrate_ + " 积分成功', 1, " + type + ")";
                            if (integrateLog_listbll.Sql_D(sql))
                            {
                                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "给用户《" + Function.HtmlDiscode(user_list.name) + " / " + user_list.tel + "》发放 " + num_integrate_ + " 积分成功!", BackURL, 0);
                            }
                            else
                            {
                                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "发放积分失败，请稍后再试!", BackURL, 2);
                            }
                        }
                        else
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "用户状态异常，请重新编辑信息!", BackURL, 2);
                        }
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "用户不存在，请重新编辑信息!", BackURL, 2);
                    }
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "发放积分信息不合格，请重新编辑信息!", BackURL, 2);
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
        }

        protected void ErrList(string info, int daoruid)
        {
            daoruerr_list daoruerr_list = new daoruerr_list();
            daoruerr_list.info = info;
            daoruerr_list.filename = file_name;
            daoruerr_list.addtime = DateTime.Now;
            daoruerr_list.daoruid = daoruid;
            int addid = daoruerr_listbll.Add(daoruerr_list, "ID");
        }

        protected void OnClick_AddBatch(object sender, EventArgs e)
        {
            try
            {
                isLoading = false;
                Main.Visible = false;
                string BackURL = Request.QueryString["BackURL"];
                if (string.IsNullOrWhiteSpace(BackURL))
                {
                    BackURL = "Admin_IntegrateAdd.aspx?MenuId=" + MenuId;
                }
                if (FileUpload1.HasFile)
                {
                    if (FileUpload1.PostedFile.ContentLength > UploadPolicy.MaxImportBytes)
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "导入文件不能超过 " + UploadPolicy.ToMbLabel(UploadPolicy.MaxImportBytes) + "！", BackURL, 2);
                        return;
                    }

                    string name = FileUpload1.PostedFile.FileName;                  // 客户端文件路径
                    FileInfo file = new FileInfo(name);
                    if (file.Name.IndexOf('.') > 0)
                    {
                        string[] aa = file.Name.Split('.');
                        if (aa[1].ToString() != "xlsx" && aa[1].ToString() != "xls")
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "提示：文件类型不符!", BackURL, 0);
                        }
                        else
                        {
                            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + aa[1];    // 文件名称
                            string webFilePath = Server.MapPath("Excel/" + fileName);        // 服务器端文件路径
                            if (!File.Exists(webFilePath))
                            {
                                try
                                {
                                    FileUpload1.SaveAs(webFilePath);
                                    DataTable dt = CommonFunc.ExcelDataSource(webFilePath, "Sheet0$");
                                    file_name = file.Name;
                                    ParameterizedThreadStart pts = new ParameterizedThreadStart(SayIntegrateAdd);
                                    Thread td2 = new Thread(pts);
                                    td2.Start(dt);
                                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "导入批量发送积分任务已创建，请查看进度", BackURL, 0);
                                }
                                catch (Exception exs)
                                {
                                    ImportDataLog.WriteLog(LogType.Error, exs.Message + "-" + exs.StackTrace);
                                }
                            }
                            else
                            {
                                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "上传失败：文件已经存在，请重命名后上传!", BackURL, 2);
                            }
                        }
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "请上传正确的文件!", BackURL, 2);
                    }
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "请选择您要上传的文件!", BackURL, 2);
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
        }
        protected void SayIntegrateAdd(object dt_daoru)
        {
            int sumnum = 0;
            int countt = 0;
            bool isyes_add = true;
            DataTable dt = Function.ConvertTo<DataTable>(dt_daoru, new DataTable());
            if (dt != null && dt.Rows.Count > 0)
            {
                daoru_list daoru_list = new daoru_list();
                daoru_list.posttime = DateTime.Now;
                daoru_list.status = 0;
                daoru_list.type = type;
                daoru_list.r_info = "创建导入批量发送积分任务:共" + dt.Rows.Count + "条数据";
                int addid = Function.ConvertTo<int>(daoru_listbll.AddIdentity(daoru_list, "id"), 0);
                if (addid > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        bool isyes = true;
                        sumnum++;
                        try
                        {
                            int user_id = 0;
                            string user_tel_ = Function.HtmlEncode(item["电话"].ToString().Trim());
                            int num_integrate_ = Function.ConvertTo<int>(item["积分"].ToString().Trim(), 0);
                            string about_ = Function.HtmlEncode(item["简介"].ToString().Trim());

                            string err_str = string.Empty;
                            if (string.IsNullOrWhiteSpace(user_tel_))
                            {
                                err_str += "<br/>【电话:" + user_tel_ + "】不能为空";
                                isyes = false;
                            }
                            else
                            {
                                user_list user_list = user_listbll.SelectSingle("tel='" + user_tel_ + "'");
                                if (user_list != null && user_list.id > 0)
                                {
                                    if (user_list.isshow == 1)
                                    {
                                        user_id = user_list.id;
                                    }
                                    else
                                    {
                                        err_str += "<br/>【电话:" + user_tel_ + "】用户状态异常";
                                        isyes = false;
                                    }
                                }
                                else
                                {
                                    err_str += "<br/>【电话:" + user_tel_ + "】未找到相关用户信息";
                                    isyes = false;
                                }
                            }

                            if (num_integrate_ > 0 || num_integrate_ < 0)
                            {

                            }
                            else
                            {
                                err_str += "<br/>【积分输入不正确】未找到相关用户信息";
                                isyes = false;
                            }

                            if (!isyes)
                            {
                                ErrList("第" + sumnum + "行" + err_str, addid);
                                isyes_add = false;
                                continue;
                            }
                            else
                            {
                                integrateLog_list integrateLog_list = new integrateLog_list();
                                integrateLog_list.num_integrate = num_integrate_;
                                integrateLog_list.type = 3;
                                integrateLog_list.name = "系统录入";
                                integrateLog_list.info_ = about_;
                                integrateLog_list.addtime = DateTime.Now;
                                integrateLog_list.adminname = AdminName;
                                integrateLog_list.user_id = user_id;
                                if (integrateLog_listbll.Add(integrateLog_list, "id") > 0)
                                {
                                    countt++;
                                }
                                else
                                {
                                    ErrList("第" + sumnum + "行【电话:" + user_tel_ + "】导入发送积分失败！", addid);
                                    isyes_add = false;
                                    continue;
                                }
                            }
                        }
                        catch (Exception ex_err)
                        {
                            ErrList("第" + sumnum + "行，异常：" + ex_err.Message, addid);
                            isyes_add = false;
                            continue;
                        }

                    }


                    if (isyes_add)
                    {
                        try
                        {
                            daoru_listbll.Update("r_info='" + Function.HtmlEncode("导入批量发送积分文件上传成功，共 " + sumnum + " 条数据成功插入 " + countt + " 条数据") + "',status=1", "id=" + addid);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    else
                    {
                        try
                        {
                            daoru_listbll.Update("r_info='" + Function.HtmlEncode("导入批量发送积分文件上传成功，共 " + sumnum + " 条数据成功插入 " + countt + " 条数据") + "',status=-1", "id=" + addid);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
                else
                {
                    ImportDataLog.WriteLog(LogType.Error, "创建导入批量发送积分任务失败");
                }
            }
        }
    }

}
