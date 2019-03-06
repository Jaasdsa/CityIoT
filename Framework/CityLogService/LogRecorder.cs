using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CityLogService
{
    class LogRecorder
    {
        private SQLiteHelper sqLite;
        private string filePath;
        private string tableName;//日志保存的表名

        private BlockingCollection<TraceItem> queue;
        private Task task;
        public bool IsRuning { get; set; }

        // 日志记录者
        public LogRecorder(string filePath,string tableName)
        {
            this.sqLite = new SQLiteHelper(filePath);
            this.filePath = filePath;
            this.tableName = tableName;
        }

        public void Start()
        {
            if (IsRuning)
                return;

            // 检查准备日志环境
            if (!ReadyLogEnv())
                return;

            queue = new BlockingCollection<TraceItem>();
            task = new Task(() =>
            {
                foreach (TraceItem item in queue.GetConsumingEnumerable())
                {
                    try
                    {
                        Record(item);
                    }
                    catch { }

                }

            }, TaskCreationOptions.LongRunning);
            task.Start();
            IsRuning = true;


        }
        public void Stop()
        {
            if (!IsRuning)
                return;
            // 先完成添加
            queue.CompleteAdding();
            DateTime time1 = DateTime.Now;
            while (queue.Count > 0)
            {
                Thread.Sleep(1);
                // 最多等待10秒避免关不掉
                if (DateTime.Now - time1 > TimeSpan.FromSeconds(10))
                {
                    break;
                }
            }
            while (queue.Count > 0)
            {
                // 等了十秒还没听，队列全部元素废弃
                queue.Take();
            }

            queue = null;
            Task.WaitAll(task);
            task.Dispose();
            task = null;
            IsRuning = false;
        }

        public void Append(TraceItem loggerRow)
        {
            if (!IsRuning)
                return;
            if (queue.IsAddingCompleted)
                return;
            if (queue.Count > 4096)
                return;
            queue.Add(loggerRow);
        }

        private void Record(TraceItem item)
        {
            Log log = item.ToLog();
            string id = Guid.NewGuid().ToString();
            string sql = string.Format(@"insert into {0} (类型,信息文本,插入时间,系统名称) values ('{1}',""{2}"",""{3}"",""{4}"")",
                                       this.tableName, log.type, log.text, log.dateTime,log.serverName);
            this.sqLite.ExecuteNonQuery(sql);
        }

        private bool ReadyLogEnv()
        {
            // 判断日志文件是否存在
            if (!File.Exists(filePath))
            {
                if (!SQLiteHelper.CreateDB(filePath))
                    return false; ;
            }

            // 根据日志表存在情况创建表，并加上时间的索引
            string dt = DataUtil.ToDateString(DateTime.Now);
            SQLiteHelper sQLite = new SQLiteHelper(filePath);
            if (!sQLite.ExistTable(this.tableName))
            {
                string creatTable =string.Format(@"CREATE TABLE if not exists  {0} (
                                        ID  integer  PRIMARY KEY autoincrement,
                                        类型   VARCHAR (50),
                                        信息文本 TEXT,
                                        插入时间 DATETIME,
                                        系统名称 VARCHAR (50) 
                                    );
                                    CREATE  INDEX 'Index_LogTime' on {0} ('插入时间' DESC);
                                    insert into {0} (类型,信息文本,插入时间,系统名称) values ('信息','创建数据库日志表成功','{1}','系统')", this.tableName,dt);
                sQLite.ExecuteNonQuery(creatTable);
            }

            // 检查表是否创建成功
            if (!sQLite.ExistTable(this.tableName))
                return false;

            return true;
        }

    }

}
