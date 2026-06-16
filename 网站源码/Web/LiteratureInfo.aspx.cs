using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace Web
{
    public partial class LiteratureInfo : System.Web.UI.Page
    {
        private readonly BLLBase<Literature> literatureBll = new BLLBase<Literature>();
        private readonly BLLBase<LiteratureDownloadLog> downloadLogBll = new BLLBase<LiteratureDownloadLog>();
        private readonly BLLBase<integrateExchangeLog_list> exchangeLogBll = new BLLBase<integrateExchangeLog_list>();
        private readonly BLLBase<LiteratureComment> literatureCommentBll = new BLLBase<LiteratureComment>();
        private readonly BLLBase<user_list> userBll = new BLLBase<user_list>();
        private readonly BLLBase<LiteratureLike> literatureLikeBll = new BLLBase<LiteratureLike>();
        private readonly BLLBase<LiteratureFavorite> literatureFavoriteBll = new BLLBase<LiteratureFavorite>();
        private const string DuplicateMergedRemarkPrefix = "\u91CD\u590D\u6295\u7A3F\u5BA1\u6838\u901A\u8FC7\uFF0C\u5171\u7528\u6587\u732EID:";
        private const string MetadataRevisionRemarkPrefix = "[元数据修改]原文献ID:";
        private const string MetadataRevisionAppliedRemarkPrefix = "\u5143\u6570\u636E\u4FEE\u6539\u5DF2\u5BA1\u6838\u901A\u8FC7\u5E76\u5E94\u7528\u5230\u6587\u732EID:";
        public string pageTitle = "\u6587\u732E\u8BE6\u60C5";
        public string title = "\u672A\u627E\u5230\u6587\u732E";
        public string metaLine = string.Empty;
        public string doi = "\u6682\u65E0";
        public string institution = "\u6682\u65E0";
        public string authorInstitutionHtml = "\u6682\u65E0";
        public string journalName = "\u6682\u65E0";
        public string conferenceName = "\u6682\u65E0";
        public string keywords = "\u6682\u65E0";
        public string tags = "\u6682\u65E0";
        public string pages = "\u6682\u65E0";
        public string publisher = "\u6682\u65E0";
        public string sourceDb = "\u6682\u65E0";
        public string externalLinkHtml = "\u6682\u65E0";
        public string abstractText = "\u6682\u65E0\u6458\u8981";
        public string remark = "\u6682\u65E0\u5907\u6CE8";
        public string pdfLinkHtml = string.Empty;
        public string downloadPointsText = "0";
        public string statusNoticeHtml = string.Empty;
        public int literatureId = 0;
        public bool IsLogin = false;
        public string currentPageUrl = string.Empty;
        public string commentSectionHtml = string.Empty;
        public string citationModalHtml = string.Empty;
        public string detailCardClass = "lit-detail-card";
        public string ownerBadgeHtml = string.Empty;
        public string ownerPanelHtml = string.Empty;
        public string ownerMetaAuditHtml = string.Empty;
        public string ownerMetadataModalHtml = string.Empty;
        public string reactionToolsHtml = string.Empty;
        public string detailTipHtml = "* \u79EF\u5206\u8D2D\u4E70\u6216\u4F7F\u7528\u514D\u8D39\u4E0B\u8F7D\u5238\u540E\uFF0C\u53EF\u65E0\u9650\u6B21\u91CD\u590D\u4E0B\u8F7D\u3002";

        protected void Page_Load(object sender, EventArgs e)
        {
            int id = Function.ConvertTo<int>(Function.GetRequest("id"), 0);
            user_list currentUser = CommonUserFunc.GetUserLoginStatus();
            IsLogin = currentUser != null && currentUser.id > 0;
            Literature literature = literatureBll.SelectSingle("id=" + id + " and status in(1,3)");
            if ((literature == null || literature.id <= 0) && currentUser != null && currentUser.id > 0)
            {
                literature = literatureBll.SelectSingle("id=" + id + " and userid=" + currentUser.id);
            }
            if (literature == null || literature.id <= 0)
            {
                return;
            }
            int mergedMasterId = GetCanonicalLiteratureId(literature);
            if (mergedMasterId > 0 && mergedMasterId != literature.id)
            {
                Response.Redirect("/LiteratureInfo.aspx?id=" + mergedMasterId, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            int appliedRevisionMasterId = GetAppliedMetadataMasterId(literature.remark);
            if (literature.status == 4 && appliedRevisionMasterId > 0 && appliedRevisionMasterId != literature.id)
            {
                Response.Redirect("/LiteratureInfo.aspx?id=" + appliedRevisionMasterId, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            literatureId = literature.id;
            currentPageUrl = "/LiteratureInfo.aspx?id=" + literature.id;
            bool isUploader = currentUser != null && currentUser.id > 0 && literature.userid == currentUser.id;

            if (isUploader && string.Equals(Function.GetRequest("action"), "metadata_update", StringComparison.OrdinalIgnoreCase))
            {
                SubmitMetadataRevision(literature, currentUser);
                return;
            }

            if (Function.GetRequest("action") == "download")
            {
                if (literature.status != 1)
                {
                    Function.Show_Msg("\u8BE5\u6587\u732E\u5C1A\u672A\u5BA1\u6838\u901A\u8FC7\uFF0C\u6682\u4E0D\u652F\u6301\u4E0B\u8F7D\uFF01", "/LiteratureInfo.aspx?id=" + literature.id);
                    return;
                }
                DownloadLiterature(literature);
                return;
            }

            title = Safe(Function.HtmlDiscode(literature.title), "\u672A\u547D\u540D\u6587\u732E");
            pageTitle = title;
            doi = Safe(Function.HtmlDiscode(literature.doi), "\u6682\u65E0");
            institution = Safe(Function.HtmlDiscode(literature.institution), "\u6682\u65E0");
            authorInstitutionHtml = GetAuthorInstitutionHtml(literature.id);
            journalName = Safe(Function.HtmlDiscode(literature.journal_name), "\u6682\u65E0");
            conferenceName = Safe(Function.HtmlDiscode(literature.conference_name), "\u6682\u65E0");
            keywords = Safe(Function.HtmlDiscode(literature.keywords), "\u6682\u65E0");
            tags = Safe(LiteratureRelationSync.GetTagNames(literature.id), "\u6682\u65E0");
            pages = Safe(Function.HtmlDiscode(literature.pages), "\u6682\u65E0");
            publisher = Safe(Function.HtmlDiscode(literature.publisher), "\u6682\u65E0");
            sourceDb = Safe(Function.HtmlDiscode(literature.source_db), "\u6682\u65E0");
            abstractText = Safe(Function.HtmlDiscode(literature.abstract_text), "\u6682\u65E0\u6458\u8981");
            remark = Safe(Function.HtmlDiscode(literature.remark), "\u6682\u65E0\u5907\u6CE8");
            string authorNames = LiteratureRelationSync.GetAuthorNames(literature.id);
            if (isUploader)
            {
                detailCardClass = "lit-detail-card lit-owner-card";
                ownerBadgeHtml = "<span class=\"lit-owner-badge\">\u6211\u7684\u6295\u7A3F</span>";
                ownerPanelHtml = GetOwnerPanelHtml(literature);
                ownerMetaAuditHtml = GetOwnerMetaAuditHtml(literature, authorNames);
                ownerMetadataModalHtml = GetOwnerMetadataModalHtml(literature, authorNames);
                detailTipHtml = "\u8FD9\u662F\u4F60\u7684\u6295\u7A3F\u8BE6\u60C5\u9875\uFF0C\u7528\u4E8E\u67E5\u770B\u5BA1\u6838\u8FDB\u5EA6\u3001\u6838\u5BF9\u5143\u6570\u636E\u548C\u8BBF\u95EE\u9644\u4EF6\u3002";
            }
            downloadPointsText = isUploader ? "\u6295\u7A3F\u8005\u514D\u79EF\u5206" : (literature.download_points <= 0 ? "\u514D\u79EF\u5206" : literature.download_points + " \u79EF\u5206");
            statusNoticeHtml = GetStatusNoticeHtml(literature.status);

            StringBuilder meta = new StringBuilder();
            AppendMeta(meta, authorNames);
            AppendMeta(meta, FormatPublishDate(literature));
            AppendMeta(meta, Function.HtmlDiscode(literature.source_type));
            AppendMeta(meta, Function.HtmlDiscode(literature.language));
            metaLine = meta.ToString();
            citationModalHtml = GetCitationModalHtml(literature, authorNames);
            reactionToolsHtml = GetReactionToolsHtml(literature.id, currentUser);

            if (!string.IsNullOrWhiteSpace(literature.external_url))
            {
                string url = Function.HtmlDiscode(literature.external_url);
                externalLinkHtml = "<a href=\"" + url + "\" target=\"_blank\">" + Server.HtmlEncode(url) + "</a>";
            }

            if (literature.status != 1)
            {
                pdfLinkHtml = "<span class=\"disabled\">\u5BA1\u6838\u901A\u8FC7\u540E\u53EF\u4E0B\u8F7D</span>";
            }
            else if (!string.IsNullOrWhiteSpace(GetPrimaryPdfFile(literature.id)))
            {
                pdfLinkHtml = GetDownloadActionHtml(literature, currentUser);
            }
            else
            {
                pdfLinkHtml = "<span class=\"disabled\">\u6682\u65E0\u53EF\u4E0B\u8F7D\u9644\u4EF6</span>";
            }

            commentSectionHtml = GetCommentSectionHtml(literature.id, currentUser);
        }

        private int GetCanonicalLiteratureId(Literature literature)
        {
            if (literature == null || literature.id <= 0)
            {
                return 0;
            }
            if (literature.canonical_literature_id.HasValue && literature.canonical_literature_id.Value > 0)
            {
                return literature.canonical_literature_id.Value;
            }
            return literature.status == 3 ? GetMergedMasterLiteratureId(literature.remark) : 0;
        }

        private string GetReactionToolsHtml(int currentLiteratureId, user_list currentUser)
        {
            int userId = currentUser != null ? currentUser.id : 0;
            int likeCount = GetReactionCount(true, currentLiteratureId);
            int favoriteCount = GetReactionCount(false, currentLiteratureId);
            LiteratureLike like = userId > 0 ? literatureLikeBll.SelectSingle("literature_id=" + currentLiteratureId + " and userid=" + userId) : null;
            LiteratureFavorite favorite = userId > 0 ? literatureFavoriteBll.SelectSingle("literature_id=" + currentLiteratureId + " and userid=" + userId) : null;
            bool liked = like != null && like.id > 0;
            bool favorited = favorite != null && favorite.id > 0;

            StringBuilder html = new StringBuilder();
            html.Append("<button type=\"button\" class=\"lit-reaction-btn");
            html.Append(liked ? " active" : string.Empty);
            html.Append("\" data-action=\"like\" onclick=\"toggleLiteratureReaction(this, 'like')\"><span>");
            html.Append(liked ? "\u5DF2\u70B9\u8D5E" : "\u70B9\u8D5E");
            html.Append("</span><em id=\"litLikeCount\">");
            html.Append(likeCount);
            html.Append("</em></button>");
            html.Append("<button type=\"button\" class=\"lit-reaction-btn");
            html.Append(favorited ? " active" : string.Empty);
            html.Append("\" data-action=\"favorite\" onclick=\"toggleLiteratureReaction(this, 'favorite')\"><span>");
            html.Append(favorited ? "\u5DF2\u6536\u85CF" : "\u6536\u85CF");
            html.Append("</span><em id=\"litFavoriteCount\">");
            html.Append(favoriteCount);
            html.Append("</em></button>");
            return html.ToString();
        }

        private int GetReactionCount(bool isLike, int currentLiteratureId)
        {
            string table = isLike ? "LiteratureLike" : "LiteratureFavorite";
            DataTable dt = literatureBll.GetDatatable("select count(1) as num from " + table + " where literature_id=" + currentLiteratureId);
            int count = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                count = Function.ConvertTo<int>(Convert.ToString(dt.Rows[0]["num"]), 0);
            }
            if (dt != null)
            {
                dt.Dispose();
            }
            return count;
        }

        private string GetOwnerPanelHtml(Literature literature)
        {
            string statusText = GetOwnerStatusText(literature.status);
            string statusClass = literature.status == 1 ? "ok" : literature.status == 2 ? "reject" : literature.status == 3 ? "merged" : "pending";
            StringBuilder html = new StringBuilder();
            html.Append("<section class=\"lit-owner-panel\"><div class=\"lit-owner-panel-head\"><div><span>\u6295\u7A3F\u7BA1\u7406</span><strong>");
            html.Append(statusText);
            html.Append("</strong></div><div class=\"lit-owner-panel-actions\"><button type=\"button\" onclick=\"openOwnerMetadataModal()\">\u63D0\u4EA4\u5143\u6570\u636E\u4FEE\u6539</button><a href=\"/User/Center\">\u8FD4\u56DE\u6211\u7684\u6295\u7A3F</a></div></div><div class=\"lit-owner-stats\">");
            AppendOwnerStat(html, "\u6295\u7A3F\u65F6\u95F4", literature.addtime == DateTime.MinValue ? "\u6682\u65E0" : literature.addtime.ToString("yyyy-MM-dd HH:mm"));
            AppendOwnerStat(html, "\u5BA1\u6838\u72B6\u6001", "<em class=\"lit-owner-status " + statusClass + "\">" + statusText + "</em>", false);
            AppendOwnerStat(html, "\u5BA1\u6838\u65F6\u95F4", literature.review_time.HasValue ? literature.review_time.Value.ToString("yyyy-MM-dd HH:mm") : "\u6682\u672A\u5BA1\u6838");
            AppendOwnerStat(html, "\u4E0B\u8F7D\u8BBE\u7F6E", literature.download_points <= 0 ? "\u514D\u79EF\u5206" : literature.download_points + " \u79EF\u5206");
            html.Append("</div><p>");
            html.Append(GetOwnerStatusTip(literature.status));
            html.Append("</p></section>");
            return html.ToString();
        }

        private void SubmitMetadataRevision(Literature original, user_list currentUser)
        {
            if (HasPendingMetadataRevision(original.id, currentUser.id))
            {
                Function.Show_Msg("\u8FD9\u7BC7\u6587\u732E\u5DF2\u6709\u4E00\u6761\u5143\u6570\u636E\u4FEE\u6539\u7533\u8BF7\u6B63\u5728\u5F85\u5BA1\u6838\uFF0C\u8BF7\u7B49\u5F85\u5904\u7406\u540E\u518D\u63D0\u4EA4\u3002", "/LiteratureInfo.aspx?id=" + original.id);
                return;
            }

            string submittedTitle = (Request.Form["owner_title"] ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(submittedTitle))
            {
                Function.Show_Msg("\u6587\u732E\u6807\u9898\u4E0D\u80FD\u4E3A\u7A7A\uFF01", "/LiteratureInfo.aspx?id=" + original.id);
                return;
            }

            Literature revision = new Literature();
            revision.title = Function.HtmlEncode(submittedTitle);
            revision.subtitle = original.subtitle;
            revision.institution = LiteratureRelationSync.EncodeForColumn(Request.Form["owner_institution"], 500);
            revision.doi = Function.HtmlEncode((Request.Form["owner_doi"] ?? string.Empty).Trim());
            revision.keywords = Function.HtmlEncode((Request.Form["owner_keywords"] ?? string.Empty).Trim());
            revision.abstract_text = Function.HtmlEncode((Request.Form["owner_abstract_text"] ?? string.Empty).Trim());
            revision.source_type = original.source_type;
            revision.language = original.language;
            string publishDateError = ApplyPublicationDate(revision, Request.Form["owner_publish_year"], Request.Form["owner_publish_month"], Request.Form["owner_publish_day"]);
            if (!string.IsNullOrWhiteSpace(publishDateError))
            {
                Function.Show_Msg(publishDateError, "/LiteratureInfo.aspx?id=" + original.id);
                return;
            }
            revision.journal_name = Function.HtmlEncode((Request.Form["owner_journal_name"] ?? string.Empty).Trim());
            revision.conference_name = Function.HtmlEncode((Request.Form["owner_conference_name"] ?? string.Empty).Trim());
            revision.publisher = Function.HtmlEncode((Request.Form["owner_publisher"] ?? string.Empty).Trim());
            revision.volume = Function.HtmlEncode((Request.Form["owner_volume"] ?? string.Empty).Trim());
            revision.issue = Function.HtmlEncode((Request.Form["owner_issue"] ?? string.Empty).Trim());
            revision.pages = Function.HtmlEncode((Request.Form["owner_pages"] ?? string.Empty).Trim());
            revision.category_id = original.category_id;
            revision.cover_pic = original.cover_pic;
            revision.download_points = original.download_points;
            revision.external_url = original.external_url;
            revision.source_db = original.source_db;
            revision.remark = Function.HtmlEncode(MetadataRevisionRemarkPrefix + original.id + "\uFF1B\u7528\u6237\u4E8C\u6B21\u63D0\u4EA4\u5143\u6570\u636E\u4FEE\u6539\uFF0C\u5BA1\u6838\u901A\u8FC7\u540E\u5E94\u7528\u5230\u539F\u6587\u732E\u3002");
            revision.is_top = 0;
            revision.status = 0;
            revision.userid = currentUser.id;
            revision.addtime = DateTime.Now;
            revision.updatetime = DateTime.Now;

            int revisionId = Convert.ToInt32(literatureBll.AddIdentity(revision, "id"));
            if (revisionId > 0)
            {
                revision.id = revisionId;
                string ownerAuthorNames = (Request.Form["owner_author_names"] ?? string.Empty).Trim();
                string ownerAuthorDetails = (Request.Form["owner_author_details"] ?? string.Empty).Trim();
                LiteratureRelationSync.SyncMetadata(revision, ownerAuthorNames, string.Empty, ownerAuthorDetails);
                Function.Show_Msg("\u5143\u6570\u636E\u4FEE\u6539\u7533\u8BF7\u5DF2\u63D0\u4EA4\uFF0C\u8BF7\u7B49\u5F85\u540E\u53F0\u5BA1\u6838\u3002", "/LiteratureInfo.aspx?id=" + original.id);
            }
            else
            {
                Function.Show_Msg("\u63D0\u4EA4\u5931\u8D25\uFF0C\u8BF7\u7A0D\u540E\u518D\u8BD5\uFF01", "/LiteratureInfo.aspx?id=" + original.id);
            }
        }

        private bool HasPendingMetadataRevision(int literatureId, int userId)
        {
            string marker = Function.HtmlEncode(MetadataRevisionRemarkPrefix + literatureId).Replace("'", "''");
            return literatureBll.Exists("userid=" + userId + " and status=0 and remark like N'" + marker + "%'");
        }

        private string ApplyPublicationDate(Literature literature, string yearText, string monthText, string dayText)
        {
            int year = Function.ConvertTo<int>((yearText ?? string.Empty).Trim(), 0);
            int month = Function.ConvertTo<int>((monthText ?? string.Empty).Trim(), 0);
            int day = Function.ConvertTo<int>((dayText ?? string.Empty).Trim(), 0);

            if (year <= 0)
            {
                if (month > 0 || day > 0)
                {
                    return "填写发表月份或日期时必须同时填写发表年份。";
                }
                literature.publish_year = null;
                literature.publish_month = null;
                literature.publish_day = null;
                literature.publish_date = null;
                literature.publish_date_precision = "unknown";
                return string.Empty;
            }
            if (year < 1000 || year > 9999)
            {
                return "发表年份格式不正确。";
            }
            if (month < 0 || month > 12)
            {
                return "发表月份必须在 1-12 之间。";
            }
            if (month == 0 && day > 0)
            {
                return "填写发表日期时必须同时填写发表月份。";
            }
            if (day < 0 || day > 31)
            {
                return "发表日期格式不正确。";
            }

            literature.publish_year = year;
            literature.publish_month = month > 0 ? (int?)month : null;
            literature.publish_day = null;
            literature.publish_date = new DateTime(year, 12, 31);
            literature.publish_date_precision = "year";

            if (month > 0)
            {
                int maxDay = DateTime.DaysInMonth(year, month);
                if (day > maxDay)
                {
                    return "发表日期超过该月份最大天数。";
                }
                literature.publish_date = new DateTime(year, month, maxDay);
                literature.publish_date_precision = "month";
                if (day > 0)
                {
                    literature.publish_day = day;
                    literature.publish_date = new DateTime(year, month, day);
                    literature.publish_date_precision = "day";
                }
            }

            return string.Empty;
        }

        private string GetOwnerMetadataModalHtml(Literature literature, string authorNames)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<div class=\"lit-modal-mask\" id=\"ownerMetadataModal\"><div class=\"lit-modal lit-owner-meta-modal\">");
            html.Append("<div class=\"lit-modal-head\"><h3>\u63D0\u4EA4\u5143\u6570\u636E\u4FEE\u6539</h3><button type=\"button\" class=\"lit-modal-close\" onclick=\"closeOwnerMetadataModal()\">×</button></div>");
            html.Append("<form method=\"post\" action=\"/LiteratureInfo.aspx?id=");
            html.Append(literature.id);
            html.Append("&action=metadata_update\" onsubmit=\"return collectOwnerMetadata();\"><div class=\"lit-modal-body lit-owner-meta-body\">");
            html.Append("<input type=\"hidden\" name=\"owner_author_details\" id=\"owner_author_details\" value=\"\" />");
            html.Append("<p class=\"lit-modal-tip\">\u63D0\u4EA4\u540E\u4F1A\u751F\u6210\u4E00\u6761\u5F85\u5BA1\u6838\u4FEE\u6539\u8BB0\u5F55\uFF0C\u5BA1\u6838\u901A\u8FC7\u524D\u4E0D\u4F1A\u5F71\u54CD\u5DF2\u516C\u5F00\u7684\u6587\u732E\u5185\u5BB9\u3002</p>");
            AppendOwnerInput(html, "\u6587\u732E\u6807\u9898 *", "owner_title", Function.HtmlDiscode(literature.title), false);
            AppendOwnerInput(html, "\u4F5C\u8005", "owner_author_names", authorNames, false);
            AppendOwnerInput(html, "\u4F5C\u8005\u5355\u4F4D", "owner_institution", Function.HtmlDiscode(literature.institution), false);
            html.Append("<div class=\"lit-owner-author-actions\"><span>\u4F5C\u8005\u673A\u6784\u5BF9\u5E94\u5173\u7CFB</span><button type=\"button\" onclick=\"refreshOwnerAuthorRows()\">\u6309\u4F5C\u8005\u5B57\u6BB5\u5237\u65B0</button></div>");
            html.Append("<div id=\"ownerAuthorRefreshStatus\" class=\"lit-owner-author-refresh-status\"></div>");
            html.Append("<div id=\"ownerAuthorEditor\" class=\"lit-owner-author-editor\">");
            html.Append(BuildOwnerAuthorAffiliationEditorHtml(literature.id, authorNames));
            html.Append("</div>");
            html.Append("<div class=\"lit-owner-author-hint\">\u8FD9\u91CC\u586B\u5199\u7684\u662F\u6BCF\u4F4D\u4F5C\u8005\u5728\u672C\u6587\u4E2D\u7684\u673A\u6784\uFF1B\u591A\u4E2A\u673A\u6784\u7528\u5206\u53F7\u5206\u9694\u3002\u63D0\u4EA4\u540E\u8FDB\u5165\u540E\u53F0\u5BA1\u6838\uFF0C\u4E0D\u4F1A\u76F4\u63A5\u8986\u76D6\u516C\u5F00\u6587\u732E\u3002</div>");
            AppendOwnerInput(html, "DOI", "owner_doi", Function.HtmlDiscode(literature.doi), false);
            html.Append("<div class=\"lit-owner-meta-grid\">");
            AppendOwnerInput(html, "\u671F\u520A", "owner_journal_name", Function.HtmlDiscode(literature.journal_name), false);
            AppendOwnerInput(html, "\u4F1A\u8BAE", "owner_conference_name", Function.HtmlDiscode(literature.conference_name), false);
            html.Append("</div><div class=\"lit-owner-meta-grid\">");
            AppendOwnerInput(html, "\u53D1\u8868\u5E74\u4EFD", "owner_publish_year", literature.publish_year.HasValue ? literature.publish_year.Value.ToString() : string.Empty, false);
            AppendOwnerInput(html, "\u53D1\u8868\u6708\u4EFD", "owner_publish_month", literature.publish_month.HasValue ? literature.publish_month.Value.ToString() : string.Empty, false);
            html.Append("</div><div class=\"lit-owner-meta-grid\">");
            AppendOwnerInput(html, "\u53D1\u8868\u65E5\u671F", "owner_publish_day", literature.publish_day.HasValue ? literature.publish_day.Value.ToString() : string.Empty, false);
            AppendOwnerInput(html, "\u51FA\u7248\u793E", "owner_publisher", Function.HtmlDiscode(literature.publisher), false);
            html.Append("</div><div class=\"lit-owner-meta-grid\">");
            AppendOwnerInput(html, "\u5377", "owner_volume", Function.HtmlDiscode(literature.volume), false);
            AppendOwnerInput(html, "\u671F", "owner_issue", Function.HtmlDiscode(literature.issue), false);
            html.Append("</div>");
            AppendOwnerInput(html, "\u9875\u7801", "owner_pages", Function.HtmlDiscode(literature.pages), false);
            AppendOwnerInput(html, "\u5173\u952E\u8BCD", "owner_keywords", Function.HtmlDiscode(literature.keywords), false);
            AppendOwnerInput(html, "\u6458\u8981", "owner_abstract_text", Function.HtmlDiscode(literature.abstract_text), true);
            html.Append("</div><div class=\"lit-modal-foot\"><button type=\"button\" class=\"cancel\" onclick=\"closeOwnerMetadataModal()\">\u53D6\u6D88</button><button type=\"submit\" class=\"submit\">\u63D0\u4EA4\u5BA1\u6838</button></div></form></div></div>");
            return html.ToString();
        }

        private string BuildOwnerAuthorAffiliationEditorHtml(int literatureId, string authorNames)
        {
            StringBuilder html = new StringBuilder();
            DataTable dt = literatureBll.GetDatatable(@"
select
    m.author_id,
    coalesce(nullif(a.name_cn,N''), nullif(a.name_en,N''), nullif(m.display_author_name,N''), nullif(m.raw_author_text,N''), N'') as author_name,
    coalesce(
        nullif(
            stuff((
                select N'；' + coalesce(nullif(i.name_cn,N''), nullif(i.name_en,N''), nullif(aim.affiliation_text,N''))
                from LiteratureAuthorInstitutionMap aim
                left join Institution i on i.id=aim.institution_id and i.status<>-1
                where aim.literature_author_map_id=m.id
                   or (isnull(aim.literature_author_map_id,0)=0 and aim.literature_id=m.literature_id and aim.author_id=m.author_id)
                order by aim.institution_order, aim.id
                for xml path(''), type
            ).value('.','nvarchar(max)'),1,1,N''),
            N''
        ),
        nullif(m.affiliation_text,N''),
        N''
    ) as institution_names
from LiteratureAuthorMap m
inner join Author a on a.id=m.author_id
where m.literature_id=" + literatureId + @"
order by m.author_order,m.id");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string name = Function.HtmlDiscode(Convert.ToString(row["author_name"]));
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        continue;
                    }
                    string affiliation = Function.HtmlDiscode(Convert.ToString(row["institution_names"]));
                    AppendOwnerAuthorAffiliationRow(html, Function.ConvertTo<int>(Convert.ToString(row["author_id"]), 0), name, affiliation);
                }
                dt.Dispose();
            }

            if (html.Length == 0)
            {
                foreach (string name in SplitOwnerAuthorNames(authorNames))
                {
                    AppendOwnerAuthorAffiliationRow(html, 0, name, string.Empty);
                }
            }

            if (html.Length == 0)
            {
                html.Append("<div class=\"lit-owner-author-hint\">\u8BF7\u5148\u586B\u5199\u4F5C\u8005\u59D3\u540D\uFF0C\u518D\u5237\u65B0\u751F\u6210\u4F5C\u8005\u673A\u6784\u5BF9\u5E94\u5173\u7CFB\u3002</div>");
            }
            return html.ToString();
        }

        private void AppendOwnerAuthorAffiliationRow(StringBuilder html, int authorId, string authorName, string affiliation)
        {
            string cleanName = Function.HtmlDiscode(authorName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(cleanName))
            {
                return;
            }
            html.Append("<div class=\"lit-owner-author-row\" data-author-id=\"");
            html.Append(authorId);
            html.Append("\" data-original-name=\"");
            html.Append(Server.HtmlEncode(cleanName));
            html.Append("\"><input type=\"text\" data-owner-author-name=\"1\" value=\"");
            html.Append(Server.HtmlEncode(cleanName));
            html.Append("\" placeholder=\"\u4F5C\u8005\u59D3\u540D\" /><textarea data-owner-author-affiliation=\"1\" placeholder=\"\u8BE5\u4F5C\u8005\u5728\u672C\u6587\u4E2D\u7684\u673A\u6784\uFF1B\u591A\u4E2A\u673A\u6784\u7528\u5206\u53F7\u5206\u9694\">");
            html.Append(Server.HtmlEncode(Function.HtmlDiscode(affiliation ?? string.Empty)));
            html.Append("</textarea></div>");
        }

        private List<string> SplitOwnerAuthorNames(string value)
        {
            List<string> names = new List<string>();
            foreach (string part in Regex.Split(Function.HtmlDiscode(value ?? string.Empty), @"\s+(?:and|&)\s+|[,，;；|、\r\n]+", RegexOptions.IgnoreCase))
            {
                string current = part.Trim();
                if (!string.IsNullOrWhiteSpace(current) && !names.Contains(current))
                {
                    names.Add(current);
                }
            }
            return names;
        }

        private void AppendOwnerInput(StringBuilder html, string label, string name, string value, bool area)
        {
            html.Append("<label class=\"lit-owner-meta-field\"><span>");
            html.Append(label);
            html.Append("</span>");
            if (area)
            {
                html.Append("<textarea name=\"");
                html.Append(name);
                html.Append("\">");
                html.Append(Server.HtmlEncode(value ?? string.Empty));
                html.Append("</textarea>");
            }
            else
            {
                html.Append("<input type=\"text\" name=\"");
                html.Append(name);
                html.Append("\" value=\"");
                html.Append(Server.HtmlEncode(value ?? string.Empty));
                html.Append("\" />");
            }
            html.Append("</label>");
        }

        private void AppendOwnerStat(StringBuilder html, string label, string value)
        {
            AppendOwnerStat(html, label, Server.HtmlEncode(value), false);
        }

        private void AppendOwnerStat(StringBuilder html, string label, string value, bool encodeValue)
        {
            html.Append("<div><span>");
            html.Append(label);
            html.Append("</span><strong>");
            html.Append(encodeValue ? Server.HtmlEncode(value) : value);
            html.Append("</strong></div>");
        }

        private string GetOwnerMetaAuditHtml(Literature literature, string authorNames)
        {
            List<string> missing = GetMissingCitationFields(literature, authorNames, false);
            StringBuilder html = new StringBuilder();
            html.Append("<section class=\"lit-owner-audit\"><h3>\u5143\u6570\u636E\u6838\u5BF9</h3>");
            if (missing.Count == 0)
            {
                html.Append("<p class=\"ok\">\u5F15\u7528\u3001\u68C0\u7D22\u548C\u5C55\u793A\u6240\u9700\u7684\u6838\u5FC3\u5143\u6570\u636E\u5DF2\u8F83\u5B8C\u6574\u3002</p>");
            }
            else
            {
                html.Append("<p>\u8FD9\u4E9B\u5B57\u6BB5\u8FD8\u4E0D\u5B8C\u6574\uFF0C\u53EF\u80FD\u5F71\u54CD\u5F15\u7528\u751F\u6210\u548C\u68C0\u7D22\u547D\u4E2D\uFF1A");
                html.Append(Server.HtmlEncode(string.Join("\u3001", missing.ToArray())));
                html.Append("\u3002\u5982\u9700\u4FEE\u6539\uFF0C\u53EF\u5728\u5907\u6CE8\u6216\u8BC4\u8BBA\u4E2D\u8865\u5145\u8BF4\u660E\uFF0C\u7531\u540E\u53F0\u5BA1\u6838\u4EBA\u5458\u5904\u7406\u3002</p>");
            }
            html.Append("</section>");
            return html.ToString();
        }

        private string GetOwnerStatusText(int status)
        {
            if (status == 1)
            {
                return "\u5DF2\u901A\u8FC7";
            }
            if (status == 2)
            {
                return "\u5DF2\u9A73\u56DE";
            }
            if (status == 3)
            {
                return "\u5DF2\u5408\u5E76";
            }
            return "\u5F85\u5BA1\u6838";
        }

        private string GetOwnerStatusTip(int status)
        {
            if (status == 1)
            {
                return "\u8FD9\u7BC7\u6587\u732E\u5DF2\u516C\u5F00\u5C55\u793A\uFF0C\u4F60\u53EF\u4EE5\u50CF\u8BFB\u8005\u4E00\u6837\u67E5\u770B\u5F15\u7528\u3001\u8BC4\u8BBA\u548C\u9644\u4EF6\u4E0B\u8F7D\u60C5\u51B5\u3002";
            }
            if (status == 2)
            {
                return "\u8FD9\u7BC7\u6587\u732E\u6682\u672A\u901A\u8FC7\u5BA1\u6838\uFF0C\u8BF7\u6839\u636E\u5907\u6CE8\u6216\u901A\u77E5\u5185\u5BB9\u8865\u5145\u6750\u6599\u540E\u91CD\u65B0\u63D0\u4EA4\u3002";
            }
            if (status == 3)
            {
                return "\u8FD9\u6B21\u6295\u7A3F\u5DF2\u4E0E\u5E73\u53F0\u5DF2\u6709\u6587\u732E\u5408\u5E76\uFF0C\u540E\u7EED\u5C06\u5171\u7528\u540C\u4E00\u4E2A\u516C\u5F00\u8BE6\u60C5\u9875\u3002";
            }
            return "\u8FD9\u7BC7\u6587\u732E\u6B63\u5728\u7B49\u5F85\u540E\u53F0\u5BA1\u6838\uFF0C\u5BA1\u6838\u524D\u4EC5\u4F60\u672C\u4EBA\u53EF\u89C1\u3002";
        }

        private string GetStatusNoticeHtml(int status)
        {
            if (status == 1)
            {
                return string.Empty;
            }

            string text = status == 3
                ? "\u8BE5\u6295\u7A3F\u5DF2\u5BA1\u6838\u901A\u8FC7\u5E76\u4E0E\u5E73\u53F0\u5DF2\u6709\u6587\u732E\u5408\u5E76\uFF0C\u5C06\u5171\u7528\u539F\u6587\u732E\u8BE6\u60C5\u9875\u3002"
                : status == 2
                ? "\u8BE5\u6587\u732E\u5DF2\u88AB\u9A73\u56DE\uFF0C\u8BF7\u6839\u636E\u901A\u77E5\u6216\u540E\u53F0\u53CD\u9988\u4FEE\u6539\u540E\u91CD\u65B0\u63D0\u4EA4\u3002"
                : "\u8BE5\u6587\u732E\u6B63\u5728\u5F85\u5BA1\u6838\u72B6\u6001\uFF0C\u4EC5\u60A8\u53EF\u4EE5\u67E5\u770B\uFF0C\u5BA1\u6838\u901A\u8FC7\u540E\u624D\u4F1A\u516C\u5F00\u5C55\u793A\u3002";
            return "<div class=\"lit-status-notice\">" + text + "</div>";
        }

        private int GetMergedMasterLiteratureId(string remark)
        {
            string cleanRemark = Function.HtmlDiscode(remark ?? string.Empty).Trim();
            if (!cleanRemark.StartsWith(DuplicateMergedRemarkPrefix, StringComparison.Ordinal))
            {
                return 0;
            }

            string idText = cleanRemark.Substring(DuplicateMergedRemarkPrefix.Length);
            int separatorIndex = idText.IndexOfAny(new[] { '\u7684', '\uFF1B', ';', ' ', '\r', '\n' });
            if (separatorIndex >= 0)
            {
                idText = idText.Substring(0, separatorIndex);
            }
            return Function.ConvertTo<int>(idText, 0);
        }

        private int GetAppliedMetadataMasterId(string remark)
        {
            string cleanRemark = Function.HtmlDiscode(remark ?? string.Empty).Trim();
            if (!cleanRemark.StartsWith(MetadataRevisionAppliedRemarkPrefix, StringComparison.Ordinal))
            {
                return 0;
            }

            string idText = cleanRemark.Substring(MetadataRevisionAppliedRemarkPrefix.Length);
            int separatorIndex = idText.IndexOfAny(new[] { '\u3002', '\uFF1B', ';', ' ', '\r', '\n' });
            if (separatorIndex >= 0)
            {
                idText = idText.Substring(0, separatorIndex);
            }
            return Function.ConvertTo<int>(idText, 0);
        }

        private string GetDownloadActionHtml(Literature literature, user_list currentUser)
        {
            if (currentUser != null && currentUser.id > 0 && literature.userid == currentUser.id)
            {
                return "<a href=\"/LiteratureInfo.aspx?id=" + literature.id + "&action=download\">\u4E0B\u8F7D\u6211\u7684\u9644\u4EF6\uFF08\u514D\u79EF\u5206\uFF09</a>";
            }

            if (literature.download_points <= 0)
            {
                return "<a href=\"/LiteratureInfo.aspx?id=" + literature.id + "&action=download\">\u514D\u8D39\u4E0B\u8F7D\u9644\u4EF6</a>";
            }

            if (currentUser != null && currentUser.id > 0)
            {
                LiteratureDownloadLog log = downloadLogBll.SelectSingle("user_id=" + currentUser.id + " and literature_id=" + literature.id);
                if (log != null && log.id > 0)
                {
                    return "<a href=\"/LiteratureInfo.aspx?id=" + literature.id + "&action=download\">\u5DF2\u8D2D\u4E70\uFF0C\u7EE7\u7EED\u4E0B\u8F7D</a>";
                }
            }

            int couponCount = currentUser != null && currentUser.id > 0 ? GetAvailableCouponCount(currentUser.id) : 0;
            StringBuilder html = new StringBuilder();
            html.Append("<form class=\"lit-download-form\" method=\"get\" action=\"/LiteratureInfo.aspx\">");
            html.Append("<input type=\"hidden\" name=\"id\" value=\"");
            html.Append(literature.id);
            html.Append("\" />");
            html.Append("<input type=\"hidden\" name=\"action\" value=\"download\" />");
            html.Append("<div class=\"lit-pay-title\">\u9009\u62E9\u4E0B\u8F7D\u65B9\u5F0F</div>");
            html.Append("<label class=\"lit-pay-option\"><input type=\"radio\" name=\"pay_method\" value=\"points\" checked=\"checked\" /> <span>\u4F7F\u7528\u79EF\u5206\u4E0B\u8F7D</span><em>");
            html.Append(literature.download_points);
            html.Append(" \u79EF\u5206</em></label>");
            if (couponCount > 0)
            {
                html.Append("<label class=\"lit-pay-option\"><input type=\"radio\" name=\"pay_method\" value=\"coupon\" /> <span>\u4F7F\u7528\u514D\u8D39\u4E0B\u8F7D\u5238</span><em>\u5269\u4F59 ");
                html.Append(couponCount);
                html.Append(" \u5F20</em></label>");
            }
            else
            {
                html.Append("<label class=\"lit-pay-option unavailable\"><input type=\"radio\" disabled=\"disabled\" /> <span>\u514D\u8D39\u4E0B\u8F7D\u5238</span><em>\u6682\u65E0\u53EF\u7528</em></label>");
            }
            html.Append("<button type=\"submit\">\u786E\u8BA4\u4E0B\u8F7D</button>");
            html.Append("</form>");
            return html.ToString();
        }

        private void DownloadLiterature(Literature literature)
        {
            string pdfFile = GetPrimaryPdfFile(literature.id);
            if (string.IsNullOrWhiteSpace(pdfFile))
            {
                Function.Show_Msg("\u8BE5\u6587\u732E\u6682\u65E0\u53EF\u4E0B\u8F7D\u9644\u4EF6\uFF01", "/LiteratureInfo.aspx?id=" + literature.id);
                return;
            }

            user_list user = CommonUserFunc.GetUserLoginStatus();
            if (user == null || user.id <= 0)
            {
                Function.Show_Msg("\u8BF7\u5148\u767B\u5F55\u540E\u518D\u4E0B\u8F7D\u6587\u732E\uFF01", "/LiteratureInfo.aspx?id=" + literature.id);
                return;
            }

            bool firstDownload = false;
            bool couponDownload = false;
            bool isUploader = literature.userid == user.id;
            LiteratureDownloadLog log = downloadLogBll.SelectSingle("user_id=" + user.id + " and literature_id=" + literature.id);
            if (log == null || log.id <= 0)
            {
                if (isUploader)
                {
                    if (!SaveDownloadLog(literature, user.id, 0, false, 0, pdfFile))
                    {
                        Function.Show_Msg("\u4E0B\u8F7D\u72B6\u6001\u5DF2\u53D8\u66F4\uFF0C\u8BF7\u5237\u65B0\u540E\u91CD\u8BD5\uFF01", "/LiteratureInfo.aspx?id=" + literature.id);
                        return;
                    }
                }
                else
                {
                    string payMethod = Function.GetRequest("pay_method");
                    if (literature.download_points > 0 && string.Equals(payMethod, "coupon", StringComparison.OrdinalIgnoreCase))
                    {
                        integrateExchangeLog_list coupon = GetAvailableCoupon(user.id);
                        if (coupon == null || coupon.id <= 0)
                        {
                            Function.Show_Msg("\u6682\u65E0\u53EF\u7528\u7684\u514D\u8D39\u4E0B\u8F7D\u5238\uFF0C\u8BF7\u9009\u62E9\u79EF\u5206\u4E0B\u8F7D\u6216\u5148\u5151\u6362\u4E0B\u8F7D\u5238\uFF01", "/LiteratureInfo.aspx?id=" + literature.id);
                            return;
                        }

                        if (!SaveDownloadLog(literature, user.id, 0, false, coupon.id, pdfFile))
                        {
                            Function.Show_Msg("\u514D\u8D39\u4E0B\u8F7D\u5238\u72B6\u6001\u5DF2\u53D8\u66F4\uFF0C\u8BF7\u5237\u65B0\u540E\u91CD\u8BD5\uFF01", "/LiteratureInfo.aspx?id=" + literature.id);
                            return;
                        }
                        firstDownload = true;
                        couponDownload = true;
                    }
                    else
                    {
                        int userPoints = CommonUserFunc.GetUserIntegrateSumFunc(user.id, 0);
                        if (literature.download_points > userPoints)
                        {
                            Function.Show_Msg("\u79EF\u5206\u4E0D\u8DB3\uFF0C\u65E0\u6CD5\u4E0B\u8F7D\u8BE5\u6587\u732E\uFF01", "/LiteratureInfo.aspx?id=" + literature.id);
                            return;
                        }

                        if (!SaveDownloadLog(literature, user.id, literature.download_points, true, 0, pdfFile))
                        {
                            Function.Show_Msg("\u79EF\u5206\u4E0D\u8DB3\u6216\u4E0B\u8F7D\u72B6\u6001\u5DF2\u53D8\u66F4\uFF0C\u8BF7\u5237\u65B0\u540E\u91CD\u8BD5\uFF01", "/LiteratureInfo.aspx?id=" + literature.id);
                            return;
                        }
                        firstDownload = literature.download_points > 0;
                    }
                }
            }

            if (firstDownload)
            {
                string fileUrl = "/A_UpLoad/upload_file/" + pdfFile;
                string message = couponDownload
                    ? "\u5DF2\u4F7F\u7528 1 \u5F20\u514D\u8D39\u4E0B\u8F7D\u5238\uFF0C\u540E\u7EED\u53EF\u91CD\u590D\u4E0B\u8F7D\u3002"
                    : "\u5DF2\u6263\u9664 " + literature.download_points + " \u79EF\u5206\uFF0C\u540E\u7EED\u53EF\u91CD\u590D\u4E0B\u8F7D\u3002";
                Response.Write("<script>alert('" + message + "');window.location.href='" + fileUrl + "';</script>");
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            Response.Redirect("/A_UpLoad/upload_file/" + pdfFile, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private int GetAvailableCouponCount(int userId)
        {
            return exchangeLogBll.GetCount("integrateExchangeLog_list", "user_id=" + userId + " and status=1 and name like N'%\u514D\u8D39\u4E0B\u8F7D%'");
        }

        private integrateExchangeLog_list GetAvailableCoupon(int userId)
        {
            return exchangeLogBll.SelectSingle("user_id=" + userId + " and status=1 and name like N'%\u514D\u8D39\u4E0B\u8F7D%'");
        }

        private bool SaveDownloadLog(Literature literature, int userId, int chargedPoints, bool grantUploaderPoints, long couponId, string pdfFile)
        {
            string safeTitle = Function.HtmlEncode(Function.HtmlDiscode(literature.title)).Replace("'", "''");
            StringBuilder sql = new StringBuilder();
            AppendDownloadAuthorizationStart(sql, userId, chargedPoints, couponId);
            sql.Append("INSERT INTO LiteratureDownloadLog(literature_id,user_id,literature_title,file_url,download_points,literature_user_id,addtime) VALUES (");
            sql.Append(literature.id);
            sql.Append(",");
            sql.Append(userId);
            sql.Append(",N'");
            sql.Append(safeTitle);
            sql.Append("',N'");
            sql.Append(Function.HtmlEncode(pdfFile).Replace("'", "''"));
            sql.Append("',");
            sql.Append(chargedPoints);
            sql.Append(",");
            sql.Append(literature.userid);
            sql.Append(",GETDATE());");

            if (couponId > 0)
            {
                sql.Append(";UPDATE integrateExchangeLog_list SET status=-1,hexiaotime='");
                sql.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sql.Append("' WHERE id=");
                sql.Append(couponId);
                sql.Append(" AND user_id=");
                sql.Append(userId);
                sql.Append(" AND status=1");
            }

            if (chargedPoints > 0)
            {
                sql.Append(";INSERT INTO integrateLog_list (num_integrate, type, name, info_, addtime, user_id) VALUES (-");
                sql.Append(chargedPoints);
                sql.Append(",4,N'\u6587\u732E\u4E0B\u8F7D',N'\u4E0B\u8F7D\u6587\u732E\u300A");
                sql.Append(safeTitle);
                sql.Append("\u300B\u6263\u9664");
                sql.Append(chargedPoints);
                sql.Append("\u79EF\u5206',GETDATE(),");
                sql.Append(userId);
                sql.Append(")");

                if (grantUploaderPoints && literature.userid > 0 && literature.userid != userId)
                {
                    sql.Append(";INSERT INTO integrateLog_list (num_integrate, type, name, info_, addtime, user_id) VALUES (");
                    sql.Append(chargedPoints);
                    sql.Append(",5,N'\u6587\u732E\u88AB\u4E0B\u8F7D',N'\u60A8\u7684\u6587\u732E\u300A");
                    sql.Append(safeTitle);
                    sql.Append("\u300B\u88AB\u4E0B\u8F7D\uFF0C\u83B7\u5F97");
                    sql.Append(chargedPoints);
                    sql.Append("\u79EF\u5206',GETDATE(),");
                    sql.Append(literature.userid);
                    sql.Append(")");
                }
            }

            AppendDownloadAuthorizationEnd(sql, chargedPoints, couponId);
            return downloadLogBll.Sql_D(sql.ToString());
        }

        private void AppendDownloadAuthorizationStart(StringBuilder sql, int userId, int chargedPoints, long couponId)
        {
            if (chargedPoints > 0)
            {
                sql.Append("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; IF ((SELECT ISNULL(SUM(num_integrate),0) FROM integrateLog_list WITH (UPDLOCK,HOLDLOCK) WHERE user_id=");
                sql.Append(userId);
                sql.Append(") >= ");
                sql.Append(chargedPoints);
                sql.Append(") BEGIN ");
            }
            else if (couponId > 0)
            {
                sql.Append("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; IF EXISTS(SELECT 1 FROM integrateExchangeLog_list WITH (UPDLOCK,HOLDLOCK) WHERE id=");
                sql.Append(couponId);
                sql.Append(" AND user_id=");
                sql.Append(userId);
                sql.Append(" AND status=1) BEGIN ");
            }
        }

        private void AppendDownloadAuthorizationEnd(StringBuilder sql, int chargedPoints, long couponId)
        {
            if (chargedPoints > 0 || couponId > 0)
            {
                sql.Append("; END ELSE BEGIN RAISERROR(N'\u4E0B\u8F7D\u652F\u4ED8\u72B6\u6001\u5DF2\u53D8\u66F4',16,1); END;");
            }
        }

        private void AppendMeta(StringBuilder sb, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }
            if (sb.Length > 0)
            {
                sb.Append(" | ");
            }
            sb.Append(Server.HtmlEncode(value));
        }

        private string Safe(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : Server.HtmlEncode(value);
        }

        private string FormatPublishDate(Literature literature)
        {
            if (literature == null || !literature.publish_year.HasValue || literature.publish_year.Value <= 0)
            {
                return string.Empty;
            }
            if (!literature.publish_month.HasValue || literature.publish_month.Value <= 0)
            {
                return literature.publish_year.Value.ToString();
            }
            if (!literature.publish_day.HasValue || literature.publish_day.Value <= 0)
            {
                return literature.publish_year.Value.ToString("0000") + "-" + literature.publish_month.Value.ToString("00");
            }
            return literature.publish_year.Value.ToString("0000") + "-" + literature.publish_month.Value.ToString("00") + "-" + literature.publish_day.Value.ToString("00");
        }

        private string GetAuthorInstitutionHtml(int currentLiteratureId)
        {
            DataTable dt = literatureBll.GetDatatable(@"
select
    coalesce(nullif(a.name_cn,N''), nullif(a.name_en,N'')) as author_name,
    coalesce(
        nullif(
            stuff((
                select N'；' + coalesce(nullif(i.name_cn,N''), nullif(i.name_en,N''), nullif(aim.affiliation_text,N''))
                from LiteratureAuthorInstitutionMap aim
                left join Institution i on i.id=aim.institution_id and i.status<>-1
                where aim.literature_author_map_id=m.id
                   or (isnull(aim.literature_author_map_id,0)=0 and aim.literature_id=m.literature_id and aim.author_id=m.author_id)
                order by aim.institution_order, aim.id
                for xml path(''), type
            ).value('.','nvarchar(max)'),1,1,N''),
            N''
        ),
        nullif(m.affiliation_text,N''),
        N''
    ) as institution_names
from LiteratureAuthorMap m
inner join Author a on a.id=m.author_id
where m.literature_id=" + currentLiteratureId + @"
order by m.author_order,m.id");
            if (dt == null || dt.Rows.Count == 0)
            {
                return "\u6682\u65E0";
            }

            StringBuilder html = new StringBuilder();
            html.Append("<div class=\"lit-author-institution-list\">");
            foreach (DataRow row in dt.Rows)
            {
                string author = Function.HtmlDiscode(Convert.ToString(row["author_name"]));
                string institutions = Function.HtmlDiscode(Convert.ToString(row["institution_names"]));
                if (string.IsNullOrWhiteSpace(author))
                {
                    continue;
                }
                html.Append("<div><strong>");
                html.Append(Server.HtmlEncode(author));
                html.Append("</strong><span>");
                html.Append(Server.HtmlEncode(string.IsNullOrWhiteSpace(institutions) ? "\u672A\u5339\u914D\u673A\u6784" : institutions));
                html.Append("</span></div>");
            }
            html.Append("</div>");
            dt.Dispose();
            return html.ToString();
        }

        private string GetCitationModalHtml(Literature literature, string authorNames)
        {
            string missingFieldsText = GetMissingCitationFieldsText(literature, authorNames);
            List<KeyValuePair<string, string>> citeItems = BuildCitationItems(literature, authorNames);
            string bibtex = BuildBibTex(literature, authorNames);

            StringBuilder html = new StringBuilder();
            html.Append("<div class=\"lit-modal-mask\" id=\"literatureCitationModal\"><div class=\"lit-modal lit-citation-modal\">");
            html.Append("<div class=\"lit-modal-head\"><h3>\u5F15\u7528</h3><button type=\"button\" class=\"lit-modal-close\" onclick=\"closeLiteratureCitation()\">×</button></div>");
            html.Append("<div class=\"lit-modal-body lit-citation-body\">");
            html.Append("<div class=\"lit-citation-tabs\"><button type=\"button\" class=\"active\" data-tab=\"Cite\" onclick=\"switchLiteratureCitationTab('Cite')\">CITE</button><button type=\"button\" data-tab=\"Bib\" onclick=\"switchLiteratureCitationTab('Bib')\">BIB</button></div>");
            html.Append("<div class=\"lit-citation-panel active\" id=\"litCitationPanelCite\">");
            html.Append("<p class=\"lit-citation-note\">");
            if (string.IsNullOrWhiteSpace(missingFieldsText))
            {
                html.Append("\u4EE5\u4E0B\u5F15\u7528\u7531\u5E73\u53F0\u6839\u636E\u6587\u732E\u5143\u6570\u636E\u81EA\u52A8\u751F\u6210\uFF0C\u590D\u5236\u524D\u5EFA\u8BAE\u6309\u539F\u6587\u6216 DOI \u9875\u9762\u518D\u6838\u5BF9\u4E00\u6B21\u3002");
            }
            else
            {
                html.Append("\u5F53\u524D\u5143\u6570\u636E\u4E0D\u5B8C\u6574\uFF0C\u5F15\u7528\u53EF\u80FD\u4E0D\u51C6\u786E\u3002\u5EFA\u8BAE\u5148\u8865\u5168\uFF1A");
                html.Append(Server.HtmlEncode(missingFieldsText));
                html.Append("\u3002");
            }
            html.Append("</p>");
            int index = 0;
            foreach (KeyValuePair<string, string> item in citeItems)
            {
                string citeId = "litCitationText" + index;
                html.Append("<div class=\"lit-citation-row\"><div class=\"lit-citation-label\">");
                html.Append(Server.HtmlEncode(item.Key));
                html.Append("</div><div class=\"lit-citation-card\"><div id=\"");
                html.Append(citeId);
                html.Append("\">");
                html.Append(Server.HtmlEncode(item.Value));
                html.Append("</div><button type=\"button\" class=\"lit-citation-copy\" onclick=\"copyLiteratureCitation('");
                html.Append(citeId);
                html.Append("')\">\u590D\u5236</button></div></div>");
                index++;
            }
            html.Append("</div>");
            html.Append("<div class=\"lit-citation-panel\" id=\"litCitationPanelBib\"><div class=\"lit-citation-row\"><div class=\"lit-citation-label\">BibTeX</div><div class=\"lit-citation-card\"><div class=\"lit-citation-pre\" id=\"litCitationBib\">");
            html.Append(Server.HtmlEncode(bibtex));
            html.Append("</div><button type=\"button\" class=\"lit-citation-copy\" onclick=\"copyLiteratureCitation('litCitationBib')\">\u590D\u5236</button></div></div></div>");
            html.Append("</div></div></div>");
            return html.ToString();
        }

        private List<KeyValuePair<string, string>> BuildCitationItems(Literature literature, string authorNames)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();
            string titleText = CleanCitationText(literature.title);
            string source = GetCitationSource(literature);
            string missingCore = GetMissingCoreCitationFieldsText(literature, authorNames);
            if (!string.IsNullOrWhiteSpace(missingCore))
            {
                string message = "\u5143\u6570\u636E\u4E0D\u8DB3\uFF0C\u65E0\u6CD5\u751F\u6210\u53EF\u9760\u5F15\u7528\u3002\u8BF7\u5148\u8865\u5168\uFF1A" + missingCore + "\u3002";
                items.Add(new KeyValuePair<string, string>("MLA", message));
                items.Add(new KeyValuePair<string, string>("APA", message));
                items.Add(new KeyValuePair<string, string>("Chicago", message));
                items.Add(new KeyValuePair<string, string>("GB/T 7714", message));
                return items;
            }

            string year = literature.publish_year.HasValue ? literature.publish_year.Value.ToString() : "n.d.";
            string authorMla = FormatAuthors(authorNames, "mla");
            string authorApa = FormatAuthors(authorNames, "apa");
            string authorChicago = FormatAuthors(authorNames, "chicago");
            string authorGbt = FormatAuthors(authorNames, "gbt");
            string volumeIssue = FormatVolumeIssue(literature);
            string pagesText = CleanCitationText(literature.pages);
            string doiText = CleanCitationText(literature.doi);
            string pagePart = string.IsNullOrWhiteSpace(pagesText) ? string.Empty : ", " + pagesText;
            string doiPart = string.IsNullOrWhiteSpace(doiText) ? string.Empty : ". https://doi.org/" + doiText.Replace("https://doi.org/", string.Empty).Replace("http://dx.doi.org/", string.Empty);

            items.Add(new KeyValuePair<string, string>("MLA", JoinCitationParts(new[]
            {
                EndWithPeriod(authorMla),
                "\"" + titleText + ".\"",
                EndWithPeriod(JoinCitationParts(new[] { source, volumeIssue }, " ")),
                year + pagePart + doiPart + "."
            }, " ")));

            items.Add(new KeyValuePair<string, string>("APA", JoinCitationParts(new[]
            {
                authorApa,
                "(" + year + ").",
                titleText + ".",
                string.IsNullOrWhiteSpace(source) ? string.Empty : source + (string.IsNullOrWhiteSpace(volumeIssue) ? string.Empty : ", " + volumeIssue) + pagePart + doiPart + "."
            }, " ")));

            items.Add(new KeyValuePair<string, string>("Chicago", JoinCitationParts(new[]
            {
                EndWithPeriod(authorChicago),
                "\"" + titleText + ".\"",
                source + (string.IsNullOrWhiteSpace(volumeIssue) ? string.Empty : " " + volumeIssue) + " (" + year + ")" + pagePart + doiPart + "."
            }, " ")));

            items.Add(new KeyValuePair<string, string>("GB/T 7714", JoinCitationParts(new[]
            {
                authorGbt + ".",
                titleText + "[J].",
                source + (string.IsNullOrWhiteSpace(year) ? string.Empty : ", " + year) + (string.IsNullOrWhiteSpace(volumeIssue) ? string.Empty : ", " + volumeIssue) + pagePart + "."
            }, " ")));

            return items;
        }

        private string BuildBibTex(Literature literature, string authorNames)
        {
            string sourceType = CleanCitationText(literature.source_type);
            string entryType = sourceType.Contains("\u4F1A\u8BAE") ? "inproceedings" : "article";
            string key = BuildBibTexKey(literature, authorNames);
            StringBuilder bib = new StringBuilder();
            bib.Append("@");
            bib.Append(entryType);
            bib.Append("{");
            bib.Append(key);
            AppendBibTexField(bib, "title", CleanCitationText(literature.title), true);
            AppendBibTexField(bib, "author", FormatAuthors(authorNames, "bibtex"), true);
            AppendBibTexField(bib, entryType == "inproceedings" ? "booktitle" : "journal", GetCitationSource(literature), true);
            AppendBibTexField(bib, "year", literature.publish_year.HasValue ? literature.publish_year.Value.ToString() : string.Empty, true);
            AppendBibTexField(bib, "volume", CleanCitationText(literature.volume), true);
            AppendBibTexField(bib, "number", CleanCitationText(literature.issue), true);
            AppendBibTexField(bib, "pages", CleanCitationText(literature.pages), true);
            AppendBibTexField(bib, "doi", CleanCitationText(literature.doi), false);
            bib.Append("\n}");
            return bib.ToString();
        }

        private string GetMissingCitationFieldsText(Literature literature, string authorNames)
        {
            List<string> fields = GetMissingCitationFields(literature, authorNames, false);
            return string.Join("\u3001", fields.ToArray());
        }

        private string GetMissingCoreCitationFieldsText(Literature literature, string authorNames)
        {
            List<string> fields = GetMissingCitationFields(literature, authorNames, true);
            return string.Join("\u3001", fields.ToArray());
        }

        private List<string> GetMissingCitationFields(Literature literature, string authorNames, bool coreOnly)
        {
            List<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(CleanCitationText(literature.title)))
            {
                fields.Add("\u6807\u9898");
            }
            if (SplitCitationAuthors(authorNames).Count == 0)
            {
                fields.Add("\u4F5C\u8005");
            }
            if (string.IsNullOrWhiteSpace(GetCitationSource(literature)))
            {
                fields.Add("\u671F\u520A/\u4F1A\u8BAE/\u51FA\u7248\u6E90");
            }
            if (!literature.publish_year.HasValue)
            {
                fields.Add("\u53D1\u8868\u5E74\u4EFD");
            }
            if (coreOnly)
            {
                return fields;
            }
            if (string.IsNullOrWhiteSpace(CleanCitationText(literature.volume)))
            {
                fields.Add("\u5377");
            }
            if (string.IsNullOrWhiteSpace(CleanCitationText(literature.issue)))
            {
                fields.Add("\u671F");
            }
            if (string.IsNullOrWhiteSpace(CleanCitationText(literature.pages)))
            {
                fields.Add("\u9875\u7801");
            }
            if (string.IsNullOrWhiteSpace(CleanCitationText(literature.doi)))
            {
                fields.Add("DOI");
            }
            return fields;
        }

        private void AppendBibTexField(StringBuilder bib, string field, string value, bool comma)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }
            bib.Append("\n  ");
            bib.Append(field);
            bib.Append(" = {");
            bib.Append(value.Replace("{", "\\{").Replace("}", "\\}"));
            bib.Append("}");
            if (comma)
            {
                bib.Append(",");
            }
        }

        private string BuildBibTexKey(Literature literature, string authorNames)
        {
            List<string> authors = SplitCitationAuthors(authorNames);
            string first = authors.Count > 0 ? authors[0] : "literature";
            string year = literature.publish_year.HasValue ? literature.publish_year.Value.ToString() : "nd";
            string titleText = CleanCitationText(literature.title);
            string keySource = first + year + titleText;
            string key = Regex.Replace(keySource, @"[^A-Za-z0-9]+", "");
            if (key.Length > 40)
            {
                key = key.Substring(0, 40);
            }
            return string.IsNullOrWhiteSpace(key) ? "literature" + literature.id : key;
        }

        private string GetCitationSource(Literature literature)
        {
            string journal = CleanCitationText(literature.journal_name);
            if (!string.IsNullOrWhiteSpace(journal))
            {
                return journal;
            }
            string conference = CleanCitationText(literature.conference_name);
            if (!string.IsNullOrWhiteSpace(conference))
            {
                return conference;
            }
            return CleanCitationText(literature.publisher);
        }

        private string FormatVolumeIssue(Literature literature)
        {
            string volume = CleanCitationText(literature.volume);
            string issue = CleanCitationText(literature.issue);
            if (!string.IsNullOrWhiteSpace(volume) && !string.IsNullOrWhiteSpace(issue))
            {
                return volume + "(" + issue + ")";
            }
            return !string.IsNullOrWhiteSpace(volume) ? volume : issue;
        }

        private string FormatAuthors(string authorNames, string style)
        {
            List<string> authors = SplitCitationAuthors(authorNames);
            if (authors.Count == 0)
            {
                return "\u4F5C\u8005\u672A\u77E5";
            }
            if (style == "bibtex")
            {
                return string.Join(" and ", authors.ToArray());
            }
            if (style == "gbt")
            {
                return string.Join(", ", authors.ToArray());
            }
            if (authors.Count == 1)
            {
                return authors[0];
            }
            if (authors.Count == 2)
            {
                return authors[0] + (style == "apa" ? " & " : ", and ") + authors[1];
            }
            return authors[0] + " et al.";
        }

        private List<string> SplitCitationAuthors(string authorNames)
        {
            List<string> authors = new List<string>();
            string decoded = Function.HtmlDiscode(authorNames ?? string.Empty);
            string[] parts = decoded.Split(new[] { ',', '\uFF0C', ';', '\uFF1B', '|', '\u3001' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                string clean = Regex.Replace(part.Trim(), @"\s+", " ");
                if (!string.IsNullOrWhiteSpace(clean))
                {
                    authors.Add(clean);
                }
            }
            return authors;
        }

        private string CleanCitationText(string value)
        {
            string text = Function.HtmlDiscode(value ?? string.Empty);
            text = Regex.Replace(text, "<.*?>", string.Empty);
            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        private string JoinCitationParts(string[] parts, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string part in parts)
            {
                if (string.IsNullOrWhiteSpace(part))
                {
                    continue;
                }
                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }
                sb.Append(part.Trim());
            }
            return sb.ToString();
        }

        private string EndWithPeriod(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }
            value = value.Trim();
            return value.EndsWith(".") ? value : value + ".";
        }
        private string GetCommentSectionHtml(int currentLiteratureId, user_list currentUser)
        {
            int currentUserId = currentUser != null ? currentUser.id : 0;
            string sql = @"
select top 50 id,content as comment_text,addtime,userid
from LiteratureComment
where parent_id=0
  and is_deleted=0
  and status=1
  and (canonical_literature_id=" + currentLiteratureId + @" or literature_id=" + currentLiteratureId + @")
order by updatetime desc,addtime desc,id desc";
            DataTable commentDt = literatureCommentBll.GetDatatable(sql);

            StringBuilder html = new StringBuilder();
            int count = commentDt != null ? commentDt.Rows.Count : 0;
            html.Append("<section class=\"lit-comments-section\" id=\"literatureComments\">");
            html.Append("<div class=\"lit-comments-head\"><div><span class=\"lit-comments-kicker\">DISCUSSION</span><h3>评论区</h3></div><span class=\"lit-comments-count\">");
            html.Append("<em id=\"litCommentCount\">");
            html.Append(count);
            html.Append("</em> 条公开评论</span></div>");

            if (count == 0)
            {
                html.Append("<div class=\"lit-comments-empty\"><strong>暂无公开评论</strong><p>用户提交的评论会先进入后台处理，管理员审核或回复后将在这里展示。</p></div>");
            }
            else
            {
                html.Append("<div class=\"lit-comment-list\">");
                foreach (DataRow row in commentDt.Rows)
                {
                    int userId = Function.ConvertTo<int>(Convert.ToString(row["userid"]), 0);
                    int commentId = Function.ConvertTo<int>(Convert.ToString(row["id"]), 0);
                    user_list commentUser = userBll.SelectSingle("id=" + userId);
                    string userName = GetDisplayUserName(commentUser, userId);
                    string avatar = commentUser != null && commentUser.id > 0
                        ? CommonUserFunc.GetUserAvatarFunc(commentUser.upload_pic_avatar)
                        : "/images/touxiang1.png";
                    string commentText = Function.HtmlDiscode(Convert.ToString(row["comment_text"]));
                    DateTime addtime = Function.ConvertTo<DateTime>(Convert.ToString(row["addtime"]), DateTime.MinValue);

                    html.Append("<article class=\"lit-comment-item\" data-comment-id=\"");
                    html.Append(commentId);
                    html.Append("\">");
                    html.Append("<div class=\"lit-comment-main\"><img class=\"lit-comment-avatar\" src=\"");
                    html.Append(Server.HtmlEncode(avatar));
                    html.Append("\" alt=\"\" /><div class=\"lit-comment-body\"><div class=\"lit-comment-author-row\"><div class=\"lit-comment-author\">");
                    html.Append(Server.HtmlEncode(userName));
                    html.Append("</div>");
                    if (currentUserId > 0 && currentUserId == userId)
                    {
                        html.Append("<button type=\"button\" class=\"lit-comment-delete\" onclick=\"deleteLiteratureComment(");
                        html.Append(commentId);
                        html.Append(")\">删除</button>");
                    }
                    html.Append("</div><div class=\"lit-comment-time\">");
                    html.Append(addtime == DateTime.MinValue ? string.Empty : addtime.ToString("yyyy-MM-dd HH:mm"));
                    html.Append("</div><div class=\"lit-comment-text\">");
                    html.Append(FormatPublicText(commentText));
                    html.Append("</div>");
                    html.Append(GetLiteratureCommentReplyHtml(commentId));
                    html.Append("</div></div></article>");
                }
                html.Append("</div>");
            }

            html.Append("</section>");
            if (commentDt != null)
            {
                commentDt.Dispose();
            }
            return html.ToString();
        }
        private string GetLiteratureCommentReplyHtml(int parentCommentId)
        {
            DataTable replyDt = literatureCommentBll.GetDatatable(
                "select content,addtime,userid from LiteratureComment where parent_id=" + parentCommentId + " and is_deleted=0 and status=1 order by addtime asc,id asc");
            if (replyDt == null || replyDt.Rows.Count == 0)
            {
                if (replyDt != null)
                {
                    replyDt.Dispose();
                }
                return string.Empty;
            }

            StringBuilder html = new StringBuilder();
            html.Append("<div class=\"lit-comment-replies\">");
            foreach (DataRow reply in replyDt.Rows)
            {
                int userId = Function.ConvertTo<int>(Convert.ToString(reply["userid"]), 0);
                user_list replyUser = userId > 0 ? userBll.SelectSingle("id=" + userId) : null;
                string displayName = userId > 0 ? GetDisplayUserName(replyUser, userId) : "管理员";
                DateTime addtime = Function.ConvertTo<DateTime>(Convert.ToString(reply["addtime"]), DateTime.MinValue);
                html.Append("<div class=\"lit-comment-reply\"><div class=\"lit-comment-reply-head\"><strong>");
                html.Append(Server.HtmlEncode(displayName));
                html.Append("</strong><span>");
                html.Append(addtime == DateTime.MinValue ? string.Empty : addtime.ToString("yyyy-MM-dd HH:mm"));
                html.Append("</span></div><div class=\"lit-comment-reply-text\">");
                html.Append(FormatPublicText(Function.HtmlDiscode(Convert.ToString(reply["content"]))));
                html.Append("</div></div>");
            }
            html.Append("</div>");
            replyDt.Dispose();
            return html.ToString();
        }
        private string GetDisplayUserName(user_list user, int userId)
        {
            if (user != null && user.id > 0)
            {
                string name = Function.HtmlDiscode(user.name);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }
                string tel = Function.HtmlDiscode(user.tel);
                if (!string.IsNullOrWhiteSpace(tel))
                {
                    return tel.Length > 4 ? "用户 " + tel.Substring(tel.Length - 4) : "用户 " + tel;
                }
            }
            return userId > 0 ? "用户 " + userId : "匿名用户";
        }
        private string FormatPublicText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "暂无内容";
            }
            return Server.HtmlEncode(text).Replace("\r\n", "\n").Replace("\n", "<br />");
        }

        private string GetPrimaryPdfFile(int literatureId)
        {
            string sql = "select top 1 file_path from LiteratureFile where literature_id=" + literatureId + " and status=1 order by orderid asc,id asc";
            return GetMappedValue(sql, "file_path", string.Empty);
        }

        private string GetMappedValue(string sql, string field, string fallback)
        {
            System.Data.DataTable dt = literatureBll.GetDatatable(sql);
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][field] != DBNull.Value)
            {
                string value = Function.HtmlDiscode(dt.Rows[0][field].ToString());
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
            return fallback;
        }
    }
}

