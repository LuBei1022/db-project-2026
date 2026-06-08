using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Web.admin
{
    public partial class Right : System.Web.UI.Page
    {
        BLLBase<Model.admin> adminbll = new BLLBase<Model.admin>();
        BLLBase<popedom> popedombll = new BLLBase<popedom>();
        BLLBase<popedomhead> popedomheadbll = new BLLBase<popedomhead>();
        string MenuId = Function.GetRequest("MenuID");
        string Action = Function.GetRequest("Action");
        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            switch (Action)
            {
                case "set_default_page":
                    Default_PageFunc();
                    break;
                case "setcheck":
                    SetCheckFunc();
                    break;
                case "setcache":
                    SetCacheFunc();
                    break;
                case "logout":
                    SetLogoutFunc();
                    break;
                default:
                    Default_PageFunc();
                    break;
            }
        }


        protected void SetLogoutFunc()
        {

            Cookie.ClearCookie("LMS_AdminID");
            Cookie.ClearCookie("LMS_AdminName");
            Cookie.ClearCookie("LMS_Popedom");
            Function.Show_Msg("已成功退出！", "Login.aspx");
        }

        protected void SetCacheFunc()
        {
            Model.admin admin = adminbll.SelectSingle("username='" + Cookie.GetCookie("LMS_AdminName") + "'");
            if (admin != null && admin.id > 0)
            {
                Cookie.SaveCookie("LMS_Popedom", admin.popedom, 0);
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "更新缓存成功!", "Admin_ServerInfo.aspx?MenuID=Right", 0);
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "更新缓存失败!", "Admin_ServerInfo.aspx?MenuID=Right", 2);
            }

        }

        protected void SetCheckFunc()
        {

            int id = Function.ConvertTo<int>(Function.GetRequest("id"), 0);
            if (id > 0)
            {

                popedom popedom = popedombll.SelectSingle("id", id);
                if (popedom != null && popedom.id > 0)
                {
                    string[] PopedomStr = Cookie.GetCookie("LMS_Popedom").Split(',');
                    bool isyes = false;
                    foreach (string item in PopedomStr)
                    {
                        if (Function.ConvertTo<string>(popedom.id, "") == item)
                        {
                            isyes = true;
                        }
                    }
                    if (isyes)
                    {

                        if (popedom.popedom_father != null)
                        {
                            popedomhead popedomhead = popedomheadbll.SelectSingle("popedomid=" + popedom.popedom_father + " and adminid=" + Function.ConvertTo<int>(Cookie.GetCookie("LMS_AdminID"), 0));
                            if (popedomhead != null && popedomhead.id != null && popedomhead.id > 0)
                            {
                                if (popedomheadbll.Update("headid=" + id, "id=" + popedomhead.id))
                                {
                                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "栏目默认页设置成功!", "right.aspx?MenuID=Right&Action=set_default_page", 0);
                                }
                                else
                                {
                                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "栏目默认页设置失败!", "right.aspx?MenuID=Right&Action=set_default_page", 2);
                                }
                            }
                            else
                            {
                                popedomhead popedomhead_add = new popedomhead();
                                popedomhead_add.adminid = Function.ConvertTo<int>(Cookie.GetCookie("LMS_AdminID"), 0);
                                popedomhead_add.headid = id;
                                popedomhead_add.popedomid = popedom.popedom_father;
                                if (popedomheadbll.Add(popedomhead_add, "id") > 0)
                                {
                                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "栏目默认页设置成功!", "right.aspx?MenuID=Right&Action=set_default_page", 0);
                                }
                                else
                                {
                                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "栏目默认页设置失败!", "right.aspx?MenuID=Right&Action=set_default_page", 2);
                                }
                            }


                        }
                        else
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "栏目默认页设置失败!", "right.aspx?MenuID=Right&Action=set_default_page", 2);
                        }
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "获取设置栏目默认页的参数错误", "right.aspx?MenuID=Right&Action=set_default_page", 2);
                    }

                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "获取设置栏目默认页的参数错误!", "right.aspx?MenuID=Right&Action=set_default_page", 2);
                }

            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "获取设置栏目默认页的参数错误", "right.aspx?MenuID=Right&Action=set_default_page", 2);
            }
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        protected void Default_PageFunc()
        {
            Default_Page.Visible = true;
            DataTable popedomdt = popedombll.GetDatatable("select * from popedom where popedom_father=0 and id in(" + Cookie.GetCookie("LMS_Popedom") + ") order by orderid asc");
            if (popedomdt != null && popedomdt.Rows.Count > 0)
            {
                this.myRepeater.DataSource = popedomdt.DefaultView;
                this.myRepeater.DataBind();
            }
            popedomdt.Dispose();
        }
        //Repeater显示两列第二种方法
        //int index = 0; // 全局字段 
        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // if (index % 2 == 0 && index > 0)
            // {
            //     e.Item.Controls.Add(new LiteralControl(" </tr> <tr>"));
            // }
            // index++;


            //内部myRepeater2
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                int Id = Convert.ToInt32(((DataRowView)e.Item.DataItem).Row["Id"]);

                Repeater myRepeater2 = (Repeater)e.Item.FindControl("myRepeater2");

                if (myRepeater2 != null)
                {

                    DataTable popedomdt = popedombll.GetDatatable("select * from popedom where  popedom_father=" + Id + " and  id in(" + Cookie.GetCookie("LMS_Popedom") + ") order by orderid asc");
                    if (popedomdt != null && popedomdt.Rows.Count > 0)
                    {
                        myRepeater2.DataSource = popedomdt.DefaultView;
                        myRepeater2.DataBind();
                    }
                    popedomdt.Dispose();
                }
            }
        }

        public string GetIsHead(string popedomidstr)
        {
            string rstr = "";
            int popedomidint = Function.ConvertTo<int>(popedomidstr, 0);
            if (popedomidint > 0)
            {
                popedomhead popedomhead = popedomheadbll.SelectSingle("popedomid=" + popedomidint + " and adminid=" + Function.ConvertTo<int>(Cookie.GetCookie("LMS_AdminID"), 0));
                if (!(popedomhead != null && popedomhead.id != null && popedomhead.id > 0))
                {
                    rstr = "<font color=\"red\">(还没有设置默认页)</font>";
                }
                else
                {
                    string[] PopedomStr = Cookie.GetCookie("LMS_Popedom").Split(',');
                    bool isyes = false;
                    foreach (string item in PopedomStr)
                    {
                        if (Function.ConvertTo<string>(popedomhead.headid, "") == item)
                        {
                            isyes = true;
                        }
                    }
                    if (!isyes)
                    {
                        rstr = "<font color=\"red\">(还没有设置默认页)</font>";
                    }
                }
            }
            else
            {
                rstr = "<font color=\"red\">&nbsp;--&nbsp;(还没有设置默认页)</font>";
            }
            return rstr;
        }

        public string GetIsSet(string f_id, string id)
        {
            int f_id_int = Function.ConvertTo<int>(f_id, 0);
            int id_int = Function.ConvertTo<int>(id, 0);
            string R_str = "获取失败";
            if (f_id_int > 0 && id_int > 0)
            {
                popedomhead popedomhead = popedomheadbll.SelectSingle("popedomid=" + f_id_int + " and adminid=" + Function.ConvertTo<int>(Cookie.GetCookie("LMS_AdminID"), 0));
                if (popedomhead != null && popedomhead.id != null && popedomhead.id > 0)
                {
                    string[] PopedomStr = Cookie.GetCookie("LMS_Popedom").Split(',');
                    bool isyes = false;
                    foreach (string item in PopedomStr)
                    {
                        if (Function.ConvertTo<string>(popedomhead.headid, "") == item && id == item)
                        {
                            isyes = true;
                        }
                    }
                    if (isyes)
                    {
                        R_str = "<font color=\"red\">已设置</font>";
                    }
                    else
                    {
                        R_str = "<a href='Right.aspx?MenuId=Right&Action=setcheck&id=" + id + "'>设置默认页</a>";
                    }
                }
                else
                {
                    R_str = "<a href='Right.aspx?MenuId=Right&Action=setcheck&id=" + id + "'>设置默认页</a>";
                }
            }
            return R_str;
        }
    }
}