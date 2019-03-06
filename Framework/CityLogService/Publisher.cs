using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace CityLogService
{
    /// <summary>
    /// 日志发布服务器
    /// </summary>
     class LogPublisher
    {
        private ConfLogServiceInfo config; // 发布服务所需配置信息

        private MQTTParam mqttParam; //发布服务参数
        private string logNewsTopic; // 日志服务发布的主题

        private BlockingCollection<TraceItem> queue;
        private Task task;
        public bool IsRuning { get; set; }
        private MQTTServer mqttServer=null;

        public LogPublisher(ConfLogServiceInfo confLogServiceInfo)
        {
            this.config = confLogServiceInfo;
        }

        public void Start(out string errMsg)
        {
            errMsg = "";
            if (IsRuning)
                return;

            // 加载日志数据发布服务所需配置文件信息
            this.mqttParam = new MQTTParam();
            if (!LoadConfig(out errMsg))
                return;

            // 检查端口是否空闲
            if(!IPTool.IsValidPort(this.mqttParam.port))
            {
                errMsg = "日志服务器的发布端口被占用，请更换端口号";
                return;
            }

            // 开启MQTT服务当做发布器
            mqttServer = new MQTTServer(mqttParam);
            mqttServer.Start(out  errMsg);
            if(!mqttServer.IsRuning || !string.IsNullOrWhiteSpace(errMsg))
            {
                Stop();
                return;
            }

            // 开启消息队列的消费器
            queue = new BlockingCollection<TraceItem>();
            task = new Task(() =>
            {
                foreach (TraceItem item in queue.GetConsumingEnumerable())
                {
                    try
                    {
                        ActionTimeout<TraceItem> timeout = new ActionTimeout<TraceItem>();
                        timeout.Do = Excute;
                        bool isTimeout = timeout.DoWithTimeout(item,TimeSpan.FromSeconds(5));//只等待5秒
                        if (isTimeout)// 超时
                            GC.Collect();                            
                    }
                    catch
                    {

                    }
                }

            }, TaskCreationOptions.LongRunning);
            task.Start();
            IsRuning = true;


        }
        public void Stop()
        {
            // 关闭消息队列的消费器
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

            // 关闭发布器
            if (mqttServer != null)
            {
                mqttServer.Stop();
                mqttServer = null;
            }

            IsRuning = false;
        }

        public void Append(TraceItem loggerRow)
        {
            if (!IsRuning)
                return;
            if (queue.IsAddingCompleted)
                return;
            if (queue.Count > 4096)
                return;
            queue.Add(loggerRow);
        }

        private void Excute(TraceItem item)
        {
            Log log = item.ToLog();
            if (mqttServer == null || mqttServer.IsRuning == false)
                return;
            PublishData data = new PublishData() {topic= this.logNewsTopic ,data= log };
            if (!data.ToPayLoad())
                return;

            Task task= this.mqttServer.Publish(data);
            try
            {
                task.Wait(5000);// 外面加上超时判断，防止一条发送失败，后面一直卡死
            }
            catch { }
            //JsonConvert.DeserializeObject<PumpJZ>();
        }

        private bool LoadConfig(out string errMsg)
        {
            errMsg = "";
            if (!this.config.EnvIsOkay)
            {
                errMsg = this.config.ErrMsg;
                return false;
            }
            this.mqttParam.ip = this.config.IP;
            this.mqttParam.port = this.config.Port;
            this.mqttParam.userName = this.config.UserName;
            this.mqttParam.password = this.config.Password;
            this.logNewsTopic = this.config.NewsTopic;

            return true;

        }

    }
}
