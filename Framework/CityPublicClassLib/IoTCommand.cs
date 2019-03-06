using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityPublicClassLib
{
    public enum CommandServerType
    {
        // 子服务类型
        UnKnown,
        Pump_WEB,
        Pump_OPC,
        YL_WEB,
        ZHCD_WEB,
        SCADA_OPC,
        SpecialProject,
    }
    public enum CommandOperType
    {
        // 命令的操作模式
        UnKnown,
        Read,
        Write,
        TestAlive,
        ReLoadData,
    }
    public enum CommandState
    {
        // 命令的状态
        Pending,
        Waiting,
        Succeed,
        Failed,
        Timeout,
    }

    /// <summary>
    /// 请求命令的结构
    /// </summary>
    public class RequestCommand
    {
        public string ID { get; set; }
        public string sessionID;
        public CommandServerType sonServerType;
        public CommandOperType operType; 
        public int jzID;
        public string fDBAddress;
        public string sensorID;
        public double value;
        public DateTime updateTime;
        public int userID;
        public CommandState state;
        public DateTime beginTime;
        public DateTime endTime;
        public string timeConsuming;
        public string message;

        public int timeoutSeconds; // 超时时间

        /// <summary>
        /// 任务结束的回调,string sessionID, string data,bool isCloseSession,string info
        /// </summary>
        public Func<string, string, bool,string> FinshCallBack;
    }

    /// <summary>
    /// 回应命令的结构
    /// </summary>
    public class ResponseCommand
    {
        public string info;
        public string statusCode;
        public string errMsg;
    }

}
