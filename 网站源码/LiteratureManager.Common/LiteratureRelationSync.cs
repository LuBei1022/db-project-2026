using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
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
            if (literature == null || literature.id <= 0)
            {
                return;
            }

            SyncAuthors(literature, authorNames);
            SyncTags(literature.id, tagNames);
            SyncFiles(literature.id, filePath, fileName);
        }

        public static void SyncMetadata(Literature literature, string authorNames, string tagNames)
        {
            if (literature == null || literature.id <= 0)
            {
                return;
            }

            SyncAuthors(literature, authorNames);
            SyncTags(literature.id, tagNames);
        }

        public static string GetAuthorNames(int literatureId)
        {
            string sql = "select string_agg(a.name_cn,N'\uFF0C') within group (order by m.author_order) as names from LiteratureAuthorMap m inner join Author a on a.id=m.author_id where m.literature_id=" + literatureId;
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

        private static void SyncAuthors(Literature literature, string authorNames)
        {
            BLLBase<LiteratureAuthorMap> mapBll = new BLLBase<LiteratureAuthorMap>();
            BLLBase<Author> authorBll = new BLLBase<Author>();
            mapBll.Delete("literature_id=" + literature.id);

            List<string> authors = SplitNames(authorNames);
            for (int i = 0; i < authors.Count; i++)
            {
                string encodedName = EncodeForSql(authors[i]);
                Author author = authorBll.SelectSingle("name_cn='" + encodedName + "'");
                if (author == null || author.id <= 0)
                {
                    author = new Author
                    {
                        name_cn = EncodeForColumn(authors[i], 100),
                        name_en = string.Empty,
                        institution = EncodeForColumn(literature.institution, 300),
                        orcid = string.Empty,
                        email = string.Empty,
                        status = 1,
                        addtime = DateTime.Now
                    };
                    author.id = ToInt(authorBll.AddIdentity(author, "id"));
                }

                if (author.id <= 0)
                {
                    continue;
                }

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
            return Convert.ToInt32(value);
        }

        private static string NormalizePlainText(string value)
        {
            string text = (value ?? string.Empty)
                .Replace('\u00A0', ' ')
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ");
            return Regex.Replace(text, @"\s+", " ").Trim();
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
    }
}
