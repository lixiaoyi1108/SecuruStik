/***********************************************************************
 * CLR Version :      $civersion$
 * Class Name  :      FileTypeRegister
 * Name Space  :      SecuruStik.BaseExtension
 * File Name   :      FileTypeRegister
 * Create Time :      2015/3/2 10:00:41
 * Author      :      JianYe,Huang
 * Brief       :      Regesiter file type
 * Modification history : 
 ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace SecuruStik.BaseExtension
{
    public class FileTypeRegister
    {
        #region RegisterFileType   
        public static void RegisterFileType( FileTypeRegInfo regInfo )
        {
            if ( IsFileTypeRegistered( regInfo.ExtendName ) )
            {
                UpdateFileTypeRegInfo( regInfo );
            }
            else
            {
                String relationName = regInfo.ExtendName.Substring( 1 , regInfo.ExtendName.Length - 1 ).ToUpper();
                RegistryKey fileTypeKey = Registry.ClassesRoot.CreateSubKey( regInfo.ExtendName );
                fileTypeKey.SetValue( "" , relationName );
                fileTypeKey.Close();
                RegistryKey relationKey = Registry.ClassesRoot.CreateSubKey( relationName );
                relationKey.SetValue( "" , regInfo.Description );
                RegistryKey iconKey = relationKey.CreateSubKey( "DefaultIcon" );
                iconKey.SetValue( "" , String.Format( "\"{0}\"" , regInfo.IcoPath ) );
                RegistryKey shellKey = relationKey.CreateSubKey( "Shell" );
                RegistryKey openKey = shellKey.CreateSubKey( "Open" );
                RegistryKey commandKey = openKey.CreateSubKey( "Command" );
                commandKey.SetValue( "" , String.Format( "\"{0}\"" , regInfo.ExePath ) + " %1" );
                relationKey.Close();
            }
        }
        public static void UnRegisterFileType(String ExtendName)
        {
            if (!IsFileTypeRegistered(ExtendName))
            {
                return;
            }
            String relationName = ExtendName.Substring(1, ExtendName.Length - 1).ToUpper();
            Registry.ClassesRoot.DeleteSubKey(ExtendName);
            Registry.ClassesRoot.DeleteSubKeyTree(relationName);
        }
        /// <summary>
        /// Get infomation of the file type
        /// </summary>          
        public static FileTypeRegInfo GetFileTypeRegInfo( string extendName )
        {
            if ( !IsFileTypeRegistered( extendName ) )
            {
                return null;
            }
            else
            {
                FileTypeRegInfo regInfo = new FileTypeRegInfo( extendName );
                String relationName = extendName.Substring( 1 , extendName.Length - 1 ).ToUpper() + "_FileType";
                RegistryKey relationKey = Registry.ClassesRoot.OpenSubKey( relationName );
                regInfo.Description = relationKey.GetValue( "" ).ToString();
                RegistryKey iconKey = relationKey.OpenSubKey( "DefaultIcon" );
                regInfo.IcoPath = iconKey.GetValue( "" ).ToString();
                RegistryKey shellKey = relationKey.OpenSubKey( "Shell" );
                RegistryKey openKey = shellKey.OpenSubKey( "Open" );
                RegistryKey commandKey = openKey.OpenSubKey( "Command" );
                string temp = commandKey.GetValue( "" ).ToString();
                regInfo.ExePath = temp.Substring( 0 , temp.Length - 3 );
                return regInfo;
            }
        }
        /// <summary>  
        /// Update infomation of the file type
        /// </summary>
        public static bool UpdateFileTypeRegInfo( FileTypeRegInfo regInfo )
        {
            String relationName = regInfo.ExtendName.Substring( 1 , regInfo.ExtendName.Length - 1 ).ToUpper();
            RegistryKey relationKey = Registry.ClassesRoot.OpenSubKey( relationName , true );
            relationKey.SetValue( "" , regInfo.Description );
            RegistryKey iconKey = relationKey.OpenSubKey( "DefaultIcon" , true );
            iconKey.SetValue( "" , String.Format( "\"{0}\"" , regInfo.IcoPath ) );
            RegistryKey shellKey = relationKey.OpenSubKey( "Shell" );
            RegistryKey openKey = shellKey.OpenSubKey( "Open" );
            RegistryKey commandKey = openKey.OpenSubKey( "Command" , true );
            commandKey.SetValue( "" , String.Format( "\"{0}\" %1" , regInfo.ExePath ) );
            relationKey.Close();

            return true;
        }
        /// <summary>  
        /// Check the file type is already registered.
        /// </summary>          
        public static bool IsFileTypeRegistered( string extendName )
        {
            RegistryKey softwareKey = Registry.ClassesRoot.OpenSubKey( extendName );
            if ( softwareKey != null )
            {
                return true;
            }
            return false;
        }
        #endregion
    }
    public class FileTypeRegInfo
    {
        public String ExtendName;  //".xcf"  
        public String Description; //"XCodeFactory"
        public String IcoPath;
        public String ExePath;
        public FileTypeRegInfo(){}
        public FileTypeRegInfo( string extendName )
        {
            this.ExtendName = extendName;
        }
    } 
}
