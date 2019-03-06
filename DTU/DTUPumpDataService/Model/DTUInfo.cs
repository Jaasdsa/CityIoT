using CityUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace DTUPumpDataService
{
    public enum ProtocolType
    {
        UnKonwn,
        ModbusRTU,        
    }

    public class DTUInfo
    {
        public string ID { get; set; }
        public int DBID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FactoryName { get; set; }
        public string Model { get; set; }
        public string WorkType { get; set; }
        public int IsUsed { get; set; }
        public int IsActive { get; set; }
        public string ActiveTime { get; set; }
        public string LastRegisterTime { get; set; }
        public string TerminalIP { get; set; }
        public int TerminalPort { get; set; }
        public string GatewayIP { get; set; }
        public int GatewayPort { get; set; }
        public ProtocolType Protocol { get; set; }

        private string pumpAreaName { get; set; }
        private string JZID { get; set; }
        private int StationID { get; set; }
        private int VersionID { get; set; }
        private Point[] Points { get; set; }
        private AlarmParam[] Alarms { get; set; }
        private AlarmParam[] AlarmsTimely { get; set; }

        public static ProtocolType GetProtocol(string protocol)
        {
            protocol = protocol.Trim();
            switch (protocol)
            {
                case "ModbusRTU":
                    return ProtocolType.ModbusRTU;
                default:
                    return ProtocolType.UnKonwn;
            }
        }
        public string GetProtocolStr()
        {
            switch (this.Protocol)
            {
                case ProtocolType.ModbusRTU:
                    return "ModbusRTU";
                default:
                    return "未知的协议类型";

            }
        }

        public static DTUInfo ToDTUInfoForSoc(GPRS_USER_INFO gprsUserInfo)
        {
            try
            {
                DTUInfo dtuInfo = new DTUInfo();
                dtuInfo.ID = gprsUserInfo.m_userid;
                dtuInfo.IsUsed = 1;
                dtuInfo.IsActive = 1;
                dtuInfo.ActiveTime = DataUtil.ToDateString(gprsUserInfo.m_logon_date);
                dtuInfo.LastRegisterTime = DataUtil.ToDateString(HDDTUAPI.ConvertToDateString(gprsUserInfo.m_update_time));
                dtuInfo.TerminalIP = HDDTUAPI.inet_ntoa(HDDTUAPI.ntohl(gprsUserInfo.m_sin_addr));
                dtuInfo.TerminalPort = gprsUserInfo.m_sin_port;
                dtuInfo.GatewayIP = HDDTUAPI.inet_ntoa(HDDTUAPI.ntohl(gprsUserInfo.m_local_addr));
                dtuInfo.GatewayPort = gprsUserInfo.m_local_port;

                return dtuInfo;
            }
            catch
            {

            }
            return null;

        }

        public void Analysis(byte[] plcData, ushort length)
        {
            try
            {
                switch (this.Protocol)
                {
                    case ProtocolType.ModbusRTU:
                        {
                            byte[] validData = plcData.Take(length).ToArray();// 有效数据
                            if (!ModbusRTU.CheckData(validData, out string err))
                            {
                                TraceManager.AppendDebug(this.ID + "数据异常：" + err);
                                return;
                            }
                            if (validData.Length ==509)// 249 以前的版本
                                AnalysisAllModRTU(validData);
                            else
                                AnalysisResponseForModRTU(validData);
                        }
                        break;
                }
            }
            catch(Exception e)
            {
                TraceManager.AppendErrMsg(this.ID + "解析数据时出错：" + e.Message);
            }
        }

        // 解析所有
        private void AnalysisAllModRTU(byte[] validData)
        {
            byte[] headBytes = validData.Take(7).ToArray();
            if (ByteUtil.BytesToText(headBytes, headBytes.Length).Trim() != "02 10 00 00 00 78 F0")
                return;
            // PLC定时推送120个寄存器和数据长度240的数组         
            //减去前面7个字节数据描述，最后两个字节的CRC
            byte[] plcData = validData.Skip(7).Take(validData.Length - 9).ToArray();
            ushort plcAddress = ByteUtil.GetUshortValue(plcData, 0);
            this.GetPumpAreaName(plcAddress);
            // 加载点表信息
            if (!GetDBInfos(out string errMsg))
            {
                TraceManager.AppendErrMsg("查询DTU挂接点表集合失败:" + errMsg);
                return;
            }
            // 解析加载点表值
            GetValue(plcData);
            // 存储到数据库
            SaveDataToDB();
            // 清楚对象中点表等缓存,避免内存过大
            this.CLearDBInfos();
        }


        // 解析主动问答上来的数据
        private void AnalysisResponseForModRTU(byte[] validData)
        {

        }

        private void GetPumpAreaName(ushort plcAddress)
        {
            int r = DataUtil.ToInt(plcAddress);
            switch (r)
            {
                case 1:
                    this.pumpAreaName = "低区";
                    break;
                case 2:
                    this.pumpAreaName = "中区";
                    break;
                case 3:
                    this.pumpAreaName = "高区";
                    break;
                case 4:
                    this.pumpAreaName = "高高区";
                    break;
                default:
                    this.pumpAreaName = "不知道是啥子区";
                    break;
            }
        }
        private bool GetDBInfos(out string errMsg)
        {
            errMsg = "";
            string pointsSQL = string.Format(@"select c.ID,c.名称,c.PLC地址,c.偏移地址,c.数据业务地址,c.数据类型,c.数据长度,c.是否激活,c.倍率  ,b.ID JZID
                                            from DTUBase a,pumpJZ b,PointAddressEntry c
                                            where a.终端登录号码='{0}' and b.PumpJZReadMode='DTU' and  a.终端登录号码=b.DTUCode and b.PumpJZArea='{1}' and c.版本ID=b.PointAddressID and c.是否激活=1;",
                                            this.ID, this.pumpAreaName);
            //string sensorsSQL = string.Format(@"select d.ID,d.Name,d.Pump_PointID
            //                    from DTUBase a,pumpJZ b,PointAddressEntry c,Sensor d
            //                    where a.终端登录号码='{0}' and  a.终端登录号码=b.DTUCode and b.PumpJZArea='{1}' and c.ID=b.PointAddressID and d.Pump_JZID=b.ID;",
            //                    this.ID, this.pumpAreaName);
            string alarmSQL = "SELECT *  FROM PumpAlarmParam ;";
            string alarmTimelySQL = string.Format(@"select c.* from DTUBase a,PumpJZ b,PumpAlarmTimely c
                                                   where a.终端登录号码='{0}' and b.DTUCode=a.终端登录号码 and b.ID=c.PumpJZID;", this.ID);
            DataSet ds = DBUtil.GetDataSet(pointsSQL  + alarmSQL + alarmTimelySQL, out errMsg);
            if (!string.IsNullOrEmpty(errMsg))
                return false;
            DataTable dtPoints = ds.Tables[0];
           // DataTable dtSensors = ds.Tables[1];
            DataTable dtAlarms = ds.Tables[1];
            DataTable dtAlarmsTimely = ds.Tables[2];
            // 机组信息
            if (dtPoints.Rows.Count > 0)
                this.JZID = DataUtil.ToString(dtPoints.Rows[0]["JZID"]);
            // 点表信息
            List<Point> pointsCache = new List<Point>();
            foreach (DataRow dr in dtPoints.Rows)
            {
                Point point = new Point();
                point.ID = DataUtil.ToInt(dr["ID"]);
                point.Name = DataUtil.ToString(dr["名称"]);
                point.PLCAddress = DataUtil.ToString(dr["PLC地址"]);
                point.offsetAddress= DataUtil.ToInt(dr["偏移地址"]);
                point.DBBSAddress = DataUtil.ToString(dr["数据业务地址"]);
                point.Type = Point.ToType(DataUtil.ToString(dr["数据类型"]));
                point.Length = DataUtil.ToInt(dr["数据长度"]);
                point.IsActive = DataUtil.ToInt(dr["是否激活"]);
                point.Scale = DataUtil.ToDouble(dr["倍率"]);
                pointsCache.Add(point);
            }
            this.Points = pointsCache.ToArray();
            //// snesor信息
            //List<Sensor> sensorsCache = new List<Sensor>();
            //foreach (DataRow dr in dtSensors.Rows)
            //{
            //    Sensor sensor = new Sensor();
            //    sensor.ID = DataUtil.ToString(dr["ID"]);
            //    sensor.Name = DataUtil.ToString(dr["Name"]);
            //    sensor.PointID = DataUtil.ToInt(dr["Pump_PointID"]);
            //    sensorsCache.Add(sensor);
            //}
            //this.Sensors = sensorsCache.ToArray();
            // 报警信息
            List<AlarmParam> alarmCache = new List<AlarmParam>();
            foreach (DataRow dr in dtAlarms.Rows)
            {
                AlarmParam alarm = new AlarmParam();
                alarm.ParamID = DataUtil.ToInt(dr["ID"]);
                alarm.FMarkerType = DataUtil.ToInt(dr["FMarkerType"]);
                alarm.Fkey = DataUtil.ToString(dr["Fkey"]);
                alarm.FMsg = DataUtil.ToString(dr["FMsg"]);
                alarm.FLev = DataUtil.ToInt(dr["FLev"]);
                alarm.FIsDef = DataUtil.ToInt(dr["FIsDef"]);
                alarm.FCreateDate = DataUtil.ToString(dr["FCreateDate"]);
                alarmCache.Add(alarm);
            }
            this.Alarms = alarmCache.ToArray();
            // 报警实时表
            List<AlarmParam> alarmTimelyCache = new List<AlarmParam>();
            foreach (DataRow dr in dtAlarmsTimely.Rows)
            {
                AlarmParam alarm = new AlarmParam();
                alarm.ParamID = DataUtil.ToInt(dr["ParamID"]);
                alarm.TimelyID = DataUtil.ToInt(dr["ID"]);
                alarm.PumpJZID = DataUtil.ToString(dr["PumPJZID"]);
                alarm.FMarkerType = DataUtil.ToInt(dr["FMarkerType"]);
                alarm.Fkey = DataUtil.ToString(dr["Fkey"]);
                alarm.FMsg = DataUtil.ToString(dr["FMsg"]);
                alarm.FSetMsg = DataUtil.ToString(dr["FSetMsg"]);

                alarm.FLev = DataUtil.ToInt(dr["FLev"]);
                alarm.FStatus = DataUtil.ToInt(dr["FStatus"]);
                alarm.FIsPhone = DataUtil.ToInt(dr["FIsPhone"]);

                alarm.BeginAlarmTime = DataUtil.ToString(dr["BeginAlarmTime"]);
                alarm.LastTime = DataUtil.ToString(dr["UpdateAlarmTime"]);
                alarmTimelyCache.Add(alarm);
            }
            this.AlarmsTimely = alarmTimelyCache.FindAll(a=> a.PumpJZID==this.JZID).ToArray();
            return true;
        }
        private void CLearDBInfos()
        {
            this.pumpAreaName = null;
            this.JZID = null;
            this.StationID = 0;
            this.VersionID = 0;
            this.Points = null;
            //this.Sensors = null;
            this.Alarms = null;
            this.AlarmsTimely = null;
        }

        private void GetValue(byte[] plcData)
        {
            foreach (Point point in this.Points)
            {
                point.ReadValForModRTU(plcData);
                if (point.State == ValueState.Fail)
                {
                    point.Value = 0;
                    TraceManager.AppendErrMsg(this.ID + "下的" + point.PLCAddress + "解析出错:" + point.Mess);
                }
            }

            //foreach (Sensor sensor in this.Sensors)
            //{
            //    Point[] curPoints = this.Points.Where(p => p.ID == sensor.PointID).ToArray();
            //    if (curPoints == null || curPoints.Length != 1)
            //        continue;
            //    Point point = curPoints[0];
            //    sensor.Value = point.Value;
            //    sensor.State = point.State;
            //    sensor.Mess = point.Mess;
            //    sensor.LastTime = point.LastTime;
            //    sensor.FDate=Convert.ToDateTime(sensor.LastTime).ToString("yyyy-MM-dd"); ;
            //}
        }

        private void SaveDataToDB()
        {
            //if ((DateTime.Now - Config.LastTime).TotalDays <= 1)
            //    return;

            string sql = this.GetSavePointsSQL() + this.GetSaveAlarmsSQL();
            if (string.IsNullOrEmpty(sql))
            {
                TraceManager.AppendErrMsg(this.ID + "未查询到相关点表业务信息");
                return;
            }
            DBUtil.ExecuteNonQuery(sql, out string err);
            string str = this.Name == null ? this.ID : this.Name + "--" + this.pumpAreaName;
            if (!string.IsNullOrEmpty(err))
                TraceManager.AppendErrMsg(str + "保存PLC数据到数据库失败");
            //else
            //    TraceManager.AppendInfo(str + "--保存PLC数据到数据库成功");

            //string err = "";
            //DBUtil.ExecuteNonQuery(this.GetSavePointsSQL(), out err);
            //if (!string.IsNullOrEmpty(err))
            //    TraceManager.AppendErrMsg(this.ID + "保存PLC点表数据到数据库失败" + err);
            //DBUtil.ExecuteNonQuery(this.GetSaveSensorsSQL(), out err);
            //if (!string.IsNullOrEmpty(err))
            //    TraceManager.AppendErrMsg(this.ID + "保存PLC-SACDA数据到数据库失败" + err);
            //DBUtil.ExecuteNonQuery(this.GetSaveAlarmsSQL(), out err);
            //if (!string.IsNullOrEmpty(err))
            //    TraceManager.AppendErrMsg(this.ID + "保存PLC报警数据到数据库失败" + err);

        }
        private string GetSavePointsSQL()
        {
            if (this.Points == null || this.Points.Length == 0)
                return "";

            #region merge into更新插入模板
            //string ss = @"merge into PumpRealData as t
            //             using (select '13554097305' FDTUCode, 1 FOnline, CONVERT(varchar,GETDATE(),20) FUpdateDate, '0F11D588-F6F0-4F42-92BC-C45D3614E798' BASEID,1 'F40001',2 'F40002') as s
            //             on t.FDTUCode=s.FDTUCode 
            //             when matched then update set t.FOnline=s.FOnline,t.FUpdateDate=s.FUpdateDate, t.F40001=s.F40001 , t.F40002=s.F40002
            //             when not matched then insert (FDTUCode,FOnline,FUpdateDate,BASEID,F40001,F40002)  values (s.FDTUCode,s.FOnline,s.FUpdateDate,s.BASEID,s.F40001,s.F40002);";
            //string sss = @"INSERT INTO PumpHisData (FCreateDate, TempTime,BASEID) VALUES (值1, 值2,....)";
            #endregion

            string sColnumAndVal = "";//,1 'F40001',2 'F40002'
            string sSetVal = "";//, t.F40001=s.F40001 , t.F40002=s.F40002
            string sCol = "";//,F40001,F40002
            string sVal = "";//,s.F40001,s.F40002
            string sValHis = "";//,1,2

            foreach (Point point in this.Points)
            {
                sColnumAndVal += string.Format(@",{0}'{1}'", point.Value, point.DBBSAddress);
                sSetVal += string.Format(@",t.{0}=s.{0}", point.DBBSAddress);
                sCol += string.Format(@",{0}", point.DBBSAddress);
                sVal += string.Format(@",s.{0}", point.DBBSAddress);
                sValHis += string.Format(@",{0}", point.Value);
            }
            string lastTime = Points[0].LastTime;
            string pointsRealSQL = string.Format(@"merge into PumpRealData as t
                                    using (select '{0}' FDTUCode, 1 FOnline,'{6}' FUpdateDate,'{6}' TempTime, '{1}' BASEID {2}) as s
                                    on t.BASEID=s.BASEID 
                                    when matched then update set t.FOnline=s.FOnline,t.FUpdateDate=s.FUpdateDate,t.TempTime=s.TempTime {3}
                                    when not matched then insert (FOnline,FUpdateDate,TempTime,BASEID {4})  values (s.FOnline,s.FUpdateDate,s.TempTime,s.BASEID {5}); ",
                        this.ID, this.JZID, sColnumAndVal, sSetVal, sCol, sVal, lastTime);
            string pointsHisSQL = string.Format(@"INSERT INTO PumpHisData (FCreateDate, TempTime,BASEID{0}) VALUES 
                                            ( '{3}', '{3}','{1}'{2});", sCol, this.JZID, sValHis,lastTime);

            return pointsRealSQL + pointsHisSQL;
            //DBUtil.ExecuteNonQuery( pointsRealSQL + pointsHisSQL,out string err);
            //if (!string.IsNullOrEmpty(err))
            //    TraceManager.AppendDebug(this.ID + "保存PLC点表数据到数据库失败" + err);
        }
        private string GetSaveSensorsSQL()
        {
            //if (this.Sensors == null || this.Sensors.Length == 0)
            //    return "";
            //#region sensor metge into模板
            ////string ssss = @"  merge into SensorRealTime t
            ////          using (  
            ////           select 'testID1' SensorID, 1.11 LastValue, '2018-06-21 15:49:00' LastTime
            ////           union 
            ////           select 'testID2' SensorID, 2.2 LastValue,'2018-06-21 15:49:00' LastTime  
            ////             ) as s
            ////         on t.SensorID=s.SensorID
            ////         when matched then 
            ////         update set t.LastValue=s.LastValue,t.LastTime=s.LastTime
            ////         when not matched then
            ////         insert (SensorID,LastValue,LastTime) values (s.SensorID,s.LastValue,s.LastTime);";
            //#endregion
            //string sensorsTab = "";//select 'testID1' SensorID, 1.11 LastValue, '2018-06-21 15:49:00' LastTime union  select 'testID2' SensorID, 2.2 LastValue,'2018-06-21 15:49:00' LastTime
            //string sensorsHisVal = "";//('1',1.1,'2018-06-21 00:00:00'),('2',2.2,'2018-06-21 00:00:00')
            //int i = 0;
            //foreach (Sensor sensor in this.Sensors)
            //{
            //    string strCache = string.Format(@"select '{0}' SensorID, {1} LastValue, '{2}' LastTime", sensor.ID, sensor.Value, sensor.LastTime);
            //    string strValCache = string.Format(@"('{0}',{1},'{2}','{3}')", sensor.ID, sensor.Value, sensor.LastTime,sensor.FDate);
            //    if (i == 0)
            //    {
            //        sensorsTab += strCache;
            //        sensorsHisVal += strValCache;
            //    }
            //    else
            //    {
            //        sensorsTab += " union " + strCache;
            //        sensorsHisVal += " ," + strValCache;
            //    }
            //    i++;
            //}
            //string sensorsRealSQL = string.Format(@"merge into SensorRealTime t
            //                                  using ({0}) as s
            //                                 on t.SensorID=s.SensorID
            //                                 when matched then 
            //                                 update set t.LastValue=s.LastValue,t.LastTime=s.LastTime
            //                                 when not matched then
            //                                 insert (SensorID,LastValue,LastTime) values (s.SensorID,s.LastValue,s.LastTime);", sensorsTab);
            //string sensorsHisSQL = string.Format(@"INSERT INTO SensorHistory (SensorID, PV,PT,Date) VALUES {0}", sensorsHisVal);

            //return sensorsRealSQL + sensorsHisSQL;
            return "";
        }
        private string GetSaveAlarmsSQL()
        {
            return this.GetHisAlarmSQL() + this.GetRealAlarmSQL();
        }
        private string GetHisAlarmSQL()
        {
            if (this.AlarmsTimely == null || this.AlarmsTimely.Length == 0)
                return "";

            foreach (AlarmParam alarm in this.AlarmsTimely)
            {
                Point[] curPoints = this.Points.Where(p => p.DBBSAddress == alarm.Fkey).ToArray();
                if (curPoints == null || curPoints.Length != 1)
                    continue;
                Point point = curPoints[0];
                alarm.Value = DataUtil.ToInt(point.Value) == 1 ? true : false;
                alarm.LastTime = point.LastTime;
                if (alarm.Value == false)
                {
                    // 实时表报警消失
                    alarm.EndAlarmTime = alarm.LastTime;
                    alarm.AlarmTime = DataUtil.DateDiff(Convert.ToDateTime(alarm.EndAlarmTime), Convert.ToDateTime(alarm.BeginAlarmTime));
                }
            }
            AlarmParam[] alarmClearParams = this.AlarmsTimely.Where(a => a.Value == false).ToArray();
            if (alarmClearParams == null || alarmClearParams.Length == 0)
                return "";

            string IdsStr = "";
            string alarmValsStr = "";
            int ii = 0;
            foreach (AlarmParam alarm in alarmClearParams)
            {
                string alarmValsCache = string.Format(@"({0},'{1}',{2},'{3}','{4}','{5}',{6},{7},'{8}','{9}','{10}')",
                                                 alarm.ParamID, this.JZID, alarm.FMarkerType, alarm.Fkey, alarm.FMsg, alarm.FSetMsg, alarm.FLev, alarm.FIsPhone, alarm.BeginAlarmTime, alarm.EndAlarmTime, alarm.AlarmTime);
                if (ii == 0)
                    alarmValsStr += alarmValsCache;
                else
                    alarmValsStr += "," + alarmValsCache;

                IdsStr += "," + alarm.TimelyID;
                ii++;
            }
            string insertHisSQl = string.Format(@"insert into PumpAlarmHistory (ParamID,PumpJZID,FMarkerType,FKey,FMsg,FSetMsg,FLev,FIsPhone,FBeginAlarmTime,FEndAlarmTime,FAlarmTime)
                                                 values {0}; ", alarmValsStr);
            string deleteRealSQL = string.Format(@"delete PumpAlarmTimely where id in (0{0}); ", IdsStr);
            return insertHisSQl + deleteRealSQL;
        }
        private string GetRealAlarmSQL()
        {
            if (this.Alarms == null || this.Alarms.Length == 0)
                return "";
            foreach (AlarmParam alarm in this.Alarms)
            {
                Point[] curPoints = this.Points.Where(p => p.DBBSAddress == alarm.Fkey).ToArray();
                if (curPoints == null || curPoints.Length != 1)
                    continue;
                Point point = curPoints[0];
                alarm.Value = DataUtil.ToInt(point.Value) == 1 ? true : false;
                alarm.LastTime = point.LastTime;
            }


            AlarmParam[] alarmParams = this.Alarms.Where(a => a.Value == true).ToArray();
            if (alarmParams == null || alarmParams.Length == 0)
                return "";

            string sTimelyTab = "";//select 1 ParamID ,2 PumpJZID,'3' FKey ,'报警啦' FMsg, '报警啦' FSetMsg, 1 FLev,0 FStatus ,1 FIsPhone ,'2018-06-21 00:00:00' FAlamTime
            int FStatus = 0;
            int FIsPhone = 0;
            int i = 0;
            foreach (AlarmParam alarm in alarmParams)
            {
                string cache = string.Format(@"select {0} ParamID ,'{1}' PumpJZID,'{2}' FKey ,'{3}' FMsg, '{3}' FSetMsg, {4} FLev,{5} FStatus ,{6} FIsPhone ,'{7}' BeginAlarmTime,'{7}' UpdateAlarmTime",
                                              alarm.ParamID, this.JZID, alarm.Fkey, alarm.FMsg, alarm.FLev, FStatus, FIsPhone, alarm.LastTime);
                string cacheVal = string.Format(@"({0},'{1}','{2}','{3}','{3}',{4},{5},{6},'{7}','{7}')", alarm.ParamID, this.JZID, alarm.Fkey, alarm.FMsg, alarm.FLev, FStatus, FIsPhone, alarm.LastTime);
                if (i == 0)
                    sTimelyTab += cache;
                else
                    sTimelyTab += " union " + cache;
                i++;
            }
            string timelySQL = string.Format(@"merge into PumpAlarmTimely t
                                            using( {0} ) as s
                                            on t.ParamID=s.ParamID and t.PumpJZID=s.PumpJZID
                                            when matched then 
                                            update set t.UpdateAlarmTime=s.UpdateAlarmTime
                                            when not matched then 
                                            insert (ParamID,PumpJZID,FKey,FMsg,FSetMsg,FLev,FStatus,FIsPhone,BeginAlarmTime,UpdateAlarmTime)
                                            values (s.ParamID,s.PumpJZID,s.FKey,s.FMsg,s.FSetMsg,s.FLev,s.FStatus,s.FIsPhone,s.BeginAlarmTime,s.UpdateAlarmTime);
                                            ", sTimelyTab);

            return timelySQL;
            //// 有报警-更新和插入实时表操作
            // DBUtil.ExecuteNonQuery(timelySQL, out string err);
            // if (!string.IsNullOrEmpty(err))
            //     TraceManager.AppendDebug(this.ID + "保存PLC报警数据到数据库失败" + err);
            #region 报警 merge into模板
            //string ss = @"merge into AlarmTimely t
            //            using(
            //            select 1 ParamID ,2 PumpJZID,'3' FKey ,'报警啦' FMsg, '报警啦' FSetMsg, 1 FLev,0 FStatus ,1 FIsPhone,'2018-06-21 00:00:00' BeginAlarmTime ,'2018-06-21 00:00:00' UpdateAlarmTime
            //            union
            //            select 11 ParamID ,22 PumpJZID,'33' FKey ,'报警啦' FMsg, '报警啦' FSetMsg, 1 FLev,0 FStatus ,1 FIsPhone ,'2018-06-21 00:00:00' BeginAlarmTime ,'2018-06-21 00:00:00' UpdateAlarmTime
            //            ) as s
            //            on t.ParamID=s.ParamID and t.PumpJZID=s.PumpJZID
            //            when matched then 
            //            update set t.UpdateAlarmTime=s.UpdateAlarmTime
            //            when not matched then 
            //            insert (ParamID,PumpJZID,FKey,FMsg,FSetMsg,FLev,FStatus,FIsPhone,BeginAlarmTime,UpdateAlarmTime) values (1,2,'3','报警啦','报警啦',1,0,0,'2018-06-21 00:00:00','2018-06-21 00:00:00');";
            #endregion
        }

    }
}
