using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityPublicClassLib
{
   public interface ISonService
    {
        /// <summary>
        /// 接受命令
        /// </summary>
        /// <param name="command"></param>
        void ReceiveCommand(RequestCommand command);

    }

}
