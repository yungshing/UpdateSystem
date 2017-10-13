using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace UpdateSystem
{
    public class Utility
    {
        public static void KillProgram()
        {
            if (GlobalData.localXML.x_KillProcessNames == null)
            {
                return;
            }
            foreach (var item in GlobalData.localXML.x_KillProcessNames)
            {
                var v = item.Replace(".exe", "");
                foreach (var item2 in Process.GetProcessesByName(v))
                {
                    item2.Kill();
                }
            }
        }

        public static bool OpenProgram()
        {
            if (File.Exists(GlobalData.filePath.ProgramFullPath))
            {
                Process p = new Process();
                p.StartInfo.FileName = GlobalData.filePath.ProgramFullPath;
                p.StartInfo.WorkingDirectory = GlobalData.filePath.ProgramFullPath.Replace(GlobalData.localXML.x_ProcessInfo.name, "");
                p.Start();
                GlobalEvent.WriteLog("打開程序成功：" + GlobalData.filePath.ProgramFullPath);
                GlobalEvent.SaveDebugLog();
                return true;
            }
            GlobalEvent.WriteLog("打開程序失敗：" + GlobalData.filePath.ProgramFullPath);
            GlobalEvent.SaveDebugLog();
            return false;
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        static public T Decode<T>(string path)
        {
            string s = "";
            using (var sr = new StreamReader(path, Encoding.UTF8))
            {
                s = sr.ReadToEnd();
            }
            var c = s.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                c[i] -= 'z';
            }
            s = new string(c);
            T t = default(T);
            var xmlser = new XmlSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(s)))
            {
                t = (T)xmlser.Deserialize(ms);
            }
            return t;
        }

        /// <summary>
        /// 解密Data配置文件，非下载的配置文件
        /// </summary>
        /// <returns></returns>
        public static XmlDocument ODecode(string path)
        {
            XmlDocument xml = new XmlDocument();
            byte[] key = new byte[8] { 0x01, 0x04, 0x03, 0x04, 0x01, 0x06, 0x06, 0x08 };
            byte[] iv = new byte[8] { 0x0a, 0x07, 0x0c, 0x01, 0x04, 0x03, 0x02, 0x0b };
            using (var fs = new FileStream(path, FileMode.Open))
            {
                using (var des = new DESCryptoServiceProvider { Key = key, IV = iv })
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            int n = 1;
                            var buff = new byte[1024];
                            while (n > 0)
                            {
                                n = fs.Read(buff, 0, buff.Length);
                                cs.Write(buff, 0, n);
                            }
                            cs.FlushFinalBlock();

                            ms.Position = 0;
                            xml.Load(ms);
                        }
                    }
                }
            }
            return xml;
        }

        public static string GetMD5Value(string fileName)
        {
            string md5 = "";
            
            if (!File.Exists(fileName))
            {
                return md5;
            }
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                MD5 cal = MD5.Create();
                byte[] buff = cal.ComputeHash(fs);
                cal.Clear();
                StringBuilder strBu = new StringBuilder();
                for (int i = 0; i < buff.Length; i++)
                {
                    strBu.Append(buff[i].ToString("x2"));
                }
                md5 = strBu.ToString();
            }
            return md5;
        }
        public static string ToTime(int time)
        {
            if (time == -1)
            {
                return "00:00:00";
            }
            int hour = time / 3600;
            int min = time / 60 - hour * 60;
            int sec = time - hour * 3600 - min * 60;
            if (hour > 0)
            {
                return ShowTime(hour) + ":" + ShowTime(min) + ":" + ShowTime(sec);
            }
            else if (min > 0)
            {
                return "00:" + ShowTime(min) + ":" + ShowTime(sec);
            }
            else
            {
                return "00:00:" + ShowTime(sec);
            }
        }

        static string ShowTime(int time)
        {
            if (time < 10)
            {
                return "0" + time.ToString();
            }
            else
            {
                return time.ToString();
            }
        }

        /// <summary>
        /// 移动配置文件
        /// data.config
        /// </summary>
        public static void CopyOData()
        {
            var p = Path.Combine(Directory.GetCurrentDirectory(), "data.config.tmp");
            var op = Path.Combine(Directory.GetCurrentDirectory(), "data.config");
            File.Delete(op);
            File.Move(p,op);
            var d = Path.Combine(new FileInfo(GlobalData.filePath.ProgramFullPath).Directory.ToString(), "VersionsData\\data.config");
            var od = new FileInfo(d).Directory.FullName;
            if (!Directory.Exists (od))
            {
                Directory.CreateDirectory(od);
            }
            if (File.Exists(d))
            {
                File.Delete(d);
            }
            File.Copy(op, d);
        }

        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        #region 处理网络连接
        public static bool IsConnectInternet()
        {
            return InternetGetConnectedState(0, 0);
        }
        /// <summary>
        /// 与调制器有关
        /// 如有虚拟机，会出现虚拟网上，会一直返回true
        /// </summary>
        /// <param name="dwFlag"></param>
        /// <param name="dwReserved"></param>
        /// <returns></returns>
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(
            int dwFlag,
            int dwReserved
        );

        public static bool IsConnectInternetPing()
        {
            try
            {
                Ping ping = new Ping();
                PingOptions options = new PingOptions();
                options.DontFragment = true;
                string data = "";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 1000;
                PingReply reply = ping.Send("www.baidu.com", timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 处理异常
        public static EcpType SetException(Exception e)
        {
            return SetCurrExceptionType(e);
        }
        private static EcpType SetCurrExceptionType(Exception e)
        {
            GlobalEvent.WriteLog(e.ToString());
            EcpType currEcpType = EcpType.Normal;
            if (e is System.NotSupportedException ||
                           e is System.UriFormatException)
            {
                currEcpType = EcpType.AddressError;
            }
            else if (e.Message.Contains("无法连接到远程服务器"))
            {
                currEcpType = EcpType.SerDisconnect;
            }
            else if (e.Message.Contains("421"))
            {
                currEcpType = EcpType.LimitConnect;
            }
            else if (e.Message.Contains("接收时发生错误"))
            {
                currEcpType = EcpType.ReceiveError;
            }
            else if (e.Message.Contains("操作超时"))
            {
                currEcpType = EcpType.ReceiveError;
            }
            else if (e.Message.Contains("远程主机强迫关闭了一个现有的连接"))
            {
                currEcpType = EcpType.Disconnect;
            }
            else if (e.Message.Contains("331 Password required"))
            {
                currEcpType = EcpType.ErrorPw;
            }
            else if (e.Message.Contains("文件不可用"))
            {
                currEcpType = EcpType.FileNotFound;
            }
            else if (e is System.Threading.ThreadAbortException)
            {
                currEcpType = EcpType.ThreadEcp;
            }
            else currEcpType = EcpType.Null;

            if (!Utility.IsConnectInternetPing())
            {
                currEcpType = EcpType.Disconnect;
            }
            GlobalEvent.WriteLog(e.ToString());
            GlobalEvent.SaveDebugLog();
            return currEcpType;
        }
        public enum EcpType
        {
            Null = 0,
            /// <summary>
            /// 没有异常
            /// </summary>
            Normal,
            /// <summary>
            /// 本地断网
            /// </summary>
            Disconnect,
            /// <summary>
            /// 服务器断网或故障
            /// </summary>
            SerDisconnect,
            AddressError,
            /// <summary>
            /// 连接达到上限
            /// </summary>
            LimitConnect,
            /// <summary>
            /// 线程意外中断异常
            /// </summary>
            ThreadEcp,
            ReceiveError,
            /// <summary>
            /// 没有程序，打开模拟器时，如果找不到程序，则报错
            /// </summary>
            NoProgress,
            /// <summary>
            /// 用户名或者密码错误
            /// </summary>
            ErrorPw,
            /// <summary>
            /// 文件不可用
            /// </summary>
            FileNotFound,
        }
        #endregion
        public static void Delay(int secode)
        {
            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            while(sw.Elapsed.Seconds <= secode)
            {
                continue;
            }
            sw.Stop();
        }
        #region 异常信息内容
        public const string E_Disconnect = "无网络连接";
        public const string E_LimitConnect = "当前网络阻塞，尝试连接服务器";
        public const string E_SerDisconnect = "服务器故障，请稍后在试";
        public const string E_Default = "未知错误";
        public const string E_ReceiveError = "重新接收数据";
        #endregion

        public static void ExitApp()
        {
            Environment.Exit(0);
        }
    }
}
