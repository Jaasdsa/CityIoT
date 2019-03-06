using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CityLogService;
using CityPublicClassLib;
using CityUtils;

namespace CityHisVacuate
{
    public class HisVacuate:IServiceWorker
    {
        // 历史数据抽稀接口
        public bool IsRuning { get; set; }

        ScadaHisVacuate scadaHisVacuate;
        PumpHisVacuate pumpHisVacuate;

        ConfSonHisVacuateService config;


        public HisVacuate(ConfSonHisVacuateService confHisVacuateService)
        {
            this.config = confHisVacuateService;
        }

        public void Start(out string errMsg)
        {
            errMsg = "";
            if (IsRuning)
                return;

            if (!Check(out errMsg))
                return;

            try
            {
                if (config.ScadaIsNeedRun)
                {
                    scadaHisVacuate = new ScadaHisVacuate(config.EndTime);
                    scadaHisVacuate.Start(out errMsg);
                    if (!scadaHisVacuate.IsRuning || !string.IsNullOrWhiteSpace(errMsg))
                    {
                        TraceManagerForHisVacuate.AppendErrMsg("Scada历史抽稀服务启动失败:" + errMsg);
                        Stop();
                        return;
                    }
                    TraceManagerForHisVacuate.AppendDebug("Scada历史抽稀服务启动成功");
                }
                if (config.PumpIsNeedRun)
                {
                    pumpHisVacuate = new PumpHisVacuate(config.EndTime);
                    pumpHisVacuate.Start(out errMsg);
                    if (!pumpHisVacuate.IsRuning || !string.IsNullOrWhiteSpace(errMsg))
                    {
                        TraceManagerForHisVacuate.AppendErrMsg("Pump历史抽稀服务启动失败:" + errMsg);
                        Stop();
                        return;
                    }
                    TraceManagerForHisVacuate.AppendDebug("Pump历史抽稀服务启动成功");
                }
            }
            catch(Exception e)
            {
                TraceManagerForHisVacuate.AppendErrMsg("历史抽稀服务启动异常:" + e.Message+"堆栈:"+e.StackTrace);
                Stop();
                return;
            }

            IsRuning = true;
        }
        public void Stop()
        {
            try
            {
                //  历史抽稀服务
                if (this.pumpHisVacuate != null)
                {
                    this.pumpHisVacuate.Stop();
                    this.pumpHisVacuate = null;
                    TraceManagerForHisVacuate.AppendDebug("Pump历史抽稀服务停止成功");
                }

                if (this.scadaHisVacuate != null)
                {
                    this.scadaHisVacuate.Stop();
                    this.scadaHisVacuate = null;
                    TraceManagerForHisVacuate.AppendDebug("Scada历史抽稀服务停止成功");
                }
                IsRuning = false;
            }
            catch(Exception e)
            {
                TraceManagerForHisVacuate.AppendErrMsg("Scada历史抽稀服务停止异常"+e.Message+"堆栈:"+e.StackTrace);
            }
        }

        public bool Check(out string errMsg)
        {
            errMsg = "";
            if (!config.EnvIsOkay)
            {
                errMsg = config.ErrMsg;
                return false;
            }
            if (!DateTime.TryParse(config.EndTime, out DateTime dt))
            {
                errMsg = "截止时间格式不正确.";
                return false;
            }
            return true;
        }
    }
}
