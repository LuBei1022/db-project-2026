using LiteratureManager.Common;
using BLL;
using DAL;
using Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web;

namespace Web.admin
{
    public partial class Admin_SiteMenu : System.Web.UI.Page
    {
        string Action = Function.GetRequest("Action");
        BLLBase<tbl_class> tbl_classbll = new BLLBase<tbl_class>();
        public int MenuId = Function.ConvertTo<int>(Function.GetRequest("MenuId"), 0);
        public bool isLoading = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            if (!IsPostBack)
            {
                switch (Action)
                {
                    case "AddColumn":
                        AddColumnFunc();
                        break;
                    case "EditColumn":
                        EditColumnFunc();
                        break;
                    case "DelColumn":
                        DelColumnFunc();
                        break;
                    default:
                        BindData();
                        break;
                }
            }
        }
        protected string GetMenuHtml()
        {
            string R_str = "";
            tbl_class tbl_class = tbl_classbll.SelectSingle("id", Function.ConvertTo<int>(Function.GetRequest("ParentId"), 0));
            if (tbl_class != null && tbl_class.id > 0)
            {
                R_str = Function.HtmlDiscode(tbl_class.classname);

                while (tbl_class.parentid > 359)
                {
                    tbl_class = tbl_classbll.SelectSingle("id", tbl_class.parentid);
                    if (tbl_class != null)
                    {
                        int parentid = tbl_class.id;
                        R_str = "<a href=\"?MenuId=" + MenuId + "&ParentId=" + parentid + "\" style=\"margin-right: 0.5em;\">" + Function.HtmlDiscode(tbl_class.classname) + "></a>" + R_str + "";
                    }
                    else
                    {
                        tbl_class = new tbl_class();
                        tbl_class.parentid = 359;
                    }
                }
            }


            return R_str;
        }


        protected void AddColumnFunc()
        {
            del_upload_pic_pc_0.Checked = true;
            AddUp.Visible = true;
            Main.Visible = false;
            Select_List();
        }

