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
    public partial class WEBPandaPumpService : UserControl
    {
        public WEBPandaPumpService()
        {
            InitializeComponent();
        }

        EnvConfigInfo config = EnvConfigInfo.SingleInstanceForCS;
        public event Action<string, EnvConfigInfo.Status> evtSubmit;

        private void WEBPandaPumpService_Load(object sender, EventArgs e)
        {
            txt_appKey.Text = config.confSonWebPandaPumpDataService.AppKey;
            txt_appSecret.Text = config.confSonWebPandaPumpDataService.AppSecret;
            txt_getTokenUrl.Text = config.confSonWebPandaPumpDataService.GetTokenUrl;
            txt_getDataUrl.Text = config.confSonWebPandaPumpDataService.GetDataUrl;
            txt_useName.Text = config.confSonWebPandaPumpDataService.UseName;
            txt_collectInterval.Value = config.confSonWebPandaPumpDataService.CollectInterval;
            txt_saveInterVal.Value = config.confSonWebPandaPumpDataService.SaveInterVal;
            txt_commandTimeoutSeconds.Value = config.confSonWebPandaPumpDataService.CommandTimeoutSeconds;

            if (!config.EnvIsOkay || Config.ServerIsRuning)
            {
                btnSubmit.Enabled = false;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            ConfSonWebPandaPumpDataService info = new ConfSonWebPandaPumpDataService();

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
                MessageBox.Show("TokenUrl不能为空!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_getDataUrl.Text))
            {
                MessageBox.Show("DataUrl不能为空!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_useName.Text))
            {
                MessageBox.Show("useName不能为空!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            info.AppKey = txt_appKey.Text;
            info.AppSecret = txt_appSecret.Text;
            info.GetTokenUrl = txt_getTokenUrl.Text;
            info.GetDataUrl = txt_getDataUrl.Text;
            info.UseName = txt_useName.Text;
            info.SaveInterVal = Convert.ToInt32(txt_saveInterVal.Value);
            info.CollectInterval = Convert.ToInt32(txt_collectInterval.Value);
            info.CommandTimeoutSeconds = Convert.ToInt32(txt_commandTimeoutSeconds.Value);

            if (config.SubmitSonWebPandaPumpData(info, out string errMsg))
            {
                MessageBox.Show("提交 WEB熊猫二供接入服务 配置信息成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                evtSubmit("WEB熊猫二供接入服务", EnvConfigInfo.Status.success);
            }
            else
            {
                MessageBox.Show("提交 WEB熊猫二供接入服务 配置信息失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                evtSubmit("WEB熊猫二供接入服务", EnvConfigInfo.Status.fail);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            evtSubmit("WEB熊猫二供接入服务", EnvConfigInfo.Status.cancel);
        }

    }
}
