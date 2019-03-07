using CityLogService;
using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace JiangXiXingGuo
{
    public enum WEBSoapCommandType
    {
        CollectAndSaveScadaSensors,
        InitSensorRealData,
    }

    public class WEBSoapCommand
    {
        WEBSoapCommandType Type;
        Param param;

        public static WEBSoapCommand CreateCollectAndSaveScadaSensors(Param param)
        {
            return new WEBSoapCommand() { Type = WEBSoapCommandType.CollectAndSaveScadaSensors, param = param };
        }
        public static WEBSoapCommand CreateInitSensorRealData(Param param)
        {
            return new WEBSoapCommand() { Type = WEBSoapCommandType.InitSensorRealData, param = param };
        }

        private static DateTime lastSaveTime = DateTime.MinValue;

        public void Execute()
        {
            try
            {
                lock (this)  //会被多线程调用注意安全
                {
                    switch (Type)
                    {
                        case WEBSoapCommandType.CollectAndSaveScadaSensors:
                            CollectAndSaveSnesors();
                            break;
                        case WEBSoapCommandType.InitSensorRealData:
                            InitSensorRealData();
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                TraceManagerForProject.AppendErrMsg("执行 WEB-SOAP-兴国 数据库工作器失败：" + e.Message);
            }

        }
        private void CollectAndSaveSnesors()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间

            Dictionary<int, Station> dicStations = new Dictionary<int, Station>();
            #region 查询sensor表
            string sqlSensors = @" select t.ID stationID, t.GUID as stationCode,t.Name stationName,t1.ID sensorID,t1.Name sensorName 
                                   from SCADA_Station t ,SCADA_Sensor t1 where t.ID=t1.StationID and t.ReadMode='WEB_SOAP_XINGGUO' and IsNull(t1.是否删除,0)=0;";
            DataTable dtSensors = DBUtil.ExecuteDataTable(sqlSensors, out string errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                TraceManagerForProject.AppendErrMsg("查询sensor列表失败：" + errMsg);
                return;
            }
            foreach (DataRow dr in dtSensors.Rows)
            {
                Station station = new Station()
                {
                    _ID = DataUtil.ToInt(dr["stationID"]),
                    _GUID = DataUtil.ToString(dr["stationCode"]),
                    _Name = DataUtil.ToString(dr["stationName"]),
                    sensors = new List<Sensor>()
                };
                Sensor sensor = new Sensor()
                {
                    sensorID = DataUtil.ToString(dr["sensorID"]),
                    sensorName = DataUtil.ToString(dr["sensorName"])
                };

                if (dicStations.Keys.Contains(station._ID))
                    dicStations[station._ID].sensors.Add(sensor);
                else
                {
                    station.sensors.Add(sensor);
                    dicStations.Add(station._ID, station);
                }
            }
            if (dicStations.Keys.Count == 0)
            {
                TraceManagerForProject.AppendWarning("站点表没有读取模式:WEB_SOAP_XINGGUO的站点表");
                return;
            }
            #endregion

            #region 请求监测点数据并存入
            List<SoapData> list = GetSoapData();
            Collect(list, ref dicStations);
            string saveSQL = GetSaveSensorsSQL(dicStations);
            if (string.IsNullOrWhiteSpace(saveSQL))
            {
                TraceManagerForProject.AppendWarning(string.Format(@"采集WEB-SOAP-兴国 数量{0}获取存入数据库SQL失败,可能原因没有在线的站点", dicStations.Keys.Count));
                return;
            }
            DBUtil.ExecuteNonQuery(saveSQL, out string err);
            stopwatch.Stop(); //  停止监视
            TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
            double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数

            if (!string.IsNullOrWhiteSpace(err))
                TraceManagerForProject.AppendErrMsg("更新WEB-SOAP-兴国实时数据失败" + ",耗时:" + milliseconds.ToString() + "毫秒," + err);
            else
                TraceManagerForProject.AppendDebug("更新WEB-SOAP-兴国实时数据成功" + ",耗时:" + milliseconds.ToString() + "毫秒");
            #endregion
        }

        private List<SoapData> GetSoapData()
        {
            string url = "http://120.77.66.89:90/WebService1.asmx";
            HttpRequestUtil requestUtil = new HttpRequestUtil(ContentType.textXml);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(new Param().request);
            List<SoapData> data = new List<SoapData>();
            requestUtil.CreateSyncPostHttpRequest(url, stringBuilder, (successData) =>
            {
                data = GetSoapDataFromXML(successData);
                if (data.Count == 0)
                {
                    TraceManagerForProject.AppendErrMsg("获取WEB-SOAP-兴国数据接口失败:" + "返回的数据格式不正确");
                    data = null;
                    return;
                }
               else
                {
                    TraceManagerForProject.AppendDebug("获取WEB-SOAP-兴国数据接口成功");
                }
            }, (failData) => {
                data = null;
                TraceManagerForProject.AppendErrMsg("获取WEB-SOAP-兴国数据接口失败" + failData);
            }, (doErrorData) => {
                data = null;
                TraceManagerForProject.AppendErrMsg("处理WEB-SOAP-兴国数据接口失败" + doErrorData);
            });

            if (data == null)
                return null;
            return data;
        }

        private List<SoapData> GetSoapDataFromXML(string streamString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(streamString);
            string s = doc.DocumentElement["soap:Body"]["getXGInfoResponse"]["getXGInfoResult"]["diffgr:diffgram"].InnerXml;
            doc.LoadXml(s);
            XmlNode node = doc.SelectSingleNode("NewDataSet");
            List<SoapData> list = new List<SoapData>();
            foreach (XmlNode childNode in node.ChildNodes)
            {
                SoapData data = new SoapData();
                foreach (XmlNode sonNode in childNode.ChildNodes)
                {

                    switch (sonNode.Name)
                    {
                        case "名称":
                            data.名称 = sonNode.InnerText;
                            break;
                        case "id":
                            data.id = Convert.ToInt32(sonNode.InnerText);
                            break;
                        case "设备ID":
                            data.设备ID = Convert.ToInt32(sonNode.InnerText);
                            break;
                        case "记录时间":
                            data.记录时间 = Convert.ToDateTime(sonNode.InnerText);
                            break;
                        case "采集时间":
                            data.采集时间 = Convert.ToDateTime(sonNode.InnerText);
                            break;
                        case "设备状态":
                            data.设备状态 = sonNode.InnerText;
                            break;
                        case "通讯状态":
                            data.通讯状态 = sonNode.InnerText;
                            break;
                        case "数据来源":
                            data.数据来源 = sonNode.InnerText;
                            break;
                        case "压力":
                            data.压力 = Convert.ToDecimal(sonNode.InnerText);
                            break;
                        case "电池电压":
                            data.电池电压 = Convert.ToDecimal(sonNode.InnerText);
                            break;
                        case "信号强度":
                            data.信号强度 = Convert.ToDecimal(sonNode.InnerText);
                            break;
                        case "瞬时流量":
                            data.瞬时流量 = Convert.ToDecimal(sonNode.InnerText);
                            break;
                        case "正累计流量":
                            data.正累计流量 = Convert.ToDecimal(sonNode.InnerText);
                            break;
                        case "负累计流量":
                            data.负累计流量 = Convert.ToDecimal(sonNode.InnerText);
                            break;
                        case "净累计流量":
                            data.净累计流量 = Convert.ToDecimal(sonNode.InnerText);
                            break;
                        case "压力2":
                            data.压力2 = Convert.ToDecimal(sonNode.InnerText);
                            break;
                        case "电池电压报警":
                            data.电池电压报警 = sonNode.InnerText;
                            break;
                        case "压力下限报警":
                            data.压力下限报警 = sonNode.InnerText;
                            break;
                        case "压力上限报警":
                            data.压力上限报警 = sonNode.InnerText;
                            break;
                        case "泵1运行":
                            data.泵1运行 = sonNode.InnerText;
                            break;
                        case "泵1故障":
                            data.泵1故障 = sonNode.InnerText;
                            break;
                        case "泵2运行":
                            data.泵2运行 = sonNode.InnerText;
                            break;
                        case "泵2故障":
                            data.泵2故障 = sonNode.InnerText;
                            break;
                        case "浊度":
                            data.浊度 = Convert.ToDecimal(sonNode.InnerText);
                            break;
                        case "余氯":
                            data.余氯 = Convert.ToDecimal(sonNode.InnerText);
                            break;
                        case "门开关":
                            data.门开关 = sonNode.InnerText;
                            break;
                        case "浊度故障":
                            data.浊度故障 = sonNode.InnerText;
                            break;
                        case "浊度上限报警":
                            data.浊度上限报警 = sonNode.InnerText;
                            break;
                        case "浊度下限报警":
                            data.浊度下限报警 = sonNode.InnerText;
                            break;
                        case "余氯故障":
                            data.余氯故障 = sonNode.InnerText;
                            break;
                        case "余氯上限报警":
                            data.余氯上限报警 = sonNode.InnerText;
                            break;
                        case "余氯下限报警":
                            data.余氯下限报警 = sonNode.InnerText;
                            break;
                        case "水表通讯故障":
                            data.水表通讯故障 = sonNode.InnerText;
                            break;
                        case "浊度状态":
                            data.浊度状态 = sonNode.InnerText;
                            break;
                        case "余氯状态":
                            data.余氯状态 = sonNode.InnerText;
                            break;
                    }

                }
                list.Add(data);
            }
            return list;
        }

        private void Collect(List<SoapData> soapDatas, ref Dictionary<int, Station> dicStations)
        {
            if (soapDatas == null)
                return;
            if (dicStations == null)
                return;

            foreach (int key in dicStations.Keys)
            {
                Station station = dicStations[key];
                int errorTimes = 0; // 三个离线就认为其离线了
                foreach (Sensor sensor in dicStations[key].sensors)
                {
                    // 防止采集的点多了，错误消息炸了，每个都报出来了---直接让其离线
                    if (errorTimes >= 3)
                    {
                        TraceManagerForProject.AppendErrMsg("StationName：" + station._Name + "三个条目采集失败,已跳过该站点采集，请检查点表和数据源");
                        dicStations[key].IsOnline = false;
                        break;
                    }
                            
                    // 拿到数据源
                    SoapData[] curSoapDatas = soapDatas.Where(y => y.名称 == station._Name.ToUpper()).ToArray();// 注意转换大写在比较
                    if (curSoapDatas.Length == 0)
                    {
                        sensor.MakeFail("未在WEB-SOAP-兴国数据源中找到配置站点信息,站点编码:" + station._GUID);
                        TraceManagerForProject.AppendErrMsg("未在WEB-SOAP-兴国数据源中找到配置站点信息,站点编码:" + station._GUID);
                        errorTimes++;
                        continue;
                    }
                    object pointDataSource;
                    try
                    {
                        SoapData curSoapData = curSoapDatas[0];
                       
                        // 在拿到值
                        Type type = curSoapData.GetType(); //获取类型
                        System.Reflection.PropertyInfo propertyInfo = type.GetProperty(sensor.sensorName); //获取指定名称的属性
                        pointDataSource = propertyInfo.GetValue(curSoapData, null); //获取属性值
                    }
                    catch (Exception e)
                    {
                        string er = string.Format("未在WEB监测点数据源中找到配置站点信息:{0}找到点地址为:{1}的点,错误原因:{2}" + station._Name, sensor.sensorName, e.Message);
                        sensor.MakeFail(er);
                        TraceManagerForProject.AppendErrMsg(er);
                        errorTimes++;
                        continue;
                    }

                    // 根据数据源获取数据
                    sensor.ReadWEBSoap(pointDataSource);
                    //sensor.LastTime = tempTime;// 使用采集时间，不要用当前时间
                    //sensor.State = ValueState.Success;
                    station.IsOnline = true;

                    if (sensor.State == ValueState.Fail)
                    {
                        string er = string.Format("站点名称:{0},sensorName:{1},取值错误:{2}", station._Name, sensor.sensorName, sensor.Mess);
                        TraceManagerForProject.AppendErrMsg(er);
                        errorTimes++;
                        continue;
                    }
                    
                }
            }
        }

        private string GetSaveSensorsSQL(Dictionary<int, Station> dicStations)
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
            if (DateTime.Now - lastSaveTime > TimeSpan.FromSeconds(param.saveInterVal * 60))
            {
                StaionDataOper.Instance.GetMulitStationHisSQL(stationBases.ToArray(), out sensorsHisSQL);
                lastSaveTime = DateTime.Now;
            }
            return sensorsRealSQL + sensorsHisSQL;
        }

        // 实时表是否在线字段实时维护
        private void InitSensorRealData()
        {
            StaionDataOper.Instance.InitSCADASensorRealTime(out string errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                TraceManagerForProject.AppendErrMsg(errMsg);
        }
    }
}
