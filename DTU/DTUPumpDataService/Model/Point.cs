using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTUPumpDataService
{
    public enum PointType
    {
        Bool,
        Ushort,
        Real,
        Group,
        Unkown,
    }
    public enum ValueState
    {
        Success,
        Fail
    }
    public class ValueBase
    {
        public double Value { get; set; }
        public ValueState State { get; set; }
        public string Mess { get; set; }
        public string LastTime { get; set; }
    }

    public class Point: ValueBase
    {
        public int ID;
        public int VersionID;
        public string Name;
        public string PLCAddress;
        public int offsetAddress;
        public string DBBSAddress;
        public PointType Type;
        public int Length;
        public int IsActive;
        public int IsSaveSCADA;
        public int IsWrite;
        public double Scale;

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
                default:
                    return PointType.Unkown;
            }
        }
        public  string ToTypeStr(PointType type)
        {
            switch (type)
            {
                case PointType.Ushort:
                    return "短整型";
                case PointType.Real:
                    return "浮点型";
                case PointType.Bool:
                    return "开关型";
                default:
                    return "未知的数据类型";
            }
        }

        public void ReadValForModRTU(byte[] buffer)
        {
           this.LastTime = DataUtil.ToDateString(DateTime.Now);
            // this.LastTime = DataUtil.ToDateString(Config.LastTime);
            ModbusRTUReader read = ToModbusRTUReader();
            read.Read(buffer);
            switch (read.state)
            {
                case ValueState.Success:
                    {
                        this.State = read.state;
                        this.Value = DataUtil.ToDouble(read.Value);
                    }
                    break;
                case ValueState.Fail:
                    {
                        MakeFail(read.mess);
                        return;
                    }
            }
        }

        private ModbusRTUReader ToModbusRTUReader()
        {
            ModbusRTUReader read = new ModbusRTUReader();
            read.Type = this.Type;
            read.Address = this.PLCAddress;
            read.Length = this.Length;
            read.Scale = this.Scale;
            read.offsetAddress = this.offsetAddress;
            return read;
        }
        private void MakeFail(string errMsg)
        {
            this.Value = 0;
            this.State = ValueState.Fail;
            this.Mess = errMsg;
        }
    }


}
