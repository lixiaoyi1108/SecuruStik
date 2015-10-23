using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Metro;
using SecuruStik.BaseExtension;
using SecuruStik.Opt;
using System.Text;
using SecuruStik.DropBox;
using System.Collections.Generic;
using System.Threading;
using SecuruStik.DB;
using SecuruStik.PRE;
using Microsoft;
using SecuruStik.MessageQueue;

namespace SecuruStik.UIDesign
{
    public partial class SecuruStik_MainForm :MetroForm
    {
        private DBox_User DropBoxUser;
        private Boolean IsChecking = true;

        public SecuruStik_MainForm( DBox_User dropBoxUser )
        {
            try
            {
                InitializeComponent();

                this.CreateDelegateTable();

                this.DropBoxUser = dropBoxUser;
                
                this.SynchronizeAsync();

                if (!VelostiScsi.IsPlugin_PubDrive)
                    this.Notify_Eject();
            }
            catch ( System.Exception ex )
            {
                MessageBoxEx.Show(ex.Message);
                this.Close();
            }
        }
        private void DBoxPRE_UI_Main_Load( object sender , EventArgs e )
        {
            #region Monitor the local SecFolder
            this.fileSystemWatcher_LocalSecFolder.EnableRaisingEvents = true;
            this.fileSystemWatcher_LocalSecFolder.Filter = "*.*";
            this.fileSystemWatcher_LocalSecFolder.IncludeSubdirectories = true;
            this.fileSystemWatcher_LocalSecFolder.NotifyFilter =
                NotifyFilters.FileName |
                NotifyFilters.DirectoryName |
                NotifyFilters.Size |
                NotifyFilters.LastWrite;
            this.fileSystemWatcher_LocalSecFolder.Path = DropBoxController.Local_SecuruStikFolder;
            this.OpenSecuruSticSecFolder();
            #endregion

            #region Monitor the dropbox folder
            this.fileSystemWatcher_DropboxShareFolder.EnableRaisingEvents = true;
            this.fileSystemWatcher_DropboxShareFolder.Filter = "*.*";
            this.fileSystemWatcher_DropboxShareFolder.IncludeSubdirectories = true;
            this.fileSystemWatcher_DropboxShareFolder.NotifyFilter =
                NotifyFilters.FileName |
                NotifyFilters.DirectoryName |
                NotifyFilters.Size |
                NotifyFilters.LastWrite;
            this.fileSystemWatcher_DropboxShareFolder.Path = DropBoxController.DropBox_DownloadFolder;
            #endregion
            
            #region MainForm
            this.TopMost = true;
            this.Shown += ( sender_shown , e_shown ) => { this.Opacity = 100;  this.Visible = false; };
            this.Deactivate += ( sender_leave , e_leave ) =>
                {
                    if ( Cursor.Position.X < this.Left || Cursor.Position.X > this.Right ||
                        Cursor.Position.Y < this.Top || Cursor.Position.Y > this.Bottom )
                        this.Visible = false;
                };
            this.FormClosing += ( sender_formcloing , event_formclosing ) =>
                {
                    this.DropBoxUser.Close();
                    this.notifyIcon.Visible = false; 
                };
            #endregion

            this.Size = new Size( 66 , 25 );
            this.notifyIcon.Tag = String.Empty;
        }

        #region Options
        private void OpenSecuruSticSecFolder() { Win32API.BringWindowToTopOrCreate( DropBoxController.Local_SecFolder ); }
        private void OpenDropBoxFolder() { Win32API.BringWindowToTopOrCreate( DropBoxController.DropBoxFolder ); }
        private void OpenDownloadFolder() { Win32API.BringWindowToTopOrCreate( DropBoxController.Local_DownloadFolder ); }
        private void OpenShareInfosFolder() { Win32API.BringWindowToTopOrCreate( DropBoxController.Local_ShareInfoFolder ); }
        private void Close()
        {
            this.notifyIcon.Visible = false;
            Process.GetCurrentProcess().Kill();
        }

