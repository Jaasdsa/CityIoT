using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTUPumpDataService
{
    class Config
    {
        // UI版本信息
        //版本  
        private static string _version = "V1.0.0";
        public static string Version { get { return _version; } }
        //最后更新日期
        private static string _versionLastTime = "2018-10-09";
        public static string VersionLastTime { get { return _versionLastTime; } }

        //// 服务版本信息--连接成功后从服务获取
        //public static string ServerVersion { get; set; }
        //public static string ServerVersionLastTime { get; set; }

        public static string ip;
        public static string serverName;
        public static string user;
        public static string password;
        public static string DBConnectString;

        public static string configFilePath;

        public static string CreateConnStr()
        {
            DBConnectString = string.Format("server={0};database='{1}';User id={2};password={3};Integrated Security=false", ip, serverName, user, password);
            return DBConnectString;
        }
    }
    // DTU服务配置
    class DTUDSCConfig
    {
        public static string ip;
        public static int port;
        public static string socketTypt;
        public static int timeoutNum;
        public static int dtuListUpdateTime;
        public static int dtuListConnectDBTime;
        public static int pointListConnectDBTime;
        public static int commandConnectDBTime;
    }

}
