using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CityOPCDataService
{
    class PumpCollecter : IWorker
    {
        // 泵房表管理器--定时采集、写入
        System.Timers.Timer timer;
        OpcClient opcClientManager;
        PumpCommandConsumer pumpCommandConsumer;

        public PumpCollecter(OpcClient opcClientManager, PumpCommandConsumer pumpCommandConsumer)
        {
            this.opcClientManager = opcClientManager;
            this.pumpCommandConsumer = pumpCommandConsumer;
        }

        public bool IsRuning { get; set; } = false;
        public void Start()
        {
            if (IsRuning)
                return;

            // 初始化实时表
            PumpJZDataOper.Instance.InitPumpRealData(out string errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                TraceManagerForOPC.AppendErrMsg(errMsg);
                return;
            }

            // 开启定时工作者
            timer = new System.Timers.Timer();
            timer.Interval = Config.pumpConfig.PointsCollectInterVal * 1000;
            timer.Elapsed += (o, ee) =>
            {
                try
                {
                    // 采集缓存中机组关联的点表信息
                    Excute();
                }
                catch (Exception e)
                {
                    TraceManagerForOPC.AppendErrMsg("泵站表采集器异常" + e.Message);
                }
            };
            timer.Enabled = true;
            IsRuning = true;

            // 开启之后就异步执行一次
            Action action = Excute;
            action.BeginInvoke(null, null);
        }
        public void Stop()
        {
            if (timer == null)
                return;

            timer.Enabled = false;
            timer.Close();
            timer = null;

            IsRuning = false;
        }

        // 定时的执行体
        private void Excute()
        {
            this.pumpCommandConsumer.Append(PumpCommand.CreateCollectCommand(this.opcClientManager));
        }
    }
}
