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
    enum PumpCommandType
    {
        Collect,
        Write,
        Reload,
    }
    enum OPCCommandType
    {
        Pending,
        Cancle,
    }
    class PumpCommand
    {
        PumpCommandType type;
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
        public static PumpCommand CreateCollectCommand(OpcClient opcClientManager)
        {
            return new PumpCommand() {
                type = PumpCommandType.Collect,
                opcClientManager=opcClientManager,
                opcCommandType=OPCCommandType.Pending,
            };
        } 
        /// <summary>
        /// 创建重载机组数据任务
        /// </summary>
        /// <param name="opcClientManager"></param>
        /// <param name="pointsSaveInterVal"></param>
        /// <returns></returns>
        public static PumpCommand CreateReloadCommand(OpcClient opcClientManager, RequestCommand requestCommand)
        {
            return new PumpCommand()
            {
                type = PumpCommandType.Reload,
                opcClientManager = opcClientManager,
                requestCommand= requestCommand,
                opcCommandType = OPCCommandType.Pending,
            };
        }
        /// <summary>
        /// 创建机组写值任务
        /// </summary>
        /// <param name="opcClientManager"></param>
        /// <param name="pointsSaveInterVal"></param>
        /// <returns></returns>
        public static PumpCommand CreateWriteCommand(OpcClient opcClientManager, RequestCommand requestCommand)
        {
            return new PumpCommand() 
            {
                type = PumpCommandType.Write,
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
                    case PumpCommandType.Collect:
                        {
                            if (opcCommandType == OPCCommandType.Cancle)  //已经在调度被标记放弃了，干要紧的任务
                                return;
                            timeoutObj.Do = ExcuteCollect;
                            bool bo = timeoutObj.DoWithTimeout(new TimeSpan(0, 0, 0, Config.pumpConfig.commandTimeoutSeconds));
                            if (bo)
                            {
                                TraceManagerForOPC.AppendWarning("机组定时采集任务执行超时");
                                GC.Collect();
                            }
                        }
                        break;
                    case PumpCommandType.Write:
                        {
                            timeoutObj.Do = ExcuteWrite;
                            bool bo = timeoutObj.DoWithTimeout(new TimeSpan(0, 0, 0, Config.pumpConfig.commandTimeoutSeconds));
                            if (bo)
                            {
                                CommandManager.MakeTimeout("机组：" + requestCommand.jzID + "--点业务地址：" + requestCommand.fDBAddress + "执行写值命令超时", ref requestCommand);
                                CommandManager.CompleteCommand(requestCommand);
                                TraceManagerForCommand.AppendWarning(requestCommand.message);
                                GC.Collect();
                            }
                        }
                        break;
                    case PumpCommandType.Reload:
                        {
                            timeoutObj.Do = ExcuteReload;
                            bool bo = timeoutObj.DoWithTimeout(new TimeSpan(0, 0, 0, Config.pumpConfig.commandTimeoutSeconds));
                            if (bo)
                            {
                                CommandManager.MakeTimeout("执行重载机组数据命令超时", ref requestCommand);
                                CommandManager.CompleteCommand(requestCommand);
                                TraceManagerForCommand.AppendWarning(requestCommand.message);
                                GC.Collect();
                            }
                        }
                        break;
                    default:
                        TraceManagerForOPC.AppendWarning("未知的OPC命令类型,无法执行");
                        return;
                }
            }
            catch(Exception e)
            {
                TraceManagerForOPC.AppendErrMsg("执行OPC任务失败,未知的任务类型:"+e.Message + "堆栈:" + e.StackTrace);
            }        
        }

        // 任务分发口
        public void ExcuteCollect()
        {
            try
            {
                if(!ExcuteCollectHandle(out string errMsg))
                    TraceManagerForOPC.AppendWarning("机组定时采集任务执行失败;" + errMsg);
            }
            catch (Exception e)
            {
                TraceManagerForOPC.AppendErrMsg("机组定时采集任务执行失败--" + e.Message + "堆栈:" + e.StackTrace);
            }
        }
        public void ExcuteWrite()
        {
            try
            {
                if (!ExcuteWriteHandle(out string errMsg,out string info))
                {
                    CommandManager.MakeFail(errMsg, ref requestCommand);
                    CommandManager.CompleteCommand(requestCommand);
                    TraceManagerForCommand.AppendWarning(requestCommand.message);
                    return;
                }
                CommandManager.MakeSuccess(info, ref requestCommand);
                CommandManager.CompleteCommand(requestCommand);
                TraceManagerForCommand.AppendInfo(info);
            }
            catch (Exception e)
            {
                TraceManagerForOPC.AppendErrMsg("机组写值任务执行失败--" + e.Message + "堆栈:" + e.StackTrace);
                SendError("机组写值任务执行失败--" + e.Message);
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
                CommandManager.MakeSuccess("重载机组数据命令成功,Pump-OPC数据已更新", ref requestCommand);
                CommandManager.CompleteCommand(requestCommand);
                TraceManagerForCommand.AppendInfo("重载机组数据命令成功,Pump-OPC数据已更新");
            }
            catch (Exception e)
            {
                TraceManagerForOPC.AppendErrMsg("机组主动采集任务执行失败--" + e.Message + "堆栈:" + e.StackTrace);
                SendError("机组主动采集任务执行失败--" + e.Message);
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间
            // 加载需要采集点
            if (!LoadCache(out List<PumpJZ> jzs, out errMsg))
                return false;
            if (jzs.Count == 0)
            {
                errMsg ="机组表没有读取模式为OPC的有效机组"; ;
                return false;
            }
            // 从OPC缓存中取数
            if (!Collect(ref jzs, out errMsg))
                return false;
            // 存入到数据库
            string saveSQL = GetSavePointsSQL(jzs);
            if (string.IsNullOrWhiteSpace(saveSQL))
            {
                errMsg= string.Format(@"采集OPC-机组数量{0}获取存入数据库SQL失败", jzs.Count);
                return false;
            }
            DBUtil.ExecuteNonQuery(saveSQL, out string err);
            stopwatch.Stop(); //  停止监视
            TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
            double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数

            if (!string.IsNullOrWhiteSpace(err))
            {
                errMsg = "更新OPC-二供实时数据失败" + ",耗时:" + milliseconds.ToString() + "毫秒," + err;
                return false;
            }
            TraceManagerForOPC.AppendDebug("更新OPC-二供实时数据成功" + ",耗时:" + milliseconds.ToString() + "毫秒");
            return true;
        }
        private bool LoadCache(out List<PumpJZ> jzs, out string err)
        {
            err = "";
            jzs = null;
            Dictionary<int, List<Point>> pointsCache = new Dictionary<int, List<Point>>();
            #region 查询点表
            string sqlPoints = @"select t.*,t1.数据业务地址,t1.名称 from PointAddressEntry t,PumpPointAddressDetail t1  where t.点明细ID=t1.ID and t.是否激活=1
                                     and t.版本ID in(select distinct PointAddressID  from PumpJZ x,pump x1 where PumpJZReadMode='OPC' and (x.是否删除=0 or x.是否删除 is null) and (x1.是否删除=0 or x1.是否删除 is null));";
            DataTable dtPoints = DBUtil.ExecuteDataTable(sqlPoints, out string errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                err = "查询二供点表版本失败：" + errMsg;
                return false;
            }
            foreach (DataRow dr in dtPoints.Rows)
            {
                int versionID = DataUtil.ToInt(dr["版本ID"]);
                Point point = new Point()
                {
                    pointID = DataUtil.ToInt(dr["ID"]),
                    versionID = versionID,
                    name = DataUtil.ToString(dr["名称"]),
                    dataSourceAddress = DataUtil.ToString(dr["数据源地址"]).Trim(),
                    offsetAddress = DataUtil.ToString(dr["偏移地址"]),
                    dbSAddress = DataUtil.ToString(dr["数据业务地址"]).Trim(),
                    type = Point.ToType(DataUtil.ToString(dr["数据类型"])),
                    isActive = DataUtil.ToInt(dr["是否激活"]),
                    isWrite = DataUtil.ToInt(dr["是否写入"]),
                    scale = DataUtil.ToDouble(dr["倍率"])
                };
                if (pointsCache.Keys.Contains(versionID))
                    pointsCache[versionID].Add(point);
                else
                    pointsCache.Add(versionID, new List<Point>() { point });
            }
            if (pointsCache.Keys.Count == 0)
            {
                err = "点表明细没有关于OPC的点表";
                return false;
            }
            #endregion

            jzs = new List<PumpJZ>();
            #region 查询JZ表
            string sqlJZ = @"select CONVERT(varchar(50),t.ID) as BASEID ,t.PointAddressID,t.PumpJZArea,t.FOPCDeviceName ,t.FOPCServerName,t.FPLCIP, t1.PName
                                from PumpJZ t,Pump t1 where t.PumpId=t1.ID and (t.是否删除=0 or t.是否删除 is null) and (t1.是否删除=0 or t1.是否删除 is null) and t.PumpJZReadMode='OPC' ;";
            DataTable dtJZIDs = DBUtil.ExecuteDataTable(sqlJZ, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                err = "查询二供机组ID列表失败：" + errMsg;
                return false;
            }
            foreach (DataRow dr in dtJZIDs.Rows)
            {
                int versionID = DataUtil.ToInt(dr["PointAddressID"]);
                if (!pointsCache.Keys.Contains(versionID))
                {
                    TraceManagerForOPC.AppendWarning("没有点表版本ID=" + versionID.ToString() + "的点表");
                    continue;// 此机组不采集，其他的正常采集
                }
                List<Point> points = pointsCache[versionID];

                // 准备点表
                Point[] pointsCopy = ByteUtil.DeepClone<List<Point>>(points).ToArray();// 一定要深度辅助一个副本，防止引用类型
                PumpJZ jz = new PumpJZ()
                {
                    ID = DataUtil.ToString(dr["BASEID"]),
                    PointAddressID = versionID,
                    PName = DataUtil.ToString(dr["PName"]),
                    PumpJZArea = DataUtil.ToString(dr["PumpJZArea"]),
                    FOPCDeviceName = DataUtil.ToString(dr["FOPCDeviceName"]),
                    FOPCServerName = DataUtil.ToString(dr["FOPCServerName"]),

                    points = pointsCopy
                };
                // jz加入OPC客户端对象
                jz.opc = this.opcClientManager.GetOPCClient(jz.FOPCServerName);
                if (jz.Check(out errMsg))
                {
                    jzs.Add(jz);
                }
                else
                    TraceManagerForOPC.AppendWarning("PumpJZID:" + jz.ID + "环境异常:" + errMsg);
            }
            #endregion

            return true; ;
        }
        private bool Collect(ref List<PumpJZ> jzs, out string errMsg)
        {
            errMsg = "";
            if (jzs == null)
            {
                errMsg = "无法采集空对象的机组";
                return false;
            }
            foreach (PumpJZ jz in jzs)
            {
                if (jz.opc == null || !jz.opc.IsOpcConnected)
                {
                    TraceManagerForOPC.AppendWarning(jz.PName + jz.PumpJZArea + "关联的OPCServer已经离线");
                    jz.IsOnline = false;
                    continue;
                }

                if (jz.points == null || jz.points.Length == 0)
                {
                    TraceManagerForOPC.AppendWarning(jz.PName + jz.PumpJZArea + "没有关联对应版本的点表");
                    jz.IsOnline = false;
                    continue;
                }

                //  加入离线、采集时间的点
                string lastTime = DataUtil.ToDateString(DateTime.Now);
                Point pOnline = new Point() { dbSAddress = "FOnLine", Value = jz.IsOnline == true ? 1 : 0, State = ValueState.Success, LastTime = lastTime };
                Point pTempTime = new Point() { dbSAddress = "TempTime", Value = lastTime, State = ValueState.Success, LastTime = lastTime };
                // 单点采集
                int errorTimes = 0; // n个失败就不采集
                int okayTimes = 0; // n个成功，就在线
                string errPointIDs = "";
                foreach (Point point in jz.points)
                {
                    // 防止采集的点多了，错误消息炸了，每个都报出来了---直接让机组离线
                    if (errorTimes >= Config.pumpConfig.errorTimes)
                    {
                        TraceManagerForOPC.AppendWarning(string.Format("{0}-{1}-pointID列表:{2}-采集失败,已跳过该机组采集，可能原因:机组离线或者被禁用采集", jz.PName, jz.PumpJZArea,errPointIDs));
                        jz.IsOnline = false;
                        break;
                    }
                    // 检查未通过
                    if (!point.CheckPumpOPC(out string err))
                    {
                        point.MakeFail(point.name + err);
                        TraceManagerForOPC.AppendWarning(point.name + "配置错误" + err);
                        continue;
                    }
                    string opcItemName = OpcDaClient.GetTagName(jz.FOPCServerName, jz.FOPCDeviceName, point.offsetAddress, point.dataSourceAddress);

                    // OPC 客户端缓存队列寻找该条目值
                    OpcTag tag = jz.opc.GetTagByName(opcItemName, out string er);
                    if (tag == null)
                    {
                        point.MakeFail(er);
                        TraceManagerForOPC.AppendWarning(er); // 未找到tag
                        continue;
                    }
                    if (!string.IsNullOrEmpty(tag.Error))
                    {
                        point.MakeFail(tag.Error);
                        errorTimes++;
                        errPointIDs += point.pointID.ToString()+" ";
                       // TraceManagerForOPC.AppendWarning(string.Format("{0}-{1}-{2}采集失败,错误原因:{3}", jz.PName, jz.PumpJZArea, point.name, point.Mess));
                        continue;
                    }
                    if (tag.Value == null)
                    {
                        point.MakeFail(opcItemName + "取值失败");
                        errorTimes++;
                        errPointIDs += point.pointID.ToString() + " ";
                        //  TraceManagerForOPC.AppendWarning(string.Format("{0}-{1}-{2}取值失败,错误原因:{3}", jz.PName, jz.PumpJZArea, point.name, point.Mess));
                        continue;
                    }
                    // 根据数据源获取数据
                    point.ReadOPCPoint(tag.Value);
                    if (point.State == ValueState.Fail)
                    {
                       // TraceManagerForOPC.AppendWarning(string.Format("{0}-{1}-{2}采集失败,取值错误:{3}", jz.PName, jz.PumpJZArea, point.name, point.Mess));
                        errorTimes++;
                        errPointIDs += point.pointID.ToString() + " ";
                        continue;
                    }
                    else
                        okayTimes++;
                }

                // 判断是否离线
                if (okayTimes >= Config.pumpConfig.okayTimes)
                    jz.IsOnline = true;
                else
                    jz.IsOnline = false;

                // 附加离线、时间点
                if (jz.IsOnline)
                    pOnline.Value = 1;
                else
                    pOnline.Value = 0;
                List<Point> points = new List<Point>();
                points.AddRange(new Point[] { pOnline, pTempTime });
                points.AddRange(jz.points);
                jz.points = points.ToArray();
            }
            return true;
        }
        private string GetSavePointsSQL(List<PumpJZ> jzs)
        {
            if (jzs == null || jzs.Count == 0)
                return "";
            List<PumpJZValueBase> jsBases = new List<PumpJZValueBase>();
            foreach (PumpJZ jz in jzs)
            {
                jsBases.Add(jz.ToPumpJZValueBase());
            }
            string pointsRealSQL = "";
            string pointsHisSQL = "";
            string pointsHisDaySQL = "";
            PumpJZDataOper.Instance.GetMulitJZRealSQL(jsBases.ToArray(), out pointsRealSQL);
            if (DateTime.Now - lastSaveTime > TimeSpan.FromSeconds(Config.pumpConfig.PointsSaveInterVal * 60))
            {
                PumpJZDataOper.Instance.GetMulitJZHisSQL(jsBases.ToArray(), out pointsHisSQL);
                lastSaveTime = DateTime.Now;
            }
            return pointsRealSQL + pointsHisSQL + pointsHisDaySQL;
        }

        // 写值任务执行体
        public bool ExcuteWriteHandle(out string errMsg,out string info)
        {
            errMsg = "";
            info="";
            // 参数检查
            if (!CheckParam(requestCommand.jzID, requestCommand.fDBAddress, out  errMsg))
            {
                errMsg = "command参数异常：" + errMsg;
                return false;
            }
            // 加载数据库点信息
            ControlPoint point = LoadPointInfo(requestCommand.jzID, requestCommand.fDBAddress, out errMsg);
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
            if (!point.CheckPumpControl(out errMsg))
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
        private bool CheckParam(int jzID, string fdbAdress, out string errMsg)
        {
            errMsg = "";
            if (jzID == 0)
            {
                errMsg = "机组ID未知";
                return false;
            }
            if (string.IsNullOrWhiteSpace(fdbAdress))
            {
                errMsg = "数据业务地址未知";
                return false;
            }
            return true;
        }
        private ControlPoint LoadPointInfo(int jziD, string fdbAdress, out string errMsg)
        {
            string sql = string.Format(@"select t1.PName,t2.PumpJZArea,t2.FOPCDeviceName,t2.FOPCServerName,t2.PointAddressID,
                                        t3.数据源地址,t3.偏移地址,t3.倍率,t3.数据类型,t3.是否激活,t3.是否写入,
                                        t4.数据业务地址 
                                        from Pump t1,PumpJZ t2,PointAddressEntry t3 ,PumpPointAddressDetail t4
                                        where (t1.是否删除=0 or t1.是否删除 is null) and (t2.是否删除=0 or t2.是否删除 is null) and t1.ID=t2.PumpId and t2.PointAddressID =t3.版本ID and t3.点明细ID=t4.ID
                                        and t2.ID={0} and t4.数据业务地址='{1}'", jziD, fdbAdress);
            DataTable dt = DBUtil.ExecuteDataTable(sql, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                errMsg = "加载控制点详细信息失败:" + errMsg;
                return null;
            }
            if (dt == null || dt.Rows.Count == 0)
            {
                errMsg = "加载控制点详细信息失败:" + "未查询JZID" + jziD + " 业务地址:" + fdbAdress + " 的控制点信息" + "sql:" + sql;
                return null;
            }
            if (dt.Rows.Count > 1)
            {
                errMsg = "加载控制点详细信息失败,有多条信息匹配:" + "查询JZID" + jziD + " 业务地址:" + fdbAdress + " 的控制点信息有匹配信息,数据库数据异常" + "sql:" + sql; ;
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
                dbSAddress = DataUtil.ToString(dr["数据业务地址"]),
            };
            return point;
        }

        // 主动采集任务执行体
        public bool ExcuteReloadHandle(out string errMsg)
        {
             errMsg = "";
            if (requestCommand == null)
            {
                errMsg = "接受到调度命令为对象,无法重载机组数据";
                return false;
            }
            if (requestCommand.operType != CommandOperType.ReLoadData)
            {
                errMsg = "PUMP-OPC错误的请求服务类型";
                return false;
            }
            if (!ExcuteCollectHandle(out  errMsg))
            {
                errMsg = "OPC二级缓存器重新采集任务失败:" + errMsg;
                return false;
            }
            return true;
        }

        // 采集任务标记为放弃-用于做用户主动送过来的请求
        public void CollectTaskWriteCancleToken()
        {
            if (this.type == PumpCommandType.Collect)
                this.opcCommandType = OPCCommandType.Cancle;
        }
    }
}
