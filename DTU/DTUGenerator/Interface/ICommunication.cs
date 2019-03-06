using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTUGenerator
{
    /// <summary>
    /// 通讯调试接口
    /// </summary>
    public interface ICommunication
    {
        /// <summary>
        /// 事件：接收数据
        /// </summary>
        event DataReceivedHandler DataReceived;

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        bool SendData(byte[] data,out string errMsg);

        /// <summary>
        /// 清理通讯资源
        /// </summary>
        void ClearSelf();
    }
}
