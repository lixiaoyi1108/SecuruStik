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

        #region 0.1 Path of dropbox application

        /// <summary> DropBox folder path</summary>
        public static String DropBoxFolderLocation
        {
            get
            {
                String dbPath = System.IO.Path.Combine( DropBoxService.InstallLocation , "host.db" );
                if (File.Exists(dbPath) == false)
                {
                    dbPath = System.IO.Path.Combine(DropBoxService.InstallLocation, "host.dbx");
                }
                String[] lines = System.IO.File.ReadAllLines( dbPath );
                byte[] dbBase64Text = Convert.FromBase64String( lines[1] );

                return System.Text.ASCIIEncoding.ASCII.GetString( dbBase64Text );
            }
        }
        /// <summary>Install path</summary>
        private static String installLocation = String.Empty;
        public static String InstallLocation
        {
            get
            {
                if ( DropBoxService.installLocation == String.Empty )
                {
                    String appDataPath = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
                    DropBoxService.installLocation = System.IO.Path.Combine( appDataPath , "Dropbox" );
                    String dbPath = System.IO.Path.Combine( DropBoxService.InstallLocation , "host.db" );
                    String dbxPath = System.IO.Path.Combine( DropBoxService.InstallLocation , "host.dbx" );
                    if ( File.Exists( dbPath ) == false && File.Exists(dbxPath) == false)
                    {
                        appDataPath = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );
                        DropBoxService.installLocation = System.IO.Path.Combine( appDataPath , "Dropbox" );
                    }
                    return DropBoxService.installLocation;
                }
                return DropBoxService.installLocation;
            }
        }

        /// <summary>Application path</summary>
        private static String appLocation = string.Empty;
        public static String AppLocation
        {
            get
            {
                if ( DropBoxService.appLocation == String.Empty )
                {
                    //Old version app path
                    DropBoxService.appLocation = System.IO.Path.Combine( DropBoxService.InstallLocation , "bin\\Dropbox.exe" );
                    //New version app path
                    if ( !File.Exists(DropBoxService.appLocation) )
                        DropBoxService.appLocation = System.IO.Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),"Dropbox/Client/Dropbox.exe");
                }
                return DropBoxService.appLocation;
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
            if ( System.Diagnostics.Process.GetProcessesByName( "Dropbox" ).ToList().Count <= 0 )
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
                catch ( System.Exception )
                {
                    DialogResult result = MessageBoxEx.Show(
                        new Form { TopMost = true },
                       "Cannot find your Dropbox Folder!\r\nPlease check if it has been installed or logined.\r\nWould you like to install Dropbox?" ,
                        "Error" , MessageBoxButtons.YesNo, MessageBoxIcon.Question );
                   if (result == DialogResult.Yes)
                   {
                       System.Diagnostics.Process.Start("iexplore.exe", "https://www.dropbox.com/downloading?src=index");
                   }
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
            catch ( System.Exception err ){}
        }
    }
}
