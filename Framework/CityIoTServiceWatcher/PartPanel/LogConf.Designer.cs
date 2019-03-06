namespace CityIoTServiceWatcher
{
    partial class LogConf
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txt_Port = new System.Windows.Forms.NumericUpDown();
            this.txt_SaveDay = new System.Windows.Forms.NumericUpDown();
            this.btn_Save = new System.Windows.Forms.Button();
            this.txt_IP = new System.Windows.Forms.TextBox();
            this.dt_wh = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Port)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_SaveDay)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(509, 374);
            this.panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txt_Port);
            this.groupBox1.Controls.Add(this.txt_SaveDay);
            this.groupBox1.Controls.Add(this.btn_Save);
            this.groupBox1.Controls.Add(this.txt_IP);
            this.groupBox1.Controls.Add(this.dt_wh);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(22, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(463, 251);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "日志中心";
            // 
            // txt_Port
            // 
            this.txt_Port.Location = new System.Drawing.Point(210, 125);
            this.txt_Port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.txt_Port.Minimum = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            this.txt_Port.Name = "txt_Port";
            this.txt_Port.Size = new System.Drawing.Size(172, 21);
            this.txt_Port.TabIndex = 25;
            this.txt_Port.Value = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            // 
            // txt_SaveDay
            // 
            this.txt_SaveDay.Location = new System.Drawing.Point(210, 65);
            this.txt_SaveDay.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.txt_SaveDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_SaveDay.Name = "txt_SaveDay";
            this.txt_SaveDay.Size = new System.Drawing.Size(172, 21);
            this.txt_SaveDay.TabIndex = 24;
            this.txt_SaveDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(170, 197);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(75, 28);
            this.btn_Save.TabIndex = 23;
            this.btn_Save.Text = "保存";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // txt_IP
            // 
            this.txt_IP.Location = new System.Drawing.Point(210, 95);
            this.txt_IP.Name = "txt_IP";
            this.txt_IP.ReadOnly = true;
            this.txt_IP.Size = new System.Drawing.Size(172, 21);
            this.txt_IP.TabIndex = 22;
            // 
            // dt_wh
            // 
            this.dt_wh.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dt_wh.Location = new System.Drawing.Point(210, 35);
            this.dt_wh.Name = "dt_wh";
            this.dt_wh.ShowUpDown = true;
            this.dt_wh.Size = new System.Drawing.Size(172, 21);
            this.dt_wh.TabIndex = 21;
            this.dt_wh.Value = new System.DateTime(2019, 2, 26, 15, 0, 0, 0);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(51, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 12);
            this.label4.TabIndex = 20;
            this.label4.Text = "日志发布启用的端口：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(51, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 12);
            this.label3.TabIndex = 19;
            this.label3.Text = "日志发布启用的IP：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 18;
            this.label2.Text = "最大保存天数：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 12);
            this.label1.TabIndex = 17;
            this.label1.Text = "每天日志维护时间点：";
            // 
            // LogConf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "LogConf";
            this.Size = new System.Drawing.Size(509, 374);
            this.Load += new System.EventHandler(this.LogConf_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Port)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_SaveDay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown txt_Port;
        private System.Windows.Forms.NumericUpDown txt_SaveDay;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.TextBox txt_IP;
        private System.Windows.Forms.DateTimePicker dt_wh;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
