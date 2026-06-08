using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using LiteratureManager.Common;

namespace Web.A_UpLoadTool.tool
{
    public partial class upload_json : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            Response.ContentType = "text/plain";
            HttpPostedFile imgFile = null;
            imgFile = Request.Files[0];

            if (imgFile != null)
            {
                try
                {
                    String aspxUrl = Request.Path.Substring(0, Request.Path.LastIndexOf("/") + 1);

                    //文件保存目录路径
                    String savePath = "../../A_UpLoad/";

                    //文件保存目录URL
                    String saveUrl = aspxUrl + "../../A_UpLoad/";

                    //定义允许上传的文件扩展名
                    Hashtable extTable = new Hashtable();
                    extTable.Add("image", "gif,jpg,jpeg,png,bmp");
                    extTable.Add("flash", "swf,flv");
                    extTable.Add("media", "mp4");
                    extTable.Add("file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2,pdf");
                    extTable.Add("upload_pic", "gif,jpg,jpeg,png,bmp");
                    extTable.Add("upload_file", "doc,docx,xls,xlsx,pdf,zip,rar,gz");
                    //最大文件大小
                    int maxSize = UploadPolicy.MaxAttachmentBytes;

                    if (imgFile == null)
                    {
                        showError("请选择文件。");
                        return;
                    }

                    String dirPath = Server.MapPath(savePath);
                    if (!Directory.Exists(dirPath))
                    {
                        showError("上传目录不存在。");
                        return;
                    }

                    String dirName = Request.QueryString["dir"];

                    if (HttpContext.Current.Request.QueryString["action"] == "upload_pic")
                    {
                        dirName = "upload_pic";
                    }

                    if (String.IsNullOrEmpty(dirName))
                    {
                        dirName = "image";
                    }
                    if (!extTable.ContainsKey(dirName))
                    {
                        showError("目录名不正确。");
                        return;
                    }
                    String fileName = imgFile.FileName;
                    String fileExt = Path.GetExtension(fileName).ToLower();

                    if (imgFile.InputStream == null || imgFile.InputStream.Length > maxSize)
                    {
                        showError("上传文件大小超过 " + UploadPolicy.ToMbLabel(maxSize) + " 限制。");
                        return;
                    }

                    if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(((String)extTable[dirName]).Split(','), fileExt.Substring(1).ToLower()) == -1)
                    {
                        showError("上传文件扩展名是不允许的扩展名。\n只允许" + ((String)extTable[dirName]) + "格式。");
                        return;
                    }

                    //创建文件夹
                    dirPath += dirName + "/";
                    saveUrl += dirName + "/";
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    String ymd = DateTime.Now.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
                    dirPath += ymd + "/";
                    saveUrl += ymd + "/";
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }

                    String newFileName = Guid.NewGuid().ToString() + fileExt;
                    String filePath = dirPath + newFileName;

                    imgFile.SaveAs(filePath);

                    String fileUrl = saveUrl + newFileName;
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    Response.AddHeader("Content-Type", "application/json;charset=UTF-8");
                    Response.Write(serializer.Serialize(new Dictionary<string, object> { ["error"] = 0, ["url"] = fileUrl }));

                }
                catch (Exception ex)
                {
                    Response.Write("上传失败1" + ex.Message);
                }
            }
            else
            {
                Response.Write("上传失败0");
            }

        }
        private void showError(string message)
        {
            Hashtable hash = new Hashtable();
            hash["error"] = 1;
            hash["message"] = message;
            Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
            Response.Write(JsonMapper.ToJson(hash));
        }
    }
}
