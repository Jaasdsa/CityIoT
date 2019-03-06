using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityPublicClassLib
{
    public class StaionValueBase
    {
        public int _ID;
        public bool IsOnline { get; set; }
        public string _StationOnlieStateSensorID { get; set; }
        public SnesorValueBase[] sensors;
    }

    public class SnesorValueBase : ValueBase
    {
        /// <summary>
        /// snesorID
        /// </summary>
        /// 
        public string _ID;
    }

    public class StaionDataOper
    {
        public static StaionDataOper Instance
        {
            get
            {
                return new StaionDataOper();
            }
        }

        public void InitSCADASensorRealTime(out string errrMsg)
        {
            errrMsg = "";
            // 从SCADA_Sensor表初始化SCADA_SensorRealTime表
            string sql = string.Format(@"merge into SCADA_SensorRealTime t
		                    using(select CONVERT(varchar(50),ID) SensorID from SCADA_Sensor) as s
		                    on t.SensorID=s.SensorID
		                    when not matched then 
		                    insert(SensorID,LastValue,LastTime)
		                    values(s.SensorID,0,'{0}');", DataUtil.ToDateString(DateTime.Now));
            DBUtil.ExecuteNonQuery(sql, out string err);
            if (!string.IsNullOrWhiteSpace(err))
            {
                errrMsg = "初始化SCADA_Sensor实时表失败:" + err;
                return;
            }
        }

        //sensor实时表刷新SQL
        public  void GetMulitStationRealSQL(StaionValueBase[] staions, out string realSQL)
        {
            // 将站点列表拆分为离线或在线分开获取对应SQL
            realSQL = "";
            List<StaionValueBase> onlineStations = new List<StaionValueBase>();
            List<StaionValueBase> offflineStations = new List<StaionValueBase>(); 
            foreach (StaionValueBase station in staions)
            {
                if (station.IsOnline)
                    onlineStations.Add(station);
                else
                    offflineStations.Add(station);
            }
            GetMulitOnlineStationRealSQL(onlineStations, out string realOnlineSQL);
            GetMulitOfflineStationRealSQL(onlineStations, out string realOfflineSQL);
            realSQL = realOnlineSQL + realOfflineSQL;
        } 
        private void GetMulitOnlineStationRealSQL(List<StaionValueBase> staions, out string realOnlineSQL)
        {
            realOnlineSQL = "";
            if (staions == null || staions.Count == 0)
                return;
            string tab = "";
            bool isFirst = false;
            foreach (StaionValueBase station in staions)
            {
                if (!station.IsOnline)
                    continue;
                foreach (SnesorValueBase sensor in station.sensors)
                {
                    double value = DataUtil.ToDouble(sensor.Value);
                    string tabCache = "";
                    tabCache = string.Format(@"select '{0}' SensorID, {1} LastValue, '{2}' LastTime
                                                ", sensor._ID, value, sensor.LastTime);
                    if (!isFirst)
                    {
                        tab += tabCache;
                        isFirst = true;
                    }
                    else
                        tab += " union all " + tabCache;
                }
            }
            if (!string.IsNullOrWhiteSpace(tab))
            {
                realOnlineSQL = string.Format(@"merge into SCADA_SensorRealTime as t
                                      using ({0}) as s
                                      on t.SensorID=s.SensorID
                                      when matched then update set t.LastValue=s.LastValue,t.LastTime=s.LastTime
                                      when not matched then insert (SensorID,LastValue,LastTime) values (s.SensorID,s.LastValue,s.LastTime);", tab);
            }
        }
        private void GetMulitOfflineStationRealSQL(List<StaionValueBase> staions, out string realOfflineSQL)  
        {
            realOfflineSQL = "";
            if (staions == null || staions.Count == 0)
                return;
            string notOnlintsensorIDs = ""; 
            bool isFirst = false;
            foreach (StaionValueBase station in staions)
            {
                if (station.IsOnline)
                    continue;
                if (string.IsNullOrWhiteSpace(station._StationOnlieStateSensorID) || station.IsOnline)  //没有在线状态点或者在线就算了
                    continue;
                if (isFirst)
                {
                    notOnlintsensorIDs += "'" + station._StationOnlieStateSensorID + "'";
                    isFirst = false;
                }
                else
                    notOnlintsensorIDs += ",'" + station._StationOnlieStateSensorID + "'";
            }
            string dt = DataUtil.ToDateString(DateTime.Now);
            if (!string.IsNullOrWhiteSpace(notOnlintsensorIDs))
                realOfflineSQL = string.Format(@"update SCADA_SensorRealTime set LastValue=0,LastTime='{0}'  where SensorID in ({1})", dt, notOnlintsensorIDs);
        }

        // sensor历史表刷新SQL
        public  void GetMulitStationHisSQL(StaionValueBase[] staions, out string hisSQL)
        {
            hisSQL = "";
            if (staions == null || staions.Length == 0)
                return;
            string tab = "";
            bool isFirst = false;
            foreach (StaionValueBase station in staions)
            {
                if (!station.IsOnline)
                    continue;
                foreach (SnesorValueBase sensor in station.sensors)
                {
                    double value = DataUtil.ToDouble(sensor.Value);
                    string tabCache = "";
                    tabCache = string.Format(@"select '{0}' SensorID, {1} PV, '{2}' PT, '{3}' Date
                                                ", sensor._ID, value, sensor.LastTime, DateTime.Now.ToString("yyyy-MM-dd"));
                    if (!isFirst)
                    {
                        tab += tabCache;
                        isFirst = true;
                    }
                    else
                        tab += " union all " + tabCache;
                }
            }
            if (string.IsNullOrWhiteSpace(tab))
                return;
            hisSQL = string.Format("insert into SCADA_SensorHistory(SensorID,PV,PT,Date) {0}", tab);
        }

        /// <summary>
        /// 将当前时间变成当天的最后一秒时间，如果为null，则返回1900-01-01 23:59:59
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private DateTime ToDayMaxTime(string dateTime)
        {
            if (string.IsNullOrWhiteSpace(dateTime))
                dateTime = "1900-01-01 23:59:59";
            DateTime dt = DataUtil.ToDateTime(dateTime);
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
        }
    }
}