        protected void EditColumnFunc()
        {
            AddUp.Visible = true;
            Main.Visible = false;
            Select_List();
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            tbl_class tbl_class = tbl_classbll.SelectSingle("id", ID);
            if (tbl_class != null && tbl_class.id > 0)
            {
                del_upload_pic_pc_0.Checked = true;
                if (tbl_class.isurl == 1)
                {
                    isurl1.Checked = true;
                    if (!string.IsNullOrWhiteSpace(tbl_class.info_))
                    {
                        info_.Text = Function.HtmlSqlDiscode(tbl_class.info_);

                        if (!string.IsNullOrWhiteSpace(tbl_class.description))
                        {
                            description.Text = Function.HtmlDiscode(tbl_class.description);
                        }
                    }
                }
                else if (tbl_class.isurl == 2)
                {
                    isurl2.Checked = true;
                    if (!string.IsNullOrWhiteSpace(tbl_class.classurl))
                    {
                        ClassURL.Text = Function.HtmlDiscode(tbl_class.classurl);
                    }
                }

                if (!string.IsNullOrWhiteSpace(tbl_class.about))
                {
                    about.Text = Function.HtmlDiscode(tbl_class.about);
                }
                if (!string.IsNullOrWhiteSpace(tbl_class.classname))
                {
                    name.Text = Function.HtmlDiscode(tbl_class.classname);
                }
                if (!string.IsNullOrWhiteSpace(tbl_class.urlnamebtn))
                {
                    urlnamebtn.Text = Function.HtmlDiscode(tbl_class.urlnamebtn);
                }

                if (!string.IsNullOrWhiteSpace(tbl_class.upload_pic_pc))
                {
                    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/A_UpLoad/upload_pic/" + tbl_class.upload_pic_pc));
                    if (file.Exists)
                    {
                        upload_pic_pc_Old.Value = Function.HtmlDiscode(tbl_class.upload_pic_pc);
                        upload_pic_pc_img.ImageUrl = Function.GetAdminUpload_Pic(tbl_class.upload_pic_pc);
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
        protected void DelColumnFunc()
        {
            isLoading = false;
            AddUp.Visible = false;
            Main.Visible = false;
            int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            int ParentId = Function.ConvertTo<int>(Function.GetRequest("ParentId"), -1);
            string BackURL = Request.QueryString["BackURL"];
            if (string.IsNullOrWhiteSpace(BackURL))
            {
                BackURL = "Admin_SiteMenu.aspx?ParentId=360&MenuId=" + MenuId;
            }
            if (ID > 0)
            {
                tbl_class tbl_class = tbl_classbll.SelectSingle("parentid=" + ID);
                if (tbl_class != null && tbl_class.id > 0)
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "操作失败，请先删除下属栏目!", BackURL, 2);
                }
                if (ParentId == 360)
                {
                    if (Cookie.GetCookie("LMS_AdminName").ToString().ToUpper() != "SYSADMIN")
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "栏目删除失败，网站根目录禁止删除!", "javascript:history.go(-1)", 2);
                    }
                }
                tbl_class tbl_class_model = tbl_classbll.SelectSingle("id", ID);
                if (tbl_class_model != null && tbl_class_model.id > 0)
                {

                    if (tbl_classbll.Delete("id", ID))
                    {
#pragma warning disable CS0168 // 声明了变量，但从未使用过
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(tbl_class_model.upload_pic_pc))
                            {
                                Function.FileDelete("/A_UpLoad/upload_pic/" + tbl_class_model.upload_pic_pc + "");
                            }

                        }
                        catch (Exception ex)
                        {


                        }
#pragma warning restore CS0168 // 声明了变量，但从未使用过
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "栏目删除成功!", BackURL, 0);
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "栏目删除失败!", BackURL, 2);
                    }
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "未找到相应的栏目信息", BackURL, 3);
                }

            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "获取删除栏目请求的参数错误!", BackURL, 3);
            }
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        protected void BindData()
        {
            string sql = "select * from tbl_class where 1=1";

            int ParentId = Function.ConvertTo<int>(Function.GetRequest("ParentId"), -1);
            string BackURL = Request.QueryString["BackURL"];

            if (ParentId == 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "无上级栏目，已经是根目录！", "javascript:history.go(-1)", 1);
            }

            if (ParentId != -1)
            {
                sql = sql + " and parentid = " + ParentId + "";
                tbl_class tbl_class = tbl_classbll.SelectSingle("id", ParentId);
                if (tbl_class != null && tbl_class.id > 0)
                {

                    if (!tbl_classbll.Update("children='" + BindDrpClass(ParentId) + "'", "id=" + ParentId))
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "系统拉取子集栏目错误", "javascript:history.go(-1)", 2);
                    }
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "查找不到项目栏目列表信息", "javascript:history.go(-1)", 3);
                }

            }
            else
            {
                sql = sql + " and parentid = 360";
            }

            sql = sql + " order by orderid asc";

            DataTable tbl_classdt = tbl_classbll.GetDatatable(sql);
            if (tbl_classdt != null && tbl_classdt.Rows.Count > 0)
            {
                AspNetPager1.RecordCount = tbl_classdt.Rows.Count;
                SqlDataAdapter sda = new SqlDataAdapter(sql, DBHelper.ConnectionString);
                DataSet ds = new DataSet();
                AspNetPager1.AlwaysShow = true;
                AspNetPager1.PageSize = 10;

                sda.Fill(ds, AspNetPager1.PageSize * (AspNetPager1.CurrentPageIndex - 1), AspNetPager1.PageSize, "dbTable");

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            ds.CaseSensitive = false;
                            this.Repeater1.DataSource = ds.Tables[0].DefaultView;
                            this.Repeater1.DataBind();
                            DivNull.Visible = false;
                        }
                        dt.Dispose();
                    }
                }
            }
            tbl_classdt.Dispose();
        }

        protected void AspNetPager1_PageChanged(object src, EventArgs e)
        {
            BindData();
        }

        protected void DelSelect_Click(object sender, EventArgs e)
        {
            isLoading = false;
            int ParentId = Function.ConvertTo<int>(Function.GetRequest("ParentId"), -1);
            string BackURL = Request.Form["BackURL"];
            if (string.IsNullOrWhiteSpace(BackURL))
            {
                BackURL = "Admin_SiteMenu.aspx?ParentId=360&MenuId=" + MenuId;
            }
            string id = Function.FormRequest("id");
            int num = 0;
            if (ParentId == -1)
            {
                Main.Visible = false;
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "清除失败，您没有选择要清除的栏目!", BackURL, 3);
            }
            else
            {

                if (ParentId == 360 && Cookie.GetCookie("LMS_AdminName").ToString().ToUpper() != "SYSADMIN")
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "栏目删除失败，网站根目录禁止删除!", "javascript:history.go(-1)", 2);
                }
                else
                {

                    string[] Dic = Function.ret_Power(id);

                    for (int i = 0; i < Dic.Length; i++)
                    {
                        int did = Function.ConvertTo<int>(Dic[i], 0);
                        tbl_class tbl_class = tbl_classbll.SelectSingle("parentid=" + did);
                        if (tbl_class != null && tbl_class.id > 0)
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "操作失败，请先删除下属栏目!", BackURL, 3);
                        }
                        tbl_class tbl_class_did = tbl_classbll.SelectSingle("id=" + did);
                        if (tbl_class_did != null && tbl_class_did.id > 0)
                        {

                            if (tbl_classbll.Delete("id", did))
                            {
#pragma warning disable CS0168 // 声明了变量，但从未使用过
                                try
                                {
                                    if (!string.IsNullOrWhiteSpace(tbl_class_did.upload_pic_pc))
                                    {
                                        Function.FileDelete("/A_UpLoad/upload_pic/" + tbl_class_did.upload_pic_pc + "");
                                    }

                                }
                                catch (Exception ex)
                                {


                                }
#pragma warning restore CS0168 // 声明了变量，但从未使用过
                                num++;
                            }
                        }

                    }
                    Main.Visible = false;
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "共勾选" + Dic.Length + "个栏目，成功删除" + num + "个栏目!", BackURL, 0);
                }
            }
        }

        protected void OnClick_AddUp(object sender, EventArgs e)
        {
            isLoading = false;
            try
            {
                int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
                string BackURL = Request.QueryString["BackURL"];
                if (string.IsNullOrWhiteSpace(BackURL))
                {
                    BackURL = "Admin_SiteMenu.aspx?ParentId=360&MenuId=" + MenuId;
                }
                tbl_class tbl_class = new tbl_class();
                if (Action == "EditColumn")
                {
                    tbl_class = tbl_classbll.SelectSingle("id", ID);
                    if (!(tbl_class != null && tbl_class.id > 0))
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "请求参数错误！", BackURL, 2);
                    }
                }
                tbl_class.about = Function.HtmlEncode(Function.FormRequest("about"));
                tbl_class.classname = Function.HtmlEncode(Function.FormRequest("name"));
                tbl_class.urlnamebtn = Function.HtmlEncode(Function.FormRequest("urlnamebtn"));
                tbl_class.model = Function.ConvertTo<int>(Function.FormRequest("Model"), -1);
                tbl_class.isurl = Function.ConvertTo<int>(Function.FormRequest("isurl"), 0);
                if (tbl_class.isurl == 2)
                {
                    tbl_class.classurl = Function.HtmlEncode(Function.FormRequest("ClassURL"));
                }
                else
                {
                    tbl_class.description = Function.HtmlEncode(Function.FormRequest("description"));
                    tbl_class.info_ = Function.HtmlSqlEncode(Function.FormRequest("info_"));

                }


                tbl_class.parentid = Function.ConvertTo<int>(Function.GetRequest("ParentId"), -1);
                tbl_class.adddate = DateTime.Now;
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
                            string ymd = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + "_1" + Path.GetExtension(imgurl).ToLower();

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
                    tbl_class.upload_pic_pc = file_pc_img;
                }
                else if (del_pc_img && isyes_pc_img)
                {
                    tbl_class.upload_pic_pc = "";
                }




                if (Action == "AddColumn")
                {
                    AddUp.Visible = false;
                    tbl_class.isshow = 1;
                    tbl_class.istop = 0;
                    int orderint = 0;
                    DataTable orderdt = tbl_classbll.GetDatatable("select max(orderid) as num from tbl_class  where parentid=" + tbl_class.parentid);
                    if (orderdt != null && orderdt.Rows.Count > 0)
                    {
                        orderint = Function.ConvertTo<int>(orderdt.Rows[0]["num"].ToString(), 0);
                    }
                    orderdt.Dispose();
                    orderint++;
                    tbl_class.orderid = orderint;
                    if (tbl_classbll.Add(tbl_class, "id") > 0)
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
                        if (isyes_pc_img)
                        {

                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "栏目 <font color=\"red\">" + Function.HtmlDiscode(tbl_class.classname) + "</font> 添加成功!", BackURL, 0);
                        }
                        else
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "栏目 <font color=\"red\">" + Function.HtmlDiscode(tbl_class.classname) + "</font> 添加成功,但图片上传失败，请重新编辑信息!", BackURL, 2);
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
                            if (!string.IsNullOrWhiteSpace(tbl_class.upload_pic_pc))
                            {
                                try
                                {
                                    Function.FileDelete("/A_UpLoad/upload_pic/" + tbl_class.upload_pic_pc + "");
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
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "栏目 <font color=\"red\">" + Function.HtmlDiscode(tbl_class.classname) + "</font> 添加失败!", BackURL, 2);
                    }
                }
                else if (Action == "EditColumn")
                {
                    AddUp.Visible = false;


                    string[] file = { "id" };
                    if (tbl_classbll.Update(file, tbl_class))
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
                            if (tbl_class.upload_pic_pc != upload_pic_pc_Old)
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
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "栏目 <font color=\"red\">" + Function.HtmlDiscode(tbl_class.classname) + "</font> 修改成功!", BackURL, 0);
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
                            if (tbl_class.upload_pic_pc != upload_pic_pc_Old)
                            {

                                if (tbl_class.upload_pic_pc.IndexOf("/") >= 0)//包含
                                {
                                    Function.FileDelete("/A_UpLoad/upload_pic/" + tbl_class.upload_pic_pc + "");
                                }
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "栏目 <font color=\"red\">" + Function.HtmlDiscode(tbl_class.classname) + "</font> 修改失败!", BackURL, 2);
                    }

                }

            }
            catch (Exception)
            {

                throw;
            }

        }



        public string GetCount(string id)
        {
            string ReturnStr = "";
            DataTable tbl_classdt = tbl_classbll.GetDatatable("select Count(*) as num from tbl_class where parentid=" + id);
            if (tbl_classdt != null && tbl_classdt.Rows.Count > 0)
            {
                ReturnStr = "(" + tbl_classdt.Rows[0]["num"] + ")";
            }
            tbl_classdt.Dispose();
            return ReturnStr;
        }

        //上级栏目
        public string GetTool(string ParentId)
        {
            int? backid = 0;

            if (ParentId != "")
            {
                tbl_class tbl_class = tbl_classbll.SelectSingle("id", ParentId);
                if (tbl_class != null && tbl_class.id > 0)
                {
                    backid = tbl_class.parentid;
                }

            }
            return "<a href=\"Admin_SiteMenu.aspx?ParentId=" + ParentId + "&MenuId=" + MenuId + "&Action=AddColumn&BackURL=" + Function.GetEncodeURL() + "\" style=\"font-weight:800;color:#F60;margin-right: 0.5em;\" hidefocus=\"true\">添加栏目</a> <a href=\"Admin_SiteMenu.aspx?ParentId=" + backid + "&MenuId=" + MenuId + "\" style=\"font-weight:800;color:#F60;\" hidefocus=\"true\">返回上级</a>";
        }

        protected void Select_List()
        {
            DataTable modelDt = new DataTable();
            modelDt.Columns.Add("id", typeof(int));
            modelDt.Columns.Add("m_name", typeof(string));
            modelDt.Columns.Add("upload_pic", typeof(string));
            modelDt.Rows.Add(1, CommonFunc.GetModelName(1), string.Empty);
            modelDt.Rows.Add(2, CommonFunc.GetModelName(2), string.Empty);
            modelDt.Rows.Add(3, CommonFunc.GetModelName(3), string.Empty);
            this.Repeater2.DataSource = modelDt.DefaultView;
            this.Repeater2.DataBind();
            modelDt.Dispose();
        }
        protected string GetModelChecked(string id_)
        {
            string R_str = string.Empty;
            if (Action == "EditColumn")
            {
                int ID = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
                if (ID > 0)
                {
                    tbl_class tbl_class = tbl_classbll.SelectSingle("id", ID);
                    if (tbl_class != null && tbl_class.model.ToString() == id_)
                    {
                        R_str = " checked=\"checked\"";
                    }
                }
            }
            return R_str;
        }

        public string GetModel(string mid)
        {
            int mint = Function.ConvertTo<int>(mid, 0);
            return CommonFunc.GetModelName(mint);
        }

        public string GetOperation(string parentid, string id)
        {

            string return_str = "";
            if (Cookie.GetCookie("LMS_AdminName").ToString().ToUpper() != "SYSADMIN")
            {
                if (parentid == "0" || parentid == "360")
                {
                    if (id != "422")
                    {
                        return_str = return_str + "<a class=\"badge text-bg-success\" href='?ParentId=" + parentid + "&MenuId=" + MenuId + "&Action=EditColumn&ID=" + id + "&BackURL=" + Function.GetEncodeURL() + "'>编辑</a>";
                    }
                }
                else
                {
                    return_str = return_str + "<a class=\"badge text-bg-success\" href='?ParentId=" + parentid + "&MenuId=" + MenuId + "&Action=EditColumn&ID=" + id + "&BackURL=" + Function.GetEncodeURL() + "'>编辑</a>";
                    return_str = return_str + "<a class=\"badge text-bg-danger\" data-href='?ParentId=" + parentid + "&MenuId=" + MenuId + "&Action=DelColumn&ID=" + id + "&BackURL=" + Function.GetEncodeURL() + "' onclick=\"DataDelFunc(this)\">删除</a>";
                }
            }
            else
            {
                return_str = return_str + "<a class=\"badge text-bg-success\" href='?ParentId=" + parentid + "&MenuId=" + MenuId + "&Action=EditColumn&ID=" + id + "&BackURL=" + Function.GetEncodeURL() + "'>编辑</a>";
                return_str = return_str + "<a class=\"badge text-bg-danger\" data-href='?ParentId=" + parentid + "&MenuId=" + MenuId + "&Action=DelColumn&ID=" + id + "&BackURL=" + Function.GetEncodeURL() + "' onclick=\"DataDelFunc(this)\">删除</a>";
            }

            return return_str;
        }

        string classid_str = "";
        private string BindDrpClass(int id)
        {
            string nav_str = "";
            string yi = "";
            DataTable tbl_classdt = tbl_classbll.GetDatatable("select * from tbl_class where 1=1 order by orderid asc");
            if (tbl_classdt != null && tbl_classdt.Rows.Count > 0)
            {
                DataRow[] drs = tbl_classdt.Select("parentid= " + id);
                foreach (DataRow dr in drs)
                {
                    int classid = int.Parse(dr["id"].ToString());
                    yi = yi + classid + ",";
                    nav_str = BindNode(classid, tbl_classdt) + yi;
                }
                classid_str = "";
            }
            tbl_classdt.Dispose();
            return nav_str + id;
        }

        //绑定子分类
        private string BindNode(int cid, DataTable dt)
        {
            DataRow[] drs = dt.Select("parentid= " + cid);

            foreach (DataRow dr in drs)
            {
                int classid = int.Parse(dr["id"].ToString());
                classid_str = dr["id"] + "," + BindNode(classid, dt);
            }
            return classid_str;
        }
    }

}
