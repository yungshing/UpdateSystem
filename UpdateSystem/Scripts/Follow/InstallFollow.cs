using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace UpdateSystem
{
    public class InstallFollow : Follow
    {
        public InstallFollow() { }

        public event GlobalEvent.CallV_II doChangeProgressBarValue;
        /// <summary>
        /// 检测到文件有损坏时，调用 
        /// 弹出消息盒子，点击确定后，关闭界面，回到下载界面
        /// </summary>
        public event GlobalEvent.CallV_V doShowMessageBox;
        public event GlobalEvent.CallV_S doShowInstallInfo;
        /// <summary>
        /// 打开软件后，关闭安装界面
        /// </summary>
        public event GlobalEvent.CallV_V doInstallOver;
        private Thread installThread;
        public void StartInstall()
        {
            if (installThread != null)
            {
                try { installThread.Abort(); }
                catch { }
            }
            installThread = new Thread(()=>
            {
                Install();
                InstallOver();
            });
            installThread.Start();
        }
        /// <summary>
        /// true: 没有文件损坏
        /// false : 有文件损坏 
        /// 此方法已作废，检测文件放下下载中了。
        /// </summary>
        /// <returns></returns>
        private bool CheckFiles()
        {
            var t = GlobalData.needUpdateFiles;
            GlobalData.failureFiles = new List<XMLFileInfo>();
            for (int i = 0; i < t.Count; i++)
            {
                RundoChangeProgressBarValue(i + 1, t.Count);
                RundoShowInstallInfo("检测文件：" + (i + 1).ToString() + "/" + t.Count.ToString());
                GlobalEvent.WriteLog("检测第" + i.ToString() + "个文件：" + t[i].Name);
                var tP = Path.Combine(GlobalData.filePath.UpdatePath, t[i].Address);
                var b = (t[i].InstallPath.Contains("Mono") && t[i].InstallPath.Contains("etc")) || (t[i].InstallPath.ToLower().EndsWith(".xml"));
                if (b)
                {
                    GlobalEvent.WriteLog("xml文件,跳过检测");
                    continue;
                }
                try
                {
                    #region 检测文件-----------------------MD5方式
                    var md5 = Utility.GetMD5Value(tP);
                    if (t[i].Hash != md5)
                    {
                        var tmp = t[i];
                        GlobalData.failureFiles.Add(tmp);
                        GlobalEvent.WriteLog(tmp.Address);
                        GlobalEvent.WriteLog("云MD5：" + tmp.Hash);
                        GlobalEvent.WriteLog("本地MD5：" + md5);
                    }
                    #endregion
                }
                catch
                {
                    #region 检测文件-------------------文件是否存在方式 因为MD5目前有些电脑会出问题
                    if (!File.Exists(tP))
                    {
                        var tmp = t[i];
                        GlobalData.failureFiles.Add(tmp);
                    }
                    #endregion
                }
            }
            if (GlobalData.failureFiles.Count > 0)
            {
                RundoShowMessageBox();
                return false;
            }
            return true;
        }
        private void Install()
        {
            var t = GlobalData.needUpdateFiles;

            ///结束游戏进程
            Utility.KillProgram();

            InstallBodyFile();
            InstallConfigData();
            Utility.CopyOData();
            RundoShowInstallInfo("安装文件完成");

            DeleteTmpFile();

            RundoShowInstallInfo("删除冗沉文件");
            GlobalEvent.WriteLog("删除冗沉文件");

            //DeleteRedundant();
        }

        private void InstallBodyFile()
        {
            var t = GlobalData.needUpdateFiles;
            for (int i = 0; i < t.Count; i++)
            {
                RundoChangeProgressBarValue(i + 1, t.Count);
                RundoShowInstallInfo("正安装文件中:" + (i + 1).ToString() + "/" + t.Count.ToString());

                var nP = Path.Combine(GlobalData.filePath.ProgramPath, t[i].InstallPath);
                var oP = Path.Combine(GlobalData.filePath.UpdatePath, t[i].Address);

                FileInfo F = new FileInfo(nP);
                GlobalEvent.WriteLog("移动第" + i.ToString() + "个文件：" + F.Name);

                if (!Directory.Exists(F.Directory.FullName))
                {
                    Directory.CreateDirectory(F.Directory.FullName);
                }
                if (File.Exists(nP))
                {
                    File.Delete(nP);
                }
                File.Copy(oP, nP, true);
            }
        }
        private void InstallConfigData()
        {
            if (!Directory.Exists(GlobalData.filePath.ConfigDataPath))
            {
                Directory.CreateDirectory(GlobalData.filePath.ConfigDataPath);
            }
            if (File.Exists(GlobalData.filePath.ConfigDataFullPath))
            {
                File.Delete(GlobalData.filePath.ConfigDataFullPath);
            }
            GlobalEvent.WriteLog("移动配置文件");
            File.Copy(GlobalData.filePath.ConfigDataFullPath_Tmp, GlobalData.filePath.ConfigDataFullPath);
        }
        private void DeleteTmpFile()
        {
            var t = GlobalData.needUpdateFiles;
            for (int i = 0; i < t.Count; i++)
            {
                RundoShowInstallInfo("删除临时文件:" + (i + 1).ToString() + "/" + t.Count.ToString());
                GlobalEvent.WriteLog("删除临时文件:" + t[i].Name);
                RundoChangeProgressBarValue(i + 1, t.Count);
                var tP = Path.Combine(GlobalData.filePath.UpdatePath, t[i].Address);
                if (File.Exists(tP))
                {
                    File.Delete(tP);
                }
            }
        }

        private void DeleteRedundant()
        {
            for (int i = 0; i < GlobalData.needDeleteFiles.Count; i++)
            {
                RundoShowInstallInfo("删除冗沉文件:" + (i + 1).ToString() + "/" + GlobalData.needDeleteFiles.Count.ToString());
                GlobalEvent.WriteLog("删除冗沉文件:" + GlobalData.needDeleteFiles[i].Name);
                RundoChangeProgressBarValue(i + 1, GlobalData.needDeleteFiles.Count);
                var tp = Path.Combine(GlobalData.filePath.ProgramPath, GlobalData.needDeleteFiles[i].InstallPath);
                if (File.Exists(tp))
                {
                    File.Delete(tp);
                }
            }
        }
        private void RundoChangeProgressBarValue(int v,int m)
        {
            if (doChangeProgressBarValue!=null)
            {
                doChangeProgressBarValue(v, m);
            }
        }
        private void RundoShowInstallInfo(string s)
        {
            if (doShowInstallInfo!=null)
            {
                doShowInstallInfo(s);
            }
        }
        private void RundoShowMessageBox()
        {
            if (doShowMessageBox!=null)
            {
                doShowMessageBox();
            }
        }
        private void RundoInstallOver()
        {
            if (doInstallOver!=null)
            {
                doInstallOver();
            }
        }
        private void InstallOver()
        {
            RundoShowInstallInfo("打开软件中");
            GlobalData.localVersionXML = Utility.Decode<VersionXML>(GlobalData.filePath.ConfigDataFullPath);
            if (!Utility.OpenProgram())
            {
                System.Windows.Forms.MessageBox.Show("程序打开失败");
            }
            RundoInstallOver();
        }
    }
}
