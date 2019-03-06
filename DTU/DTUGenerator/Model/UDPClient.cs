using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace DTUGenerator
{
    public class UDPClient
    {
        /// <summary>
        /// 名称
        /// </summary>
        private string _Name = "未定义";
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        IPEndPoint _ipLocalEndPoint;
        /// <summary>
        /// 本地终结点
        /// </summary>
        public IPEndPoint ipLocalEndPoint
        {
            get
            {
                return _ipLocalEndPoint;
            }
            set
            {
                _ipLocalEndPoint = value;
            }
        }

        /// <summary>
        /// UDP 客户端
        /// </summary>
        private UdpClient _NetWork = null;
        public UdpClient NetWork
        {
            get
            {
                return _NetWork;
            }
            set
            {
                _NetWork = value;
            }
        }

        /// <summary>
        /// 断开客户端连接
        /// </summary>
        public void Close()
        {
            try
            {
                if (_NetWork != null && _NetWork.Client.Connected)
                {
                    _NetWork.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
