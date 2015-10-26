/***********************************************************************
 * CLR Version :      4.0.30319.18444
 * Class Name  :      PreKeyring
 * Name Space  :      Model
 * File Name   :      PreKeyring
 * Create Time :      2014/10/17 10:22:57
 * Author      :      JianYe,Huang
 * Brief       :      Stores the access Token,user's key ,file keys & sharing info
 ***********************************************************************/
using System;
using System.Collections.Generic;

using System.Data.SQLite;
using System.IO;
using SecuruStik.DB.Base;
using SecuruStik.BaseExtension;
using System.Data;
using SecuruStik.Exception;

namespace SecuruStik.DB
{
    public static class PreKeyring
    {
        #region 0. Fields

        #region 0.1 AccessToken & UserKey

        #region 0.1.1 AccessToken

        // TODO: mention somewhere that this is for Dropbox OAuth access...
        private static String Node_AccessToken_UserToken = "AccessToken_UserToken";
        private static String Node_AccessToken_UserSecret = "AccessToken_UserSecret";
        private static AccessToken _accessToken = null;
        public static AccessToken AccessToken
        {
            get
            {
                if ( _accessToken == null )
                {
                    if (String.IsNullOrEmpty(ConfigHelper.UserSettings.DropboxAuthUserToken) || String.IsNullOrEmpty(ConfigHelper.UserSettings.DropboxAuthUserSecret))
                    {
                        return null;
                    }
                    else
                    {
                        var settings = ConfigHelper.UserSettings;
                        _accessToken = new AccessToken();
                        _accessToken.UserToken = settings.DropboxAuthUserToken;
                        _accessToken.UserSecret = settings.DropboxAuthUserSecret;
                    }
                }
                return _accessToken;                    
            }
            set 
            {
                if ( _accessToken == null ) _accessToken = new AccessToken();
                // don't know why it's making a copy here but whatever
                _accessToken.UserToken = value.UserToken;
                _accessToken.UserSecret = value.UserSecret;

                ConfigHelper.UserSettings.DropboxAuthUserToken = value.UserToken;
                ConfigHelper.UserSettings.DropboxAuthUserSecret = value.UserSecret;
            }
        }
        #endregion 0.1.1

        #region 0.1.2 UserKey
        private static String Node_UserKey_PK1 = "PK1";
        private static String Node_UserKey_PK2 = "PK2";
        private static String Node_UserKey_SK1 = "SK1";
        private static String Node_UserKey_SK2 = "SK2";
        private static String Node_UserKey_IsPublicized = "IsPublicized";
        private static UserKey _userKey;
        public static UserKey UserKey
        {
            get 
            {
                if ( _userKey == null )
                {
                    String UserKey_PK1 = ConfigHelper.GetAppConfig( PreKeyring.Node_UserKey_PK1 );
                    String UserKey_PK2 = ConfigHelper.GetAppConfig( PreKeyring.Node_UserKey_PK2 );
                    String UserKey_SK1 = ConfigHelper.GetAppConfig( PreKeyring.Node_UserKey_SK1 );
                    String UserKey_SK2 = ConfigHelper.GetAppConfig( PreKeyring.Node_UserKey_SK2 );
                    String UserKey_IsPublicized = ConfigHelper.GetAppConfig( PreKeyring.Node_UserKey_IsPublicized );

                    if ( String.IsNullOrEmpty( UserKey_PK1 ) || String.IsNullOrEmpty( UserKey_PK2 )||
                        String.IsNullOrEmpty( UserKey_SK1 )|| String.IsNullOrEmpty( UserKey_SK2 ) ||
                        String.IsNullOrEmpty( UserKey_IsPublicized ) )
                        return null;
                    else
                    {
                        _userKey = new UserKey();
                        _userKey.PK1 = UserKey_PK1;
                        _userKey.PK2 = UserKey_PK2;
                        _userKey.SK1 = UserKey_SK1;
                        _userKey.SK2 = UserKey_SK2;
                        _userKey.IsPublicized = Boolean.Parse( UserKey_IsPublicized );
                    }
                }
                return _userKey;
            }
            set 
            {
                if ( PreKeyring._userKey == null )
                    PreKeyring._userKey = new DB.UserKey();
                _userKey.PK1 = value.PK1;
                _userKey.PK2 = value.PK2;
                _userKey.SK1 = value.SK1;
                _userKey.SK2 = value.SK2;
                _userKey.IsPublicized = value.IsPublicized;

                ConfigHelper.UpdateAppConfig( PreKeyring.Node_UserKey_PK1 , value.PK1 );
                ConfigHelper.UpdateAppConfig( PreKeyring.Node_UserKey_PK2 , value.PK2);
                ConfigHelper.UpdateAppConfig( PreKeyring.Node_UserKey_SK1 , value.SK1 );
                ConfigHelper.UpdateAppConfig( PreKeyring.Node_UserKey_SK2 , value.SK2 );
                ConfigHelper.UpdateAppConfig( PreKeyring.Node_UserKey_IsPublicized , value.IsPublicized.ToString() );
            }
        }
        public static Boolean IsPublicized
        {
            get
            {
                return UserKey.IsPublicized;
            }
            set
            {
                PreKeyring.UserKey.IsPublicized = value;
                ConfigHelper.UpdateAppConfig( PreKeyring.Node_UserKey_IsPublicized , value.ToString() );
            }
        }
        #endregion 0.1.2

