using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityPublicClassLib
{
    public enum PointType
    {
        Bool,
        Ushort,
        Real,
        Group,
        Unkown,
        String,
        BoolStr,
        Ignore,
    }
    public enum ValueState
    {
        Success,
        Fail
    }
    public class ValueBase
    {
        public object Value { get; set; }
        public ValueState State { get; set; }
        public string Mess { get; set; }
        public string LastTime { get; set; }

        public void MakeFail(string errMsg)
        {
            this.Value = 0;
            this.State = ValueState.Fail;
            this.Mess = errMsg;
            this.LastTime = DataUtil.ToDateString(DateTime.Now);
        }
    }

    public class Point : PointValueBase
    {
        public int pointID;
        public int versionID;
        public string name;
        public string dataSourceAddress;
        public string offsetAddress;
       // public string dbBSAddress;
        public PointType type;
        public int isActive;
        public int isWrite;
        public double scale;

        public int jzID;

        public static PointType ToType(string type)
        {
            switch (type)
            {
                case "短整型":
                    return PointType.Ushort;
                case "浮点型":
                    return PointType.Real;
                case "开关型":
                    return PointType.Bool;
                case "字符开关型":
                    return PointType.BoolStr;
                case "字符串":
                    return PointType.String;
                case "忽略型":
                    return PointType.Ignore;
                default:
                    return PointType.Unkown;
            }
        }
        public string ToTypeStr(PointType type)
        {
            switch (type)
            {
                case PointType.Ushort:
                    return "短整型";
                case PointType.Real:
                    return "浮点型";
                case PointType.Bool:
                    return "开关型";
                case PointType.BoolStr:
                    return "字符开关型";
                case PointType.Ignore:
                    return "忽略型";
                case PointType.String:
                    return "字符串";
                default:
                    return "未知的数据类型";
            }
        }

        public static void MakeFail(string errMsg,ref Point[] points)
        {
            foreach(Point p in points)
            {
                p.MakeFail(errMsg);
            }
        }

        // 点表检查
        public bool CheckPumpOPC(out string errMsg)
        {
            errMsg = "";
            if (string.IsNullOrEmpty(this.dataSourceAddress))
            {
                errMsg = "数据源地址不能为空";
                return false;
            }
            if (string.IsNullOrEmpty(this.dbSAddress))
            {
                errMsg = "数据业务地址不能为空";
                return false;
            }
            if (!DataUtil.CheckScale(this.scale))
            {
                errMsg = "寄存器地址:" + this.dataSourceAddress + "倍率错误";
                return false;
            }
            if (this.type == PointType.Unkown)
            {
                errMsg = "寄存器地址:" + this.dataSourceAddress + "数据类型未知";
                return false;
            }
            return true;
        }
        public bool CheckScadaOPC(out string errMsg)
        {
            errMsg = "";
            if (string.IsNullOrEmpty(this.dataSourceAddress))
            {
                errMsg = "数据源地址不能为空";
                return false;
            }
            if (!DataUtil.CheckScale(this.scale))
            {
                errMsg = "寄存器地址:" + this.dataSourceAddress + "倍率错误";
                return false;
            }
            if (this.type == PointType.Unkown)
            {
                errMsg = "寄存器地址:" + this.dataSourceAddress + "数据类型未知";
                return false;
            }
            return true;
        }
        public bool CheckPumpWeb(out string errMsg)
        {
            errMsg = "";
            if (string.IsNullOrEmpty(this.dataSourceAddress))
            {
                errMsg = "数据源地址不能为空";
                return false;
            }
            if (string.IsNullOrEmpty(this.dbSAddress))
            {
                errMsg = "数据业务地址不能为空";
                return false;
            }

            if (!DataUtil.CheckScale(this.scale))
            {
                errMsg = "寄存器地址:" + this.dataSourceAddress + "倍率错误";
                return false;
            }
            if (this.type == PointType.Unkown)
            {
                errMsg = "寄存器地址:" + this.dataSourceAddress + "数据类型未知";
                return false;
            }
            if (this.type == PointType.Bool)
            {
                if (!GetBoolAddressInfo(out errMsg, out int byteID, out int addrByte))
                    return false;
            }
            return true;
        }
        public bool CheckScadaWeb(out string errMsg) 
        {
            errMsg = "";
            if (string.IsNullOrEmpty(this.dataSourceAddress))
            {
                errMsg = "数据源地址不能为空";
                return false;
            }
            if (!DataUtil.CheckScale(this.scale))
            {
                errMsg = "寄存器地址:" + this.dataSourceAddress + "倍率错误";
                return false;
            }
            if (this.type == PointType.Unkown)
            {
                errMsg = "寄存器地址:" + this.dataSourceAddress + "数据类型未知";
                return false;
            }
            return true;
        }

        // 开关型偏移地址取出
        private bool GetBoolAddressInfo(out string errMsg,out int byteID,out int addrByte)
        {
            errMsg = "";
            byteID = 0;
            addrByte = 0;
            if (string.IsNullOrWhiteSpace(this.offsetAddress))
            {
                errMsg = "寄存器类型为开关型偏移地址不能为空,pointID为:" + this.pointID;
                return false;
            }
            string[] addrStrs = offsetAddress.Split('.');
            if (addrStrs.Length != 2)
            {
                errMsg = "寄存器类型为开关型偏移地址格式错误,pointID为:" + this.pointID;
                return false;
            }
            if (addrStrs[0] != "0" && addrStrs[0] != "1")
            {
                errMsg = "寄存器类型为开关型偏移地址格式错误,pointID为:" + this.pointID;
                return false;
            }
            if (addrStrs[1] != "0" && addrStrs[1] != "1" && addrStrs[1] != "2" && addrStrs[1] != "3" && addrStrs[1] != "4" && addrStrs[1] != "5" && addrStrs[1] != "6" && addrStrs[1] != "7")
            {
                errMsg = "寄存器类型为开关型偏移地址格式错误,pointID为:" + this.pointID;
                return false;
            }
            byteID =DataUtil.ToInt(addrStrs[0]);
            addrByte = DataUtil.ToInt(addrStrs[1]);
            return true;
        }
        private bool GetBoolStrAddressInfo(out string errMsg, out int index) 
        {
            errMsg = "";
            index = 0;
            if (string.IsNullOrWhiteSpace(this.offsetAddress))
            {
                errMsg = "寄存器类型为字符开关型偏移地址不能为空,ID为:" + this.pointID;
                return false;
            }
            bool r = int.TryParse(this.offsetAddress, out int result);
            if(r)
            {
                index = result;
                return true;
            }
            errMsg = "寄存器类型为字符开关型偏移地址转整型失败,ID为:" + this.pointID;
            return false;
        }
        private bool CheckPointDataSource(string pointDataSource,out string errMsg)
        {
            errMsg = "";
            if (string.IsNullOrWhiteSpace(pointDataSource))
            {
                errMsg = "点的数据源为空对象,ID为:" + this.pointID;
                return false;
            }
            if (pointDataSource.Length != 16)
            {
                errMsg = "点的数据源长度必须是16位开关值,ID为:" + this.pointID;
                return false;
            }
            for(int i = 0; i < 16; i++)
            {
                string r = pointDataSource[i].ToString();
                if (int.TryParse(r, out int result) && (result==1 || result==0))
                    continue;
                else
                {
                    errMsg = "点的数据源长度必须是16位开关值,ID为:" + this.pointID;
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 取值函数-取值之前应该调用Check方法检查
        /// </summary>
        /// <param name="pointDataSource"></param>
        /// <returns></returns>
        public void ReadWEBPoint(object pointDataSource)
        {
            this.State = ValueState.Success;
            this.LastTime = DataUtil.ToDateString(DateTime.Now);
            if (pointDataSource == null)
            {
                this.Value = null;
                return; // 数据源为null，直接返回
            }
            switch (this.type)
            {
                case PointType.String:
                    this.Value = pointDataSource.ToString();
                    break;
                case PointType.Ushort:
                    {
                        double response = 0;  //防止数据源给过来是"123.0"这样的数据
                        bool r = double.TryParse(Convert.ToString(pointDataSource), out response);
                        if (r)
                            this.Value = response;
                        else
                            this.MakeFail("PointID:" + this.pointID + "转整数型失败");
                    }
                    break;
                case PointType.Real:
                    {
                        double response = 0;
                        bool r = double.TryParse(Convert.ToString(pointDataSource), out response);
                        if (r)
                            this.Value = response * this.scale;
                        else
                            this.MakeFail("PointID:" + this.pointID + "转浮点数失败");
                    }
                    break;
                case PointType.Bool:
                    {
                        // 拿偏移地址
                        bool r = this.GetBoolAddressInfo(out string errMsg, out int byteID, out int addrByte);// 此方法之前已经验证过数据，不用再次验证
                        if (!r)
                        {
                            this.MakeFail(errMsg);
                            break;
                        }
                        int response = 0;
                        // 转数据源
                        r = int.TryParse(Convert.ToString(pointDataSource), out response);
                        if (r)
                            this.Value = response * this.scale;
                        else
                        {
                            this.MakeFail("PointID:" + this.pointID + "转浮点数失败");
                            break;
                        }
                        // 取值
                        ushort v = DataUtil.ToUshort(response);
                        this.Value = ByteUtil.GetBoolValue(v, byteID, addrByte) == true ? 1 : 0;
                    }
                    break;
                case PointType.BoolStr:
                    {
                        // 拿偏移地址
                        bool r = this.GetBoolStrAddressInfo(out string errMsg, out int index);// 此方法之前已经验证过数据，不用再次验证
                        if (!r)
                        {
                            this.MakeFail(errMsg);
                            break;
                        }
                        // 检查数据源                      
                        r = CheckPointDataSource(pointDataSource.ToString(), out errMsg);
                        if (!r)
                        {
                            this.MakeFail(errMsg);
                            break;
                        }
                        // 取值
                        this.Value = pointDataSource.ToString()[index].ToString()=="1"?1:0;
                    }
                    break;
                default:
                    this.MakeFail("点ID:" + this.pointID + "未知的数据类型");
                    break;
            }
        }
        public void ReadOPCPoint(object pointDataSource) 
        {
            this.State = ValueState.Success;
           // this.LastTime = DataUtil.ToDateString(DateTime.Now);  // 不要改时间
            if (pointDataSource == null)
            {
                this.Value = null;
                return; // 数据源为null，直接返回
            }
            if (pointDataSource.ToString().ToUpper() == "TRUE")
            {
                this.Value = 1;
                return;
            }
            if (pointDataSource.ToString().ToUpper() == "FALSE")
            {
                this.Value = 0;
                return;
            }

            switch (this.type)
            {
                case PointType.String:
                    this.Value = pointDataSource.ToString();
                    break;
                case PointType.Ushort:
                    {
                        double response = 0;  //防止数据源给过来是"123.0"这样的数据
                        bool r = double.TryParse(Convert.ToString(pointDataSource), out response);
                        if (r)
                            this.Value = response;
                        else
                            this.MakeFail("PointID:" + this.pointID + "转整数型失败");
                    }
                    break;
                case PointType.Real:
                    {
                        double response = 0;
                        bool r = double.TryParse(Convert.ToString(pointDataSource), out response);
                        if (r)
                            this.Value = response * this.scale;
                        else
                            this.MakeFail("PointID:" + this.pointID + "转浮点数失败");
                    }
                    break;
                case PointType.Bool:
                    {
                        if (pointDataSource.ToString().ToUpper() == "TRUE")
                            this.Value = 1;
                        else if (pointDataSource.ToString().ToUpper() == "FALSE")
                            this.Value = 0;
                        else
                            this.MakeFail("PointID:" + this.pointID + "开关型取值失败,数据源:"+pointDataSource.ToString());
                    }
                    break;
                default:
                    this.MakeFail("点ID:" + this.pointID + "未知的数据类型");
                    break;
            }
        }

        public void ReadWEBSoap(object pointDataSource)
        {
            this.State = ValueState.Success;
            this.LastTime = DataUtil.ToDateString(DateTime.Now);
            if (pointDataSource == null)
            {
                this.Value = null;
                return; // 数据源为null，直接返回
            }
            this.Value = pointDataSource.ToString();
        }

    }

}
