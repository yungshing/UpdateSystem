#define DEBUG_NO
using System;
using System.IO;
namespace UpdateSystem
{
    public class ConstValue
    {
        public const string ConfigDataFullName = "Version-C.config";
        /// <summary>
        /// 文本名字，用与检测是否有更新，内容只有两条，一个bool变量和配置文件下载地址
        /// </summary>
        public const string UpdateSign = "Version";

        public const string RootFolder = "ZZFZ";
        public const string Program = "Program";
        public const string DataFolder = "Data";
        public const string UpdateFolder = "Update";

        public const string downloadAddressTxt = "data.txt";
        /// <summary>
        /// 配置文件根节点名字
        /// </summary>
        public const string XMLRootNodeName = "ConfigData";
    }

    public class FilePathSet
    {
        public FilePathSet()
        {
            CreateFolder();
        }
        private string rootPath = null;

        /// <summary>
        /// X:\\ZZFZ\\
        /// </summary>
        public string RootPath
        {
            get
            {
                if (rootPath == null)
                {
#if DEBUG_YES
                    int pIn = 0;
                    string sP = ConstValue.RootFolder + pIn.ToString();
                    rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), sP);
                    while (Directory.Exists(rootPath))
                    {
                        pIn++;
                        sP = ConstValue.RootFolder + pIn.ToString();
                        rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), sP);
                    }
                    rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), sP);
#else
                    if (Directory.Exists("D:\\"))
                    {
                        rootPath = Path.Combine("D:\\", ConstValue.RootFolder);
                    }
                    else
                    {
                        rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), ConstValue.RootFolder);
                    }
#endif
                }
                return rootPath;
            }
        }
        private string programPath = "";
        /// <summary>
        /// 程序安装根路径
        /// X:\\ZZFZ\\Program\\
        /// </summary>
        public string ProgramPath
        {
            get
            {
                if (programPath == "")
                {
                    programPath = Path.Combine(RootPath, ConstValue.Program);
                }
                return programPath;
            }
        }
        /// <summary>
        /// 要运行的程序路径
        /// X:\\ZZFZ\\Program\\TestXXX\\Base\\client.exe
        /// </summary>
        public string ProgramFullPath
        {
            get
            {
                if (GlobalData.localVersionXML.x_ProcessInfo.name == null)
                {
                    return Path.Combine(ProgramPath, "12.12");
                }
                return Path.Combine(ProgramPath, GlobalData.localVersionXML.x_ProcessInfo.path);
            }
        }
        private string updatePath = "";
        private string configDataPath = "";
        /// <summary>
        /// 配置文件存放的文件夹
        /// X:\\ZZFZ\\Data\\
        /// </summary>
        public string ConfigDataPath
        {
            get
            {
                if (configDataPath == "")
                {
                    configDataPath = Path.Combine(RootPath,ConstValue.DataFolder);
                }
                return configDataPath;
            }
        }
        /// <summary>
        /// 配置文件的存放路径(带后缀名)
        ///  X:\\ZZFZ\\Data\\Version-C.config
        /// </summary>
        public string ConfigDataFullPath
        {
            get
            {
                return Path.Combine(ConfigDataPath, ConstValue.ConfigDataFullName);
            }
        }
        /// <summary>
        /// X:\\ZZFZ\\Update\\Version-C.config
        /// </summary>
        public string ConfigDataFullPath_Tmp
        {
            get
            {
                return Path.Combine(UpdatePath, ConstValue.ConfigDataFullName);
            }
        }
        /// <summary>
        /// X:\\ZZFZ\\Update\\
        /// </summary>
        public string UpdatePath
        {
            get
            {
                if (updatePath == "")
                {
                    updatePath = Path.Combine(RootPath,ConstValue.UpdateFolder);
                }
                return updatePath;
            }
        }

        public string DataFullPath
        {
            get
            {
                return Path.Combine(ConfigDataPath, ConstValue.downloadAddressTxt);
            }
        }
        void CreateFolder()
        {
            CreateFolder(RootPath);
            CreateFolder(UpdatePath);
            CreateFolder(ConfigDataPath);
            CreateFolder(ProgramPath);
        }

        void CreateFolder(string name)
        {
            if (!Directory.Exists(name))
            {
                Directory.CreateDirectory(name);
            }
        }
    }
}
