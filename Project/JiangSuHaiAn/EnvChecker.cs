using CityUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace JiangSuHaiAn
{
    class EnvChecker
    {
        //**** 环境检查工作者 *****

        private static XmlDocument doc =new XmlDocument();

        // 系统环境检测者
        public static bool Check(out string errMsg)
        {
            errMsg = "";

            if (!XMLHelper.LoadDoc(Config.projectConfigPath, out doc, out errMsg))
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

            return true;
        }        

        // 检查并创造连接字符串
        private static bool CheckAndCreateConnStr(out string errMsg)
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
    }
}
