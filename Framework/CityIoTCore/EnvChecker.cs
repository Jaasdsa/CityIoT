using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace CityIoTCore 
{
    class EnvChecker
    {
        //**** 环境检查工作者 *****

        // 系统环境检测者
        public static bool Check(EnvConfigInfo configInfo, out string errMsg)
        {
            errMsg = "";

            if (!configInfo.EnvIsOkay)
            {
                errMsg = configInfo.ErrMsg;
                return false;
            }
            if (!configInfo.confDataBaseInfo.CheckDBConnect(out errMsg))
                return false;
            return true;
        }
    }
}
