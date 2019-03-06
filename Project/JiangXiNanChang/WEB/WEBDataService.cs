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
    class WEBDataService: IServiceWorker,ISonService
    {
        private WEBPandaPumpManager webPumpManager;
        private WEBPandaYLSacdaManager webYLScadaManager;
        private WEBPandaZHCDSacdaManager webZHCDScadaMandager;

        // 对调度服务实现的接口
        public void ReceiveCommand(RequestCommand command)
        {
            if (webPumpManager != null && webPumpManager.IsRuning)
                webPumpManager.ReceiveCommand(command);

            if (webYLScadaManager != null && webYLScadaManager.IsRuning)
                webYLScadaManager.ReceiveCommand(command);
        }

        public bool IsRuning { get; set; } = false;
        public void Start(out string errMsg)
        {
            errMsg = "";

            if (IsRuning)
                return;

            try
            {
                // 二供WEB通信服务
                if (webPumpManager != null)
                    webPumpManager.Stop();
                webPumpManager =new WEBPandaPumpManager();
                webPumpManager.Start(out errMsg);
                if (webPumpManager.IsRuning)
                    TraceManagerForWeb.AppendDebug("二供WEB通信服务管理器已经打开");
                else
                {
                    errMsg="二供WEB通信服务管理器打开失败";
                    Stop();
                    return;
                }

                // SACADA—WEB-监测点通信服务
                if (webYLScadaManager != null)
                    webYLScadaManager.Stop();
                webYLScadaManager = new WEBPandaYLSacdaManager();
                webYLScadaManager.Start(out errMsg);
                if (webYLScadaManager.IsRuning)
                    TraceManagerForWeb.AppendDebug("SACADA—WEB-监测点通信服务管理器已经打开");
                else
                {
                    errMsg = "SACADA—WEB-监测点通信服务管理器打开失败";
                    Stop();
                    return;
                }

                // SACADA—WEB-综合测点通信服务
                if (webZHCDScadaMandager != null)
                    webZHCDScadaMandager.Stop();
                webZHCDScadaMandager = new WEBPandaZHCDSacdaManager();
                webZHCDScadaMandager.Start(out errMsg);
                if (webZHCDScadaMandager.IsRuning)
                    TraceManagerForWeb.AppendDebug("SACADA—WEB-综合测点通信服务管理器已经打开");
                else
                {
                    errMsg = "SACADA—WEB-综合测点通信服务管理器打开失败";
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
                // SACADA—WEB-综合测点通信服务
                if (webZHCDScadaMandager != null)
                {
                    webZHCDScadaMandager.Stop();
                    if (!webZHCDScadaMandager.IsRuning)
                    {
                        TraceManagerForWeb.AppendDebug("SACADA—WEB-综合测点通信服务管理器停止成功");
                        this.webZHCDScadaMandager = null;
                    }
                    else
                        TraceManagerForWeb.AppendErrMsg("SACADA—WEB-综合测点通信服务管理器停止失败");
                }
            }
            catch (Exception e) { TraceManagerForWeb.AppendErrMsg("二供—WEB-综合测点通信服务管理器停止失败:" + e.Message); }

            try
            {
                // SACADA—WEB-监测点通信服务
                if (webYLScadaManager != null)
                {
                    webYLScadaManager.Stop();
                    if (!webYLScadaManager.IsRuning)
                    {
                        TraceManagerForWeb.AppendDebug("SACADA—WEB-监测点通信服务管理器停止成功");
                        this.webYLScadaManager = null;
                    }
                    else
                        TraceManagerForWeb.AppendErrMsg("SACADA—WEB-监测点通信服务管理器停止失败");
                }
            }
            catch (Exception e) { TraceManagerForWeb.AppendErrMsg("二供—WEB-监测点通信服务管理器停止失败:" + e.Message); }

            try
            {
                // 二供WEB通信服务
                if (webPumpManager != null)
                {
                    webPumpManager.Stop();
                    if (!webPumpManager.IsRuning)
                    {
                        TraceManagerForWeb.AppendDebug("二供—WEB通信服务管理器停止成功");
                        this.webPumpManager = null;
                    }
                    else
                        TraceManagerForWeb.AppendErrMsg("二供—WEB通信服务管理器停止失败");
                }
            }
            catch (Exception e){ TraceManagerForWeb.AppendErrMsg("二供—WEB通信服务管理器停止失败:" + e.Message); }

            IsRuning = false;
        }
    }
}
