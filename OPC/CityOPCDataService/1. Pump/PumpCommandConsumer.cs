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

namespace CityOPCDataService
{
    class PumpCommandConsumer:IWorker
    {
        public bool IsRuning { get; set; } = false;
        BlockingCollection<PumpCommand> queue;
        Task task;
        OpcClient opcClientManager;

        public PumpCommandConsumer(OpcClient opcClientManager)
        {
            this.opcClientManager = opcClientManager;
        }

        public  void Start()
        {
            if (IsRuning)
                return;

            queue = new BlockingCollection<PumpCommand>();
            task = new Task(() =>
            {
                foreach (PumpCommand item in queue.GetConsumingEnumerable())
                {
                    try
                    {
                        Excute(item);
                    }
                    catch(Exception e)
                    {
                        TraceManagerForOPC.AppendErrMsg("二供-命令消费器消费过程中出错:"+ e.Message + "堆栈:" + e.StackTrace);
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
                    TraceManagerForOPC.AppendErrMsg("二供-命令消费器关闭超时丢弃了" + queue.Count.ToString() + "条任务");
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
        public  void Append(PumpCommand command)
        {
            if (!IsRuning)
                return;
            if (queue.IsAddingCompleted)
                return;
            if (queue.Count > 4096)
            {
                TraceManagerForOPC.AppendErrMsg("二供-命令消费队列到达上限无法插入");
                return;
            }
            queue.Add(command);
        }

        // 命令消费-设定超时功能
        private void Excute(PumpCommand command)
        {
            command.Excute();
        }

        // 调度过来的命令
        public void ReceiveCommand(RequestCommand command)
        {
           // TraceManagerForOPC.AppendInfo("PUMP-OPC子服务当前任务条目数:"+ this.queue.Count.ToString());
            //写值操作
            if (command.sonServerType == CommandServerType.Pump_OPC && command.operType == CommandOperType.Write)
            {
                TakeOutCollectCommand();
                this.Append(PumpCommand.CreateWriteCommand(this.opcClientManager, command));
                //命令记录
                TraceManagerForCommand.AppendDebug(string.Format("命令ID:{0}开始执行JZID:{1},业务地址{2},设定值{3}操作",command.ID,command.jzID,command.fDBAddress,command.value));
                return;
            }
            // OPC_Pump--重载请求
            if (command.sonServerType == CommandServerType.Pump_OPC && command.operType == CommandOperType.ReLoadData)
            {
                TakeOutCollectCommand();
                this.Append(PumpCommand.CreateReloadCommand(this.opcClientManager, command));
                //命令记录
                TraceManagerForCommand.AppendDebug(string.Format("命令ID:{0}开始执行PUMP-OPC重载机组数据操作:" , command.ID));
                return;
            }
            // 错误请求
            CommandManager.MakeFail("错误的请求类型", ref command);
            CommandManager.CompleteCommand(command);
            TraceManagerForCommand.AppendErrMsg(command.message);
            return;
        }

        // 丢掉采集命令
        public void TakeOutCollectCommand()
        {
            try
            {
                foreach (PumpCommand command in queue)
                {
                    command.CollectTaskWriteCancleToken();
                }
            }
            catch(Exception e)
            {
                TraceManagerForCommand.AppendErrMsg("优先执行Command，采集任务移除失败"+e.Message+"堆栈"+e.StackTrace);
            }
        }
    }
}
