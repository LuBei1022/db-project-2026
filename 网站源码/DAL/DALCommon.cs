using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace DAL
{
    public class DALCommon<T> where T : class, new()
    {
        #region Main Part
        /// <summary>
        /// 格式化SQL字符串
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string ToSetString(string name)
        {
            string formate = "{0}=@{1}";
            return string.Format(formate, name, name);
        }

        public List<T> ToModel(SqlDataReader reader)
        {
            Dictionary<string, PropertyInfo> ps = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo p in typeof(T).GetProperties())
            {
                ps.Add(p.Name, p);
            }

            List<T> ms = new List<T>();
            while (reader.Read())
            {
                T m = new T();
                object[] objs = new object[reader.FieldCount];
                int count = reader.GetValues(objs);
                int i = 0;
                foreach (object o in objs)
                {
                    if (o != DBNull.Value)
                        ps[reader.GetName(i)].SetValue(m, o, null);
                    i++;
                }
                ms.Add(m);
            }
            return ms;
        }
        public List<T> ToModel(DataTable dt)
        {
            List<T> list = new List<T>();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    T model = new T();
                    foreach (PropertyInfo p in typeof(T).GetProperties())
                    {
                        p.SetValue(model, row[p.Name], null);
                    }
                    list.Add(model);
                }
            }
            return list;
        }

        #region Properties
        //当前实体类型
        Type entityType;
        Type type
        {
            get
            {
                if (entityType == null)
                    entityType = typeof(T);
                return entityType;
            }
        }

        //实体的所有属性
        PropertyInfo[] propertyInfos;
        PropertyInfo[] pros
        {
            get
            {
                if (propertyInfos == null) propertyInfos = this.type.GetProperties();
                return propertyInfos;
            }
        }

        //相对应的表名
        string _tableName = string.Empty;
        string tableName
        {
            get
            {
                if (_tableName == string.Empty)
                {
                    string className = typeof(T).Name;

                    switch (className)
                    {
                        case "Users": _tableName = "[Users]"; break;
                        case "Role_Node_Permissions":
                            _tableName = "PE_" + className; break;
                        default:
                            _tableName = className; break;
                    }
                }
                return _tableName;
            }
        }
        #endregion
        #endregion


        #region  成员方法

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
            return DBHelper.GetListByPage(tblName, strGetFields, fldName, PageSize, PageIndex, strWhere);
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
            return DBHelper.GetListByPage(tblName, strGetFields, fldName, PageSize, PageIndex, strWhere, distinct);
        }
        /// <summary>
        /// 是否存在该记录
        /// <param name="field">字段</param>
        /// <param name="ID">要查询的值</param>
        /// </summary>
        public bool Exists(string field, object ID)
        {
            bool flag = false;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from ");
            strSql.Append(tableName);
            strSql.Append(" where ");
            strSql.Append(ToSetString(field));
            int i = DBHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), new SqlParameter("@" + field, ID));
            if (i > 0)
                flag = true;
            return flag;
        }

        /// <summary>
        ///添加一条数据ID 同时提交多条sql语句
        /// </summary>
        public int Add_R_Id_D(SqlParameter[] parameters, StringBuilder strSql, string Nextsql)
        {

            return DBHelper.ExecuteScalar_d(CommandType.Text, strSql.ToString(), Nextsql, parameters);

        }
        /// <summary>
        ///添加一条数据ID 同时提交多条sql语句
        /// </summary>
        public int Add_R_(SqlParameter[] parameters, StringBuilder strSql, string Nextsql)
        {

            return DBHelper.ExecuteScalar_R(CommandType.Text, strSql.ToString(), Nextsql, parameters);

        }
        /// <summary>
        /// 添加一条数据ID 同时提交一条sql语句
        /// </summary>
        public int Add_R_Id_One(SqlParameter[] parameters, StringBuilder strSql, string Nextsql)
        {

            return DBHelper.ExecuteScalar_(CommandType.Text, strSql.ToString(), Nextsql, parameters);

        }

        /// <summary>
        ///  同时提交多条sql语句
        /// </summary>
        /// <param name="Sql_Str_One"></param>
        /// <param name="Sql_Str_Two"></param>
        ///  /// <param name="Sql_Str_Three"></param>
        /// <returns></returns>
        public bool Sql_D(string Sql_Str)
        {

            return DBHelper.ExecuteScalar_D(Sql_Str);
        }


        public bool SqlBulkCopy(DataTable dt)
        {
            return DBHelper.SqlBulkCopy(dt);
        }
        /// <summary>
        /// 是否存在该记录
        /// <param name="strwhere">条件(不需要写where,需要做安全性处理)</param>
        /// </summary>
        public bool Exists(string strwhere)
        {
            bool flag = false;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from ");
            strSql.Append(tableName);
            strSql.Append(" where ");
            strSql.Append(strwhere);
            //ImportDataLog.WriteLog(LogType.SQL,strSql.ToString());
            int i = DBHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), null);
            if (i > 0)
                flag = true;
            return flag;
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(T model)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("insert into ");
            strSql.Append(tableName);
            strSql.Append(" (");

            //生成XXXvalues(XXX)
            int i = 1;
            foreach (PropertyInfo p in pros)
            {
                strSql.Append(p.Name);
                if (i != pros.Length)
                    strSql.Append(",");
                i++;
            }
            strSql.Append(") values (");
            i = 1;
            foreach (PropertyInfo p in pros)
            {
                strSql.Append("@");
                strSql.Append(p.Name);
                if (i != pros.Length)
                    strSql.Append(",");
                i++;
            }
            strSql.Append(")");

            //生成sqlparameters
            List<SqlParameter> sqlpars = new List<SqlParameter>();
            foreach (PropertyInfo p in pros)
            {
                object obje = p.GetValue(model, null);
                if (obje == null)
                    obje = DBNull.Value;
                SqlParameter sp = new SqlParameter("@" + p.Name, obje);
                sqlpars.Add(sp);
            }
            int obj = DBHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), sqlpars.ToArray());
            if (obj > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 增加一条数据,去标识列
        /// </summary>
        public int Add(T model, string name)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder sbtemp = new StringBuilder();
            strSql.Append("insert into ");
            strSql.Append(tableName);
            strSql.Append(" (");

            //生成XXXvalues(XXX)
            int i = 1;
            foreach (PropertyInfo p in pros)
            {
                if (p.Name.ToLower() != name.ToLower())
                {
                    sbtemp.Append(p.Name);
                    if (i != pros.Length)
                        sbtemp.Append(",");
                }
                i++;
            }
            strSql.Append(sbtemp.ToString().TrimEnd(",".ToCharArray()));
            sbtemp.Length = 0;
            strSql.Append(") values (");
            i = 1;
            foreach (PropertyInfo p in pros)
            {
                if (p.Name.ToLower() != name.ToLower())
                {
                    sbtemp.Append("@");
                    sbtemp.Append(p.Name);
                    if (i != pros.Length)
                        sbtemp.Append(",");
                }
                i++;
            }
            strSql.Append(sbtemp.ToString().TrimEnd(",".ToCharArray()));
            strSql.Append(")");

            //生成sqlparameters
            List<SqlParameter> sqlpars = new List<SqlParameter>();
            foreach (PropertyInfo p in pros)
            {
                object obje = p.GetValue(model, null);
                if (obje == null)
                    obje = DBNull.Value;
                SqlParameter sp = new SqlParameter("@" + p.Name, obje);
                sqlpars.Add(sp);
            }
            int obj = DBHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), sqlpars.ToArray());
            return obj;
        }




        /// <summary>
        /// 执行sql语句没有返回值
        /// </summary>
        /// <param name="sql"></param>
        public bool GetExecSql(string sql)
        {
            return DBHelper.ExecuteNonQuery(CommandType.Text, sql, null) > 0;
        }



        /// <summary>
        /// 增加一条数据,去标识列，返回标识列数据
        /// </summary>
        public object AddIdentity(T model, string name)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder sbtemp = new StringBuilder();
            strSql.Append("insert into ");
            strSql.Append(tableName);
            strSql.Append(" (");

            //生成XXXvalues(XXX)
            int i = 1;
            foreach (PropertyInfo p in pros)
            {
                if (!p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    sbtemp.Append(p.Name);
                    if (i != pros.Length)
                        sbtemp.Append(",");
                }
                i++;
            }
            strSql.Append(sbtemp.ToString().TrimEnd(",".ToCharArray()));
            sbtemp.Length = 0;
            strSql.Append(") values (");
            i = 1;
            foreach (PropertyInfo p in pros)
            {
                if (!p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    sbtemp.Append("@");
                    sbtemp.Append(p.Name);
                    if (i != pros.Length)
                        sbtemp.Append(",");
                }
                i++;
            }
            strSql.Append(sbtemp.ToString().TrimEnd(",".ToCharArray()));
            strSql.Append(");SELECT SCOPE_IDENTITY()");

            //生成sqlparameters
            List<SqlParameter> sqlpars = new List<SqlParameter>();
            foreach (PropertyInfo p in pros)
            {
                object obje = p.GetValue(model, null);
                if (obje == null)
                    obje = DBNull.Value;
                SqlParameter sp = new SqlParameter("@" + p.Name, obje);
                sqlpars.Add(sp);
            }
            object obj = DBHelper.ExecuteScalarObject(CommandType.Text, strSql.ToString(), sqlpars.ToArray());
            return obj;
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="field">不更新的列</param>
        /// <param name="model">对象</param>
        /// <returns></returns>
        public bool Update(string[] field, T model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder sbtemp = new StringBuilder();
            List<string> ar = new List<string>(field);
            strSql.Append("update ");
            strSql.Append(tableName);
            strSql.Append(" set ");
            int i = 1;
            foreach (PropertyInfo p in pros)
            {
                if (!ar.Contains(p.Name))
                {
                    sbtemp.Append(ToSetString(p.Name));
                    if (i != pros.Length)
                        sbtemp.Append(",");
                }
                i++;
            }
            strSql.Append(sbtemp.ToString().TrimEnd(",".ToCharArray()));
            strSql.Append(" where ");
            for (int j = 0; j < field.Length; j++)
            {
                if (j != 0)
                    strSql.Append(" and ");
                strSql.Append(ToSetString(field[j]));
            }
            //生成sqlparameters
            List<SqlParameter> sqlpars = new List<SqlParameter>();
            foreach (PropertyInfo p in pros)
            {
                object obje = p.GetValue(model, null);
                if (obje == null)
                    obje = DBNull.Value;
                SqlParameter sp = new SqlParameter("@" + p.Name, obje);
                sqlpars.Add(sp);
            }
            return DBHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), sqlpars.ToArray()) > 0;
        }
        /// <summary>
        /// 按条件修改某些字段的信息
        /// </summary>
        /// <param name="values">字段设置值  如：active=1,name='123'</param>
        /// <param name="where">条件语句(不需加where)</param>
        /// <returns>执行是否成功</returns>
        public bool Update(string field, string where)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update ");
            strSql.Append(tableName);
            strSql.Append(" set ");
            strSql.Append(field);
            strSql.Append(" where ");
            strSql.Append(where);
            return DBHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), null) > 0;
        }
        /// <summary>
        /// 删除一条数据
        /// <param name="field">字段名称</param>
        /// <param name="ID">字段值</param>
        /// </summary>
        public bool Delete(string field, object ID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from ");
            strSql.Append(tableName);
            strSql.Append(" where ");
            strSql.Append(ToSetString(field));
            return DBHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), new SqlParameter("@" + field, ID)) > 0;
        }
        /// <summary>
        /// 按条件删除记录
        /// </summary>
        /// <param name="strWhere">查询条件(无需写where)</param>
        /// <returns></returns>
        public bool Delete(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from ");
            strSql.Append(tableName);
            strSql.Append(" where ");
            strSql.Append(strWhere);
            return DBHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), null) > 0;
        }


        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="dt">待插入数据</param>
        /// <param name="tablename">目标数据表名</param>
        /// <returns></returns>
        public bool SqlBulkCopy(DataTable dt, string tablename)
        {
            return DBHelper.SqlBulkCopy(dt, tablename);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public T GetModel(string field, object ID)
        {
#pragma warning disable CS0219 // 变量已被赋值，但从未使用过它的值
            T t = default(T);
#pragma warning restore CS0219 // 变量已被赋值，但从未使用过它的值
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 ");
            int i = 1;
            foreach (PropertyInfo p in pros)
            {
                strSql.Append(p.Name);
                if (i != pros.Length)
                    strSql.Append(",");
                i++;
            }
            strSql.Append(" from " + tableName);
            strSql.Append(" where  ");
            strSql.Append(ToSetString(field));
            List<T> ms;
            using (SqlDataReader reader = DBHelper.ExecuteReader(CommandType.Text, strSql.ToString(), new SqlParameter("@" + field, ID)))
            {
                ms = ToModel(reader);
                reader.Close();
            }
            if (ms.Count > 0) return ms[0];
            else return default(T);
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public T GetModel(string where)
        {
#pragma warning disable CS0219 // 变量已被赋值，但从未使用过它的值
            T t = default(T);
#pragma warning restore CS0219 // 变量已被赋值，但从未使用过它的值
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 ");
            int i = 1;
            foreach (PropertyInfo p in pros)
            {
                strSql.Append(p.Name);
                if (i != pros.Length)
                    strSql.Append(",");
                i++;
            }
            strSql.Append(" from " + tableName);
            strSql.Append(" where  ");
            strSql.Append(where);
            List<T> ms;
            using (SqlDataReader reader = DBHelper.ExecuteReader(CommandType.Text, strSql.ToString(), null))
            {
                ms = ToModel(reader);
                reader.Close();
            }
            if (ms.Count > 0) return ms[0];
            else return default(T);
        }
        /// <summary>
        /// 获得数据列表
        /// <param name="top">几条数据</param>
        /// <param name="field">列字段</param>
        /// <param name="strWhere">条件(不需加where)</param>
        /// <param name="strOrder">排序</param>
        /// </summary>
        public List<T> GetList(int? top, string field, string strWhere, string strOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (top.HasValue)
                strSql.Append(" top " + top.ToString() + " ");
            strSql.Append(field);
            strSql.Append(" FROM ");
            strSql.Append(tableName);
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            if (strOrder.Trim().Length > 0)
                strSql.Append(" order by " + strOrder);
            List<T> ms;
            using (SqlDataReader reader = DBHelper.ExecuteReader(CommandType.Text, strSql.ToString(), null))
            {
                ms = ToModel(reader);
                reader.Close();
            }
            if (ms.Count > 0) return ms;
            else return new List<T>();
        }



        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="top"></param>
        /// <param name="fields"></param>
        /// <param name="strWhere"></param>
        /// <param name="strOrder"></param>
        /// <returns></returns>
        public DataTable GetDatatable(int? top, string fields, string strWhere, string strOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (top.HasValue)
                strSql.Append(" top " + top.ToString() + " ");
            strSql.Append(fields);
            strSql.Append(" FROM ");
            strSql.Append(tableName);
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            if (strOrder.Trim().Length > 0)
                strSql.Append(" order by " + strOrder);
            return DBHelper.GetDataTable(CommandType.Text, strSql.ToString(), null);
        }
        public DataTable GetDatatable(string sql)
        {
            return DBHelper.GetDataTable(CommandType.Text, sql, null);
        }
        /// <summary>
        ///  存储过程分页
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="pagesize">每页多少条记录</param>
        /// <param name="pageindex">指定当前为第几页</param>
        /// <param name="totalcount">返回总记录数</param>
        /// <returns></returns>
        public DataTable GetDataTable_Pro(string where, int pagesize, int pageindex, out int totalcount, string orderby)
        {
            //@TableName varchar(50),            --表名
            //@ReFieldsStr varchar(200) = '*',   --字段名(全部字段为*)
            //@OrderString varchar(200),         --排序字段(必须!支持多字段不用加order by)
            //@WhereString varchar(500) =N'',  --条件语句(不用加where)
            //@PageSize int,                     --每页多少条记录
            //@PageIndex int = 1 ,               --指定当前为第几页
            //@TotalRecord int output            --返回总记录数
            SqlParameter[] listp = {
                                       new SqlParameter("@TableName",SqlDbType.NVarChar,500),
                                       new SqlParameter("@ReFieldsStr",SqlDbType.NVarChar,500),
                                       new SqlParameter("@OrderString",SqlDbType.NVarChar,500),
                                       new SqlParameter("@WhereString",SqlDbType.NVarChar,500),

                                       new SqlParameter("@PageSize",SqlDbType.Int,4),
                                       new SqlParameter("@PageIndex ",SqlDbType.Int,4),
                                       new SqlParameter("@TotalRecord",SqlDbType.Int,4)
                                   };
            listp[0].Value = tableName;
            listp[1].Value = "*";
            listp[2].Value = orderby; //"id desc";
            listp[3].Value = where;
            listp[4].Value = pagesize;
            listp[5].Value = pageindex;


            listp[6].Direction = ParameterDirection.Output;

            DataTable dt = DBHelper.RunProcedure("PROCE_SQL2005PAGECHANGE", listp).Tables[0];
            totalcount = int.Parse(listp[6].Value.ToString());
            return dt;
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
            DataTable dt = GetDataTable_Pro(where, pagesize, pageindex, out totalcount, orderby);
            return ToModel(dt);
        }
        #endregion  成员方法
    }
}
