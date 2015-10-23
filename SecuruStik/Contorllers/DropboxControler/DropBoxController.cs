/***********************************************************************
 * CLR Version :      4.0.30319.18444
 * Class Name  :      DropBoxClient
 * Name Space  :      DropBox
 * File Name   :      DropBoxClient
 * Create Time :      2014/10/28 14:45:21
 * Author      :      JianYe,Huang
 * Brief       :      Communicate with dropbox( login & file options )
 ***********************************************************************/
using System;
using System.IO;
using SecuruStik.Exception;
using SecuruStik.PRE;
using SecuruStikSettings;

namespace SecuruStik.DropBox
{
    public partial class DropBoxController
    {
        #region 0. Fields
        
        #region 0.1 Dropbox Paths

        /// <summary>Dropbox Folder </summary>
        private DropBoxService dropBoxService;
        public static String DropBoxFolder
        {
            get { return DropBoxService.DropBoxFolderLocation; }
        }
        public static String DropBox_SecuruStikFolder
        {
            get { return  Path.Combine( DropBoxController.DropBoxFolder, AppSetting.Name_SecuruSitkFolder) ; }
        }
        public static String DropBox_SecFolder
        { get { return Path.GetFullPath( Path.Combine( DropBoxController.DropBox_SecuruStikFolder , AppSetting.Name_DropBoxSecFolder ) ); } }
        public static String DropBox_DownloadFolder
        {
            get
            {
                return Path.GetFullPath( Path.Combine( DropBoxController.DropBox_SecuruStikFolder , AppSetting.Name_DownloadFolder ) );
            }
        }
        public static String DropBox_ShareInfoFolder
        {
            get { return Path.GetFullPath( Path.Combine( DropBoxController.DropBox_SecuruStikFolder , AppSetting.Name_ShareInfoFolder ) ); }
        }
        public static String Dropbox_PKFolder
        {
            get { return Path.GetFullPath( Path.Combine( DropBoxController.DropBox_SecuruStikFolder , "" ) ); }
        }
        #endregion Dropbox Paths

        #region 0.2 Local Paths
        /// <summary>Local SecuruStik Folder C:\User\Administrator\..</summary>
        public static String Local_SecuruStikFolder = AppSetting.Path_Local_SecuruStikFolder;
        public static String Local_SecFolder = AppSetting.Path_Local_SecuruStikSecFolder;
        public static String Local_DownloadFolder = AppSetting.Path_Local_SecuruStikDownloadFolder;
        public static String Local_ShareInfoFolder = AppSetting.Path_Local_SecuruStikShareInfoFolder;
        #endregion Local Paths

        #endregion Fields

        #region 1. Construction
        public DropBoxController()
        {
            this.LoopBackTool = new VelostiScsi();
            this.dropBoxService = new DropBoxService();
            this.CreateDroboxSecuruStikFolders();
            this.CreateLocalSecuruStikFolders();
        }
        ~DropBoxController()
        {
            ProxyReEncryption.UnSetup();
        }
        #endregion Construction

        #region 2. Create SecuruFolders

        private void CreateDroboxSecuruStikFolders()
        {
            DirectoryInfo di = new DirectoryInfo(DropBoxController.DropBox_SecuruStikFolder);
            if (!di.Exists)
            {
                di.Create();
                di.Attributes = FileAttributes.Hidden;

                di.CreateSubdirectory(AppSetting.Name_DropBoxSecFolder);
                di.CreateSubdirectory(AppSetting.Name_DownloadFolder);
                di.CreateSubdirectory(AppSetting.Name_ShareInfoFolder);
            }
            else
            {
                if (di.Attributes != FileAttributes.Hidden)
                {
                    di.Attributes = FileAttributes.Hidden;
                }
                if (!Directory.Exists(DropBoxController.DropBox_SecFolder))
                {
                    di.CreateSubdirectory(AppSetting.Name_DropBoxSecFolder);
                }
                if (!Directory.Exists(DropBoxController.DropBox_DownloadFolder))
                {
                    di.CreateSubdirectory(AppSetting.Name_DownloadFolder);
                }
                if (!Directory.Exists(DropBoxController.DropBox_ShareInfoFolder))
                {
                    di.CreateSubdirectory(AppSetting.Name_ShareInfoFolder);
                }

            }
        }

