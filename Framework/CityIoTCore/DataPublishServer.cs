using CityLogService;
using CityUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace CityIoTCore
{
    /// <summary>
    /// 数据发布器，通过MQTT实现
    /// </summary>
    class DataPublishServer
    {
        private string configPath; // 发布服务所需配置文件路径
        private MQTTParam mqttParam; //发布服务参数

        private BlockingCollection<PublishData> queue;
        private Task task;
        public bool IsRuning { get; set; }
        private MQTTServer mqttServer = null;

        public DataPublishServer(string configPath)
        {
            this.configPath = configPath;
        }

        public void Start(out string errMsg)
        {
            errMsg = "";
            if (IsRuning)
                return;

            // 加载日志数据发布服务所需配置文件信息
            this.mqttParam = new MQTTParam();
            if (!LoadConfig(this.configPath, out errMsg))
                return;

            // 开启MQTT服务当做发布器
            mqttServer = new MQTTServer(mqttParam);
            mqttServer.Start(out errMsg);
            if (!mqttServer.IsRuning || !string.IsNullOrWhiteSpace(errMsg))
            {
                Stop();
                return;
            }

            // 开启消息队列的消费器
            queue = new BlockingCollection<PublishData>();
            task = new Task(() =>
            {
                foreach (PublishData item in queue.GetConsumingEnumerable())
                {
                    try
                    {
                        ActionTimeout<PublishData> timeout = new ActionTimeout<PublishData>();
                        timeout.Do = Excute;
                        bool isTimeout = timeout.DoWithTimeout(item, new TimeSpan(0, 0, 0, 5));//只等待5秒
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
            if (!IsRuning)
                return;

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
                mqttServer.Stop();
            mqttServer = null;

            IsRuning = false;
        }

        public void Append(PublishData data)
        {
            if (!IsRuning)
                return;
            if (queue.IsAddingCompleted)
                return;
            if (queue.Count > 4096)
                return;
            queue.Add(data);
        }

        private void Excute(PublishData item)
        {
            if (mqttServer == null || mqttServer.IsRuning == false)
                return;

            if (!item.ToPayLoad())
                return;

            Task task = this.mqttServer.Publish(item);
            try
            {
                Task.WaitAll(task);// 外面加上超时判断，防止一条发送失败，后面一直卡死
            }
            catch { GC.Collect(); }
            //JsonConvert.DeserializeObject<PumpJZ>();
        }

        private bool LoadConfig(string configFilePath, out string errMsg)
        {
            errMsg = "";
            XmlDocument doc;
            if (!XMLHelper.LoadDoc(configFilePath, out doc, out errMsg))
                return false;
            if (!XMLHelper.ExistsNode(doc, "service/CityIoTService/dataPublishServer", out XmlNode node, out errMsg))
                return false;
            if (!XMLHelper.LoadStringNode(node, "ip", out this.mqttParam.ip, out errMsg))
                return false;
            if (!XMLHelper.LoadNumNode(node, "port", out this.mqttParam.port, out errMsg))
                return false;
            if (!XMLHelper.LoadStringNode(node, "userName", out this.mqttParam.userName, out errMsg))
                return false;
            if (!XMLHelper.LoadStringNode(node, "password", out this.mqttParam.password, out errMsg))
                return false;

            return true;
        }

    }
}
