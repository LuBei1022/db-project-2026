using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace Web.admin
{
    public partial class Admin_LiteratureCategory : System.Web.UI.Page
    {
        private readonly BLLBase<LiteratureCategory> categoryBll = new BLLBase<LiteratureCategory>();
        private readonly string Action = Function.GetRequest("Action");
        public string MenuId = Function.GetRequest("MenuId");
        public bool isLoading = false;
        private Dictionary<int, string> parentMap;

        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            if (!IsPostBack)
            {
                BindParentDropDown(0);
            }

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
            Txt_Title.Text = "\u6DFB\u52A0\u6587\u732E\u5206\u7C7B";
        }

        protected void EditFunc()
        {
            int id = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            LiteratureCategory model = categoryBll.SelectSingle("id=" + id);
            if (model != null && model.id > 0)
            {
                AddUp.Visible = true;
                Main.Visible = false;
                Txt_Title.Text = "\u7F16\u8F91\u6587\u732E\u5206\u7C7B";
                BindParentDropDown(model.id);
                name.Text = Function.HtmlDiscode(model.name);
                name_en.Text = Function.HtmlDiscode(model.name_en);
                code.Text = Function.HtmlDiscode(model.code);
                SetDropDownValue(parent_id, model.parent_id.HasValue ? model.parent_id.Value.ToString() : "0", "0");
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
                backUrl = "Admin_LiteratureCategory.aspx?MenuId=" + MenuId;
            }

            int id = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            LiteratureCategory model = categoryBll.SelectSingle("id=" + id);
            if (model != null && model.id > 0)
            {
                if (categoryBll.Exists("parent_id=" + model.id))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u8BF7\u5148\u5220\u9664\u5B50\u5206\u7C7B\u540E\u518D\u64CD\u4F5C!", backUrl, 2);
                    return;
                }
                if (categoryBll.Update("status=-1,updatetime=GETDATE()", "id=" + model.id))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u5206\u7C7B\u300A" + Function.HtmlDiscode(model.name) + "\u300B\u5220\u9664\u6210\u529F!", backUrl, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u5206\u7C7B\u300A" + Function.HtmlDiscode(model.name) + "\u300B\u5220\u9664\u5931\u8D25!", backUrl, 2);
                }
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u83B7\u53D6\u5220\u9664\u7684\u53C2\u6570\u9519\u8BEF!", backUrl, 1);
            }
        }

        protected void BindParentDropDown(int currentId)
        {
            parent_id.Items.Clear();
            parent_id.Items.Add(new System.Web.UI.WebControls.ListItem("\u65E0", "0"));
            string sql = "select id,name from LiteratureCategory where status<>-1";
            if (currentId > 0)
            {
                sql += " and id<>" + currentId;
            }
            sql += " order by orderid asc,id asc";
            DataTable dt = categoryBll.GetDatatable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    parent_id.Items.Add(new System.Web.UI.WebControls.ListItem(row["name"].ToString(), row["id"].ToString()));
                }
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

            string tblName = "LiteratureCategory";
            string strGetFields = "id,name,name_en,code,parent_id,orderid,status,updatetime";
            string fldname = "orderid asc,id asc";

            AspNetPager1.PageSize = 15;
            int count = categoryBll.GetCount(tblName, condition);
            DivNull.Visible = count <= 0;
            DataTable dt = categoryBll.GetListByPage(tblName, strGetFields, fldname, AspNetPager1.PageSize, AspNetPager1.CurrentPageIndex, condition);
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
                backUrl = "Admin_LiteratureCategory.aspx?MenuId=" + MenuId;
            }

            int id = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            LiteratureCategory model = new LiteratureCategory();
            if (Action == "Edit")
            {
                model = categoryBll.SelectSingle("id=" + id);
                if (!(model != null && model.id > 0))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u8BF7\u6C42\u53C2\u6570\u9519\u8BEF!", backUrl, 2);
                    return;
                }
            }

            string nameText = Function.HtmlEncode(Function.FormRequest("name"));
            string existsWhere = "name='" + nameText + "' and status<>-1";
            if (Action == "Edit")
            {
                existsWhere += " and id not in(" + id + ")";
            }
            if (categoryBll.Exists(existsWhere))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u5206\u7C7B\u300A" + Function.HtmlDiscode(nameText) + "\u300B\u5DF2\u5B58\u5728!", backUrl, 2);
                return;
            }

            model.name = nameText;
            model.name_en = Function.HtmlEncode(Function.FormRequest("name_en"));
            model.code = Function.HtmlEncode(Function.FormRequest("code"));
            int parentIdValue = Function.ConvertTo<int>(Function.FormRequest("parent_id"), 0);
            model.parent_id = parentIdValue > 0 ? (int?)parentIdValue : null;
            model.orderid = Function.ConvertTo<int>(Function.FormRequest("orderid"), 0);
            model.status = Function.ConvertTo<int>(Function.FormRequest("status"), 1);
            model.updatetime = DateTime.Now;

            if (Action == "Add")
            {
                model.addtime = DateTime.Now;
                if (categoryBll.Add(model, "id") > 0)
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u5206\u7C7B\u300A" + Function.HtmlDiscode(model.name) + "\u300B\u6DFB\u52A0\u6210\u529F!", backUrl, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u5206\u7C7B\u300A" + Function.HtmlDiscode(model.name) + "\u300B\u6DFB\u52A0\u5931\u8D25!", backUrl, 2);
                }
            }
            else
            {
                if (categoryBll.Update(new[] { "id", "addtime" }, model))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u5206\u7C7B\u300A" + Function.HtmlDiscode(model.name) + "\u300B\u4FEE\u6539\u6210\u529F!", backUrl, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u5206\u7C7B\u300A" + Function.HtmlDiscode(model.name) + "\u300B\u4FEE\u6539\u5931\u8D25!", backUrl, 2);
                }
            }
        }

        protected void OnClick_Search(object sender, EventArgs e)
        {
            Response.Redirect(Request.CurrentExecutionFilePath + "?SearchKeyWords=" + Server.UrlEncode(Function.FormRequest("SearchKeyWords")) + "&MenuId=" + MenuId);
        }

        public string GetParentName(object parentIdObj)
        {
            EnsureParentMap();
            int parentIdValue = Function.ConvertTo<int>(parentIdObj, 0);
            if (parentIdValue <= 0)
            {
                return "\u65E0";
            }
            return parentMap.ContainsKey(parentIdValue) ? Function.HtmlDiscodeWeb(parentMap[parentIdValue]) : "\u65E0";
        }

        public string GetStatusText(object statusObj)
        {
            return Function.ConvertTo<int>(statusObj, 0) == 1 ? "\u542F\u7528" : "\u505C\u7528";
        }

        private void EnsureParentMap()
        {
            if (parentMap != null)
            {
                return;
            }
            parentMap = new Dictionary<int, string>();
            DataTable dt = categoryBll.GetDatatable("select id,name from LiteratureCategory where status<>-1");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    int id = Function.ConvertTo<int>(row["id"], 0);
                    if (!parentMap.ContainsKey(id))
                    {
                        parentMap.Add(id, row["name"].ToString());
                    }
                }
            }
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
    }
}
