using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BakkesModInjector
{
    class InjectorUpdater
    {
        static string injectorUpdaterFileName = Path.GetTempPath() + "\\injector.zip";
        static string tempExtractDirectory = Path.GetTempPath() + "\\injector\\";
        static string injectorUpdaterURL = "";
        public static void UpdateInjector(Updater u)
        {
            if (File.Exists(injectorUpdaterFileName))
            {
                File.Delete(injectorUpdaterFileName);
            }
            injectorUpdaterURL = u.GetInjectorUpdateURL();

            var t = Task.Run(() => DoActualUpdate());
        }

        static void DoActualUpdate()
        {
            WebClient client = new WebClient();
            client.Proxy = null;
            Uri uri = new Uri(injectorUpdaterURL);

            // Specify that the DownloadFileCallback method gets called
            // when the download completes.
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadUpdaterFileComplete);
            // Specify a progress notification handler.

            client.DownloadFileAsync(uri, injectorUpdaterFileName);

        }

        private static void DownloadUpdaterFileComplete(object sender, AsyncCompletedEventArgs e)
        {
            if(Directory.Exists(tempExtractDirectory))
            {
                DeleteDirectory(tempExtractDirectory);
            }
            Directory.CreateDirectory(tempExtractDirectory);
            using (FileStream zipToOpen = new FileStream(injectorUpdaterFileName, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    archive.ExtractToDirectory(tempExtractDirectory, true);
                }
            }
            string injectorName = Directory.GetFiles(tempExtractDirectory, "*.exe")[0];
            MessageBox.Show("The application will restart itself within 2 seconds");
            string strCmdText = "@echo off\n" +
                                "timeout 2 > NUL\n" +
                                "copy " + injectorName + " " + System.Reflection.Assembly.GetExecutingAssembly() + "\n"+
                                "start " + injectorName + "\n";
            File.WriteAllText(tempExtractDirectory + "update.bat", strCmdText);
            System.Diagnostics.Process.Start(tempExtractDirectory + "update.bat");
            Application.Exit();
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
    }
}
