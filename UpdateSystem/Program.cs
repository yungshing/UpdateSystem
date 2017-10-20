using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UpdateSystem
{
    static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hWnd, bool bInvert);
        [DllImport("user32.dll")]
        private static extern bool FlashWindowEx(int pfwi);
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
               switch(args[0])
                {
                    case "-debug": GlobalData.isDebug = true;break;
                }
            }
            if (!GlobalData.isDebug && HadInstance())
            {
                MessageBox.Show("程序正在运行");
                Application.Exit();
                return;
            }
            //GlobalData.isDebug = true;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            System.Net.ServicePointManager.DefaultConnectionLimit = 1000;
            GlobalData.debugLogName = @"D:\Log_" + DateTime.Now.ToString("MM-dd--HH-mm-ss").Replace(" ", "") + ".txt";
            GlobalData.mainForm = new UpdateSystem.NewMainUI();
            Application.Run(GlobalData.mainForm);
        }

        static bool HadInstance()
        {
            if (GlobalData.isDebug)
            {
                return false;
            }
            int sum = 0;
            foreach (var item in Process.GetProcesses())
            {
                if (item.ProcessName.Contains("UpdateSystem"))
                    sum ++;
            }
            return sum >= 2;
        }
    }
}
