namespace TActionProject
{
    partial class PagerControl
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnFirst = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnLast = new System.Windows.Forms.Button();
            this.lblState = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCurrentPage = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnFirst
            // 
            this.btnFirst.BackgroundImage = global::CityFormControls.Properties.Resources.first;
            this.btnFirst.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnFirst.Location = new System.Drawing.Point(14, 5);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(34, 26);
            this.btnFirst.TabIndex = 58;
            this.btnFirst.UseVisualStyleBackColor = true;
            this.btnFirst.Click += new System.EventHandler(this.lnkFirst_LinkClicked);
            // 
            // btnPrev
            // 
            this.btnPrev.BackgroundImage = global::CityFormControls.Properties.Resources.prev;
            this.btnPrev.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnPrev.Location = new System.Drawing.Point(54, 5);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(34, 26);
            this.btnPrev.TabIndex = 59;
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.lnkPrev_LinkClicked);
            // 
            // btnNext
            // 
            this.btnNext.BackgroundImage = global::CityFormControls.Properties.Resources.next;
            this.btnNext.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnNext.Location = new System.Drawing.Point(94, 5);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(34, 26);
            this.btnNext.TabIndex = 59;
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.lnkNext_LinkClicked);
            // 
            // btnLast
            // 
            this.btnLast.BackgroundImage = global::CityFormControls.Properties.Resources.last;
            this.btnLast.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnLast.Location = new System.Drawing.Point(134, 5);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(34, 26);
            this.btnLast.TabIndex = 59;
            this.btnLast.UseVisualStyleBackColor = true;
            this.btnLast.Click += new System.EventHandler(this.lnkLast_LinkClicked);
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.Location = new System.Drawing.Point(329, 12);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(263, 12);
            this.lblState.TabIndex = 61;
            this.lblState.Text = "总共2条记录，当前第1页，共1页，每页10条记录";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(179, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 62;
            this.label1.Text = "第";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(263, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 62;
            this.label2.Text = "页";
            // 
            // txtCurrentPage
            // 
            this.txtCurrentPage.Location = new System.Drawing.Point(202, 9);
            this.txtCurrentPage.Name = "txtCurrentPage";
            this.txtCurrentPage.Size = new System.Drawing.Size(55, 21);
            this.txtCurrentPage.TabIndex = 60;
            this.txtCurrentPage.Text = "1";
            this.txtCurrentPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCurrentPage_KeyPress);
            // 
            // btnGo
            // 
            this.btnGo.BackgroundImage = global::CityFormControls.Properties.Resources.load;
            this.btnGo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnGo.Location = new System.Drawing.Point(286, 5);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(34, 26);
            this.btnGo.TabIndex = 63;
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // PagerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblState);
            this.Controls.Add(this.txtCurrentPage);
            this.Controls.Add(this.btnLast);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.btnFirst);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(78)))), ((int)(((byte)(151)))));
            this.Name = "PagerControl";
            this.Size = new System.Drawing.Size(607, 42);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFirst;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnLast;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCurrentPage;
        private System.Windows.Forms.Button btnGo;

    }
}
