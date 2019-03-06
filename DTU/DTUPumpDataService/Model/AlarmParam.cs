using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTUPumpDataService
{
    class AlarmParam
    {
        public int ParamID { get; set; }
        public int TimelyID { get; set; }

        public string PumpJZID { get; set; }

        public int FMarkerType { get; set; }
        public string Fkey { get; set; }
        public string FMsg { get; set; }
        public string FSetMsg { get; set; }

        public int FLev { get; set; }
        public int FIsDef { get; set; }
        public int FStatus { get; set; }
        public int FIsPhone { get; set; }

        public string FCreateDate { get; set; }
        public string BeginAlarmTime { get; set; }
        public string EndAlarmTime { get; set; }
        public string AlarmTime { get; set; }

        public bool Value { get; set; }
        public string LastTime { get; set; }
    }
}
