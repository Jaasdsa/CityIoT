using CityLogService;
using CityOPCDataService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeBeiBaoDing
{
    class OPCPumpManager: ISonService
    {
        // OPC -Pump 数据采集任务

        private OPCPumpService opcPumpService;
        public void Start(out string errMsg)
        {
            errMsg = "";
            if (IsRuning)
                return;

            try
            {
                // Pump-OPC通信服务
                if (opcPumpService != null)
                    opcPumpService.Stop();
                opcPumpService = new OPCPumpService(Config.projectConfigPath);
                opcPumpService.Start(out errMsg);
                if (opcPumpService.IsRuning)
                    TraceManagerForOPC.AppendDebug("Pump-OPC通信服务已经打开");
                else
                {
                    errMsg = "Pump-OPC通信服务打开失败";
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
        public bool IsRuning { get; set; }
        public void Stop()
        {
            if (!IsRuning)
                return;

            try
            {
                // Pump-OPC通信服务
                if (opcPumpService != null)
            {
                opcPumpService.Stop();
                if (!opcPumpService.IsRuning)
                {
                    TraceManagerForWeb.AppendDebug("Pump-OPC通信服务停止成功");
                    this.opcPumpService = null;
                }
                else
                    TraceManagerForWeb.AppendErrMsg("Pump-OPC通信服务停止失败");
                }
            }
            catch (Exception e) { TraceManagerForOPC.AppendErrMsg("Pump-OPC通信服务停止失败:" + e.Message); }

            IsRuning = false;
        }

        public void ReceiveCommand(RequestCommand request)
        {
            this.opcPumpService.ReceiveCommand(request);
        }
    }
}
