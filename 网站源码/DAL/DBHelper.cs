using Common;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public static class DBHelper
    {
        // Deployments can override the checked-in development setting without changing application behavior.
        public static readonly string ConnectionString = ResolveConnectionString();

        private static string ResolveConnectionString()
        {
            string environmentValue = Environment.GetEnvironmentVariable("MANAGE_SQL_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(environmentValue))
            {
                return environmentValue;
            }

            return ConfigurationManager.ConnectionStrings["SQLCONNECTIONSTRING"].ConnectionString;
        }

        // 哈希表用来存储缓存的参数信息，哈希表可以存储任意类型的参数。
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        private static readonly string RETURNVALUE = "RETURNVALUE";
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">存储过程的名称</param>
        /// <param name="prams">存储过程所需参数</param>
        /// <param name="dataSet">返回DataSet对象</param>
        public static void RunProc(string procName, SqlParameter[] prams, ref DataSet dataSet)
        {
            if (dataSet == null)
            {
                dataSet = new DataSet();
            }
            ///创建SqlDataAdapter
            SqlDataAdapter da = CreateProcDataAdapter(procName, prams);

            ///读取数据
            da.Fill(dataSet);
            ///关闭数据库的连接
            //Close();
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
        public static DataTable GetListByPage(string tblName, string strGetFields, string fldName, int PageSize, int PageIndex, string strWhere)
        {
            string strsql = DBHelper.ConnectionString;//数据库链接字符串  
            string sql = "SP_PageList";//要调用的存储过程名  
            SqlConnection conStr = new SqlConnection(strsql);//SQL数据库连接对象，以数据库链接字符串为参数  
            SqlCommand comStr = new SqlCommand(sql, conStr);//SQL语句执行对象，第一个参数是要执行的语句，第二个是数据库连接对象  
            comStr.CommandType = CommandType.StoredProcedure;//因为要使用的是存储过程，所以设置执行类型为存储过程  
            comStr.CommandTimeout = 600;
            //依次设定存储过程的参数  
            comStr.Parameters.Add("@tblName", SqlDbType.VarChar).Value = tblName;
            comStr.Parameters.Add("@strGetFields", SqlDbType.VarChar).Value = strGetFields;
            comStr.Parameters.Add("@fldName", SqlDbType.VarChar).Value = fldName;
            comStr.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
            comStr.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
            comStr.Parameters.Add("@strWhere", SqlDbType.VarChar).Value = strWhere;
            comStr.Parameters.Add("@doCount", SqlDbType.Bit).Value = 0;

            conStr.Open();//打开数据库连接  
                          //  MessageBox.Show(comStr.ExecuteNonQuery().ToString());//执行存储过程  
            SqlDataAdapter SqlDataAdapter1 = new SqlDataAdapter(comStr);
            DataTable DT = new DataTable();
            SqlDataAdapter1.Fill(DT);
            conStr.Close();//关闭连接
            return DT;
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
        public static DataTable GetListByPage(string tblName, string strGetFields, string fldName, int PageSize, int PageIndex, string strWhere, string distinct)
        {
            string strsql = DBHelper.ConnectionString;//数据库链接字符串  
            string sql = "SP_PageList";//要调用的存储过程名  
            SqlConnection conStr = new SqlConnection(strsql);//SQL数据库连接对象，以数据库链接字符串为参数  
            SqlCommand comStr = new SqlCommand(sql, conStr);//SQL语句执行对象，第一个参数是要执行的语句，第二个是数据库连接对象  
            comStr.CommandType = CommandType.StoredProcedure;//因为要使用的是存储过程，所以设置执行类型为存储过程  
            comStr.CommandTimeout = 600;
            //依次设定存储过程的参数  
            comStr.Parameters.Add("@tblName", SqlDbType.VarChar).Value = tblName;
            comStr.Parameters.Add("@strGetFields", SqlDbType.VarChar).Value = strGetFields;
            comStr.Parameters.Add("@fldName", SqlDbType.VarChar).Value = fldName;
            comStr.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize;
            comStr.Parameters.Add("@PageIndex", SqlDbType.Int).Value = PageIndex;
            comStr.Parameters.Add("@strWhere", SqlDbType.VarChar).Value = strWhere;
            comStr.Parameters.Add("@distinct", SqlDbType.VarChar).Value = distinct;
            comStr.Parameters.Add("@doCount", SqlDbType.Bit).Value = 0;

            conStr.Open();//打开数据库连接  
                          //  MessageBox.Show(comStr.ExecuteNonQuery().ToString());//执行存储过程  
            SqlDataAdapter SqlDataAdapter1 = new SqlDataAdapter(comStr);
            DataTable DT = new DataTable();
            SqlDataAdapter1.Fill(DT);
            conStr.Close();//关闭连接
            return DT;
        }
        /// <summary>
        /// 创建一个SqlDataAdapter对象，用此来执行存储过程
        /// </summary>
        /// <param name="procName">存储过程的名称</param>
        /// <param name="prams">存储过程所需参数</param>
        /// <returns>返回SqlDataAdapter对象</returns>
        private static SqlDataAdapter CreateProcDataAdapter(string procName, SqlParameter[] prams)
        {
            SqlConnection conn = new SqlConnection(DBHelper.ConnectionString);
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dtSet = new DataSet();

            // 在这里使用try/catch处理是因为如果方法出现异常，则SqlDataReader就不存在，
            //CommandBehavior.CloseConnection的语句就不会执行，触发的异常由catch捕获。
            //关闭数据库连接，并通过throw再次引发捕捉到的异常。
            try
            {
                dataAdapter = new SqlDataAdapter(procName, conn);
                dataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                ///添加把存储过程的参数
                if (prams != null)
                {
                    foreach (SqlParameter parameter in prams)
                    {
                        dataAdapter.SelectCommand.Parameters.Add(parameter);
                    }
                }

                /////添加返回参数ReturnValue
                dataAdapter.SelectCommand.Parameters.Add(
                    new SqlParameter(RETURNVALUE, SqlDbType.Int, 4, ParameterDirection.ReturnValue,
                    false, 0, 0, string.Empty, DataRowVersion.Default, null));


                //PrepareCommand(cmd, conn, null, CommandType.StoredProcedure, procName, prams);

                ////设置dataAdapter的SelectCommand为cmd。cmd在PrepareCommand里面已经设置过了。
                //dataAdapter.SelectCommand = cmd;
                //dataAdapter.Fill(dtSet);
                //cmd.Parameters.Clear();

                ////关闭connection对象
                //conn.Close();
                //conn.Dispose();

                return dataAdapter;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        /// <summary>
        /// 传入输入参数
        /// </summary>
        /// <param name="ParamName">存储过程名称</param>
        /// <param name="DbType">参数类型</param></param>
        /// <param name="Size">参数大小</param>
        /// <param name="Value">参数值</param>
        /// <returns>新的parameter 对象</returns>
        public static SqlParameter CreateInParam(string ParamName, SqlDbType DbType, int Size, object Value)
        {
            return CreateParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }
        /// <summary>
        /// 生成存储过程参数
        /// </summary>
        /// <param name="ParamName">存储过程名称</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数大小</param>
        /// <param name="Direction">参数方向</param>
        /// <param name="Value">参数值</param>
        /// <returns>新的 parameter 对象</returns>
        public static SqlParameter CreateParam(string ParamName, SqlDbType DbType, Int32 Size, ParameterDirection Direction, object Value)
        {
            SqlParameter param;

            ///当参数大小为0时，不使用该参数大小值
            if (Size > 0)
            {
                param = new SqlParameter(ParamName, DbType, Size);
            }
            else
            {
                ///当参数大小为0时，不使用该参数大小值
                param = new SqlParameter(ParamName, DbType);
            }

            ///创建输出类型的参数
            param.Direction = Direction;
            if (!(Direction == ParameterDirection.Output && Value == null))
            {
                param.Value = Value;
            }

            ///返回创建的参数
            return param;
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="prams">存储过程所需参数</param>
        /// <returns>返回存储过程返回值</returns>
        public static int RunProc_Value(string procName, SqlParameter[] prams)
        {
            int count = 0;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 600;
            using (SqlConnection conn = new SqlConnection(DBHelper.ConnectionString))
            {
#pragma warning disable CS0168 // 声明了变量，但从未使用过
                try
                {
                    PrepareCommand(cmd, conn, null, CommandType.StoredProcedure, procName, prams);
                    cmd.ExecuteNonQuery();
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                    //清空SqlCommand中的参数列表
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                }
#pragma warning restore CS0168 // 声明了变量，但从未使用过

            }

            return count;
        }
        /// <summary>
        /// 创建一个SqlCommand对象以此来执行存储过程
        /// </summary>
        /// <param name="procName">存储过程的名称</param>
        /// <param name="prams">存储过程所需参数</param>
        /// <returns>返回SqlCommand对象</returns>
        private static SqlCommand CreateProcCommand(string procName, SqlParameter[] prams)
        {
            ///返回创建的SqlCommand对象
            SqlCommand cmd;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();//连接数据库

                cmd = new SqlCommand(procName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;
#pragma warning disable CS0168 // 声明了变量，但从未使用过
                try
                { ///添加把存储过程的参数
                    if (prams != null)
                    {
                        foreach (SqlParameter parameter in prams)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }
                    ///添加返回参数ReturnValue
                    cmd.Parameters.Add(
                        new SqlParameter(RETURNVALUE, SqlDbType.Int, 4, ParameterDirection.ReturnValue,
                        false, 0, 0, string.Empty, DataRowVersion.Default, null));
                }
                catch (Exception ex)
                {
                }
#pragma warning restore CS0168 // 声明了变量，但从未使用过
            }
            return cmd;
        }
        /// <summary>
        ///执行一个不需要返回值的SqlCommand命令，通过指定专用的连接字符串。
        /// 使用参数数组形式提供参数列表 
        /// </summary>
        /// <remarks>
        /// 使用示例：
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(DBHelper.ConnectionString))
            {
                //通过PrePareCommand方法将参数逐个加入到SqlCommand的参数集合中
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                cmd.ExecuteNonQuery();
                //清空SqlCommand中的参数列表
                cmd.Parameters.Clear();
                conn.Close();
                return 1;
            }
        }
        public static bool SqlBulkCopy(DataTable dt)
        {

            try
            {
                //数据批量导入sqlserver,创建实例     SqlBulkCopyOptions.UseInternalTransaction采用事务   复制失败自动回滚
                System.Data.SqlClient.SqlBulkCopy sqlbulk = new System.Data.SqlClient.SqlBulkCopy(ConnectionString, SqlBulkCopyOptions.UseInternalTransaction);
                //sqlbulk.SqlRowsCopied +=new SqlRowsCopiedEventHandler(OnRowsCopied); //订阅复制完成后的方法,参数是 sqlbulk.NotifyAfter的值
                //sqlbulk.NotifyAfter = dt.Rows.Count;

                //目标数据库表名
                sqlbulk.DestinationTableName = "XMLORDERLIST";
                //数据集字段索引与数据库字段索引映射
                sqlbulk.ColumnMappings.Add(0, "ID");
                sqlbulk.ColumnMappings.Add(1, "FIELD");
                sqlbulk.ColumnMappings.Add(2, "VALUE");
                sqlbulk.ColumnMappings.Add(3, "PID");
                sqlbulk.ColumnMappings.Add(4, "PNAME");
                sqlbulk.ColumnMappings.Add(5, "UID");
                //导入
                sqlbulk.WriteToServer(dt);
                sqlbulk.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                dt.Dispose();
            }
        }

        /// <summary>
        /// 执行一条不返回结果的SqlCommand，通过一个已经存在的数据库事物处理 
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例： 
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">一个存在的 sql 事物处理</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行一条返回结果集的SqlCommand命令，通过专用的连接字符串。
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例：  
        ///  SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个包含结果的SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(DBHelper.ConnectionString);

            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            // 在这里使用try/catch处理是因为如果方法出现异常，则SqlDataReader就不存在，
            //CommandBehavior.CloseConnection的语句就不会执行，触发的异常由catch捕获。
            //关闭数据库连接，并通过throw再次引发捕捉到的异常。
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();

                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// 执行一条返回结果集的SqlCommand命令，通过专用的连接字符串。
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例：  
        ///  DataSet dtSet = GetDataSet(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个包含结果的SqlDataReader</returns>
        public static DataSet GetDataSet(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(DBHelper.ConnectionString);
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dtSet = new DataSet();

            // 在这里使用try/catch处理是因为如果方法出现异常，则SqlDataReader就不存在，
            //CommandBehavior.CloseConnection的语句就不会执行，触发的异常由catch捕获。
            //关闭数据库连接，并通过throw再次引发捕捉到的异常。
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

                //设置dataAdapter的SelectCommand为cmd。cmd在PrepareCommand里面已经设置过了。
                dataAdapter.SelectCommand = cmd;
                dataAdapter.Fill(dtSet);
                cmd.Parameters.Clear();

                //关闭connection对象
                conn.Close();
                conn.Dispose();

                return dtSet;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        public static DataTable GetDataTable(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(DBHelper.ConnectionString);
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            //DataSet dtSet = new DataSet();

            DataTable dt = new DataTable();

            // 在这里使用try/catch处理是因为如果方法出现异常，则SqlDataReader就不存在，
            //CommandBehavior.CloseConnection的语句就不会执行，触发的异常由catch捕获。
            //关闭数据库连接，并通过throw再次引发捕捉到的异常。
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

                //设置dataAdapter的SelectCommand为cmd。cmd在PrepareCommand里面已经设置过了。
                dataAdapter.SelectCommand = cmd;
                //dataAdapter.Fill(dtSet);
                dataAdapter.Fill(dt);
                cmd.Parameters.Clear();

                //关闭connection对象
                conn.Close();
                conn.Dispose();

                //return dtSet.Tables[0];
                return dt;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// 执行一条返回结果集的SqlCommand命令，通过专用的连接字符串。
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例：  
        ///  DataSet dtSet = GetDataSet(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个包含结果的SqlDataReader</returns>
        public static DataTable GetDataTable(string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(DBHelper.ConnectionString);
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            //DataSet dtSet = new DataSet();

            DataTable dt = new DataTable();

            // 在这里使用try/catch处理是因为如果方法出现异常，则SqlDataReader就不存在，
            //CommandBehavior.CloseConnection的语句就不会执行，触发的异常由catch捕获。
            //关闭数据库连接，并通过throw再次引发捕捉到的异常。
            try
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, commandParameters);

                //设置dataAdapter的SelectCommand为cmd。cmd在PrepareCommand里面已经设置过了。
                dataAdapter.SelectCommand = cmd;
                //dataAdapter.Fill(dtSet);
                dataAdapter.Fill(dt);
                cmd.Parameters.Clear();

                //关闭connection对象
                conn.Close();
                conn.Dispose();

                //return dtSet.Tables[0];
                return dt;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }


        /// <summary>
        /// 执行一条返回第一条记录第一列的SqlCommand命令，通过专用的连接字符串。 
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例：  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个object类型的数据，可以通过 Convert.To{Type}方法转换类型</returns>
        public static Int32 ExecuteScalar(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(DBHelper.ConnectionString))
            {
                //通过PrePareCommand方法将参数逐个加入到SqlCommand的参数集合中
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                Int32 val = Convert.ToInt32(cmd.ExecuteScalar());

                //清空SqlCommand中的参数列表
                cmd.Parameters.Clear();
                return val;
            }
        }


        ///  同时提交多条sql语句
        /// </summary>
        /// <param name="Sql_Str_One"></param>
        /// <param name="Sql_Str_Two"></param>
        ///  /// <param name="Sql_Str_Three"></param>
        /// <returns></returns>
        public static bool ExecuteScalar_D(string Sql_Str)
        {
            bool isyes = false;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string datetime = DateTime.Now.ToString("yyyyMMddHHmmssffff_");
                conn.Open();//连接数据库
                SqlTransaction transaction;//开始一个本地事务
                transaction = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand("", conn, transaction);
#pragma warning disable CS0168 // 声明了变量，但从未使用过
                try
                {//向数据表中插入记录的命令语句

                    string[] Sql_ = Sql_Str.Split(new[] { "ξLiteratureManagerξ" }, StringSplitOptions.None);
                    foreach (string item in Sql_)
                    {
                        //ImportDataLog.WriteLog(LogType.Error, "datetime_item_"+ datetime + ":" + item);
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            cmd.CommandText = @"" + item;
                            int sql_num = cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();//提交事务
                    isyes = true;
                }
                catch (Exception ex)
                {
                    //ImportDataLog.WriteLog(LogType.Error, "ex_sql_"+ datetime + ":"+ Sql_Str + "\r\nex:" + ex);
                    isyes = false;
                    try
                    {
                        transaction.Rollback();//回滚事务
                    }
                    catch (Exception ex2)
                    {
                        isyes = false;
                        ImportDataLog.WriteLog(LogType.Error, "ex2:" + ex2);
                    }
                }
#pragma warning restore CS0168 // 声明了变量，但从未使用过
            }
            return isyes;
        }


        public static int ExecuteScalar_d(CommandType cmdType, string cmdText, string Nextsql, params SqlParameter[] commandParameters)
        {
            int isyes = 0;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();//连接数据库
                SqlTransaction transaction;//开始一个本地事务
                transaction = conn.BeginTransaction();//必须为SqlCommand指定数据库连接和登记的事务
                //或transaction = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand("", conn, transaction);
                try
                {
                    //向数据表中插入记录的命令语句
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);



                    Int32 val = Convert.ToInt32(cmd.ExecuteScalar());
                    if (!string.IsNullOrWhiteSpace(Nextsql))
                    {
                        string[] Nextsql_ = Nextsql.Split(new[] { "ξLiteratureManagerξ" }, StringSplitOptions.None);
                        foreach (string item in Nextsql_)
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                cmd.CommandText = @"" + item.Replace("LiteratureManagerteshu", val.ToString());
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();//提交事务
                    isyes = val;
                }
                catch (Exception ex)
                {
                    ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
                    isyes = 0;
#pragma warning disable CS0168 // 声明了变量，但从未使用过
                    try
                    {
                        transaction.Rollback();//回滚事务
                    }
                    catch (Exception ex2)
                    {
                        isyes = 0;
                    }
#pragma warning restore CS0168 // 声明了变量，但从未使用过
                }
            }

            return isyes;
        }


        public static int ExecuteScalar_(CommandType cmdType, string cmdText, string Nextsql, params SqlParameter[] commandParameters)
        {
            int isyes = 0;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();//连接数据库
                SqlTransaction transaction;//开始一个本地事务
                transaction = conn.BeginTransaction();//必须为SqlCommand指定数据库连接和登记的事务
                //或transaction = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand("", conn, transaction);
#pragma warning disable CS0168 // 声明了变量，但从未使用过
                try
                {
                    //向数据表中插入记录的命令语句
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);



                    Int32 val = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd.CommandText = @"" + Nextsql.Replace("LiteratureManagerteshu", val.ToString());
                    cmd.ExecuteNonQuery();
                    transaction.Commit();//提交事务
                    isyes = val;
                }
                catch (Exception ex)
                {
                    isyes = 0;
#pragma warning disable CS0168 // 声明了变量，但从未使用过
                    try
                    {
                        transaction.Rollback();//回滚事务
                    }
                    catch (Exception ex2)
                    {
                        isyes = 0;
                    }
#pragma warning restore CS0168 // 声明了变量，但从未使用过
                }
#pragma warning restore CS0168 // 声明了变量，但从未使用过
            }

            return isyes;
        }

        public static int ExecuteScalar_R(CommandType cmdType, string cmdText, string Nextsql, params SqlParameter[] commandParameters)
        {
            int isyes = 0;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();//连接数据库
                SqlTransaction transaction;//开始一个本地事务
                transaction = conn.BeginTransaction();//必须为SqlCommand指定数据库连接和登记的事务
                //或transaction = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand("", conn, transaction);
#pragma warning disable CS0168 // 声明了变量，但从未使用过
                try
                {
                    //向数据表中插入记录的命令语句
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    int R_int = cmd.ExecuteNonQuery();

                    if (!string.IsNullOrWhiteSpace(Nextsql))
                    {
                        string[] Nextsql_ = Nextsql.Split(new[] { "ξLiteratureManagerξ" }, StringSplitOptions.None);
                        foreach (string item in Nextsql_)
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                cmd.CommandText = @"" + item;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();//提交事务
                    isyes = R_int;
                }
                catch (Exception ex)
                {
                    isyes = 0;
#pragma warning disable CS0168 // 声明了变量，但从未使用过
                    try
                    {
                        transaction.Rollback();//回滚事务
                    }
                    catch (Exception ex2)
                    {
                        isyes = 0;
                    }
#pragma warning restore CS0168 // 声明了变量，但从未使用过
                }
#pragma warning restore CS0168 // 声明了变量，但从未使用过
            }

            return isyes;
        }
        /// <summary>
        /// 执行一条返回第一条记录第一列，返回一个Object类型的值
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static Object ExecuteScalarObject(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            Object obj = new Object();
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(DBHelper.ConnectionString))
            {
                //通过PrePareCommand方法将参数逐个加入到SqlCommand的参数集合中
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                obj = cmd.ExecuteScalar();
                //清空SqlCommand中的参数列表
                cmd.Parameters.Clear();
            }
            return obj;
        }
        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="dt">待插入数据</param>
        /// <param name="tablename">目标数据表名</param>
        /// <returns></returns>
        public static bool SqlBulkCopy(DataTable dt, string tablename)
        {
            bool flag = false;
            if (dt != null && dt.Rows.Count > 0)
            {
                using (SqlConnection conn = new SqlConnection(DBHelper.ConnectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    using (SqlBulkCopy bulk = new SqlBulkCopy(conn))
                    {
                        try
                        {
                            bulk.BatchSize = dt.Rows.Count;
                            bulk.DestinationTableName = tablename;
                            bulk.WriteToServer(dt);
                            bulk.Close();
                            flag = true;
                        }
                        catch (Exception ex)
                        {
                            conn.Close();
                            throw ex;
                        }
                    }
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
            return flag;
        }

        /// <summary>
        /// 缓存参数数组
        /// </summary>
        /// <param name="cacheKey">参数缓存的键值</param>
        /// <param name="cmdParms">被缓存的参数列表</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// 获取被缓存的参数
        /// </summary>
        /// <param name="cacheKey">用于查找参数的KEY值</param>
        /// <returns>返回缓存的参数数组</returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            //新建一个参数的克隆列表
            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

            //通过循环为克隆参数列表赋值
            for (int i = 0, j = cachedParms.Length; i < j; i++)
                //使用clone方法复制参数列表中的参数
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        /// <summary>
        /// 为执行命令准备参数
        /// </summary>
        /// <param name="cmd">SqlCommand 命令</param>
        /// <param name="conn">已经存在的数据库连接</param>
        /// <param name="trans">数据库事物处理</param>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">Command text，T-SQL语句 例如 Select * from Products</param>
        /// <param name="cmdParms">返回带参数的命令</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {

            //判断数据库连接状态
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            //判断是否需要事物处理
            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
        }
        /// <summary>
        /// 存储过程调用
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataSet RunProcedure(string storedProcName, SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet);
                connection.Close();
                return dataSet;
            }
        }


        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, SqlParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
            {
                foreach (SqlParameter pa in parameters)
                {
                    command.Parameters.Add(pa);
                }


            }
            return command;
        }
    }
}
