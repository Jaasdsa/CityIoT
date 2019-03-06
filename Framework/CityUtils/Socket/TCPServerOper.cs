using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CityUtils
{
    public class TCPServerOper : ICommunication
    {
        /// <summary>
        /// TCP服务端监听
        /// </summary>
        TcpListener tcpsever = null;
        /// <summary>
        /// 监听状态
        /// </summary>
        bool isListen = false;

        public event SocketEvent.DataReceivedHandler DataReceived;

        /// <summary>
        /// 当前已连接客户端集合
        /// </summary>
        private ConcurrentDictionary<string, TCPClient> dicClients;

        private int port;
        private string ip;

        public TCPServerOper(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        // TCP 客户端增删改查
        private void AddTcpClient(TCPClient client)
        {
            this.dicClients.TryAdd(client.Name, client);
        }
        public void DeleteTcpClient(string clientName)
        {
            if (this.dicClients.TryRemove(clientName, out TCPClient client))
                client.DisConnect(out string errMes);
        }
        public TCPClient QueryTcpClient(string clientName)
        {
            if (this.dicClients.TryGetValue(clientName, out TCPClient client))
                return client;
            else
                return null;
        }
        private bool ContainTcpClient(string clientName)
        {
            return this.dicClients.TryGetValue(clientName, out TCPClient client);
        }

        /// <summary>
        /// 开启TCP监听
        /// </summary>
        /// <returns></returns>
        public bool Start(out string errMsg)
        {
            errMsg = "";
            dicClients = new ConcurrentDictionary<string, TCPClient>();
            try
            {
                tcpsever = new TcpListener(IPAddress.Parse(ip.Trim()), this.port);
                tcpsever.Start();
                tcpsever.BeginAcceptTcpClient(new AsyncCallback(Acceptor), tcpsever);
                isListen = true;
                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 停止TCP监听
        /// </summary>
        /// <returns></returns>
        public void Stop()
        {
            if (!isListen)
                return;
            ClearSelf();
            dicClients = null;
            isListen = false;
        }

        /// <summary>
        /// 客户端连接初始化
        /// </summary>
        /// <param name="o"></param>
        private void Acceptor(IAsyncResult o)
        {
            TcpListener server = o.AsyncState as TcpListener;
            try
            {
                //初始化连接的客户端
                TCPClient newClient = new TCPClient();
                newClient.NetWork = server.EndAcceptTcpClient(o);
                AddTcpClient(newClient);//添加缓存字典
                newClient.NetWork.GetStream().BeginRead(newClient.buffer, 0, newClient.buffer.Length, new AsyncCallback(TCPCallBack), newClient);
                server.BeginAcceptTcpClient(new AsyncCallback(Acceptor), server);//继续监听客户端连接
            }
            catch (ObjectDisposedException ex)
            { //监听被关闭
            }
            catch (Exception ex)
            {
               /// TraceManager.AppendErrMsg(ex.Message);
            }
        }

        /// <summary>
        /// 对当前选中的客户端发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public bool SendData(string clientName, byte[] data, out string errMsg)
        {
            errMsg = "";
            if (data == null)
            {
                errMsg = "发送数据对象为空对象，发送失败";
                return false;
            }
            if (string.IsNullOrWhiteSpace(clientName))
            {
                errMsg = "发送的客户端对象为空对象，发送失败";
                return false;
            }
            TCPClient client = QueryTcpClient(clientName);
            if (client == null)
            {
                errMsg = "发送的客户端对象为空对象，发送失败";
                return false;
            }

          return  client.Send(data, out errMsg);
        }

        /// <summary>
        /// 客户端通讯回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void TCPCallBack(IAsyncResult ar)
        {
            TCPClient client = (TCPClient)ar.AsyncState;
            if (client.NetWork.Connected)
            {
                try
                {
                    NetworkStream ns = client.NetWork.GetStream();
                    byte[] recdata = new byte[ns.EndRead(ar)];
                    if (recdata.Length > 0)
                    {
                        Array.Copy(client.buffer, recdata, recdata.Length);
                        if (DataReceived != null)
                        {
                            DataReceived.BeginInvoke(client.Name, recdata, null, null);//异步输出数据
                        }
                        ns.BeginRead(client.buffer, 0, client.buffer.Length, new AsyncCallback(TCPCallBack), client);
                    }
                    else
                        DeleteTcpClient(client.Name);
                }
                catch (Exception e)
                {
                    try { DeleteTcpClient(client.Name); } catch { }
                }
            }
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void ClearSelf()
        {
            foreach (string clientName in dicClients.Keys)
            {
                DeleteTcpClient(clientName);
            }
            if (tcpsever != null)
            {
                tcpsever.Stop();
            }
        }

    }
}
