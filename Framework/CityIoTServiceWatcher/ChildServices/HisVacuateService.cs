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
    public partial class HisVacuateService : UserControl
    {
        public HisVacuateService()
        {
            InitializeComponent();
        }

        EnvConfigInfo config = EnvConfigInfo.SingleInstanceForCS;
        public event Action<string, EnvConfigInfo.Status> evtSubmit;

        private void HisVacuateService_Load(object sender, EventArgs e)
        {
            checkBox_Scada.Checked = config.confSonHisVacuateService.ScadaIsNeedRun;
            checkBox_Pump.Checked = config.confSonHisVacuateService.PumpIsNeedRun;
            dt_EndTime.Text = config.confSonHisVacuateService.EndTime;

            if (!config.EnvIsOkay || Config.ServerIsRuning)
            {
                btnSubmit.Enabled = false;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            ConfSonHisVacuateService info = new ConfSonHisVacuateService();

            info.ScadaIsNeedRun = checkBox_Scada.Checked;
            info.PumpIsNeedRun = checkBox_Pump.Checked;
            info.EndTime = dt_EndTime.Text;

            if(config.SubmitSonHisVacuateData(info, out string errMsg))
            {
                MessageBox.Show("提交 历史抽稀服务 配置信息成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                evtSubmit("历史抽稀服务", EnvConfigInfo.Status.success);
            }
            else
            {
                MessageBox.Show("提交 历史抽稀服务 配置信息失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                evtSubmit("历史抽稀服务", EnvConfigInfo.Status.fail);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            evtSubmit("历史抽稀服务", EnvConfigInfo.Status.cancel);
        }
    }
}
