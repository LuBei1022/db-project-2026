using DAL;
using LiteratureManager.Common;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace Web.Inc
{
    /// <summary>
    /// Provides literature relationship graph data for the upload page.
    /// </summary>
    public class LiteratureGraph : IHttpHandler
    {
        private const int MaxLiteratureCount = 100;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json;charset=UTF-8";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            try
            {
                user_list user = CommonUserFunc.GetUserLoginStatus();
                int userId = user == null ? 0 : user.id;

                GraphResponse response = BuildGraph(userId);
                WriteJson(context, response);
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, "LiteratureGraph.ashx_Error:" + ex.Message + "-" + ex.StackTrace);
                WriteJson(context, new GraphResponse
                {
                    code = 500,
                    msg = "文献图谱数据读取失败",
                    update_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    nodes = new List<GraphNode>(),
                    edges = new List<GraphEdge>()
                });
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        private static GraphResponse BuildGraph(int userId)
        {
            DataTable literatureRows = QueryLiterature(userId);
            Dictionary<int, GraphNode> literatureNodes = new Dictionary<int, GraphNode>();
            Dictionary<string, GraphNode> allNodes = new Dictionary<string, GraphNode>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, GraphEdge> allEdges = new Dictionary<string, GraphEdge>(StringComparer.OrdinalIgnoreCase);
            List<int> literatureIds = new List<int>();

            if (literatureRows != null)
            {
                foreach (DataRow row in literatureRows.Rows)
                {
                    int literatureId = ToInt(row["id"]);
                    if (literatureId <= 0)
                    {
                        continue;
                    }

                    literatureIds.Add(literatureId);
                    string nodeId = "lit_" + literatureId.ToString(CultureInfo.InvariantCulture);
                    string title = Trim(ToText(row["title"]));
                    GraphNode literatureNode = new GraphNode
                    {
                        id = nodeId,
                        label = "Literature",
                        name = string.IsNullOrWhiteSpace(title) ? "未命名文献" : title,
                        properties = new Dictionary<string, string>
                        {
                            { "文献ID", literatureId.ToString(CultureInfo.InvariantCulture) },
                            { "标题", string.IsNullOrWhiteSpace(title) ? "未命名文献" : title },
                            { "DOI", Trim(ToText(row["doi"])) },
                            { "发表机构", Trim(ToText(row["institution"])) },
                            { "发表年份", Trim(ToText(row["publish_year"])) },
                            { "文献类型", Trim(ToText(row["source_type"])) },
                            { "审核状态", ToInt(row["status"]) == 1 ? "已审核" : "待审核" },
                            { "分类", Trim(ToText(row["category_name"])) },
                            { "期刊", Trim(ToText(row["journal_name"])) },
                            { "会议", Trim(ToText(row["conference_name"])) },
                            { "出版方", Trim(ToText(row["publisher"])) },
                            { "上传时间", FormatDate(row["addtime"]) }
                        }
                    };

                    literatureNodes[literatureId] = literatureNode;
                    allNodes[nodeId] = literatureNode;

                    int categoryId = ToInt(row["category_id"]);
                    string categoryName = Trim(ToText(row["category_name"]));
                    if (categoryId > 0 && !string.IsNullOrWhiteSpace(categoryName))
                    {
                        string categoryNodeId = "category_" + categoryId.ToString(CultureInfo.InvariantCulture);
                        if (!allNodes.ContainsKey(categoryNodeId))
                        {
                            allNodes[categoryNodeId] = new GraphNode
                            {
                                id = categoryNodeId,
                                label = "Category",
                                name = categoryName,
                                properties = new Dictionary<string, string>
                                {
                                    { "分类ID", categoryId.ToString(CultureInfo.InvariantCulture) },
                                    { "分类名称", categoryName }
                                }
                            };
                        }

                        AddEdge(allEdges, nodeId, categoryNodeId, "所属分类");
                    }

                    string venueName = GetVenueName(row);
                    if (!string.IsNullOrWhiteSpace(venueName))
                    {
                        string venueNodeId = StableId("venue_", venueName);
                        if (!allNodes.ContainsKey(venueNodeId))
                        {
                            allNodes[venueNodeId] = new GraphNode
                            {
                                id = venueNodeId,
                                label = "Venue",
                                name = venueName,
                                properties = new Dictionary<string, string>
                                {
                                    { "出版物名称", venueName },
                                    { "类型", GetVenueType(row) }
                                }
                            };
                        }

                        AddEdge(allEdges, nodeId, venueNodeId, "所属出版物");
                    }
                }
            }

            AddAuthorNodes(literatureIds, allNodes, allEdges);

            return new GraphResponse
            {
                code = 200,
                msg = "ok",
                update_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                nodes = new List<GraphNode>(allNodes.Values),
                edges = new List<GraphEdge>(allEdges.Values)
            };
        }

        private static DataTable QueryLiterature(int userId)
        {
            string sql = @"
SELECT TOP (@Limit)
    l.id,
    l.title,
    l.doi,
    l.institution,
    l.publish_year,
    l.source_type,
    l.journal_name,
    l.conference_name,
    l.publisher,
    l.category_id,
    l.status,
    l.userid,
    l.addtime,
    c.name AS category_name
FROM dbo.Literature l
LEFT JOIN dbo.LiteratureCategory c ON l.category_id = c.id
WHERE l.status = 1
   OR (@UserId > 0 AND l.status = 0 AND l.userid = @UserId)
ORDER BY
    CASE WHEN @UserId > 0 AND l.status = 0 AND l.userid = @UserId THEN 0 ELSE 1 END,
    l.addtime DESC,
    l.id DESC";

            using (SqlConnection conn = new SqlConnection(DBHelper.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@Limit", SqlDbType.Int).Value = MaxLiteratureCount;
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        private static void AddAuthorNodes(List<int> literatureIds, Dictionary<string, GraphNode> allNodes, Dictionary<string, GraphEdge> allEdges)
        {
            if (literatureIds == null || literatureIds.Count == 0)
            {
                return;
            }

            string idList = BuildIdList(literatureIds);
            if (string.IsNullOrWhiteSpace(idList))
            {
                return;
            }

            bool hasAffiliationText = ColumnExists("LiteratureAuthorMap", "affiliation_text");
            bool hasInstitutionMap = TableExists("LiteratureAuthorInstitutionMap");
            bool hasInstitution = TableExists("Institution");
            string affiliationSelect = hasAffiliationText ? "m.affiliation_text" : "CAST(NULL AS nvarchar(500))";
            string mapSelect = hasInstitutionMap
                ? "aim.id AS map_id, aim.institution_id, aim.affiliation_text AS map_affiliation_text, aim.institution_order"
                : "CAST(NULL AS int) AS map_id, CAST(NULL AS int) AS institution_id, CAST(NULL AS nvarchar(1000)) AS map_affiliation_text, 0 AS institution_order";
            string institutionSelect = hasInstitutionMap && hasInstitution
                ? "inst.name_cn AS institution_name_cn, inst.name_en AS institution_name_en"
                : "CAST(NULL AS nvarchar(500)) AS institution_name_cn, CAST(NULL AS nvarchar(500)) AS institution_name_en";
            string institutionMapJoin = hasInstitutionMap
                ? "LEFT JOIN dbo.LiteratureAuthorInstitutionMap aim ON aim.literature_author_map_id = m.id"
                : string.Empty;
            string institutionJoin = hasInstitutionMap && hasInstitution
                ? "LEFT JOIN dbo.Institution inst ON inst.id = aim.institution_id AND inst.status<>-1"
                : string.Empty;
            string sql = @"
SELECT
    m.id AS literature_author_map_id,
    m.literature_id,
    a.id AS author_id,
    a.name_cn,
    a.name_en,
    a.institution,
    " + affiliationSelect + @" AS affiliation_text,
    " + mapSelect + @",
    " + institutionSelect + @",
    m.author_order
FROM dbo.LiteratureAuthorMap m
INNER JOIN dbo.Author a ON m.author_id = a.id
" + institutionMapJoin + @"
" + institutionJoin + @"
WHERE m.literature_id IN (" + idList + @")
ORDER BY m.literature_id, m.author_order, m.id, institution_order, map_id";

            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(DBHelper.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(table);
            }

            foreach (DataRow row in table.Rows)
            {
                int literatureId = ToInt(row["literature_id"]);
                int authorId = ToInt(row["author_id"]);
                if (literatureId <= 0 || authorId <= 0)
                {
                    continue;
                }

                string authorNodeId = "author_" + authorId.ToString(CultureInfo.InvariantCulture);
                string rawNameCn = Trim(ToText(row["name_cn"]));
                string rawNameEn = Trim(ToText(row["name_en"]));
                string affiliationText = Trim(ToText(row["affiliation_text"]));
                string masterAffiliationText = Trim(ToText(row["map_affiliation_text"]));
                string institutionName = FirstNonEmpty(ToText(row["institution_name_cn"]), ToText(row["institution_name_en"]), masterAffiliationText);
                string nameCn = ContainsChinese(rawNameCn) ? rawNameCn : string.Empty;
                string nameEn = !string.IsNullOrWhiteSpace(rawNameEn) ? rawNameEn : (string.IsNullOrWhiteSpace(nameCn) ? rawNameCn : string.Empty);
                string authorName = !string.IsNullOrWhiteSpace(nameCn) ? nameCn : nameEn;
                if (string.IsNullOrWhiteSpace(authorName))
                {
                    authorName = "未命名作者";
                }

                if (!allNodes.ContainsKey(authorNodeId))
                {
                    allNodes[authorNodeId] = new GraphNode
                    {
                        id = authorNodeId,
                        label = "Author",
                        name = authorName,
                        properties = new Dictionary<string, string>
                        {
                            { "作者ID", authorId.ToString(CultureInfo.InvariantCulture) },
                            { "中文名", nameCn },
                            { "英文名", nameEn },
                            { "关联机构", string.Empty },
                            { "说明", "机构归属以图中的机构节点为准；未生成机构节点时表示该作者暂未精确匹配。" }
                        }
                    };
                }

                AddEdge(allEdges, "lit_" + literatureId.ToString(CultureInfo.InvariantCulture), authorNodeId, "所属作者");
                if (!string.IsNullOrWhiteSpace(institutionName))
                {
                    AddInstitutionNodeAndEdge(allNodes, allEdges, authorNodeId, ToInt(row["institution_id"]), institutionName, masterAffiliationText);
                }
                else
                {
                    foreach (string fallbackInstitution in SplitAffiliations(affiliationText))
                    {
                        AddInstitutionNodeAndEdge(allNodes, allEdges, authorNodeId, 0, fallbackInstitution, fallbackInstitution);
                    }
                }
            }
        }

        private static void AddInstitutionNodeAndEdge(Dictionary<string, GraphNode> allNodes, Dictionary<string, GraphEdge> allEdges, string authorNodeId, int institutionId, string institutionName, string sourceText)
        {
            string name = FirstNonEmpty(institutionName, sourceText);
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            string institutionNodeId = institutionId > 0
                ? "institution_" + institutionId.ToString(CultureInfo.InvariantCulture)
                : StableId("institution_", name);

            if (!allNodes.ContainsKey(institutionNodeId))
            {
                allNodes[institutionNodeId] = new GraphNode
                {
                    id = institutionNodeId,
                    label = "Institution",
                    name = name,
                    properties = new Dictionary<string, string>
                    {
                        { "机构ID", institutionId > 0 ? institutionId.ToString(CultureInfo.InvariantCulture) : string.Empty },
                        { "机构名称", name },
                        { "来源文本", Trim(sourceText) }
                    }
                };
            }

            AppendAuthorInstitution(allNodes, authorNodeId, name);
            AddEdge(allEdges, authorNodeId, institutionNodeId, "所属机构");
        }

        private static void AppendAuthorInstitution(Dictionary<string, GraphNode> allNodes, string authorNodeId, string institutionName)
        {
            GraphNode authorNode;
            if (!allNodes.TryGetValue(authorNodeId, out authorNode) || string.IsNullOrWhiteSpace(institutionName))
            {
                return;
            }

            if (authorNode.properties == null)
            {
                authorNode.properties = new Dictionary<string, string>();
            }

            string current;
            if (!authorNode.properties.TryGetValue("关联机构", out current) || string.IsNullOrWhiteSpace(current))
            {
                authorNode.properties["关联机构"] = institutionName;
                return;
            }

            foreach (string item in SplitAffiliations(current))
            {
                if (string.Equals(item, institutionName, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            authorNode.properties["关联机构"] = current + "；" + institutionName;
        }

        private static IEnumerable<string> SplitAffiliations(string value)
        {
            HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string part in Regex.Split(value ?? string.Empty, @"[;\uFF1B|\r\n]+"))
            {
                string clean = Trim(part);
                if (!string.IsNullOrWhiteSpace(clean) && !seen.Contains(clean))
                {
                    seen.Add(clean);
                    yield return clean;
                }
            }
        }

        private static string FirstNonEmpty(params string[] values)
        {
            foreach (string value in values)
            {
                string clean = Trim(value);
                if (!string.IsNullOrWhiteSpace(clean))
                {
                    return clean;
                }
            }

            return string.Empty;
        }

        private static string BuildIdList(List<int> ids)
        {
            List<string> safeIds = new List<string>();
            foreach (int id in ids)
            {
                if (id > 0)
                {
                    safeIds.Add(id.ToString(CultureInfo.InvariantCulture));
                }
            }
            return string.Join(",", safeIds.ToArray());
        }

        private static void AddEdge(Dictionary<string, GraphEdge> edges, string from, string to, string label)
        {
            string edgeId = "edge_" + from + "_" + to + "_" + StableId(string.Empty, label);
            if (edges.ContainsKey(edgeId))
            {
                return;
            }

            edges[edgeId] = new GraphEdge
            {
                id = edgeId,
                from = from,
                to = to,
                label = label,
                arrows = "to"
            };
        }

        private static string GetVenueName(DataRow row)
        {
            string journalName = Trim(ToText(row["journal_name"]));
            if (!string.IsNullOrWhiteSpace(journalName))
            {
                return journalName;
            }

            string conferenceName = Trim(ToText(row["conference_name"]));
            if (!string.IsNullOrWhiteSpace(conferenceName))
            {
                return conferenceName;
            }

            return Trim(ToText(row["publisher"]));
        }

        private static string GetVenueType(DataRow row)
        {
            if (!string.IsNullOrWhiteSpace(Trim(ToText(row["journal_name"]))))
            {
                return "期刊";
            }

            if (!string.IsNullOrWhiteSpace(Trim(ToText(row["conference_name"]))))
            {
                return "会议";
            }

            return "出版方";
        }

        private static bool ContainsChinese(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            foreach (char ch in value)
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

        private static bool ColumnExists(string tableName, string columnName)
        {
            string sql = "select count(1) from sys.columns where object_id=object_id(@tableName) and name=@columnName";
            using (SqlConnection conn = new SqlConnection(DBHelper.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@tableName", SqlDbType.NVarChar, 128).Value = "dbo." + tableName;
                cmd.Parameters.Add("@columnName", SqlDbType.NVarChar, 128).Value = columnName;
                conn.Open();
                object value = cmd.ExecuteScalar();
                return ToInt(value) > 0;
            }
        }

        private static bool TableExists(string tableName)
        {
            string sql = "select count(1) from sys.objects where object_id=object_id(@tableName) and type=N'U'";
            using (SqlConnection conn = new SqlConnection(DBHelper.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@tableName", SqlDbType.NVarChar, 128).Value = "dbo." + tableName;
                conn.Open();
                object value = cmd.ExecuteScalar();
                return ToInt(value) > 0;
            }
        }

        private static string StableId(string prefix, string value)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(value ?? string.Empty));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length && i < 8; i++)
                {
                    builder.Append(bytes[i].ToString("x2", CultureInfo.InvariantCulture));
                }
                return prefix + builder.ToString();
            }
        }

        private static int ToInt(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0;
            }

            int result;
            return int.TryParse(value.ToString(), out result) ? result : 0;
        }

        private static string ToText(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }

            string text = Function.HtmlDiscode(HttpUtility.HtmlDecode(value.ToString()));
            return string.IsNullOrEmpty(text) ? string.Empty : text.Replace('\u00A0', ' ').Replace(' ', ' ');
        }

        private static string Trim(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static string FormatDate(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }

            DateTime time;
            if (!DateTime.TryParse(value.ToString(), out time))
            {
                return string.Empty;
            }

            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private static void WriteJson(HttpContext context, object data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            context.Response.Write(serializer.Serialize(data));
        }

        private class GraphResponse
        {
            public int code { get; set; }
            public string msg { get; set; }
            public string update_time { get; set; }
            public List<GraphNode> nodes { get; set; }
            public List<GraphEdge> edges { get; set; }
        }

        private class GraphNode
        {
            public string id { get; set; }
            public string label { get; set; }
            public string name { get; set; }
            public Dictionary<string, string> properties { get; set; }
        }

        private class GraphEdge
        {
            public string id { get; set; }
            public string from { get; set; }
            public string to { get; set; }
            public string label { get; set; }
            public string arrows { get; set; }
        }
    }
}
