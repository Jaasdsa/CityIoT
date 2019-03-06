using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace CityUtils
{
    public class XMLHelper
    {
        /// <summary>
        /// 返回程序根目录
        /// </summary>
        public static string CurSystemPath { get { return System.AppDomain.CurrentDomain.BaseDirectory; } }

        public static string GetRootFilePath(string rootFilePath)
        {
            return Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)
             , rootFilePath);
        }
        public static string PathAddPath(string beginPath,string endPath)
        {
            return Path.Combine(beginPath, endPath);
        }

        public static bool LoadDoc(string path,out XmlDocument doc,out string errMsg)
        {
            doc = null;
            errMsg = "";
            if (string.IsNullOrWhiteSpace(path))
            {
                errMsg = "文件路径不正确!" + path; ;
                return false;
            }
            if (!File.Exists(path))
            {
                errMsg = "文件未找到!" + path;
                return false;
            }
            doc = new XmlDocument();
            try
            {
                doc.Load(path);
                return true;
            }
            catch {
                errMsg = "读取xml文件出错" + path; ;
                return false;
            }

        } 
        // 判断节点有无
        public static bool ExistsNode(XmlDocument doc, string nodePath, out XmlNode node, out string errMsg)
        {
            errMsg = "";
            try
            {
                //得到连接字符串节点
                node = doc.SelectSingleNode(nodePath);
                if (node == null)
                {
                    errMsg = "配置文件缺失"+ nodePath + "节点";
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                node = null;
                return false;
            }
        }
        // 数字节点检查
        public static bool LoadNumNode(XmlNode souceNode, string checkNodeName, out int num, out string errMsg)
        {
            errMsg = "";
            num = 0;

            XmlNode node = souceNode.SelectSingleNode(checkNodeName);
            if (node == null)
            {
                errMsg = "配置文件缺失" + souceNode.Name + "下的" + checkNodeName + "节点";
                return false;
            }
            if (!int.TryParse(node.InnerText, out num))
            {
                errMsg = "配置文件" + souceNode.Name + "下的" + checkNodeName + "节点必须数字格式";
                return false;
            }
            return true;
        }
        // 字符串节点检查
        public static bool LoadStringNode(XmlNode souceNode, string checkNodeName, out string targetStr, out string errMsg)
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

        // 加载启动的项目
        public static bool LoadSolutionInfo(string solutionConfigPath, out string solutionName, out string errMsg)
        {
            solutionName = "";
            errMsg = "";
            XmlDocument doc = new XmlDocument();
            if (!XMLHelper.LoadDoc(solutionConfigPath, out doc, out errMsg))
                return false;
            if (!XMLHelper.ExistsNode(doc, "configuration", out XmlNode node, out errMsg))
                return false;
            if (!XMLHelper.LoadStringNode(node, "solutionName", out solutionName, out errMsg))
                return false;
            return true;
        }
        public static bool LoadProjectConfigPath(string solutionConfigPath, string solutionFolderName, out string projectConfigPath, out string errMsg)
        {
            projectConfigPath = "";
            errMsg = "";
            string configFilePath = new DirectoryInfo(solutionConfigPath).Parent.FullName + @"\项目\" + solutionFolderName + @"\projectConfig.xml";
            if (!File.Exists(configFilePath))
            {
                errMsg = solutionFolderName + "项目文件未找到!" + configFilePath;
                return false;
            }
            projectConfigPath = configFilePath;
            return true;
        }
        public static bool LoadProjectInfo(string projectConfigPath, out string projectName,out string errMsg)
        {
            projectName = "";
            errMsg = "";
            XmlDocument doc = new XmlDocument();
            if (!XMLHelper.LoadDoc(projectConfigPath, out doc, out errMsg))
                return false;
            if (!XMLHelper.ExistsNode(doc, "service", out XmlNode node, out errMsg))
                return false;
            if (!XMLHelper.LoadStringNode(node, "projectName", out projectName, out errMsg))
                return false;
            return true;
        }
        public static bool LoadProjectInfo(string configPath,string processPath,out string dllName,out string projectPath, out string errMsg)
        {
            dllName = "";
            projectPath = "";
            errMsg = "";
            try
            {
                if (string.IsNullOrWhiteSpace(configPath))
                {
                    errMsg = "框架配置文件路径未指定明确";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(processPath))
                {
                    errMsg = "程序根路径异常";
                    return false;
                }
                if (!XMLHelper.LoadDoc(configPath, out XmlDocument doc, out errMsg))
                    return false;
                if (!XMLHelper.ExistsNode(doc, "service/ConfCenter", out XmlNode node, out errMsg))
                    return false;
                if (node.ChildNodes.Count == 0)
                {
                    errMsg = "没有配置需要加载的项目节点projectName";
                    return false;
                }
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    XmlNode curNode = node.ChildNodes.Item(i);
                    if (curNode.Attributes["isActive"].Value.Trim() == "true") // 需要启动项目
                    {
                        // 项目动态库名称
                        string fileName = curNode.Attributes["dllName"].Value.Trim();
                        if (string.IsNullOrWhiteSpace(fileName))
                        {
                            errMsg = "未指明需要激活的动态库";
                            return false;
                        }
                        dllName = fileName;
                        // 项目配置文件
                        string configPathCahce = curNode.Attributes["configPath"].Value.Trim();
                        if (string.IsNullOrWhiteSpace(configPathCahce))
                        {
                            errMsg = "未指明项目所需的配置文件";
                            return false;
                        }
                        DBUtil.InstanceDBStr(PathAddPath(processPath,configPathCahce), out errMsg); // 动态实例化数据库的工具类

                        if (!string.IsNullOrWhiteSpace(errMsg))
                            return false;
                        projectPath = configPathCahce;
                        return true; // 只加载第一个匹配到项目
                    }
                }
            }
            catch (Exception e)
            {
                errMsg = "框架配置文件读取处理错误:" + e.Message;
            }
            return false;
        }

        // 加载日志的端口防止起不来，错误无法报出来
        public static bool LoadLogPort(string projectConfigPath,out int port,out string errMsg)
        {
            port = 0;
            XmlDocument doc;
            if (!XMLHelper.LoadDoc(projectConfigPath, out doc, out errMsg))
                return false;
            if (!XMLHelper.ExistsNode(doc, "service/CityIoTService/log", out XmlNode node, out errMsg))
                return false;
            if (!XMLHelper.LoadNumNode(node, "port", out port, out errMsg))
                return false;
            return true;
        }

    }
}
