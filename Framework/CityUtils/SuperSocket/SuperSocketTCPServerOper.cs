using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityUtils
{
    public class ReciveData
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        public string sessionID; 
        /// <summary>
        /// 接受的正文
        /// </summary>
        public SuperSocketStringData data;
        /// <summary>
        /// 任务结束的回调,string sessionID, string data,bool isCloseSession,string info
        /// </summary>
        public Func<string, string,bool, string> FinshCallBack;
    }

    public class SuperSocketStringData
    {
        public string key;
        public string body;
        public string[] parameters;
    }

    public class SuperSocketTcpServerOper
    {
        // 开源框架superSocket应用封装工具类--TCP，UTF-8编码

        int port;
        string ip;
        AppServer appServer;

        public SuperSocketTcpServerOper(string ip, int port)
        {
            this.ip = ip;
            this.port = port; 
        }

        public event Action<ReciveData> evtReciveHandle;

        /// <summary>
        /// 开启TCP监听
        /// </summary>
        /// <returns></returns>
        public bool Start(out string errMsg)
        {
            errMsg = "";
            appServer = new AppServer();
            ServerConfig serverConfig = new ServerConfig() {
                Ip = ip,
                Port = port,
                TextEncoding="UTF-8",
                Mode=SocketMode.Tcp,
                MaxConnectionNumber=500,
                ClearIdleSession=true,
                ClearIdleSessionInterval=30,
                IdleSessionTimeOut=60,
            };
            // 检查端口是否空闲
            if (!IPTool.IsValidPort(serverConfig.Port))
            {
                errMsg = "命令服务器的监听端口被占用，请更换端口号";
                return false;
            }
            if (!appServer.Setup(serverConfig)) // 设置监听的IP和端口,不设置传输层协议，采用框架默认的TCP协议
            {
                errMsg = "superSocket设置IP和端口失败";
                return false;
            }
            if (!appServer.Start())
            {
                errMsg = "superSocket启动失败";
                return false;
            }
            // 绑定事件
            appServer.NewRequestReceived += new RequestHandler<AppSession, StringRequestInfo>(appServer_NewRequestReceived);
            appServer.NewSessionConnected += new SessionHandler<AppSession>(appServer_NewSessionConnected);
            appServer.SessionClosed += new SessionHandler<AppSession, CloseReason>(appServer_SessionClosed);
            return true;
        }

        /// <summary>
        /// 停止TCP监听
        /// </summary>
        /// <returns></returns>
        public void Stop()
        {
            appServer.Stop();
        }

        /// <summary>
        /// 新会话连接
        /// </summary>
        /// <param name="session"></param>
        private void appServer_NewSessionConnected(AppSession session)
        {

        }
        /// <summary>
        /// 会话断开
        /// </summary>
        /// <param name="session"></param>
        /// <param name="closeReason"></param>
        private void appServer_SessionClosed(AppSession session, CloseReason closeReason)
        {

        }
        
        private void appServer_NewRequestReceived(AppSession session, StringRequestInfo requestInfo)
        {
            if (evtReciveHandle == null)
                return;

            ReciveData data = new ReciveData()
            {
                sessionID = session.SessionID,
                data = new SuperSocketStringData()
                {
                    key = requestInfo.Key,
                    body = requestInfo.Body,
                    parameters = requestInfo.Parameters
                },
                FinshCallBack= SendResponse
            };

            evtReciveHandle(data);
        }
        /// <summary>
        /// 给客户端发送消息
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string SendResponse(string sessionID, string data,bool isCloseSession)
        {
            AppSession session = this.appServer.GetSessionByID(sessionID);
            if (session == null)
                return "未查询到会话ID:" + sessionID;
            if (!session.Connected)
                return "会话ID:" + sessionID + "已经关闭无法发送回执响应消息";
            if (!session.TrySend(data))
                return "会话ID:" + sessionID + "发送回执消息失败";
            // 发送完断开连接
            if (isCloseSession)
                session.Close(CloseReason.ServerShutdown);
            return "";
        }
    }
}
