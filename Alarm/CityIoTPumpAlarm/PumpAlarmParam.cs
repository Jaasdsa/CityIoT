using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityIoTPumpAlarm
{
    public class Value
    {
        public double value;
        public string lastTime;
        public bool IsOkay = true;
    }
    public class PumpAlarmParam:Value
    {
        public int _ID;
        public string _FCreateDate;
        public string _StartTime;
        public string _EndTime;
        public string _PumpJZID;
        public double _Standard;
        public double _Standardlev;
        public int _IsUsed;
        public string _FKey;
        public string _FType;
        public string _Unit;
        public string _FMsg;
        public int _FLev; 
    }

    public class PumpAlarmTimely 
    {
        public int _ID;
        public string _PumpJZID;
        public int _ParamID;
        public int _FStatus;
        public int _FIsPhone;
        public string _BeginAlarmTime;
        public string _UpdateAlarmTime;
        public int _AlarmOrWarn;
        public string _Tips;
        public string _Fvalue;

        public PumpAlarmParam alarmParam;

        public string _EndAlarmTime;
        public string _FAlarmTime;
    }

    public class PumpAlarmHistory
    {
        public int _ID; 
        public string _PumpJZID;
        public int _ParamID;
        public int _FStatus;
        public int _FIsPhone;
        public string _BeginAlarmTime;
        public string _EndAlarmTime;
        public string _FAlarmTime; 
        public int _AlarmOrWarn;
        public string _Tips;
        public string _Fvalue;
    }



    public class PumpJZOnline
    {
        public int _FOnLine;
        public string _FUpdateDate;
        public string _BASEID;
    }
}
