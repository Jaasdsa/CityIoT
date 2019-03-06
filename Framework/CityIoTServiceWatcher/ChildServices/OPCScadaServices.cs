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
    public partial class OPCScadaServices : UserControl
    {
        public OPCScadaServices()
        {
            InitializeComponent();
        }

        EnvConfigInfo config = EnvConfigInfo.SingleInstanceForCS;
        public event Action<string, EnvConfigInfo.Status> evtSubmit;

        private void OPCScadaServices_Load(object sender, EventArgs e)
        {
            txt_PointsSaveInterVal.Value = config.confSonOPCScadaDataService.PointsSaveInterVal; 
            txt_PointsCollectInterVal.Value = config.confSonOPCScadaDataService.PointsCollectInterVal;
            txt_DefaultGroupDeadband.Value = config.confSonOPCScadaDataService.DefaultGroupDeadband;
            txt_UpdateRate.Value = config.confSonOPCScadaDataService.UpdateRate;
            txt_ReadRate.Value = config.confSonOPCScadaDataService.ReadRate;
            txt_errorTimes.Value = config.confSonOPCScadaDataService.errorTimes;
            txt_okayTimes.Value = config.confSonOPCScadaDataService.okayTimes;
            txt_commandTimeoutSeconds.Value = config.confSonOPCScadaDataService.commandTimeoutSeconds;

            if (!config.EnvIsOkay || Config.ServerIsRuning)
            {
                btnSubmit.Enabled = false;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            ConfSonOPCScadaDataService info = new ConfSonOPCScadaDataService();
            info.PointsSaveInterVal = Convert.ToInt32(txt_PointsSaveInterVal.Value);
            info.PointsCollectInterVal = Convert.ToInt32(txt_PointsCollectInterVal.Value);
            info.DefaultGroupDeadband = Convert.ToInt32(txt_DefaultGroupDeadband.Value);
            info.UpdateRate = Convert.ToInt32(txt_UpdateRate.Value);
            info.ReadRate = Convert.ToInt32(txt_ReadRate.Value);
            info.errorTimes = Convert.ToInt32(txt_errorTimes.Value);
            info.okayTimes = Convert.ToInt32(txt_okayTimes.Value);
            info.commandTimeoutSeconds = Convert.ToInt32(txt_commandTimeoutSeconds.Value);

            if (config.SubmitSonOPCScadaData(info, out string errMsg))
            {
                MessageBox.Show("提交 OPC-SCADA接入服务 配置信息成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                evtSubmit("OPC-SCADA接入服务", EnvConfigInfo.Status.success);
            }
            else
            {
                MessageBox.Show("提交 OPC-SCADA接入服务 配置信息失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                evtSubmit("OPC-SCADA接入服务", EnvConfigInfo.Status.fail);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            evtSubmit("OPC-SCADA接入服务", EnvConfigInfo.Status.cancel);
        }
    }
}
