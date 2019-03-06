using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityPublicClassLib
{
   public class PumpJZValueBase
    {
        public string _ID;
        public int pointsVersionID;
        public PointValueBase[] points;
        public bool IsOnline { get; set; }
    }
    public class PointValueBase : ValueBase
    {
        public string dbSAddress;
    }

    public class PumpJZDataOper
    {
        public static PumpJZDataOper Instance
        {
            get
            {
                return new PumpJZDataOper();
            }
        }

        /// <summary>
        /// 实时表是否在线字段实时维护
        /// </summary>
        public void InitPumpRealData(out string errrMsg)
        {
            errrMsg = "";
            // 从JZ表初始化实时表离线字段
            string sql = string.Format(@"merge into PumpRealData t
		                    using(select CONVERT(varchar(50),t.ID) BASEID from PumpJZ t,Pump t1 where t.PumpId=t1.ID and (t.是否删除=0 or t.是否删除 is null) and (t1.是否删除=0 or t1.是否删除 is null)) as s
		                    on t.BASEID=s.BASEID
		                    when not matched then 
		                    insert(FOnLine,FUpdateDate,BASEID)
		                    values(0,'{0}',s.BASEID);", DataUtil.ToDateString(DateTime.Now));
            DBUtil.ExecuteNonQuery(sql, out string err);
            if (!string.IsNullOrWhiteSpace(err))
            {
                errrMsg="初始化二供实时表失败:" + err;
                return;
            }
        }

        /// <summary>
        /// 根据带有实时值得机组数组信息输出实时表SQL
        /// </summary>
        /// <param name="jzs">带有点值集合的机组数组</param>
        /// <param name="pointsRealSQL">实时表mergeSQL</param>
        public void GetMulitJZRealSQL(PumpJZValueBase[] jzs, out string pointsRealSQL)
        {
            pointsRealSQL = "";
            string pointsOnlineRealSQL = "";
            string pointsNotOnlineRealSQL = "";

            // 先过滤出离线的机组只更新在线状态和更新时间，其它的值不动
            if (jzs == null || jzs.Length == 0)
                return;
            List<PumpJZValueBase> onLineJZs = new List<PumpJZValueBase>();
            List<PumpJZValueBase> notOnLineJZs = new List<PumpJZValueBase>();
            foreach (PumpJZValueBase jz in jzs)
            {
                if (jz.IsOnline)
                    onLineJZs.Add(jz);
                else
                    notOnLineJZs.Add(jz);
            }
            // 在线机组实时SQL
            Dictionary<int, List<PumpJZValueBase>> jzVersionCaches = GetGroupByJZVersion(onLineJZs.ToArray());
            if (jzVersionCaches != null)
            {
                // 相同类型点表机组shiyongmeige整个表
                foreach (int key in jzVersionCaches.Keys)
                {
                    // 相同点表类型的JZ才能使用整张表进行merge
                    GetMulitJZRealSQLHander(jzVersionCaches[key].ToArray(), out string pointsOnlineRealSQLCache);
                    pointsOnlineRealSQL += pointsOnlineRealSQLCache;
                }
            }
            // 离线机组实时SQL
            string notOnlintIDs = "";
            bool isFirst = true;
            foreach (PumpJZValueBase jz in notOnLineJZs)
            {
                if(isFirst)
                {
                    notOnlintIDs += "'"+jz._ID+"'";
                    isFirst = false;
                }
                else
                    notOnlintIDs += ",'"+jz._ID + "'"; 
            }
            string dt =DataUtil.ToDateString(DateTime.Now);
            if (!string.IsNullOrWhiteSpace(notOnlintIDs))
                pointsNotOnlineRealSQL = string.Format(@"update PumpRealData set FOnLine=0,FUpdateDate='{0}'  where BASEID in ({1})", dt, notOnlintIDs);
            pointsRealSQL = pointsOnlineRealSQL + pointsNotOnlineRealSQL;
        }
        /// <summary>
        /// 根据带有实时值得机组数组信息输出、历史表、历史天表SQL
        /// </summary>
        /// <param name="jzs">带有点值集合的机组数组</param>
        /// <param name="pointsHisSQL">历史表insertSQL</param>
        /// <param name="_LastUpdateDays">机组历史天表最后更新日期字典</param>
        /// <param name="pointsHisDaySQL">历史天表insertSQL</param>
        public void GetMulitJZHisSQL(PumpJZValueBase[] jzs, out string pointsHisSQL)
        {
            pointsHisSQL = "";
            // 先过滤出离线的机组只更新在线状态和更新时间
            if (jzs == null || jzs.Length == 0)
                return;
            List<PumpJZValueBase> onLineJZs = new List<PumpJZValueBase>();
            foreach (PumpJZValueBase jz in jzs)
            {
                if (jz.IsOnline)
                    onLineJZs.Add(jz);
            }
            // 只记录在线的机组
            Dictionary<int, List<PumpJZValueBase>> jzVersionCaches = GetGroupByJZVersion(onLineJZs.ToArray());
            if (jzVersionCaches == null)
                return;
            // 相同类型点表机组shiyongmeige整个表
            foreach (int key in jzVersionCaches.Keys)
            {
                // 相同点表类型的JZ才能使用整张表进行merge
                GetMulitJZHisSQLHander(jzVersionCaches[key].ToArray(), out string pointsHisSQLCache);
                pointsHisSQL += pointsHisSQLCache;
            }
        }

        //根据带有相同点表版本实时值得机组信息输出实时表SQL
        private void GetMulitJZRealSQLHander(PumpJZValueBase[] jzs, out string pointsRealSQL)
        {
            pointsRealSQL = "";

            if (jzs == null || jzs.Length == 0)
                return;

            string lastTime = DataUtil.ToDateString(DateTime.Now);
            bool first =false;

            string sSetVal = "";//, t.F40001=s.F40001 , t.F40002=s.F40002
            string sCol = "";//,F40001,F40002
            string sVal = "";//,s.F40001,s.F40002
            string pumpRealTab = "";

            foreach (PumpJZValueBase jz in jzs)
            {
                if (jz.points == null || jz.points.Length == 0)
                    continue;
                //if (!jz.IsOnline)
                //    continue;

                string sColnumAndVal = "";//,1 'F40001',2 'F40002'
                Dictionary<string, string> pointDBaddrCache = new Dictionary<string, string>();
                foreach (PointValueBase point in jz.points)
                {
                    if (pointDBaddrCache.Keys.Contains(point.dbSAddress))  // 如果缓存有这个业务地址，就把所有相同业务地址点拿出来取最大值为有效值，解决多个萝卜一个坑
                    {
                        PointValueBase[] ps = jz.points.Where(p => p.dbSAddress == point.dbSAddress).ToArray();
                        GetSamePointMaxValue(ps, out string sColnumAndValMaxCache, out string sValHisMaxCache);
                        pointDBaddrCache[point.dbSAddress] = sColnumAndValMaxCache;
                    }
                    else
                    {
                        GetPointColAndValueStr(point, out string sColnumAndValCache, out string sValHisCache);
                        pointDBaddrCache.Add(point.dbSAddress, sColnumAndValCache);
                        if (!first)
                        {    //列拼接，只要在第一行进入
                            sSetVal += string.Format(@",t.{0}=s.{0}", point.dbSAddress);
                            sCol += string.Format(@",{0}", point.dbSAddress);
                            sVal += string.Format(@",s.{0}", point.dbSAddress);
                        }
                    }
                }
                first=true;
                foreach(string key in pointDBaddrCache.Keys)
                {
                    sColnumAndVal += pointDBaddrCache[key];
                }

                // 准备一个机组部分实时值SQL
                string cacheReal = string.Format(@"select '{0}' BASEID, '{1}' FUpdateDate {2}
                                                    ", jz._ID, lastTime, sColnumAndVal);
                // 实时表
                if (string.IsNullOrWhiteSpace(pumpRealTab))
                    pumpRealTab += cacheReal;
                else
                    pumpRealTab += " union all " + cacheReal;
            }

            // 实时SQL
            if (!string.IsNullOrWhiteSpace(pumpRealTab))
            {
                pointsRealSQL = string.Format(@"merge into PumpRealData as t 
                                            using ({0}) as s 
                                            on t.BASEID=s.BASEID 
                                            when matched then update set t.FUpdateDate=s.FUpdateDate {1} 
                                            when not matched then insert (BASEID,FUpdateDate {2})  values (s.BASEID,s.FUpdateDate {3}); 
                                             ", pumpRealTab, sSetVal, sCol, sVal);
            }

        }
        //根据带有相同点表类型实时值得机组信息输出历史表
        private void GetMulitJZHisSQLHander(PumpJZValueBase[] jzs, out string pointsHisSQL)
        {
            pointsHisSQL = "";

            if (jzs == null || jzs.Length == 0)
                return;

            string lastTime = DataUtil.ToDateString(DateTime.Now);
            bool first = false;

            string sColHis = "";//,s.F40001,s.F40002
            string pumpHisvalues = "";
            foreach (PumpJZValueBase jz in jzs)
            {
                if (jz.points == null || jz.points.Length == 0)
                    continue;

                if (!jz.IsOnline)                // 只有在线的机组才存入历史
                    continue;

                string sValHis = "";//,1,2
                Dictionary<string, string> pointDBaddrCache = new Dictionary<string, string>();
                foreach (PointValueBase point in jz.points)
                {
                    if (pointDBaddrCache.Keys.Contains(point.dbSAddress))  // 如果缓存有这个业务地址，就把所有相同业务地址点拿出来取最大值为有效值，解决多个萝卜一个坑
                    {
                        PointValueBase[] ps = jz.points.Where(p => p.dbSAddress == point.dbSAddress).ToArray();
                        GetSamePointMaxValue(ps, out string sColnumAndValMaxCache, out string sValHisMaxCache);
                        pointDBaddrCache[point.dbSAddress] = sValHisMaxCache;
                    }
                    else
                    {
                        GetPointColAndValueStr(point, out string sColnumAndValCache, out string sValHisCache);
                        pointDBaddrCache.Add(point.dbSAddress, sValHisCache);
                        if (!first)
                        {     //列拼接，只要在第一行进入
                            if (point.dbSAddress != "FOnLine") // 历史表没有在线状态
                                sColHis += string.Format(@",{0}", point.dbSAddress);
                        }
                    }          
                }
                first = true;
                foreach (string key in pointDBaddrCache.Keys)
                {
                    sValHis += pointDBaddrCache[key];
                }
                string cacheHis = string.Format(@"('{0}','{1}' {2})
                                                    ", jz._ID, lastTime, sValHis);

                // 历史表
                if (string.IsNullOrWhiteSpace(pumpHisvalues))
                    pumpHisvalues += cacheHis;
                else
                    pumpHisvalues += "," + cacheHis;
            }

            // 历史表SQL
            if (!string.IsNullOrWhiteSpace(pumpHisvalues))
            {
                pointsHisSQL = string.Format(@"INSERT INTO PumpHisData (BASEID,FCreateDate {0}) VALUES {1}
                                            ", sColHis, pumpHisvalues);
            }
        }

        // 根据值拼接merge的部分SQL
        private void GetPointColAndValueStr(PointValueBase point, out string sColnumAndVal, out string sValHis)
        {
            sColnumAndVal = "";//,1 'F40001',2 'F40002'
            sValHis = "";//,1,2

            if (point.dbSAddress.ToUpper() == "TEMPTIME")   // 转日期型
            {
                if (point.Value != null)
                {
                    sColnumAndVal = string.Format(@",'{0}' {1}", DataUtil.ToDateString(point.Value), point.dbSAddress);
                    sValHis = string.Format(@",'{0}'", DataUtil.ToDateString(point.Value));
                }
                else
                {
                    sColnumAndVal = string.Format(@",null {0}", point.dbSAddress);
                    sValHis = string.Format(@",null");
                }
            }
            else if (point.dbSAddress.ToUpper() == "FONLINE") // 转整型
            {
                sColnumAndVal = string.Format(@",{0} {1}", DataUtil.ToInt(point.Value), point.dbSAddress);
            }
            else                                    //转浮点数
            {
                if (point.Value != null)
                {
                    sColnumAndVal = string.Format(@",{0} {1}", DataUtil.ToDouble(point.Value), point.dbSAddress);
                    sValHis = string.Format(@",{0}", DataUtil.ToDouble(point.Value));
                }
                else
                {
                    sColnumAndVal = string.Format(@",null {0}", point.dbSAddress);
                    sValHis = string.Format(@",null");
                }
            }
        }
        // 从相同业务地址点集合取出最大值的那个点，如果为字符串点返回第一点
        private void GetSamePointMaxValue(PointValueBase[] points, out string sColnumAndVal, out string sValHis)
        {
            sColnumAndVal = "";
            sValHis = "";
            if (points == null || points.Length == 0)
                return;
            if(points.Length==1)
            {
                GetPointColAndValueStr(points[0], out sColnumAndVal, out sValHis);
                return;
            }
            double valueCache=0;
            PointValueBase pointCache= points[0];// 防止第一个就是最大值
            foreach (PointValueBase point in points)
            {
                if(point.dbSAddress== "TEMPTIME")
                {
                    GetPointColAndValueStr(point, out  sColnumAndVal, out  sValHis);
                    return;
                }
                else
                {
                    // 其余都是数字类型
                    if (valueCache < DataUtil.ToDouble(point.Value))
                    {
                        valueCache = DataUtil.ToDouble(point.Value);
                        pointCache = point;// 最大值的点记下来
                    }
                }
            }
            GetPointColAndValueStr(pointCache, out sColnumAndVal, out sValHis);
        }
        // 将具有相同点表类型的机组分类装存
        private Dictionary<int, List<PumpJZValueBase>> GetGroupByJZVersion(PumpJZValueBase[] jzs)
        {
            if (jzs == null || jzs.Length == 0)
                return null;

            // 将相同点表类型的机组分组
            Dictionary<int, List<PumpJZValueBase>> jzVersionCaches = new Dictionary<int, List<PumpJZValueBase>>();
            foreach (PumpJZValueBase jz in jzs)
            {
                int pointVersionID = jz.pointsVersionID;
                if (jzVersionCaches.Keys.Contains(pointVersionID))
                    jzVersionCaches[pointVersionID].Add(jz);
                else
                    jzVersionCaches.Add(pointVersionID, new List<PumpJZValueBase>() { jz });
            }
            return jzVersionCaches;
        }
      
    }
}
