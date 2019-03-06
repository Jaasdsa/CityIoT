namespace CityIoTServiceWatcher
{
    partial class DBConf
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_Clear = new System.Windows.Forms.Button();
            this.comboBox_DB = new System.Windows.Forms.ComboBox();
            this.txt_Password = new System.Windows.Forms.TextBox();
            this.txt_UserName = new System.Windows.Forms.TextBox();
            this.txt_IP = new System.Windows.Forms.TextBox();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_Test = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DGV_History = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HostName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DBName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UserName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PassWord = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SaveDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Delete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_History)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btn_Clear);
            this.panel2.Controls.Add(this.comboBox_DB);
            this.panel2.Controls.Add(this.txt_Password);
            this.panel2.Controls.Add(this.txt_UserName);
            this.panel2.Controls.Add(this.txt_IP);
            this.panel2.Controls.Add(this.btn_Save);
            this.panel2.Controls.Add(this.btn_Test);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(621, 214);
            this.panel2.TabIndex = 1;
            // 
            // btn_Clear
            // 
            this.btn_Clear.Location = new System.Drawing.Point(370, 160);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.Size = new System.Drawing.Size(75, 28);
            this.btn_Clear.TabIndex = 11;
            this.btn_Clear.Text = "清除历史";
            this.btn_Clear.UseVisualStyleBackColor = true;
            this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
            // 
            // comboBox_DB
            // 
            this.comboBox_DB.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_DB.FormattingEnabled = true;
            this.comboBox_DB.Location = new System.Drawing.Point(154, 112);
            this.comboBox_DB.Name = "comboBox_DB";
            this.comboBox_DB.Size = new System.Drawing.Size(213, 21);
            this.comboBox_DB.TabIndex = 10;
            this.comboBox_DB.DropDown += new System.EventHandler(this.comboBox_DB_DropDown);
            // 
            // txt_Password
            // 
            this.txt_Password.Location = new System.Drawing.Point(154, 82);
            this.txt_Password.Name = "txt_Password";
            this.txt_Password.Size = new System.Drawing.Size(213, 21);
            this.txt_Password.TabIndex = 9;
            this.txt_Password.UseSystemPasswordChar = true;
            // 
            // txt_UserName
            // 
            this.txt_UserName.Location = new System.Drawing.Point(154, 52);
            this.txt_UserName.Name = "txt_UserName";
            this.txt_UserName.Size = new System.Drawing.Size(213, 21);
            this.txt_UserName.TabIndex = 8;
            // 
            // txt_IP
            // 
            this.txt_IP.Location = new System.Drawing.Point(154, 22);
            this.txt_IP.Name = "txt_IP";
            this.txt_IP.Size = new System.Drawing.Size(213, 21);
            this.txt_IP.TabIndex = 7;
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(249, 160);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(75, 28);
            this.btn_Save.TabIndex = 5;
            this.btn_Save.Text = "保存连接";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_Test
            // 
            this.btn_Test.Location = new System.Drawing.Point(132, 160);
            this.btn_Test.Name = "btn_Test";
            this.btn_Test.Size = new System.Drawing.Size(75, 28);
            this.btn_Test.TabIndex = 4;
            this.btn_Test.Text = "测试连接";
            this.btn_Test.UseVisualStyleBackColor = true;
            this.btn_Test.Click += new System.EventHandler(this.btn_Test_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(64, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "数据库名：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(64, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "密码：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(64, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "登录名：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(64, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "服务器名：";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.DGV_History);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 214);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(621, 47);
            this.panel1.TabIndex = 2;
            // 
            // DGV_History
            // 
            this.DGV_History.AllowUserToAddRows = false;
            this.DGV_History.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_History.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.HostName,
            this.DBName,
            this.UserName,
            this.PassWord,
            this.SaveDate,
            this.Delete});
            this.DGV_History.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGV_History.Location = new System.Drawing.Point(0, 0);
            this.DGV_History.Name = "DGV_History";
            this.DGV_History.RowTemplate.Height = 23;
            this.DGV_History.Size = new System.Drawing.Size(621, 47);
            this.DGV_History.TabIndex = 0;
            this.DGV_History.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_History_CellClick);
            // 
            // ID
            // 
            this.ID.HeaderText = "序号";
            this.ID.Name = "ID";
            this.ID.Width = 60;
            // 
            // HostName
            // 
            this.HostName.HeaderText = "服务器名";
            this.HostName.Name = "HostName";
            this.HostName.Width = 90;
            // 
            // DBName
            // 
            this.DBName.HeaderText = "数据库名";
            this.DBName.Name = "DBName";
            // 
            // UserName
            // 
            this.UserName.HeaderText = "登录名";
            this.UserName.Name = "UserName";
            this.UserName.Width = 70;
            // 
            // PassWord
            // 
            this.PassWord.HeaderText = "密码";
            this.PassWord.Name = "PassWord";
            this.PassWord.Visible = false;
            this.PassWord.Width = 60;
            // 
            // SaveDate
            // 
            this.SaveDate.HeaderText = "保存时间";
            this.SaveDate.Name = "SaveDate";
            this.SaveDate.Width = 90;
            // 
            // Delete
            // 
            this.Delete.HeaderText = "删除";
            this.Delete.Name = "Delete";
            this.Delete.Text = "删除";
            this.Delete.ToolTipText = "删除";
            this.Delete.UseColumnTextForButtonValue = true;
            this.Delete.Width = 60;
            // 
            // DBConf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "DBConf";
            this.Size = new System.Drawing.Size(621, 261);
            this.Load += new System.EventHandler(this.DBConf_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_History)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button btn_Test;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_DB;
        private System.Windows.Forms.TextBox txt_Password;
        private System.Windows.Forms.TextBox txt_UserName;
        private System.Windows.Forms.TextBox txt_IP;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView DGV_History;
        private System.Windows.Forms.Button btn_Clear;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn HostName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DBName;
        private System.Windows.Forms.DataGridViewTextBoxColumn UserName;
        private System.Windows.Forms.DataGridViewTextBoxColumn PassWord;
        private System.Windows.Forms.DataGridViewTextBoxColumn SaveDate;
        private System.Windows.Forms.DataGridViewButtonColumn Delete;
    }
}
