using CityIoTCommand;
using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CityIoTCommand 
{
   public class CommandConsumer:IWorker
    {
        public bool IsRuning { get; set; } = false;
        private  BlockingCollection<RequestCommand> queue;
        private  Task task;
        public Action<RequestCommand> actExcute;

        public CommandConsumer (Action<RequestCommand> excuteCommand) 
        {
            actExcute = excuteCommand;
        }

        public  void Start()
        {
            if (IsRuning)
                return;

            queue = new BlockingCollection<RequestCommand>();
            task = new Task(() =>
            {
                foreach (RequestCommand item in queue.GetConsumingEnumerable())
                {
                    try
                    {
                        Excute(item);
                    }
                    catch(Exception e)
                    {
                        TraceManagerForCommand.AppendErrMsg("命令消费器消费过程中出错:"+ e.Message + "堆栈:" + e.StackTrace);
                    }
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
                    TraceManagerForCommand.AppendErrMsg("命令消费器关闭超时丢弃了" + queue.Count.ToString() + "条任务");
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
        public  void Append(RequestCommand command)
        {
            if (!IsRuning)
                return;
            if (queue.IsAddingCompleted)
                return;
            if (queue.Count > 4096)
            {
                TraceManagerForCommand.AppendErrMsg("命令消费队列到达上限无法插入");
                return;
            }
            queue.Add(command);
        }

        // 命令消费-设定超时功能
        private void Excute(RequestCommand command)
        {
            // 设定超时功能
            ActionTimeout<RequestCommand> timeout = new ActionTimeout<RequestCommand>();
            timeout.Do = ExcuteHandle;
            bool bo = timeout.DoWithTimeout(command, new TimeSpan(0, 0, 0, command.timeoutSeconds+3));//设定基础多等待3秒，防止调用着也做了超时功能
            if (bo)
            {
                CommandManager.MakeTimeout("会话ID：" + command.sensorID + "执行命令超时", ref command);
                CommandManager.CompleteCommand(command);
                TraceManagerForCommand.AppendErrMsg(command.message);
                GC.Collect();
            }
        }
        private void ExcuteHandle(RequestCommand command)
        {
            if (actExcute == null)
            {
                CommandManager.MakeFail("命令执行体未注册", ref command);
                CommandManager.CompleteCommand(command);
                TraceManagerForCommand.AppendErrMsg(command.message);
                return;
            }
            actExcute(command);         
        }
    }


}
