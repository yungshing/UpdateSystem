using System;
using System.Windows.Forms;

namespace UpdateSystem
{
    public partial class MinCloseBtn : UserControl
    {
        public MinCloseBtn()
        {
            InitializeComponent();
        }

        private void Min_pic_MouseEnter(object sender, EventArgs e)
        {
            Min_pic.Image = Properties.Resources.UI_Min_Over;
        }

        private void Min_pic_MouseLeave(object sender, EventArgs e)
        {
            Min_pic.Image = Properties.Resources.UI_Min_BG;
        }

        private void Min_pic_Click(object sender, EventArgs e)
        {
            Min_pic.Image = Properties.Resources.UI_Min_BG;
            this.ParentForm.WindowState = FormWindowState.Minimized;
        }

        private void Close_pic_MouseEnter(object sender, EventArgs e)
        {
            Close_pic.Image = Properties.Resources.UI_Close_Over;
        }

        private void Close_pic_MouseLeave(object sender, EventArgs e)
        {
            Close_pic.Image = Properties.Resources.UI_Close_BG;
        }

        private void Close_pic_Click(object sender, EventArgs e)
        {
            Close_pic.Image = Properties.Resources.UI_Close_BG;
            Environment.Exit(0);
        }
    }
}
