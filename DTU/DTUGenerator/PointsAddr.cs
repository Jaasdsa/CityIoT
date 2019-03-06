using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTUGenerator
{
    public static class PointsAddr
    {
        // 泵状态
        public static string PLCAddr { get { return "F40001"; } }

        // 泵状态
        public static string P1State { get { return "F40026"; } }
        public static string P2State { get { return "F40027"; } }
        public static string P3State { get { return "F40028"; } }

        // 泵运行时间
        public static string P1Time { get { return "F40044"; } }
        public static string P2Time { get { return "F40045"; } }
        public static string P3Time { get { return "F40046"; } }

        // 排水泵状态
        public static string PSP1State { get { return "F40056"; } }
        public static string PSP2State { get { return "F40057"; } }


        // 进水流量压力
        public static string JinShiYa { get { return "F40009"; } } 
        public static string JinShun { get { return "F40010"; } }
        public static string JinLei { get { return "F40011"; } }

        // 出水流量压力累计
        public static string ChuShiYa { get { return "F40014"; } }
        public static string ChuSheYa { get { return "F40015"; } }
        public static string ChuShun { get { return "F40016"; } }
        public static string ChuLei { get { return "F40017"; } }

        // 变频器功率
        public static string Bian1P { get { return "F40088"; } }
        public static string Bian2P { get { return "F40089"; } }
        public static string Bian3P { get { return "F40090"; } }

        // 变频器电流
        public static string Bian1A { get { return "F40070"; } }
        public static string Bian2A { get { return "F40071"; } }
        public static string Bian3A { get { return "F40072"; } }

        // 累计电量
        public static string LJDianLiang { get { return "F40137"; } }

        // 报警模拟
        public static string Baojing { get { return "F40006"; } } 

    }
}
