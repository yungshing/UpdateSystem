using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Launch
{
    static class Program
    {
        public static bool isAlpha = false;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args .Length <= 0 || args[0] != "-callUpdate")
            {
                return;
            }
            if (args.Length >= 2)
            {
                if (args[1] == "-alpha")
                {
                    isAlpha = true;
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
