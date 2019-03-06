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
  public  class TraceTrigger
    {
        public event Action<TraceItem> evtTraceTrigger;
        
        private ServerTypeName serverTypeName;
        private  BlockingCollection<TraceItem> queue;
        private  Task task;
        public  bool IsRuning { get; set; }

        public TraceTrigger(ServerTypeName serverTypeName)
        {
            this.serverTypeName = serverTypeName;
        }

        public  void Start()
        {
            if (IsRuning)
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

            IsRuning = true;
        }
        public  void Stop()
        {
            if (!IsRuning)
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

            IsRuning = false;
        }

        private  void Append(TraceItem item)
        {
            if (!IsRuning)
                return;
            if (queue.IsAddingCompleted)
                return;
            if (queue.Count > 4096)
                return;
            queue.Add(item);
        }
        public  void AppendErrMsg(string errMsg)
        {
            TraceItem item = new TraceItem(TraceType.Error,this.serverTypeName, errMsg);
            Append(item);
        }
        public  void AppendDebug(string debugMsg)
        {
            TraceItem item = new TraceItem(TraceType.Debug, this.serverTypeName, debugMsg);
            Append(item);
        }
        public  void AppendInfo(string infoMsg)
        {
            TraceItem item = new TraceItem(TraceType.Info, this.serverTypeName, infoMsg);
            Append(item);
        }
        public  void AppendWarning(string warningMsg)
        {
            TraceItem item = new TraceItem(TraceType.Warning, this.serverTypeName, warningMsg);
            Append(item);
        }

        private  void Manage(TraceItem item)
        {
            // 触发该事件
            if (evtTraceTrigger != null)
                evtTraceTrigger(item);
        }               
    }
}