        private void CreateLocalSecuruStikFolders()
        {
            DirectoryInfo di = new DirectoryInfo(DropBoxController.Local_SecuruStikFolder);
            if (!di.Exists)
            {
                di.Create();
                di.CreateSubdirectory(AppSetting.Name_LocalSecFolder);
                di.CreateSubdirectory(AppSetting.Name_DownloadFolder);
                di.CreateSubdirectory(AppSetting.Name_ShareInfoFolder);
            }
            else
            {
                if (!Directory.Exists(DropBoxController.Local_SecFolder))
                {
                    di.CreateSubdirectory(AppSetting.Name_LocalSecFolder);
                }
                if (!Directory.Exists(DropBoxController.Local_DownloadFolder))
                {
                    di.CreateSubdirectory(AppSetting.Name_DownloadFolder);
                }
                if (!Directory.Exists(DropBoxController.Local_ShareInfoFolder))
                {
                    di.CreateSubdirectory(AppSetting.Name_ShareInfoFolder);
                }
            }
        }

        #endregion 

        #region 3. Dropbox path map to SecFolder path
        /// <summary>
        /// Translate path from local SecuruStik path to dropbox SecuruStik path
        /// Example:
        /// The path in local fullpath: ~LocalSecFolder   \\test1\\myfile.txt
        /// The path in local dropbox : ~DropBox_SecFolder \\test1\\myfile.txt
        /// </summary>
        public String SecuruStikFolder2DropboxSecuruStikFolder( String localPath )
        {
            String fullPath = Path.GetFullPath( localPath );
            String OldRoot = String.Empty;
            String NewRoot = String.Empty;
            if(fullPath.StartsWith(DropBoxController.Local_SecFolder))
            {
                OldRoot = DropBoxController.Local_SecFolder;
                NewRoot = DropBoxController.DropBox_SecFolder;
            }
            else
            {
                OldRoot = DropBoxController.Local_SecuruStikFolder;
                NewRoot = DropBoxController.DropBox_SecuruStikFolder;
            }
            String DropBoxPath = fullPath.Replace( OldRoot , NewRoot ).Replace( '\\' , '/' );
            return Path.GetFullPath(DropBoxPath);
        }
        /// <summary>
        /// Translate path from dropbox-SecuruStik path to local-SecuruStik path
        /// </summary>
        public String DropboxSecuruStikFolder2SecuruStikFolder( String dropBoxPath )
        {
            String fullPath = Path.GetFullPath( dropBoxPath );
            String OldRoot = String.Empty;
            String NewRoot = String.Empty;
            if ( fullPath.StartsWith( DropBoxController.DropBox_SecFolder ) )
            {
                OldRoot = DropBoxController.DropBox_SecFolder;
                NewRoot = DropBoxController.Local_SecFolder;
            }
            else
            {
                OldRoot = DropBoxController.DropBox_SecuruStikFolder;
                NewRoot = DropBoxController.Local_SecuruStikFolder;
            }
            String LocalPath = fullPath.Replace( OldRoot , NewRoot ).Replace( '\\' , '/' );
            return Path.GetFullPath(LocalPath);
        }
        public String SecuruStikFolder2RemoteDropboxPath( String localPath )
        {
            String fullPath = Path.GetFullPath( localPath );
            String OldRoot = String.Empty;
            String NewRoot = String.Empty;
            if ( fullPath.StartsWith( DropBoxController.Local_SecFolder ) )
            {
                OldRoot = DropBoxController.Local_SecFolder;
                NewRoot = String.Format("/{0}/{1}",
                    AppSetting.Name_SecuruSitkFolder,
                    AppSetting.Name_DropBoxSecFolder);
            }
            else
            {
                OldRoot = DropBoxController.Local_SecuruStikFolder;
                NewRoot = String.Format( "/{0}" ,
                    AppSetting.Name_SecuruSitkFolder );
            }
            String DropBoxPath = localPath.Replace( OldRoot , NewRoot ).Replace( '\\' , '/' );
            return DropBoxPath;
        }

        #endregion Dropbox path map to SecFolder path
    }
}