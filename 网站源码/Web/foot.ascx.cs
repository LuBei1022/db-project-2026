using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Collections.Generic;

namespace Web
{
    public partial class foot : System.Web.UI.UserControl
    {
        BLLBase<websiteinfo_list> websiteinfo_listbll = new BLLBase<websiteinfo_list>();
        BLLBase<tbl_class> tbl_classbll = new BLLBase<tbl_class>();
        BLLBase<link_list> link_listbll = new BLLBase<link_list>();
        public static websiteinfo_list websiteinfo_list = new websiteinfo_list();
        public static bool isImgLink = false;
        public static bool isTbClassLink = false;
        public string FooterPrivacyHref = "/Website/news";
        public string FooterTermsHref = "/Website/news";
        public string FooterSupportHref = "/User/ServiceLog";
        public string FooterAboutHref = "/Website/news";
        public string FooterContactHref = "/Website/news";
        public string FooterPrivacyContent = "<p>暂无内容。</p>";
        public string FooterTermsContent = "<p>暂无内容。</p>";
        public string FooterSupportContent = "<p>暂无内容。</p>";
        public string FooterAboutContent = "<p>暂无内容。</p>";
        public string FooterContactContent = "<p>暂无内容。</p>";
        public string FooterGitHubHref = "https://github.com/LuBei1022/db-project-2026/";
        public bool FooterGitHubVisible = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                isImgLink = false;
                isTbClassLink = false;
                websiteinfo_list = websiteinfo_listbll.SelectSingle("id=1");

                DataTable tbl_classdt = tbl_classbll.GetDatatable("select id,classname,model from tbl_class where isshow=1 and isfoot=1 and parentid=360 order by orderid asc,id asc");
                if (tbl_classdt != null && tbl_classdt.Rows.Count > 0)
                {
                    this.MenuClassList.DataSource = tbl_classdt.DefaultView;
                    this.MenuClassList.DataBind();
                    isTbClassLink = true;
                }
                tbl_classdt.Dispose();

                BindFooterContentLinks();
                BindFooterIconLinks();


                DataTable link_list_foot_dt = link_listbll.GetDatatable("select id,upload_pic_icon,url from link_list where isshow=1 and type=1 order by orderid asc,uptime asc,id asc");
                if (link_list_foot_dt != null && link_list_foot_dt.Rows.Count > 0)
                {
                    this.ImgLinkList.DataSource = link_list_foot_dt.DefaultView;
                    this.ImgLinkList.DataBind();
                    isImgLink = true;
                }
                link_list_foot_dt.Dispose();
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "foot.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
            }
        }

        private void BindFooterIconLinks()
        {
            DataTable dt = link_listbll.GetDatatable("select top 1 name,url,isshow from link_list where type=1 and name=N'GitHub' order by orderid asc,id asc");
            if (dt == null || dt.Rows.Count == 0)
            {
                if (dt != null)
                {
                    dt.Dispose();
                }
                return;
            }

            int isshow = Function.ConvertTo<int>(Convert.ToString(dt.Rows[0]["isshow"]), 0);
            string url = Function.HtmlDiscode(Convert.ToString(dt.Rows[0]["url"]));
            FooterGitHubVisible = isshow == 1;
            if (!string.IsNullOrWhiteSpace(url))
            {
                FooterGitHubHref = url.Trim();
            }
            dt.Dispose();
        }

        private void BindFooterContentLinks()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            Dictionary<string, string> contentMap = new Dictionary<string, string>();
            DataTable dt = tbl_classbll.GetDatatable("select id,classname,model,info_ from tbl_class where isshow=1 and parentid=360 order by orderid asc,id asc");
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string name = Function.HtmlDiscode(Convert.ToString(row["classname"])).Trim();
                    if (string.IsNullOrWhiteSpace(name) || map.ContainsKey(name))
                    {
                        continue;
                    }

                    string id = Convert.ToString(row["id"]);
                    map[name] = CommonFunc.GetTopHtmlHref(id, "0");
                    contentMap[name] = NormalizeFooterContent(Convert.ToString(row["info_"]));
                }
                dt.Dispose();
            }

            FooterPrivacyHref = GetFooterHref(map, "隐私政策");
            FooterPrivacyContent = GetFooterContent(contentMap, "隐私政策");
            FooterTermsHref = GetFooterHref(map, "用户协议");
            FooterTermsContent = GetFooterContent(contentMap, "用户协议");
            string communityContent = GetFooterContent(contentMap, "社区准则");
            if (communityContent != "<p>暂无内容。</p>")
            {
                FooterTermsContent += "<h3>社区准则</h3>" + communityContent;
            }
            if (FooterTermsHref == "javascript:void(0);")
            {
                FooterTermsHref = GetFooterHref(map, "社区准则");
            }
            FooterSupportHref = GetFooterHref(map, "常见问题");
            FooterSupportContent = GetFooterContent(contentMap, "常见问题");
            if (FooterSupportHref == "javascript:void(0);")
            {
                FooterSupportHref = "/User/ServiceLog";
            }
            FooterAboutHref = GetFooterHref(map, "关于我们");
            FooterContactHref = GetFooterHref(map, "联系我们");
            FooterAboutContent = GetFooterContent(contentMap, "关于我们");
            FooterContactContent = GetFooterContent(contentMap, "联系我们");
        }

        private string GetFooterHref(Dictionary<string, string> map, string name)
        {
            if (map != null && map.ContainsKey(name) && !string.IsNullOrWhiteSpace(map[name]))
            {
                return map[name];
            }
            return "javascript:void(0);";
        }

        private string GetFooterContent(Dictionary<string, string> map, string name)
        {
            if (map != null && map.ContainsKey(name) && !string.IsNullOrWhiteSpace(map[name]))
            {
                return map[name];
            }
            return "<p>暂无内容。</p>";
        }

        private string NormalizeFooterContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return "<p>暂无内容。</p>";
            }
            return Function.Replace_Content(content);
        }
    }
}
