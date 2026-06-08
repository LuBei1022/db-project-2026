using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Web.WebsiteData
{
    public partial class NewsInfo : System.Web.UI.Page
    {
        BLLBase<data_list> data_listbll = new BLLBase<data_list>();
        BLLBase<tbl_class> tbl_classbll = new BLLBase<tbl_class>();
        public string data_title = string.Empty;
        public string banner = string.Empty;
        public bool webisyes = false;
        public tbl_class tbl_class = new tbl_class();
        public data_list data_list = new data_list();
        public string nex_href = string.Empty;
        public string prev_href = string.Empty;
        public string nex_name = "null";
        public string prev_name = "null";
        public bool istop = false;

        public string GetConsultingDetailHtml()
        {
            string title = Function.HtmlDiscode(data_list == null ? string.Empty : data_list.name);
            string description = Function.HtmlDiscode(data_list == null ? string.Empty : data_list.description);
            string intro = !string.IsNullOrWhiteSpace(description) ? description : BuildDefaultIntro(title);
            string focus = GetConsultingFocus(title);
            string[] scenarios = GetScenarioItems(title);
            string[] process = GetProcessItems(title);
            string[] deliverables = GetDeliverableItems(title);

            StringBuilder html = new StringBuilder();
            html.Append("<section class=\"consult-enhance\">");
            html.Append("<div class=\"consult-enhance-head\"><span>咨询导读</span><h5>");
            html.Append(Server.HtmlEncode(focus));
            html.Append("</h5><p>");
            html.Append(Server.HtmlEncode(intro));
            html.Append("</p></div>");
            html.Append(BuildConsultBlock("适用场景", scenarios));
            html.Append(BuildConsultBlock("服务流程", process));
            html.Append(BuildConsultBlock("可获得的支持", deliverables));
            html.Append("<div class=\"consult-note\"><strong>建议准备</strong><p>为了提高沟通效率，可提前整理研究主题、当前进度、目标期刊或院校要求、已有材料与希望解决的具体问题，咨询时将围绕这些信息给出更聚焦的建议。</p></div>");
            html.Append("</section>");
            return html.ToString();
        }

        private string BuildConsultBlock(string title, string[] items)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<div class=\"consult-block\"><h6>");
            html.Append(Server.HtmlEncode(title));
            html.Append("</h6><ul>");
            foreach (string item in items)
            {
                html.Append("<li>");
                html.Append(Server.HtmlEncode(item));
                html.Append("</li>");
            }
            html.Append("</ul></div>");
            return html.ToString();
        }

        private string BuildDefaultIntro(string title)
        {
            string cleanTitle = string.IsNullOrWhiteSpace(title) ? "该主题" : title;
            return cleanTitle + "围绕学术研究过程中的关键问题提供结构化说明，帮助用户明确问题边界、拆解执行步骤，并形成更可落地的研究与写作方案。";
        }

        private string GetConsultingFocus(string title)
        {
            string key = NormalizeText(title);
            if (key.Contains("投稿") || key.Contains("期刊") || key.Contains("会议") || key.Contains("发表"))
            {
                return "从选刊定位到投稿材料的全流程建议";
            }
            if (key.Contains("论文") || key.Contains("写作") || key.Contains("润色") || key.Contains("修改"))
            {
                return "围绕论文结构、论证表达与修改路径进行优化";
            }
            if (key.Contains("文献") || key.Contains("综述") || key.Contains("检索"))
            {
                return "帮助建立文献检索、筛选、阅读与综述框架";
            }
            if (key.Contains("开题") || key.Contains("课题") || key.Contains("选题") || key.Contains("研究方案"))
            {
                return "协助明确研究问题、技术路线与开题表达";
            }
            if (key.Contains("查重") || key.Contains("降重") || key.Contains("重复率"))
            {
                return "在保持学术原意的前提下优化表达与引用规范";
            }
            return "面向研究、写作、发表与材料准备的综合咨询";
        }

        private string[] GetScenarioItems(string title)
        {
            string key = NormalizeText(title);
            if (key.Contains("投稿") || key.Contains("期刊") || key.Contains("会议") || key.Contains("发表"))
            {
                return new[] {
                    "已经完成论文初稿，希望判断投稿方向与期刊匹配度。",
                    "需要梳理投稿前的格式、作者信息、推荐审稿人等材料。",
                    "收到编辑或审稿意见后，需要拆解回复策略与修改优先级。"
                };
            }
            if (key.Contains("文献") || key.Contains("综述") || key.Contains("检索"))
            {
                return new[] {
                    "研究主题已经确定，但缺少系统的检索词、数据库与筛选标准。",
                    "阅读文献较多却难以归纳研究脉络、方法差异和创新空间。",
                    "需要将文献材料整理为综述框架、研究背景或理论基础。"
                };
            }
            if (key.Contains("开题") || key.Contains("课题") || key.Contains("选题") || key.Contains("研究方案"))
            {
                return new[] {
                    "需要从宽泛兴趣中收敛出可执行、可论证的研究问题。",
                    "开题报告中的研究意义、技术路线、可行性论证还不够清晰。",
                    "希望提前识别数据、方法、周期和成果形式中的风险点。"
                };
            }
            if (key.Contains("查重") || key.Contains("降重") || key.Contains("重复率"))
            {
                return new[] {
                    "论文重复率偏高，需要区分引用问题、表达重复和结构重复。",
                    "希望在不改变原意和学术规范的基础上提升表达原创性。",
                    "需要检查参考文献、引文标注和常见格式细节。"
                };
            }
            return new[] {
                "研究或论文推进过程中遇到方向、材料、表达或规范问题。",
                "需要把零散想法整理成清晰的问题清单和执行路径。",
                "希望获得更贴合当前阶段的学术资料、方法和写作建议。"
            };
        }

        private string[] GetProcessItems(string title)
        {
            string focus = GetConsultingFocus(title);
            return new[] {
                "需求梳理：确认当前阶段、目标要求、已有材料和最需要解决的问题。",
                "问题诊断：围绕“" + focus + "”检查关键缺口，明确优先处理事项。",
                "方案细化：给出检索、写作、修改、投稿或材料准备的具体执行步骤。",
                "结果复盘：根据反馈继续调整方向，形成可继续推进的下一步清单。"
            };
        }

        private string[] GetDeliverableItems(string title)
        {
            string key = NormalizeText(title);
            if (key.Contains("投稿") || key.Contains("期刊") || key.Contains("会议") || key.Contains("发表"))
            {
                return new[] {
                    "投稿方向与候选期刊/会议筛选思路。",
                    "投稿材料、格式规范和审稿意见回复建议。",
                    "修改优先级清单与后续时间安排。"
                };
            }
            if (key.Contains("文献") || key.Contains("综述") || key.Contains("检索"))
            {
                return new[] {
                    "关键词组合、数据库检索式与筛选标准建议。",
                    "文献阅读表、主题聚类和综述结构建议。",
                    "可用于论文背景、相关工作或理论基础的整理框架。"
                };
            }
            if (key.Contains("开题") || key.Contains("课题") || key.Contains("选题") || key.Contains("研究方案"))
            {
                return new[] {
                    "研究问题、研究目标与创新点表达建议。",
                    "技术路线、章节结构和进度安排优化建议。",
                    "开题答辩中常见问题的准备方向。"
                };
            }
            return new[] {
                "问题诊断记录和关键修改建议。",
                "适合当前阶段的资料准备清单。",
                "下一步可执行的研究、写作或沟通计划。"
            };
        }

        private string NormalizeText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }
            return Regex.Replace(HttpUtility.HtmlDecode(value), "\\s+", string.Empty).ToLowerInvariant();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    data_title = string.Empty;
                    banner = string.Empty;
                    webisyes = false;
                    istop = false;
                    data_list = data_listbll.SelectSingle("id=" + Function.ConvertTo<int>(Function.GetRequest("id"), 0) + " and isshow=1");
                    if (data_list != null && data_list.id > 0)
                    {
                        tbl_class = tbl_classbll.SelectSingle("id=" + data_list.tbclass_id + " and isshow=1 and model in(2) and id in(" + Function.Decrypt(CommonFunc.GetChildrenId(360)) + ")");
                        if (tbl_class != null && tbl_class.id > 0 && tbl_class.parentid > 0)
                        {
                            data_title = CommonFunc.GetTbClassTitle(tbl_class);
                            banner = CommonFunc.GetBannerImg(tbl_class.upload_pic_pc, tbl_class.upload_pic_m);

                            string prevsql = "select id,name from data_list where tbclass_id=" + tbl_class.id + " and isshow=1 and orderid>=" + data_list.orderid + " order by orderid asc,uptime asc,id asc";
                            DataTable prev_data_listdt = data_listbll.GetDatatable(prevsql);
                            if (prev_data_listdt != null && prev_data_listdt.Rows.Count > 0)
                            {
                                while (true)
                                {
                                    if (prev_data_listdt.Rows[0]["id"].ToString().Equals(data_list.id.ToString()))
                                    {
                                        prev_data_listdt.Rows.RemoveAt(0);
                                        break;
                                    }
                                    prev_data_listdt.Rows.RemoveAt(0);
                                }
                                if (prev_data_listdt != null && prev_data_listdt.Rows.Count > 0)
                                {
                                    prev_href = "/WebsiteData/NewsInfo.aspx?id=" + Function.HtmlDiscode(prev_data_listdt.Rows[0]["id"].ToString());
                                    prev_name = Function.HtmlDiscodeWeb(prev_data_listdt.Rows[0]["name"].ToString());
                                }
                            }
                            prev_data_listdt.Dispose();


                            string nexsql = "select id,name from data_list where tbclass_id=" + tbl_class.id + " and isshow=1 and orderid<=" + data_list.orderid + " order by orderid desc,uptime desc,id desc";
                            DataTable nex_data_listdt = data_listbll.GetDatatable(nexsql);
                            if (nex_data_listdt != null && nex_data_listdt.Rows.Count > 0)
                            {
                                while (true)
                                {
                                    if (nex_data_listdt.Rows[0]["id"].ToString().Equals(data_list.id.ToString()))
                                    {
                                        nex_data_listdt.Rows.RemoveAt(0);
                                        break;
                                    }
                                    nex_data_listdt.Rows.RemoveAt(0);
                                }


                                if (nex_data_listdt != null && nex_data_listdt.Rows.Count > 0)
                                {
                                    nex_href = "/WebsiteData/NewsInfo.aspx?id=" + Function.HtmlDiscode(nex_data_listdt.Rows[0]["id"].ToString());
                                    nex_name = Function.HtmlDiscodeWeb(nex_data_listdt.Rows[0]["name"].ToString());
                                }
                            }
                            nex_data_listdt.Dispose();

                            DataTable top_data_listdt = data_listbll.GetDatatable("select top 6 id,name,datetime,upload_pic_img from data_list where tbclass_id=" + data_list.tbclass_id + " and isshow=1 and id not in (" + data_list.id + ") order by istop desc, orderid desc,uptime desc,id desc");
                            if (top_data_listdt != null && top_data_listdt.Rows.Count > 0)
                            {
                                this.TopNewsList.DataSource = top_data_listdt.DefaultView;
                                this.TopNewsList.DataBind();
                                istop = true;
                            }
                            top_data_listdt.Dispose();

                            webisyes = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ImportDataLog.WriteLog(LogType.Error, "NewsInfo.aspx_Error:" + ex.Message + "-" + ex.StackTrace);
                }
                if (!webisyes)
                {
                    Response.Redirect("/err");
                    Response.End();
                }
            }
        }
    }
}
