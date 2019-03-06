using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CityIoTCore;

namespace CityIoTService
{
    public partial class CityIoTService : ServiceBase
    {
        public CityIoTService()
        {
            InitializeComponent();
        }
        IoTServiceCore core;

        protected override void OnStart(string[] args)
        {
            if (args != null && args.Length == 1 && args[0] == "delayStart")
                DelayedStart();
            else
                Start();
        }

        protected override void OnStop()
        {
            try
            {
                if (core != null)
                {
                    core.Stop();
                    core = null;
                }

                if (timer != null)
                {
                    timer.Enabled = false;
                    timer.Dispose();
                    timer = null;
                }
            }
            catch
            {
            }
        }

        private System.Timers.Timer timer;

        private void DelayedStart()
        {
            timer = new System.Timers.Timer();
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Interval = 1000;
            bool flag = false;
            DateTime t1 = DateTime.Now;
            timer.Elapsed += (o, e) => {   
                DateTime t2 = DateTime.Now;
                if (t2 - t1 > TimeSpan.FromSeconds(20))
                {
                    if (!flag)
                    {
                        flag = true;
                        Start();                  
                    }                  
                }
            };
            timer.Start();
        }
        private void Start()
        {
            try
            {
                core = new IoTServiceCore();
                core.Start();
            }
            catch
            {
            }
        }

    }
}
