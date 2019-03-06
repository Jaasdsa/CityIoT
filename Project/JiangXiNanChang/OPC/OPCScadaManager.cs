using CityLogService;
using CityOPCDataService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiangXiNanChang
{
    class OPCScadaManager:ISonService
    {
        // OPC -SCADA 数据采集任务

        private OPCScadaService opcScadaService;
        public void Start(out string errMsg)
        {
            errMsg = "";
            if (IsRuning)
                return;

            try
            {
                // Scada-OPC通信服务
                if (opcScadaService != null)
                    opcScadaService.Stop();
                opcScadaService = new OPCScadaService(Config.projectConfigPath);
                opcScadaService.Start(out errMsg);
                if (opcScadaService.IsRuning)
                    TraceManagerForOPC.AppendDebug("Scada-OPC通信服务已经打开");
                else
                {
                    errMsg = "Scada-OPC通信服务打开失败";
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
            // Scada-OPC通信服务
            if (opcScadaService != null)
            {
                opcScadaService.Stop();
                if (!opcScadaService.IsRuning)
                {
                    TraceManagerForWeb.AppendDebug("Scada-OPC通信服务停止成功");
                    this.opcScadaService = null;
                }
                else
                    TraceManagerForWeb.AppendErrMsg("Scada-OPC通信服务停止失败");
                }
            }
            catch (Exception e) { TraceManagerForOPC.AppendErrMsg("Scada-OPC通信服务停止失败:" + e.Message); }

            IsRuning = false;
        }

        public void ReceiveCommand(RequestCommand dispatchCommand)
        {

        }

    }
}
