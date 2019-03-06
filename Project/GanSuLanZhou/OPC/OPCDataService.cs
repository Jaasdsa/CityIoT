using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanSuLanZhou
{
    class OPCDataService : IServiceWorker, ISonService
    {
        // OPC 服务入口中心

        private OPCPumpManager opcPumpManager;

        // 对调度服务实现的接口
        public void ReceiveCommand(RequestCommand request)
        {
            if (opcPumpManager != null && opcPumpManager.IsRuning)
                opcPumpManager.ReceiveCommand(request);
        }

        public bool IsRuning { get; set; } = false;
        public void Start(out string errMsg)
        {
            errMsg = "";

            if (IsRuning)
                return;
            try
            {
                // pump-OPC通信服务
                if (opcPumpManager != null)
                    opcPumpManager.Stop();
                opcPumpManager = new OPCPumpManager();
                opcPumpManager.Start(out errMsg);
                if (opcPumpManager.IsRuning)
                    TraceManagerForWeb.AppendDebug("Pump-OPC通信服务管理器已经打开");
                else
                {
                    errMsg = "Pump-OPC通信服务管理器打开失败";
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
                // Pump-OPC通信服务
                if (opcPumpManager != null)
                {
                    opcPumpManager.Stop();
                    if (!opcPumpManager.IsRuning)
                    {
                        TraceManagerForWeb.AppendDebug("Pump-OPC通信服务管理器停止成功");
                        this.opcPumpManager = null;
                    }
                    else
                        TraceManagerForWeb.AppendErrMsg("Pump-OPC通信服务管理器停止失败");
                }
            }
            catch (Exception e) { TraceManagerForWeb.AppendErrMsg("Pump-OPC通信服务管理器停止失败:" + e.Message); }

            IsRuning = false;
        }
    }
}
