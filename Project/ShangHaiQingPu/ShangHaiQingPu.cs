using CityIoTCore;
using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShangHaiQingPu
{
    // 上海青浦工程
    public class WebInterfaceExcute: CaseManagerInjection
    {
        private string projectName = "上海青浦工程";
        private SonServiceManager sonServiceManager;
        // 任务接口入口
        public override bool IsRuning { get; set; }

        public override void Start(out string errMsg)
        {
            errMsg = "";
            // 环境检查
            if (!EnvChecker.Check(out  errMsg))
                return;

            // 子服务管理器
            if (sonServiceManager != null)
                sonServiceManager.Stop();
            sonServiceManager = new SonServiceManager();
            sonServiceManager.Start(out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg) || sonServiceManager.IsRuning == false)
            {
                Stop();
                return;
            }
            LogManager.AppendInfo(ServerTypeName.Dispatch, projectName+"子服务管理器已经启动");

            IsRuning = true;
        }
        public override void Stop()
        {
            try
            {
                // 子服务管理器
                if (sonServiceManager != null)
                {
                    sonServiceManager.Stop();
                    if (!sonServiceManager.IsRuning)
                    {
                        LogManager.AppendDebug(ServerTypeName.Dispatch, projectName+"子服务管理器已停止");
                        this.sonServiceManager = null;
                    }
                    else
                        LogManager.AppendErrMsg(ServerTypeName.Dispatch, projectName + "子服务管理器停止失败");
                }
            }
            catch (Exception e)
            {
                LogManager.AppendErrMsg(ServerTypeName.Dispatch, projectName + "子服务管理器停止失败:" + e.Message);
            }

            IsRuning = false;
        }

        public override void ReceiveCommand(RequestCommand command)
        {

        }

    }
}
