using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace Web.admin
{
    public partial class Left : System.Web.UI.Page
    {
        BLLBase<popedom> popedombll = new BLLBase<popedom>();
        private static readonly HashSet<int> AllowedTopMenuIds = new HashSet<int> { 43, 246, 605, 677, 671, 1722, 692, 699 };
        private static readonly HashSet<int> AllowedMenuIds = new HashSet<int>
        {
            43,45,636,
            246,247,248,615,
            605,669,670,589,667,657,
            677,678,
            671,672,
            1722,1723,1724,1725,1726,1727,1728,1729,1730,1731,1732,1733,
            692,693,695,696,697,698,704,
            699,700
        };
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                Function.Check_AdminLogin();
                BindData();
            }
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        protected void BindData()
        {
            DataTable popedomdt = popedombll.GetDatatable("select * from popedom where popedom_father=0 and id in(" + Cookie.GetCookie("LMS_Popedom") + ") order by orderid asc");
            if (popedomdt != null && popedomdt.Rows.Count > 0)
            {
                DataView view = new DataView(popedomdt);
                view.RowFilter = BuildAllowedFilter(AllowedTopMenuIds);
                this.myRepeater.DataSource = view;
                this.myRepeater.DataBind();
            }
            popedomdt.Dispose();


        }

        public string GetPopedomChildren2(string Popedom_id)
        {
            string R_str = string.Empty;
            //判断权限
            DataTable popedomdt = popedombll.GetDatatable("select * from popedom where popedom_father=" + Popedom_id + " and id in(" + Cookie.GetCookie("LMS_Popedom") + ") order by orderid asc");
            if (popedomdt != null && popedomdt.Rows.Count > 0)
            {
                DataView view = new DataView(popedomdt);
                view.RowFilter = BuildAllowedFilter(AllowedMenuIds);
                if (view.Count <= 0)
                {
                    popedomdt.Dispose();
                    return string.Empty;
                }
                R_str += "                    <ul class=\"nav nav-treeview\">";
                foreach (DataRowView rowView in view)
                {
                    DataRow item = rowView.Row;
                    R_str += "                        <li class=\"nav-item\">";
                    R_str += "                            <a href=\"" + GetUrl(item["popedom_url"].ToString(), item["id"].ToString()) + "\" class=\"nav-link\"  hidefocus=\"true\" target=\"main\">";
                    //R_str += "                                <i class=\"nav-icon bi bi-circle\"></i>";
                    R_str += "                                <p>" + Function.HtmlDiscode(item["popedom_name"].ToString()) + "</p>";
                    R_str += "                            </a>";
                    R_str += "                        </li>";
                }
                R_str += "              </ul>";
            }
            popedomdt.Dispose();
            return R_str;
        }

        private string BuildAllowedFilter(HashSet<int> ids)
        {
            if (ids == null || ids.Count <= 0)
            {
                return "1=0";
            }
            return "id in (" + string.Join(",", ids) + ")";
        }

        public string GetUrl(string popedom_url, string id_str)
        {
            string url = "";
            int id_ = Function.ConvertTo<int>(id_str, 0);
            if (!string.IsNullOrWhiteSpace(popedom_url) && id_ > 0)
            {
                if (popedom_url.IndexOf("?") != -1)
                {
                    url = popedom_url + "&MenuId=" + id_;
                }
                else
                {
                    url = popedom_url + "?MenuId=" + id_;
                }
            }
            else
            {
                url = "";
            }
            return url;
        }
    }
}
