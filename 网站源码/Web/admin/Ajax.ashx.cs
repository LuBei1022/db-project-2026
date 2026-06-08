using LiteratureManager.Common;
using BLL;
using Model;
using System.Data;
using System.Web;

namespace Web.admin
{
    /// <summary>
    /// Ajax 的摘要说明
    /// </summary>
    public class Ajax : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Function.Check_AdminLogin();
            context.Response.ContentType = "text/plain";
            string action = context.Request.QueryString["action"];
            string tb = context.Request.QueryString["tb"];
            string id = context.Request.QueryString["id"];
            string val = context.Request.QueryString["val"];
            BLLBase<popedom> popedombll = new BLLBase<popedom>();

            if (id != "" && tb != "" && action != "")
            {
                if (action.ToUpper().Contains("IS"))
                {
                    DataTable dt = popedombll.GetDatatable("select * from " + tb + " where id=" + id);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][action.ToLower()].ToString() == "1")
                        {
                            if (popedombll.GetExecSql("update " + tb + " set " + action.ToLower() + "=0 where id=" + id))
                            {

                                context.Response.Write("images/no.gif");
                            }
                            else
                            {
                                context.Response.Write("images/yes.gif");
                            }
                        }
                        else
                        {
                            if (popedombll.GetExecSql("update " + tb + " set " + action.ToLower() + "=1 where id=" + id))
                            {

                                context.Response.Write("images/yes.gif");
                            }
                            else
                            {
                                context.Response.Write("images/no.gif");
                            }
                        }
                    }
                    else
                    {
                        context.Response.Write("images/no.gif");
                    }
                    dt.Dispose();
                }
                else if (action.ToUpper().Contains("ORDERID"))
                {
                    if (popedombll.GetExecSql("update " + tb + " set " + action + "=" + val + " where id=" + id))
                    {
                        System.Threading.Thread.Sleep(300);
                        context.Response.Write(val);
                    }

                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}