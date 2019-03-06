using CityIoTCommand;
using CityLogService;
using CityPublicClassLib;
using CityUtils;
using CityWEBDataService;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GanSuLanZhou
{
   public class WEBPandaPumpManager
    {
        // WEB -二供 数据采集任务
        private System.Timers.Timer timer;
        private int collectInterval;

        public  void Start(out string errMsg)
        {
            errMsg = "";
            if (IsRuning)
                return;

            LoadConfig();

            WebPandaPumpCommand.CreateInitPumpRealData(Config.pumpParam).Execute(); //初始化实时表

            timer = new System.Timers.Timer();
            timer.Interval = collectInterval *60 * 1000;
            timer.Elapsed += (o, e) =>
            {
                try
                {
                    Excute();
                }
                catch(Exception ee)
                {
                    TraceManagerForWeb.AppendErrMsg("二供-WEB 定时任务执行失败:"+ee.Message);
                }
            };
            timer.Enabled = true;

            IsRuning = true;

            // 开始异步执行一次-防止启动卡死
            Action action = Excute; 
            action.BeginInvoke( null, null);
        }
        public  bool IsRuning { get; set; }
        private bool ExcuteDoing { get; set; } = false;
        public  void Stop()
        {
            if (!IsRuning)
                return;

            // 关闭定时器
            if (timer != null)
            {
                timer.Enabled = false;
                timer.Close();
                timer = null;
            }

            IsRuning = false;
        }

        private void Excute()
        {
            if (ExcuteDoing)
                return;
            ExcuteDoing = true;
            ExcuteHandle();
            ExcuteDoing = false;
        }
        private void ExcuteHandle()
        {
            // 报警维护
            WebPandaPumpCommand.CreateCollectAndSavePumpPoints(Config.pumpParam).Execute();
        }
        public void ReceiveCommand(RequestCommand command)
        {
            //if (command == null)
            //    return;
            //if (command.commandServerType == CommandServerType.OPC_Pump && command.commandOperType == CommandOperType.ReLoadJZData)
            //{
            //    if (ExcuteDoing) // 正在采集，等这次采集结束，在采集一次
            //    {
            //        DateTime time1 = DateTime.Now;
            //        while (true)
            //        {
            //            Thread.Sleep(1);
            //            if (DateTime.Now - time1 > TimeSpan.FromSeconds(command.timeoutSeconds)) // 超时
            //            {
            //                RecCommand.MakeTimeout("Pump-OPC数据更新超时", ref command);
            //                CommandManager.CompleteCommand(command);
            //                TraceManagerForOPC.AppendInfo(command.message);
            //                return;
            //            }
            //            if (!ExcuteDoing)
            //                break;
            //        }
            //    }
            //    // 刷新一次
            //    Excute();
            //    RecCommand.MakeSuccess("Pump-OPC数据已更新", ref command);
            //    CommandManager.CompleteCommand(command);
            //    TraceManagerForOPC.AppendInfo("Pump-OPC数据已更新");
            //    return;
            //}
            //RecCommand.MakeFail("错误的请求服务类型", ref command);
            //CommandManager.CompleteCommand(command);
            //TraceManagerForOPC.AppendErrMsg(command.message);
            //return;
        } 
        private void LoadConfig()
        {
            collectInterval = Config.collectInterval;
        } 
    }
}
