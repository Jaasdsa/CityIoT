using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CityIoTServiceWatcher
{
    class LogSubscriber
    {
        private MQTTClientParam param;
        private string filePath;
        private string tableName;
        private string newsTopic;

        public LogSubscriber()
        {
            param = new MQTTClientParam();
            param.clientID = Guid.NewGuid().ToString();

            param.ip = Config.envConfigInfo.confLogServiceInfo.IP;
            param.port = Config.envConfigInfo.confLogServiceInfo.Port;
            param.userName = Config.envConfigInfo.confLogServiceInfo.UserName;
            param.password =MD5Util.MD5Decrypt(Config.envConfigInfo.confLogServiceInfo.Password);

            filePath = Config.envConfigInfo.confLogServiceInfo.filePath;
            tableName = Config.envConfigInfo.confLogServiceInfo.tableName;
            newsTopic = Config.envConfigInfo.confLogServiceInfo.NewsTopic;
        }

        private LogReciver logReciver;
        private MQTTClient mqttClient;
        public bool IsRuning { get; set; }
        private Action<Log> actReciveLog;
        public void Start(Action<Log> actReciveLog, out string errMsg)
        {
            errMsg = "";
            this.actReciveLog = actReciveLog;
            //日志接收触发器
            this.logReciver = new LogReciver(this.newsTopic, this.actReciveLog);
            this.logReciver.Start();
            if (!this.logReciver.IsRuning)
            {
                Stop();
                return;
            }

            // 启动MQTT客户端
            mqttClient = new MQTTClient(this.param,ReciveMqttClientDataHander);
            mqttClient.Connect(out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                Stop();
                return;
            }

            if (!this.mqttClient.IsConnected)
            {
                errMsg = "连接MQTT客户端失败";
                Stop();
                return;
            }
            // 订阅日志事件
            this.mqttClient.SubScribe(this.newsTopic, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                Stop();
                return;
            }

            //启动维护MQTT维护线程
            //StartKeepAlive();

            IsRuning = true;
        }

        public void Stop()
        {
            //维护MQTT维护线程
            //StopKeepAlive();

            if (this.mqttClient != null)
            {
                try
                {
                    if (this.mqttClient.IsConnected)
                        this.mqttClient.UnSubScribe(this.newsTopic, out string errMsg);
                }
                catch { }

                try
                {
                    this.mqttClient.DisConnect();
                }
                catch { }
                this.mqttClient = null;
            }

            if (this.logReciver != null)
            {
                try
                {
                    this.logReciver.Stop();
                    this.logReciver = null;
                }
                catch { }
            }

            IsRuning = false;
        }

        private void ReciveMqttClientDataHander(MQTTTopicData data)
        {
            if (this.logReciver != null && this.logReciver.IsRuning)
                this.logReciver.Append(data);
        }

        private Task aliveTask;
        private CancellationTokenSource tokenSource;
        private void StartKeepAlive()
        {
            tokenSource = new CancellationTokenSource();
            aliveTask = new Task(new Action(()=> {
                int times = 0;
                while (!tokenSource.IsCancellationRequested)
                {
                    if (!this.mqttClient.IsConnected)
                    {
                        times++;
                        if (times > 10)
                        {
                            times = 0;  //每10秒重连一次
                            this.actReciveLog(new TraceItem(TraceType.Info, ServerTypeName.Watcher, "日志服务器自动重连中...").ToLog());
                            this.mqttClient.ReConnect(out string errMsg);
                            if (!string.IsNullOrEmpty(errMsg) && this.mqttClient.IsConnected)
                                this.actReciveLog(new TraceItem(TraceType.Info, ServerTypeName.Watcher, "日志服务器自动重连成功").ToLog());
                            else
                                this.actReciveLog(new TraceItem(TraceType.Error, ServerTypeName.Watcher, "日志服务器重连失败,等待下次重连。"+ errMsg).ToLog());
                        }
                    }
                    Thread.Sleep(1000);
                }
            }), tokenSource.Token);
            aliveTask.Start();
        }
        private void StopKeepAlive()
        {
            if (aliveTask != null)
            {
                if (tokenSource != null)
                    tokenSource.Cancel();// 停止监视
                try { aliveTask.Wait(); }
                catch { }
                aliveTask.Dispose();
                aliveTask = null;
            }
        }
    }

    class LogReciver : IWorker
    {
        public bool IsRuning { get; set; } = false;
        private BlockingCollection<MQTTTopicData> queue;
        private Task task;

        private string logNewsTopic;
        private Action<Log> actionReciveLog;

        public LogReciver(string logNewsTopic,Action<Log> actionReciveLog)
        {
            this.logNewsTopic = logNewsTopic;
            this.actionReciveLog = actionReciveLog;
        }

        public void Start()
        {
            if (IsRuning)
                return;

            queue = new BlockingCollection<MQTTTopicData>();
            task = new Task(() =>
            {
                foreach (MQTTTopicData data  in queue.GetConsumingEnumerable())
                {
                    try
                    {
                        Excute(data);
                    }
                    catch (Exception e)
                    {

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
                    break;
                }
            }
            while (queue.Count > 0)
            {
                // 等了十秒还没听，队列全部元素废弃
                queue.Take();
            }

            queue = null;
            task.Wait(3000);
            task.Dispose();
            task = null;

            IsRuning = false;
        }
        public void Append(MQTTTopicData data)
        {
            if (!IsRuning)
                return;
            if (queue.IsAddingCompleted)
                return;
            if (queue.Count > 4096)
                return;

            queue.Add(data);
        }
        private void Excute(MQTTTopicData data)
        {
            if (data == null)
                return;

            // 只接收机日志消息
            if (data.topic != this.logNewsTopic)
                return;
            if (data.palyLoad == null)
                return;

            Log log = ByteUtil.ToDeserializeObject<Log>(data.palyLoad);

            actionReciveLog(log);

        }
    } 
}
