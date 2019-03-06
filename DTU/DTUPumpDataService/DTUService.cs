using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTUPumpDataService
{
    public class DTUService : ISonService, IServiceWorker
    {
        // 构造传递配置文件路径
        public DTUService(string configFilePath)
        {
            Config.configFilePath = configFilePath;
        }
        public DTUService()
        {
            Config.configFilePath = XMLHelper.CurSystemPath + "config.xml";
        }

        // 开关口
        public bool IsRuning { get; set; } = false;
        public void Start(out string errMsg)
        {
            errMsg = "";

            if (IsRuning)
                return;

            try
            {
                TraceManager.Start(TriggerTrace);

                // 环境检查
                if (!EnvChecker.Check(out errMsg))
                    return;
                TraceManager.AppendDebug("环境检查通过");


                //数据库工作服务打开
                if (DBWorker.IsRuning)
                    DBWorker.Stop();
                DBWorker.Start();
                if (DBWorker.IsRuning)
                    TraceManager.AppendDebug("数据库工作器已经打开");
                else
                {
                    errMsg = "数据库工作器打开失败";
                    TraceManager.AppendErrMsg(errMsg);
                    Stop();
                    return;
                }


                // 打开DTU缓存管理器
                if (DTUCacheManager.IsRuning)
                    DTUCacheManager.Stop();
                DTUCacheManager.Start();
                if (DTUCacheManager.IsRuning)
                    TraceManager.AppendDebug("DTU缓存管理器已经打开");
                else
                {
                    errMsg = "DTU缓存管理器打开失败";
                    TraceManager.AppendErrMsg(errMsg);
                    Stop();
                    return;
                }

                // DTU通信器
                if (HDDTUService.IsRuning)
                    HDDTUService.Stop();
                HDDTUService.Start(out string err);
                if (HDDTUService.IsRuning)
                    TraceManager.AppendDebug("DTU监听器已经打开");
                else
                {
                    errMsg = "DTU控制器打开失败:" + err;
                    TraceManager.AppendErrMsg(errMsg);
                    Stop();
                    return;
                }

                // DTU控制器
                if (DTUController.IsRuning)
                    DTUController.Stop();
                DTUController.Start();
                if (DTUController.IsRuning)
                    TraceManager.AppendDebug("DTU控制器已经打开");
                else
                {
                    errMsg = "DTU控制器打开失败";
                    TraceManager.AppendErrMsg(errMsg);
                    Stop();
                    return;
                }

                // DTU监听器
                if (DTUListener.IsRuning)
                    DTUListener.Stop();
                DTUListener.Start();
                if (DTUListener.IsRuning)
                    TraceManager.AppendDebug("DTU监听器已经打开");
                else
                {
                    errMsg = "DTU监听器打开失败";
                    TraceManager.AppendErrMsg(errMsg);
                    Stop();
                    return;
                }
            }
            catch(Exception e)
            {
                errMsg = e.Message;
                Stop();
                return;
            }
            IsRuning = true;
        }
        public void Stop()
        {
            try
            {
                // DTU监听器
                if (DTUListener.IsRuning)
                {
                    DTUListener.Stop();
                    if (!DTUListener.IsRuning)
                        TraceManager.AppendDebug("DTU监听器已经关闭");
                    else
                        TraceManager.AppendErrMsg("DTU监听器关闭失败");
                }

                // DTU控制器
                if (DTUController.IsRuning)
                {
                    DTUController.Stop();
                    if (!DTUController.IsRuning)
                        TraceManager.AppendDebug("DTU控制器已经关闭");
                    else
                        TraceManager.AppendErrMsg("DTU控制器关闭失败");
                }

                // DTU通信器
                if (HDDTUService.IsRuning)
                {
                    HDDTUService.Stop();
                    if (!HDDTUService.IsRuning)
                        TraceManager.AppendDebug("DTU通信器已经关闭");
                    else
                        TraceManager.AppendErrMsg("DTU控制器关闭失败");
                }

                // DTU缓存管理器
                if (DTUCacheManager.IsRuning)
                {
                    DTUCacheManager.Stop();
                    if (!DTUCacheManager.IsRuning)
                        TraceManager.AppendDebug("DTU缓存管理器已经关闭");
                    else
                        TraceManager.AppendErrMsg("DTU缓存管理器关闭失败");
                }

                //数据库工作器
                if (DBWorker.IsRuning)
                {
                    DBWorker.Stop();
                    if (!DBWorker.IsRuning)
                        TraceManager.AppendDebug("数据库工作器已经关闭");
                    else
                        TraceManager.AppendErrMsg("数据库工作器关闭失败");
                }

                // 日志管理器
                if (TraceManager.IsRuning)
                    TraceManager.Stop();
            }
            catch { }

            IsRuning = false;
        }

        // 对调度服务实现的接口
        public event Action<TraceItem> evtTraceTrigger;
        public void ReceiveCommand(RequestCommand command)
        {

        }
        /// <summary>
        /// 触发日志事件
        /// </summary>
        /// <param name="item"></param>
        private void TriggerTrace(TraceItem item)
        {
            if (this.evtTraceTrigger != null)
                evtTraceTrigger(item);
        }
    }
}
