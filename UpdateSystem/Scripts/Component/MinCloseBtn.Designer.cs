namespace UpdateSystem
{
    partial class MinCloseBtn
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.Min_pic = new System.Windows.Forms.PictureBox();
            this.Close_pic = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Min_pic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Close_pic)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.Min_pic);
            this.flowLayoutPanel1.Controls.Add(this.Close_pic);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(86, 30);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // Min_pic
            // 
            this.Min_pic.Image = global::UpdateSystem.Properties.Resources.UI_Min_BG;
            this.Min_pic.Location = new System.Drawing.Point(0, 0);
            this.Min_pic.Margin = new System.Windows.Forms.Padding(0);
            this.Min_pic.Name = "Min_pic";
            this.Min_pic.Size = new System.Drawing.Size(42, 30);
            this.Min_pic.TabIndex = 0;
            this.Min_pic.TabStop = false;
            this.Min_pic.Click += new System.EventHandler(this.Min_pic_Click);
            this.Min_pic.MouseEnter += new System.EventHandler(this.Min_pic_MouseEnter);
            this.Min_pic.MouseLeave += new System.EventHandler(this.Min_pic_MouseLeave);
            // 
            // Close_pic
            // 
            this.Close_pic.Image = global::UpdateSystem.Properties.Resources.UI_Close_BG;
            this.Close_pic.Location = new System.Drawing.Point(42, 0);
            this.Close_pic.Margin = new System.Windows.Forms.Padding(0);
            this.Close_pic.Name = "Close_pic";
            this.Close_pic.Size = new System.Drawing.Size(44, 30);
            this.Close_pic.TabIndex = 1;
            this.Close_pic.TabStop = false;
            this.Close_pic.Click += new System.EventHandler(this.Close_pic_Click);
            this.Close_pic.MouseEnter += new System.EventHandler(this.Close_pic_MouseEnter);
            this.Close_pic.MouseLeave += new System.EventHandler(this.Close_pic_MouseLeave);
            // 
            // MinCloseBtn
            // 
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "MinCloseBtn";
            this.Size = new System.Drawing.Size(86, 30);
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Min_pic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Close_pic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.PictureBox Min_pic;
        private System.Windows.Forms.PictureBox Close_pic;
    }
}
