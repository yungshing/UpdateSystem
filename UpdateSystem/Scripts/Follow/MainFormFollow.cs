using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Timers;
namespace UpdateSystem
{
    public class MainFormFollow : Follow
    {
        public MainFormFollow()
        {
            Initaialize();
        }
        //public event GlobalEvent.CallV_S doShowLabelTxt;
        public event GlobalEvent.CallV_II doShowProgressBar = null;
        /// <summary>
        /// 是否有更新，非第一次使用时，在软件打开后，就后台执行检测是否有更新，
        /// 当用户点开始时，如果有更新就弹出更新提示。不打开Unity程序
        /// 当没有更新时，点开始就打开Unity程序 
        /// 当用户点击开始后，因网络或其它原因导致还示检测出来，那必须停止检测
        /// 调用isHaveUpdates.dispose();
        /// </summary>
        public Timer isHaveUpdates = null;
        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initaialize()
        {
            GlobalData.checkedUpdate = false;
        }

        /// <summary>
        /// 是否连接到互联网
        /// </summary>
        /// <returns></returns>
        private bool CheckInternet()
        {
            return Utility.IsConnectInternetPing();
        }
        /// <summary>
        /// 是否首次使用
        /// </summary>
        /// <returns></returns>
        public bool CheckFirstUse()
        {
            GlobalData.isFirstUse = !File.Exists(GlobalData.filePath.ConfigDataFullPath);
            return GlobalData.isFirstUse;
        }
        /// <summary>
        /// 程序启动时执行 
        /// true : 进入更新界面
        /// false : 打开程序
        /// </summary>
        public bool OnStart()
        {
            ///step  ---------------- 检测data.config文件是否存在，不存在，则退出程序
            DataConfigExist();

            ///step  ------------解析data.config文件
            AnalyseDataConfig();
            
            ///step---------解析Version-C.config
            AnalysisLocalXML();

            ///step ---------延迟启动，如果没网，则等待一分钟检测，如果有网，直接进入下一流程
            if (!DelayStart())
            {
                return CheckFirstUse();
            }

            ///step  ------------------- 启动程序 版本自检
            if (GlobalData.isDebug)
            {
                // CheckSelfVersion_Alpha();
                CheckSelfVersion();
            }
            else
            {
                CheckSelfVersion();
            }

            ///step  -------------检测是否有必要下载data.confg
            if (!NeedDownloadDataConfig())
            {
                return true;
            }

            ///step  --------------下载data.config,保存为data.config.tmp
            DownloadDataConfig();

            ///step  ------------对比data.config和data.config.tmp中version,检测是否有更新
            return ComparerDataConfig();
        }

        /// <summary>
        /// true: 成功连上网
        /// false: 没连上网
        /// </summary>
        /// <returns></returns>
        private bool DelayStart()
        {
            System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
            s.Start();
            bool success = false;
            while (!(success = CheckInternet()))
            {
                doShowProgressBar((int)(s.ElapsedMilliseconds * 0.001f), 120);
                if (s.ElapsedMilliseconds >= 60 * 1000)
                {
                    break;
                }
            }
            s.Stop();
            return success;
        }

