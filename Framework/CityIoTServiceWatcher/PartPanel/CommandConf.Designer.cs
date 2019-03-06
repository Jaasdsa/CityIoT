namespace CityIoTServiceWatcher
{
    partial class CommandConf
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
            this.text_Sec = new System.Windows.Forms.NumericUpDown();
            this.text_Port = new System.Windows.Forms.NumericUpDown();
            this.button_save = new System.Windows.Forms.Button();
            this.text_IP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.text_Sec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.text_Port)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(511, 385);
            this.panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.text_Sec);
            this.groupBox1.Controls.Add(this.text_Port);
            this.groupBox1.Controls.Add(this.button_save);
            this.groupBox1.Controls.Add(this.text_IP);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(22, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(463, 221);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "控制中心";
            // 
            // text_Sec
            // 
            this.text_Sec.Location = new System.Drawing.Point(190, 95);
            this.text_Sec.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.text_Sec.Name = "text_Sec";
            this.text_Sec.Size = new System.Drawing.Size(172, 21);
            this.text_Sec.TabIndex = 15;
            // 
            // text_Port
            // 
            this.text_Port.Location = new System.Drawing.Point(190, 65);
            this.text_Port.Maximum = new decimal(new int[] {
            95535,
            0,
            0,
            0});
            this.text_Port.Minimum = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            this.text_Port.Name = "text_Port";
            this.text_Port.Size = new System.Drawing.Size(172, 21);
            this.text_Port.TabIndex = 14;
            this.text_Port.Value = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(170, 167);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 28);
            this.button_save.TabIndex = 13;
            this.button_save.Text = "保存";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // text_IP
            // 
            this.text_IP.Location = new System.Drawing.Point(190, 35);
            this.text_IP.Name = "text_IP";
            this.text_IP.ReadOnly = true;
            this.text_IP.Size = new System.Drawing.Size(172, 21);
            this.text_IP.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(51, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "超时秒数：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "控制服务端口：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "控制服务IP：";
            // 
            // CommandConf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "CommandConf";
            this.Size = new System.Drawing.Size(511, 385);
            this.Load += new System.EventHandler(this.CommandConf_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.text_Sec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.text_Port)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown text_Sec;
        private System.Windows.Forms.NumericUpDown text_Port;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.TextBox text_IP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
