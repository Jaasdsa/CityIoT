using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTUPumpDataService 
{
    class DTUController
    {
        private static int commandConnectDBTime;

        public static bool IsRuning = false;
        public static void Start()
        {
            if (IsRuning)
                return;

            IsRuning = true;
        }
        public static void Stop()
        {
            if (!IsRuning)
                return;

            IsRuning = false;
        }
    }
}
