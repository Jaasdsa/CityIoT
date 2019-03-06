using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace CityIoTPumpAlarm 
{
    class EnvChecker
    {
        //**** 环境检查工作者 *****

        // 系统环境检测者
        public static bool Check(out string errMsg)
        {
            errMsg = "";
            if (!Config.confPumpAlarm.EnvIsOkay)
            {
                errMsg = Config.confPumpAlarm.ErrMsg;
                return false;
            }
            if (!CheckPumpAlarmParam(out errMsg))
                return false;
            return true;
        }
        private static bool CheckPumpAlarmParam(out string errMsg)
        {
            errMsg = "";
            if (Config.confPumpAlarm == null)
            {
                errMsg = "配置对象为空对象";
                return false;
            }
            if (Config.confPumpAlarm.UpdateInterval == 0)
            {
                errMsg = "报警维护时间间隔不能为0";
                return false;
            }
            if (Config.confPumpAlarm.JZTimeOut == 0)
            {
                errMsg = "判定机组离线时间不能为0";
                return false;
            }
            if (Config.confPumpAlarm.CommandTimeoutSeconds == 0)
            {
                errMsg = "超时间隔不能为0";
                return false;
            }
            return true;
        }

    }
}
