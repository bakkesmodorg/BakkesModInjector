using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace BakkesModInjector
{
    public partial class Form1 : Form
    {
        private Object injectionLock = new Object();
        Boolean isInjected = false;
        Boolean startedAfterTrainer = false;
        bool injectNextTick = false;
        string updaterStorePath = Path.GetTempPath() + "\\bmupdate.zip";
        string downloadUpdateUrl;
        string rocketLeagueDirectory;
        string bakkesModDirectory;

        private String _injectionStatus = "";
        private System.Timers.Timer processCheckTimer;

        String InjectionStatus
        {
            get
            {
                return _injectionStatus;
            }
            set
            {
                _injectionStatus = value;
                statusLabel.Invoke((Action)(() => statusLabel.Text = _injectionStatus));
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        void injectDLL()
        {
            DllInjectionResult result = DllInjector.GetInstance.Inject("RocketLeague", bakkesModDirectory + "\\" + "bakkesmod.dll");
            switch (result)
            {
                case DllInjectionResult.DllNotFound:
                    InjectionStatus = StatusStrings.INSTALLATION_WRONG;
                    break;
                case DllInjectionResult.GameProcessNotFound:
                    InjectionStatus = StatusStrings.PROCESS_NOT_ACTIVE;
                    break;
                case DllInjectionResult.InjectionFailed:
                    InjectionStatus = StatusStrings.INJECTION_FAILED;
                    break;
                case DllInjectionResult.Success:
                    InjectionStatus = StatusStrings.INJECTED;
                    isInjected = true;
                    break;
            }
        }

        private void doInjections()
        {
            bool isRunning = Process.GetProcessesByName("RocketLeague").Length > 0;
            if (injectNextTick)
            {
            //Do injection
                injectDLL();
                injectNextTick = false;
                processCheckTimer.Interval = 2000;
            }
            else if (isRunning)
            {
                if (!isInjected)
                {
                    if(startedAfterTrainer)
                    {
                        processCheckTimer.Interval = 5000;
                        injectNextTick = true;
                        //Give 5 seconds for RL to start
                        InjectionStatus = StatusStrings.WAITING_FOR_LOAD;
                        startedAfterTrainer = false;
                    }
                    else
                    {
                        injectDLL();
                    }
                        
                        
                }
                else
                {
                    //Do nothing, already injected
                }
            }
            else
            {
                isInjected = false;
                startedAfterTrainer = true;
                InjectionStatus = StatusStrings.PROCESS_NOT_ACTIVE;
            }
            
        }

        private static readonly string BAKKESMOD_FILES_ZIP_DIR = "bminstall.zip";

        private static readonly string REGISTRY_CURRENTUSER_BASE_DIR    = @"Software\BakkesMod";
        private static readonly string REGISTRY_BASE_DIR                = @"HKEY_CURRENT_USER\SOFTWARE\BakkesMod\AppPath";
        private static readonly string REGISTRY_ROCKET_LEAGUE_PATH      = "RocketLeaguePath";
        private static readonly string REGISTRY_BAKKESMOD_PATH          = "BakkesModPath";

        void install()
        {
            string filePath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\rocketleague\\Binaries\\Win32\\RocketLeague.exe";

            if (!File.Exists(filePath))
            {
                MessageBox.Show("It seems that this is the first time you run BakkesMod. Please select the Rocket League executable. \r\nExecutable can be found in [STEAM_FOLDER]/steamapps/common/rocketleague/binaries/win32/");
                DialogResult result = openFileDialog1.ShowDialog();
                if (result != DialogResult.OK)
                {
                    MessageBox.Show("No executable selected. Exiting...");
                    Application.Exit();
                    return;
                }
                filePath = openFileDialog1.FileName;
                if (!filePath.ToLower().EndsWith("rocketleague.exe"))
                {
                    MessageBox.Show("This is not the Rocket League executable!");
                    Application.Exit();
                    return;
                }
            }
            rocketLeagueDirectory = filePath.Substring(0, filePath.LastIndexOf("\\") + 1);
            bakkesModDirectory = rocketLeagueDirectory + "bakkesmod\\";
            if (Directory.Exists(bakkesModDirectory))
            {
                Directory.Delete(bakkesModDirectory, true);
            }
            Directory.CreateDirectory(bakkesModDirectory);
            
            Registry.SetValue(REGISTRY_BASE_DIR, REGISTRY_ROCKET_LEAGUE_PATH, rocketLeagueDirectory);
            Registry.SetValue(REGISTRY_BASE_DIR, REGISTRY_BAKKESMOD_PATH, bakkesModDirectory);

            if (!File.Exists(BAKKESMOD_FILES_ZIP_DIR))
            {
                MessageBox.Show("Unable to find bminstall.zip. Either include this file or press \"yes\" on the next screen.");
                return;
            }

            using (FileStream zipToOpen = new FileStream(BAKKESMOD_FILES_ZIP_DIR, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    archive.ExtractToDirectory(bakkesModDirectory, true);
                }
            }
        }

        void checkForInstall()
        {
            string InstallPath = (string)Registry.GetValue(REGISTRY_BASE_DIR, REGISTRY_ROCKET_LEAGUE_PATH, null);
            if (InstallPath == null)
            {
                install();
            }
            else
            {
                rocketLeagueDirectory = InstallPath;
                string bakkesModDir = (string)Registry.GetValue(REGISTRY_BASE_DIR, REGISTRY_BAKKESMOD_PATH, null);
                if(!Directory.Exists(bakkesModDir))
                {
                    RegistryKey keys = Registry.CurrentUser.OpenSubKey(REGISTRY_CURRENTUSER_BASE_DIR, true);
                    keys.DeleteSubKeyTree("apppath");
                    install();
                } else
                {
                    bakkesModDirectory = bakkesModDir; //Is already installed
                }
            }
        }

        

        void checkForUpdates()
        {
            InjectionStatus = StatusStrings.CHECKING_FOR_UPDATES;
            
            string versionFile = bakkesModDirectory + "\\" + "version.txt";
            string versionText = "0";
            if (File.Exists(versionFile))
            {
                var version = File.ReadAllLines(bakkesModDirectory + "\\" + "version.txt").Select(txt => new { Version = txt }).First();
                versionText = version.Version;
            }
            Updater u = new Updater(versionText);
            UpdateResult res = u.CheckForUpdates();
            if(res == UpdateResult.ServerOffline)
            {
                MessageBox.Show("Could not connect to update server.");
            } else if(res == UpdateResult.UpToDate)
            {
                //Do nothing
            }
            else if(res == UpdateResult.UpdateAvailable)
            {
                DialogResult dialogResult = MessageBox.Show("An update is available. \r\nMessage: " + u.GetUpdateMessage() + "\r\nWould you like to update?", "Update", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    if(File.Exists(updaterStorePath))
                    {
                        File.Delete(updaterStorePath);
                    }
                    downloadUpdateUrl = u.GetUpdateURL();
                    InjectionStatus = StatusStrings.DOWNLOADING_UPDATE;
                    updater.RunWorkerAsync();
                    return;
                }
                else if (dialogResult == DialogResult.No)
                {
                    MessageBox.Show("Alright. The tool might be broken now though. Updating is recommended!");
                }
            }
            processCheckTimer.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            processCheckTimer = new System.Timers.Timer(2000);
            processCheckTimer.Elapsed += new ElapsedEventHandler(timer_Tick);
            checkForInstall();
            checkForUpdates();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            doInjections();
        }

        private void updater_DoWork(object sender, DoWorkEventArgs e)
        {
            downloadProgressBar.Invoke((Action)(() => downloadProgressBar.Visible = true));
            try {
                WebClient client = new WebClient();
                client.Proxy = null;
                Uri uri = new Uri(downloadUpdateUrl + "?rand=" + new Random().Next());

                // Specify that the DownloadFileCallback method gets called
                // when the download completes.
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileComplete);
                // Specify a progress notification handler.
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
                client.DownloadFileAsync(uri, updaterStorePath);


            } catch(Exception ex)
            {
                MessageBox.Show("There was an error downloading the update. " + ex.Message);
            }
        }

        private void DownloadFileComplete(object sender, AsyncCompletedEventArgs e)
        {
            downloadProgressBar.Invoke((Action)(() => downloadProgressBar.Visible = false));
            InjectionStatus = StatusStrings.EXTRACTING_UPDATE;

            using (FileStream zipToOpen = new FileStream(updaterStorePath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    archive.ExtractToDirectory(bakkesModDirectory, true);
                }
            }
            //if(ZipArchiveExtensions.BakkesModUpdated)
            //{
            //    MessageBox.Show("This tool has been updated, please run this tool again.\r\nExiting...");
            //    System.IO.FileInfo file = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);

            //    System.IO.File.Move(bakkesModDirectory + "\\" + "bakkesmod.exe", file.DirectoryName + "\\" + file.Name.Replace(file.Extension, "") + "-1" + file.Extension);

            //    Application.Exit();
            //    return;
            //}
            processCheckTimer.Start();
        }

        private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadProgressBar.Invoke((Action)(() => downloadProgressBar.Value = e.ProgressPercentage));
        }
    }



    static class StatusStrings
    {
        public static readonly string PROCESS_NOT_ACTIVE = "Uninjected (Rocket League is not running)";
        public static readonly string INSTALLATION_WRONG = "Uninjected (Could not find BakkesMod folder?)";
        public static readonly string WAITING_FOR_LOAD = "Waiting until Rocket League finished loading.";
        public static readonly string INJECTED = "Injected";
        public static readonly string INJECTION_FAILED = "Injection failed, not enough rights to inject or DLL is wrong?";
        public static readonly string CHECKING_FOR_UPDATES = "Checking for updates...";
        public static readonly string DOWNLOADING_UPDATE = "Downloading update...";
        public static readonly string EXTRACTING_UPDATE = "Extracting update...";
    }
}
