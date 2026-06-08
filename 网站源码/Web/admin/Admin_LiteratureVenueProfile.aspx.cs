using BLL;
using LiteratureManager.Common;
using Model;
using System;
using System.Data;

namespace Web.admin
{
    public partial class Admin_LiteratureVenueProfile : System.Web.UI.Page
    {
        private readonly BLLBase<LiteratureVenueProfile> profileBll = new BLLBase<LiteratureVenueProfile>();
        private readonly string Action = Function.GetRequest("Action");
        public string MenuId = Function.GetRequest("MenuId");
        public bool isLoading = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            if (Action == "Add") AddFunc();
            else if (Action == "Edit") EditFunc();
            else BindData();
        }

        private void AddFunc()
        {
            AddUp.Visible = true;
            Main.Visible = false;
            Txt_Title.Text = "添加期刊/会议资料";
            status.SelectedValue = "1";
        }

        private void EditFunc()
        {
            int id = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            LiteratureVenueProfile model = profileBll.SelectSingle("id=" + id + " and status<>-1");
            if (model == null || model.id <= 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "未找到资料记录!", "Admin_LiteratureVenueProfile.aspx?MenuId=" + MenuId, 1);
                return;
            }
            AddUp.Visible = true;
            Main.Visible = false;
            Txt_Title.Text = "编辑期刊/会议资料";
            SetValue(model);
        }

        private void BindData()
        {
            string condition = "status<>-1";
            string search = Function.GetRequest("SearchKeyWords");
            string type = Function.GetRequest("SearchType");
            string statusValue = Function.GetRequest("SearchStatus");
            if (!string.IsNullOrWhiteSpace(search))
            {
                condition += " and venue_name like N'%" + Function.HtmlEncode(search).Replace("'", "''") + "%'";
                SearchKeyWords.Text = search;
            }
            if (type == "journal" || type == "conference")
            {
                condition += " and venue_type=N'" + type + "'";
                SearchType.SelectedValue = type;
            }
            if (statusValue == "0" || statusValue == "1")
            {
                condition += " and status=" + statusValue;
                SearchStatus.SelectedValue = statusValue;
            }

            AspNetPager1.PageSize = 15;
            int count = profileBll.GetCount("LiteratureVenueProfile", condition);
            DivNull.Visible = count <= 0;
            DataTable dt = profileBll.GetListByPage("LiteratureVenueProfile", "id,venue_type,venue_name,impact_factor,jcr_quartile,issn,conference_level,status,updatetime", "status asc,updatetime desc,id desc", AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, condition);
            AspNetPager1.RecordCount = count;
            AspNetPager1.AlwaysShow = true;
            if (dt != null && dt.Rows.Count > 0)
            {
                Repeater1.DataSource = dt.DefaultView;
                Repeater1.DataBind();
            }
        }

        protected void AspNetPager1_PageChanged(object src, EventArgs e)
        {
            BindData();
        }

        protected void OnClick_Search(object sender, EventArgs e)
        {
            string url = "?MenuId=" + MenuId + "&btn=search";
            if (!string.IsNullOrWhiteSpace(SearchKeyWords.Text)) url += "&SearchKeyWords=" + Server.UrlEncode(SearchKeyWords.Text.Trim());
            if (!string.IsNullOrWhiteSpace(SearchType.SelectedValue)) url += "&SearchType=" + SearchType.SelectedValue;
            if (!string.IsNullOrWhiteSpace(SearchStatus.SelectedValue)) url += "&SearchStatus=" + SearchStatus.SelectedValue;
            Response.Redirect(Request.CurrentExecutionFilePath + url);
        }

        protected void OnClick_AddUp(object sender, EventArgs e)
        {
            string backUrl = Request.QueryString["BackURL"];
            if (string.IsNullOrWhiteSpace(backUrl)) backUrl = "Admin_LiteratureVenueProfile.aspx?MenuId=" + MenuId;
            int id = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            LiteratureVenueProfile model = Action == "Edit" ? profileBll.SelectSingle("id=" + id + " and status<>-1") : new LiteratureVenueProfile();
            if (model == null)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "请求参数错误!", backUrl, 2);
                return;
            }
            string type = Function.FormRequest("venue_type");
            string nameText = Function.FormRequest("venue_name").Trim();
            if ((type != "journal" && type != "conference") || string.IsNullOrWhiteSpace(nameText))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "类型和名称不能为空!", backUrl, 2);
                return;
            }
            string safeName = Function.HtmlEncode(nameText);
            string existsWhere = "status<>-1 and venue_type=N'" + type + "' and venue_name=N'" + safeName.Replace("'", "''") + "'";
            if (Action == "Edit") existsWhere += " and id<>" + id;
            if (profileBll.Exists(existsWhere))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "该期刊/会议资料已存在!", backUrl, 2);
                return;
            }
            FillModel(model, type, safeName);
            bool ok;
            if (Action == "Edit")
            {
                ok = profileBll.Update(new[] { "id", "addtime", "created_by" }, model);
            }
            else
            {
                model.addtime = DateTime.Now;
                model.created_by = Function.ConvertTo<int>(Cookie.GetCookie("LMS_AdminID"), 0);
                ok = profileBll.Add(model, "id") > 0;
            }
            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), ok ? "资料保存成功!" : "资料保存失败!", backUrl, ok ? 0 : 2);
        }

        private void SetValue(LiteratureVenueProfile model)
        {
            venue_type.SelectedValue = model.venue_type;
            venue_name.Text = Function.HtmlDiscode(model.venue_name);
            introduction.Text = Function.HtmlDiscode(model.introduction);
            impact_factor.Text = Function.HtmlDiscode(model.impact_factor);
            jcr_quartile.Text = Function.HtmlDiscode(model.jcr_quartile);
            issn.Text = Function.HtmlDiscode(model.issn);
            conference_level.Text = Function.HtmlDiscode(model.conference_level);
            conference_cycle.Text = Function.HtmlDiscode(model.conference_cycle);
            location.Text = Function.HtmlDiscode(model.location);
            website_url.Text = Function.HtmlDiscode(model.website_url);
            publisher.Text = Function.HtmlDiscode(model.publisher);
            remark.Text = Function.HtmlDiscode(model.remark);
            status.SelectedValue = model.status.ToString();
        }

        private void FillModel(LiteratureVenueProfile model, string type, string safeName)
        {
            model.venue_type = type;
            model.venue_name = safeName;
            model.introduction = Function.HtmlEncode(Function.FormRequest("introduction"));
            model.impact_factor = Function.HtmlEncode(Function.FormRequest("impact_factor"));
            model.jcr_quartile = Function.HtmlEncode(Function.FormRequest("jcr_quartile"));
            model.issn = Function.HtmlEncode(Function.FormRequest("issn"));
            model.conference_level = Function.HtmlEncode(Function.FormRequest("conference_level"));
            model.conference_cycle = Function.HtmlEncode(Function.FormRequest("conference_cycle"));
            model.location = Function.HtmlEncode(Function.FormRequest("location"));
            model.website_url = Function.HtmlEncode(Function.FormRequest("website_url"));
            model.publisher = Function.HtmlEncode(Function.FormRequest("publisher"));
            model.remark = Function.HtmlEncode(Function.FormRequest("remark"));
            model.status = Function.ConvertTo<int>(Function.FormRequest("status"), 1);
            model.updated_by = Function.ConvertTo<int>(Cookie.GetCookie("LMS_AdminID"), 0);
            model.updatetime = DateTime.Now;
        }

        public string GetTypeText(object value)
        {
            return Convert.ToString(value) == "journal" ? "期刊" : "会议";
        }

        public string GetStatusText(object value)
        {
            return Function.ConvertTo<int>(Convert.ToString(value), 0) == 1 ? "已维护" : "待维护";
        }

        public string GetImpactText(object impact, object level)
        {
            string text = Function.HtmlDiscode(Convert.ToString(impact));
            if (string.IsNullOrWhiteSpace(text)) text = Function.HtmlDiscode(Convert.ToString(level));
            return string.IsNullOrWhiteSpace(text) ? "暂无" : Server.HtmlEncode(text);
        }

        public string GetQuartileText(object quartile, object issnValue)
        {
            string q = Function.HtmlDiscode(Convert.ToString(quartile));
            string i = Function.HtmlDiscode(Convert.ToString(issnValue));
            string text = string.Empty;
            if (!string.IsNullOrWhiteSpace(q)) text += q;
            if (!string.IsNullOrWhiteSpace(i)) text += (text.Length > 0 ? " / " : "") + i;
            return string.IsNullOrWhiteSpace(text) ? "暂无" : Server.HtmlEncode(text);
        }
    }
}
