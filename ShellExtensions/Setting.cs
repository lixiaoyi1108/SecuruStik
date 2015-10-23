/***********************************************************************
 * CLR Version :      $civersion$
 * Class Name  :      AppSetting
 * Name Space  :      SecuruStik.BaseExtension
 * File Name   :      AppSetting
 * Create Time :      2015/1/1 9:50:35
 * Author      :
 * Brief       :
 * Modification history : 
 ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;
using Helpers;

namespace SecuruStikSettings
{
    /// <summary> Setup the config of app </summary>
    /// Take care of the order of variable since their dependence.
    public static class AppSetting
    {
        #region 0. System Path
        //use for shortcuts
        public static String UserProfile =  Environment.GetFolderPath( Environment.SpecialFolder.UserProfile );
        public static String Desktop =  Environment.GetFolderPath( Environment.SpecialFolder.Desktop );
        public static String SendTo =  Environment.GetFolderPath( Environment.SpecialFolder.SendTo );
        public static String StartMenu =  Environment.GetFolderPath( Environment.SpecialFolder.StartMenu );
        public static String Startup =  Environment.GetFolderPath( Environment.SpecialFolder.Startup );
        public static String NetworkShortcuts =  Environment.GetFolderPath( Environment.SpecialFolder.NetworkShortcuts );
        public static String Links =  Path.Combine( AppSetting.UserProfile , "Links" );
        // C:\\Users\\Hjnbuys\\AppData\\Roaming
        public static String ApplicationDataPath = Path.GetFullPath( Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ) );
        #endregion System Path

        #region 1. User Setting
        public static String AppName = "SecuruStik";
        public static String Company = "VELOSTI TECHNOLOGY LTD";
        public static String apiKey = "adqaukzw3605k8u";
        public static String appSecret = "lus2cykvur5g4iy";
        #endregion User Setting

        #region 2. Names
        public static String ShareInfoFileExtension = ".securu";
        public static String Name_ShareInfoFileExtension = "securu";
        public static String Name_SecuruStikFormClass = "SecuruStik_UIDesign_MainForm";

        public static String Name_SecuruSitkFolder = AppSetting.AppName;
        public static String Name_LocalSecFolder = "Home";
        public static String Name_DropBoxSecFolder = "SecuruStikPrivate";
        public static String Name_DownloadFolder = "Downloads";
        public static String Name_ShareInfoFolder = "ShareInfos";

        public static String Name_Downloader = "Downloader";
        public static String Name_AppIconFile = "appicon.ico";
        #endregion Names

        #region 3. Local SecuruStik Path

        #region 3.1 App path
        public static String AppDir = Environment.CurrentDirectory;
        public static String AppFullPath = Application.ExecutablePath;
        public static String App_IconPath = Path.GetFullPath( "./Icon/appicon.ico" );
        public static String App_ShellExtension = Path.GetFullPath( "./Extension/ShellExtensions.dll" );
        public static String App_Downloader = Path.GetFullPath( "./Extension/Downloader.exe" );
        #endregion App path

        #region 3.2 Registry
        public static Boolean IsInited
        {
            get
            {
                String result = RegistryHelper.GetValue( Registry.CurrentUser , AppSetting.registryKey , AppSetting.registryValueName_IsInited );
                if ( String.IsNullOrEmpty( result ) )
                {
                    AppSetting.IsInited = false;
                    return false;
                }
                return Boolean.Parse( result );
            }
            set
            {
                String value_str = value.ToString();
                RegistryHelper.UpdateValue( Registry.CurrentUser , AppSetting.registryKey ,
                    AppSetting.registryValueName_IsInited , value_str );
            }
        }
        private static String registryKey = String.Format( "Software\\{0}\\{1}" , AppSetting.Company , AppSetting.AppName );
        private static String registryValueName_IsInited = "IsInited";
        private static String registryValueName_SecuruStikPath = "Path_Local_SecuruStikFolder";
        private static String _securuStikPath = String.Empty;
        public static String Path_Local_SecuruStikFolder
        {
            get
            {
                if ( String.IsNullOrEmpty( _securuStikPath ) )
                {
                    String result = RegistryHelper.GetValue( Registry.CurrentUser , AppSetting.registryKey , AppSetting.registryValueName_SecuruStikPath );
                    if ( String.IsNullOrEmpty( result ) )
                    {
                        String securuStikPath = Path.Combine(
                            Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ) ,
                            AppSetting.Name_SecuruSitkFolder );

                        RegistryHelper.UpdateValue( Registry.CurrentUser , AppSetting.registryKey ,
                            AppSetting.registryValueName_SecuruStikPath , securuStikPath );
                        _securuStikPath = securuStikPath;
                    }
                    else
                    {
                        _securuStikPath = result;
                    }
                }
                return _securuStikPath;
            }
            set
            {
                _securuStikPath = value;
                RegistryHelper.UpdateValue( Registry.CurrentUser , AppSetting.registryKey ,
                    AppSetting.registryValueName_SecuruStikPath , value );
            }
        }

        public static String Path_Local_SecuruStikSecFolder= Path.Combine( AppSetting.Path_Local_SecuruStikFolder , AppSetting.Name_LocalSecFolder );
        public static String Path_Local_SecuruStikDownloadFolder= Path.Combine( AppSetting.Path_Local_SecuruStikFolder , AppSetting.Name_DownloadFolder );
        public static String Path_Local_SecuruStikShareInfoFolder= Path.Combine( AppSetting.Path_Local_SecuruStikFolder , AppSetting.Name_ShareInfoFolder );
        #endregion Registry

        #region 3.3 ApplacationData folder path

        /// <summary> ApplicationData path data(C:\\Users\\Hjnbuys\\AppData\\Roaming\\SecuruStik)</summary>
        public static String ApplicationData_SecuruStikPath = Path.Combine(
            AppSetting.ApplicationDataPath ,
            String.Format( "{0}/{1}" , AppSetting.Company , AppSetting.AppName ) );

        public static String AppDataFolder_ShellExtensionsFullPath = Path.Combine( AppSetting.ApplicationData_SecuruStikPath , "ShellExtensions.dll" );
        public static String AppDataFolder_DownloaderFullPath = Path.Combine( AppSetting.ApplicationData_SecuruStikPath , "Downloader.exe" );

        #endregion ApplacationData folder path

        #region 3.4 Shortcut
        //Shortcut path( 6 )
        //Synchronization with CustonAction & SecuruStik.User.Init
        public static String Shortcut_LinkName      = String.Format( "{0}.lnk" , AppSetting.AppName );
        public static String Shortcut_Desktop   = Path.Combine( AppSetting.Desktop , AppSetting.Shortcut_LinkName );
        public static String Shortcut_SendTo    = Path.Combine( AppSetting.SendTo , AppSetting.Shortcut_LinkName );
        public static String Shortcut_StartMenu = Path.Combine( AppSetting.StartMenu , AppSetting.Shortcut_LinkName );
        public static String Shortcut_NetworkShortcuts  = Path.Combine( AppSetting.NetworkShortcuts , AppSetting.Shortcut_LinkName );
        public static String Shortcut_Links     = Path.Combine( AppSetting.Links , AppSetting.Shortcut_LinkName );
        public static String Description = "A secure home for your dropbox.";

        #endregion Shortcut

        #endregion Local SecuruStik Path

    }
}
