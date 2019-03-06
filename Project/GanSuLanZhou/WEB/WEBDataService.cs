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
    class WEBDataService: IServiceWorker,ISonService
    {
        private WEBPandaPumpManager webPumpManager;

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
