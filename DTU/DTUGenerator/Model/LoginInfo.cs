using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTUGenerator
{
    public class LoginInfo
    {
        public string DTUID { get; set; }
        public string PumpName { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(this.PumpName))
                return this.PumpName;
            else
                return
                    this.DTUID;
        }
    }
}
