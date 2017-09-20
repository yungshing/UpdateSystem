namespace UpdateSystem
{
    partial class UpdateContentForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.installNow_Btn = new System.Windows.Forms.Button();
            this.waitInstall_Btn = new System.Windows.Forms.Button();
            this.detail_Btn = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(77, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "软件更新";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(28, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(193, 107);
            this.label2.TabIndex = 1;
            this.label2.Text = "\"1.2.30\"版本可用于您的机器且已经可以安装。";
            // 
            // installNow_Btn
            // 
            this.installNow_Btn.Location = new System.Drawing.Point(79, 168);
            this.installNow_Btn.Name = "installNow_Btn";
            this.installNow_Btn.Size = new System.Drawing.Size(75, 23);
            this.installNow_Btn.TabIndex = 2;
            this.installNow_Btn.Text = "现在安装";
            this.installNow_Btn.UseVisualStyleBackColor = true;
            this.installNow_Btn.Click += new System.EventHandler(this.installNow_Btn_Click);
            // 
            // waitInstall_Btn
            // 
            this.waitInstall_Btn.Location = new System.Drawing.Point(79, 196);
            this.waitInstall_Btn.Name = "waitInstall_Btn";
            this.waitInstall_Btn.Size = new System.Drawing.Size(75, 23);
            this.waitInstall_Btn.TabIndex = 3;
            this.waitInstall_Btn.Text = "稍后安装";
            this.waitInstall_Btn.UseVisualStyleBackColor = true;
            this.waitInstall_Btn.Click += new System.EventHandler(this.waitInstall_Btn_Click);
            // 
            // detail_Btn
            // 
            this.detail_Btn.Location = new System.Drawing.Point(79, 223);
            this.detail_Btn.Name = "detail_Btn";
            this.detail_Btn.Size = new System.Drawing.Size(75, 23);
            this.detail_Btn.TabIndex = 4;
            this.detail_Btn.Text = "详细信息";
            this.detail_Btn.UseVisualStyleBackColor = true;
            this.detail_Btn.Click += new System.EventHandler(this.detail_Btn_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(-1, 256);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(252, 10);
            this.progressBar1.TabIndex = 5;
            // 
            // UpdateContentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(248, 262);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.detail_Btn);
            this.Controls.Add(this.waitInstall_Btn);
            this.Controls.Add(this.installNow_Btn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "UpdateContentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "中智";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button installNow_Btn;
        private System.Windows.Forms.Button waitInstall_Btn;
        private System.Windows.Forms.Button detail_Btn;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}