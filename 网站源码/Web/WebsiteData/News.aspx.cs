using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;

namespace Web.Website
{
    public partial class News : System.Web.UI.Page
    {
        BLLBase<tbl_class> tbl_classbll = new BLLBase<tbl_class>();
        BLLBase<data_list> data_listbll = new BLLBase<data_list>();
        public int mid = 0;
        public bool webisyes = false;
        public tbl_class tbl_class = new tbl_class();
        public string tbclass_title = string.Empty;
        public string banner = string.Empty;
        public static int PageIndex = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    webisyes = false;
                    PageIndex = 0;
                    mid = Function.ConvertTo<int>(Function.GetRequest("mid"), 0);
                    if (mid <= 0)
                    {
                        mid = GetDefaultNewsClassId();
                    }
                    tbl_class = tbl_classbll.SelectSingle("id=" + mid + " and isshow=1 and model=2 and id in(" + Function.Decrypt(CommonFunc.GetChildrenId(360)) + ")");
                    if (tbl_class != null && tbl_class.id > 0 && tbl_class.parentid > 0)
                    {
                        tbclass_title = CommonFunc.GetTbClassTitle(tbl_class);
                        banner = CommonFunc.GetBannerImg(tbl_class.upload_pic_pc, tbl_class.upload_pic_m);
                        BindDataList();
                        webisyes = true;
                    }
                }
                catch (Exception ex)
                {
                    ImportDataLog.WriteLog(LogType.Error, "News.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
                }
                if (!webisyes)
                {
                    Response.Redirect("/err");
                    Response.End();
                }
            }
        }
        protected void BindDataList()
        {
            string Condition = " tbclass_id=" + tbl_class.id + " and isshow=1";


            ViewState["strWhere"] = Condition;
            string strWhere = ViewState["strWhere"].ToString();

            //表或视图名
            string tblName = "data_list";
            //需要返回的列
            string strGetFields = " id, name, upload_pic_img,datetime";
            //排序的字段名
            string fldname = " orderid desc,uptime desc,id desc";


            //每页显示的记录数
            int page_Size = 12;
            //统计总记录数
            int intRecordCount = data_listbll.GetCount(tblName, strWhere);

            PageIndex = Function.ConvertTo<int>(Function.GetRequest("page"), 0);
            if (PageIndex > 0)
            {
            }
            else
            {
                PageIndex = 1;
            }

            DataTable dt = data_listbll.GetListByPage(tblName, strGetFields, fldname, page_Size, PageIndex, strWhere);
            if (dt != null && dt.Rows.Count > 0)
            {
                this.DataList.DataSource = dt.DefaultView;
                this.DataList.DataBind();
            }
        }
        protected void AspNetPager1_PageChanged(object src, EventArgs e)
        {
            BindDataList();
        }

        private int GetDefaultNewsClassId()
        {
            DataTable dt = tbl_classbll.GetDatatable("select top 1 id from tbl_class where isshow=1 and model=2 and id in(" + Function.Decrypt(CommonFunc.GetChildrenId(360)) + ") order by orderid asc,id asc");
            if (dt != null && dt.Rows.Count > 0)
            {
                int classId = Function.ConvertTo<int>(dt.Rows[0]["id"], 0);
                dt.Dispose();
                return classId;
            }
            if (dt != null)
            {
                dt.Dispose();
            }
            return 0;
        }

        public string GetNewsBannerImage()
        {
            return string.IsNullOrWhiteSpace(banner) ? "/images/news-banner.svg" : banner;
        }

        public string GetNewsCardImage(object uploadPicObj, int itemIndex)
        {
            string uploadPic = uploadPicObj == null ? string.Empty : uploadPicObj.ToString();
            if (!string.IsNullOrWhiteSpace(uploadPic))
            {
                string uploaded = CommonFunc.GetWebUpload_Pic(uploadPic, string.Empty);
                if (!string.IsNullOrWhiteSpace(uploaded))
                {
                    return uploaded;
                }
            }

            int imageIndex = itemIndex % 4 + 1;
            return "/images/news-card-" + imageIndex + ".svg";
        }
    }
}
