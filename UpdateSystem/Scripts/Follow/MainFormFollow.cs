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

            ///step  ------------解析本地data.config文件
            ///获取版本号
            AnalyseDataConfig();

            ///step---------解析Version-C.config
            ///2017.11.01 好像没有在此解析的必要了
            ///2017.11.14 还是得解析本地XML
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
            ///2017.11.01 必需下载data.config
            if (!NeedDownloadDataConfig())
            {
                //return true;
            }

            ///step  --------------下载data.config,保存为data.config.tmp
            DownloadDataConfig();
            ///step-----分析下载的data.config.tmp
            AnalyseDataConfig(false);
            ///step  ------------对比data.config和data.config.tmp中version,检测是否有更新
            var isUpdate = CheckNewVersion();
            if (!isUpdate)
            {
                ///下载广告
                System.Threading.Thread adThread = new System.Threading.Thread(DownADImgs);
                adThread.Start();
            }
            return isUpdate;
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
                GlobalData.localVersionXML = Utility.Decode<VersionXML>(GlobalData.filePath.ConfigDataFullPath);
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
        private bool CheckNewVersion()
        {
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

            ///广告
            try
            {
                GlobalData.adVersion = xmlDoc.SelectSingleNode("Version/CompanyLogo").Attributes["value"].InnerText;
            }
            catch { }

            ///更新内容
            if (CheckFirstUse())
            {
                GlobalData.updateText = new string[] { "首次更新将会下载全部文件需要一定时间，请耐心等待。" };
            }
            else
            {
                if (xmlDoc.SelectSingleNode("Version/UpdateText") != null)
                {
                    var updates = xmlDoc.SelectSingleNode("Version/UpdateText").ChildNodes;
                    GlobalData.updateText = new string[updates.Count];
                    for (int i = 0; i < updates.Count; i++)
                    {
                        GlobalData.updateText[i] = updates[i].InnerText.TrimEnd() + "\n";
                    }
                }
            }
            ///FTP信息
            try
            {
                var v = xmlDoc.SelectNodes("Version/FTP");

                for (int i = 0; i < v.Count; i++)
                {
                    GlobalData.dataXML.FTPUsername.Add(v[i].Attributes[0].Value);
                    GlobalData.dataXML.FTPPassword.Add(v[i].Attributes[1].Value);
                    GlobalData.dataXML.Version_CAddr.Add(v[i].Attributes[2].Value);
                    GlobalData.dataXML.IP.Add(Utility.AnalysisFTPAddr(v[i].Attributes[2].Value)[0]);
                }

                v = xmlDoc.SelectNodes("Version/Path");
                for (int i = 0; i < v.Count; i++)
                {
                    GlobalData.dataXML.DataConfigAddr.Add(v[i].Attributes[0].Value);
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("配置文件损坏");
                System.Environment.Exit(0);
            }
            ///客户端版本
            if (xmlDoc.SelectSingleNode("Version/ClientVersion") != null)
            {
                var version = xmlDoc.SelectSingleNode("Version/ClientVersion ").Attributes[0].Value;
                if (isLocal)
                {
                    GlobalData.version = version;
                }
                else
                {
                    GlobalData.webVersion = version;
                }
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
            var ftp = Utility.CreateFTPDownload();
            var p = Path.Combine(Directory.GetCurrentDirectory(), "data.config.tmp");
            if (File.Exists(p))
            {
                File.Delete(p);
            }
            ftp.SetProgerssBar(doShowProgressBar);

            while (!ftp.Download(GlobalData.dataXML.CurrDataAddr, p))
            {
                if (Utility.SetException(ftp.E) == Utility.EcpType.LimitConnect)
                {
                    GlobalData.dataXML.CurrIndex++;
                }
                continue;
            }
            ftp.Dispose();
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

            var addr = GlobalData.dataXML.CurrVersionAddr.Replace("Version-C.config", "Launch/Launch.config");
            var lPath = Path.Combine(Directory.GetCurrentDirectory(), "Launch.config.tmp");
            if (File.Exists(lPath))
            {
                File.Delete(lPath);
            }
            var ftp = Utility.CreateFTPDownload();
            ///下载Launch.config，判断更新软件是否有更新 
            while (!ftp.Download(addr, lPath))
            {
                if (Utility.SetException(ftp.E) == Utility.EcpType.LimitConnect)
                {
                    GlobalData.dataXML.CurrIndex++;
                    addr = GlobalData.dataXML.CurrVersionAddr.Replace("Version-C.config", "Launch/Launch.config");
                }
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
            ///如果更新软件有更新
            ftp.SetProgerssBar(doShowProgressBar);
            addr = addr.Replace("Launch.config", "UpdateSystem.exe");
            lPath = lPath.Replace("Launch.config.tmp", "UpdateSystem.exe.tmp");
            if (File.Exists(lPath))
            {
                File.Delete(lPath);
            }
            while (!ftp.Download(addr, lPath))
            {
                if (Utility.SetException(ftp.E) == Utility.EcpType.LimitConnect)
                {
                    GlobalData.dataXML.CurrIndex++;
                    addr = GlobalData.dataXML.CurrVersionAddr.Replace("Version-C.config", "Launch/Launch.config");
                }
            }
            var p = Path.Combine(Environment.CurrentDirectory, "Launch.exe");
            System.Diagnostics.Process.Start(p, "-callUpdate");
            Utility.ExitApp();
        }

        private void DownADImgs()
        {
            if (File.Exists(GlobalData.filePath.ConfigDataFullPath_Tmp))
            {
                File.Delete(GlobalData.filePath.ConfigDataFullPath_Tmp);
            }
            if (File.Exists(GlobalData.filePath.ConfigDataFullPath_Tmp + ".zzfz"))
            {
                File.Delete(GlobalData.filePath.ConfigDataFullPath_Tmp + ".zzfz");
            }
            var ftp = Utility.CreateFTPDownload();
            while (!ftp.Download(GlobalData.dataXML.CurrVersionAddr, GlobalData.filePath.ConfigDataFullPath_Tmp))
            {
                Utility.SetException(ftp.E);
                if (File.Exists(GlobalData.filePath.ConfigDataFullPath_Tmp + ".zzfz"))
                {
                    File.Delete(GlobalData.filePath.ConfigDataFullPath_Tmp + ".zzfz");
                }
            }
            GlobalData.webVersionXML = Utility.Decode<VersionXML>(GlobalData.filePath.ConfigDataFullPath_Tmp);
            foreach (var item in GlobalData.webVersionXML.x_FileList.x_change)
            {
                if (item.Folder == "CompanyLogo")
                {
                    var logo = item.Files.FindAll(d => d.Address.Contains(GlobalData.adVersion));
                    var ftpa = Utility.CreateFTPDownload();
                    foreach (var m in logo)
                    {
                        var dP = Path.Combine(GlobalData.dataXML.CurrIP, m.Address);
                        var nP = Path.Combine(GlobalData.filePath.ProgramPath, m.InstallPath);
                        if (File.Exists(nP))
                        {
                            if (Utility.GetMD5Value(nP) == m.Hash)
                            {
                                continue;
                            }
                            else
                            {
                                File.Delete(nP);
                            }
                        }
                        if (File.Exists(nP + ".zzfz"))
                        {
                            File.Delete(nP + ".zzfz");
                        }
                        GlobalEvent.WriteLog("下载广告：" + m.Address);
                        while (!ftpa.Download(dP.Replace("\\", "/"), nP))
                        {
                            GlobalEvent.WriteLog("下载广告失败：" + m.Address + "-----" + ftpa.E.Message);
                            if (Utility.SetException(ftpa.E) == Utility.EcpType.LimitConnect)
                            {
                                GlobalData.dataXML.CurrIndex++;
                                dP = Path.Combine(GlobalData.dataXML.CurrIP, m.Address);
                            }
                        }
                    }
                    return;
                }
            }
        }
    }
}
