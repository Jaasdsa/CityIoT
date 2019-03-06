using CityIoTServiceManager;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace CityIoTServiceWatcher
{
    class EnvChecker
    {
        //**** 环境检查工作者 *****

        // 系统环境检测者
        public static bool Check(out string errMsg)
        {
            errMsg = "";
            if (!Config.envConfigInfo.EnvIsOkay)
            {
                errMsg = Config.envConfigInfo.ErrMsg;
                return false;
            }
            return true;
        }        

        public static Color GetColor(TraceType type)
        {
            Color color = Color.White;
            switch (type)
            {
                case TraceType.Error:
                    color = Color.Red;
                    break;
                case TraceType.Warning:
                    color = Color.Yellow;
                    break;
                case TraceType.Info:
                    color = Color.LimeGreen;
                    break;
                case TraceType.Debug:
                    color = Color.LightGray;
                    break;
                default:
                    color = Color.White;
                    break;
            }
            return color;
        }
    }
}
