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
    public partial class AddSonService : UserControl
    {
        public AddSonService()
        {
            InitializeComponent();
        }

        public event Action<string> evtSubmit;

        EnvConfigInfo config = EnvConfigInfo.SingleInstanceForCS;
        ConfigCenter configCenter = new ConfigCenter();

        string[] array = { "OPC二供接入服务", "OPC-SCADA接入服务", "二供报警服务", "WEB熊猫二供接入服务", "WEB熊猫二供SCADA接入服务", "WEB熊猫监测点接入服务", "WEB熊猫综合测点接入服务", "WEB熊猫控制服务", "项目的特殊服务", "历史抽稀服务" };

        private void AddSonService_Load(object sender, EventArgs e)
        {
            List<string> runServices = config.GetRunServiceList(config.confFileInfo.ProjectConfigPath, out string errMsg);
            List<string> list = array.ToList().Except(runServices).ToList();
            foreach (string name in list)
            {
                
                comboBox_sonServiceList.Items.Add(name);
            }

            if (!config.EnvIsOkay || Config.ServerIsRuning)
            {
                btnSubmit.Enabled = false;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> runServices = config.GetRunServiceList(config.confFileInfo.ProjectConfigPath, out string errMsg);
            if(runServices.Count == 0 || comboBox_sonServiceList.SelectedItem == null)
            {
                btnSubmit.Enabled = false;
            }

           evtSubmit(comboBox_sonServiceList.SelectedItem.ToString());

        }
    }
}
