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
    public partial class CommandConf : UserControl
    {
        public CommandConf()
        {
            InitializeComponent();
        }

        EnvConfigInfo config = EnvConfigInfo.SingleInstanceForCS;

        private void CommandConf_Load(object sender, EventArgs e)
        {
            text_IP.Text = config.confCommandServiceInfo.IP;
            text_Port.Value = config.confCommandServiceInfo.Port == 0 ? text_Port.Minimum : config.confCommandServiceInfo.Port;
            text_Sec.Value = config.confCommandServiceInfo.timeoutSeconds;

            if (!config.EnvIsOkay)
            {
                button_save.Enabled = false;
            }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            ConfCommandServiceInfo info = new ConfCommandServiceInfo();

            int port = Convert.ToInt32(text_Port.Text == "" ? "0" : text_Port.Text);
            int second = Convert.ToInt32(text_Sec.Text == "" ? "0" : text_Sec.Text);  
            
            if (port <= 9000 && port >= 65535)
            {
                MessageBox.Show("端口范围不对,9000~65535!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (second < 0 && second > 60)
            {
                MessageBox.Show("命令超时秒数范围不对,0~60!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            info.Port = port;
            info.timeoutSeconds = second;

            if (config.UpdateCommandInfo(info, out string errMsg))
            {
                MessageBox.Show("更新命令配置信息成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("更新命令配置信息," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
