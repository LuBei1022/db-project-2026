using BLL;
using DAL;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace LiteratureManager.Common
{
    public static class LiteratureRelationSync
    {
        private static readonly char[] Separators = new[] { ',', '\uFF0C', ';', '\uFF1B', '|', '\u3001' };

        public static string EncodeForColumn(string value, int maxLength)
        {
            string decoded = NormalizePlainText(Function.HtmlDiscode(value ?? string.Empty));
            string encoded = Function.HtmlEncode(decoded);
            if (maxLength > 0 && encoded.Length > maxLength)
            {
                encoded = encoded.Substring(0, maxLength);
            }
            return encoded;
        }

        public static void Sync(Literature literature, string authorNames, string tagNames, string filePath, string fileName)
        {
            Sync(literature, authorNames, tagNames, filePath, fileName, string.Empty);
        }

        public static void Sync(Literature literature, string authorNames, string tagNames, string filePath, string fileName, string authorDetailsJson)
        {
            if (literature == null || literature.id <= 0)
            {
                return;
            }

            SyncAuthors(literature, authorNames, authorDetailsJson);
            SyncTags(literature.id, tagNames);
            SyncFiles(literature.id, filePath, fileName);
        }

        public static void SyncMetadata(Literature literature, string authorNames, string tagNames)
        {
            SyncMetadata(literature, authorNames, tagNames, string.Empty);
        }

        public static void SyncMetadata(Literature literature, string authorNames, string tagNames, string authorDetailsJson)
        {
            if (literature == null || literature.id <= 0)
            {
                return;
            }

            SyncAuthors(literature, authorNames, authorDetailsJson);
            SyncTags(literature.id, tagNames);
        }

        public static string GetAuthorNames(int literatureId)
        {
            string sql = "select string_agg(coalesce(nullif(a.name_cn,N''),nullif(a.name_en,N'')),N'\uFF0C') within group (order by m.author_order) as names from LiteratureAuthorMap m inner join Author a on a.id=m.author_id where m.literature_id=" + literatureId;
            return GetMappedValue(sql, "names");
        }

        public static string GetTagNames(int literatureId)
        {
            string sql = "select string_agg(t.name,N'\uFF0C') as names from LiteratureTagMap m inner join LiteratureTag t on t.id=m.tag_id where m.literature_id=" + literatureId + " and t.status<>-1";
            return GetMappedValue(sql, "names");
        }

        public static LiteratureFile GetPrimaryFile(int literatureId)
        {
            BLLBase<LiteratureFile> fileBll = new BLLBase<LiteratureFile>();
            DataTable dt = fileBll.GetDatatable("select top 1 * from LiteratureFile where literature_id=" + literatureId + " and status=1 order by orderid asc,id asc");
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }

            DataRow row = dt.Rows[0];
            return new LiteratureFile
            {
                id = Convert.ToInt32(row["id"]),
                literature_id = Convert.ToInt32(row["literature_id"]),
                file_type = row["file_type"] == DBNull.Value ? string.Empty : row["file_type"].ToString(),
                file_name = row["file_name"] == DBNull.Value ? string.Empty : row["file_name"].ToString(),
                file_path = row["file_path"] == DBNull.Value ? string.Empty : row["file_path"].ToString(),
                file_size = row["file_size"] == DBNull.Value ? (long?)null : Convert.ToInt64(row["file_size"]),
                mime_type = row["mime_type"] == DBNull.Value ? string.Empty : row["mime_type"].ToString(),
                orderid = row["orderid"] == DBNull.Value ? 0 : Convert.ToInt32(row["orderid"]),
                status = row["status"] == DBNull.Value ? 0 : Convert.ToInt32(row["status"]),
                addtime = row["addtime"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["addtime"])
            };
        }

        private static void SyncAuthors(Literature literature, string authorNames, string authorDetailsJson)
        {
            BLLBase<LiteratureAuthorMap> mapBll = new BLLBase<LiteratureAuthorMap>();
            BLLBase<Author> authorBll = new BLLBase<Author>();
            HashSet<int> affectedAuthorIds = LoadLiteratureAuthorIds(literature.id);
            mapBll.Delete("literature_id=" + literature.id);
            DeleteMasterInstitutionMaps(literature.id);
            ArchiveLiteratureInstitutionHistory(literature.id);

            List<AuthorSyncItem> authors = BuildAuthorSyncItems(authorNames, authorDetailsJson);
            bool canSaveAffiliation = HasAuthorMapAffiliationColumns();
            for (int i = 0; i < authors.Count; i++)
            {
                AuthorSyncItem item = authors[i];
                Author author = FindAuthor(authorBll, item);
                if (author == null || author.id <= 0)
                {
                    author = new Author
                    {
                        name_cn = EncodeForColumn(item.NameCn, 100),
                        name_en = EncodeForColumn(item.NameEn, 200),
                        institution = string.Empty,
                        current_institution_id = null,
                        current_institution_name = string.Empty,
                        current_institution_literature_id = null,
                        current_institution_sort_date = null,
                        current_institution_precision = "unknown",
                        identity_status = "unconfirmed",
                        merge_group_id = null,
                        orcid = string.Empty,
                        email = string.Empty,
                        status = 1,
                        addtime = DateTime.Now,
                        updatetime = DateTime.Now
                    };
                    author.id = ToInt(authorBll.AddIdentity(author, "id"));
                }

                if (author.id <= 0)
                {
                    continue;
                }
                affectedAuthorIds.Add(author.id);

                if (canSaveAffiliation)
                {
                    int mapId = InsertAuthorMap(literature.id, author.id, i + 1, item.AffiliationText, item.RawAuthorText);
                    SyncAuthorInstitutionHistory(author.id, literature.id, mapId, i + 1, item.AffiliationText);
                }
                else
                {
                    mapBll.Add(new LiteratureAuthorMap
                    {
                        literature_id = literature.id,
                        author_id = author.id,
                        author_order = i + 1,
                        is_corresponding = 0,
                        addtime = DateTime.Now
                    }, "id");
                }
            }
            RecalculateCurrentInstitutionForAuthors(affectedAuthorIds);
        }

        private static Author FindAuthor(BLLBase<Author> authorBll, AuthorSyncItem item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Name))
            {
                return null;
            }

            if (item.AuthorId > 0)
            {
                Author selected = authorBll.SelectSingle("id=" + item.AuthorId + " and status<>-1");
                if (selected != null && selected.id > 0)
                {
                    return selected;
                }
            }

            string nameCondition = string.Empty;
            if (!string.IsNullOrWhiteSpace(item.NameCn))
            {
                nameCondition = "name_cn='" + EncodeForSql(item.NameCn) + "'";
            }
            if (!string.IsNullOrWhiteSpace(item.NameEn))
            {
                string encodedName = EncodeForSql(item.NameEn);
                if (!string.IsNullOrWhiteSpace(nameCondition))
                {
                    nameCondition += " or ";
                }
                nameCondition += "name_en='" + encodedName + "' or (isnull(name_en,'')='' and name_cn='" + encodedName + "')";
            }

            if (string.IsNullOrWhiteSpace(nameCondition))
            {
                return null;
            }

            DataTable dt = authorBll.GetDatatable("select id from Author where status<>-1 and (" + nameCondition + ") order by id asc");
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                {
                    return null;
                }

                int matchedByInstitution = FindAuthorIdByAffiliation(dt, item.AffiliationText);
                if (matchedByInstitution > 0)
                {
                    return authorBll.SelectSingle("id=" + matchedByInstitution + " and status<>-1");
                }

                if (dt.Rows.Count == 1)
                {
                    int onlyId = ToInt(dt.Rows[0]["id"]);
                    if (onlyId > 0 && (string.IsNullOrWhiteSpace(item.AffiliationText) || !AuthorHasAnyInstitution(onlyId)))
                    {
                        return authorBll.SelectSingle("id=" + onlyId + " and status<>-1");
                    }
                }

                return null;
            }
            finally
            {
                if (dt != null)
                {
                    dt.Dispose();
                }
            }
        }

        private static int InsertAuthorMap(int literatureId, int authorId, int authorOrder, string affiliationText, string rawAuthorText)
        {
            object value = DBHelper.ExecuteScalarObject(
                CommandType.Text,
                @"insert into dbo.LiteratureAuthorMap
                    (literature_id, author_id, author_order, is_corresponding, affiliation_text, raw_author_text, display_author_name, author_name_raw, is_confirmed, confirm_time, identity_confidence, addtime)
                  values
                    (@literature_id, @author_id, @author_order, @is_corresponding, @affiliation_text, @raw_author_text, @display_author_name, @author_name_raw, 1, getdate(), @identity_confidence, getdate());
                  select cast(scope_identity() as int);",
                new SqlParameter("@literature_id", SqlDbType.Int) { Value = literatureId },
                new SqlParameter("@author_id", SqlDbType.Int) { Value = authorId },
                new SqlParameter("@author_order", SqlDbType.Int) { Value = authorOrder },
                new SqlParameter("@is_corresponding", SqlDbType.Int) { Value = 0 },
                new SqlParameter("@affiliation_text", SqlDbType.NVarChar, 500) { Value = ToDbValue(EncodeForColumn(affiliationText, 500)) },
                new SqlParameter("@raw_author_text", SqlDbType.NVarChar, 300) { Value = ToDbValue(EncodeForColumn(rawAuthorText, 300)) },
                new SqlParameter("@display_author_name", SqlDbType.NVarChar, 300) { Value = ToDbValue(EncodeForColumn(rawAuthorText, 300)) },
                new SqlParameter("@author_name_raw", SqlDbType.NVarChar, 600) { Value = ToDbValue(EncodeForColumn(rawAuthorText, 600)) },
                new SqlParameter("@identity_confidence", SqlDbType.Decimal) { Precision = 5, Scale = 2, Value = string.IsNullOrWhiteSpace(affiliationText) ? 60m : 90m });
            return ToInt(value);
        }

        private static void SyncAuthorInstitutionHistory(int authorId, int literatureId, int authorMapId, int authorOrder, string affiliationText)
        {
            if (authorId <= 0 || literatureId <= 0 || string.IsNullOrWhiteSpace(affiliationText) || !HasAuthorInstitutionHistoryTable())
            {
                return;
            }

            int institutionOrder = 1;
            bool hasInstitutionColumn = HasColumn("AuthorInstitutionHistory", "institution_id");
            foreach (string rawPart in Regex.Split(NormalizePlainText(Function.HtmlDiscode(affiliationText)), @"[;；|]+"))
            {
                string institution = NormalizePlainText(rawPart);
                if (string.IsNullOrWhiteSpace(institution))
                {
                    continue;
                }

                int institutionId = EnsureInstitutionMaster(institution);

                if (hasInstitutionColumn)
                {
                    DBHelper.ExecuteNonQuery(
                        CommandType.Text,
                        @"if exists
                          (
                              select 1
                              from dbo.AuthorInstitutionHistory
                              where author_id=@author_id
                                and source_literature_id=@source_literature_id
                                and status<>-1
                                and ltrim(rtrim(institution_name))=ltrim(rtrim(@institution_name))
                          )
                          begin
                              update dbo.AuthorInstitutionHistory
                              set institution_id=case when institution_id is null and @institution_id>0 then @institution_id else institution_id end,
                                  updatetime=getdate()
                              where author_id=@author_id
                                and source_literature_id=@source_literature_id
                                and status<>-1
                                and ltrim(rtrim(institution_name))=ltrim(rtrim(@institution_name))
                          end
                          else
                          begin
                              insert into dbo.AuthorInstitutionHistory
                                  (author_id, institution_id, institution_name, is_current, source_literature_id, source_type, remark, status, addtime, updatetime)
                              values
                                  (@author_id, nullif(@institution_id,0), @institution_name, 0, @source_literature_id, N'literature', N'Linked from literature author affiliation.', 1, getdate(), getdate())
                          end",
                        new SqlParameter("@author_id", SqlDbType.Int) { Value = authorId },
                        new SqlParameter("@institution_id", SqlDbType.Int) { Value = institutionId },
                        new SqlParameter("@source_literature_id", SqlDbType.Int) { Value = literatureId },
                        new SqlParameter("@institution_name", SqlDbType.NVarChar, 500) { Value = EncodeForColumn(institution, 500) });
                }
                else
                {
                    DBHelper.ExecuteNonQuery(
                        CommandType.Text,
                        @"if not exists
                          (
                              select 1
                              from dbo.AuthorInstitutionHistory
                              where author_id=@author_id
                                and source_literature_id=@source_literature_id
                                and status<>-1
                                and ltrim(rtrim(institution_name))=ltrim(rtrim(@institution_name))
                          )
                          begin
                              insert into dbo.AuthorInstitutionHistory
                                  (author_id, institution_name, is_current, source_literature_id, source_type, remark, status, addtime, updatetime)
                              values
                                  (@author_id, @institution_name, 0, @source_literature_id, N'literature', N'Linked from literature author affiliation.', 1, getdate(), getdate())
                          end",
                        new SqlParameter("@author_id", SqlDbType.Int) { Value = authorId },
                        new SqlParameter("@source_literature_id", SqlDbType.Int) { Value = literatureId },
                        new SqlParameter("@institution_name", SqlDbType.NVarChar, 500) { Value = EncodeForColumn(institution, 500) });
                }

                InsertLiteratureAuthorInstitutionMap(literatureId, authorId, authorMapId, institutionId, institution, authorOrder, institutionOrder);
                institutionOrder++;
            }
        }

        private static void DeleteMasterInstitutionMaps(int literatureId)
        {
            if (literatureId <= 0 || !HasTable("LiteratureAuthorInstitutionMap"))
            {
                return;
            }

            DBHelper.ExecuteNonQuery(
                CommandType.Text,
                "delete from dbo.LiteratureAuthorInstitutionMap where literature_id=@literature_id",
                new SqlParameter("@literature_id", SqlDbType.Int) { Value = literatureId });
        }

        private static void ArchiveLiteratureInstitutionHistory(int literatureId)
        {
            if (literatureId <= 0 || !HasAuthorInstitutionHistoryTable())
            {
                return;
            }

            DBHelper.ExecuteNonQuery(
                CommandType.Text,
                @"update dbo.AuthorInstitutionHistory
                  set status=-1,is_current=0,updatetime=getdate()
                  where source_literature_id=@literature_id
                    and status<>-1
                    and isnull(source_type,N'') in (N'literature',N'literature_fallback')",
                new SqlParameter("@literature_id", SqlDbType.Int) { Value = literatureId });
        }

        private static void InsertLiteratureAuthorInstitutionMap(int literatureId, int authorId, int authorMapId, int institutionId, string affiliationText, int authorOrder, int institutionOrder)
        {
            if (literatureId <= 0 || authorId <= 0 || institutionId <= 0 || !HasTable("LiteratureAuthorInstitutionMap"))
            {
                return;
            }

            DBHelper.ExecuteNonQuery(
                CommandType.Text,
                @"if not exists
                  (
                      select 1
                      from dbo.LiteratureAuthorInstitutionMap
                      where literature_id=@literature_id
                        and author_id=@author_id
                        and institution_id=@institution_id
                  )
                  begin
                      insert into dbo.LiteratureAuthorInstitutionMap
                          (literature_id, author_id, literature_author_map_id, institution_id, affiliation_text, author_order, institution_order, is_current_for_author, source_type, is_confirmed, confirm_time, addtime, updatetime)
                      values
                          (@literature_id, @author_id, @literature_author_map_id, @institution_id, @affiliation_text, @author_order, @institution_order, 0, N'admin_confirmed', 1, getdate(), getdate(), getdate())
                  end",
                new SqlParameter("@literature_id", SqlDbType.Int) { Value = literatureId },
                new SqlParameter("@author_id", SqlDbType.Int) { Value = authorId },
                new SqlParameter("@literature_author_map_id", SqlDbType.Int) { Value = authorMapId },
                new SqlParameter("@institution_id", SqlDbType.Int) { Value = institutionId },
                new SqlParameter("@affiliation_text", SqlDbType.NVarChar, 500) { Value = EncodeForColumn(affiliationText, 500) },
                new SqlParameter("@author_order", SqlDbType.Int) { Value = authorOrder },
                new SqlParameter("@institution_order", SqlDbType.Int) { Value = institutionOrder });
        }

        private static HashSet<int> LoadLiteratureAuthorIds(int literatureId)
        {
            HashSet<int> ids = new HashSet<int>();
            if (literatureId <= 0 || !HasTable("LiteratureAuthorMap"))
            {
                return ids;
            }

            DataTable dt = DBHelper.GetDataTable(
                CommandType.Text,
                @"select distinct author_id from dbo.LiteratureAuthorMap where literature_id=@literature_id
                  union
                  select distinct author_id from dbo.LiteratureAuthorInstitutionMap where literature_id=@literature_id",
                new SqlParameter("@literature_id", SqlDbType.Int) { Value = literatureId });
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    int id = ToInt(row["author_id"]);
                    if (id > 0)
                    {
                        ids.Add(id);
                    }
                }
                dt.Dispose();
            }
            return ids;
        }

        private static int FindAuthorIdByAffiliation(DataTable candidates, string affiliationText)
        {
            List<string> affiliationKeys = new List<string>();
            foreach (string part in Regex.Split(NormalizePlainText(Function.HtmlDiscode(affiliationText ?? string.Empty)), @"[;；|]+"))
            {
                string key = NormalizeMasterName(part);
                if (!string.IsNullOrWhiteSpace(key) && !affiliationKeys.Contains(key))
                {
                    affiliationKeys.Add(key);
                }
            }
            if (affiliationKeys.Count == 0)
            {
                return 0;
            }

            foreach (DataRow row in candidates.Rows)
            {
                int authorId = ToInt(row["id"]);
                if (authorId <= 0)
                {
                    continue;
                }
                DataTable history = DBHelper.GetDataTable(
                    CommandType.Text,
                    @"select institution_name=current_institution_name
                      from dbo.Author
                      where id=@author_id and nullif(current_institution_name,N'') is not null
                      union
                      select coalesce(nullif(i.name_cn,N''),nullif(i.name_en,N''),nullif(aim.affiliation_text,N''))
                      from dbo.LiteratureAuthorInstitutionMap aim
                      left join dbo.Institution i on i.id=aim.institution_id and i.status<>-1
                      where aim.author_id=@author_id
                      union
                      select coalesce(nullif(i.name_cn,N''),nullif(i.name_en,N''),nullif(h.institution_name,N''))
                      from dbo.AuthorInstitutionHistory h
                      left join dbo.Institution i on i.id=h.institution_id and i.status<>-1
                      where h.author_id=@author_id and h.status<>-1",
                    new SqlParameter("@author_id", SqlDbType.Int) { Value = authorId });
                try
                {
                    if (history == null)
                    {
                        continue;
                    }
                    foreach (DataRow historyRow in history.Rows)
                    {
                        string key = NormalizeMasterName(Convert.ToString(historyRow["institution_name"]));
                        if (string.IsNullOrWhiteSpace(key))
                        {
                            continue;
                        }
                        foreach (string affiliationKey in affiliationKeys)
                        {
                            if (key == affiliationKey || key.Contains(affiliationKey) || affiliationKey.Contains(key))
                            {
                                return authorId;
                            }
                        }
                    }
                }
                finally
                {
                    if (history != null)
                    {
                        history.Dispose();
                    }
                }
            }
            return 0;
        }

        private static bool AuthorHasAnyInstitution(int authorId)
        {
            if (authorId <= 0)
            {
                return false;
            }

            int count = DBHelper.ExecuteScalar(
                CommandType.Text,
                @"select
                    (select count(1) from dbo.Author where id=@author_id and nullif(current_institution_name,N'') is not null)
                    + (select count(1) from dbo.LiteratureAuthorInstitutionMap where author_id=@author_id)
                    + (select count(1) from dbo.AuthorInstitutionHistory where author_id=@author_id and status<>-1 and nullif(institution_name,N'') is not null)",
                new SqlParameter("@author_id", SqlDbType.Int) { Value = authorId });
            return count > 0;
        }

        public static void RecalculateCurrentInstitutionForAuthors(IEnumerable<int> authorIds)
        {
            if (authorIds == null || !HasTable("Author") || !HasTable("LiteratureAuthorInstitutionMap"))
            {
                return;
            }

            HashSet<int> cleanIds = new HashSet<int>();
            foreach (int authorId in authorIds)
            {
                if (authorId > 0)
                {
                    cleanIds.Add(authorId);
                }
            }
            if (cleanIds.Count == 0)
            {
                return;
            }

            foreach (int authorId in cleanIds)
            {
                RecalculateCurrentInstitutionForAuthor(authorId);
            }
        }

        public static void RecalculateCurrentInstitutionForAuthor(int authorId)
        {
            if (authorId <= 0 || !HasTable("Author") || !HasTable("LiteratureAuthorInstitutionMap"))
            {
                return;
            }

            DataTable latest = DBHelper.GetDataTable(
                CommandType.Text,
                @"select top 1
                      aim.literature_id,
                      sort_date=coalesce(l.publish_date,case when l.publish_year between 1000 and 9999 then datefromparts(l.publish_year,12,31) end,convert(date,l.addtime)),
                      precision_text=coalesce(nullif(l.publish_date_precision,N''),case when l.publish_year is not null then N'year' else N'unknown' end)
                  from dbo.LiteratureAuthorInstitutionMap aim
                  inner join dbo.Literature l on l.id=aim.literature_id
                  where aim.author_id=@author_id and l.status<>-1
                  group by aim.literature_id,l.publish_date,l.publish_year,l.addtime,l.publish_date_precision
                  order by coalesce(l.publish_date,case when l.publish_year between 1000 and 9999 then datefromparts(l.publish_year,12,31) end,convert(date,l.addtime)) desc, aim.literature_id desc",
                new SqlParameter("@author_id", SqlDbType.Int) { Value = authorId });
            int literatureId = 0;
            DateTime? sortDate = null;
            string precision = "unknown";
            if (latest != null && latest.Rows.Count > 0)
            {
                literatureId = ToInt(latest.Rows[0]["literature_id"]);
                if (latest.Rows[0]["sort_date"] != DBNull.Value)
                {
                    sortDate = Convert.ToDateTime(latest.Rows[0]["sort_date"]);
                }
                precision = Convert.ToString(latest.Rows[0]["precision_text"]);
            }
            if (latest != null)
            {
                latest.Dispose();
            }

            if (literatureId <= 0)
            {
                DBHelper.ExecuteNonQuery(
                    CommandType.Text,
                    @"update dbo.LiteratureAuthorInstitutionMap set is_current_for_author=0,updatetime=getdate() where author_id=@author_id;
                      update dbo.AuthorInstitutionHistory set is_current=0,updatetime=getdate() where author_id=@author_id and status<>-1;
                      update dbo.Author
                      set current_institution_id=null,current_institution_name=null,current_institution_literature_id=null,current_institution_sort_date=null,current_institution_precision=N'unknown',updatetime=getdate()
                      where id=@author_id",
                    new SqlParameter("@author_id", SqlDbType.Int) { Value = authorId });
                return;
            }

            DataTable names = DBHelper.GetDataTable(
                CommandType.Text,
                @"select
                      display_name=coalesce(nullif(i.name_cn,N''),nullif(i.name_en,N''),nullif(aim.affiliation_text,N'')),
                      display_order=min(aim.institution_order),
                      first_id=min(aim.id),
                      institution_id=min(aim.institution_id)
                  from dbo.LiteratureAuthorInstitutionMap aim
                  left join dbo.Institution i on i.id=aim.institution_id and i.status<>-1
                  where aim.author_id=@author_id and aim.literature_id=@literature_id
                  group by coalesce(nullif(i.name_cn,N''),nullif(i.name_en,N''),nullif(aim.affiliation_text,N''))
                  order by display_order,first_id",
                new SqlParameter("@author_id", SqlDbType.Int) { Value = authorId },
                new SqlParameter("@literature_id", SqlDbType.Int) { Value = literatureId });
            List<string> currentNames = new List<string>();
            int currentInstitutionId = 0;
            if (names != null)
            {
                foreach (DataRow row in names.Rows)
                {
                    string displayName = NormalizePlainText(Function.HtmlDiscode(Convert.ToString(row["display_name"])));
                    if (!string.IsNullOrWhiteSpace(displayName) && !currentNames.Contains(displayName))
                    {
                        currentNames.Add(displayName);
                    }
                    if (currentInstitutionId <= 0)
                    {
                        currentInstitutionId = ToInt(row["institution_id"]);
                    }
                }
                names.Dispose();
            }

            string currentName = string.Join("；", currentNames.ToArray());
            DBHelper.ExecuteNonQuery(
                CommandType.Text,
                @"update dbo.LiteratureAuthorInstitutionMap
                  set is_current_for_author=case when literature_id=@literature_id then 1 else 0 end,
                      updatetime=getdate()
                  where author_id=@author_id;
                  update dbo.AuthorInstitutionHistory
                  set is_current=case when source_literature_id=@literature_id then 1 else 0 end,
                      updatetime=getdate()
                  where author_id=@author_id and status<>-1;
                  update dbo.Author
                  set current_institution_id=nullif(@institution_id,0),
                      current_institution_name=@institution_name,
                      current_institution_literature_id=@literature_id,
                      current_institution_sort_date=@sort_date,
                      current_institution_precision=@precision,
                      institution=@institution_name,
                      updatetime=getdate()
                  where id=@author_id",
                new SqlParameter("@author_id", SqlDbType.Int) { Value = authorId },
                new SqlParameter("@literature_id", SqlDbType.Int) { Value = literatureId },
                new SqlParameter("@institution_id", SqlDbType.Int) { Value = currentInstitutionId },
                new SqlParameter("@institution_name", SqlDbType.NVarChar, 1000) { Value = ToDbValue(EncodeForColumn(currentName, 1000)) },
                new SqlParameter("@sort_date", SqlDbType.Date) { Value = sortDate.HasValue ? (object)sortDate.Value.Date : DBNull.Value },
                new SqlParameter("@precision", SqlDbType.NVarChar, 20) { Value = string.IsNullOrWhiteSpace(precision) ? "unknown" : precision });
        }

        private static int EnsureInstitutionMaster(string institution)
        {
            string normalizedName = NormalizeMasterName(institution);
            if (string.IsNullOrWhiteSpace(normalizedName) || !HasTable("Institution"))
            {
                return 0;
            }

            object exists = DBHelper.ExecuteScalarObject(
                CommandType.Text,
                "select top 1 id from dbo.Institution where status<>-1 and normalized_name=@normalized_name order by id asc",
                new SqlParameter("@normalized_name", SqlDbType.NVarChar, 500) { Value = normalizedName });
            int existingId = ToInt(exists);
            if (existingId > 0)
            {
                return existingId;
            }

            string cleanName = NormalizePlainText(Function.HtmlDiscode(institution));
            bool isChinese = ContainsChinese(cleanName);
            int parentId = HasColumn("Institution", "parent_id") ? EnsureParentInstitutionMaster(cleanName, normalizedName) : 0;
            string insertSql = HasColumn("Institution", "parent_id")
                ? @"insert into dbo.Institution
                    (parent_id, name_cn, name_en, normalized_name, alias_names, country, province, city, website, status, addtime, updatetime)
                  values
                    (nullif(@parent_id,0), @name_cn, @name_en, @normalized_name, null, null, null, null, null, 1, getdate(), getdate());
                  select cast(scope_identity() as int);"
                : @"insert into dbo.Institution
                    (name_cn, name_en, normalized_name, alias_names, country, province, city, website, status, addtime, updatetime)
                  values
                    (@name_cn, @name_en, @normalized_name, null, null, null, null, null, 1, getdate(), getdate());
                  select cast(scope_identity() as int);";
            object newId = DBHelper.ExecuteScalarObject(
                CommandType.Text,
                insertSql,
                new SqlParameter("@parent_id", SqlDbType.Int) { Value = parentId },
                new SqlParameter("@name_cn", SqlDbType.NVarChar, 150) { Value = isChinese ? cleanName : string.Empty },
                new SqlParameter("@name_en", SqlDbType.NVarChar, 250) { Value = ToDbValue(isChinese ? string.Empty : cleanName) },
                new SqlParameter("@normalized_name", SqlDbType.NVarChar, 500) { Value = normalizedName });
            return ToInt(newId);
        }

        private static int EnsureParentInstitutionMaster(string childName, string childNormalizedName)
        {
            string parentName = ExtractParentInstitutionName(childName);
            string parentNormalized = NormalizeMasterName(parentName);
            if (string.IsNullOrWhiteSpace(parentNormalized) || string.Equals(parentNormalized, childNormalizedName, StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            object exists = DBHelper.ExecuteScalarObject(
                CommandType.Text,
                "select top 1 id from dbo.Institution where status<>-1 and normalized_name=@normalized_name order by id asc",
                new SqlParameter("@normalized_name", SqlDbType.NVarChar, 500) { Value = parentNormalized });
            int existingId = ToInt(exists);
            if (existingId > 0)
            {
                return existingId;
            }

            bool isChinese = ContainsChinese(parentName);
            object newId = DBHelper.ExecuteScalarObject(
                CommandType.Text,
                @"insert into dbo.Institution
                    (parent_id, name_cn, name_en, normalized_name, alias_names, country, province, city, website, status, addtime, updatetime)
                  values
                    (null, @name_cn, @name_en, @normalized_name, null, null, null, null, null, 1, getdate(), getdate());
                  select cast(scope_identity() as int);",
                new SqlParameter("@name_cn", SqlDbType.NVarChar, 150) { Value = isChinese ? parentName : string.Empty },
                new SqlParameter("@name_en", SqlDbType.NVarChar, 250) { Value = ToDbValue(isChinese ? string.Empty : parentName) },
                new SqlParameter("@normalized_name", SqlDbType.NVarChar, 500) { Value = parentNormalized });
            return ToInt(newId);
        }

        private static string ExtractParentInstitutionName(string institutionName)
        {
            string clean = NormalizePlainText(Function.HtmlDiscode(institutionName ?? string.Empty));
            if (string.IsNullOrWhiteSpace(clean))
            {
                return string.Empty;
            }

            string[] parts = Regex.Split(clean, @"[,，;；|]+");
            List<string> tokens = new List<string>();
            foreach (string raw in parts)
            {
                string token = NormalizePlainText(raw);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    tokens.Add(token);
                }
            }

            if (tokens.Count < 2)
            {
                return string.Empty;
            }

            int organizationParts = 0;
            foreach (string token in tokens)
            {
                if (LooksLikeInstitutionUnit(token))
                {
                    organizationParts++;
                }
            }

            if (organizationParts < 2)
            {
                return string.Empty;
            }

            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                string token = tokens[i];
                if (IsLocationOnly(token))
                {
                    continue;
                }
                if (LooksLikeParentInstitution(token))
                {
                    return token;
                }
            }

            return string.Empty;
        }

        private static bool LooksLikeInstitutionUnit(string value)
        {
            string lower = (value ?? string.Empty).ToLowerInvariant();
            return lower.Contains("university")
                || lower.Contains("institute")
                || lower.Contains("college")
                || lower.Contains("school")
                || lower.Contains("laboratory")
                || Regex.IsMatch(lower, @"\blab\b")
                || lower.Contains("department")
                || lower.Contains("center")
                || lower.Contains("centre");
        }

        private static bool LooksLikeParentInstitution(string value)
        {
            string lower = (value ?? string.Empty).ToLowerInvariant();
            return lower.Contains("university")
                || lower.Contains("institute")
                || lower.Contains("college")
                || lower.Contains("laboratory")
                || Regex.IsMatch(lower, @"\blab\b");
        }

        private static bool IsLocationOnly(string value)
        {
            string lower = NormalizePlainText(value).ToLowerInvariant();
            return lower == "china"
                || lower == "usa"
                || lower == "u.s.a."
                || lower == "united states"
                || lower == "united kingdom"
                || lower == "uk"
                || lower == "hong kong"
                || lower == "singapore"
                || lower == "canada"
                || lower == "japan";
        }

        private static void FillMissingAuthorInstitutionCandidates(int literatureId)
        {
            if (literatureId <= 0 || !HasAuthorInstitutionHistoryTable() || !HasTable("LiteratureAuthorMap"))
            {
                return;
            }

            DataTable candidates = DBHelper.GetDataTable(
                CommandType.Text,
                @"select distinct h.institution_id,
                         coalesce(nullif(i.name_cn,N''), nullif(i.name_en,N''), h.institution_name) as institution_name
                  from dbo.AuthorInstitutionHistory h
                  left join dbo.Institution i on i.id=h.institution_id and i.status<>-1
                  where h.source_literature_id=@literature_id
                    and h.status<>-1
                    and isnull(h.source_type,N'')=N'literature'
                    and ltrim(rtrim(isnull(coalesce(nullif(i.name_cn,N''), nullif(i.name_en,N''), h.institution_name),N'')))<>N''",
                new SqlParameter("@literature_id", SqlDbType.Int) { Value = literatureId });
            if (candidates == null || candidates.Rows.Count <= 0)
            {
                if (candidates != null) candidates.Dispose();
                return;
            }

            DataTable missing = DBHelper.GetDataTable(
                CommandType.Text,
                @"select m.id, m.author_id
                  from dbo.LiteratureAuthorMap m
                  where m.literature_id=@literature_id
                    and ltrim(rtrim(isnull(m.affiliation_text,N'')))=N''
                    and not exists
                    (
                        select 1
                        from dbo.AuthorInstitutionHistory h
                        where h.author_id=m.author_id
                          and h.source_literature_id=m.literature_id
                          and h.status<>-1
                    )",
                new SqlParameter("@literature_id", SqlDbType.Int) { Value = literatureId });
            if (missing == null || missing.Rows.Count <= 0)
            {
                if (missing != null) missing.Dispose();
                candidates.Dispose();
                return;
            }

            foreach (DataRow missingRow in missing.Rows)
            {
                int authorId = ToInt(missingRow["author_id"]);
                foreach (DataRow candidateRow in candidates.Rows)
                {
                    string institutionName = Convert.ToString(candidateRow["institution_name"]);
                    if (string.IsNullOrWhiteSpace(institutionName))
                    {
                        continue;
                    }

                    DBHelper.ExecuteNonQuery(
                        CommandType.Text,
                        @"if not exists
                          (
                              select 1
                              from dbo.AuthorInstitutionHistory
                              where author_id=@author_id
                                and source_literature_id=@source_literature_id
                                and status<>-1
                                and ltrim(rtrim(institution_name))=ltrim(rtrim(@institution_name))
                          )
                          begin
                              insert into dbo.AuthorInstitutionHistory
                                  (author_id, institution_id, institution_name, is_current, source_literature_id, source_type, remark, status, addtime, updatetime)
                              values
                                  (@author_id, nullif(@institution_id,0), @institution_name, 0, @source_literature_id, N'literature_fallback', N'Candidate inferred from same literature; please confirm in review/edit page.', 1, getdate(), getdate())
                          end",
                        new SqlParameter("@author_id", SqlDbType.Int) { Value = authorId },
                        new SqlParameter("@institution_id", SqlDbType.Int) { Value = ToInt(candidateRow["institution_id"]) },
                        new SqlParameter("@source_literature_id", SqlDbType.Int) { Value = literatureId },
                        new SqlParameter("@institution_name", SqlDbType.NVarChar, 500) { Value = institutionName });
                }
            }

            missing.Dispose();
            candidates.Dispose();
        }

        private static bool HasAuthorInstitutionHistoryTable()
        {
            try
            {
                int count = DBHelper.ExecuteScalar(CommandType.Text, "select count(1) from sys.objects where object_id=object_id(N'dbo.AuthorInstitutionHistory') and type=N'U'");
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        private static bool HasTable(string tableName)
        {
            try
            {
                int count = DBHelper.ExecuteScalar(
                    CommandType.Text,
                    "select count(1) from sys.objects where object_id=object_id(@table_name) and type=N'U'",
                    new SqlParameter("@table_name", SqlDbType.NVarChar, 256) { Value = "dbo." + tableName });
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        private static bool HasColumn(string tableName, string columnName)
        {
            try
            {
                int count = DBHelper.ExecuteScalar(
                    CommandType.Text,
                    "select count(1) from sys.columns where object_id=object_id(@table_name) and name=@column_name",
                    new SqlParameter("@table_name", SqlDbType.NVarChar, 256) { Value = "dbo." + tableName },
                    new SqlParameter("@column_name", SqlDbType.NVarChar, 128) { Value = columnName });
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        private static bool HasAuthorMapAffiliationColumns()
        {
            try
            {
                int count = DBHelper.ExecuteScalar(
                    CommandType.Text,
                    "select count(1) from sys.columns where object_id=object_id(N'dbo.LiteratureAuthorMap') and name in (N'affiliation_text',N'raw_author_text')");
                return count >= 2;
            }
            catch
            {
                return false;
            }
        }

        private static List<AuthorSyncItem> BuildAuthorSyncItems(string authorNames, string authorDetailsJson)
        {
            List<AuthorSyncItem> result = new List<AuthorSyncItem>();
            HashSet<string> exists = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (AuthorSyncItem item in ParseAuthorDetails(authorDetailsJson))
            {
                AddAuthorSyncItem(result, exists, item);
            }

            foreach (string name in SplitNames(authorNames))
            {
                AddAuthorSyncItem(result, exists, CreateAuthorSyncItem(name, string.Empty, string.Empty));
            }

            return result;
        }

        private static List<AuthorSyncItem> ParseAuthorDetails(string authorDetailsJson)
        {
            List<AuthorSyncItem> result = new List<AuthorSyncItem>();
            if (string.IsNullOrWhiteSpace(authorDetailsJson))
            {
                return result;
            }

            try
            {
                JToken token = JToken.Parse(authorDetailsJson);
                JArray array = token as JArray;
                if (array == null && token.Type == JTokenType.Object)
                {
                    JObject obj = (JObject)token;
                    array = obj["author_details"] as JArray ?? obj["authors"] as JArray;
                }

                if (array == null)
                {
                    return result;
                }

                foreach (JToken itemToken in array)
                {
                    AuthorSyncItem item = ParseAuthorDetail(itemToken);
                    if (item != null)
                    {
                        result.Add(item);
                    }
                }
            }
            catch
            {
            }

            return result;
        }

        private static AuthorSyncItem ParseAuthorDetail(JToken token)
        {
            if (token == null)
            {
                return null;
            }

            if (token.Type != JTokenType.Object)
            {
                return CreateAuthorSyncItem(JsonText(token), string.Empty, string.Empty);
            }

            JObject obj = (JObject)token;
            int authorId = ToInt(JsonText(obj["author_id"]));
            string name = JsonText(obj["name"]);
            string nameCn = JsonText(obj["name_cn"]);
            string nameEn = JsonText(obj["name_en"]);
            if (string.IsNullOrWhiteSpace(name))
            {
                name = !string.IsNullOrWhiteSpace(nameCn) ? nameCn : nameEn;
            }

            string affiliationText = JsonText(obj["affiliation_text"]);
            if (string.IsNullOrWhiteSpace(affiliationText))
            {
                affiliationText = JoinJsonArray(obj["affiliations"], "\uFF1B");
            }

            AuthorSyncItem item = CreateAuthorSyncItem(name, affiliationText, name);
            item.AuthorId = authorId;
            string cleanNameCn = NormalizePlainText(Function.HtmlDiscode(nameCn ?? string.Empty));
            string cleanNameEn = NormalizePlainText(Function.HtmlDiscode(nameEn ?? string.Empty));
            if (!string.IsNullOrWhiteSpace(cleanNameCn) && ContainsChinese(cleanNameCn))
            {
                item.NameCn = cleanNameCn;
                item.NameEn = string.Empty;
            }
            else if (!string.IsNullOrWhiteSpace(cleanNameEn))
            {
                item.NameCn = ContainsChinese(cleanNameEn) ? cleanNameEn : string.Empty;
                item.NameEn = ContainsChinese(cleanNameEn) ? string.Empty : cleanNameEn;
            }
            return item;
        }

        private static AuthorSyncItem CreateAuthorSyncItem(string name, string affiliationText, string rawAuthorText)
        {
            string cleanName = NormalizePlainText(Function.HtmlDiscode(name ?? string.Empty));
            if (string.IsNullOrWhiteSpace(cleanName))
            {
                return null;
            }

            AuthorSyncItem item = new AuthorSyncItem
            {
                Name = cleanName,
                NameCn = ContainsChinese(cleanName) ? cleanName : string.Empty,
                NameEn = ContainsChinese(cleanName) ? string.Empty : cleanName,
                AffiliationText = NormalizePlainText(Function.HtmlDiscode(affiliationText ?? string.Empty)),
                RawAuthorText = NormalizePlainText(Function.HtmlDiscode(string.IsNullOrWhiteSpace(rawAuthorText) ? cleanName : rawAuthorText))
            };
            return item;
        }

        private static void AddAuthorSyncItem(List<AuthorSyncItem> result, HashSet<string> exists, AuthorSyncItem item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Name))
            {
                return;
            }

            string key = item.Name.ToLowerInvariant();
            if (exists.Contains(key))
            {
                return;
            }

            exists.Add(key);
            result.Add(item);
        }

        private static string JsonText(JToken token)
        {
            return token == null ? string.Empty : NormalizePlainText(Function.HtmlDiscode(token.ToString()));
        }

        private static string JoinJsonArray(JToken token, string separator)
        {
            JArray array = token as JArray;
            if (array == null)
            {
                return JsonText(token);
            }

            List<string> values = new List<string>();
            foreach (JToken item in array)
            {
                string value = JsonText(item);
                if (!string.IsNullOrWhiteSpace(value) && !values.Contains(value))
                {
                    values.Add(value);
                }
            }
            return string.Join(separator, values.ToArray());
        }

        private static bool ContainsChinese(string value)
        {
            foreach (char ch in value ?? string.Empty)
            {
                if ((ch >= '\u4e00' && ch <= '\u9fff') ||
                    (ch >= '\u3400' && ch <= '\u4dbf') ||
                    (ch >= '\uf900' && ch <= '\ufaff'))
                {
                    return true;
                }
            }
            return false;
        }

        private static object ToDbValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value;
        }

        private static void SyncTags(int literatureId, string tagNames)
        {
            BLLBase<LiteratureTagMap> mapBll = new BLLBase<LiteratureTagMap>();
            BLLBase<LiteratureTag> tagBll = new BLLBase<LiteratureTag>();
            mapBll.Delete("literature_id=" + literatureId);

            List<string> tags = SplitNames(tagNames);
            for (int i = 0; i < tags.Count; i++)
            {
                string encodedName = EncodeForSql(tags[i]);
                LiteratureTag tag = tagBll.SelectSingle("name='" + encodedName + "' and status<>-1");
                if (tag == null || tag.id <= 0)
                {
                    tag = new LiteratureTag
                    {
                        name = Function.HtmlEncode(tags[i]),
                        orderid = 0,
                        status = 1,
                        addtime = DateTime.Now
                    };
                    tag.id = ToInt(tagBll.AddIdentity(tag, "id"));
                }

                if (tag.id <= 0)
                {
                    continue;
                }

                mapBll.Add(new LiteratureTagMap
                {
                    literature_id = literatureId,
                    tag_id = tag.id,
                    addtime = DateTime.Now
                }, "id");
            }
        }

        private static void SyncFiles(int literatureId, string filePath, string fileName)
        {
            BLLBase<LiteratureFile> fileBll = new BLLBase<LiteratureFile>();
            fileBll.Delete("literature_id=" + literatureId + " and file_type='PDF'");

            string cleanPath = Function.HtmlDiscode(filePath ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(cleanPath))
            {
                return;
            }

            string cleanName = Function.HtmlDiscode(fileName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(cleanName))
            {
                cleanName = Path.GetFileName(cleanPath);
            }

            fileBll.Add(new LiteratureFile
            {
                literature_id = literatureId,
                file_type = "PDF",
                file_name = Function.HtmlEncode(cleanName),
                file_path = Function.HtmlEncode(cleanPath),
                file_size = null,
                mime_type = "application/pdf",
                orderid = 1,
                status = 1,
                addtime = DateTime.Now
            }, "id");
        }

        private static List<string> SplitNames(string value)
        {
            string decoded = Function.HtmlDiscode(value ?? string.Empty);
            List<string> result = new List<string>();
            HashSet<string> exists = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string item in decoded.Split(Separators, StringSplitOptions.RemoveEmptyEntries))
            {
                string clean = item.Trim();
                if (clean.Length == 0 || exists.Contains(clean))
                {
                    continue;
                }

                exists.Add(clean);
                result.Add(clean);
            }
            return result;
        }

        private static string EncodeForSql(string value)
        {
            return Function.HtmlEncode(value ?? string.Empty).Replace("'", "''");
        }

        private static int ToInt(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0;
            }
            int result;
            return int.TryParse(Convert.ToString(value), out result) ? result : 0;
        }

        private static string NormalizePlainText(string value)
        {
            string text = (value ?? string.Empty)
                .Replace('\u00A0', ' ')
                .Replace('\u2002', ' ')
                .Replace('\u2003', ' ')
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ");
            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        private static string NormalizeMasterName(string value)
        {
            return NormalizePlainText(Function.HtmlDiscode(value ?? string.Empty)).ToLowerInvariant();
        }

        private static string GetMappedValue(string sql, string field)
        {
            BLLBase<Literature> bll = new BLLBase<Literature>();
            DataTable dt = bll.GetDatatable(sql);
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][field] != DBNull.Value)
            {
                string value = Function.HtmlDiscode(dt.Rows[0][field].ToString());
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
            return string.Empty;
        }

        private class AuthorSyncItem
        {
            public int AuthorId { get; set; }
            public string Name { get; set; }
            public string NameCn { get; set; }
            public string NameEn { get; set; }
            public string AffiliationText { get; set; }
            public string RawAuthorText { get; set; }
        }
    }
}
