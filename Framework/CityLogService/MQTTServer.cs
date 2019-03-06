using CityUtils;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CityLogService
{
    class MQTTParam
    {
        public string ip;// 日志发布启用的本地IP地址
        public int port;// 日志发布服务启用的本地端口
        public string userName; // 日志发布服务的用户名
        public string password; // 日志发布服务的密码
    }

    class PublishData
    {
        /// <summary>
        /// 发布的数据
        /// </summary>
        public object data;
        /// <summary>
        /// 发布数据的名称
        /// </summary>
        public string topic;
        /// <summary>
        /// 发布数据的有效载荷，UTF8编码转换流
        /// </summary>
        public byte[] Payload;

        /// <summary>
        /// 判断发布的数据有效性
        /// </summary>
        /// <returns></returns>
        public bool ToPayLoad()
        {
            if (string.IsNullOrWhiteSpace(topic))
                return false;
            if (data == null)
                return false;
            this.Payload = ByteUtil.ToSerializeBuffer<object>(this.data);
            return true;
        }
    }

    /// <summary>
    /// MQTT 服务器
    /// </summary>
    class MQTTServer
    {
        private MQTTParam mqttParam; //发布服务参数
        private MqttServer mqttServer = null;
        private List<string> subClientIDs = new List<string>();
        public bool IsRuning { get; set; }

        public MQTTServer(MQTTParam mqttParam)
        {
            this.mqttParam = mqttParam;
        }

        public void Start(out string errMsg)
        {
            errMsg = "";
            if (IsRuning)
                return;

            if (this.mqttParam == null)
            {
                errMsg = "MQTT服务启动失败,没有指明服务所需参数";
                return;
            }
            if (mqttServer == null)
            {
                try
                {
                    mqttServer.StopAsync();
                    mqttServer = null;
                }
                catch { }
            }

            // MQTT 动态库本身已经实现异步启动，这里不用在用异步调用了
            var optionsBuilder = new MqttServerOptionsBuilder();
            try
            {
                //在 MqttServerOptions 选项中，你可以使用 ConnectionValidator 来对客户端连接进行验证。
                //比如客户端ID标识 ClientId，用户名 Username 和密码 Password 等。
                optionsBuilder.WithConnectionValidator(ClientCheck);
                //指定 ip地址，默认为本地
                optionsBuilder.WithDefaultEndpointBoundIPAddress(IPAddress.Parse(this.mqttParam.ip));
                //指定端口
                optionsBuilder.WithDefaultEndpointPort(this.mqttParam.port);
                //连接记录数，默认 一般为2000
                //optionsBuilder.WithConnectionBacklog(2000);
                mqttServer = new MqttFactory().CreateMqttServer() as MqttServer;

                // 客户端支持 Connected、Disconnected 和 ApplicationMessageReceived 事件，
                //用来处理客户端与服务端连接、客户端从服务端断开以及客户端收到消息的事情。
                //其中 ClientConnected 和 ClientDisconnected 事件的事件参数一个客户端连接对象 ConnectedMqttClient，
                //通过该对象可以获取客户端ID标识 ClientId 和 MQTT 版本 ProtocolVersion。
                mqttServer.ClientConnected += MqttServer_ClientConnected;
                mqttServer.ClientDisconnected += MqttServer_ClientDisconnected;

                //ApplicationMessageReceived 的事件参数包含了客户端ID标识 ClientId 和 MQTT 应用消息 MqttApplicationMessage 对象，
                //通过该对象可以获取主题 Topic、QoS QualityOfServiceLevel 和消息内容 Payload 等信息。
                mqttServer.ApplicationMessageReceived += MqttServer_ApplicationMessageReceived;
            }
            catch (Exception ex)
            {
                // log.Info("创建Mqtt服务,连接客户端的Id长度过短（不得小于5）,或不是指定的合法客户端（以Eohi_开头）");
                errMsg = "创建MQTT服务失败:" + ex.Message;
                return;
            }

            Task task = mqttServer.StartAsync(optionsBuilder.Build());
            task.Wait(5000);

            IsRuning = task.IsCompleted;
        }
        public void Stop()
        {
            if (mqttServer != null)
            {
                try
                {
                    Task task = mqttServer.StopAsync();
                    task.Wait(5000);
                    mqttServer = null;
                }
                catch { }
            }

            IsRuning = false;
        }

        /// <summary>
        /// 验证客户端
        /// </summary>
        /// <param name="context"></param>
        private void ClientCheck(MqttConnectionValidatorContext context)
        {
            //string clientIdStartsWith = "";
            // int clientIdMinLegth = 0;

            //if (context.ClientId.Length < clientIdMinLegth || !context.ClientId.StartsWith(clientIdStartsWith))
            //{
            //    context.ReturnCode = MqttConnectReturnCode.ConnectionRefusedIdentifierRejected;
            //    return;
            //}

            if (context.Username != this.mqttParam.userName || MD5Util.MD5Encrypt(context.Password) != this.mqttParam.password)
            {
                context.ReturnCode = MqttConnectReturnCode.ConnectionRefusedBadUsernameOrPassword;
                return;
            }
            context.ReturnCode = MqttConnectReturnCode.ConnectionAccepted;
        }
        /// <summary>
        /// 服务器接受到消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MqttServer_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            //  log.Info(DateTime.Now + $"客户端[{e.ClientId}]>> 主题：{e.ApplicationMessage.Topic} 负荷：{Encoding.UTF8.GetString(e.ApplicationMessage.Payload)} Qos：{e.ApplicationMessage.QualityOfServiceLevel} 保留：{e.ApplicationMessage.Retain}");
            var msg = @"发送消息的客户端id:" + e.ClientId + "\r\n"
                + "发送时间：" + DateTime.Now + "\r\n"
                + "发送消息的主题：" + e.ApplicationMessage.Topic + "\r\n"
               + "发送的消息内容：" + Encoding.UTF8.GetString(e.ApplicationMessage.Payload ?? new byte[0]) + "\r\n"
               + "--------------------------------------------------\r\n"
               ;
            // log.Info(msg);
        }
        /// <summary>
        /// 客户端断开连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MqttServer_ClientDisconnected(object sender, MqttClientDisconnectedEventArgs e)
        {
            if (subClientIDs.Contains(e.ClientId))
            {
                subClientIDs.Remove(e.ClientId);
            }
            // log.Info(DateTime.Now + $"客户端[{e.ClientId}]已断开连接！还有" + subClientIDs.Count + "客户端在连接！");
        }
        /// <summary>
        /// 客户端连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MqttServer_ClientConnected(object sender, MqttClientConnectedEventArgs e)
        {
            if (!subClientIDs.Contains(e.ClientId))
            {
                subClientIDs.Add(e.ClientId);
            }
            //   log.Info(DateTime.Now + $"客户端[{e.ClientId}]已连接，共有" + subClientIDs.Count + "客户端在连接！");
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <returns>异步发布的方法任务，不用打开可监听isComplete看完成状态</returns>
        public Task Publish(PublishData data)
        {
            MqttApplicationMessage message = new MqttApplicationMessage();
            message.Topic = data.topic;
            message.Payload = data.Payload;
            message.QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce;
            return mqttServer.PublishAsync(message);
        }

    }
}
