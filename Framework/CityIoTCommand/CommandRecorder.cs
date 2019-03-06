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
    public class CommandRecorder : IWorker
    {
        // 消费者模型，可快速复用
        public bool IsRuning { get; set; } = false;
        private BlockingCollection<RecCommand> queue;
        private Task task;

        public void Start()
        {
            if (IsRuning)
                return;

            queue = new BlockingCollection<RecCommand>();
            task = new Task(() =>
            {
                foreach (RecCommand item in queue.GetConsumingEnumerable())
                {
                    try
                    {
                        Excute(item);
                    }
                    catch (Exception e)
                    {
                        TraceManagerForCommand.AppendErrMsg( "命令记录器记录过程中出错:" + e.Message + "堆栈:" + e.StackTrace);
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
                    TraceManagerForCommand.AppendErrMsg( "命令记录器关闭超时丢弃了" + queue.Count.ToString() + "条任务");
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
        public void Append(RecCommand data)
        {
            if (!IsRuning)
                return;
            if (queue.IsAddingCompleted)
                return;
            if (queue.Count > 4096)
            {
                TraceManagerForCommand.AppendErrMsg( "命令记录器队列到达上限无法插入");
                return;
            }
            queue.Add(data);
        }

        private void Excute(RecCommand command)
        {
            command.UpdateDateTime = DataUtil.ToDateString(DateTime.Now);
            string sql = string.Format(@"INSERT INTO IoTCommand (会话ID,服务类型, 操作类型,SensorID,PumpJZID,数据业务地址,预设值,最后更新时间,用户ID,状态,开始时间,结束时间,耗时,消息) 
                                                            VALUES ('{0}','{1}','{2}','{3}',{4},'{5}',{6},'{7}',{8},'{9}','{10}','{11}','{12}','{13}');",
                                                                  DataUtil.ToString(command.SessionID), DataUtil.ToString(command.ServerType), DataUtil.ToString(command.OperType), DataUtil.ToString(command.SensorID),
                                                                    DataUtil.ToInt(command.PumpJZID), DataUtil.ToString(command.FDBAddress), DataUtil.ToDouble(command.SetValue), DataUtil.ToDateString(command.UpdateDateTime),
                                                                    DataUtil.ToInt(command.UserID), DataUtil.ToString(command.State), DataUtil.ToDateString(command.BeginTime), DataUtil.ToDateString(command.EndTime), DataUtil.ToString(command.TimeConsuming), DataUtil.ToString(command.Message));
            DBUtil.ExecuteNonQuery(sql, out string errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                TraceManagerForCommand.AppendErrMsg( "命令记录器记录数据库失败"+ errMsg);

        }
    }
}
