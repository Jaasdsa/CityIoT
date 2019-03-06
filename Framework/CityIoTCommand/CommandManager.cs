using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CityIoTCommand
{
    /// <summary>
    ///  命令管理器
    /// </summary>
   public class CommandManager
    {
        static ConfCommandServiceInfo config;//配置对象

        static string ip;
        static int port;
        static int timeoutSeconds;

        static CommandRecorder commandRecorder;
        static SocketCommandConsumer commandConsumer;
        static SuperSocketTcpServerOper tcpServer;
        static Action<RequestCommand> DoRequestCommand;

        public static bool IsRuning { get; set; }
        public static void Start(Action<RequestCommand> actionRequestCommand, ConfCommandServiceInfo confCommandServiceInfo, out string errMsg)
        {
            errMsg = "";
            if (IsRuning)
                return;

            config = confCommandServiceInfo;

            DoRequestCommand = actionRequestCommand;

            // 加载服务所需配置文件信息
            if (!CheckConfig(config, out errMsg))
                return;

            // 命令记录服务
            if (commandRecorder != null)
                commandRecorder.Stop();
            commandRecorder = new CommandRecorder();
            commandRecorder.Start();
            if (!commandRecorder.IsRuning)
            {
                TraceManagerForCommand.AppendErrMsg("命令记录服务打开失败");
                Stop();
                return;
            }

            // 命令消费者服务
            if (commandConsumer != null)
                commandConsumer.Stop();
            commandConsumer = new SocketCommandConsumer(DoRequestCommand);
            commandConsumer.Start();
            if (!commandConsumer.IsRuning)
            {
                TraceManagerForCommand.AppendErrMsg( "命令消费者服务打开失败");
                Stop();
                return;
            }

            //端口监听服务
            if (tcpServer != null)
                tcpServer.Stop();
            tcpServer = new SuperSocketTcpServerOper(CommandManager.ip, CommandManager.port);
            tcpServer.evtReciveHandle += CommandManager.commandConsumer.Append; //注册接受处理体
            if (!tcpServer.Start(out errMsg))
            {
                TraceManagerForCommand.AppendErrMsg( "命令端口监听服务打开失败");
                Stop();
                return;
            }

            IsRuning = true;
        }
        public static void Stop()
        {
            //端口监听服务
            try
            {
                if (tcpServer != null)
                {
                    tcpServer.evtReciveHandle -= CommandManager.commandConsumer.Append; //注册接受处理体
                    tcpServer.Stop();
                }
                tcpServer = null;
            }
            catch (Exception e)
            {
                TraceManagerForCommand.AppendErrMsg( "命令端口监听服务停止失败" + "堆栈:" + e.StackTrace);
            }


            // 命令消费者服务
            try
            {    
                if (commandConsumer != null)
                {
                    commandConsumer.Stop();
                    if (!commandConsumer.IsRuning)
                        CommandManager.commandConsumer = null;
                    else
                        TraceManagerForCommand.AppendErrMsg( "命令消费者服务停止失败");
                }
            }
            catch(Exception e)
            {
                TraceManagerForCommand.AppendErrMsg( "命令消费者服务停止失败:"+e.Message + "堆栈:" + e.StackTrace);
            }


            // 命令记录服务
            try
            {
                if (commandRecorder != null)
                {
                    commandRecorder.Stop();
                    if (!commandRecorder.IsRuning)
                        CommandManager.commandRecorder = null;
                    else
                        TraceManagerForCommand.AppendErrMsg( "命令记录服务停止失败");
                }
            }
            catch(Exception e)
            {
                TraceManagerForCommand.AppendErrMsg("命令记录服务停止失败:"+e.Message + "堆栈:" + e.StackTrace);
            }
            
            IsRuning = false;
        }

        private static bool CheckConfig(ConfCommandServiceInfo config, out string errMsg)
        {
            errMsg = "";
            if (!config.EnvIsOkay)
            {
                errMsg = config.ErrMsg;
                return false;
            }
            CommandManager.ip = config.IP;
            CommandManager.port = config.Port;
            CommandManager.timeoutSeconds = config.timeoutSeconds;

            if (!config.CheckIsValidPort(out errMsg))
                return false;

            return true;
        }

        /// <summary>
        /// 添加要记录的命令
        /// </summary>
        /// <param name="data"></param>
        public static void AppendRecCommand(RecCommand data)
        {
            if (commandRecorder == null || !commandRecorder.IsRuning)
                return;
            commandRecorder.Append(data);
        }

        /// <summary>
        /// 完成命令
        /// </summary>
        /// <param name="command"></param>
        public static void CompleteCommand(RequestCommand command)
        {
            command.endTime = DateTime.Now;
            command.timeConsuming = RecCommand.GetTimeSpanStr(command.beginTime, command.endTime);
            // 命令记录
            CommandManager.AppendRecCommand(RecCommand.ToRecCommand(command));
            // 命令回执
            ResponseCommand responseCommand;
            switch (command.state)
            {
                case CommandState.Succeed:
                    {
                        responseCommand = new ResponseCommand()
                        {
                            info = "命令执行成功",
                            errMsg = "",
                            statusCode = "200"
                        };
                    }
                    break;
                case CommandState.Timeout:
                    {
                        responseCommand = new ResponseCommand()
                        {
                            info = "",
                            errMsg = "命令执行超时",
                            statusCode = "490"
                        };
                    }
                    break;
                default:
                    {
                        responseCommand = new ResponseCommand()
                        {
                            info = "",
                            errMsg = "命令执行失败:" + command.message,
                            statusCode = "491"
                        };
                    }
                    break;
            }
            string data = ByteUtil.ToSerializeObject(responseCommand);
            string err = command.FinshCallBack(command.sessionID, data, true);
            if (!string.IsNullOrWhiteSpace(err))
                TraceManagerForCommand.AppendErrMsg( "命令ID:" + command.ID + "发送回执命令失败");
        }

        // 请求命令快速失败方法
        public static void MakeFail(string errMsg, ref RequestCommand command)
        {
            command.state = CommandState.Failed;
            command.message = errMsg;
        }
        public static void MakeTimeout(string errMsg, ref RequestCommand command)
        {
            command.state = CommandState.Timeout;
            command.message = errMsg;
        }
        public static void MakeSuccess(string message, ref RequestCommand command)
        {
            command.state = CommandState.Succeed;
            command.message = message;
        }

    }
}
