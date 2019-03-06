using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityUtils
{
    public class ConnectionItem
    {
        public string ip;
        public string database;
        public string user;
        public string password;
        public string saveTime;
    }

    public class JsonHelper
    {
        private static string historyFile = new DirectoryInfo(XMLHelper.CurSystemPath).Parent.FullName + @"\CityIoTBuild\Log\ConnHistory.json";
        public static List<ConnectionItem> connHistroyList = new List<ConnectionItem>();

        public static List<ConnectionItem> GetConnHistroy(out string errMsg)
        {
            errMsg = "";
            try
            {
                string strConn = "";

                if (File.Exists(historyFile))
                {
                    strConn = File.ReadAllText(historyFile);
                    if (!string.IsNullOrWhiteSpace(strConn))
                    {
                        connHistroyList = JsonConvert.DeserializeObject<List<ConnectionItem>>(strConn);
                        return connHistroyList;
                    }
                }
            }
            catch(Exception e)
            {
                errMsg = e.Message;
                return connHistroyList;
            }
            return connHistroyList;
        }
        
        public static bool SaveConnection(string ip, string dbName, string userName,string password, out string errMsg)
        {
            ip = ip.Trim();
            dbName = dbName.Trim();
            userName = userName.Trim();
            password = password.Trim();

            errMsg = "";
            try
            {
                ConnectionItem conn = new ConnectionItem();
                conn.ip = ip;
                conn.database = dbName;
                conn.user = userName;
                conn.password = password;
                conn.saveTime = DateTime.Now.ToString();

                string strConn = "";

                if (!File.Exists(historyFile))
                {
                    //新建Log文件夹
                    if (!Directory.Exists(new DirectoryInfo(XMLHelper.CurSystemPath).Parent.FullName + @"\CityIoTBuild\Log"))
                    {
                        Directory.CreateDirectory(new DirectoryInfo(XMLHelper.CurSystemPath).Parent.FullName + @"\CityIoTBuild\Log");
                    }
                    FileStream fs = new FileStream(historyFile, FileMode.Create, FileAccess.ReadWrite);
                    fs.Close();
                    List<ConnectionItem> connList = new List<ConnectionItem>();
                    connList.Add(conn);
                    strConn = JsonConvert.SerializeObject(connList, Formatting.Indented);
                    File.WriteAllText(historyFile, strConn);
                }
                else
                {
                    strConn = File.ReadAllText(historyFile);
                    List<ConnectionItem> connList = JsonConvert.DeserializeObject<List<ConnectionItem>>(strConn);
                    for (int i = 0; i < connList.Count; i++)
                    {
                        if (connList[i].ip == conn.ip && connList[i].database == conn.database &&
                            connList[i].user == conn.user && connList[i].password == conn.password)
                        {
                            connList.RemoveAt(i);
                        }
                    }
                    connList.Add(conn);
                    strConn = JsonConvert.SerializeObject(connList, Formatting.Indented);
                    File.WriteAllText(historyFile, strConn);
                }
            }
            catch(Exception e)
            {
                errMsg = e.Message;
                return false;
            }
            return true;
        }

        public static bool DelHistory(out string errMsg)
        {
            errMsg = "";
            try
            {
                File.Delete(historyFile);
            }
            catch(Exception e)
            {
                errMsg = e.Message;
                return false;
            }
           
            return true;
        }

        public static bool DelSingleHistory(string ip, string dbName, string userName, string password, out string errMsg)
        {
            connHistroyList = GetConnHistroy(out errMsg);

            ip = ip.Trim();
            dbName = dbName.Trim();
            userName = userName.Trim();
            password = password.Trim();

            try
            {
                for (int i = 0; i < connHistroyList.Count; i++)
                {
                    if (connHistroyList[i].ip == ip && connHistroyList[i].database == dbName &&
                        connHistroyList[i].user == userName && connHistroyList[i].password == password)
                    {
                        connHistroyList.RemoveAt(i);
                    }
                }

                File.Delete(historyFile);

                string strConn = JsonConvert.SerializeObject(connHistroyList, Formatting.Indented);
                File.WriteAllText(historyFile, strConn);
            }
            catch(Exception e)
            {
                errMsg = e.Message;
                return false;
            }
            return true;
        }
    }
}
