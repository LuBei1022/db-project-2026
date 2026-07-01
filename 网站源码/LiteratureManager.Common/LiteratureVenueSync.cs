using BLL;
using Model;
using System;
using System.Text.RegularExpressions;

namespace LiteratureManager.Common
{
    public static class LiteratureVenueSync
    {
        private static readonly BLLBase<Literature> LiteratureBll = new BLLBase<Literature>();
        private static readonly BLLBase<Journal> JournalBll = new BLLBase<Journal>();
        private static readonly BLLBase<Conference> ConferenceBll = new BLLBase<Conference>();
        private static readonly BLLBase<ServiceLog_List> ServiceLogBll = new BLLBase<ServiceLog_List>();

        public static void EnsureForLiterature(Literature literature)
        {
            if (literature == null || literature.id <= 0)
            {
                return;
            }

            int journalId = EnsureJournal(literature);
            int conferenceId = EnsureConference(literature);
            if (journalId <= 0 && conferenceId <= 0)
            {
                return;
            }

            string setSql = string.Empty;
            if (journalId > 0 && (!literature.journal_id.HasValue || literature.journal_id.Value != journalId))
            {
                literature.journal_id = journalId;
                setSql = AppendSet(setSql, "journal_id=" + journalId);
            }
            if (conferenceId > 0 && (!literature.conference_id.HasValue || literature.conference_id.Value != conferenceId))
            {
                literature.conference_id = conferenceId;
                setSql = AppendSet(setSql, "conference_id=" + conferenceId);
            }
            if (!string.IsNullOrWhiteSpace(setSql))
            {
                LiteratureBll.Update(setSql + ",updatetime=GETDATE()", "id=" + literature.id);
            }
        }

        private static int EnsureJournal(Literature literature)
        {
            string journalName = NormalizeName(Function.HtmlDiscode(literature.journal_name));
            if (string.IsNullOrWhiteSpace(journalName) && !LooksLikeJournal(literature.source_type))
            {
                return 0;
            }
            if (string.IsNullOrWhiteSpace(journalName))
            {
                return 0;
            }

            string encodedName = Function.HtmlEncode(journalName);
            string normalized = NormalizeKey(journalName);
            Journal existing = JournalBll.SelectSingle("status<>-1 and (normalized_name=N'" + SqlLiteral(normalized) + "' or name_cn=N'" + SqlLiteral(encodedName) + "' or name_en=N'" + SqlLiteral(encodedName) + "')");
            if (existing != null && existing.id > 0)
            {
                return existing.id;
            }

            Journal journal = new Journal
            {
                name_cn = encodedName,
                name_en = string.Empty,
                normalized_name = normalized,
                issn = string.Empty,
                eissn = string.Empty,
                publisher = Function.HtmlEncode(Function.HtmlDiscode(literature.publisher)),
                country = string.Empty,
                subject = string.Empty,
                website = string.Empty,
                status = 1,
                addtime = DateTime.Now,
                updatetime = DateTime.Now
            };
            int id = Function.ConvertTo<int>(Convert.ToString(JournalBll.AddIdentity(journal, "id")), 0);
            if (id > 0)
            {
                CreateMaintenanceTicket("journal", journalName, literature);
            }
            return id;
        }

        private static int EnsureConference(Literature literature)
        {
            string conferenceName = NormalizeName(Function.HtmlDiscode(literature.conference_name));
            if (string.IsNullOrWhiteSpace(conferenceName) && !LooksLikeConference(literature.source_type))
            {
                return 0;
            }
            if (string.IsNullOrWhiteSpace(conferenceName))
            {
                return 0;
            }

            string encodedName = Function.HtmlEncode(conferenceName);
            string normalized = NormalizeKey(conferenceName);
            Conference existing = ConferenceBll.SelectSingle("status<>-1 and (normalized_name=N'" + SqlLiteral(normalized) + "' or name_cn=N'" + SqlLiteral(encodedName) + "' or name_en=N'" + SqlLiteral(encodedName) + "' or acronym=N'" + SqlLiteral(encodedName) + "')");
            if (existing != null && existing.id > 0)
            {
                return existing.id;
            }

            Conference conference = new Conference
            {
                name_cn = encodedName,
                name_en = string.Empty,
                acronym = string.Empty,
                normalized_name = normalized,
                organizer = Function.HtmlEncode(Function.HtmlDiscode(literature.publisher)),
                country = string.Empty,
                city = string.Empty,
                start_date = null,
                end_date = null,
                website = string.Empty,
                status = 1,
                addtime = DateTime.Now,
                updatetime = DateTime.Now
            };
            int id = Function.ConvertTo<int>(Convert.ToString(ConferenceBll.AddIdentity(conference, "id")), 0);
            if (id > 0)
            {
                CreateMaintenanceTicket("conference", conferenceName, literature);
            }
            return id;
        }

        private static void CreateMaintenanceTicket(string venueType, string venueName, Literature literature)
        {
            string typeText = venueType == "journal" ? "期刊" : "会议";
            string title = "[期刊/会议维护] " + typeText + ": " + venueName;
            string existsWhere = "status<>-1 and name=N'" + SqlLiteral(Function.HtmlEncode(title)) + "'";
            if (ServiceLogBll.Exists(existsWhere))
            {
                return;
            }

            ServiceLog_List ticket = new ServiceLog_List();
            ticket.name = Function.HtmlEncode(title);
            ticket.info_ = Function.HtmlEncode("系统检测到文献《" + Function.HtmlDiscode(literature.title) + "》包含新的" + typeText + ": " + venueName + "。请管理员在 Journal 或 Conference 主数据中补充 ISSN、官网、主办方、国家等信息。");
            ticket.addtime = DateTime.Now;
            ticket.uptime = DateTime.Now;
            ticket.status = 0;
            ticket.userid = 0;
            ticket.looktime = string.Empty;
            ServiceLogBll.Add(ticket, "id");
        }

        private static bool LooksLikeJournal(string sourceType)
        {
            string text = Function.HtmlDiscode(sourceType ?? string.Empty).ToLowerInvariant();
            return text.Contains("journal") || text.Contains("期刊");
        }

        private static bool LooksLikeConference(string sourceType)
        {
            string text = Function.HtmlDiscode(sourceType ?? string.Empty).ToLowerInvariant();
            return text.Contains("conference") || text.Contains("会议") || text.Contains("proceeding");
        }

        private static string NormalizeName(string value)
        {
            string text = Function.HtmlDiscode(value ?? string.Empty)
                .Replace('\u00A0', ' ')
                .Replace('\u2002', ' ')
                .Replace('\u2003', ' ');
            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        private static string NormalizeKey(string value)
        {
            return NormalizeName(value).ToLowerInvariant();
        }

        private static string AppendSet(string current, string next)
        {
            return string.IsNullOrWhiteSpace(current) ? next : current + "," + next;
        }

        private static string SqlLiteral(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }
    }
}
