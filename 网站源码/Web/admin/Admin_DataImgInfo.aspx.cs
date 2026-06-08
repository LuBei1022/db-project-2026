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
    public partial class Admin_DataImgInfo : System.Web.UI.Page
    {
        BLLBase<tbl_class> tbl_classbll = new BLLBase<tbl_class>();
        BLLBase<data_list> data_listbll = new BLLBase<data_list>();
        public int tbclass_id = Function.ConvertTo<int>(Function.GetRequest("tbclass_id"), 0);
        string Action = Function.GetRequest("Action");
        public int MenuId = Function.ConvertTo<int>(Function.GetRequest("MenuId"), 0);
        public bool isLoading = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            tbl_class tbl_class = tbl_classbll.SelectSingle("id=" + tbclass_id + " and model=2");
            if (tbl_class != null && tbl_class.id > 0)
            {
                isLoading = true;
                tbclass_name.Text = Function.HtmlDiscode(tbl_class.classname);
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
            else
            {
                Function.Show_Msg("访问路劲异常！", "Admin_Null.aspx");
            }
        }
        protected void AddFunc()
        {
            del_upload_pic_img_0.Checked = true;
            AddUp.Visible = true;
            Main.Visible = false;
            Txt_Title.Text = "<font color=\"red\">添加数据</font>";
        }

        protected void EditFunc()
        {
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            data_list data_list = data_listbll.SelectSingle("id=" + ID + " and tbclass_id=" + tbclass_id);
            if (data_list != null && data_list.id > 0)
            {
                del_upload_pic_img_0.Checked = true;
                AddUp.Visible = true;
                Main.Visible = false;
                Txt_Title.Text = "<font color=\"red\">《" + Function.HtmlDiscode(data_list.name) + "》详情</font>";
                datetime.Text = data_list.datetime.ToString("yyyy-MM-dd HH:mm");
                if (!string.IsNullOrWhiteSpace(data_list.name))
                {
                    name.Text = Function.HtmlDiscode(data_list.name);
                }


                if (!string.IsNullOrWhiteSpace(data_list.description))
                {
                    description.Text = Function.HtmlDiscode(data_list.description);
                }
                datetime.Text = data_list.datetime.ToString("yyyy-MM-dd");
                if (!string.IsNullOrWhiteSpace(data_list.info_))
                {
                    info_.Text = Function.HtmlSqlDiscode(data_list.info_);
                }
                if (!string.IsNullOrWhiteSpace(data_list.upload_pic_img))
                {
                    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/A_UpLoad/upload_pic/" + data_list.upload_pic_img));
                    if (file.Exists)
                    {
                        upload_pic_img_Old.Value = Function.HtmlDiscode(data_list.upload_pic_img);
                        upload_pic_img_img.ImageUrl = Function.GetAdminUpload_Pic(data_list.upload_pic_img);
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
                BackURL = "Admin_DataImgInfo.aspx?tbclass_id=" + tbclass_id + "&MenuId=" + MenuId;
            }

            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            data_list data_list = data_listbll.SelectSingle("id=" + ID + " and tbclass_id=" + tbclass_id);
            if (data_list != null && data_list.id > 0)
            {
                AddUp.Visible = false;
                Main.Visible = false;
                if (data_listbll.Delete("id", ID))
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(data_list.upload_pic_img))
                        {
                            Function.FileDelete("/A_UpLoad/upload_pic/" + data_list.upload_pic_img + "");
                        }

                    }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
                    catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
                    {


                    }
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "数据《" + Function.HtmlDiscode(data_list.name) + "》删除成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "数据《" + Function.HtmlDiscode(data_list.name) + "》删除失败!", BackURL, 2);
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
            string Condition = " tbclass_id=" + tbclass_id;
            string SearchKeyWords_str = Function.GetRequest("SearchKeyWords");
            if (!string.IsNullOrWhiteSpace(SearchKeyWords_str))
            {
                Condition += " and name like'%" + Function.HtmlEncode(SearchKeyWords_str) + "%'";
                SearchKeyWords.Text = SearchKeyWords_str;
            }

            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "data_list";
            //需要返回的列
            string strGetFields = " id,name,tbclass_id,upload_pic_img,uptime,datetime,isshow,istop,orderid";
            //排序的字段名
            string fldname = " orderid desc,uptime desc,id desc";
            //每页显示的记录数

            AspNetPager1.PageSize = 15;
            int page_Size = this.AspNetPager1.PageSize;
            //统计总记录数
            int intRecordCount = data_listbll.GetCount(tblName, strWhere);
            if (intRecordCount > 0)
            {
                DivNull.Visible = false;
            }
            DataTable dt = data_listbll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, strWhere);
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
                BackURL = "Admin_DataImgInfo.aspx?tbclass_id=" + tbclass_id + "&MenuId=" + MenuId;
            }
            data_list data_list = new data_list();
            if (Action == "Edit")
            {
                data_list = data_listbll.SelectSingle("id=" + ID + " and tbclass_id=" + tbclass_id);
                if (!(data_list != null && data_list.id > 0))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "请求参数错误！", BackURL, 2);
                }
            }
            data_list.name = Function.HtmlEncode(Function.FormRequest("name"));
            data_list.info_ = Function.HtmlSqlEncode(Function.FormRequest("info_"));
            data_list.description = Function.HtmlEncode(Function.FormRequest("description"));
            data_list.datetime = Function.ConvertTo<DateTime>(Function.FormRequest("datetime"), DateTime.Now);
            data_list.uptime = DateTime.Now;

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
                data_list.upload_pic_img = file_img_img;
            }
            else if (del_img_img && isyes_img_img)
            {
                data_list.upload_pic_img = "";
            }


            if (Action == "Add")
            {
                AddUp.Visible = false;
                int orderint = 0;
                DataTable orderdt = data_listbll.GetDatatable("select max(orderid) as num from data_list where tbclass_id=" + tbclass_id);
                if (orderdt != null && orderdt.Rows.Count > 0)
                {
                    orderint = Function.ConvertTo<int>(orderdt.Rows[0]["num"].ToString(), 0);
                }
                orderdt.Dispose();
                orderint++;
                data_list.orderid = orderint;
                data_list.isshow = 1;
                data_list.tbclass_id = tbclass_id;
                data_list.addtime = DateTime.Now;
                if (data_listbll.Add(data_list, "id") > 0)
                {
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
                    if (isyes_img_img)
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "数据《<font color=\"red\">" + Function.HtmlDiscode(data_list.name) + "</font>》 添加成功!", BackURL, 0);
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "数据《<font color=\"red\">" + Function.HtmlDiscode(data_list.name) + "</font>》 添加成功,但图片上传失败，请重新编辑信息!", BackURL, 0);
                    }
                }
                else
                {
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
                        if (!string.IsNullOrWhiteSpace(data_list.upload_pic_img))
                        {
                            try
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + data_list.upload_pic_img + "");
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
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "数据《<font color=\"red\">" + Function.HtmlDiscode(data_list.name) + "</font>》 添加失败!", BackURL, 2);

                }
            }
            else if (Action == "Edit")
            {
                AddUp.Visible = false;
                string[] file = { "id" };
                if (data_listbll.Update(file, data_list))
                {
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
                        if (data_list.upload_pic_img != upload_pic_img_Old)
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
                    if (isyes_img_img)
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "数据《<font color=\"red\">" + Function.HtmlDiscode(data_list.name) + "</font>》 修改成功!", BackURL, 0);
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "数据《<font color=\"red\">" + Function.HtmlDiscode(data_list.name) + "</font>》 修改成功,但是图片上传失败!", BackURL, 0);
                    }
                }
                else
                {
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
                        if (data_list.upload_pic_img != upload_pic_img_Old)
                        {

                            if (data_list.upload_pic_img.IndexOf("/") >= 0)//包含
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + data_list.upload_pic_img + "");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "数据《<font color=\"red\">" + Function.HtmlDiscode(data_list.name) + "</font>》 修改失败!", BackURL, 2);
                }
            }
        }

        protected void OnClick_Search(object sender, EventArgs e)
        {
            Response.Redirect(Request.CurrentExecutionFilePath + "?tbclass_id=" + tbclass_id + "&MenuId=" + MenuId + "&SearchKeyWords=" + Server.UrlEncode(Function.FormRequest("SearchKeyWords")) + "");
        }
    }

}
