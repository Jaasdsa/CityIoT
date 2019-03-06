using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CityOPCDataService
{
    enum OPCClientManagerType
    {
        Pump,       // 二供系统
        Scada       // SCADA 系统
    }
    class OpcClient : IWorker
    {
        private ConcurrentDictionary<string, OpcDaClient> opcCaches;
        private OPCClientManagerType type;

        public OpcClient(OPCClientManagerType type) 
        {
            this.type = type;
        }
        public bool IsRuning { get; set; } = false;
        public void Start()
        {
            // 同步配置信息
            LoadConfig();

            // 同步 jz 表中有几种 OPCServer
            string[] dbServerNames = LoadPumpJZOPCServerNames();
            if (dbServerNames == null || dbServerNames.Length == 0)
            {
                TraceManagerForOPC.AppendErrMsg("机组表中没有读取模式为 OPC 的机组, 无法启动 OPC 服务");
                Stop();
                return;
            }

            string[] serverNames = OpcDaClient.GetLocalServer();
            if (serverNames == null || serverNames.Length == 0)
            {
                TraceManagerForOPC.AppendErrMsg("本地计算机没有安装 OPCServer, 无法启动 OPC 服务");
                Stop();
                return;
            }
            opcCaches = new ConcurrentDictionary<string, OpcDaClient>();

            // 创建 OPC 客户端对象
            foreach (string dbServerName in dbServerNames)
            {
                if (string.IsNullOrWhiteSpace(dbServerName) || dbServerName.Length < 6)
                {
                    TraceManagerForOPC.AppendErrMsg("数据库中 OPC 服务器名非法, 无法启动 OPC 客户端");
                    Stop();
                    return;
                }
                string dbServerNameHead = dbServerName.Trim().ToLower().Substring(0, 5);
                if (serverNames.Where(str => str.Contains(dbServerNameHead)).ToArray().Length == 0)
                {
                    TraceManagerForOPC.AppendErrMsg("本地没有安装 " + dbServerName + " 服务器, 无法启动 OPC 客户端");
                    Stop();
                    return;
                }

                OpcDaClient opc = new OpcDaClient(serverNames.Where(str => str.Contains(dbServerNameHead)).ToArray()[0], UpdateRate, null, DefaultGroupDeadband);
                opcCaches.TryAdd(dbServerName, opc);
            }
            // OPC 客户端连接服务器
            foreach (string key in opcCaches.Keys)
            {
                OpcDaClient opc = opcCaches[key];
                if (opc != null)
                {
                    FuncTimeout<OpcDaClient, bool> timeout = new FuncTimeout<OpcDaClient, bool>();
                    timeout.Do = StartOPCClient;

                    bool isTimeout = timeout.DoWithTimeout(opc, new TimeSpan(0, 0, 0, 15), out bool reslut);    // 只等待 15 秒
                    if (isTimeout)
                    {
                        // 超时
                        if (key == "KingView.View.1")
                            TraceManagerForOPC.AppendErrMsg("连接 " + opc.ProgID + " OPC服务器超时 " + ", 可能原因为未配置其 DCOM 权限");
                        else
                            TraceManagerForOPC.AppendErrMsg("连接 " + opc.ProgID + " OPC服务器超时");
                        Stop();
                        return;
                    }

                    if (reslut)
                    {
                        TraceManagerForOPC.AppendDebug("已连接 " + opc.ProgID + " OPC 服务器");
                    }
                    else
                    {
                        if (key == "KingView.View.1")
                            TraceManagerForOPC.AppendErrMsg("连接 " + opc.ProgID + " OPC 服务器失败" + ", 可能原因为组态王未运行");
                        else
                            TraceManagerForOPC.AppendErrMsg("连接 " + opc.ProgID + " OPC 服务器失败");
                        Stop();
                        return;
                    }
                }
            }
            IsRuning = true;
        }
        public void Stop()
        {
            if (opcCaches == null)
                return;

            foreach (string key in opcCaches.Keys)
            {
                OpcDaClient opc = opcCaches[key];
                if (opc == null)
                    continue;
                try
                {
                    opc.Stop();
                    TraceManagerForOPC.AppendDebug(key + "已断开连接");
                }
                catch { }
            }
            opcCaches = null;
            IsRuning = false;
        }

        // 读取服务配置文件
        private int cmdGetTimes { get; set; }
        private int DefaultGroupDeadband { get; set; }
        private int UpdateRate { get; set; }
        private int ReadRate { get; set; }

        // 加载配置
        private void LoadConfig()
        {
            if (this.type == OPCClientManagerType.Pump)
            {
                cmdGetTimes = Config.pumpConfig.PointsCollectInterVal;

                DefaultGroupDeadband = Config.pumpConfig.DefaultGroupDeadband;
                UpdateRate = Config.pumpConfig.UpdateRate;
                ReadRate = Config.pumpConfig.ReadRate;
            }
            else if(this.type == OPCClientManagerType.Scada)
            {
                cmdGetTimes = Config.scadaConfig.PointsCollectInterVal;

                DefaultGroupDeadband = Config.scadaConfig.DefaultGroupDeadband;
                UpdateRate = Config.scadaConfig.UpdateRate;
                ReadRate = Config.scadaConfig.ReadRate;
            }
        }
        private string[] LoadPumpJZOPCServerNames()
        {
            string sql="";
            switch (type)
            {
                case OPCClientManagerType.Pump:
                    sql = "select distinct t1.FOPCServerName from Pump t,pumpjz t1 where (t.是否删除 = 0 or t.是否删除 is null) and (t1.是否删除 = 0 or t1.是否删除 is null) and t1.PumpJZReadMode = 'OPC';";
                    break;
                case OPCClientManagerType.Scada:
                    sql = "select distinct FOPCServerName from SCADA_Station where ReadMode = 'OPC';";
                    break;
            }
            DataTable dt = DBUtil.ExecuteDataTable(sql, out string err);
            if (!string.IsNullOrEmpty(err))
            {
                TraceManagerForOPC.AppendErrMsg("加载机组 OPCServerName 出错" + err);
                return null;
            }
            List<string> r = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                r.Add(DataUtil.ToString(dr["FOPCServerName"]));
            }
            return r.ToArray();
        }

        // OPC 客户端
        public OpcDaClient GetOPCClient(string opcServerName)
        {
            if (opcCaches.Keys.Contains(opcServerName))
                return opcCaches[opcServerName];
            else
                return null;
        }
        private bool StartOPCClient(OpcDaClient opc)
        {
            return opc.Start();
        }
    }
}
