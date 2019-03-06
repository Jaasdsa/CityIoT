using CityPublicClassLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityOPCDataService
{
    class ControlPoint:Point
    {
        public OpcDaClient opc;
        public double setValue;
        public string fOPCDeviceName;
        public string fOPCServerName;

        public bool CheckPumpControl(out string errMsg)
        {
            errMsg = "";
            if (string.IsNullOrWhiteSpace(fOPCDeviceName))
            {
                errMsg = "机组未指定 OPC 服务中的设备名称";
                return false;
            }
            if (string.IsNullOrWhiteSpace(fOPCServerName))
            {
                errMsg = "未指定 OPC 服务名称";
                return false;
            }
            if (opc == null)
            {
                errMsg = "OPC 服务为空对象";
                return false;
            }
            if (isActive != 1)
            {
                errMsg = "没有激活的点";
                return false;
            }
            if (isWrite != 1)
            {
                errMsg = "没有写入权限的点";
                return false;
            }
            if (!CheckPumpOPC(out errMsg))
                return false;

            return CheckValue(out errMsg);
        }
        public bool CheckScadaControl(out string errMsg)
        {
            errMsg = "";
            if (string.IsNullOrWhiteSpace(fOPCDeviceName))
            {
                errMsg = "站点未指定OPC服务中的设备名称";
                return false;
            }
            if (string.IsNullOrWhiteSpace(fOPCServerName))
            {
                errMsg = "站点未指定OPC服务名称";
                return false;
            }
            if (opc == null)
            {
                errMsg = "OPC服务为空对象";
                return false;
            }
            if (isActive != 1)
            {
                errMsg = "没有激活的点";
                return false;
            }
            if (isWrite != 1)
            {
                errMsg = "没有写入权限的点";
                return false;
            }
            if (!CheckScadaOPC(out errMsg))
                return false;

            return CheckValue(out errMsg);
        }
        public bool CheckValue(out string errMsg)
        {
            errMsg = "";
            string valueStr = Value.ToString();
            switch (this.type)
            {
                case PointType.Bool:
                    {
                        if (!int.TryParse(valueStr, out int r) || (r != 1 && r != 0))
                        {
                            errMsg = "开关型的点只接受0/1数值";
                            return false;
                        }
                        return true;
                    }
                case PointType.Ushort:
                case PointType.Real:
                    {
                        if (!double.TryParse(valueStr, out double r))
                        {
                            errMsg = "设定值:" + valueStr + "必须为数值类型";
                            return false;
                        }
                        return true;
                    }
                default:
                    {
                        errMsg = "其它数值类型暂时不支持控制";
                        return false;
                    }
            }
        }
    }
}
