using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTUPumpDataService
{
    /// <summary>
    ///  宏电API维护服务管理
    /// </summary>
    class HDDTUService
    {
        // DTU 工作参数
        private static string ip;
        private static int port;
        private static string socketType;
        private static int serverMode = 1;
        // 超时参数
        private static int dtuTimeoutNum;
        private static int dtuListUpdateTime;
        private static System.Timers.Timer HDAPIMaintainTimer;

        // 开关口
        public static bool  IsRuning{get;set;}
        public static void Start(out string errMsg)
        {
            errMsg = "";

            if (IsRuning)
                return;

            LoadConfig();

            // 打开宏电官方库
            HDDTUAPI.SetCustomIP(HDDTUAPI.inet_addr(ip));
            HDDTUAPI.SetWorkMode(serverMode);//开发包函数，设置服务模式：2-消息，0-阻塞，1-非阻塞
            HDDTUAPI.SelectProtocol((socketType == "TCP") ? 1 : 0);//开发包函数，设置服务类型：0-UDP，1-TCP
            StringBuilder mess = new StringBuilder(1000);
            if (HDDTUAPI.start_net_service(IntPtr.Zero, 0, port, mess) != 0)    //开发包函数，非消息模式启动服务
            {
                errMsg = mess.ToString().Trim();
                Stop();
                return;
            }

            // 宏电API的状态维护
            StartMaintainHDAPI();
            if (!IsRuningMaintainHDAPI)
            {
                TraceManager.AppendErrMsg("HDAPI维护服务启动失败" + errMsg);
                Stop();
                return;
            }

            IsRuning =true;
        }
        public static void Stop()
        {
            // 宏电API的状态维护
            if (IsRuningMaintainHDAPI)
            {
                StopMaintainHDAPI();
                if (IsRuningMaintainHDAPI)
                    TraceManager.AppendErrMsg("HDAPI维护服务停止失败");
            }

            //宏电官方库
            try
            {
                StringBuilder mess = new StringBuilder(1000);
                int socketShutFlag = HDDTUAPI.stop_net_service(mess);
                if (socketShutFlag != 0)
                    TraceManager.AppendErrMsg("DTU通信服务关闭失败:" + mess);
            }
            catch { }

            IsRuning = false;
        }

        // dtu API维护服务
        private static bool IsRuningMaintainHDAPI { get; set; } = false;
        private static void StartMaintainHDAPI()
        {
            if (IsRuningMaintainHDAPI)
                return;

            //开始循环调用Dll拿所有DTU方法去更新所有的状态
            HDAPIMaintainTimer = new System.Timers.Timer();
            HDAPIMaintainTimer.Interval = dtuListUpdateTime * 1000;
            HDAPIMaintainTimer.Elapsed += (o, e) =>
            {
                try { HDAPIRefresh(); }
                catch { }
            };
            HDAPIMaintainTimer.Enabled = true;

            IsRuningMaintainHDAPI = true;
        }
        private static void StopMaintainHDAPI()
        {
            if (!IsRuningMaintainHDAPI)
                return;

            if (HDAPIMaintainTimer != null)
            {
                HDAPIMaintainTimer.Enabled = false;
                HDAPIMaintainTimer.Close();
                HDAPIMaintainTimer = null;
                IsRuningMaintainHDAPI = false;
            }
        }
        private static void HDAPIRefresh()
        {
            uint i, iDtuAmount;
            string str = "";

            StringBuilder mess = new StringBuilder(1000);
            GPRS_USER_INFO user_info = new GPRS_USER_INFO();

            str = str + 0x00 + 0x00 + 0x00;
            iDtuAmount = HDDTUAPI.get_max_user_amount();    // 取最大数量, 为 3000

            for (i = 0; i < iDtuAmount; i++)
            {
                HDDTUAPI.get_user_at(i, ref user_info);

                if (user_info.m_status == 1)    // 在线
                {
                    // 判断 DTU 最后注册时间与现在时间的差值是否超过设置的超时时间
                    if ((DateTime.Now - HDDTUAPI.ConvertToDateTime(user_info.m_update_time)) > TimeSpan.FromMinutes(dtuTimeoutNum))
                    {
                        string dtuID = user_info.m_userid;
                        // 若超时则认为该 DTU 不在线, 调用开发包函数使其下线
                        HDDTUAPI.do_close_one_user2(dtuID, mess);
                        // 从缓存队列移除
                        DTUInfo dtuCache =DTUCacheManager.GetDTUInfo(dtuID);
                        if (dtuCache != null)
                        {
                            DTUCacheManager.OperDTUCache(DTUCacheManager.OPeratingType.Delete, dtuID, null);//DTUCacheManager.RemoveDtuCache(dtuID);
                             // 下线状态回填数据库
                            DBWorker.Append(DBCommand.CreateDtuOffline(dtuID));
                        }   
                        else
                            TraceManager.AppendWarning("DTU:" + dtuID + "超时未注册,从缓存队列移除未发现该对象！");
                        continue;
                    }
                }
            }
        }

        // 读取服务配置文件
        private static void LoadConfig()
        {
            // 宏电参数
            ip = DTUDSCConfig.ip;
            port = DTUDSCConfig.port;
            socketType = DTUDSCConfig.socketTypt;

            // 业务时间
            dtuTimeoutNum = DTUDSCConfig.timeoutNum;
            dtuListUpdateTime = DTUDSCConfig.dtuListUpdateTime;
        }
    }
}
