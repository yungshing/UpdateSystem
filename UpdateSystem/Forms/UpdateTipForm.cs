using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace UpdateSystem
{
    public partial class UpdateContentForm : Form
    {
        event GlobalEvent.CallProgressBarState doChangeProgressBarValue;
        delegate void V_F_V();
        event V_F_V doInstallOver;
        event V_F_V doShowMessageBox;
        Thread installThread;
        public UpdateContentForm()
        {
            InitializeComponent();
            GlobalEvent.SetUpdateState(true);
        }
        private void installNow_Btn_Click(object sender, System.EventArgs e) 
        {
            InstallFile();
        }
        private void waitInstall_Btn_Click(object sender, System.EventArgs e) 
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void detail_Btn_Click(object sender, System.EventArgs e) 
        {
            UpdateInfoForm uc = new UpdateInfoForm();
            Dispose();
            uc.Owner = GlobalData.mainForm;
            uc.ShowDialog();
        }

        void InstallFile()
        {
            installNow_Btn.Enabled = false;
            waitInstall_Btn.Enabled = false;
            detail_Btn.Enabled = false;
            try
            {
                if (installThread != null)
                {
                    installThread.Abort();
                }
            }
            catch { }
            doChangeProgressBarValue = new GlobalEvent.CallProgressBarState(SetProgressbarValue);
            doInstallOver = new V_F_V(SetInstallOver);
            doShowMessageBox = new V_F_V(ShowMessgeBox);
            installThread = new Thread(InstallFileBackground);

            installThread.Start();
        }

        void InstallFileBackground()
        {
            var t = GlobalData.needUpdateFileInfo;
            //GlobalData.incompleteFileInfo.Clear();
            //for (int i = 0; i < t.Count; i++)
            //{
            //    doChangeProgressBarValue(i + 1, t.Count);
            //    if (t[i].FileHash != GlobalEvent.GetMD5Value(t[i].TmpFullPath))
            //    {
            //        t[i].FileHash = GlobalEvent.GetMD5Value(t[i].TmpFullPath);
            //        GlobalEvent.WriteLog("文件损坏：" + t[i].FileFullName);
            //        GlobalData.incompleteFileInfo.Add(t[i]);
            //    }
            //}
            //GlobalData.needUpdateFileInfo.Clear();
            if (GlobalData.incompleteFileInfo.Count > 0)
            {
                GlobalData.needUpdateFileInfo = GlobalData.incompleteFileInfo;
                doShowMessageBox();
                return;
            }
            if (!Directory.Exists(GlobalData.filePath.ConfigDataPath))
            {
                Directory.CreateDirectory(GlobalData.filePath.ConfigDataPath);
            }
            if (File.Exists(GlobalData.filePath.ConfigDataFullPath))
            {
                File.Delete(GlobalData.filePath.ConfigDataFullPath);
            }
            GlobalEvent.WriteLog("移动配置文件");
            File.Move(GlobalData.filePath.ConfigDataFullPath_Tmp, GlobalData.filePath.ConfigDataFullPath);
            for (int i = 0; i <t.Count; i++)
            {
                doChangeProgressBarValue(i + 1, t.Count);
                FileInfo F = new FileInfo(t[i].MoveFullPath);
                GlobalEvent.WriteLog("移动第" + i.ToString() + "个文件：" + t[i].FileFullName);
                if (!Directory.Exists(F.DirectoryName))
                {
                    Directory.CreateDirectory(F.DirectoryName);
                }
                if (File.Exists(t[i].MoveFullPath))
                {
                    File.Delete(t[i].MoveFullPath);
                }
                File.Move(t[i].TmpFullPath, t[i].MoveFullPath);
                Application.DoEvents();
            }
            doInstallOver ();
        }
        void ShowMessgeBox()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new V_F_V(ShowMessgeBox));
            }
            else
            {
                if (!GlobalData.ExpMsg.isMessageBoxOpen)
                {
                    MessageBox.Show(this, "检测到部分文件损坏，请重新下载", "警告", MessageBoxButtons.OK);
                    GlobalData.ExpMsg.isMessageBoxOpen = false;
                    DownloadForm d = new DownloadForm();
                    d.Owner = GlobalData.mainForm;
                    Dispose();
                    d.ShowDialog();
                }
            }
        }
        void SetProgressbarValue(int curr, int max)
        {
            if (this.progressBar1.InvokeRequired)
            {
                this.progressBar1.Invoke(new GlobalEvent.CallProgressBarState(SetProgressbarValue), new object[] { curr, max });
            }
            else
            {
                this.progressBar1.Maximum = max;
                this.progressBar1.Value = curr;
            }
        }

        void SetInstallOver()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new V_F_V(SetInstallOver));
            }
            else
            {
                GlobalData.localFileInfo = new XMLAnalysis().GetFileInfomation(GlobalData.filePath.ConfigDataFullPath, true);
                GlobalEvent.SaveDebugLog();
                GlobalEvent.OpenProgram();
                GlobalEvent.SetUpdateState(false);
                Dispose();
            }
        }

        protected override void OnClosed(System.EventArgs e)
        {
            if (installThread != null)
            {
                installThread.Abort();
            }
            GlobalEvent.SetUpdateState(false);
            base.OnClosed(e);
        }
    }
}