        #endregion AccessToken & UserKey

        #region 0.2 DataBase & Table
        private static SQLiteConnection conn;
        private static Type[] TableName = 
        {
            typeof( FileMetaData ),
            typeof( SharingInfo ) 
        };
        #endregion DataBase & Table

        #endregion Fields

        #region 1. Construction
        static PreKeyring()
        {
            try
            {
                PreKeyring.conn = new SQLiteConnection();

                SQLiteConnectionStringBuilder connStr = new SQLiteConnectionStringBuilder();
                connStr.DataSource = Config.PrekeyringFile_Path;
                //connStr.Password = Config.PrekeyringFile_Password;
                PreKeyring.conn.ConnectionString = connStr.ToString();

                if ( Directory.Exists( Config.PrekeyringFile_Dir ) == false )
                    Directory.CreateDirectory( Config.PrekeyringFile_Dir );
                if ( File.Exists( Config.PrekeyringFile_Path ) == false )
                {
                    SQLiteConnection.CreateFile( Config.PrekeyringFile_Path );
                    Boolean ret = CreateTablesByStructures( PreKeyring.TableName );
                }
                conn.Open();
            }
            catch ( SQLiteException ex )
            {
                SecuruStikException sex = new SecuruStikException( SecuruStikExceptionType.Init_Database,"SQLite - Create/Connect Failed." , ex );
                throw sex;
            }
            catch ( System.Exception ex )
            {
                throw ex;
            }
            finally
            {
                PreKeyring.conn.Close();
            }
        }
        public static void Close()
        {
            if ( PreKeyring.conn != null && PreKeyring.conn.State != ConnectionState.Closed )
                PreKeyring.conn.Close();
        }
        #endregion Construction

