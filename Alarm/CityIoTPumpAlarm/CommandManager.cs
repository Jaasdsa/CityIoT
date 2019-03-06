using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace  CityIoTPumpAlarm

{
    public enum DBCommandType
    {
        PumpAlarm,
        PumpJZOnlie,
    }

     class DBCommand
    {
        DBCommandType Type;

        public static DBCommand CreatePumpAlarmCommand()
        {
            return new DBCommand() { Type = DBCommandType.PumpAlarm };
        }
        public static DBCommand CreatePumpJZOnlineCommand(int timeout)
        {
            return new DBCommand() { Type = DBCommandType.PumpJZOnlie, timeout=timeout };
        }

        public int timeout;
        public void Execute()
        {
            switch (Type)
            {
                case DBCommandType.PumpAlarm:
                    RunPumpAlarm();
                    break;
                case DBCommandType.PumpJZOnlie:
                    RunPumpJZOnline();
                    break;
            }
        }
        private void RunPumpAlarm()
        {
            System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间

            #region 查询机组实时表
            string sqlPumpRealData = @"select t1.* from PumpRealData t1,Pump t2, PumpJZ t3 where t2.ID=t3.PumpId and (t2.是否删除=0 or t2.是否删除 is null) and (t3.是否删除=0 or t3.是否删除 is null)  and t1.BASEID= CONVERT(varchar(50), t3.ID);";
            DataTable dtRealData = DBUtil.ExecuteDataTable(sqlPumpRealData, out string errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                TraceManagerForPumpAlarm.AppendErrMsg("查询二供实时数据失败："+errMsg);
                return;
            }
            #endregion

            #region 查询报警参数表
            string sqlPumpAlarmParam = @"select t1.* from PumpAlarmParam t1,Pump t2, PumpJZ t3 where t2.ID=t3.PumpId and (t2.是否删除=0 or t2.是否删除 is null) and (t3.是否删除=0 or t3.是否删除 is null) and t1.是否启用=1 and t1.PumpJZID= CONVERT(varchar(50), t3.ID);";
            DataTable dtAlarmParam = DBUtil.ExecuteDataTable(sqlPumpAlarmParam, out  errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                TraceManagerForPumpAlarm.AppendErrMsg("查询二供报警参数失败：" + errMsg); 
                return;
            }
            List<PumpAlarmParam> alarmParams = new List<PumpAlarmParam>();
            foreach(DataRow dr in dtAlarmParam.Rows)
            {
                PumpAlarmParam pumpAlarmParam = new PumpAlarmParam()
                {
                    _ID = DataUtil.ToInt(dr["ID"]),
                    _FCreateDate = DataUtil.ToDateString(dr["FCreateDate"]).Trim(),
                    _StartTime = DataUtil.ToString(dr["StartTime"]).Trim(),
                    _EndTime = DataUtil.ToString(dr["EndTime"]).Trim(),
                    _PumpJZID = DataUtil.ToString(dr["PumpJZID"]).Trim(),
                    _Standard = DataUtil.ToDouble(dr["Standard"]),
                    _Standardlev = DataUtil.ToDouble(dr["Standardlev"]),
                    _IsUsed = DataUtil.ToInt(dr["是否启用"]),
                    _FKey = DataUtil.ToString(dr["FKey"]).Trim(),
                    _FType = DataUtil.ToString(dr["FType"]).Trim(),
                    _Unit = DataUtil.ToString(dr["Unit"]).Trim(),
                    _FMsg = DataUtil.ToString(dr["FMsg"]).Trim(),
                    _FLev = DataUtil.ToInt(dr["FLev"])
                };
                alarmParams.Add(pumpAlarmParam);
            }
            #endregion

            #region 查询实时报警表
            string sqlPumpAlarmTimely = @"select * from PumpAlarmTimely where AlarmOrWarn=1";
            DataTable dtAlarmTimely = DBUtil.ExecuteDataTable(sqlPumpAlarmTimely, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                TraceManagerForPumpAlarm.AppendErrMsg("查询二供实时报警失败：" + errMsg);
                return;
            }
            List<PumpAlarmTimely> alarmTimelys = new List<PumpAlarmTimely>();
            foreach (DataRow dr in dtAlarmTimely.Rows)
            {
                PumpAlarmTimely alarmTimely = new PumpAlarmTimely()
                {
                    _ID = DataUtil.ToInt(dr["ID"]),
                    _PumpJZID = DataUtil.ToString(dr["PumpJZID"]).Trim(),
                    _ParamID = DataUtil.ToInt(dr["ParamID"]),                
                    _FStatus = DataUtil.ToInt(dr["FStatus"]),
                    _FIsPhone = DataUtil.ToInt(dr["FIsPhone"]),
                    _BeginAlarmTime = DataUtil.ToDateString(dr["BeginAlarmTime"]).Trim(),
                    _UpdateAlarmTime = DataUtil.ToDateString(dr["UpdateAlarmTime"]).Trim(),
                    _AlarmOrWarn = DataUtil.ToInt(dr["AlarmOrWarn"]),
                    _Tips = DataUtil.ToString(dr["Tips"]).Trim(),
                    _Fvalue = DataUtil.ToString(dr["Fvalue"]).Trim()
                };
                alarmTimelys.Add(alarmTimely);
            }
            #endregion

            #region 执行报警维护SQL
            GetRealAlarmValues(dtRealData, ref alarmParams); // 得到报警参数里面的值

            string sql = GetHisAlarmSQL(alarmParams, alarmTimelys);
            string errHis="", errReal="";
            if (!string.IsNullOrWhiteSpace(sql))
            {
                DBUtil.ExecuteNonQuery(sql, out  errHis);
                if (!string.IsNullOrEmpty(errHis))
                    TraceManagerForPumpAlarm.AppendErrMsg("更新历史报警信息失败" + errHis);
            }
            sql = GetRealAlarmSQL(alarmParams);
            if (!string.IsNullOrWhiteSpace(sql))
            {
                DBUtil.ExecuteNonQuery(sql, out errReal);
                if (!string.IsNullOrEmpty(errReal))
                    TraceManagerForPumpAlarm.AppendErrMsg("更新实时报警信息失败" + errReal);
            }
            stopwatch.Stop(); //  停止监视
            TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
            double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数

            if (string.IsNullOrWhiteSpace(errHis) && string.IsNullOrWhiteSpace(errReal))
                TraceManagerForPumpAlarm.AppendDebug("已更新报警信息"+",耗时:"+ milliseconds.ToString()+"毫秒");

            //string sql = GetAlarmHisSQL(alarmParams, alarmTimelys) + GetAlarmRealSQL(alarmParams);
            //if (string.IsNullOrEmpty(sql))
            //{
            //    TraceManager.AppendErrMsg("未查询到相关报警点信息");
            //    return;
            //}
            //DBUtil.ExecuteNonQuery(sql, out string err);
            //if (!string.IsNullOrEmpty(err))
            //    TraceManager.AppendErrMsg("更新报警信息失败" + err);
            //else
            //    TraceManager.AppendDebug("已更新报警信息");
            #endregion

        }

        // 获取报警参数表对应的实时值
        private void GetRealAlarmValues(DataTable dtRealData,ref List<PumpAlarmParam> alarmParams)
        {
            foreach (PumpAlarmParam alarmParam in alarmParams)
            {
                // 是否启用
                if (alarmParam._IsUsed != 1)
                {
                    alarmParam.IsOkay = false;
                    continue;
                }

                // 判断报警类型
                if (alarmParam._FType != "阈值报警" && alarmParam._FType != "硬件报警")
                {
                    TraceManagerForPumpAlarm.AppendErrMsg(string.Format("报警参数表ID:{0}报警类型:{1}配置错误：",alarmParam._ID,alarmParam._FType ));
                    alarmParam.IsOkay = false;
                    continue;
                }

                // 报警点
                DataRow[] drs = dtRealData.Select(string.Format(@"BASEID = '{0}'", alarmParam._PumpJZID));
                if (drs.Length == 0)
                {
                    TraceManagerForPumpAlarm.AppendErrMsg(string.Format("报警参数表绑定的机组ID:{0}未查询到相关实时信息,请及时删除:", alarmParam._PumpJZID));
                    alarmParam.IsOkay = false;
                    continue;
                }
                try
                {
                    alarmParam.value = DataUtil.ToDouble(drs[0][alarmParam._FKey]);
                    alarmParam.lastTime = DataUtil.ToDateString(drs[0]["FUpdateDate"]);
                }
                catch (Exception e)
                {
                    TraceManagerForPumpAlarm.AppendErrMsg("匹配报警业务点实时值失败" + e.Message);
                    alarmParam.IsOkay = false;
                    continue;
                }

                alarmParam.IsOkay = true; //成功读取
            }
        }
        private bool IsAlarm(PumpAlarmParam alarmParam)
        {
            if (!alarmParam.IsOkay)
                return false;            

            DateTime lastTime;
            DateTime startTime;
            DateTime endtime;
            try
            {
                // 截取出时间部分，转成当天时间比较
                var time = DataUtil.ToDateTime(alarmParam.lastTime);
                lastTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, time.Hour, time.Minute, time.Second);
                startTime = Convert.ToDateTime(alarmParam._StartTime);
                endtime = Convert.ToDateTime(alarmParam._EndTime);
            }
            catch (Exception e)
            {
                TraceManagerForPumpAlarm.AppendErrMsg(string.Format("报警参数表ID：{0} 报警时间点设置错误", e.Message));
                return false;
            }
            if (DateTime.Compare(lastTime, startTime) >= 0 && DateTime.Compare(lastTime, endtime) <= 0)  // 时间判断
            {
                if(alarmParam._FType== "硬件报警")
                {
                    if (alarmParam.value == alarmParam._Standard)
                        return true;
                    else
                        return false;
                }
                else if(alarmParam._FType == "阈值报警")
                {
                    // 上阈值判断
                    if (alarmParam._Standardlev == 1 && alarmParam.value > alarmParam._Standard)
                        return true;
                    // 下阈值判断
                    if (alarmParam._Standardlev == 0 && alarmParam.value < alarmParam._Standard)
                        return true;
                }
            }
            return false;
        }
        // 获取实时报警SQL
        private string GetRealAlarmSQL(List<PumpAlarmParam> alarmParams)
        {
            string sql = string.Empty;
            if (alarmParams.Count == 0)
                return "";
            List<PumpAlarmParam> isAlarms = new List<PumpAlarmParam>();
            // 是否报警
            foreach (PumpAlarmParam alarmParam in alarmParams)
            {
                if (IsAlarm(alarmParam))
                    isAlarms.Add(alarmParam);
            }
            // 已经产生的报警拼接SQL
            if (isAlarms.Count == 0)
                return "";
            string sTimelyTab = "";
            const int FStatus = 0;
            const int FIsPhone = 0;
            const int alarmOrWarn = 1;
            //const string tips = "硬件故障";
            int i = 0;
            foreach (PumpAlarmParam alarm in isAlarms)
            {
                string cache = string.Format(@"select '{0}' PumpJZID ,{1} ParamID,{2} FStatus ,{3} FIsPhone ,'{4}' BeginAlarmTime,'{5}' UpdateAlarmTime, {6} AlarmOrWarn, '{7}' Tips, {8} Fvalue 
                                                ",
                                              alarm._PumpJZID, alarm._ID, FStatus, FIsPhone, alarm.lastTime, DataUtil.ToDateString(DateTime.Now), alarmOrWarn, alarm._FType, alarm.value);
                if (i == 0)
                    sTimelyTab += cache;
                else
                    sTimelyTab += " union all " + cache;
                i++;
            }
            string timelySQL = string.Format(@"merge into PumpAlarmTimely t
                                            using( {0} ) as s
                                            on t.ParamID=s.ParamID and t.PumpJZID=s.PumpJZID
                                            when matched then 
                                            update set t.UpdateAlarmTime=s.UpdateAlarmTime,t.Fvalue=s.Fvalue
                                            when not matched then 
                                            insert (PumpJZID,ParamID,FStatus,FIsPhone,BeginAlarmTime,UpdateAlarmTime,AlarmOrWarn,Tips,Fvalue)
                                            values (s.PumpJZID,s.ParamID,s.FStatus,s.FIsPhone,s.BeginAlarmTime,s.UpdateAlarmTime,s.AlarmOrWarn,s.Tips,s.Fvalue); ", 
                                            sTimelyTab);

            return timelySQL;
        }
        // 获取历史报警SQL
        private string GetHisAlarmSQL(List<PumpAlarmParam> alarmParams, List<PumpAlarmTimely> alarmTimelys)
        {
            if (alarmTimelys.Count == 0)
                return "";
            // 过滤报警消失的点
            string sqlClear = "";
            List<PumpAlarmTimely> IsnotAlarms = new List<PumpAlarmTimely>(); // 实时报警表报警消失的点集合
            foreach(PumpAlarmTimely timely in alarmTimelys)
            {
                // 附加模板信息 此时带有了实时值
                PumpAlarmParam[] paps = alarmParams.Where(p => p._ID == timely._ParamID).ToArray();
                if (paps.Count() > 0)
                    timely.alarmParam = paps[0];
                else
                {
                    sqlClear += @"delete PumpAlarmTimely where ID=" + timely._ID + "; ";  // 没有匹配到参数表直接删除，防止数据异常
                    continue;
                }


                if (!IsAlarm(timely.alarmParam))
                {
                    timely._EndAlarmTime = timely.alarmParam.lastTime;
                    timely._FAlarmTime = DataUtil.DateDiff(Convert.ToDateTime(timely._EndAlarmTime), Convert.ToDateTime(timely._BeginAlarmTime));
                    IsnotAlarms.Add(timely);
                }
            }
            // 拼接报警消失的SQL ，分为删除实时表，插入历史表
            if (IsnotAlarms.Count == 0)
                return sqlClear;
            string IdsStr = "";
            string alarmValsStr = "";
            int ii = 0;
            foreach (PumpAlarmTimely alarm in IsnotAlarms)
            {
                string alarmValsCache = string.Format(@"('{0}',{1},{2},{3},'{4}','{5}','{6}',{7},'{8}','{9}')",
                                                alarm._PumpJZID, alarm._ParamID, alarm._FStatus, alarm._FIsPhone, alarm._BeginAlarmTime, alarm._EndAlarmTime, alarm._FAlarmTime, alarm._AlarmOrWarn, alarm._Tips, alarm.alarmParam.value);
                if (ii == 0)
                    alarmValsStr += alarmValsCache;
                else
                    alarmValsStr += "," + alarmValsCache;

                IdsStr += "," + alarm._ID;
                ii++;
            }
            string insertHisSQl = string.Format(@"insert into PumpAlarmHistory (PumpJZID,ParamID,FStatus,FIsPhone,BeginAlarmTime,EndAlarmTime,FAlarmTime,AlarmOrWarn,Tips,Fvalue)
                                                 values {0}; ", alarmValsStr);
            string deleteRealSQL = string.Format(@"delete PumpAlarmTimely where ID in (0{0}); ", IdsStr);
            return sqlClear+insertHisSQl + deleteRealSQL;
        }

        // 实时表是否在线字段实时维护
        private void RunPumpJZOnline()
        {
            PumpJZDataOper.Instance.InitPumpRealData(out string errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                TraceManagerForPumpAlarm.AppendErrMsg(errMsg);
                return;
            }

            // 加载机组表信息
            string sql = @"select FOnLine,FUpdateDate,TempTime,BASEID from PumpRealData;";
            DataTable dt = DBUtil.ExecuteDataTable(sql, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                TraceManagerForPumpAlarm.AppendErrMsg("加载机组实时表信息失败:" + errMsg);
                return;
            }
            List<PumpJZOnline> jzs = new List<PumpJZOnline>();
            foreach(DataRow dr in dt.Rows)
            {
                PumpJZOnline jZOnline = new PumpJZOnline()
                {
                    _FOnLine = DataUtil.ToInt(dr["FOnLine"]),
                    _FUpdateDate = DataUtil.ToDateString(dr["FUpdateDate"]).Trim(),
                    _BASEID = DataUtil.ToString(dr["BASEID"]).Trim()
                };
                jzs.Add(jZOnline);
            }

            // 最后更新时间是否超时
            int i = 0;
            string tableSQL = "";
            bool isTimeout = false;
            foreach(PumpJZOnline jz in jzs)
            {
                isTimeout = false;// 复位
                if (jz._FOnLine == 0 && !string.IsNullOrWhiteSpace(jz._FUpdateDate) )
                    continue;  // 离线状态正常，不要更新到数据库

                if (string.IsNullOrWhiteSpace(jz._FUpdateDate) )
                {
                    jz._FOnLine = 0;
                    jz._FUpdateDate = DataUtil.ToDateString(DateTime.Now);
                    isTimeout = true;// 需要更新到库
                }
                if (DateTime.Now - DataUtil.ToDateTime(jz._FUpdateDate) > TimeSpan.FromMinutes(this.timeout))
                {
                    jz._FOnLine = 0;
                    isTimeout = true;// 需要更新到库
                }  

                if (jz._FOnLine == 0 && isTimeout==true) // 已超时，拼接SQL
                {
                    string cache = string.Format(@"select {0} FOnLine ,'{1}' FUpdateDate,'{2}' BASEID 
                                                    ",
                                                  jz._FOnLine, jz._FUpdateDate, jz._BASEID);
                    if (i == 0)
                    {
                        tableSQL += cache;
                        i++;
                    }                 
                    else
                        tableSQL += " union all " + cache;          
                }

           }
            // 更新状态到数据库
            if (string.IsNullOrWhiteSpace(tableSQL))
                return;
            string mergeSQL = string.Format(@"merge into PumpRealData t
                                            using( {0} ) as s
                                            on t.BASEID=s.BASEID 
                                            when matched then 
                                            update set t.FOnLine=s.FOnLine,t.FUpdateDate=s.FUpdateDate;
                                            ", tableSQL);
            DBUtil.ExecuteNonQuery(mergeSQL, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                TraceManagerForPumpAlarm.AppendErrMsg("二供实时表离线状态更新失败:" + errMsg);
                return;
            }
        }
    }

    class CommandManager
    {
        private System.Timers.Timer timer;
        private int alarmUpdateInterval;
        private int pumpJZTimeOut;

        public void Start()
        {
            if (IsRuning)
                return;

            LoadConfig();

            timer = new System.Timers.Timer();
            timer.Interval = alarmUpdateInterval * 1000;
            timer.Elapsed += (o, e) =>
            {
                try
                {
                    ReceiveCommand(new RequestCommand());
                }
                catch (Exception ee)
                {
                    TraceManagerForPumpAlarm.AppendErrMsg("二供报警定时任务执行失败:" + ee.Message);
                }
            };
            timer.Enabled = true;

            IsRuning = true;

            Action<RequestCommand> action = ReceiveCommand;
            action.BeginInvoke(new RequestCommand(), null, null);
        }
        public bool IsRuning { get; set; }
        public void Stop()
        {
            if (!IsRuning)
                return;

            // 关闭定时器
            if (timer != null)
            {
                timer.Enabled = false;
                timer.Close();
                timer = null;
            }

            IsRuning = false;
        }

        public void Excute()
        {
            // 离线状态维护
            DBCommand.CreatePumpJZOnlineCommand(pumpJZTimeOut).Execute();
            // 报警维护
            DBCommand.CreatePumpAlarmCommand().Execute();
        }

        public void ReceiveCommand(RequestCommand dispatchCommand)
        {
            if (dispatchCommand != null)
                Excute();
        }

        private void LoadConfig()
        {
            alarmUpdateInterval = Config.confPumpAlarm.UpdateInterval;
            pumpJZTimeOut = Config.confPumpAlarm.JZTimeOut;
        }
    }
}
