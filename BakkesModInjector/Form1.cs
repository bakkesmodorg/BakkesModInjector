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
using System.Runtime.InteropServices;

namespace BakkesModInjector
{
    public partial class Form1 : Form
    {
        public static readonly int UPDATER_VERSION = 3;

        private bool isUpdatingInjector = false;
        private bool isFirstRun = false;
        private Object injectionLock = new Object();
        Boolean isInjected = false;
        Boolean startedAfterTrainer = false;
        bool injectNextTick = false;
        string updaterStorePath = Path.GetTempPath() + "\\bmupdate.zip";

        string safeVersion = "";
        string downloadUpdateUrl;
        string rocketLeagueDirectory;
        string bakkesModDirectory;

        private String _injectionStatus = "";
        private System.Timers.Timer processCheckTimer;
        private System.Timers.Timer updateCheckTimer;

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
            DLLInjectionResult result = DllInjector.Instance.Inject("RocketLeague", bakkesModDirectory + "" + "bakkesmod.dll");
            switch (result)
            {
                case DLLInjectionResult.DLL_NOT_FOUND:
                    InjectionStatus = StatusStrings.INSTALLATION_WRONG;
                    break;
                case DLLInjectionResult.GAME_PROCESS_NOT_FOUND:
                    InjectionStatus = StatusStrings.PROCESS_NOT_ACTIVE;
                    break;
                case DLLInjectionResult.INJECTION_FAILED:
                    InjectionStatus = StatusStrings.INJECTION_FAILED;
                    break;
                case DLLInjectionResult.SUCCESS:
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
                Process rocketLeagueProcess = Process.GetProcessesByName("RocketLeague").First();
                if (!rocketLeagueProcess.Responding)
                {
                    processCheckTimer.Interval = 9000;
                    return;
                }
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
                        processCheckTimer.Interval = 8000;
                        injectNextTick = true;
                        //Give 5 seconds for RL to start
                        InjectionStatus = StatusStrings.WAITING_FOR_LOAD;
                        //startedAfterTrainer = false;

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
        private static readonly string APPLICATION_NAME                 = "BakkesMod";
        private static readonly string REGISTRY_RUN                     = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private static readonly string REGISTRY_ROCKET_LEAGUE_PATH      = "RocketLeaguePath";
        private static readonly string REGISTRY_BAKKESMOD_PATH          = "BakkesModPath";

        void install()
        {
            string filePath = RLLauncher.GetRocketLeagueDirFromLog() + "RocketLeague.exe";

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
            if (!Directory.Exists(bakkesModDirectory))
            {
                //Directory.Delete(bakkesModDirectory, true);
                Directory.CreateDirectory(bakkesModDirectory);
            }
            
            
            Registry.SetValue(REGISTRY_BASE_DIR, REGISTRY_ROCKET_LEAGUE_PATH, rocketLeagueDirectory);
            Registry.SetValue(REGISTRY_BASE_DIR, REGISTRY_BAKKESMOD_PATH, bakkesModDirectory);

            if (!File.Exists(BAKKESMOD_FILES_ZIP_DIR))
            {
                isFirstRun = true;
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
            if(isUpdatingInjector)
                return;
            if (IsSafeToInject())
            {
                InjectionStatus = StatusStrings.CHECKING_FOR_UPDATES;
            }
            else
            {
                InjectionStatus = "Mod out of date, waiting for update...\nDisable safe mode you still want to try injection";
            }
            
            string versionFile = bakkesModDirectory + "\\" + "version.txt";
            string versionText = "0";
            if (File.Exists(versionFile))
            {
                var version = File.ReadAllLines(bakkesModDirectory + "\\" + "version.txt").Select(txt => new { Version = txt }).First();
                versionText = version.Version;
            } else
            {
                isFirstRun = true;
            }
            Updater u = new Updater(versionText);
            UpdateResult res = u.CheckForUpdates();
            string newSafe = u.GetSafeVersion();

            if(u.GetUpdaterVersion() > UPDATER_VERSION)
            {
                DialogResult dialogResult = MessageBox.Show("An update for the injector is available. \r\nWould you like to update?", "Update", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    InjectionStatus = "Updating the injector... Please wait";
                    InjectorUpdater.UpdateInjector(u);
                    isUpdatingInjector = true;
                    updateCheckTimer.Stop();
                    return;
                }
            }

            if (u.IsBlocked())
            {
                MessageBox.Show("Access denied, contact the developer");
                Application.Exit();
            }
            if (res == UpdateResult.ServerOffline)
            {
                MessageBox.Show("Could not connect to update server.");
            }
            else if (res == UpdateResult.UpToDate)
            {
                //Do nothing
                updateCheckTimer.Stop();
            } 
            else if (res == UpdateResult.UpdateAvailable)
            {
                if (isFirstRun)
                {
                    if (File.Exists(updaterStorePath))
                    {
                        File.Delete(updaterStorePath);
                    }
                    downloadUpdateUrl = u.GetUpdateURL();
                    InjectionStatus = StatusStrings.DOWNLOADING_UPDATE;
                    updater.RunWorkerAsync();
                    updateCheckTimer.Stop();
                    return;
                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show("An update is available. \r\nMessage: " + u.GetUpdateMessage() + "\r\nWould you like to update?", "Update", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (File.Exists(updaterStorePath))
                        {
                            File.Delete(updaterStorePath);
                        }
                        downloadUpdateUrl = u.GetUpdateURL();
                        InjectionStatus = StatusStrings.DOWNLOADING_UPDATE;
                        updater.RunWorkerAsync();
                        updateCheckTimer.Stop();
                        return;
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        updateCheckTimer.Stop();
                        MessageBox.Show("Alright. The tool might be broken now though. Updating is recommended!");
                    }
                    
                }
            }
            if (!newSafe.Equals(safeVersion))
            {
                safeVersion = newSafe;
            }
            if (IsSafeToInject() || res == UpdateResult.UpdateAvailable || !newSafe.Equals(safeVersion))
            {
                if (!processCheckTimer.Enabled)
                {
                    processCheckTimer.Start();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            processCheckTimer = new System.Timers.Timer(2000);
            processCheckTimer.Elapsed += new ElapsedEventHandler(timer_Tick);

            updateCheckTimer = new System.Timers.Timer(10000);
            updateCheckTimer.Elapsed += new ElapsedEventHandler(updater_Tick);
            checkForInstall();
        }

        private void updater_Tick(object sender, EventArgs e)
        {
            checkForUpdates();

            updateCheckTimer.Interval = Math.Min(updateCheckTimer.Interval * 2, 120000);
        }

        private bool IsSafeToInject()
        {
            string version = RLLauncher.GetRocketLeagueSteamVersion(rocketLeagueDirectory + "/../../../../");
            return version.Equals(safeVersion) || !enableSafeModeToolStripMenuItem.Checked;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            
            if (!IsSafeToInject())
            {
                    processCheckTimer.Stop();
                    updateCheckTimer.Interval = 10000;
                    updateCheckTimer.Start();
                    MessageBox.Show("Rocket League version and BakkesMod version don't match up. Please disable safe mode if you still want to inject or wait for an update.");
                    InjectionStatus = "Mod out of date, waiting for update...\nDisable safe mode y" +
                    "ou still want to try injection";
                    return;
            }
            //
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
            if (isFirstRun) {
                string readme = bakkesModDirectory + "\\readme.txt";
                if (File.Exists(readme))
                {
                    DialogResult dialogResult = MessageBox.Show("It looks like this is your first time, would you like to open the readme?", "BakkesMod", MessageBoxButtons.YesNo);
                    if(dialogResult == DialogResult.Yes)
                    {
                        Process.Start(readme);
                    }
                } 
           }
            processCheckTimer.Start();
        }

        private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadProgressBar.Invoke((Action)(() => downloadProgressBar.Value = e.ProgressPercentage));
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            checkForUpdates();
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(REGISTRY_RUN, true);
            
            SetRunOnStartup(isFirstRun || !rk.GetValue(APPLICATION_NAME, "Ex").Equals("Ex")); //|| !rk.GetValue(APPLICATION_NAME, Application.ExecutablePath.ToString()).Equals(Application.ExecutablePath.ToString())

            RegistryKey keys = Registry.CurrentUser.OpenSubKey(REGISTRY_CURRENTUSER_BASE_DIR, true);
            int? val = (int?)keys.GetValue("HideOnMinimize");
            SetHideWhenMinimized(val != 0x00);

            int? val2 = (int?)keys.GetValue("EnableSafeMode");
            SetEnableSafeMode(val2 == 0x01);

            SetNoGUI(File.Exists(bakkesModDirectory + "\\nogui.txt"));
        }

        void SetRunOnStartup(bool runOnStartup)
        {
            runOnStartupToolStripMenuItem.Checked = runOnStartup;

            RegistryKey rk = Registry.CurrentUser.OpenSubKey(REGISTRY_RUN, true);

            if (runOnStartup)
                rk.SetValue(APPLICATION_NAME, Application.ExecutablePath.ToString());
            else
                rk.DeleteValue(APPLICATION_NAME, false);
        }

        void SetHideWhenMinimized(bool hideWhenMinimized)
        {
            hideWhenMinimizedToolStripMenuItem.Checked = hideWhenMinimized;
            RegistryKey keys = Registry.CurrentUser.OpenSubKey(REGISTRY_CURRENTUSER_BASE_DIR, true);
            keys.SetValue("HideOnMinimize", hideWhenMinimized, RegistryValueKind.DWord);
        }

        void SetEnableSafeMode(bool enableSafeMode)
        {
            enableSafeModeToolStripMenuItem.Checked = enableSafeMode;
            RegistryKey keys = Registry.CurrentUser.OpenSubKey(REGISTRY_CURRENTUSER_BASE_DIR, true);
            keys.SetValue("EnableSafeMode", enableSafeMode, RegistryValueKind.DWord);
            if(enableSafeMode)
            {
                updateCheckTimer.Start();
                
            }
            else if(updateCheckTimer.Enabled)
            {
                updateCheckTimer.Stop();
                processCheckTimer.Start();
            }
        }

        private void runOnStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            SetRunOnStartup(!item.Checked);

        }

        private void hideWhenMinimizedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            SetHideWhenMinimized(!item.Checked);

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (hideWhenMinimizedToolStripMenuItem.Checked)
            {
                if (FormWindowState.Minimized == this.WindowState)
                {
                    notifyIcon1.Visible = true;
                    notifyIcon1.BalloonTipText = "BakkesMod will stay active when minimized";
                    notifyIcon1.ShowBalloonTip(500);
                    notifyIcon1.ContextMenuStrip = menuStrip1.ContextMenuStrip;
                    this.Hide();
                }

                else if (FormWindowState.Normal == this.WindowState)
                {
                    notifyIcon1.Visible = false;
                }
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }



        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void reinstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("This will fully remove all bakkesmod files, are you sure you want to continue?", "Confirm reinstall", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                string bakkesModDirectory = rocketLeagueDirectory + "bakkesmod\\";
                if (Directory.Exists(bakkesModDirectory))
                {
                    Directory.Delete(bakkesModDirectory, true);
                }
                checkForInstall();
                checkForUpdates();
                isFirstRun = false;
            }
        }

        private void openBakkesModFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string bakkesModDirectory = rocketLeagueDirectory + "bakkesmod\\";
            if (Directory.Exists(bakkesModDirectory))
            {
                Process.Start(bakkesModDirectory);
            } else
            {
                MessageBox.Show("BakkesMod folder does not exist. (Did you delete it?)");
            }
        }

