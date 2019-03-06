using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityPublicClassLib
{
    public enum TraceType
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public enum ServerTypeName
    {
        Core,
        OPC,
        DTU,
        MQTT,
        Watcher,
        PumpAlarm,
        Web,
        Project, 
        Dispatch,
        Command,
        HisVacuate,
    }

    public class TraceItem
    {
        public TraceType type;
        public ServerTypeName serverName;
        public string text;
        public DateTime dateTime;


        public TraceItem(TraceType type, ServerTypeName serverName, string text)
        {
            this.type = type;
            this.serverName = serverName;
            this.text = text;
            this.dateTime = DateTime.Now;
        }
        public TraceItem()
        {

        }

        public string GetItemType()
        {
            string itemType = "";
            switch (this.type)
            {
                case TraceType.Error:
                    itemType = "错误";
                    break;
                case TraceType.Debug:
                    itemType = "流水";
                    break;
                case TraceType.Info:
                    itemType = "信息";
                    break;
                case TraceType.Warning:
                    itemType = "警告";
                    break;
                default:
                    itemType = "其他";
                    break;
            }
            return itemType;
        }
        public static TraceType GetItemType(string type)
        {
            TraceType traceType ;
            switch (type)
            {
                case "错误":
                    traceType = TraceType.Error; 
                    break;
                case "流水":
                    traceType = TraceType.Debug;
                    break;
                case "信息":
                    traceType = TraceType.Info;
                    break;
                case "警告":
                    traceType = TraceType.Warning;
                    break;
                default :
                    traceType = TraceType.Error;
                    break;
            }
            return traceType;
        }
        public string GetServerName()
        {
            string serverName = "";
            switch (this.serverName)
            {
                case ServerTypeName.OPC:
                    serverName = "OPC";
                    break;
                case ServerTypeName.DTU:
                    serverName = "DTU";
                    break;
                case ServerTypeName.MQTT:
                    serverName = "MQTT";
                    break;
                case ServerTypeName.Core:
                    serverName = "内核";
                    break;
                case ServerTypeName.Watcher:
                    serverName = "监视器";
                    break;
                case ServerTypeName.PumpAlarm:
                    serverName = "二供报警";
                    break;
                case ServerTypeName.Web:
                    serverName = "web接入服务";
                    break;
                case ServerTypeName.Project:
                    serverName = "项目";
                    break;
                case ServerTypeName.Dispatch:
                    serverName = "调度器";
                    break;
                case ServerTypeName.Command:
                    serverName = "命令";
                    break;
                case ServerTypeName.HisVacuate:
                    serverName = "历史抽稀";
                    break;
                default:
                    serverName = "其他";
                    break;
            }
            return serverName;
        }
        public static ServerTypeName GetServerName(string serverTypeName)
        {
            ServerTypeName serverType;
            switch (serverTypeName)
            {
                case "OPC":
                    serverType = ServerTypeName.OPC;
                    break;
                case "DTU":
                    serverType = ServerTypeName.DTU;
                    break;
                case "MQTT":
                    serverType = ServerTypeName.MQTT;
                    break;
                case "内核":
                    serverType = ServerTypeName.Core;
                    break;
                case "监视器":
                    serverType = ServerTypeName.Watcher;
                    break;
                case "二供报警":
                    serverType = ServerTypeName.PumpAlarm;
                    break;
                case "web接入服务":
                    serverType = ServerTypeName.Web;
                    break;
                case "项目":
                    serverType = ServerTypeName.Project;
                    break;
                case "调度器":
                    serverType = ServerTypeName.Dispatch;
                    break;
                case "命令":
                    serverType = ServerTypeName.Command;
                    break;
                case "历史抽稀":
                    serverType = ServerTypeName.HisVacuate;
                    break;
                default:
                    serverType = ServerTypeName.Core;
                    break;
            }
            return serverType;
        }

        public Log ToLog()
        {
            Log log = new Log();
            log.type = this.GetItemType();
            log.serverName = this.GetServerName();
            log.text = this.text;
            log.dateTime = DataUtil.ToDateString(this.dateTime);
            return log;
        }

        public static TraceItem ToTraceItem(Log log)
        {
            TraceItem traceItem = new TraceItem();
            traceItem.type = GetItemType(log.type);
            traceItem.serverName = GetServerName(log.serverName);
            traceItem.text = log.text;
            traceItem.dateTime = DataUtil.ToDateTime(log.dateTime);
            return traceItem;
        }
    }
}
