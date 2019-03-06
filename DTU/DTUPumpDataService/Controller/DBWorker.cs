using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CityUtils;

namespace DTUPumpDataService 
{
    public enum DBCommandType
    {
        DtuRegister,
        DtuOffline,
        PointUpdate,
        PandaCommand,
        DtuListUpdate,
    }

    public class DBCommand
    {
        DBCommandType Type;
        string DTUID;
        DTUInfo[] DtuList;
        DTUInfo DTUInfo;

        public static DBCommand CreateDtuRegister(DTUInfo info)
        {
            return new DBCommand() { Type = DBCommandType.DtuRegister, DTUInfo = info };
        }
        public static DBCommand CreateDtuOffline(string dtuID)
        {
            return new DBCommand() { Type = DBCommandType.DtuOffline, DTUID = dtuID };
        }
        public static DBCommand CreateDtuCacheUpdate(DTUInfo[] dtuInfos)
        {
            return new DBCommand() { Type = DBCommandType.DtuListUpdate, DtuList = dtuInfos };
        }

        public void Execute()
        {
            try
            {
                switch (Type)
                {
                    case DBCommandType.DtuRegister:
                        RegisterDTU(DTUInfo);
                        break;
                    case DBCommandType.DtuOffline:
                        DtuOffline(DTUID);
                        break;
                    case DBCommandType.DtuListUpdate:
                        DtuListUpdate(DtuList);
                        break;
                }
            }
            catch (Exception e)
            {
                TraceManager.AppendErrMsg("工作队列执行出错:" + e.Message);
            }

        }
        private void DtuListUpdate(DTUInfo[] dtuInfos)
        {
            if (dtuInfos == null || dtuInfos.Length == 0)
                return;

            string dtuIDs = "'第一个补个位'";
            string setOnlineSQL = "";
            foreach (DTUInfo dtu in dtuInfos)
            {
                if (dtu.IsActive == 1)
                {
                    setOnlineSQL += string.Format(@" update DTUBase set                                     
                                                    [是否使用] ={0},        
                                                    [是否在线] ={1},
                                                    [登录时间] = '{2}',
                                                    [最后注册时间] ='{3}',
                                                    [终端IP地址] ='{4}',
                                                    [终端端口] ={5},
                                                    [网关IP地址] ='{6}',
                                                    [网关端口] ={7} 
                                                    where [终端登录号码] = '{8}'",
                                            dtu.IsUsed, dtu.IsActive, dtu.ActiveTime,
                                            dtu.LastRegisterTime, dtu.TerminalIP, dtu.TerminalPort,
                                            dtu.GatewayIP, dtu.GatewayPort, dtu.ID);
                    dtuIDs += "'" + dtu.ID + "'";
                }
            }
            string setAllOfflineSQL = string.Format(@"UPDATE DTUBase SET 是否在线 =0;");
            // 注意先复位先更新
            DBUtil.ExecuteNonQuery(setOnlineSQL, out string errMsg);
            if (!string.IsNullOrEmpty(errMsg))
            {
                TraceManager.AppendErrMsg("DTU列表更新到数据库失败：" + errMsg);
                return;
            }
            DataTable dt = new DataTable();
            string querySQL = @"select * from DTUBase where 终端登录号码 in (" + dtuIDs + ");";
            // string querySQL = @"select * from DTUBase;";
            dt = DBUtil.ExecuteDataTable(querySQL, out errMsg);
            if (!string.IsNullOrEmpty(errMsg))
            {
                TraceManager.AppendErrMsg("DTU列表从数据库加载时出错" + errMsg);
                return;
            }
            List<DTUInfo> dbDtus = new List<DTUInfo>();

            foreach (DataRow dr in dt.Rows)
            {
                DTUInfo dtu = new DTUInfo();
                dtu.DBID = DataUtil.ToInt(dr["ID"]);
                dtu.ID = DataUtil.ToString(dr["终端登录号码"]);
                dtu.Name = DataUtil.ToString(dr["名称"]);
                dtu.Description = DataUtil.ToString(dr["描述"]);
                dtu.FactoryName = DataUtil.ToString(dr["DTU厂家"]);
                dtu.Model = DataUtil.ToString(dr["DTU型号"]);
                dtu.WorkType = DataUtil.ToString(dr["DTU工作方式"]);
                dtu.Protocol = DTUInfo.GetProtocol(DataUtil.ToString(dr["通信协议"]));
                dtu.IsUsed = DataUtil.ToInt(dr["是否使用"]);
                dtu.IsActive = DataUtil.ToInt(dr["是否在线"]);
                dtu.ActiveTime = DataUtil.ToDateString(dr["登录时间"]);
                dtu.LastRegisterTime = DataUtil.ToDateString(dr["最后注册时间"]);
                dtu.TerminalIP = DataUtil.ToString(dr["终端IP地址"]);
                dtu.TerminalPort = DataUtil.ToInt(dr["终端端口"]);
                dtu.GatewayIP = DataUtil.ToString(dr["网关IP地址"]);
                dtu.GatewayPort = DataUtil.ToInt(dr["网关端口"]);

                dbDtus.Add(dtu);
            }
            // 同步到内存
            if (dbDtus.Count > 0)
                DTUCacheManager.OperDTUCache(DTUCacheManager.OPeratingType.UpdateBSInfoMulti, dbDtus.ToArray(), null);

        }
        private void RegisterDTU(DTUInfo dtu)
        {
            string setOnlineSQL = string.Format(@" update DTUBase set                                     
                                                    [是否使用] ={0},        
                                                    [是否在线] ={1},
                                                    [登录时间] = '{2}',
                                                    [最后注册时间] ='{3}',
                                                    [终端IP地址] ='{4}',
                                                    [终端端口] ={5},
                                                    [网关IP地址] ='{6}',
                                                    [网关端口] ={7} 
                                                    where [终端登录号码] = '{8}'",
                         dtu.IsUsed, dtu.IsActive, dtu.ActiveTime,
                         dtu.LastRegisterTime, dtu.TerminalIP, dtu.TerminalPort,
                         dtu.GatewayIP, dtu.GatewayPort, dtu.ID);
            int rows = DBUtil.ExecuteNonQuery(setOnlineSQL, out string err);
            if (!string.IsNullOrEmpty(err))
            {
                TraceManager.AppendErrMsg(dtu.ID + "数据链路同步到数据库失败");
                return;
            }
            if (rows == 0)
            {
                TraceManager.AppendInfo(dtu.ID + "数据库没有该条链路信息");
                return;
            }
            if (rows > 0)
            {
                string sqlQuery = string.Format(@"SELECT TOP 1 [ID]
                                  ,[名称]
                                  ,[描述]
                                  ,[DTU厂家]
                                  ,[DTU型号]
                                  ,[DTU工作方式]
                                  ,[通信协议] 
                              FROM DTUBase where 终端登录号码='{0}'; ", dtu.ID);
                DataTable dt = DBUtil.ExecuteDataTable(sqlQuery, out err);
                if (!string.IsNullOrEmpty(err))
                {
                    TraceManager.AppendErrMsg(dtu.ID + "数据库同步链路的业务信息失败");
                    return;
                }
                DataRow dr = dt.Rows[0];// 只查询一行，在条件里面可以不用担心出错
                dtu.DBID = DataUtil.ToInt(dr["ID"]);
                dtu.Name = DataUtil.ToString(dr["名称"]);
                dtu.Description = DataUtil.ToString(dr["描述"]);
                dtu.FactoryName = DataUtil.ToString(dr["DTU厂家"]);
                dtu.Model = DataUtil.ToString(dr["DTU型号"]);
                dtu.WorkType = DataUtil.ToString(dr["DTU工作方式"]);
                dtu.Protocol = DTUInfo.GetProtocol(DataUtil.ToString(dr["通信协议"]));

                DTUCacheManager.OperDTUCache(DTUCacheManager.OPeratingType.Add, dtu, null);
            }
        }
        private void DtuOffline(string dtuID)
        {
            string errMsg = "";
            string updateSQL = string.Format(@"update DTUBase set [是否在线]=0  where 终端登录号码 ='{0}'", dtuID);
            int effectRows = DBUtil.ExecuteNonQuery(updateSQL, out errMsg);
            if (!string.IsNullOrEmpty(errMsg))
            {
                TraceManager.AppendErrMsg("DTU下线执行出错" + errMsg);
                return;
            }
            //if (effectRows > 0)
            //    TraceManager.AppendInfo(dtuID + "【下线】更新成功");

        }
    }
    class DBWorker
    {
        private static BlockingCollection<DBCommand> commandQueue;
        private static Task task;

