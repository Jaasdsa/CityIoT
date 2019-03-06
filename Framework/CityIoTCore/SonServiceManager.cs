using CityPublicClassLib;
using CityOPCDataService;
using CityWEBDataService;
using System;
using System.Xml;
using CityUtils;
using CityLogService;
using CityIoTCommand;
using CityIoTPumpAlarm;
using CityHisVacuate;

namespace CityIoTCore
{
    class SonServiceManager : IServiceWorker
    {
        // 子服务
        private OPCPumpService opcPumpService;
        private OPCScadaService opcScadaService;

        private WEBPandaPumpService webPandaPumpService;
        private WEBPandaPumpSCADAService webPandaPumpScadaService;

        private WEBPandaYLSacdaService webPandaYLScadaService;
        private WEBPandaZHCDSacdaService webPandaZHCDScadaServcice;

        // 配置特殊项目子服务
        private CaseManagerInjection caseManagerInjection;

        public PumpAlarmService pumpAlarmService;
        public HisVacuate hisVacuate;

        public bool IsRuning { get; set; }


        public void Start(out string errMsg)
        {
            // 子服务启动失败，应该不影响其他子服务
            errMsg = "";

            try
            {
                // Pump-OPC通信子服务
                if (Config.configInfo.confSonOPCPumpDataService!=null && Config.configInfo.confSonOPCPumpDataService.IsNeedRun)
                {
                    opcPumpService = new OPCPumpService(Config.configInfo.confSonOPCPumpDataService);
                    opcPumpService.Start(out errMsg);
                    if (opcPumpService.IsRuning)
                        TraceManagerForOPC.AppendInfo("Pump-OPC通信子服务已经启动");
                    else
                    {
                        errMsg = "Pump-OPC通信子服务启动失败：" + errMsg;
                        TraceManagerForOPC.AppendErrMsg(errMsg);
                    }

                }
            }
            catch (Exception e) { TraceManagerForOPC.AppendErrMsg("Pump-OPC通信子服务启动失败：" + e.Message + "堆栈:" + e.StackTrace); }

            try
            {
                // Scada-OPC通信子服务
                if (Config.configInfo.confSonOPCScadaDataService != null && Config.configInfo.confSonOPCScadaDataService.IsNeedRun)
                {
                    opcScadaService = new OPCScadaService(Config.configInfo.confSonOPCScadaDataService);
                    opcScadaService.Start(out errMsg);
                    if (opcScadaService.IsRuning)
                        TraceManagerForOPC.AppendInfo("Scada-OPC通信子服务已经启动");
                    else
                    {
                        errMsg = "Scada-OPC通信子服务启动失败：" + errMsg;
                        TraceManagerForOPC.AppendErrMsg(errMsg);
                    }
                        
                }
            }
            catch (Exception e) { TraceManagerForOPC.AppendErrMsg("Scada-OPC通信子服务启动失败：" + e.Message + "堆栈:" + e.StackTrace); }

            try
            {
                // 二供WEB-pump通信子服务
                if (Config.configInfo.confSonWebPandaPumpDataService != null && Config.configInfo.confSonWebPandaPumpDataService.IsNeedRun)
                {
                    webPandaPumpService = new WEBPandaPumpService(Config.configInfo.confSonWebPandaPumpDataService);
                    webPandaPumpService.Start(out errMsg);
                    if (webPandaPumpService.IsRuning)
                        TraceManagerForWeb.AppendInfo("二供WEB通信子服务已经启动");
                    else
                    {
                        errMsg = "二供WEB通信子服务启动失败：" + errMsg;
                        TraceManagerForWeb.AppendErrMsg(errMsg);
                    }
                }
            }
            catch (Exception e) { TraceManagerForWeb.AppendErrMsg("二供WEB通信子服务启动失败：" + e.Message + "堆栈:" + e.StackTrace); }

            try
            {
                // WEB-pump-Scada 通信子服务
                if (Config.configInfo.confSonWebPandaPumpScadaDataService != null && Config.configInfo.confSonWebPandaPumpScadaDataService.IsNeedRun)
                {
                    webPandaPumpScadaService = new WEBPandaPumpSCADAService(Config.configInfo.confSonWebPandaPumpScadaDataService);
                    webPandaPumpScadaService.Start(out errMsg);
                    if (webPandaPumpScadaService.IsRuning)
                        TraceManagerForWeb.AppendInfo("WEB-pandaPump_Scada 通信子服务已经启动");
                    else
                    {
                        errMsg = "WEB-pandaPump_Scada 通信子服务启动失败：" + errMsg;
                        TraceManagerForWeb.AppendErrMsg(errMsg);
                    }
                }
            }
            catch (Exception e) { TraceManagerForWeb.AppendErrMsg("WEB-pandaPump_Scada 通信子服务启动失败：" + e.Message + "堆栈:" + e.StackTrace); }

            try
            {
                // SACADA—WEB-监测点通信子服务
                if (Config.configInfo.confSonWebPandaYLDataService != null && Config.configInfo.confSonWebPandaYLDataService.IsNeedRun)
                {
                    webPandaYLScadaService = new WEBPandaYLSacdaService(Config.configInfo.confSonWebPandaYLDataService);
                    webPandaYLScadaService.Start(out errMsg);
                    if (webPandaYLScadaService.IsRuning)
                        TraceManagerForWeb.AppendInfo("SACADA—WEB-监测点通信子服务已经启动");
                    else
                    {
                        errMsg = "SACADA—WEB-监测点通信子服务启动失败：" + errMsg;
                        TraceManagerForWeb.AppendErrMsg(errMsg);
                    }

                }
            }
            catch (Exception e) { TraceManagerForWeb.AppendErrMsg("SACADA—WEB-监测点通信子服务启动失败：" + e.Message + "堆栈:" + e.StackTrace); }

            try
            {
                // SACADA—WEB-综合测点通信子服务
                if (Config.configInfo.confSonWebPandaZHCDDataService != null && Config.configInfo.confSonWebPandaZHCDDataService.IsNeedRun)
                {
                    webPandaZHCDScadaServcice = new WEBPandaZHCDSacdaService(Config.configInfo.confSonWebPandaZHCDDataService);
                    webPandaZHCDScadaServcice.Start(out errMsg);
                    if (webPandaZHCDScadaServcice.IsRuning)
                        TraceManagerForWeb.AppendInfo("SACADA—WEB-综合测点通信子服务已经启动");
                    else
                    {
                        errMsg = "SACADA—WEB-综合测点通信子服务启动失败：" + errMsg;
                        TraceManagerForWeb.AppendErrMsg(errMsg);
                    }

                }
            }
            catch (Exception e) { TraceManagerForWeb.AppendErrMsg("SACADA—WEB-综合测点通信子服务启动失败：" + e.Message + "堆栈:" + e.StackTrace); }

            try
            {
                // 二供报警服务
                if (Config.configInfo.confSonCityIoTPumpAlarm != null && Config.configInfo.confSonCityIoTPumpAlarm.IsNeedRun)
                {
                    pumpAlarmService = new PumpAlarmService(Config.configInfo.confSonCityIoTPumpAlarm);
                    pumpAlarmService.Start(out errMsg);
                    if (pumpAlarmService.IsRuning)
                        TraceManagerForPumpAlarm.AppendInfo("二供报警服务已经启动");
                    else
                    {
                        errMsg = "二供报警服务启动失败：" + errMsg;
                        TraceManagerForPumpAlarm.AppendErrMsg(errMsg);
                    }
                }
            }
            catch (Exception e) { TraceManagerForPumpAlarm.AppendErrMsg("二供报警服务启动失败：" + e.Message + "堆栈:" + e.StackTrace); }

            try
            {
                // 历史抽稀服务
                if (Config.configInfo.confSonHisVacuateService != null && Config.configInfo.confSonHisVacuateService.IsNeedRun)
                {
                    hisVacuate = new HisVacuate(Config.configInfo.confSonHisVacuateService);
                    hisVacuate.Start(out errMsg);
                    if (hisVacuate.IsRuning)
                        TraceManagerForHisVacuate.AppendInfo("历史抽稀服务已经启动");
                    else
                    {
                        errMsg = "历史抽稀服务启动失败：" + errMsg;
                        TraceManagerForHisVacuate.AppendErrMsg(errMsg);
                    }
                }
            }
            catch (Exception e) { TraceManagerForHisVacuate.AppendErrMsg("历史抽稀服务启动失败：" + e.Message + "堆栈:" + e.StackTrace); }

            try
            {
                // 特殊项目子服务
                if (Config.configInfo.confSonProjectDataService != null && Config.configInfo.confSonProjectDataService.IsNeedRun)
                {
                    if (!Config.configInfo.confSonProjectDataService.EnvIsOkay)
                        TraceManagerForProject.AppendErrMsg("特殊项目动态库环境配置异常：" + Config.configInfo.confSonProjectDataService.ErrMsg);
                    else
                    {
                        caseManagerInjection = Interface.GetInjection(Config.configInfo.confSonProjectDataService.DllPath);
                        caseManagerInjection.Start(out errMsg);
                        if (caseManagerInjection.IsRuning)
                            TraceManagerForProject.AppendInfo(Config.configInfo.confSonProjectDataService.ProjectName + "服务已经启动");
                        else
                        {
                            errMsg = Config.configInfo.confSonProjectDataService.ProjectName + "服务启动失败：" + errMsg;
                            TraceManagerForProject.AppendErrMsg(errMsg);
                        }
                    }
                }
            }
            catch (Exception e) { TraceManagerForProject.AppendErrMsg(Config.configInfo.confSonProjectDataService.ProjectName + "服务启动失败：" + e.Message + "堆栈:" + e.StackTrace); }

            IsRuning = true;
        }
        public void Stop()
        {
            try
            {
                // 特殊项目子服务
                if (this.caseManagerInjection != null)
                {
                    this.caseManagerInjection.Stop();
                    if (this.caseManagerInjection.IsRuning)
                        TraceManagerForProject.AppendErrMsg(Config.configInfo.confSonProjectDataService.ProjectName + "服务停止失败");
                    else
                        TraceManagerForProject.AppendInfo(Config.configInfo.confSonProjectDataService.ProjectName + "服务已经全部停止");
                    this.caseManagerInjection = null;
                }
            }
            catch (Exception e) { TraceManagerForProject.AppendErrMsg(Config.configInfo.confSonProjectDataService.ProjectName + "服务停止失败：" + e.Message + "堆栈:" + e.StackTrace); }

            try
            {
                //  历史抽稀服务
                if (this.hisVacuate != null)
                {
                    this.hisVacuate.Stop();
                    if (this.hisVacuate.IsRuning)
                        TraceManagerForHisVacuate.AppendErrMsg("历史抽稀服务停止失败");
                    else
                        TraceManagerForHisVacuate.AppendInfo("历史抽稀服务已经全部停止");
                    this.hisVacuate = null;
                }
            }
            catch (Exception e) { TraceManagerForHisVacuate.AppendErrMsg("历史抽稀服务停止失败：" + e.Message + "堆栈:" + e.StackTrace); }

            try
            {
                //  二供报警服务
                if (this.pumpAlarmService != null)
                {
                    this.pumpAlarmService.Stop();
                    if (this.pumpAlarmService.IsRuning)
                        TraceManagerForPumpAlarm.AppendErrMsg( "二供报警服务停止失败");
                    else
                        TraceManagerForPumpAlarm.AppendInfo( "二供报警服务已经全部停止");
                    this.pumpAlarmService = null;
                }
            }
            catch (Exception e) { TraceManagerForPumpAlarm.AppendErrMsg( "二供报警服务停止失败：" + e.Message + "堆栈:" + e.StackTrace); }

            try
            {
                //  SACADA—WEB-综合测点通信子服务
                if (this.webPandaZHCDScadaServcice != null)
                {
                    this.webPandaZHCDScadaServcice.Stop();
                    if (this.webPandaZHCDScadaServcice.IsRuning)
                        TraceManagerForWeb.AppendErrMsg("SACADA-WEB-综合测点通信子服务停止失败");
                    else
                        TraceManagerForPumpAlarm.AppendInfo("SACADA—WEB-综合测点通信子服务已经全部停止");
                    this.webPandaZHCDScadaServcice = null;
                }
            }
            catch (Exception e) { TraceManagerForWeb.AppendErrMsg("SACADA—WEB-综合测点通信子服务停止失败：" + e.Message + "堆栈:" + e.StackTrace); }

            try
            {
                //  SACADA—WEB-监测点通信子服务
                if (this.webPandaYLScadaService != null)
                {
                    this.webPandaYLScadaService.Stop();
                    if (this.webPandaYLScadaService.IsRuning)
                        TraceManagerForWeb.AppendErrMsg("SACADA—WEB-监测点通信子服务停止失败");
                    else
                        TraceManagerForPumpAlarm.AppendInfo("SACADA—WEB-监测点通信子服务已经全部停止");
                    this.webPandaYLScadaService = null;
                }
            }
            catch (Exception e) { TraceManagerForWeb.AppendErrMsg("SACADA—WEB-监测点通信子服务停止失败：" + e.Message + "堆栈:" + e.StackTrace); }

            try
            {
                //  WEB-pandaPump_Scada 通信子服务
                if (this.webPandaPumpScadaService != null)
                {
                    this.webPandaPumpScadaService.Stop();
                    if (this.webPandaPumpScadaService.IsRuning)
                        TraceManagerForWeb.AppendErrMsg("二供WEB通信子服务停止失败");
                    else
                        TraceManagerForPumpAlarm.AppendInfo("二供WEB通信子服务已经全部停止");
                    this.webPandaPumpScadaService = null;
                }
            }
            catch (Exception e) { TraceManagerForWeb.AppendErrMsg("二供WEB通信子服务停止失败：" + e.Message + "堆栈:" + e.StackTrace); }

            try
            {
                //  二供WEB通信子服务
                if (this.webPandaPumpService != null)
                {
                    this.webPandaPumpService.Stop();
                    if (this.webPandaPumpService.IsRuning)
                        TraceManagerForWeb.AppendErrMsg("二供WEB通信子服务停止失败");
                    else
                        TraceManagerForPumpAlarm.AppendInfo("二供WEB通信子服务已经全部停止");
                    this.webPandaPumpService = null;
                }
            }
            catch (Exception e) { TraceManagerForWeb.AppendErrMsg("二供WEB通信子服务停止失败：" + e.Message + "堆栈:" + e.StackTrace); }

            try
            {
                //  Scada-OPC通信子服务
                if (this.opcScadaService != null)
                {
                    this.opcScadaService.Stop();
                    if (this.opcScadaService.IsRuning)
                        TraceManagerForOPC.AppendErrMsg("Scada-OPC通信子服务停止失败");
                    else
                        TraceManagerForPumpAlarm.AppendInfo("Scada-OPC通信子服务已经全部停止");
                    this.opcScadaService = null;
                }
            }
            catch (Exception e) { TraceManagerForOPC.AppendErrMsg("Scada-OPC通信子服务停止失败：" + e.Message + "堆栈:" + e.StackTrace); }

            try
            {
                // Pump-OPC通信子服务
                if (this.opcPumpService != null)
                {
                    this.opcPumpService.Stop();
                    if (this.opcPumpService.IsRuning)
                        TraceManagerForOPC.AppendErrMsg("Pump-OPC通信子服务停止失败");
                    else
                        TraceManagerForPumpAlarm.AppendInfo("Pump-OPC通信子服务已经全部停止");
                    this.opcPumpService = null;
                }
            }
            catch (Exception e) { TraceManagerForOPC.AppendErrMsg("Pump-OPC通信子服务停止失败：" + e.Message + "堆栈:" + e.StackTrace); }

            IsRuning = false;
        }

