using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace Web.admin
{
    public partial class Admin_LiteratureList : System.Web.UI.Page
    {
        private readonly BLLBase<Literature> literatureBll = new BLLBase<Literature>();
        private readonly BLLBase<LiteratureCategory> categoryBll = new BLLBase<LiteratureCategory>();
        private readonly BLLBase<LiteratureTag> tagBll = new BLLBase<LiteratureTag>();
        private readonly BLLBase<LiteratureAuthorMap> authorMapBll = new BLLBase<LiteratureAuthorMap>();
        private readonly BLLBase<LiteratureTagMap> tagMapBll = new BLLBase<LiteratureTagMap>();
        private readonly BLLBase<LiteratureExportLog> exportLogBll = new BLLBase<LiteratureExportLog>();
        private readonly BLLBase<NoticeLog_List> noticeLogBll = new BLLBase<NoticeLog_List>();
        private Dictionary<int, string> categoryMap;
        private readonly string Action = Function.GetRequest("Action");
        private const string DuplicateSubmissionRemarkPrefix = "[重复投稿]关联文献ID:";
        private const string MetadataRevisionRemarkPrefix = "[元数据修改]原文献ID:";
        public string MenuId = Function.GetRequest("MenuId");
        public bool isLoading = false;
        public bool IsReviewMode = false;
        public bool IsApprovedMode = false;
        public string ListTitle = "文献列表";
        public string ListSubtitle = string.Empty;
        public string ApprovedDashboardHtml = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            ResolveListMode();
            if (!IsPostBack)
            {
                BindCategoryDropDown();
                BindTagDropDown();
            }

            switch (Action)
            {
                case "Del":
                    DelFunc();
                    break;
                case "Approve":
                    ReviewFunc(1, "\u5BA1\u6838\u901A\u8FC7");
                    break;
                case "Reject":
                    ReviewFunc(2, "\u5BA1\u6838\u9A73\u56DE");
                    break;
                case "Export":
                    ExportFunc();
                    break;
                default:
                    BindData();
                    break;
            }
        }

        protected void BindCategoryDropDown()
        {
            SearchCategoryId.Items.Clear();
            SearchCategoryId.Items.Add(new System.Web.UI.WebControls.ListItem("\u5168\u90E8\u5206\u7C7B", "0"));
            DataTable dt = categoryBll.GetDatatable("select id,name from LiteratureCategory where status=1 order by orderid asc,id asc");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    SearchCategoryId.Items.Add(new System.Web.UI.WebControls.ListItem(row["name"].ToString(), row["id"].ToString()));
                }
            }

            string categoryId = Function.GetRequest("SearchCategoryId");
            if (!string.IsNullOrWhiteSpace(categoryId) && SearchCategoryId.Items.FindByValue(categoryId) != null)
            {
                SearchCategoryId.SelectedValue = categoryId;
            }
        }

        protected void BindTagDropDown()
        {
            SearchTagId.Items.Clear();
            SearchTagId.Items.Add(new System.Web.UI.WebControls.ListItem("\u5168\u90E8\u6807\u7B7E", "0"));
            DataTable dt = tagBll.GetDatatable("select id,name from LiteratureTag where status=1 order by orderid asc,id asc");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    SearchTagId.Items.Add(new System.Web.UI.WebControls.ListItem(row["name"].ToString(), row["id"].ToString()));
                }
            }

            string tagId = Function.GetRequest("SearchTagId");
            if (!string.IsNullOrWhiteSpace(tagId) && SearchTagId.Items.FindByValue(tagId) != null)
            {
                SearchTagId.SelectedValue = tagId;
            }
        }

        protected void BindData()
        {
            string condition = BuildCondition();
            if (IsApprovedMode)
            {
                BuildApprovedDashboard();
            }

            AspNetPager1.PageSize = 15;
            int intRecordCount = Function.ConvertTo<int>(literatureBll.GetDatatable("select count(1) as total_count from Literature l where " + condition).Rows[0]["total_count"].ToString(), 0);
            DivNull.Visible = intRecordCount <= 0;

            int startRow = (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + 1;
            int endRow = AspNetPager1.CurrentPageIndex * AspNetPager1.PageSize;
            DataTable dt = literatureBll.GetDatatable(@"
select *
from
(
    select
        rank() over(order by l.is_top desc,l.addtime desc,l.id desc) as xuhao,
        l.id,
        l.title,
        (select string_agg(coalesce(nullif(a.name_cn,N''),nullif(a.name_en,N''),N'未命名作者'),N'，') within group (order by m.author_order) from LiteratureAuthorMap m inner join Author a on a.id=m.author_id where m.literature_id=l.id) as author_names,
        l.category_id,
        l.publish_year,
        l.source_type,
        l.cover_pic,
        l.status,
        l.userid,
        l.import_batch_id,
        l.remark,
        l.canonical_literature_id,
        (select count(1) from LiteratureLike lk where lk.literature_id=l.id) as like_count,
        (select count(1) from LiteratureFavorite fav where fav.literature_id=l.id) as favorite_count,
        (select count(1) from LiteratureComment lc where lc.parent_id=0 and lc.is_deleted=0 and lc.status=1 and (lc.canonical_literature_id=l.id or lc.literature_id=l.id)) as comment_count,
        l.addtime,
        row_number() over(order by l.is_top desc,l.addtime desc,l.id desc) as row_no
    from Literature l
    where " + condition + @"
) t
where t.row_no between " + startRow + " and " + endRow + @"
order by t.row_no");
            AspNetPager1.RecordCount = intRecordCount;
            AspNetPager1.AlwaysShow = true;
            if (dt != null && dt.Rows.Count > 0)
            {
                Repeater1.DataSource = dt.DefaultView;
                Repeater1.DataBind();
            }
        }

        private void ResolveListMode()
        {
            string mode = Function.GetRequest("Mode");
            if (string.IsNullOrWhiteSpace(mode))
            {
                mode = MenuId == "1728" ? "Review" : "Approved";
            }

            IsReviewMode = string.Equals(mode, "Review", StringComparison.OrdinalIgnoreCase);
            IsApprovedMode = !IsReviewMode;
            ListTitle = IsReviewMode ? "待审核文献" : "文献列表";
            ListSubtitle = IsReviewMode
                ? "集中处理用户投稿、重复投稿和元数据修改审核。"
                : "已通过文献的运营监控与内容维护。";
        }

        private void BuildApprovedDashboard()
        {
            int literatureCount = GetScalarCount("select count(1) from Literature where status=1 and canonical_literature_id is null");
            int tagCount = GetScalarCount("select count(1) from LiteratureTag where status=1");
            int uncategorizedCount = GetScalarCount("select count(1) from Literature where status=1 and canonical_literature_id is null and isnull(category_id,0)=0");
            string tagSummary = GetTagSummaryHtml();

            StringBuilder html = new StringBuilder();
            html.Append("<div class=\"lit-monitor\"><div class=\"lit-monitor-head\"><div><strong>文献概览</strong><span>已通过文献的总体分布，单篇互动趋势请进入后台详情查看。</span></div><a href=\"Admin_LiteratureList.aspx?Mode=Review&MenuId=1728\">查看待审核文献</a></div>");
            html.Append("<div class=\"lit-monitor-stats\">");
            AppendMonitorStat(html, "总文献数", literatureCount);
            AppendMonitorStat(html, "标签数", tagCount);
            AppendMonitorStat(html, "未分类文献", uncategorizedCount);
            AppendMonitorStat(html, "当前筛选结果", Function.ConvertTo<int>(literatureBll.GetDatatable("select count(1) from Literature l where " + BuildCondition()).Rows[0][0], 0));
            html.Append("</div><div class=\"lit-tag-summary\"><strong>标签文献数</strong><div>");
            html.Append(tagSummary);
            html.Append("</div></div></div>");
            ApprovedDashboardHtml = html.ToString();
        }

        private void AppendMonitorStat(StringBuilder html, string label, int value)
        {
            html.Append("<div><span>");
            html.Append(label);
            html.Append("</span><strong>");
            html.Append(value);
            html.Append("</strong></div>");
        }

        private int GetScalarCount(string sql)
        {
            DataTable dt = literatureBll.GetDatatable(sql);
            int count = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                count = Function.ConvertTo<int>(Convert.ToString(dt.Rows[0][0]), 0);
            }
            if (dt != null)
            {
                dt.Dispose();
            }
            return count;
        }

        private string GetTagSummaryHtml()
        {
            DataTable dt = literatureBll.GetDatatable(@"
select top 24 t.name,count(l.id) as lit_count
from LiteratureTag t
left join LiteratureTagMap tm on tm.tag_id=t.id
left join Literature l on l.id=tm.literature_id and l.status=1 and l.canonical_literature_id is null
where t.status=1
group by t.id,t.name
order by lit_count desc,t.id asc");
            StringBuilder html = new StringBuilder();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    html.Append("<span>");
                    html.Append(Function.HtmlDiscode(Convert.ToString(row["name"])));
                    html.Append(" <em>");
                    html.Append(Function.ConvertTo<int>(Convert.ToString(row["lit_count"]), 0));
                    html.Append("</em></span>");
                }
                dt.Dispose();
            }
            if (html.Length == 0)
            {
                html.Append("<span>暂无标签 <em>0</em></span>");
            }
            return html.ToString();
        }

        protected void DelFunc()
        {
            isLoading = false;
            Main.Visible = false;
            string backUrl = Request.QueryString["BackURL"];
            if (string.IsNullOrWhiteSpace(backUrl))
            {
                backUrl = "Admin_LiteratureList.aspx?MenuId=" + MenuId;
            }

            int id = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            Literature literature = literatureBll.SelectSingle("id=" + id + " and status<>-1");
            if (literature != null && literature.id > 0)
            {
                if (literatureBll.Update("status=-1,updatetime=GETDATE()", "id=" + literature.id))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u6587\u732E\u300A" + Function.HtmlDiscode(literature.title) + "\u300B\u5220\u9664\u6210\u529F!", backUrl, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u6587\u732E\u300A" + Function.HtmlDiscode(literature.title) + "\u300B\u5220\u9664\u5931\u8D25!", backUrl, 2);
                }
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u83B7\u53D6\u5220\u9664\u7684\u53C2\u6570\u9519\u8BEF!", backUrl, 1);
            }
        }

        protected void ReviewFunc(int reviewStatus, string reviewName)
        {
            isLoading = false;
            Main.Visible = false;
            string backUrl = Request.QueryString["BackURL"];
            if (string.IsNullOrWhiteSpace(backUrl))
            {
                backUrl = "Admin_LiteratureList.aspx?MenuId=" + MenuId;
            }

            int id = Function.ConvertTo<int>(Function.GetRequest("ID"), 0);
            Literature literature = literatureBll.SelectSingle("id=" + id + " and status<>-1");
            if (literature != null && literature.id > 0)
            {
                int adminId = Function.ConvertTo<int>(Cookie.GetCookie("LMS_AdminID"), 0);
                int metadataMasterId = GetMetadataRevisionMasterId(literature.remark);
                if (reviewStatus == 1 && metadataMasterId > 0)
                {
                    if (ApplyMetadataRevision(literature, metadataMasterId, adminId))
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u5143\u6570\u636E\u4FEE\u6539\u300A" + Function.HtmlDiscode(literature.title) + "\u300B\u5DF2\u5BA1\u6838\u901A\u8FC7\u5E76\u5E94\u7528\u5230\u539F\u6587\u732E!", backUrl, 0);
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u5143\u6570\u636E\u4FEE\u6539\u300A" + Function.HtmlDiscode(literature.title) + "\u300B\u5BA1\u6838\u5931\u8D25!", backUrl, 2);
                    }
                    return;
                }

                int duplicateMasterId = GetDuplicateMasterId(literature.remark);
                if (duplicateMasterId <= 0 && literature.canonical_literature_id.HasValue)
                {
                    duplicateMasterId = literature.canonical_literature_id.Value;
                }
                if (reviewStatus == 1 && duplicateMasterId <= 0)
                {
                    duplicateMasterId = FindCanonicalMasterForApproval(literature);
                }
                if (reviewStatus == 1 && duplicateMasterId > 0)
                {
                    string mergeSql = "status=3,canonical_literature_id=" + duplicateMasterId + ",reviewed_by=" + adminId + ",review_time=GETDATE(),updatetime=GETDATE(),remark=N'\u91CD\u590D\u6295\u7A3F\u5BA1\u6838\u901A\u8FC7\uFF0C\u5171\u7528\u6587\u732EID:" + duplicateMasterId + "\u7684\u8BE6\u60C5\u9875'";
                    if (literatureBll.Update(mergeSql, "id=" + literature.id))
                    {
                        MergeDuplicateLiteratureData(literature.id, duplicateMasterId);
                        LiteratureRagSync.QueueReindex(duplicateMasterId);
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u91CD\u590D\u6295\u7A3F\u300A" + Function.HtmlDiscode(literature.title) + "\u300B\u5DF2\u5BA1\u6838\u901A\u8FC7\uFF0C\u5C06\u5171\u7528\u539F\u6587\u732E\u8BE6\u60C5\u9875!", backUrl, 0);
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u91CD\u590D\u6295\u7A3F\u300A" + Function.HtmlDiscode(literature.title) + "\u300B\u5BA1\u6838\u5931\u8D25!", backUrl, 2);
                    }
                    return;
                }

                string updateSql = "status=" + reviewStatus + ",reviewed_by=" + adminId + ",review_time=GETDATE(),updatetime=GETDATE()";
                if (reviewStatus == 1 && IsSystemReviewRemark(literature.remark))
                {
                    updateSql += ",remark=N'\u5BA1\u6838\u901A\u8FC7'";
                }
                else if (reviewStatus == 2 && IsSystemReviewRemark(literature.remark))
                {
                    updateSql += ",remark=N'\u8BF7\u4FEE\u6539\u540E\u91CD\u65B0\u63D0\u4EA4\u5BA1\u6838'";
                }

                if (literatureBll.Update(updateSql, "id=" + literature.id))
                {
                    literature.status = reviewStatus;
                    if (reviewStatus == 1)
                    {
                        LiteratureVenueProfileSync.EnsureForLiterature(literature);
                        LiteratureRagSync.QueueReindex(literature.id);
                    }
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u6587\u732E\u300A" + Function.HtmlDiscode(literature.title) + "\u300B" + reviewName + "\u6210\u529F!", backUrl, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u6587\u732E\u300A" + Function.HtmlDiscode(literature.title) + "\u300B" + reviewName + "\u5931\u8D25!", backUrl, 2);
                }
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u83B7\u53D6\u6587\u732E\u53C2\u6570\u9519\u8BEF!", backUrl, 1);
            }
        }

        private int FindCanonicalMasterForApproval(Literature literature)
        {
            if (literature == null || literature.id <= 0)
            {
                return 0;
            }

            string doi = NormalizeDoi(Function.HtmlDiscode(literature.doi));
            if (!string.IsNullOrWhiteSpace(doi))
            {
                int id = GetScalarCount("select top 1 id from Literature where id<>" + literature.id + " and status=1 and canonical_literature_id is null and LOWER(REPLACE(REPLACE(LTRIM(RTRIM(ISNULL(doi,''))),'https://doi.org/',''),'http://dx.doi.org/',''))='" + SqlLiteral(doi) + "' order by addtime asc,id asc");
                if (id > 0)
                {
                    return id;
                }
            }

            string titleKey = NormalizeTitle(Function.HtmlDiscode(literature.title));
            if (string.IsNullOrWhiteSpace(titleKey))
            {
                return 0;
            }
            return GetScalarCount("select top 1 id from Literature where id<>" + literature.id + " and status=1 and canonical_literature_id is null and LOWER(REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(ISNULL(title,''))),' ',''),CHAR(9),''),NCHAR(12288),''))='" + SqlLiteral(titleKey) + "' order by addtime asc,id asc");
        }

        private void MergeDuplicateLiteratureData(int duplicateId, int masterId)
        {
            if (duplicateId <= 0 || masterId <= 0 || duplicateId == masterId)
            {
                return;
            }

            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("DECLARE @duplicateId INT = " + duplicateId + ";");
                sql.AppendLine("DECLARE @masterId INT = " + masterId + ";");
                sql.AppendLine(@"
IF NOT EXISTS (SELECT 1 FROM LiteratureFile WHERE literature_id=@masterId AND status=1)
BEGIN
    UPDATE LiteratureFile
    SET literature_id=@masterId
    WHERE literature_id=@duplicateId AND status=1;
END;

DELETE dup
FROM LiteratureLike dup
WHERE dup.literature_id=@duplicateId
  AND EXISTS (
      SELECT 1 FROM LiteratureLike master
      WHERE master.literature_id=@masterId AND master.userid=dup.userid
  );

UPDATE LiteratureLike
SET literature_id=@masterId
WHERE literature_id=@duplicateId;

;WITH like_dedupe AS (
    SELECT id, ROW_NUMBER() OVER (PARTITION BY literature_id, userid ORDER BY id ASC) AS row_no
    FROM LiteratureLike
    WHERE literature_id=@masterId
)
DELETE FROM like_dedupe WHERE row_no>1;

DELETE dup
FROM LiteratureFavorite dup
WHERE dup.literature_id=@duplicateId
  AND EXISTS (
      SELECT 1 FROM LiteratureFavorite master
      WHERE master.literature_id=@masterId AND master.userid=dup.userid
  );

UPDATE LiteratureFavorite
SET literature_id=@masterId
WHERE literature_id=@duplicateId;

;WITH favorite_dedupe AS (
    SELECT id, ROW_NUMBER() OVER (PARTITION BY literature_id, userid ORDER BY id ASC) AS row_no
    FROM LiteratureFavorite
    WHERE literature_id=@masterId
)
DELETE FROM favorite_dedupe WHERE row_no>1;

UPDATE LiteratureDownloadLog
SET literature_id=@masterId
WHERE literature_id=@duplicateId;

UPDATE LiteratureComment
SET literature_id=@masterId,
    canonical_literature_id=@masterId,
    updatetime=GETDATE()
WHERE literature_id=@duplicateId
   OR canonical_literature_id=@duplicateId;

");
                literatureBll.GetExecSql(sql.ToString());
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "MergeDuplicateLiteratureData:" + ex.Message + "-" + ex.StackTrace);
            }
        }

        private string NormalizeDoi(string value)
        {
            string text = (value ?? string.Empty).Trim().ToLowerInvariant();
            text = text.Replace("https://doi.org/", string.Empty).Replace("http://dx.doi.org/", string.Empty);
            return text.Replace(" ", string.Empty).Replace("\t", string.Empty);
        }

        private string NormalizeTitle(string value)
        {
            string text = Function.HtmlEncode(Function.HtmlDiscode(value ?? string.Empty).Trim()).ToLowerInvariant();
            return text.Replace(" ", string.Empty).Replace("\t", string.Empty).Replace("\u3000", string.Empty);
        }

        private bool IsSystemReviewRemark(string remark)
        {
            string cleanRemark = Function.HtmlDiscode(remark ?? string.Empty).Trim();
            return string.IsNullOrWhiteSpace(cleanRemark)
                || cleanRemark == "\u7528\u6237\u524D\u53F0\u63D0\u4EA4\u5F85\u5BA1\u6838"
                || cleanRemark == "\u5BA1\u6838\u901A\u8FC7"
                || cleanRemark == "\u8BF7\u4FEE\u6539\u540E\u91CD\u65B0\u63D0\u4EA4\u5BA1\u6838";
        }

        private int GetDuplicateMasterId(string remark)
        {
            string cleanRemark = Function.HtmlDiscode(remark ?? string.Empty).Trim();
            if (!cleanRemark.StartsWith(DuplicateSubmissionRemarkPrefix, StringComparison.Ordinal))
            {
                return 0;
            }

            string idText = cleanRemark.Substring(DuplicateSubmissionRemarkPrefix.Length);
            int separatorIndex = idText.IndexOfAny(new[] { '\uFF1B', ';', ' ', '\r', '\n' });
            if (separatorIndex >= 0)
            {
                idText = idText.Substring(0, separatorIndex);
            }
            return Function.ConvertTo<int>(idText, 0);
        }

        private int GetMetadataRevisionMasterId(string remark)
        {
            string cleanRemark = Function.HtmlDiscode(remark ?? string.Empty).Trim();
            if (!cleanRemark.StartsWith(MetadataRevisionRemarkPrefix, StringComparison.Ordinal))
            {
                return 0;
            }

            string idText = cleanRemark.Substring(MetadataRevisionRemarkPrefix.Length);
            int separatorIndex = idText.IndexOfAny(new[] { '\uFF1B', ';', ' ', '\r', '\n' });
            if (separatorIndex >= 0)
            {
                idText = idText.Substring(0, separatorIndex);
            }
            return Function.ConvertTo<int>(idText, 0);
        }

        private bool ApplyMetadataRevision(Literature revision, int masterId, int adminId)
        {
            Literature master = literatureBll.SelectSingle("id=" + masterId + " and status<>-1");
            if (master == null || master.id <= 0)
            {
                return false;
            }

            master.title = revision.title;
            master.subtitle = revision.subtitle;
            master.institution = revision.institution;
            master.doi = revision.doi;
            master.keywords = revision.keywords;
            master.abstract_text = revision.abstract_text;
            master.source_type = revision.source_type;
            master.language = revision.language;
            master.publish_year = revision.publish_year;
            master.publish_month = revision.publish_month;
            master.publish_day = revision.publish_day;
            master.publish_date = revision.publish_date;
            master.publish_date_precision = revision.publish_date_precision;
            master.journal_name = revision.journal_name;
            master.conference_name = revision.conference_name;
            master.publisher = revision.publisher;
            master.volume = revision.volume;
            master.issue = revision.issue;
            master.pages = revision.pages;
            master.updatetime = DateTime.Now;

            bool masterUpdated = literatureBll.Update(new[] { "id" }, master);
            if (!masterUpdated)
            {
                return false;
            }

            string revisionAuthors = LiteratureRelationSync.GetAuthorNames(revision.id);
            string masterTags = LiteratureRelationSync.GetTagNames(master.id);
            LiteratureRelationSync.SyncMetadata(master, revisionAuthors, masterTags, BuildAuthorDetailsJson(revision.id));
            LiteratureVenueProfileSync.EnsureForLiterature(master);

            string updateRevisionSql = "status=4,reviewed_by=" + adminId + ",review_time=GETDATE(),updatetime=GETDATE(),remark=N'\u5143\u6570\u636E\u4FEE\u6539\u5DF2\u5BA1\u6838\u901A\u8FC7\u5E76\u5E94\u7528\u5230\u6587\u732EID:" + masterId + "\u3002'";
            bool revisionUpdated = literatureBll.Update(updateRevisionSql, "id=" + revision.id);
            if (revisionUpdated)
            {
                LiteratureRagSync.QueueReindex(master.id);
            }
            return revisionUpdated;
        }

        private string BuildAuthorDetailsJson(int literatureId)
        {
            DataTable dt = literatureBll.GetDatatable(@"
select
    m.author_id,
    coalesce(nullif(a.name_cn,N''),nullif(a.name_en,N'')) as author_name,
    a.name_cn,
    a.name_en,
    m.affiliation_text,
    (
        select string_agg(coalesce(nullif(i.name_cn,N''),nullif(i.name_en,N''),nullif(aim.affiliation_text,N'')),N'；') within group (order by aim.institution_order, aim.id)
        from LiteratureAuthorInstitutionMap aim
        left join Institution i on i.id=aim.institution_id and i.status<>-1
        where aim.literature_author_map_id=m.id
           or (isnull(aim.literature_author_map_id,0)=0 and aim.literature_id=m.literature_id and aim.author_id=m.author_id)
    ) as institution_names
from LiteratureAuthorMap m
inner join Author a on a.id=m.author_id
where m.literature_id=" + literatureId + @"
order by m.author_order asc,m.id asc");
            List<Dictionary<string, object>> details = new List<Dictionary<string, object>>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string name = Function.HtmlDiscode(Convert.ToString(row["author_name"]));
                    string affiliationText = Function.HtmlDiscode(Convert.ToString(row["institution_names"]));
                    if (string.IsNullOrWhiteSpace(affiliationText))
                    {
                        affiliationText = Function.HtmlDiscode(Convert.ToString(row["affiliation_text"]));
                    }
                    List<string> affiliations = SplitAffiliations(affiliationText);
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    item["author_id"] = Function.ConvertTo<int>(Convert.ToString(row["author_id"]), 0);
                    item["name"] = name;
                    item["name_cn"] = Function.HtmlDiscode(Convert.ToString(row["name_cn"]));
                    item["name_en"] = Function.HtmlDiscode(Convert.ToString(row["name_en"]));
                    item["affiliations"] = affiliations;
                    item["affiliation_text"] = string.Join("; ", affiliations.ToArray());
                    item["mapping_status"] = affiliations.Count > 0 ? "matched" : "unmatched";
                    details.Add(item);
                }
                dt.Dispose();
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(details);
        }

        private List<string> SplitAffiliations(string value)
        {
            List<string> values = new List<string>();
            foreach (string part in (value ?? string.Empty).Split(new[] { ';', '\uFF1B', '|', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string current = part.Trim();
                if (!string.IsNullOrWhiteSpace(current) && !values.Contains(current))
                {
                    values.Add(current);
                }
            }
            return values;
        }

        public string GetDuplicateMasterLinkHtml(object remarkObj, object canonicalObj)
        {
            int masterId = Function.ConvertTo<int>(Convert.ToString(canonicalObj), 0);
            if (masterId <= 0)
            {
                masterId = GetDuplicateMasterId(Convert.ToString(remarkObj));
            }
            if (masterId <= 0)
            {
                int metadataMasterId = GetMetadataRevisionMasterId(Convert.ToString(remarkObj));
                if (metadataMasterId <= 0)
                {
                    return string.Empty;
                }
                return "<a class=\"lit-action-btn\" href=\"Admin_LiteratureInfo.aspx?MenuId=" + MenuId + "&ID=" + metadataMasterId + "&BackURL=" + Function.GetEncodeURL() + "\">\u67E5\u770B\u539F\u6587\u732E</a> ";
            }

            return "<a class=\"lit-action-btn\" href=\"Admin_LiteratureInfo.aspx?MenuId=" + MenuId + "&ID=" + masterId + "&BackURL=" + Function.GetEncodeURL() + "\">\u67E5\u770B\u5DF2\u6709\u76F8\u540C\u6587\u732E</a> ";
        }

        protected void ExportFunc()
        {
            ExportCsv(BuildCondition(), "literature_export_");
        }

        protected void OnClick_BatchExport(object sender, EventArgs e)
        {
            List<int> ids = GetSelectedLiteratureIds();

            if (ids.Count <= 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u8BF7\u5148\u9009\u62E9\u8981\u5BFC\u51FA\u7684\u6587\u732E!", Request.RawUrl, 2);
                return;
            }

            string condition = BuildCondition() + " and l.id in(" + string.Join(",", ids.ToArray()) + ")";
            ExportCsv(condition, "literature_batch_export_");
        }

        protected void OnClick_BatchPdfExport(object sender, EventArgs e)
        {
            List<int> ids = GetSelectedLiteratureIds();
            if (ids.Count <= 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u8BF7\u5148\u9009\u62E9\u8981\u5BFC\u51FA\u7684\u6587\u732E!", Request.RawUrl, 2);
                return;
            }

            string condition = BuildCondition() + " and l.id in(" + string.Join(",", ids.ToArray()) + ")";
            DataTable dt = literatureBll.GetDatatable(@"
select
    l.id,
    l.title,
    f.file_path,
    f.file_name
from Literature l
inner join LiteratureFile f on f.literature_id=l.id and f.status=1
where " + condition + @"
order by l.is_top desc,l.addtime desc,l.id desc");

            List<AdminPdfExportItem> items = new List<AdminPdfExportItem>();
            string uploadRoot = Server.MapPath("~/A_UpLoad/upload_file/");
            string fullUploadRoot = Path.GetFullPath(uploadRoot);
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string relativePath = Function.HtmlDiscode(Convert.ToString(row["file_path"])).Replace("\\", "/").TrimStart('/');
                    if (string.IsNullOrWhiteSpace(relativePath))
                    {
                        continue;
                    }
                    string fullPath = Path.GetFullPath(Path.Combine(uploadRoot, relativePath.Replace("/", "\\")));
                    if (!fullPath.StartsWith(fullUploadRoot, StringComparison.OrdinalIgnoreCase) || !File.Exists(fullPath))
                    {
                        continue;
                    }

                    items.Add(new AdminPdfExportItem
                    {
                        LiteratureId = Function.ConvertTo<int>(Convert.ToString(row["id"]), 0),
                        Title = Function.HtmlDiscode(Convert.ToString(row["title"])),
                        FileName = Function.HtmlDiscode(Convert.ToString(row["file_name"])),
                        FullPath = fullPath
                    });
                }
                dt.Dispose();
            }

            if (items.Count <= 0)
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "\u9009\u4E2D\u7684\u6587\u732E\u6CA1\u6709\u53EF\u5BFC\u51FA\u7684 PDF \u9644\u4EF6!", Request.RawUrl, 2);
                return;
            }

            string zipPath = CreateAdminPdfZip(items);
            LiteratureExportLog exportLog = new LiteratureExportLog
            {
                export_name = Function.HtmlEncode("\u6587\u732EPDF\u6279\u91CF\u5BFC\u51FA_" + DateTime.Now.ToString("yyyyMMddHHmmss")),
                export_type = "ZIP",
                file_name = Function.HtmlEncode(Path.GetFileName(zipPath)),
                record_count = items.Count,
                userid = 0,
                addtime = DateTime.Now
            };
            exportLogBll.Add(exportLog, "id");
            SendZip(zipPath);
        }

        private List<int> GetSelectedLiteratureIds()
        {
            string[] selected = Request.Form.GetValues("lit_ids");
            List<int> ids = new List<int>();
            if (selected != null)
            {
                foreach (string item in selected)
                {
                    int id = Function.ConvertTo<int>(item, 0);
                    if (id > 0 && !ids.Contains(id))
                    {
                        ids.Add(id);
                    }
                }
            }
            return ids;
        }

        private void ExportCsv(string condition, string filePrefix)
        {
            DataTable dt = literatureBll.GetDatatable(@"
select
    l.id,
    l.title,
    (select string_agg(coalesce(nullif(a.name_cn,N''),nullif(a.name_en,N''),N'未命名作者'),N'，') within group (order by m.author_order) from LiteratureAuthorMap m inner join Author a on a.id=m.author_id where m.literature_id=l.id) as author_names,
    l.institution,
    l.doi,
    l.keywords,
    l.abstract_text,
    l.source_type,
    l.language,
    l.publish_year,
    l.journal_name,
    l.conference_name,
    l.publisher,
    l.volume,
    l.issue,
    l.pages,
    l.category_id,
    (select string_agg(t.name,N'，') from LiteratureTagMap m inner join LiteratureTag t on t.id=m.tag_id where m.literature_id=l.id and t.status<>-1) as tag_names,
    l.external_url,
    l.source_db,
    l.remark,
    l.status,
    l.is_top,
    l.addtime
from Literature l
where " + condition + @"
order by l.is_top desc,l.addtime desc,l.id desc");
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("id,title,author_names,institution,doi,keywords,abstract_text,source_type,language,publish_year,journal_name,conference_name,publisher,volume,issue,pages,category_id,tag_names,external_url,source_db,remark,status,is_top,addtime");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    csv.AppendLine(string.Join(",", new[]
                    {
                        EscapeCsv(row["id"].ToString()),
                        EscapeCsv(Function.HtmlDiscode(row["title"].ToString())),
                        EscapeCsv(Function.HtmlDiscode(row["author_names"].ToString())),
                        EscapeCsv(Function.HtmlDiscode(row["institution"].ToString())),
                        EscapeCsv(Function.HtmlDiscode(row["doi"].ToString())),
                        EscapeCsv(Function.HtmlDiscode(row["keywords"].ToString())),
                        EscapeCsv(Function.HtmlDiscode(row["abstract_text"].ToString())),
                        EscapeCsv(Function.HtmlDiscode(row["source_type"].ToString())),
                        EscapeCsv(Function.HtmlDiscode(row["language"].ToString())),
                        EscapeCsv(row["publish_year"].ToString()),
                        EscapeCsv(Function.HtmlDiscode(row["journal_name"].ToString())),
                        EscapeCsv(Function.HtmlDiscode(row["conference_name"].ToString())),
                        EscapeCsv(Function.HtmlDiscode(row["publisher"].ToString())),
                        EscapeCsv(Function.HtmlDiscode(row["volume"].ToString())),
                        EscapeCsv(Function.HtmlDiscode(row["issue"].ToString())),
                        EscapeCsv(Function.HtmlDiscode(row["pages"].ToString())),
                        EscapeCsv(row["category_id"].ToString()),
                        EscapeCsv(Function.HtmlDiscode(row["tag_names"].ToString())),
                        EscapeCsv(Function.HtmlDiscode(row["external_url"].ToString())),
                        EscapeCsv(Function.HtmlDiscode(row["source_db"].ToString())),
                        EscapeCsv(Function.HtmlDiscode(row["remark"].ToString())),
                        EscapeCsv(row["status"].ToString()),
                        EscapeCsv(row["is_top"].ToString()),
                        EscapeCsv(Function.ConvertTo<DateTime>(row["addtime"], DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss"))
                    }));
                }
            }

            LiteratureExportLog exportLog = new LiteratureExportLog
            {
                export_name = Function.HtmlEncode("\u6587\u732E\u5217\u8868\u5BFC\u51FA_" + DateTime.Now.ToString("yyyyMMddHHmmss")),
                export_type = "CSV",
                file_name = Function.HtmlEncode(filePrefix + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv"),
                record_count = dt == null ? 0 : dt.Rows.Count,
                userid = 0,
                addtime = DateTime.Now
            };
            exportLogBll.Add(exportLog, "id");

            string fileName = filePrefix + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "utf-8";
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + Server.UrlEncode(fileName));
            Response.Write("\uFEFF");
            Response.Write(csv.ToString());
            Response.End();
        }

        private string CreateAdminPdfZip(List<AdminPdfExportItem> items)
        {
            string zipRoot = Server.MapPath("~/A_UpLoad/upload_file/temp/");
            if (!Directory.Exists(zipRoot))
            {
                Directory.CreateDirectory(zipRoot);
            }
            CleanupExpiredZipFiles(zipRoot);
            string zipPath = Path.Combine(zipRoot, "admin-literature-pdf-" + Guid.NewGuid().ToString("N") + ".zip");
            try
            {
                using (FileStream stream = File.Create(zipPath))
                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create, false, Encoding.UTF8))
                {
                    HashSet<string> usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (AdminPdfExportItem item in items)
                    {
                        string entryName = GetZipEntryName(item, usedNames);
                        ZipArchiveEntry entry = archive.CreateEntry(entryName, CompressionLevel.Fastest);
                        using (Stream entryStream = entry.Open())
                        using (FileStream fileStream = File.OpenRead(item.FullPath))
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    }
                }
                return zipPath;
            }
            catch
            {
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }
                throw;
            }
        }

        private void CleanupExpiredZipFiles(string zipRoot)
        {
            try
            {
                foreach (string file in Directory.GetFiles(zipRoot, "admin-literature-pdf-*.zip"))
                {
                    if (File.GetLastWriteTimeUtc(file) < DateTime.UtcNow.AddHours(-1))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch
            {
            }
        }

        private string GetZipEntryName(AdminPdfExportItem item, HashSet<string> usedNames)
        {
            string baseName = string.IsNullOrWhiteSpace(item.Title) ? "literature-" + item.LiteratureId : item.Title;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                baseName = baseName.Replace(c, '_');
            }
            baseName = baseName.Trim();
            if (string.IsNullOrWhiteSpace(baseName))
            {
                baseName = "literature-" + item.LiteratureId;
            }

            string entryName = baseName + ".pdf";
            int index = 2;
            while (usedNames.Contains(entryName))
            {
                entryName = baseName + "-" + index + ".pdf";
                index++;
            }
            usedNames.Add(entryName);
            return entryName;
        }

        private void SendZip(string zipPath)
        {
            FileInfo file = new FileInfo(zipPath);
            Response.Clear();
            Response.ContentType = "application/zip";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"admin-literature-pdf-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip\"");
            Response.AddHeader("Content-Length", file.Length.ToString());
            using (FileStream fileStream = File.OpenRead(zipPath))
            {
                fileStream.CopyTo(Response.OutputStream);
            }
            Response.Flush();
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void AspNetPager1_PageChanged(object src, EventArgs e)
        {
            BindData();
        }

        protected void OnClick_Search(object sender, EventArgs e)
        {
            string where = "?btn=search&MenuId=" + MenuId + "&Mode=" + (IsReviewMode ? "Review" : "Approved");
            if (!string.IsNullOrWhiteSpace(SearchKeyWords.Text))
            {
                where += "&SearchKeyWords=" + Server.UrlEncode(SearchKeyWords.Text.Trim());
            }
            if (!string.IsNullOrWhiteSpace(SearchCategoryId.SelectedValue) && SearchCategoryId.SelectedValue != "0")
            {
                where += "&SearchCategoryId=" + SearchCategoryId.SelectedValue;
            }
            if (!string.IsNullOrWhiteSpace(SearchTagId.SelectedValue) && SearchTagId.SelectedValue != "0")
            {
                where += "&SearchTagId=" + SearchTagId.SelectedValue;
            }
            Response.Redirect(Request.CurrentExecutionFilePath + where);
        }

        private string BuildCondition()
        {
            string condition = "l.status<>-1";
            if (IsReviewMode)
            {
                condition += " and l.status=0";
            }
            else
            {
                condition += " and l.status=1 and l.canonical_literature_id is null";
            }

            string searchKeywords = Function.GetRequest("SearchKeyWords");
            if (!string.IsNullOrWhiteSpace(searchKeywords))
            {
                string safeKeywords = SqlLiteral(Function.HtmlEncode(searchKeywords));
                condition += " and (l.title like N'%" + safeKeywords + "%' or l.institution like N'%" + safeKeywords + "%' or l.doi like N'%" + safeKeywords + "%' or l.keywords like N'%" + safeKeywords + "%' or exists(select 1 from LiteratureAuthorMap m inner join Author a on a.id=m.author_id where m.literature_id=l.id and (a.name_cn like N'%" + safeKeywords + "%' or a.name_en like N'%" + safeKeywords + "%')) or exists(select 1 from LiteratureTagMap tm inner join LiteratureTag t on t.id=tm.tag_id where tm.literature_id=l.id and t.name like N'%" + safeKeywords + "%'))";
                SearchKeyWords.Text = searchKeywords;
            }

            int searchCategoryId = Function.ConvertTo<int>(Function.GetRequest("SearchCategoryId"), 0);
            if (searchCategoryId > 0)
            {
                condition += " and l.category_id=" + searchCategoryId;
            }

            int searchTagId = Function.ConvertTo<int>(Function.GetRequest("SearchTagId"), 0);
            if (searchTagId > 0)
            {
                condition += " and exists(select 1 from LiteratureTagMap tm where tm.literature_id=l.id and tm.tag_id=" + searchTagId + ")";
            }

            return condition;
        }

        private string SqlLiteral(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }

        private string EscapeCsv(string value)
        {
            string safe = (value ?? string.Empty).Replace("\"", "\"\"");
            return "\"" + safe.Replace("\r", " ").Replace("\n", " ") + "\"";
        }

        public string GetExportUrl()
        {
            string where = "?Action=Export&MenuId=" + MenuId + "&Mode=" + (IsReviewMode ? "Review" : "Approved");
            string searchKeywords = Function.GetRequest("SearchKeyWords");
            string searchCategoryId = Function.GetRequest("SearchCategoryId");
            string searchTagId = Function.GetRequest("SearchTagId");
            if (!string.IsNullOrWhiteSpace(searchKeywords))
            {
                where += "&SearchKeyWords=" + Server.UrlEncode(searchKeywords);
            }
            if (!string.IsNullOrWhiteSpace(searchCategoryId))
            {
                where += "&SearchCategoryId=" + searchCategoryId;
            }
            if (!string.IsNullOrWhiteSpace(searchTagId))
            {
                where += "&SearchTagId=" + searchTagId;
            }
            return Request.CurrentExecutionFilePath + where;
        }

        public string GetOperationHtml(object idObj, object remarkObj, object canonicalObj)
        {
            int id = Function.ConvertTo<int>(Convert.ToString(idObj), 0);
            StringBuilder html = new StringBuilder();
            html.Append(GetDuplicateMasterLinkHtml(remarkObj, canonicalObj));
            if (IsReviewMode)
            {
                html.Append("<a class=\"lit-action-btn\" href='?Action=Approve&Mode=Review&MenuId=");
                html.Append(MenuId);
                html.Append("&ID=");
                html.Append(id);
                html.Append("&BackURL=");
                html.Append(Function.GetEncodeURL());
                html.Append("'>通过</a> ");
                html.Append("<a class=\"lit-action-btn\" href='?Action=Reject&Mode=Review&MenuId=");
                html.Append(MenuId);
                html.Append("&ID=");
                html.Append(id);
                html.Append("&BackURL=");
                html.Append(Function.GetEncodeURL());
                html.Append("'>驳回</a> ");
            }
            else
            {
                html.Append("<a class=\"lit-action-btn\" href='Admin_LiteratureInfo.aspx?MenuId=");
                html.Append(MenuId);
                html.Append("&ID=");
                html.Append(id);
                html.Append("&BackURL=");
                html.Append(Function.GetEncodeURL());
                html.Append("'>后台详情</a> ");
            }
            html.Append("<a class=\"lit-action-btn\" href='Admin_LiteratureEdit.aspx?Action=Edit&MenuId=");
            html.Append(MenuId);
            html.Append("&ID=");
            html.Append(id);
            html.Append("&BackURL=");
            html.Append(Function.GetEncodeURL());
            html.Append("'>编辑</a> ");
            html.Append("<a class=\"lit-action-btn\" data-href='?Action=Del&Mode=");
            html.Append(IsReviewMode ? "Review" : "Approved");
            html.Append("&MenuId=");
            html.Append(MenuId);
            html.Append("&ID=");
            html.Append(id);
            html.Append("&BackURL=");
            html.Append(Function.GetEncodeURL());
            html.Append("' onclick=\"DataDelFunc(this)\">删除</a>");
            return html.ToString();
        }

        public string GetInteractionStatsHtml(object idObj, object likeObj, object favoriteObj, object commentObj)
        {
            int id = Function.ConvertTo<int>(Convert.ToString(idObj), 0);
            int likes = Function.ConvertTo<int>(Convert.ToString(likeObj), 0);
            int favorites = Function.ConvertTo<int>(Convert.ToString(favoriteObj), 0);
            int comments = Function.ConvertTo<int>(Convert.ToString(commentObj), 0);
            string detailUrl = "Admin_LiteratureInfo.aspx?MenuId=" + MenuId + "&ID=" + id + "&BackURL=" + Function.GetEncodeURL() + "#comments";
            return "<div class=\"lit-row-stats\"><span><b>\u70B9\u8D5E</b><em>" + likes + "</em></span><a href=\"" + detailUrl + "\"><b>\u8BC4\u8BBA</b><em>" + comments + "</em></a><span><b>\u6536\u85CF</b><em>" + favorites + "</em></span></div>";
        }

        public string GetCoverUrl(object coverPic)
        {
            string cover = coverPic == null ? string.Empty : coverPic.ToString();
            if (string.IsNullOrWhiteSpace(cover))
            {
                return "/admin/images/nophoto.gif";
            }
            return Function.GetAdminUpload_Pic(cover);
        }

        public string GetCategoryName(object categoryIdObj)
        {
            if (categoryMap == null)
            {
                categoryMap = new Dictionary<int, string>();
                DataTable dt = categoryBll.GetDatatable("select id,name from LiteratureCategory where status=1");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        int id = Function.ConvertTo<int>(row["id"], 0);
                        if (!categoryMap.ContainsKey(id))
                        {
                            categoryMap.Add(id, row["name"].ToString());
                        }
                    }
                }
            }

            int categoryId = Function.ConvertTo<int>(categoryIdObj, 0);
            if (categoryMap.ContainsKey(categoryId))
            {
                return Function.HtmlDiscodeWeb(categoryMap[categoryId]);
            }
            return "\u672A\u5206\u7C7B";
        }

        public string GetStatusText(object statusObj)
        {
            int status = Function.ConvertTo<int>(statusObj, 0);
            if (status == 1)
            {
                return "\u5BA1\u6838\u901A\u8FC7";
            }
            if (status == 2)
            {
                return "\u5BA1\u6838\u9A73\u56DE";
            }
            if (status == -1)
            {
                return "\u5DF2\u5220\u9664";
            }
            if (status == 3)
            {
                return "\u91CD\u590D\u6295\u7A3F\u5DF2\u5408\u5E76";
            }
            if (status == 4)
            {
                return "\u5143\u6570\u636E\u4FEE\u6539\u5DF2\u5E94\u7528";
            }
            return "\u5F85\u5BA1\u6838";
        }

        public string GetSourceText(object userIdObj, object importBatchIdObj)
        {
            int userId = Function.ConvertTo<int>(userIdObj, 0);
            int importBatchId = Function.ConvertTo<int>(importBatchIdObj, 0);
            if (userId > 0)
            {
                return "\u524D\u53F0\u63D0\u4EA4";
            }
            if (importBatchId > 0)
            {
                return "\u540E\u53F0\u5BFC\u5165";
            }
            return "\u540E\u53F0\u65B0\u589E";
        }

        public string GetSourceBadgeHtml(object userIdObj, object importBatchIdObj)
        {
            int userId = Function.ConvertTo<int>(userIdObj, 0);
            int importBatchId = Function.ConvertTo<int>(importBatchIdObj, 0);
            string css = "lit-source-admin";
            if (userId > 0)
            {
                css = "lit-source-user";
            }
            else if (importBatchId > 0)
            {
                css = "lit-source-import";
            }

            return "<span class=\"lit-source-pill " + css + "\">" + GetSourceText(userIdObj, importBatchIdObj) + "</span>";
        }

        private class AdminPdfExportItem
        {
            public int LiteratureId { get; set; }
            public string Title { get; set; }
            public string FileName { get; set; }
            public string FullPath { get; set; }
        }
    }
}

