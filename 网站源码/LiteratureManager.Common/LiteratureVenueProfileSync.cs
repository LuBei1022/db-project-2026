using BLL;
using Model;
using System;

namespace LiteratureManager.Common
{
    public static class LiteratureVenueProfileSync
    {
        private static readonly BLLBase<LiteratureVenueProfile> ProfileBll = new BLLBase<LiteratureVenueProfile>();
        private static readonly BLLBase<ServiceLog_List> ServiceLogBll = new BLLBase<ServiceLog_List>();

        public static void EnsureForLiterature(Literature literature)
        {
            if (literature == null || literature.id <= 0)
            {
                return;
            }
            EnsureVenue("journal", Function.HtmlDiscode(literature.journal_name), literature);
            EnsureVenue("conference", Function.HtmlDiscode(literature.conference_name), literature);
        }

        private static void EnsureVenue(string venueType, string venueName, Literature literature)
        {
            venueName = (venueName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(venueName))
            {
                return;
            }

            string safeType = SqlLiteral(venueType);
            string safeName = SqlLiteral(venueName);
            LiteratureVenueProfile existing = ProfileBll.SelectSingle("status<>-1 and venue_type=N'" + safeType + "' and venue_name=N'" + safeName + "'");
            if (existing != null && existing.id > 0)
            {
                return;
            }

            LiteratureVenueProfile profile = new LiteratureVenueProfile();
            profile.venue_type = venueType;
            profile.venue_name = Function.HtmlEncode(venueName);
            profile.introduction = string.Empty;
            profile.impact_factor = string.Empty;
            profile.jcr_quartile = string.Empty;
            profile.issn = string.Empty;
            profile.conference_level = string.Empty;
            profile.conference_cycle = string.Empty;
            profile.location = string.Empty;
            profile.website_url = string.Empty;
            profile.publisher = Function.HtmlEncode(Function.HtmlDiscode(literature.publisher));
            profile.remark = Function.HtmlEncode("系统检测到新" + GetTypeText(venueType) + "，待管理员维护学术信息。");
            profile.status = 0;
            profile.created_by = 0;
            profile.updated_by = 0;
            profile.addtime = DateTime.Now;
            profile.updatetime = DateTime.Now;
            int profileId = Convert.ToInt32(ProfileBll.AddIdentity(profile, "id"));
            if (profileId > 0)
            {
                CreateMaintenanceTicket(venueType, venueName, profileId, literature);
            }
        }

        private static void CreateMaintenanceTicket(string venueType, string venueName, int profileId, Literature literature)
        {
            string safeTypeText = GetTypeText(venueType);
            string safeName = Function.HtmlEncode(venueName);
            string existsWhere = "status<>-1 and name=N'" + SqlLiteral("[期刊/会议资料维护] " + safeTypeText + "：" + venueName) + "'";
            if (ServiceLogBll.Exists(existsWhere))
            {
                return;
            }

            ServiceLog_List ticket = new ServiceLog_List();
            ticket.name = Function.HtmlEncode("[期刊/会议资料维护] " + safeTypeText + "：" + venueName);
            ticket.info_ = Function.HtmlEncode(
                "系统检测到文献《" + Function.HtmlDiscode(literature.title) + "》包含新的" + safeTypeText + "：" + venueName +
                "。请管理员维护介绍、影响因子/会议等级、ISSN/分区、官网等信息。维护入口：/admin/Admin_LiteratureVenueProfile.aspx?Action=Edit&MenuId=1729&ID=" + profileId);
            ticket.addtime = DateTime.Now;
            ticket.uptime = DateTime.Now;
            ticket.status = 0;
            ticket.userid = 0;
            ticket.looktime = string.Empty;
            ServiceLogBll.Add(ticket, "id");
        }

        private static string GetTypeText(string venueType)
        {
            return venueType == "journal" ? "期刊" : "会议";
        }

        private static string SqlLiteral(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }
    }
}
