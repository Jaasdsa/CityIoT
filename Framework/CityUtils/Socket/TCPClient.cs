using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CityUtils
{
    public class TCPClient
    {
        /// <summary>
        /// 当前客户端名称
        /// </summary>
        private string _Name = "未定义";
        public string Name
        {
            get
            {
                return _Name;
            }
        }

        public void SetName()
        {
            if (_NetWork.Connected)
            {
                IPEndPoint iepR = (IPEndPoint)_NetWork.Client.RemoteEndPoint;
                IPEndPoint iepL = (IPEndPoint)_NetWork.Client.LocalEndPoint;
                _Name = iepL.Port + "->" + iepR.ToString();
            }
        }

        /// <summary>
        /// TCP客户端
        /// </summary>
        private TcpClient _NetWork = null;
        public TcpClient NetWork
        {
            get
            {
                return _NetWork;
            }
            set
            {
                _NetWork = value;
                SetName();
            }
        }

        /// <summary>
        /// 数据接收缓存区
        /// </summary>
        public byte[] buffer = new byte[2048];

        /// <summary>
        /// 断开客户端连接
        /// </summary>
        public bool DisConnect(out string errMsg)
        {
            errMsg = "";
            try
            {
                if (_NetWork != null && _NetWork.Connected)
                {
                    NetworkStream ns = _NetWork.GetStream();
                    ns.Close();
                    _NetWork.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }

        public bool Send( byte[] data, out string errMsg)
        {
            errMsg = "";
            try
            {
                this.NetWork.GetStream().Write(data, 0, data.Length);
                return true;
            }
            catch (Exception ex)
            {
                //  TraceManager.AppendErrMsg(client.Name + "发送失败:" + ex.Message;);
                errMsg = ex.Message;
                return false;
            }
        }
    }
}
