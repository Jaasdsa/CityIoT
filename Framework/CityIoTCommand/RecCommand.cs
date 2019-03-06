using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityIoTCommand 
{
    public class RecCommand
    {
        public RecCommand()
        {
            // 防止命令记录时，null报错
            SessionID = "";
            ServerType = "";
            OperType = "";
            SensorID = "";
            FDBAddress = "";
            UpdateDateTime = "";
            State = "";
            BeginTime = "";
            EndTime = "";
            TimeConsuming = "";
            Message = "";
        }

        public int ID { get; set; }
        public string SessionID { get;set;}
        public string ServerType { get; set; }
        public string OperType { get; set; }
        public string SensorID { get; set; }
        public int PumpJZID { get; set; } 
        public string FDBAddress { get; set; }
        public double SetValue { get; set; }
        public string UpdateDateTime { get; set; }
        public int UserID { get; set; }
        public string State { get; set; }
        public string BeginTime { get; set; }
        public string EndTime { get; set; }
        public string TimeConsuming { get; set; }
        public string Message { get; set; }

        // 将枚举值转字符串方便存储
        public static string ToCommandServerTypeStr(CommandServerType type)
        {
            switch (type)
            {
                case CommandServerType.Pump_WEB:
                    return "二供-WEB";
                case CommandServerType.Pump_OPC:
                    return "二供_OPC";
                case CommandServerType.YL_WEB:
                    return "压力监测点_WEB";
                case CommandServerType.ZHCD_WEB:
                    return "综合测点_WEB";
                case CommandServerType.SCADA_OPC:
                    return "SCADA_OPC";
                case CommandServerType.SpecialProject:
                    return "特殊项目";
                default:
                    return "未知服务类型";
            }
        }
        public static string ToCommandOperType(CommandOperType type)
        {
            switch (type)
            {
                case CommandOperType.Read:
                    return "读";
                case CommandOperType.Write:
                    return "写";
                case CommandOperType.TestAlive:
                    return "测试连接";
                case CommandOperType.ReLoadData:
                    return "重载数据";
                default:
                    return "";
            }
        }
        public static string ToStateStr(CommandState state)
        {
            switch (state)
            {
                case CommandState.Pending:
                    return "任务获取";
                case CommandState.Waiting:
                    return "任务等待";
                case CommandState.Succeed:
                    return "任务成功";
                case CommandState.Failed:
                    return "任务失败";
                case CommandState.Timeout:
                    return "任务超时";
                default:
                    return "任务超时";
            }
        }

        // 获取两个时间差转固定格式
        public static string GetTimeSpanStr(DateTime beginTime,DateTime endTime)
        {
            TimeSpan span = endTime - beginTime;
            return span.Hours.ToString() + "小时" + span.Minutes.ToString() + "分钟" + span.Seconds.ToString() + "秒" + span.Milliseconds.ToString() + "毫秒";
        }

        // 将命令转换成可存储的命令
        public static RecCommand ToRecCommand(RequestCommand command)
        {
            RecCommand recCommand = new RecCommand();
            recCommand.SessionID = command.sessionID;
            recCommand.ServerType = ToCommandServerTypeStr(command.sonServerType);
            recCommand.OperType = ToCommandOperType(command.operType);
            recCommand.SensorID = command.sensorID;
            recCommand.PumpJZID = command.jzID;
            recCommand.FDBAddress = command.fDBAddress;
            recCommand.SetValue = command.value;
            recCommand.UpdateDateTime = DataUtil.ToDateString(command.updateTime);
            recCommand.UserID = command.userID;
            recCommand.State = ToStateStr(command.state);
            recCommand.BeginTime = DataUtil.ToDateString(command.beginTime);
            recCommand.EndTime = DataUtil.ToDateString(command.endTime);
            recCommand.TimeConsuming = command.timeConsuming;
            recCommand.Message = command.message;
            return recCommand;
        }

    }

}