        private void bakkesModWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://bakkesmod.com");
        }

        private void bakkesModWorkshopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://workshop.bakkesmod.com/maps/playlists/");
        }


        static string PYTHON_DOWNLOAD = Path.GetTempPath() + "\\python_bm.zip";
        private void installPythonSupportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string libs_dir = rocketLeagueDirectory + "bakkesmod\\libs\\";
            if(File.Exists(libs_dir + "python36.dll") && File.Exists(libs_dir + "python36.zip"))
            {
                DialogResult continueDownload = MessageBox.Show("It looks like python for BakkesMod is already installed, would you like to reinstall?", "Continue python download", MessageBoxButtons.YesNo);
                if (continueDownload != DialogResult.Yes)
                    return;
            }
            string download_url = "https://www.python.org/ftp/python/3.6.3/python-3.6.3-embed-win32.zip";
            downloadProgressBar.Invoke((Action)(() => downloadProgressBar.Visible = true));
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += python_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri(download_url), Path.GetTempPath() + "\\python_bm.zip");
                wc.DownloadFileCompleted += python_Downloaded;
            }
        }

        private void python_Downloaded(object sender, AsyncCompletedEventArgs e)
        {
            downloadProgressBar.Invoke((Action)(() => downloadProgressBar.Visible = false));
            string libs_dir = rocketLeagueDirectory + "bakkesmod\\libs\\";
            using (FileStream zipToOpen = new FileStream(PYTHON_DOWNLOAD, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    archive.ExtractToDirectory(libs_dir, true);
                }
            }
        }

        private void python_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            
            downloadProgressBar.Invoke((Action)(() => downloadProgressBar.Value = e.ProgressPercentage));
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not yet implemented");
        }

        private void enableSafeModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            SetEnableSafeMode(!item.Checked);
            processCheckTimer.Start();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F1)
            {
                bakkesModDirectory = @"F:\Bakkesmod\development\BakkesMod-rewrite\Release\";
                MessageBox.Show("BakkesModDirectory set to release");
            } else
            {
                if (e.KeyCode == Keys.F2)
                {
                    bakkesModDirectory = @"F:\Bakkesmod\development\BakkesMod-rewrite\Debug\";
                    MessageBox.Show("BakkesModDirectory set to debug");
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
           
        }


        void SetNoGUI(bool noGui)
        {
            if(!noGui)
            {
                if(File.Exists(bakkesModDirectory + "\\nogui.txt"))
                {
                    File.Delete(bakkesModDirectory + "\\nogui.txt");
                }
            }
            else
            {
                if(!File.Exists(bakkesModDirectory + "\\nogui.txt"))
                {
                    File.Create(bakkesModDirectory + "\\nogui.txt");
                }
            }
            noGUIToolStripMenuItem.Checked = noGui;
        }
        private void noGUIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            SetNoGUI(!item.Checked);
        }

        private void checkInjectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isRunning = false;
            bool isInjected = false;
            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                
                if (p.ProcessName == "RocketLeague")
                {
                    isRunning = true;
                    foreach (ProcessModule module in p.Modules)
                    {
                        if(module.ModuleName.Equals("bakkesmod.dll"))
                        {
                            isInjected = true;

                        }
                    }
                    break;   
                }
            }
            if(!isRunning)
            {
                MessageBox.Show("Rocket league is not running, cannot check status");
                return;
            }
            if(!isInjected)
            {
                MessageBox.Show("Could not find the bakkesmod.dll in the Rocket League process.");
                return;
            }
            MessageBox.Show("Injection works!");
           
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
