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
    public partial class OPCPumpService : UserControl
    {
        public OPCPumpService()
        {
            InitializeComponent();
        }

        EnvConfigInfo config = EnvConfigInfo.SingleInstanceForCS;
        public event Action<string, EnvConfigInfo.Status> evtSubmit;


        private void OPCPumpService_Load(object sender, EventArgs e)
        {
            txt_PointsSaveInterVal.Value = config.confSonOPCPumpDataService.PointsSaveInterVal;
            txt_PointsCollectInterVal.Value = config.confSonOPCPumpDataService.PointsCollectInterVal;
            txt_DefaultGroupDeadband.Value = config.confSonOPCPumpDataService.DefaultGroupDeadband;
            txt_UpdateRate.Value = config.confSonOPCPumpDataService.UpdateRate;
            txt_ReadRate.Value = config.confSonOPCPumpDataService.ReadRate;
            txt_errorTimes.Value = config.confSonOPCPumpDataService.errorTimes;
            txt_okayTimes.Value = config.confSonOPCPumpDataService.okayTimes;
            txt_commandTimeoutSeconds.Value = config.confSonOPCPumpDataService.commandTimeoutSeconds;

            if (!config.EnvIsOkay || Config.ServerIsRuning)
            {
                btnSubmit.Enabled = false;
            }       
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            ConfSonOPCPumpDataService info = new ConfSonOPCPumpDataService();

            info.PointsSaveInterVal = Convert.ToInt32(txt_PointsSaveInterVal.Value);
            info.PointsCollectInterVal = Convert.ToInt32(txt_PointsCollectInterVal.Value);
            info.DefaultGroupDeadband = Convert.ToInt32(txt_DefaultGroupDeadband.Value);
            info.UpdateRate = Convert.ToInt32(txt_UpdateRate.Value);
            info.ReadRate = Convert.ToInt32(txt_ReadRate.Value);
            info.errorTimes = Convert.ToInt32(txt_errorTimes.Value);
            info.okayTimes = Convert.ToInt32(txt_okayTimes.Value);
            info.commandTimeoutSeconds = Convert.ToInt32(txt_commandTimeoutSeconds.Value);

            if(config.SubmitSonOPCPumpData(info, out string errMsg))
            {
                MessageBox.Show("提交 OPC二供接入服务 配置信息成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                evtSubmit("OPC二供接入服务", EnvConfigInfo.Status.success);
            }
            else
            {
                MessageBox.Show("提交 OPC二供接入服务 配置信息失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                evtSubmit("OPC二供接入服务", EnvConfigInfo.Status.fail);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            evtSubmit("OPC二供接入服务", EnvConfigInfo.Status.cancel);
        }
    }
}
