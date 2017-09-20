using System.Collections.Generic;
using System.Xml.Serialization;

namespace UpdateSystem
{
    [XmlRoot(ElementName = "ConfigData")]
    public class VersionXML
    {
        public VersionXML()
        {
            x_FileList = new XMLDirectoryList();
            x_ProcessInfo = new XMLProcessInfo();
            x_FtpAccount = new XMLFtpAccount();
        }
        [XmlElement("Version")]
        public string x_Version = "V1.0";
        [XmlElement("FileList")]
        public XMLDirectoryList x_FileList;
        [XmlElement("ProcessInfo")]
        public XMLProcessInfo x_ProcessInfo;
        [XmlArray("Updates")]
        [XmlArrayItem("uts")]
        public string[] x_Updates = new string[] { "初始使用下载全部文件" };
        [XmlElement("UpdateTime")]
        public int x_UpdateTime = 7200;
        [XmlElement("UpdateWebAddress")]
        public string x_UpdateWebAddress = @"WenDingVersion-B\Config\Version-C.config";
        [XmlArray("FtpAddress")]
        [XmlArrayItem("ftp")]
        public string[] x_FTPAddress = new string[] { "ftp://120.76.28.224/" };
        [XmlElement("Account")]
        public XMLFtpAccount x_FtpAccount;
        [XmlArray("KillProcessName")]
        [XmlArrayItem("k")]
        public string[] x_KillProcessNames = new string[] { "Unity.exe", "UI2.0.exe" };
    }

    [XmlRoot("FileList")]
    public class XMLDirectoryList
    {
        public XMLDirectoryList()
        {
            x_base = new XMLFileList();
            x_change = new List<XMLFileList>();
            x_other = new XMLFileList();
        }
        [XmlElement("Base")]
        public XMLFileList x_base;
        [XmlArray("Change")]
        [XmlArrayItem("Child")]
        public List<XMLFileList> x_change;
        [XmlElement("Other")]
        public XMLFileList x_other;
    }

    [XmlRoot("Change")]
    public class XMLFileList
    {
        public XMLFileList()
        {
            Files = new List<XMLFileInfo>();
        }
        /// <summary>
        /// change文件下的文件使用的，用于判断是哪个文件夹下的文件
        /// </summary>
        [XmlElement("Folder")]
        public string Folder;
        [XmlArray("FileDataArray")]
        [XmlArrayItem("FileData")]
        public List<XMLFileInfo> Files;
    }
    public class XMLFileInfo
    {
        public string Name;
        public string Hash;
        /// <summary>
        /// 下载地址
        /// e.g. ftp://120.76.28.224/WenDingVersion-B/Base/Bione.dll
        /// 刚Address为 WenDingVersion-B\Base\Bione.dll
        /// </summary>
        public string Address;
        /// <summary>
        /// 安装路径，文件下载完成后，安装到哪个路径
        /// e.g. WenDingVersion-B\Bione.dll
        /// </summary>
        public string InstallPath;
        /// <summary>
        /// 文件上传时地址（临时存放路径）
        /// e.g. ftp://120.76.28.224/Test/tmp/Base/a.txt
        /// </summary>
        [XmlIgnore]
        public string UploadloadTmpAddr;
        /// <summary>
        /// 文件上传完成后，本地副本移到缓存目录
        /// e.g. E:Test/tmp/Base/a.txt
        /// </summary>
        [XmlIgnore]
        public string UploadOverLocalPath;
    }
    [XmlRoot]
    public class XMLFtpAccount
    {
        [XmlElement("Username")]
        public string Username = "ftpa";
        [XmlElement("Password")]
        public string Password = "123456";
    }
    [XmlRoot]
    public class XMLProcessInfo
    {
        [XmlElement("Path")]
        public string path;
        [XmlElement("Name")]
        public string name;
    }
}
