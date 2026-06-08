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
    public partial class Admin_IndexSingleList : System.Web.UI.Page
    {
        BLLBase<indexsingle_list> indexsingle_listbll = new BLLBase<indexsingle_list>();
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
            del_upload_pic_img_0.Checked = true;
            del_upload_pic_pc_0.Checked = true;
            AddUp.Visible = true;
            Main.Visible = false;
            Txt_Title.Text = "<font color=\"red\">添加数据</font>";
        }

        protected void EditFunc()
        {
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            indexsingle_list indexsingle_list = indexsingle_listbll.SelectSingle("id=" + ID);
            if (indexsingle_list != null && indexsingle_list.id > 0)
            {
                del_upload_pic_img_0.Checked = true;
                del_upload_pic_pc_0.Checked = true;
                AddUp.Visible = true;
                Main.Visible = false;
                Txt_Title.Text = "<font color=\"red\">《" + Function.HtmlDiscode(indexsingle_list.name) + "》详情</font>";
                if (!string.IsNullOrWhiteSpace(indexsingle_list.name))
                {
                    name.Text = Function.HtmlDiscode(indexsingle_list.name);
                }
                if (indexsingle_list.istype == 1)
                {
                    istype1.Checked = true;
                }
                else if (indexsingle_list.istype == 2)
                {
                    istype2.Checked = true;
                }
                if (!string.IsNullOrWhiteSpace(indexsingle_list.url))
                {
                    url.Text = Function.HtmlDiscode(indexsingle_list.url);
                }
                if (!string.IsNullOrWhiteSpace(indexsingle_list.description))
                {
                    description.Text = Function.HtmlDiscode(indexsingle_list.description);
                }
                if (!string.IsNullOrWhiteSpace(indexsingle_list.info_))
                {
                    info_.Text = Function.HtmlSqlDiscode(indexsingle_list.info_);
                }
                if (!string.IsNullOrWhiteSpace(indexsingle_list.upload_pic_img))
                {
                    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/A_UpLoad/upload_pic/" + indexsingle_list.upload_pic_img));
                    if (file.Exists)
                    {
                        upload_pic_img_Old.Value = Function.HtmlDiscode(indexsingle_list.upload_pic_img);
                        upload_pic_img_img.ImageUrl = Function.GetAdminUpload_Pic(indexsingle_list.upload_pic_img);
                    }
                    else
                    {
                        upload_pic_img_img.ImageUrl = "/admin/images/nophoto.gif";
                    }
                }
                else
                {
                    upload_pic_img_img.ImageUrl = "/admin/images/nophoto.gif";
                }

                if (!string.IsNullOrWhiteSpace(indexsingle_list.upload_pic_pc))
                {
                    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/A_UpLoad/upload_pic/" + indexsingle_list.upload_pic_pc));
                    if (file.Exists)
                    {
                        upload_pic_pc_Old.Value = Function.HtmlDiscode(indexsingle_list.upload_pic_pc);
                        upload_pic_pc_img.ImageUrl = Function.GetAdminUpload_Pic(indexsingle_list.upload_pic_pc);
                    }
                    else
                    {
                        upload_pic_pc_img.ImageUrl = "/admin/images/nophoto.gif";
                    }
                }
                else
                {
                    upload_pic_pc_img.ImageUrl = "/admin/images/nophoto.gif";
                }
            }

        }


        protected void DelFunc()
        {
            isLoading = false;
            AddUp.Visible = false;
            Main.Visible = false;
            string BackURL = Request.QueryString["BackURL"];
            if (string.IsNullOrWhiteSpace(BackURL))
            {
                BackURL = "Admin_IndexSingleList.aspx?MenuId=" + MenuId;
            }

            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            indexsingle_list indexsingle_list = indexsingle_listbll.SelectSingle("id=" + ID);
            if (indexsingle_list != null && indexsingle_list.id > 0)
            {
                AddUp.Visible = false;
                Main.Visible = false;
                if (indexsingle_listbll.Delete("id", ID))
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(indexsingle_list.upload_pic_img))
                        {
                            Function.FileDelete("/A_UpLoad/upload_pic/" + indexsingle_list.upload_pic_img + "");
                        }

                    }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
                    catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
                    {


                    }
#pragma warning disable CS0168 // 声明了变量，但从未使用过
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(indexsingle_list.upload_pic_pc))
                        {
                            Function.FileDelete("/A_UpLoad/upload_pic/" + indexsingle_list.upload_pic_pc + "");
                        }

                    }
                    catch (Exception ex)
                    {


                    }
