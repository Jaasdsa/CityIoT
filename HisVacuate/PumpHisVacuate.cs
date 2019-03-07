using CityLogService;
using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CityHisVacuate
{
    class PumpHisVacuate
    {
        // Pump历史数据抽稀接口
        public bool IsRuning { get; set; }
        DateTime endTime;
        Task task;
        bool taskIsDoing;

        public PumpHisVacuate(string endTime)
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
                    DateTime curTime = DateTime.Now;
                    if ((curTime.Hour == 0 && curTime.Minute == 10 && curTime.Second == 0) || (curTime.Hour == endTime.Hour && curTime.Minute == endTime.Minute && curTime.Second == endTime.Second))
                    {
                        Excute();
                        Thread.Sleep(1000); //防止再次触发
                    }
                    Thread.Sleep(1);
                }
            }, TaskCreationOptions.LongRunning);
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
            string sql = @"merge into PumpHisDayData as t
                        using (select * from(
                        select row_number() over(partition by BASEID order by TempTime asc) nRow ,* 
                        from PumpHisData s1 where  s1.TempTime >cast(convert(varchar(10),getdate(),120)+' 00:00:00' as datetime)) s2 
                        where s2.nRow=1) as s
                        on t.TempTime >cast(convert(varchar(10),getdate(),120)+' 00:00:00' as datetime) and t.BASEID=s.BASEID
                        when not matched
                        then insert values (
                               s.[FCreateDate]
                              ,s.[TempTime]
                              ,s.[BASEID]
                              ,s.[F40001]
                              ,s.[F40002]
                              ,s.[F40003]
                              ,s.[F40004]
                              ,s.[F40005]
                              ,s.[F40006_0_0]
                              ,s.[F40006_0_1]
                              ,s.[F40006_0_2]
                              ,s.[F40006_0_3]
                              ,s.[F40006_0_4]
                              ,s.[F40006_0_5]
                              ,s.[F40006_0_6]
                              ,s.[F40006_0_7]
                              ,s.[F40006_1_0]
                              ,s.[F40006_1_1]
                              ,s.[F40006_1_2]
                              ,s.[F40006_1_3]
                              ,s.[F40006_1_4]
                              ,s.[F40006_1_5]
                              ,s.[F40006_1_6]
                              ,s.[F40006_1_7]
                              ,s.[F40007_0_0]
                              ,s.[F40007_0_1]
                              ,s.[F40007_0_2]
                              ,s.[F40007_0_3]
                              ,s.[F40007_0_4]
                              ,s.[F40007_0_5]
                              ,s.[F40007_0_6]
                              ,s.[F40007_0_7]
                              ,s.[F40007_1_0]
                              ,s.[F40007_1_1]
                              ,s.[F40007_1_2]
                              ,s.[F40007_1_3]
                              ,s.[F40007_1_4]
                              ,s.[F40007_1_5]
                              ,s.[F40007_1_6]
                              ,s.[F40007_1_7]
                              ,s.[F40008_0_0]
                              ,s.[F40008_0_1]
                              ,s.[F40008_0_2]
                              ,s.[F40008_0_3]
                              ,s.[F40008_0_4]
                              ,s.[F40008_0_5]
                              ,s.[F40008_0_6]
                              ,s.[F40008_0_7]
                              ,s.[F40008_1_0]
                              ,s.[F40008_1_1]
                              ,s.[F40008_1_2]
                              ,s.[F40008_1_3]
                              ,s.[F40008_1_4]
                              ,s.[F40008_1_5]
                              ,s.[F40008_1_6]
                              ,s.[F40008_1_7]
                              ,s.[F40009]
                              ,s.[F40010]
                              ,s.[F40011]
                              ,s.[F40014]
                              ,s.[F40015]
                              ,s.[F40016]
                              ,s.[F40017]
                              ,s.[F40020]
                              ,s.[F40021]
                              ,s.[F40024]
                              ,s.[F40025]
                              ,s.[F40026]
                              ,s.[F40027]
                              ,s.[F40028]
                              ,s.[F40029]
                              ,s.[F40030]
                              ,s.[F40031]
                              ,s.[F40032]
                              ,s.[F40033]
                              ,s.[F40034]
                              ,s.[F40035]
                              ,s.[F40036]
                              ,s.[F40037]
                              ,s.[F40038]
                              ,s.[F40039]
                              ,s.[F40040]
                              ,s.[F40041]
                              ,s.[F40042]
                              ,s.[F40043]
                              ,s.[F40044]
                              ,s.[F40045]
                              ,s.[F40046]
                              ,s.[F40047]
                              ,s.[F40048]
                              ,s.[F40049]
                              ,s.[F40050]
                              ,s.[F40051]
                              ,s.[F40052]
                              ,s.[F40053]
                              ,s.[F40054]
                              ,s.[F40055]
                              ,s.[F40056]
                              ,s.[F40057]
                              ,s.[F40058]
                              ,s.[F40059]
                              ,s.[F40060]
                              ,s.[F40061]
                              ,s.[F40062]
                              ,s.[F40063]
                              ,s.[F40064]
                              ,s.[F40065]
                              ,s.[F40066]
                              ,s.[F40067]
                              ,s.[F40068]
                              ,s.[F40069]
                              ,s.[F40070]
                              ,s.[F40071]
                              ,s.[F40072]
                              ,s.[F40073]
                              ,s.[F40074]
                              ,s.[F40075]
                              ,s.[F40076]
                              ,s.[F40077]
                              ,s.[F40078]
                              ,s.[F40079]
                              ,s.[F40080]
                              ,s.[F40081]
                              ,s.[F40082]
                              ,s.[F40083]
                              ,s.[F40084]
                              ,s.[F40085]
                              ,s.[F40086]
                              ,s.[F40087]
                              ,s.[F40088]
                              ,s.[F40089]
                              ,s.[F40090]
                              ,s.[F40091]
                              ,s.[F40092]
                              ,s.[F40093]
                              ,s.[F40094]
                              ,s.[F40095]
                              ,s.[F40096]
                              ,s.[F40097]
                              ,s.[F40098]
                              ,s.[F40099]
                              ,s.[F40100]
                              ,s.[F40101]
                              ,s.[F40102]
                              ,s.[F40103]
                              ,s.[F40104]
                              ,s.[F40105]
                              ,s.[F40106]
                              ,s.[F40107]
                              ,s.[F40108]
                              ,s.[F40109]
                              ,s.[F40110]
                              ,s.[F40111]
                              ,s.[F40112]
                              ,s.[F40113]
                              ,s.[F40114]
                              ,s.[F40115]
                              ,s.[F40116]
                              ,s.[F40117]
                              ,s.[F40118]
                              ,s.[F40119]
                              ,s.[F40120]
                              ,s.[F40121]
                              ,s.[F40122]
                              ,s.[F40123]
                              ,s.[F40124]
                              ,s.[F40125]
                              ,s.[F40126]
                              ,s.[F40127]
                              ,s.[F40128]
                              ,s.[F40129]
                              ,s.[F40130]
                              ,s.[F40131]
                              ,s.[F40132]
                              ,s.[F40133]
                              ,s.[F40134]
                              ,s.[F40135]
                              ,s.[F40136]
                              ,s.[F40137]
                              ,s.[F40140]
                              ,s.[F40141]
                              ,s.[F40142]
                              ,s.[F40143]
                              ,s.[F40144]
                              ,s.[F40145]
                              ,s.[F40146]
                              ,s.[F40147]
                              ,s.[F40148]
                              ,s.[F40149]
                              ,s.[F40150]
                              ,s.[F40250]
                              ,s.[F40249]
                              ,s.[F40248]
                              ,s.[F40247]
                              ,s.[F40246]
                              ,s.[F40245]
                              ,s.[F40244]
                              ,s.[F40243]
                              ,s.[F40242]
                              ,s.[F40241]
                              ,s.[F40240]
                              ,s.[F40239]
                              ,s.[F40238]
                              ,s.[F40237]
                              ,s.[F40236]
                              ,s.[F40235]
                              ,s.[F40234]
                              ,s.[F40233]
                              ,s.[F40232]
                              ,s.[F40231]
                              ,s.[F40230]
                              ,s.[F40229]
                              ,s.[F40228]
                              ,s.[F40227]
                              ,s.[F40226]
                              ,s.[F40225]
                              ,s.[F40224]
                              ,s.[F40223]
                              ,s.[F40222]
                              ,s.[F40221]
                              ,s.[F40220]
                              ,s.[F40219]
                              ,s.[F40218]
                              ,s.[F40217]
                              ,s.[F40216]
                              ,s.[F40215]
                              ,s.[F40214]
                              ,s.[F40213]
                              ,s.[F40212]
                              ,s.[F40211]
                              ,s.[F40210]
                              ,s.[F40209]
                              ,s.[F40208]
                              ,s.[F40207]
                              ,s.[F40206]
                              ,s.[F40205]
                              ,s.[F40204]
                              ,s.[F40203]
                              ,s.[F40202]
                              ,s.[F40201]
                              ,s.[F40200]
                              ,s.[F40199_0_0]
                              ,s.[F40199_0_1]
                              ,s.[F40199_0_2]
                              ,s.[F40199_0_3]
                              ,s.[F40199_0_4]
                              ,s.[F40199_0_5]
                              ,s.[F40199_0_6]
                              ,s.[F40199_0_7]
                              ,s.[F40199_1_0]
                              ,s.[F40199_1_1]
                              ,s.[F40199_1_2]
                              ,s.[F40199_1_3]
                              ,s.[F40199_1_4]
                              ,s.[F40199_1_5]
                              ,s.[F40199_1_6]
                              ,s.[F40199_1_7]
                        );";
            DBUtil.ExecuteNonQuery(sql, out string err);
            if (!string.IsNullOrWhiteSpace(err))
                TraceManagerForHisVacuate.AppendWarning("Pump天表缓存任务执行失败:" + err);
            else
                TraceManagerForHisVacuate.AppendInfo("Pump天表缓存任务执行成功");
        }
    }
}
