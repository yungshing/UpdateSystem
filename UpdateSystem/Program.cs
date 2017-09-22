
#define DEBUG_NO_

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
#if DEBUG_NO
            bool runOne;
            System.Threading.Mutex run = new System.Threading.Mutex(true, Application.ProductName, out runOne);

            if (!runOne)
            {
                MessageBox.Show("程序正在运行");
                //IntPtr hdc = new IntPtr(1312810);
                //FlashWindow(hdc, true);
                Application.Exit();
                return;
            }
            run.ReleaseMutex();
#endif
            if (HadInstance())
            {
                MessageBox.Show("程序正在运行");
                Application.Exit();
                return;
            }
            var v = Environment.Version;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args .Length >0)
            {
                if (args [0] == "-debug")
                {
                    GlobalData.isDebug = true;
                }
            }
            System.Net.ServicePointManager.DefaultConnectionLimit = 1000;
            GlobalData.debugLogName = @"D:\Log_" + DateTime.Now.ToString("MM-dd--HH-mm-ss").Replace(" ", "") + ".txt";
            Application.Run(new NewMainUI ());
        }

        static bool HadInstance()
        {
            int sum = 0;
            foreach (var item in Process.GetProcesses())
            {
                if (item.ProcessName.Contains("UpdateSystem"))
                    sum ++;
            }
            return sum >= 2;
            //return false;
        }
    }
}
