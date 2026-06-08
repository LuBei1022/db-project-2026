using LiteratureManager.Common;
using BLL;
using LitJson;
using Model;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Web;

namespace Web.Inc
{
    public partial class Upload_Img : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "text/plain";

            BLLBase<user_list> user_listbll = new BLLBase<user_list>();
            BLLBase<userimg_list> userimg_listbll = new BLLBase<userimg_list>();
            user_list user_list = CommonUserFunc.GetUserLoginStatus();
            try
            {
                int count = Request.Files.Count;
                string dirName = Function.GetRequest("btn");
                if (count > 0)
                {
                    HttpPostedFile File1 = Request.Files[0];
                    if (user_list != null && user_list.id > 0)
                    {
                        try
                        {
                            String aspxUrl = Request.Path.Substring(0, Request.Path.LastIndexOf("/") + 1);

                            //文件保存目录路径
                            String savePath = "../A_UpLoad/upload_pic/";

                            //文件保存目录URL
                            String saveUrl = aspxUrl + "../A_UpLoad/upload_pic/";

                            //定义允许上传的文件扩展名
                            Hashtable extTable = new Hashtable();
                            extTable.Add("upload_pic_pro", "gif,jpg,jpeg,png");
                            extTable.Add("upload_feedback_file", "gif,jpg,jpeg,png,pdf,doc,docx,xls,xlsx,ppt,pptx,txt,zip,rar,7z");
                            int maxSize = dirName == "upload_feedback_file"
                                ? UploadPolicy.MaxAttachmentBytes
                                : UploadPolicy.MaxImageBytes;
                            String dirPath = Server.MapPath(savePath);
                            if (!Directory.Exists(dirPath))
                            {
                                showError("上传目录不存在。");
                            }
                            else
                            {

                                if (!extTable.ContainsKey(dirName))
                                {
                                    showError("目录名不正确。");
                                }
                                else
                                {
                                    String fileName = File1.FileName;
                                    String fileExt = Path.GetExtension(fileName).ToLower();

                                    if (File1.ContentLength <= 0 || File1.ContentLength > maxSize)
                                    {
                                        showError("上传文件大小超过 " + UploadPolicy.ToMbLabel(maxSize) + " 限制。");
                                    }
                                    else if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(((String)extTable[dirName]).Split(','), fileExt.Substring(1).ToLower()) == -1)
                                    {
                                        showError("上传文件扩展名是不允许的扩展名。\n只允许" + ((String)extTable[dirName]) + "格式。");
                                    }
                                    else
                                    {
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
                                        Guid guid = Guid.NewGuid();

                                        String newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + "_" + guid.ToString().Replace("-", "").ToLower() + fileExt;
                                        String filePath = dirPath + newFileName;
                                        File1.SaveAs(filePath);

                                        string _imgurl = dirName + "/" + ymd + "/" + newFileName;
                                        userimg_list userimg_list = new userimg_list();
                                        userimg_list.userid = user_list.id;
                                        userimg_list.addtime = DateTime.Now;
                                        userimg_list.upload_pic_img = _imgurl;
                                        if (userimg_listbll.Add(userimg_list))
                                        {
                                            Hashtable hash = new Hashtable();
                                            hash["error"] = 1;
                                            hash["url"] = "/A_UpLoad/upload_pic/" + _imgurl;
                                            hash["name"] = Path.GetFileName(fileName);
                                            hash["ext"] = fileExt.Substring(1).ToLower();
                                            Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
                                            Response.Write(JsonMapper.ToJson(hash));
                                        }
                                        else
                                        {
                                            try
                                            {
                                                Function.FileDelete(@"/A_UpLoad/upload_pic/" + _imgurl);
                                            }
                                            catch (Exception ex_err)
                                            {
                                                ImportDataLog.WriteLog(LogType.Error, ex_err.Message + "-" + ex_err.StackTrace);
                                            }
                                            showError("上传保存失败，请稍后再试！");
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex_)
                        {
                            ImportDataLog.WriteLog(LogType.Error, ex_.Message + "-" + ex_.StackTrace);
                            showError(ex_.Message);
                        }
                    }
                    else
                    {
                        showError("登录状态异常");
                    }
                }
                else
                {
                    showError("上传文件为空");
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
                showError(ex.Message);
            }


        }
        public class RJson
        {
            public string ossurl { get; set; }
        }
        private void showError(string message)
        {
            Hashtable hash = new Hashtable();
            hash["error"] = 0;
            hash["message"] = message;
            Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
            Response.Write(JsonMapper.ToJson(hash));
        }
    }
}
