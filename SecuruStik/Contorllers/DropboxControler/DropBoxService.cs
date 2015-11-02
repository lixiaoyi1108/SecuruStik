/***********************************************************************
 * CLR Version :      4.0.30319.18444
 * Class Name  :      DropBoxService
 * Name Space  :      DBoxPRE.DropBox
 * File Name   :      DropBoxService
 * Create Time :      2014/11/4 15:02:35
 * Author      :      JianYe,Huang
 * Brief       :      Control the local dropbox application
 * Modification history : NULL
 ***********************************************************************/
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using System.IO;

namespace SecuruStik.DropBox
{
    public class DropBoxService
    {
        #region 0. Fields

        static log4net.ILog log = Tracer.GetClassLogger();

        #region 0.1 Path of dropbox application

        /// <summary> DropBox folder path</summary>
        private static string _dropboxFolderLocation;
        public static String DropBoxFolderLocation
        {
            get
            { return _dropboxFolderLocation; }
        }

        /// <summary>Install path of the program which on Windows is AppData</summary>
        private static String _installLocation;
        public static String InstallLocation
        {
            get { return _installLocation; }
        }

        /// <summary>Application path</summary>
        private static String _appLocation;
        public static String AppLocation
        {
            get
            { return DropBoxService._appLocation; }
        }

        private static string _dropboxConfigLocation;
        private static bool FindConfigLocation(Environment.SpecialFolder starting, string tryName, out string pConfigLocation, out string pDbPath)
        {
            pConfigLocation = null;
            pDbPath = null;

            string full = Path.Combine(Environment.GetFolderPath(starting), tryName);
            log.DebugFormat("DB config path trying {0}", full);
            if (!Directory.Exists(full)) return false;

            pConfigLocation = full;
            string dbPath = Path.Combine(full, "host.db");
            string dbxPath = Path.Combine(full, "host.dbx");
            log.DebugFormat("DB config path trying to find these files\n{0}\n{1}", dbPath, dbxPath);
            if (File.Exists(dbPath))
            {
                pDbPath = dbPath;
                return true;
            }
            if (File.Exists(dbxPath))
            {
                pDbPath = dbxPath;
                return true;
            }
            return false;
        }

        static DropBoxService() {
            string dbpath = null;
            if (Platform.Folders == Platform.Name.Windows)
            {
                if (!FindConfigLocation(Environment.SpecialFolder.ApplicationData, "Dropbox", out _dropboxConfigLocation, out dbpath))
                    if (!FindConfigLocation(Environment.SpecialFolder.LocalApplicationData, "Dropbox", out _dropboxConfigLocation, out dbpath))
                        throw new DropboxInstallError("config_location_not_found");

                _installLocation = _dropboxConfigLocation;
            }
            else
            {
                if (!FindConfigLocation(Environment.SpecialFolder.UserProfile, ".dropbox", out _dropboxConfigLocation, out dbpath))
                    throw new DropboxInstallError("config_location_not_found");

                if (Platform.Folders == Platform.Name.Mac) _installLocation = "/Applications/Dropbox.app";
                else throw new DropboxInstallError("dont know where dropbox is installed on Linux");
            }

            log.Info("Found Dropbox config Location " + _dropboxConfigLocation);

            // the user data folder
            string[] lines = File.ReadAllLines(dbpath);
            byte[] dbBase64Text = Convert.FromBase64String(lines[1]);

            _dropboxFolderLocation = System.Text.ASCIIEncoding.ASCII.GetString(dbBase64Text);
            log.Info("Found Dropbox at " + _dropboxFolderLocation);

            // application location
            if (Platform.Folders == Platform.Name.Windows)
            {
                //Old version app path
                DropBoxService._appLocation = System.IO.Path.Combine(DropBoxService.InstallLocation, "bin", "Dropbox.exe");

                //New version app path
                if (!File.Exists(DropBoxService._appLocation))
                    DropBoxService._appLocation = System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Dropbox", "Client", "Dropbox.exe");
            } else
            {
                _appLocation = "/Applications/Dropbox.app";
            }
            log.Info("Found Dropbox App Location " + _appLocation);
        }



#endregion Path of dropbox application

        private Process AppProcess;

#endregion 0. Fields

        public DropBoxService()
        {
            this.Start();
        }

        public void Start()
        {
            if ( System.Diagnostics.Process.GetProcessesByName( "Dropbox" ).Length <= 0 )
            {
                try
                {
                    AppProcess = new Process();
                    AppProcess.StartInfo.WorkingDirectory = DropBoxService.InstallLocation;
                    AppProcess.StartInfo.FileName = DropBoxService.AppLocation;

                    AppProcess.StartInfo.UseShellExecute = false;
                    AppProcess.StartInfo.CreateNoWindow = true;
                    AppProcess.StartInfo.Arguments = "/home";
                    AppProcess.Start();
                }
                catch ( System.Exception ex )
                {
                    log.Error("DropBoxService trying to start dropbox (handled by user dialog)");

                    DialogResult result = MessageBoxEx.Show(
                        new Form { TopMost = true },
                       "Cannot find your Dropbox Folder!\r\nPlease check if it has been installed.\r\nWould you like to install Dropbox?" ,
                        "Error" , MessageBoxButtons.YesNo, MessageBoxIcon.Question );
                   if (result == DialogResult.Yes)
                   {
                       System.Diagnostics.Process.Start("iexplore.exe", "https://www.dropbox.com/downloading?src=index");
                   }

                   // TODO: WTF
                   Process.GetCurrentProcess().Kill();
                }
            }
        }

        public void Stop()
        {
            try
            {
                AppProcess.Close();
            }
            catch ( System.Exception ex ){
                log.Error("Stopping DropBoxService", ex);
            }
        }
    }
}
