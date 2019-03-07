using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CityLogService;

namespace CityHisVacuate
{
    class ScadaHisVacuate
    {
        // Scada历史数据抽稀接口
        public bool IsRuning { get; set; }
        DateTime endTime;
        Task task;
        bool taskIsDoing;

        public ScadaHisVacuate(string endTime)
        {
            DateTime.TryParse(endTime, out this.endTime);//已经验证过
        }

        public void Start(out string errMsg)
        {
            errMsg = "";
            if (IsRuning)
                return;

            task = new Task(() => {
                while (this.taskIsDoing)
                {
                    DateTime curTime= DateTime.Now;
                    if ((curTime.Hour == 0 && curTime.Minute == 10 && curTime.Second == 0) || (curTime.Hour == endTime.Hour && curTime.Minute == endTime.Minute && curTime.Second == endTime.Second))
                    {
                        Excute();
                        Thread.Sleep(1000); //防止再次触发
                    }
                    Thread.Sleep(1);
                }
            },TaskCreationOptions.LongRunning);
            this.taskIsDoing = true;
            task.Start();

            IsRuning = true;
        }
        public void Stop()
        {
            if (task != null)
            {
                this.taskIsDoing = false;
                task.Wait(5000);
                task.Dispose();
                task = null;
            }
            IsRuning = false;
        }

        public void Excute()
        {
            string sql = @"merge into SCADA_SensorDayHistory as t
                            using (select * from(
                            select row_number() over(partition by SensorID order by PT asc) nRow ,* 
                            from SCADA_SensorHistory s1 where  s1.PT >cast(convert(varchar(10),getdate(),120)+' 00:00:00' as datetime)) s2 
                            where s2.nRow=1) as s
                            on t.PT >cast(convert(varchar(10),getdate(),120)+' 00:00:00' as datetime) and t.SensorID=s.SensorID
                            when not matched
                            then insert values (s.SensorID,s.PV,s.PT,s.Date);";
            DBUtil.ExecuteNonQuery(sql, out string err);
            if (!string.IsNullOrWhiteSpace(err))
                TraceManagerForHisVacuate.AppendWarning("Scada天表缓存任务执行失败:"+err);
            else
                TraceManagerForHisVacuate.AppendInfo("Scada天表缓存任务执行成功");
        }
    }
}
