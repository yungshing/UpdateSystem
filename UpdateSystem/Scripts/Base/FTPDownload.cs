
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
namespace UpdateSystem
{
    public class FTPDownload : IDownload, IDisposable
    {
        private event GlobalEvent.CallV_II doChangeProgressBarValue;
        private event GlobalEvent.CallV_S doShowDownloadSpeed;
        /// <summary>
        /// 显示剩余时间
        /// </summary>
        private event GlobalEvent.CallV_S doShowRemainTime;
        /// <summary>
        /// 显示剩余百分比
        /// </summary>
        private event GlobalEvent.CallV_S doShowPercent;

        public Exception E
        {
            get
            {
                return _e;
            }
        }
        private Exception _e;
        public FTPDownload(string username, string password)
        {
            userName = username;
            this.password = password;
        }
        public FTPDownload()
        {

        }
        private string userName, password;
        private bool pause = false;
        public bool Pause
        {
            get { return pause; }
            set { pause = value; }
        }
        public float DownloadSpeed { get;protected set; }
        private FtpWebRequest Request = null;
        private FtpWebResponse Response;
        private Stream GetStream;
        FileStream WrStream;
        private void ConnectFTP(string url)
        {
            _ShowDownloadSpeed("建立通信中..");
            Request = (FtpWebRequest)WebRequest.Create(new Uri(url));
            Request.KeepAlive = false;
            Request.UseBinary = true;
            Request.Timeout = 15000;
            if (userName == null || userName.Replace(" ", "") == "")
            {
                Request.Credentials = new NetworkCredential();
            }
            else
            {
                Request.Credentials = new NetworkCredential(userName, password);
            }
            Request.Method = WebRequestMethods.Ftp.DownloadFile;
        }
        /// <summary>
        /// 路径要带后缀名
        /// </summary>
        /// <param name="downloadPath"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public bool Download(string downloadPath, string savePath)
        {
            try
            {
                while (Pause)
                {
                    continue;
                }
                if (savePath.Contains("data.config.tmp"))
                {
                    GlobalEvent.SaveDebugLog();
                }
                _SetProgressBar(0, 1);
                _ShowPercent("0%");
                ConnectFTP(downloadPath);
                long downloadBytes = 0;
                long TotalBytes = 0;
                string _path_Tmp = savePath + ".zzfz";
                if (File.Exists(savePath))
                {
                    _SetProgressBar(1, 1);
                    _ShowRemainTime("0");
                    _ShowPercent("100%");
                    _ShowDownloadSpeed("已下载");
                    return true;
                }
                ///断点续传
                if (File.Exists(_path_Tmp))
                {
                    WrStream = File.OpenWrite(_path_Tmp);
                    downloadBytes = WrStream.Length;
                    WrStream.Seek(downloadBytes, SeekOrigin.Current);
                    Request.ContentOffset = downloadBytes;
                    TotalBytes = Size(downloadPath);
                    _SetProgressBar((int)downloadBytes, (int)TotalBytes);
                    if (WrStream.Length == TotalBytes)
                    {
                        Dispose();
                        File.Move(_path_Tmp, savePath);
                        return true;
                    }
                }
                else
                {
                    WrStream = new FileStream(_path_Tmp, FileMode.Create, FileAccess.ReadWrite);
                    TotalBytes = Size(downloadPath);
                }
                Stopwatch stopWatch = new Stopwatch();
                _ShowDownloadSpeed("接收数据中..");
                Response = (FtpWebResponse)Request.GetResponse();
                GetStream = Response.GetResponseStream();
                int RemainTime = 0;
                int n = 1;
                var bytes = new byte[102400];
                stopWatch.Reset();
                stopWatch.Start();
                int speed_tmp = 0;
                while (n > 0)
                {
                    if (Pause) continue;
                    n = GetStream.Read(bytes, 0, bytes.Length);
                    downloadBytes += n;
                    _SetProgressBar((int)downloadBytes, (int)TotalBytes);
                    var f = (float)downloadBytes / (float)TotalBytes;
                    f = f * 100f;
                    _ShowPercent(((int)f).ToString() + "%");
                    speed_tmp += n;
                    if (stopWatch.Elapsed.Milliseconds >= 800)
                    {
                        stopWatch.Stop();
                        DownloadSpeed = speed_tmp * 1000f / stopWatch.Elapsed.Milliseconds;
                        _ShowDownloadSpeed(DownloadSpeed.ToString());
                        RemainTime = (int)((TotalBytes - downloadBytes) / DownloadSpeed);
                        _ShowRemainTime(RemainTime.ToString());
                        speed_tmp = 0;
                        stopWatch.Reset();
                        stopWatch.Start();
                    }
                    WrStream.Write(bytes, 0, n);
                }
                GetStream.Dispose();
                WrStream.Dispose();
                File.Move(_path_Tmp, savePath);
                Dispose();
                return true;
            }
            catch (Exception e)
            {
                _e = e;
                GlobalEvent.WriteLog(e.Message);
                Dispose();
                return false;
            }
            finally
            {
                Dispose();
            }
        }

