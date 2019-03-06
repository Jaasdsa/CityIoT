using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CityPublicClassLib;

namespace CityIoTServiceWatcher
{
    public partial class PumpAlarmService : UserControl
    {
        public PumpAlarmService()
        {
            InitializeComponent();
        }

        EnvConfigInfo config = EnvConfigInfo.SingleInstanceForCS;
        public event Action<string, EnvConfigInfo.Status> evtSubmit;

        private void PumpAlarmService_Load(object sender, EventArgs e)
        {
            txt_alarmUpdateInterval.Value = config.confSonCityIoTPumpAlarm.UpdateInterval;
            txt_pumpJZTimeOut.Value = config.confSonCityIoTPumpAlarm.JZTimeOut;
            txt_commandTimeoutSeconds.Value = config.confSonCityIoTPumpAlarm.CommandTimeoutSeconds;

            if (!config.EnvIsOkay || Config.ServerIsRuning)
            {
                btnSubmit.Enabled = false;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            ConfSonCityIoTPumpAlarm info = new ConfSonCityIoTPumpAlarm();
            info.UpdateInterval = Convert.ToInt32(txt_alarmUpdateInterval.Value);
            info.JZTimeOut = Convert.ToInt32(txt_pumpJZTimeOut.Value);
            info.CommandTimeoutSeconds = Convert.ToInt32(txt_commandTimeoutSeconds.Value);

            if (config.SubmitSonPumpAlarmData(info, out string errMsg))
            {
                MessageBox.Show("提交 二供报警服务 配置信息成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                evtSubmit("二供报警服务", EnvConfigInfo.Status.success);
            }
            else
            {
                MessageBox.Show("提交 二供报警服务 配置信息失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                evtSubmit("二供报警服务", EnvConfigInfo.Status.fail);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            evtSubmit("二供报警服务", EnvConfigInfo.Status.cancel);
        }

    }
}
