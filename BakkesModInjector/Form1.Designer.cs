namespace BakkesModInjector
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.processChecker = new System.ComponentModel.BackgroundWorker();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.updater = new System.ComponentModel.BackgroundWorker();
            this.downloadProgressBar = new System.Windows.Forms.ProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reinstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openBakkesModFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.linksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bakkesModWebsiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bakkesModWorkshopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runOnStartupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideWhenMinimizedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.installPythonSupportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Status:";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(62, 39);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(207, 13);
            this.statusLabel.TabIndex = 1;
            this.statusLabel.Text = "Uninjected (Rocket League is not running)";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "RocketLeague.exe";
            this.openFileDialog1.Filter = "Rocket League|RocketLeague.exe";
            this.openFileDialog1.InitialDirectory = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\rocketleague\\Binaries\\Win32\\";
            this.openFileDialog1.ShowHelp = true;
            // 
            // updater
            // 
            this.updater.DoWork += new System.ComponentModel.DoWorkEventHandler(this.updater_DoWork);
            // 
            // downloadProgressBar
            // 
            this.downloadProgressBar.Location = new System.Drawing.Point(26, 56);
            this.downloadProgressBar.Name = "downloadProgressBar";
            this.downloadProgressBar.Size = new System.Drawing.Size(243, 13);
            this.downloadProgressBar.TabIndex = 2;
            this.downloadProgressBar.Visible = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.linksToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(296, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installPythonSupportToolStripMenuItem,
            this.reinstallToolStripMenuItem,
            this.openBakkesModFolderToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // reinstallToolStripMenuItem
            // 
            this.reinstallToolStripMenuItem.Name = "reinstallToolStripMenuItem";
            this.reinstallToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.reinstallToolStripMenuItem.Text = "Reinstall";
            this.reinstallToolStripMenuItem.Click += new System.EventHandler(this.reinstallToolStripMenuItem_Click);
            // 
            // openBakkesModFolderToolStripMenuItem
            // 
            this.openBakkesModFolderToolStripMenuItem.Name = "openBakkesModFolderToolStripMenuItem";
            this.openBakkesModFolderToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.openBakkesModFolderToolStripMenuItem.Text = "Open BakkesMod folder";
            this.openBakkesModFolderToolStripMenuItem.Click += new System.EventHandler(this.openBakkesModFolderToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click_1);
            // 
            // linksToolStripMenuItem
            // 
            this.linksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bakkesModWebsiteToolStripMenuItem,
            this.bakkesModWorkshopToolStripMenuItem});
            this.linksToolStripMenuItem.Name = "linksToolStripMenuItem";
            this.linksToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.linksToolStripMenuItem.Text = "Links";
            // 
            // bakkesModWebsiteToolStripMenuItem
            // 
            this.bakkesModWebsiteToolStripMenuItem.Name = "bakkesModWebsiteToolStripMenuItem";
            this.bakkesModWebsiteToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.bakkesModWebsiteToolStripMenuItem.Text = "BakkesMod website";
            this.bakkesModWebsiteToolStripMenuItem.Click += new System.EventHandler(this.bakkesModWebsiteToolStripMenuItem_Click);
            // 
            // bakkesModWorkshopToolStripMenuItem
            // 
            this.bakkesModWorkshopToolStripMenuItem.Name = "bakkesModWorkshopToolStripMenuItem";
            this.bakkesModWorkshopToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.bakkesModWorkshopToolStripMenuItem.Text = "BakkesMod workshop";
            this.bakkesModWorkshopToolStripMenuItem.Click += new System.EventHandler(this.bakkesModWorkshopToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runOnStartupToolStripMenuItem,
            this.hideWhenMinimizedToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // runOnStartupToolStripMenuItem
            // 
            this.runOnStartupToolStripMenuItem.Name = "runOnStartupToolStripMenuItem";
            this.runOnStartupToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.runOnStartupToolStripMenuItem.Text = "Run on startup";
            this.runOnStartupToolStripMenuItem.Click += new System.EventHandler(this.runOnStartupToolStripMenuItem_Click);
            // 
            // hideWhenMinimizedToolStripMenuItem
            // 
            this.hideWhenMinimizedToolStripMenuItem.Name = "hideWhenMinimizedToolStripMenuItem";
            this.hideWhenMinimizedToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.hideWhenMinimizedToolStripMenuItem.Text = "Hide when minimized";
            this.hideWhenMinimizedToolStripMenuItem.Click += new System.EventHandler(this.hideWhenMinimizedToolStripMenuItem_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.Click += new System.EventHandler(this.notifyIcon1_Click);
            // 
            // installPythonSupportToolStripMenuItem
            // 
            this.installPythonSupportToolStripMenuItem.Name = "installPythonSupportToolStripMenuItem";
            this.installPythonSupportToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.installPythonSupportToolStripMenuItem.Text = "Install python support";
            this.installPythonSupportToolStripMenuItem.Click += new System.EventHandler(this.installPythonSupportToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 77);
            this.Controls.Add(this.downloadProgressBar);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "BakkesMod";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label statusLabel;
        private System.ComponentModel.BackgroundWorker processChecker;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.ComponentModel.BackgroundWorker updater;
        private System.Windows.Forms.ProgressBar downloadProgressBar;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runOnStartupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideWhenMinimizedToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reinstallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openBakkesModFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem linksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bakkesModWebsiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bakkesModWorkshopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installPythonSupportToolStripMenuItem;
    }
}

