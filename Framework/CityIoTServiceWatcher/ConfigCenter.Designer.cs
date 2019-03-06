namespace CityIoTServiceWatcher
{
    partial class ConfigCenter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("解决方案");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("数据库配置");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("日志中心");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("控制中心");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("发布中心");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("基础配置中心", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("服务配置中心");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigCenter));
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemForAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemForDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.treeView = new System.Windows.Forms.TreeView();
            this.panelConf = new System.Windows.Forms.Panel();
            this.contextMenuStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemForAdd,
            this.toolStripMenuItemForDelete});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(137, 48);
            // 
            // toolStripMenuItemForAdd
            // 
            this.toolStripMenuItemForAdd.Name = "toolStripMenuItemForAdd";
            this.toolStripMenuItemForAdd.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItemForAdd.Text = "添加子服务";
            this.toolStripMenuItemForAdd.Click += new System.EventHandler(this.toolStripMenuItemForAdd_Click);
            // 
            // toolStripMenuItemForDelete
            // 
            this.toolStripMenuItemForDelete.Name = "toolStripMenuItemForDelete";
            this.toolStripMenuItemForDelete.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItemForDelete.Text = "删除子服务";
            this.toolStripMenuItemForDelete.Click += new System.EventHandler(this.toolStripMenuItemForDelete_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.treeView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(186, 381);
            this.panel1.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.ItemHeight = 19;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            treeNode1.Checked = true;
            treeNode1.Name = "projectNameConf";
            treeNode1.Text = "解决方案";
            treeNode2.Name = "DBConf";
            treeNode2.Text = "数据库配置";
            treeNode3.Name = "logConf";
            treeNode3.Text = "日志中心";
            treeNode4.Name = "commandConf";
            treeNode4.Text = "控制中心";
            treeNode5.Name = "publishConf";
            treeNode5.Text = "发布中心";
            treeNode6.Checked = true;
            treeNode6.Name = "节点0";
            treeNode6.Text = "基础配置中心";
            treeNode7.Name = "treeSonService";
            treeNode7.Text = "服务配置中心";
            this.treeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode7});
            this.treeView.Size = new System.Drawing.Size(186, 381);
            this.treeView.TabIndex = 1;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseDown);
            // 
            // panelConf
            // 
            this.panelConf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelConf.Location = new System.Drawing.Point(186, 0);
            this.panelConf.Name = "panelConf";
            this.panelConf.Size = new System.Drawing.Size(512, 381);
            this.panelConf.TabIndex = 1;
            // 
            // ConfigCenter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 381);
            this.Controls.Add(this.panelConf);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigCenter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "配置中心";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigCenter_FormClosing);
            this.Load += new System.EventHandler(this.ConfigCenter_Load);
            this.contextMenuStrip.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Panel panelConf;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemForAdd;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemForDelete;
    }
}