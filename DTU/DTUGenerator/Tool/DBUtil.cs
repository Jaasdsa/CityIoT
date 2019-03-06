using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DTUGenerator
{
    public class DBCommandParam
    {
        public string Name;
        public object Value;
    }

    public class DBUtil
    {
        /// <summary>
        /// 测试数据库连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="dbName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool TestDBConnection(string ip, string dbName, string userName, string password)
        {
            DataTable dt = new DataTable();
            try
            {
                ip = ip.Trim();
                dbName = dbName.Trim();
                userName = userName.Trim();
                password = password.Trim();

                using (SqlConnection connection = new SqlConnection("server=" + ip + ";database='" + dbName + "';User id=" + userName + ";password=" + password + ";Integrated Security=false"))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT Name FROM SysObjects Where XType='U' ORDER BY Name";
                        command.CommandTimeout = 10;

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dt);

                        command.Dispose();
                    }

                    connection.Close();
                    connection.Dispose();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
        public static bool TestDBConnection()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(Config.DBConnectString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT Name FROM SysObjects Where XType='U' ORDER BY Name";
                        command.CommandTimeout = 10;

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dt);

                        command.Dispose();
                    }

                    connection.Close();
                    connection.Dispose();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///  查询转换为表格Table
        /// </summary>
        public static DataTable ExecuteDataTable(string sql, out string err)
        {
            err = "";

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(Config.DBConnectString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                return dt;
            }
            catch (Exception e)
            {
                err = sql + "执行错误：" + e.Message;
                return new DataTable();
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响行数
        /// </summary>
        public static int ExecuteNonQuery(string sql, string connString, out string err)
        {
            err = "";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();                    
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                err = sql + "执行错误：" + e.Message;
                return -1;
            }
        }
        public static int ExecuteNonQuery(string sql, out string err)
        {
            err = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.DBConnectString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                err = sql + "执行错误：" + e.Message;
                return -1;
            }
        }
        public static int ExecuteNonQuery(string sql, DBCommandParam[] dbParams, out string err)
        {
            err = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.DBConnectString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        foreach (DBCommandParam dbParam in dbParams)
                            command.Parameters.AddWithValue(dbParam.Name, dbParam.Value);

                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                err = sql + "执行错误：" + e.Message;
                return -1;
            }
        }
        /// <summary>
        /// 执行SQL语句，返回DataTable数据
        /// </summary>
        public static DataTable ExecuteDataTable(string sql, string connString, out string err)
        {
            err = "";

            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                return dt;
            }
            catch (Exception e)
            {
                err = sql + "执行错误：" + e.Message;
                return new DataTable();
            }
        }

        /// <summary>
        /// 是否存在表
        /// </summary>
        public static bool ExistTable(string TableName, out string err)
        {
            string GetExistTableSql(string table)
            {
                return "select count(name) from sysobjects where name = '" + TableName + "'";
            }

            string strSql = GetExistTableSql(TableName);
            try
            {
                int nCount = Convert.ToInt32(ExecuteScalar(strSql, out err));
                if (err == "")
                {
                    return nCount > 0;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回查询值
        public static object ExecuteScalar(string sql, string connString, out string err)
        {
            err = "";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        return command.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                err = sql + "执行错误：" + e.Message;
                return -1;
            }
        }

        /// <summary>
        /// 查询多张表，返回dataSet
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="connString"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string strSQL, string connString, out string err)
        {
            err = "";
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(strSQL, conn);
                    adp.Fill(ds);
                    return ds;
                }
            }
            catch (Exception e)
            {
                err = strSQL + "执行错误：" + e.Message;
                return new DataSet();
            }
        }
        public static DataSet GetDataSet(string strSQL, out string err)
        {
            err = "";
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.DBConnectString))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(strSQL, conn);
                    adp.Fill(ds);
                    return ds;
                }
            }
            catch (Exception e)
            {
                err = strSQL + "执行错误：" + e.Message;
                return new DataSet();
            }
        }

        /// <summary>
        ///  查询首行首列
        /// </summary>
        public static object ExecuteScalar(string sql, out string err)
        {
            err = "";

            try
            {
                using (SqlConnection conn = new SqlConnection(Config.DBConnectString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        return command.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                err = sql + "执行错误：" + e.Message;
                return -1;
            }
        }

        /// <summary>
        /// 传一个事务方法过来，让数据执行完后再关掉链接
        /// </summary>
        /// <param name="action"></param>
        /// <param name="obj"></param>
        /// <param name="errMsg"></param>
        public static void ExecuteBusiness(Action<object> action,object obj, out string errMsg)
        {
            errMsg = "";
            try
            {
                //由于需要插入多表，故用原生ADO配上事务所。所有SQL干完在关掉数据库连接并提交事务
                // 事务操作数据库
                DbConnection conn = new SqlConnection(Config.DBConnectString);
                conn.Open();
                DbCommand cmd = conn.CreateCommand();
                DbTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                cmd.Transaction = trans;
                try
                {
                    action(obj);
                }
                catch (Exception e)
                {
                    errMsg = "事务执行失败" + e.Message;
                }
                finally
                {
                    //手动释放替代using
                    conn.Close();
                    conn.Dispose();
                    trans.Dispose();
                    cmd.Dispose();
                }
            }
            catch (Exception e)
            {
                errMsg = "打开数据库失败" + e.Message;
            }
        }

        /// <summary>
        /// 测试数据链接
        /// </summary>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static bool GetConnectionTest(string connectStr)
        {

            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectStr))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT Name FROM SysObjects Where XType='U' ORDER BY Name";
                        command.CommandTimeout = 10;

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dt);

                        command.Dispose();
                    }

                    connection.Close();
                    connection.Dispose();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }


}
