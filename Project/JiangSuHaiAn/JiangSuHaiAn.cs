using CityIoTCommand;
using CityIoTCore;
using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiangSuHaiAn
{
    // 江西南昌工程
    public class WebInterfaceExcute: CaseManagerInjection
    {
        private string projectName = "江苏海安工程";
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
                        LogManager.AppendDebug(ServerTypeName.Dispatch, projectName + "子服务管理器已停止");
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
            if (command == null)
            {
                CommandManager.MakeFail(projectName + "接受调度命令的异常命令", ref command);
                CommandManager.CompleteCommand(command);
                TraceManagerForDispatch.AppendErrMsg(projectName + "接受调度命令的异常命令");
                return;
            }
            if(command.sonServerType!= CommandServerType.Pump_OPC && command.sonServerType != CommandServerType.ReLoadJZData)
            {
                CommandManager.MakeFail(projectName + "错误的服务类型", ref command);
                CommandManager.CompleteCommand(command);
                TraceManagerForDispatch.AppendErrMsg(projectName + "错误的服务类型");
                return;
            }
            if (this.sonServiceManager != null && this.sonServiceManager.IsRuning
                && this.sonServiceManager.opcDataService != null && this.sonServiceManager.opcDataService.IsRuning)
            {
                this.sonServiceManager.opcDataService.ReceiveCommand(command);
            }
            else
            {
                CommandManager.MakeFail(projectName + "子服务管理器未运行", ref command);
                CommandManager.CompleteCommand(command);
                TraceManagerForDispatch.AppendErrMsg(projectName + "子服务管理器未运行");
                return;
            }
        }
    }
}
