using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace CityWEBDataService 
{
    class EnvChecker
    {
        //**** 环境检查工作者 *****

        // 熊猫二供WEB服务环境环境检查器
        public static bool CheckPandaPumpWEB(out string errMsg)
        {
            errMsg = "";
            if (!Config.pandaPumpParam.EnvIsOkay)
            {
                errMsg = Config.pandaPumpParam.ErrMsg;
                return false;
            }
            if (!CheckPandaPumpWEBParam(out errMsg))
                return false;
            return true;
        }
        public static bool CheckPandaPumpScadaWEB(out string errMsg) 
        {
            errMsg = "";
            if (!Config.pandaPumpScadaParam.EnvIsOkay)
            {
                errMsg = Config.pandaPumpScadaParam.ErrMsg;
                return false;
            }
            if (!CheckPandaPumpScadaWEBParam(out errMsg))
                return false;
            return true;
        }
        public static bool CheckPandaYaLiWEB(out string errMsg)
        {
            errMsg = "";
            if (!Config.pandaYaLiParam.EnvIsOkay)
            {
                errMsg = Config.pandaYaLiParam.ErrMsg;
                return false;
            }
            if (!CheckPandaYaLiWEBParam(out errMsg))
                return false;
            return true;
        }
        public static bool CheckPandaCeDianWEB(out string errMsg)
        {
            errMsg = "";
            if (!Config.pandaCeDianParam.EnvIsOkay)
            {
                errMsg = Config.pandaCeDianParam.ErrMsg;
                return false;
            }
            if (!CheckPandaCeDianWEBParam(out errMsg))
                return false;
            return true;
        }

        // 检查熊猫二供WEB服务参数
        private static bool CheckPandaPumpWEBParam(out string errMsg)
        {
            errMsg = "";
            if (Config.pandaPumpParam == null)
            {
                errMsg = "配置对象为空对象s";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaPumpParam.AppKey))
            {
                errMsg = "AppKey不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaPumpParam.AppSecret))
            {
                errMsg = "AppSecret不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaPumpParam.GetTokenUrl))
            {
                errMsg = "TokenUrl不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaPumpParam.GetDataUrl))
            {
                errMsg = "DataUrl不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaPumpParam.UseName))
            {
                errMsg = "UseName不能为空";
                return false;
            }
            if (Config.pandaPumpParam.CollectInterval == 0)
            {
                errMsg = "采集间隔不能为0";
                return false;
            }
            if (Config.pandaPumpParam.SaveInterVal == 0)
            {
                errMsg = "存入历史间隔不能为0";
                return false;
            }
            if (Config.pandaPumpParam.CommandTimeoutSeconds == 0)
            {
                errMsg = "超时间隔不能为0";
                return false;
            }
            return true;
        }
        // 检查熊猫二供WEB-SCADA服务参数
        private static bool CheckPandaPumpScadaWEBParam(out string errMsg)
        {
            errMsg = "";
            if (Config.pandaPumpScadaParam == null)
            {
                errMsg = "配置对象为空对象";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaPumpScadaParam.AppKey))
            {
                errMsg = "AppKey不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaPumpScadaParam.AppSecret))
            {
                errMsg = "AppSecret不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaPumpScadaParam.GetTokenUrl))
            {
                errMsg = "TokenUrl不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaPumpScadaParam.GetDataUrl))
            {
                errMsg = "DataUrl不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaPumpScadaParam.UseName))
            {
                errMsg = "UseName不能为空";
                return false;
            }
            if (Config.pandaPumpScadaParam.CollectInterval == 0)
            {
                errMsg = "采集间隔不能为0";
                return false;
            }
            if (Config.pandaPumpScadaParam.SaveInterVal == 0)
            {
                errMsg = "存入历史间隔不能为0";
                return false;
            }
            if (Config.pandaPumpScadaParam.CommandTimeoutSeconds == 0)
            {
                errMsg = "超时间隔不能为0";
                return false;
            }
            return true;
        }
        // 检查熊猫监测压力点服务参数
        private static bool CheckPandaYaLiWEBParam(out string errMsg)
        {
            errMsg = "";
            if (Config.pandaYaLiParam == null)
            {
                errMsg = "配置对象为空对象";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaYaLiParam.AppKey))
            {
                errMsg = "AppKey不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaYaLiParam.AppSecret))
            {
                errMsg = "AppSecret不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaYaLiParam.GetTokenUrl))
            {
                errMsg = "TokenUrl不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaYaLiParam.GetDataUrl))
            {
                errMsg = "DataUrl不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaYaLiParam.UseName))
            {
                errMsg = "UseName不能为空";
                return false;
            }
            if (Config.pandaYaLiParam.CollectInterval == 0)
            {
                errMsg = "采集间隔不能为0";
                return false;
            }
            if (Config.pandaYaLiParam.SaveInterVal == 0)
            {
                errMsg = "存入历史间隔不能为0";
                return false;
            }
            if (Config.pandaYaLiParam.CommandTimeoutSeconds == 0)
            {
                errMsg = "超时间隔不能为0";
                return false;
            }
            return true;
        }
        // 检查熊猫综合测点服务参数
        private static bool CheckPandaCeDianWEBParam(out string errMsg)
        {
            errMsg = "";
            if (Config.pandaCeDianParam == null)
            {
                errMsg = "配置对象为空对象";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaCeDianParam.AppKey))
            {
                errMsg = "AppKey不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaCeDianParam.AppSecret))
            {
                errMsg = "AppSecret不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaCeDianParam.GetTokenUrl))
            {
                errMsg = "TokenUrl不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaCeDianParam.GetDataUrl))
            {
                errMsg = "DataUrl不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Config.pandaCeDianParam.UseName))
            {
                errMsg = "UseName不能为空";
                return false;
            }
            if (Config.pandaCeDianParam.CollectInterval == 0)
            {
                errMsg = "采集间隔不能为0";
                return false;
            }
            if (Config.pandaCeDianParam.SaveInterVal == 0)
            {
                errMsg = "存入历史间隔不能为0";
                return false;
            }
            if (Config.pandaCeDianParam.CommandTimeoutSeconds == 0)
            {
                errMsg = "超时间隔不能为0";
                return false;
            }
            return true;
        }
    }
}
