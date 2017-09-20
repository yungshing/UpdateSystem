using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Launch
{
   public class Follow
    {
        public bool isMoveOver = true;

        public void Move()
        {
            Thread t = new Thread(()=>
            {
                var path = Path.Combine(Environment.CurrentDirectory, "UpdateSystem.exe");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                var nPath = path + ".tmp";
                File.Move(nPath, path);
                path = Path.Combine(Environment.CurrentDirectory, "Launch.config");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                nPath = path + ".tmp";
                File.Move(nPath, path);
                isMoveOver = true;
            });
            t.Start();
        }

        public void Open()
        {
            Process p = new Process();
            var path = Path.Combine(Environment.CurrentDirectory, "UpdateSystem.exe");
            Process.Start(path);
        }
    }
}
