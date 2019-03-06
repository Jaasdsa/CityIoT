namespace CityIoTServiceWatcher
{
    partial class PumpAlarmService
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txt_commandTimeoutSeconds = new System.Windows.Forms.NumericUpDown();
            this.txt_pumpJZTimeOut = new System.Windows.Forms.NumericUpDown();
            this.txt_alarmUpdateInterval = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_commandTimeoutSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_pumpJZTimeOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_alarmUpdateInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnSubmit);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txt_commandTimeoutSeconds);
            this.groupBox1.Controls.Add(this.txt_pumpJZTimeOut);
            this.groupBox1.Controls.Add(this.txt_alarmUpdateInterval);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(22, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(463, 222);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "二供报警服务";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(243, 167);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 42;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(143, 167);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 28);
            this.btnSubmit.TabIndex = 41;
            this.btnSubmit.Text = "提交";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(310, 100);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 12);
            this.label11.TabIndex = 40;
            this.label11.Text = "秒";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(310, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 39;
            this.label1.Text = "分钟";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(310, 40);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 12);
            this.label9.TabIndex = 38;
            this.label9.Text = "秒";
            // 
            // txt_commandTimeoutSeconds
            // 
            this.txt_commandTimeoutSeconds.Location = new System.Drawing.Point(200, 95);
            this.txt_commandTimeoutSeconds.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.txt_commandTimeoutSeconds.Name = "txt_commandTimeoutSeconds";
            this.txt_commandTimeoutSeconds.Size = new System.Drawing.Size(100, 21);
            this.txt_commandTimeoutSeconds.TabIndex = 37;
            this.txt_commandTimeoutSeconds.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // txt_pumpJZTimeOut
            // 
            this.txt_pumpJZTimeOut.Location = new System.Drawing.Point(200, 65);
            this.txt_pumpJZTimeOut.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.txt_pumpJZTimeOut.Name = "txt_pumpJZTimeOut";
            this.txt_pumpJZTimeOut.Size = new System.Drawing.Size(100, 21);
            this.txt_pumpJZTimeOut.TabIndex = 36;
            this.txt_pumpJZTimeOut.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // txt_alarmUpdateInterval
            // 
            this.txt_alarmUpdateInterval.Location = new System.Drawing.Point(200, 35);
            this.txt_alarmUpdateInterval.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_alarmUpdateInterval.Name = "txt_alarmUpdateInterval";
            this.txt_alarmUpdateInterval.Size = new System.Drawing.Size(100, 21);
            this.txt_alarmUpdateInterval.TabIndex = 35;
            this.txt_alarmUpdateInterval.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(51, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 34;
            this.label3.Text = "任务超时间隔：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 33;
            this.label2.Text = "机组离线阈值：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(51, 40);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 32;
            this.label6.Text = "维护间隔：";
            // 
            // PumpAlarmService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "PumpAlarmService";
            this.Size = new System.Drawing.Size(515, 377);
            this.Load += new System.EventHandler(this.PumpAlarmService_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_commandTimeoutSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_pumpJZTimeOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_alarmUpdateInterval)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown txt_commandTimeoutSeconds;
        private System.Windows.Forms.NumericUpDown txt_pumpJZTimeOut;
        private System.Windows.Forms.NumericUpDown txt_alarmUpdateInterval;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
    }
}
