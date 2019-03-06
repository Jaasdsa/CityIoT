using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace CityUtils
{
    /// <summary>
    /// TCP客户端使用说明：
    /// 先实例化，注册接受处理的事件handler，在开始连接，handler处理，断开连接。
    /// 叶飞-2018-12-10
    /// </summary>
    public class SuperSocketTCPClientOper : ICommunication
    {
        /// <summary>
        /// 当前已连接客户端集合
        /// </summary>
        private ConcurrentDictionary<string, TCPClient> dicClients;

        public event SocketEvent.DataReceivedHandler DataReceived;

        private string ip;
        private int port;

        public SuperSocketTCPClientOper(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        // TCP 客户端增删改查
        private void AddTcpClient(TCPClient client)
        {
            this.dicClients.TryAdd(client.Name, client);
        }
        private void DeleteTcpClient(string clientName)
        {
            if (this.dicClients.TryRemove(clientName, out TCPClient client))
            {
                try
                {
                    client.DisConnect(out string errMes);
                }
                catch { }
            }


        }
        private TCPClient QueryTcpClient(string clientName)
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
        /// 连接新的服务端
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool Connect(out string clientName, out string errMsg)
        {
            errMsg = "";
            clientName = "";
            dicClients = new ConcurrentDictionary<string, TCPClient>();
            TCPClient client = new TCPClient();
            try
            {
                client.NetWork = new TcpClient();
                client.NetWork.Connect(this.ip.Trim(), this.port);//连接服务端
                client.SetName();
                client.NetWork.GetStream().BeginRead(client.buffer, 0, client.buffer.Length, new AsyncCallback(TCPCallBack), client);
                this.dicClients.TryAdd(client.Name, client);
                clientName = client.Name;
                return true;
            }
            catch (Exception ex)
            {
                client.DisConnect(out string ee);
                errMsg = ex.Message;
                return false;
            }
        }
        public void DisConnect()
        {
            ClearSelf();
            this.dicClients = null;
        }

        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void TCPCallBack(IAsyncResult ar)
        {
            TCPClient client = (TCPClient)ar.AsyncState;
            if (client.NetWork.Connected)
            {
                NetworkStream ns = client.NetWork.GetStream();
                try
                {
                    byte[] recdata = new byte[ns.EndRead(ar)];
                    Array.Copy(client.buffer, recdata, recdata.Length);
                    if (recdata.Length > 0)
                    {
                        if (DataReceived != null)
                        {
                            DataReceived.BeginInvoke(client.Name, recdata, null, null);//异步输出数据
                        }
                        ns.BeginRead(client.buffer, 0, client.buffer.Length, new AsyncCallback(TCPCallBack), client);
                    }
                    else
                        DeleteTcpClient(client.Name);
                }
                catch (Exception ex)
                {
                    DeleteTcpClient(client.Name);
                }
            }
        }
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
            try
            {
                client.NetWork.GetStream().Write(data, 0, data.Length);
                return true;
            }
            catch (Exception ex)
            {
                errMsg = client.Name + "发送失败:" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 关闭所有连接
        /// </summary>
        public void ClearSelf()
        {
            foreach (string clientName in dicClients.Keys)
            {
                DeleteTcpClient(clientName);
            }
        }
    }
}
