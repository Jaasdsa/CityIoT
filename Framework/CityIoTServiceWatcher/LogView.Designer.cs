namespace CityIoTServiceWatcher
{
    partial class LogView
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxLogType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxServerType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerStop = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.listViewLog = new System.Windows.Forms.ListView();
            this.columnID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnServerName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnText = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pagerControl = new TActionProject.PagerControl();
            this.contextMenuCopy = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.contextMenuCopy.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSearch);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.comboBoxLogType);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.comboBoxServerType);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.dateTimePickerStop);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.dateTimePickerStart);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(857, 46);
            this.panel1.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(765, 14);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 11;
            this.btnSearch.Text = "查询";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(542, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "日志类型：";
            // 
            // comboBoxLogType
            // 
            this.comboBoxLogType.FormattingEnabled = true;
            this.comboBoxLogType.Location = new System.Drawing.Point(611, 15);
            this.comboBoxLogType.Name = "comboBoxLogType";
            this.comboBoxLogType.Size = new System.Drawing.Size(121, 20);
            this.comboBoxLogType.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(338, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "系统名称：";
            // 
            // comboBoxServerType
            // 
            this.comboBoxServerType.FormattingEnabled = true;
            this.comboBoxServerType.Location = new System.Drawing.Point(404, 15);
            this.comboBoxServerType.Name = "comboBoxServerType";
            this.comboBoxServerType.Size = new System.Drawing.Size(121, 20);
            this.comboBoxServerType.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(176, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "---";
            // 
            // dateTimePickerStop
            // 
            this.dateTimePickerStop.Location = new System.Drawing.Point(203, 14);
            this.dateTimePickerStop.Name = "dateTimePickerStop";
            this.dateTimePickerStop.Size = new System.Drawing.Size(118, 21);
            this.dateTimePickerStop.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "日期：";
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.Checked = false;
            this.dateTimePickerStart.Location = new System.Drawing.Point(53, 14);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(118, 21);
            this.dateTimePickerStart.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pagerControl);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 366);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(857, 46);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.listViewLog);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 46);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(857, 320);
            this.panel3.TabIndex = 2;
            // 
            // listViewLog
            // 
            this.listViewLog.AllowColumnReorder = true;
            this.listViewLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnID,
            this.columnServerName,
            this.columnTime,
            this.columnType,
            this.columnText});
            this.listViewLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewLog.FullRowSelect = true;
            this.listViewLog.GridLines = true;
            this.listViewLog.HideSelection = false;
            this.listViewLog.Location = new System.Drawing.Point(0, 0);
            this.listViewLog.Name = "listViewLog";
            this.listViewLog.Size = new System.Drawing.Size(857, 320);
            this.listViewLog.TabIndex = 0;
            this.listViewLog.UseCompatibleStateImageBehavior = false;
            this.listViewLog.View = System.Windows.Forms.View.Details;
            this.listViewLog.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewLog_MouseClick);
            // 
            // columnID
            // 
            this.columnID.Text = "ID";
            // 
            // columnServerName
            // 
            this.columnServerName.Text = "系统名称";
            this.columnServerName.Width = 100;
            // 
            // columnTime
            // 
            this.columnTime.Text = "插入时间";
            this.columnTime.Width = 130;
            // 
            // columnType
            // 
            this.columnType.Text = "类型";
            this.columnType.Width = 80;
            // 
            // columnText
            // 
            this.columnText.Text = "信息文本";
            this.columnText.Width = 450;
            // 
            // pagerControl
            // 
            this.pagerControl.BackColor = System.Drawing.SystemColors.Control;
            this.pagerControl.Dock = System.Windows.Forms.DockStyle.Right;
            this.pagerControl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(78)))), ((int)(((byte)(151)))));
            this.pagerControl.Location = new System.Drawing.Point(197, 0);
            this.pagerControl.Name = "pagerControl";
            this.pagerControl.PageIndex = 1;
            this.pagerControl.PageSize = 100;
            this.pagerControl.RecordCount = 0;
            this.pagerControl.Size = new System.Drawing.Size(660, 46);
            this.pagerControl.TabIndex = 0;
            // 
            // contextMenuCopy
            // 
            this.contextMenuCopy.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemCopy});
            this.contextMenuCopy.Name = "contextMenuCopy";
            this.contextMenuCopy.Size = new System.Drawing.Size(153, 48);
            // 
            // ToolStripMenuItemCopy
            // 
            this.ToolStripMenuItemCopy.Name = "ToolStripMenuItemCopy";
            this.ToolStripMenuItemCopy.Size = new System.Drawing.Size(152, 22);
            this.ToolStripMenuItemCopy.Text = "复制信息文本";
            this.ToolStripMenuItemCopy.Click += new System.EventHandler(this.ToolStripMenuItemCopy_Click);
            // 
            // FormLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(857, 412);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FormLog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "日志查看器";
            this.Load += new System.EventHandler(this.FormLog_Load);
            this.SizeChanged += new System.EventHandler(this.FormLog_SizeChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.contextMenuCopy.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private TActionProject.PagerControl pagerControl;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ListView listViewLog;
        private System.Windows.Forms.ColumnHeader columnID;
        private System.Windows.Forms.ColumnHeader columnServerName;
        private System.Windows.Forms.ColumnHeader columnTime;
        private System.Windows.Forms.ColumnHeader columnType;
        private System.Windows.Forms.ColumnHeader columnText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.DateTimePicker dateTimePickerStop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxServerType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxLogType;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ContextMenuStrip contextMenuCopy;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemCopy;
    }
}