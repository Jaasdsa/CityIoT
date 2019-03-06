
using CityIoTCommand;
using CityIoTCore;
using CityIoTPumpAlarm;
using CityIoTServiceManager;
using CityOPCDataService;
using CityPublicClassLib;
using CityUtils;
using CityWEBDataService;
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

            // Test_core();

            Console.ReadLine();
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
