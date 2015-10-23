/***********************************************************************
 * CLR Version :      $civersion$
 * Class Name  :      RegistryHelper
 * Name Space  :      SecuruStik.BaseExtension
 * File Name   :      RegistryHelper
 * Create Time :      2015/1/1 9:33:57
 * Author      :
 * Brief       :
 * Modification history : 
 ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Helpers
{
    public class RegistryHelper
    {
        private String SubKey = String.Empty;
        private RegistryKey RootRegistry;

        public RegistryHelper( RegistryKey rootRegistry , String Key )
        {
            this.SubKey = Key;
            this.RootRegistry = rootRegistry;
        }

        public String GetValue( String ValueName )
        {
            if ( String.IsNullOrEmpty( ValueName ) )
            {
                throw new ArgumentNullException( "This method does not accept null paramters." );
            }
            String result = String.Empty;
            try
            {
                RegistryKey registryKey = RootRegistry.OpenSubKey( this.SubKey );
                result = registryKey.GetValue( ValueName ).ToString();
            } catch ( System.Exception )
            {
                return String.Empty;
            }
            return result;
        }
        public Boolean UpdateValue( String ValueName , String Value )
        {
            if ( String.IsNullOrEmpty( ValueName ) || String.IsNullOrEmpty( Value ) )
            {
                throw new ArgumentNullException( "This method does not accept null paramters." );
            }
            RegistryKey registryKey = RootRegistry.OpenSubKey( this.SubKey , true );

            if ( null == registryKey )
            {
                registryKey = RootRegistry.CreateSubKey( this.SubKey );
            }
            registryKey.SetValue( ValueName , Value );

            return true;
        }

        public static String GetValue( RegistryKey RootRegistry , String Key , String ValueName )
        {
            if ( RootRegistry == null ||  String.IsNullOrEmpty( Key ) || String.IsNullOrEmpty( ValueName ) )
            {
                throw new ArgumentNullException( "The key or rootRegistry is null" );
            }
            String result = String.Empty;
            try
            {
                RegistryKey registryKey = RootRegistry.OpenSubKey( Key );
                result = registryKey.GetValue( ValueName ).ToString();
            } catch ( System.Exception )
            {
                return String.Empty;
            }
            return result;
        }
        public static Boolean UpdateValue( RegistryKey RootRegistry , String Key , String ValueName , String Value )
        {
            if ( RootRegistry == null || String.IsNullOrEmpty( Key ) ||
                String.IsNullOrEmpty( ValueName ) || String.IsNullOrEmpty( Value ) )
            {
                throw new ArgumentNullException( "This method does not accept null paramters." );
            }

            RegistryKey registryKey = RootRegistry.OpenSubKey( Key , true );
            if ( registryKey == null )
            {
                registryKey = RootRegistry.CreateSubKey( Key );
            }
            registryKey.SetValue( ValueName , Value );

            return true;
        }
    }
}