        private void SynchronizeAsync()
        {
            this.DropBoxUser.dropBoxController.SynchronizeAsync();
        }
        private String GetNonConflictFileName( String fullPath )
        {
            String oldFileName = Path.GetFileName( fullPath );
            String destPath = Path.Combine( DropBoxController.Local_SecFolder , oldFileName );
            if ( fullPath == destPath )
                return String.Empty;
            return FileStringHelper.GetNonConflictFileName( destPath );
        }
        /// <returns>The file full path in SecuruStik secfolder</returns>
        private Boolean MoveToSecuruStikSecFolder( String fullPath )
        {
            try
            {
                String destPath = this.GetNonConflictFileName( fullPath );
                File.Move( fullPath , destPath );
                return true;
            }
            catch ( System.Exception )
            {
                return false;
            }
        }
        private void MoveToSecuruStikSecFolder( String[] fullPaths )
        {
            foreach ( String fullPath in fullPaths )
            {
                MoveToSecuruStikSecFolder( fullPath );
            }
        }
        private Boolean CopyToSecuruStikSecFolder( String fullPath )
        {
            try
            {
                String destPath = this.GetNonConflictFileName( fullPath );
                File.Copy( fullPath , destPath );
                return true;
            }
            catch ( System.Exception )
            {
                return false;
            }
        }
        private void CopyToSecuruStikSecFolder( String[] fullPaths )
        {
            foreach( String fullPath in fullPaths )
            {
                CopyToSecuruStikSecFolder( fullPath );
            }
        }
        private Boolean ShareFile( String fullPath )
        {
            if ( !VelostiScsi.IsPlugin_PubDrive )
            {
                this.Notify_Eject();
                return false;
            }
            ShareForm sf = new ShareForm();
            sf.ShowDialog();
            if ( sf.EmailAddrStr == String.Empty || sf.DialogResult == DialogResult.Cancel )
                return false;
            return this.DropBoxUser.ShareFile( sf.EmailAddrStr , fullPath );
        }
        private void ShareFile( String[] fullPaths )
        {
            if ( !VelostiScsi.IsPlugin_PubDrive )
            {
                this.Notify_Eject();
                return;
            }
            ShareForm sf = new ShareForm();
            sf.ShowDialog();
            if ( sf.EmailAddrStr == String.Empty || sf.DialogResult == DialogResult.Cancel )
                return;
            foreach ( String fullPath in fullPaths )
            {
                this.DropBoxUser.ShareFile( sf.EmailAddrStr , fullPath );
            }
        }
        private void Download( String fullPath )
        {
            Byte[] siBytes = File.ReadAllBytes( fullPath );
            SecuruStik.Protocal.SharingInfo si = SerializeHelper.DeSerialize( siBytes ) as SecuruStik.Protocal.SharingInfo;
            if (si != null)
            {
                this.DropBoxUser.DownloadSharingFile(si);
                Notify_Download_Begin(si.FileName, si.ID_TO);
            }
            else
            {
                SecuruStikMessageQueue.SendMessage_Download_Failed();
            }
        }
        #endregion Options

        #region Button Click
        private void buttonX_Exit_Click( object sender , EventArgs e )
        {
            this.Visible = false;
            this.notifyIcon.Visible = false;
            Process.GetCurrentProcess().Kill();
        }
        #endregion
        
        #region Monitor SecFolder
        private void SecFolder_Created( object sender , FileSystemEventArgs e )
        {
            if ( File.Exists( e.FullPath ) )
                this.DropBoxUser.dropBoxController.AddEncryptTask( e.FullPath );
            else if ( Directory.Exists( e.FullPath ) )
                this.DropBoxUser.dropBoxController.Synchronize_Local2DropBox( e.FullPath );
        }
        private void fileSystemWatcher_LocalSecFolder_Deleted( object sender , FileSystemEventArgs e )
        {
            String dropboxpath = this.DropBoxUser.dropBoxController.SecuruStikFolder2DropboxSecuruStikFolder( e.FullPath );
            this.DropBoxUser.dropBoxController.DeleteDropboxFile( dropboxpath );
        }
        #endregion Monitor SecFolder

        #region Monitor Share Folder
        private void fileSystemWatcher_DropboxShareFolder_Created( object sender , FileSystemEventArgs e )
        {
            this.DropBoxUser.dropBoxController.AddDecryptTask( e.FullPath );
        }
        private void fileSystemWatcher_DropboxShareFolder_Deleted( object sender , FileSystemEventArgs e )
        {
            String localpath = this.DropBoxUser.dropBoxController.DropboxSecuruStikFolder2SecuruStikFolder( e.FullPath );
            try
            {
                File.Delete( localpath );
            }
            catch ( System.Exception ) { }
        }
        #endregion Monitor Share Folder

    }
}