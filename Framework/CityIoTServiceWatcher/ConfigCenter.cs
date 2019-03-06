using CityPublicClassLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CityIoTServiceWatcher
{
    public partial class ConfigCenter : Form
    {
        public ConfigCenter()
        {
            InitializeComponent();
        }
       
        private void ConfigCenter_Load(object sender, EventArgs e)
        {
            Config.envConfigInfo.evtConfigIsChanged += Reload;
            Reload();
            // 默认值
            this.CheckTreeFirstNode();
            this.ShowPanel("");
        }

        private void ReloadTreeNode()
        {
            List<string> list = Config.envConfigInfo.GetRunServiceList(Config.envConfigInfo.confFileInfo.ProjectConfigPath, out string errMsg);

            foreach (TreeNode node in treeView.Nodes)
            {
                if (node.Text == "服务配置中心")
                {
                    node.Nodes.Clear();
                    foreach (string name in list)
                    {
                        node.Nodes.Add(name);
                        foreach (TreeNode childNode in node.Nodes)
                        {
                            childNode.ContextMenuStrip = contextMenuStrip;
                        }
                    }
                }
            }
        }

        private void CheckTreeFirstNode()
        {
            foreach (TreeNode node in this.treeView.Nodes)
            {
                foreach (TreeNode sonNode in node.Nodes)
                {
                    treeView.SelectedNode = sonNode;
                    break;
                }
                break;
            }
        }

        private void ShowPanel(string checkedName)
        {
            if (string.IsNullOrWhiteSpace(checkedName))
            {
                // 只有两层
                foreach (TreeNode node in this.treeView.Nodes)
                {
                    foreach (TreeNode sonNode in node.Nodes)
                    {
                        if (sonNode.Checked)
                        {
                            checkedName = sonNode.Text;
                        }
                    }
                }
            }
            this.LoadPanel(checkedName);
        }
        private void LoadPanel(string panelName)
        {
            if (string.IsNullOrWhiteSpace(panelName))
                return;
            // 清空内容区
            foreach (Control control in this.panelConf.Controls)
            {
                this.panelConf.Controls.Remove(control);
            }
           
            switch (panelName)
            {
                case "解决方案":
                    SolutionConf solutionConf = new SolutionConf();
                    addPanel(solutionConf);
                    break;
                case "数据库配置":
                    DBConf dbConf = new DBConf();
                    addPanel(dbConf);
                    break;
                case "日志中心":
                    LogConf logConf = new LogConf();
                    addPanel(logConf);
                    break;
                case "控制中心":
                    CommandConf commandConf = new CommandConf();
                    addPanel(commandConf);
                    break;
                case "发布中心":
                    PublishConf publishConf = new PublishConf();
                    addPanel(publishConf);
                    break;
                case "服务配置中心":
                    AddSonService addSonService = new AddSonService();
                    addSonService.evtSubmit += AddSonServiceToPanel;
                    addPanel(addSonService);
                    break;
                case "OPC二供接入服务":
                    OPCPumpService opcPumpService = new OPCPumpService();
                    opcPumpService.evtSubmit += AddSonServiceToTreeNode;
                    addPanel(opcPumpService);
                    break;
                case "OPC-SCADA接入服务":
                    OPCScadaServices opcScadaServices = new OPCScadaServices();
                    opcScadaServices.evtSubmit += AddSonServiceToTreeNode;
                    addPanel(opcScadaServices);
                    break;
                case "二供报警服务":
                    PumpAlarmService pumpAlarmService = new PumpAlarmService();
                    pumpAlarmService.evtSubmit += AddSonServiceToTreeNode;
                    addPanel(pumpAlarmService);
                    break;
                case "WEB熊猫二供接入服务":
                    WEBPandaPumpService webPandaPumpService = new WEBPandaPumpService();
                    webPandaPumpService.evtSubmit += AddSonServiceToTreeNode;
                    addPanel(webPandaPumpService);
                    break;
                case "WEB熊猫二供SCADA接入服务":
                    WEBPandaPumpScadaService webPandaPumpScadaService = new WEBPandaPumpScadaService();
                    webPandaPumpScadaService.evtSubmit += AddSonServiceToTreeNode;
                    addPanel(webPandaPumpScadaService);
                    break;
                case "WEB熊猫监测点接入服务":
                    WEBPandaYLService webPandaYLService = new WEBPandaYLService();
                    webPandaYLService.evtSubmit += AddSonServiceToTreeNode;
                    addPanel(webPandaYLService);
                    break;
                case "WEB熊猫综合测点接入服务":
                    WEBPandaZHCDService webPandaZHCDService = new WEBPandaZHCDService();
                    webPandaZHCDService.evtSubmit += AddSonServiceToTreeNode;
                    addPanel(webPandaZHCDService);
                    break;
                case "WEB熊猫控制服务":
                    WebPandaControlService webPandaControlService = new WebPandaControlService();
                    webPandaControlService.evtSubmit += AddSonServiceToTreeNode;
                    addPanel(webPandaControlService);
                    break;
                case "项目的特殊服务":
                    SpecialProjectService specialProjectService = new SpecialProjectService();
                    specialProjectService.evtSubmit += AddSonServiceToTreeNode;
                    addPanel(specialProjectService);
                    break;
                case "历史抽稀服务":
                    HisVacuateService hisVacuateService = new HisVacuateService();
                    hisVacuateService.evtSubmit += AddSonServiceToTreeNode;
                    addPanel(hisVacuateService);
                    break;
            }
        }

        private void AddSonServiceToPanel(string serivceName)
        {
            LoadPanel(serivceName);
        }

        private void AddSonServiceToTreeNode(string serviceName, EnvConfigInfo.Status code)
        {   
            ShowPanel("服务配置中心");
        }

        private void addPanel(UserControl obj)
        {
            this.panelConf.Controls.Add(obj);
            obj.Dock = System.Windows.Forms.DockStyle.Fill;
            obj.Location = new System.Drawing.Point(0, 0);
            obj.Size = new System.Drawing.Size(512, 381);
            obj.TabIndex = 0;
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            LoadPanel(e.Node.Text.Trim());
        }

        private void toolStripMenuItemForAdd_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in this.treeView.Nodes)
            {
                if (node.Text == "服务配置中心")
                {
                    treeView.SelectedNode = node;
                    break;
                }
            }
            ShowPanel("服务配置中心");
        }

        private void toolStripMenuItemForDelete_Click(object sender, EventArgs e)
        {
            
            if (sender == null || Config.ServerIsRuning)
                return;

            string checkName = treeView.SelectedNode.Text;

            if (Config.envConfigInfo.RemoveSonService(checkName, out string errMsg))
            {
                MessageBox.Show("删除 " + checkName +"  配置信息成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowPanel("服务配置中心");
            }
            else
            {
                MessageBox.Show("删除 " + checkName + "  配置信息失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                System.Drawing.Point ClickPoint = new System.Drawing.Point(e.X, e.Y);
                TreeNode CurrentNode = treeView.GetNodeAt(ClickPoint);
                if (CurrentNode != null)//判断你点的是不是一个节点
                {
                    treeView.SelectedNode = CurrentNode;//选中这个节点
                }
            }
        }

        private void ConfigCenter_FormClosing(object sender, FormClosingEventArgs e)
        {
            Config.envConfigInfo.evtConfigIsChanged -= Reload;
        }

        private void Reload()
        {
            ReloadTreeNode();
            this.treeView.ExpandAll();
        }
    }
}
