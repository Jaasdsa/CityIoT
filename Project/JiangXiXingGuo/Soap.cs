using CityPublicClassLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiangXiXingGuo
{
    public class Param
    {
        public string request = "<?xml version=\"1.0\" encoding=\"utf - 8\"?><soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\"><soap12:Body><getXGInfo xmlns=\"http://tempuri.org/\" /></soap12:Body></soap12:Envelope>";
        public int saveInterVal = 5;
        public int collectInterval = 60;
    }
    public class SoapData
    {
        public string 名称 { get; set; }
        public int id { get; set; } 
        public int 设备ID { get; set; } 
        public DateTime 记录时间 { get; set; } 
        public DateTime 采集时间 { get; set; } 
        public string 设备状态 { get; set; } 
        public string 通讯状态 { get; set; }
        public string 数据来源 { get; set; }
        public decimal 压力 { get; set; }
        public decimal 电池电压 { get; set; }
        public decimal 信号强度 { get; set; }
        public decimal 瞬时流量 { get; set; }
        public decimal 正累计流量 { get; set; }
        public decimal 负累计流量 { get; set; }
        public decimal 净累计流量 { get; set; }
        public decimal 压力2 { get; set; }
        public string 电池电压报警 { get; set; }
        public string 压力下限报警 { get; set; }
        public string 压力上限报警 { get; set; }
        public string 泵1运行 { get; set; }
        public string 泵1故障 { get; set; }
        public string 泵2运行 { get; set; }
        public string 泵2故障 { get; set; }
        public decimal 浊度 { get; set; }
        public decimal 余氯 { get; set; }
        public string 门开关 { get; set; }
        public string 浊度故障 { get; set; }
        public string 浊度上限报警 { get; set; }
        public string 浊度下限报警 { get; set; }
        public string 余氯故障 { get; set; }
        public string 余氯上限报警 { get; set; }
        public string 余氯下限报警 { get; set; }
        public string 水表通讯故障 { get; set; }
        public string 浊度状态 { get; set; }
        public string 余氯状态 { get; set; }
    }

    class Station
    {
        public int _ID;
        public string _GUID;
        public string _Name;
        public bool IsOnline { get; set; }
        public List<Sensor> sensors;

        public StaionValueBase ToStaionValueBase()
        {
            List<SnesorValueBase> valueBases = new List<SnesorValueBase>();
            if (sensors == null)
                valueBases = null;
            else
            {
                foreach (Sensor sensor in sensors)
                {
                    valueBases.Add(sensor.ToSnesorValueBase());
                }
            }
            return new StaionValueBase()
            {
                _ID = this._ID,
                IsOnline = this.IsOnline,
                sensors = valueBases.ToArray()
            };
        }
    }
    class Sensor : Point
    {
        public string sensorID;
        public string sensorName;


        public SnesorValueBase ToSnesorValueBase()
        {
            return new SnesorValueBase()
            {
                _ID = this.sensorID,
                Value = this.Value,
                State = this.State,
                Mess = this.Mess,
                LastTime = this.LastTime,
            };
        }
    }
}
