using LiteratureManager.Common;
using BLL;
using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;

namespace Web.A_Uploadify
{
    public partial class UpDownloadHandler : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            Response.ContentType = "text/plain";
            //context.Response.ContentEncoding = System.Text.Encoding.UTF8;

            //context.Response.ContentType = "text/plain";
            Response.Expires = -1;


            //通过传来的参数判断执行哪个方法
            string getFunction = Request["GetFunction"] ?? string.Empty;
            if (getFunction.Equals("UploadFile", StringComparison.InvariantCultureIgnoreCase))
                UploadFile(HttpContext.Current);
            if (getFunction.Equals("DeleteDocument", StringComparison.InvariantCultureIgnoreCase))
                DeleteDocument(HttpContext.Current);
            if (getFunction.Equals("GetFileList", StringComparison.InvariantCultureIgnoreCase))
                GetFileList(HttpContext.Current);
            if (getFunction.Equals("GetFileLists", StringComparison.InvariantCultureIgnoreCase))
                GetFileLists(HttpContext.Current);


        }

#pragma warning disable CS0108 // 成员隐藏继承的成员；缺少关键字 new
        public bool IsReusable
#pragma warning restore CS0108 // 成员隐藏继承的成员；缺少关键字 new
        {
            get
            {
                return false;
            }
        }
        //上传文件
        private void UploadFile(HttpContext context)
        {
            try
            {
                HttpPostedFile file = context.Request.Files["Filedata"];
                if (file == null || file.ContentLength <= 0)
                {
                    context.Response.StatusCode = 400;
                    context.Response.Write("上传文件为空");
                    return;
                }
                if (file.ContentLength > UploadPolicy.MaxAttachmentBytes)
                {
                    context.Response.StatusCode = 413;
                    context.Response.Write("上传文件大小超过 " + UploadPolicy.ToMbLabel(UploadPolicy.MaxAttachmentBytes) + " 限制");
                    return;
                }

                UploadDocumentItem objUploadDocumentItem = new UploadDocumentItem();
                objUploadDocumentItem.DocName = file.FileName;
                string sExtension = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                objUploadDocumentItem.DocMuid = DateTime.Now.ToString("yyyyMMddhhmmsfff") + sExtension;//生成一个新的MUID，保证文件名称的唯一性
                objUploadDocumentItem.UploadDate = DateTime.Now.ToShortDateString();

                /*在此通常需要配合数据库使用，把文件名和格式记录下来，在DEMO中我就不配合这个使用了，直接上传*/

                string uploadPath = HttpContext.Current.Server.MapPath(@"../A_UpLoad/upload_file") + "\\";
                //string uploadPath = HttpContext.Current.Server.MapPath("/"+context.Request["folder"] + "\\");
                if (file != null)
                {
                    //如果没有该目录则创建该上传目录
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    file.SaveAs(uploadPath + objUploadDocumentItem.DocMuid);

                    context.Response.Write(new JavaScriptSerializer().Serialize(objUploadDocumentItem));
                }
            }
            catch (Exception ex)
            {

                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }

        }

        //删除文件
        private void DeleteDocument(HttpContext context)
        {

            //string filePath = HttpContext.Current.Server.MapPath(@"../A_UpLoad/upload_file/" + context.Request["DocMuid"]);
            //System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            //if (file.Exists)
            //{
            //    file.Delete();
            //}
            if (!string.IsNullOrEmpty(context.Request["DocMuid"]))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize("Success"));
            }
        }


        private void GetFileLists(HttpContext context)
        {
            string return_str = "";
            string json = "";
            DataTable datatable = GetUploadTable(context, "id");
            if (datatable != null && datatable.Rows.Count == 1)
            {
                DataRow dr = datatable.Rows[0];
                if (dr["up_filename"].ToString() != "" && dr["up_filename"].ToString() != null)
                {
                    if (dr["up_filename"].ToString().IndexOf(",") > 0)
                    {
                        string[] str = dr["up_filename"].ToString().Split(',');
                        for (int i = 0; i < str.Length; i++)
                        {
                            return_str = return_str + "<div class='uploadify-queue-item' id='LMS_0_" + i + "'>";
                            return_str = return_str + "<div class='cancel'><a href=javascript:$('#uploadify').uploadify('cancel','LMS_0_" + i + "')>X</a></div>";
                            string safeName = HttpUtility.HtmlEncode(str[i]);
                            return_str = return_str + "<span id='fileNameLMS_0_" + i + "' class='fileName'>" + safeName + "</span>";
                            return_str = return_str + "<input type='hidden' value='" + safeName + "' id='up_fileNameLMS_0_" + i + "' name='up_fileName'>";
                            return_str = return_str + "<span class='data'> - 上传完成</span></div>";
                            json = "{\"info\":\"" + return_str + "\",\"cookie\":\"" + str.Length + "\"}";
                        }
                    }
                    else
                    {
                        return_str = return_str + "<div class='uploadify-queue-item' id='LMS_0_0'>";
                        return_str = return_str + "<div class='cancel'><a href=javascript:$('#uploadify').uploadify('cancel','LMS_0_0')>X</a></div>";
                        string safeName = HttpUtility.HtmlEncode(dr["up_filename"].ToString());
                        return_str = return_str + "<span id='fileNameLMS_0_0' class='fileName'>" + safeName + "</span>";
                        return_str = return_str + "<input type='hidden' value='" + safeName + "' id='up_fileNameLMS_0_0' name='up_fileName'>";
                        return_str = return_str + "<span class='data'> - 上传完成</span></div>";
                        json = "{\"info\":\"" + return_str + "\",\"cookie\":\"1\"}";
                    }
                }
                else
                {
                    json = "{\"info\":\"null\",\"cookie\":\"0\"}"; //获取失败
                }
            }
            else
            {
                json = "{\"info\":\"null\",\"cookie\":\"0\"}"; //获取失败
            }
            context.Response.Write(json); //成功
        }
        private void GetFileList(HttpContext context)
        {
            string return_str = "";
            string json = "";
            DataTable datatable = GetUploadTable(context, "orderid");
            if (datatable != null && datatable.Rows.Count == 1)
            {
                DataRow dr = datatable.Rows[0];
                if (dr["up_filename"].ToString() != "" && dr["up_filename"].ToString() != null)
                {
                    if (dr["up_filename"].ToString().IndexOf(",") > 0)
                    {
                        string[] str = dr["up_filename"].ToString().Split(',');
                        for (int i = 0; i < str.Length; i++)
                        {
                            return_str = return_str + "<div class='uploadify-queue-item' id='LMS_0_" + i + "'>";
                            return_str = return_str + "<div class='cancel'><a href=javascript:$('#uploadify').uploadify('cancel','LMS_0_" + i + "')>X</a></div>";
                            string safeName = HttpUtility.HtmlEncode(str[i]);
                            return_str = return_str + "<span id='fileNameLMS_0_" + i + "' class='fileName'>" + safeName + "</span>";
                            return_str = return_str + "<input type='hidden' value='" + safeName + "' id='up_fileNameLMS_0_" + i + "' name='up_fileName'>";
                            return_str = return_str + "<span class='data'> - 上传完成</span></div>";
                            json = "{\"info\":\"" + return_str + "\",\"cookie\":\"" + str.Length + "\"}";
                        }
                    }
                    else
                    {
                        return_str = return_str + "<div class='uploadify-queue-item' id='LMS_0_0'>";
                        return_str = return_str + "<div class='cancel'><a href=javascript:$('#uploadify').uploadify('cancel','LMS_0_0')>X</a></div>";
                        string safeName = HttpUtility.HtmlEncode(dr["up_filename"].ToString());
                        return_str = return_str + "<span id='fileNameLMS_0_0' class='fileName'>" + safeName + "</span>";
                        return_str = return_str + "<input type='hidden' value='" + safeName + "' id='up_fileNameLMS_0_0' name='up_fileName'>";
                        return_str = return_str + "<span class='data'> - 上传完成</span></div>";
                        json = "{\"info\":\"" + return_str + "\",\"cookie\":\"1\"}";
                    }
                }
                else
                {
                    json = "{\"info\":\"null\",\"cookie\":\"0\"}"; //获取失败
                }
            }
            else
            {
                json = "{\"info\":\"null\",\"cookie\":\"0\"}"; //获取失败
            }
            context.Response.Write(json); //成功
        }

        private DataTable GetUploadTable(HttpContext context, string orderField)
        {
            string tableName = (context.Request.QueryString["tb"] ?? string.Empty).Trim();
            int id;
            if (!IsSafeIdentifier(tableName) || !int.TryParse(context.Request.QueryString["Id"], out id) || id <= 0)
            {
                return null;
            }

            BLLBase<Model.admin> adminbll = new BLLBase<Model.admin>();
            return adminbll.GetDatatable("select * from [" + tableName + "] where id=" + id + " order by " + orderField + " asc");
        }

        private bool IsSafeIdentifier(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !(char.IsLetter(value[0]) || value[0] == '_'))
            {
                return false;
            }

            foreach (char character in value)
            {
                if (!(char.IsLetterOrDigit(character) || character == '_'))
                {
                    return false;
                }
            }

            return true;
        }



        public class UploadDocumentItem
        {
            public UploadDocumentItem()
            { }
            public string DocMuid { get; set; }
            public string DocName { get; set; }
            public string UploadDate { get; set; }
        }
    }
}