        /// <summary>
        /// 检测是否有必要下载data.confg
        /// </summary>
        /// <returns></returns>
        private bool NeedDownloadDataConfig()
        {
            if (CheckFirstUse())
            {
                GlobalData.updateText = new string[] { "首次更新将会下载全部文件需要一定时间，请耐心等待。" };
                return false;
            }
            else
            {
                AnalysisLocalXML();
                if (!CheckInternet())
                {
                    return false;
                }
                return true;
            }
        }
        /// <summary>
        /// 解析本地Version-C文件
        /// True:非首次使用，解析文件
        /// False:首次使用，不解析 
        /// </summary>
        /// <returns></returns>
        private bool AnalysisLocalXML()
        {
            if (!CheckFirstUse())
            {
                ///读取本地Version-C文件
                GlobalData.localXML = Utility.Decode<VersionXML>(GlobalData.filePath.ConfigDataFullPath);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 对比data.config和data.config.tmp中version,检测是否有更新
        /// true : 有更新
        /// false : 无更新
        /// </summary>
        /// <returns></returns>
        private bool ComparerDataConfig()
        {
            //var webDataXml = Utility.ODecode(Path.Combine(Directory.GetCurrentDirectory(), "data.config.tmp"));
            //return GlobalData.Version != webDataXml.SelectSingleNode("ClientVersion").Attributes[0].Value;
            return CheckFirstUse() || GlobalData.version != GlobalData.webVersion;
        }

        /// <summary>
        /// 解析data.config文件
        /// </summary>
        private void AnalyseDataConfig(bool isLocal = true)
        {
            string dataName = isLocal ? "data.config" : "data.config.tmp";
            var p = Path.Combine(Directory.GetCurrentDirectory(), dataName);
            var xmlDoc = Utility.ODecode(p);
            if (isLocal)
            {
                GlobalData.localUpdateNodesName.Clear();
                GlobalData.localUpdateCarNodesName.Clear();
            }
            else
            {
                GlobalData.webUpdateCarNodesName.Clear();
                GlobalData.webUpdateNodesName.Clear();
            }
            ///科目 包
            if (xmlDoc.SelectSingleNode("Version").SelectSingleNode("FileHashList") != null)
            {
                try
                {
                    var v = xmlDoc.SelectSingleNode("Version").SelectSingleNode("FileHashList").SelectNodes("FileName");
                    for (int i = 0; i < v.Count; i++)
                    {
                        var _v = v[i].Attributes[0].InnerText.Replace(" ", "");
                        if (isLocal)
                        {
                            GlobalData.localUpdateNodesName.Add(_v);
                        }
                        else
                        {
                            GlobalData.webUpdateNodesName.Add(_v);
                        }
                    }
                }
                catch { }
            }
            ///车模型
            try
            {
                var c = xmlDoc.SelectSingleNode("Version").SelectNodes("CarModel");
                for (int i = 0; i < c.Count; i++)
                {
                    var _v = c[i].Attributes[0].InnerText.Replace(" ", "");
                    if (isLocal)
                    {
                        GlobalData.localUpdateCarNodesName.Add(_v);
                    }
                    else
                    {
                        GlobalData.webUpdateCarNodesName.Add(_v);
                    }
                }
            }
            catch
            {

            }

            ///更新内容
            if (xmlDoc.SelectSingleNode("Version").SelectSingleNode("UpdateText") != null)
            {
                var updates = xmlDoc.SelectSingleNode("Version").SelectSingleNode("UpdateText").ChildNodes;
                GlobalData.updateText = new string[updates.Count];
                for (int i = 0; i < updates.Count; i++)
                {
                    GlobalData.updateText[i] = updates[i].InnerText.TrimEnd() + "\n";
                }
            }
            ///FTP信息
            try
            {
                var v = xmlDoc.SelectSingleNode("Version").SelectSingleNode("FTP").Attributes;
                GlobalData.mAccount.UserName = v[0].Value;
                GlobalData.mAccount.Password = v[1].Value;
                GlobalData.mAccount.Webaddr = v[2].Value;
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("配置文件损坏");
                System.Environment.Exit(0);
            }
            ///客户端版本
            if (xmlDoc.SelectSingleNode("Version").SelectSingleNode("ClientVersion") != null)
            {
                var version = xmlDoc.SelectSingleNode("Version").SelectSingleNode("ClientVersion ").Attributes[0].Value;
                if (isLocal)
                {
                    GlobalData.version = version;
                }
                else
                {
                    GlobalData.webVersion = version;
                }
            }
            if (xmlDoc.SelectSingleNode("Version").SelectSingleNode("Path") != null)
            {
                GlobalData.dataWebAddress = xmlDoc.SelectSingleNode("Version").SelectSingleNode("Path").Attributes[0].Value;
            }
        }

        /// <summary>
        /// step 
        /// 检测data.config文件是否存在
        /// 不存在，则关闭程序
        /// </summary>
        /// <returns></returns>
        private bool DataConfigExist()
        {
            var p = Path.Combine(Directory.GetCurrentDirectory(), "data.config");
            if (!File.Exists(p))
            {
                GlobalEvent.WriteLog("没有配置文件。");
                System.Windows.Forms.MessageBox.Show("配置文件丢失");
                System.Environment.Exit(0);
                return false;
            }
            return true;
        }
        /// <summary>
        /// step 
        /// 下载data.config,保存为data.config.tmp
        /// </summary>
        private void DownloadDataConfig()
        {
            FTPDownload ftp = new FTPDownload(GlobalData.localXML.x_FtpAccount.Username, GlobalData.localXML.x_FtpAccount.Password);
            var p = Path.Combine(Directory.GetCurrentDirectory(), "data.config.tmp");
            if (File.Exists(p))
            {
                File.Delete(p);
            }
            ftp.SetProgerssBar(doShowProgressBar);
            
            while (!ftp.Download(GlobalData.dataWebAddress, p))
            {
                continue;
            }
            ftp.Dispose();
            //var dataXml = Utility.ODecode(p).SelectSingleNode("Version");
            //if (dataXml.SelectSingleNode("UpdateText") != null)
            //{
            //    var updates = dataXml.SelectSingleNode("UpdateText").ChildNodes;
            //    GlobalData.updateText = new string[updates.Count];
            //    for (int i = 0; i < updates.Count; i++)
            //    {
            //        GlobalData.updateText[i] = updates[i].InnerText.TrimEnd() + "\n";
            //    }
            //}
            //GlobalData.webVersion = dataXml.SelectSingleNode("ClientVersion").Attributes[0].Value;
            AnalyseDataConfig(false);
        }
        /// <summary>
        /// 检测更新软件是否有更新
        /// step 
        /// </summary>
        private void CheckSelfVersion()
        {
            if (!CheckInternet())
            {
                return;
            }
            var path = Path.Combine(Environment.CurrentDirectory, "Launch.config");
            string oVersion = "v1.0";
            string nVersion = "v1.0";
            if (!File.Exists(path))
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    using (var sr = new StreamWriter(fs))
                    {
                        sr.WriteLine(oVersion);
                    }
                }
            }
            else
            {
                using (var sr = new StreamReader(path))
                {
                    oVersion = sr.ReadLine();
                }
            }

            var addr = GlobalData.mAccount.Webaddr.Replace("Version-C.config", "Launch/Launch.config");
            var addr1 = Utility.AnalysisFTPAddr(addr);
            var lPath = Path.Combine(Directory.GetCurrentDirectory(), "Launch.config.tmp");
            if (File.Exists(lPath))
            {
                File.Delete(lPath);
            }
            FTPDownload ftp = new FTPDownload(GlobalData.mAccount.UserName, GlobalData.mAccount.Password);
            
            while (!ftp.Download(addr, lPath))
            {
                //ftp = new FTPDownload("anonymous", "yungshing@tom.com");
                continue;
            }
            using (var sr = new StreamReader(lPath))
            {
                nVersion = sr.ReadLine();
            }
            GlobalData.selfVersion = nVersion;
            if (nVersion == oVersion)
            {
                return;
            }
            ftp.SetProgerssBar(doShowProgressBar);
            addr = addr.Replace("Launch.config", "UpdateSystem.exe");
            lPath = lPath.Replace("Launch.config.tmp", "UpdateSystem.exe.tmp");
            if (File.Exists(lPath))
            {
                File.Delete(lPath);
            }
            while (!ftp.Download(addr, lPath))
            {
                //ftp = new FTPDownload("anonymous", "yungshing@tom.com");
                continue;
            }
            var p = Path.Combine(Environment.CurrentDirectory, "Launch.exe");
            System.Diagnostics.Process.Start(p, "-callUpdate");
            Utility.ExitApp();
        }

