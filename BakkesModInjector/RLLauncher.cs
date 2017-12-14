using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BakkesModInjector
{
    class RLLauncher
    {

        public static string GetRocketLeagueDirFromLog()
        {
            //Init: Base directory: 
            string myDocuments = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string logDir = myDocuments + @"\My Games\Rocket League\TAGame\Logs\";
            string logFile = logDir + "launch.log";
            string returnDir = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\rocketleague\\Binaries\\Win32\\";
            if(File.Exists(logFile))
            {
                string line;
                using (FileStream stream = File.Open(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(stream);
                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.Contains("Base directory"))
                        {
                            returnDir = line.Split(' ').Last();
                            break;
                        }
                    }
                }
            }
            return returnDir;
        }

        void Launch()
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "F:\\SteamLibrary\\steamapps\\common\\rocketleague\\Binaries\\Win32\\RocketLeague.exe",
                    Arguments = "",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                MessageBox.Show(line);
                // do something with line
            }
        }
    }
}
