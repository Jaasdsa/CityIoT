using CityLogService;
using CityUtils;
using OPCAutomation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CityOPCDataService
{
    public class OpcDaClient
    {
        // OPC DA 接口, 全局一个就 Group
        private OPCServer _client = null;
        private OPCGroup _group = null;
        private bool _isOpcConnected = false;

        // 值缓存器
        private Dictionary<string, OpcTag> _tagNamedIndex = new Dictionary<string, OpcTag>();
        private Dictionary<int, OpcTag> _tagClientHandleIndex = new Dictionary<int, OpcTag>();
        private Dictionary<int, OpcTag> _tagServerHandleIndex = new Dictionary<int, OpcTag>();

        // OPCDA 的配置参数
        private string _hostIp;
        private string _progId;
        private int _updateRate = 1000;
        private int _defaultGroupDeadband = 0;

        // 枚举本地 OPC
        public static string[] GetLocalServer()
        {
            List<string> serverName = new List<string>();

            // 获取本地计算机上的 OPCServerName
            try
            {
                OPCServer server = new OPCServer();
                object serverList = server.GetOPCServers();
                
                foreach (string turn in (Array)serverList)
                    serverName.Add(turn.ToLower());
            }
            catch (Exception ex)
            {
                TraceManagerForOPC.AppendErrMsg(ex.Message);
                return null;
            }

            return serverName.ToArray();
        }
        // OPC 条目命名规则
        public static string GetTagName(string opcServerName, string fOpcserverDeviceName, string offsetAddress, string opcAddress)
        {
            return fOpcserverDeviceName + offsetAddress + opcAddress;
        }

        // 准备采集点表缓存     
        public OpcDaClient(string opcServerName, int updateRate = 250, string hostIP = null, int defaultGroupDeadband = 0)
        {
            this._defaultGroupDeadband = defaultGroupDeadband;
            this._updateRate = updateRate;
            this._hostIp = string.IsNullOrEmpty(hostIP) ? "localhost" : hostIP;
            this._progId = opcServerName;
        }
        public bool IsOpcConnected { get { return _isOpcConnected; } }
        public string ProgID { get { return _progId; } }

        // 开始/关闭 OPC服务器
        public bool Start()
        {
            if (!Connect())
                return false;

            StartTestAlive();

            StartAsyncRefreshFromDevice();
            return true;
        }
        public void Stop()
        {
            StopAsyncRefreshFromDevice();
            StopTestAlive();
            Disconnect();
        }

        public OpcTag GetTagByName(string name, out string errMsg)
        {
            errMsg = "";
            if (!_isOpcConnected)
            {
                errMsg = "OPC服务器已经关闭，请重新打开服务器";
                return null;
            }

            lock (_tagNamedIndex)
            {
                if (_tagNamedIndex.TryGetValue(name, out OpcTag opcTag))
                    return opcTag;
            }

            errMsg = "未在 " + _progId + " 缓存器中找到-" + name + "-条目";

            return null;
        }
        // 控制入口  
        public bool Write(string tagName, object value, out string errMsg)
        {
            errMsg = "";

            try
            {
                // 为避免 COM 对象的并发访问, 再建一个连接
                OPCServer client = new OPCServer();
                client.Connect(this._progId, this._hostIp);

                OPCGroup group = client.OPCGroups.Add("WritingGroup");                
                OPCItem item = group.OPCItems.AddItem(tagName, 1);

                int[] serverHandle = new int[] { 0, item.ServerHandle };
                object[] writingValue = new object[] { null, value };
                group.SyncWrite(1, serverHandle, writingValue, out Array error);
                client.Disconnect();

                // 据推测故障码是 COM 函数返回值, 0 表示正常
                int errorCode = Convert.ToInt32(error.GetValue(1));
                if (errorCode != 0)
                {
                    errMsg = this._progId + "的条目: " + tagName + " 写入失败; " + errorCode;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errMsg = this._progId + "的条目: " + tagName + "写入失败: " + ex.Message + "\n" + ex.StackTrace;
                TraceManagerForOPC.AppendErrMsg(errMsg);

                return false;
            }
        }

        // 连接/断开连接 OPC 服务器
        private bool Connect(bool silent = false)
        {
            if (_isOpcConnected)
                return true;
            
            try
            {
                // KingView.View 或 Kepware.KepServerEX.V6
                _client = new OPCServer();
                _client.Connect(this._progId, this._hostIp);
                if (_client.ServerState != (int)OPCServerState.OPCRunning)
                    return false;
         
                BuildGroupsAndTags();   
                _isOpcConnected = true;

                return true;
            }
            catch(Exception e)
            {
                if (!silent)
                    TraceManagerForOPC.AppendErrMsg("连接OPC服务器 " + this._progId + " 失败: " + e.Message);

                return false;
            }
        }
        private void Disconnect()
        {
            try
            {
                _isOpcConnected = false;

                if (_client != null)
                {
                    lock (_client)
                        _client.Disconnect();

                    _client = null;
                }

                _group = null;

                lock (_tagNamedIndex)
                {
                    _tagNamedIndex.Clear();
                }

                lock (_tagClientHandleIndex)
                {
                    _tagClientHandleIndex.Clear();
                }

                lock (_tagServerHandleIndex)
                {
                    _tagServerHandleIndex.Clear();
                }
            }
            catch (Exception ex)
            {
                TraceManagerForOPC.AppendErrMsg("OPC 客户端主动下线失败: " + ex.Message);
            }
        }
        // 枚举 OPC 服务器中所有标签
        private void BuildGroupsAndTags()
        {
            _client.OPCGroups.DefaultGroupIsActive = true;
            _client.OPCGroups.DefaultGroupDeadband = _defaultGroupDeadband;
            _client.OPCGroups.DefaultGroupUpdateRate = _updateRate;

            // 准备批量创建节点
            List<string> listItemIds = new List<string>();
            List<int> listClientHandles = new List<int>();
            int itemHandle = 1;
            int itemCount = 0;

            // 批量创建时, 数组标号从 1 开始, 0 位无用
            listItemIds.Add("");
            listClientHandles.Add(0);

            // 枚举节点
            OPCBrowser opcBrowser = _client.CreateBrowser();
            opcBrowser.ShowBranches();
            opcBrowser.ShowLeafs(true);
            foreach (object turn in opcBrowser)
            {
                string name = turn.ToString();
                if (string.IsNullOrEmpty(name))
                    continue;

                // 过滤系统变量
                if (name.StartsWith("_") || name.IndexOf("._") > 0)
                    continue;

                listItemIds.Add(name);
                listClientHandles.Add(itemHandle++);
                itemCount++;
            }

            // 创建组和节点
            _group = _client.OPCGroups.Add("RootGroup");
            Array itemIds = listItemIds.ToArray();
            Array clientHandles = listClientHandles.ToArray();
            _group.OPCItems.AddItems(itemCount, ref itemIds, ref clientHandles, out Array serverHandles, out Array errors);

            // 创建标签
            for (int i= 1; i < itemCount + 1; i++)
            {
                string itemName = Convert.ToString(itemIds.GetValue(i));
                int clientHandle = Convert.ToInt32(clientHandles.GetValue(i));
                int serverHandle = Convert.ToInt32(serverHandles.GetValue(i));

                OpcTag opcTag = new OpcTag(itemName, clientHandle, serverHandle);

                lock (_tagNamedIndex)
                    _tagNamedIndex[itemName] = opcTag;

                lock (_tagClientHandleIndex)
                    _tagClientHandleIndex[clientHandle] = opcTag;

                lock (_tagServerHandleIndex)
                    _tagServerHandleIndex[serverHandle] = opcTag;
            }

            // 启用订阅
            _group.IsSubscribed = true;
            _group.DataChange += OnDataChange;

            // 从客户端缓存读取一次, 获取初始值
            itemCount = _group.OPCItems.Count;
            _group.SyncRead((short)OPCDataSource.OPCCache, itemCount, ref serverHandles, out Array values, out errors, out object qualities, out object timeStamps);
            for (int i = 1; i < itemCount + 1; i++)
            {
                int serverHandle = Convert.ToInt32(serverHandles.GetValue(i));
                OpcTag opcTag = null;

                lock (_tagServerHandleIndex)
                    _tagServerHandleIndex.TryGetValue(serverHandle, out opcTag);
                if (opcTag == null)
                    continue;

                object value = values.GetValue(i);
                int quality = Convert.ToInt32(((Array)qualities).GetValue(i));
                DateTime timeStamp = Convert.ToDateTime(((Array)timeStamps).GetValue(i));

                opcTag.Set(value, quality, timeStamp);
            }

            // 从设备异步读取一次
            _group.AsyncRefresh((short)OPCDataSource.OPCDevice, 1001, out int cancelId);
        }
        private void OnDataChange(int TransactionID, int NumItems, ref Array clientHandles, ref Array itemValues, ref Array qualities, ref Array timeStamps)
        {
            // 服务端异常退出后, 可能还会有残留的回调, 丢弃
            if (!_isOpcConnected)
                return;

            for (int i = 1; i < NumItems + 1; i++)
            {
                try
                {
                    int clientHandle = Convert.ToInt32(clientHandles.GetValue(i));
                    OpcTag opcTag = null;
                    lock (_tagClientHandleIndex)
                        _tagClientHandleIndex.TryGetValue(clientHandle, out opcTag);

                    if (opcTag == null)
                        continue;

                    object value = itemValues.GetValue(i);
                    int quality = Convert.ToInt32(qualities.GetValue(i));
                    DateTime timeStamp = Convert.ToDateTime(timeStamps.GetValue(i));
                    opcTag.Set(value, quality, timeStamp);
                }
                catch { }
            }
        }
        private bool EqualsValue(object value, object setValue)
        {
            if (value.Equals(setValue))
                return true;
            // 开关型判断
            if (value.ToString().ToUpper() == "TRUE" && DataUtil.ToInt(setValue) == 1)
                return true;
            if (value.ToString().ToUpper() == "FALSE" && DataUtil.ToInt(setValue) == 0)
                return true;
            // 尝试转数值类型试一下
            if (double.TryParse(value.ToString(), out double dValue) && double.TryParse(setValue.ToString(), out double dSetValue))
            {
                if (dValue == dSetValue)
                    return true;
                else
                    return false;
            }
            // 其它都转字符串
            if (value.ToString() == setValue.ToString())
                return true;
            return false;
        }

        // 自动重连线程
        private Task _taskTestAlive;
        private AutoResetEvent _testAliveExit;
        private void StartTestAlive()
        {
            _testAliveExit = new AutoResetEvent(false);
            _taskTestAlive = new Task(() =>
            {
                bool isReconnecting = false;
                int timeout = 100;

                while (!_testAliveExit.WaitOne(timeout))  // 每秒检测一次
                {
                    try
                    {
                        lock (_client)
                        {
                            if (_client.ServerState == (int)OPCServerState.OPCRunning)  //状态码正常
                                continue;
                        }
                    }
                    catch { }

                    if (!isReconnecting)
                    {
                        _isOpcConnected = false;
                        isReconnecting = true;

                        TraceManagerForOPC.AppendInfo("OPCServer 离线了自动重连中...");

                        _client = null;

                        lock (_tagNamedIndex)
                        {
                            _tagNamedIndex.Clear();
                        }

                        lock (_tagClientHandleIndex)
                        {
                            _tagClientHandleIndex.Clear();
                        }

                        lock (_tagServerHandleIndex)
                        {
                            _tagServerHandleIndex.Clear();
                        }
                    }

                    if (Connect(true))
                    {
                        TraceManagerForOPC.AppendInfo("OPCServer 离线重连成功");

                        isReconnecting = false;
                        timeout = 100;
                    }
                    else
                    {
                        // 重连失败说明服务端可能正在启动, 适当延长重连周期
                        timeout = 2000;
                    }
                }
            }, TaskCreationOptions.LongRunning);

            _taskTestAlive.Start();
        }
        private void StopTestAlive()
        {
            _testAliveExit.Set();
            _taskTestAlive.Wait();
            _taskTestAlive.Dispose();
            _taskTestAlive = null;
        }

        // 定时从设备拿数据-网络中断恢复才不上数据
        private System.Timers.Timer _timerRefreshFromDevice; 
        private void StartAsyncRefreshFromDevice()
        {
            _timerRefreshFromDevice = new System.Timers.Timer();
            _timerRefreshFromDevice.Interval = 1 *60 * 1000;
            _timerRefreshFromDevice.Elapsed += (o, e) =>
            {
                try
                {
                    if (this.IsOpcConnected)
                    {
                        _group.AsyncRefresh((short)OPCDataSource.OPCDevice, 1002, out int cancelId);
                    }
                }
                catch (Exception ee)
                {
                    TraceManagerForOPC.AppendErrMsg("OPC DA定时从客户端刷新数据异常" + ee.Message);
                }
            };
            _timerRefreshFromDevice.Enabled = true;
        }
        private void StopAsyncRefreshFromDevice()
        {
            if (_timerRefreshFromDevice != null)
            {
                _timerRefreshFromDevice.Enabled = false;
                _timerRefreshFromDevice.Close();
                _timerRefreshFromDevice = null;
            }
        }
    }
}
