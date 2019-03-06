namespace CityIoTServiceWatcher
{
    partial class PublishConf
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
            this.txt_Port = new System.Windows.Forms.NumericUpDown();
            this.btn_Save = new System.Windows.Forms.Button();
            this.txt_IP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Port)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txt_Port);
            this.groupBox1.Controls.Add(this.btn_Save);
            this.groupBox1.Controls.Add(this.txt_IP);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(22, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(463, 191);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "发布中心";
            // 
            // txt_Port
            // 
            this.txt_Port.Location = new System.Drawing.Point(171, 66);
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
            this.txt_Port.Size = new System.Drawing.Size(171, 21);
            this.txt_Port.TabIndex = 14;
            this.txt_Port.Value = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(170, 142);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(75, 28);
            this.btn_Save.TabIndex = 13;
            this.btn_Save.Text = "保存";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // txt_IP
            // 
            this.txt_IP.Location = new System.Drawing.Point(171, 36);
            this.txt_IP.Name = "txt_IP";
            this.txt_IP.ReadOnly = true;
            this.txt_IP.Size = new System.Drawing.Size(171, 21);
            this.txt_IP.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "发布服务端口：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "发布服务IP：";
            // 
            // PublishConf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "PublishConf";
            this.Size = new System.Drawing.Size(512, 374);
            this.Load += new System.EventHandler(this.PublishConf_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Port)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown txt_Port;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.TextBox txt_IP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