        public void ReceiveCommand(RequestCommand command)
        {
            if (command == null)
            {
                CommandManager.MakeFail("接受命令为空请求对象", ref command);
                CommandManager.CompleteCommand(command);
                TraceManagerForCommand.AppendErrMsg("接受命令为空请求对象");
                return;
            }
            if (!IsRuning)
            {
                MakeFailCommand(command, "子服务器管理器未运行");
                return;
            }
            switch (command.sonServerType)
            {
                case CommandServerType.Pump_OPC:
                    {
                        if (this.opcPumpService != null && this.opcPumpService.IsRuning)
                            this.opcPumpService.ReceiveCommand(command);
                        else
                        {
                            MakeFailCommand(command, "Pump-OPC 子服务器管理器未运行");
                            return;
                        }
                    }
                    break;
                case CommandServerType.Pump_WEB:
                    {
                        if (this.webPandaPumpService != null && this.webPandaPumpService.IsRuning)
                            this.webPandaPumpService.ReceiveCommand(command);
                        else
                        {
                            MakeFailCommand(command, "Pump-WEB 子服务器管理器未运行");
                            return;
                        }
                    }
                    break;
                case CommandServerType.SCADA_OPC:
                    {
                        if (this.opcScadaService != null && this.opcScadaService.IsRuning)
                            this.opcScadaService.ReceiveCommand(command);
                        else
                        {
                            MakeFailCommand(command, "SCADA-OPC 子服务器管理器未运行");
                            return;
                        }
                    }
                    break;
                case CommandServerType.YL_WEB:
                    {
                        if (this.webPandaYLScadaService != null && this.webPandaYLScadaService.IsRuning)
                            this.webPandaYLScadaService.ReceiveCommand(command);
                        else
                        {
                            MakeFailCommand(command, "YL_WEB 子服务器管理器未运行");
                            return;
                        }
                    }
                    break;
                case CommandServerType.ZHCD_WEB:
                    {
                        if (this.webPandaZHCDScadaServcice != null && this.webPandaZHCDScadaServcice.IsRuning)
                            this.webPandaZHCDScadaServcice.ReceiveCommand(command);
                        else
                        {
                            MakeFailCommand(command, "ZHCD_WEB 子服务器管理器未运行");
                            return;
                        }
                    }
                    break;
                case CommandServerType.SpecialProject:
                    {
                        if (this.caseManagerInjection != null && this.caseManagerInjection.IsRuning)
                            this.caseManagerInjection.ReceiveCommand(command);
                        else
                        {
                            MakeFailCommand(command, "项目子服务器管理器未运行");
                            return;
                        }
                    }
                    break;
                default:
                    {
                        MakeFailCommand(command, "错误的子服务类型");
                        return;
                    }
            }
        }
        private void MakeFailCommand(RequestCommand command, string errMsg)
        {
            CommandManager.MakeFail(errMsg, ref command);
            CommandManager.CompleteCommand(command);
            TraceManagerForCommand.AppendErrMsg(errMsg);
        }
    }
}
