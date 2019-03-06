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
    public partial class PublishConf : UserControl
    {
        public PublishConf()
        {
            InitializeComponent();
        }

        EnvConfigInfo config = EnvConfigInfo.SingleInstanceForCS;

        private void PublishConf_Load(object sender, EventArgs e)
        {
            txt_IP.Text = config.confPublishServiceInfo.IP;
            txt_Port.Value = config.confPublishServiceInfo.Port == 0 ? txt_Port.Minimum : config.confPublishServiceInfo.Port;

            if (!config.EnvIsOkay)
            {
                btn_Save.Enabled = false;
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            ConfPublishServiceInfo info = new ConfPublishServiceInfo();

            int port = Convert.ToInt32(txt_Port.Text == "" ? "0" : txt_Port.Text);

            if (port <= 9000 && port >= 65535)
            {
                MessageBox.Show("端口范围不对,9000~65535!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            info.Port = port;

            if (config.UpdatePublishInfo(info, out string errMsg))
            {
                MessageBox.Show("更新发布配置信息成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("更新发布配置信息," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
