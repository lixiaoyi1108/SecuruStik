using System;
using System.Collections.Generic;


namespace SecuruStik.DB
{
    class Program
    {
        static void Main( string[] args )
        {

            FileMetaData fi = PreKeyring.FileInfo_Query(@"C:\Users\637\Documents\SecuruStik\Home\RestartExplorer.bat");
            Console.WriteLine(
                ( fi == null ) ? "The File HHHHH is not exist!!" :
                fi.FileName + "  " +
                fi.FilePath + "  " +
                fi.Key + "  " +
                fi.PlainTextHash + "  " +
                fi.CryptTextHash );
            Console.ReadLine();
        }
        public static void PREKEYRING_TEST()
        {
            PreKeyring.AccessToken = new AccessToken { UserToken = "Hello" , UserSecret = "World" };
            AccessToken at = PreKeyring.AccessToken;
            Console.WriteLine( at.UserToken + "  " + at.UserSecret );
            PreKeyring.AccessToken = new AccessToken { UserToken = "Good" , UserSecret = "morning" };
            at = PreKeyring.AccessToken;
            Console.WriteLine( at.UserToken + "  " + at.UserSecret );

            PreKeyring.UserKey = new UserKey { PK1 = "123" , PK2 = "456" , SK1 = "789" , SK2 = "abc" };
            UserKey uk = PreKeyring.UserKey;
            Console.WriteLine( uk.PK1 + "  " + uk.PK2 + "  " + uk.SK1 + "  " + uk.SK2 );
            PreKeyring.UserKey = new UserKey { PK1 = "abc" , PK2 = "def" , SK1 = "ghi" , SK2 = "jkl" };
            uk = PreKeyring.UserKey;
            Console.WriteLine( uk.PK1 + "  " + uk.PK2 + "  " + uk.SK1 + "  " + uk.SK2 );

        }
        public static void FILEINFO_TEST()
        {
            PreKeyring.FileInfo_Update( new FileMetaData { FileName = "HJY" , FilePath = "SCAU" , Key = "19920625" , PlainTextHash = "12345" , CryptTextHash = "67890" } );
            FileMetaData fi = PreKeyring.FileInfo_Query( "SCAU" );
            Console.WriteLine(
                fi.FileName + "  " +
                fi.FilePath + "  " +
                fi.Key + "  " +
                fi.PlainTextHash + "  " +
                fi.CryptTextHash );
            PreKeyring.FileInfo_Update( new FileMetaData { FileName = "HJY2" , FilePath = "SCAU2" , Key = "19920625" , PlainTextHash = "12345" , CryptTextHash = "67890" } );
            PreKeyring.FileInfo_Update( new FileMetaData { FileName = "HJY2" , FilePath = "SCAU3" , Key = "19920625" , PlainTextHash = "12345" , CryptTextHash = "67890" } );

            PreKeyring.FileInfo_Update( new FileMetaData { FileName = "DLC" , FilePath = "SCAU" , Key = "19920625" , PlainTextHash = "12345" , CryptTextHash = "67890" } );
            fi = PreKeyring.FileInfo_Query( "HHHHH" );
            Console.WriteLine(
                ( fi == null ) ? "The File HHHHH is not exist!!" :
                fi.FileName + "  " +
                fi.FilePath + "  " +
                fi.Key + "  " +
                fi.PlainTextHash + "  " +
                fi.CryptTextHash );

            //PreKeyring.FileInfo_Delete( new String[]{"SCAU2","SCAU3"} );
            fi = PreKeyring.FileInfo_Query( "SCAU2" );
            Console.WriteLine(
                ( fi == null ) ? "The File SCAU2 is not exist!!" :
                fi.FileName + "  " +
                fi.FilePath + "  " +
                fi.Key + "  " +
                fi.PlainTextHash + "  " +
                fi.CryptTextHash );
            fi = PreKeyring.FileInfo_Query( "SCAU3" );
            Console.WriteLine(
                ( fi == null ) ? "The File SCAU3 is not exist!!" :
                fi.FileName + "  " +
                fi.FilePath + "  " +
                fi.Key + "  " +
                fi.PlainTextHash + "  " +
                fi.CryptTextHash );

            PreKeyring.Close();
        }
        public static void SharingFile_TEST()
        {
            PreKeyring.SharingFile_Update( 
                new SharingInfo 
                { 
                    CopyRef = "ref1" , 
                    FileName = "file" , 
                    ID_From = "Hjnubys",
                    CKEY_E = "testE",
                    CKEY_F = "testF" ,
                    CKEY_U = "testU" ,
                    CKEY_W = "testW"
                } );
            PreKeyring.SharingFile_Update( new SharingInfo
            {
                CopyRef = "ref2" ,
                FileName = "file2" ,
                ID_From = "Hjnubys" ,
                CKEY_E = "testE" ,
                CKEY_F = "testF" ,
                CKEY_U = "testU" ,
                CKEY_W = "testW"
            } ); PreKeyring.SharingFile_Update( new SharingInfo
            {
                CopyRef = "ref3" ,
                FileName = "file2" ,
                ID_From = "Hjnubys" ,
                CKEY_E = "testE" ,
                CKEY_F = "testF" ,
                CKEY_U = "testU" ,
                CKEY_W = "testW"
            } );
            SharingInfo sf = PreKeyring.SharingFile_Query( "ref1" );
            Console.WriteLine(
                ( sf == null ) ? "The SharingFileInfo is not exist" :
                "Find the file info of the copyref with ref" );
            PreKeyring.SharingFile_Delete( new String[]{"ref1","ref3"} );
            List<SharingInfo> sharingfileList = PreKeyring.GetSharingFileList();
            foreach ( SharingInfo f in sharingfileList )
            {
                Console.WriteLine( f.CopyRef + "   " +
                f.FileName + "  " +
                f.ID_From +"   "+
                f.CKEY_E+"   "+
                f.CKEY_F+"   "+
                f.CKEY_U+"   "+
                f.CKEY_W+"   ");
            }
        }
    }
}
