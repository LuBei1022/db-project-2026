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
    public partial class Admin_IntegrateList : System.Web.UI.Page
    {
        BLLBase<integrate_list> integrate_listBll = new BLLBase<integrate_list>();
        string Action = Function.GetRequest("Action");
        public string MenuId = Function.GetRequest("MenuId");
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
            Txt_Title.Text = "<font color=\"red\">添加文献下载权益</font>";
        }

        protected void EditFunc()
        {
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            integrate_list integrate_list = integrate_listBll.SelectSingle("Id=" + ID);
            if (integrate_list != null && integrate_list.id > 0)
            {
                AddUp.Visible = true;
                Main.Visible = false;
                Txt_Title.Text = "<font color=\"red\">文献下载权益详情</font>";

                if (!string.IsNullOrWhiteSpace(integrate_list.name))
                {
                    name.Text = Function.HtmlDiscode(integrate_list.name);
                }
                if (!string.IsNullOrWhiteSpace(integrate_list.about_))
                {
                    about_.Text = Function.HtmlDiscode(integrate_list.about_);
                }
                num_integrate.Text = integrate_list.num_integrate.ToString();
                if (!string.IsNullOrWhiteSpace(integrate_list.upload_pic_img))
                {
                    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/A_UpLoad/upload_pic/" + integrate_list.upload_pic_img));
                    if (file.Exists)
                    {
                        upload_pic_img_old.Value = Function.HtmlDiscode(integrate_list.upload_pic_img);
                        upload_pic_img_img.ImageUrl = Function.GetAdminUpload_Pic(integrate_list.upload_pic_img);
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
                BackURL = "Admin_IntegrateList.aspx?MenuId=" + MenuId;
            }

            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            integrate_list integrate_list = integrate_listBll.SelectSingle("id=" + ID);
            if (integrate_list != null && integrate_list.id > 0)
            {
                AddUp.Visible = false;
                Main.Visible = false;
                if (integrate_listBll.Delete("Id", ID))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "文献下载权益《" + Function.HtmlDiscode(integrate_list.name) + "》删除成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "文献下载权益《" + Function.HtmlDiscode(integrate_list.name) + "》删除失败!", BackURL, 2);
                }
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "获取删除的参数错误!", BackURL, 1);
            }

        }

        /// <summary>
        /// 绑定下载权益
        /// </summary>
        protected void BindData()
        {
            string Condition = " 1=1";

            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "integrate_list";
            //需要返回的列
            string strGetFields = " RANK()  OVER (order by OrderId asc,uptime asc,Id asc) AS xuhao,Id,Name,num_integrate,upload_pic_img,uptime,OrderId";
            //排序的字段名
            string fldname = "OrderId desc,uptime desc,Id desc";
            //每页显示的记录数

            AspNetPager1.PageSize = 15;
            int page_Size = this.AspNetPager1.PageSize;
            //统计总记录数
            int intRecordCount = integrate_listBll.GetCount(tblName, strWhere);
            if (intRecordCount > 0)
            {
                DivNull.Visible = false;
            }
            DataTable dt = integrate_listBll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, strWhere);
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
                BackURL = "Admin_IntegrateList.aspx?MenuId=" + MenuId;
            }
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            integrate_list integrate_list = new integrate_list();
            if (Action == "Edit")
            {
                integrate_list = integrate_listBll.SelectSingle("Id=" + ID);
                if (!(integrate_list != null && integrate_list.id > 0))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "请求参数错误！", BackURL, 2);
                }
            }
            integrate_list.name = Function.HtmlEncode(Function.FormRequest("Name"));
            integrate_list.about_ = Function.HtmlEncode(Function.FormRequest("about_"));
            integrate_list.num_integrate = Function.ConvertTo<int>(Function.FormRequest("num_integrate"), 0);
            integrate_list.uptime = DateTime.Now;

            string file_img_img = "";
            try
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
                }
            }
            catch (Exception ex)
            {

                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }

            if (!string.IsNullOrWhiteSpace(file_img_img))
            {
                integrate_list.upload_pic_img = file_img_img;
            }


            if (Action == "Add")
            {
                AddUp.Visible = false;
                int orderint = 0;
                DataTable orderdt = integrate_listBll.GetDatatable("select max(orderid) as num from integrate_list where 1=1");
                if (orderdt != null && orderdt.Rows.Count > 0)
                {
                    orderint = Function.ConvertTo<int>(orderdt.Rows[0]["num"].ToString(), 0);
                }
                orderdt.Dispose();
                orderint++;
                integrate_list.orderid = orderint;
                integrate_list.addtime = DateTime.Now;
                if (integrate_listBll.Add(integrate_list, "Id") > 0)
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "文献下载权益《<font color=\"red\">" + Function.HtmlDiscode(integrate_list.name) + "</font>》 添加成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "文献下载权益《<font color=\"red\">" + Function.HtmlDiscode(integrate_list.name) + "</font>》 添加失败!", BackURL, 2);

                }
            }
            else if (Action == "Edit")
            {
                AddUp.Visible = false;
                string[] file = { "id" };
                if (integrate_listBll.Update(file, integrate_list))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "文献下载权益《<font color=\"red\">" + Function.HtmlDiscode(integrate_list.name) + "</font>》 修改成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "文献下载权益《<font color=\"red\">" + Function.HtmlDiscode(integrate_list.name) + "</font>》 修改失败!", BackURL, 2);
                }
            }
        }

        protected void OnClick_Search(object sender, EventArgs e)
        {
            Response.Redirect(Request.CurrentExecutionFilePath + "?SearchKeyWords=" + Server.UrlEncode(Function.FormRequest("SearchKeyWords")) + "&MenuId=" + MenuId);
        }
    }

}
