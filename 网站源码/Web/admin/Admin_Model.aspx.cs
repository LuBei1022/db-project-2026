using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;

namespace Web.admin
{
    public partial class Admin_Model : System.Web.UI.Page
    {
        BLLBase<model_list> portalTemplateBll = new BLLBase<model_list>();
        string Action = Function.GetRequest("Action");
        public int MenuId = Function.ConvertTo<int>(Function.GetRequest("MenuId"), 0);
        public bool isLoading = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            switch (Action)
            {
                case "Add":
                    AddFunc();
                    break;
                case "Edit":
                    EditFunc();
                    break;
                case "Del":
                    DelFunc();
                    break;
                default:
                    BindData();
                    break;
            }
        }

        protected void AddFunc()
        {
            AddUp.Visible = true;
            Main.Visible = false;
            del_upload_pic_0.Checked = true;
            Txt_Title.Text = "添加门户模板";
        }
        protected void DelFunc()
        {
            isLoading = false;
            AddUp.Visible = false;
            Main.Visible = false;
            string BackURL = Request.QueryString["BackURL"];
            if (string.IsNullOrWhiteSpace(BackURL))
            {
                BackURL = "Admin_Model.aspx?MenuId=" + MenuId;
            }
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            model_list portalTemplate = portalTemplateBll.SelectSingle("id=" + ID);
            if (portalTemplate != null && portalTemplate.id > 0)
            {
                if (portalTemplateBll.Delete("id", ID))
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(portalTemplate.upload_pic))
                        {
                            Function.FileDelete("../A_UpLoad/upload_pic/" + portalTemplate.upload_pic + "");
                        }

                    }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
                    catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
                    {


                    }
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "门户模板《" + Function.HtmlDiscodeWeb(portalTemplate.m_name) + "》删除成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "门户模板《" + Function.HtmlDiscodeWeb(portalTemplate.m_name) + "》删除失败!", BackURL, 2);
                }
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "获取删除的参数错误!", BackURL, 1);
            }

        }

        protected void EditFunc()
        {
            Main.Visible = false;
            Txt_Title.Text = "修改门户模板";
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            model_list portalTemplate = portalTemplateBll.SelectSingle("id=" + ID);
            if (portalTemplate != null && portalTemplate.id > 0)
            {
                AddUp.Visible = true;
                del_upload_pic_0.Checked = true;
                if (!string.IsNullOrWhiteSpace(portalTemplate.m_name))
                {
                    m_name.Text = Function.HtmlDiscode(portalTemplate.m_name);
                }
                if (!string.IsNullOrWhiteSpace(portalTemplate.m_url))
                {
                    m_url.Text = Function.HtmlSqlDiscode(portalTemplate.m_url);
                }
                if (!string.IsNullOrWhiteSpace(portalTemplate.page_url))
                {
                    page_url.Text = Function.HtmlSqlDiscode(portalTemplate.page_url);
                }
                if (!string.IsNullOrWhiteSpace(portalTemplate.upload_pic))
                {
                    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath("../A_UpLoad/upload_pic/" + portalTemplate.upload_pic));
                    if (file.Exists)
                    {
                        upload_pic_Old.Value = Function.HtmlDiscode(portalTemplate.upload_pic);
                        upload_pic_img.ImageUrl = Function.GetAdminUpload_Pic(portalTemplate.upload_pic);
                    }
                    else
                    {
                        upload_pic_img.ImageUrl = "images/nophoto.gif";
                    }
                }
                else
                {
                    upload_pic_img.ImageUrl = "images/nophoto.gif";
                }
            }
        }


        /// <summary>
        /// 绑定数据
        /// </summary>
        protected void BindData()
        {
            string Condition = "1=1";

            string SearchKeyName_str = Function.GetRequest("SearchKeyName");
            if (!string.IsNullOrWhiteSpace(SearchKeyName_str))
            {
                Condition += " and  name like N'%" + Function.HtmlEncode(SearchKeyName_str) + "%'";
                SearchKeyName.Text = SearchKeyName_str;
            }


            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "model_list";
            //需要返回的列
            string strGetFields = "RANK()  OVER (order by orderid asc,id asc) AS xuhao,id, m_name, m_url, page_url, orderid, upload_pic";
            //排序的字段名
            string fldname = " orderid desc,id desc";
            //每页显示的记录数
            int page_Size = this.AspNetPager1.PageSize;
            //统计总记录数
            int intRecordCount = portalTemplateBll.GetCount(tblName, strWhere);
            if (intRecordCount > 0)
            {
                DivNull.Visible = false;
            }
            DataTable dt = portalTemplateBll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, strWhere);
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
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            string BackURL = Request.QueryString["BackURL"];
            model_list portalTemplate = new model_list();
            string templateName = Function.HtmlEncode(Function.FormRequest("m_name"));
            if (Action == "Add")
            {
                model_list existedTemplate = portalTemplateBll.SelectSingle("m_name='" + templateName + "'");
                if (existedTemplate != null && existedTemplate.id > 0)
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "门户模板名称《<font color=\"red\">" + Function.HtmlDiscode(templateName) + "</font>》已存在!", BackURL, 2);
                }
            }
            else if (Action == "Edit")
            {
                portalTemplate = portalTemplateBll.SelectSingle("id=" + ID);
                if (portalTemplate != null && portalTemplate.id > 0)
                {
                    model_list existedTemplate = portalTemplateBll.SelectSingle("m_name='" + templateName + "' and id not in(" + portalTemplate.id + ")");
                    if (existedTemplate != null && existedTemplate.id > 0)
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "门户模板名称《<font color=\"red\">" + Function.HtmlDiscode(templateName) + "</font>》已存在!", BackURL, 2);
                    }
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "请求参数错误！", BackURL, 2);
                }
            }
            portalTemplate.m_name = templateName;
            portalTemplate.m_url = Function.HtmlEncode(Function.FormRequest("m_url"));
            portalTemplate.page_url = Function.HtmlEncode(Function.FormRequest("page_url"));

            string upload_pic_Old = Function.FormRequest("upload_pic_Old");

            bool isyes_img = false;
            bool del_img = false;
            string file_img = "";
            try
            {
                int delbtn = Function.ConvertTo<int>(Function.FormRequest("del_upload_pic"), 0);
                if (delbtn == 1)
                {
                    del_img = true;
                    isyes_img = true;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(upload_pic.PostedFile.FileName))
                    {
                        if (upload_pic.PostedFile.ContentLength > UploadPolicy.MaxImageBytes)
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "图片不能超过 " + UploadPolicy.ToMbLabel(UploadPolicy.MaxImageBytes) + "！", Request.RawUrl, 2);
                            return;
                        }
                        Stream stream = upload_pic.PostedFile.InputStream;
                        string imgurl = upload_pic.PostedFile.FileName;
                        string ymd = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + "_2" + Path.GetExtension(imgurl).ToLower();

                        string path = DateTime.Now.ToString("yyyyMMdd") + "/";
                        string dirPath = Server.MapPath(@"../A_UpLoad/upload_pic/");
                        if (!Directory.Exists(dirPath + path))
                        {
                            Directory.CreateDirectory(dirPath + path);
                        }
                        file_img = path + ymd;
                        string savePath = dirPath + file_img;
                        upload_pic.SaveAs(savePath);
                        isyes_img = true;
                    }
                    else
                    {

                        isyes_img = true;
                    }
                }

            }
            catch (Exception ex)
            {

                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }

            if (isyes_img && !string.IsNullOrWhiteSpace(file_img))
            {
                portalTemplate.upload_pic = file_img;
            }
            else if (del_img && isyes_img)
            {
                portalTemplate.upload_pic = "";
            }

            if (Action == "Add")
            {
                AddUp.Visible = false;
                int orderint = 0;
                DataTable orderdt = portalTemplateBll.GetDatatable("select max(orderid) as num from model_list where 1=1");
                if (orderdt != null && orderdt.Rows.Count > 0)
                {
                    orderint = Function.ConvertTo<int>(orderdt.Rows[0]["num"].ToString(), 0);
                }
                orderdt.Dispose();
                orderint++;
                portalTemplate.orderid = orderint;
                if (portalTemplateBll.Add(portalTemplate, "id") > 0)
                {
                    try
                    {
                        if (del_img)
                        {
                            if (upload_pic_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("../A_UpLoad/upload_pic/" + upload_pic_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    if (isyes_img)
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "门户模板《<font color=\"red\">" + Function.HtmlDiscode(portalTemplate.m_name) + "</font>》添加成功!", BackURL, 0);
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "门户模板《<font color=\"red\">" + Function.HtmlDiscode(portalTemplate.m_name) + "</font>》添加成功，但图片上传失败，请重新编辑信息!", BackURL, 0);
                    }
                }
                else
                {
                    try
                    {
                        if (del_img)
                        {
                            if (upload_pic_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("../A_UpLoad/upload_pic/" + upload_pic_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(portalTemplate.upload_pic))
                        {
                            try
                            {
                                Function.FileDelete("../A_UpLoad/upload_pic/" + portalTemplate.upload_pic + "");
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "门户模板《<font color=\"red\">" + Function.HtmlDiscode(portalTemplate.m_name) + "</font>》添加失败!", BackURL, 2);

                }
            }
            else if (Action == "Edit")
            {
                AddUp.Visible = false;
                string[] file = { "id" };
                if (portalTemplateBll.Update(file, portalTemplate))
                {
                    try
                    {
                        if (del_img)
                        {
                            if (upload_pic_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("../A_UpLoad/upload_pic/" + upload_pic_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    try
                    {
                        if (portalTemplate.upload_pic != upload_pic_Old)
                        {

                            if (upload_pic_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("../A_UpLoad/upload_pic/" + upload_pic_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    if (isyes_img)
                    {

                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "门户模板《<font color=\"red\">" + Function.HtmlDiscode(portalTemplate.m_name) + "</font>》修改成功!", BackURL, 0);
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "门户模板《<font color=\"red\">" + Function.HtmlDiscode(portalTemplate.m_name) + "</font>》修改成功，但图片上传失败，请重新编辑信息!", BackURL, 0);
                    }
                }
                else
                {
                    try
                    {
                        if (del_img)
                        {
                            if (upload_pic_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("../A_UpLoad/upload_pic/" + upload_pic_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    try
                    {
                        if (portalTemplate.upload_pic != upload_pic_Old)
                        {

                            if (portalTemplate.upload_pic.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("../A_UpLoad/upload_pic/" + portalTemplate.upload_pic + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "门户模板《<font color=\"red\">" + Function.HtmlDiscode(portalTemplate.m_name) + "</font>》修改失败!", BackURL, 2);
                }
            }
        }

        protected void OnClick_Search(object sender, EventArgs e)
        {
            string str = string.Empty;


            string SearchKeyName_str = Function.FormRequest("SearchKeyName");
            if (!string.IsNullOrWhiteSpace(SearchKeyName_str))
            {
                str += "&SearchKeyName=" + Server.UrlEncode(SearchKeyName_str);
            }

            Response.Redirect(Request.CurrentExecutionFilePath + "?btn=search&MenuId=" + MenuId + str);
        }
    }
}
