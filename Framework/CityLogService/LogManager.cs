using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace CityLogService
{
    /// <summary>
    ///  日志管理器，包含日志的记录，分割，发布
    /// </summary>
    public class LogManager
    {
        static ConfLogServiceInfo config; // 日志配置参数 

        private static string fileFullPath;//日志文件路径
        private static string logTableName;// 存储的日志的表名

        private static BlockingCollection<TraceItem> queue;
        private static Task task;
        public static bool IsRuning { get; set; }

        private static LogPublisher logPublisher = null;
        private static LogRecorder logRecorder = null;
        private static LogShrink logShrink = null;

        public static void Start(ConfLogServiceInfo confLogServiceInfo, out string errMsg)
        {
            errMsg = "";
            config = confLogServiceInfo;

            if (IsRuning)
                return;

            // 加载日志环境所需的配置文件信息
            if (!CheckConfig(config,out errMsg))
                return;

            // 打开日志发布者
            logPublisher = new LogPublisher(config);
            logPublisher.Start(out errMsg);
            if (!logPublisher.IsRuning)
            {
                Stop();
                return;
            }

            // 打开日志记录者
            logRecorder = new LogRecorder(fileFullPath, logTableName);
            logRecorder.Start();
            if (!logRecorder.IsRuning)
            {
                Stop();
                return;
            }

            // 打开日志收缩器
            logShrink = new LogShrink(fileFullPath, logTableName, config.ManintainTime, config.LogMaxSaveDays);
            logShrink.Start();
            if (!logShrink.IsRuning)
            {
                Stop();
                return;
            }


            // 打开日志管理者
            StartMannge();
            if (!ManageIsRuning)
            {
                Stop();
                return;
            }

            IsRuning = true;
        }
        public static void Stop()
        {
            StopManage();

            // 关闭维护期
            if (logShrink != null)
            {
                logShrink.Stop();
                logShrink = null;
            }

            // 关闭记录者
            if(logRecorder != null)
            {
                logRecorder.Stop();
                logRecorder = null;
            }

            //关闭日志发布者
            if (logPublisher != null)
            {
                logPublisher.Stop();
                logPublisher = null;
            }

            IsRuning = false;
        }

        private static bool CheckConfig(ConfLogServiceInfo config, out string errMsg)
        {
            errMsg = "";
            if (!config.EnvIsOkay)
            {
                errMsg = config.ErrMsg;
                return false;
            }

            fileFullPath = config.filePath;
            logTableName = config.tableName;

            return true;
        }

        private static bool ManageIsRuning { get; set; } = false;
        private static void StartMannge()
        {
            if (ManageIsRuning)
                return;

            queue = new BlockingCollection<TraceItem>();
            task = new Task(() =>
            {
                foreach (TraceItem item in queue.GetConsumingEnumerable())
                {
                    try
                    {
                        Manage(item);
                    }
                    catch { }
                }

            }, TaskCreationOptions.LongRunning);
            task.Start();

            ManageIsRuning = true;
        }
        private static void StopManage()
        {
            if (!ManageIsRuning)
                return;

            // 先完成添加
            queue.CompleteAdding();
            DateTime time1 = DateTime.Now;
            while (queue.Count > 0)
            {
                Thread.Sleep(1);
                // 最多等待10秒避免关不掉
                if (DateTime.Now - time1 > TimeSpan.FromSeconds(10))
                {
                    break;
                }
            }
            while (queue.Count > 0)
            {
                // 等了十秒还没听，队列全部元素废弃
                queue.Take();
            }

            queue = null;
            Task.WaitAll(task);
            task.Dispose();
            task = null;
            ManageIsRuning = false;
        }
        private static void Manage(TraceItem item)
        {
            logPublisher.Append(item);

            logRecorder.Append(item);
        }

        // 日志追加的入口
        public static void Append(TraceItem item)
        {
            if (!IsRuning)
                return;
            if (queue.IsAddingCompleted)
                return;
            if (queue.Count > 4096)
                return;
            queue.Add(item);
        }
        public static void AppendErrMsg(ServerTypeName serverName,string errMsg)
        {
            TraceItem item = new TraceItem(TraceType.Error, serverName, errMsg);
            Append(item);
        }
        public static void AppendDebug(ServerTypeName serverName, string debugMsg)
        {
            TraceItem item = new TraceItem(TraceType.Debug, serverName, debugMsg);
            Append(item);
        }
        public static void AppendInfo(ServerTypeName serverName, string infoMsg)
        {
            TraceItem item = new TraceItem(TraceType.Info, serverName, infoMsg);
            Append(item);
        }
        public static void AppendWarning(ServerTypeName serverName, string warningMsg)
        {
            TraceItem item = new TraceItem(TraceType.Warning, serverName, warningMsg);
            Append(item);
        }

        // 专门给调度使用，在另外一个进程，只记录
        public static void StartDisPatchRecorder(ConfLogServiceInfo confLogServiceInfo) 
        {
            if (CheckConfig(confLogServiceInfo, out string errMsg))
                DisPatchRecordEnvFlag = true;
            else
                DisPatchRecordEnvFlag = false;
        }
        static bool DisPatchRecordEnvFlag = false;
        public static void RecordDisPatchLog(TraceItem item)
        {
            if (!DisPatchRecordEnvFlag)
                return;
            try
            {
                SQLiteHelper sqLite = new SQLiteHelper(fileFullPath);
                Log log = item.ToLog();
                string id = Guid.NewGuid().ToString();
                string sql = string.Format(@"insert into {0} (类型,信息文本,插入时间,系统名称) values ('{1}',""{2}"",""{3}"",""{4}"")",
                                           logTableName, log.type, log.text, log.dateTime, log.serverName);
                sqLite.ExecuteNonQuery(sql);
            }
            catch { }
        }
    }
    public class TraceManagerForDispatch 
    {
        public static void AppendErrMsg(string errMsg)
        {
            LogManager.AppendErrMsg(ServerTypeName.Core, errMsg);
        }
        public static void AppendDebug(string debugMsg)
        {
            LogManager.AppendDebug(ServerTypeName.Core, debugMsg);
        }
        public static void AppendInfo(string infoMsg)
        {
            LogManager.AppendInfo(ServerTypeName.Core, infoMsg);
        }
        public static void AppendWarning(string warningMsg)
        {
            LogManager.AppendWarning(ServerTypeName.Core, warningMsg);
        }
    }

    public class TraceManagerForPumpAlarm 
    {
        public static void AppendErrMsg(string errMsg)
        {
            LogManager.AppendErrMsg(ServerTypeName.PumpAlarm, errMsg);
        }
        public static void AppendDebug(string debugMsg)
        {
            LogManager.AppendDebug(ServerTypeName.PumpAlarm, debugMsg);
        }
        public static void AppendInfo(string infoMsg)
        {
            LogManager.AppendInfo(ServerTypeName.PumpAlarm, infoMsg);
        }
        public static void AppendWarning(string warningMsg)
        {
            LogManager.AppendWarning(ServerTypeName.PumpAlarm, warningMsg);
        }
    }

    public class TraceManagerForWeb
    {
        public static void AppendErrMsg(string errMsg)
        {
            LogManager.AppendErrMsg(ServerTypeName.Web, errMsg);
        }
        public static void AppendDebug(string debugMsg)
        {
            LogManager.AppendDebug(ServerTypeName.Web, debugMsg);
        }
        public static void AppendInfo(string infoMsg)
        {
            LogManager.AppendInfo(ServerTypeName.Web, infoMsg);
        }
        public static void AppendWarning(string warningMsg)
        {
            LogManager.AppendWarning(ServerTypeName.Web, warningMsg);
        }
    }

    public class TraceManagerForOPC
    {
        public static void AppendErrMsg(string errMsg)
        {
            LogManager.AppendErrMsg(ServerTypeName.OPC, errMsg);
        }
        public static void AppendDebug(string debugMsg)
        {
            LogManager.AppendDebug(ServerTypeName.OPC, debugMsg);
        }
        public static void AppendInfo(string infoMsg)
        {
            LogManager.AppendInfo(ServerTypeName.OPC, infoMsg);
        }
        public static void AppendWarning(string warningMsg)
        {
            LogManager.AppendWarning(ServerTypeName.OPC, warningMsg);
        }
    }

    public class TraceManagerForDTU
    {
        public static void AppendErrMsg(string errMsg)
        {
            LogManager.AppendErrMsg(ServerTypeName.DTU, errMsg);
        }
        public static void AppendDebug(string debugMsg)
        {
            LogManager.AppendDebug(ServerTypeName.DTU, debugMsg);
        }
        public static void AppendInfo(string infoMsg)
        {
            LogManager.AppendInfo(ServerTypeName.DTU, infoMsg);
        }
        public static void AppendWarning(string warningMsg)
        {
            LogManager.AppendWarning(ServerTypeName.DTU, warningMsg);
        }
    }

    public class TraceManagerForProject 
    {
        public static void AppendErrMsg(string errMsg)
        {
            LogManager.AppendErrMsg(ServerTypeName.Project, errMsg);
        }
        public static void AppendDebug(string debugMsg)
        {
            LogManager.AppendDebug(ServerTypeName.Project, debugMsg);
        }
        public static void AppendInfo(string infoMsg)
        {
            LogManager.AppendInfo(ServerTypeName.Project, infoMsg);
        }
        public static void AppendWarning(string warningMsg)
        {
            LogManager.AppendWarning(ServerTypeName.Project, warningMsg);
        }
    }

    public class TraceManagerForCommand
    {
        public static void AppendErrMsg(string errMsg)
        {
            LogManager.AppendErrMsg(ServerTypeName.Command, errMsg);
        }
        public static void AppendDebug(string debugMsg)
        {
            LogManager.AppendDebug(ServerTypeName.Command, debugMsg);
        }
        public static void AppendInfo(string infoMsg)
        {
            LogManager.AppendInfo(ServerTypeName.Command, infoMsg);
        }
        public static void AppendWarning(string warningMsg)
        {
            LogManager.AppendWarning(ServerTypeName.Command, warningMsg);
        }
    }

    public class TraceManagerForHisVacuate 
    {
        public static void AppendErrMsg(string errMsg)
        {
            LogManager.AppendErrMsg(ServerTypeName.HisVacuate, errMsg);
        }
        public static void AppendDebug(string debugMsg)
        {
            LogManager.AppendDebug(ServerTypeName.HisVacuate, debugMsg);
        }
        public static void AppendInfo(string infoMsg)
        {
            LogManager.AppendInfo(ServerTypeName.HisVacuate, infoMsg);
        }
        public static void AppendWarning(string warningMsg)
        {
            LogManager.AppendWarning(ServerTypeName.HisVacuate, warningMsg);
        }
    }
}
