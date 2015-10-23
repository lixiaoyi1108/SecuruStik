using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using SecuruStik.BaseExtension;
using SecuruStikSettings;

namespace CustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult Uninstall( Session session )
        {
            session.Log( "Begin CustomAction1" );

            ReStartExplorer();
            UnRegisterShellExtension();
            DeleteShortcuts();
            UnRegisterFileType();
            ReStartExplorer();

            AppSetting.IsInited = false;

            return ActionResult.Success;
        }

        public static void UnRegisterShellExtension()
        {
            try
            {
                ComRegisterHelper.UnRegister( AppSetting.AppDataFolder_ShellExtensionsFullPath );

                if ( File.Exists( AppSetting.AppDataFolder_ShellExtensionsFullPath ) )
                    File.Delete( AppSetting.AppDataFolder_ShellExtensionsFullPath );
            } catch ( Exception ex ) { MessageBox.Show( ex.Message ); }
        }
        public static void DeleteShortcuts()
        {
            String[] shortcutList = new String[] { 
                    AppSetting.Shortcut_Desktop , 
                    AppSetting.Shortcut_SendTo , 
                    AppSetting.Shortcut_StartMenu , 
                    AppSetting.Shortcut_NetworkShortcuts , 
                    AppSetting.Shortcut_Links ,
             };
            foreach ( String scu in shortcutList )
            {
                if ( File.Exists( scu ) )
                {
                    File.Delete( scu );
                }
            }
        }
        public static void UnRegisterFileType()
        {
            FileTypeRegister.UnRegisterFileType( AppSetting.ShareInfoFileExtension );

            if ( File.Exists( AppSetting.AppDataFolder_DownloaderFullPath ) )
            {
                File.Delete( AppSetting.AppDataFolder_DownloaderFullPath );
            }
        }
        private static void ReStartExplorer()
        {
            //Restart the explorer
            Process[] process = Process.GetProcesses();
            //var p = ( from proc in process
            //          where proc.ProcessName.Equals( "explorer" )
            //          select proc ).FirstOrDefault();
            var ps = ( from proc in process
                      where proc.ProcessName.Equals( "explorer" )
                      select proc );
            foreach ( Process p in ps )
            {
                p.Kill();
            }
        }
    }
}
