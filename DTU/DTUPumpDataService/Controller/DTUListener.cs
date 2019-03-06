using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DTUPumpDataService 
{
    class DTUListener
    {
        // 管理读取工作者，在扔给解析工作者
        public static bool IsRuning { get; set; } = false;
        public static void Start()
        {
            if (IsRuning)
                return;
            //先打开解析器，再打开读取器
            DTUResolverManager.Start();
            if (!DTUResolverManager.IsRuning)
            {
                Stop();
                TraceManager.AppendErrMsg("DTU解析器打开失败");
                return;
            }
            DTUReader.Start();
            if (!DTUReader.IsRuning)
            {
                Stop();
                TraceManager.AppendErrMsg("DTU读取器打开失败");
                return;
            }

            IsRuning = true;
        }
        public static void Stop()
        {
            if (!IsRuning)
                return;
            DTUReader.Stop();
            if (DTUReader.IsRuning)
            {
                TraceManager.AppendErrMsg("DTU读取器关闭失败");
            }
            DTUResolverManager.Stop();
            if (DTUResolverManager.IsRuning)
            {
                TraceManager.AppendErrMsg("DTU解析器关闭失败");
            }
            IsRuning = false;
        }
    }

    class DTUReader
    {
        // 读取工作者---从宏电动态库读取数据扔给解析器
        public static bool IsRuning { get; set; } = false;
        private static bool ReadingFlag { get; set; } = false;
        private static Task task;
        public static void Start()
        {
            if (IsRuning)
                return;
            ReadingFlag = true;
            task = new Task(() =>
            {
                try
                {
                    SpinWait spinWait = new SpinWait();
                    while (ReadingFlag)
                    {
                        ReadOnce();
                        spinWait.SpinOnce();
                    }
                }
                catch (Exception e)
                {
                    TraceManager.AppendErrMsg("DTU读取工作者异常--" + e.Message);
                }
            }, TaskCreationOptions.LongRunning);
            task.Start();

            IsRuning = true;
        }
        public static void Stop()
        {
            if (!IsRuning)
                return;

            // 停止while
            ReadingFlag = false;

            Task.WaitAll(task);
            task.Dispose();
            task = null;

            IsRuning = false;
        }
        private static void ReadOnce()
        {
            GPRS_DATA_RECORD dataRecord = new GPRS_DATA_RECORD();
            bool isNeedReply = false;
            StringBuilder mess = new StringBuilder(100);
            if (HDDTUAPI.do_read_proc(ref dataRecord, mess, isNeedReply) >= 0)
            {
                // 本服务以非阻塞模式运行, do_read_proc 无论有无消息都立即返回
                DTUResolverManager.Append(dataRecord);
            }
        }
    }

    class DTUResolverManager
    {
        // 解析器 管理器
        public static bool IsRuning = false;
        public static void Start()
        {
            if (IsRuning)
                return;

            DTUResolver1.Start();
            if (!DTUResolver1.IsRuning)
            {
                TraceManager.AppendErrMsg("DTU解析器1打开出错");
                Stop();
                return;
            }

            DTUResolver2.Start();
            if (!DTUResolver2.IsRuning)
            {
                TraceManager.AppendErrMsg("DTU解析器2打开出错");
                Stop();
                return;
            }

            DTUResolver3.Start();
            if (!DTUResolver3.IsRuning)
            {
                TraceManager.AppendErrMsg("DTU解析器3打开出错");
                Stop();
                return;
            }

            IsRuning = true;
        }

        public static void Stop()
        {
            if (!IsRuning)
                return;

            if (DTUResolver1.IsRuning)
            {
                DTUResolver1.Stop();
                if (DTUResolver1.IsRuning)
                {
                    TraceManager.AppendErrMsg("DTU解析器1关闭出错");
                }
            }
            if (DTUResolver2.IsRuning)
            {
                DTUResolver2.Stop();
                if (DTUResolver1.IsRuning)
                {
                    TraceManager.AppendErrMsg("DTU解析器2关闭出错");
                }
            }
            if (DTUResolver3.IsRuning)
            {
                DTUResolver3.Stop();
                if (DTUResolver3.IsRuning)
                {
                    TraceManager.AppendErrMsg("DTU解析器3关闭出错");
                }
            }

            IsRuning = false;
        }

        public static void Append(GPRS_DATA_RECORD data)
        {
            if (!IsRuning)
                return;
            if (DTUResolver1.TaskNumber < 4000)
            {
                DTUResolver1.Append(data);
            }
            else
            {
                if(DTUResolver2.TaskNumber < 4000)
                {
                    DTUResolver2.Append(data);
                }
                else
                {
                    if (DTUResolver3.TaskNumber < 4000)
                    {
                        DTUResolver3.Append(data);
                    }
                    else
                    {
                        TraceManager.AppendWarning("DTU解析队列三个消费者队列已达到上线无法插入");
                    }
                }
            }
        }
    }

    class DTUResolver1
    {
        // 解析器
        public static bool IsRuning = false;
        private static BlockingCollection<GPRS_DATA_RECORD> queue;
        private static Task task;

        public static int TaskNumber
        {
            get
            {
                return queue.Count;
            }
        }

        public static void Start()
        {
            if (IsRuning)
                return;

            queue = new BlockingCollection<GPRS_DATA_RECORD>();
            task = new Task(() =>
            {
                foreach (GPRS_DATA_RECORD item in queue.GetConsumingEnumerable())
                {
                    try
                    {
                        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                        watch.Start();
                        //开始监视代码运行时间
                        Excute(item);
                        //停止监视
                        watch.Stop();
                        TimeSpan timespan = watch.Elapsed;  //获取当前实例测量得出的总时间
                      //  MessageBox.Show(string.Format("打开窗口代码执行时间：{0}(毫秒)", timespan.TotalMilliseconds.ToString()));  //总毫秒数
                      //  Excute(item);
                    }
                    catch(Exception e)
                    {
                        TraceManager.AppendErrMsg("DTU解析器出错:" + e.Message);
                    }
                }

            }, TaskCreationOptions.LongRunning);
            task.Start();

            IsRuning = true;
        }
        public static void Stop()
        {
            if (!IsRuning)

                return;
            // 先完成添加
            queue.CompleteAdding();
            DateTime time1 = DateTime.Now;
            while (queue.Count > 0)
            {
                Thread.Sleep(1);
                // 最多等待10秒避免关不掉
                if (DateTime.Now-time1  > TimeSpan.FromSeconds(10))
                {
                    TraceManager.AppendErrMsg("DTU解析器关闭超时丢弃了" + queue.Count.ToString() + "条任务");
                    break;
                }
            }
            while (queue.Count > 0)
            {
                // 等了十秒还没听，队列全部元素废弃
                queue.Take();
            }

            queue = null;
            Task.WaitAll(task);
            task.Dispose();
            task = null;

            IsRuning = false;
        }
        private static void Excute(GPRS_DATA_RECORD item)
        {
            switch (item.m_data_type)
            {
                case 0x01:  // 注册包												
                    GPRS_USER_INFO userInfo = new GPRS_USER_INFO();
                    if (HDDTUAPI.get_user_info(item.m_userid, ref userInfo) == 0)
                    {
                        DTUInfo sourceDTU = DTUInfo.ToDTUInfoForSoc(userInfo);
                        if (sourceDTU == null)
                            return;
                        DTUInfo targetDTU = DTUCacheManager.GetDTUInfo(item.m_userid);
                        if (targetDTU == null)
                        {
                            // 首次注册--在DBWorker从数据读取业务信息添加到缓存
                            DBWorker.Append(DBCommand.CreateDtuRegister(sourceDTU));
                        }
                        else
                        {
                            // 更新链路
                            DTUCacheManager.OperDTUCache(DTUCacheManager.OPeratingType.UpdateSocketInfo,targetDTU, sourceDTU);
                        }
                    }
                    break;
                case 0x02:  // 注销包
                    {
                        DTUCacheManager.OperDTUCache(DTUCacheManager.OPeratingType.Delete, item.m_userid,null);
                    }
                    break;
                case 0x04:  // 无效包
                    TraceManager.AppendInfo(item.m_userid + "---接收无效的数据包");
                    break;
                case 0x05:  // DTU已经接收到DSC发送的用户数据包
                    TraceManager.AppendInfo(item.m_userid + "---已经接收到DSC发送的用户数据包");
                    break;
                case 0x09:  // 数据包
                    {
                        //TraceManager.AppendInfo("DTU:" + item.m_userid + "接收了" +
                        //                       ByteUtil.BytesToText(item.m_data_buf, item.m_data_len)
                        //                       + "--共计" + item.m_data_len.ToString() + "个字节");

                        DTUInfo dtu = DTUCacheManager.GetDTUInfo(item.m_userid);
                        if (dtu == null)
                            return;
                        dtu.Analysis(item.m_data_buf, item.m_data_len);
                    }
                    break;
                case 0x0d:
                    TraceManager.AppendInfo(item.m_userid + "---参数设置成功");
                    break;
                case 0x0b:
                    TraceManager.AppendInfo(item.m_userid + "---参数查询成功");
                    //  config.readconf();
                    break;
                case 0x06:
                    TraceManager.AppendInfo("---断开PPP连接成功");
                    break;
                case 0x07:
                    TraceManager.AppendInfo(item.m_userid + "---停止向DSC发送数据");
                    break;
                case 0x08:
                    TraceManager.AppendInfo("---允许向DSC发送数据");
                    break;
                case 0x0A:
                    TraceManager.AppendInfo("---丢弃用户数据");
                    break;
                default:
                    break;
            }
        }
        public static void Append(GPRS_DATA_RECORD data)
        {
            if (!IsRuning)
                return;
            if (queue.IsAddingCompleted)
                return;
            if (queue.Count > 4096)
            {
                TraceManager.AppendErrMsg("DTU数据解析队列到达上限无法插入");
                return;
            }   
            queue.Add(data);
        }

    }

    class DTUResolver2: DTUResolver1
    {

    }

    class DTUResolver3 : DTUResolver1
    {

    }
}
