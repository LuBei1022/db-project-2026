using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Text;

namespace Web.UserCenter
{
    public partial class Center : System.Web.UI.Page
    {
        public user_list user_list = new user_list();
        private readonly BLLBase<Literature> literatureBll = new BLLBase<Literature>();
        public int TotalLiteratureCount = 0;
        public int PendingLiteratureCount = 0;
        public int ApprovedLiteratureCount = 0;
        public string PendingLiteratureHtml = string.Empty;
        public string RecentLiteratureHtml = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    PendingLiteratureHtml = string.Empty;
                    RecentLiteratureHtml = string.Empty;
                    user_list = CommonUserFunc.GetUserLoginStatus();
                    if (user_list != null && user_list.id > 0)
                    {
                        BindSummary();
                        BindPendingLiterature();
                        BindRecentLiterature();
                    }
                    else
                    {
                        Response.Redirect("/");
                    }
                }
                catch (Exception ex)
                {
                    ImportDataLog.WriteLog(LogType.Error, "Center.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
                }
            }
        }

        private void BindSummary()
        {
            string userCondition = "userid=" + user_list.id + " and status<>-1";
            TotalLiteratureCount = literatureBll.GetCount("Literature", userCondition);
            PendingLiteratureCount = literatureBll.GetCount("Literature", userCondition + " and status=0");
            ApprovedLiteratureCount = literatureBll.GetCount("Literature", userCondition + " and status in(1,3)");
        }

        private void BindRecentLiterature()
        {
            DataTable dt = literatureBll.GetDatatable("select top 6 id,title,source_type,publish_year,status,addtime,canonical_literature_id from Literature where userid=" + user_list.id + " and status<>-1 order by addtime desc,id desc");
            RecentLiteratureHtml = BuildLiteratureHtml(dt);
            if (dt != null)
            {
                dt.Dispose();
            }
        }

        private void BindPendingLiterature()
        {
            DataTable dt = literatureBll.GetDatatable("select top 20 id,title,source_type,publish_year,status,addtime,canonical_literature_id from Literature where userid=" + user_list.id + " and status=0 order by addtime desc,id desc");
            PendingLiteratureHtml = BuildLiteratureHtml(dt);
            if (dt != null)
            {
                dt.Dispose();
            }
        }

        private string BuildLiteratureHtml(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                int id = Function.ConvertTo<int>(row["id"].ToString(), 0);
                int canonicalId = dt.Columns.Contains("canonical_literature_id") && row["canonical_literature_id"] != DBNull.Value
                    ? Function.ConvertTo<int>(row["canonical_literature_id"].ToString(), 0)
                    : 0;
                int detailId = canonicalId > 0 ? canonicalId : id;
                string title = Function.HtmlDiscode(row["title"].ToString());
                string sourceType = Function.HtmlDiscode(row["source_type"].ToString());
                string publishYear = row["publish_year"] == DBNull.Value ? string.Empty : row["publish_year"].ToString();
                string statusText = GetStatusText(Function.ConvertTo<int>(row["status"].ToString(), 0));
                DateTime time = Function.ConvertTo<DateTime>(row["addtime"].ToString(), DateTime.MinValue);

                sb.Append("<a class=\"lit-center-item\" href=\"/LiteratureInfo.aspx?id=");
                sb.Append(detailId);
                sb.Append("\">");
                sb.Append("<h5>");
                sb.Append(Server.HtmlEncode(title));
                sb.Append("</h5><p>");
                if (!string.IsNullOrWhiteSpace(sourceType))
                {
                    sb.Append(Server.HtmlEncode(sourceType));
                }
                else
                {
                    sb.Append("文献投稿");
                }
                if (!string.IsNullOrWhiteSpace(publishYear))
                {
                    sb.Append(" | ");
                    sb.Append(Server.HtmlEncode(publishYear));
                }
                sb.Append(" | ");
                sb.Append(Server.HtmlEncode(statusText));
                if (time != DateTime.MinValue)
                {
                    sb.Append(" | 提交时间 ");
                    sb.Append(time.ToString("yyyy-MM-dd HH:mm"));
                }
                sb.Append("</p></a>");
            }

            return sb.ToString();
        }

        private string GetStatusText(int status)
        {
            switch (status)
            {
                case 1:
                    return "已通过";
                case 2:
                    return "已驳回";
                case 3:
                    return "已合并";
                case 4:
                    return "修改已应用";
                default:
                    return "待审核";
            }
        }
    }
}
