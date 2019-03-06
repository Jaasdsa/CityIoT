using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace CityOPCDataService
{
    class EnvChecker
    {
        // 系统环境检测者
        public static bool CheckForPump(out string errMsg)
        {
            errMsg = "";
            if (!Config.pumpConfig.EnvIsOkay)
            {
                errMsg = Config.pumpConfig.ErrMsg;
                return false;
            }
            if (!CheckPumpConfig(out errMsg))
                return false;
            if(!CheckRegSvrDll(out errMsg))
                return false;
            return true;
        }
        public static bool CheckForScada(out string errMsg) 
        {
            errMsg = "";
            if (!Config.scadaConfig.EnvIsOkay)
            {
                errMsg = Config.scadaConfig.ErrMsg;
                return false;
            }
            if (!CheckScadaConfig(out errMsg))
                return false;
            if (!CheckRegSvrDll(out errMsg))
                return false;
            return true;
        }

        // 配置信息检查
        private static bool CheckPumpConfig(out string errMsg)
        {
            errMsg = "";
            if (Config.pumpConfig == null)
            {
                errMsg = "二供OPC配置对象为空对象";
                return false;
            }
            if (Config.pumpConfig.PointsCollectInterVal == 0)
            {
                errMsg = "点采集间隔不能为0";
                return false;
            }
            if (Config.pumpConfig.PointsSaveInterVal == 0)
            {
                errMsg = "点存入历史间隔不能为0";
                return false;
            }
            if (Config.pumpConfig.errorTimes == 0)
            {
                errMsg = "点表采集错误条目数不能为0";
                return false;
            }
            if (Config.pumpConfig.okayTimes == 0)
            {
                errMsg = "点表采集正确条目数不能为0";
                return false;
            }
            if (Config.pumpConfig.UpdateRate == 0)
            {
                errMsg = "OPC一级缓存间隔时间不能为0";
                return false;
            }
            if (Config.pumpConfig.ReadRate == 0)
            {
                errMsg = "OPC二级缓存刷新间隔不能为为0";
                return false;
            }
            if (Config.pumpConfig.commandTimeoutSeconds == 0)
            {
                errMsg = "OPC任务超时时间不能为0";
                return false;
            }
            return true;
        }
        private static bool CheckScadaConfig(out string errMsg)
        {
            errMsg = "";
            if (Config.scadaConfig.PointsCollectInterVal == 0)
            {
                errMsg = "点采集间隔不能为0";
                return false;
            }
            if (Config.scadaConfig.PointsSaveInterVal == 0)
            {
                errMsg = "点存入历史间隔不能为0";
                return false;
            }
            if (Config.scadaConfig.errorTimes == 0)
            {
                errMsg = "点表采集错误条目数不能为0";
                return false;
            }
            if (Config.scadaConfig.okayTimes == 0)
            {
                errMsg = "点表采集正确条目数不能为0";
                return false;
            }
            if (Config.scadaConfig.UpdateRate == 0)
            {
                errMsg = "OPC一级缓存间隔时间不能为0";
                return false;
            }
            if (Config.scadaConfig.ReadRate == 0)
            {
                errMsg = "OPC二级缓存刷新间隔不能为为0";
                return false;
            }
            if (Config.scadaConfig.commandTimeoutSeconds == 0)
            {
                errMsg = "OPC任务超时时间不能为0";
                return false;
            }
            return true;
        }

        // 动态库环境注册检查
        private static bool CheckRegSvrDll(out string errMsg)
        {
           string sourcePath =  XMLHelper.CurSystemPath+ @"Lib\OPCDAAuto.dll";
            //    string targetPath = Path.Combine(@"C:\Windows\SysWOW64", @"OPCDAAuto.dll");
            errMsg = "";
            string opcDAAUtoClsID = "28E68F9A-8D75-11D1-8DC3-3C302A000000";// 可F12到这个动态库查看它的GUID码
            try
            {
                if (!Environment.Is64BitOperatingSystem)
                    return true; // 32位系统无需注册
                if (COMHelper.IsRegister(opcDAAUtoClsID, out string filePath))
                {
                    // 系统已经注册该COM组件，判断路径是不是该程序自带这个dll
                    if (sourcePath.ToUpper() == filePath.ToUpper())
                        return true;
                    else
                    {
                        // 不是同一个路径，卸载掉，重新自带的dll，防止版本不一致
                        if (!COMHelper.IsRegsvr32(opcDAAUtoClsID, filePath, false, out errMsg))
                            errMsg = "未能成功卸载" + filePath + "这个版本dll，无法解决版本一致性问题";
                    }
                }
                if (!File.Exists(sourcePath))
                    errMsg = "未发现该文件，注册失败"+sourcePath;

                bool a = COMHelper.IsRegsvr32(opcDAAUtoClsID, sourcePath, true, out errMsg);
                return a;
            }
            catch (Exception e)
            {
                errMsg = e.Message+e.StackTrace;
                return false;
            }
        }

    }
}
