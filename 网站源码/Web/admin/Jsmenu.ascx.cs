using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;

namespace Web.admin
{
    public partial class Jsmenu : System.Web.UI.UserControl
    {
        BLLBase<tbl_class> tbl_classbll = new BLLBase<tbl_class>();
        public string str = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            str = string.Empty;
            if (!IsPostBack)
            {
                LoadList();
            }
        }
        /// <summary>
        /// 绑定数据
        /// </summary>
        protected void LoadList()
        {
            string left_str = "";
            string mid_str = "";
            string right_str = "";

            left_str = left_str + "var zNodes_ = [";
            mid_str = BindDrpClass();
            right_str = "];";
            str = left_str + mid_str + right_str;

        }

        string return_str = "";
        string menu_str = "";

        private string BindDrpClass()
        {
            string nav_str = "";
            DataTable tbl_classdt = tbl_classbll.GetDatatable("select * from tbl_class where 1=1 order by orderid asc");
            if (tbl_classdt != null && tbl_classdt.Rows.Count > 0)
            {
                DataRow[] drs = tbl_classdt.Select("parentid= " + 360);
                foreach (DataRow dr in drs)
                {
                    int classid = int.Parse(dr["id"].ToString());
                    int mid = int.Parse(dr["Model"].ToString());

                    if (!dr.IsNull("classname"))
                    {
                        if (string.IsNullOrWhiteSpace(nav_str))
                        {
                            nav_str = "{ name: \"" + Function.HtmlDiscodeWeb(dr["classname"].ToString()).Replace("\"", "&quot;") + "\"" + BindNode(classid, mid, tbl_classdt) + GetUrl(mid, classid) + "}";
                        }
                        else
                        {
                            nav_str = nav_str + ",{ name: \"" + Function.HtmlDiscodeWeb(dr["classname"].ToString()).Replace("\"", "&quot;") + "\"" + BindNode(classid, mid, tbl_classdt) + GetUrl(mid, classid) + "}";
                        }
                    }
                    return_str = "";
                    menu_str = "";
                }
            }
            tbl_classdt.Dispose();
            return nav_str;
        }

        //绑定子分类

        private string BindNode(int cid, int model_id, DataTable dt)
        {
            return_str = "";
            menu_str = "";
            DataRow[] drs = dt.Select("parentid= " + cid);
            if (drs.Length >= 1)
            {
                foreach (DataRow dr in drs)
                {
                    int classid = int.Parse(dr["id"].ToString());
                    int mid = int.Parse(dr["Model"].ToString());

                    if (string.IsNullOrWhiteSpace(menu_str))
                    {
                        menu_str = "{ name: \"" + Function.HtmlDiscodeWeb(dr["classname"].ToString()).Replace("\"", "&quot;") + "\"" + BindNode(classid, mid, dt) + GetUrl(mid, classid) + "}";
                    }
                    else
                    {
                        menu_str = menu_str + ",{ name: \"" + Function.HtmlDiscodeWeb(dr["classname"].ToString()).Replace("\"", "&quot;") + "\"" + BindNode(classid, mid, dt) + GetUrl(mid, classid) + "}";
                    }

                    return_str = ",children: [" + menu_str + "]";
                }

            }

            else
            {
                return_str = GetUrl(model_id, cid);
            }


            return return_str;
        }


        public DataTable GetClassList(string strWhere)
        {
            string strsql = "select * from tbl_class ";

            if (strWhere.Trim() != "")
            {
                strsql += strsql;
            }
            strsql += " order by orderid asc";
            return tbl_classbll.GetDatatable(strsql);
        }


        public string GetUrl(int mid, int cid)
        {
            string pageUrl;
            int menuId = Function.ConvertTo<int>(Function.GetRequest("MenuId"), 43);

            if (mid == 2)
            {
                pageUrl = CommonFunc.GetModelUrl(mid.ToString()) + "?tbclass_id=" + cid + "&listid=" + GetChildrenId(cid) + "&MenuId=" + menuId;
            }
            else if (mid == 1 || mid == 3)
            {
                pageUrl = "Admin_SiteMenu.aspx?Action=EditColumn&ID=" + cid + "&ParentId=360&MenuId=" + menuId;
            }
            else
            {
                pageUrl = CommonFunc.GetModelUrl(mid.ToString()) + "?tbclass_id=" + cid + "&listid=" + GetChildrenId(cid) + "&MenuId=" + menuId;
            }

            string return_url = ", url: \"" + pageUrl + "\", \"target\": \"menu_info\"";
            return return_url;
        }




        string classid_str = "";
        private string GetChildrenId(int id)
        {
            string nav_str = "";
            string yi = "";
            DataTable dt = GetClassList("");
            DataRow[] drs = dt.Select("parentid= " + id);

            foreach (DataRow dr in drs)
            {
                int classid = int.Parse(dr["id"].ToString());
                yi = yi + classid + ",";
                nav_str = GetId(classid, dt) + yi;
            }
            classid_str = "";
            dt.Dispose();
            return nav_str + id;
        }

        //绑定子分类
        private string GetId(int cid, DataTable dt)
        {
            DataRow[] drs = dt.Select("parentid= " + cid);

            foreach (DataRow dr in drs)
            {
                int classid = int.Parse(dr["id"].ToString());
                classid_str = dr["id"] + "," + GetId(classid, dt);
            }
            return classid_str;
        }

    }

}
