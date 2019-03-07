using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityIoTServiceWatcher
{
    public static class Config
    {
        //版本信息
        public static readonly string ServerVersion = "V2.4.0";
        public static readonly string ServerVersionLastTime = "2019-03-04 18:00:00";
        public static readonly string Version = ServerVersion;
        public static readonly string VersionLastTime = ServerVersionLastTime;

        public static EnvConfigInfo envConfigInfo = EnvConfigInfo.SingleInstanceForCS;

        /// <summary>
        /// 服务正在运行标识
        /// </summary>
        public static bool ServerIsRuning { get; set; }
    }

}
