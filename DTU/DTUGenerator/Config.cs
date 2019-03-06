using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTUGenerator
{
    public class Config
    {
        // UI版本信息
        //版本  
        private static string _version = "V1.0.0";
        public static string Version { get { return _version; } }
        //最后更新日期
        private static string _versionLastTime = "2018-03-15";
        public static string VersionLastTime { get { return _versionLastTime; } }

        //// 服务版本信息--连接成功后从服务获取
        //public static string ServerVersion { get; set; }
        //public static string ServerVersionLastTime { get; set; }

        public static string ip;
        public static string serverName;
        public static string user;
        public static string password;
        public static string DBConnectString;

        public static string ScadaIp;
        public static string ScadaServerName;
        public static string ScadaUser;
        public static string ScadaPassword;
        public static string ScadaDBConnectString;

        public static int heartTime;
        public static int plcDataTime;
        public static int plcRefreshTime;

        public static string CreateConnStr()
        {
            DBConnectString = string.Format("server={0};database='{1}';User id={2};password={3};Integrated Security=false", ip, serverName, user, password);
            return DBConnectString;
        }

        public static string CreateScadaConnStr()
        {
            ScadaDBConnectString = string.Format("server={0};database='{1}';User id={2};password={3};Integrated Security=false", ScadaIp, ScadaServerName, ScadaUser, ScadaPassword);
            return DBConnectString;
        }

    }
}
