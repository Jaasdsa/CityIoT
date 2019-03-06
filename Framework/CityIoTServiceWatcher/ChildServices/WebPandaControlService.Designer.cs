namespace CityIoTServiceWatcher
{
    partial class WebPandaControlService
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
            this.txt_commandTimeoutSeconds = new System.Windows.Forms.NumericUpDown();
            this.txt_setPumpControl = new System.Windows.Forms.TextBox();
            this.txt_getTokenUrl = new System.Windows.Forms.TextBox();
            this.txt_appSecret = new System.Windows.Forms.TextBox();
            this.txt_appKey = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_commandTimeoutSeconds)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnSubmit);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.txt_commandTimeoutSeconds);
            this.groupBox1.Controls.Add(this.txt_setPumpControl);
            this.groupBox1.Controls.Add(this.txt_getTokenUrl);
            this.groupBox1.Controls.Add(this.txt_appSecret);
            this.groupBox1.Controls.Add(this.txt_appKey);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(22, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(463, 261);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "WEB熊猫控制服务";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(243, 207);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 46;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(143, 207);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 28);
            this.btnSubmit.TabIndex = 45;
            this.btnSubmit.Text = "提交";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(281, 141);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 12);
            this.label11.TabIndex = 44;
            this.label11.Text = "秒";
            // 
            // txt_commandTimeoutSeconds
            // 
            this.txt_commandTimeoutSeconds.Location = new System.Drawing.Point(171, 136);
            this.txt_commandTimeoutSeconds.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.txt_commandTimeoutSeconds.Name = "txt_commandTimeoutSeconds";
            this.txt_commandTimeoutSeconds.Size = new System.Drawing.Size(100, 21);
            this.txt_commandTimeoutSeconds.TabIndex = 43;
            this.txt_commandTimeoutSeconds.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // txt_setPumpControl
            // 
            this.txt_setPumpControl.Location = new System.Drawing.Point(171, 111);
            this.txt_setPumpControl.Name = "txt_setPumpControl";
            this.txt_setPumpControl.Size = new System.Drawing.Size(260, 21);
            this.txt_setPumpControl.TabIndex = 42;
            this.txt_setPumpControl.Text = "https://new.s-water.cn/App/SetPumpControl";
            // 
            // txt_getTokenUrl
            // 
            this.txt_getTokenUrl.Location = new System.Drawing.Point(171, 86);
            this.txt_getTokenUrl.Name = "txt_getTokenUrl";
            this.txt_getTokenUrl.Size = new System.Drawing.Size(260, 21);
            this.txt_getTokenUrl.TabIndex = 41;
            this.txt_getTokenUrl.Text = "https://new.s-water.cn/App/GetAccessToken";
            // 
            // txt_appSecret
            // 
            this.txt_appSecret.Location = new System.Drawing.Point(171, 61);
            this.txt_appSecret.Name = "txt_appSecret";
            this.txt_appSecret.Size = new System.Drawing.Size(260, 21);
            this.txt_appSecret.TabIndex = 40;
            this.txt_appSecret.Text = "45tnn5juyojgn3rn3fnn3t5j4to3fn6y64p3";
            // 
            // txt_appKey
            // 
            this.txt_appKey.Location = new System.Drawing.Point(171, 36);
            this.txt_appKey.Name = "txt_appKey";
            this.txt_appKey.Size = new System.Drawing.Size(260, 21);
            this.txt_appKey.TabIndex = 39;
            this.txt_appKey.Text = "34h3rj3ri3jrt5y778934t5yfg3333h4h";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(51, 140);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 12);
            this.label8.TabIndex = 38;
            this.label8.Text = "任务超时间隔：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(51, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 12);
            this.label4.TabIndex = 37;
            this.label4.Text = "setPumpControl：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(51, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 36;
            this.label3.Text = "getTokenUrl：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 35;
            this.label2.Text = "appSecret：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 34;
            this.label1.Text = "appKey：";
            // 
            // WebPandaControlService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "WebPandaControlService";
            this.Size = new System.Drawing.Size(517, 379);
            this.Load += new System.EventHandler(this.WebPandaControlService_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_commandTimeoutSeconds)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown txt_commandTimeoutSeconds;
        private System.Windows.Forms.TextBox txt_setPumpControl;
        private System.Windows.Forms.TextBox txt_getTokenUrl;
        private System.Windows.Forms.TextBox txt_appSecret;
        private System.Windows.Forms.TextBox txt_appKey;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
