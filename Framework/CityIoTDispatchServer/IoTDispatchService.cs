using CityIoTServiceManager;
using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CityIoTDispatchServer
{
    public partial class IoTDispatchService : ServiceBase
    {
        public IoTDispatchService()
        {
            InitializeComponent();
        }

        Task aliveTask;
        string projectConfigPath;
        EnvConfigInfo configInfo = EnvConfigInfo.SingleInstanceForCS;

        protected override void OnStart(string[] args)
        {
            LogManager.StartDisPatchRecorder(configInfo.confLogServiceInfo);
            IoTStartKeepAlive();
            LogManager.RecordDisPatchLog(new TraceItem(TraceType.Info, ServerTypeName.Dispatch, "调度服务启动成功"));
        }

        protected override void OnStop()
        {
            IoTStopKeepAlive();
            LogManager.RecordDisPatchLog(new TraceItem(TraceType.Info, ServerTypeName.Dispatch, "调度服务停止成功"));
        }

        bool IsFirstReConnect = true;
        bool isNeedDoFla = false;
        private void IoTStartKeepAlive()
        {
            aliveTask = new Task(new Action(() => {
                int times = 0;
                while (isNeedDoFla)
                {
                    if (!IoTIsAlived())
                    {
                        times++;
                        if (times > 10 || IsFirstReConnect)
                        {
                            times = 0;  //每10秒重连一次 或者第一次马上重连
                            IsFirstReConnect = false;
                            AutoReConnect();
                        }
                    }
                    Thread.Sleep(1000);
                }
            }), TaskCreationOptions.LongRunning);
            isNeedDoFla = true;
            aliveTask.Start();
        }
        private void IoTStopKeepAlive()
        {
            isNeedDoFla = false;
            if (aliveTask != null)
            {
                try
                {
                    aliveTask.Wait();
                    aliveTask.Dispose();
                }
                catch { }
                aliveTask = null;
            }
        }
        private bool IoTIsAlived()
        {
            return new ServiceManager(configInfo).IsCoreServiceRun();
        }
        private void AutoReConnect()
        {
            ServiceManager serviceManager = new ServiceManager(configInfo);
            if (serviceManager.IsCoreServiceRun())
                return;
            if (serviceManager.IsNeedOPCServer)
            {
                // 先把网关服务杀掉-让OPC客户端连接时自动打开
                if (serviceManager.StopOPCServer(out string err))
                    LogManager.RecordDisPatchLog(new TraceItem(TraceType.Info, ServerTypeName.Dispatch, "IoT网关服务关闭成功,等待自启..."));
                else
                    LogManager.RecordDisPatchLog(new TraceItem(TraceType.Error, ServerTypeName.Dispatch, "IoT网关服务关闭失败" + err));
            }
            // 打开核心服务
            serviceManager.IsStartCoreHander(out string statusCode,out string errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                LogManager.RecordDisPatchLog(new TraceItem(TraceType.Error, ServerTypeName.Dispatch, "调度服务重启核心服务失败：" + errMsg));
            else
                LogManager.RecordDisPatchLog(new TraceItem(TraceType.Info, ServerTypeName.Dispatch, "调度服务重启核心服务成功"));
        }

        private void TestStartTimer()
        {
            //System.Timers.Timer timer = new System.Timers.Timer();
            //timer.AutoReset = true;
            //timer.Enabled = true;
            //timer.Interval = 1000;
            //bool flag = false;
            //DateTime t1 = DateTime.Now;
            //timer.Elapsed += (o, e) =>
            //{
            //    DateTime t2 = DateTime.Now;
            //    if (t2 - t1 > TimeSpan.FromSeconds(20))
            //    {
            //        if (!flag)
            //        {
            //            flag = true;
            //        }
            //    }
            //};
            //timer.Start();
        }
    }
}
