using CityUtils;
using CityWEBDataService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShangHaiQingPu
{
    class Config
    {
        private static DirectoryInfo info = new DirectoryInfo(XMLHelper.CurSystemPath);
        private static string rootPath = info.Parent.Parent.Parent.FullName;
        public static string projectConfigPath = rootPath + @"\ConfCenter\物联网数据服务中心\项目\上海青浦\projectConfig.xml";

        public static PandaParam param;

        public static int collectInterval;
        public static int saveInterVal; 

        public static string ip;
        public static string serverName;
        public static string user;
        public static string password;
        public static string DBConnectString;

        public static string CreateConnStr()
        {
            DBConnectString = string.Format("server={0};database='{1}';User id={2};password={3};Integrated Security=false", ip, serverName, user, password);
            return DBConnectString;
        }
    }
}
