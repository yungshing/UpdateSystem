namespace UpdateSystem
{
    partial class NewFormB
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewFormB));
            this.info_pro = new System.Windows.Forms.PictureBox();
            this.version_lab = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.updateinfo_lab = new System.Windows.Forms.Label();
            this.speed_lab = new System.Windows.Forms.Label();
            this.sum_lab = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.time_lab = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.start_btn = new UpdateSystem.Scripts.Component.ZButtn();
            this.mix_btn = new UpdateSystem.Scripts.Component.ZButtn();
            this.minCloseBtn1 = new UpdateSystem.MinCloseBtn();
            ((System.ComponentModel.ISupportInitialize)(this.info_pro)).BeginInit();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.start_btn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mix_btn)).BeginInit();
            this.SuspendLayout();
            // 
            // info_pro
            // 
            this.info_pro.BackColor = System.Drawing.Color.Transparent;
            this.info_pro.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("info_pro.BackgroundImage")));
            this.info_pro.Location = new System.Drawing.Point(0, 584);
            this.info_pro.Name = "info_pro";
            this.info_pro.Size = new System.Drawing.Size(600, 17);
            this.info_pro.TabIndex = 4;
            this.info_pro.TabStop = false;
            // 
            // version_lab
            // 
            this.version_lab.AutoSize = true;
            this.version_lab.BackColor = System.Drawing.Color.Transparent;
            this.version_lab.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.version_lab.ForeColor = System.Drawing.Color.White;
            this.version_lab.Location = new System.Drawing.Point(3, 562);
            this.version_lab.Name = "version_lab";
            this.version_lab.Size = new System.Drawing.Size(169, 19);
            this.version_lab.TabIndex = 6;
            this.version_lab.Text = "RH2-DG-GR-V1.0.1";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = global::UpdateSystem.Properties.Resources.UI_Info_BG;
            this.panel1.Controls.Add(this.updateinfo_lab);
            this.panel1.Location = new System.Drawing.Point(6, 228);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(394, 308);
            this.panel1.TabIndex = 7;
            // 
            // updateinfo_lab
            // 
            this.updateinfo_lab.BackColor = System.Drawing.Color.Transparent;
            this.updateinfo_lab.ForeColor = System.Drawing.Color.White;
            this.updateinfo_lab.Location = new System.Drawing.Point(12, 8);
            this.updateinfo_lab.Name = "updateinfo_lab";
            this.updateinfo_lab.Size = new System.Drawing.Size(371, 294);
            this.updateinfo_lab.TabIndex = 0;
            this.updateinfo_lab.Text = "劳而无功革夺";
            // 
            // speed_lab
            // 
            this.speed_lab.AutoSize = true;
            this.speed_lab.BackColor = System.Drawing.Color.Transparent;
            this.speed_lab.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.speed_lab.Location = new System.Drawing.Point(62, 0);
            this.speed_lab.Name = "speed_lab";
            this.speed_lab.Size = new System.Drawing.Size(47, 12);
            this.speed_lab.TabIndex = 11;
            this.speed_lab.Text = "500Kb/s";
            this.speed_lab.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sum_lab
            // 
            this.sum_lab.AutoSize = true;
            this.sum_lab.BackColor = System.Drawing.Color.Transparent;
            this.sum_lab.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.sum_lab.Location = new System.Drawing.Point(3, 0);
            this.sum_lab.Name = "sum_lab";
            this.sum_lab.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.sum_lab.Size = new System.Drawing.Size(53, 12);
            this.sum_lab.TabIndex = 12;
            this.sum_lab.Text = "50/12306";
            this.sum_lab.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel1.Controls.Add(this.sum_lab);
            this.flowLayoutPanel1.Controls.Add(this.speed_lab);
            this.flowLayoutPanel1.Controls.Add(this.time_lab);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(323, 562);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(476, 19);
            this.flowLayoutPanel1.TabIndex = 13;
            // 
            // time_lab
            // 
            this.time_lab.AutoSize = true;
            this.time_lab.BackColor = System.Drawing.Color.Transparent;
            this.time_lab.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.time_lab.Location = new System.Drawing.Point(115, 0);
            this.time_lab.Name = "time_lab";
            this.time_lab.Size = new System.Drawing.Size(0, 12);
            this.time_lab.TabIndex = 13;
            this.time_lab.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(572, 267);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "保存日志";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // start_btn
            // 
            this.start_btn.BackColor = System.Drawing.Color.Transparent;
            this.start_btn.Image = ((System.Drawing.Image)(resources.GetObject("start_btn.Image")));
            this.start_btn.Location = new System.Drawing.Point(427, 470);
            this.start_btn.Name = "start_btn";
            this.start_btn.OnMouseDownBG = global::UpdateSystem.Properties.Resources.UI_Run_Press;
            this.start_btn.OnMouseEnterBG = global::UpdateSystem.Properties.Resources.UI_Run_Over;
            this.start_btn.OnMouseLeaveBG = ((System.Drawing.Image)(resources.GetObject("start_btn.OnMouseLeaveBG")));
            this.start_btn.Size = new System.Drawing.Size(372, 64);
            this.start_btn.TabIndex = 8;
            this.start_btn.TabStop = false;
            this.start_btn.Click += new System.EventHandler(this.start_btn_Click);
            // 
            // mix_btn
            // 
            this.mix_btn.BackColor = System.Drawing.Color.Transparent;
            this.mix_btn.Image = ((System.Drawing.Image)(resources.GetObject("mix_btn.Image")));
            this.mix_btn.Location = new System.Drawing.Point(427, 405);
            this.mix_btn.Name = "mix_btn";
            this.mix_btn.OnMouseDownBG = global::UpdateSystem.Properties.Resources.UI_Update_Press;
            this.mix_btn.OnMouseEnterBG = global::UpdateSystem.Properties.Resources.UI_Update_Over;
            this.mix_btn.OnMouseLeaveBG = ((System.Drawing.Image)(resources.GetObject("mix_btn.OnMouseLeaveBG")));
            this.mix_btn.Size = new System.Drawing.Size(372, 64);
            this.mix_btn.TabIndex = 9;
            this.mix_btn.TabStop = false;
            this.mix_btn.Click += new System.EventHandler(this.mix_btn_Click);
            // 
            // minCloseBtn1
            // 
            this.minCloseBtn1.BackColor = System.Drawing.Color.Transparent;
            this.minCloseBtn1.Location = new System.Drawing.Point(707, 1);
            this.minCloseBtn1.Name = "minCloseBtn1";
            this.minCloseBtn1.Size = new System.Drawing.Size(86, 30);
            this.minCloseBtn1.TabIndex = 3;
            // 
            // NewFormB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Lime;
            this.BackgroundImage = global::UpdateSystem.Properties.Resources.UI_BG;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(802, 602);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.start_btn);
            this.Controls.Add(this.mix_btn);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.version_lab);
            this.Controls.Add(this.info_pro);
            this.Controls.Add(this.minCloseBtn1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Blue;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NewFormB";
            this.Text = "睿航";
            this.TransparencyKey = System.Drawing.Color.Lime;
            this.Load += new System.EventHandler(this.NewFormB_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UIBG_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UIBG_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.info_pro)).EndInit();
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.start_btn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mix_btn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ZProgressBar zProgressBar1;
        private MinCloseBtn minCloseBtn1;
        private System.Windows.Forms.PictureBox info_pro;
        private System.Windows.Forms.Label version_lab;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label updateinfo_lab;
        private Scripts.Component.ZButtn start_btn;
        private Scripts.Component.ZButtn mix_btn;
        private System.Windows.Forms.Label speed_lab;
        private System.Windows.Forms.Label sum_lab;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label time_lab;
        private System.Windows.Forms.Button button1;
    }
}