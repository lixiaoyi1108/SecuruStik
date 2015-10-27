/***********************************************************************
 * CLR Version :      4.0.30319.18444
 * Class Name  :      MetaData
 * Name Space  :      Model
 * File Name   :      MetaData
 * Create Time :      2014/10/17 16:01:15
 * Author      :      JianYe,Huang
 * Brief       :      Define the structures of the database
 ***********************************************************************/
using System;
using SecuruStik.DB.Base;

namespace SecuruStik.DB
{

    #region 1. Definitions

    //Emabled in application
    public class AccessToken
    {
        public String UserToken;
        public String UserSecret;
    }

    public class UserKey
    {
        public String PK1;
        public String PK2;
        public String SK1;
        public String SK2;
        public Boolean IsPublicized;
    }

    //Store in the database
    public class FileMetaData
    {
        [Field( MaxSize = 300 )]public String FileName;
        [Field( MaxSize = 300 )]public String FilePath;
        [Field( MaxSize = 100 )]public String Key;
        [Field( MaxSize = 100 )]public String PlainTextHash;
        [Field( MaxSize = 100 )]public String CryptTextHash;
    }

    public class SharingInfo
    {
        [Field( MaxSize = 300 )]public String CopyRef;
        [Field( MaxSize = 300 )]public String FileName;
        [Field( MaxSize = 100 )]public String ID_From;

        [Field( MaxSize = 200 )]public String CKEY_E;
        [Field( MaxSize = 200 )]public String CKEY_F;
        [Field( MaxSize = 200 )]public String CKEY_U;
        [Field( MaxSize = 200 )]public String CKEY_W;
    }

    #endregion Definitions

    #region 2. SQL statement

    /// <summary> Note: It needs to be modified only when the fields are changed.</summary>
    internal class SQLStatement
    {
        // TODO: WTF, not using parameters?
        #region 2.1 FileMetaData
        public static String FileInfo_Insert
        {
            get
            {
                String tableName = StructOpt.GetStructureName( typeof( FileMetaData ) );
                return "INSERT INTO " + tableName + " VALUES('{0}','{1}','{2}','{3}','{4}')";
            }
        }
        public static String FileInfo_Update
        {
            get
            {
                String tableName = StructOpt.GetStructureName( typeof( FileMetaData ) );
                return "UPDATE " + tableName + " SET FileName ='{0}',Key='{2}',PlainTextHash='{3}',CryptTextHash='{4}' WHERE FilePath='{1}'";
            }
        }
        public static String FileInfo_Delete
        {
            get
            {
                String tableName = StructOpt.GetStructureName( typeof( FileMetaData ) );
                return "DELETE FROM " + tableName + " WHERE FilePath='{0}'";
            }
        }
        public static String FileInfo_Query
        {
            get
            {
                String tableName = StructOpt.GetStructureName( typeof( FileMetaData ) );
                return "SELECT * FROM "+tableName+" WHERE FilePath='{0}'" ;
            }
        }
        public static String FileInfo_Count
        {
            get
            {
                String tableName = StructOpt.GetStructureName( typeof( FileMetaData ) );
                return "SELECT COUNT(*) FROM " + tableName + " WHERE FilePath='{0}'";
            }
        }
        #endregion FileMetaData
        
        #region 2.2 SharingFile
        public static String SharingFile_Insert
        {
            get
            {
                String tableName = StructOpt.GetStructureName( typeof( SharingInfo ) );

                return "INSERT INTO " + tableName + " VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";
            }
        }
        public static String SharingFile_Update
        {
            get
            {
                String tableName = StructOpt.GetStructureName( typeof( SharingInfo ) );
                return "UPDATE " + tableName +
                    " SET FileName ='{1}',ID_From='{2}',CKEY_E='{3}',CKEY_F='{4}',CKEY_U='{5}',CKEY_W='{6}' WHERE CopyRef='{0}'";
            }
        }
        public static String SharingFile_Delete
        {
            get
            {
                String tableName = StructOpt.GetStructureName( typeof( SharingInfo ) );
                return "DELETE FROM " + tableName +
                    " WHERE CopyRef='{0}'";
            }
        }
        public static String SharingFile_Query
        {
            get
            {
                String tableName = StructOpt.GetStructureName( typeof( SharingInfo ) );
                return "SELECT * FROM " + tableName + " WHERE CopyRef='{0}'";
            }
        }
        public static String SharingFile_Count
        {
            get
            {
                String tableName = StructOpt.GetStructureName( typeof( SharingInfo ) );
                return "SELECT COUNT(*) FROM " + tableName + " WHERE CopyRef='{0}'";
            }
        }
        public static String SharingFile_All
        {
            get
            {
                String tableName = StructOpt.GetStructureName( typeof( SharingInfo ) );
                return "SELECT * FROM "+tableName;
            }
        }
        #endregion
    }
    #endregion SQL statement

    #region 3. Attribute for recording the size of the field

    [System.AttributeUsage( AttributeTargets.Field )]
    internal class Field: System.Attribute
    {
        public int MaxSize;
        public Field()
        {
            this.MaxSize = 20;
        }
        public Field( int size )
        {
            this.MaxSize = size;
        }
    }
    #endregion
}
