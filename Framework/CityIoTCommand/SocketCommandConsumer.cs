using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CityIoTCommand
{
    class SocketCommandConsumer : IWorker
    {
        public bool IsRuning { get; set; } = false;
        private BlockingCollection<ReciveData> queue;
        private Task task;
        private  Action<RequestCommand> DoRequestCommand;

        public SocketCommandConsumer(Action<RequestCommand> actionRequestCommand)
        {
            this.DoRequestCommand = actionRequestCommand;
        }

        public void Start()
        {
            if (IsRuning)
                return;

            queue = new BlockingCollection<ReciveData>();
            task = new Task(() =>
            {
                foreach (ReciveData item in queue.GetConsumingEnumerable())
                {
                    try
                    {
                        Excute(item);
                    }
                    catch (Exception e)
                    {
                        TraceManagerForCommand.AppendErrMsg( "命令消费器消费过程中出错:" + e.Message + "堆栈:" + e.StackTrace);
                    }
                }

            }, TaskCreationOptions.LongRunning);
            task.Start();

            IsRuning = true;
        }
        public void Stop()
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
                    TraceManagerForCommand.AppendErrMsg( "命令消费器关闭超时丢弃了" + queue.Count.ToString() + "条任务");
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

        public void Append(ReciveData reciveData)
        {
            if (!IsRuning)
                return;
            if (queue.IsAddingCompleted)
                return;
            if (queue.Count > 4096)
            {
                TraceManagerForCommand.AppendErrMsg( "命令消费队列到达上限无法插入");
                return;
            }
            queue.Add(reciveData);
        }

        /// <summary>
        /// 命令消费
        /// </summary>
        /// <param name="reciveData"></param>
        private void Excute(ReciveData reciveData)
        {
            //将接受到数据反序列化成命令对象
            RequestCommand request;
            try
            {
                request = ByteUtil.ToDeserializeObject<RequestCommand>(reciveData.data.body);
            }
            catch (Exception e)
            {
                ResponseCommand response = new ResponseCommand()
                {
                    errMsg = "序列化请求对象失败," + e.Message + "堆栈:" + e.StackTrace,
                    statusCode = "440",
                    info = ""
                };
                string data = ByteUtil.ToSerializeObject(response);
                reciveData.FinshCallBack(reciveData.sessionID, data,true);
                TraceManagerForCommand.AppendErrMsg(response.errMsg);
                return;
            }
            // 序列化检查
            if (request == null)
            {
                ResponseCommand response = new ResponseCommand()
                {
                    errMsg = "序列化请求对象失败,",
                    statusCode = "440",
                    info = ""
                };
                string data = ByteUtil.ToSerializeObject(response);
                reciveData.FinshCallBack(reciveData.sessionID, data, true);
                TraceManagerForCommand.AppendErrMsg(response.errMsg);
                return;
            }
            // 将委托和会话ID传递给命令对象
            request.FinshCallBack = reciveData.FinshCallBack;
            request.sessionID = reciveData.sessionID;
            //命令记录
            TraceManagerForCommand.AppendDebug("已获取命令ID:"+ request.ID);
            // 让调度开始分发给对应的子服务
            this.DoRequestCommand(request);
        }

    }
}