        /// <summary>
        /// 内部测试版更新
        /// </summary>
        private void CheckSelfVersion_Alpha()
        {
            if (!CheckInternet())
            {
                return;
            }
            var path = Path.Combine(Environment.CurrentDirectory, "Launch.config");
            string oVersion = "v1.0";
            string nVersion = "v1.0";
            if (!File.Exists(path))
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    using (var sr = new StreamWriter(fs))
                    {
                        sr.WriteLine(oVersion);
                    }
                }
            }
            else
            {
                using (var sr = new StreamReader(path))
                {
                    oVersion = sr.ReadLine();
                }
            }

            var addr = GlobalData.mAccount.Webaddr.Replace("Version-C.config", "Launch\\Alpha\\Launch.config");
            var lPath = Path.Combine(Directory.GetCurrentDirectory(), "Launch.config.tmp");
            if (File.Exists(lPath))
            {
                File.Delete(lPath);
            }
            FTPDownload ftp = new FTPDownload(GlobalData.mAccount.UserName, GlobalData.mAccount.Password);
            
            while (!ftp.Download(addr, lPath))
            {
                // ftp = new FTPDownload("anonymous", "yungshing@tom.com");
                continue;
            }
            using (var sr = new StreamReader(lPath))
            {
                nVersion = sr.ReadLine();
            }
            GlobalData.selfVersion = nVersion;
            if (nVersion == oVersion)
            {
                return;
            }
            ftp.SetProgerssBar(doShowProgressBar);
            addr = addr.Replace("Launch.config", "UpdateSystem.exe");
            lPath = lPath.Replace("Launch.config.tmp", "UpdateSystem.exe.tmp");
            if (File.Exists(lPath))
            {
                File.Delete(lPath);
            }
            while (!ftp.Download(addr, lPath))
            {
                continue;
            }
            var p = Path.Combine(Environment.CurrentDirectory, "Launch.exe");
            System.Diagnostics.Process.Start(p, "-callUpdate -alpha");
            Utility.ExitApp();
        }
    }
}
