using CityUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CityPublicClassLib
{
    /// <summary>
    /// 宿主类型
    /// </summary>
    public enum HostType
    {
        CS,  //本地CS调用
        IIS, //IIS服务调用
    }
    public class ConfigState
    {
        public string ErrMsg { get; set; } = "";
        public bool EnvIsOkay { get; set; } = false;
    }

    /// <summary>
    /// 单实例运行的环境信息类
    /// </summary>
    public class EnvConfigInfo:ConfigState
    {
        // 环境对象
        HostType _HostType;
        static EnvConfigInfo _SingleInstanceForIIS;
        static EnvConfigInfo _SingleInstanceForCS;

        #region 配置文件里静态对象
        public const string appKey = @"34h3rj3ri3jrt5y778934t5yfg3333h4h";
        public const string appSecret = @"45tnn5juyojgn3rn3fnn3t5j4to3fn6y64p3";
        public const string tokenUrl = @"https://new.s-water.cn/App/GetAccessToken";
        public const string dataUrl = @"https://new.s-water.cn/App/GetPumpData";
        public const string pumpControlUrl = @"https://new.s-water.cn/App/SetPumpControl";
        public const string ylUrl = @"https://new.s-water.cn/App/GetYLData";
        public const string zhcdUrl = @"https://new.s-water.cn/App/GetZHData";
        public const string userName = "项目名称";
        public const int updateInterval = 60;
        public const int collectInterval = 60;
        public const int saveInterVal = 5;
        public const int jzTimeOut = 30;
        public const int commandTimeoutSeconds = 15;
        public const int updateRate = 250;
        public const int readRate = 5;
        public const int errorTimes = 3;
        public const int okayTimes = 3;
        public const string endTime = "01:00:00";

        #endregion

        public enum Status
        {
            success = 1,
            fail = 0,
            cancel = 0
        }

        // 单实例
        /// <summary>
        /// 用于宿主为IIS环境配置信息单实例对象
        /// </summary>
        public static EnvConfigInfo SingleInstanceForIIS
        {
            get
            {
                if (_SingleInstanceForIIS == null)
                {
                    _SingleInstanceForIIS = new EnvConfigInfo() { _HostType = HostType.IIS };
                    _SingleInstanceForIIS.LoadConfigInfo();
                }
                return _SingleInstanceForIIS;
            }
        }
        /// <summary>
        /// 用于宿主为可执行程序环境配置信息单实例对象
        /// </summary>
        public static EnvConfigInfo SingleInstanceForCS
        {
            get
            {
                if (_SingleInstanceForCS == null)
                {
                    _SingleInstanceForCS = new EnvConfigInfo() { _HostType = HostType.CS };
                    _SingleInstanceForCS.LoadConfigInfo();
                }
                return _SingleInstanceForCS;
            }
        }

        // 更改完成通知
        public event Action evtConfigIsChanged;

        // 环境信息        
        public ConfFileInfo confFileInfo=new ConfFileInfo(); //防止配置异常，调用空对象异常
        public ConfProjectInfo confProjectInfo = new ConfProjectInfo();
        public ConfDataBaseInfo confDataBaseInfo = new ConfDataBaseInfo();

        // 基础功能配置
        public ConfLogServiceInfo confLogServiceInfo = new ConfLogServiceInfo();
        public ConfCommandServiceInfo confCommandServiceInfo = new ConfCommandServiceInfo();
        public ConfPublishServiceInfo confPublishServiceInfo = new ConfPublishServiceInfo();

        //子服务配置信息
        public ConfSonCityIoTPumpAlarm confSonCityIoTPumpAlarm = new ConfSonCityIoTPumpAlarm();

        public ConfSonOPCPumpDataService confSonOPCPumpDataService = new ConfSonOPCPumpDataService();
        public ConfSonOPCScadaDataService confSonOPCScadaDataService = new ConfSonOPCScadaDataService();

        public ConfSonWebPandaPumpDataService confSonWebPandaPumpDataService = new ConfSonWebPandaPumpDataService();
        public ConfSonWebPandaPumpScadaDataService confSonWebPandaPumpScadaDataService = new ConfSonWebPandaPumpScadaDataService();
        public ConfSonWebPandaYLDataService confSonWebPandaYLDataService = new ConfSonWebPandaYLDataService();
        public ConfSonWebPandaZHCDDataService confSonWebPandaZHCDDataService = new ConfSonWebPandaZHCDDataService();

        public ConfSonWebPandaControlService confSonWebPandaControlService = new ConfSonWebPandaControlService();
        public ConfSonProjectDataService confSonProjectDataService = new ConfSonProjectDataService();

        public ConfSonHisVacuateService confSonHisVacuateService = new ConfSonHisVacuateService();

        // 启动前环境检查-包括日志端口空闲检查
        public bool EnvCheckForBeforeRun(out string errMsg)
        {
            errMsg = "";
            if (!EnvIsOkay)
            {
                errMsg = ErrMsg;
                return false;
            }
            if (!confLogServiceInfo.CheckIsValidPort(out errMsg))
                return false;
            if (!confCommandServiceInfo.CheckIsValidPort(out errMsg))
                return false;
            //if (!confPublishServiceInfo.CheckIsValidPort(out errMsg))
            //    return false;
            return true;
        }

        // 加载配置文件信息
        private void LoadConfigInfo()
        {
            // 加载文件信息
            confFileInfo = ConfFileInfo.LoadConfigInfo(_HostType);
            if (!confFileInfo.EnvIsOkay)
            {
                EnvIsOkay = confFileInfo.EnvIsOkay;   //配置文件配置必须正常
                ErrMsg = confFileInfo.ErrMsg;
                return;
            }
            // 加载项目名称
            confProjectInfo = ConfProjectInfo.LoadProjectInfo(confFileInfo.ProjectConfigPath);
            if (!confProjectInfo.EnvIsOkay)
            {
                EnvIsOkay = confProjectInfo.EnvIsOkay;   //服务信息配置必须正常
                ErrMsg = confProjectInfo.ErrMsg;
                return;
            }
            // 加载数据库配置
            confDataBaseInfo = ConfDataBaseInfo.LoadDBInfo(confFileInfo.ProjectConfigPath);
            if (!confDataBaseInfo.EnvIsOkay)
            {
                EnvIsOkay = confDataBaseInfo.EnvIsOkay;   //数据库配置必须正常
                ErrMsg = confDataBaseInfo.ErrMsg;
                return;
            }

            // 加载日志配置
            confLogServiceInfo = ConfLogServiceInfo.LoadLogInfo(confFileInfo.ProjectConfigPath);
            if (!confLogServiceInfo.EnvIsOkay)
            {
                EnvIsOkay = confLogServiceInfo.EnvIsOkay;   //日志配置必须正常
                ErrMsg = confLogServiceInfo.ErrMsg;
                return;
            }

            // 加载任务配置
            confCommandServiceInfo = ConfCommandServiceInfo.LoadCommandInfo(confFileInfo.ProjectConfigPath);
            if (!confCommandServiceInfo.EnvIsOkay)
            {
                EnvIsOkay = confCommandServiceInfo.EnvIsOkay;   //任务配置必须正常
                ErrMsg = confCommandServiceInfo.ErrMsg;
                return;
            }

            // 加载数据发布配置
            confPublishServiceInfo = ConfPublishServiceInfo.LoadPublishInfo(confFileInfo.ProjectConfigPath);
            if (!confPublishServiceInfo.EnvIsOkay)
            {
                EnvIsOkay = confPublishServiceInfo.EnvIsOkay;   //数据发布配置必须正常
                ErrMsg = confPublishServiceInfo.ErrMsg;
                return;
            }

            //****子服务配置-子服务允许未配置****// 
            confSonCityIoTPumpAlarm = ConfSonCityIoTPumpAlarm.LoadInfo(confFileInfo.ProjectConfigPath);
            confSonOPCPumpDataService = ConfSonOPCPumpDataService.LoadInfo(confFileInfo.ProjectConfigPath);
            confSonOPCScadaDataService = ConfSonOPCScadaDataService.LoadInfo(confFileInfo.ProjectConfigPath);
            confSonWebPandaPumpDataService = ConfSonWebPandaPumpDataService.LoadInfo(confFileInfo.ProjectConfigPath);
            confSonWebPandaPumpScadaDataService = ConfSonWebPandaPumpScadaDataService.LoadInfo(confFileInfo.ProjectConfigPath);
            confSonWebPandaYLDataService = ConfSonWebPandaYLDataService.LoadInfo(confFileInfo.ProjectConfigPath);
            confSonWebPandaZHCDDataService = ConfSonWebPandaZHCDDataService.LoadInfo(confFileInfo.ProjectConfigPath);
            confSonWebPandaControlService = ConfSonWebPandaControlService.LoadInfo(confFileInfo.ProjectConfigPath);
            confSonProjectDataService = ConfSonProjectDataService.LoadInfo(confFileInfo.ProjectConfigPath);
            confSonHisVacuateService = ConfSonHisVacuateService.LoadInfo(confFileInfo.ProjectConfigPath);

            EnvIsOkay = true;
        }

        // 重新加载配置
        public void ReloadConfigInfo()
        {
            // 先清空缓存
            EnvIsOkay = false;
            ErrMsg = "";

            confFileInfo = new ConfFileInfo();
            confProjectInfo = new ConfProjectInfo();
            confDataBaseInfo = new ConfDataBaseInfo();

            confLogServiceInfo = new ConfLogServiceInfo();
            confCommandServiceInfo = new ConfCommandServiceInfo();
            confPublishServiceInfo = new ConfPublishServiceInfo();

            confSonCityIoTPumpAlarm = new ConfSonCityIoTPumpAlarm();
            confSonOPCPumpDataService = new ConfSonOPCPumpDataService();
            confSonOPCScadaDataService = new ConfSonOPCScadaDataService();
            confSonWebPandaPumpDataService = new ConfSonWebPandaPumpDataService();
            confSonWebPandaPumpScadaDataService = new ConfSonWebPandaPumpScadaDataService();
            confSonWebPandaYLDataService = new ConfSonWebPandaYLDataService();
            confSonWebPandaZHCDDataService = new ConfSonWebPandaZHCDDataService();
            confSonWebPandaControlService = new ConfSonWebPandaControlService();
            confSonProjectDataService = new ConfSonProjectDataService();
            confSonHisVacuateService = new ConfSonHisVacuateService();

            LoadConfigInfo();

            this.evtConfigIsChanged();
        }

        // 解决方案-增删查改
        public bool AddSolution(string solutionfolderName,out string errMsg)
        {
            bool r = ConfFileInfo.AddSolution(_HostType, solutionfolderName, out errMsg); 
            ReloadConfigInfo();
            return r;
        }
        public List<string> GetSolutionList()
        {
            return ConfFileInfo.GetSolutionList(_HostType);
        }
        //public bool UpdateSolution(string solutionfolderName, out string errMsg)
        //{
        //    bool r = ConfFileInfo.UpdateSolution(this.hostType, solutionfolderName, out errMsg);
        //    ReloadXMLInfo();
        //    return r;
        //}
        //public bool AddOrUpdateSolution(string solutionfolderName, out string errMsg)
        //{
        //    bool r = ConfFileInfo.AddOrUpdateSolution(this.hostType, solutionfolderName, out errMsg);
        //    ReloadXMLInfo();
        //    return r;
        //}
        public  bool DeleteSolution( string solutionName,out string errMsg)
        {
            bool r = ConfFileInfo.DeleteSolution(_HostType, solutionName, out errMsg);
            ReloadConfigInfo();
            return r;
        }
        public bool IsExistSolution(string solutionName, out string errMsg)
        {
            bool r = ConfFileInfo.IsExistSolution(_HostType, solutionName, out errMsg);
            return r;
        }
        public bool ChangeConfCenter(string solutionName, out string errMsg)
        {
            bool r = ConfFileInfo.ChangeConfCenter(solutionName, out errMsg);
            ReloadConfigInfo();
            return r;
        }

        //数据库配置-增删查改
        public List<string> GetDBList(ConfDataBaseInfo info, out string errMsg)
        {
            return ConfDataBaseInfo.GetDBList(info, out errMsg);
        }
        public bool CheckDBConnect(ConfDataBaseInfo info, out string errMsg)
        {
            bool r = ConfDataBaseInfo.CheckDBConnect(info, out errMsg);
            return r;
        }
        public bool UpdateDBInfo(ConfDataBaseInfo info, out string errMsg)
        {
            if (!confFileInfo.EnvIsOkay)
            {
                errMsg = confFileInfo.ErrMsg;
                return false;
            }
            string path = confFileInfo.ProjectConfigPath;
            bool r = ConfDataBaseInfo.UpdateDBInfo(info, path, out errMsg);
            ReloadConfigInfo();
            return r;
        }
        public List<ConnectionItem> GetDBHistroyList(out string errMsg)
        {
            return ConfDataBaseInfo.GetDBHistroyList(out errMsg);
        }
        public bool SaveDBInfo(ConfDataBaseInfo info, out string errMsg)
        {
            bool r = ConfDataBaseInfo.SaveDBInfo(info, out errMsg);
            return r;
        }
        public bool ClearDBHistoryInfo(out string errMsg)
        {
            return ConfDataBaseInfo.ClearDBHistoryInfo(out errMsg);
        }
        public bool DeleteSignleDBHistory(ConfDataBaseInfo info, out string errMsg)
        {
            return ConfDataBaseInfo.DeleteSignleDBHistory(info, out errMsg);
        }

        //日志配置-查改
        public bool UpdateLogInfo(ConfLogServiceInfo info, out string errMsg)
        {
            if (!confFileInfo.EnvIsOkay)
            {
                errMsg = confFileInfo.ErrMsg;
                return false;
            }
            string path = confFileInfo.ProjectConfigPath;
            bool r = ConfLogServiceInfo.UpdateLogInfo(info, path, out errMsg);
            ReloadConfigInfo();
            return r;
        }

        //命令配置-查改
        public bool UpdateCommandInfo(ConfCommandServiceInfo info, out string errMsg)
        {
            if (!confFileInfo.EnvIsOkay)
            {
                errMsg = confFileInfo.ErrMsg;
                return false;
            }
            string path = confFileInfo.ProjectConfigPath;
            bool r = ConfCommandServiceInfo.UpdateCommandInfo(info, path, out errMsg);
            ReloadConfigInfo();
            return r;
        }

        //发布配置-查改
        public bool UpdatePublishInfo(ConfPublishServiceInfo info, out string errMsg)
        {
            if (!confFileInfo.EnvIsOkay)
            {
                errMsg = confFileInfo.ErrMsg;
                return false;
            }
            string path = confFileInfo.ProjectConfigPath;
            bool r = ConfPublishServiceInfo.UpdatePublishInfo(info, path, out errMsg);
            ReloadConfigInfo();
            return r;
        }

        #region 子服务
        //获取当前解决方案的项目配置里runService有哪些
        public List<string> GetRunServiceList(string projectConfigPath, out string errMsg)
        {
            List<string> list = new List<string>();
            errMsg = "";

            if (!confFileInfo.EnvIsOkay)
            {
                errMsg = confFileInfo.ErrMsg;
                return list;
            }

            if (confSonOPCPumpDataService.IsNeedRun)
                list.Add("OPC二供接入服务");
            if (confSonOPCScadaDataService.IsNeedRun)
                list.Add("OPC-SCADA接入服务");
            if (confSonCityIoTPumpAlarm.IsNeedRun)
                list.Add("二供报警服务");
            if (confSonWebPandaPumpDataService.IsNeedRun)
                list.Add("WEB熊猫二供接入服务");
            if (confSonWebPandaPumpScadaDataService.IsNeedRun)
                list.Add("WEB熊猫二供SCADA接入服务");
            if (confSonWebPandaYLDataService.IsNeedRun)
                list.Add("WEB熊猫监测点接入服务");
            if (confSonWebPandaZHCDDataService.IsNeedRun)
                list.Add("WEB熊猫综合测点接入服务");
            if (confSonWebPandaControlService.IsNeedRun)
                list.Add("WEB熊猫控制服务");
            if (confSonProjectDataService.IsNeedRun)
                list.Add("项目的特殊服务");
            if (confSonHisVacuateService.IsNeedRun)
                list.Add("历史抽稀服务");
            return list;
        }
        //移除子服务
        public bool RemoveSonService(string serviceName, out string errMsg)
        {
            errMsg = "";
            switch (serviceName)
            {
                case "OPC二供接入服务":
                    RemoveSonServiceNode("OPCPumpDataService", out errMsg);
                    ReloadConfigInfo();
                    break;
                case "OPC-SCADA接入服务":
                    RemoveSonServiceNode("OPCScadaDataService", out errMsg);
                    ReloadConfigInfo();
                    break;
                case "二供报警服务":
                    RemoveSonServiceNode("CityIoTPumpAlarm", out errMsg);
                    ReloadConfigInfo();
                    break;
                case "WEB熊猫二供接入服务":
                    RemoveSonServiceNode("WebPandaPumpDataService", out errMsg);
                    ReloadConfigInfo();
                    break;
                case "WEB熊猫二供SCADA接入服务":
                    RemoveSonServiceNode("WebPandaPumpScadaDataService", out errMsg);
                    ReloadConfigInfo();
                    break;
                case "WEB熊猫监测点接入服务":
                    RemoveSonServiceNode("WebPandaYLDataService", out errMsg);
                    ReloadConfigInfo();
                    break;
                case "WEB熊猫综合测点接入服务":
                    RemoveSonServiceNode("WebPandaZHCDDataService", out errMsg);
                    ReloadConfigInfo();
                    break;
                case "WEB熊猫控制服务":
                    RemoveSonServiceNode("WebPandaControlService", out errMsg);
                    ReloadConfigInfo();
                    break;
                case "项目的特殊服务":
                    RemoveSonServiceNode("ProjectDataService", out errMsg);
                    ReloadConfigInfo();
                    break;
                case "历史抽稀服务":
                    RemoveSonServiceNode("HisVacuateService", out errMsg);
                    ReloadConfigInfo();
                    break;
            }

            return true;
        }

        public bool RemoveSonServiceNode(string name, out string errMsg)
        {
            XmlDocument doc = new XmlDocument();
            if (!confFileInfo.EnvIsOkay)
            {
                errMsg = confFileInfo.ErrMsg;
                return false;
            }
            if (!XMLHelper.LoadDoc(confFileInfo.ProjectConfigPath, out doc, out errMsg))
            {
                errMsg = "配置文件获取失败：" + errMsg;
                return false;
            }
            string nodePath = "service/RunServerList/" + name;
            XmlNode node = doc.SelectSingleNode(nodePath);
            XmlNode parentNode = doc.SelectSingleNode("service/RunServerList");
            parentNode.RemoveChild(node);
            nodePath = "service/" + name;
            node = doc.SelectSingleNode(nodePath);
            parentNode = doc.SelectSingleNode("service");
            parentNode.RemoveChild(node);
            doc.Save(confFileInfo.ProjectConfigPath);
            return true;
        }

        //子服务-OPCPump
        public bool SubmitSonOPCPumpData(ConfSonOPCPumpDataService info, out string errMsg)
        {
            if (!confFileInfo.EnvIsOkay)
            {
                errMsg = confFileInfo.ErrMsg;
                return false;
            }
            string path = confFileInfo.ProjectConfigPath;
            bool r = ConfSonOPCPumpDataService.SubmitSonOPCPumpData(info, path, out errMsg);
            ReloadConfigInfo();
            return r;
        }

        //子服务-OPCScada
        public bool SubmitSonOPCScadaData(ConfSonOPCScadaDataService info, out string errMsg)
        {
            if (!confFileInfo.EnvIsOkay)
            {
                errMsg = confFileInfo.ErrMsg;
                return false;
            }
            string path = confFileInfo.ProjectConfigPath;
            bool r = ConfSonOPCScadaDataService.SubmitSonOPCScadaData(info, path, out errMsg);
            ReloadConfigInfo();
            return r;
        }

        //子服务-PumpAlarm
        public bool SubmitSonPumpAlarmData(ConfSonCityIoTPumpAlarm info, out string errMsg)
        {
            if (!confFileInfo.EnvIsOkay)
            {
                errMsg = confFileInfo.ErrMsg;
                return false;
            }
            string path = confFileInfo.ProjectConfigPath;
            bool r = ConfSonCityIoTPumpAlarm.SubmitSonPumpAlarmData(info, path, out errMsg);
            ReloadConfigInfo();
            return r;
        }

        //子服务-SpecialProject
        public bool SubmitSonSpecialProjectData(ConfSonProjectDataService info, out string errMsg)
        {
            if(!confFileInfo.EnvIsOkay)
            {
                errMsg = confFileInfo.ErrMsg;
                return false;
            }
            string path = confFileInfo.ProjectConfigPath;
            bool r = ConfSonProjectDataService.SubmitSonSpecialProjectData(info, path, out errMsg);
            ReloadConfigInfo();
            return r;
        }

        //子服务-WEBPandaControl
        public bool SubmitSonWebPandaControlData(ConfSonWebPandaControlService info, out string errMsg)
        {
            if (!confFileInfo.EnvIsOkay)
            {
                errMsg = confFileInfo.ErrMsg;
                return false;
            }
            string path = confFileInfo.ProjectConfigPath;
            bool r = ConfSonWebPandaControlService.SubmitSonWebPandaControlData(info, path, out errMsg);
            ReloadConfigInfo();
            return r;
        }

        //子服务-WEBPandaPumpScada
        public bool SubmitSonWebPandaPumpScadaData(ConfSonWebPandaPumpScadaDataService info, out string errMsg)
        {
            if (!confFileInfo.EnvIsOkay)
            {
                errMsg = confFileInfo.ErrMsg;
                return false;
            }
            string path = confFileInfo.ProjectConfigPath;
            bool r = ConfSonWebPandaPumpScadaDataService.SubmitSonWebPandaPumpScadaData(info, path, out errMsg);
            ReloadConfigInfo();
            return r;
        }

        //子服务-WEBPandaPump
        public bool SubmitSonWebPandaPumpData(ConfSonWebPandaPumpDataService info, out string errMsg)
        {
            if (!confFileInfo.EnvIsOkay)
            {
                errMsg = confFileInfo.ErrMsg;
                return false;
            }
            string path = confFileInfo.ProjectConfigPath;
            bool r = ConfSonWebPandaPumpDataService.SubmitSonWebPandaPumpData(info, path, out errMsg);
            ReloadConfigInfo();
            return r;
        }

        //子服务-WEBPandaYL
        public bool SubmitSonWebPandaYLData(ConfSonWebPandaYLDataService info, out string errMsg)
        {
            if (!confFileInfo.EnvIsOkay)
            {
                errMsg = confFileInfo.ErrMsg;
                return false;
            }
            string path = confFileInfo.ProjectConfigPath;
            bool r = ConfSonWebPandaYLDataService.SubmitSonWebPandaYLData(info, path, out errMsg);
            ReloadConfigInfo();
            return r;
        }

        //子服务-WEBPandaZHCD
        public bool SubmitSonWebPandaZHCDData(ConfSonWebPandaZHCDDataService info, out string errMsg)
        {
            if (!confFileInfo.EnvIsOkay)
            {
                errMsg = confFileInfo.ErrMsg;
                return false;
            }
            string path = confFileInfo.ProjectConfigPath;
            bool r = ConfSonWebPandaZHCDDataService.SubmitSonWebPandaZHCDData(info, path, out errMsg);
            ReloadConfigInfo();
            return r;
        }

        //子服务-HisVacuate
        public bool SubmitSonHisVacuateData(ConfSonHisVacuateService info, out string errMsg)
        {
            if (!confFileInfo.EnvIsOkay)
            {
                errMsg = confFileInfo.ErrMsg;
                return false;
            }
            string path = confFileInfo.ProjectConfigPath;
            bool r = ConfSonHisVacuateService.SubmitSonHisVacuateData(info, path, out errMsg);
            ReloadConfigInfo();
            return r;
        }
        #endregion
    }

    #region****** 基础功能配置*****

    // 配置文件信息类
    public class ConfFileInfo: ConfigState
    {
        // 解决方案文件路径
        public string SolutionFilePath { get; set; }
        // 服务文件路径
        public string IotServerPath { get; set; }
        public string DispatchIotServerPath { get; set; }

        //解决方案文件夹名称
        public string SolutionName { get; set; }
        public string ProjectConfigPath { get; set; }

        static readonly string iotServerRootPath = "熊猫智慧水务物联网数据服务中心.exe";
        static readonly string dispatchIotServerRootPath = "熊猫智慧水务物联网数据服务调度中心.exe";

        // IIS 发布的WCF服务所需要的环境
        public static readonly string solutionFilePathForIIS = new DirectoryInfo(XMLHelper.CurSystemPath).Parent.FullName + @"\ConfCenter\物联网数据服务中心\Config.xml";
        public static readonly string iotServerFilePathForIIS = new DirectoryInfo(XMLHelper.CurSystemPath).Parent.FullName + @"\CityIoTBuild\" + iotServerRootPath;
        public static readonly string iotDispatchServerFilePathForIIS = new DirectoryInfo(XMLHelper.CurSystemPath).Parent.FullName + @"\CityIoTBuild\" + dispatchIotServerRootPath;

        // CS 窗体需要的环境
        public static readonly string solutionFilePathForCS = new DirectoryInfo(XMLHelper.CurSystemPath).Parent.FullName + @"\ConfCenter\物联网数据服务中心\Config.xml";
        public static readonly string iotServerFilePathForCS = Path.Combine(XMLHelper.CurSystemPath, iotServerRootPath);
        public static readonly string iotDispatchServerFilePathForCS = Path.Combine(XMLHelper.CurSystemPath, dispatchIotServerRootPath);

        // 项目配置文件路径
        public static string GerProjectConfigPath(string solutionConfigPath, string solutionFolderName) 
        {
            return new DirectoryInfo(solutionConfigPath).Parent.FullName + @"\项目\" + solutionFolderName + @"\projectConfig.xml";
        }

        /// <summary>
        /// 加载配置信息
        /// </summary>
        /// <param name="hostType"></param>
        /// <returns></returns>
        public static ConfFileInfo LoadConfigInfo(HostType hostType)
        {
            ConfFileInfo confFileInfo = new ConfFileInfo();
            switch (hostType)
            {
                case HostType.IIS:
                    {
                        confFileInfo.SolutionFilePath = ConfFileInfo.solutionFilePathForIIS;
                        confFileInfo.IotServerPath = ConfFileInfo.iotServerFilePathForIIS;
                        confFileInfo.DispatchIotServerPath = ConfFileInfo.iotDispatchServerFilePathForIIS;
                    }
                    break;
                case HostType.CS:
                    {
                        confFileInfo.SolutionFilePath = ConfFileInfo.solutionFilePathForCS;
                        confFileInfo.IotServerPath = ConfFileInfo.iotServerFilePathForCS;
                        confFileInfo.DispatchIotServerPath = ConfFileInfo.iotDispatchServerFilePathForCS;
                    }
                    break;
                default:
                    {
                        confFileInfo.ErrMsg = "未知服务宿主类型";
                        confFileInfo.EnvIsOkay = false;
                        return confFileInfo;
                    }
            }
            // 选择加载的需要项目名称
            if (!XMLHelper.LoadSolutionInfo(confFileInfo.SolutionFilePath, out string solutionName, out  string errMsg))
            {
                confFileInfo.ErrMsg = errMsg;
                confFileInfo.EnvIsOkay = false;
                return confFileInfo;
            }
            confFileInfo.SolutionName = solutionName;
            // 找到该项目配置文件
            confFileInfo.ProjectConfigPath = GerProjectConfigPath(confFileInfo.SolutionFilePath, solutionName);
            if (!File.Exists(confFileInfo.ProjectConfigPath))
            {
                confFileInfo.ErrMsg = confFileInfo.ProjectConfigPath + "项目文件未找到!";
                confFileInfo.EnvIsOkay = false;
                return confFileInfo;
            }

            // 正确返回
            confFileInfo.EnvIsOkay = true;
            return confFileInfo;
        }

        // 解决方案操作-增删查改
        public static bool AddSolution(HostType hostType, string solutionfolderName, out string errMsg)
        {
            errMsg = "";
            if (string.IsNullOrWhiteSpace(solutionfolderName))
            {
                errMsg = "添加的解决方案名称不能为空";
                return false;
            }
            ConfFileInfo confFileInfo = LoadConfigInfo(hostType);
            // 判断ConfCenter文件夹
            if(!Directory.Exists(new DirectoryInfo(XMLHelper.CurSystemPath).Parent.FullName + @"\ConfCenter"))
            {
                Directory.CreateDirectory(new DirectoryInfo(XMLHelper.CurSystemPath).Parent.FullName + @"\ConfCenter");
                Directory.CreateDirectory(new DirectoryInfo(XMLHelper.CurSystemPath).Parent.FullName + @"\ConfCenter\物联网数据服务中心");
            }
            // 添加解决方案配置
            if (!File.Exists(confFileInfo.SolutionFilePath))
            {
                XDocument docSoulution = new XDocument(
                                             new XDeclaration("1.0", "utf-8", "yes"),
                                             new XElement("configuration",
                                                         new XElement("solutionName", solutionfolderName)
                                                         )
                                             );
                docSoulution.Save(confFileInfo.SolutionFilePath);
            }
            //else
            //{
            //    if (!UpdateSolution(hostType, solutionfolderName, out errMsg))
            //        return false;
            //}
            // 配套项目文件初始配置
            confFileInfo = LoadConfigInfo(hostType);

            string newProjectFloderPath = new DirectoryInfo(confFileInfo.SolutionFilePath).Parent.FullName + @"\项目\" + solutionfolderName;
            string newProjectConfigPath = newProjectFloderPath + @"\projectConfig.xml";
            
            if (!File.Exists(newProjectConfigPath))
            {
                if(!Directory.Exists(newProjectFloderPath))
                {
                    Directory.CreateDirectory(newProjectFloderPath);
                }
            }
            XDocument doc = new XDocument(
                                         new XDeclaration("1.0", "utf-8", "yes"),
                                         new XElement("service",
                                            new XElement("projectName", solutionfolderName + "项目"),
                                            new XElement("connStr", new XAttribute("ip", "127.0.0.1"), new XAttribute("serverName", "数据库名称"), new XAttribute("user", "用户名"), new XAttribute("password", "密码")),
                                            new XElement("CityIoTService",
                                                new XElement("log",
                                                    new XElement("maintainTime", "05:00:00", new XAttribute("note", "每天的设置时间点维护日志")),
                                                    new XElement("logMaxSaveDays", "365", new XAttribute("note", "最大保存的天数")),
                                                    new XElement("ip", "127.0.0.1", new XAttribute("note", "日志发布服务启用的IP")),
                                                    new XElement("port", "9360", new XAttribute("note", "日志发布服务启用的端口")),
                                                    new XElement("userName", "admin", new XAttribute("note", "日志服务用户名")),
                                                    new XElement("password", "54F8820DDFDC5A5E", new XAttribute("note", "日志服务密码")),
                                                    new XElement("logNewsTopic", "logNews", new XAttribute("note", "日志订阅的主题"))
                                                            ),
                                                new XElement("iotCommand",
                                                    new XElement("ip", "Any", new XAttribute("note", "物联网控制服务IP")),
                                                    new XElement("port", "9361", new XAttribute("note", "物联网控制服务端口")),
                                                    new XElement("timeoutSeconds", "15", new XAttribute("note", "命令设定超时的秒数"))
                                                            ),
                                                new XElement("dataPublishServer",
                                                    new XElement("ip", "127.0.0.1", new XAttribute("note", "物联网数据发布服务IP")),
                                                    new XElement("port", "9362", new XAttribute("note", "物联网数据发布服务端口")),
                                                    new XElement("userName", "admin", new XAttribute("note", "数据发布服务用户名")),
                                                    new XElement("password", "54F8820DDFDC5A5E", new XAttribute("note", "数据发布服务密码"))
                                                            )
                                                         ),
                                            new XElement("RunServerList",
                                                      new XElement("HisVacuateService", "1", new XAttribute("dsc", "历史抽稀服务"), new XAttribute("note", "1-启动;0-不启动"))
                                                      ),
                                            new XElement("HisVacuateService",
                                                       new XElement("ScadaIsNeedRun","1"),
                                                       new XElement("PumpIsNeedRun","1"),
                                                       new XElement("EndTime", "01:00:00", new XAttribute("note", "截止时间"))
                                                        )

                                                     )
                                            
                                         );

            doc.Save(newProjectConfigPath);

            return true;
        }
        public static List<string> GetSolutionList(HostType hostType)
        {
            ConfFileInfo confFileInfo = LoadConfigInfo(hostType);

            List<string> confCenterList = new List<string>();
            string configFilePath = new DirectoryInfo(confFileInfo.SolutionFilePath).Parent.FullName + @"\项目\";
            DirectoryInfo theFolder = new DirectoryInfo(configFilePath);
            DirectoryInfo[] dirInfo = null;
            if (Directory.Exists(configFilePath))
            {
                dirInfo = theFolder.GetDirectories();
            }
            else
            {
                return confCenterList;
            }
            //遍历文件夹
            foreach (DirectoryInfo NextFolder in dirInfo)
            {
                confCenterList.Add(NextFolder.Name);
            }
            return confCenterList;
        }
        //public static bool UpdateSolution(HostType hostType,string solutionfolderName, out string errMsg)
        //{
        //    ConfFileInfo confFileInfo = LoadConfigInfo(hostType);

        //    XmlDocument doc = new XmlDocument();
        //    if (string.IsNullOrWhiteSpace(confFileInfo.SolutionFilePath))
        //    {
        //        errMsg = "框架配置文件路径未指定明确";
        //        return false;
        //    }
        //    if (!XMLHelper.LoadDoc(confFileInfo.SolutionFilePath, out doc, out errMsg))
        //    {
        //        errMsg = "配置文件获取失败：" + errMsg;
        //        return false;
        //    }
        //    if (!XMLHelper.ExistsNode(doc, "configuration/solutionName", out XmlNode node, out errMsg))
        //    {
        //        errMsg = "获取配置文件项目节点失败：" + errMsg;
        //        return false;
        //    }
        //    if (string.IsNullOrWhiteSpace(solutionfolderName))
        //    {
        //        errMsg = "没有配置当前解决方案名称";
        //        return false;
        //    }
        //    node.InnerText = solutionfolderName;
        //    doc.Save(confFileInfo.SolutionFilePath);

        //    return true;
        //}
        //public static bool AddOrUpdateSolution(HostType hostType, string solutionfolderName, out string errMsg)
        //{
        //    errMsg = "";
        //    ConfFileInfo confFileInfo = LoadConfigInfo(hostType);


        //    return true;
        //}
        public static bool DeleteSolution(HostType hostType, string solutionName, out string errMsg)
        {
            errMsg = "";
            try
            {
                ConfFileInfo confFileInfo = LoadConfigInfo(hostType);
                if (!confFileInfo.EnvIsOkay)
                {
                    errMsg = "删除解决方案:" + solutionName + "失败:"+confFileInfo.ErrMsg;
                    return false;
                }
                //if(GetSolutionList(hostType).Count == 1) //删除的是最后一个解决方案
                //{
                //    errMsg = "只剩下最后一个解决方案!";
                //    return false;
                //}
                // 解决方案移除
                XmlDocument doc = new XmlDocument();
                doc.Load(confFileInfo.SolutionFilePath);
                string curSolutionName = doc.SelectSingleNode("configuration/solutionName").InnerText;
                List<string> soluList = GetSolutionList(hostType);
                if (solutionName == curSolutionName)
                {
                    //soluList.Remove(solutionName); //删除该名称，取集合中第一个作为当前解决方案
                    doc.SelectSingleNode("configuration/solutionName").InnerText = "";
                    doc.Save(confFileInfo.SolutionFilePath);
                }               
                //要删除的项目配置文件夹路径
                string deleteConfigPath = new DirectoryInfo(confFileInfo.SolutionFilePath).Parent.FullName + @"\项目\" + solutionName;
                // 项目配置文件删除
                FileUtil.DeleteDirectory(deleteConfigPath);
                // 检查删除是否成功
                if (!Directory.Exists(deleteConfigPath))
                    return true;
                else
                {
                    errMsg = "删除解决方案:"+solutionName+"失败";
                    return false;
                }
            }
            catch(Exception e)
            {
                errMsg = "删除解决方案:" + solutionName + "异常"+e.Message;
                return false;
            }
        }

        public static bool IsExistSolution(HostType hostType, string solutionName, out string errMsg)
        {
            errMsg = "";
            try
            {
                List<string> list = GetSolutionList(hostType);
                if (list.Contains(solutionName))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                errMsg = "查找解决方案:" + solutionName + "异常" + e.Message;
                return false;
            }
        }

        public static bool ChangeConfCenter(string solutionfolderName, out string errMsg)
        {
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrWhiteSpace(solutionFilePathForCS))
            {
                errMsg = "框架配置文件路径未指定明确";
                return false;
            }
            if (!XMLHelper.LoadDoc(solutionFilePathForCS, out doc, out errMsg))
            {
                errMsg = "配置文件获取失败：" + errMsg;
                return false;
            }
            if (!XMLHelper.ExistsNode(doc, "configuration/solutionName", out XmlNode node, out errMsg))
            {
                errMsg = "获取配置文件项目节点失败：" + errMsg;
                return false;
            }
            if (string.IsNullOrWhiteSpace(solutionfolderName))
            {
                errMsg = "没有配置当前解决方案名称";
                return false;
            }
            node.InnerText = solutionfolderName;
            doc.Save(solutionFilePathForCS);

            return true;
        }      
    }
    // 项目服务名称
    public class ConfProjectInfo : ConfigState
    {
        // 环境解决方案信息
        static readonly string serverNameHead = "PandaCityIoTService-";
        static readonly string dispatchServerNameHead = "PandaCityIoTDispatchService-";
        static readonly string serverDescribeHead = "熊猫智慧水务---" + "物联网数据服务中心-";
        static readonly string dispatchServerDescribeHead = "熊猫智慧水务---" + "物联网数据服务调度中心-";

        public string ProjectName { get; set; }

        public string ServerName { get; set; }
        public string ServerDescribe { get; set; }

        public string DispatchServerName { get; set; }
        public string DispatchServerDescribe { get; set; }

        // 根据项目名称得到服务信息
        private static string GetServerName(string projectName)
        {
            return serverNameHead + projectName;
        }
        private static string GetServerDescribe(string projectName)
        {
            return serverDescribeHead + projectName;
        }

        // 根据项目名称得到调度服务信息
        private static string GetDispatchServerName(string projectName)
        {
            return dispatchServerNameHead + projectName;
        }
        private static string GetDispatchServerDescribe(string projectName)
        {
            return dispatchServerDescribeHead + projectName;
        }

        public static ConfProjectInfo LoadProjectInfo(string projectConfigPath)  
        {
            ConfProjectInfo confProjectInfo = new ConfProjectInfo();
            if (string.IsNullOrWhiteSpace(projectConfigPath))
            {
                confProjectInfo.ErrMsg = "未知的项目配置文件路径";
                confProjectInfo.EnvIsOkay = false;
                return confProjectInfo;
            }
            // 找到该项目配置文件
            if (!XMLHelper.LoadProjectInfo(projectConfigPath, out string projectName, out string errMsg))
            {
                confProjectInfo.ErrMsg = errMsg;
                confProjectInfo.EnvIsOkay = false;
                return confProjectInfo;
            }
            confProjectInfo.ProjectName = projectName;
            // 合成服务信息
            // 核心服务信息
            confProjectInfo.ServerName = GetServerName(confProjectInfo.ProjectName);
            confProjectInfo.ServerDescribe = GetServerDescribe(confProjectInfo.ProjectName);
            // 调度服务信息
            confProjectInfo.DispatchServerName = GetDispatchServerName(confProjectInfo.ProjectName);
            confProjectInfo.DispatchServerDescribe =GetDispatchServerDescribe(confProjectInfo.ProjectName);

            confProjectInfo.EnvIsOkay = true;
            return confProjectInfo;
        }
        
    }
    // 数据库配置
    public class ConfDataBaseInfo : ConfigState
    {
        public string IP { get; set; }
        public string ServerName { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ConnectString { get; set; }

        private  string CreateConnStr(ConfDataBaseInfo info)
        {
            ConnectString = string.Format("server={0};database='{1}';User id={2};password={3};Integrated Security=false", info.IP, info.ServerName, info.User, info.Password);
            return ConnectString;
        }


        // 测试数据库连接
        private static bool checkConnect(ConfDataBaseInfo info)
        {
            return DBUtil.TestDBConnection(info.IP, info.ServerName, info.User, info.Password);
        }
        public bool CheckDBConnect(out string errMsg)
        {
            return CheckDBConnect(this, out errMsg);
        }
        public static bool CheckDBConnect(ConfDataBaseInfo info, out string errMsg)
        {
            errMsg = "";

            // 检查配置
            //if (!IPTool.PingIP(info.IP))
            //{
            //    errMsg = "业务数据库网络不通，请保证网络通畅!";
            //    return false;
            //}

            if (!checkConnect(info))
            {
                errMsg = "测试连接业务数据库失败!";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 加载数据库连接配置
        /// </summary>
        /// <param name="projectConfigPath"></param>
        /// <returns></returns>
        public static ConfDataBaseInfo LoadDBInfo(string projectConfigPath)
        {
            ConfDataBaseInfo info = new ConfDataBaseInfo();
            if (string.IsNullOrWhiteSpace(projectConfigPath))
            {
                info.ErrMsg = "未知的项目配置文件路径";
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadDoc(projectConfigPath, out XmlDocument doc, out string errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }

            // 加载配置
            try
            {
                //得到连接字符串节点
                if (!XMLHelper.ExistsNode(doc, "service/connStr", out XmlNode connStrNode, out errMsg))
                {
                    info.ErrMsg = errMsg;
                    info.EnvIsOkay = false;
                    return info;
                }

                info.IP = connStrNode.Attributes["ip"].Value;
                info.User = connStrNode.Attributes["user"].Value;
                info.Password = connStrNode.Attributes["password"].Value;
                info.ServerName = connStrNode.Attributes["serverName"].Value;
                info.ConnectString= info.CreateConnStr(info);
            }
            catch (Exception e)
            {
                info.ErrMsg ="读取数据库配置失败:"+ e.Message+"堆栈:"+e.StackTrace;
                info.EnvIsOkay = false;
                return info;
            }

            // 初始化工具类
            DBUtil.InstanceDBStr(info.ConnectString);

            info.EnvIsOkay = true;
            return info;
        }

        //获取数据库下所有表名
        public static List<string> GetDBList(ConfDataBaseInfo info, out string errMsg)
        {
            List<string> list = new List<string>();
            list = DBUtil.GetDataBaseList(info.IP, info.User, info.Password, out errMsg);
            return list;
        }

        //修改配置文件数据库信息
        public static bool UpdateDBInfo(ConfDataBaseInfo info, string path, out string errMsg)
        {
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
            {
                errMsg = "框架配置文件路径未指定明确";
                return false;
            }
            if (!XMLHelper.LoadDoc(path, out doc, out errMsg))
            {
                errMsg = "配置文件获取失败：" + errMsg;
                return false;
            }
            if (!XMLHelper.ExistsNode(doc, "service/connStr", out XmlNode node, out errMsg))
            {
                errMsg = "获取配置文件数据库节点失败：" + errMsg;
                return false;
            }
            node.Attributes["ip"].Value = info.IP;
            node.Attributes["serverName"].Value = info.ServerName;
            node.Attributes["user"].Value = info.User;
            node.Attributes["password"].Value = info.Password;
            doc.Save(path);
            return true;
        }

        //获取历史记录
        public static List<ConnectionItem> GetDBHistroyList(out string errMsg)
        {
            List<ConnectionItem> list = new List<ConnectionItem>();
            list = JsonHelper.GetConnHistroy(out errMsg);
            return list;
        }

        //保存连接
        public static bool SaveDBInfo(ConfDataBaseInfo info, out string errMsg)
        {
            if(CheckDBConnect(info, out errMsg))
            {
                return JsonHelper.SaveConnection(info.IP, info.ServerName, info.User, info.Password, out errMsg);
            }
            else
            {
                return false;
            }           
        }

        //清除历史信息
        public static bool ClearDBHistoryInfo(out string errMsg)
        {
            return JsonHelper.DelHistory(out errMsg);
        }

        //删除单条历史记录
        public static bool DeleteSignleDBHistory(ConfDataBaseInfo info, out string errMsg)
        {
            return JsonHelper.DelSingleHistory(info.IP, info.ServerName, info.User, info.Password, out errMsg);
        }
    }
    // 日志服务配置
    public class ConfLogServiceInfo : ConfigState 
    {
        public string ManintainTime { get; set; }//每天日志维护的时间
        public int LogMaxSaveDays { get; set; }//日志保存的最大天数 

        public string IP { get; set; }
        public int Port { get; set; } 
        public string UserName { get; set; }
        public string Password { get; set; }
        public string NewsTopic { get; set; }

        public string filePath = XMLHelper.CurSystemPath + @"Log\CityIoTServerLog.db";
        public string tableName = "Log";

        public static ConfLogServiceInfo LoadLogInfo(string projectConfigPath)
        {
            ConfLogServiceInfo info = new ConfLogServiceInfo();
            if (string.IsNullOrWhiteSpace(projectConfigPath))
            {
                info.ErrMsg = "未知的项目配置文件路径";
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadDoc(projectConfigPath, out XmlDocument doc, out string errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.ExistsNode(doc, "service/CityIoTService/log", out XmlNode node, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadStringNode(node, "maintainTime", out string manintainTime, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.ManintainTime = manintainTime;
            if (!XMLHelper.LoadNumNode(node, "logMaxSaveDays", out int logMaxSaveDays, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.LogMaxSaveDays = logMaxSaveDays;
            if (!XMLHelper.LoadStringNode(node, "ip", out string ip, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.IP = ip;
            if (!XMLHelper.LoadNumNode(node, "port", out int port, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.Port = port;
            if (!XMLHelper.LoadStringNode(node, "userName", out string userName, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.UserName = userName;
            if (!XMLHelper.LoadStringNode(node, "password", out string password, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.Password = password;
            if (!XMLHelper.LoadStringNode(node, "logNewsTopic", out string logNewsTopic, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.NewsTopic = logNewsTopic;

            info.EnvIsOkay = true;
            return info;
        }

        public bool CheckIsValidPort(out string errMsg)
        {
            errMsg = "";
            if (!IPTool.IsValidPort(this.Port))
            {
                errMsg = "日志服务器的发布端口被占用，请更换端口号,端口:" + Port;
                return false;
            }
            return true;
        }

        public static bool UpdateLogInfo(ConfLogServiceInfo info, string path, out string errMsg)
        {         
            if (!IPTool.IsValidPort(info.Port))
            {
                errMsg = "日志服务器的发布端口被占用，请更换端口号!";
                return false;
            }
        
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
            {
                errMsg = "框架配置文件路径未指定明确";
                return false;
            }
            if (!XMLHelper.LoadDoc(path, out doc, out errMsg))
            {
                errMsg = "配置文件获取失败：" + errMsg;
                return false;
            }
            if (!XMLHelper.ExistsNode(doc, "service/CityIoTService/log", out XmlNode node, out errMsg))
            {
                errMsg = "获取配置文件日志节点失败：" + errMsg;
                return false;
            }
            foreach(XmlNode childNode in node.ChildNodes)
            {
                if(childNode.Name == "maintainTime")
                {
                    childNode.InnerText = info.ManintainTime;
                }
                if(childNode.Name == "logMaxSaveDays")
                {
                    childNode.InnerText = info.LogMaxSaveDays.ToString();
                }
                if(childNode.Name == "port")
                {
                    childNode.InnerText = info.Port.ToString();
                }
            }
            doc.Save(path);
            return true;
        }
    }
    // 任务服务配置
    public class ConfCommandServiceInfo : ConfigState
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public int timeoutSeconds { get; set; }

        public bool CheckIsValidPort(out string errMsg)
        {
            errMsg = "";
            if (!IPTool.IsValidPort(this.Port))
            {
                errMsg = "任务服务器的发布端口被占用，请更换端口号,端口:" + Port;
                return false;
            }
            return true;
        }

        public static ConfCommandServiceInfo LoadCommandInfo(string projectConfigPath)
        {
            ConfCommandServiceInfo info = new ConfCommandServiceInfo();
            if (string.IsNullOrWhiteSpace(projectConfigPath))
            {
                info.ErrMsg = "未知的项目配置文件路径";
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadDoc(projectConfigPath, out XmlDocument doc, out string errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.ExistsNode(doc, "service/CityIoTService/iotCommand", out XmlNode node, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadStringNode(node, "ip", out string ip, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.IP = ip;
            if (ip.ToUpper() == "ANY")
                info.IP = "127.0.0.1";
            if (!XMLHelper.LoadNumNode(node, "port", out int port, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.Port = port;
            if (!XMLHelper.LoadNumNode(node, "timeoutSeconds", out int timeoutSeconds, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.timeoutSeconds = timeoutSeconds;

            info.EnvIsOkay = true;
            return info;
        }

        public static bool UpdateCommandInfo(ConfCommandServiceInfo info, string path, out string errMsg)
        {
            if (!IPTool.IsValidPort(info.Port))
            {
                errMsg = "命令服务器的发布端口被占用，请更换端口号!";
                return false;
            }

            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
            {
                errMsg = "框架配置文件路径未指定明确";
                return false;
            }
            if (!XMLHelper.LoadDoc(path, out doc, out errMsg))
            {
                errMsg = "配置文件获取失败：" + errMsg;
                return false;
            }
            if (!XMLHelper.ExistsNode(doc, "service/CityIoTService/iotCommand", out XmlNode node, out errMsg))
            {
                errMsg = "获取配置文件命令节点失败：" + errMsg;
                return false;
            }
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name == "port")
                {
                    childNode.InnerText = info.Port.ToString();
                }
                if(childNode.Name == "timeoutSeconds")
                {
                    childNode.InnerText = info.timeoutSeconds.ToString();
                }
            }
            doc.Save(path);
            return true;

        }
    }
    // 发布服务配置
    public class ConfPublishServiceInfo : ConfigState
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public bool CheckIsValidPort(out string errMsg)
        {
            errMsg = "";
            if (!IPTool.IsValidPort(this.Port))
            {
                errMsg = "数据发布服务器的发布端口被占用，请更换端口号,端口:" + Port;
                return false;
            }
            return true;
        }

        public static ConfPublishServiceInfo LoadPublishInfo(string projectConfigPath)
        {
            ConfPublishServiceInfo info = new ConfPublishServiceInfo();
            if (string.IsNullOrWhiteSpace(projectConfigPath))
            {
                info.ErrMsg = "未知的项目配置文件路径";
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadDoc(projectConfigPath, out XmlDocument doc, out string errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.ExistsNode(doc, "service/CityIoTService/dataPublishServer", out XmlNode node, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadStringNode(node, "ip", out string ip, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.IP = ip;
            if (!XMLHelper.LoadNumNode(node, "port", out int port, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.Port = port;
            if (!XMLHelper.LoadStringNode(node, "userName", out string userName, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.UserName = userName;
            if (!XMLHelper.LoadStringNode(node, "password", out string password, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.Password = password;

            info.EnvIsOkay = true;
            return info;
        }

        public static bool UpdatePublishInfo(ConfPublishServiceInfo info, string path, out string errMsg)
        {
            if (!IPTool.IsValidPort(info.Port))
            {
                errMsg = "命令服务器的发布端口被占用，请更换端口号!";
                return false;
            }

            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
            {
                errMsg = "框架配置文件路径未指定明确";
                return false;
            }
            if (!XMLHelper.LoadDoc(path, out doc, out errMsg))
            {
                errMsg = "配置文件获取失败：" + errMsg;
                return false;
            }
            if (!XMLHelper.ExistsNode(doc, "service/CityIoTService/dataPublishServer", out XmlNode node, out errMsg))
            {
                errMsg = "获取配置文件发布节点失败：" + errMsg;
                return false;
            }
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name == "port")
                {
                    childNode.InnerText = info.Port.ToString();
                }
            }
            doc.Save(path);
            return true;

        }
    }

    #endregion

    #region****** 子服务配置*****

    // 二供报警子服务
    public class ConfSonCityIoTPumpAlarm:ConfigState
    {
        public bool IsNeedRun { get; set; }
        public int UpdateInterval { get; set; } = EnvConfigInfo.updateInterval;
        public int JZTimeOut { get; set; } = EnvConfigInfo.jzTimeOut;
        public int CommandTimeoutSeconds { get; set; } = EnvConfigInfo.commandTimeoutSeconds;

        public static ConfSonCityIoTPumpAlarm LoadInfo(string projectConfigPath)
        {
            ConfSonCityIoTPumpAlarm info = new ConfSonCityIoTPumpAlarm();
            if (string.IsNullOrWhiteSpace(projectConfigPath))
            {
                info.ErrMsg = "未知的项目配置文件路径";
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadDoc(projectConfigPath, out XmlDocument doc, out string errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode node, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadNumNode(node, "CityIoTPumpAlarm", out int runPumpAlarmService, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.IsNeedRun = runPumpAlarmService == 1 ? true : false;
            if (!XMLHelper.ExistsNode(doc, "service/CityIoTPumpAlarm", out XmlNode clientNode, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadNumNode(clientNode, "alarmUpdateInterval", out int alarmUpdateInterval, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.UpdateInterval = alarmUpdateInterval;
            if (!XMLHelper.LoadNumNode(clientNode, "pumpJZTimeOut", out int pumpJZTimeOut, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.JZTimeOut = pumpJZTimeOut;
            if (!XMLHelper.LoadNumNode(clientNode, "commandTimeoutSeconds", out int commandTimeoutSeconds, out errMsg)) 
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.CommandTimeoutSeconds = commandTimeoutSeconds;

            info.EnvIsOkay = true;
            return info;
        }

        //提交子服务节点配置
        public static bool SubmitSonPumpAlarmData(ConfSonCityIoTPumpAlarm info, string path, out string errMsg)
        {
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
            {
                errMsg = "项目配置文件路径未指定明确";
                return false;
            }
            if (!XMLHelper.LoadDoc(path, out doc, out errMsg))
            {
                errMsg = "配置文件获取失败：" + errMsg;
                return false;
            }
            //PumpAlarm是否在子服务列表中
            if (XMLHelper.ExistsNode(doc, "service/RunServerList/CityIoTPumpAlarm", out XmlNode node, out errMsg))
            {
                //存在，则设值为 1
                node.InnerText = "1";
            }
            else
            {
                //不存在，则添加节点信息
                if (XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode serviceList, out errMsg))
                {
                    XmlElement sonService = doc.CreateElement("CityIoTPumpAlarm");
                    sonService.SetAttribute("dsc", "二供报警服务");
                    sonService.SetAttribute("note", "1-启动;0-不启动");
                    sonService.InnerText = "1";
                    serviceList.AppendChild(sonService);
                }
            }
            //判断是否有CityIoTPumpAlarm节点
            if (XMLHelper.ExistsNode(doc, "service/CityIoTPumpAlarm", out XmlNode pumpAlarm, out errMsg))
            {
                //存在，则更新
                foreach (XmlNode sonNode in pumpAlarm.ChildNodes)
                {
                    if (sonNode.Name == "alarmUpdateInterval")
                    {
                        sonNode.InnerText = info.UpdateInterval.ToString();
                    }
                    if (sonNode.Name == "pumpJZTimeOut")
                    {
                        sonNode.InnerText = info.JZTimeOut.ToString();
                    }
                    if (sonNode.Name == "commandTimeoutSeconds")
                    {
                        sonNode.InnerText = info.CommandTimeoutSeconds.ToString();
                    }
                }
            }
            else
            {
                //不存在，则新建
                XmlNode serviceNode = doc.SelectSingleNode("service");
                XmlElement pumpAlarmNode = doc.CreateElement("CityIoTPumpAlarm");
                XmlElement sonNode1 = doc.CreateElement("alarmUpdateInterval");
                sonNode1.SetAttribute("unit", "分钟");
                sonNode1.InnerText = info.UpdateInterval.ToString();
                XmlElement sonNode2 = doc.CreateElement("pumpJZTimeOut");
                sonNode2.SetAttribute("unit", "秒");
                sonNode2.InnerText = info.JZTimeOut.ToString();
                XmlElement sonNode3 = doc.CreateElement("commandTimeoutSeconds");
                sonNode3.InnerText = info.CommandTimeoutSeconds.ToString();
                pumpAlarmNode.AppendChild(sonNode1);
                pumpAlarmNode.AppendChild(sonNode2);
                pumpAlarmNode.AppendChild(sonNode3);
                serviceNode.AppendChild(pumpAlarmNode);
            }

            doc.Save(path);
            return true;
        }
        
    }
    // 二供OPC子服务
    public class ConfSonOPCPumpDataService : ConfigState 
    {
        public bool IsNeedRun { get; set; }
        public int PointsSaveInterVal { get; set; } = EnvConfigInfo.saveInterVal;
        public int PointsCollectInterVal { get; set; } = EnvConfigInfo.collectInterval;
        public int DefaultGroupDeadband { get; set; }
        public int UpdateRate { get; set; } = EnvConfigInfo.updateRate;
        public int ReadRate { get; set; } = EnvConfigInfo.readRate;
        public int errorTimes { get; set; } = EnvConfigInfo.errorTimes;
        public int okayTimes { get; set; } = EnvConfigInfo.okayTimes;
        public int commandTimeoutSeconds { get; set; } = EnvConfigInfo.commandTimeoutSeconds;

        public static ConfSonOPCPumpDataService LoadInfo(string projectConfigPath)
        {
            ConfSonOPCPumpDataService info = new ConfSonOPCPumpDataService();
            if (string.IsNullOrWhiteSpace(projectConfigPath))
            {
                info.ErrMsg = "未知的项目配置文件路径";
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadDoc(projectConfigPath, out XmlDocument doc, out string errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode node, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadNumNode(node, "OPCPumpDataService", out int isNeedRun, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.IsNeedRun = isNeedRun == 1 ? true : false;
            if (!XMLHelper.ExistsNode(doc, "service/OPCPumpDataService", out XmlNode clientNode, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadNumNode(clientNode, "PointsSaveInterVal", out int PointsSaveInterVal, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.PointsSaveInterVal = PointsSaveInterVal;
            if (!XMLHelper.LoadNumNode(clientNode, "PointsCollectInterVal", out int PointsCollectInterVal, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.PointsCollectInterVal = PointsCollectInterVal;
            if (!XMLHelper.LoadNumNode(clientNode, "DefaultGroupDeadband", out int DefaultGroupDeadband, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.DefaultGroupDeadband = DefaultGroupDeadband;
            if (!XMLHelper.LoadNumNode(clientNode, "UpdateRate", out int UpdateRate, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.UpdateRate = UpdateRate;
            if (!XMLHelper.LoadNumNode(clientNode, "ReadRate", out int ReadRate, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.ReadRate = ReadRate;
            if (!XMLHelper.LoadNumNode(clientNode, "errorTimes", out int errorTimes, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.errorTimes = errorTimes;
            if (!XMLHelper.LoadNumNode(clientNode, "okayTimes", out int okayTimes, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.okayTimes = okayTimes;
            if (!XMLHelper.LoadNumNode(clientNode, "commandTimeoutSeconds", out int commandTimeoutSeconds, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.commandTimeoutSeconds = commandTimeoutSeconds;

            info.EnvIsOkay = true;
            return info;
        }

        //提交子服务节点配置
        public static bool SubmitSonOPCPumpData(ConfSonOPCPumpDataService info, string path, out string errMsg)
        {
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
            {
                errMsg = "项目配置文件路径未指定明确";
                return false;
            }
            if (!XMLHelper.LoadDoc(path, out doc, out errMsg))
            {
                errMsg = "配置文件获取失败：" + errMsg;
                return false;
            }
            //OPCPump是否在子服务列表中
            if (XMLHelper.ExistsNode(doc, "service/RunServerList/OPCPumpDataService", out XmlNode node, out errMsg))
            {
                //存在，则设值为 1
                node.InnerText = "1";
            }
            else
            {
                //不存在，则添加节点信息
                if(XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode serviceList, out errMsg))
                {
                    XmlElement sonService = doc.CreateElement("OPCPumpDataService");
                    sonService.SetAttribute("dsc", "OPC二供接入服务");
                    sonService.SetAttribute("note", "1-启动;0-不启动");
                    sonService.InnerText = "1";
                    serviceList.AppendChild(sonService);
                }
            }
            //判断是否有OPCPumpDataService节点
            if (XMLHelper.ExistsNode(doc, "service/OPCPumpDataService", out XmlNode opcPump, out errMsg))
            {
                //存在，则更新
                foreach(XmlNode sonNode in opcPump.ChildNodes)
                {
                    if(sonNode.Name == "PointsSaveInterVal")
                    {
                        sonNode.InnerText = info.PointsSaveInterVal.ToString();
                    }
                    if(sonNode.Name == "PointsCollectInterVal")
                    {
                        sonNode.InnerText = info.PointsCollectInterVal.ToString();
                    }
                    if(sonNode.Name == "DefaultGroupDeadband")
                    {
                        sonNode.InnerText = info.DefaultGroupDeadband.ToString();
                    }
                    if(sonNode.Name == "UpdateRate")
                    {
                        sonNode.InnerText = info.UpdateRate.ToString();
                    }
                    if(sonNode.Name == "ReadRate")
                    {
                        sonNode.InnerText = info.ReadRate.ToString();
                    }
                    if(sonNode.Name == "errorTimes")
                    {
                        sonNode.InnerText = info.errorTimes.ToString();
                    }
                    if(sonNode.Name == "okayTimes")
                    {
                        sonNode.InnerText = info.okayTimes.ToString();
                    }
                    if(sonNode.Name == "commandTimeoutSeconds")
                    {
                        sonNode.InnerText = info.commandTimeoutSeconds.ToString();
                    }
                }
            }
            else
            {
                //不存在，则新建
                XmlNode serviceNode = doc.SelectSingleNode("service");
                XmlElement opcPumpNode = doc.CreateElement("OPCPumpDataService");
                XmlElement sonNode1 = doc.CreateElement("PointsSaveInterVal");               
                sonNode1.SetAttribute("unit", "分钟");
                sonNode1.InnerText = info.PointsSaveInterVal.ToString();
                XmlElement sonNode2 = doc.CreateElement("PointsCollectInterVal");
                sonNode2.SetAttribute("unit", "秒");
                sonNode2.InnerText = info.PointsCollectInterVal.ToString();
                XmlElement sonNode3 = doc.CreateElement("DefaultGroupDeadband");
                sonNode3.InnerText = info.DefaultGroupDeadband.ToString();
                XmlElement sonNode4 = doc.CreateElement("UpdateRate");
                sonNode4.SetAttribute("unit", "毫秒");
                sonNode4.InnerText = info.UpdateRate.ToString();
                XmlElement sonNode5 = doc.CreateElement("ReadRate");
                sonNode5.SetAttribute("unit", "秒");
                sonNode5.InnerText = info.ReadRate.ToString();
                XmlElement sonNode6 = doc.CreateElement("errorTimes");
                sonNode6.SetAttribute("unit", "个");
                sonNode6.InnerText = info.errorTimes.ToString();
                XmlElement sonNode7 = doc.CreateElement("okayTimes");
                sonNode7.SetAttribute("unit", "个");
                sonNode7.InnerText = info.okayTimes.ToString();
                XmlElement sonNode8 = doc.CreateElement("commandTimeoutSeconds");
                sonNode8.SetAttribute("node", "命令设定超时的秒数");
                sonNode8.InnerText = info.commandTimeoutSeconds.ToString();
                opcPumpNode.AppendChild(sonNode1);
                opcPumpNode.AppendChild(sonNode2);
                opcPumpNode.AppendChild(sonNode3);
                opcPumpNode.AppendChild(sonNode4);
                opcPumpNode.AppendChild(sonNode5);
                opcPumpNode.AppendChild(sonNode6);
                opcPumpNode.AppendChild(sonNode7);
                opcPumpNode.AppendChild(sonNode8);
                serviceNode.AppendChild(opcPumpNode);
            }

            doc.Save(path);
            return true;
        }
    }
    // scada OPC子服务
    public class ConfSonOPCScadaDataService : ConfigState
    {
        public bool IsNeedRun { get; set; }
        public int PointsSaveInterVal { get; set; } = EnvConfigInfo.saveInterVal;
        public int PointsCollectInterVal { get; set; } = EnvConfigInfo.collectInterval;
        public int DefaultGroupDeadband { get; set; }
        public int UpdateRate { get; set; } = EnvConfigInfo.updateInterval;
        public int ReadRate { get; set; } = EnvConfigInfo.readRate;
        public int errorTimes { get; set; } = EnvConfigInfo.errorTimes;
        public int okayTimes { get; set; } = EnvConfigInfo.okayTimes;
        public int commandTimeoutSeconds { get; set; } = EnvConfigInfo.commandTimeoutSeconds;

        public static ConfSonOPCScadaDataService LoadInfo(string projectConfigPath)
        {
            ConfSonOPCScadaDataService info = new ConfSonOPCScadaDataService();
            if (string.IsNullOrWhiteSpace(projectConfigPath))
            {
                info.ErrMsg = "未知的项目配置文件路径";
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadDoc(projectConfigPath, out XmlDocument doc, out string errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode node, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadNumNode(node, "OPCScadaDataService", out int isNeedRun, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.IsNeedRun = isNeedRun == 1 ? true : false;
            if (!XMLHelper.ExistsNode(doc, "service/OPCScadaDataService", out XmlNode clientNode, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadNumNode(clientNode, "PointsSaveInterVal", out int PointsSaveInterVal, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.PointsSaveInterVal = PointsSaveInterVal;
            if (!XMLHelper.LoadNumNode(clientNode, "PointsCollectInterVal", out int PointsCollectInterVal, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.PointsCollectInterVal = PointsCollectInterVal;
            if (!XMLHelper.LoadNumNode(clientNode, "DefaultGroupDeadband", out int DefaultGroupDeadband, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.DefaultGroupDeadband = DefaultGroupDeadband;
            if (!XMLHelper.LoadNumNode(clientNode, "UpdateRate", out int UpdateRate, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.UpdateRate = UpdateRate;
            if (!XMLHelper.LoadNumNode(clientNode, "ReadRate", out int ReadRate, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.ReadRate = ReadRate;
            if (!XMLHelper.LoadNumNode(clientNode, "errorTimes", out int errorTimes, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.errorTimes = errorTimes;
            if (!XMLHelper.LoadNumNode(clientNode, "okayTimes", out int okayTimes, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.okayTimes = okayTimes;
            if (!XMLHelper.LoadNumNode(clientNode, "commandTimeoutSeconds", out int commandTimeoutSeconds, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.commandTimeoutSeconds = commandTimeoutSeconds;

            info.EnvIsOkay = true;
            return info;
        }

        //提交子服务节点配置
        public static bool SubmitSonOPCScadaData(ConfSonOPCScadaDataService info, string path, out string errMsg)
        {
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
            {
                errMsg = "项目配置文件路径未指定明确";
                return false;
            }
            if (!XMLHelper.LoadDoc(path, out doc, out errMsg))
            {
                errMsg = "配置文件获取失败：" + errMsg;
                return false;
            }
            //OPCScada是否在子服务列表中
            if (XMLHelper.ExistsNode(doc, "service/RunServerList/OPCScadaDataService", out XmlNode node, out errMsg))
            {
                //存在，则设值为 1
                node.InnerText = "1";
            }
            else
            {
                //不存在，则添加节点信息
                if (XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode serviceList, out errMsg))
                {
                    XmlElement sonService = doc.CreateElement("OPCScadaDataService");
                    sonService.SetAttribute("dsc", "OPC-SCADA接入服务");
                    sonService.SetAttribute("note", "1-启动;0-不启动");
                    sonService.InnerText = "1";
                    serviceList.AppendChild(sonService);
                }
            }
            //判断是否有OPCScadaDataService节点
            if (XMLHelper.ExistsNode(doc, "service/OPCScadaDataService", out XmlNode opcScada, out errMsg))
            {
                //存在，则更新
                foreach (XmlNode sonNode in opcScada.ChildNodes)
                {
                    if (sonNode.Name == "PointsSaveInterVal")
                    {
                        sonNode.InnerText = info.PointsSaveInterVal.ToString();
                    }
                    if (sonNode.Name == "PointsCollectInterVal")
                    {
                        sonNode.InnerText = info.PointsCollectInterVal.ToString();
                    }
                    if (sonNode.Name == "DefaultGroupDeadband")
                    {
                        sonNode.InnerText = info.DefaultGroupDeadband.ToString();
                    }
                    if (sonNode.Name == "UpdateRate")
                    {
                        sonNode.InnerText = info.UpdateRate.ToString();
                    }
                    if (sonNode.Name == "ReadRate")
                    {
                        sonNode.InnerText = info.ReadRate.ToString();
                    }
                    if (sonNode.Name == "errorTimes")
                    {
                        sonNode.InnerText = info.errorTimes.ToString();
                    }
                    if (sonNode.Name == "okayTimes")
                    {
                        sonNode.InnerText = info.okayTimes.ToString();
                    }
                    if (sonNode.Name == "commandTimeoutSeconds")
                    {
                        sonNode.InnerText = info.commandTimeoutSeconds.ToString();
                    }
                }
            }
            else
            {
                //不存在，则新建
                XmlNode serviceNode = doc.SelectSingleNode("service");
                XmlElement opcScadaNode = doc.CreateElement("OPCScadaDataService");
                XmlElement sonNode1 = doc.CreateElement("PointsSaveInterVal");
                sonNode1.SetAttribute("unit", "分钟");
                sonNode1.InnerText = info.PointsSaveInterVal.ToString();
                XmlElement sonNode2 = doc.CreateElement("PointsCollectInterVal");
                sonNode2.SetAttribute("unit", "秒");
                sonNode2.InnerText = info.PointsCollectInterVal.ToString();
                XmlElement sonNode3 = doc.CreateElement("DefaultGroupDeadband");
                sonNode3.InnerText = info.DefaultGroupDeadband.ToString();
                XmlElement sonNode4 = doc.CreateElement("UpdateRate");
                sonNode4.SetAttribute("unit", "毫秒");
                sonNode4.InnerText = info.UpdateRate.ToString();
                XmlElement sonNode5 = doc.CreateElement("ReadRate");
                sonNode5.SetAttribute("unit", "秒");
                sonNode5.InnerText = info.ReadRate.ToString();
                XmlElement sonNode6 = doc.CreateElement("errorTimes");
                sonNode6.SetAttribute("unit", "个");
                sonNode6.InnerText = info.errorTimes.ToString();
                XmlElement sonNode7 = doc.CreateElement("okayTimes");
                sonNode7.SetAttribute("unit", "个");
                sonNode7.InnerText = info.okayTimes.ToString();
                XmlElement sonNode8 = doc.CreateElement("commandTimeoutSeconds");
                sonNode8.SetAttribute("node", "命令设定超时的秒数");
                sonNode8.InnerText = info.commandTimeoutSeconds.ToString();
                opcScadaNode.AppendChild(sonNode1);
                opcScadaNode.AppendChild(sonNode2);
                opcScadaNode.AppendChild(sonNode3);
                opcScadaNode.AppendChild(sonNode4);
                opcScadaNode.AppendChild(sonNode5);
                opcScadaNode.AppendChild(sonNode6);
                opcScadaNode.AppendChild(sonNode7);
                opcScadaNode.AppendChild(sonNode8);
                serviceNode.AppendChild(opcScadaNode);
            }

            doc.Save(path);
            return true;
        }
    }

    // 二供-pandaPump子服务
    public class ConfSonWebPandaPumpDataService : ConfigState 
    {
        public bool IsNeedRun { get; set; }

        public string AppKey { get; set; } = EnvConfigInfo.appKey;
        public string AppSecret { get; set; } = EnvConfigInfo.appSecret;
        public string GetTokenUrl { get; set; } = EnvConfigInfo.tokenUrl;
        public string GetDataUrl { get; set; } = EnvConfigInfo.dataUrl;
        public string UseName { get; set; } = EnvConfigInfo.userName;
        public int CollectInterval { get; set; } = EnvConfigInfo.collectInterval;
        public int SaveInterVal { get; set; } = EnvConfigInfo.saveInterVal;
        public int CommandTimeoutSeconds { get; set; } = EnvConfigInfo.commandTimeoutSeconds;

        public static ConfSonWebPandaPumpDataService LoadInfo(string projectConfigPath)
        {
            ConfSonWebPandaPumpDataService info = new ConfSonWebPandaPumpDataService();
            if (string.IsNullOrWhiteSpace(projectConfigPath))
            {
                info.ErrMsg = "未知的项目配置文件路径";
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadDoc(projectConfigPath, out XmlDocument doc, out string errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode node, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadNumNode(node, "WebPandaPumpDataService", out int isNeedRun, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.IsNeedRun = isNeedRun == 1 ? true : false;
            if (!XMLHelper.ExistsNode(doc, "service/WebPandaPumpDataService", out XmlNode clientNode, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadStringNode(clientNode, "appKey", out string appKey, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.AppKey = appKey;
            if (!XMLHelper.LoadStringNode(clientNode, "appSecret", out string appSecret, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.AppSecret = appSecret;
            if (!XMLHelper.LoadStringNode(clientNode, "getTokenUrl", out string getTokenUrl, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.GetTokenUrl = getTokenUrl;
            if (!XMLHelper.LoadStringNode(clientNode, "getDataUrl", out string getDataUrl, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.GetDataUrl = getDataUrl;
            if (!XMLHelper.LoadStringNode(clientNode, "useName", out string useName, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.UseName = useName;
            if (!XMLHelper.LoadNumNode(clientNode, "collectInterval", out int collectInterval, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.CollectInterval = collectInterval;
            if (!XMLHelper.LoadNumNode(clientNode, "saveInterVal", out int saveInterVal, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.SaveInterVal = saveInterVal;
            if (!XMLHelper.LoadNumNode(clientNode, "commandTimeoutSeconds", out int commandTimeoutSeconds, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.CommandTimeoutSeconds = commandTimeoutSeconds;

            info.EnvIsOkay = true;
            return info;
        }

        //提交子服务节点配置
        public static bool SubmitSonWebPandaPumpData(ConfSonWebPandaPumpDataService info, string path, out string errMsg)
        {
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
            {
                errMsg = "项目配置文件路径未指定明确";
                return false;
            }
            if (!XMLHelper.LoadDoc(path, out doc, out errMsg))
            {
                errMsg = "配置文件获取失败：" + errMsg;
                return false;
            }
            //WebPandaPumpDataService是否在子服务列表中
            if (XMLHelper.ExistsNode(doc, "service/RunServerList/WebPandaPumpDataService", out XmlNode node, out errMsg))
            {
                //存在，则设值为 1
                node.InnerText = "1";
            }
            else
            {
                //不存在，则添加节点信息
                if (XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode serviceList, out errMsg))
                {
                    XmlElement sonService = doc.CreateElement("WebPandaPumpDataService");
                    sonService.SetAttribute("dsc", "WEB熊猫二供接入服务");
                    sonService.SetAttribute("note", "1-启动;0-不启动");
                    sonService.InnerText = "1";
                    serviceList.AppendChild(sonService);
                }
            }
            //判断是否有WebPandaPumpDataService节点
            if (XMLHelper.ExistsNode(doc, "service/WebPandaPumpDataService", out XmlNode pandaPump, out errMsg))
            {
                //存在，则更新
                foreach (XmlNode sonNode in pandaPump.ChildNodes)
                {
                    if (sonNode.Name == "appKey")
                    {
                        sonNode.InnerText = info.AppKey.ToString();
                    }
                    if (sonNode.Name == "appSecret")
                    {
                        sonNode.InnerText = info.AppSecret.ToString();
                    }
                    if (sonNode.Name == "getTokenUrl")
                    {
                        sonNode.InnerText = info.GetTokenUrl.ToString();
                    }
                    if (sonNode.Name == "getDataUrl")
                    {
                        sonNode.InnerText = info.GetDataUrl.ToString();
                    }
                    if (sonNode.Name == "useName")
                    {
                        sonNode.InnerText = info.UseName.ToString();
                    }
                    if (sonNode.Name == "collectInterval")
                    {
                        sonNode.InnerText = info.CollectInterval.ToString();
                    }
                    if (sonNode.Name == "saveInterVal")
                    {
                        sonNode.InnerText = info.SaveInterVal.ToString();
                    }
                    if (sonNode.Name == "commandTimeoutSeconds")
                    {
                        sonNode.InnerText = info.CommandTimeoutSeconds.ToString();
                    }
                }
            }
            else
            {
                //不存在，则新建
                XmlNode serviceNode = doc.SelectSingleNode("service");
                XmlElement pandaPumpNode = doc.CreateElement("WebPandaPumpDataService");
                XmlElement sonNode1 = doc.CreateElement("appKey");
                sonNode1.InnerText = info.AppKey.ToString();
                XmlElement sonNode2 = doc.CreateElement("appSecret");
                sonNode2.InnerText = info.AppSecret.ToString();
                XmlElement sonNode3 = doc.CreateElement("getTokenUrl");
                sonNode3.InnerText = info.GetTokenUrl.ToString();
                XmlElement sonNode4 = doc.CreateElement("getDataUrl");
                sonNode4.InnerText = info.GetDataUrl.ToString();
                XmlElement sonNode5 = doc.CreateElement("useName");
                sonNode5.InnerText = info.UseName.ToString();
                XmlElement sonNode6 = doc.CreateElement("collectInterval");
                sonNode6.SetAttribute("unit", "秒");
                sonNode6.InnerText = info.CollectInterval.ToString();
                XmlElement sonNode7 = doc.CreateElement("saveInterVal");
                sonNode7.SetAttribute("unit", "分钟");
                sonNode7.InnerText = info.SaveInterVal.ToString();
                XmlElement sonNode8 = doc.CreateElement("commandTimeoutSeconds");
                sonNode8.SetAttribute("node", "命令设定超时的秒数");
                sonNode8.InnerText = info.CommandTimeoutSeconds.ToString();
                pandaPumpNode.AppendChild(sonNode1);
                pandaPumpNode.AppendChild(sonNode2);
                pandaPumpNode.AppendChild(sonNode3);
                pandaPumpNode.AppendChild(sonNode4);
                pandaPumpNode.AppendChild(sonNode5);
                pandaPumpNode.AppendChild(sonNode6);
                pandaPumpNode.AppendChild(sonNode7);
                pandaPumpNode.AppendChild(sonNode8);
                serviceNode.AppendChild(pandaPumpNode);
            }

            doc.Save(path);
            return true;
        }

    }
    // Scada-pandaPump-子服务
    public class ConfSonWebPandaPumpScadaDataService : ConfigState 
    {
        public bool IsNeedRun { get; set; }

        public string AppKey { get; set; } = EnvConfigInfo.appKey;
        public string AppSecret { get; set; } = EnvConfigInfo.appSecret;
        public string GetTokenUrl { get; set; } = EnvConfigInfo.tokenUrl;
        public string GetDataUrl { get; set; } = EnvConfigInfo.dataUrl;
        public string UseName { get; set; } = EnvConfigInfo.userName;
        public int CollectInterval { get; set; } = EnvConfigInfo.collectInterval;
        public int SaveInterVal { get; set; } = EnvConfigInfo.saveInterVal;
        public int CommandTimeoutSeconds { get; set; } = EnvConfigInfo.commandTimeoutSeconds;

        public static ConfSonWebPandaPumpScadaDataService LoadInfo(string projectConfigPath)
        {
            ConfSonWebPandaPumpScadaDataService info = new ConfSonWebPandaPumpScadaDataService();
            if (string.IsNullOrWhiteSpace(projectConfigPath))
            {
                info.ErrMsg = "未知的项目配置文件路径";
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadDoc(projectConfigPath, out XmlDocument doc, out string errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode node, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadNumNode(node, "WebPandaPumpScadaDataService", out int isNeedRun, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.IsNeedRun = isNeedRun == 1 ? true : false;
            if (!XMLHelper.ExistsNode(doc, "service/WebPandaPumpScadaDataService", out XmlNode clientNode, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadStringNode(clientNode, "appKey", out string appKey, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.AppKey = appKey;
            if (!XMLHelper.LoadStringNode(clientNode, "appSecret", out string appSecret, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.AppSecret = appSecret;
            if (!XMLHelper.LoadStringNode(clientNode, "getTokenUrl", out string getTokenUrl, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.GetTokenUrl = getTokenUrl;
            if (!XMLHelper.LoadStringNode(clientNode, "getDataUrl", out string getDataUrl, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.GetDataUrl = getDataUrl;
            if (!XMLHelper.LoadStringNode(clientNode, "useName", out string useName, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.UseName = useName;
            if (!XMLHelper.LoadNumNode(clientNode, "collectInterval", out int collectInterval, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.CollectInterval = collectInterval;
            if (!XMLHelper.LoadNumNode(clientNode, "saveInterVal", out int saveInterVal, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.SaveInterVal = saveInterVal;
            if (!XMLHelper.LoadNumNode(clientNode, "commandTimeoutSeconds", out int commandTimeoutSeconds, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.CommandTimeoutSeconds = commandTimeoutSeconds;

            info.EnvIsOkay = true;
            return info;
        }

        //提交子服务节点配置
        public static bool SubmitSonWebPandaPumpScadaData(ConfSonWebPandaPumpScadaDataService info, string path, out string errMsg)
        {
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
            {
                errMsg = "项目配置文件路径未指定明确";
                return false;
            }
            if (!XMLHelper.LoadDoc(path, out doc, out errMsg))
            {
                errMsg = "配置文件获取失败：" + errMsg;
                return false;
            }
            //WebPandaPumpScadaDataService是否在子服务列表中
            if (XMLHelper.ExistsNode(doc, "service/RunServerList/WebPandaPumpScadaDataService", out XmlNode node, out errMsg))
            {
                //存在，则设值为 1
                node.InnerText = "1";
            }
            else
            {
                //不存在，则添加节点信息
                if (XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode serviceList, out errMsg))
                {
                    XmlElement sonService = doc.CreateElement("WebPandaPumpScadaDataService");
                    sonService.SetAttribute("dsc", "WEB熊猫二供SCADA接入服务");
                    sonService.SetAttribute("note", "1-启动;0-不启动");
                    sonService.InnerText = "1";
                    serviceList.AppendChild(sonService);
                }
            }
            //判断是否有WebPandaPumpScadaDataService节点
            if (XMLHelper.ExistsNode(doc, "service/WebPandaPumpScadaDataService", out XmlNode pandaScada, out errMsg))
            {
                //存在，则更新
                foreach (XmlNode sonNode in pandaScada.ChildNodes)
                {
                    if (sonNode.Name == "appKey")
                    {
                        sonNode.InnerText = info.AppKey.ToString();
                    }
                    if (sonNode.Name == "appSecret")
                    {
                        sonNode.InnerText = info.AppSecret.ToString();
                    }
                    if (sonNode.Name == "getTokenUrl")
                    {
                        sonNode.InnerText = info.GetTokenUrl.ToString();
                    }
                    if (sonNode.Name == "getDataUrl")
                    {
                        sonNode.InnerText = info.GetDataUrl.ToString();
                    }
                    if (sonNode.Name == "useName")
                    {
                        sonNode.InnerText = info.UseName.ToString();
                    }
                    if (sonNode.Name == "collectInterval")
                    {
                        sonNode.InnerText = info.CollectInterval.ToString();
                    }
                    if (sonNode.Name == "saveInterVal")
                    {
                        sonNode.InnerText = info.SaveInterVal.ToString();
                    }
                    if (sonNode.Name == "commandTimeoutSeconds")
                    {
                        sonNode.InnerText = info.CommandTimeoutSeconds.ToString();
                    }
                }
            }
            else
            {
                //不存在，则新建
                XmlNode serviceNode = doc.SelectSingleNode("service");
                XmlElement pandaScadaNode = doc.CreateElement("WebPandaPumpScadaDataService");
                XmlElement sonNode1 = doc.CreateElement("appKey");
                sonNode1.InnerText = info.AppKey.ToString();
                XmlElement sonNode2 = doc.CreateElement("appSecret");
                sonNode2.InnerText = info.AppSecret.ToString();
                XmlElement sonNode3 = doc.CreateElement("getTokenUrl");
                sonNode3.InnerText = info.GetTokenUrl.ToString();
                XmlElement sonNode4 = doc.CreateElement("getDataUrl");
                sonNode4.InnerText = info.GetDataUrl.ToString();
                XmlElement sonNode5 = doc.CreateElement("useName");
                sonNode5.InnerText = info.UseName.ToString();
                XmlElement sonNode6 = doc.CreateElement("collectInterval");
                sonNode6.SetAttribute("unit", "秒");
                sonNode6.InnerText = info.CollectInterval.ToString();
                XmlElement sonNode7 = doc.CreateElement("saveInterVal");
                sonNode7.SetAttribute("unit", "分钟");
                sonNode7.InnerText = info.SaveInterVal.ToString();
                XmlElement sonNode8 = doc.CreateElement("commandTimeoutSeconds");
                sonNode8.SetAttribute("node", "命令设定超时的秒数");
                sonNode8.InnerText = info.CommandTimeoutSeconds.ToString();
                pandaScadaNode.AppendChild(sonNode1);
                pandaScadaNode.AppendChild(sonNode2);
                pandaScadaNode.AppendChild(sonNode3);
                pandaScadaNode.AppendChild(sonNode4);
                pandaScadaNode.AppendChild(sonNode5);
                pandaScadaNode.AppendChild(sonNode6);
                pandaScadaNode.AppendChild(sonNode7);
                pandaScadaNode.AppendChild(sonNode8);
                serviceNode.AppendChild(pandaScadaNode);
            }

            doc.Save(path);
            return true;
        }
    }
    // Scada-pandaYL子服务
    public class ConfSonWebPandaYLDataService : ConfigState
    {
        public bool IsNeedRun { get; set; }

        public string AppKey { get; set; } = EnvConfigInfo.appKey;
        public string AppSecret { get; set; } = EnvConfigInfo.appSecret;
        public string GetTokenUrl { get; set; } = EnvConfigInfo.tokenUrl;
        public string GetDataUrl { get; set; } = EnvConfigInfo.ylUrl;
        public string UseName { get; set; } = EnvConfigInfo.userName;
        public int CollectInterval { get; set; } = EnvConfigInfo.collectInterval;
        public int SaveInterVal { get; set; } = EnvConfigInfo.saveInterVal;
        public int CommandTimeoutSeconds { get; set; } = EnvConfigInfo.commandTimeoutSeconds;

        public static ConfSonWebPandaYLDataService LoadInfo(string projectConfigPath)
        {
            ConfSonWebPandaYLDataService info = new ConfSonWebPandaYLDataService();
            if (string.IsNullOrWhiteSpace(projectConfigPath))
            {
                info.ErrMsg = "未知的项目配置文件路径";
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadDoc(projectConfigPath, out XmlDocument doc, out string errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode node, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadNumNode(node, "WebPandaYLDataService", out int isNeedRun, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.IsNeedRun = isNeedRun == 1 ? true : false;
            if (!XMLHelper.ExistsNode(doc, "service/WebPandaYLDataService", out XmlNode clientNode, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadStringNode(clientNode, "appKey", out string appKey, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.AppKey = appKey;
            if (!XMLHelper.LoadStringNode(clientNode, "appSecret", out string appSecret, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.AppSecret = appSecret;
            if (!XMLHelper.LoadStringNode(clientNode, "getTokenUrl", out string getTokenUrl, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.GetTokenUrl = getTokenUrl;
            if (!XMLHelper.LoadStringNode(clientNode, "getDataUrl", out string getDataUrl, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.GetDataUrl = getDataUrl;
            if (!XMLHelper.LoadStringNode(clientNode, "useName", out string useName, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.UseName = useName;
            if (!XMLHelper.LoadNumNode(clientNode, "collectInterval", out int collectInterval, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.CollectInterval = collectInterval;
            if (!XMLHelper.LoadNumNode(clientNode, "saveInterVal", out int saveInterVal, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.SaveInterVal = saveInterVal;
            if (!XMLHelper.LoadNumNode(clientNode, "commandTimeoutSeconds", out int commandTimeoutSeconds, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.CommandTimeoutSeconds = commandTimeoutSeconds;

            info.EnvIsOkay = true;
            return info;
        }

        //提交子服务节点配置
        public static bool SubmitSonWebPandaYLData(ConfSonWebPandaYLDataService info, string path, out string errMsg)
        {
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
            {
                errMsg = "项目配置文件路径未指定明确";
                return false;
            }
            if (!XMLHelper.LoadDoc(path, out doc, out errMsg))
            {
                errMsg = "配置文件获取失败：" + errMsg;
                return false;
            }
            //WebPandaYLDataService是否在子服务列表中
            if (XMLHelper.ExistsNode(doc, "service/RunServerList/WebPandaYLDataService", out XmlNode node, out errMsg))
            {
                //存在，则设值为 1
                node.InnerText = "1";
            }
            else
            {
                //不存在，则添加节点信息
                if (XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode serviceList, out errMsg))
                {
                    XmlElement sonService = doc.CreateElement("WebPandaYLDataService");
                    sonService.SetAttribute("dsc", "WEB熊猫监测点接入服务");
                    sonService.SetAttribute("note", "1-启动;0-不启动");
                    sonService.InnerText = "1";
                    serviceList.AppendChild(sonService);
                }
            }
            //判断是否有WebPandaYLDataService节点
            if (XMLHelper.ExistsNode(doc, "service/WebPandaYLDataService", out XmlNode pandaYL, out errMsg))
            {
                //存在，则更新
                foreach (XmlNode sonNode in pandaYL.ChildNodes)
                {
                    if (sonNode.Name == "appKey")
                    {
                        sonNode.InnerText = info.AppKey.ToString();
                    }
                    if (sonNode.Name == "appSecret")
                    {
                        sonNode.InnerText = info.AppSecret.ToString();
                    }
                    if (sonNode.Name == "getTokenUrl")
                    {
                        sonNode.InnerText = info.GetTokenUrl.ToString();
                    }
                    if (sonNode.Name == "getDataUrl")
                    {
                        sonNode.InnerText = info.GetDataUrl.ToString();
                    }
                    if (sonNode.Name == "useName")
                    {
                        sonNode.InnerText = info.UseName.ToString();
                    }
                    if (sonNode.Name == "collectInterval")
                    {
                        sonNode.InnerText = info.CollectInterval.ToString();
                    }
                    if (sonNode.Name == "saveInterVal")
                    {
                        sonNode.InnerText = info.SaveInterVal.ToString();
                    }
                    if (sonNode.Name == "commandTimeoutSeconds")
                    {
                        sonNode.InnerText = info.CommandTimeoutSeconds.ToString();
                    }
                }
            }
            else
            {
                //不存在，则新建
                XmlNode serviceNode = doc.SelectSingleNode("service");
                XmlElement pandaYLNode = doc.CreateElement("WebPandaYLDataService");
                XmlElement sonNode1 = doc.CreateElement("appKey");
                sonNode1.InnerText = info.AppKey.ToString();
                XmlElement sonNode2 = doc.CreateElement("appSecret");
                sonNode2.InnerText = info.AppSecret.ToString();
                XmlElement sonNode3 = doc.CreateElement("getTokenUrl");
                sonNode3.InnerText = info.GetTokenUrl.ToString();
                XmlElement sonNode4 = doc.CreateElement("getDataUrl");
                sonNode4.InnerText = info.GetDataUrl.ToString();
                XmlElement sonNode5 = doc.CreateElement("useName");
                sonNode5.InnerText = info.UseName.ToString();
                XmlElement sonNode6 = doc.CreateElement("collectInterval");
                sonNode6.SetAttribute("unit", "秒");
                sonNode6.InnerText = info.CollectInterval.ToString();
                XmlElement sonNode7 = doc.CreateElement("saveInterVal");
                sonNode7.SetAttribute("unit", "分钟");
                sonNode7.InnerText = info.SaveInterVal.ToString();
                XmlElement sonNode8 = doc.CreateElement("commandTimeoutSeconds");
                sonNode8.SetAttribute("node", "命令设定超时的秒数");
                sonNode8.InnerText = info.CommandTimeoutSeconds.ToString();
                pandaYLNode.AppendChild(sonNode1);
                pandaYLNode.AppendChild(sonNode2);
                pandaYLNode.AppendChild(sonNode3);
                pandaYLNode.AppendChild(sonNode4);
                pandaYLNode.AppendChild(sonNode5);
                pandaYLNode.AppendChild(sonNode6);
                pandaYLNode.AppendChild(sonNode7);
                pandaYLNode.AppendChild(sonNode8);
                serviceNode.AppendChild(pandaYLNode);
            }

            doc.Save(path);
            return true;
        }

    }
    // Scada-pandaZHCD子服务
    public class ConfSonWebPandaZHCDDataService : ConfigState
    {
        public bool IsNeedRun { get; set; }

        public string AppKey { get; set; } = EnvConfigInfo.appKey;
        public string AppSecret { get; set; } = EnvConfigInfo.appSecret;
        public string GetTokenUrl { get; set; } = EnvConfigInfo.tokenUrl;
        public string GetDataUrl { get; set; } = EnvConfigInfo.zhcdUrl;
        public string UseName { get; set; } = EnvConfigInfo.userName;
        public int CollectInterval { get; set; } = EnvConfigInfo.collectInterval;
        public int SaveInterVal { get; set; } = EnvConfigInfo.saveInterVal;
        public int CommandTimeoutSeconds { get; set; } = EnvConfigInfo.commandTimeoutSeconds;

        public static ConfSonWebPandaZHCDDataService LoadInfo(string projectConfigPath)
        {
            ConfSonWebPandaZHCDDataService info = new ConfSonWebPandaZHCDDataService();
            if (string.IsNullOrWhiteSpace(projectConfigPath))
            {
                info.ErrMsg = "未知的项目配置文件路径";
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadDoc(projectConfigPath, out XmlDocument doc, out string errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode node, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadNumNode(node, "WebPandaZHCDDataService", out int isNeedRun, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.IsNeedRun = isNeedRun == 1 ? true : false;
            if (!XMLHelper.ExistsNode(doc, "service/WebPandaZHCDDataService", out XmlNode clientNode, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadStringNode(clientNode, "appKey", out string appKey, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.AppKey = appKey;
            if (!XMLHelper.LoadStringNode(clientNode, "appSecret", out string appSecret, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.AppSecret = appSecret;
            if (!XMLHelper.LoadStringNode(clientNode, "getTokenUrl", out string getTokenUrl, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.GetTokenUrl = getTokenUrl;
            if (!XMLHelper.LoadStringNode(clientNode, "getDataUrl", out string getDataUrl, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.GetDataUrl = getDataUrl;
            if (!XMLHelper.LoadStringNode(clientNode, "useName", out string useName, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.UseName = useName;
            if (!XMLHelper.LoadNumNode(clientNode, "collectInterval", out int collectInterval, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.CollectInterval = collectInterval;
            if (!XMLHelper.LoadNumNode(clientNode, "saveInterVal", out int saveInterVal, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.SaveInterVal = saveInterVal;
            if (!XMLHelper.LoadNumNode(clientNode, "commandTimeoutSeconds", out int commandTimeoutSeconds, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.CommandTimeoutSeconds = commandTimeoutSeconds;

            info.EnvIsOkay = true;
            return info;
        }

        //提交子服务节点配置
        public static bool SubmitSonWebPandaZHCDData(ConfSonWebPandaZHCDDataService info, string path, out string errMsg)
        {
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
            {
                errMsg = "项目配置文件路径未指定明确";
                return false;
            }
            if (!XMLHelper.LoadDoc(path, out doc, out errMsg))
            {
                errMsg = "配置文件获取失败：" + errMsg;
                return false;
            }
            //WebPandaZHCDDataService是否在子服务列表中
            if (XMLHelper.ExistsNode(doc, "service/RunServerList/WebPandaZHCDDataService", out XmlNode node, out errMsg))
            {
                //存在，则设值为 1
                node.InnerText = "1";
            }
            else
            {
                //不存在，则添加节点信息
                if (XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode serviceList, out errMsg))
                {
                    XmlElement sonService = doc.CreateElement("WebPandaZHCDDataService");
                    sonService.SetAttribute("dsc", "WEB熊猫综合测点接入服务");
                    sonService.SetAttribute("note", "1-启动;0-不启动");
                    sonService.InnerText = "1";
                    serviceList.AppendChild(sonService);
                }
            }
            //判断是否有WebPandaZHCDDataService节点
            if (XMLHelper.ExistsNode(doc, "service/WebPandaZHCDDataService", out XmlNode pandaZHCD, out errMsg))
            {
                //存在，则更新
                foreach (XmlNode sonNode in pandaZHCD.ChildNodes)
                {
                    if (sonNode.Name == "appKey")
                    {
                        sonNode.InnerText = info.AppKey.ToString();
                    }
                    if (sonNode.Name == "appSecret")
                    {
                        sonNode.InnerText = info.AppSecret.ToString();
                    }
                    if (sonNode.Name == "getTokenUrl")
                    {
                        sonNode.InnerText = info.GetTokenUrl.ToString();
                    }
                    if (sonNode.Name == "getDataUrl")
                    {
                        sonNode.InnerText = info.GetDataUrl.ToString();
                    }
                    if (sonNode.Name == "useName")
                    {
                        sonNode.InnerText = info.UseName.ToString();
                    }
                    if (sonNode.Name == "collectInterval")
                    {
                        sonNode.InnerText = info.CollectInterval.ToString();
                    }
                    if (sonNode.Name == "saveInterVal")
                    {
                        sonNode.InnerText = info.SaveInterVal.ToString();
                    }
                    if (sonNode.Name == "commandTimeoutSeconds")
                    {
                        sonNode.InnerText = info.CommandTimeoutSeconds.ToString();
                    }
                }
            }
            else
            {
                //不存在，则新建
                XmlNode serviceNode = doc.SelectSingleNode("service");
                XmlElement pandaZHCDNode = doc.CreateElement("WebPandaZHCDDataService");
                XmlElement sonNode1 = doc.CreateElement("appKey");
                sonNode1.InnerText = info.AppKey.ToString();
                XmlElement sonNode2 = doc.CreateElement("appSecret");
                sonNode2.InnerText = info.AppSecret.ToString();
                XmlElement sonNode3 = doc.CreateElement("getTokenUrl");
                sonNode3.InnerText = info.GetTokenUrl.ToString();
                XmlElement sonNode4 = doc.CreateElement("getDataUrl");
                sonNode4.InnerText = info.GetDataUrl.ToString();
                XmlElement sonNode5 = doc.CreateElement("useName");
                sonNode5.InnerText = info.UseName.ToString();
                XmlElement sonNode6 = doc.CreateElement("collectInterval");
                sonNode6.SetAttribute("unit", "秒");
                sonNode6.InnerText = info.CollectInterval.ToString();
                XmlElement sonNode7 = doc.CreateElement("saveInterVal");
                sonNode7.SetAttribute("unit", "分钟");
                sonNode7.InnerText = info.SaveInterVal.ToString();
                XmlElement sonNode8 = doc.CreateElement("commandTimeoutSeconds");
                sonNode8.SetAttribute("node", "命令设定超时的秒数");
                sonNode8.InnerText = info.CommandTimeoutSeconds.ToString();
                pandaZHCDNode.AppendChild(sonNode1);
                pandaZHCDNode.AppendChild(sonNode2);
                pandaZHCDNode.AppendChild(sonNode3);
                pandaZHCDNode.AppendChild(sonNode4);
                pandaZHCDNode.AppendChild(sonNode5);
                pandaZHCDNode.AppendChild(sonNode6);
                pandaZHCDNode.AppendChild(sonNode7);
                pandaZHCDNode.AppendChild(sonNode8);
                serviceNode.AppendChild(pandaZHCDNode);
            }

            doc.Save(path);
            return true;
        }
    }
    // pandaControl子服务
    public class ConfSonWebPandaControlService : ConfigState
    {
        public bool IsNeedRun { get; set; }

        public string AppKey { get; set; } = EnvConfigInfo.appKey;
        public string AppSecret { get; set; } = EnvConfigInfo.appSecret;
        public string GetTokenUrl { get; set; } = EnvConfigInfo.tokenUrl;
        public string SetPumpControl { get; set; } = EnvConfigInfo.pumpControlUrl;
        public int CommandTimeoutSeconds { get; set; } = EnvConfigInfo.commandTimeoutSeconds;

        public static ConfSonWebPandaControlService LoadInfo(string projectConfigPath)
        {
            ConfSonWebPandaControlService info = new ConfSonWebPandaControlService();
            if (string.IsNullOrWhiteSpace(projectConfigPath))
            {
                info.ErrMsg = "未知的项目配置文件路径";
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadDoc(projectConfigPath, out XmlDocument doc, out string errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode node, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadNumNode(node, "WebPandaControlService", out int isNeedRun, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.IsNeedRun = isNeedRun == 1 ? true : false;
            if (!XMLHelper.ExistsNode(doc, "service/WebPandaControlService", out XmlNode clientNode, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadStringNode(clientNode, "appKey", out string appKey, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.AppKey = appKey;
            if (!XMLHelper.LoadStringNode(clientNode, "appSecret", out string appSecret, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.AppSecret = appSecret;
            if (!XMLHelper.LoadStringNode(clientNode, "getTokenUrl", out string getTokenUrl, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.GetTokenUrl = getTokenUrl;
            if (!XMLHelper.LoadStringNode(clientNode, "setPumpControl", out string setPumpControl, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.SetPumpControl = setPumpControl;
            if (!XMLHelper.LoadNumNode(clientNode, "commandTimeoutSeconds", out int commandTimeoutSeconds, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.CommandTimeoutSeconds = commandTimeoutSeconds;

            info.EnvIsOkay = true;
            return info;
        }

        //提交子服务节点配置
        public static bool SubmitSonWebPandaControlData(ConfSonWebPandaControlService info, string path, out string errMsg)
        {
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
            {
                errMsg = "项目配置文件路径未指定明确";
                return false;
            }
            if (!XMLHelper.LoadDoc(path, out doc, out errMsg))
            {
                errMsg = "配置文件获取失败：" + errMsg;
                return false;
            }
            //WebPandaControl是否在子服务列表中
            if (XMLHelper.ExistsNode(doc, "service/RunServerList/WebPandaControlService", out XmlNode node, out errMsg))
            {
                //存在，则设值为 1
                node.InnerText = "1";
            }
            else
            {
                //不存在，则添加节点信息
                if (XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode serviceList, out errMsg))
                {
                    XmlElement sonService = doc.CreateElement("WebPandaControlService");
                    sonService.SetAttribute("dsc", "WEB熊猫控制服务");
                    sonService.SetAttribute("note", "1-启动;0-不启动");
                    sonService.InnerText = "1";
                    serviceList.AppendChild(sonService);
                }
            }
            //判断是否有WebPandaControlService节点
            if (XMLHelper.ExistsNode(doc, "service/WebPandaControlService", out XmlNode pandaControl, out errMsg))
            {
                //存在，则更新
                foreach (XmlNode sonNode in pandaControl.ChildNodes)
                {
                    if (sonNode.Name == "appKey")
                    {
                        sonNode.InnerText = info.AppKey.ToString();
                    }
                    if (sonNode.Name == "appSecret")
                    {
                        sonNode.InnerText = info.AppSecret.ToString();
                    }
                    if (sonNode.Name == "getTokenUrl")
                    {
                        sonNode.InnerText = info.GetTokenUrl.ToString();
                    }
                    if (sonNode.Name == "setPumpControl")
                    {
                        sonNode.InnerText = info.SetPumpControl.ToString();
                    }
                    if (sonNode.Name == "commandTimeoutSeconds")
                    {
                        sonNode.InnerText = info.CommandTimeoutSeconds.ToString();
                    }
                }
            }
            else
            {
                //不存在，则新建
                XmlNode serviceNode = doc.SelectSingleNode("service");
                XmlElement pandaControlNode = doc.CreateElement("WebPandaControlService");
                XmlElement sonNode1 = doc.CreateElement("appKey");
                sonNode1.InnerText = info.AppKey.ToString();
                XmlElement sonNode2 = doc.CreateElement("appSecret");
                sonNode2.InnerText = info.AppSecret.ToString();
                XmlElement sonNode3 = doc.CreateElement("getTokenUrl");
                sonNode3.InnerText = info.GetTokenUrl.ToString();
                XmlElement sonNode4 = doc.CreateElement("setPumpControl");
                sonNode4.InnerText = info.SetPumpControl.ToString();
                XmlElement sonNode5 = doc.CreateElement("commandTimeoutSeconds");
                sonNode5.SetAttribute("node", "命令设定超时的秒数");
                sonNode5.InnerText = info.CommandTimeoutSeconds.ToString();
                pandaControlNode.AppendChild(sonNode1);
                pandaControlNode.AppendChild(sonNode2);
                pandaControlNode.AppendChild(sonNode3);
                pandaControlNode.AppendChild(sonNode4);
                pandaControlNode.AppendChild(sonNode5);
                serviceNode.AppendChild(pandaControlNode);
            }

            doc.Save(path);
            return true;
        }
    }
    // 特殊项目子服务
    public class ConfSonProjectDataService : ConfigState
    {
        public bool IsNeedRun { get; set; }

        public string DllPath { get; set; }
        public string ProjectName { get; set; }

        public static ConfSonProjectDataService LoadInfo(string projectConfigPath)
        {
            ConfSonProjectDataService info = new ConfSonProjectDataService();
            if (string.IsNullOrWhiteSpace(projectConfigPath))
            {
                info.ErrMsg = "未知的项目配置文件路径";
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadDoc(projectConfigPath, out XmlDocument doc, out string errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode node, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadNumNode(node, "ProjectDataService", out int isNeedRun, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.IsNeedRun = isNeedRun == 1 ? true : false;
            if (!XMLHelper.ExistsNode(doc, "service/ProjectDataService", out XmlNode clientNode, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadStringNode(clientNode, "dllPath", out string dllPath, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.DllPath = dllPath;
            if (!XMLHelper.LoadStringNode(clientNode, "projectName", out string projectName, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.ProjectName = projectName;

            info.EnvIsOkay = true;
            return info;
        }

        //提交子服务节点配置
        public static bool SubmitSonSpecialProjectData(ConfSonProjectDataService info, string path, out string errMsg)
        {
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
            {
                errMsg = "项目配置文件路径未指定明确";
                return false;
            }
            if (!XMLHelper.LoadDoc(path, out doc, out errMsg))
            {
                errMsg = "配置文件获取失败：" + errMsg;
                return false;
            }
            //SpecialProject是否在子服务列表中
            if (XMLHelper.ExistsNode(doc, "service/RunServerList/ProjectDataService", out XmlNode node, out errMsg))
            {
                //存在，则设值为 1
                node.InnerText = "1";
            }
            else
            {
                //不存在，则添加节点信息
                if (XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode serviceList, out errMsg))
                {
                    XmlElement sonService = doc.CreateElement("ProjectDataService");
                    sonService.SetAttribute("dsc", "项目的特殊服务");
                    sonService.SetAttribute("note", "1-启动;0-不启动");
                    sonService.InnerText = "1";
                    serviceList.AppendChild(sonService);
                }
            }
            //判断是否有ProjectDataService节点
            if (XMLHelper.ExistsNode(doc, "service/ProjectDataService", out XmlNode specialProject, out errMsg))
            {
                //存在，则更新
                foreach (XmlNode sonNode in specialProject.ChildNodes)
                {
                    if (sonNode.Name == "dllPath")
                    {
                        sonNode.InnerText = info.DllPath.ToString();
                    }
                    if (sonNode.Name == "projectName")
                    {
                        sonNode.InnerText = info.ProjectName.ToString();
                    }
                }
            }
            else
            {
                //不存在，则新建
                XmlNode serviceNode = doc.SelectSingleNode("service");
                XmlElement specialProjectNode = doc.CreateElement("ProjectDataService");
                XmlElement sonNode1 = doc.CreateElement("dllPath");
                sonNode1.InnerText = info.DllPath.ToString();
                XmlElement sonNode2 = doc.CreateElement("projectName");
                sonNode2.InnerText = info.ProjectName.ToString();
                specialProjectNode.AppendChild(sonNode1);
                specialProjectNode.AppendChild(sonNode2);
                serviceNode.AppendChild(specialProjectNode);
            }

            doc.Save(path);
            return true;
        }
    }

    // 历史抽稀服务
    public class ConfSonHisVacuateService : ConfigState
    {
        public bool IsNeedRun { get; set; }

        public bool ScadaIsNeedRun { get; set; } = true;
        public bool PumpIsNeedRun { get; set; } = true;

        public string EndTime { get; set; } = EnvConfigInfo.endTime;

        public static ConfSonHisVacuateService LoadInfo(string projectConfigPath)
        {
            ConfSonHisVacuateService info = new ConfSonHisVacuateService();
            if (string.IsNullOrWhiteSpace(projectConfigPath))
            {
                info.ErrMsg = "未知的项目配置文件路径";
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadDoc(projectConfigPath, out XmlDocument doc, out string errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode node, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadNumNode(node, "HisVacuateService", out int isNeedRun, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.IsNeedRun = isNeedRun == 1 ? true : false;
            if (!XMLHelper.ExistsNode(doc, "service/HisVacuateService", out XmlNode clientNode, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            if (!XMLHelper.LoadNumNode(clientNode, "ScadaIsNeedRun", out int ScadaIsNeedRun, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.ScadaIsNeedRun = ScadaIsNeedRun == 1 ? true : false;
            if (!XMLHelper.LoadNumNode(clientNode, "PumpIsNeedRun", out int PumpIsNeedRun, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.PumpIsNeedRun = PumpIsNeedRun == 1 ? true : false;
            if (!XMLHelper.LoadStringNode(clientNode, "EndTime", out string EndTime, out errMsg))
            {
                info.ErrMsg = errMsg;
                info.EnvIsOkay = false;
                return info;
            }
            info.EndTime = EndTime;

            info.EnvIsOkay = true;
            return info;
        }

        //提交子服务节点配置
        public static bool SubmitSonHisVacuateData(ConfSonHisVacuateService info, string path, out string errMsg)
        {
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
            {
                errMsg = "项目配置文件路径未指定明确";
                return false;
            }
            if (!XMLHelper.LoadDoc(path, out doc, out errMsg))
            {
                errMsg = "配置文件获取失败：" + errMsg;
                return false;
            }
            //HisVacuate是否在子服务列表中
            if (XMLHelper.ExistsNode(doc, "service/RunServerList/HisVacuateService", out XmlNode node, out errMsg))
            {
                //存在，则设值为 1
                node.InnerText = "1";
            }
            else
            {
                //不存在，则添加节点信息
                if (XMLHelper.ExistsNode(doc, "service/RunServerList", out XmlNode serviceList, out errMsg))
                {
                    XmlElement sonService = doc.CreateElement("HisVacuateService");
                    sonService.SetAttribute("dsc", "历史抽稀服务");
                    sonService.SetAttribute("note", "1-启动;0-不启动");
                    sonService.InnerText = "1";
                    serviceList.AppendChild(sonService);
                }
            }
            //判断是否有HisVacuateService节点
            if (XMLHelper.ExistsNode(doc, "service/HisVacuateService", out XmlNode hisVacuate, out errMsg))
            {
                //存在，则更新
                foreach (XmlNode sonNode in hisVacuate.ChildNodes)
                {
                    if (sonNode.Name == "ScadaIsNeedRun")
                    {
                        sonNode.InnerText = info.ScadaIsNeedRun == true ? "1" : "0";
                    }
                    if (sonNode.Name == "PumpIsNeedRun")
                    {
                        sonNode.InnerText = info.PumpIsNeedRun == true ? "1" : "0";
                    }
                    if (sonNode.Name == "EndTime")
                    {
                        sonNode.InnerText = info.EndTime;
                    }
                }
            }
            else
            {
                //不存在，则新建
                XmlNode serviceNode = doc.SelectSingleNode("service");
                XmlElement hisVacuateNode = doc.CreateElement("HisVacuateService");
                XmlElement sonNode1 = doc.CreateElement("ScadaIsNeedRun");
                sonNode1.InnerText = info.ScadaIsNeedRun == true ? "1" : "0";
                XmlElement sonNode2 = doc.CreateElement("PumpIsNeedRun");
                sonNode2.InnerText = info.PumpIsNeedRun == true ? "1" : "0";
                XmlElement sonNode3 = doc.CreateElement("EndTime");
                sonNode3.SetAttribute("note", "截止时间");
                sonNode3.InnerText = info.EndTime;
                hisVacuateNode.AppendChild(sonNode1);
                hisVacuateNode.AppendChild(sonNode2);
                hisVacuateNode.AppendChild(sonNode3);
                serviceNode.AppendChild(hisVacuateNode);
            }

            doc.Save(path);
            return true;
        }
    }

    #endregion
}
