namespace CityIoTServiceWatcher
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.服务ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_isStart = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_isStop = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_isRestart = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem_install = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_uninstall = new System.Windows.Forms.ToolStripMenuItem();
            this.日志ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Clear = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_showlog = new System.Windows.Forms.ToolStripMenuItem();
            this.配置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_ConfigDB = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.productHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_IsAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.iStart = new System.Windows.Forms.ToolStripButton();
            this.isStop = new System.Windows.Forms.ToolStripButton();
            this.IsRestart = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.isInstal = new System.Windows.Forms.ToolStripButton();
            this.isUnInstal = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.iClear = new System.Windows.Forms.ToolStripButton();
            this.textBoxTrace = new System.Windows.Forms.RichTextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatuslabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel6 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusProjectName = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.realtimer = new System.Windows.Forms.Timer(this.components);
            this.helpProviderForProduct = new System.Windows.Forms.HelpProvider();
            this.menuStrip1.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.服务ToolStripMenuItem,
            this.日志ToolStripMenuItem,
            this.配置ToolStripMenuItem,
            this.帮助ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(998, 25);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 服务ToolStripMenuItem
            // 
            this.服务ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_isStart,
            this.ToolStripMenuItem_isStop,
            this.ToolStripMenuItem_isRestart,
            this.toolStripSeparator4,
            this.toolStripMenuItem_install,
            this.toolStripMenuItem_uninstall});
            this.服务ToolStripMenuItem.Name = "服务ToolStripMenuItem";
            this.服务ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.服务ToolStripMenuItem.Text = "服务";
            // 
            // ToolStripMenuItem_isStart
            // 
            this.ToolStripMenuItem_isStart.Image = global::CityIoTServiceWatcher.Properties.Resources.Status;
            this.ToolStripMenuItem_isStart.Name = "ToolStripMenuItem_isStart";
            this.ToolStripMenuItem_isStart.Size = new System.Drawing.Size(100, 22);
            this.ToolStripMenuItem_isStart.Text = "启动";
            this.ToolStripMenuItem_isStart.Click += new System.EventHandler(this.iStart_Click);
            // 
            // ToolStripMenuItem_isStop
            // 
            this.ToolStripMenuItem_isStop.Image = global::CityIoTServiceWatcher.Properties.Resources.Stop;
            this.ToolStripMenuItem_isStop.Name = "ToolStripMenuItem_isStop";
            this.ToolStripMenuItem_isStop.Size = new System.Drawing.Size(100, 22);
            this.ToolStripMenuItem_isStop.Text = "停止";
            this.ToolStripMenuItem_isStop.Click += new System.EventHandler(this.isStop_Click);
            // 
            // ToolStripMenuItem_isRestart
            // 
            this.ToolStripMenuItem_isRestart.Image = global::CityIoTServiceWatcher.Properties.Resources.Refresh;
            this.ToolStripMenuItem_isRestart.Name = "ToolStripMenuItem_isRestart";
            this.ToolStripMenuItem_isRestart.Size = new System.Drawing.Size(100, 22);
            this.ToolStripMenuItem_isRestart.Text = "重启";
            this.ToolStripMenuItem_isRestart.Click += new System.EventHandler(this.IsRestart_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(97, 6);
            // 
            // toolStripMenuItem_install
            // 
            this.toolStripMenuItem_install.Image = global::CityIoTServiceWatcher.Properties.Resources.Chat;
            this.toolStripMenuItem_install.Name = "toolStripMenuItem_install";
            this.toolStripMenuItem_install.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItem_install.Text = "注册";
            this.toolStripMenuItem_install.Click += new System.EventHandler(this.isInstal_Click);
            // 
            // toolStripMenuItem_uninstall
            // 
            this.toolStripMenuItem_uninstall.Image = global::CityIoTServiceWatcher.Properties.Resources.ClearScript;
            this.toolStripMenuItem_uninstall.Name = "toolStripMenuItem_uninstall";
            this.toolStripMenuItem_uninstall.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItem_uninstall.Text = "卸载";
            this.toolStripMenuItem_uninstall.Click += new System.EventHandler(this.isUnInstal_Click);
            // 
            // 日志ToolStripMenuItem
            // 
            this.日志ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Clear,
            this.ToolStripMenuItem_showlog});
            this.日志ToolStripMenuItem.Name = "日志ToolStripMenuItem";
            this.日志ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.日志ToolStripMenuItem.Text = "日志";
            // 
            // ToolStripMenuItem_Clear
            // 
            this.ToolStripMenuItem_Clear.Image = global::CityIoTServiceWatcher.Properties.Resources.Status_Gray;
            this.ToolStripMenuItem_Clear.Name = "ToolStripMenuItem_Clear";
            this.ToolStripMenuItem_Clear.Size = new System.Drawing.Size(124, 22);
            this.ToolStripMenuItem_Clear.Text = "清屏";
            this.ToolStripMenuItem_Clear.Click += new System.EventHandler(this.iClear_Click);
            // 
            // ToolStripMenuItem_showlog
            // 
            this.ToolStripMenuItem_showlog.Image = global::CityIoTServiceWatcher.Properties.Resources.Log;
            this.ToolStripMenuItem_showlog.Name = "ToolStripMenuItem_showlog";
            this.ToolStripMenuItem_showlog.Size = new System.Drawing.Size(124, 22);
            this.ToolStripMenuItem_showlog.Text = "查看日志";
            this.ToolStripMenuItem_showlog.Click += new System.EventHandler(this.iViewLog_Click);
            // 
            // 配置ToolStripMenuItem
            // 
            this.配置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_ConfigDB});
            this.配置ToolStripMenuItem.Name = "配置ToolStripMenuItem";
            this.配置ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.配置ToolStripMenuItem.Text = "配置";
            // 
            // ToolStripMenuItem_ConfigDB
            // 
            this.ToolStripMenuItem_ConfigDB.Image = global::CityIoTServiceWatcher.Properties.Resources.Config;
            this.ToolStripMenuItem_ConfigDB.Name = "ToolStripMenuItem_ConfigDB";
            this.ToolStripMenuItem_ConfigDB.Size = new System.Drawing.Size(152, 22);
            this.ToolStripMenuItem_ConfigDB.Text = "配置中心";
            this.ToolStripMenuItem_ConfigDB.Click += new System.EventHandler(this.configSet_Click);
            // 
            // 帮助ToolStripMenuItem
            // 
            this.帮助ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.productHelp,
            this.ToolStripMenuItem_IsAbout});
            this.帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
            this.帮助ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.帮助ToolStripMenuItem.Text = "帮助";
            // 
            // productHelp
            // 
            this.productHelp.Image = global::CityIoTServiceWatcher.Properties.Resources.Log;
            this.productHelp.Name = "productHelp";
            this.productHelp.Size = new System.Drawing.Size(152, 22);
            this.productHelp.Text = "产品帮助";
            this.productHelp.Click += new System.EventHandler(this.ProductHelp_Click);
            // 
            // ToolStripMenuItem_IsAbout
            // 
            this.ToolStripMenuItem_IsAbout.Image = global::CityIoTServiceWatcher.Properties.Resources.Monitor;
            this.ToolStripMenuItem_IsAbout.Name = "ToolStripMenuItem_IsAbout";
            this.ToolStripMenuItem_IsAbout.Size = new System.Drawing.Size(152, 22);
            this.ToolStripMenuItem_IsAbout.Text = "关于";
            this.ToolStripMenuItem_IsAbout.Click += new System.EventHandler(this.isAbout_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.GripMargin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iStart,
            this.isStop,
            this.IsRestart,
            this.toolStripSeparator3,
            this.isInstal,
            this.isUnInstal,
            this.toolStripSeparator2,
            this.toolStripSeparator1,
            this.iClear});
            this.toolStrip.Location = new System.Drawing.Point(0, 25);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.Size = new System.Drawing.Size(998, 27);
            this.toolStrip.TabIndex = 12;
            this.toolStrip.Text = "toolStrip1";
            // 
            // iStart
            // 
            this.iStart.Enabled = false;
            this.iStart.Image = ((System.Drawing.Image)(resources.GetObject("iStart.Image")));
            this.iStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.iStart.Name = "iStart";
            this.iStart.Size = new System.Drawing.Size(56, 24);
            this.iStart.Text = "启动";
            this.iStart.Click += new System.EventHandler(this.iStart_Click);
            // 
            // isStop
            // 
            this.isStop.Enabled = false;
            this.isStop.Image = ((System.Drawing.Image)(resources.GetObject("isStop.Image")));
            this.isStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.isStop.Name = "isStop";
            this.isStop.Size = new System.Drawing.Size(56, 24);
            this.isStop.Text = "停止";
            this.isStop.Click += new System.EventHandler(this.isStop_Click);
            // 
            // IsRestart
            // 
            this.IsRestart.Enabled = false;
            this.IsRestart.Image = ((System.Drawing.Image)(resources.GetObject("IsRestart.Image")));
            this.IsRestart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.IsRestart.Name = "IsRestart";
            this.IsRestart.Size = new System.Drawing.Size(56, 24);
            this.IsRestart.Text = "重启";
            this.IsRestart.Click += new System.EventHandler(this.IsRestart_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // isInstal
            // 
            this.isInstal.Enabled = false;
            this.isInstal.Image = ((System.Drawing.Image)(resources.GetObject("isInstal.Image")));
            this.isInstal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.isInstal.Name = "isInstal";
            this.isInstal.Size = new System.Drawing.Size(56, 24);
            this.isInstal.Text = "注册";
            this.isInstal.ToolTipText = "注册";
            this.isInstal.Click += new System.EventHandler(this.isInstal_Click);
            // 
            // isUnInstal
            // 
            this.isUnInstal.Enabled = false;
            this.isUnInstal.Image = ((System.Drawing.Image)(resources.GetObject("isUnInstal.Image")));
            this.isUnInstal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.isUnInstal.Name = "isUnInstal";
            this.isUnInstal.Size = new System.Drawing.Size(56, 24);
            this.isUnInstal.Text = "卸载";
            this.isUnInstal.ToolTipText = "卸载";
            this.isUnInstal.Click += new System.EventHandler(this.isUnInstal_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // iClear
            // 
            this.iClear.Image = global::CityIoTServiceWatcher.Properties.Resources.Status_Gray;
            this.iClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.iClear.Name = "iClear";
            this.iClear.Size = new System.Drawing.Size(56, 24);
            this.iClear.Text = "清屏";
            this.iClear.Click += new System.EventHandler(this.iClear_Click);
            // 
            // textBoxTrace
            // 
            this.textBoxTrace.BackColor = System.Drawing.Color.DimGray;
            this.textBoxTrace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxTrace.Font = new System.Drawing.Font("新宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxTrace.ForeColor = System.Drawing.Color.Lime;
            this.textBoxTrace.Location = new System.Drawing.Point(0, 52);
            this.textBoxTrace.Name = "textBoxTrace";
            this.textBoxTrace.ReadOnly = true;
            this.textBoxTrace.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textBoxTrace.Size = new System.Drawing.Size(998, 395);
            this.textBoxTrace.TabIndex = 13;
            this.textBoxTrace.Text = "";
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatuslabel,
            this.toolStripStatusLabel6,
            this.toolStripStatusProjectName,
            this.toolStripStatusLabel3,
            this.toolStripStatusLabel4,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel5,
            this.toolStripTime});
            this.statusStrip.Location = new System.Drawing.Point(0, 425);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(998, 22);
            this.statusStrip.TabIndex = 14;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatuslabel
            // 
            this.toolStripStatuslabel.Name = "toolStripStatuslabel";
            this.toolStripStatuslabel.Size = new System.Drawing.Size(68, 17);
            this.toolStripStatuslabel.Text = "解决方案：";
            // 
            // toolStripStatusLabel6
            // 
            this.toolStripStatusLabel6.Name = "toolStripStatusLabel6";
            this.toolStripStatusLabel6.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusProjectName
            // 
            this.toolStripStatusProjectName.Name = "toolStripStatusProjectName";
            this.toolStripStatusProjectName.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(68, 17);
            this.toolStripStatusLabel3.Text = "系统状态：";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(44, 17);
            this.toolStripStatusLabel4.Text = "已停止";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(609, 17);
            this.toolStripStatusLabel2.Spring = true;
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(68, 17);
            this.toolStripStatusLabel5.Text = "当前时间：";
            // 
            // toolStripTime
            // 
            this.toolStripTime.Name = "toolStripTime";
            this.toolStripTime.Size = new System.Drawing.Size(126, 17);
            this.toolStripTime.Text = "1999-09-09 09:09:09";
            // 
            // realtimer
            // 
            this.realtimer.Enabled = true;
            this.realtimer.Interval = 1000;
            this.realtimer.Tick += new System.EventHandler(this.realtimer_Tick);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(998, 447);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.textBoxTrace);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "熊猫智慧水务数据服务管理器";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 服务ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_isStart;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_isStop;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_isRestart;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_install;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_uninstall;
        private System.Windows.Forms.ToolStripMenuItem 日志ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Clear;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_showlog;
        private System.Windows.Forms.ToolStripMenuItem 配置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_ConfigDB;
        private System.Windows.Forms.ToolStripMenuItem 帮助ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem productHelp;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_IsAbout;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton iStart;
        private System.Windows.Forms.ToolStripButton isStop;
        private System.Windows.Forms.ToolStripButton IsRestart;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton isInstal;
        private System.Windows.Forms.ToolStripButton isUnInstal;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton iClear;
        private System.Windows.Forms.RichTextBox textBoxTrace;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatuslabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel6;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.ToolStripStatusLabel toolStripTime;
        private System.Windows.Forms.Timer realtimer;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusProjectName;
        private System.Windows.Forms.HelpProvider helpProviderForProduct;
    }
}