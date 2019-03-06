using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace DTUGenerator
{
    public class UDPClientManager:ICommunication
    {
        BindingList<UDPClient> lstClient = new BindingList<UDPClient>();

        public event DataReceivedHandler DataReceived;

        public event Action<string> evtError;

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        public string  GetLocalHostStr()
        {
            IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in ipHostEntry.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {//筛选IPV4
                   return ip.ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// 绑定客户端列表
        /// </summary>
        public void BindLstClient(ListBox lstConn)
        {
            lstConn.Invoke(new MethodInvoker(delegate
            {
                lstConn.DataSource = null;
                lstConn.DataSource = lstClient;
                lstConn.DisplayMember = "Name";
            }));
        }

        /// <summary>
        /// 移除客户端
        /// </summary>
        /// <param name="WaitRemove"></param>
        public void RemoveClient(List<UDPClient> WaitRemove)
        {
            foreach (UDPClient client in WaitRemove)
            {
                client.NetWork.Close();
                lstClient.Remove(client);
            }
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        public UDPClient ConnServer(string hostName, int port)
        {
            try
            {
                UDPClient client = new UDPClient();
                client.NetWork = new UdpClient();
                client.NetWork.Connect(hostName.Trim(), port);
                client.ipLocalEndPoint = (IPEndPoint)client.NetWork.Client.LocalEndPoint;
                client.Name = client.ipLocalEndPoint.Port + "->" + client.NetWork.Client.RemoteEndPoint.ToString();
                client.NetWork.BeginReceive(new AsyncCallback(ReceiveCallback), client);//继续异步接收数据
                lstClient.Add(client);
                return client;
            }
            catch (Exception ex)
            {
                TriggerErrorEvent(ex.Message);
               // MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// 接收到数据
        /// </summary>
        /// <param name="ar"></param>
        public void ReceiveCallback(IAsyncResult ar)
        {
            UDPClient uclient = (UDPClient)ar.AsyncState;
            try
            {
                if (uclient.NetWork.Client != null && uclient.NetWork.Client.Connected)
                {
                    IPEndPoint fclient = uclient.ipLocalEndPoint;
                    Byte[] recdata = uclient.NetWork.EndReceive(ar, ref fclient);
                    string ConnName = uclient.ipLocalEndPoint.Port + "->" + fclient.ToString();
                    if (DataReceived != null)
                    {
                        DataReceived.BeginInvoke(ConnName, recdata, null, null);//异步输出数据
                    }
                    uclient.NetWork.BeginReceive(new AsyncCallback(ReceiveCallback), uclient);//继续异步接收数据
                }
            }
            catch (Exception ex)
            {
                TriggerErrorEvent(ex.Message);
              //  MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SendData(UDPClient client,byte[] data,out string errMsg)
        {
            errMsg = "";
            try
            {
                client.NetWork.Send(data, data.Length);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
            return true;
        }
        public bool SendData(byte[] data, out string errMsg)
        {
            errMsg = "";
            if (lstClient.Count == 1)
            {
                try
                {
                    UDPClient client = lstClient.ToArray()[0];
                    client.NetWork.Send(data, data.Length);
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                    return false;
                }
            }
            else
            {
                errMsg = "客户端列表不唯一";
                return false;
            }
            return true;
        }


        /// <summary>
        /// 清理
        /// </summary>
        public void ClearSelf()
        {
            foreach (UDPClient client in lstClient)
            {
                client.Close();
            }
            lstClient.Clear();
        }

        /// <summary>
        /// 触发错误事件
        /// </summary>
        /// <param name="errMsg"></param>
        private void TriggerErrorEvent(string errMsg)
        {
            if (this.evtError == null)
                return;
            this.evtError(errMsg);
        }
    }
}
