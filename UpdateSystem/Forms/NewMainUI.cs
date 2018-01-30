using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
namespace UpdateSystem
{
    public partial class NewMainUI : Form
    {
        private Point mouseOffset;
        #region 进度条
        /// <summary>
        /// 程序打开时，进度条显示时间,叠加，毫秒
        /// </summary>
        private int timeOffset = 0;
        #endregion
        private MainFormFollow follow;
        Thread Update_Thread;
        public NewMainUI()
        {
            InitializeComponent();
            
            follow = new UpdateSystem.MainFormFollow();
            follow.doShowProgressBar += ShowProgressBar;
        }
        protected override void OnShown(EventArgs e)
        {
            timer1.Start();
            Update_Thread = new Thread(new ThreadStart(()=>
            {
                GlobalData.checkedUpdate = follow.OnStart();
                ShowProgressBar(1, 1);
                Application.DoEvents();
                Thread.Sleep(500);
                if (!GlobalData.checkedUpdate)
                {
                    if (Utility.OpenProgram())
                    {
                        Utility.ExitApp();
                    }
                    else
                    {
                        ShowMessageBox();
                    }
                }
                else
                {
                    ShowNextForm();
                }
            }));
        }
        private void UIBG_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOffset.X = e.X;
                mouseOffset.Y = e.Y;
            }
        }

        private void UIBG_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Top = MousePosition.Y - mouseOffset.Y;
                Left = MousePosition.X - mouseOffset.X;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (++timeOffset <= 8)
            {
                pictureBox1.Size = new Size(timeOffset, 17);
            }
            else
            {
                timer1.Stop();
                Update_Thread.Start();
            }
        }

        private void ShowProgressBar(int value, int max)
        {
            if (pictureBox1.InvokeRequired)
            {
                pictureBox1.Invoke(new GlobalEvent.CallV_II(ShowProgressBar),new object[] { value ,max});
            }
            else
            {
                if (value / (float)max >= 8 / 800f)
                {
                    pictureBox1.Size = new Size((int)(800 * value / (float)max), 17);
                }
            }
        }

        private void ShowNextForm()
        {
            if (this.InvokeRequired)
            {
                Invoke(new GlobalEvent.CallV_V(ShowNextForm));
            }
            else
            {
                NewFormB nextForm = new UpdateSystem.NewFormB();
                this.Hide();
                nextForm.Show();
            }
        }

        private void ShowMessageBox()
        {
            if (this.InvokeRequired)
            {
                Invoke(new GlobalEvent.CallV_V(ShowMessageBox));
            }
            else
            {
                MessageBox.Show(this,"启动程序失败");
                Utility.ExitApp();
            }
        }

        private void NewMainUI_Load(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(Properties.Resources .UI_BG);
            BitmapRegion.CreateControlRegion(this, b);
        }
    }
}
