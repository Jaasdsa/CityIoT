using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace DTUGenerator
{
    class EnvChecker
    {
        //**** 环境检查工作者 *****


        // 配置文件路径
        private static string path = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)
             , @".\config.xml");
        private static XmlDocument doc =new XmlDocument();

        // 系统环境检测者
        public static bool Check(out string errMsg)
        {
            errMsg = "";

            if (!IsHaveFile())
            {
                errMsg = "配置文件不存在！";
                return false;
            }

            doc.Load(path);

            if(!CheckAndCreateConnStr(doc,out errMsg))
            {
                return false;
            }

            if (!PingIP(Config.ip))
            {
                errMsg = "业务数据库网络不通，请保证网络通畅！";
                return false;
            }

            if (!checkConnect())
            {
                errMsg = "测试连接业务数据库失败！";
                return false;
            }

            if (!CheckAndCreateDTUGenerrateConfig(doc, out errMsg))
                return false;

            doc = null;
            return true;
        }        

        // 判断配置文件是否存在
        private static bool IsHaveFile()
        {
            if (File.Exists(path))
                return true;
            return false;
        }
        // 检查并创造连接字符串
        private static bool CheckAndCreateConnStr(XmlDocument doc, out string errMsg)
        {
            errMsg = "";
            try
            {
                //得到连接字符串节点
                XmlNode connStrNode = doc.SelectSingleNode("service/connStr");
                if (connStrNode == null)
                {
                    errMsg = "配置文件缺失连接字符串节点";
                    return false;
                }
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
            return DBUtil.GetConnectionTest(Config.DBConnectString);
        }
        // 检测IP  
        private static bool PingIP(string strIP)
        {
            if (!IsValidIP(strIP))
            {
                return false;
            }
            for(int i = 0; i < 3; i++)
            {
                // 只检测三次
                System.Net.NetworkInformation.Ping psender = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingReply prep = psender.Send(strIP, 500, Encoding.Default.GetBytes("afdafdadfsdacareqretrqtqeqrq8899tu"));
                if (prep.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    return true;
                }
            }
            return false;
        }
        // 验证IP  
        private static bool IsValidIP(string ip)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(ip, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}"))
            {
                string[] ips = ip.Split('.');
                if (ips.Length == 4 || ips.Length == 6)
                {
                    if (System.Int32.Parse(ips[0]) < 256 && System.Int32.Parse(ips[1]) < 256 & System.Int32.Parse(ips[2]) < 256 & System.Int32.Parse(ips[3]) < 256)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }

        // DTUServer配置文件
        public static bool CheckAndCreateDTUGenerrateConfig(XmlDocument doc, out string errMsg)
        {
            errMsg = "";
            try
            {
                //得到连接字符串节点
                XmlNode dtuServerNode = doc.SelectSingleNode("service/DTUGenerrator");
                if (dtuServerNode == null)
                {
                    errMsg = "配置文件缺失DTUGenerrator配置节点";
                    return false;
                }

                //ip节点
                if (!CheckNumNode(dtuServerNode, "heartTime", out Config.heartTime, out errMsg))
                    return false;
                if (!CheckNumNode(dtuServerNode, "plcDataTime", out Config.plcDataTime, out errMsg))
                    return false;
                if (!CheckNumNode(dtuServerNode, "refreshTime", out Config.plcRefreshTime, out errMsg))
                    return false;

                return true;
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return false;
            }
        }

        // 数字节点检查
        private static bool CheckNumNode(XmlNode souceNode,string checkNodeName, out int num, out string errMsg)
        {
            errMsg = "";
            num = 0;

            XmlNode node = souceNode.SelectSingleNode(checkNodeName);
            if (node == null)
            {
                errMsg = "配置文件缺失"+souceNode.Name+"下的"+ checkNodeName + "节点";
                return false;
            }
            if (!int.TryParse(node.InnerText, out num))
            {
                errMsg = "配置文件"+souceNode.Name+ "下的" + checkNodeName + "节点必须数字格式";
                return false;
            }            
            return true;
        }
        // 字符串节点检查
        private static bool CheckStringNode(XmlNode souceNode,string checkNodeName,out string targetStr,out string errMsg)
        {
            errMsg = "";
            targetStr = "";
            //ip节点
            XmlNode node = souceNode.SelectSingleNode(checkNodeName);
            if (node == null)
            {
                errMsg = "配置文件缺失" + souceNode.Name + "下的" + checkNodeName + "节点";
                return false;
            }
            targetStr = node.InnerText;
            return true;
        }
    }
}
