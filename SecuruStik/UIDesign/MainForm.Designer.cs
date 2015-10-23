namespace SecuruStik.UIDesign
{
    partial class SecuruStik_MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param pathFullName="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SecuruStik_MainForm));
            this.fileSystemWatcher_LocalSecFolder = new System.IO.FileSystemWatcher();
            this.fileSystemWatcher_DropboxShareFolder = new System.IO.FileSystemWatcher();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.buttonX_Exit = new DevComponents.DotNetBar.ButtonX();
            this.Notify_Timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher_LocalSecFolder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher_DropboxShareFolder)).BeginInit();
            this.SuspendLayout();
            // 
            // fileSystemWatcher_LocalSecFolder
            // 
            this.fileSystemWatcher_LocalSecFolder.EnableRaisingEvents = true;
            this.fileSystemWatcher_LocalSecFolder.IncludeSubdirectories = true;
            this.fileSystemWatcher_LocalSecFolder.NotifyFilter = ((System.IO.NotifyFilters)((((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.DirectoryName) 
            | System.IO.NotifyFilters.Size) 
            | System.IO.NotifyFilters.LastWrite)));
            this.fileSystemWatcher_LocalSecFolder.SynchronizingObject = this;
            this.fileSystemWatcher_LocalSecFolder.Created += new System.IO.FileSystemEventHandler(this.SecFolder_Created);
            this.fileSystemWatcher_LocalSecFolder.Deleted += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_LocalSecFolder_Deleted);
            // 
            // fileSystemWatcher_DropboxShareFolder
            // 
            this.fileSystemWatcher_DropboxShareFolder.EnableRaisingEvents = true;
            this.fileSystemWatcher_DropboxShareFolder.IncludeSubdirectories = true;
            this.fileSystemWatcher_DropboxShareFolder.NotifyFilter = ((System.IO.NotifyFilters)((((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.DirectoryName) 
            | System.IO.NotifyFilters.Size) 
            | System.IO.NotifyFilters.LastWrite)));
            this.fileSystemWatcher_DropboxShareFolder.SynchronizingObject = this;
            this.fileSystemWatcher_DropboxShareFolder.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_DropboxShareFolder_Created);
            this.fileSystemWatcher_DropboxShareFolder.Deleted += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_DropboxShareFolder_Deleted);
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipText = "SecuruStik is synchronizing your files";
            this.notifyIcon.BalloonTipTitle = "Synchronizing";
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Visible = true;
            this.notifyIcon.BalloonTipClicked += new System.EventHandler(this.NotifyIcon_BalloonTipClicked);
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseClick);
            this.notifyIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseMove);
            // 
            // buttonX_Exit
            // 
            this.buttonX_Exit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX_Exit.ColorTable = DevComponents.DotNetBar.eButtonColor.Orange;
            this.buttonX_Exit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonX_Exit.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonX_Exit.Location = new System.Drawing.Point(0, 0);
            this.buttonX_Exit.Name = "buttonX_Exit";
            this.buttonX_Exit.Padding = new System.Windows.Forms.Padding(1);
            this.buttonX_Exit.PulseSpeed = 9;
            this.buttonX_Exit.ShowSubItems = false;
            this.buttonX_Exit.Size = new System.Drawing.Size(66, 27);
            this.buttonX_Exit.Symbol = "";
            this.buttonX_Exit.SymbolSize = 12F;
            this.buttonX_Exit.TabIndex = 0;
            this.buttonX_Exit.TabStop = false;
            this.buttonX_Exit.Text = "  Exit";
            this.buttonX_Exit.TextAlignment = DevComponents.DotNetBar.eButtonTextAlignment.Left;
            this.buttonX_Exit.Click += new System.EventHandler(this.buttonX_Exit_Click);
            // 
            // Notify_Timer
            // 
            this.Notify_Timer.Interval = 1207;
            this.Notify_Timer.Tick += new System.EventHandler(this.Notify_Timer_Tick);
            // 
            // SecuruStik_MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(66, 27);
            this.ControlBox = false;
            this.Controls.Add(this.buttonX_Exit);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SecuruStik_MainForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "SecuruStik_UIDesign_MainForm";
            this.Load += new System.EventHandler(this.DBoxPRE_UI_Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher_LocalSecFolder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher_DropboxShareFolder)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.FileSystemWatcher fileSystemWatcher_LocalSecFolder;
        private System.IO.FileSystemWatcher fileSystemWatcher_DropboxShareFolder;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private DevComponents.DotNetBar.ButtonX buttonX_Exit;
        private System.Windows.Forms.Timer Notify_Timer;
    }
}

