using BLL;
using LiteratureManager.Common;
using Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Web.admin
{
    public partial class Admin_AuthorMerge : System.Web.UI.Page
    {
        private readonly BLLBase<Author> authorBll = new BLLBase<Author>();
        public bool isLoading = false;
        public string MenuId = Function.GetRequest("MenuId");
        public int MasterAuthorId = 0;
        public int DuplicateAuthorId = 0;
        public string Remark = string.Empty;
        public string PreviewHtml = string.Empty;
        public string MasterAuthorOptionsHtml = string.Empty;
        public string DuplicateAuthorOptionsHtml = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            MasterAuthorId = Function.ConvertTo<int>(Request.Form["master_author_id"] ?? Function.GetRequest("MasterID"), 0);
            DuplicateAuthorId = Function.ConvertTo<int>(Request.Form["duplicate_author_id"] ?? Function.GetRequest("DuplicateID"), 0);
            Remark = Request.Form["remark"] ?? string.Empty;

            if (Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                Merge();
                return;
            }

            List<Author> authors = LoadAuthorOptions();
            MasterAuthorOptionsHtml = BuildAuthorOptionsHtml(authors, MasterAuthorId, "请选择主作者");
            DuplicateAuthorOptionsHtml = BuildAuthorOptionsHtml(authors, DuplicateAuthorId, "请选择重复作者");
            PreviewHtml = BuildPreviewHtml();
        }

        private void Merge()
        {
            isLoading = false;
            if (MasterAuthorId <= 0 || DuplicateAuthorId <= 0 || MasterAuthorId == DuplicateAuthorId)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "请输入不同且有效的主作者 ID 和重复作者 ID。", BuildBackUrl(), 2);
                return;
            }

            try
            {
                int adminId = Function.ConvertTo<int>(Cookie.GetCookie("LMS_AdminID"), 0);
                AuthorMergeService.MergeAuthors(MasterAuthorId, DuplicateAuthorId, adminId, Remark);
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "作者合并成功，重复作者的论文与机构历史已迁移到主作者。", "Admin_AuthorInfo.aspx?ID=" + MasterAuthorId + "&MenuId=" + Server.UrlEncode(MenuId), 0);
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "Admin_AuthorMerge:" + ex.Message + "-" + ex.StackTrace);
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "作者合并失败：" + ex.Message, BuildBackUrl(), 2);
            }
        }

        private string BuildPreviewHtml()
        {
            Author master = MasterAuthorId > 0 ? authorBll.SelectSingle("id=" + MasterAuthorId + " and status<>-1") : null;
            Author duplicate = DuplicateAuthorId > 0 ? authorBll.SelectSingle("id=" + DuplicateAuthorId + " and status<>-1") : null;
            return "主作者：" + Server.HtmlEncode(GetAuthorLabel(master, MasterAuthorId))
                + "<br />重复作者：" + Server.HtmlEncode(GetAuthorLabel(duplicate, DuplicateAuthorId))
                + "<br />提交前请确认二者确实为同一作者。";
        }

        private List<Author> LoadAuthorOptions()
        {
            return authorBll.SelectList(null, "status<>-1", "name_cn asc,name_en asc,id asc");
        }

        private string BuildAuthorOptionsHtml(List<Author> authors, int selectedId, string placeholder)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<option value=\"0\"");
            if (selectedId <= 0)
            {
                html.Append(" selected=\"selected\"");
            }
            html.Append(">").Append(Server.HtmlEncode(placeholder)).Append("</option>");

            foreach (Author author in authors)
            {
                if (author == null || author.id <= 0)
                {
                    continue;
                }

                html.Append("<option value=\"").Append(author.id).Append("\"");
                if (author.id == selectedId)
                {
                    html.Append(" selected=\"selected\"");
                }
                html.Append(">").Append(Server.HtmlEncode(BuildAuthorOptionLabel(author))).Append("</option>");
            }

            return html.ToString();
        }

        private string BuildAuthorOptionLabel(Author author)
        {
            return GetAuthorDisplayName(author) + " / ID " + author.id;
        }

        private string GetAuthorLabel(Author author, int id)
        {
            if (author == null || author.id <= 0)
            {
                return id > 0 ? ("ID " + id + " 未找到或已合并") : "未选择";
            }
            string name = GetAuthorDisplayName(author);
            string institution = Function.HtmlDiscode(author.current_institution_name);
            return "ID " + author.id + " / " + (string.IsNullOrWhiteSpace(name) ? "未命名作者" : name) + (string.IsNullOrWhiteSpace(institution) ? "" : " / " + institution);
        }

        private string GetAuthorDisplayName(Author author)
        {
            string cn = Function.HtmlDiscode(author.name_cn);
            string en = Function.HtmlDiscode(author.name_en);
            string name = !string.IsNullOrWhiteSpace(cn) ? cn : en;
            return string.IsNullOrWhiteSpace(name) ? "未命名作者" : name;
        }

        private string BuildBackUrl()
        {
            return "Admin_AuthorMerge.aspx?MasterID=" + MasterAuthorId + "&DuplicateID=" + DuplicateAuthorId + "&MenuId=" + Server.UrlEncode(MenuId);
        }
    }
}
