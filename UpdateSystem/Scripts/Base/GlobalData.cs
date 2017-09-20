using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
namespace UpdateSystem
{
    public static class GlobalData
    {
        private static string webXmlAddress = "ftp://120.76.28.224";

        public static bool IsFirstUse = false;

        public static string Version = "";
        public static string WebVersion = "we";
        /// <summary>
        /// 另一张表，检测要下载哪些文件用的
        /// </summary>
        public static string DataWebAddress = "";
        /// <summary>
        /// 是否正在下载更新文件
        /// 如果true，则后台不检测更新
        /// 如果false,则后台在指定时间间隔检测更新
        /// </summary>
        public static bool IsUpdating = false;
        public static string WebXmlAddress
        {
            get
            {
                return webXmlAddress;
            }
            set 
            {
                if (value .Length >= 4 && value.Substring(0,4).Contains("ftp"))
                {
                    webXmlAddress = value;
                }
                else
                {
                    webXmlAddress = Path.Combine(GlobalData.ftpAddress.CurrAddress, value);
                }
            }
        }


        public static VersionXML localXML = new VersionXML();
        public static VersionXML webXML = new VersionXML();
        public static List<XMLFileInfo> needUpdateFiles = new List<XMLFileInfo>();
        /// <summary>
        /// 本地有哪些文件没有用了，删掉
        /// </summary>
        public static List<XMLFileInfo> needDeleteFiles = new List<XMLFileInfo>();

        public static FilePathSet filePath = new FilePathSet();
        public static FTPAddress ftpAddress = new FTPAddress();
        public static Account mAccount = new Account();
        /// <summary>
        /// 检测到了有更新 ？
        /// </summary>
        public static bool CheckedUpdate = false;
        /// <summary>
        /// 更新检测间隔时间
        /// </summary>
        public static int UpdateTime = 7200;
        public static string[] UpdateText = new string[1] { "优化版本，修复一些Bug" };
        public static bool isPause = false;

       // public static Form1 mainForm;


        public static List<string> UpdateNodesName = new List<string>();
        public static List<string> UpdateCarNodesName = new List<string>();

        public static string DebugLogName = "Log";

        public static bool isDebug = false;
       
    }
   
    public class GlobalEvent
    {
        public delegate void CallV_S(string s);
        public delegate void CallV_SI(string s, int i);
        public delegate void CallV_II(int i, int j);
        public delegate void CallV_V();
        public delegate void CallV_B(bool b);

        static List<string> DebugLog = new List<string>();

        public static void WriteLog(string log)
        {
            if (!GlobalData.isDebug)
            {
                return;
            }
            log = DateTime.Now.ToString() + "--" + log;
            DebugLog.Add(log);
        }
        public static void SaveDebugLog()
        {
            if (!GlobalData.isDebug) return;
            //string v = @"D:\Log_" + DateTime.Now.ToString("MM-dd--HH-mm-ss").Replace(" ","") + ".txt";
            string v = GlobalData.DebugLogName;
            if (File.Exists(v))
            {
                File.Delete(v);
            }
            using (FileStream fs = new FileStream(v, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    for (int i = 0; i < DebugLog.Count; i++)
                    {
                        sw.WriteLine(DebugLog[i]);
                    }
                }
            }
        }
    }
}