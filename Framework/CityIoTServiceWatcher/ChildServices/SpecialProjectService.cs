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
    public partial class SpecialProjectService : UserControl
    {
        public SpecialProjectService()
        {
            InitializeComponent();
        }

        EnvConfigInfo config = EnvConfigInfo.SingleInstanceForCS;
        public event Action<string, EnvConfigInfo.Status> evtSubmit;

        private void SpecialProjectService_Load(object sender, EventArgs e)
        {
            txt_dllPath.Text = config.confSonProjectDataService.DllPath;
            txt_projectName.Text = config.confSonProjectDataService.ProjectName;

            if (!config.EnvIsOkay || Config.ServerIsRuning)
            {
                btnSubmit.Enabled = false;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            ConfSonProjectDataService info = new ConfSonProjectDataService();

            if (string.IsNullOrWhiteSpace(txt_dllPath.Text))
            {
                MessageBox.Show("dllPath不能为空!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_projectName.Text))
            {
                MessageBox.Show("projectName不能为空", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            info.DllPath = txt_dllPath.Text.Trim();
            info.ProjectName = txt_projectName.Text.Trim();

            if (config.SubmitSonSpecialProjectData(info, out string errMsg))
            {
                MessageBox.Show("提交 项目的特殊服务 配置信息成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                evtSubmit("项目的特殊服务", EnvConfigInfo.Status.success);
            }
            else
            {
                MessageBox.Show("提交 项目的特殊服务 配置信息失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                evtSubmit("项目的特殊服务", EnvConfigInfo.Status.fail);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            evtSubmit("项目的特殊服务", EnvConfigInfo.Status.cancel);
        }
    }
}
