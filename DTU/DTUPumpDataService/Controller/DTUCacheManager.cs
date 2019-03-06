using CityUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace DTUPumpDataService 
{
    class DTUCacheManager 
    {
        // DTU 缓存器---DTU缓存服务管理
        public enum OPeratingType
        {
            Add,
            UpdateSocketInfo,
            UpdateBSInfoMulti,
            Delete,
        }


        // 缓存字典对象
        private static int dtuListUpdateTime;
        private static ConcurrentDictionary<string, DTUInfo> dtusCache;
        private static System.Timers.Timer dtuCacheUpdateTimer;

        // 服务区
        public static bool IsRuning { get; set; } = false;
        public static void Start()
        {
            if (IsRuning)
                return;

            LoadConfig();

            // 数据库DTU缓存表状态清理
            if (!MakeAllDTUOffline(out string errMsg))
            {
                TraceManager.AppendErrMsg("DTU状态更新失败" + errMsg);
                Stop();
                return;
            }

            // 启用DTU缓存队列
            StartDTUCache();
            if (!IsRuningMaintainDTUCache)
            {
                TraceManager.AppendErrMsg("DTU数据同步服务启动失败" + errMsg);
                Stop();
                return;
            }

            // DTU 缓存器启动成功
            IsRuning = true;
        }
        public static void Stop()
        {
            if (!IsRuning)
                return;

            if (IsRuningMaintainDTUCache)
            {
                StopDTUCache();
                if (IsRuningMaintainDTUCache)
                    TraceManager.AppendErrMsg("DTU数据同步服务停止失败");
            }

            if (!MakeAllDTUOffline(out string errMsg))
            {
                TraceManager.AppendErrMsg("DTU状态更新失败" + errMsg);
                return;
            }

            IsRuning = false;
        }

        // 缓存到数据库服务开关
        private static bool IsRuningMaintainDTUCache { get; set; } = false;
        private static void StartDTUCache()
        {
            if (IsRuningMaintainDTUCache)
                return;
            dtusCache = new ConcurrentDictionary<string, DTUInfo>();
            dtuCacheUpdateTimer = new System.Timers.Timer();
            dtuCacheUpdateTimer.Interval = dtuListUpdateTime * 1000;
            dtuCacheUpdateTimer.Elapsed += (o, e) =>
            {
                try
                {
                    if (dtusCache == null)
                        return;
                    DBWorker.Append(DBCommand.CreateDtuCacheUpdate(GetDtuInfos()));
                }
                catch
                {
                    TraceManager.AppendErrMsg("DTU缓存队列同步数据库失败");
                }
            };
            dtuCacheUpdateTimer.Enabled = true;

            IsRuningMaintainDTUCache = true;

        }
        private static void StopDTUCache()
        {
            if (!IsRuningMaintainDTUCache)
                return;
            // 关闭定时器
            if (dtuCacheUpdateTimer != null)
            {
                dtuCacheUpdateTimer.Enabled = false;
                dtuCacheUpdateTimer.Close();
                dtuCacheUpdateTimer = null;
                IsRuningMaintainDTUCache = false;
            }
            // 销毁队列
            dtusCache = null;
        }

        // 增删改调度及事件触发
        public static bool OperDTUCache(OPeratingType type, object target, DTUInfo souceDTU)
        {
            try
            {
                switch (type)
                {
                    case OPeratingType.Add:
                        {
                            DTUInfo dtu = target as DTUInfo;
                            Add(dtu);
                            TriggerAdd(dtu);
                            TraceManager.AppendInfo(dtu.ID + "上线");
                            return true;
                        }
                    case OPeratingType.UpdateSocketInfo:
                        {
                            DTUInfo targetDTU = target as DTUInfo;
                            UpdateSocketInfo(targetDTU, souceDTU);
                            TriggerUpdateSocInfo(targetDTU);
                            return true;
                        }
                    case OPeratingType.UpdateBSInfoMulti:
                        {
                            DTUInfo[] dtus = target as DTUInfo[];
                            if (UpdateBSInfo(dtus))
                            {
                                TriggerUpdateMulti(dtus);
                                return true;
                            }
                            else
                            {
                                TraceManager.AppendErrMsg("DTU缓存更新业务对象失败");
                                return false;
                            }
                        }
                    case OPeratingType.Delete:
                        {
                            string dtuID = target as String;
                            if (RemoveDtuCache(dtuID))
                            {
                                TriggerDelOne(dtuID);
                                TraceManager.AppendInfo(dtuID + "下线");
                                return true;
                            }
                            else
                            {
                                TraceManager.AppendErrMsg("DTU缓存移除对象失败");
                                return false;
                            }
                        }

                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                TraceManager.AppendErrMsg("DTU缓存字典操作出错:" + e.Message);
                return false;
            }
        }
        // 添加
        private static void Add(DTUInfo dtu)
        {
            dtusCache.TryAdd(dtu.ID, dtu);
        }
        // 更新
        private static void UpdateSocketInfo(DTUInfo targetDTU, DTUInfo sourceDTU)
        {
            // 更新链路信息
            if (sourceDTU == null)
                return;

            targetDTU.IsUsed = sourceDTU.IsUsed;
            targetDTU.IsActive = sourceDTU.IsActive;
            targetDTU.ActiveTime = sourceDTU.ActiveTime;
            targetDTU.LastRegisterTime = sourceDTU.LastRegisterTime;
            targetDTU.TerminalIP = sourceDTU.TerminalIP;
            targetDTU.TerminalPort = sourceDTU.TerminalPort;
            targetDTU.GatewayIP = sourceDTU.GatewayIP;
            targetDTU.TerminalPort = sourceDTU.GatewayPort;
        }
        private static bool UpdateBSInfo(DTUInfo[] dtus)
        {
            // 更新dtu缓存中的业务信息部分，链路部分不更新
            if (dtus == null)
                return false;
            foreach (DTUInfo dtu in dtus)
            {
                DTUInfo targetDTU = GetDTUInfo(dtu.ID);
                if (targetDTU == null)
                    continue;
                targetDTU.DBID = dtu.DBID;
                targetDTU.Name = dtu.Name;
                targetDTU.Description = dtu.Description;
                targetDTU.FactoryName = dtu.FactoryName;
                targetDTU.Model = dtu.Model;
                targetDTU.WorkType = dtu.WorkType;
                targetDTU.Protocol = dtu.Protocol;
            }
            return true;
        }
        // 添加、更新
        private static void AddOrUpdate(DTUInfo dtuInfo)
        {
            // 需要做事件，放弃使用，尽管好用
            dtusCache.AddOrUpdate(dtuInfo.ID, dtuInfo, (dtuID, exists) =>
            {
                return dtuInfo;
            });

        }
        // 删除
        public static bool RemoveDtuCache(string dtuID)
        {
            bool r = dtusCache.TryRemove(dtuID, out DTUInfo dtuInfo);
            if (r)
                TraceManager.AppendInfo(dtuID + "注销下线");
            return r;
        }
        // 查询
        public static DTUInfo[] GetDtuInfos()
        {
            // 按最后注册时间降序
            return dtusCache.Values.ToArray().OrderByDescending(d => d.LastRegisterTime).ToArray();
        }
        public static DTUInfo GetDTUInfo(string dtuID)
        {
            //int retryTimes = 0;
            //bool IsGeted = false;
            //retryCheck: IsGeted = dtusCache.TryGetValue(dtuID, out DTUInfo dtuInfo);
            //if (!IsGeted)
            //{
            //    if (retryTimes < 3)
            //    {
            //        Thread.Sleep(10);
            //        retryTimes += 1;
            //        goto retryCheck;
            //    }
            //    return null;
            //}
            //return dtuInfo;
            dtusCache.TryGetValue(dtuID, out DTUInfo dtuInfo);
            return dtuInfo;
        }

        // 事件触发器
        public static bool eventTriggerFlag { get; set; } = true;// DTU列表更新事件标志默认开启
        public static event Action<DTUInfo> evtAdd;
        private static void TriggerAdd(DTUInfo dtu)
        {
            if (!eventTriggerFlag)
                return;
            if (evtAdd == null)
                return;
            evtAdd(dtu);
        }
        public static event Action<DTUInfo> evtUpdateSocInfo;
        private static void TriggerUpdateSocInfo(DTUInfo dtu)
        {
            if (!eventTriggerFlag)
                return;
            if (evtUpdateSocInfo == null)
                return;
            evtUpdateSocInfo(dtu);
        }
        public static event Action<DTUInfo[]> evtUpdateMulti;
        private static void TriggerUpdateMulti(DTUInfo[] dtus)
        {
            if (!eventTriggerFlag)
                return;
            if (evtUpdateMulti == null)
                return;
            evtUpdateMulti(dtus);
        }
        public static event Action<string> evtDelOne;
        private static void TriggerDelOne(string dtuID)
        {
            if (!eventTriggerFlag)
                return;
            if (evtDelOne == null)
                return;
            evtDelOne(dtuID);
        }

        // 工具
        public static DTUInfo CreateDtuInfo(GPRS_USER_INFO gprsUserInfo)
        {
            try
            {
                DTUInfo dtuInfo = new DTUInfo();
                dtuInfo.ID = gprsUserInfo.m_userid;
                dtuInfo.WorkType = DTUDSCConfig.socketTypt;
                dtuInfo.IsUsed = 1;
                dtuInfo.IsActive = 1;
                dtuInfo.ActiveTime = DataUtil.ToDateString(gprsUserInfo.m_logon_date);
                dtuInfo.LastRegisterTime = DataUtil.ToDateString(HDDTUAPI.ConvertToDateString(gprsUserInfo.m_update_time));
                dtuInfo.TerminalIP = HDDTUAPI.inet_ntoa(HDDTUAPI.ntohl(gprsUserInfo.m_sin_addr));
                dtuInfo.TerminalPort = gprsUserInfo.m_sin_port;
                dtuInfo.GatewayIP = HDDTUAPI.inet_ntoa(HDDTUAPI.ntohl(gprsUserInfo.m_local_addr));
                dtuInfo.GatewayPort = gprsUserInfo.m_local_port;

                return dtuInfo;
            }
            catch
            {

            }
            return null;

        }
        private static bool MakeAllDTUOffline(out string errMsg)
        {
            string sql;
            sql = @"UPDATE DTUBase SET 是否在线 = 0 ";
            DBUtil.ExecuteNonQuery(sql, out errMsg);//让所有DTU状态归零，用注册方法更新其状态
            if (!string.IsNullOrEmpty(errMsg))
            {
                errMsg = "所有DTU下线维护失败" + errMsg;
                return false;
            }
            return true;
        }

        // 读取服务配置文件
        private static void LoadConfig()
        {
            dtuListUpdateTime = DTUDSCConfig.dtuListUpdateTime;
        }
    }

}
