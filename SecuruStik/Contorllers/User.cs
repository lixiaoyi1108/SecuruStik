﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using SecuruStik.BaseExtension;
using SecuruStik.Contorllers;
using SecuruStik.DB;
using SecuruStik.DropBox;
using SecuruStik.Exception;
using SecuruStik.PRE;
using SecuruStik.Protocal;
using SecuruStikSettings;
using UserSetting.Key;

namespace SecuruStik.Opt
{
    public class DBox_User
    {
        #region 0. Fields( Controllers )
        public DropBoxController dropBoxController;
        private ProxyServerController proxyServerController;
        private ShareTaskWorker ShareWorker;
        #endregion Fields( Controllers )

        public PublicKey PK;
        private System.Timers.Timer Timer = null ;
        private readonly int OneMinutes = 60000;

        #region 1. Setup & UnSetup

        #region 1.1 Contructor & Destructor

        public DBox_User()
        {
            ///Initialize controllers
            Init_DropBoxController();
            Init_ProxyServerController();
            Init_UserKey();
            Init_Environment();

            this.ShareWorker = new ShareTaskWorker(
                this.dropBoxController ,
                this.proxyServerController );
        }
        ~DBox_User()
        {
            this.Close();
        }
        public void Close()
        {
            try
            {
                ProxyReEncryption.UnSetup();
                this.proxyServerController.UnInit();
            }
            catch ( System.Exception ) { }

        }

        #endregion Contructor & Destructor

        #region 1.2 Inits

        private void Init_DropBoxController()
        {
            try
            {
                this.dropBoxController = new DropBoxController();
                if ( this.dropBoxController.Login() == false )
                    Process.GetCurrentProcess().Kill();
            } catch ( SecuruStikException ex )
            {
                throw ex;
            } catch ( System.Exception ex )
            {
                throw new SecuruStikException( SecuruStikExceptionType.Init_DropBoxController , "Failed to initial DropBoxController" , ex );
            }
        }
        private void Init_ProxyServerController()
        {
            try
            {
                proxyServerController = new ProxyServerController( this.dropBoxController.Email );
            } catch ( SecuruStikException ex )
            {
                throw ex;
            }catch (System.Exception ex)
            {
                throw new SecuruStikException( SecuruStikExceptionType.Init_DropBoxController , "Failed to initial ProxyServerController" , ex );
            }
        }
        private void Init_UserKey()
        {
            try
            {
                UserKey userKey = PreKeyring.UserKey;

                if ( userKey == null )
                {
                    this.ReGenUserKey();
                    this.PublishPK();
                }
                else 
                {
                    this.PK = new Protocal.PublicKey(
                        id: this.dropBoxController.Email ,
                        pk1: userKey.PK1 ,
                        pk2: userKey.PK2 );

                    if ( !userKey.IsPublicized )
                        this.PublishPK();
                }
            }catch ( SecuruStikException ex )
            {
                throw ex;
            } 
            catch (System.Exception ex)
            {
                throw new SecuruStikException( SecuruStikExceptionType.Init_UserKey , "Failed to initial UserKey" , ex );
            }
        }

        private void Init_Environment()
        {
            try
            {
                if ( !AppSetting.IsInited )
                {
                    Init_ShellExtension();  //Register the shell extension
                    Init_CreateShortCuts(); //Create shortcuts
                    Init_RegisterFileType();//register the file type ".securu"
                    AppSetting.IsInited = true;
                    ReStartExplorer();
                }
            } catch ( System.Exception ex )
            {
                throw new SecuruStikException( SecuruStikExceptionType.Init_Environment , "Failed to initial environment" , ex );
            }
        }
        private void Init_ShellExtension()
        {
            if ( !Directory.Exists( AppSetting.ApplicationData_SecuruStikPath ) )
            {
                Directory.CreateDirectory( AppSetting.ApplicationData_SecuruStikPath );
            }
            //NOTE: The "SharpShell.dll" must locate in app folder
            try
            {
                File.Copy( AppSetting.App_ShellExtension , AppSetting.AppDataFolder_ShellExtensionsFullPath , true );
                File.Copy( AppSetting.App_ShellExtension_Sharpdll , AppSetting.AppDataFolder_ShellExtensions_SharpdllFullPath , true );
            } catch ( System.Exception ex ){}
            ComRegisterHelper.Register( AppSetting.AppDataFolder_ShellExtensionsFullPath );
        }
        private void Init_CreateShortCuts()
        {
             ShortcutUnit[] shortcutList = new ShortcutUnit[] { 
                 new ShortcutUnit( AppSetting.Shortcut_Desktop , AppSetting.AppFullPath , AppSetting.App_IconPath , AppSetting.Description ) ,
                new ShortcutUnit( AppSetting.Shortcut_SendTo , AppSetting.AppFullPath , AppSetting.App_IconPath , AppSetting.Description ) ,
                new ShortcutUnit( AppSetting.Shortcut_StartMenu , AppSetting.AppFullPath , AppSetting.App_IconPath , AppSetting.Description ) ,
                new ShortcutUnit( AppSetting.Shortcut_NetworkShortcuts , AppSetting.AppFullPath , AppSetting.App_IconPath , AppSetting.Description ) ,
                new ShortcutUnit( AppSetting.Shortcut_Links , AppSetting.AppFullPath , AppSetting.App_IconPath , AppSetting.Description ) ,
             };
             foreach ( ShortcutUnit scu in shortcutList )
             {
                 if(!File.Exists(scu.LinkName))
                    ShortcutHelper.CreateShortcut( scu );
             } 
        }
        private void Init_RegisterFileType()
        {
            if ( !System.IO.Directory.Exists( AppSetting.ApplicationData_SecuruStikPath ) )
            {
                Directory.CreateDirectory( AppSetting.ApplicationData_SecuruStikPath );
            }
            File.Copy( AppSetting.App_Downloader , AppSetting.AppDataFolder_DownloaderFullPath ,true );
            File.Copy( AppSetting.App_IconPath , AppSetting.AppDatafolder_Icon , true );

            FileTypeRegInfo fileTypeRegInfo = new FileTypeRegInfo();
            fileTypeRegInfo.Description = "SecuruStik share info file";
            fileTypeRegInfo.ExePath = AppSetting.AppDataFolder_DownloaderFullPath;
            fileTypeRegInfo.ExtendName = AppSetting.ShareInfoFileExtension;
            fileTypeRegInfo.IcoPath = AppSetting.AppDatafolder_Icon;
            FileTypeRegister.RegisterFileType( fileTypeRegInfo );
        }
        private void ReStartExplorer()
        {
            //Restart the explorer
            Process[] process = Process.GetProcesses();
            //var p = (from proc in process
            //         where proc.ProcessName.Equals("explorer")
            //         select proc).FirstOrDefault();
            //p.Kill(); 
            var ps = ( from proc in process
                       where proc.ProcessName.Equals( "explorer" )
                       select proc );
            foreach ( Process p in ps )
            {
                p.Kill();
            }
        }

