using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiangXiNanChang
{
    class OPCDataService : IServiceWorker, ISonService
    {
        // OPC 服务入口中心

        private OPCScadaManager opcScadaManager;

        // 对调度服务实现的接口
        public void ReceiveCommand(RequestCommand command)
        {
            if (opcScadaManager != null && opcScadaManager.IsRuning)
                opcScadaManager.ReceiveCommand(command);
        }

        public bool IsRuning { get; set; } = false;
        public void Start(out string errMsg)
        {
            errMsg = "";

            if (IsRuning)
                return;
            try
            {
                // Scada-OPC通信服务
                if (opcScadaManager != null)
                    opcScadaManager.Stop();
                opcScadaManager = new OPCScadaManager();
                opcScadaManager.Start(out errMsg);
                if (opcScadaManager.IsRuning)
                    TraceManagerForWeb.AppendDebug("Scada-OPC通信服务管理器已经打开");
                else
                {
                    errMsg = "Scada-OPC通信服务管理器打开失败";
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
                // Scada-OPC通信服务
                if (opcScadaManager != null)
                {
                    opcScadaManager.Stop();
                    if (!opcScadaManager.IsRuning)
                    {
                        TraceManagerForWeb.AppendDebug("Scada-OPC通信服务管理器停止成功");
                        this.opcScadaManager = null;
                    }
                    else
                        TraceManagerForWeb.AppendErrMsg("Scada-OPC通信服务管理器停止失败");
                }
            }
            catch (Exception e) { TraceManagerForWeb.AppendErrMsg("Scada-OPC通信服务管理器停止失败:" + e.Message); }

            IsRuning = false;
        }
    }
}
