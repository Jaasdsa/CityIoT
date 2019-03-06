using CityUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CityLogService 
{
    /// <summary>
    ///  日志收缩，设置日志保存的天数。防止日志文件过大
    /// </summary>
    class LogShrink
    {
        private string filePath;
        private string tableName;
        private string manintainTime;
        private int logMaxSaveDays;

        /// <summary>
        /// 日志维护器
        /// </summary>
        /// <param name="filePath">日志文件完整路径</param>
        /// <param name="tableName">日志存储表名</param>
        /// <param name="manintainTime">每天设定维护时间</param>
        /// <param name="logMaxSaveDays">日志保存多少天</param>
        public LogShrink(string filePath,string tableName, string manintainTime, int logMaxSaveDays)
        {
            this.filePath =filePath;
            this.tableName =tableName;
            this.manintainTime = manintainTime;
            this.logMaxSaveDays = logMaxSaveDays;
        }

        public bool IsRuning { get; set; }
        private bool startFlag;
        private Task task;
        public void Start()
        {
            if (IsRuning)
                return;

            startFlag = true;
            task = new Task(() => {
                while (startFlag)
                {
                    Thread.Sleep(50);
                    string timeNow = DateTime.Now.ToString("HH:mm:ss");
                    if (timeNow == manintainTime)
                    {
                        Thread.Sleep(1000);//睡眠一秒让时间过去，防止再次循环触发维护事件
                        try
                        {
                            Shrink();
                        }
                        catch { }
                    }
                }
            });
            task.Start();
            IsRuning = true;
        }
        public void Stop()
        {
            if (!IsRuning)
                return;

            startFlag = false;
            Task.WaitAll(task);
            task.Dispose();
            task = null;

            IsRuning = false;
        }

        private void Shrink()
        {
            if (!File.Exists(filePath))
                return;

            SQLiteHelper sqlite = new SQLiteHelper(filePath);
            if (!sqlite.ExistTable(tableName))
                return;
            string time= DataUtil.ToDateString( DateTime.Now.AddDays(-logMaxSaveDays));
            string sql = string.Format(@"delete from {0} where 插入时间<'{1}';",tableName,time);
            sqlite.ExecuteNonQuery(sql);
        }
    }
}