        private bool Download(string downloadPath, string savePath, long from, long to)
        {
            try
            {
                while (Pause)
                {
                    continue;
                }
                doChangeProgressBarValue(0, 1);
                _ShowPercent("0%");
                ConnectFTP(downloadPath);
                long downloadBytes = 0;
                long TotalBytes = 0;
                string _path_Tmp = savePath + ".zzfz";
                if (File.Exists(savePath))
                {
                    _SetProgressBar(1, 1);
                    _ShowRemainTime("0");
                    _ShowPercent("100%");
                    _ShowDownloadSpeed("0");
                    return true;
                }
                ///断点续传
                if (File.Exists(_path_Tmp))
                {
                    WrStream = File.OpenWrite(_path_Tmp);
                    downloadBytes = WrStream.Length;
                    WrStream.Seek(downloadBytes, SeekOrigin.Current);
                    Request.ContentOffset = downloadBytes;
                    TotalBytes = Size(downloadPath);
                    if (TotalBytes < 0)
                    {
                        return false;
                    }
                    _SetProgressBar((int)downloadBytes, (int)TotalBytes);
                    if (WrStream.Length == TotalBytes)
                    {
                        Dispose();
                        File.Move(_path_Tmp, savePath);
                        return true;
                    }
                }
                else
                {
                    WrStream = new FileStream(_path_Tmp, FileMode.Create, FileAccess.ReadWrite);
                    TotalBytes = Size(downloadPath);
                }
                Stopwatch stopWatch = new Stopwatch();
                Response = (FtpWebResponse)Request.GetResponse();
                GetStream = Response.GetResponseStream();
                int RemainTime = 0;
                int n = 1;
                var bytes = new byte[102400];
                stopWatch.Reset();
                stopWatch.Start();
                int speed_tmp = 0;
                while (n > 0)
                {
                    if (Pause) continue;
                    n = GetStream.Read(bytes, 0, bytes.Length);
                    downloadBytes += n;
                    _SetProgressBar((int)downloadBytes, (int)TotalBytes);
                    var f = (float)downloadBytes / (float)TotalBytes;
                    f = f * 100f;
                    _ShowPercent(((int)f).ToString() + "%");
                    speed_tmp += n;
                    if (stopWatch.Elapsed.Milliseconds >= 500)
                    {
                        stopWatch.Stop();
                        DownloadSpeed = speed_tmp / stopWatch.Elapsed.Milliseconds * 1000;
                        _ShowDownloadSpeed(DownloadSpeed.ToString());
                        RemainTime = (int)((TotalBytes - downloadBytes) / DownloadSpeed);
                        _ShowRemainTime(RemainTime.ToString());
                        speed_tmp = 0;
                        stopWatch.Reset();
                        stopWatch.Start();
                    }
                    WrStream.Write(bytes, 0, n);
                }
                Dispose();
                File.Move(_path_Tmp, savePath);
                return true;
            }
            catch (Exception e)
            {
                _e = e;
                GlobalEvent.WriteLog(e.Message);
                Dispose();
                return false;
            }
            finally
            {
                Dispose();
            }
        }
        void _SetProgressBar(int c, int max)
        {
            if (doChangeProgressBarValue != null)
            {
                doChangeProgressBarValue(c, max);
            }
        }
        /// <summary>
        /// 设置下载进度条
        /// 传值：e.g. (currValue,MaxValue)
        /// </summary>
        /// <param name="a"></param>
        public void SetProgerssBar(GlobalEvent.CallV_II a)
        {
            doChangeProgressBarValue = a;
        }

