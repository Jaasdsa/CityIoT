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
    public partial class LogConf : UserControl
    {
        public LogConf()
        {
            InitializeComponent();
        }

        EnvConfigInfo config = EnvConfigInfo.SingleInstanceForCS;

        private void LogConf_Load(object sender, EventArgs e)
        {
            dt_wh.Text = config.confLogServiceInfo.ManintainTime;
            txt_SaveDay.Value = config.confLogServiceInfo.LogMaxSaveDays == 0 ? txt_SaveDay.Minimum : config.confLogServiceInfo.LogMaxSaveDays;
            txt_IP.Text = config.confLogServiceInfo.IP;
            txt_Port.Value = config.confLogServiceInfo.Port == 0 ? txt_Port.Minimum : config.confLogServiceInfo.Port;

            if (!config.EnvIsOkay)
            {
                btn_Save.Enabled = false;
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            ConfLogServiceInfo info = new ConfLogServiceInfo();

            int saveDay = Convert.ToInt32(txt_SaveDay.Text == "" ? "0": txt_SaveDay.Text);
            int port = Convert.ToInt32(txt_Port.Text == "" ? "0" : txt_Port.Text);

            if (string.IsNullOrWhiteSpace(dt_wh.Text))
            {
                MessageBox.Show("每天日志维护的时间不能为空!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(saveDay <= 0 && saveDay > 365)
            {
                MessageBox.Show("日志保存天数范围不对,1~365!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(port <= 9000 && port >= 65535)
            {
                MessageBox.Show("端口范围不对,9000~65535!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
           
            info.ManintainTime = dt_wh.Text;
            info.LogMaxSaveDays = saveDay;
            info.Port = port;

            if(config.UpdateLogInfo(info, out string errMsg))
            {
                MessageBox.Show("更新日志配置信息成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("更新日志配置信息," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
