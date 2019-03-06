using CityPublicClassLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CityLogService 
{
  public  class TraceManager1 
    {
        private static ServerTypeName ServerType = ServerTypeName.Web;
        public static bool IsRuning { get; set; }
        public static Action<TraceItem> TriggerTrace1;

        public static void Start(Action<TraceItem> TriggerTrace)
        {
            if (IsRuning) 
                return;

            TriggerTrace1 = TriggerTrace;

            IsRuning = true;
        }
        public static void Stop()
        {
            if (!IsRuning)
                return;

            TriggerTrace1 = null;

            IsRuning = false;
        }

        private static void Append(TraceItem item)
        {
            if (!IsRuning)
                return;

            Manage(item);
        }

        public static void AppendErrMsg(string errMsg)
        {
            TraceItem item = new TraceItem(TraceType.Error, ServerType, errMsg);
            Append(item);
        }
        public static void AppendDebug(string debugMsg)
        {
            TraceItem item = new TraceItem(TraceType.Debug, ServerType, debugMsg);
            Append(item);
        }
        public static void AppendInfo(string infoMsg)
        {
            TraceItem item = new TraceItem(TraceType.Info, ServerType, infoMsg);
            Append(item);
        }
        public static void AppendWarning(string warningMsg)
        {
            TraceItem item = new TraceItem(TraceType.Warning, ServerType, warningMsg);
            Append(item);
        }

        private static void Manage(TraceItem item)
        {
            //// 触发日志事件
            //if (TriggerTrace1 != null)
            //    TriggerTrace1(item);

            LogManager.Append(item);
        }
    }
}
