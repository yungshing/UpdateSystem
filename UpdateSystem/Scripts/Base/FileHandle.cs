
using System.Collections.Generic;

namespace UpdateSystem
{
    //public class FileInfomation
    //{
    //    private string fileHash;
    //    /// <summary>
    //    /// 文件hash值
    //    /// </summary>
    //    public string FileHash
    //    {
    //        get { return fileHash; }
    //        set { fileHash = value; }
    //    }
    //    private string fileFullName;
    //    /// <summary>
    //    /// 文件名字带后缀
    //    /// </summary>
    //    public string FileFullName
    //    {
    //        get { return fileFullName; }
    //        set { fileFullName = value; }
    //    }

    //    private string downloadAddress;
    //    /// <summary>
    //    /// 文件下载地址
    //    /// </summary>
    //    public string DownloadAddress
    //    {
    //        get { return downloadAddress; }
    //        set { downloadAddress = value; }
    //    }
    //    private string movePath;
    //    /// <summary>
    //    /// 文件移动的目标路径(带名字和的后缀)
    //    /// </summary>

    //    public string MoveFullPath
    //    {
    //        get { return movePath; }
    //        set { movePath = value; }
    //    }
    //    private string tmpPath;
    //    /// <summary>
    //    /// 文件本地缓存路径。(带名字和的后缀)
    //    /// </summary>

    //    public string TmpFullPath
    //    {
    //        get { return tmpPath; }
    //        set { tmpPath = value; }
    //    }
    //}
    //public class ProcessInfo
    //{
    //    private string name;

    //    /// <summary>
    //    /// 程序名字
    //    /// </summary>
    //    public string Name
    //    {
    //        get { return name; }
    //        set { name = value; }
    //    }
    //    private string localPath;
    //    /// <summary>
    //    /// 存放的硬盘相对路径
    //    /// </summary>
    //    public string LocalFullPath
    //    {
    //        get { return localPath; }
    //        set { localPath = value; }
    //    }

    //    private string[] killNames;
    //    /// <summary>
    //    /// 需要杀死的进程名字
    //    /// </summary>
    //    public string[] KillNames
    //    {
    //        get { return killNames; }
    //        set { killNames = value; }
    //    }
    //}
    //public class Account
    //{
    //    private string userName = "ftpa";

    //    public string UserName
    //    {
    //        get { return userName; }
    //        set { userName = value; }
    //    }

    //    private string password = "123456";

    //    public string Password
    //    {
    //        get { return password; }
    //        set { password = value; }
    //    }
    //    /// <summary>
    //    /// 格式 ftp://120.76.28.224/WenDingVersion-C/Config/Version-C.config
    //    /// </summary>
    //    public string Webaddr
    //    {
    //        get
    //        {
    //            return webaddr;
    //        }

    //        set
    //        {
    //            webaddr = value;
    //        }
    //    }

    //    private string webaddr;

    //}
    /// <summary>
    /// data.config 数据
    /// </summary>
    public class DataXML
    {
        public DataXML()
        {
            ip = new List<string>();
            username = new List<string>();
            password = new List<string>();
            version_cAddr = new List<string>();
            data_config_Addr = new List<string>();
        }
        private List<string> username;
        private List<string> password;
        private List<string> ip;
        private List<string> version_cAddr;
        private List<string> data_config_Addr;
        private int currIndex = 0;

        public int CurrIndex
        {
            get { return currIndex; }
            set
            {
                currIndex = value;
                if (CurrIndex >= IP.Count)
                {
                    CurrIndex = 0;
                }
            }
        }

        /// <summary>
        /// 格式：
        /// ftp://120.76.28.224/
        /// ftp://10.46.98.208/
        /// 中的一个
        /// </summary>
        public string CurrIP
        {
            get
            {
                if (CurrIndex >= IP.Count)
                {
                    CurrIndex = 0;
                }
                return IP[CurrIndex];
            }
        }
        public string CurrFTPUsername
        {
            get
            {
                if (CurrIndex >= FTPUsername.Count)
                {
                    CurrIndex = 0;
                }
                return FTPUsername[CurrIndex];
            }
        }

        public string CurrFTPPassword
        {
            get
            {
                if (CurrIndex >= FTPPassword.Count)
                {
                    CurrIndex = 0;
                }
                return FTPPassword[CurrIndex];
            }
        }
        public string CurrVersionAddr
        {
            get
            {
                if (CurrIndex >= Version_CAddr.Count)
                {
                    CurrIndex = 0;
                }
                return Version_CAddr[CurrIndex];
            }
        }
        public string CurrDataAddr
        {
            get
            {
                if (CurrIndex >= DataConfigAddr.Count)
                {
                    CurrIndex = 0;
                }
                return DataConfigAddr[CurrIndex];
            }
        }


        /// <summary>
        /// 格式：
        /// ftp://120.76.28.224
        /// ftp://10.46.98.208
        /// </summary>
        public List<string> IP
        {
            get
            {
                return ip;
            }
            set
            {
                ip = value;
            }
        }
        public List<string> FTPUsername
        {
            get
            {
                return username;
            }

            set
            {
                username = value;
            }
        }

        public List<string> FTPPassword
        {
             get
            {
                return password;
            }

            set
            {
                password = value;
            }
        }

        public List<string> Version_CAddr
        {
             get
            {
                return version_cAddr;
            }
            set
            {
                version_cAddr = value;
            }
        }
        public List<string> DataConfigAddr
        {
             get
            {
                return data_config_Addr;
            }
            set
            {
                data_config_Addr = value;
            }
        }
    }
}
