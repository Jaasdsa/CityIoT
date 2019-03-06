namespace DTUGenerator
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.contextMenuStrip_log = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_clearLog = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.contextMenuStrip_reciveLog = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_clearReciveLog = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dataGridView_commands = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sensorID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sensorValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.beginTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.耗时 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mess = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.numeric_write = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.btnDelCommands = new System.Windows.Forms.Button();
            this.numeric_read = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.btn_creatSomeCommands = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btnRefreshCommands = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxLog = new System.Windows.Forms.RichTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxRecive = new System.Windows.Forms.RichTextBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButton_16Hex = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.btnSendOneData = new System.Windows.Forms.Button();
            this.numericSendTimeSpan = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textSendText = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.IsStoptDataTest = new System.Windows.Forms.Button();
            this.IsStartDataTest = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.radioButton_delay = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.isStopAnswer = new System.Windows.Forms.Button();
            this.isStartAnswer = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.btnSendOncePLCData = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.isStopSendPLC = new System.Windows.Forms.Button();
            this.numericUpDownPLCTimeSpan = new System.Windows.Forms.NumericUpDown();
            this.isStartSendPLC = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnTestOnceHeart = new System.Windows.Forms.Button();
            this.numericHeartTimeSpan = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.isStopXinTiao = new System.Windows.Forms.Button();
            this.isStartXinTiao = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bttnIsDisconnect = new System.Windows.Forms.Button();
            this.numericPort = new System.Windows.Forms.NumericUpDown();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textYuMing = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textTel = new System.Windows.Forms.TextBox();
            this.contextMenuStrip_log.SuspendLayout();
            this.contextMenuStrip_reciveLog.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_commands)).BeginInit();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_write)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_read)).BeginInit();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSendTimeSpan)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPLCTimeSpan)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericHeartTimeSpan)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPort)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip_log
            // 
            this.contextMenuStrip_log.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_clearLog});
            this.contextMenuStrip_log.Name = "contextMenuStrip_log";
            this.contextMenuStrip_log.Size = new System.Drawing.Size(101, 26);
            // 
            // ToolStripMenuItem_clearLog
            // 
            this.ToolStripMenuItem_clearLog.Name = "ToolStripMenuItem_clearLog";
            this.ToolStripMenuItem_clearLog.Size = new System.Drawing.Size(100, 22);
            this.ToolStripMenuItem_clearLog.Text = "清空";
            this.ToolStripMenuItem_clearLog.Click += new System.EventHandler(this.ToolStripMenuItem_clearLog_Click);
            // 
            // contextMenuStrip_reciveLog
            // 
            this.contextMenuStrip_reciveLog.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_clearReciveLog});
            this.contextMenuStrip_reciveLog.Name = "contextMenuStrip_reciveLog";
            this.contextMenuStrip_reciveLog.Size = new System.Drawing.Size(101, 26);
            // 
            // ToolStripMenuItem_clearReciveLog
            // 
            this.ToolStripMenuItem_clearReciveLog.Name = "ToolStripMenuItem_clearReciveLog";
            this.ToolStripMenuItem_clearReciveLog.Size = new System.Drawing.Size(100, 22);
            this.ToolStripMenuItem_clearReciveLog.Text = "清空";
            this.ToolStripMenuItem_clearReciveLog.Click += new System.EventHandler(this.ToolStripMenuItem_clearReciveLog_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1278, 861);
            this.panel1.TabIndex = 33;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.groupBox9);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 626);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1278, 235);
            this.panel3.TabIndex = 1;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.dataGridView_commands);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(279, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(999, 235);
            this.panel4.TabIndex = 1;
            // 
            // dataGridView_commands
            // 
            this.dataGridView_commands.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.type,
            this.sensorID,
            this.sensorValue,
            this.status,
            this.beginTime,
            this.endTime,
            this.耗时,
            this.Mess});
            this.dataGridView_commands.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_commands.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_commands.Name = "dataGridView_commands";
            this.dataGridView_commands.RowTemplate.Height = 23;
            this.dataGridView_commands.Size = new System.Drawing.Size(999, 235);
            this.dataGridView_commands.TabIndex = 0;
            // 
            // ID
            // 
            this.ID.Frozen = true;
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.Width = 42;
            // 
            // type
            // 
            this.type.Frozen = true;
            this.type.HeaderText = "类型";
            this.type.Name = "type";
            this.type.Width = 54;
            // 
            // sensorID
            // 
            this.sensorID.Frozen = true;
            this.sensorID.HeaderText = "sensorID";
            this.sensorID.Name = "sensorID";
            this.sensorID.Width = 60;
            // 
            // sensorValue
            // 
            this.sensorValue.Frozen = true;
            this.sensorValue.HeaderText = "sensorValue";
            this.sensorValue.Name = "sensorValue";
            this.sensorValue.Width = 80;
            // 
            // status
            // 
            this.status.Frozen = true;
            this.status.HeaderText = "任务状态";
            this.status.Name = "status";
            this.status.Width = 80;
            // 
            // beginTime
            // 
            this.beginTime.HeaderText = "任务开始时间";
            this.beginTime.Name = "beginTime";
            this.beginTime.Width = 155;
            // 
            // endTime
            // 
            this.endTime.HeaderText = "任务结束时间";
            this.endTime.Name = "endTime";
            this.endTime.Width = 155;
            // 
            // 耗时
            // 
            this.耗时.HeaderText = "耗时";
            this.耗时.Name = "耗时";
            this.耗时.Width = 120;
            // 
            // Mess
            // 
            this.Mess.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Mess.HeaderText = "备注";
            this.Mess.Name = "Mess";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.label18);
            this.groupBox9.Controls.Add(this.label17);
            this.groupBox9.Controls.Add(this.numeric_write);
            this.groupBox9.Controls.Add(this.label15);
            this.groupBox9.Controls.Add(this.btnDelCommands);
            this.groupBox9.Controls.Add(this.numeric_read);
            this.groupBox9.Controls.Add(this.label16);
            this.groupBox9.Controls.Add(this.btn_creatSomeCommands);
            this.groupBox9.Controls.Add(this.label14);
            this.groupBox9.Controls.Add(this.label13);
            this.groupBox9.Controls.Add(this.label12);
            this.groupBox9.Controls.Add(this.btnRefreshCommands);
            this.groupBox9.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox9.Location = new System.Drawing.Point(0, 0);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(279, 235);
            this.groupBox9.TabIndex = 0;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "操作票";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(11, 46);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(17, 12);
            this.label18.TabIndex = 35;
            this.label18.Text = "写";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(193, 46);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(17, 12);
            this.label17.TabIndex = 34;
            this.label17.Text = "张";
            // 
            // numeric_write
            // 
            this.numeric_write.Location = new System.Drawing.Point(58, 42);
            this.numeric_write.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numeric_write.Name = "numeric_write";
            this.numeric_write.Size = new System.Drawing.Size(108, 21);
            this.numeric_write.TabIndex = 33;
            this.numeric_write.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(193, 23);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(17, 12);
            this.label15.TabIndex = 32;
            this.label15.Text = "张";
            // 
            // btnDelCommands
            // 
            this.btnDelCommands.Location = new System.Drawing.Point(142, 69);
            this.btnDelCommands.Name = "btnDelCommands";
            this.btnDelCommands.Size = new System.Drawing.Size(110, 23);
            this.btnDelCommands.TabIndex = 23;
            this.btnDelCommands.Text = "删除操作票";
            this.btnDelCommands.UseVisualStyleBackColor = true;
            this.btnDelCommands.Click += new System.EventHandler(this.btnDelCommands_Click);
            // 
            // numeric_read
            // 
            this.numeric_read.Location = new System.Drawing.Point(58, 19);
            this.numeric_read.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numeric_read.Name = "numeric_read";
            this.numeric_read.Size = new System.Drawing.Size(108, 21);
            this.numeric_read.TabIndex = 31;
            this.numeric_read.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(11, 23);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(29, 12);
            this.label16.TabIndex = 30;
            this.label16.Text = "读：";
            // 
            // btn_creatSomeCommands
            // 
            this.btn_creatSomeCommands.Location = new System.Drawing.Point(13, 69);
            this.btn_creatSomeCommands.Name = "btn_creatSomeCommands";
            this.btn_creatSomeCommands.Size = new System.Drawing.Size(110, 23);
            this.btn_creatSomeCommands.TabIndex = 22;
            this.btn_creatSomeCommands.Text = "生成操作票";
            this.btn_creatSomeCommands.UseVisualStyleBackColor = true;
            this.btn_creatSomeCommands.Click += new System.EventHandler(this.btn_creatSomeCommands_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.Color.Red;
            this.label14.Location = new System.Drawing.Point(61, 144);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(125, 12);
            this.label14.TabIndex = 21;
            this.label14.Text = "请手动刷新操作票状态";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.Color.Red;
            this.label13.Location = new System.Drawing.Point(7, 122);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(77, 12);
            this.label13.TabIndex = 20;
            this.label13.Text = "客户端过多，";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(6, 102);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(41, 12);
            this.label12.TabIndex = 19;
            this.label12.Text = "提示：";
            // 
            // btnRefreshCommands
            // 
            this.btnRefreshCommands.Location = new System.Drawing.Point(13, 179);
            this.btnRefreshCommands.Name = "btnRefreshCommands";
            this.btnRefreshCommands.Size = new System.Drawing.Size(110, 23);
            this.btnRefreshCommands.TabIndex = 0;
            this.btnRefreshCommands.Text = "刷新命令表";
            this.btnRefreshCommands.UseVisualStyleBackColor = true;
            this.btnRefreshCommands.Click += new System.EventHandler(this.btnRefreshCommands_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.groupBox8);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1278, 626);
            this.panel2.TabIndex = 0;
            this.panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseDown);
            this.panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseMove);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxLog);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(702, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(576, 626);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "日志";
            // 
            // textBoxLog
            // 
            this.textBoxLog.ContextMenuStrip = this.contextMenuStrip_log;
            this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLog.Location = new System.Drawing.Point(3, 17);
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.Size = new System.Drawing.Size(570, 606);
            this.textBoxLog.TabIndex = 0;
            this.textBoxLog.Text = "";
            this.textBoxLog.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxLog_MouseDown);
            this.textBoxLog.MouseMove += new System.Windows.Forms.MouseEventHandler(this.textBoxLog_MouseMove);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxRecive);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox3.Location = new System.Drawing.Point(279, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(423, 626);
            this.groupBox3.TabIndex = 34;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "接收数据";
            // 
            // textBoxRecive
            // 
            this.textBoxRecive.ContextMenuStrip = this.contextMenuStrip_reciveLog;
            this.textBoxRecive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxRecive.Location = new System.Drawing.Point(3, 17);
            this.textBoxRecive.Name = "textBoxRecive";
            this.textBoxRecive.ReadOnly = true;
            this.textBoxRecive.Size = new System.Drawing.Size(417, 606);
            this.textBoxRecive.TabIndex = 1;
            this.textBoxRecive.Text = "";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.groupBox4);
            this.groupBox8.Controls.Add(this.groupBox6);
            this.groupBox8.Controls.Add(this.groupBox7);
            this.groupBox8.Controls.Add(this.groupBox5);
            this.groupBox8.Controls.Add(this.groupBox1);
            this.groupBox8.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox8.Location = new System.Drawing.Point(0, 0);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(30);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(279, 626);
            this.groupBox8.TabIndex = 33;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "操作";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButton_16Hex);
            this.groupBox4.Controls.Add(this.radioButton1);
            this.groupBox4.Controls.Add(this.btnSendOneData);
            this.groupBox4.Controls.Add(this.numericSendTimeSpan);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.textSendText);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.IsStoptDataTest);
            this.groupBox4.Controls.Add(this.IsStartDataTest);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox4.Location = new System.Drawing.Point(3, 459);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(273, 180);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "数据测试";
            // 
            // radioButton_16Hex
            // 
            this.radioButton_16Hex.Location = new System.Drawing.Point(146, 77);
            this.radioButton_16Hex.Name = "radioButton_16Hex";
            this.radioButton_16Hex.Size = new System.Drawing.Size(64, 24);
            this.radioButton_16Hex.TabIndex = 28;
            this.radioButton_16Hex.Text = "16进制";
            // 
            // radioButton1
            // 
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(19, 77);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(48, 24);
            this.radioButton1.TabIndex = 27;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "文本";
            // 
            // btnSendOneData
            // 
            this.btnSendOneData.Location = new System.Drawing.Point(6, 107);
            this.btnSendOneData.Name = "btnSendOneData";
            this.btnSendOneData.Size = new System.Drawing.Size(111, 23);
            this.btnSendOneData.TabIndex = 25;
            this.btnSendOneData.Text = "单次";
            this.btnSendOneData.UseVisualStyleBackColor = true;
            this.btnSendOneData.Click += new System.EventHandler(this.btnSendOneData_Click);
            // 
            // numericSendTimeSpan
            // 
            this.numericSendTimeSpan.Location = new System.Drawing.Point(78, 17);
            this.numericSendTimeSpan.Maximum = new decimal(new int[] {
            655350000,
            0,
            0,
            0});
            this.numericSendTimeSpan.Name = "numericSendTimeSpan";
            this.numericSendTimeSpan.Size = new System.Drawing.Size(108, 21);
            this.numericSendTimeSpan.TabIndex = 26;
            this.numericSendTimeSpan.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(208, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 12);
            this.label8.TabIndex = 25;
            this.label8.Text = "毫秒";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 24;
            this.label9.Text = "测试间隔：";
            // 
            // textSendText
            // 
            this.textSendText.Location = new System.Drawing.Point(73, 54);
            this.textSendText.Name = "textSendText";
            this.textSendText.Size = new System.Drawing.Size(162, 21);
            this.textSendText.TabIndex = 19;
            this.textSendText.Text = "Hello World ！";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(2, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 18;
            this.label7.Text = "发送文本：";
            // 
            // IsStoptDataTest
            // 
            this.IsStoptDataTest.Enabled = false;
            this.IsStoptDataTest.Location = new System.Drawing.Point(129, 136);
            this.IsStoptDataTest.Name = "IsStoptDataTest";
            this.IsStoptDataTest.Size = new System.Drawing.Size(100, 23);
            this.IsStoptDataTest.TabIndex = 17;
            this.IsStoptDataTest.Text = "停止定时";
            this.IsStoptDataTest.UseVisualStyleBackColor = true;
            this.IsStoptDataTest.Click += new System.EventHandler(this.IsStoptDataTest_Click);
            // 
            // IsStartDataTest
            // 
            this.IsStartDataTest.Enabled = false;
            this.IsStartDataTest.Location = new System.Drawing.Point(6, 136);
            this.IsStartDataTest.Name = "IsStartDataTest";
            this.IsStartDataTest.Size = new System.Drawing.Size(111, 23);
            this.IsStartDataTest.TabIndex = 16;
            this.IsStartDataTest.Text = "开始定时";
            this.IsStartDataTest.UseVisualStyleBackColor = true;
            this.IsStartDataTest.Click += new System.EventHandler(this.IsStartDataTest_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.radioButton_delay);
            this.groupBox6.Controls.Add(this.radioButton2);
            this.groupBox6.Controls.Add(this.isStopAnswer);
            this.groupBox6.Controls.Add(this.isStartAnswer);
            this.groupBox6.Controls.Add(this.button3);
            this.groupBox6.Controls.Add(this.button4);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox6.Location = new System.Drawing.Point(3, 380);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(273, 79);
            this.groupBox6.TabIndex = 1;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "DTU模拟应答测试";
            // 
            // radioButton_delay
            // 
            this.radioButton_delay.AutoSize = true;
            this.radioButton_delay.Location = new System.Drawing.Point(139, 20);
            this.radioButton_delay.Name = "radioButton_delay";
            this.radioButton_delay.Size = new System.Drawing.Size(71, 16);
            this.radioButton_delay.TabIndex = 28;
            this.radioButton_delay.Text = "延迟响应";
            this.radioButton_delay.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Location = new System.Drawing.Point(18, 20);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(71, 16);
            this.radioButton2.TabIndex = 27;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "立即响应";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // isStopAnswer
            // 
            this.isStopAnswer.Location = new System.Drawing.Point(129, 42);
            this.isStopAnswer.Name = "isStopAnswer";
            this.isStopAnswer.Size = new System.Drawing.Size(88, 23);
            this.isStopAnswer.TabIndex = 26;
            this.isStopAnswer.Text = "停止";
            this.isStopAnswer.UseVisualStyleBackColor = true;
            this.isStopAnswer.Click += new System.EventHandler(this.isStopAnswer_Click);
            // 
            // isStartAnswer
            // 
            this.isStartAnswer.Location = new System.Drawing.Point(13, 42);
            this.isStartAnswer.Name = "isStartAnswer";
            this.isStartAnswer.Size = new System.Drawing.Size(87, 23);
            this.isStartAnswer.TabIndex = 25;
            this.isStartAnswer.Text = "开始";
            this.isStartAnswer.UseVisualStyleBackColor = true;
            this.isStartAnswer.Click += new System.EventHandler(this.isStartAnswer_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(129, 136);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(100, 23);
            this.button3.TabIndex = 17;
            this.button3.Text = "停止定时";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(6, 136);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(111, 23);
            this.button4.TabIndex = 16;
            this.button4.Text = "开始定时";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.btnSendOncePLCData);
            this.groupBox7.Controls.Add(this.label10);
            this.groupBox7.Controls.Add(this.isStopSendPLC);
            this.groupBox7.Controls.Add(this.numericUpDownPLCTimeSpan);
            this.groupBox7.Controls.Add(this.isStartSendPLC);
            this.groupBox7.Controls.Add(this.label11);
            this.groupBox7.Controls.Add(this.button6);
            this.groupBox7.Controls.Add(this.button7);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox7.Location = new System.Drawing.Point(3, 300);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(273, 80);
            this.groupBox7.TabIndex = 30;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "DTU模拟定时推送测试";
            // 
            // btnSendOncePLCData
            // 
            this.btnSendOncePLCData.Location = new System.Drawing.Point(3, 46);
            this.btnSendOncePLCData.Name = "btnSendOncePLCData";
            this.btnSendOncePLCData.Size = new System.Drawing.Size(80, 23);
            this.btnSendOncePLCData.TabIndex = 30;
            this.btnSendOncePLCData.Text = "单次";
            this.btnSendOncePLCData.UseVisualStyleBackColor = true;
            this.btnSendOncePLCData.Click += new System.EventHandler(this.btnSendOncePLCData_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(192, 23);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 12);
            this.label10.TabIndex = 29;
            this.label10.Text = "毫秒";
            // 
            // isStopSendPLC
            // 
            this.isStopSendPLC.Location = new System.Drawing.Point(182, 47);
            this.isStopSendPLC.Name = "isStopSendPLC";
            this.isStopSendPLC.Size = new System.Drawing.Size(88, 23);
            this.isStopSendPLC.TabIndex = 26;
            this.isStopSendPLC.Text = "停止";
            this.isStopSendPLC.UseVisualStyleBackColor = true;
            this.isStopSendPLC.Click += new System.EventHandler(this.isStopSendPLC_Click);
            // 
            // numericUpDownPLCTimeSpan
            // 
            this.numericUpDownPLCTimeSpan.Location = new System.Drawing.Point(75, 19);
            this.numericUpDownPLCTimeSpan.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownPLCTimeSpan.Name = "numericUpDownPLCTimeSpan";
            this.numericUpDownPLCTimeSpan.Size = new System.Drawing.Size(108, 21);
            this.numericUpDownPLCTimeSpan.TabIndex = 28;
            this.numericUpDownPLCTimeSpan.Value = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            // 
            // isStartSendPLC
            // 
            this.isStartSendPLC.Location = new System.Drawing.Point(89, 46);
            this.isStartSendPLC.Name = "isStartSendPLC";
            this.isStartSendPLC.Size = new System.Drawing.Size(87, 23);
            this.isStartSendPLC.TabIndex = 25;
            this.isStartSendPLC.Text = "开始";
            this.isStartSendPLC.UseVisualStyleBackColor = true;
            this.isStartSendPLC.Click += new System.EventHandler(this.isStartSendPLC_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 23);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 27;
            this.label11.Text = "测试间隔：";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(129, 136);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(100, 23);
            this.button6.TabIndex = 17;
            this.button6.Text = "停止定时";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(6, 136);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(111, 23);
            this.button7.TabIndex = 16;
            this.button7.Text = "开始定时";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.btnTestOnceHeart);
            this.groupBox5.Controls.Add(this.numericHeartTimeSpan);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.isStopXinTiao);
            this.groupBox5.Controls.Add(this.isStartXinTiao);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox5.Location = new System.Drawing.Point(3, 185);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(273, 115);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "心跳测试";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(192, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 26;
            this.label6.Text = "毫秒";
            // 
            // btnTestOnceHeart
            // 
            this.btnTestOnceHeart.Location = new System.Drawing.Point(15, 46);
            this.btnTestOnceHeart.Name = "btnTestOnceHeart";
            this.btnTestOnceHeart.Size = new System.Drawing.Size(83, 23);
            this.btnTestOnceHeart.TabIndex = 24;
            this.btnTestOnceHeart.Text = "单次";
            this.btnTestOnceHeart.UseVisualStyleBackColor = true;
            this.btnTestOnceHeart.Click += new System.EventHandler(this.btnTestOnceHeart_Click);
            // 
            // numericHeartTimeSpan
            // 
            this.numericHeartTimeSpan.Location = new System.Drawing.Point(75, 19);
            this.numericHeartTimeSpan.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericHeartTimeSpan.Name = "numericHeartTimeSpan";
            this.numericHeartTimeSpan.Size = new System.Drawing.Size(108, 21);
            this.numericHeartTimeSpan.TabIndex = 23;
            this.numericHeartTimeSpan.Value = new decimal(new int[] {
            15000,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 21;
            this.label5.Text = "测试间隔：";
            // 
            // isStopXinTiao
            // 
            this.isStopXinTiao.Location = new System.Drawing.Point(129, 72);
            this.isStopXinTiao.Name = "isStopXinTiao";
            this.isStopXinTiao.Size = new System.Drawing.Size(83, 23);
            this.isStopXinTiao.TabIndex = 20;
            this.isStopXinTiao.Text = "停止定时";
            this.isStopXinTiao.UseVisualStyleBackColor = true;
            this.isStopXinTiao.Click += new System.EventHandler(this.isStopXinTiao_Click);
            // 
            // isStartXinTiao
            // 
            this.isStartXinTiao.Location = new System.Drawing.Point(15, 72);
            this.isStartXinTiao.Name = "isStartXinTiao";
            this.isStartXinTiao.Size = new System.Drawing.Size(83, 23);
            this.isStartXinTiao.TabIndex = 19;
            this.isStartXinTiao.Text = "开始定时";
            this.isStartXinTiao.UseVisualStyleBackColor = true;
            this.isStartXinTiao.Click += new System.EventHandler(this.isStartXinTiao_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bttnIsDisconnect);
            this.groupBox1.Controls.Add(this.numericPort);
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textYuMing);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textTel);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(3, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(273, 168);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配置";
            // 
            // bttnIsDisconnect
            // 
            this.bttnIsDisconnect.Location = new System.Drawing.Point(135, 127);
            this.bttnIsDisconnect.Name = "bttnIsDisconnect";
            this.bttnIsDisconnect.Size = new System.Drawing.Size(100, 23);
            this.bttnIsDisconnect.TabIndex = 19;
            this.bttnIsDisconnect.Text = "断开服务器";
            this.bttnIsDisconnect.UseVisualStyleBackColor = true;
            this.bttnIsDisconnect.Click += new System.EventHandler(this.bttnIsDisconnect_Click);
            // 
            // numericPort
            // 
            this.numericPort.Location = new System.Drawing.Point(80, 66);
            this.numericPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericPort.Name = "numericPort";
            this.numericPort.Size = new System.Drawing.Size(162, 21);
            this.numericPort.TabIndex = 17;
            this.numericPort.Value = new decimal(new int[] {
            6022,
            0,
            0,
            0});
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.Gainsboro;
            this.btnConnect.Location = new System.Drawing.Point(15, 127);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 23);
            this.btnConnect.TabIndex = 16;
            this.btnConnect.Text = "连接服务器";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(2, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "*";
            // 
            // textYuMing
            // 
            this.textYuMing.Location = new System.Drawing.Point(78, 29);
            this.textYuMing.Name = "textYuMing";
            this.textYuMing.Size = new System.Drawing.Size(164, 21);
            this.textYuMing.TabIndex = 4;
            this.textYuMing.Text = "127.0.0.1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "端口：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "域名：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 104);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "电话号码：";
            // 
            // textTel
            // 
            this.textTel.Location = new System.Drawing.Point(78, 100);
            this.textTel.Name = "textTel";
            this.textTel.Size = new System.Drawing.Size(164, 21);
            this.textTel.TabIndex = 0;
            this.textTel.Text = "13554097305";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1278, 861);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DTU模拟器";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.contextMenuStrip_log.ResumeLayout(false);
            this.contextMenuStrip_reciveLog.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_commands)).EndInit();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_write)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_read)).EndInit();
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSendTimeSpan)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPLCTimeSpan)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericHeartTimeSpan)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_log;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_clearLog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_reciveLog;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_clearReciveLog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox textBoxLog;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox textBoxRecive;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioButton_16Hex;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Button btnSendOneData;
        private System.Windows.Forms.NumericUpDown numericSendTimeSpan;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textSendText;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button IsStoptDataTest;
        private System.Windows.Forms.Button IsStartDataTest;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton radioButton_delay;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.Button isStopAnswer;
        private System.Windows.Forms.Button isStartAnswer;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button isStopSendPLC;
        private System.Windows.Forms.NumericUpDown numericUpDownPLCTimeSpan;
        private System.Windows.Forms.Button isStartSendPLC;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnTestOnceHeart;
        private System.Windows.Forms.NumericUpDown numericHeartTimeSpan;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button isStopXinTiao;
        private System.Windows.Forms.Button isStartXinTiao;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bttnIsDisconnect;
        private System.Windows.Forms.NumericUpDown numericPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textYuMing;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textTel;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridView dataGridView_commands;
        private System.Windows.Forms.Button btnRefreshCommands;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btn_creatSomeCommands;
        private System.Windows.Forms.Button btnDelCommands;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn type;
        private System.Windows.Forms.DataGridViewTextBoxColumn sensorID;
        private System.Windows.Forms.DataGridViewTextBoxColumn sensorValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn status;
        private System.Windows.Forms.DataGridViewTextBoxColumn beginTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn endTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn 耗时;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mess;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.NumericUpDown numeric_write;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown numeric_read;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button btnSendOncePLCData;
    }
}

