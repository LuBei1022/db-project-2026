

using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

namespace LiteratureManager.Common
{
    public class CommonFunc
    {
        public static BLLBase<websiteinfo_list> websiteinfo_listbll = new BLLBase<websiteinfo_list>();
        public static BLLBase<logincode_list> logincode_listbll = new BLLBase<logincode_list>();
        public static BLLBase<ResourceClass2_List> ResourceClass2_Listbll = new BLLBase<ResourceClass2_List>();
        public static BLLBase<ResourceClass1_List> ResourceClass1_Listbll = new BLLBase<ResourceClass1_List>();
        public static BLLBase<ResourceClass3_List> ResourceClass3_Listbll = new BLLBase<ResourceClass3_List>();
        public static BLLBase<model_list> model_listbll = new BLLBase<model_list>();
        public static BLLBase<tbl_class> tbl_classbll = new BLLBase<tbl_class>();
        public static BLLBase<userimg_list> userimg_listbll = new BLLBase<userimg_list>();
        public static BLLBase<userfile_list> userfile_listbll = new BLLBase<userfile_list>();
        public static BLLBase<ServiceLogStatus_List> ServiceLogStatus_Listbll = new BLLBase<ServiceLogStatus_List>();
        public static BLLBase<ResourceDataInfoCopy_list> ResourceDataInfoCopy_listbll = new BLLBase<ResourceDataInfoCopy_list>();
        public static BLLBase<ResourceLicense_List> ResourceLicense_Listbll = new BLLBase<ResourceLicense_List>();
        public static BLLBase<ResourceFormatTag_List> ResourceFormatTag_Listbll = new BLLBase<ResourceFormatTag_List>();
        public static BLLBase<user_list> user_listbll = new BLLBase<user_list>();
        public static BLLBase<ResourceTag_List> ResourceTag_Listbll = new BLLBase<ResourceTag_List>();
        public static BLLBase<ResourceHomeClassData_List> ResourceHomeClassData_Listbll = new BLLBase<ResourceHomeClassData_List>();
        public static BLLBase<Resource_list> Resource_listbll = new BLLBase<Resource_list>();
        public static BLLBase<ResourceCollect_List> ResourceCollect_Listbll = new BLLBase<ResourceCollect_List>();
        public static BLLBase<ResourceLike_List> ResourceLike_Listbll = new BLLBase<ResourceLike_List>();
        public static BLLBase<ResourceComment_list> ResourceComment_listbll = new BLLBase<ResourceComment_list>();
        public static BLLBase<ResourceCommentImg_list> ResourceCommentImg_listbll = new BLLBase<ResourceCommentImg_list>();
        public static BLLBase<ResourceCommentReply_list> ResourceCommentReply_listbll = new BLLBase<ResourceCommentReply_list>();
        public static BLLBase<ResourceCommentReplyImg_list> ResourceCommentReplyImg_listbll = new BLLBase<ResourceCommentReplyImg_list>();
        public static BLLBase<ResourceCommentLike_List> ResourceCommentLike_Listbll = new BLLBase<ResourceCommentLike_List>();
        public static BLLBase<integrateLogType_list> integrateLogType_listbll = new BLLBase<integrateLogType_list>();
        public static BLLBase<ResourceFormat_List> ResourceFormat_Listbll = new BLLBase<ResourceFormat_List>();
        public static BLLBase<ResourceSearchLog_list> ResourceSearchLog_listbll = new BLLBase<ResourceSearchLog_list>();
        public static BLLBase<cosfile_list> cosfile_listbll = new BLLBase<cosfile_list>();
        public static BLLBase<WorkflowTaskComment_list> WorkflowTaskComment_listbll = new BLLBase<WorkflowTaskComment_list>();
        public static BLLBase<WorkflowTaskReply_list> WorkflowTaskReply_listbll = new BLLBase<WorkflowTaskReply_list>();
        public static BLLBase<WorkflowTaskReplyImage_list> WorkflowTaskReplyImage_listbll = new BLLBase<WorkflowTaskReplyImage_list>();
        public static BLLBase<WorkflowTaskReaction_list> WorkflowTaskReaction_listbll = new BLLBase<WorkflowTaskReaction_list>();
        public static BLLBase<WorkflowTaskCommentImage_list> WorkflowTaskCommentImage_listbll = new BLLBase<WorkflowTaskCommentImage_list>(); 
        public static string MD5Key = ConfigurationManager.AppSettings["md5_key"];
        public static string A_UpLoad_Url = ConfigurationManager.AppSettings["website_url"] + "/A_UpLoad/upload_pic/";
        public static DataTable ExcelDataSource(string filepath, string sheetname)
        {

            //string strConn = "Provider=Microsoft.Jet.OleDb.4.0;" + "data source=" + filepath + ";Extended Properties='Excel 8.0; HDR=Yes; IMEX=1'";
            string strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + filepath + ";Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";
            OleDbConnection conn = new OleDbConnection(strConn);
            OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + sheetname + "]", strConn);
            DataTable dt = new DataTable();
            oada.Fill(dt);
            return dt;

        }
        public static string GetSearchLogDelFunc(string Search_key)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                if (!string.IsNullOrWhiteSpace(Search_key))
                {
                    Search_key = Function.HtmlInputValueDiscode(Search_key);
                    int user_id = 0;
                    string codestr = string.Empty;
                    user_list user_list = CommonUserFunc.GetUserLoginStatus();
                    if (user_list != null && user_list.id > 0)
                    {
                        user_id = user_list.id;
                    }
                    else
                    {
                        codestr = Cookie.GetCookie("search_code");
                    }
                    if (ResourceSearchLog_listbll.Update("isshow=-1", "user_id=" + user_id + " and keyword='" + Function.HtmlEncode(Search_key) + "' and codestr='" + Function.HtmlEncode(codestr) + "' and isshow=1"))
                    {
                        str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = "删除成功！" });
                    }
                    else
                    {
                        str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "删除失败！" });
                    }
                }
                else
                {
                    str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "搜索词异常！" });
                }
            }
            catch (Exception ex)
            {
                str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = ex.Message });
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
            return str_;
        }

        public static string GetSearchLogAddFunc(string Search_key)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                if (!string.IsNullOrWhiteSpace(Search_key))
                {
                    Search_key = Function.HtmlInputValueDiscode(Search_key);
                    int user_id = 0;
                    string codestr = string.Empty;
                    user_list user_list = CommonUserFunc.GetUserLoginStatus();
                    if (user_list != null && user_list.id > 0)
                    {
                        user_id = user_list.id;
                    }
                    else
                    {
                        codestr = Cookie.GetCookie("search_code");
                    }

                    ResourceSearchLog_list ResourceSearchLog_list = new ResourceSearchLog_list();
                    ResourceSearchLog_list.user_id = user_id;
                    ResourceSearchLog_list.addtime = DateTime.Now;
                    ResourceSearchLog_list.keyword = Function.HtmlEncode(Search_key);
                    ResourceSearchLog_list.isshow = 1;
                    ResourceSearchLog_list.codestr = Function.HtmlEncode(codestr);
                    if (ResourceSearchLog_listbll.Add(ResourceSearchLog_list))
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
            return str_;
        }
        public static string GetResourceCommentPageFunc(int resource_id, int pageindex, int type, int user_id)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                string strWhere = " Resource_id=" + resource_id + " and status=1";

                //表或视图名
                string tblName = "ResourceComment_list";
                //需要返回的列
                string strGetFields = " id, info_, addtime, user_id, num_dianzan,num_msg";
                //排序的字段名
                string fldname = " addtime desc,id desc";
                if (type == 2)
                {
                    fldname = " num_dianzan desc,addtime desc,id desc";
                }
                else if (type == 3)
                {
                    fldname = " num_msg desc,addtime desc,id desc";
                }
                //每页显示的记录数
                int page_Size = 20;
                string pagehtml = string.Empty;
                DataTable ResourceComment_listdt = null;
                try
                {
                    ResourceComment_listdt = ResourceComment_listbll.GetListByPage(tblName, strGetFields, fldname, page_Size, pageindex, strWhere);
                }
                catch (Exception ex)
                {
                    if (ex.Message.IndexOf("status", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        throw;
                    }
                    ResourceComment_listdt = ResourceComment_listbll.GetListByPage(tblName, strGetFields, fldname, page_Size, pageindex, " Resource_id=" + resource_id);
                }
                if (ResourceComment_listdt != null && ResourceComment_listdt.Rows.Count > 0)
                {
                    foreach (DataRow item in ResourceComment_listdt.Rows)
                    {
                        string user_name = string.Empty;
                        string user_avatar = "/images/touxiang1.png";
                        user_list user_list = user_listbll.SelectSingle("id=" + Function.ConvertTo<int>(item["user_id"].ToString(), 0));
                        if (user_list != null && user_list.id > 0)
                        {
                            user_name = Function.HtmlDiscode(user_list.name);
                            user_avatar = CommonUserFunc.GetUserAvatarFunc(user_list.upload_pic_avatar);
                        }
                        string msg_img = string.Empty;
                        DataTable ResourceCommentImg_listdt = ResourceCommentImg_listbll.GetDatatable("select upload_pic_info from ResourceCommentImg_list where ResourceComment_Id=" + item["id"].ToString() + "  order by orderid asc,addtime asc");
                        if (ResourceCommentImg_listdt != null && ResourceCommentImg_listdt.Rows.Count > 0)
                        {

                            msg_img += "            <div class=\"com-img\">";
                            foreach (DataRow item_img in ResourceCommentImg_listdt.Rows)
                            {
                                msg_img += "                <div class=\"com-img-item\"> <img src=\"" + GetWebUpload_Pic(item_img["upload_pic_info"].ToString(), "/images/null.png") + "\" /></div>";
                            }
                            msg_img += "            </div>";
                        }
                        ResourceCommentImg_listdt.Dispose();


                        string msg_reply = string.Empty;
                        DataTable ResourceCommentReply_listdt = null;
                        try
                        {
                            ResourceCommentReply_listdt = ResourceCommentReply_listbll.GetDatatable("select id, info_, addtime, user_id from ResourceCommentReply_list where ResourceComment_Id=" + item["id"].ToString() + " and status=1 order by addtime desc, id desc");
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.IndexOf("status", StringComparison.OrdinalIgnoreCase) < 0)
                            {
                                throw;
                            }
                            ResourceCommentReply_listdt = ResourceCommentReply_listbll.GetDatatable("select id, info_, addtime, user_id from ResourceCommentReply_list where ResourceComment_Id=" + item["id"].ToString() + " order by addtime desc, id desc");
                        }
                        if (ResourceCommentReply_listdt != null && ResourceCommentReply_listdt.Rows.Count > 0)
                        {
                            int index_ResourceCommentReply_ = 1;
                            msg_reply += "            <div class=\"com-huifu\">";
                            foreach (DataRow item_reply in ResourceCommentReply_listdt.Rows)
                            {
                                string user_name_reply = string.Empty;
                                string user_avatar_reply = "/images/touxiang1.png";
                                user_list user_list_reply = user_listbll.SelectSingle("id=" + Function.ConvertTo<int>(item_reply["user_id"].ToString(), 0));
                                if (user_list_reply != null && user_list_reply.id > 0)
                                {
                                    user_name_reply = Function.HtmlDiscode(user_list_reply.name);
                                    user_avatar_reply = CommonUserFunc.GetUserAvatarFunc(user_list_reply.upload_pic_avatar);
                                }

                                string msg_replyimg = string.Empty;
                                DataTable ResourceCommentReplyImg_listdt = ResourceCommentReplyImg_listbll.GetDatatable("select upload_pic_info from ResourceCommentReplyImg_list where ResourceCommentReply_Id=" + item_reply["id"].ToString() + "  order by orderid asc,addtime asc");
                                if (ResourceCommentReplyImg_listdt != null && ResourceCommentReplyImg_listdt.Rows.Count > 0)
                                {
                                    msg_replyimg += "                        <div class=\"com-img\">";
                                    foreach (DataRow item_ReplyImg in ResourceCommentReplyImg_listdt.Rows)
                                    {
                                        msg_replyimg += "<div class=\"com-img-item\"><img src=\"" + GetWebUpload_Pic(item_ReplyImg["upload_pic_info"].ToString(), "/images/null.png") + "\" /></div>";
                                    }
                                    msg_replyimg += "                        </div>";
                                }
                                ResourceCommentReplyImg_listdt.Dispose();



                                msg_reply += "                <div class=\"huifu-item " + (index_ResourceCommentReply_ > 3 ? " huifu-item-hide" : "") + "\">";
                                msg_reply += "                    <div class=\"huifu-l\">";
                                msg_reply += "                        <img src=\"" + user_avatar_reply + "\" />";
                                msg_reply += "                    </div>";
                                msg_reply += "                    <div class=\"huifu-r\">";
                                msg_reply += "                        <h4>" + user_name_reply + "</h4>";
                                msg_reply += "                        <p>" + Function.HtmlDiscodeWeb(item_reply["info_"].ToString()) + "</p>";
                                msg_reply += msg_replyimg;
                                msg_reply += "                        <div class=\"huifu-data\">" + Function.ConvertTo<DateTime>(item_reply["addtime"].ToString(), DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss") + " </div>";
                                msg_reply += "                    </div>";
                                msg_reply += "                </div>";

                                index_ResourceCommentReply_++;
                            }
                            msg_reply += "                <div class=\"huifu-m\">";
                            msg_reply += "                    <button>共 " + ResourceCommentReply_listdt.Rows.Count + " 回复，查看更多</button></div>";
                            msg_reply += "            </div>";
                        }
                        ResourceCommentReply_listdt.Dispose();


                        pagehtml += "<div class=\"com-item data-item-box\">";
                        pagehtml += "        <div class=\"com-l\">";
                        pagehtml += "            <img src=\"" + user_avatar + "\" />";
                        pagehtml += "        </div>";
                        pagehtml += "        <div class=\"com-r\">";
                        pagehtml += "            <h4>" + user_name + "</h4>";
                        pagehtml += "            <p>" + Function.HtmlDiscodeWeb(item["info_"].ToString()) + "</p>";
                        pagehtml += msg_img;
                        pagehtml += "            <div class=\"com-but\"  id=\"comment_div_" + item["id"].ToString() + "\">";
                        pagehtml += "                <div class=\"com-but-text\">" + Function.ConvertTo<DateTime>(item["addtime"].ToString(), DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss") + " </div>";
                        pagehtml += "                <div class=\"pe-li-icon\" >";
                        bool iszan = false;
                        ResourceCommentLike_List ResourceCommentLike_List = ResourceCommentLike_Listbll.SelectSingle("comment_id=" + item["id"].ToString() + " and user_id=" + user_id);
                        if (ResourceCommentLike_List != null && ResourceCommentLike_List.comment_id > 0)
                        {
                            iszan = true;
                        }

                        pagehtml += "                    <span class=\"like btn-like-svg" + (iszan ? " selected" : "") + "\" onclick=\"MsgZanFunc(this)\" data-id=\"" + item["id"].ToString() + "\">";
                        if (iszan)
                        {
                            pagehtml += "                     <svg t=\"1766646701304\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"12394\" width=\"16\" height=\"16\"><path d=\"M213.57056 399.95392c-48.8448 0-88.58112 39.72096-88.58112 88.54528l0 334.75584c0 48.8192 39.73632 88.54528 88.58112 88.54528l56.08448 0 0-511.8464L213.57056 399.95392z\" fill=\"#232323\" p-id=\"12395\"></path><path d=\"M821.43232 381.09184l-189.33248 0c10.34752-28.83072 17.17248-60.16512 19.61472-90.48064 3.77856-47.14496-3.47136-89.02144-20.96128-121.09312-21.5296-39.45984-58.17344-64.07168-105.99424-71.17824-21.6576-3.22048-40.28928 0.95232-55.3728 12.37504-30.49472 23.10656-36.09088 67.06688-42.5728 117.96992-5.18144 40.69376-11.05408 86.81472-30.79168 119.87456-9.86112 16.52736-26.72128 37.11488-64.06144 46.41792l0 516.82304 415.96416 0c24.33024 0 48.22016-8.32512 67.2768-23.42912 19.06688-15.10912 32.60416-36.46976 38.144-60.14976l73.51808-314.27072c7.57248-32.34816 0.06656-65.83296-20.58752-91.86816C885.61152 396.032 854.69184 381.09184 821.43232 381.09184z\" fill=\"#232323\" p-id=\"12396\"></path></svg>";
                        }
                        else
                        {
                            pagehtml += "                     <svg t=\"1766643700342\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"2333\" width=\"16\" height=\"16\"><path d=\"M853.333333 332.8h-234.666666c0-17.066667 4.266667-34.133333 4.266666-55.466667 0-42.666667-8.533333-98.133333-34.133333-145.066666-38.4-64-89.6-93.866667-123.733333-106.666667C413.866667 8.533333 375.466667 42.666667 362.666667 76.8L260.266667 405.333333l-8.533334 8.533334H170.666667c-72.533333 0-128 55.466667-128 128v332.8c0 72.533333 55.466667 128 128 128h622.933333c64 0 115.2-46.933333 128-110.933334l64-418.133333c4.266667-72.533333-55.466667-140.8-132.266667-140.8zM251.733333 917.333333H170.666667c-25.6 0-42.666667-17.066667-42.666667-42.666666v-332.8c0-25.6 17.066667-42.666667 42.666667-42.666667h85.333333l-4.266667 418.133333z m580.266667-34.133333c-4.266667 21.333333-21.333333 38.4-42.666667 38.4H337.066667L341.333333 435.2l102.4-324.266667c21.333333 8.533333 46.933333 25.6 68.266667 64 17.066667 25.6 21.333333 64 21.333333 98.133334 0 38.4-4.266667 72.533333-8.533333 89.6L512 418.133333h341.333333c25.6 0 46.933333 21.333333 42.666667 46.933334l-64 418.133333z\" fill=\"#666666\" p-id=\"2334\"></path></svg>";
                        }
                        pagehtml += "                   " + FormatNumber(item["num_dianzan"].ToString()) + "</span>";
                        pagehtml += "                    <span class=\"huifu\" onclick=\"MsgReplyFunc(this)\" data-id=\"" + item["id"].ToString() + "\">";
                        pagehtml += "                    <svg t=\"1769413509666\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"17049\" width=\"18\" height=\"18\"><path d=\"M201.1 913.6c-1.2 0-2.4 0-3.6-0.1-11.1-1-21.6-5.2-31.2-12.5-9.3-7.7-15.9-17.2-19.7-28.4-4-10.7-4.8-22.3-2.4-34.5l24.2-111.3c-24.7-31-43.8-64.3-56.8-98.8C95.3 586.2 87 542.7 87 498.6c0-104.8 44.9-202.7 126.5-275.9 38.8-35.1 83.9-62.7 134.2-81.9 52-19.9 107.2-30 164-30 112.3 0 218 39.8 297.7 112 39.6 35.7 70.8 77.3 92.5 123.7 22.6 48.1 34 99.3 34 152.2 0 52.8-11.4 104-34 152.1-21.8 46.5-52.9 88.1-92.5 123.6-38.7 35.2-83.8 62.8-134.1 82-51.9 19.9-106.9 29.9-163.6 29.9-33.8 0-67.9-3.8-101.6-11.4-28.8-6.2-55.6-14.9-79.7-25.7l-100.5 56.9c-9.5 4.9-19.2 7.5-28.8 7.5z m29.4-223.2c9.1 9 12.8 22 10 34.7l-22.8 105.5 94.9-53.9c5.2-2.8 10.9-4.3 16.6-4.3 5 0 9.7 1.1 14.1 3.2 26.5 12.9 53.9 22.4 81.4 28.3 27.4 6.2 56.6 9.4 87 9.4 95.5 0 185.3-33.4 252.7-94.1 31.9-28.6 57-62.1 74.5-99.4 18-38.5 27.2-79.3 27.2-121.3s-9.1-82.8-27.2-121.3c-17.5-37.3-42.6-70.9-74.5-99.7-67.7-60.7-157.4-94.1-252.7-94.1-95.5 0-185.4 33.4-253.1 94.1-31.8 28.9-56.9 62.4-74.5 99.7-18.2 38.6-27.5 79.5-27.5 121.3 0 34.7 6.5 68.9 19.2 101.8 12.9 33.1 31.2 63.4 54.7 90.1z m453.3-124.8c-13.2 0-26.2-5.7-36.5-16.1-9.7-10.2-15-23.6-15-37.8 0-14.1 5.3-27.6 14.9-38.1 10.1-10.2 23.1-15.8 36.6-15.8s26.5 5.6 36.5 15.7c9.7 10.6 15 24.1 15 38.2 0 14.3-5.3 27.7-15 37.8-10.3 10.4-23.2 16.1-36.5 16.1z m-172 0c-13.7 0-26.6-5.7-36.5-16.1-9.8-10.3-15.1-23.7-15.1-37.8 0-13.9 5.4-27.5 15.1-38.1 9.7-10.2 22.7-15.8 36.6-15.8 13.7 0 26.5 5.6 36.2 15.7 9.6 10.3 15 23.9 15 38.2 0 14.5-5.3 27.9-15 37.8-10 10.4-22.8 16.1-36.3 16.1z m-168.7 0c-13.4 0-26.6-5.9-36.3-16.1-9.6-9.9-14.9-23.3-14.9-37.8 0-14.3 5.3-27.8 14.9-38.1 9.7-10.2 22.6-15.8 36.4-15.8 13.6 0 26.5 5.6 36.4 15.7 9.8 10.5 15.1 24 15.1 38.2 0 14.3-5.4 27.8-15.1 37.8-10.2 10.4-23.2 16.1-36.5 16.1z\" p-id=\"17050\" fill=\"#232323\"></path></svg>";
                        pagehtml += "                      回复(" + FormatNumber(item["num_msg"].ToString()) + ")</span>";
                        pagehtml += "                </div>";
                        pagehtml += "            </div>";
                        pagehtml += msg_reply;
                        pagehtml += "        </div>";
                        pagehtml += "    </div>";
                    }
                }
                ResourceComment_listdt.Dispose();
                str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["pagehtml"] = pagehtml });
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
            return str_;
        }
        

        public static string GetWorkflowModelMsgPageFunc(int GeneratedAssetRecord_id, int pageindex, int type, int user_id)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                string strWhere = " GeneratedAssetRecord_id=" + GeneratedAssetRecord_id + " and status=1";

                //表或视图名
                string tblName = "WorkflowTaskComment_list";
                //需要返回的列
                string strGetFields = " id, info_, addtime, user_id, num_dianzan,num_msg";
                //排序的字段名
                string fldname = " addtime desc,id desc";
                if (type == 2)
                {
                    fldname = " num_dianzan desc,addtime desc,id desc";
                }
                else if (type == 3)
                {
                    fldname = " num_msg desc,addtime desc,id desc";
                }
                //每页显示的记录数
                int page_Size = 20;
                string pagehtml = string.Empty;
                DataTable WorkflowTaskComment_listdt = WorkflowTaskComment_listbll.GetListByPage(tblName, strGetFields, fldname, page_Size, pageindex, strWhere);
                if (WorkflowTaskComment_listdt != null && WorkflowTaskComment_listdt.Rows.Count > 0)
                {
                    foreach (DataRow item in WorkflowTaskComment_listdt.Rows)
                    {
                        string user_name = string.Empty;
                        string user_avatar = "/images/touxiang1.png";
                        user_list user_list = user_listbll.SelectSingle("id=" + Function.ConvertTo<int>(item["user_id"].ToString(), 0));
                        if (user_list != null && user_list.id > 0)
                        {
                            user_name = Function.HtmlDiscode(user_list.name);
                            user_avatar = CommonUserFunc.GetUserAvatarFunc(user_list.upload_pic_avatar);
                        }
                        string msg_img = string.Empty;
                        DataTable WorkflowTaskCommentImage_listdt = WorkflowTaskCommentImage_listbll.GetDatatable("select upload_pic_info from WorkflowTaskCommentImage_list where WorkflowTaskComment_Id=" + item["id"].ToString() + "  order by orderid asc,addtime asc");
                        if (WorkflowTaskCommentImage_listdt != null && WorkflowTaskCommentImage_listdt.Rows.Count > 0)
                        {

                            msg_img += "            <div class=\"com-img\">";
                            foreach (DataRow item_img in WorkflowTaskCommentImage_listdt.Rows)
                            {
                                msg_img += "                <div class=\"com-img-item\"> <img src=\"" + GetWebUpload_Pic(item_img["upload_pic_info"].ToString(), "/images/null.png") + "\" /></div>";
                            }
                            msg_img += "            </div>";
                        }
                        WorkflowTaskCommentImage_listdt.Dispose();


                        string msg_reply = string.Empty;
                        DataTable WorkflowTaskReply_listdt = WorkflowTaskReply_listbll.GetDatatable("select id, info_, addtime, user_id from WorkflowTaskReply_list where task_comment_id=" + item["id"].ToString() + " and status=1 order by addtime desc, id desc");
                        if (WorkflowTaskReply_listdt != null && WorkflowTaskReply_listdt.Rows.Count > 0)
                        {
                            int index_ResourceCommentReply_ = 1;
                            msg_reply += "            <div class=\"com-huifu\">";
                            foreach (DataRow item_reply in WorkflowTaskReply_listdt.Rows)
                            {
                                string user_name_reply = string.Empty;
                                string user_avatar_reply = "/images/touxiang1.png";
                                user_list user_list_reply = user_listbll.SelectSingle("id=" + Function.ConvertTo<int>(item_reply["user_id"].ToString(), 0));
                                if (user_list_reply != null && user_list_reply.id > 0)
                                {
                                    user_name_reply = Function.HtmlDiscode(user_list_reply.name);
                                    user_avatar_reply = CommonUserFunc.GetUserAvatarFunc(user_list_reply.upload_pic_avatar);
                                }

                                string msg_replyimg = string.Empty;
                                DataTable WorkflowTaskReplyImage_listdt = WorkflowTaskReplyImage_listbll.GetDatatable("select upload_pic_info from WorkflowTaskReplyImage_list where WorkflowTaskReply_Id=" + item_reply["id"].ToString() + "  order by orderid asc,addtime asc");
                                if (WorkflowTaskReplyImage_listdt != null && WorkflowTaskReplyImage_listdt.Rows.Count > 0)
                                {
                                    msg_replyimg += "                        <div class=\"com-img\">";
                                    foreach (DataRow item_ReplyImg in WorkflowTaskReplyImage_listdt.Rows)
                                    {
                                        msg_replyimg += "<div class=\"com-img-item\"><img src=\"" + GetWebUpload_Pic(item_ReplyImg["upload_pic_info"].ToString(), "/images/null.png") + "\" /></div>";
                                    }
                                    msg_replyimg += "                        </div>";
                                }
                                WorkflowTaskReplyImage_listdt.Dispose();



                                msg_reply += "                <div class=\"huifu-item " + (index_ResourceCommentReply_ > 3 ? " huifu-item-hide" : "") + "\">";
                                msg_reply += "                    <div class=\"huifu-l\">";
                                msg_reply += "                        <img src=\"" + user_avatar_reply + "\" />";
                                msg_reply += "                    </div>";
                                msg_reply += "                    <div class=\"huifu-r\">";
                                msg_reply += "                        <h4>" + user_name_reply + "</h4>";
                                msg_reply += "                        <p>" + Function.HtmlDiscodeWeb(item_reply["info_"].ToString()) + "</p>";
                                msg_reply += msg_replyimg;
                                msg_reply += "                        <div class=\"huifu-data\">" + Function.ConvertTo<DateTime>(item_reply["addtime"].ToString(), DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss") + " </div>";
                                msg_reply += "                    </div>";
                                msg_reply += "                </div>";

                                index_ResourceCommentReply_++;
                            }
                            msg_reply += "                <div class=\"huifu-m\">";
                            msg_reply += "                    <button>共 " + WorkflowTaskReply_listdt.Rows.Count + " 回复，查看更多</button></div>";
                            msg_reply += "            </div>";
                        }
                        WorkflowTaskReply_listdt.Dispose();


                        pagehtml += "<div class=\"com-item data-item-box\">";
                        pagehtml += "        <div class=\"com-l\">";
                        pagehtml += "            <img src=\"" + user_avatar + "\" />";
                        pagehtml += "        </div>";
                        pagehtml += "        <div class=\"com-r\">";
                        pagehtml += "            <h4>" + user_name + "</h4>";
                        pagehtml += "            <p>" + Function.HtmlDiscodeWeb(item["info_"].ToString()) + "</p>";
                        pagehtml += msg_img;
                        pagehtml += "            <div class=\"com-but\"  id=\"comment_div_" + item["id"].ToString() + "\">";
                        pagehtml += "                <div class=\"com-but-text\">" + Function.ConvertTo<DateTime>(item["addtime"].ToString(), DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss") + " </div>";
                        pagehtml += "                <div class=\"pe-li-icon\" >";
                        bool iszan = false;
                        WorkflowTaskReaction_list WorkflowTaskReaction_list = WorkflowTaskReaction_listbll.SelectSingle("task_comment_id=" + item["id"].ToString() + " and user_id=" + user_id);
                        if (WorkflowTaskReaction_list != null && WorkflowTaskReaction_list.task_comment_id > 0)
                        {
                            iszan = true;
                        }

                        pagehtml += "                    <span class=\"like " + (iszan ? " selected" : "") + "\" onclick=\"MsgZanFunc(this)\" data-id=\"" + item["id"].ToString() + "\">";
                        if (iszan)
                        {
                        pagehtml += "                     <svg t=\"1766646701304\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"12394\" width=\"16\" height=\"16\"><path d=\"M213.57056 399.95392c-48.8448 0-88.58112 39.72096-88.58112 88.54528l0 334.75584c0 48.8192 39.73632 88.54528 88.58112 88.54528l56.08448 0 0-511.8464L213.57056 399.95392z\" fill=\"#232323\" p-id=\"12395\"></path><path d=\"M821.43232 381.09184l-189.33248 0c10.34752-28.83072 17.17248-60.16512 19.61472-90.48064 3.77856-47.14496-3.47136-89.02144-20.96128-121.09312-21.5296-39.45984-58.17344-64.07168-105.99424-71.17824-21.6576-3.22048-40.28928 0.95232-55.3728 12.37504-30.49472 23.10656-36.09088 67.06688-42.5728 117.96992-5.18144 40.69376-11.05408 86.81472-30.79168 119.87456-9.86112 16.52736-26.72128 37.11488-64.06144 46.41792l0 516.82304 415.96416 0c24.33024 0 48.22016-8.32512 67.2768-23.42912 19.06688-15.10912 32.60416-36.46976 38.144-60.14976l73.51808-314.27072c7.57248-32.34816 0.06656-65.83296-20.58752-91.86816C885.61152 396.032 854.69184 381.09184 821.43232 381.09184z\" fill=\"#232323\" p-id=\"12396\"></path></svg>";   
                        }else{
                        pagehtml += "                     <svg t=\"1766643700342\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"2333\" width=\"16\" height=\"16\"><path d=\"M853.333333 332.8h-234.666666c0-17.066667 4.266667-34.133333 4.266666-55.466667 0-42.666667-8.533333-98.133333-34.133333-145.066666-38.4-64-89.6-93.866667-123.733333-106.666667C413.866667 8.533333 375.466667 42.666667 362.666667 76.8L260.266667 405.333333l-8.533334 8.533334H170.666667c-72.533333 0-128 55.466667-128 128v332.8c0 72.533333 55.466667 128 128 128h622.933333c64 0 115.2-46.933333 128-110.933334l64-418.133333c4.266667-72.533333-55.466667-140.8-132.266667-140.8zM251.733333 917.333333H170.666667c-25.6 0-42.666667-17.066667-42.666667-42.666666v-332.8c0-25.6 17.066667-42.666667 42.666667-42.666667h85.333333l-4.266667 418.133333z m580.266667-34.133333c-4.266667 21.333333-21.333333 38.4-42.666667 38.4H337.066667L341.333333 435.2l102.4-324.266667c21.333333 8.533333 46.933333 25.6 68.266667 64 17.066667 25.6 21.333333 64 21.333333 98.133334 0 38.4-4.266667 72.533333-8.533333 89.6L512 418.133333h341.333333c25.6 0 46.933333 21.333333 42.666667 46.933334l-64 418.133333z\" fill=\"#666666\" p-id=\"2334\"></path></svg>";   
                        }
                        pagehtml += "                   "+ FormatNumber(item["num_dianzan"].ToString()) + "</span>";
                        pagehtml += "                    <span class=\"huifu\" onclick=\"MsgReplyFunc(this)\" data-id=\"" + item["id"].ToString() + "\">";
                        pagehtml += "                    <svg t=\"1769413509666\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"17049\" width=\"18\" height=\"18\"><path d=\"M201.1 913.6c-1.2 0-2.4 0-3.6-0.1-11.1-1-21.6-5.2-31.2-12.5-9.3-7.7-15.9-17.2-19.7-28.4-4-10.7-4.8-22.3-2.4-34.5l24.2-111.3c-24.7-31-43.8-64.3-56.8-98.8C95.3 586.2 87 542.7 87 498.6c0-104.8 44.9-202.7 126.5-275.9 38.8-35.1 83.9-62.7 134.2-81.9 52-19.9 107.2-30 164-30 112.3 0 218 39.8 297.7 112 39.6 35.7 70.8 77.3 92.5 123.7 22.6 48.1 34 99.3 34 152.2 0 52.8-11.4 104-34 152.1-21.8 46.5-52.9 88.1-92.5 123.6-38.7 35.2-83.8 62.8-134.1 82-51.9 19.9-106.9 29.9-163.6 29.9-33.8 0-67.9-3.8-101.6-11.4-28.8-6.2-55.6-14.9-79.7-25.7l-100.5 56.9c-9.5 4.9-19.2 7.5-28.8 7.5z m29.4-223.2c9.1 9 12.8 22 10 34.7l-22.8 105.5 94.9-53.9c5.2-2.8 10.9-4.3 16.6-4.3 5 0 9.7 1.1 14.1 3.2 26.5 12.9 53.9 22.4 81.4 28.3 27.4 6.2 56.6 9.4 87 9.4 95.5 0 185.3-33.4 252.7-94.1 31.9-28.6 57-62.1 74.5-99.4 18-38.5 27.2-79.3 27.2-121.3s-9.1-82.8-27.2-121.3c-17.5-37.3-42.6-70.9-74.5-99.7-67.7-60.7-157.4-94.1-252.7-94.1-95.5 0-185.4 33.4-253.1 94.1-31.8 28.9-56.9 62.4-74.5 99.7-18.2 38.6-27.5 79.5-27.5 121.3 0 34.7 6.5 68.9 19.2 101.8 12.9 33.1 31.2 63.4 54.7 90.1z m453.3-124.8c-13.2 0-26.2-5.7-36.5-16.1-9.7-10.2-15-23.6-15-37.8 0-14.1 5.3-27.6 14.9-38.1 10.1-10.2 23.1-15.8 36.6-15.8s26.5 5.6 36.5 15.7c9.7 10.6 15 24.1 15 38.2 0 14.3-5.3 27.7-15 37.8-10.3 10.4-23.2 16.1-36.5 16.1z m-172 0c-13.7 0-26.6-5.7-36.5-16.1-9.8-10.3-15.1-23.7-15.1-37.8 0-13.9 5.4-27.5 15.1-38.1 9.7-10.2 22.7-15.8 36.6-15.8 13.7 0 26.5 5.6 36.2 15.7 9.6 10.3 15 23.9 15 38.2 0 14.5-5.3 27.9-15 37.8-10 10.4-22.8 16.1-36.3 16.1z m-168.7 0c-13.4 0-26.6-5.9-36.3-16.1-9.6-9.9-14.9-23.3-14.9-37.8 0-14.3 5.3-27.8 14.9-38.1 9.7-10.2 22.6-15.8 36.4-15.8 13.6 0 26.5 5.6 36.4 15.7 9.8 10.5 15.1 24 15.1 38.2 0 14.3-5.4 27.8-15.1 37.8-10.2 10.4-23.2 16.1-36.5 16.1z\" p-id=\"17050\" fill=\"#232323\"></path></svg>";
                        pagehtml += "                      回复(" + FormatNumber(item["num_msg"].ToString()) + ")</span>";
                        pagehtml += "                </div>";
                        pagehtml += "            </div>";
                        pagehtml +=            msg_reply;
                        pagehtml += "        </div>";
                        pagehtml += "    </div>";
                    }
                }
                WorkflowTaskComment_listdt.Dispose();
                str_ = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["pagehtml"] = pagehtml });
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
            return str_;
        }

        public class emailjson
        {
            public string email_to { get; set; }
            public string subject { get; set; }
            public string body { get; set; }
            public string emailname { get; set; }
            public string emailnum { get; set; }
            public string emailpasswd { get; set; }
            public int smtpserverport { get; set; }
            public string host { get; set; }

        }
        public static void ToEmail(string Subject, string Body, string email_to)
        {
            try
            {
                websiteinfo_list websiteinfo_list = websiteinfo_listbll.SelectSingle("id=1");
                if (websiteinfo_list != null && !string.IsNullOrWhiteSpace(websiteinfo_list.host) && !string.IsNullOrWhiteSpace(websiteinfo_list.emailnum) && !string.IsNullOrWhiteSpace(websiteinfo_list.emailpasswd) && !string.IsNullOrWhiteSpace(websiteinfo_list.smtpserverport) && !string.IsNullOrWhiteSpace(websiteinfo_list.emailname))
                {
                    emailjson emailjson = new emailjson();
                    if (string.IsNullOrWhiteSpace(email_to))
                    {
                        emailjson.email_to = Function.HtmlDiscode(websiteinfo_list.email_to);
                    }
                    else
                    {
                        emailjson.email_to = Function.HtmlDiscode(email_to);
                    }
                    emailjson.subject = Subject;
                    emailjson.body = Body;
                    emailjson.emailname = Function.HtmlDiscode(websiteinfo_list.emailname);
                    emailjson.emailnum = Function.HtmlDiscode(websiteinfo_list.emailnum);
                    emailjson.emailpasswd = Function.HtmlDiscode(websiteinfo_list.emailpasswd);
                    emailjson.smtpserverport = Function.ConvertTo<int>(websiteinfo_list.smtpserverport, 0);
                    emailjson.host = Function.HtmlDiscode(websiteinfo_list.host);

                    ParameterizedThreadStart pts = new ParameterizedThreadStart(SayEmailFunc_);
                    Thread td2 = new Thread(pts);
                    td2.Start(emailjson);
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "CommonFunc.ToOrderEmail:" + ex.Message + "-" + ex.StackTrace);
            }
        }
        public static void SayEmailFunc_(object emailjson_)
        {
            try
            {
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                emailjson emailjson = Function.ConvertTo<emailjson>(emailjson_, null);
                if (emailjson != null && !string.IsNullOrWhiteSpace(emailjson.email_to) && !string.IsNullOrWhiteSpace(emailjson.emailname) && !string.IsNullOrWhiteSpace(emailjson.emailnum) && !string.IsNullOrWhiteSpace(emailjson.emailpasswd) && emailjson.smtpserverport > 0 && !string.IsNullOrWhiteSpace(emailjson.host))
                {
                    try
                    {
                        // 创建 SmtpClient 对象用于发送邮件
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = Function.HtmlDiscode(emailjson.host); // SMTP服务器地址
                        smtp.Port = emailjson.smtpserverport; // SMTP服务器端口号，例如587（可能需要SSL/TLS）或25（非SSL）
                        smtp.Timeout = UploadPolicy.ExternalRequestTimeoutMs;
                        smtp.EnableSsl = true; // 如果使用SSL/TLS，则设置为true
                        //smtp.DeliveryMethod = SmtpDeliveryMethod.Network; // 使用网络发送邮件
                        smtp.UseDefaultCredentials = false; // 不使用默认的Windows账户认证
                        smtp.Credentials = new NetworkCredential(Function.HtmlDiscode(emailjson.emailnum), Function.HtmlDiscode(emailjson.emailpasswd)); // SMTP服务器认证信息
                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(Function.HtmlDiscode(emailjson.emailnum), Function.HtmlDiscode(emailjson.emailname));
                        string[] email_to_ = Function.HtmlDiscode(emailjson.email_to).Split(',');
                        foreach (string item in email_to_)
                        {
                            mail.To.Add(new MailAddress(Function.HtmlDiscode(item)));
                        }
                        mail.Subject = Function.HtmlDiscode(emailjson.subject); // 邮件主题
                        mail.BodyEncoding = Encoding.UTF8;
                        mail.IsBodyHtml = true;
                        mail.Body = Function.HtmlDiscode(emailjson.body);// 邮件正文

                        smtp.Send(mail);

                    }
                    catch (SmtpException ex)
                    {
                        ImportDataLog.WriteLog(LogType.Error, "SMTP_err:" + ex.Message);

                    }
                    catch (Exception ex)
                    {
                        ImportDataLog.WriteLog(LogType.Error, "Error_err:" + ex.Message);
                    }



                    //MailMessage mmsg = new MailMessage();
                    ////邮件主题
                    //mmsg.Subject = Function.HtmlDiscode(emailjson.subject);
                    //mmsg.BodyFormat = MailFormat.Html;
                    ////邮件正文
                    //mmsg.Body = Function.HtmlDiscode(emailjson.body);
                    ////正文编码
                    //mmsg.BodyEncoding = Encoding.UTF8;
                    ////优先级
                    //mmsg.Priority = MailPriority.High;
                    ////发件者邮箱地址
                    //mmsg.From = "\"" + emailjson.emailname + "\" <" + emailjson.emailnum + ">";                                                          //收件人收箱地址
                    //mmsg.To = Function.HtmlDiscode(emailjson.email_to);//收件人地址


                    ////if (!string.IsNullOrWhiteSpace(filePath))
                    ////{
                    ////    MailAttachment oAttch = new MailAttachment(filePath, MailEncoding.Base64);
                    ////    mmsg.Attachments.Add(oAttch);
                    ////}


                    //mmsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
                    ////用户名
                    //mmsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", Function.HtmlDiscode(emailjson.emailnum));
                    ////密码
                    //mmsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", Function.HtmlDiscode(emailjson.emailpasswd));
                    ////端口 
                    //mmsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", Function.HtmlDiscode(emailjson.smtpserverport));
                    ////使用SSL 
                    //mmsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", true);
                    ////Smtp服务器
                    //SmtpMail.SmtpServer = Function.HtmlDiscode(emailjson.host);
                    //SmtpMail.Send(mmsg);
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "CommonFunc.SayEmailFunc_Error:" + ex.Message + "-" + ex.StackTrace);
            }
        }
        public static string GetWorkflowModelMsgNumFunc(string GeneratedAssetRecord_id)
        {
            string num = "0";
            DataTable WorkflowTaskComment_list_dt = WorkflowTaskComment_listbll.GetDatatable("select count(1) as num from WorkflowTaskComment_list where GeneratedAssetRecord_id=" + Function.ConvertTo<int>(GeneratedAssetRecord_id, 0) + " and status=1");
            if (WorkflowTaskComment_list_dt != null && WorkflowTaskComment_list_dt.Rows.Count > 0)
            {
                num = FormatNumber(Function.ConvertTo<int>(WorkflowTaskComment_list_dt.Rows[0]["num"].ToString(), 0).ToString());
            }
            WorkflowTaskComment_list_dt.Dispose();
            return num;
        }
        
        public static string GetResourceCommentNumFunc(string Resource_id)
        {
            string num = "0";
            DataTable ResourceComment_list_dt = null;
            try
            {
                ResourceComment_list_dt = ResourceComment_listbll.GetDatatable("select count(1) as num from ResourceComment_list where Resource_id=" + Function.ConvertTo<int>(Resource_id, 0) + " and status=1");
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("status", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    throw;
                }
                ResourceComment_list_dt = ResourceComment_listbll.GetDatatable("select count(1) as num from ResourceComment_list where Resource_id=" + Function.ConvertTo<int>(Resource_id, 0));
            }
            if (ResourceComment_list_dt != null && ResourceComment_list_dt.Rows.Count > 0)
            {
                num = FormatNumber(Function.ConvertTo<int>(ResourceComment_list_dt.Rows[0]["num"].ToString(), 0).ToString());
            }
            ResourceComment_list_dt.Dispose();
            return num;
        }

        public static string GetResourceHomeClassDataNum(string ResourceHomeClass_id)
        {
            string num = "0";
            DataTable Resource_list_dt = Resource_listbll.GetDatatable("select count(1) as num from Resource_list where id in(select resource_id from ResourceHomeClassData_List where ResourceHomeClass_id=" + Function.ConvertTo<int>(ResourceHomeClass_id, 0) + ") and status=1 and isshow=1");
            if (Resource_list_dt != null && Resource_list_dt.Rows.Count > 0)
            {
                num = FormatNumber(Function.ConvertTo<int>(Resource_list_dt.Rows[0]["num"].ToString(), 0).ToString());
            }
            Resource_list_dt.Dispose();
            return num;
        }
        public static string GetResourceDataHtml(DataTable Resource_list_dt, user_list user_list)
        {
            string str = string.Empty;
            foreach (DataRow item in Resource_list_dt.Rows)
            {
                string avatar_img = "/images/touxiang1.png";
                string user_name = "游客";
                bool isCollect = false;
                if (user_list != null && user_list.id > 0)
                {
                    ResourceCollect_List ResourceCollect_List = ResourceCollect_Listbll.SelectSingle("resource_id=" + item["id"].ToString() + " and user_id=" + user_list.id);
                    if (ResourceCollect_List != null && ResourceCollect_List.resource_id > 0)
                    {
                        isCollect = true;
                    }
                }

                user_list user_list_model = user_listbll.SelectSingle("id=" + item["userid"].ToString());
                if (user_list_model != null && user_list_model.id > 0)
                {
                    user_name = Function.HtmlDiscode(user_list_model.name);
                    if (GetImgBool(user_list_model.upload_pic_avatar))
                    {
                        avatar_img = A_UpLoad_Url + user_list_model.upload_pic_avatar;
                    }
                }

                string datatext = string.Empty;
                string dataimg = string.Empty;
                if (item["ishot"].ToString() == "1")
                {
                    datatext = "热门文献";
                    dataimg = "/images/huo.png";
                }
                if (string.IsNullOrWhiteSpace(datatext) || string.IsNullOrWhiteSpace(dataimg))
                {

                    if (!string.IsNullOrWhiteSpace(item["ResourceFormatTag_idstr"].ToString()))
                    {

                        ResourceFormatTag_List ResourceFormatTag_List = ResourceFormatTag_Listbll.SelectSingle("id in(" + item["ResourceFormatTag_idstr"].ToString() + ") order by OrderId asc,UpTime desc,AddTime desc,Id desc");
                        if (ResourceFormatTag_List != null && ResourceFormatTag_List.Id > 0)
                        {
                            datatext = Function.HtmlDiscode(ResourceFormatTag_List.Name);
                            if (GetImgBool(ResourceFormatTag_List.Upload_Pic_Img))
                            {
                                dataimg = "/A_UpLoad/upload_pic/" + ResourceFormatTag_List.Upload_Pic_Img;
                            }
                        }
                    }
                }
                bool isimg_gif = false;
                if (GetImgBool(item["upload_pic_gif"].ToString()))
                {
                    isimg_gif = true;
                }

                str += "<div class=\"pe-li-item\">";
                str += "	<div class=\"pe-li-img\">";
                str += "		<a href=\"/Models_" + item["id"].ToString() + "\" target=\"_blank\">";
                str += "			<img class=\"pe-li-gif\"  data-gif=\"" + (isimg_gif ? "/A_UpLoad/upload_pic/" + item["upload_pic_gif"].ToString() : "") + "\"";
                str += "			data-img=\"" + GetWebUpload_Pic(item["upload_pic_cover"].ToString(), "/images/modelnull.jpg") + "\"";
                str += "			src=\"" + GetWebUpload_Pic(item["upload_pic_cover"].ToString(), "/images/modelnull.jpg") + "\"";
                str += "			/>";
                str += "		</a>";

                if (isimg_gif)
                {
                    str += "		<div class=\"mw-css-16mljcb\">";
                    str += "			<span>";
                    str += "				G";
                    str += "			</span>";
                    str += "			<span>";
                    str += "				I";
                    str += "			</span>";
                    str += "			<span>";
                    str += "				F";
                    str += "			</span>";
                    str += "		</div>";
                }

                if (!string.IsNullOrWhiteSpace(datatext) && !string.IsNullOrWhiteSpace(dataimg))
                {
                    str += "    <div class=\"tips\" data-text=\"" + datatext + "\">";
                    str += "        <img src=\"" + dataimg + "\" />";
                    str += "    </div>";
                }

                str += "		<div class=\"shoucang " + (isCollect ? " selected" : "") + "\" onclick=\"ResourceCollectFunc(this)\" data-id=\"" + item["id"].ToString() + "\">";
                str += "			<div class=\"icon\">";
                str += "			</div>";
                str += "		</div>";
                str += "	</div>";
                str += "	<div class=\"pe-li-xinxi\">";
                str += "		<a href=\"/Models_" + item["id"].ToString() + "\" target=\"_blank\" class=\"pe-xinxi-img\">";
                str += "			<img src=\"" + avatar_img + "\">";
                str += "		</a>";
                str += "		<div class=\"pe-xinxi-text\">";
                str += "			<h4>";
                str += "				<a href=\"/Models_" + item["id"].ToString() + "\" target=\"_blank\">" + Function.HtmlDiscode(item["name"].ToString()) + "</a>";
                str += "			</h4>";
                str += "			<p>";
                str += "				<a href=\"/Models_" + item["id"].ToString() + "\" target=\"_blank\">" + user_name + "</a>";
                str += "			</p>";
                str += "			<div class=\"pe-li-icon\">";
                str += "				<span class=\"like\">";
                str += "                  <svg t=\"1766643700342\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"2333\" width=\"16\" height=\"16\"><path d=\"M853.333333 332.8h-234.666666c0-17.066667 4.266667-34.133333 4.266666-55.466667 0-42.666667-8.533333-98.133333-34.133333-145.066666-38.4-64-89.6-93.866667-123.733333-106.666667C413.866667 8.533333 375.466667 42.666667 362.666667 76.8L260.266667 405.333333l-8.533334 8.533334H170.666667c-72.533333 0-128 55.466667-128 128v332.8c0 72.533333 55.466667 128 128 128h622.933333c64 0 115.2-46.933333 128-110.933334l64-418.133333c4.266667-72.533333-55.466667-140.8-132.266667-140.8zM251.733333 917.333333H170.666667c-25.6 0-42.666667-17.066667-42.666667-42.666666v-332.8c0-25.6 17.066667-42.666667 42.666667-42.666667h85.333333l-4.266667 418.133333z m580.266667-34.133333c-4.266667 21.333333-21.333333 38.4-42.666667 38.4H337.066667L341.333333 435.2l102.4-324.266667c21.333333 8.533333 46.933333 25.6 68.266667 64 17.066667 25.6 21.333333 64 21.333333 98.133334 0 38.4-4.266667 72.533333-8.533333 89.6L512 418.133333h341.333333c25.6 0 46.933333 21.333333 42.666667 46.933334l-64 418.133333z\" fill=\"#666666\" p-id=\"2334\"></path></svg>";
                str += "                  "+ FormatNumber(item["num_dianzan"].ToString()) + "</span>";
                str += "				<span>";
                str += "                <svg t=\"1766995208966\" class=\"icon\" viewBox=\"0 0 1047 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"3737\" width=\"16\" height=\"16\"><path d=\"M523.987449 0a42.759353 42.759353 0 0 0-42.759353 42.759353v498.158822L301.663529 361.353608a43.500845 43.500845 0 0 0-60.555153 0 42.759353 42.759353 0 0 0 0 60.431572l227.020033 227.267198a79.092445 79.092445 0 0 0 111.223751 0l227.514362-227.143616a42.635771 42.635771 0 0 0 0-60.555154 42.882935 42.882935 0 0 0-60.555153 0L566.870384 540.918175V42.759353A42.882935 42.882935 0 0 0 523.987449 0z\" fill=\"#666666\" p-id=\"3738\"></path><path d=\"M860.871832 120.245233a42.882935 42.882935 0 1 0 0 85.642288 101.584359 101.584359 0 0 1 101.460777 101.584359v529.301473a101.584359 101.584359 0 0 1-101.460777 101.584359H187.226647a101.707941 101.707941 0 0 1-101.584359-101.584359V307.47188a101.707941 101.707941 0 0 1 101.584359-101.584359 42.882935 42.882935 0 0 0 0-85.642288A187.473811 187.473811 0 0 0 0 307.47188v529.301473a187.473811 187.473811 0 0 0 187.226647 187.226647h673.645185A187.350229 187.350229 0 0 0 1047.974897 836.773353V307.47188A187.350229 187.350229 0 0 0 860.871832 120.245233z\" fill=\"#666666\" p-id=\"3739\"></path></svg>";
                str += "                "+ FormatNumber(item["num_xiazai"].ToString()) + "</span>";
                str += "			</div>";
                str += "		</div>";
                str += "	</div>";
                str += "</div>";
            }
            return str;
        }
        public static string IsProXiaJiaFunc(string Resource_id)
        {
            string str_R = "<img src=\"/images/xiajia.png\" class=\"pe-li-xiajiaimg\"/>";
            Resource_list Resource_list = Resource_listbll.SelectSingle(" id= " + Function.ConvertTo<long>(Resource_id, 0) + " and status=1 and ResourceClass3_id in(select id from ResourceClass3_List where IsShow=1 and ResourceClass2_Id in(select id from ResourceClass2_List where isshow=1 and ResourceClass1_Id in(select id from ResourceClass1_List where isshow=1)))");
            if (Resource_list != null && Resource_list.id > 0)
            {
                str_R = "";
            }
            return str_R;
        }


        public static string GetCollectResourceDataInfoHtml(DataTable ResourceCollect_List_dt, user_list user_list)
        {
            string str = string.Empty;
            foreach (DataRow item_ResourceCollect in ResourceCollect_List_dt.Rows)
            {
                string avatar_img = "/images/touxiang1.png";
                string user_name = "游客";
                bool isCollect = true;

                string upload_pic_cover = string.Empty;
                bool ishot = false;
                string ResourceFormatTag_idstr = string.Empty;
                string proName = "null";
                bool isxiajia = true;
                int num_dianzan = 0;
                int num_xiazai = 0;
                Resource_list Resource_list = Resource_listbll.SelectSingle(" id= " + item_ResourceCollect["resource_id"].ToString() + " and ResourceClass3_id in(select id from ResourceClass3_List where IsShow=1 and ResourceClass2_Id in(select id from ResourceClass2_List where isshow=1 and ResourceClass1_Id in(select id from ResourceClass1_List where isshow=1)))");
                if (Resource_list != null && Resource_list.id > 0)
                {
                    upload_pic_cover = Resource_list.upload_pic_cover;
                    proName = Function.HtmlDiscode(Resource_list.name);
                    num_dianzan = Resource_list.num_dianzan;
                    num_xiazai = Resource_list.num_xiazai;
                    if (Resource_list.status == 1)
                    {
                        isxiajia = false;
                        ishot = true;
                        ResourceFormatTag_idstr = Resource_list.ResourceFormatTag_idstr;
                    }
                    user_list user_list_model = user_listbll.SelectSingle("id=" + Resource_list.userid);
                    if (user_list_model != null && user_list_model.id > 0)
                    {
                        user_name = Function.HtmlDiscode(user_list_model.name);
                        if (GetImgBool(user_list_model.upload_pic_avatar))
                        {
                            avatar_img = A_UpLoad_Url + user_list_model.upload_pic_avatar;
                        }
                    }
                }
                str += " <div class=\"pe-li-item\">";
                str += "<div  class=\"pe-li-img\">";
                str += "  <a href=\"/Models_" + item_ResourceCollect["resource_id"].ToString() + "\"  target=\"_blank\">  <img src=\"" + GetWebUpload_Pic(upload_pic_cover, "/images/modelnull.jpg") + "\" />" + (isxiajia ? "<img src=\"/images/xiajia.png\" class=\"pe-li-xiajiaimg\"/>" : "") + "</a>";
                string datatext = string.Empty;
                string dataimg = string.Empty;
                if (ishot)
                {
                    datatext = "热门文献";
                    dataimg = "/images/huo.png";
                }
                if (string.IsNullOrWhiteSpace(datatext) || string.IsNullOrWhiteSpace(dataimg))
                {

                    if (!string.IsNullOrWhiteSpace(ResourceFormatTag_idstr))
                    {
                        ResourceFormatTag_List ResourceFormatTag_List = ResourceFormatTag_Listbll.SelectSingle("id in(" + ResourceFormatTag_idstr + ") order by OrderId asc,UpTime desc,AddTime desc,Id desc");
                        if (ResourceFormatTag_List != null && ResourceFormatTag_List.Id > 0)
                        {
                            datatext = Function.HtmlDiscode(ResourceFormatTag_List.Name);
                            if (GetImgBool(ResourceFormatTag_List.Upload_Pic_Img))
                            {
                                dataimg = "/A_UpLoad/upload_pic/" + ResourceFormatTag_List.Upload_Pic_Img;
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(datatext) && !string.IsNullOrWhiteSpace(dataimg))
                {
                    str += "    <div class=\"tips\" data-text=\"" + datatext + "\">";
                    str += "        <img src=\"" + dataimg + "\" />";
                    str += "    </div>";
                }


                str += "    <div class=\"shoucang " + (isCollect ? " selected" : "") + "\" onclick=\"ResourceCollectFunc(this)\" data-id=\"" + item_ResourceCollect["resource_id"].ToString() + "\">";
                str += "        <div class=\"icon\"></div>";
                str += "    </div>";
                str += "</div>";
                str += "<div class=\"pe-li-xinxi\">";
                str += "    <a href=\"/Models_" + item_ResourceCollect["resource_id"].ToString() + "\" class=\"pe-xinxi-img\">";
                str += "        <img src=\"" + avatar_img + "\" /></a>";
                str += "    <div class=\"pe-xinxi-text\">";
                str += "        <h4><a href=\"/Models_" + item_ResourceCollect["resource_id"].ToString() + "\">" + Function.HtmlDiscode(proName) + "</a></h4>";
                str += "        <p><a href=\"/Models_" + item_ResourceCollect["resource_id"].ToString() + "\">" + user_name + "</a></p>";
                str += "        <div class=\"pe-li-icon\">";
                str += "            <span class=\"like\">";
                str += "               <svg t=\"1766643700342\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"2333\" width=\"16\" height=\"16\"><path d=\"M853.333333 332.8h-234.666666c0-17.066667 4.266667-34.133333 4.266666-55.466667 0-42.666667-8.533333-98.133333-34.133333-145.066666-38.4-64-89.6-93.866667-123.733333-106.666667C413.866667 8.533333 375.466667 42.666667 362.666667 76.8L260.266667 405.333333l-8.533334 8.533334H170.666667c-72.533333 0-128 55.466667-128 128v332.8c0 72.533333 55.466667 128 128 128h622.933333c64 0 115.2-46.933333 128-110.933334l64-418.133333c4.266667-72.533333-55.466667-140.8-132.266667-140.8zM251.733333 917.333333H170.666667c-25.6 0-42.666667-17.066667-42.666667-42.666666v-332.8c0-25.6 17.066667-42.666667 42.666667-42.666667h85.333333l-4.266667 418.133333z m580.266667-34.133333c-4.266667 21.333333-21.333333 38.4-42.666667 38.4H337.066667L341.333333 435.2l102.4-324.266667c21.333333 8.533333 46.933333 25.6 68.266667 64 17.066667 25.6 21.333333 64 21.333333 98.133334 0 38.4-4.266667 72.533333-8.533333 89.6L512 418.133333h341.333333c25.6 0 46.933333 21.333333 42.666667 46.933334l-64 418.133333z\" fill=\"#666666\" p-id=\"2334\"></path></svg>";
                str += "               "+ FormatNumber(num_dianzan.ToString()) + "</span>";
                str += "            <span>";
                str += "                <svg t=\"1766995208966\" class=\"icon\" viewBox=\"0 0 1047 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"3737\" width=\"16\" height=\"16\"><path d=\"M523.987449 0a42.759353 42.759353 0 0 0-42.759353 42.759353v498.158822L301.663529 361.353608a43.500845 43.500845 0 0 0-60.555153 0 42.759353 42.759353 0 0 0 0 60.431572l227.020033 227.267198a79.092445 79.092445 0 0 0 111.223751 0l227.514362-227.143616a42.635771 42.635771 0 0 0 0-60.555154 42.882935 42.882935 0 0 0-60.555153 0L566.870384 540.918175V42.759353A42.882935 42.882935 0 0 0 523.987449 0z\" fill=\"#666666\" p-id=\"3738\"></path><path d=\"M860.871832 120.245233a42.882935 42.882935 0 1 0 0 85.642288 101.584359 101.584359 0 0 1 101.460777 101.584359v529.301473a101.584359 101.584359 0 0 1-101.460777 101.584359H187.226647a101.707941 101.707941 0 0 1-101.584359-101.584359V307.47188a101.707941 101.707941 0 0 1 101.584359-101.584359 42.882935 42.882935 0 0 0 0-85.642288A187.473811 187.473811 0 0 0 0 307.47188v529.301473a187.473811 187.473811 0 0 0 187.226647 187.226647h673.645185A187.350229 187.350229 0 0 0 1047.974897 836.773353V307.47188A187.350229 187.350229 0 0 0 860.871832 120.245233z\" fill=\"#666666\" p-id=\"3739\"></path></svg>";
                str += "                "+ FormatNumber(num_xiazai.ToString()) + "</span>";
                str += "        </div>";
                str += "    </div>";
                str += "</div>";
                str += "</div>";

            }
            return str;
        }
        public static string FormatNumber(string number_str)
        {
            double number = Function.ConvertTo<double>(number_str, 0);
            if (number < 1000) return number_str;

            string[] units = new[] { "", "K", "W", "E" }; // K for thousand, W for ten thousand, E for hundred thousand
            int unitIndex = 0;
            double absNumber = Math.Abs(number);

            while (absNumber >= 1000 && unitIndex < units.Length - 1)
            {
                absNumber /= 1000;
                unitIndex++;
            }

            return $"{absNumber:0.##}{units[unitIndex]}";
        }
        public static string GetSearchClass3ResourceHomeFunc(int resourceClass3Id, string ok_idstr)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                ResourceClass3_List ResourceClass3_List = ResourceClass3_Listbll.SelectSingle("id=" + resourceClass3Id + " and isshow=1");
                if (ResourceClass3_List != null && ResourceClass3_List.Id > 0)
                {
                    ResourceClass2_List ResourceClass2_List = ResourceClass2_Listbll.SelectSingle("id=" + ResourceClass3_List.ResourceClass2_Id + " and isshow=1");
                    if (ResourceClass2_List != null && ResourceClass2_List.Id > 0)
                    {
                        ResourceClass1_List ResourceClass1_List = ResourceClass1_Listbll.SelectSingle("id=" + ResourceClass2_List.ResourceClass1_Id + " and isshow=1");
                        if (ResourceClass1_List != null && ResourceClass1_List.Id > 0)
                        {
                            string pro_html = string.Empty;
                            string sql = "select pro.id, pro.name,pro.upload_pic_cover from Resource_list as pro  where pro.status=1 and pro.isshow=1 and pro.ResourceClass3_id=" + ResourceClass3_List.Id + " " + (!string.IsNullOrWhiteSpace(ok_idstr) ? " and pro.id not in(" + ok_idstr + ")" : "") + " order by pro.addtime,pro.id desc";
                            DataTable Resource_listdt = Resource_listbll.GetDatatable(sql);
                            pro_html = GetRelatedResourceHtml(Resource_listdt);
                            Resource_listdt.Dispose();

                            str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = "拉取相关产品数据列表成功！", ["pro_html"] = pro_html });
                        }
                        else
                        {
                            str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "请求参数异常！！" });
                        }
                    }
                    else
                    {
                        str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "请求参数异常！！" });
                    }
                }
                else
                {
                    str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "请求参数异常！" });
                }
            }
            catch (Exception ex)
            {
                str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = ex.Message });
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
            return str;
        }

        public static string GetSearchKeyResourceHomeFunc(string searchkey, string ok_idstr)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "网络繁忙请稍后再试！" });
            try
            {
                if (!string.IsNullOrWhiteSpace(searchkey))
                {
                    string pro_html = string.Empty;
                    string sql = "select pro.id, pro.name,pro.upload_pic_cover from Resource_list as pro  where pro.status=1 and pro.isshow=1 and pro.ResourceClass3_id in(select id from ResourceClass3_List where IsShow=1 and ResourceClass2_Id in(select id from ResourceClass2_List where isshow=1 and ResourceClass1_Id in(select id from ResourceClass1_List where isshow=1)))" + " and pro.name like'%" + Function.HtmlEncode(searchkey.Trim()) + "%'" + (!string.IsNullOrWhiteSpace(ok_idstr) ? " and pro.id not in(" + ok_idstr + ")" : "") + " order by pro.addtime,pro.id desc";

                    DataTable Resource_listdt = Resource_listbll.GetDatatable(sql);
                    pro_html = GetRelatedResourceHtml(Resource_listdt);
                    Resource_listdt.Dispose();

                    str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = "拉取相关产品数据列表成功！", ["pro_html"] = pro_html });
                }
                else
                {
                    str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = "请输入要搜索的产品名！" });
                }
            }
            catch (Exception ex)
            {
                str = serializer.Serialize(new Dictionary<string, object> { ["status"] = 0, ["info"] = ex.Message });
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
            return str;
        }
        public static string GetRelatedResourceHtml(DataTable resourceListDt)
        {
            string staffuser_html = string.Empty;
            if (resourceListDt != null && resourceListDt.Rows.Count > 0)
            {
                foreach (DataRow item in resourceListDt.Rows)
                {
                    staffuser_html += "<li class=\"checkbox-li\">";
                    staffuser_html += " <label class=\"checkboxlabel-box\" for=\"ok_pro_" + item["id"].ToString() + "\">";
                    staffuser_html += "     <input type=\"checkbox\" name=\"ok_pro\" id=\"ok_pro_" + item["id"].ToString() + "\" value=\"" + item["id"].ToString() + "\"/>";
                    staffuser_html += "     <a href=\"/Models_" + item["id"].ToString() + "\" target=\"_blank\"><img src=\"" + Function.GetAdminUpload_Pic(item["upload_pic_cover"].ToString()) + "\" height=\"20\" style=\"border: 1px solid #cccccc\" class=\"tooltip_img\"><span class=\"personnel-name\">" + Function.HtmlDiscode(item["name"].ToString()) + "</span></a>";
                    staffuser_html += " </label>";
                    staffuser_html += "</li>";
                }
            }
            else
            {
                staffuser_html += "<li><label><span>暂无数据</span></label></li>";
            }
            resourceListDt.Dispose();
            return staffuser_html;
        }

        public static string ToFriendlySize(long byteCount)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            if (byteCount == 0) return "0 " + sizes[0];
            int place = Convert.ToInt32(Math.Floor(Math.Log(byteCount, 1024)));
            double num = Math.Round(byteCount / Math.Pow(1024, place), 1);
            return $"{num} {sizes[place]}";
        }
        public static void GetProHotData()
        {
            try
            {
                string sql = "UPDATE Resource_list SET ishot=0";
                sql += "UPDATE Resource_list SET ishot=1 where id in(select id from Resource_list where isshow=1 and ResourceClass3_id in(select top 10 id from ResourceClass3_List where IsShow=1 and ResourceClass2_Id in(select id from ResourceClass2_List where isshow=1 and ResourceClass1_Id in(select id from ResourceClass1_List where isshow=1))) order by num_look desc,num_dianzan desc,num_shoucang desc,addtime desc,id desc) ";
                Resource_listbll.Sql_D(sql);

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void GetDelData()
        {
            try
            {
                DataTable ResourceDataInfoCopy_list_dt = ResourceDataInfoCopy_listbll.GetDatatable("select id,upload_pic_cover from ResourceDataInfoCopy_list where status=0 and DateDiff(hh,addtime,getDate())>72");
                if (ResourceDataInfoCopy_list_dt != null && ResourceDataInfoCopy_list_dt.Rows.Count > 0)
                {
                    foreach (DataRow item in ResourceDataInfoCopy_list_dt.Rows)
                    {
                        if (!string.IsNullOrWhiteSpace(item["upload_pic_cover"].ToString()))
                        {
                            string del_sql = "DELETE FROM ResourceDataInfoDataCopy_list WHERE ResourceDataInfoCopy_id=" + item["id"].ToString();
                            del_sql += "ξLiteratureManagerξDELETE FROM ResourceDataInfoCopy_list WHERE id=" + item["id"].ToString();
                            ResourceDataInfoCopy_listbll.Sql_D(del_sql);
                            Function.FileDelete("/A_UpLoad/upload_pic/" + item["upload_pic_cover"].ToString());
                        }
                    }
                }
                ResourceDataInfoCopy_list_dt.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
            try
            {
                DataTable cosfile_list_dt = cosfile_listbll.GetDatatable("select * from cosfile_list where DateDiff(hh,addtime,getDate())>72");
                if (cosfile_list_dt != null && cosfile_list_dt.Rows.Count > 0)
                {
                    foreach (DataRow item in cosfile_list_dt.Rows)
                    {
                        if (!string.IsNullOrWhiteSpace(item["up_filename"].ToString()))
                        {
                            DeleteTrackedUploadRecord(item["up_filename"].ToString());
                            PutObjectModel.DeleteObject(item["up_filename"].ToString());
                        }
                    }
                }
                cosfile_list_dt.Dispose();
            }
            catch (Exception)
            {

                throw;
            }
            try
            {
                DataTable userfile_list_dt = userfile_listbll.GetDatatable("select * from userfile_list where DateDiff(hh,addtime,getDate())>72");
                if (userfile_list_dt != null && userfile_list_dt.Rows.Count > 0)
                {
                    foreach (DataRow item in userfile_list_dt.Rows)
                    {
                        if (!string.IsNullOrWhiteSpace(item["up_filename"].ToString()))
                        {
                            DeleteTrackedUploadRecord(item["up_filename"].ToString());
                            Function.FileDelete("/A_UpLoad/upload_file/" + item["up_filename"].ToString());
                        }
                    }
                }
                userfile_list_dt.Dispose();
            }
            catch (Exception)
            {

                throw;
            }
            try
            {
                DataTable userimg_list_dt = userimg_listbll.GetDatatable("select * from userimg_list where DateDiff(hh,addtime,getDate())>72");
                if (userimg_list_dt != null && userimg_list_dt.Rows.Count > 0)
                {
                    foreach (DataRow item in userimg_list_dt.Rows)
                    {
                        if (!string.IsNullOrWhiteSpace(item["upload_pic_img"].ToString()))
                        {
                            userimg_listbll.Delete("upload_pic_img='" + item["upload_pic_img"].ToString() + "'");
                            Function.FileDelete("/A_UpLoad/upload_pic/" + item["upload_pic_img"].ToString());
                        }
                    }
                }
                userimg_list_dt.Dispose();
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region 返回执行结果
        ///// <summary>
        ///// 返回执行结果
        ///// </summary>
        public static void Ok_Return(string title, string URL, int t)
        {
            string img = "";
            StringBuilder sb = new StringBuilder();
#pragma warning disable CS0472 // 由于此类型的值永不等于 "null"，该表达式的结果始终相同
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(URL) || t == null)
            {
                Function.Show_Msg("非法操作！", "");

            }
#pragma warning restore CS0472 // 由于此类型的值永不等于 "null"，该表达式的结果始终相同

            if (t == 0)   //正确
            {
                img = "OK.gif";
            }
            else if (t == 1)   //提示
            {
                img = "information.gif";
            }
            else if (t == 2)
            {
                img = "Failure.gif";   //错误显示图片不一样
            }
            else if (t == 3)
            {
                img = "warning.gif";   //警告显示图片不一样
            }


            sb.Append("<script>" + "\r\n");

            sb.Append("var seconds = 2;" + "\r\n");
            sb.Append("var defaultUrl = '" + URL + "';" + "\r\n");

            sb.Append("onload = function()" + "\r\n");
            sb.Append("{" + "\r\n");
            sb.Append("var ok_html =\"<link media='all' type='text/css' href='/admin/css/style.css' rel='stylesheet' /><div class='container' id='cpcontainer'><h3>操作提示</h3><div class='infobox'><table width='600' border='0' align='center' cellpadding='4' cellspacing='0'><tr><td width='154' rowspan='2' align='right'><img height='32' alt='information' src='/admin/images/" + img + "' width='32' border='0' style='margin-right:10px;' /></td><td width='430' align='left' class='infotitle2'>" + title + "</td></tr><tr><td align='left'>将在 <span id='spanSeconds'>2</span> 秒后跳转到第一个链接地址。</td></tr></table></div></div></div>\"\r\n");
            sb.Append("$('#ok_html').html(ok_html);\r\n");
            sb.Append("  if (defaultUrl == 'javascript:history.go(-1)' && window.history.length == 0)" + "\r\n");
            sb.Append("  {" + "\r\n");
            sb.Append("    document.getElementById('redirectionMsg').innerHTML = '';" + "\r\n");
            sb.Append("    return;" + "\r\n");
            sb.Append("  }" + "\r\n");

            sb.Append("}" + "\r\n");
            sb.Append("var aa = window.setInterval(redirection, 1000);" + "\r\n");
            sb.Append("function redirection()" + "\r\n");
            sb.Append("{" + "\r\n");
            sb.Append("  if (seconds <= 0)" + "\r\n");
            sb.Append("  {" + "\r\n");
            sb.Append("if(aa) window.clearInterval(aa);" + "\r\n");
            sb.Append("    return;" + "\r\n");
            sb.Append("  }" + "\r\n");
            sb.Append("  seconds --;" + "\r\n");
            sb.Append("  document.getElementById('spanSeconds').innerHTML = seconds;" + "\r\n");
            sb.Append("  if (seconds == 0) " + "\r\n");
            sb.Append("  {" + "\r\n");
            sb.Append("if(aa) window.clearInterval(aa);" + "\r\n");
            sb.Append("    location.href = defaultUrl;" + "\r\n");
            sb.Append("  }" + "\r\n");
            sb.Append("}" + "\r\n");
            sb.Append("</script>");

            HttpContext.Current.Response.Write(sb.ToString());
        }
        #endregion


        public static string GetResourceClass1_Id(string ResourceClass3_id)
        {
            string R_str = string.Empty;
            ResourceClass3_List ResourceClass3_List = ResourceClass3_Listbll.SelectSingle("id=" + Function.ConvertTo<int>(ResourceClass3_id, 0));
            if (ResourceClass3_List != null && ResourceClass3_List.Id > 0)
            {
                ResourceClass2_List ResourceClass2_List = ResourceClass2_Listbll.SelectSingle("id=" + ResourceClass3_List.ResourceClass2_Id);
                if (ResourceClass2_List != null && ResourceClass2_List.Id > 0)
                {
                    ResourceClass1_List ResourceClass1_List = ResourceClass1_Listbll.SelectSingle("id=" + ResourceClass2_List.ResourceClass1_Id);
                    if (ResourceClass1_List != null && ResourceClass1_List.Id > 0)
                    {
                        R_str = ResourceClass1_List.Id.ToString();
                    }
                }
            }
            return R_str;
        }
        public static string GetTitle(string data_title, string tbclass_title)
        {
            string R_str = string.Empty;
            if (!string.IsNullOrWhiteSpace(data_title))
            {
                R_str = Function.HtmlDiscode(data_title).Trim();
            }
            if (string.IsNullOrWhiteSpace(R_str))
            {
                R_str = Function.HtmlDiscode(tbclass_title).Trim();
            }
            if (string.IsNullOrWhiteSpace(R_str))
            {
                websiteinfo_list websiteinfo_list = websiteinfo_listbll.SelectSingle("id=1");
                if (websiteinfo_list != null && websiteinfo_list.id > 0)
                {
                    R_str = Function.HtmlDiscode(websiteinfo_list.title);
                    if (string.IsNullOrWhiteSpace(R_str))
                    {
                        R_str = Function.HtmlDiscode(websiteinfo_list.companyname);
                    }
                }
            }
            return R_str;
        }
        public static string GetDescription(string data_description)
        {
            string R_str = string.Empty;
            if (!string.IsNullOrWhiteSpace(data_description))
            {
                R_str = Function.HtmlDiscode(data_description).Trim();
            }
            if (string.IsNullOrWhiteSpace(R_str))
            {
                websiteinfo_list websiteinfo_list = websiteinfo_listbll.SelectSingle("id=1");
                if (websiteinfo_list != null && websiteinfo_list.id > 0)
                {
                    R_str = Function.HtmlDiscode(websiteinfo_list.description);
                }
            }
            return R_str;
        }
        public static string GetKeywords(string data_keywords)
        {
            string R_str = string.Empty;
            if (!string.IsNullOrWhiteSpace(data_keywords))
            {
                R_str = Function.HtmlDiscode(data_keywords).Trim();
            }

            if (string.IsNullOrWhiteSpace(R_str))
            {
                websiteinfo_list websiteinfo_list = websiteinfo_listbll.SelectSingle("id=1");
                if (websiteinfo_list != null && websiteinfo_list.id > 0)
                {
                    R_str = Function.HtmlDiscode(websiteinfo_list.keywords);
                }
            }
            return R_str;
        }
        public static string GetTbClassTitle(tbl_class tbl_class)
        {
            string R_str = string.Empty;
            if (tbl_class != null && tbl_class.id > 0)
            {
                if (!string.IsNullOrWhiteSpace(tbl_class.classname))
                {
                    R_str = Function.HtmlDiscode(tbl_class.classname);
                }

                if (tbl_class.parentid != 360)
                {
                    tbl_class tbl_class_p = tbl_classbll.SelectSingle("id=" + tbl_class.parentid + " and isshow=1");
                    if (tbl_class_p != null && tbl_class_p.id > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(tbl_class_p.classname))
                        {
                            if (!string.IsNullOrWhiteSpace(R_str))
                            {
                                R_str += "-" + Function.HtmlDiscode(tbl_class_p.classname);
                            }
                            else
                            {
                                R_str += Function.HtmlDiscode(tbl_class_p.classname);
                            }

                        }
                    }
                }
            }
            websiteinfo_list websiteinfo_list = websiteinfo_listbll.SelectSingle("id=1");
            if (websiteinfo_list != null && websiteinfo_list.id > 0)
            {
                if (!string.IsNullOrWhiteSpace(websiteinfo_list.companyname))
                {
                    if (!string.IsNullOrWhiteSpace(R_str))
                    {
                        R_str += "-" + Function.HtmlDiscode(websiteinfo_list.companyname);
                    }
                    else
                    {
                        R_str += Function.HtmlDiscode(websiteinfo_list.companyname);
                    }

                }
            }
            return R_str;
        }
        public static string GetCaptcha()
        {
            int number;
            char code;
            string checkCode = String.Empty;

            Random random = new Random();

            for (int i = 0; i < 4; i++)
            {
                number = random.Next();

                if (number % 3 == 0)
                {
                    code = (char)('0' + (char)(number % 10));
                }
                else if (number % 3 == 1)
                {
                    code = (char)('a' + (char)(number % 26));
                }
                else
                {
                    code = (char)('A' + (char)(number % 26));
                }
                checkCode += " " + code.ToString();
            }
            checkCode = checkCode.Replace(" ", "");
            return checkCode.ToLower();
        }

        public static string GetTbClassModelUrl(string tbclass_id)
        {
            string R_str = string.Empty;
            tbl_class tbl_class = tbl_classbll.SelectSingle("id=" + Function.ConvertTo<int>(tbclass_id, 0));
            if (tbl_class != null && tbl_class.id > 0)
            {
                R_str = "<a href=\"" + GetModelUrl(tbl_class.model.ToString()) + "?tbclass_id=" + tbl_class.id + "\" >" + Function.HtmlDiscode(tbl_class.classname) + "</a>";
            }
            return R_str;
        }

        public static string GetIntegrateLogTypeFunc(string type_id)
        {
            string R_str = string.Empty;
            integrateLogType_list integrateLogType_list = integrateLogType_listbll.SelectSingle("id=" + Function.ConvertTo<int>(type_id, 0));
            if (integrateLogType_list != null && integrateLogType_list.id > -1)
            {
                R_str = Function.HtmlDiscode(integrateLogType_list.name);
            }
            return R_str;
        }

        public static string GetServiceLogStatusNameFunc(string status_id)
        {
            string R_str = string.Empty;
            ServiceLogStatus_List ServiceLogStatus_List = ServiceLogStatus_Listbll.SelectSingle("id=" + Function.ConvertTo<int>(status_id, 0));
            if (ServiceLogStatus_List != null && ServiceLogStatus_List.id > -1)
            {
                R_str = Function.HtmlDiscode(ServiceLogStatus_List.name);
            }
            return R_str;
        }

        public static string GetTbClassNameFunc(string tbclass_id)
        {
            string R_str = string.Empty;
            tbl_class tbl_class = tbl_classbll.SelectSingle("id=" + Function.ConvertTo<int>(tbclass_id, 0));
            if (tbl_class != null && tbl_class.id > 0)
            {
                R_str = Function.HtmlDiscode(tbl_class.classname);
            }
            return R_str;
        }

        #region 前端的栏目跳转连接
        /// <summary>
        /// 前端的栏目跳转连接；
        /// </summary>
        public static string GetTopHtmlHref(string id_str, string type)
        {
            string R_str = "javascript:void(0);";
            string wherestr = string.Empty;
            if (type == "1")
            {
                wherestr = " and istop=1 ";
            }
            else if (type == "2")
            {
                wherestr = " and isfoot=1 ";
            }

            int id_ = Function.ConvertTo<int>(id_str, 0);
            if (id_ > 0)
            {
                tbl_class tbl_class = tbl_classbll.SelectSingle("id=" + id_ + " and isshow=1");
                if (tbl_class != null && tbl_class.id > 0)
                {
                    if (tbl_class.isurl == 2)
                    {
                        if (!string.IsNullOrWhiteSpace(tbl_class.classurl))
                        {
                            string classurl = Function.HtmlDiscode(tbl_class.classurl);
                            if (classurl.ToLower().IndexOf("http") == 0)
                            {
                                R_str = classurl + "\" target=\"_blank";
                            }
                            else
                            {
                                R_str = classurl;
                            }
                        }
                        else
                        {
                            R_str = "javascript:void(0);";
                        }

                    }
                    else
                    {
                        if (tbl_class.model.Equals(1))
                        {
                            DataTable tbl_classdt = tbl_classbll.GetDatatable("select * from tbl_class where parentid=" + tbl_class.id + " and isshow=1 " + wherestr + "  order by orderid asc");


                            if (tbl_classdt != null && tbl_classdt.Rows.Count > 0)
                            {
                                if (tbl_classdt.Rows[0]["model"].ToString() == "1")
                                {
                                    int pid = Function.ConvertTo<int>(tbl_classdt.Rows[0]["id"].ToString(), 0);
                                    DataTable tbl_classdt_ = tbl_classbll.GetDatatable("select * from tbl_class where  parentid=" + pid + " and isshow=1 " + wherestr + " order by orderid asc");


                                    if (tbl_classdt_ != null && tbl_classdt_.Rows.Count > 0)
                                    {
                                        R_str = GetHtmlHref(Function.ConvertTo<int>(tbl_classdt_.Rows[0]["id"].ToString(), 0), wherestr, Function.ConvertTo<int>(tbl_classdt.Rows[0]["id"].ToString(), 0));
                                    }
                                    else
                                    {
                                        R_str = GetHtmlHref(Function.ConvertTo<int>(tbl_classdt.Rows[0]["id"].ToString(), 0), wherestr, tbl_class.id);
                                    }
                                }
                                else
                                {
                                    R_str = GetHtmlHref(Function.ConvertTo<int>(tbl_classdt.Rows[0]["id"].ToString(), 0), wherestr, tbl_class.id);
                                }
                            }
                            else
                            {
                                R_str = "javascript:void(0);";
                            }
                            tbl_classdt.Dispose();
                        }
                        else
                        {
                            R_str = GetHtmlHref(Function.ConvertTo<int>(tbl_class.id, 0), wherestr, Function.ConvertTo<int>(tbl_class.id, 0));
                        }
                    }
                }
            }
            return Function.HtmlDiscode(R_str);
        }

        public static string GetHtmlHref(int id_, string wherestr, int pid_)
        {
            string R_str = "javascript:void(0);";
            if (id_ > 0)
            {
                tbl_class tbl_class = tbl_classbll.SelectSingle("id=" + id_ + " and isshow=1" + wherestr);
                if (tbl_class != null && tbl_class.id > 0)
                {
                    if (tbl_class.isurl == 2)
                    {
                        string classurl = tbl_class.classurl;
                        if (!string.IsNullOrWhiteSpace(classurl))
                        {
                            if (classurl.ToLower().IndexOf("http") == 0)
                            {
                                R_str = classurl + "\" target=\"_blank";
                            }
                            else
                            {
                                R_str = classurl;
                            }
                        }
                        else
                        {
                            R_str = "javascript:void(0);";
                        }
                    }
                    else
                    {
                        R_str = "/Website/" + Function.HtmlDiscode(tbl_class.urlnamebtn);
                        //R_str = GetHtmlHrefUrl(tbl_class.model, "?mid=" + tbl_class.id);
                    }
                }
            }
            return R_str;
        }
        public static string GetModelUrl(string id)
        {
            string m_url = "";
            model_list model_list = model_listbll.SelectSingle("id", Function.ConvertTo<int>(id, 0));
            if (model_list != null && model_list.id > 0)
            {
                m_url = Function.HtmlDiscode(model_list.m_url);
            }
            return m_url;
        }


        #endregion

        public static string GetResourceTagNameStrFunc(string ResourceTag_idstr)
        {
            string R_str = string.Empty;
            if (!string.IsNullOrWhiteSpace(ResourceTag_idstr))
            {
                DataTable ResourceTag_Listdt_ = ResourceTag_Listbll.GetDatatable("select id,name from ResourceTag_List where  id in(" + ResourceTag_idstr + ") order by OrderId asc,UpTime desc,Id desc");
                foreach (DataRow item in ResourceTag_Listdt_.Rows)
                {
                    R_str += "<span  class=\"tag_class\">" + Function.HtmlDiscode(item["name"].ToString()) + "</span>";
                }
                ResourceTag_Listdt_.Dispose();
            }
            return R_str;
        }
        public static string GetResourceFormatNameStrFunc(string ResourceFormat_idstr)
        {
            string R_str = string.Empty;
            if (!string.IsNullOrWhiteSpace(ResourceFormat_idstr))
            {
                DataTable ResourceFormat_Listdt_ = ResourceFormat_Listbll.GetDatatable("select id,name from ResourceFormat_List where  id in(" + ResourceFormat_idstr + ") order by OrderId asc,UpTime desc,Id desc");
                foreach (DataRow item in ResourceFormat_Listdt_.Rows)
                {
                    R_str += "<span class=\"tag_class\">" + Function.HtmlDiscode(item["name"].ToString()) + "</span>";
                }
                ResourceFormat_Listdt_.Dispose();
            }
            return R_str;
        }


        public static string GetResourceFormatTagNameStrFunc(string ResourceFormatTag_idstr)
        {
            string R_str = string.Empty;
            if (!string.IsNullOrWhiteSpace(ResourceFormatTag_idstr))
            {
                DataTable ResourceFormatTag_Listdt_ = ResourceFormatTag_Listbll.GetDatatable("select id,name from ResourceFormatTag_List where  id in(" + ResourceFormatTag_idstr + ") order by OrderId asc,UpTime desc,Id desc");
                foreach (DataRow item in ResourceFormatTag_Listdt_.Rows)
                {
                    R_str += "<span  class=\"tag_class\">" + Function.HtmlDiscode(item["name"].ToString()) + "</span>";
                }
                ResourceFormatTag_Listdt_.Dispose();
            }
            return R_str;
        }

        public static string GetResourceLicenseNameFunc(string ResourceLicense_id)
        {
            string R_str = string.Empty;
            ResourceLicense_List ResourceLicense_List = ResourceLicense_Listbll.SelectSingle("id=" + Function.ConvertTo<int>(ResourceLicense_id, 0));
            if (ResourceLicense_List != null && ResourceLicense_List.Id > 0)
            {
                R_str = Function.HtmlDiscode(ResourceLicense_List.Name);
            }
            return R_str;
        }
        public static string GetProNameFunc(string Resource_id)
        {
            string R_str = string.Empty;
            Resource_list Resource_list = Resource_listbll.SelectSingle("id=" + Function.ConvertTo<int>(Resource_id, 0));
            if (Resource_list != null && Resource_list.id > 0)
            {
                R_str = Function.HtmlDiscode(Resource_list.name);
            }
            return R_str;
        }
        public static string GetResourceClassNameFunc(string ResourceClass3_Id)
        {
            string R_str = string.Empty;
            ResourceClass3_List ResourceClass3_List = ResourceClass3_Listbll.SelectSingle("id=" + Function.ConvertTo<int>(ResourceClass3_Id, 0));
            if (ResourceClass3_List != null && ResourceClass3_List.Id > 0)
            {
                ResourceClass2_List ResourceClass2_List = ResourceClass2_Listbll.SelectSingle("id=" + ResourceClass3_List.ResourceClass2_Id);
                if (ResourceClass2_List != null && ResourceClass2_List.Id > 0)
                {
                    ResourceClass1_List ResourceClass1_List = ResourceClass1_Listbll.SelectSingle("id=" + ResourceClass2_List.ResourceClass1_Id);
                    if (ResourceClass1_List != null && ResourceClass1_List.Id > 0)
                    {
                        R_str = Function.HtmlDiscode(ResourceClass1_List.Name) + " - " + Function.HtmlDiscode(ResourceClass2_List.Name) + " - " + Function.HtmlDiscode(ResourceClass3_List.Name);
                    }
                }
            }
            return R_str;
        }


        public static string GetResourceClass2AndClass1NameFunc(string ResourceClass2_Id)
        {
            string R_str = string.Empty;
            ResourceClass2_List ResourceClass2_List = ResourceClass2_Listbll.SelectSingle("id=" + Function.ConvertTo<int>(ResourceClass2_Id, 0));
            if (ResourceClass2_List != null && ResourceClass2_List.Id > 0)
            {
                ResourceClass1_List ResourceClass1_List = ResourceClass1_Listbll.SelectSingle("id=" + ResourceClass2_List.ResourceClass1_Id);
                if (ResourceClass1_List != null && ResourceClass1_List.Id > 0)
                {
                    R_str = Function.HtmlDiscode(ResourceClass1_List.Name) + " - " + Function.HtmlDiscode(ResourceClass2_List.Name);
                }
            }
            return R_str;
        }

        public static string GetResourceClass2NameFunc(string ResourceClass2_Id)
        {
            string R_str = string.Empty;
            ResourceClass2_List ResourceClass2_List = ResourceClass2_Listbll.SelectSingle("id=" + Function.ConvertTo<int>(ResourceClass2_Id, 0));
            if (ResourceClass2_List != null && ResourceClass2_List.Id > 0)
            {
                R_str = Function.HtmlDiscode(ResourceClass2_List.Name);
            }
            return R_str;
        }
        public static string GetResourceClass1NameFunc(string ResourceClass1_Id)
        {
            string R_str = string.Empty;
            ResourceClass1_List ResourceClass1_List = ResourceClass1_Listbll.SelectSingle("id=" + Function.ConvertTo<int>(ResourceClass1_Id, 0));
            if (ResourceClass1_List != null && ResourceClass1_List.Id > 0)
            {
                R_str = Function.HtmlDiscode(ResourceClass1_List.Name);
            }
            return R_str;
        }

        public static string GetResourceClass2ListFunc(int ResourceClass1_Id)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string _html = "<option value=\"\">-请选择-</option>";
            DataTable ResourceClass2_List_dt = ResourceClass2_Listbll.GetDatatable("select id,name from ResourceClass2_List where ResourceClass1_Id=" + ResourceClass1_Id + " order by orderid asc,id asc");
            if (ResourceClass2_List_dt != null && ResourceClass2_List_dt.Rows.Count > 0)
            {
                foreach (DataRow item in ResourceClass2_List_dt.Rows)
                {
                    _html += "<option value=\"" + item["id"].ToString() + "\">" + Function.HtmlDiscode(item["name"].ToString()) + "</option>";
                }
            }
            ResourceClass2_List_dt.Dispose();

            return serializer.Serialize(new Dictionary<string, object> { ["status"] = 1, ["info"] = _html });
        }
        public static bool GetLoginCodeAdd(string VNum, string NewGuidCode)
        {
            logincode_list logincode_list = new logincode_list();
            logincode_list.code = NewGuidCode;
            logincode_list.val = VNum;
            logincode_list.addtime = DateTime.Now;
            logincode_list.ip_str = Function.GetClientIP();
            logincode_list.type = 1;
            return logincode_listbll.Add(logincode_list);
        }
        public static void GetLoginCodeDelete()
        {
            string BackstageCheckCode_ = Cookie.GetCookie("BackstageCheckCode");
            if (!string.IsNullOrWhiteSpace(BackstageCheckCode_))
            {
                logincode_listbll.Delete("code='" + Function.MD5Decrypt(BackstageCheckCode_, MD5Key) + "'");
            }
        }
        public static string Replace_Content(string info, string info_m)
        {
            return Function.Replace_Content(Replace_Content(info, info_m, GetIsWeb()));
        }
        public static string Replace_Content(string info, string info_m, bool isweb)
        {
            string info_pc_ = string.Empty;
            string info_m_ = string.Empty;
            if (!string.IsNullOrWhiteSpace(info) && !string.IsNullOrWhiteSpace(info_m))
            {
                info_pc_ = info;
                info_m_ = info_m;
            }
            else if (string.IsNullOrWhiteSpace(info) && string.IsNullOrWhiteSpace(info_m))
            {

            }
            else
            {
                if (!string.IsNullOrWhiteSpace(info))
                {
                    info_pc_ = info;
                    info_m_ = info_pc_;
                }
                else
                {
                    info_m_ = info_m;
                    info_pc_ = info_m_;
                }
            }

            string info_ = string.Empty;
            if (isweb)
            {
                info_ = info_pc_;
            }
            else
            {
                info_ = info_m_;
            }
            return Function.Replace_Content(info_);
        }


        public static bool GetIsWeb()
        {
            bool isweb = true;
            string u = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
            Regex b = new Regex(@"android.+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |ipad|maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(di|rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))))
            {
                isweb = false;
            }
            return isweb;
        }
        public static string GetBannerImg(string img_url_pc, string img_url_m)
        {
            string str = GetBannerImg(img_url_pc, img_url_m, GetIsWeb());
            return str;
        }

        public static string GetBannerImg(string img_url_pc, string img_url_m, bool isweb)
        {
            string str = string.Empty;
            bool is_bj_img_pc = GetImgBool(img_url_pc);
            bool is_bj_img_m = GetImgBool(img_url_m);
            if (is_bj_img_pc || is_bj_img_m)
            {
                if (isweb)
                {
                    if (is_bj_img_pc)
                    {
                        str = GetWebUpload_Pic(img_url_pc, "/images/null.png");
                    }
                    else if (is_bj_img_m)
                    {
                        str = GetWebUpload_Pic(img_url_m, "/images/null.png");
                    }
                }
                else
                {
                    if (is_bj_img_m)
                    {
                        str = GetWebUpload_Pic(img_url_m, "/images/null.png");
                    }
                    else if (is_bj_img_pc)
                    {
                        str = GetWebUpload_Pic(img_url_pc, "/images/null.png");
                    }
                }
            }
            return str;
        }
        public static string GetWebUpload_Pic(string imgurl, string noimg)
        {
            if (GetImgBool(imgurl))
            {
                return "/A_UpLoad/upload_pic/" + imgurl;
            }
            else
            {
                return noimg;
            }
        }
        public static string GetHtmlHrefUrl(int model_, int tbclass_id)
        {
            string R_str = string.Empty;
            if (model_ == 2)
            {
                R_str = "/WebsiteData/News.aspx?mid=" + tbclass_id;
            }
            else if (model_ == 3)
            {
                R_str = "/WebsiteData/Single.aspx?mid=" + tbclass_id;
            }

            return R_str;
        }
        public static bool GetCosFileBool(string up_filename)
        {
            bool isyes = false;
            try
            {

                if (!string.IsNullOrWhiteSpace(up_filename))
                {
                    string isok = PutObjectModel.DoesObjectExist(up_filename);
                    if (isok == "ok")
                    {
                        isyes = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "CommonFunc.GetCosFileBool:" + ex.Message + "\\n" + ex.StackTrace);
            }
            return isyes;
        }

        public static void DeleteTrackedUploadRecord(string upFilename)
        {
            string safeFileName = EscapeSqlLiteral(upFilename);
            if (string.IsNullOrWhiteSpace(safeFileName))
            {
                return;
            }

            userfile_listbll.Delete("up_filename='" + safeFileName + "'");
            cosfile_listbll.Delete("up_filename='" + safeFileName + "'");
        }

        public static string EscapeSqlLiteral(string value)
        {
            return (value ?? string.Empty).Replace("'", "''").Trim();
        }

        public static bool GetFileBool(string up_filename)
        {
            bool isyes = false;
            try
            {

                if (!string.IsNullOrWhiteSpace(up_filename))
                {
                    string filePath = HttpContext.Current.Server.MapPath(@"/A_UpLoad/upload_file/" + up_filename);
                    System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                    if (file.Exists)
                    {
                        isyes = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "CommonFunc.GetFileBool:" + ex.Message + "\\n" + ex.StackTrace);
            }
            return isyes;
        }
        public static bool GetImgBool(string upload_pic)
        {
            bool isyes = false;
            try
            {

                if (!string.IsNullOrWhiteSpace(upload_pic))
                {
                    string filePath = HttpContext.Current.Server.MapPath(@"/A_UpLoad/upload_pic/" + upload_pic);
                    System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                    if (file.Exists)
                    {
                        isyes = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "CommonFunc.cs_GetImgBool:" + ex.Message + "\\n" + ex.StackTrace);
            }
            return isyes;
        }
        #region 获取当前ID的所有子ID集后
        public static string classid_str = "";
        public static string GetChildrenId(int id)
        {
            string nav_str = "";
            string yi = "";
            DataTable dt = GetClassList("");
            DataRow[] drs = dt.Select("parentid= " + id);

            foreach (DataRow dr in drs)
            {
                int classid = int.Parse(dr["id"].ToString());
                yi = yi + classid + ",";
                nav_str = GetId(classid, dt) + yi;
            }
            classid_str = "";
            return Function.Encrypt(nav_str + id);
        }

        //绑定子分类
        public static string GetId(int cid, DataTable dt)
        {
            DataRow[] drs = dt.Select("parentid= " + cid);

            foreach (DataRow dr in drs)
            {
                int classid = int.Parse(dr["id"].ToString());
                classid_str = dr["id"] + "," + GetId(classid, dt);
            }
            return classid_str;
        }


        public static DataTable GetClassList(string strWhere)
        {
            string strsql = "select * from tbl_class where isshow=1 ";

            if (!string.IsNullOrWhiteSpace(strWhere))
            {
                strsql += strsql;
            }

            strsql += " order by orderid asc";
            return tbl_classbll.GetDatatable(strsql);
        }

        #endregion

    }
}

