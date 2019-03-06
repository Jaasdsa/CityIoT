using CityIoTCommand;
using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityOPCDataService
{
    public class OPCPumpService : ISonService, IServiceWorker
    {
        // 对调度服务实现的接口
        public void ReceiveCommand(RequestCommand command)
        {
            // 已经在入口验证过命令对象
            if (!IsRuning || this.commandCustomer == null || !this.commandCustomer.IsRuning)
            {
                CommandManager.MakeFail("PUMP-OPC命令消费器运行异常", ref command);
                CommandManager.CompleteCommand(command);
                TraceManagerForCommand.AppendWarning("PUMP-OPC命令消费器运行异常");
                return;
            }
            this.commandCustomer.ReceiveCommand(command);
        }

        OpcClient opcClientManager;
        PumpCollecter pumpCollecter;
        PumpCommandConsumer commandCustomer;

        public OPCPumpService(ConfSonOPCPumpDataService confSonOPC)
        {
            Config.pumpConfig = confSonOPC;
        }

        public bool IsRuning { get; set; } = false;
        public void Start(out string errMsg)
        {
            errMsg = "";
            if (IsRuning)
                return;
            try
            {
                // 环境检查
                if (!EnvChecker.CheckForPump(out errMsg))
                {
                    TraceManagerForOPC.AppendErrMsg("PUMP-OPC环境检查未通过:"+ errMsg);
                    return;
                }
                TraceManagerForOPC.AppendDebug("PUMP-OPC环境检查通过");


                // OPC 客户端管理服务打开
                if (opcClientManager != null)
                    opcClientManager.Stop();
                opcClientManager = new OpcClient(OPCClientManagerType.Pump);
                opcClientManager.Start();
                if (opcClientManager.IsRuning)
                    TraceManagerForOPC.AppendDebug("OPC客户端管理服务已经打开");
                else
                {
                    TraceManagerForOPC.AppendErrMsg("OPC客户端管理服务打开失败");
                    Stop();
                    return;
                }

                // 命令消费器
                if (commandCustomer != null)
                    commandCustomer.Stop();
                commandCustomer = new PumpCommandConsumer(opcClientManager);
                commandCustomer.Start();
                if (commandCustomer.IsRuning)
                    TraceManagerForOPC.AppendDebug("命令消费器已经打开");
                else
                {
                    TraceManagerForOPC.AppendErrMsg("命令消费器打开失败");
                    Stop();
                    return;
                }

                // 泵房点表采集者服务
                if (pumpCollecter != null)
                    pumpCollecter.Stop();
                pumpCollecter = new PumpCollecter(opcClientManager,commandCustomer);
                pumpCollecter.Start();
                if (pumpCollecter.IsRuning)
                    TraceManagerForOPC.AppendDebug("点表采集管理服务已经打开");
                else
                {
                    TraceManagerForOPC.AppendErrMsg("点表采集管理服务打开失败");
                    Stop();
                    return;
                }

            }
            catch (Exception e)
            {
                errMsg = e.Message;
                TraceManagerForOPC.AppendErrMsg("PUMP_OPC服务启动异常"+e.Message + "堆栈:" + e.StackTrace);
                Stop();
                return;
            }

            IsRuning = true;
        }
        public void Stop()
        {
            try
            {
                // 点表采集者服务
                if (pumpCollecter != null)
                {
                    pumpCollecter.Stop();
                    if (!pumpCollecter.IsRuning)
                    {
                        TraceManagerForOPC.AppendDebug("点表采集管理服务已停止");
                        this.pumpCollecter = null;
                    }
                    else
                        TraceManagerForOPC.AppendErrMsg("点表采集管理服务停止失败");
                }

                // 命令消费器
                if (commandCustomer != null)
                {
                    commandCustomer.Stop();
                    if (!commandCustomer.IsRuning)
                    {
                        TraceManagerForOPC.AppendDebug("命令消费器已停止");
                        this.commandCustomer = null;
                    }
                    else
                        TraceManagerForOPC.AppendErrMsg("命令消费器停止失败");
                }

                // OPC 客户端管理服务
                if (opcClientManager != null)
                {
                    opcClientManager.Stop();
                    if (!opcClientManager.IsRuning)
                    {
                        TraceManagerForOPC.AppendDebug("OPC客户端管理服务已停止");
                        this.opcClientManager = null;
                    }
                    else
                        TraceManagerForOPC.AppendErrMsg("OPC客户端管理服务停止失败");
                }
            }
            catch { }

            IsRuning = false;
        }
    }
}