        private void _ShowRemainTime(string s)
        {
            if (doShowRemainTime != null)
            {
                doShowRemainTime(s);
            }
        }
        void _ShowDownloadSpeed(string speed)
        {
            if (doShowDownloadSpeed != null)
            {
                doShowDownloadSpeed(speed);
            }
        }
        private void _ShowPercent(string s)
        {
            if (doShowPercent != null)
            {
                doShowPercent(s);
            }
        }
        public void SetDownloadPercent(GlobalEvent.CallV_S a)
        {
            doShowPercent = a;
        }
        /// <summary>
        /// 显示下载速度
        /// </summary>
        public void SetDownloadSpeed(GlobalEvent.CallV_S a)
        {
            doShowDownloadSpeed = a;
        }
        void CallShowRemainTime(string content)
        {
            if (doShowRemainTime != null)
            {
                doShowRemainTime(content);
            }
        }
        /// <summary>
        /// 显示剩余下载时间
        /// </summary>
        /// <param name="a"></param>
        public void SetShowRemainTime(GlobalEvent.CallV_S a)
        {
            doShowRemainTime = a;
        }
        public long Size(string downloadPath)
        {
            FtpWebResponse res = null;
            FtpWebRequest r = null;
            long size = 0;
            try
            {
                r = (FtpWebRequest)WebRequest.Create(new Uri(downloadPath));
                r.KeepAlive = false;
                r.Method = WebRequestMethods.Ftp.GetFileSize;
                r.Timeout = 5000;
                r.Credentials = new NetworkCredential(userName, password);
                using (res = (FtpWebResponse)r.GetResponse())
                {
                    using (var sm = res.GetResponseStream())
                    {
                        size = res.ContentLength;
                    }
                }
            }
            catch 
            {
                size = -1;
            }
            finally
            {
                if (r != null)
                {
                    r.Abort();
                }
                if (res != null)
                {
                    res.Close();
                }
            }
            return size;
        }
        public void Dispose()
        {
            try { WrStream.Dispose(); }
            catch { }
            try { GetStream.Dispose(); }
            catch { }
            try { Response.Close(); }
            catch { }
            try { Request.Abort(); }
            catch { }
        }
       public void RemoveInvoke()
        {
            doChangeProgressBarValue = null;
            doShowDownloadSpeed = null;
            doShowRemainTime = null;
        }
    }

    public class FTPWebClient
    {
        public DownloadProgressChangedEventHandler doShowDownloadSpeed;

        private string username;
        private string password;
        public FTPWebClient(string username, string pw)
        {
            this.username = username;
            this.password = pw;
        }
        public void Download(string webaddr, string filename)
        {
            WebClient wc = new WebClient();
            wc.Credentials = new NetworkCredential(username, password);
            wc.DownloadProgressChanged += doShowDownloadSpeed;
            wc.DownloadFileAsync(new Uri(webaddr), filename);
        }

        public long GetSize(string addr)
        {
            WebClient wc = new WebClient();
            wc.Credentials = new NetworkCredential(username, password);
            
            return 0;
        }
    }
}