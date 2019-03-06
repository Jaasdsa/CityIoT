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

namespace JiangXiNanChang
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
                    ReceiveCommand(new RequestCommand());
                }
                catch(Exception ee)
                {
                    TraceManagerForWeb.AppendErrMsg("二供-WEB 定时任务执行失败:"+ee.Message);
                }
            };
            timer.Enabled = true;

            IsRuning = true;

            // 开始异步执行一次-防止启动卡死
            Action<RequestCommand> action = ReceiveCommand;
            action.BeginInvoke(new RequestCommand(), null, null);
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
        public void ReceiveCommand(RequestCommand dispatchCommand)
        {
            if (dispatchCommand != null)
                Excute();
        } 
        private void LoadConfig()
        {
            collectInterval = Config.collectInterval;
        } 
    }
}
