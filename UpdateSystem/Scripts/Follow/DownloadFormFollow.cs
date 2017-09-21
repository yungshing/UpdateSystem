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
            GlobalData.IsUpdating = true;
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
            _SetDownloadTime();
            downloadThread = new Thread(() =>
            {
                _DownloadVersion_C();
                _DownloadUpdateFiles();
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
        private void _DownloadVersion_C()
        {
            if (ftp != null)
            {
                ftp.Dispose();
            }
            GlobalEvent.WriteLog("下载version-C.config");
            ftp = new FTPDownload(GlobalData.mAccount.UserName, GlobalData.mAccount.Password);
            if (GlobalData.IsFirstUse)
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

                GlobalEvent.WriteLog("DownloadFormFollow:_DownloadConfig():118");
                ifd = ftp.Download(GlobalData.WebXmlAddress.Replace("\\", "/"), GlobalData.filePath.ConfigDataFullPath_Tmp);
            }
            if (GlobalData.IsFirstUse)
            {
                var p = Path.Combine(Directory.GetCurrentDirectory(), "data.config.tmp");
                if (File.Exists(p))
                {
                    File.Delete(p);
                }

                GlobalEvent.WriteLog("DownloadFormFollow:_DownloadConfig():128");
                var db = ftp.Download(GlobalData.DataWebAddress, p);
                while (!db)
                {
                    GlobalEvent.WriteLog("DownloadFormFollow:_DownloadConfig():134");
                    db = ftp.Download(GlobalData.DataWebAddress, p);
                }
                GlobalData.webXML = Utility.Decode<VersionXML>(GlobalData.filePath.ConfigDataFullPath_Tmp);
                GlobalData.localXML = GlobalData.webXML;
            }
            ftp.Dispose();
            GlobalEvent.WriteLog("下载version-C.config.................完成");
        }
        private void _DownloadUpdateFiles()
        {
            if (ftp != null)
            {
                ftp.Dispose();
            }
            _AnalysisWebVersion_C();
            GlobalData.ftpAddress.AllAddress = GlobalData.localXML.x_FTPAddress;
            ftp = new FTPDownload(GlobalData.localXML.x_FtpAccount.Username, GlobalData.localXML.x_FtpAccount.Password);
            SetFormUIEvent();
            for (int i = 0; i < GlobalData.needUpdateFiles.Count; i++)
            {
                GlobalEvent.WriteLog("下载第" + i.ToString() + "个文件 : " + GlobalData.needUpdateFiles[i].Name);

                RundoShowDownloadFileInfo((i + 1).ToString() + "/" + GlobalData.needUpdateFiles.Count.ToString());
                RundoShowFilesCountBar(i, GlobalData.needUpdateFiles.Count);
                float f = (float)i / (float)GlobalData.needUpdateFiles.Count;
                f = f * 100f;
                RundoShowFileCountPercent(((int)f).ToString() + "%");
                var dP = Path.Combine(GlobalData.ftpAddress.CurrAddress, GlobalData.needUpdateFiles[i].Address);
                var sP = Path.Combine(GlobalData.filePath.UpdatePath, GlobalData.needUpdateFiles[i].Address);
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
        private void _AnalysisWebVersion_C()
        {
            GlobalData.webXML = Utility.Decode<VersionXML>(GlobalData.filePath.ConfigDataFullPath_Tmp);
            if (GlobalData.IsFirstUse)
            {
                GlobalData.localXML = Utility.Decode<VersionXML>(GlobalData.filePath.ConfigDataFullPath_Tmp);

                GlobalData.needUpdateFiles.Clear();
                GlobalData.needUpdateFiles.AddRange(GlobalData.webXML.x_FileList.x_base.Files);
                GlobalData.needUpdateFiles.AddRange(GlobalData.webXML.x_FileList.x_other.Files);

                foreach (var l in GlobalData.webXML.x_FileList.x_change)
                {
                    if (GlobalData.UpdateNodesName.Count > 0)
                    {
                        if (GlobalData.UpdateNodesName.Contains(l.Folder))
                        {
                            GlobalData.needUpdateFiles.AddRange(l.Files);
                            continue;
                        }
                    }
                    if (GlobalData.UpdateCarNodesName.Count > 0)
                    {
                        if (GlobalData.UpdateCarNodesName.Contains(l.Folder))
                        {
                            GlobalData.needUpdateFiles.AddRange(l.Files);
                            continue;
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
            foreach (var item in GlobalData.UpdateNodesName)
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
        private void _SetDownloadTime()
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
                    //while (!Utility.IsConnectInternetPing())
                    //{
                    //    continue;
                    //}
                    CheckInternet(3);
                    break;
                case Utility.EcpType.LimitConnect:
                    GlobalData.ftpAddress.CurrIndex++;
                    doShowDownloadFileInfo(Utility.E_LimitConnect + ":" + GlobalData.ftpAddress.CurrIndex.ToString() + "号");

                    break;
                case Utility.EcpType.SerDisconnect:
                    doShowDownloadFileInfo(Utility.E_SerDisconnect);
                    break;
                case Utility.EcpType.ReceiveError:
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
        /// 线程会被一直堵塞在检测连网，直到达到限定的检测时间
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
            while(!b)
            {
                if (st.ElapsedMilliseconds >= second * 1000)
                {
                    b = Utility.IsConnectInternetPing();
                }
            }
            st.Stop();
            return b;
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
