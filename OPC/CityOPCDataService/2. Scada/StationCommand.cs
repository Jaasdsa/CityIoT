using CityIoTCommand;
using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityOPCDataService
{
    enum StationCommandType
    {
        Collect,
        Write,
        Reload,
    }
    class StationCommand
    {
        StationCommandType type;
        OpcClient opcClientManager;
        static DateTime lastSaveTime = DateTime.MinValue;
        RequestCommand requestCommand;
        OPCCommandType opcCommandType;

        /// <summary>
        /// 创建定时采集任务
        /// </summary>
        /// <param name="type"></param>
        /// <param name="opcClientManager"></param>
        /// <returns></returns>
        public static StationCommand CreateCollectCommand(OpcClient opcClientManager)
        {
            return new StationCommand()
            {
                type = StationCommandType.Collect,
                opcClientManager = opcClientManager,
                opcCommandType = OPCCommandType.Pending,
            };
        }
        /// <summary>
        /// 创建重载站点数据任务
        /// </summary>
        /// <param name="opcClientManager"></param>
        /// <param name="pointsSaveInterVal"></param>
        /// <returns></returns>
        public static StationCommand CreateReloadCommand(OpcClient opcClientManager, RequestCommand requestCommand)
        {
            return new StationCommand()
            {
                type = StationCommandType.Reload,
                opcClientManager = opcClientManager,
                requestCommand = requestCommand,
                opcCommandType = OPCCommandType.Pending,
            };
        }
        /// <summary>
        /// 创建站点写值任务
        /// </summary>
        /// <param name="opcClientManager"></param>
        /// <param name="pointsSaveInterVal"></param>
        /// <returns></returns>
        public static StationCommand CreateWriteCommand(OpcClient opcClientManager, RequestCommand requestCommand)
        {
            return new StationCommand()
            {
                type = StationCommandType.Write,
                opcClientManager = opcClientManager,
                requestCommand = requestCommand,
                opcCommandType = OPCCommandType.Pending,
            };
        }

        // 任务执行口--并附带超时功能
        public void Excute()
        {
            ActionTimeout timeoutObj = new ActionTimeout();
            try
            {
                switch (this.type)
                {
                    case StationCommandType.Collect:
                        {
                            if (opcCommandType == OPCCommandType.Cancle)  //已经在调度被标记放弃了，干要紧的任务
                                return;
                            timeoutObj.Do = ExcuteCollect;
                            bool bo = timeoutObj.DoWithTimeout(new TimeSpan(0, 0, 0, Config.scadaConfig.commandTimeoutSeconds));
                            if (bo)
                            {
                                TraceManagerForOPC.AppendWarning("站点定时采集任务执行超时");
                                GC.Collect();
                            }
                        }
                        break;
                    case StationCommandType.Write:
                        {
                            timeoutObj.Do = ExcuteWrite;
                            bool bo = timeoutObj.DoWithTimeout(new TimeSpan(0, 0, 0, Config.scadaConfig.commandTimeoutSeconds));
                            if (bo)
                            {
                                CommandManager.MakeTimeout("sensorID：" + requestCommand.sensorID + "执行写值命令超时", ref requestCommand);
                                CommandManager.CompleteCommand(requestCommand);
                                TraceManagerForCommand.AppendWarning(requestCommand.message);
                                GC.Collect();
                            }
                        }
                        break;
                    case StationCommandType.Reload:
                        {
                            timeoutObj.Do = ExcuteReload;
                            bool bo = timeoutObj.DoWithTimeout(new TimeSpan(0, 0, 0, Config.scadaConfig.commandTimeoutSeconds));
                            if (bo)
                            {
                                CommandManager.MakeTimeout("执行重载站点数据命令超时", ref requestCommand);
                                CommandManager.CompleteCommand(requestCommand);
                                TraceManagerForCommand.AppendWarning(requestCommand.message);
                                GC.Collect();
                            }
                        }
                        break;
                    default:
                        TraceManagerForOPC.AppendWarning("未知的站点OPC命令类型,无法执行");
                        return;
                }
            }
            catch (Exception e)
            {
                TraceManagerForOPC.AppendErrMsg("执行站点OPC任务失败,未知的任务类型:" + e.Message + "堆栈:" + e.StackTrace);
            }
        }

        // 任务分发口
        public void ExcuteCollect()
        {
            try
            {
                if (!ExcuteCollectHandle(out string errMsg))
                    TraceManagerForOPC.AppendWarning("站点定时采集任务执行失败;" + errMsg);
            }
            catch (Exception e)
            {
                TraceManagerForOPC.AppendErrMsg("站点定时采集任务执行失败--" + e.Message + "堆栈:" + e.StackTrace);
            }
        }
        public void ExcuteWrite()
        {
            try
            {
                if (!ExcuteWriteHandle(out string errMsg, out string info))
                {
                    CommandManager.MakeFail(errMsg, ref requestCommand);
                    CommandManager.CompleteCommand(requestCommand);
                    TraceManagerForCommand.AppendErrMsg(requestCommand.message);
                    return;
                }
                CommandManager.MakeSuccess(info, ref requestCommand);
                CommandManager.CompleteCommand(requestCommand);
                TraceManagerForCommand.AppendInfo(info);
            }
            catch (Exception e)
            {
                TraceManagerForOPC.AppendErrMsg("站点写值任务执行失败--" + e.Message + "堆栈:" + e.StackTrace);
                SendError("站点写值任务执行失败--" + e.Message);
            }
        }
        public void ExcuteReload()
        {
            try
            {
                if (!ExcuteReloadHandle(out string errMsg))
                {
                    CommandManager.MakeFail(errMsg, ref requestCommand);
                    CommandManager.CompleteCommand(requestCommand);
                    TraceManagerForCommand.AppendWarning(requestCommand.message);
                    return;
                }
                CommandManager.MakeSuccess("重载站点数据命令成功,SCADA-OPC数据已更新", ref requestCommand);
                CommandManager.CompleteCommand(requestCommand);
                TraceManagerForCommand.AppendInfo("重载站点数据命令成功,SCADA-OPC数据已更新");
            }
            catch (Exception e)
            {
                TraceManagerForOPC.AppendErrMsg("站点主动采集任务执行失败--" + e.Message + "堆栈:" + e.StackTrace);
                SendError("站点主动采集任务执行失败--" + e.Message);
            }
        }

        // 异常发送给客户端--避免一直等待超时
        private void SendError(string errMsg)
        {
            try
            {
                if (this.requestCommand != null)
                {
                    CommandManager.MakeFail(errMsg, ref requestCommand);
                    CommandManager.CompleteCommand(requestCommand);
                }
            }
            catch { }
        }

        // 定时采集任务执行体
        public bool ExcuteCollectHandle(out string errMsg)
        {
            errMsg = "";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间
            // 加载需要采集点
            if (!LoadCache(out Dictionary<int, Station> dicStations, out  errMsg))
                return false;

            // 从OPC缓存中取数
            if (!Collect(ref dicStations, out errMsg))

                return false;
            // 存入到数据库
            string saveSQL = GetSavePointsSQL(dicStations);
            if (string.IsNullOrWhiteSpace(saveSQL))
            {
                errMsg= string.Format(@"采集OPC-站点数量{0}获取存入数据库SQL失败", dicStations.Count);
                return false;
            }
            DBUtil.ExecuteNonQuery(saveSQL, out string err);
            stopwatch.Stop(); //  停止监视
            TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
            double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数

            if (!string.IsNullOrWhiteSpace(err))
            {
                errMsg= "更新OPC-SCADA实时数据失败" + ",耗时:" + milliseconds.ToString() + "毫秒," + err;
                return false;
            }
            TraceManagerForOPC.AppendDebug("更新OPC-SCADA实时数据成功" + ",耗时:" + milliseconds.ToString() + "毫秒");
            return true;
        }
        private bool LoadCache(out Dictionary<int, Station> dicStations, out string err)
        {
            err = "";
            dicStations = new Dictionary<int, Station>();

            #region 查询sensor表
            string sqlSensors = @"select  x.*,x1.ID  pointID ,x1.*  from( select t.ID stationID,t.Name StationName ,t.FOPCDeviceName ,t.FOPCServerName,t.FPLCIP, t1.ID sensorID,t1.Name sensorName,t1.PointAddressID 
                                                 from SCADA_Station t ,SCADA_Sensor t1 where t.ID=t1.StationID and t.ReadMode='OPC'
                                                 and (t.是否删除=0 or t.是否删除 is null ) and (t1.是否删除=0 or t1.是否删除 is null )) x
                                              left join PointAddressEntry x1 on x.PointAddressID=x1.ID and x1.是否激活=1;";
            DataTable dtSensors = DBUtil.ExecuteDataTable(sqlSensors, out string errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                err = "查询sensor列表失败：" + errMsg;
                return false;
            }
            foreach (DataRow dr in dtSensors.Rows)
            {
                Station station = new Station()
                {
                    _ID = DataUtil.ToInt(dr["stationID"]),
                    _StationName= DataUtil.ToString(dr["StationName"]),
                    _FOPCDeviceName = DataUtil.ToString(dr["FOPCDeviceName"]),
                    _FOPCServerName = DataUtil.ToString(dr["FOPCServerName"]),
                    _FPLCIP = DataUtil.ToString(dr["FPLCIP"]),
                    sensors = new List<Sensor>()
                };
                Sensor sensor = new Sensor()
                {
                    pointID = DataUtil.ToInt(dr["pointID"]),
                    sensorID = DataUtil.ToString(dr["sensorID"]),
                    sensorName = DataUtil.ToString(dr["sensorName"]),
                    _PointAddressID = DataUtil.ToInt(dr["PointAddressID"]),
                    versionID = DataUtil.ToInt(dr["版本ID"]),
                    dataSourceAddress = DataUtil.ToString(dr["数据源地址"]).Trim(),
                    offsetAddress = DataUtil.ToString(dr["偏移地址"]).Trim(),
                    type = Point.ToType(DataUtil.ToString(dr["数据类型"])),
                    isActive = DataUtil.ToInt(dr["是否激活"]),
                    isWrite = DataUtil.ToInt(dr["是否写入"]),
                    scale = DataUtil.ToDouble(dr["倍率"])
                };
                // 站点-sensor入字典库
                if (dicStations.Keys.Contains(station._ID))
                    dicStations[station._ID].sensors.Add(sensor);
                else
                {
                    station.sensors.Add(sensor);
                    station.opc = this.opcClientManager.GetOPCClient(station._FOPCServerName); // 将站点的OPC对象赋值
                    if (station.Check(out errMsg))
                        dicStations.Add(station._ID, station);
                    else
                        TraceManagerForOPC.AppendWarning("StationID:"+station._ID+"环境异常:"+errMsg);
                }
            }
            if (dicStations.Keys.Count == 0)
            {
                err = "站点表明细没有关于OPC的站点表";
                return false;
            }
            #endregion

            return true; ;
        }
        private bool Collect(ref Dictionary<int, Station> dicStations, out string errMsg)
        {
            errMsg = "";
            if (dicStations == null)
            {
                errMsg = "无法采集空对象的站点组";
                return false;
            }
            foreach (int key in dicStations.Keys)
            {
                Station station = dicStations[key];
                if (station.opc == null || !station.opc.IsOpcConnected)
                {
                    TraceManagerForOPC.AppendWarning("StationName:" + station._StationName + "关联的OPCServer已经离线");
                    station.IsOnline = false;
                    continue;
                }

                if (station.sensors == null || station.sensors.Count == 0)
                {
                    TraceManagerForOPC.AppendWarning("StationName:" + station._StationName + "没有关联对应的sensor需要采集");
                    station.IsOnline = false;
                    continue;
                }

                // 单点采集
                int errorTimes = 0; // n个失败就不采集
                int okayTimes = 0; // n个成功，就在线
                string errPointIDs = "";
                DateTime dt = DateTime.Now;
                foreach (Sensor sensor in dicStations[key].sensors)
                {
                    sensor.LastTime = DataUtil.ToDateString(dt);
                    // 防止采集的点多了，错误消息炸了，每个都报出来了---直接让机组离线
                    if (errorTimes >= Config.scadaConfig.errorTimes)
                    {
                        TraceManagerForOPC.AppendWarning(string.Format("站点名称:{0}-pointID列表:{1} 采集失败,已跳过该站点采集，可能原因:站点离线或者被禁用采集", station._StationName,errPointIDs));
                        station.IsOnline = false;
                        break;
                    }
                    // 检查未通过
                    if (!sensor.CheckScadaOPC(out string err))
                    {
                        sensor.MakeFail(sensor.sensorName + err);
                        TraceManagerForOPC.AppendWarning(sensor.sensorName +"配置错误"+ err);
                        continue;
                    }
                    // 跳过忽略型
                    if (sensor.type == PointType.Ignore)
                        continue;

                    string opcItemName = OpcDaClient.GetTagName(station._FOPCServerName, station._FOPCDeviceName, sensor.offsetAddress, sensor.dataSourceAddress);

                    // OPC 客户端缓存队列寻找该条目值
                    OpcTag tag = station.opc.GetTagByName(opcItemName, out string er);
                    if (tag == null)
                    {
                        sensor.MakeFail(er);// 未找到tag
                        TraceManagerForOPC.AppendErrMsg(er); // 未找到tag
                        continue;
                    }
                    if (!string.IsNullOrEmpty(tag.Error))
                    {
                        sensor.MakeFail(tag.Error);
                        errorTimes++;
                        errPointIDs += sensor.pointID.ToString() + " ";
                        //   TraceManagerForOPC.AppendWarning(string.Format("{0}-{1}采集失败,错误原因:{2}", station._StationName, sensor.sensorName, sensor.Mess));
                        continue;
                    }
                    if (tag.Value == null)
                    {
                        sensor.MakeFail(opcItemName + "取值失败");
                        errorTimes++;
                        errPointIDs += sensor.pointID.ToString() + " ";
                        //  TraceManagerForOPC.AppendWarning(string.Format("{0}-{1}取值失败,错误原因:{2}", station._StationName, sensor.sensorName, sensor.Mess));
                        continue;
                    }
                    // 根据数据源获取数据
                    sensor.ReadOPCPoint(tag.Value);
                    if (sensor.State == ValueState.Fail)
                    {
                        // TraceManagerForOPC.AppendWarning(string.Format("{0}-{1}采集失败,取值错误:{2}", station._StationName, sensor.sensorName, sensor.Mess));
                        errorTimes++;
                        errPointIDs += sensor.pointID.ToString() + " ";
                        continue;
                    }
                    else
                        okayTimes++;
                }
                // 判断是否离线

                if (okayTimes >= Config.scadaConfig.okayTimes)
                    station.IsOnline = true;
                else
                    station.IsOnline = false;
                AddOnLinePoint(station.IsOnline, ref station); //配置在线点就维护
            }
            return true;
        }
        private void AddOnLinePoint(bool isOnline,ref Station station)
        {
           // 附加在线状态点
           foreach(Sensor sensor in station.sensors)
            {
                if (sensor.dataSourceAddress == Sensor.stationOnlineState && sensor.type == PointType.Ignore)
                {
                    sensor.Value = isOnline==true?1:0;
                    sensor.State = ValueState.Success;
                    sensor.LastTime = DataUtil.ToDateString(DateTime.Now);
                    station._StationOnlieStateSensorID = sensor.sensorID;  //将来存储更新使用
                }
            }
        }
        private string GetSavePointsSQL(Dictionary<int, Station> dicStations)
        {
            if (dicStations == null || dicStations.Keys.Count == 0)
                return "";
            List<StaionValueBase> stationBases = new List<StaionValueBase>();
            foreach (int stationID in dicStations.Keys)
            {
                stationBases.Add(dicStations[stationID].ToStaionValueBase());
            }
            string sensorsRealSQL = "";
            string sensorsHisSQL = "";
            StaionDataOper.Instance.GetMulitStationRealSQL(stationBases.ToArray(), out sensorsRealSQL);
            if (DateTime.Now - lastSaveTime > TimeSpan.FromSeconds(Config.scadaConfig.PointsSaveInterVal * 60))
            {
                StaionDataOper.Instance.GetMulitStationHisSQL(stationBases.ToArray(), out sensorsHisSQL);
                lastSaveTime = DateTime.Now;
            }
            return sensorsRealSQL + sensorsHisSQL;
        }

        // 写值任务执行体
        public bool ExcuteWriteHandle(out string errMsg, out string info)
        {
            errMsg = "";
            info = "";
            // 参数检查
            if (!CheckParam(requestCommand.sensorID, out errMsg))
            {
                errMsg = "command参数异常：" + errMsg;
                return false;
            }
            // 加载数据库点信息
            ControlPoint point = LoadPointInfo(requestCommand.sensorID, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg) || point == null)
            {
                errMsg = "加载数据库点信息失败:" + errMsg;
                return false;
            }
            // 追加OPC客户端信息，设定值
            point.setValue = requestCommand.value;
            point.opc = this.opcClientManager.GetOPCClient(point.fOPCServerName);
            point.Value = point.setValue / point.scale;
            // 检查数据库点信息
            if (!point.CheckScadaControl(out errMsg))
            {
                errMsg = "检查数据库点信息失败:" + errMsg;
                return false;
            }
            // 执行设置
            string tagName = OpcDaClient.GetTagName(point.fOPCServerName, point.fOPCDeviceName, point.offsetAddress, point.dataSourceAddress);
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start(); //记录开始时间
            bool r = point.opc.Write(tagName, point.Value, out errMsg);
            watch.Stop(); //记录结束时间 
            string aa = "花了" + watch.Elapsed.Seconds + "秒" + watch.Elapsed.Milliseconds + "毫秒";
            if (!r)
                return false;
            info = tagName + "写入" + point.Value + "成功";
            return true;
        }
        private bool CheckParam(string sensorID, out string errMsg)
        {
            errMsg = "";
            if (string.IsNullOrWhiteSpace(sensorID))
            {
                errMsg = "sensorID不能为空";
                return false;
            }
            return true;
        }
        private ControlPoint LoadPointInfo(string sensorID, out string errMsg)
        {
            string sql = string.Format(@"select t.FOPCServerName,t.FOPCDeviceName,
                                         t1.PointAddressID,t2.数据源地址,t2.偏移地址,t2.倍率,t2.数据类型,t2.是否激活,t2.是否写入
                                        from SCADA_Station t,SCADA_Sensor t1,PointAddressEntry t2
                                        where t.ID=t1.StationID and t1.PointAddressID=t2.ID
                                        and t1.ID='{0}'",sensorID);
            DataTable dt = DBUtil.ExecuteDataTable(sql, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                errMsg = "加载控制点详细信息失败:" + errMsg;
                return null;
            }
            if (dt == null || dt.Rows.Count == 0)
            {
                errMsg = "加载控制点详细信息失败:" + "未查询sensorID" + sensorID+ " 的控制点信息" + "sql:" + sql;
                return null;
            }
            if (dt.Rows.Count > 1)
            {
                errMsg = "加载控制点详细信息失败,有多条信息匹配:" + "查询sensorID" + sensorID + " 的控制点信息有匹配信息,数据库数据异常" + "sql:" + sql; ;
                return null;
            }
            DataRow dr = dt.Rows[0];
            ControlPoint point = new ControlPoint()
            {
                fOPCDeviceName = DataUtil.ToString(dr["FOPCDeviceName"]),
                fOPCServerName = DataUtil.ToString(dr["FOPCServerName"]),
                pointID = DataUtil.ToInt(dr["PointAddressID"]),
                dataSourceAddress = DataUtil.ToString(dr["数据源地址"]),
                offsetAddress = DataUtil.ToString(dr["偏移地址"]),
                scale = DataUtil.ToDouble(dr["倍率"]),
                type = Point.ToType(DataUtil.ToString(dr["数据类型"])),
                isActive = DataUtil.ToInt(dr["是否激活"]),
                isWrite = DataUtil.ToInt(dr["是否写入"]),
            };
            return point;
        }

        // 主动采集任务执行体
        public bool ExcuteReloadHandle(out string errMsg)
        {
            errMsg = "";
            if (requestCommand == null)
            {
                errMsg = "接受到调度命令为对象,无法重载站点数据";
                return false;
            }
            if (requestCommand.operType != CommandOperType.ReLoadData)
            {
                errMsg = "SCADA-OPC错误的请求服务类型";
                return false;
            }
            if (!ExcuteCollectHandle(out errMsg))
            {
                errMsg = "OPC二级缓存器重新采集任务失败:" + errMsg;
                return false;
            }
            return true;
        }

        // 采集任务标记为放弃-用于做用户主动送过来的请求
        public void CollectTaskWriteCancleToken() 
        {
            if (this.type == StationCommandType.Collect)
                this.opcCommandType = OPCCommandType.Cancle;
        }
    }
}
