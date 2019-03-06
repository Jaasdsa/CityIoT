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
    public partial class WebPandaControlService : UserControl
    {
        public WebPandaControlService()
        {
            InitializeComponent();
        }

        EnvConfigInfo config = EnvConfigInfo.SingleInstanceForCS;
        public event Action<string, EnvConfigInfo.Status> evtSubmit;

        private void WebPandaControlService_Load(object sender, EventArgs e)
        {
            txt_appKey.Text = config.confSonWebPandaControlService.AppKey;
            txt_appSecret.Text = config.confSonWebPandaControlService.AppSecret;
            txt_getTokenUrl.Text = config.confSonWebPandaControlService.GetTokenUrl;
            txt_setPumpControl.Text = config.confSonWebPandaControlService.SetPumpControl;
            txt_commandTimeoutSeconds.Value = config.confSonWebPandaControlService.CommandTimeoutSeconds;

            if (!config.EnvIsOkay || Config.ServerIsRuning)
            {
                btnSubmit.Enabled = false;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            ConfSonWebPandaControlService info = new ConfSonWebPandaControlService();

            if (string.IsNullOrWhiteSpace(txt_appKey.Text))
            {
                MessageBox.Show("appKey不能为空!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_appSecret.Text))
            {
                MessageBox.Show("appSecret不能为空!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_getTokenUrl.Text))
            {
                MessageBox.Show("getTokenUrl不能为空!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_setPumpControl.Text))
            {
                MessageBox.Show("setPumpControl不能为空!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            info.AppKey = txt_appKey.Text;
            info.AppSecret = txt_appSecret.Text;
            info.GetTokenUrl = txt_getTokenUrl.Text;
            info.SetPumpControl = txt_setPumpControl.Text;
            info.CommandTimeoutSeconds = Convert.ToInt32(txt_commandTimeoutSeconds.Value);

            if (config.SubmitSonWebPandaControlData(info, out string errMsg))
            {
                MessageBox.Show("提交 WEB熊猫控制服务 配置信息成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                evtSubmit("WEB熊猫控制服务", EnvConfigInfo.Status.success);
            }
            else
            {
                MessageBox.Show("提交 WEB熊猫控制服务 配置信息失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                evtSubmit("WEB熊猫控制服务", EnvConfigInfo.Status.fail);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            evtSubmit("WEB熊猫控制服务", EnvConfigInfo.Status.cancel);
        }
    }
}
