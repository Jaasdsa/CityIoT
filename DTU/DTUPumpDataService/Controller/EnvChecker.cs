using CityUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace DTUPumpDataService
{
    class EnvChecker
    {
        //**** 环境检查工作者 *****

        private static XmlDocument doc = new XmlDocument();

        // 系统环境检测者
        public static bool Check(out string errMsg)
        {
            errMsg = "";

            if (!XMLHelper.LoadDoc(Config.configFilePath, out doc, out errMsg))
                return false;

            if (!CheckAndCreateConnStr(out errMsg))
                return false;

            if (!IPTool.PingIP(Config.ip))
            {
                errMsg = "业务数据库网络不通，请保证网络通畅！";
                return false;
            }

            if (!checkConnect())
            {
                errMsg = "测试连接业务数据库失败！";
                return false;
            }

            if (!CheckAndCreateDTUConfig(out errMsg))
                return false;

            doc = null;
            return true;
        }

        // 检查并创造连接字符串
        private static bool CheckAndCreateConnStr( out string errMsg)
        {
            errMsg = "";
            try
            {
                //得到连接字符串节点
                if (!XMLHelper.ExistsNode(doc, "service/connStr", out XmlNode connStrNode, out errMsg))
                    return false;

                Config.ip = connStrNode.Attributes["ip"].Value;
                Config.user = connStrNode.Attributes["user"].Value;
                Config.password = connStrNode.Attributes["password"].Value;
                Config.serverName = connStrNode.Attributes["serverName"].Value;
                Config.CreateConnStr();

                return true;
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return false;
            }
        }
        // 测试数据库连接
        private static bool checkConnect()
        {
            return DBUtil.GetConnectionTest();
        }

        // DTUServer配置文件
        public static bool CheckAndCreateDTUConfig(out string errMsg)
        {
            errMsg = "";

            if (!XMLHelper.ExistsNode(doc, "service/DTUPumpDataService", out XmlNode dtuDSCNode, out errMsg))
                return false;
            if (!XMLHelper.LoadStringNode(dtuDSCNode, "ip", out DTUDSCConfig.ip, out errMsg))
                return false;
            if (!XMLHelper.LoadNumNode(dtuDSCNode, "port", out DTUDSCConfig.port, out errMsg))
                return false;
            if (!XMLHelper.LoadStringNode(dtuDSCNode, "type", out DTUDSCConfig.socketTypt, out errMsg))
                return false;
            if (!XMLHelper.LoadNumNode(dtuDSCNode, "dtuTimeoutNum", out DTUDSCConfig.timeoutNum, out errMsg))
                return false;
            if (!XMLHelper.LoadNumNode(dtuDSCNode, "dtuListUpdateTime", out DTUDSCConfig.dtuListUpdateTime, out errMsg))
                return false;
            if (!XMLHelper.LoadNumNode(dtuDSCNode, "dtuListConnectTime", out DTUDSCConfig.dtuListConnectDBTime, out errMsg))
                return false;
            if (!XMLHelper.LoadNumNode(dtuDSCNode, "pointListConnectDBTime", out DTUDSCConfig.pointListConnectDBTime, out errMsg))
                return false;
            if (!XMLHelper.LoadNumNode(dtuDSCNode, "commandConnectDBTime", out DTUDSCConfig.commandConnectDBTime, out errMsg))
                return false;

            return true;
        }
    }
}
