using CityPublicClassLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiangXiNanChang
{

    public class WebInterfaceExcute: CaseManagerInjection, ISonService, IServiceWorker
    {
        // 特殊项目类型，不用进产品子服务库

        // 任务接口入口
        public override bool IsRuning { get; set; }

        public override void Start(out string errMsg)
        {
            errMsg = "";


            IsRuning = true;
        }

        public override void Stop()
        {


            IsRuning = false;
        }

        public override void ReceiveCommand(RequestCommand command)
        {

        }

    }
}
