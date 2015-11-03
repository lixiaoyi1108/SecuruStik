/***********************************************************************
 * CLR Version :      $civersion$
 * Class Name  :      PKM
 * Name Space  :      Server
 * File Name   :      PKM
 * Create Time :      2014/11/22 7:47:54
 * Author      :      JianYe Huang
 * Brief       :      Public key management
 * Modification history : 
 ***********************************************************************/
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Server.DropBox;
using SecuruStik.Protocal;
using DropNet;
using DropNet.Models;
using DropNet.Exceptions;

namespace Server
{
    public class PKM
    {
        public static readonly String PKFileName = "pubkey.txt";
        private DropNetClient Client;
        public PKM( DropNetClient client )
        {
            this.Client = client;
        }

        public Boolean Update(String user,Byte[] newPK)
        {
            String pkFileDir_Dropbox = String.Format("/{0}",user);
            String pkFilePath_Dropbox = String.Format("/{0}/{1}", user,PKFileName);
            try
            {
                this.Client.Delete( pkFilePath_Dropbox );
            }
            catch ( System.Exception  ) { }
            try
            {
                    this.Client.UploadFileAsync( pkFileDir_Dropbox , PKFileName , newPK ,
                                ( response ) =>
                                {
                                } ,
                                ( error ) =>
                                {
                                    //SecuruStikException.ThrowSecuruStik(
                                    //                  SecuruStikExceptionType.DropBoxControl_Upload , "UpLoadError" , error );
                                } );
             }catch ( System.Exception ex )
                {
                    //SecuruStikException.ThrowSecuruStik(
                    //                  SecuruStikExceptionType.DropBoxControl_Upload , "UpLoadError" , ex );
                    return false;
             }
             return true;
        }
        public Boolean Update( String user , PublicKey newPK )
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                String pkStr = String.Format( "{0}\n{1}" ,newPK.PK1,newPK.PK2);
                Byte[] pkByte = Encoding.ASCII.GetBytes( pkStr );
                return Update( user , pkByte );
            }
            catch ( System.Exception ex )
            {
                Console.WriteLine( ex.Message );
                return false;
            }
        }

        public Boolean Download( String user , ref Byte[] pkBytes )
        {
            String pkFilePath_Dropbox = String.Format( "/{0}/{1}" , user , PKFileName );
            pkBytes = null;
            try
            {
                MetaData file = this.Client.GetMetaData( pkFilePath_Dropbox, false, false );
                if ( file == null ||
                    file.Is_Dir == true ||
                    file.Is_Deleted ||
                    file.Bytes == 0 )
                {
                    pkBytes = null;
                }
                else
                {
                    pkBytes = this.Client.GetFile( pkFilePath_Dropbox );
                }
            } catch ( DropboxException )
            {
                return false;
            }
            return true;
        }
        public Boolean Download( String user , ref SecuruStik.Protocal.PublicKey pk )
        {
            Byte[] pkBytes = null;
            Boolean ret = Download( user , ref pkBytes );

            if ( pkBytes != null )
            {
                try
                {
                    String pkStr = Encoding.ASCII.GetString( pkBytes );
                    if ( this.IsOldVersion( pkStr ) )
                    {
                        //throw new Exception( "Old version" );
                        //Old version
                        MemoryStream ms = new MemoryStream( pkBytes );
                        BinaryFormatter bfer = new BinaryFormatter();
                        pk = (SecuruStik.Protocal.PublicKey)bfer.Deserialize( ms );

                        //Update
                        this.Update( user , pk );
                        return true;
                    }
                    else
                    {
                        String[] param = pkStr.Split( '\n' );
                        pk = new PublicKey( user , param[ 0 ] , param[ 1 ] );

                        return true;
                    }
                } catch ( System.Exception ){}
            }
            return false;
        }

        private Boolean IsOldVersion(String pkStr)
        {
            if ( pkStr.Contains( "CommunicatingProtocal" ) )
                return true;
            else return false;
        }
    }
}
