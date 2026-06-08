using DAL;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace BLL
{
    public class BLLBase<T> where T : class, new()
    {
        private readonly DALCommon<T> dal = new DALCommon<T>();
        /// <summary>
        /// 通用分页处理
        /// </summary>
        /// <param name="tblName">表或试图名称</param>
        /// <param name="strGetFields">查询字段</param>
        /// <param name="fldName">排序的字段名</param>
        /// <param name="PageSize">页尺寸</param>
        /// <param name="PageIndex">页码</param>
        /// <param name="OrderType">排序类型, 非 0 值则降序</param>
        /// <param name="strWhere">查询条件</param>
        /// <returns></returns>
        public int GetCount(string tableName, string strwhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from ");
            strSql.Append(tableName);
            strSql.Append(" where ");
            strSql.Append(strwhere);
            int i = DBHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), null);
            return i;
        }
        /// <summary>
        /// 通用分页处理
        /// </summary>
        /// <param name="tblName">表或试图名称</param>
        /// <param name="strGetFields">查询字段</param>
        /// <param name="fldName">排序的字段名</param>
        /// <param name="PageSize">页尺寸</param>
        /// <param name="PageIndex">页码</param>
        /// <param name="OrderType">排序类型, 非 0 值则降序</param>
        /// <param name="strWhere">查询条件</param>
        /// <returns></returns>
        public DataTable GetListByPage(string tblName, string strGetFields, string fldName, int PageSize, int PageIndex, string strWhere)
        {
            return dal.GetListByPage(tblName, strGetFields, fldName, PageSize, PageIndex, strWhere);
        }
        /// <summary>
        /// 通用分页处理
        /// </summary>
        /// <param name="tblName">表或试图名称</param>
        /// <param name="strGetFields">查询字段</param>
        /// <param name="fldName">排序的字段名</param>
        /// <param name="PageSize">页尺寸</param>
        /// <param name="PageIndex">页码</param>
        /// <param name="OrderType">排序类型, 非 0 值则降序</param>
        /// <param name="strWhere">查询条件</param>
        /// <returns></returns>
        public DataTable GetListByPage(string tblName, string strGetFields, string fldName, int PageSize, int PageIndex, string strWhere, string distinct)
        {
            return dal.GetListByPage(tblName, strGetFields, fldName, PageSize, PageIndex, strWhere, distinct);
        }
        /// <summary>
        /// 是否存在该记录
        /// <param name="field">字段</param>
        /// <param name="ID">要查询的值</param>
        /// </summary>
        public bool Exists(string field, object ID)
        {
            return dal.Exists(field, ID);
        }
        /// <summary>
        /// 是否存在该记录
        /// <param name="strwhere">条件(不需要写where,需要做安全性处理)</param>
        /// </summary>
        public bool Exists(string strwhere)
        {
            return dal.Exists(strwhere);
        }
        /// <summary>
        /// 新增一条记录
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public virtual bool Add(T model)
        {
            return dal.Add(model);
        }


        public virtual bool GetExecSql(string sql)
        {
            return dal.GetExecSql(sql);
        }
        /// <summary>
        /// 增加一条数据   去标识列
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <param name="field">标识列名</param>
        /// <returns></returns>
        public virtual int Add(T model, string field)
        {
            return dal.Add(model, field);
        }
        /// <summary>
        /// 增加一条数据,去标识列，返回标识列数据
        /// </summary>
        public object AddIdentity(T model, string name)
        {
            return dal.AddIdentity(model, name);
        }
        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual bool Update(string[] arr, T model)
        {
            return dal.Update(arr, model);
        }
        /// <summary>
        /// 按条件修改某些字段的信息
        /// </summary>
        /// <param name="values">字段设置值  如：active=1,name='123'</param>
        /// <param name="where">条件语句(不需加where)</param>
        /// <returns>执行是否成功</returns>
        public bool Update(string values, string where)
        {
            return dal.Update(values, where);
        }
        /// <summary>
        /// 根据ID删除一条记录
        /// </summary>
        /// <param name="field">列</param>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public virtual bool Delete(string field, object id)
        {
            return dal.Delete(field, id);
        }
        /// <summary>
        /// 输入条件删除数据，需要加and
        /// </summary>
        /// <param name="strwhere">条件</param>
        /// <returns></returns>
        public virtual bool Delete(string strwhere)
        {
            return dal.Delete(strwhere);
        }
        /// <summary>
        /// 根据ID 获取实体
        /// </summary>
        /// <param name="field">主键列名称</param>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public virtual T SelectSingle(string field, object id)
        {
            return dal.GetModel(field, id);
        }
        /// <summary>
        /// 添加一条数据ID 同时提交一条sql语句
        /// </summary>
        /// <param name="field">主键列名称</param>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public virtual int Add_R_Id_One(SqlParameter[] parameters, StringBuilder strSql, string Nextsql)
        {
            return dal.Add_R_Id_One(parameters, strSql, Nextsql);
        }
        /// <summary>
        ///  同时提交多条sql语句
        /// </summary>x
        /// <param name="Sql_Str_One"></param>
        /// <param name="Sql_Str_Two"></param>
        /// <returns></returns>
        public virtual bool Sql_D(string Sql_Str)
        {
            return dal.Sql_D(Sql_Str);
        }
        /// <summary>
        /// 添加一条数据ID 同时提交多条sql语句
        /// </summary>
        /// <param name="field">主键列名称</param>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public virtual int Add_R_Id_(SqlParameter[] parameters, StringBuilder strSql, string Nextsql)
        {
            return dal.Add_R_Id_D(parameters, strSql, Nextsql);
        }
        /// <summary>
        /// 添加一条数据ID 
        /// </summary>
        /// <returns></returns>
        public virtual int Add_R_(SqlParameter[] parameters, StringBuilder strSql, string Nextsql)
        {
            return dal.Add_R_(parameters, strSql, Nextsql);
        }
        /// <summary>
        /// 按条件 获取实体
        /// </summary>
        /// <param name="where">条件语句</param>
        /// <returns></returns>
        public virtual T SelectSingle(string where)
        {
            return dal.GetModel(where);
        }
        /// <summary>
        /// 按条件查询结果集
        /// </summary>
        /// <param name="top">前几条</param>
        /// <param name="strWhere">条件</param>
        /// <param name="strOrder">排序</param>
        /// <returns>返回List类型的结果集</returns>
        public virtual List<T> SelectList(int? top, string strWhere, string strOrder)
        {
            return dal.GetList(top, "*", strWhere, strOrder);
        }
        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="dt">待插入数据</param>
        /// <param name="tablename">目标数据表名</param>
        /// <returns></returns>
        public virtual bool SqlBulkCopy(DataTable dt, string tablename)
        {
            return dal.SqlBulkCopy(dt, tablename);
        }
        ///// <summary>
        ///// 按条件查询结果集
        ///// </summary>
        ///// <param name="top">前几条</param>
        ///// <param name="field">列</param>
        ///// <param name="tname">表名</param>
        ///// <param name="strWhere">条件</param>
        ///// <param name="strOrder">排序</param>
        ///// <returns>返回DataTable类型的结果集</returns>
        //public virtual DataTable SelectListToDataTable(int? top, string field, string tname, string strWhere, string strOrder)
        //{
        //    return dal.(top, field, tname, strWhere, strOrder);
        //}
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="top"></param>
        /// <param name="fields"></param>
        /// <param name="strWhere"></param>
        /// <param name="strOrder"></param>
        /// <returns></returns>
        public virtual DataTable GetDatatable(int? top, string fields, string strWhere, string strOrder)
        {
            return dal.GetDatatable(top, fields, strWhere, strOrder);
        }

        public virtual DataTable GetDatatable(string sql)
        {
            return dal.GetDatatable(sql);
        }
        /// <summary>
        ///  存储过程分页
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="pagesize">每页多少条记录</param>
        /// <param name="pageindex">指定当前为第几页</param>
        /// <param name="totalcount">返回总记录数</param>
        /// <returns></returns>
        public virtual DataTable GetDataTable_Pro(string where, int pagesize, int pageindex, out int totalcount, string orderby)
        {
            return dal.GetDataTable_Pro(where, pagesize, pageindex, out totalcount, orderby);
        }
        /// <summary>
        /// 存储过程获取分页list集合
        /// </summary>
        /// <param name="where"></param>
        /// <param name="pagesize"></param>
        /// <param name="pageindex"></param>
        /// <param name="totalcount"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public List<T> GetList_Pro(string where, int pagesize, int pageindex, out int totalcount, string orderby)
        {
            return dal.GetList_Pro(where, pagesize, pageindex, out totalcount, orderby);
        }
        /// <summary>
        /// 获取opition集合
        /// </summary>
        /// <param name="type"></param>
        /// <param name="where"></param>
        /// <param name="selectid"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Opitions(int type, string where, int selectid, string name, string value)
        {
            DataTable dt = dal.GetDatatable(null, "*", where, "");

            StringBuilder str = new StringBuilder();
            if (type == 0)
            {
                str.Append("<option value='0'>所有</option>");
            }
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    str.AppendFormat("<option value='{0}' {1}>{2}</option>", row[value], row[value].ToString() == selectid.ToString() ? "selected='selected'" : "", row[name]);
                }
            }
            return str.ToString();
        }
    }

}
