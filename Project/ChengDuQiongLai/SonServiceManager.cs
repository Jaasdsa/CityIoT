﻿using CityIoTPumpAlarm;
using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ChengDuQiongLai
{
    class SonServiceManager : IServiceWorker
    {
        // 子服务
        public PumpAlarmService pumpAlarmService;
        public WEBDataService webDataService;

        // 子服务启动标志
        private int runOPCServiceFlag;
        private int runDTUServiceFlag;
        private int runPumpAlarmService;
        private int runWebDataService;

        public bool IsRuning { get; set; }

        public void Start(out string errMsg)
        {
            // 子服务启动失败，应该不影响其他子服务
            errMsg = "";
            string err = "";

            if (!LoadConfig(out errMsg))
                return;

            try
            {
                // web接入服务
                if (this.runWebDataService == 1)
                {
                    webDataService = new WEBDataService();
                    webDataService.Start(out err);
                    if (webDataService.IsRuning)
                        TraceManagerForWeb.AppendDebug("web接入服务已经全部启动");
                    else
                        errMsg = "二供web接入服务启动失败：" + err;
                }
            }
            catch (Exception e) { TraceManagerForWeb.AppendErrMsg("web接入服务启动失败：" + e.Message); }

            try
            {
                // 二供报警服务
                if (this.runPumpAlarmService == 1)
                {
                    pumpAlarmService = new PumpAlarmService(Config.projectConfigPath);
                    pumpAlarmService.Start(out err);
                    if (pumpAlarmService.IsRuning)
                        LogManager.AppendInfo(ServerTypeName.PumpAlarm, "二供报警服务已经全部启动");
                    else
                        errMsg = "二供报警服务启动失败：" + err;
                }
            }
            catch (Exception e) { TraceManagerForWeb.AppendErrMsg("二供报警服务启动失败：" + e.Message); }

            IsRuning = true;
        }

        public void Stop()
        {
            try
            {
                //  二供报警服务
                if (this.pumpAlarmService != null)
                {
                    this.pumpAlarmService.Stop();
                    if (this.pumpAlarmService.IsRuning)
                        LogManager.AppendErrMsg(ServerTypeName.Dispatch, "二供报警服务停止失败");
                    else
                        LogManager.AppendInfo(ServerTypeName.Dispatch, "二供报警服务已经全部停止");
                    this.pumpAlarmService = null;
                }
            }
            catch (Exception e) { LogManager.AppendErrMsg(ServerTypeName.Dispatch, "二供报警服务停止失败：" + e.Message); }

            try
            {
                // web接入服务
                if (this.webDataService != null)
                {
                    this.webDataService.Stop();
                    if (this.webDataService.IsRuning)
                        LogManager.AppendErrMsg(ServerTypeName.Dispatch, "web接入服务停止失败");
                    else
                        LogManager.AppendInfo(ServerTypeName.Dispatch, "web接入服务已经全部停止");
                    this.webDataService = null;
                }
            }
            catch (Exception e) { LogManager.AppendErrMsg(ServerTypeName.Dispatch, "web接入服务停止失败：" + e.Message); }

            IsRuning = false;
        }

        private bool LoadConfig(out string errMsg)
        {
            XmlDocument doc = new XmlDocument();

            if (!XMLHelper.LoadDoc(Config.projectConfigPath, out doc, out errMsg))
                return false;

            if (!XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode node, out errMsg))
                return false;
            if (!XMLHelper.LoadNumNode(node, "DTUPumpDataService", out runDTUServiceFlag, out errMsg))
                return false;
            if (!XMLHelper.LoadNumNode(node, "OPCDataService", out runOPCServiceFlag, out errMsg))
                return false;
            if (!XMLHelper.LoadNumNode(node, "CityIoTPumpAlarm", out runPumpAlarmService, out errMsg))
                return false;
            if (!XMLHelper.LoadNumNode(node, "CityWebDataService", out runWebDataService, out errMsg))
                return false;
            return true;
        }
    }
}

