
using CityIoTCommand;
using CityIoTCore;
using CityIoTPumpAlarm;
using CityIoTServiceManager;
using CityOPCDataService;
using CityPublicClassLib;
using CityUtils;
using CityWEBDataService;
using JiangXiXingGuo;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TestDemo 
{
    class Program
    {
        static void Main(string[] args)
        {

            bool a = DateTime.TryParse("00:00:10", out DateTime dt);

            int s1 = dt.Hour;
            int s2 = dt.Minute;
            int s3 = dt.Second;

            //TestSOAP();
            Test_core();
            //Console.ReadLine();
        }

        // 测试OPC
        public static void Test_CityOPCDataService()
        {
            OpcDaClient client = new OpcDaClient("Kepware.KepServerEX.V6");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            client.Start();

            Console.WriteLine("已启动, 耗时: {0} ms", stopWatch.ElapsedMilliseconds);
            stopWatch.Restart();

            client.Write("OPCUA.设备 1.翻身河泵站翻身河泵站.泵站.设备数据.开关型测试点", true, out string message1);
            client.Write("OPCUA.设备 1.翻身河泵站翻身河泵站.泵站.设备数据.实数型测试点", 88, out string message2);

            // OPCUA.设备 1.故障通道测试
            client.Write("OPCUA.设备 1.故障通道测试.故障设备.400001", 1, out string message3);
            client.Write("OPCUA.设备 1.故障通道测试.故障设备.400002", 2, out string message4);
            client.Write("OPCUA.设备 1.故障通道测试.故障设备.400003", 3, out string message5);

            Console.WriteLine("写入耗时: {0} ms", stopWatch.ElapsedMilliseconds);
            stopWatch.Restart();

            Console.ReadLine();

            client.Stop();
        }

        public static void Test_core()
        {
            //Console.WriteLine("启动服务输入y,停止服务任意输入");
            //string r = Console.ReadLine();
            //if (r != "Y" && r != "y")
            //    return;
            IoTServiceCore core = new IoTServiceCore();
            core.Start();

            if (core.IsRuning)
                Console.WriteLine("服务都已启动！");
            else
                Console.WriteLine("服务启动失败");
            Console.ReadLine();
        }

        public static void Test_InstallServer()
        {
            //ServiceControl control = new ServiceControl();
            //ServiceManager manager = new ServiceManager(EnvType.CS);
            //string path = ServiceManager.iotServerPath;
            ////string serverName = ServiceManager.serverName;
            //string serverName = "PandaCityIoTServiceIOT";
            //control.Register(serverName,path);
            //control.UnInstallService(serverName, path);
          //  PandaCityIoTService
        }

        public static void TestOPC()
        {
            OpcDaClient oPC = new OpcDaClient("Kepware.KepServerEX.V6");
            oPC.Start();
            //oPC.TestOPCALL();
        }

        private static bool ReadyLogEnv()
        {
            string filePath = XMLHelper.CurSystemPath + @"..\Log\CityIoTServerLog.db";
            string tableName = "Log";

            // 判断日志文件是否存在
            if (!File.Exists(filePath))
            {
                if (!SQLiteHelper.CreateDB(filePath))
                    return false; ;
            }

            // 根据日志表存在情况创建表，并加上时间的索引
            string dt = DataUtil.ToDateString(DateTime.Now);
            SQLiteHelper sQLite = new SQLiteHelper(filePath);
            if (!sQLite.ExistTable(tableName))
            {
                string creatTable = string.Format(@"CREATE TABLE if not exists  {0} (
                                        ID  integer  PRIMARY KEY autoincrement,
                                        类型   VARCHAR (50),
                                        信息文本 TEXT,
                                        插入时间 DATETIME,
                                        系统名称 VARCHAR (50) 
                                    );
                                    CREATE  INDEX 'Index_LogTime' on {0} ('插入时间' DESC);
                                    insert into {0} (类型,信息文本,插入时间,系统名称) values ('信息','创建数据库日志表成功','{1}','系统')", tableName, dt);
                sQLite.ExecuteNonQuery(creatTable);
            }

            // 检查表是否创建成功
            if (!sQLite.ExistTable(tableName))
                return false;

            return true;
        }

        public static void TestReadKey()
        {
            while (true)
            {
                string key = Console.ReadLine();
                if (key == "add")
                {
                    ii++;
                    queue.Add(new RequestCommand() { ID = ii.ToString() });
                    Console.WriteLine("当前添加：" + ii.ToString());
                    Console.WriteLine("任务数："+ queue.Count);
                }
                else if (key == "xx")
                {
                    TakeCommand();
                }
            }
        }
        static BlockingCollection<RequestCommand> queue;
        static Task task;
        private static void TestCommand()
        {
            queue = new BlockingCollection<RequestCommand>();
            task = new Task(() =>
            {
                foreach (RequestCommand item in queue.GetConsumingEnumerable())
                {
                    ExcuteCommand(item);
                }

            }, TaskCreationOptions.LongRunning);
            task.Start();
        }
        static int ii =0;
        private static void ExcuteCommand(RequestCommand command)
        {
            Thread.Sleep(5000);
            Console.WriteLine(command.ID+"执行成功");
        }
        private static void TakeCommand()
        {
            RequestCommand[] aa= queue.Where(s => s.ID == "100" ).ToArray();
            RequestCommand cm;
            if (aa.Count() == 1)
                cm = aa[0];
          bool resulet=  queue.TryTake(out cm);
            if (resulet)
            {
                Console.WriteLine(cm.ID+"移除成功");
            }
        }

        private static void TestSOAP()
        {
            //构造SOAP请求信息
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf - 8\"?><soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\"><soap12:Body><getXGInfo xmlns=\"http://tempuri.org/\" /></soap12:Body></soap12:Envelope>");
            //stringBuilder.Append("<soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\">");
            //stringBuilder.Append("<soap12:Body>");
            //stringBuilder.Append("<getXGInfo xmlns=\"http://tempuri.org/\" />");
            //stringBuilder.Append("</soap12:Body>");
            //stringBuilder.Append("</soap12:Envelope>");

            //发送请求
            Uri uri = new Uri("http://120.77.66.89:90/WebService1.asmx");
            WebRequest webRequest = WebRequest.Create(uri);
            webRequest.ContentType = "text/xml; charset=utf-8";
            webRequest.Method = "POST";
            
            using (Stream requestStream = webRequest.GetRequestStream())
            {
                byte[] paramBytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                requestStream.Write(paramBytes, 0, paramBytes.Length);
            }

            //响应
            WebResponse webResponse = webRequest.GetResponse();
            using (StreamReader myStreamReader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
            {
                string streamString = myStreamReader.ReadToEnd().Trim().Replace("\n", "");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(streamString);
                string s = doc.DocumentElement["soap:Body"]["getXGInfoResponse"]["getXGInfoResult"]["diffgr:diffgram"].InnerXml;
                doc.LoadXml(s);
                XmlNode node = doc.SelectSingleNode("NewDataSet");
                List<SoapData> list = new List<SoapData>();
                foreach(XmlNode childNode in node.ChildNodes)
                {
                    SoapData data = new SoapData();
                    foreach (XmlNode sonNode in childNode.ChildNodes)
                    {
                       
                        switch (sonNode.Name)
                        {
                            case "名称":
                                data.名称 = sonNode.InnerText;
                                break;
                            case "id":
                                data.id = Convert.ToInt32(sonNode.InnerText);
                                break;
                            case "设备ID":
                                data.设备ID = Convert.ToInt32(sonNode.InnerText);
                                break;
                            case "记录时间":
                                data.记录时间 = Convert.ToDateTime(sonNode.InnerText);
                                break;
                            case "采集时间":
                                data.采集时间 = Convert.ToDateTime(sonNode.InnerText);
                                break;
                            case "设备状态":
                                data.设备状态 = sonNode.InnerText;
                                break;
                            case "通讯状态":
                                data.通讯状态 = sonNode.InnerText;
                                break;
                            case "数据来源":
                                data.数据来源 = sonNode.InnerText;
                                break;
                            case "压力":
                                data.压力 = Convert.ToDecimal(sonNode.InnerText);
                                break;
                            case "电池电压":
                                data.电池电压 = Convert.ToDecimal(sonNode.InnerText);
                                break;
                            case "信号强度":
                                data.信号强度 = Convert.ToDecimal(sonNode.InnerText);
                                break;
                            case "瞬时流量":
                                data.瞬时流量 = Convert.ToDecimal(sonNode.InnerText);
                                break;
                            case "正累计流量":
                                data.正累计流量 = Convert.ToDecimal(sonNode.InnerText);
                                break;
                            case "负累计流量":
                                data.负累计流量 = Convert.ToDecimal(sonNode.InnerText);
                                break;
                            case "净累计流量":
                                data.净累计流量 = Convert.ToDecimal(sonNode.InnerText);
                                break;
                            case "压力2":
                                data.压力2 = Convert.ToDecimal(sonNode.InnerText);
                                break;
                            case "电池电压报警":
                                data.电池电压报警 = sonNode.InnerText;
                                break;
                            case "压力下限报警":
                                data.压力下限报警 = sonNode.InnerText;
                                break;
                            case "压力上限报警":
                                data.压力上限报警 = sonNode.InnerText;
                                break;
                            case "泵1运行":
                                data.泵1运行 = sonNode.InnerText;
                                break;
                            case "泵1故障":
                                data.泵1故障 = sonNode.InnerText;
                                break;
                            case "泵2运行":
                                data.泵2运行 = sonNode.InnerText;
                                break;
                            case "泵2故障":
                                data.泵2故障 = sonNode.InnerText;
                                break;
                            case "浊度":
                                data.浊度 = Convert.ToDecimal(sonNode.InnerText);
                                break;
                            case "余氯":
                                data.余氯 = Convert.ToDecimal(sonNode.InnerText);
                                break;
                            case "门开关":
                                data.门开关 = sonNode.InnerText;
                                break;
                            case "浊度故障":
                                data.浊度故障 = sonNode.InnerText;
                                break;
                            case "浊度上限报警":
                                data.浊度上限报警 = sonNode.InnerText;
                                break;
                            case "浊度下限报警":
                                data.浊度下限报警 = sonNode.InnerText;
                                break;
                            case "余氯故障":
                                data.余氯故障 = sonNode.InnerText;
                                break;
                            case "余氯上限报警":
                                data.余氯上限报警 = sonNode.InnerText;
                                break;
                            case "余氯下限报警":
                                data.余氯下限报警 = sonNode.InnerText;
                                break;
                            case "水表通讯故障":
                                data.水表通讯故障 = sonNode.InnerText;
                                break;
                            case "浊度状态":
                                data.浊度状态 = sonNode.InnerText;
                                break;
                            case "余氯状态":
                                data.余氯状态 = sonNode.InnerText;
                                break;
                        }
                        
                    }
                    list.Add(data);
                }
                
            }
        }

    }

    public interface I1
    {
        int A { get; set; }
    }
    public interface I2 : I1
    {

    }

    public class Person : I2
    {
        public string name;
        public int age;
        public int A { get; set; }
    }

    public class MyClass
    {
        public int Property1;
    }

    public class P2 : Person
    {
        public int aa;
    }

}
