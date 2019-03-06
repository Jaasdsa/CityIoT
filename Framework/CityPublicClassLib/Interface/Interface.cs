using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityPublicClassLib 
{
    public interface IWorker
    {
        // 工作者的接口
        bool IsRuning { get; set; }
        void Start();
        void Stop();
    }

    public interface IServiceWorker
    {
        // 工作者的接口
        bool IsRuning { get; set; }
        void Start(out string errMsg);
        void Stop();
    }
}
