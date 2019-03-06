namespace CityIoTServiceWatcher
{
    partial class HisVacuateService
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
            this.label = new System.Windows.Forms.Label();
            this.dt_EndTime = new System.Windows.Forms.DateTimePicker();
            this.checkBox_Pump = new System.Windows.Forms.CheckBox();
            this.checkBox_Scada = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnSubmit);
            this.groupBox1.Controls.Add(this.label);
            this.groupBox1.Controls.Add(this.dt_EndTime);
            this.groupBox1.Controls.Add(this.checkBox_Pump);
            this.groupBox1.Controls.Add(this.checkBox_Scada);
            this.groupBox1.Location = new System.Drawing.Point(22, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(463, 211);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "历史抽稀服务";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(243, 157);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 31;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(143, 157);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 28);
            this.btnSubmit.TabIndex = 30;
            this.btnSubmit.Text = "提交";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(80, 95);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(65, 12);
            this.label.TabIndex = 7;
            this.label.Text = "截止时间：";
            // 
            // dt_EndTime
            // 
            this.dt_EndTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dt_EndTime.Location = new System.Drawing.Point(161, 89);
            this.dt_EndTime.Name = "dt_EndTime";
            this.dt_EndTime.ShowUpDown = true;
            this.dt_EndTime.Size = new System.Drawing.Size(150, 21);
            this.dt_EndTime.TabIndex = 6;
            this.dt_EndTime.Value = new System.DateTime(2019, 2, 28, 1, 0, 0, 0);
            // 
            // checkBox_Pump
            // 
            this.checkBox_Pump.AutoSize = true;
            this.checkBox_Pump.Checked = true;
            this.checkBox_Pump.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_Pump.Location = new System.Drawing.Point(232, 40);
            this.checkBox_Pump.Name = "checkBox_Pump";
            this.checkBox_Pump.Size = new System.Drawing.Size(102, 16);
            this.checkBox_Pump.TabIndex = 5;
            this.checkBox_Pump.Text = "PumpIsNeedRun";
            this.checkBox_Pump.UseVisualStyleBackColor = true;
            // 
            // checkBox_Scada
            // 
            this.checkBox_Scada.AutoSize = true;
            this.checkBox_Scada.Checked = true;
            this.checkBox_Scada.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_Scada.Location = new System.Drawing.Point(82, 40);
            this.checkBox_Scada.Name = "checkBox_Scada";
            this.checkBox_Scada.Size = new System.Drawing.Size(108, 16);
            this.checkBox_Scada.TabIndex = 4;
            this.checkBox_Scada.Text = "ScadaIsNeedRun";
            this.checkBox_Scada.UseVisualStyleBackColor = true;
            // 
            // HisVacuateService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "HisVacuateService";
            this.Size = new System.Drawing.Size(503, 263);
            this.Load += new System.EventHandler(this.HisVacuateService_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.DateTimePicker dt_EndTime;
        private System.Windows.Forms.CheckBox checkBox_Pump;
        private System.Windows.Forms.CheckBox checkBox_Scada;
        private System.Windows.Forms.Button btnCancel;
    }
}
