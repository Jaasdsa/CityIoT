using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CityIoTServiceManager;
using CityUtils;
using CityPublicClassLib;

namespace CityIoTServiceWatcher
{
    public partial class SolutionConf : UserControl
    {
        string LastAddSolutionName = "";
        public SolutionConf()
        {
            InitializeComponent();
        }

        EnvConfigInfo config =EnvConfigInfo.SingleInstanceForCS;

        private void btn_submit_Click(object sender, EventArgs e)
        {
            string selectVal = this.confCenterList.Text;

            if (!this.confCenterList.Items.Contains(this.confCenterList.Text))
            {
                MessageBox.Show("该项目配置不可用，请重新选择!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ChangeConfCenter(selectVal);
        }

        private void ChangeConfCenter(string selectedName)
        {
            if (config.ChangeConfCenter(selectedName, out string errMsg))
            {
                if (!EnvChecker.Check(out errMsg))
                {
                    MessageBox.Show("该项目配置不可用，请重新选择--" + errMsg, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                MessageBox.Show("解决方案已切换到 (" + selectedName + ")", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadSolutionList();
                
            }
            else
            {
                MessageBox.Show("切换解决方案出错:" + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            string deleteName = this.confCenterList.Text;
            if (!config.IsExistSolution(deleteName, out string errMsg))
            {
                MessageBox.Show("该解决方案名称不存在,请重新选择!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(config.DeleteSolution(deleteName, out errMsg))
            {
                MessageBox.Show("删除解决方案: (" + deleteName + ") 成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadSolutionList();
            }
            else
            {
                MessageBox.Show("删除解决方案： (" + deleteName + ")  失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }   
        }

        private void SolutionConf_Load(object sender, EventArgs e)
        {
            if (Config.ServerIsRuning)
            {
                btn_submit.Enabled = false;
                btn_delete.Enabled = false;
            }
            ReloadSolutionList();
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            string newName = this.confCenterList.Text;
            
            if (config.IsExistSolution(newName, out string errMsg))
            {
                MessageBox.Show("该解决方案名称已存在,请重新输入!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LastAddSolutionName = newName;

            if (config.AddSolution(newName, out errMsg))
            {
                if (Config.ServerIsRuning)
                {
                    MessageBox.Show("新增解决方案： (" + newName + ") 成功!","信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ReloadSolutionList();
                    return;
                }
                DialogResult result = MessageBox.Show("新增解决方案： (" + newName + ") 成功,是否切换成当前解决方案?", "信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                switch (result)
                {
                    case DialogResult.OK:
                        ChangeConfCenter(LastAddSolutionName);
                        break;
                    case DialogResult.Cancel:
                        break;
                }

                ReloadSolutionList();
                
            }
            else
            {
                MessageBox.Show("新增解决方案： (" + newName + ")  失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }

        private void ReloadSolutionList()
        {
            this.confCenterList.Items.Clear();
            foreach (string item in config.GetSolutionList())
            {
                this.confCenterList.Items.Add(item);
                if (item == Config.envConfigInfo.confFileInfo.SolutionName)
                {
                    confCenterList.Text = item;
                    confCenterList.SelectedItem = item;
                }
            }
        }

    }
}
