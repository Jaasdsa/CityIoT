using CityIoTCommand;
using CityLogService;
using CityPublicClassLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace JiangXiXingGuo
{
    public class XingGuoService : CaseManagerInjection, ISonService, IServiceWorker
    {
        // WEB-SOAP-XINGGUO 数据采集任务
        private System.Timers.Timer timer;
        private Param param = new Param();
        private CommandConsumer commandCustomer;

        public override void ReceiveCommand(RequestCommand command)
        {
            // 已经在入口验证过命令对象
            if (!IsRuning || this.commandCustomer == null || !this.commandCustomer.IsRuning)
            {
                CommandManager.MakeFail("WEB-SOAP-兴国命令消费器运行异常", ref command);
                CommandManager.CompleteCommand(command);
                TraceManagerForCommand.AppendErrMsg("WEB-SOAP-兴国命令消费器运行异常");
                return;
            }
            this.commandCustomer.Append(command);
        }

        public override void Start(out string errMsg)
        {
            errMsg = "";
            if (IsRuning)
                return;
            
            WEBSoapCommand.CreateInitSensorRealData(param).Execute(); //初始化实时表

            timer = new System.Timers.Timer();
            timer.Interval = this.param.collectInterval * 1000;
            timer.Elapsed += (o, e) =>
            {
                try
                {
                    Excute();
                }
                catch (Exception ee)
                {
                    TraceManagerForProject.AppendErrMsg("WEB-SOAP-兴国定时任务执行失败:" + ee.Message);
                }
            };
            timer.Enabled = true;

            // 控制器服务
            if (commandCustomer != null)
                commandCustomer.Stop();
            commandCustomer = new CommandConsumer(ConsumerCommand);
            commandCustomer.Start();
            if (commandCustomer.IsRuning)
                TraceManagerForProject.AppendDebug("WEB-SOAP-兴国控制器服务已经打开");
            else
            {
                TraceManagerForProject.AppendErrMsg("WEB-SOAP-兴国控制器服务打开失败");
                Stop();
                return;
            }

            IsRuning = true;

            // 开始异步执行一次-防止启动卡死
            Action action = Excute;
            action.BeginInvoke(null, null);
        }
        public override bool IsRuning { get; set; }
        private bool ExcuteDoing { get; set; } = false;
        public override void Stop()
        {
            if (!IsRuning)
                return;

            try
            {
                // 控制器服务
                if (commandCustomer != null)
                {
                    commandCustomer.Stop();
                    if (!commandCustomer.IsRuning)
                    {
                        TraceManagerForProject.AppendDebug("WEB-SOAP-兴国控制器服务已停止");
                        this.commandCustomer = null;
                    }
                    else
                        TraceManagerForProject.AppendErrMsg("WEB-SOAP-兴国控制器服务停止失败");
                }
            }
            catch { }

            // 关闭定时器
            if (timer != null)
            {
                timer.Enabled = false;
                timer.Close();
                timer = null;
            }

            IsRuning = false;
        }

        private void Excute()
        {
            lock (this)
            {
                if (ExcuteDoing)
                    return;
                ExcuteDoing = true;
                ExcuteHandle();
                ExcuteDoing = false;
            }
        }
        private void ExcuteHandle()
        {
            // 报警维护
            WEBSoapCommand.CreateCollectAndSaveScadaSensors(param).Execute();
        }

        // 执行调度命令
        private void ConsumerCommand(RequestCommand command)
        {
            try
            {
                ExcuteCommand(command);
            }
            catch (Exception e)
            {
                CommandManager.MakeFail("WEB-SOAP-兴国 定时任务执行失败:" + e.Message, ref command);
                CommandManager.CompleteCommand(command);
                TraceManagerForCommand.AppendErrMsg(command.message);
            }
        }
        private void ExcuteCommand(RequestCommand command)
        {
            if (command.operType == CommandOperType.ReLoadData)
            {
                if (ExcuteDoing) // 正在采集，等这次采集结束，在采集一次
                {
                    DateTime time1 = DateTime.Now;
                    while (true)
                    {
                        Thread.Sleep(1);
                        if (DateTime.Now - time1 > TimeSpan.FromSeconds(command.timeoutSeconds)) // 超时
                        {
                            CommandManager.MakeTimeout("WEB-SOAP-兴国 数据更新超时", ref command);
                            CommandManager.CompleteCommand(command);
                            TraceManagerForCommand.AppendInfo(command.message);
                            return;
                        }
                        if (!ExcuteDoing)
                            break;
                    }
                }
                // 调取之前先重新加载一次缓存
                Excute();
                CommandManager.MakeSuccess("WEB-SOAP-兴国 数据已更新", ref command);
                CommandManager.CompleteCommand(command);
                TraceManagerForCommand.AppendInfo("WEB-SOAP-兴国 数据已更新");
                return;
            }
            CommandManager.MakeFail("错误的请求服务类型", ref command);
            CommandManager.CompleteCommand(command);
            TraceManagerForCommand.AppendErrMsg(command.message);
            return;

        }
    }
}
