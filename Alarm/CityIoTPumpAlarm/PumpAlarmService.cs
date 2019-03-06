using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityIoTPumpAlarm
{
    public class PumpAlarmService : ISonService, IServiceWorker
    {
        private CommandManager commandManager;

        public PumpAlarmService(ConfSonCityIoTPumpAlarm confPumpAlarm)
        {
            Config.confPumpAlarm = confPumpAlarm;
        }

        // 对调度服务实现的接口
        public void ReceiveCommand(RequestCommand command)
        {

        }

        public bool IsRuning { get; set; } = false;
        public void Start(out string errMsg)
        {
            errMsg = "";

            if (IsRuning)
                return;

            try
            {
                // 环境检查
                if (!EnvChecker.Check(out errMsg))
                    return;
                TraceManagerForPumpAlarm.AppendDebug("环境检查通过");

                // 控制器服务
                if (commandManager != null)
                    commandManager.Stop();
                commandManager = new CommandManager();
                commandManager.Start();
                if (commandManager.IsRuning)
                    TraceManagerForPumpAlarm.AppendDebug("控制器服务已经打开");
                else
                {
                    TraceManagerForPumpAlarm.AppendErrMsg("控制器服务打开失败");
                    Stop();
                    return;
                }
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                Stop();
                return;
            }

            IsRuning = true;
        }
        public void Stop()
        {
            try
            {
                // 控制器服务
                if (commandManager != null)
                {
                    commandManager.Stop();
                    if (!commandManager.IsRuning)
                    {
                        TraceManagerForPumpAlarm.AppendDebug("控制器服务已停止");
                        this.commandManager = null;
                    }
                    else
                        TraceManagerForPumpAlarm.AppendErrMsg("控制器服务停止失败");
                }
            }
            catch { }

            IsRuning = false;
        }


    }
}
