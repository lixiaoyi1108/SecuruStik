/***********************************************************************
 * CLR Version :      $civersion$
 * Class Name  :      ConfigHelper
 * Name Space  :      SecuruStik.BaseExtension
 * File Name   :      ConfigHelper
 * Create Time :      2014/12/30 15:37:19
 * Author      :
 * Brief       :
 * Modification history : 
 ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SecuruStik.BaseExtension
{
    public static class ConfigHelper
    {
        public static String GetAppConfig( String strKey )
        {
            String file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration( file );
            foreach ( String key in config.AppSettings.Settings.AllKeys )
            {
                if ( key == strKey )
                {
                    return config.AppSettings.Settings[strKey].Value.ToString();
                }
            }
            return null;
        }

        public static void UpdateAppConfig( String newKey , String newValue )
        {
            String file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration( file );
            bool exist = false;
            foreach ( string key in config.AppSettings.Settings.AllKeys )
            {
                if ( key == newKey )
                {
                    exist = true;
                }
            }
            if ( exist )
            {
                config.AppSettings.Settings.Remove( newKey );
            }
            config.AppSettings.Settings.Add( newKey , newValue );
            config.Save( ConfigurationSaveMode.Modified );
            ConfigurationManager.RefreshSection( "appSettings" );
        }

    }
}
