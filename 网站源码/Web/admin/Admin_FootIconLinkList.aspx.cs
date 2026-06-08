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
    public partial class Admin_FootIconLinkList : System.Web.UI.Page
    {
        BLLBase<link_list> link_listbll = new BLLBase<link_list>();
        string Action = Function.GetRequest("Action");
        public int MenuId = Function.ConvertTo<int>(Function.GetRequest("MenuId"), 0);
        public bool isLoading = false;
        public int type = 1;
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
            del_upload_pic_icon_0.Checked = true;
            AddUp.Visible = true;
            Main.Visible = false;
            Txt_Title.Text = "<font color=\"red\">添加底部图标链接</font>";
        }

        protected void EditFunc()
        {
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            link_list link_list = link_listbll.SelectSingle("id=" + ID + " and type=" + type);
            if (link_list != null && link_list.id > 0)
            {
                del_upload_pic_icon_0.Checked = true;
                AddUp.Visible = true;
                Main.Visible = false;
                Txt_Title.Text = "<font color=\"red\">《" + Function.HtmlDiscode(link_list.name) + "》详情</font>";

                if (!string.IsNullOrWhiteSpace(link_list.name))
                {
                    name.Text = Function.HtmlDiscode(link_list.name);
                }
                if (!string.IsNullOrWhiteSpace(link_list.url))
                {
                    url.Text = Function.HtmlDiscode(link_list.url);
                }

                if (!string.IsNullOrWhiteSpace(link_list.upload_pic_icon))
                {
                    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/A_UpLoad/upload_pic/" + link_list.upload_pic_icon));
                    if (file.Exists)
                    {
                        upload_pic_icon_Old.Value = Function.HtmlDiscode(link_list.upload_pic_icon);
                        upload_pic_icon_img.ImageUrl = Function.GetAdminUpload_Pic(link_list.upload_pic_icon);
                    }
                    else
                    {
                        upload_pic_icon_img.ImageUrl = "/admin/images/nophoto.gif";
                    }
                }
                else
                {
                    upload_pic_icon_img.ImageUrl = "/admin/images/nophoto.gif";
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
                BackURL = "Admin_FootIconLinkList.aspx?MenuId=" + MenuId;
            }

            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            link_list link_list = link_listbll.SelectSingle("id=" + ID + " and type=" + type);
            if (link_list != null && link_list.id > 0)
            {
                if (link_listbll.Delete("id", ID))
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(link_list.upload_pic_icon))
                        {
                            Function.FileDelete("/A_UpLoad/upload_pic/" + link_list.upload_pic_icon + "");
                        }

                    }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
                    catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
                    {


                    }

                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "底部图标链接《" + Function.HtmlDiscode(link_list.name) + "》删除成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "底部图标链接《" + Function.HtmlDiscode(link_list.name) + "》删除失败!", BackURL, 2);
                }
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "获取删除的参数错误!", BackURL, 1);
            }

        }

        /// <summary>
        /// 绑定底部图标链接
        /// </summary>
        protected void BindData()
        {
            string Condition = " type=" + type;
            string SearchKeyWords_str = Function.GetRequest("SearchKeyWords");
            if (!string.IsNullOrWhiteSpace(SearchKeyWords_str))
            {
                Condition += " and name like'%" + Function.HtmlEncode(SearchKeyWords_str) + "%'";
                SearchKeyWords.Text = SearchKeyWords_str;
            }

            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "link_list";
            //需要返回的列
            string strGetFields = " id,name,upload_pic_icon,uptime,isshow,orderid,url";
            //排序的字段名
            string fldname = "orderid desc,uptime desc,id desc";
            //每页显示的记录数

            AspNetPager1.PageSize = 15;
            int page_Size = this.AspNetPager1.PageSize;
            //统计总记录数
            int intRecordCount = link_listbll.GetCount(tblName, strWhere);
            if (intRecordCount > 0)
            {
                DivNull.Visible = false;
            }
            DataTable dt = link_listbll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, strWhere);
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
                BackURL = "Admin_FootIconLinkList.aspx?MenuId=" + MenuId;
            }
            link_list link_list = new link_list();
            if (Action == "Edit")
            {
                link_list = link_listbll.SelectSingle("id=" + ID + " and type=" + type);
                if (!(link_list != null && link_list.id > 0))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "请求参数错误！", BackURL, 2);
                }
            }
            link_list.name = Function.HtmlEncode(Function.FormRequest("name"));
            link_list.url = Function.HtmlEncode(Function.FormRequest("url"));
            link_list.uptime = DateTime.Now;


            string upload_pic_icon_Old = Function.FormRequest("upload_pic_icon_Old");
            bool isyes_icon_img = false;
            bool del_icon_img = false;
            string file_icon_img = "";
            try
            {
                int delbtn = Function.ConvertTo<int>(Function.FormRequest("del_upload_pic_icon"), 0);
                if (delbtn == 1)
                {
                    del_icon_img = true;
                    isyes_icon_img = true;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(upload_pic_icon.PostedFile.FileName))
                    {
                        if (upload_pic_icon.PostedFile.ContentLength > UploadPolicy.MaxImageBytes)
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "图片不能超过 " + UploadPolicy.ToMbLabel(UploadPolicy.MaxImageBytes) + "！", Request.RawUrl, 2);
                            return;
                        }
                        Stream stream = upload_pic_icon.PostedFile.InputStream;
                        string imgurl = upload_pic_icon.PostedFile.FileName;
                        string ymd = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + "_2" + Path.GetExtension(imgurl).ToLower();

                        string path = DateTime.Now.ToString("yyyyMMdd") + "/";
                        string dirPath = Server.MapPath(@"/A_UpLoad/upload_pic/");
                        if (!Directory.Exists(dirPath + path))
                        {
                            Directory.CreateDirectory(dirPath + path);
                        }
                        file_icon_img = path + ymd;
                        string savePath = dirPath + file_icon_img;
                        upload_pic_icon.SaveAs(savePath);
                        isyes_icon_img = true;
                    }
                    else
                    {

                        isyes_icon_img = true;
                    }
                }

            }
            catch (Exception ex)
            {

                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }

            if (isyes_icon_img && !string.IsNullOrWhiteSpace(file_icon_img))
            {
                link_list.upload_pic_icon = file_icon_img;
            }
            else if (del_icon_img && isyes_icon_img)
            {
                link_list.upload_pic_icon = "";
            }


            if (Action == "Add")
            {
                AddUp.Visible = false;
                int orderint = 0;
                DataTable orderdt = link_listbll.GetDatatable("select max(orderid) as num from link_list where type=" + type);
                if (orderdt != null && orderdt.Rows.Count > 0)
                {
                    orderint = Function.ConvertTo<int>(orderdt.Rows[0]["num"].ToString(), 0);
                }
                orderdt.Dispose();
                orderint++;
                link_list.orderid = orderint;
                link_list.isshow = 1;
                link_list.addtime = DateTime.Now;
                link_list.type = type;
                if (link_listbll.Add(link_list, "id") > 0)
                {
                    try
                    {
                        if (del_icon_img)
                        {
                            if (upload_pic_icon_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + upload_pic_icon_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    if (isyes_icon_img)
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "底部图标链接《<font color=\"red\">" + Function.HtmlDiscode(link_list.name) + "</font>》 添加成功!", BackURL, 0);
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "底部图标链接《<font color=\"red\">" + Function.HtmlDiscode(link_list.name) + "</font>》 添加成功,但图片上传失败，请重新编辑信息!", BackURL, 0);
                    }
                }
                else
                {
                    try
                    {
                        if (del_icon_img)
                        {
                            if (upload_pic_icon_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + upload_pic_icon_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(link_list.upload_pic_icon))
                        {
                            try
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + link_list.upload_pic_icon + "");
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
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "底部图标链接《<font color=\"red\">" + Function.HtmlDiscode(link_list.name) + "</font>》 添加失败!", BackURL, 2);

                }
            }
            else if (Action == "Edit")
            {
                AddUp.Visible = false;
                string[] file = { "id" };
                if (link_listbll.Update(file, link_list))
                {
                    try
                    {
                        if (del_icon_img)
                        {
                            if (upload_pic_icon_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + upload_pic_icon_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    try
                    {
                        if (link_list.upload_pic_icon != upload_pic_icon_Old)
                        {

                            if (upload_pic_icon_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + upload_pic_icon_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "底部图标链接《<font color=\"red\">" + Function.HtmlDiscode(link_list.name) + "</font>》 修改成功!", BackURL, 0);
                }
                else
                {
                    try
                    {
                        if (del_icon_img)
                        {
                            if (upload_pic_icon_Old.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + upload_pic_icon_Old + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    try
                    {
                        if (link_list.upload_pic_icon != upload_pic_icon_Old)
                        {

                            if (link_list.upload_pic_icon.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + link_list.upload_pic_icon + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "底部图标链接《<font color=\"red\">" + Function.HtmlDiscode(link_list.name) + "</font>》 修改失败!", BackURL, 2);
                }
            }
        }

        protected void OnClick_Search(object sender, EventArgs e)
        {
            Response.Redirect(Request.CurrentExecutionFilePath + "?MenuId=" + MenuId + "&SearchKeyWords=" + Server.UrlEncode(Function.FormRequest("SearchKeyWords")) + "");
        }
    }

}
