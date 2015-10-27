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
        private static string _dropboxFolderLocation = null;
        public static String DropBoxFolderLocation
        {
            get
            {
                if (String.IsNullOrEmpty(_dropboxFolderLocation))
                {
                    String dbPath = System.IO.Path.Combine(DropBoxService.InstallLocation, "host.db");
                    if (File.Exists(dbPath) == false)
                    {
                        dbPath = System.IO.Path.Combine(DropBoxService.InstallLocation, "host.dbx");
                    }
                    String[] lines = System.IO.File.ReadAllLines(dbPath);
                    byte[] dbBase64Text = Convert.FromBase64String(lines[1]);

                    _dropboxFolderLocation = System.Text.ASCIIEncoding.ASCII.GetString(dbBase64Text);
                    log.Info("Found Dropbox at " + _dropboxFolderLocation);
                }

                return _dropboxFolderLocation;
            }
        }

        /// <summary>Install path</summary>
        private static String _installLocation = String.Empty;
        public static String InstallLocation
        {
            get
            {
                if ( DropBoxService._installLocation == String.Empty )
                {
                    String appDataPath = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
                    DropBoxService._installLocation = System.IO.Path.Combine( appDataPath , "Dropbox" );
                    String dbPath = System.IO.Path.Combine( DropBoxService._installLocation , "host.db" );
                    String dbxPath = System.IO.Path.Combine( DropBoxService._installLocation , "host.dbx" );
                    if ( File.Exists( dbPath ) == false && File.Exists(dbxPath) == false)
                    {
                        appDataPath = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );
                        DropBoxService._installLocation = System.IO.Path.Combine( appDataPath , "Dropbox" );
                    }

                    log.Info("Found Dropbox Install Location " + _installLocation);
                }
                return _installLocation;
            }
        }

        /// <summary>Application path</summary>
        private static String _appLocation = string.Empty;
        public static String AppLocation
        {
            get
            {
                if ( DropBoxService._appLocation == String.Empty )
                {
                    //Old version app path
                    DropBoxService._appLocation = System.IO.Path.Combine( DropBoxService.InstallLocation , "bin\\Dropbox.exe" );

                    //New version app path
                    if ( !File.Exists(DropBoxService._appLocation) )
                        DropBoxService._appLocation = System.IO.Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),"Dropbox/Client/Dropbox.exe");
                }

                log.Info("Found Dropbox App Location " + _appLocation);
                return DropBoxService._appLocation;
            }
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