#pragma warning restore CS0168 // 声明了变量，但从未使用过
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "数据《" + Function.HtmlDiscode(indexsingle_list.name) + "》删除成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "数据《" + Function.HtmlDiscode(indexsingle_list.name) + "》删除失败!", BackURL, 2);
                }
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "获取删除的参数错误!", BackURL, 1);
            }

        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        protected void BindData()
        {
            string Condition = " 1=1";
            string SearchKeyWords_str = Function.GetRequest("SearchKeyWords");
            if (!string.IsNullOrWhiteSpace(SearchKeyWords_str))
            {
                Condition += " and name like'%" + Function.HtmlEncode(SearchKeyWords_str) + "%'";
                SearchKeyWords.Text = SearchKeyWords_str;
            }

            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "indexsingle_list";
            //需要返回的列
            string strGetFields = " id,name,upload_pic_img,uptime,isshow,istop,orderid";
            //排序的字段名
            string fldname = " orderid desc,uptime desc,id desc";
            //每页显示的记录数

            AspNetPager1.PageSize = 15;
            int page_Size = this.AspNetPager1.PageSize;
            //统计总记录数
            int intRecordCount = indexsingle_listbll.GetCount(tblName, strWhere);
            if (intRecordCount > 0)
            {
                DivNull.Visible = false;
            }
            DataTable dt = indexsingle_listbll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, strWhere);
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
            if (string.IsNullOrWhiteSpace(BackURL))
            {
                BackURL = "Admin_IndexSingleList.aspx?MenuId=" + MenuId;
            }
            indexsingle_list indexsingle_list = new indexsingle_list();
            if (Action == "Edit")
            {
                indexsingle_list = indexsingle_listbll.SelectSingle("id=" + ID);
                if (!(indexsingle_list != null && indexsingle_list.id > 0))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "请求参数错误！", BackURL, 2);
                }
            }
            indexsingle_list.name = Function.HtmlEncode(Function.FormRequest("name"));
            indexsingle_list.istype = Function.ConvertTo<int>(Function.FormRequest("istype"), 0);
            indexsingle_list.url = Function.HtmlEncode(Function.FormRequest("url"));
            indexsingle_list.info_ = Function.HtmlSqlEncode(Function.FormRequest("info_"));
            indexsingle_list.description = Function.HtmlEncode(Function.FormRequest("description"));
            indexsingle_list.uptime = DateTime.Now;

            string upload_pic_img_Old = Function.FormRequest("upload_pic_img_Old");
            bool isyes_img_img = false;
            bool del_img_img = false;
            string file_img_img = "";
            try
            {
                int delbtn = Function.ConvertTo<int>(Function.FormRequest("del_upload_pic_img"), 0);
                if (delbtn == 1)
                {
                    del_img_img = true;
                    isyes_img_img = true;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(upload_pic_img.PostedFile.FileName))
                    {
                        if (upload_pic_img.PostedFile.ContentLength > UploadPolicy.MaxImageBytes)
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "图片不能超过 " + UploadPolicy.ToMbLabel(UploadPolicy.MaxImageBytes) + "！", Request.RawUrl, 2);
                            return;
                        }
                        Stream stream = upload_pic_img.PostedFile.InputStream;
                        string imgurl = upload_pic_img.PostedFile.FileName;
                        string ymd = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + "_1" + Path.GetExtension(imgurl).ToLower();

                        string path = DateTime.Now.ToString("yyyyMMdd") + "/";
                        string dirPath = Server.MapPath(@"/A_UpLoad/upload_pic/");
                        if (!Directory.Exists(dirPath + path))
                        {
                            Directory.CreateDirectory(dirPath + path);
                        }
                        file_img_img = path + ymd;
                        string savePath = dirPath + file_img_img;
                        upload_pic_img.SaveAs(savePath);
                        isyes_img_img = true;
                    }
                    else
                    {

                        isyes_img_img = true;
                    }
                }

            }
            catch (Exception ex)
            {

                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }

            if (isyes_img_img && !string.IsNullOrWhiteSpace(file_img_img))
            {
                indexsingle_list.upload_pic_img = file_img_img;
            }
            else if (del_img_img && isyes_img_img)
            {
                indexsingle_list.upload_pic_img = "";
            }

            string upload_pic_pc_Old = Function.FormRequest("upload_pic_pc_Old");
            bool isyes_pc_img = false;
            bool del_pc_img = false;
            string file_pc_img = "";
            try
            {
                int delbtn = Function.ConvertTo<int>(Function.FormRequest("del_upload_pic_pc"), 0);
                if (delbtn == 1)
                {
                    del_pc_img = true;
                    isyes_pc_img = true;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(upload_pic_pc.PostedFile.FileName))
                    {
                        if (upload_pic_pc.PostedFile.ContentLength > UploadPolicy.MaxImageBytes)
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "图片不能超过 " + UploadPolicy.ToMbLabel(UploadPolicy.MaxImageBytes) + "！", Request.RawUrl, 2);
                            return;
                        }
                        Stream stream = upload_pic_pc.PostedFile.InputStream;
                        string imgurl = upload_pic_pc.PostedFile.FileName;
                        string ymd = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + "_3" + Path.GetExtension(imgurl).ToLower();

                        string path = DateTime.Now.ToString("yyyyMMdd") + "/";
                        string dirPath = Server.MapPath(@"/A_UpLoad/upload_pic/");
                        if (!Directory.Exists(dirPath + path))
                        {
                            Directory.CreateDirectory(dirPath + path);
                        }
                        file_pc_img = path + ymd;
                        string savePath = dirPath + file_pc_img;
                        upload_pic_pc.SaveAs(savePath);
                        isyes_pc_img = true;
                    }
                    else
                    {

                        isyes_pc_img = true;
                    }
                }

            }
            catch (Exception ex)
            {

                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }

            if (isyes_pc_img && !string.IsNullOrWhiteSpace(file_pc_img))
            {
                indexsingle_list.upload_pic_pc = file_pc_img;
            }
            else if (del_pc_img && isyes_pc_img)
            {
                indexsingle_list.upload_pic_pc = "";
            }

            if (Action == "Add")
            {
                AddUp.Visible = false;
                int orderint = 0;
                DataTable orderdt = indexsingle_listbll.GetDatatable("select max(orderid) as num from indexsingle_list");
                if (orderdt != null && orderdt.Rows.Count > 0)
                {
                    orderint = Function.ConvertTo<int>(orderdt.Rows[0]["num"].ToString(), 0);
                }
                orderdt.Dispose();
                orderint++;
                indexsingle_list.orderid = orderint;
                indexsingle_list.isshow = 1;
                indexsingle_list.addtime = DateTime.Now;
                if (indexsingle_listbll.Add(indexsingle_list, "id") > 0)
                {
                    try
                    {
                        if (del_pc_img)
                        {
                            if (upload_pic_pc_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("../A_UpLoad/upload_pic/" + upload_pic_pc_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    try
                    {
                        if (del_img_img)
                        {
                            if (upload_pic_img_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + upload_pic_img_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    if (isyes_img_img && isyes_pc_img)
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "数据《<font color=\"red\">" + Function.HtmlDiscode(indexsingle_list.name) + "</font>》 添加成功!", BackURL, 0);
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "数据《<font color=\"red\">" + Function.HtmlDiscode(indexsingle_list.name) + "</font>》 添加成功,但图片上传失败，请重新编辑信息!", BackURL, 0);
                    }
                }
                else
                {
                    try
                    {
                        if (del_pc_img)
                        {
                            if (upload_pic_pc_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + upload_pic_pc_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(indexsingle_list.upload_pic_pc))
                        {
                            try
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + indexsingle_list.upload_pic_pc + "");
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
                    try
                    {
                        if (del_img_img)
                        {
                            if (upload_pic_img_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + upload_pic_img_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(indexsingle_list.upload_pic_img))
                        {
                            try
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + indexsingle_list.upload_pic_img + "");
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
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "数据《<font color=\"red\">" + Function.HtmlDiscode(indexsingle_list.name) + "</font>》 添加失败!", BackURL, 2);

                }
            }
            else if (Action == "Edit")
            {
                AddUp.Visible = false;
                string[] file = { "id" };
                if (indexsingle_listbll.Update(file, indexsingle_list))
                {
                    try
                    {
                        if (del_pc_img)
                        {
                            if (upload_pic_pc_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + upload_pic_pc_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    try
                    {
                        if (indexsingle_list.upload_pic_pc != upload_pic_pc_Old)
                        {

                            if (upload_pic_pc_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + upload_pic_pc_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    try
                    {
                        if (del_img_img)
                        {
                            if (upload_pic_img_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + upload_pic_img_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    try
                    {
                        if (indexsingle_list.upload_pic_img != upload_pic_img_Old)
                        {

                            if (upload_pic_img_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + upload_pic_img_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    if (isyes_img_img && isyes_pc_img)
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "数据《<font color=\"red\">" + Function.HtmlDiscode(indexsingle_list.name) + "</font>》 修改成功!", BackURL, 0);
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "数据《<font color=\"red\">" + Function.HtmlDiscode(indexsingle_list.name) + "</font>》 修改成功,但是图片上传失败!", BackURL, 0);
                    }
                }
                else
                {
                    try
                    {
                        if (del_pc_img)
                        {
                            if (upload_pic_pc_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + upload_pic_pc_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    try
                    {
                        if (indexsingle_list.upload_pic_pc != upload_pic_pc_Old)
                        {

                            if (indexsingle_list.upload_pic_pc.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + indexsingle_list.upload_pic_pc + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    try
                    {
                        if (del_img_img)
                        {
                            if (upload_pic_img_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + upload_pic_img_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    try
                    {
                        if (indexsingle_list.upload_pic_img != upload_pic_img_Old)
                        {

                            if (indexsingle_list.upload_pic_img.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + indexsingle_list.upload_pic_img + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "数据《<font color=\"red\">" + Function.HtmlDiscode(indexsingle_list.name) + "</font>》 修改失败!", BackURL, 2);
                }
            }
        }

        protected void OnClick_Search(object sender, EventArgs e)
        {
            Response.Redirect(Request.CurrentExecutionFilePath + "?MenuId=" + MenuId + "&SearchKeyWords=" + Server.UrlEncode(Function.FormRequest("SearchKeyWords")) + "");
        }
    }

}
