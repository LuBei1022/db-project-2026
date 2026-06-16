using BLL;
using LiteratureManager.Common;
using Model;
using System;
using System.Data;

namespace Web.admin
{
    public partial class Admin_AuthorEdit : System.Web.UI.Page
    {
        private readonly BLLBase<Author> authorBll = new BLLBase<Author>();
        public bool isLoading = false;
        public string MenuId = Function.GetRequest("MenuId");
        public string Action = Function.GetRequest("Action");
        public int AuthorId = 0;
        public string PageTitle = "新增作者";
        public string NameCn = string.Empty;
        public string NameEn = string.Empty;
        public string CurrentInstitution = string.Empty;
        public string CurrentInstitutionSourceHtml = string.Empty;
        public string Orcid = string.Empty;
        public string Email = string.Empty;
        public string IdentityStatus = "confirmed";
        public int Status = 1;
        public string BackUrl = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            AuthorId = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            if (string.IsNullOrWhiteSpace(Action))
            {
                Action = AuthorId > 0 ? "Edit" : "Add";
            }
            BackUrl = "Admin_AuthorList.aspx?MenuId=" + Server.UrlEncode(MenuId);

            if (Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                SaveAuthor();
                return;
            }

            if (Action.Equals("Edit", StringComparison.OrdinalIgnoreCase))
            {
                LoadAuthor();
            }
            else
            {
                CurrentInstitution = "保存作者后，请在具体论文的后台编辑页维护该作者在论文中的机构归属。";
            }
        }

        private void LoadAuthor()
        {
            PageTitle = "编辑作者";
            Author author = authorBll.SelectSingle("id=" + AuthorId + " and status<>-1");
            if (author == null || author.id <= 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "未找到对应作者记录", BackUrl, 1);
                return;
            }

            NameCn = Decode(author.name_cn);
            NameEn = Decode(author.name_en);
            CurrentInstitution = Decode(author.current_institution_name);
            if (string.IsNullOrWhiteSpace(CurrentInstitution))
            {
                CurrentInstitution = Decode(author.institution);
            }
            if (string.IsNullOrWhiteSpace(CurrentInstitution))
            {
                CurrentInstitution = "暂无当前机构。当前机构会根据该作者最后发表论文中的机构自动计算。";
            }
            CurrentInstitutionSourceHtml = BuildCurrentInstitutionSourceHtml(author);
            Orcid = Decode(author.orcid);
            Email = Decode(author.email);
            IdentityStatus = string.IsNullOrWhiteSpace(author.identity_status) ? "auto" : author.identity_status;
            Status = author.status;
        }

        private void SaveAuthor()
        {
            isLoading = false;
            string nameCn = CleanForm("name_cn");
            string nameEn = CleanForm("name_en");
            string orcid = CleanForm("orcid");
            string email = CleanForm("email");
            string identityStatus = CleanForm("identity_status");
            int status = Function.ConvertTo<int>(Request.Form["status"], 1);

            if (string.IsNullOrWhiteSpace(nameCn) && string.IsNullOrWhiteSpace(nameEn))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "中文名和英文名至少填写一个", BackUrl, 2);
                return;
            }

            Author author = Action.Equals("Edit", StringComparison.OrdinalIgnoreCase)
                ? authorBll.SelectSingle("id=" + AuthorId + " and status<>-1")
                : new Author();
            if (Action.Equals("Edit", StringComparison.OrdinalIgnoreCase) && (author == null || author.id <= 0))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "编辑的作者不存在", BackUrl, 2);
                return;
            }

            if (HasDuplicateOrcid(orcid, author == null ? 0 : author.id))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "ORCID 已被其他作者使用，请先确认是否为同一作者", BackUrl, 2);
                return;
            }

            author.name_cn = LiteratureRelationSync.EncodeForColumn(nameCn, 100);
            author.name_en = LiteratureRelationSync.EncodeForColumn(nameEn, 200);
            if (!Action.Equals("Edit", StringComparison.OrdinalIgnoreCase))
            {
                author.institution = string.Empty;
                author.current_institution_id = null;
                author.current_institution_name = string.Empty;
                author.current_institution_literature_id = null;
                author.current_institution_sort_date = null;
                author.current_institution_precision = "unknown";
                author.addtime = DateTime.Now;
            }
            author.orcid = LiteratureRelationSync.EncodeForColumn(orcid, 50);
            author.email = LiteratureRelationSync.EncodeForColumn(email, 200);
            author.identity_status = NormalizeIdentityStatus(identityStatus);
            author.status = status == 0 ? 0 : 1;
            author.updatetime = DateTime.Now;

            bool success;
            if (Action.Equals("Edit", StringComparison.OrdinalIgnoreCase))
            {
                success = authorBll.Update(new[] { "id" }, author);
            }
            else
            {
                author.id = Function.ConvertTo<int>(Convert.ToString(authorBll.AddIdentity(author, "id")), 0);
                success = author.id > 0;
            }

            if (!success)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "作者保存失败", BackUrl, 2);
                return;
            }

            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "作者保存成功", "Admin_AuthorInfo.aspx?ID=" + author.id + "&MenuId=" + MenuId, 0);
        }

        private bool HasDuplicateOrcid(string orcid, int currentId)
        {
            if (string.IsNullOrWhiteSpace(orcid))
            {
                return false;
            }
            string where = "status<>-1 and orcid=N'" + SqlLiteral(Function.HtmlEncode(orcid)) + "'";
            if (currentId > 0)
            {
                where += " and id<>" + currentId;
            }
            return authorBll.Exists(where);
        }

        private string BuildCurrentInstitutionSourceHtml(Author author)
        {
            if (author == null || !author.current_institution_literature_id.HasValue || author.current_institution_literature_id.Value <= 0)
            {
                return string.Empty;
            }

            DataTable dt = authorBll.GetDatatable("select title from Literature where id=" + author.current_institution_literature_id.Value);
            string title = string.Empty;
            if (dt != null && dt.Rows.Count > 0)
            {
                title = Decode(dt.Rows[0]["title"]);
                dt.Dispose();
            }

            return "<div class=\"author-hint\">当前机构来源于该作者最后发表论文：<a href=\"Admin_LiteratureEdit.aspx?Action=Edit&ID="
                + author.current_institution_literature_id.Value
                + "&MenuId=" + Server.UrlEncode(MenuId)
                + "\">" + Server.HtmlEncode(string.IsNullOrWhiteSpace(title) ? ("文献ID " + author.current_institution_literature_id.Value) : title)
                + "</a>。如需修改，请进入论文编辑页调整该作者在本文中的机构。</div>";
        }

        private string NormalizeIdentityStatus(string value)
        {
            string text = (value ?? string.Empty).Trim().ToLowerInvariant();
            if (text == "confirmed" || text == "unconfirmed" || text == "merged" || text == "split_needed")
            {
                return text;
            }
            return "confirmed";
        }

        private string CleanForm(string key)
        {
            return (Request.Form[key] ?? string.Empty).Trim();
        }

        private string Decode(object value)
        {
            return Function.HtmlDiscode(Convert.ToString(value ?? string.Empty));
        }

        private string SqlLiteral(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }
    }
}
