using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace Web.admin
{
    public partial class Admin_LiteratureTag : System.Web.UI.Page
    {
        private readonly BLLBase<LiteratureTag> tagBll = new BLLBase<LiteratureTag>();
        private readonly string Action = Function.GetRequest("Action");
        public string MenuId = Function.GetRequest("MenuId");
        public bool isLoading = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            switch (Action)
            {
                case "Add":
                    AddFunc();
                    break;
                case "Edit":
                    EditFunc();
                    break;
                case "Del":
                    DelFunc();
                    break;
                default:
                    BindData();
                    break;
            }
        }

        protected void AddFunc()
        {
            AddUp.Visible = true;
            Main.Visible = false;
            Txt_Title.Text = "\u6DFB\u52A0\u6587\u732E\u6807\u7B7E";
        }

        protected void EditFunc()
        {
            int id = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            LiteratureTag model = tagBll.SelectSingle("id=" + id);
            if (model != null && model.id > 0)
            {
                AddUp.Visible = true;
                Main.Visible = false;
                Txt_Title.Text = "\u7F16\u8F91\u6587\u732E\u6807\u7B7E";
                name.Text = Function.HtmlDiscode(model.name);
                orderid.Text = model.orderid.ToString();
                SetDropDownValue(status, model.status.ToString(), "1");
            }
        }

        protected void DelFunc()
        {
            isLoading = false;
            AddUp.Visible = false;
            Main.Visible = false;
            string backUrl = Request.QueryString["BackURL"];
            if (string.IsNullOrWhiteSpace(backUrl))
            {
                backUrl = "Admin_LiteratureTag.aspx?MenuId=" + MenuId;
            }

            int id = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            LiteratureTag model = tagBll.SelectSingle("id=" + id);
            if (model != null && model.id > 0)
            {
                List<int> ids = GetTagIdsByDisplayName(Function.HtmlDiscode(model.name), "status<>-1");
                if (!ids.Contains(model.id))
                {
                    ids.Add(model.id);
                }

                if (tagBll.Update("status=-1", "id in(" + string.Join(",", ids.ToArray()) + ")"))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u6807\u7B7E\u300A" + Function.HtmlDiscode(model.name) + "\u300B\u5220\u9664\u6210\u529F!", backUrl, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u6807\u7B7E\u300A" + Function.HtmlDiscode(model.name) + "\u300B\u5220\u9664\u5931\u8D25!", backUrl, 2);
                }
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u83B7\u53D6\u5220\u9664\u7684\u53C2\u6570\u9519\u8BEF!", backUrl, 1);
            }
        }

        protected void BindData()
        {
            string condition = "status<>-1";
            string search = Function.GetRequest("SearchKeyWords");
            if (!string.IsNullOrWhiteSpace(search))
            {
                condition += " and name like N'%" + Function.HtmlEncode(search) + "%'";
                SearchKeyWords.Text = search;
            }

            AspNetPager1.PageSize = 15;
            int count = tagBll.GetCount("LiteratureTag", condition);
            DivNull.Visible = count <= 0;
            DataTable dt = tagBll.GetListByPage("LiteratureTag", "id,name,orderid,status,addtime", "orderid asc,id asc", AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, condition);
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

        protected void OnClick_AddUp(object sender, EventArgs e)
        {
            isLoading = false;
            AddUp.Visible = false;
            Main.Visible = false;
            string backUrl = Request.QueryString["BackURL"];
            if (string.IsNullOrWhiteSpace(backUrl))
            {
                backUrl = "Admin_LiteratureTag.aspx?MenuId=" + MenuId;
            }

            int id = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            LiteratureTag model = new LiteratureTag();
            if (Action == "Edit")
            {
                model = tagBll.SelectSingle("id=" + id);
                if (!(model != null && model.id > 0))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u8BF7\u6C42\u53C2\u6570\u9519\u8BEF!", backUrl, 2);
                    return;
                }
            }

            string cleanName = Function.HtmlDiscode(Function.FormRequest("name")).Trim();
            string nameText = Function.HtmlEncode(cleanName);
            if (ActiveTagNameExists(cleanName, Action == "Edit" ? id : 0))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u6807\u7B7E\u300A" + Function.HtmlDiscode(nameText) + "\u300B\u5DF2\u5B58\u5728!", backUrl, 2);
                return;
            }

            model.name = nameText;
            model.orderid = Function.ConvertTo<int>(Function.FormRequest("orderid"), 0);
            model.status = Function.ConvertTo<int>(Function.FormRequest("status"), 1);
            if (Action == "Add")
            {
                model.addtime = DateTime.Now;
                if (tagBll.Add(model, "id") > 0)
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u6807\u7B7E\u300A" + Function.HtmlDiscode(model.name) + "\u300B\u6DFB\u52A0\u6210\u529F!", backUrl, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u6807\u7B7E\u300A" + Function.HtmlDiscode(model.name) + "\u300B\u6DFB\u52A0\u5931\u8D25!", backUrl, 2);
                }
            }
            else
            {
                if (tagBll.Update(new[] { "id", "addtime" }, model))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u6807\u7B7E\u300A" + Function.HtmlDiscode(model.name) + "\u300B\u4FEE\u6539\u6210\u529F!", backUrl, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u6807\u7B7E\u300A" + Function.HtmlDiscode(model.name) + "\u300B\u4FEE\u6539\u5931\u8D25!", backUrl, 2);
                }
            }
        }

        protected void OnClick_Search(object sender, EventArgs e)
        {
            Response.Redirect(Request.CurrentExecutionFilePath + "?SearchKeyWords=" + Server.UrlEncode(Function.FormRequest("SearchKeyWords")) + "&MenuId=" + MenuId);
        }

        public string GetStatusText(object statusObj)
        {
            return Function.ConvertTo<int>(statusObj, 0) == 1 ? "\u542F\u7528" : "\u505C\u7528";
        }

        private void SetDropDownValue(System.Web.UI.WebControls.DropDownList ddl, string value, string fallback)
        {
            if (ddl.Items.FindByValue(value) != null)
            {
                ddl.SelectedValue = value;
            }
            else if (ddl.Items.FindByValue(fallback) != null)
            {
                ddl.SelectedValue = fallback;
            }
        }

        private bool ActiveTagNameExists(string tagName, int excludeId)
        {
            List<int> ids = GetTagIdsByDisplayName(tagName, "status<>-1");
            foreach (int id in ids)
            {
                if (id != excludeId)
                {
                    return true;
                }
            }
            return false;
        }

        private List<int> GetTagIdsByDisplayName(string tagName, string condition)
        {
            List<int> ids = new List<int>();
            string normalizedName = NormalizeTagDisplayName(tagName);
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return ids;
            }

            DataTable dt = tagBll.GetDatatable("select id,name from LiteratureTag where " + condition);
            if (dt == null)
            {
                return ids;
            }

            foreach (DataRow row in dt.Rows)
            {
                string current = NormalizeTagDisplayName(Function.HtmlDiscode(Convert.ToString(row["name"])));
                if (string.Equals(current, normalizedName, StringComparison.OrdinalIgnoreCase))
                {
                    ids.Add(Function.ConvertTo<int>(Convert.ToString(row["id"]), 0));
                }
            }
            dt.Dispose();
            return ids;
        }

        private string NormalizeTagDisplayName(string value)
        {
            return Regex.Replace(Function.HtmlDiscode(value ?? string.Empty), @"\s+", " ").Trim();
        }
    }
}