        #region 2. Create Tables according to structures
        private static String SQL_CreateTable( Type s )
        {
            String sql = "CREATE TABLE " + StructOpt.GetStructureName( s ) + "(";
            Dictionary<String , int> fileds = StructOpt.GetFieldInfo( s );
            Dictionary<String , int>.Enumerator enumerator = fileds.GetEnumerator();
            while ( enumerator.MoveNext() == true )
            {
                KeyValuePair<String , int> field = enumerator.Current;
                sql += string.Format( "{0} varchar({1})," , field.Key , field.Value );
            }
            return sql.Substring( 0 , sql.Length - 1 ) + ")";
        }
        /// <summary>
        /// Create the tables with struct's info.
        /// It doesn't need to be modified while modefying the structure
        /// </summary>
        private static void CreateTableByStructure( Type s )
        {
            try
            {
                PreKeyring.conn.Open();
                using ( SQLiteCommand cmd = new SQLiteCommand() )
                {
                    cmd.Connection = PreKeyring.conn;
                    cmd.CommandText = SQL_CreateTable( s );
                    cmd.ExecuteNonQuery();
                }
            }
            catch ( System.Exception ex )
            {
                throw ex;
            }
            finally
            {
                PreKeyring.conn.Close();
            }
        }
        private static Boolean CreateTablesByStructures( Type[] structs )
        {
            try
            {
                PreKeyring.conn.Open();
                foreach ( Type s in structs )
                {
                    using ( SQLiteCommand cmd = new SQLiteCommand() )
                    {
                        cmd.Connection = PreKeyring.conn;
                        cmd.CommandText = SQL_CreateTable( s );
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch ( System.Exception ex )
            {
                throw ex;
            }
            finally
            {
                PreKeyring.conn.Close();
            }
            return true;
        }

        #endregion Crate Tables

        #region 3. Data opts

        #region 3.1 FileInfo
        private static Boolean FileInfo_Insert( FileMetaData fi )
        {
            lock ( conn )
            {
                try
                {
                    if ( conn.State == ConnectionState.Closed ) conn.Open();
                    using ( SQLiteCommand cmd = new SQLiteCommand() )
                    {
                        cmd.Connection = PreKeyring.conn;
                        cmd.CommandText = string.Format( SQLStatement.FileInfo_Insert ,
                            fi.FileName ,
                            Path.GetFullPath( fi.FilePath ) ,
                            fi.Key ,
                            fi.PlainTextHash ,
                            fi.CryptTextHash );
                        cmd.ExecuteNonQuery();
                    }
                }
                catch ( System.Exception ){return false;}
            }
            return true;
        }
        private static int GetRowsCountOfFilePath( String filePath )
        {
            /// <summary>Just for table "FileInfo"</summary>
            int count = -1;
            String sql = string.Format( SQLStatement.FileInfo_Count , Path.GetFullPath(filePath) );
            lock ( conn )
            {
                try
                {
                    if(conn.State == ConnectionState.Closed)conn.Open();
                    using ( SQLiteCommand cmd = new SQLiteCommand() )
                    {
                        cmd.Connection = PreKeyring.conn;
                        cmd.CommandText = sql;
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        count = reader.GetInt32( 0 );
                        reader.Close();
                    }

                }
                catch ( System.Exception ex ) { return -2; }
            }
            return count;
        }
        public static Boolean FileInfo_Update( FileMetaData fi )
        {
            lock ( conn )
            {
                try
                {
                    if ( fi.Key == null ) return false;
                    fi.FilePath = Path.GetFullPath( fi.FilePath );
                    fi.Key = Base64String.StringToBase64String( fi.Key );

                    if ( PreKeyring.GetRowsCountOfFilePath( fi.FilePath ) == 0 )
                    {
                        FileInfo_Insert( fi );
                    }
                    else
                    {
                        String sql = string.Format( SQLStatement.FileInfo_Update ,
                            fi.FileName ,
                            fi.FilePath ,
                            fi.Key ,
                            fi.PlainTextHash ,
                            fi.CryptTextHash );
                        if ( conn.State == ConnectionState.Closed ) conn.Open();
                        using ( SQLiteCommand cmd = new SQLiteCommand() )
                        {
                            cmd.Connection = PreKeyring.conn;
                            cmd.CommandText = sql;
                            if ( cmd.ExecuteNonQuery() != 1 )
                                return false;
                        }
                    }
                }
                catch ( System.Exception ){}

            }
            return true;
        }
        public static Boolean FileInfo_Delete( String filePath )
        {
            lock ( conn )
            {
                try
                {
                    String fileFullPath = Path.GetFullPath( filePath );
                    if ( PreKeyring.GetRowsCountOfFilePath( fileFullPath ) == 0 )
                    {
                        return false;
                    }
                    else
                    {
                        String sql = string.Format( SQLStatement.FileInfo_Delete , fileFullPath );
                        conn.Open();
                        using ( SQLiteCommand cmd = new SQLiteCommand() )
                        {
                            cmd.Connection = PreKeyring.conn;
                            cmd.CommandText = sql;
                            if ( cmd.ExecuteNonQuery() != 1 )
                                return false;
                        }
                    }

                }
                catch ( System.Exception ){return false;}
            }
            
            return true;
        }
        public static void FileInfo_Delete( String[] filePaths )
        {
            lock ( conn )
            {
                if ( conn.State == ConnectionState.Closed ) conn.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = PreKeyring.conn;
                foreach ( String filePath in filePaths )
                {
                    try
                    {
                        String fileFullPath = Path.GetFullPath( filePath );
                        if ( PreKeyring.GetRowsCountOfFilePath( fileFullPath ) == 0 )
                        {
                            continue;
                        }
                        else
                        {
                            String sql = string.Format( SQLStatement.FileInfo_Delete , fileFullPath );
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                        }

                    }
                    catch ( System.Exception )
                    {
                        continue;
                    }
                }
                conn.Close();
            }
        }
        public static FileMetaData FileInfo_Query( String filePath )
        {
            FileMetaData fi = null;
            lock ( conn )
            {
                try
                {
                    if ( conn.State == ConnectionState.Closed ) conn.Open();
                    using ( SQLiteCommand cmd = new SQLiteCommand() )
                    {
                        cmd.Connection = PreKeyring.conn;
                        cmd.CommandText = string.Format( SQLStatement.FileInfo_Query , Path.GetFullPath( filePath ) );
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        Boolean canRead = reader.Read();
                        if ( canRead == false )
                            return null;
                        else
                        {
                            fi = new FileMetaData();
                            fi.FileName = reader.GetString( 0 );
                            fi.FilePath = reader.GetString( 1 );
                            fi.Key = Base64String.Base64StringToString( reader.GetString( 2 ) );
                            fi.PlainTextHash = reader.GetString( 3 );
                            fi.CryptTextHash = reader.GetString( 4 );
                        }
                    }
                }
                catch ( System.Exception ) { return null; }
            }
            return fi;
        }
        #endregion FileInfo

        #region 3.2 SharingFile
        private static Boolean SharingFile_Insert( SharingInfo sf )
        {
            lock ( conn )
            {
                try
                {
                    if ( conn.State == ConnectionState.Closed ) conn.Open();
                    using ( SQLiteCommand cmd = new SQLiteCommand() )
                    {
                        cmd.Connection = PreKeyring.conn;
                        cmd.CommandText = string.Format( SQLStatement.SharingFile_Insert ,
                            sf.CopyRef ,
                            sf.FileName ,
                            sf.ID_From ,
                            sf.CKEY_E ,
                            sf.CKEY_F ,
                            sf.CKEY_U ,
                            sf.CKEY_W );
                        cmd.ExecuteNonQuery();
                    }
                }
                catch ( System.Exception ){return false;}
            }
           
            return true;
        }
        private static int GetRowsCountOfCopyRef( String copyRef )
        {
            /// <summary>Just for table "FileInfo"</summary>
            int count = -1;
            String sql = string.Format( SQLStatement.SharingFile_Count ,copyRef );
            lock ( conn )
            {
                try
                {
                    if ( conn.State == ConnectionState.Closed ) conn.Open();
                    using ( SQLiteCommand cmd = new SQLiteCommand() )
                    {
                        cmd.Connection = PreKeyring.conn;
                        cmd.CommandText = sql;
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        count = reader.GetInt32( 0 );
                        reader.Close();
                    }
                }
                catch ( System.Exception ex ){return -2;}
            }
            
            return count;
        }
        public static Boolean SharingFile_Update( SharingInfo sf )
        {
            if ( PreKeyring.SharingFile_Query( sf.CopyRef ) == null )
            {
                SharingFile_Insert( sf );
            }
            else
            {
                lock ( PreKeyring.conn )
                {
                    try
                    {
                        String sql = string.Format( SQLStatement.SharingFile_Update ,
                            sf.CopyRef ,
                            sf.FileName ,
                            sf.ID_From ,
                            sf.CKEY_E ,
                            sf.CKEY_F ,
                            sf.CKEY_U ,
                            sf.CKEY_W );
                        if ( conn.State == ConnectionState.Closed ) conn.Open();
                        using ( SQLiteCommand cmd = new SQLiteCommand() )
                        {
                            cmd.Connection = PreKeyring.conn;
                            cmd.CommandText = sql;
                            if ( cmd.ExecuteNonQuery() != 1 )
                                return false;
                        }
                    }
                    catch ( System.Exception){return false;}
                }
                
            }
            return true;
        }
        public static Boolean SharingFile_Delete( String copyRef )
        {
            lock ( conn )
            {
                try
                {
                    if ( PreKeyring.GetRowsCountOfCopyRef( copyRef ) == 0 )
                    {
                        return false;
                    }
                    else
                    {
                        String sql = string.Format( SQLStatement.SharingFile_Delete , copyRef );
                        if ( conn.State == ConnectionState.Closed ) conn.Open();
                        using ( SQLiteCommand cmd = new SQLiteCommand() )
                        {
                            cmd.Connection = PreKeyring.conn;
                            cmd.CommandText = sql;
                            if ( cmd.ExecuteNonQuery() != 1 )
                                return false;
                        }
                    }

                }
                catch ( System.Exception ) { return false; }
            }
            
            return true;
        }
        public static void SharingFile_Delete( String[] copyRefs )
        {
            lock ( conn )
            {
                foreach ( String copyRef in copyRefs )
                {
                    try
                    {
                        if ( PreKeyring.GetRowsCountOfCopyRef( copyRef ) == 0 )
                        {
                            continue;
                        }
                        else
                        {
                            String sql = string.Format( SQLStatement.SharingFile_Delete , copyRef );
                            if ( conn.State == ConnectionState.Closed ) conn.Open();
                            using ( SQLiteCommand cmd = new SQLiteCommand() )
                            {
                                cmd.Connection = PreKeyring.conn;
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }
                        }

                    }
                    catch ( System.Exception ) { continue; }
                }
                
            }
        }
        public static SharingInfo SharingFile_Query( String copyRef )
        {
            SharingInfo sf = null;
            //取出数据
            lock ( conn )
            {
                try
                {
                    if ( conn.State == ConnectionState.Closed ) conn.Open();
                    using ( SQLiteCommand cmd = new SQLiteCommand() )
                    {
                        cmd.Connection = PreKeyring.conn;
                        cmd.CommandText = string.Format( SQLStatement.SharingFile_Query , copyRef );
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        Boolean canRead = reader.Read();
                        if ( canRead == false )
                            return sf;
                        else
                        {
                            sf = new SharingInfo();
                            sf.CopyRef = reader.GetString( 0 );
                            sf.FileName = reader.GetString( 1 );
                            sf.ID_From = reader.GetString( 2 );

                            sf.CKEY_E = reader.GetString( 3 );
                            sf.CKEY_F = reader.GetString( 4 );
                            sf.CKEY_U = reader.GetString( 5 );
                            sf.CKEY_W = reader.GetString( 6 );
                        }
                    }
                }
                catch ( System.Exception ex ){ return null; }
            }
            
            return sf;
        }
        public static List<SharingInfo> GetSharingFileList()
        {
            List<SharingInfo> sharingList = null;
            lock ( conn )
            {
                try
                {
                    if ( conn.State == ConnectionState.Closed ) conn.Open();
                    using ( SQLiteCommand cmd = new SQLiteCommand() )
                    {
                        cmd.Connection = PreKeyring.conn;
                        cmd.CommandText = string.Format( SQLStatement.SharingFile_All );
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        Boolean canRead;
                        sharingList = new List<SharingInfo>();
                        while ( ( canRead = reader.Read() ) == true )
                        {
                            SharingInfo sf = new SharingInfo();
                            sf.CopyRef = reader.GetString( 0 );
                            sf.FileName = reader.GetString( 1 );
                            sf.ID_From = reader.GetString( 2 );
                            sf.CKEY_E = reader.GetString( 3 );
                            sf.CKEY_F = reader.GetString( 4 );
                            sf.CKEY_U = reader.GetString( 5 );
                            sf.CKEY_W = reader.GetString( 6 );
                            sharingList.Add( sf );
                        }
                    }
                    return sharingList;
                }
                catch ( System.Exception ){return null;}
            }
            
        }
        #endregion SharingFile

        #endregion Data opts

        #region 4. Others
        /// <summary> Get the count of rows of the table</summary>
        private static int GetRowsCount( Type s )
        {
            int count = -1;
            String tableName = StructOpt.GetStructureName( s );
            String sql = string.Format( "SELECT COUNT(*) FROM {0}" , tableName );
            try
            {
                if ( conn.State == ConnectionState.Closed ) conn.Open();
                using ( SQLiteCommand cmd = new SQLiteCommand() )
                {
                    cmd.Connection = PreKeyring.conn;
                    cmd.CommandText = sql;
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    count = reader.GetInt32( 0 );
                    reader.Close();
                }
            }
            catch ( System.Exception ex )
            {
                Console.WriteLine( ex.Message );
                return -1;
            }
            return count;
        }

        public static void DeletePreKeyFile()
        {
            File.Delete( Config.PrekeyringFile_Path );
        }

        #endregion Others
    }
}
