using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CityIoTCommand;
using CityIoTServiceManager;

namespace CityIoTCore
{
   public class IoTServiceCore:IWorker
    {
        private DataPublishServer dataPublish;
        private SonServiceManager sonServiceManager;

        public bool IsRuning { get; set; }

        public void Start()
        {
            try
            {
                // 启动前配置文件检查--包括端口检查
                Config.configInfo = EnvConfigInfo.SingleInstanceForCS;
                if (!Config.configInfo.EnvCheckForBeforeRun(out string errMsg))
                    return;

                // 日志管理器
                if (LogManager.IsRuning)
                    LogManager.Stop();
                LogManager.Start(Config.configInfo.confLogServiceInfo, out  errMsg);
                if (!string.IsNullOrWhiteSpace(errMsg) || LogManager.IsRuning == false)
                {
                    Stop();
                    return;
                }
                LogManager.AppendDebug(ServerTypeName.Core, "日志管理器已经启动");

                // 环境检查-日志启动后，在检查其它配置环境，让错误信息可以报出来
                if (!IPTool.PingIP(Config.configInfo.confDataBaseInfo.IP))
                    TraceManagerForDispatch.AppendWarning("ping:"+Config.configInfo.confDataBaseInfo.IP+"返回超时"); // 有可能开了防火墙

                if (!EnvChecker.Check(Config.configInfo, out errMsg))
                {
                    TraceManagerForDispatch.AppendErrMsg("环境检查未通过:" + errMsg); // 不用听，让客户端主动停，看到消息
                    return;
                }
                else
                    TraceManagerForDispatch.AppendInfo(string.Format("环境检查通过:已加载【{0}】配置", Config.configInfo.confProjectInfo.ProjectName));


                //// 数据发布器
                //if (dataPublish != null)
                //    dataPublish.Stop();
                //dataPublish = new DataPublishServer(Config.projectConfigPath);
                //dataPublish.Start(out  errMsg);
                //if (!string.IsNullOrWhiteSpace(errMsg) || dataPublish.IsRuning == false)
                //{
                //    Stop();
                //    return;
                //}
                //LogManager.AppendDebug(ServerTypeName.Dispatch, "数据发布器已经启动");

                // 子服务管理器
                if (sonServiceManager != null)
                    sonServiceManager.Stop();
                sonServiceManager = new SonServiceManager();
                sonServiceManager.Start(out errMsg);
                if (!string.IsNullOrWhiteSpace(errMsg) || sonServiceManager.IsRuning == false)
                {
                    Stop();
                    return;
                }
                LogManager.AppendDebug(ServerTypeName.Core, "子服务管理器器已经启动");

                // 命令管理器
                if (CommandManager.IsRuning)
                    CommandManager.Stop();
                CommandManager.Start(this.sonServiceManager.ReceiveCommand, Config.configInfo.confCommandServiceInfo, out errMsg);
                if (!string.IsNullOrWhiteSpace(errMsg) || CommandManager.IsRuning == false)
                {
                    Stop();
                    return;
                }
                LogManager.AppendDebug(ServerTypeName.Core, "命令管理器已经启动");

                LogManager.AppendInfo(ServerTypeName.Core, "所有服务都已经启动");

                IsRuning = true;
            }
            catch(Exception e)
            {
                if (LogManager.IsRuning)
                    LogManager.AppendErrMsg(ServerTypeName.Core, "服务启动失败：" + e.Message+"堆栈:"+e.StackTrace);
                Stop();
            }
        }
        public void Stop()
        {
            try
            {
                // 命令管理器
                if (CommandManager.IsRuning)
                    CommandManager.Stop();
                if (!CommandManager.IsRuning)
                    LogManager.AppendDebug(ServerTypeName.Core, "命令管理器已停止");
                else
                    LogManager.AppendErrMsg(ServerTypeName.Core, "命令管理器停止失败");
            }
            catch (Exception e)
            {
                LogManager.AppendErrMsg(ServerTypeName.Core, "命令管理器停止失败:" + e.Message + "堆栈:" + e.StackTrace);
            }

            try
            {
                // 子服务管理器
                if (sonServiceManager != null)
                {
                    sonServiceManager.Stop();
                    if (!sonServiceManager.IsRuning)
                    {
                        LogManager.AppendDebug(ServerTypeName.Core, "子服务管理器已停止");
                        this.sonServiceManager = null;
                    }
                    else
                        LogManager.AppendErrMsg(ServerTypeName.Core, "子服务管理器停止失败");
                }
            }
            catch (Exception e)
            {
                LogManager.AppendErrMsg(ServerTypeName.Core, "子服务管理器停止失败:" + e.Message + "堆栈:" + e.StackTrace);
            }

            //try
            //{
            //    // 数据发布器
            //    if (dataPublish != null)
            //    {
            //        dataPublish.Stop();
            //        if (!dataPublish.IsRuning)
            //        {
            //            LogManager.AppendDebug(ServerTypeName.Dispatch, "数据发布器已停止");
            //            this.dataPublish = null;
            //        }
            //        else
            //            LogManager.AppendErrMsg(ServerTypeName.Dispatch, "数据发布器停止失败");
            //    }
            //}
            //catch(Exception e)
            //{
            //    LogManager.AppendErrMsg(ServerTypeName.Dispatch, "数据发布器停止失败:" + e.Message);
            //}

            // 停止消息发布
            LogManager.AppendInfo(ServerTypeName.Core, "所有服务都已经停止");

            // 日志管理器
            try
            {
                // 数据发布器
                if (LogManager.IsRuning)
                    LogManager.Stop();
            }
            catch { }
            }     
    }
}
