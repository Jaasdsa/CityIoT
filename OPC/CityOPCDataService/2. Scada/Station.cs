using CityPublicClassLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityOPCDataService
{
    class Station
    {
        public int _ID;
        public string _StationName;
        public string _FOPCDeviceName;
        public string _FOPCServerName { get; set; }
        public string _FPLCIP { get; set; }
        public bool IsOnline { get; set; }
        public string _StationOnlieStateSensorID { get; set; } 
        public List<Sensor> sensors;

        public OpcDaClient opc;
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
                _StationOnlieStateSensorID=this._StationOnlieStateSensorID,
                IsOnline = this.IsOnline,
                sensors = valueBases.ToArray()
            };
        }
        public bool Check(out string errMsg)
        {
            errMsg = "";
            if (string.IsNullOrWhiteSpace(_FOPCDeviceName))
            {
                errMsg = "未指定站点名称";
                return false;
            }
            if (string.IsNullOrWhiteSpace(_FOPCServerName))
            {
                errMsg = "未指定OPC服务名称";
                return false;
            }
            if (opc == null)
            {
                errMsg = "OPC服务为空对象";
                return false;
            }
            return true;
        }

    }
    class Sensor : Point
    {
        public const string stationOnlineState = "站点在线状态";

        public string sensorID;
        public string sensorName;
        public int _PointAddressID;

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