        public static void Start()
        {
            if (IsRuning)
                return;
            commandQueue = new BlockingCollection<DBCommand>();
            task = new Task(() =>
            {
                foreach (DBCommand command in commandQueue.GetConsumingEnumerable())
                {
                    try
                    {
                        command.Execute();
                    }
                    catch (Exception e)
                    {
                        TraceManager.AppendErrMsg("数据库工作服务执行异常" + e.Message);
                    }
                }
            }, TaskCreationOptions.LongRunning);
            task.Start();
            IsRuning = true;
        }
        public static bool IsRuning { get; set; }
        public static void Stop()
        {
            if (!IsRuning)
                return;

            // 先完成添加
            commandQueue.CompleteAdding();
            DateTime time1 = DateTime.Now;
            while (commandQueue.Count > 0)
            {
                Thread.Sleep(1);
                // 最多等待10秒避免关不掉
                if (DateTime.Now - time1 > TimeSpan.FromSeconds(10))
                {
                    break;
                }
            }
            while (commandQueue.Count > 0)
            {
                // 等了十秒还没听，队列全部元素废弃
                commandQueue.Take();
            }


            Task.WaitAll(task);
            task.Dispose();
            task = null;
            commandQueue = null;
            IsRuning = false;


        }

        public static void Append(DBCommand command)
        {
            if (!IsRuning)
                return;
            if (commandQueue.IsAddingCompleted)
                return;
            if (commandQueue.Count > 4096)
                return;
            commandQueue.Add(command);
        }
    }
}
