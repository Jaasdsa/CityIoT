namespace CityIoTServiceWatcher
{
    partial class OPCPumpService
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
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txt_okayTimes = new System.Windows.Forms.NumericUpDown();
            this.txt_commandTimeoutSeconds = new System.Windows.Forms.NumericUpDown();
            this.txt_errorTimes = new System.Windows.Forms.NumericUpDown();
            this.txt_ReadRate = new System.Windows.Forms.NumericUpDown();
            this.txt_UpdateRate = new System.Windows.Forms.NumericUpDown();
            this.txt_DefaultGroupDeadband = new System.Windows.Forms.NumericUpDown();
            this.txt_PointsCollectInterVal = new System.Windows.Forms.NumericUpDown();
            this.txt_PointsSaveInterVal = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_okayTimes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_commandTimeoutSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_errorTimes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_ReadRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_UpdateRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_DefaultGroupDeadband)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_PointsCollectInterVal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_PointsSaveInterVal)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnSubmit);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txt_okayTimes);
            this.groupBox1.Controls.Add(this.txt_commandTimeoutSeconds);
            this.groupBox1.Controls.Add(this.txt_errorTimes);
            this.groupBox1.Controls.Add(this.txt_ReadRate);
            this.groupBox1.Controls.Add(this.txt_UpdateRate);
            this.groupBox1.Controls.Add(this.txt_DefaultGroupDeadband);
            this.groupBox1.Controls.Add(this.txt_PointsCollectInterVal);
            this.groupBox1.Controls.Add(this.txt_PointsSaveInterVal);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(22, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(463, 336);
            this.groupBox1.TabIndex = 56;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "OPC二供接入服务";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(243, 282);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 80;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(143, 282);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 28);
            this.btnSubmit.TabIndex = 79;
            this.btnSubmit.Text = "提交";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(347, 191);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(17, 12);
            this.label15.TabIndex = 78;
            this.label15.Text = "个";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(347, 166);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(17, 12);
            this.label14.TabIndex = 77;
            this.label14.Text = "个";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(347, 216);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 12);
            this.label13.TabIndex = 76;
            this.label13.Text = "秒";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(347, 141);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(17, 12);
            this.label12.TabIndex = 75;
            this.label12.Text = "秒";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(347, 116);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 12);
            this.label11.TabIndex = 74;
            this.label11.Text = "毫秒";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(347, 66);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 12);
            this.label10.TabIndex = 73;
            this.label10.Text = "秒";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(347, 41);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 72;
            this.label9.Text = "分钟";
            // 
            // txt_okayTimes
            // 
            this.txt_okayTimes.AccessibleDescription = "";
            this.txt_okayTimes.Location = new System.Drawing.Point(237, 186);
            this.txt_okayTimes.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txt_okayTimes.Name = "txt_okayTimes";
            this.txt_okayTimes.Size = new System.Drawing.Size(100, 21);
            this.txt_okayTimes.TabIndex = 71;
            this.txt_okayTimes.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // txt_commandTimeoutSeconds
            // 
            this.txt_commandTimeoutSeconds.Location = new System.Drawing.Point(237, 211);
            this.txt_commandTimeoutSeconds.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.txt_commandTimeoutSeconds.Name = "txt_commandTimeoutSeconds";
            this.txt_commandTimeoutSeconds.Size = new System.Drawing.Size(100, 21);
            this.txt_commandTimeoutSeconds.TabIndex = 70;
            this.txt_commandTimeoutSeconds.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // txt_errorTimes
            // 
            this.txt_errorTimes.Location = new System.Drawing.Point(237, 161);
            this.txt_errorTimes.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txt_errorTimes.Name = "txt_errorTimes";
            this.txt_errorTimes.Size = new System.Drawing.Size(100, 21);
            this.txt_errorTimes.TabIndex = 69;
            this.txt_errorTimes.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // txt_ReadRate
            // 
            this.txt_ReadRate.Location = new System.Drawing.Point(237, 136);
            this.txt_ReadRate.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txt_ReadRate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_ReadRate.Name = "txt_ReadRate";
            this.txt_ReadRate.Size = new System.Drawing.Size(100, 21);
            this.txt_ReadRate.TabIndex = 68;
            this.txt_ReadRate.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // txt_UpdateRate
            // 
            this.txt_UpdateRate.Location = new System.Drawing.Point(237, 111);
            this.txt_UpdateRate.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txt_UpdateRate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_UpdateRate.Name = "txt_UpdateRate";
            this.txt_UpdateRate.Size = new System.Drawing.Size(100, 21);
            this.txt_UpdateRate.TabIndex = 67;
            this.txt_UpdateRate.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
            // 
            // txt_DefaultGroupDeadband
            // 
            this.txt_DefaultGroupDeadband.Location = new System.Drawing.Point(237, 86);
            this.txt_DefaultGroupDeadband.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_DefaultGroupDeadband.Name = "txt_DefaultGroupDeadband";
            this.txt_DefaultGroupDeadband.Size = new System.Drawing.Size(100, 21);
            this.txt_DefaultGroupDeadband.TabIndex = 66;
            // 
            // txt_PointsCollectInterVal
            // 
            this.txt_PointsCollectInterVal.Location = new System.Drawing.Point(237, 61);
            this.txt_PointsCollectInterVal.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txt_PointsCollectInterVal.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_PointsCollectInterVal.Name = "txt_PointsCollectInterVal";
            this.txt_PointsCollectInterVal.Size = new System.Drawing.Size(100, 21);
            this.txt_PointsCollectInterVal.TabIndex = 65;
            this.txt_PointsCollectInterVal.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // txt_PointsSaveInterVal
            // 
            this.txt_PointsSaveInterVal.Location = new System.Drawing.Point(237, 36);
            this.txt_PointsSaveInterVal.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.txt_PointsSaveInterVal.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_PointsSaveInterVal.Name = "txt_PointsSaveInterVal";
            this.txt_PointsSaveInterVal.Size = new System.Drawing.Size(100, 21);
            this.txt_PointsSaveInterVal.TabIndex = 64;
            this.txt_PointsSaveInterVal.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(51, 215);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 12);
            this.label8.TabIndex = 63;
            this.label8.Text = "任务超时间隔：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(51, 190);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 12);
            this.label7.TabIndex = 62;
            this.label7.Text = "判断在线最低值：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(51, 165);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 61;
            this.label6.Text = "最大容错数：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(51, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 60;
            this.label5.Text = "ReadRate：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(51, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 59;
            this.label4.Text = "UpdateRate：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(51, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 12);
            this.label3.TabIndex = 58;
            this.label3.Text = "DefaultGroupDeadband：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 57;
            this.label2.Text = "采集间隔：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 56;
            this.label1.Text = "历史归档间隔：";
            // 
            // OPCPumpService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "OPCPumpService";
            this.Size = new System.Drawing.Size(521, 377);
            this.Load += new System.EventHandler(this.OPCPumpService_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_okayTimes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_commandTimeoutSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_errorTimes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_ReadRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_UpdateRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_DefaultGroupDeadband)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_PointsCollectInterVal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_PointsSaveInterVal)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown txt_okayTimes;
        private System.Windows.Forms.NumericUpDown txt_commandTimeoutSeconds;
        private System.Windows.Forms.NumericUpDown txt_errorTimes;
        private System.Windows.Forms.NumericUpDown txt_ReadRate;
        private System.Windows.Forms.NumericUpDown txt_UpdateRate;
        private System.Windows.Forms.NumericUpDown txt_DefaultGroupDeadband;
        private System.Windows.Forms.NumericUpDown txt_PointsCollectInterVal;
        private System.Windows.Forms.NumericUpDown txt_PointsSaveInterVal;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
