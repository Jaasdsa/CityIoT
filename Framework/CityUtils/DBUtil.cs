using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace CityUtils 
{
    public class DBCommandParam
    {
        public string Name;
        public object Value;
    }

    public class DBUtil
    {
        //使用该工具类先把数据库字符串通过配置文件获取到
        public static void InstanceDBStr(string projectConfig, out string errMsg)
        {
            errMsg = "";
            if (!string.IsNullOrWhiteSpace(dbConnectString))
                return;
            dbConnectString=GetConnectStr(projectConfig,out errMsg);
        }
        public static void InstanceDBStr(string connectStr)
        {
            dbConnectString = connectStr;
        }

        public static string dbConnectString;
        private static string GetConnectStr(string projectConfig,out string errMsg)
        {
            errMsg = "";
            // 由于静态变量的属性，只有启动第一次时才会读取文件，不会每次都读
            string path = projectConfig;
            XmlDocument doc = new XmlDocument();

            if (!File.Exists(path))
            {
                errMsg="数据库配置文件不存在：" + path;
                return "";
            }

            try
            {
                doc.Load(path);

                //得到连接字符串节点
                XmlNode connStrNode = doc.SelectSingleNode("service/connStr");
                if (connStrNode == null)
                {
                    errMsg="配置文件缺失数据库连接字符串节点";
                    return "";
                }
                string ip = connStrNode.Attributes["ip"].Value;
                string user = connStrNode.Attributes["user"].Value;
                string password = connStrNode.Attributes["password"].Value;
                string serverName = connStrNode.Attributes["serverName"].Value;

                return string.Format("server={0};database='{1}';User id={2};password={3};Integrated Security=false", ip, serverName, user, password);
            }
            catch (Exception e)
            {
                errMsg="数据库配置文件加载失败，文件是：" + path + "，错误信息是：" + e.Message;
                return "";
            }
        }

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
                using (SqlConnection connection = new SqlConnection(dbConnectString))
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
                using (SqlConnection conn = new SqlConnection(dbConnectString))
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
                using (SqlConnection conn = new SqlConnection(dbConnectString))
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
                using (SqlConnection conn = new SqlConnection(dbConnectString))
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
                using (SqlConnection conn = new SqlConnection(dbConnectString))
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
                using (SqlConnection conn = new SqlConnection(dbConnectString))
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
                DbConnection conn = new SqlConnection(dbConnectString);
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
        public static bool GetConnectionTest()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnectString))
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

        public static List<string> GetDataBaseList(string ip, string userName, string password, out string errMsg)
        {
            List<string> DBList = new List<string>();

            try
            {
                string connString = string.Format("server={0};User id={1};password={2};Integrated Security=false", ip, userName, password);

                DataTable dt = ExecuteDataTable("select name from dbo.sysdatabases order by name", connString, out errMsg);

                if (!string.IsNullOrWhiteSpace(errMsg))
                {
                    return DBList;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    string name = dr["name"].ToString();
                    if (name != "master" && name != "model" && name != "msdb" && name != "tempdb")
                    {
                        DBList.Add(name);
                    }
                }
            }
            catch(Exception e)
            {
                errMsg = e.Message;
                return DBList;
            }
            
            return DBList;
        }
    }


}
