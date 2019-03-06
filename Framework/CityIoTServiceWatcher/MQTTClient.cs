using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CityIoTServiceWatcher
{
    public class MQTTClientParam
    {
        public string clientID;
        public string ip;
        public int port;
        public string userName;
        public string password;
    }

    public class MQTTTopicData
    {
        public string topic;
        public byte[] palyLoad;
    }

    class MQTTClient
    {
        private MQTTClientParam param;
        private Action<MQTTTopicData> actionReciveData;
        private MqttClient mqttClient = null;

        /// <summary>
        /// 初始化MQTT客户端
        /// </summary>
        /// <param name="param">连接目标服务器参数</param>
        /// <param name="actionReciveData">接受主题数据的方法</param>
        public MQTTClient(MQTTClientParam param, Action<MQTTTopicData> actionReciveData)
        {
            this.param = param;
            this.actionReciveData = actionReciveData;
        }

        public bool IsConnected { get; set; }
        public void Connect(out string errMsg)
        {
            errMsg = "";
            if (this.mqttClient != null && this.mqttClient.IsConnected)
                return;

            if (this.param == null)
            {
                errMsg = "连接MQTT服务端失败,没有指明服务参数";
                return;
            }
            try
            {
                mqttClient = new MqttFactory().CreateMqttClient() as MqttClient;
                this.OnloadEvents();

                MqttClientOptionsBuilder optionsBuilder = new MqttClientOptionsBuilder();

                optionsBuilder.WithCommunicationTimeout(TimeSpan.FromSeconds(10));// 只连接等待10秒
                optionsBuilder.WithTcpServer(this.param.ip, this.param.port);
                optionsBuilder.WithCredentials(this.param.userName,this.param.password);
                optionsBuilder.WithClientId(this.param.clientID);
                optionsBuilder.WithCleanSession(true);// 临时会话

                Task task = mqttClient.ConnectAsync(optionsBuilder.Build());
                task.Wait(5000);

                if (!this.IsConnected)
                    errMsg = "连接MQTT服务器失败";
            }
            catch (Exception ex)
            {
                errMsg = "连接到MQTT服务器失败！" + ex.Message;
            }
        }
        public void DisConnect()
        {
            if (this.mqttClient != null)
            {
                try
                {     
                    Task task = mqttClient.DisconnectAsync();
                    this.IsConnected = false;
                    task.Wait(5000);
                    this.RemoveEvents();
                    mqttClient.Dispose();
                    mqttClient = null;
                }
                catch
                {

                }
            }
        }
        public void ReConnect(out string errMsg)
        {
            this.DisConnect();
            this.Connect(out errMsg);
            if (!this.IsConnected)
                errMsg = "重连失败";
        }

        // MQTT 客户端事件
        private void MqttClient_Connected(object sender, EventArgs e)
        {
            this.IsConnected = this.mqttClient.IsConnected;
        }
        private void MqttClient_Disconnected(object sender, EventArgs e)
        {
            this.IsConnected = false;
            this.RemoveEvents();
        }
        private void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            MQTTTopicData data = new MQTTTopicData();
            data.topic = e.ApplicationMessage.Topic;
            data.palyLoad = e.ApplicationMessage.Payload;
            actionReciveData(data);// 触发接受事件
        }
        private void OnloadEvents()
        {
            try
            {
                if (this.mqttClient != null)
                {
                    mqttClient.ApplicationMessageReceived += MqttClient_ApplicationMessageReceived;
                    mqttClient.Connected += MqttClient_Connected;
                    mqttClient.Disconnected += MqttClient_Disconnected;
                }
            }
            catch { }
        }
        private void RemoveEvents()
        {
            try
            {
                if (this.mqttClient != null)
                {
                    mqttClient.ApplicationMessageReceived -= MqttClient_ApplicationMessageReceived;
                    mqttClient.Connected -= MqttClient_Connected;
                    mqttClient.Disconnected -= MqttClient_Disconnected;
                }
            }
            catch { }
        }

        // MQTT订阅方法
        public void SubScribe(string topic,out string errMsg)
        {
            errMsg = "";

            if (string.IsNullOrEmpty(topic))
            {
                errMsg="订阅主题不能为空!";
                return;
            }

            if (!mqttClient.IsConnected)
            {
                errMsg="MQTT客户端尚未连接！";
                return;
            }

            Task task = mqttClient.SubscribeAsync(new List<TopicFilter> {
                new TopicFilter(topic, MqttQualityOfServiceLevel.AtMostOnce)
            });
            task.Wait(5000);// 最多只等待5秒

        }
        public void UnSubScribe(string topic,out string errMsg) 
        {
            errMsg = "";

            if (string.IsNullOrEmpty(topic))
            {
                errMsg = "取消订阅主题不能为空!";
                return;
            }

            if (!mqttClient.IsConnected)
            {
                errMsg = "MQTT客户端尚未连接！";
                return;
            }

            Task task = mqttClient.UnsubscribeAsync(new string[1] { topic});
            task.Wait(5000);// 最多只等待5秒
        }

        // MQTT 消息发布方法
        public void PublishAsync(MQTTTopicData data,out string errMsg)
        {
            errMsg = "";
            if (data == null)
            {
                errMsg = "发布的数据包错误";
                return;
            }
            if (data.topic == null)
            {
                errMsg = "发布的数据主题不能为空";
                return;
            }
            if (data.palyLoad == null)
            {
                errMsg = "发布的数据包有效载荷不能为空";
                return;
            }
            if (!mqttClient.IsConnected)
            {
                errMsg = "MQTT客户端尚未连接！";
                return;
            }
            MqttApplicationMessage message = new MqttApplicationMessage();
            message.Topic = data.topic;
            message.Payload = data.palyLoad;
            message.QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce;
            message.Retain = false;
            Task task = mqttClient.PublishAsync(message);
            task.Wait(5000);// 最多只等待5秒
        }

    }
}