        #endregion Initializations

        #endregion Setup & UnSetup

        #region 2. Sharing File Operation

        public Boolean ShareFile( String id_to , String filePath )
        {
            return this.ShareWorker.AddShareTask( id_to , filePath );
        }
        public void RequestSharingInfo()
        {
            this.proxyServerController.Request_GetSharingInfo();
        }
        public Boolean DownloadSharingFile(Protocal.SharingInfo si)
        {
            String copyRef = si.Reference;
            String fileName = si.FileName;
            try
            {
                String dropboxPath = BaseExtension.FileStringHelper.GetNonConflictFileName( Path.Combine( DropBoxController.DropBox_DownloadFolder , fileName ) );

                UserKey uk = PreKeyring.UserKey;
                PRE_KEY userKey = new PRE_KEY();
                userKey.PK.PK1 = uk.PK1;
                userKey.PK.PK2 = uk.PK2;
                userKey.SK.X1 = uk.SK1;
                userKey.SK.X2 = uk.SK2;

                PRE_Cipher CKey = new PRE_Cipher();
                CKey.E = si.CKey_E;
                CKey.F = si.CKey_F;
                CKey.U = si.CKey_U;
                CKey.W = si.CKey_W;

                String key = ProxyReEncryption.KeyDecrypt( userKey , CKey );

                FileMetaData fi = new FileMetaData();
                fi.FileName = fileName;
                fi.FilePath = this.dropBoxController.DropboxSecuruStikFolder2SecuruStikFolder( dropboxPath );
                fi.Key = key;
                fi.PlainTextHash = "";
                fi.CryptTextHash = "";
                PreKeyring.FileInfo_Update( fi );

                String dropboxRemotePath = this.dropBoxController.SecuruStikFolder2RemoteDropboxPath( fi.FilePath );
                this.dropBoxController.CopyAsync( copyRef , dropboxRemotePath );
            }
            catch ( System.Exception )
            {
                PreKeyring.SharingFile_Delete( si.Reference );
                return false;
            }
            return true;
        }
        public void ReceivePK( String id , String pk1 , String pk2 )
        {
            this.ShareWorker.ReceivePKInfo( new PublicKey( id , pk1 , pk2 ) );
        }
        
        #endregion Sharing File Operation

        #region 3. Key distribution

        public void ReGenUserKey()
        {
            //Create a new user key
            PRE_KEY key = ProxyReEncryption.GenRandomKey();

            this.PK = new Protocal.PublicKey(
                id: this.dropBoxController.Email ,
                pk1: key.PK.PK1 ,
                pk2: key.PK.PK2 );

            PreKeyring.UserKey = new SecuruStik.DB.UserKey
            {
                PK1 = key.PK.PK1 ,
                PK2 = key.PK.PK2 ,
                SK1 = key.SK.X1 ,
                SK2 = key.SK.X2 ,
                IsPublicized = false
            };

        }
        private void PublishPK()
        {
            //Upload the pk to DropBox
            Boolean ret = this.proxyServerController.Request_SetPK( this.PK );
            if ( ret )
            {
                PreKeyring.IsPublicized = true;
                if ( Timer != null )
                {
                    Timer.Stop();
                    Timer.Dispose();
                }
            }
            else
            {
                Timer = new System.Timers.Timer( this.OneMinutes );
                Timer.Elapsed+=new ElapsedEventHandler(
                    (sender,obj) =>
                    {
                        Boolean isSuccess = this.proxyServerController.Request_SetPK( this.PK );
                        if ( isSuccess )
                        {
                            PreKeyring.IsPublicized = true;
                            this.Timer.Stop();
                            this.Timer.Dispose();
                        }
                    } );
                Timer.Start();
            }
        }

        #endregion Key distribution

    }
}