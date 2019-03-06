using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.IO;
using System.Collections;

namespace CityUtils 
{
    public class SQLiteHelper
    {
        protected string connectionString = "";
        public SQLiteHelper()
        {

        }

        public SQLiteHelper(string DataSource)
        {
            connectionString = "Data Source=" + DataSource + ";Pooling=true;FailIfMissing=false";
        }

        public SQLiteHelper(string DataSource, string Password)
        {
            connectionString = "Data Source=" + DataSource + ";Password=" + Password
                        + ";Pooling=true;FailIfMissing=false";
        }

        private static readonly object obj = new object();

        #region database

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="FilePath">数据库路径</param>
        /// <returns>是否创建成功</returns>
        public static bool CreateDB(string FilePath)
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }
                // 判断目录是否存在
                string fulllName = new DirectoryInfo(FilePath).Parent.FullName;  //创建文件的上一级目录
                if (false == System.IO.Directory.Exists(fulllName))
                {
                    //创建文件夹
                    System.IO.Directory.CreateDirectory(fulllName);
                }
                SQLiteConnection.CreateFile(FilePath);
                return true;
            }
            catch(Exception e)
            {
                //SystemError.CreateErrorLog(DateTime.Now.ToString() + "\n" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace + "\n\n");
                return false;
            }
        }

        /// <summary>
        /// 创建有密码的数据库
        /// </summary>
        /// <param name="FilePath">数据库路径</param>
        /// <param name="Password">密码</param>
        /// <returns>是否创建成功</returns>
        public static bool CreateDB(string FilePath, string Password)
        {
            try
            {
                string strPath = FilePath.Substring(0, FilePath.LastIndexOf('\\'));
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }
                SQLiteConnection.CreateFile(FilePath);

                string strConn = "Data Source=" + FilePath + ";Pooling=true;FailIfMissing=false";
                SQLiteConnection conn = new SQLiteConnection(strConn);
                conn.Open();
                conn.ChangePassword(Password);
                conn.Close();
                return true;
            }
            catch
            {
                //SystemError.CreateErrorLog(DateTime.Now.ToString() + "\n" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace + "\n\n");
                return false;
            }
        }

        /// <summary>
        /// 修改数据库密码
        /// </summary>
        /// <param name="FilePath">数据库路径</param>
        /// <param name="OldPwd">原密码</param>
        /// <param name="NewPwd">新密码</param>
        /// <returns>是否修改成功</returns>
        public static bool ChangePassword(string FilePath, string OldPwd, string NewPwd)
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    //数据库不存在，则新建
                    return CreateDB(FilePath, NewPwd);
                }

                string strConn = "Data Source=" + FilePath + ";Password=" + OldPwd
                        + ";Pooling=true;FailIfMissing=false";
                SQLiteConnection conn = new SQLiteConnection(strConn);
                conn.Open();
                conn.ChangePassword(NewPwd);
                conn.Close();
                return true;
            }
            catch
            {
                //SystemError.CreateErrorLog(DateTime.Now.ToString() + "\n" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace + "\n\n");
                return false;
            }
        }

        public static SQLiteHelper GetInstance(string FilePath, string Password)
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    //数据库不存在，则新建
                    if (!CreateDB(FilePath, Password))
                    {
                        throw new Exception("创建数据库失败");
                    }
                }

                return new SQLiteHelper(FilePath, Password);
            }
            catch
            {
                //SystemError.CreateErrorLog(DateTime.Now.ToString() + "\n" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace + "\n\n");
                return null;
            }
        }
        #endregion

        #region 公用方法

        public int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ") from " + TableName;
            object obj = ExecuteScalar(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }

        public bool Exists(string strSql, params SQLiteParameter[] cmdParms)
        {
            object obj = ParameterExecuteScalar(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool ExistTable(string TableName)
        {
            string strSql = "select count(*) from sqlite_master where type = 'table' and name = '" + TableName + "'";
            object obj = ExecuteScalar(strSql);
            if (obj == null)
            {
                return false;
            }
            else
            {
                return int.Parse(obj.ToString()) > 0;
            }
        }
        #endregion

        #region 执行简单SQL语句

        /// <summary>
        /// 执行SQL语句,返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteNonQuery(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();

                        int rows = cmd.ExecuteNonQuery();

                        connection.Close();
                        SQLiteConnection.ClearAllPools();

                        return rows;
                    }
                    catch (SQLiteException E)
                    {
                        connection.Close();
                        SQLiteConnection.ClearAllPools();

                        throw new Exception(E.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句,实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param> 
        public void ExecuteSqlTran(ArrayList SQLStringList)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = conn;
                SQLiteTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();

                    conn.Close();
                    SQLiteConnection.ClearAllPools();
                }
                catch (SQLiteException E)
                {
                    tx.Rollback();

                    conn.Close();
                    SQLiteConnection.ClearAllPools();
                    throw new Exception(E.Message);
                }
            }
        }
        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章,有特殊符号,可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteNonQuery(string SQLString, string content)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand(SQLString, connection);
                SQLiteParameter myParameter = new SQLiteParameter("@content", DbType.String);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();

                    connection.Close();
                    SQLiteConnection.ClearAllPools();

                    return rows;
                }
                catch (SQLiteException E)
                {
                    connection.Close();
                    SQLiteConnection.ClearAllPools();

                    throw new Exception(E.Message);
                }
            }
        }
        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                SQLiteParameter myParameter = new SQLiteParameter("@fs", DbType.Binary);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();

                    connection.Close();
                    SQLiteConnection.ClearAllPools();

                    return rows;
                }
                catch (SQLiteException E)
                {
                    connection.Close();
                    SQLiteConnection.ClearAllPools();
                    throw new Exception(E.Message);
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句,返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object ExecuteScalar(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            connection.Close();
                            SQLiteConnection.ClearAllPools();
                            return null;
                        }
                        else
                        {
                            connection.Close();
                            SQLiteConnection.ClearAllPools();
                            return obj;
                        }
                    }
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        SQLiteConnection.ClearAllPools();

                        throw new Exception(e.Message);
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句,返回SQLiteDataReader
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SQLiteDataReader</returns>
        public SQLiteDataReader ExecuteReader(string strSQL)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
            try
            {
                connection.Open();
                SQLiteDataReader myReader = cmd.ExecuteReader();

                connection.Close();
                SQLiteConnection.ClearAllPools();
                return myReader;
            }
            catch (SQLiteException e)
            {
                connection.Close();
                SQLiteConnection.ClearAllPools();
                throw new Exception(e.Message);
            }

        }
        /// <summary>
        /// 执行查询语句,返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataSet(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SQLiteDataAdapter command = new SQLiteDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");

                    connection.Close();
                    SQLiteConnection.ClearAllPools();
                }
                catch (SQLiteException ex)
                {
                    connection.Close();
                    SQLiteConnection.ClearAllPools();

                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        /// <summary>
        /// 执行查询语句,返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataTable ExecuteDataTable(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                try
                {
                    connection.Open();
                    SQLiteDataAdapter command = new SQLiteDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");

                    command.Dispose();
                    connection.Close();
                    SQLiteConnection.ClearAllPools();
                }
                catch (SQLiteException ex)
                {
                    connection.Close();
                    SQLiteConnection.ClearAllPools();

                    throw new Exception(ex.Message);
                }
                return ds.Tables.Count > 0 ? ds.Tables[0] : dt;
            }
        }
        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句,返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ParameterExcuteNoQuery(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        connection.Close();
                        SQLiteConnection.ClearAllPools();
                        return rows;
                    }
                    catch (SQLiteException E)
                    {
                        connection.Close();
                        SQLiteConnection.ClearAllPools();
                        throw new Exception(E.Message);
                    }
                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句,实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句,value是该语句的SQLiteParameter[]）</param>
        public void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    SQLiteCommand cmd = new SQLiteCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            SQLiteParameter[] cmdParms = (SQLiteParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                        }
                        trans.Commit();

                        conn.Close();
                        SQLiteConnection.ClearAllPools();
                    }
                    catch
                    {
                        trans.Rollback();

                        conn.Close();
                        SQLiteConnection.ClearAllPools();
                        throw;
                    }
                }
            }
        }


        /// <summary>
        /// 执行一条计算查询结果语句,返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object ParameterExecuteScalar(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            connection.Close();
                            SQLiteConnection.ClearAllPools();
                            return null;
                        }
                        else
                        {
                            connection.Close();
                            SQLiteConnection.ClearAllPools();
                            return obj;
                        }
                    }
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        SQLiteConnection.ClearAllPools();
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句,返回SQLiteDataReader
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SQLiteDataReader</returns>
        public SQLiteDataReader ParameterExecuteReader(string SQLString, params SQLiteParameter[] cmdParms)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                SQLiteDataReader myReader = cmd.ExecuteReader();
                cmd.Parameters.Clear();

                connection.Close();
                SQLiteConnection.ClearAllPools();
                return myReader;
            }
            catch (SQLiteException e)
            {
                connection.Close();
                SQLiteConnection.ClearAllPools();
                throw new Exception(e.Message);
            }

        }

        /// <summary>
        /// 执行查询语句,返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet ParameterExecuteDataSet(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();

                        connection.Close();
                        SQLiteConnection.ClearAllPools();
                    }
                    catch (SQLiteException ex)
                    {
                        connection.Close();
                        SQLiteConnection.ClearAllPools();
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }


        private void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, SQLiteTransaction trans, string cmdText, SQLiteParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (SQLiteParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        #endregion

        #region 存储过程操作

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SQLiteDataReader</returns>
        public SQLiteDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteDataReader returnReader;
            connection.Open();
            SQLiteCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader();
            return returnReader;
        }


        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SQLiteDataAdapter sqlDA = new SQLiteDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }


        /// <summary>
        /// 构建 SQLiteCommand 对象(用来返回一个结果集,而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SQLiteCommand</returns>
        private SQLiteCommand BuildQueryCommand(SQLiteConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SQLiteCommand command = new SQLiteCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SQLiteParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        /// <summary>
        /// 执行存储过程,返回影响的行数 
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                int result;
                connection.Open();
                SQLiteCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
                //Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// 创建 SQLiteCommand 对象实例(用来返回一个整数值) 
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SQLiteCommand 对象实例</returns>
        private SQLiteCommand BuildIntCommand(SQLiteConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SQLiteCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new SQLiteParameter("ReturnValue",
            DbType.Int32, 4, ParameterDirection.ReturnValue,
            false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }

        private int HandleError(Exception ex)
        {
            //SystemError.CreateErrorLog(DateTime.Now.ToString() + "\n" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace + "\n\n");
            return -1;
        }
        #endregion

        #region 执行分页SQL语句
        /// <summary>
        ///  获取数量
        /// </summary>
        public  int GetRecordCount(string sql)
        {

            string strSql = "select count(*) from (" + sql + ") temp";
            object obj = ExecuteScalar(strSql);
            try
            {
                if (obj != null)
                {
                    return Convert.ToInt32(obj);
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
            return -1;
        }

        /// <summary>
        /// 获取分页信息
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="orderField">排序的字段</param>
        /// <param name="startNum">开始值</param>
        /// <param name="endNum">结束值</param>
        /// <param name="isDesc">是否倒序</param>
        /// <returns>返回带分页信息的sql</returns>
        public string GetPagedQuerySql(string sql, string orderField, int pageSize, int pageIndex, bool isDesc)
        {
            string strSql = "";
            string strSqlTemp = "";          

            if (isDesc)
                strSqlTemp = " desc ";
            else
                strSqlTemp = " asc ";

            strSql = string.Format(@"select * from ({0}) order by {1} {2} limit {3} offset {4};", sql,orderField, strSqlTemp, pageSize, (pageIndex-1)* pageSize);

            return strSql;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql">查询的SQL</param>
        /// <param name="orderField">排序的字段</param>
        /// <param name="isDesc">是否倒序</param>
        /// <param name="pageSize">页面规格</param>
        /// <param name="pageIndex">页面位置</param>
        /// <param name="errMsg">错误信息</param>
        /// <param name="totalNum">总数</param>
        /// <returns></returns>
        public DataTable GetPageQuery(string sql,string orderField, bool isDesc, int pageSize, int pageIndex, out string errMsg, out int totalNum)
        {
            errMsg = "";
            totalNum = GetRecordCount(sql);
            if(totalNum==-1)
            {
                errMsg = "查询总数失败"+sql;
                return new DataTable();
            }
            if (pageSize > 0 && pageIndex > 0 && !string.IsNullOrWhiteSpace(orderField))
            {
                sql= GetPagedQuerySql(sql, orderField, pageSize, pageIndex, isDesc);
            }
            else
                return new DataTable();

            DataTable dt = ExecuteDataTable(sql);
            return dt;
        }

        #endregion

    }
}
