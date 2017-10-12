using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace UpdateSystem
{
    public class DownloadFormFollow : Follow
    {
        /// <summary>
        /// 用进度条展示单个文件下载进度
        /// </summary>
        public event GlobalEvent.CallV_II doShowDownloadProgressBar;
        /// <summary>
        /// 用进度条展示总文件下载进度
        /// </summary>
        public event GlobalEvent.CallV_II doShowFilesCountBar;
        /// <summary>
        /// 显示已经下载的文件个数占总文件个数的百分比
        /// </summary>
        public event GlobalEvent.CallV_S doShowFileCountPercent;
        /// <summary>
        /// 显示下载速度
        /// e.g.  100kb/s
        /// </summary>
        public event GlobalEvent.CallV_S doShowDownloadSpeed;
        /// <summary>
        /// 显示单个文件下载剩余时间
        /// e.g. 剩余1时1分1秒
        /// </summary>
        public event GlobalEvent.CallV_S doShowRemainTime;
        /// <summary>
        /// 显示单个文件下载进度百分比
        /// </summary>
        public event GlobalEvent.CallV_S doShowFilePercent;
        /// <summary>
        /// 下载完成后，打开安装界面
        /// </summary>
        public event GlobalEvent.CallV_V doDownloadOver;
        /// <summary>
        /// 显示已经下载的文件个数和总数
        /// e.g. 10/100
        /// 异常处理时显示异常信息
        /// e.g. 当前网络阻塞，正在切换服务器
        /// </summary>
        public event GlobalEvent.CallV_S doShowDownloadFileInfo;
        /// <summary>
        /// 显示下载已用时间
        /// e.g. 1时1分1秒
        /// </summary>
        public event GlobalEvent.CallV_S doShowTimes;
        public DownloadFormFollow()
        {
            GlobalData.isUpdating = true;
        }

        private FTPDownload ftp;
        private Thread downloadThread;
        private System.Timers.Timer tim;

        public void StartDownload()
        {
            if (downloadThread != null)
            {
                downloadThread.Abort();
            }
            ShowDownloadTime();
            downloadThread = new Thread(() =>
            {
                DownloadVersion_C();
                DownloadUpdateFiles(GlobalData.needUpdateFiles);
                while(!CheckFiles())
                {
                    DownloadUpdateFiles(GlobalData.failureFiles);
                }
                doDownloadOver();
                downloadThread.Abort();
            });
            downloadThread.Start();
        }
        public bool Pause
        {
            get { return ftp.Pause; }
            set { ftp.Pause = value; }
        }

        private void SetFormUIEvent()
        {
            ftp.SetDownloadSpeed(doShowDownloadSpeed);
            ftp.SetProgerssBar(doShowDownloadProgressBar);
            ftp.SetShowRemainTime(doShowRemainTime);
            ftp.SetDownloadPercent(doShowFilePercent);
        }
        /// <summary>
        /// 下载Version-C.config文件
        /// </summary>
        private void DownloadVersion_C()
        {
            if (ftp != null)
            {
                ftp.Dispose();
            }
            GlobalEvent.WriteLog("下载version-C.config");
            ftp = new FTPDownload(GlobalData.mAccount.UserName, GlobalData.mAccount.Password);
            if (GlobalData.isFirstUse)
            {
                ftp = new FTPDownload(GlobalData.mAccount.UserName, GlobalData.mAccount.Password);
                GlobalData.WebXmlAddress = GlobalData.mAccount.Webaddr;
            }
            else
            {
                GlobalData.localXML = Utility.Decode<VersionXML>(GlobalData.filePath.ConfigDataFullPath);
                ftp = new FTPDownload(GlobalData.localXML.x_FtpAccount.Username, GlobalData.localXML.x_FtpAccount.Password);
                GlobalData.WebXmlAddress = GlobalData.localXML.x_FTPAddress[0] + GlobalData.localXML.x_UpdateWebAddress;
            }

            SetFormUIEvent();
            RundoShowDownloadFileInfo("下载配置文件");
            if (File.Exists(GlobalData.filePath.ConfigDataFullPath_Tmp))
            {
                File.Delete(GlobalData.filePath.ConfigDataFullPath_Tmp);
            }
            if (File.Exists(GlobalData.filePath.ConfigDataFullPath_Tmp+".zzfz"))
            {
                File.Delete(GlobalData.filePath.ConfigDataFullPath_Tmp+".zzfz");
            }
            var ifd = ftp.Download(GlobalData.WebXmlAddress.Replace("\\", "/"), GlobalData.filePath.ConfigDataFullPath_Tmp);
            while (!ifd)
            {
                Utility.SetException(ftp.E);
                if (File.Exists(GlobalData.filePath.ConfigDataFullPath_Tmp + ".zzfz"))
                {
                    File.Delete(GlobalData.filePath.ConfigDataFullPath_Tmp + ".zzfz");
                }
                ifd = ftp.Download(GlobalData.WebXmlAddress.Replace("\\", "/"), GlobalData.filePath.ConfigDataFullPath_Tmp);
            }
            if (GlobalData.isFirstUse)
            {
                var p = Path.Combine(Directory.GetCurrentDirectory(), "data.config.tmp");
                if (File.Exists(p))
                {
                    File.Delete(p);
                }
                
                var db = ftp.Download(GlobalData.dataWebAddress, p);
                while (!db)
                {
                    db = ftp.Download(GlobalData.dataWebAddress, p);
                }
            }
            ftp.Dispose();
        }
        private void DownloadUpdateFiles(List<XMLFileInfo> xfi)
        {
            if (ftp != null)
            {
                ftp.Dispose();
            }
            AnalysisWebVersion_C();
            GlobalData.ftpAddress.AllAddress = GlobalData.localXML.x_FTPAddress;
            ftp = new FTPDownload(GlobalData.localXML.x_FtpAccount.Username, GlobalData.localXML.x_FtpAccount.Password);
            SetFormUIEvent();
            for (int i = 0; i < xfi.Count; i++)
            {
                GlobalEvent.WriteLog("下载第" + i.ToString() + "个文件 : " + xfi[i].Name);

                RundoShowDownloadFileInfo((i + 1).ToString() + "/" + xfi.Count.ToString());
                RundoShowFilesCountBar(i, xfi.Count);
                float f = (float)i / (float)xfi.Count;
                f = f * 100f;
                RundoShowFileCountPercent(((int)f).ToString() + "%");
                var dP = Path.Combine(GlobalData.ftpAddress.CurrAddress, xfi[i].Address);
                var sP = Path.Combine(GlobalData.filePath.UpdatePath, xfi[i].Address);
                var d1 = new FileInfo(sP);
                if (!Directory.Exists(d1.Directory.FullName))
                {
                    Directory.CreateDirectory(d1.Directory.FullName);
                }
                GlobalEvent.WriteLog("地址：" + dP);

                GlobalEvent.WriteLog("DownloadFormFollow:_DownloadConfig():111");
                if (!ftp.Download(dP.Replace("\\", "/"), sP))
                {
                    ExceptionHandle(ftp.E);
                    i--;
                }
            }
            if (ftp != null)
            {
                ftp.Dispose();
            }
        }
        /// <summary>
        /// 解析从云端下载的Version-C.config文件
        /// 对比本地Version-C.config文件，获取需要更新的文件
        /// </summary>
        private void AnalysisWebVersion_C()
        {
            GlobalData.webXML = Utility.Decode<VersionXML>(GlobalData.filePath.ConfigDataFullPath_Tmp);
            if (GlobalData.isFirstUse)
            {
                GlobalData.localXML = Utility.Decode<VersionXML>(GlobalData.filePath.ConfigDataFullPath_Tmp);

                GlobalData.needUpdateFiles.Clear();
                GlobalData.needUpdateFiles.AddRange(GlobalData.webXML.x_FileList.x_base.Files);
                GlobalData.needUpdateFiles.AddRange(GlobalData.webXML.x_FileList.x_other.Files);

                foreach (var l in GlobalData.webXML.x_FileList.x_change)
                {
                    if (GlobalData.updateNodesName.Count > 0)
                    {
                        if (GlobalData.updateNodesName.Contains(l.Folder))
                        {
                            GlobalData.needUpdateFiles.AddRange(l.Files);
                            continue;
                        }
                    }
                    if (GlobalData.updateCarNodesName.Count > 0 && l.Folder == "CarModel")
                    {
                        //if (GlobalData.updateCarNodesName.Contains(l.Folder))
                        //{
                        //    GlobalData.needUpdateFiles.AddRange(l.Files);
                        //    continue;
                        //}
                        foreach (var item in GlobalData.updateCarNodesName)
                        {
                            foreach (var item1 in l.Files)
                            {
                                var fol = "\\CarModel\\" + item;
                                var fol2 = "/CarModel/" + item;
                                if (item1.Address.Contains(fol) || item1 .Address .Contains(fol2))
                                {
                                    item1.InstallPath = item1.Address.Replace("\\Change\\","\\");
                                    GlobalData.needUpdateFiles.Add(item1);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                GetDifferentFiles();
            }
        }

        bool GetDifferentFiles()
        {
            GlobalData.needUpdateFiles.Clear();
            GlobalData.needDeleteFiles.Clear();
            Action<List<XMLFileInfo>, List<XMLFileInfo>> _A = (_local, _web) =>
            {
                GlobalData.needUpdateFiles.AddRange(_web);
                GlobalData.needDeleteFiles.AddRange(_local);
                foreach (var l in _local)
                {
                    foreach (var w in _web)
                    {
                        if (l.Name == w.Name)
                        {
                            if (l.Hash == w.Hash)
                            {
                                GlobalData.needUpdateFiles.Remove(w);
                            }
                            GlobalData.needDeleteFiles.Remove(l);
                        }
                    }
                }
            };


            _A(GlobalData.localXML.x_FileList.x_base.Files, GlobalData.webXML.x_FileList.x_base.Files);
            _A(GlobalData.localXML.x_FileList.x_other.Files, GlobalData.webXML.x_FileList.x_other.Files);
            foreach (var item in GlobalData.updateNodesName)
            {
                var o = new List<XMLFileInfo>();
                var n = new List<XMLFileInfo>();
                foreach (var i1 in GlobalData.localXML.x_FileList.x_change)
                {
                    if (i1.Folder == item)
                    {
                        o = i1.Files;
                    }
                }
                foreach (var i1 in GlobalData.webXML.x_FileList.x_change)
                {
                    if (i1.Folder == item)
                    {
                        n = i1.Files;
                    }
                }
                _A(o, n);
            }

            return GlobalData.needUpdateFiles.Count > 0;
        }
        private void ShowDownloadTime()
        {
            if (tim != null)
            {
                tim.Start();
            }
            else
            {
                tim = new System.Timers.Timer();
                tim.Interval = 1000;
                tim.AutoReset = true;
                int t = 0;
                tim.Elapsed += (D, Z) =>
               {
                   if (!Pause)
                   {
                       t++;
                       RundoShowTimes(t.ToString());
                   }
               };
                tim.Start();
            }
        }
        private void RundoShowDownloadFileInfo(string s)
        {
            if (doShowDownloadFileInfo != null)
            {
                doShowDownloadFileInfo(s);
            }
        }
        private void RundoShowFilesCountBar(int v,int m)
        {
            if (doShowFilesCountBar!=null)
            {
                doShowFilesCountBar(v,m);
            }
        }
        private void RundoShowFileCountPercent(string s)
        {
            if (doShowFileCountPercent!=null)
            {
                doShowFileCountPercent(s);
            }
        }
        private void RundoShowTimes(string s)
        {
            if (doShowTimes!=null)
            {
                doShowTimes(s);
            }
        }
        private void RundoShowRemainTime(string s)
        {
            if (doShowRemainTime != null)
            {
                doShowRemainTime(s);
            }
        }
        private void RundoShowDownloadSpeed(string s)
        {
            if (doShowDownloadSpeed != null)
            {
                doShowDownloadSpeed(s);
            }
        }
        protected override void ExceptionHandle(Exception e)
        {
            Thread.Sleep(1000);  //****出现异常后，延迟1秒后，在处理异常
            var eType = Utility.SetException(e);
            switch (eType)
            {
                case Utility.EcpType.Normal:
                    break;
                case Utility.EcpType.Null:
                    break;
                case Utility.EcpType.AddressError:
                    break;
                case Utility.EcpType.Disconnect:
                    RundoShowDownloadFileInfo(Utility.E_Disconnect);
                    RundoShowRemainTime(null);
                    RundoShowDownloadSpeed(null);
                    CheckInternet(3);
                    break;
                case Utility.EcpType.LimitConnect:
                    GlobalData.ftpAddress.CurrIndex++;
                    doShowDownloadFileInfo(Utility.E_LimitConnect + ":" + GlobalData.ftpAddress.CurrIndex.ToString() + "号");
                    System.Windows.Forms.Application.DoEvents();
                    Utility.Delay(2); ///**********延迟
                    break;
                case Utility.EcpType.SerDisconnect:
                    doShowDownloadFileInfo(Utility.E_SerDisconnect);
                    break;
                case Utility.EcpType.ReceiveError:
                    doShowDownloadFileInfo(Utility.E_ReceiveError);

                    break;
                case Utility.EcpType.ErrorPw:
                    ftp = new FTPDownload("anonymous", "yungshing@tom.com");
                    break;
                default:
                    //System.Windows.Forms.MessageBox.Show("未知错误");
                    doShowDownloadFileInfo(e.Message);
                    GlobalEvent.SaveDebugLog();
                    Environment.Exit(0);
                    break;
            }
        }
        /// <summary>
        /// 每隔多少秒检测一次是否连网
        /// 线程会被一直堵塞在检测连网，直到达到限定的检测时间(0~59)
        /// </summary>
        /// <param name="second"></param>
        /// <param name="limit">尝试多长时间后，不在检测，直接返回，为-1时，则一直检测直到连网</param>
        /// <returns></returns>
        private bool CheckInternet(int second,int limit = -1)
        {
            bool b = Utility.IsConnectInternetPing();
            if (b)
                return b;
            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            st.Start();
            while(!b)
            {
                if (st.Elapsed.Seconds >= second)
                {
                    b = Utility.IsConnectInternetPing();
                    st.Reset();
                    st.Start();
                }
            }
            st.Stop();
            return b;
        }
        /// <summary>
        /// 下载完成后，检测文件是否全部下载完整
        /// true:全部下载完整
        /// false:有文件下载失败
        /// </summary>
        /// <returns></returns>
        private bool CheckFiles()
        {
            var t = GlobalData.needUpdateFiles;
            GlobalData.failureFiles.Clear();
            for (int i = 0; i < t.Count; i++)
            {
                RundoShowFilesCountBar(i + 1, t.Count);
                RundoShowDownloadFileInfo("检测文件：" + (i + 1).ToString() + "/" + t.Count.ToString());
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
                        GlobalData.failureFiles.Add(t[i]);
                        GlobalEvent.WriteLog(t[i].Address);
                        GlobalEvent.WriteLog("云MD5：" + t[i].Hash);
                        GlobalEvent.WriteLog("本地MD5：" + md5);
                        Utility.DeleteFile(tP);
                    }
                    #endregion
                }
                catch
                {
                    #region 检测文件-------------------文件是否存在方式 因为MD5目前有些电脑会出问题
                    if (!File.Exists(tP))
                    {
                        GlobalData.failureFiles.Add(t[i]);
                    }
                    #endregion
                }
            }
            return !(GlobalData.failureFiles.Count > 0);
        }
        public void DeleteConfig()
        {

            if (File.Exists(GlobalData.filePath.ConfigDataFullPath_Tmp))
            {
                File.Delete(GlobalData.filePath.ConfigDataFullPath_Tmp);
            }
        }
        public override void Dispose()
        {
            try
            {
                ftp.Dispose();
            }
            catch { }
            try
            {
                downloadThread.Abort();
            }
            catch { }
        }
    }
}
