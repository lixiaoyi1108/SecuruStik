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
using System.IO;

namespace SecuruStik.BaseExtension
{
    public static class ConfigHelper
    {
        private static log4net.ILog log = Exception.Tracer.GetClassLogger();

        public static string CustomConfigFile
        {
            get;
            set;
        }

        public static Properties.Settings UserSettings {
            get {
                return Properties.Settings.Default;
            }
        }

        public static string GetAppConfig(string Key)
        {
            log.Error(String.Format("GetAppConfig key={0} not implemented (move to Settings)", Key));
            return null;
        }

        public static void UpdateAppConfig(String newKey, String newValue)
        {
            log.Error(String.Format("UpdateAppConfig deprecated, setting {0}={1}", newKey, newValue));
        }
        /*
        private static Configuration OpenAppConfig()
        {
            if (CustomConfigFile != null)
            {
                ExeConfigurationFileMap map = new ExeConfigurationFileMap();
                map.ExeConfigFilename = CustomConfigFile;
                return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            }
            else
            {
                return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);
            }
        }

        public static String GetAppConfig( String strKey )
        {
            Configuration config = OpenAppConfig();

            // TODO: use system user profile folder as is default for ConfigurationManager?
            
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
            Configuration config = OpenAppConfig();
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
        */
    }
}
