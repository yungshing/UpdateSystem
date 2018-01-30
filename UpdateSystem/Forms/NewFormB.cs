using System;
using System.Drawing;
using System.Windows.Forms;

namespace UpdateSystem
{
    public partial class NewFormB : Form
    {
        BtnType btnType = BtnType.Update;
        DownloadFormFollow downloadFollow;
        InstallFollow installFollow;
        Point mouseOffset;
        public NewFormB()
        {
            InitializeComponent();
        }

        private void SetProgressBar(int value, int max)
        {
            if (info_pro.InvokeRequired)
            {
                info_pro.Invoke(new GlobalEvent.CallV_II(SetProgressBar), new object[] { value, max });
            }
            else
            {
                info_pro.Size = new Size((int)(820 * (value / (float)max)), 17);
            }
        }
        private void SetBtnText()
        {
            if (mix_btn.InvokeRequired)
            {
                mix_btn.Invoke(new GlobalEvent.CallV_V(SetBtnText));
            }
            else
            {
                btnType = BtnType.Install;
                SetMixBtnImage(btnType);
                ShowDownloadSpeed("");
            }
        }
        private void OnShow()
        {
            btnType = BtnType.Update;

            version_lab.Visible = start_btn.Visible = !GlobalData.isFirstUse;
            version_lab.Text = GlobalData.version;
            if (GlobalData.checkedUpdate)
            {
                updateinfo_lab.Text = "";
                foreach (var item in GlobalData.updateText)
                {
                    updateinfo_lab.Text += item;
                }
            }
            SetInfoControlVisible(false);
            Alpha();
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

        enum BtnType
        {
            Null = 0,
            Update,
            Pause,
            Start,
            Install
        }

        private void start_btn_Click(object sender, EventArgs e)
        {
            if (!Utility.OpenProgram())
            {
                MessageBox.Show("打开程序失败");
            }
            if (GlobalData.isFirstUse)
            {
                Utility.ExitApp();
            }
            else
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void mix_btn_Click(object sender, EventArgs e)
        {
            switch (btnType)
            {
                case BtnType.Pause:
                    downloadFollow.Pause = true;
                    btnType = BtnType.Start;
                    SetMixBtnImage(btnType);
                    break;
                case BtnType.Start:
                    downloadFollow.Pause = false;
                    btnType = BtnType.Pause;
                    SetMixBtnImage(btnType);
                    break;
                case BtnType.Install:
                    installFollow.StartInstall();
                    installFollow.doInstallOver += () => Utility.ExitApp();
                    btnType = BtnType.Null;
                    mix_btn.Visible = false;
                    start_btn.Visible = false;
                    break;
                case BtnType.Update:
                    SetInfoControlVisible(true);
                    downloadFollow.StartDownload();
                    btnType = BtnType.Pause;
                    SetMixBtnImage(btnType);
                    break;
            }
        }

        /// <summary>
        /// 显示下载速度
        /// </summary>
        /// <param name="_speed"></param>
        void ShowDownloadSpeed(string _speed)
        {
            float speed = 0;
            var b = float.TryParse(_speed, out speed);
            if (this.speed_lab.InvokeRequired)
            {
                this.speed_lab.Invoke(new GlobalEvent.CallV_S(ShowDownloadSpeed), _speed);
            }
            else
            {
                if (!b)
                {
                    this.speed_lab.Text = _speed;
                    return;
                }
                if (speed < 1024f)
                {
                    this.speed_lab.Text = speed.ToString("0") + "b/s";
                }
                else if (speed < 1024 * 1000)
                {
                    this.speed_lab.Text = (speed * 0.001f).ToString("0") + "Kb/s";
                }
                else
                {
                    this.speed_lab.Text = (speed * 0.000001f).ToString("0.00") + "Mb/s";
                }
            }
        }
        /// <summary>
        /// 显示已经下载的文件个数和总数
        /// e.g. 10/100
        /// 异常处理时显示异常信息
        /// e.g. 当前网络阻塞，正在切换服务器
        /// </summary>
        /// <param name="s"></param>
        void ShowDownloadFileInfo(string s)
        {
            if (this.sum_lab.InvokeRequired)
            {
                this.sum_lab.Invoke(new GlobalEvent.CallV_S(ShowDownloadFileInfo), s);
            }
            else
            {
                sum_lab.Text = s;
            }
        }
        void ShowUpdateContentInfo(string s)
        {
            if (this.updateinfo_lab.InvokeRequired)
            {
                this.updateinfo_lab.Invoke(new GlobalEvent.CallV_S(ShowDownloadFileInfo), s);
            }
            else
            {
                updateinfo_lab.Text = "";
                foreach (var item in GlobalData.updateText)
                {
                    updateinfo_lab.Text += item;
                }
            }
        }
        void ShowVersion(string v)
        {
            if (version_lab.InvokeRequired)
            {
                version_lab.Invoke(new GlobalEvent.CallV_S(ShowVersion), v);
            }
            else
            {
                version_lab.Text = v;
            }
        }
        private void SetMixBtnImage(BtnType bt)
        {
            switch (bt)
            {
                case BtnType.Update:
                    mix_btn.Image = Properties.Resources.UI_Update_BG;
                    mix_btn.OnMouseEnterBG = Properties.Resources.UI_Update_Over;
                    mix_btn.OnMouseLeaveBG = Properties.Resources.UI_Update_BG;
                    mix_btn.OnMouseDownBG = Properties.Resources.UI_Update_Press;
                    break;
                case BtnType.Pause:
                    mix_btn.Image = Properties.Resources.UI_Pause_BG;
                    mix_btn.OnMouseEnterBG = Properties.Resources.UI_Pause_Over;
                    mix_btn.OnMouseLeaveBG = Properties.Resources.UI_Pause_BG;
                    mix_btn.OnMouseDownBG = Properties.Resources.UI_Pause_Press;
                    time_lab.Visible = true;
                    break;
                case BtnType.Start:
                    mix_btn.Image = Properties.Resources.UI_Update_BG;
                    mix_btn.OnMouseEnterBG = Properties.Resources.UI_Update_Over;
                    mix_btn.OnMouseLeaveBG = Properties.Resources.UI_Update_BG;
                    mix_btn.OnMouseDownBG = Properties.Resources.UI_Update_Press;
                    time_lab.Visible = false;
                    break;
                case BtnType.Install:
                    mix_btn.Image = Properties.Resources.UI_Install_BG;
                    mix_btn.OnMouseEnterBG = Properties.Resources.UI_Install_Over;
                    mix_btn.OnMouseLeaveBG = Properties.Resources.UI_Install_BG;
                    mix_btn.OnMouseDownBG = Properties.Resources.UI_Install_Press;
                    break;
            }
        }

        private void SetInfoControlVisible(bool visible)
        {
            info_pro.Visible = visible;
            info_pro.Size = new Size(1, info_pro.Size.Height);
            sum_lab.Visible = visible;
            sum_lab.Text = "";
            speed_lab.Visible = visible;
            speed_lab.Text = "";
        }

        private void ShowGameTime(string s)
        {
            int time = -1;
            int.TryParse(s, out time);
            if (time_lab.InvokeRequired)
            {
                time_lab.Invoke(new GlobalEvent.CallV_S(ShowGameTime), s);
            }
            else
            {
                time_lab.Text = "已用时间：" + Utility.ToTime(time);
            }
        }

        private void SetShotGameTimeLabelVisiable(bool visible)
        {
            if (time_lab.InvokeRequired)
            {
                time_lab.Invoke(new GlobalEvent.CallV_B(SetShotGameTimeLabelVisiable), visible);
            }
            {
                time_lab.Visible = visible;
            }
        }
        private void NewFormB_Load(object sender, EventArgs e)
        {
            // this.BackColor = Color.FromArgb(0, 1, 1);
            Bitmap b = new Bitmap(Properties.Resources.UI_BG);
            BitmapRegion.CreateControlRegion(this, b);

            //TransparencyKey = Color.FromArgb(0, 255, 0);

            downloadFollow = new UpdateSystem.DownloadFormFollow();
            downloadFollow.form = this;
            downloadFollow.doDownloadOver += SetBtnText;
            downloadFollow.doShowFilesCountBar += SetProgressBar;
            downloadFollow.doShowDownloadFileInfo += ShowDownloadFileInfo;
            downloadFollow.doShowDownloadSpeed += ShowDownloadSpeed;
            downloadFollow.doShowTimes += ShowGameTime;
            downloadFollow.doShowVersion += ShowVersion;
            downloadFollow.doShowUpdateInfo += ShowUpdateContentInfo;

            installFollow = new UpdateSystem.InstallFollow();
            installFollow.doChangeProgressBarValue += SetProgressBar;
            installFollow.doShowInstallInfo += ShowDownloadFileInfo;
            installFollow.doShowMessageBox += ShowMessageBox;

            OnShow();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GlobalEvent.SaveDebugLog();
        }

        private void ShowMessageBox()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new GlobalEvent.CallV_V(ShowMessageBox));
            }
            else
            {
                // MessageBox.Show(this, "检测到部分文件损坏，请重新下载", "警告", MessageBoxButtons.OK);
                btnType = BtnType.Update;
                mix_btn.Visible = true;
                mix_btn_Click(null, null);
            }
        }

        /// <summary>
        /// 内部测试
        /// </summary>
        private void Alpha()
        {
            bool visiable = GlobalData.isDebug;
            button1.Visible = visiable;
            alpha_lab.Visible = visiable;
            if (visiable)
            {
                alpha_lab.Text = "Alpha版 " + GlobalData.selfVersion;
            }

        }
    }
}
