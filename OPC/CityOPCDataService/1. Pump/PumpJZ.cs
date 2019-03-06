using CityPublicClassLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace CityOPCDataService
{
    public class PumpJZ 
    {
        public string ID { get; set; }
        public int PointAddressID { get; set; }
        public string PName { get; set; }
        public string PumpJZArea { get; set; }
        public string FOPCDeviceName { get; set; }
        public string FOPCServerName { get; set; }
        public string FPLCIP { get; set; }     
        public Point[] points;
        public bool IsOnline { get; set; }

        public OpcDaClient opc;

        public PumpJZValueBase ToPumpJZValueBase()
        {
            return new PumpJZValueBase()
            {
                _ID = this.ID,
                pointsVersionID = this.PointAddressID,
                points = this.points,
                IsOnline = this.IsOnline
            };
        }

        public bool Check(out string errMsg)
        {
            errMsg = "";
            if (PointAddressID==0)
            {
                errMsg = "机组未指定点表版本";
                return false;
            }
            if (string.IsNullOrWhiteSpace(FOPCDeviceName))
            {
                errMsg = "未指定OPC服务中的设备名称";
                return false;
            }
            if (string.IsNullOrWhiteSpace(FOPCServerName))
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
}
