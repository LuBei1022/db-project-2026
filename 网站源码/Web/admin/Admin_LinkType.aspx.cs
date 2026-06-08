using LiteratureManager.Common;
using BLL;
using Model;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Web.admin
{
    public partial class Admin_LinkType : System.Web.UI.Page
    {
        BLLBase<popedom> popedombll = new BLLBase<popedom>();
        string MenuId = Function.GetRequest("MenuID");
        string Action = Function.GetRequest("Action");
        public bool isLoading = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            Function.Check_AdminLogin();
            isLoading = true;
            switch (Action)
            {
                case "AddBigClass":
                    AddBigFunc();
                    break;
                case "AddSmallClass":
                    AddSmallFunc();
                    break;
                case "UpBig":
                    UpBigFunc();
                    break;
                case "UpSmall":
                    UpSmallFunc();
                    break;
                case "Del":
                    Del();
                    break;
                default:
                    BindData();
                    break;
            }
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        protected void BindData()
        {
            Main.Visible = true;
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


        protected void Big_List(string ParentID)
        {
            ///加载一级类
            BigClassValue.Items.Add(new ListItem("-选择类别-", ""));

            DataTable popedomdt = popedombll.GetDatatable("select * from popedom where popedom_father=0");
            if (popedomdt != null && popedomdt.Rows.Count > 0)
            {
                foreach (DataRow item in popedomdt.Rows)
                {
                    BigClassValue.Items.Add(new ListItem("" + Function.HtmlDiscode(item["popedom_name"].ToString()) + "", "" + item["id"] + ""));

                }
            }
            popedomdt.Dispose();
            BigClassValue.SelectedValue = ParentID;
        }

        protected void AddBigFunc()
        {
            Big.Visible = true;
        }
        protected void AddSmallFunc()
        {
            Small.Visible = true;
            Big_List("");
        }

        protected void UpBigFunc()
        {
            Big.Visible = true;
            int id = Function.ConvertTo<int>(Function.GetRequest("id"), 0);
            popedom popedom = popedombll.SelectSingle("id", id);
            if (popedom != null && popedom.id > 0)
            {
                NameType.Text = popedom.popedom_name;
            }
            else
            {
                NameType.Text = "获取失败";
            }
        }

        protected void UpSmallFunc()
        {
            string ParentID = Function.GetRequest("ParentID");
            int id = Function.ConvertTo<int>(Function.GetRequest("id"), 0);
            Small.Visible = true;
            Big_List(ParentID);

            popedom popedom = popedombll.SelectSingle("id", id);
            if (popedom != null && popedom.id > 0)
            {

                SmallTypeName.Text = popedom.popedom_name;
                SmallClassUrl.Text = popedom.popedom_url;
            }
            else
            {
                SmallTypeName.Text = "获取失败";
            }
        }


        protected void UpOrderFunc()
        {
            Main.Visible = false;
            int id = Function.ConvertTo<int>(Function.GetRequest("id"), 0);
            string orderid = Function.FormRequest("orderid" + id + "");
            if (popedombll.Update("orderid=" + orderid, "id=" + id))
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "排序修改成功!", "Admin_LinkType.aspx?MenuId=" + MenuId + "", 0);
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "排序修改成功!", "Admin_LinkType.aspx?MenuId=" + MenuId + "", 2);
            }


        }

        protected void OnClick_BigClass(object sender, EventArgs e)
        {
            string NameType = Function.FormRequest("NameType");
            int id = Function.ConvertTo<int>(Function.GetRequest("id"), 0);
            isLoading = false;
            int max = 1;
            if (Action == "AddBigClass")
            {
                Big.Visible = false;
                DataTable mxdt = popedombll.GetDatatable("Select max(orderid) as mx from popedom where popedom_father=0");
                if (mxdt != null && mxdt.Rows.Count > 0)
                {
                    max = Function.ConvertTo<int>(mxdt.Rows[0]["mx"], 0) + 1;
                }
                mxdt.Dispose();
                popedom popedom = new popedom();
                popedom.popedom_name = NameType;
                popedom.popedom_father = 0;
                popedom.orderid = max;
                if (popedombll.Add(popedom, "id") > 0)
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "一级栏目添加成功!", "Admin_LinkType.aspx?MenuId=" + MenuId + "", 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "一级栏目添加失败!", "Admin_LinkType.aspx?MenuId=" + MenuId + "", 2);
                }
            }
            else if (Action == "UpBig")
            {
                Big.Visible = false;
                if (id > 0)
                {
                    popedom popedom = popedombll.SelectSingle("id", id);
                    if (popedom != null && popedom.id > 0)
                    {
                        if (popedombll.Update("popedom_name='" + NameType + "'", "id=" + popedom.id))
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "一级栏目修改成功!", "Admin_LinkType.aspx?MenuId=" + MenuId + "", 0);
                        }
                        else
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "一级栏目修改失败!", "Admin_LinkType.aspx?MenuId=" + MenuId + "", 2);
                        }
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "一级栏目修改失败!", "Admin_LinkType.aspx?MenuId=" + MenuId + "", 2);
                    }
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "获取请求修改一级栏目参数错误!", "Admin_LinkType.aspx?MenuId=" + MenuId + "", 3);
                }
            }

        }

        protected void OnClick_SmallClass(object sender, EventArgs e)
        {
            int BigClassValue = Function.ConvertTo<int>(Function.FormRequest("BigClassValue"), 0);
            string SmallTypeName = Function.FormRequest("SmallTypeName");
            string SmallClassUrl = Request.Form["SmallClassUrl"];
            int id = Function.ConvertTo<int>(Function.GetRequest("id"), 0);
            int max = 1;
            isLoading = false;
            if (Action == "AddSmallClass")
            {
                Small.Visible = false;
                DataTable maxdt = popedombll.GetDatatable("Select max(orderid) as mx from popedom where popedom_father=" + BigClassValue);
                if (maxdt != null && maxdt.Rows.Count > 0)
                {
                    max = Function.ConvertTo<int>(maxdt.Rows[0]["mx"], 0) + 1;
                }
                maxdt.Dispose();
                popedom popedom = new popedom();
                popedom.popedom_name = SmallTypeName;
                popedom.popedom_father = BigClassValue;
                popedom.popedom_url = SmallClassUrl;
                popedom.orderid = max;
                if (popedombll.Add(popedom, "id") > 0)
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "二级栏目添加成功!", "Admin_LinkType.aspx?MenuId=" + MenuId + "", 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "二级栏目添加失败!", "Admin_LinkType.aspx?MenuId=" + MenuId + "", 2);
                }

            }
            else if (Action == "UpSmall")
            {
                Small.Visible = false;

                if (id > 0)
                {
                    popedom popedom = popedombll.SelectSingle("id", id);
                    if (popedom != null && popedom.id > 0)
                    {
                        if (popedombll.Update("popedom_name='" + SmallTypeName + "',popedom_father=" + BigClassValue + ",popedom_url='" + SmallClassUrl + "'", "id=" + popedom.id))
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "二级栏目修改成功!", "Admin_LinkType.aspx?MenuId=" + MenuId + "", 0);
                        }
                        else
                        {
                            Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "二级栏目修改失败!", "Admin_LinkType.aspx?MenuId=" + MenuId + "", 2);
                        }
                    }
                    else
                    {
                        Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "二级栏目修改失败!", "Admin_LinkType.aspx?MenuId=" + MenuId + "", 2);
                    }
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "获取请求修改二级栏目参数错误!", "Admin_LinkType.aspx?MenuId=" + MenuId + "", 3);
                }
            }

        }

        protected void Del()
        {
            isLoading = false;
            string BackURL = Request.QueryString["BackURL"];
            int id = Function.ConvertTo<int>(Function.GetRequest("id"), 0);
            if (id > 0)
            {
                if (popedombll.Delete("id", id))
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "栏目删除成功!", BackURL, 0);
                }
                else
                {
                    Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "栏目删除失败!", BackURL, 2);
                }
            }
            else
            {
                Function.Ok_Return(Cookie.GetCookie("LMS_AdminName"), "获取请求删除栏目参数错误!", BackURL, 3);
            }
        }

    }

}