/***********************************************************************
 * CLR Version :      $civersion$
 * Class Name  :      SharingInfo
 * Name Space  :      Server
 * File Name   :      SharingInfo
 * Create Time :      2014/11/22 8:21:53
 * Author      :       JianYe Huang
 * Brief       :      Manage the sharing information
 * Modification history :  NULL
 ***********************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using DropNet.Models;
using SecuruStik.Protocal;
using DropNet.Exceptions;
using DropNet;

namespace Server
{
    public class SIM
    {
        private DropNet.DropNetClient Client;
        public SIM( DropNetClient client )
        {
            this.Client = client;
        }

        /// <summary>
        /// Upload the sharing-info file to the server
        /// </summary>
        /// <param pathFullName="newSharingInfo">From.To.Filename.URL.ReKey</param>
        /// <returns></returns>
        public Boolean Upload( SharingInfo newSharingInfo )
        {            
            String sharingInfoFileDir = String.Format( "/{0}" , newSharingInfo.ID_TO );
            String sharingInfoFileName = String.Format( "{0}.txt" , newSharingInfo.Reference );
            String sharingInfoFilePath = String.Format( "{0}/{1}" , sharingInfoFileDir , sharingInfoFileName );
            try
            {
                //this.Client.Delete( sharingInfoFilePath );
                Client.Delete( sharingInfoFilePath );
            }
            catch ( System.Exception ) { }
            try
            {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bfer = new BinaryFormatter();
                bfer.Serialize( ms , newSharingInfo );
                Client.UploadFileAsync( sharingInfoFileDir , sharingInfoFileName , ms.ToArray() ,
                            ( response ) =>{} ,
                            ( error ) =>
                            {
                                //SecuruStikException.ThrowSecuruStik(
                                //                  SecuruStikExceptionType.DropBoxControl_Upload , "UpLoadError" , error );
                            } );
                return true;
            }
            catch ( System.Exception  ){return false;}
        }
        /// <summary>
        /// Download user's all the sharing-info files
        /// </summary>
        /// <param pathFullName="userID">user's dropbox id</param>
        /// <returns></returns>
        public List<SharingInfo> Download( String userID )
        {
            String sharingFileDir = String.Format( "/{0}" , userID );
            StringBuilder sharingFilePath = new StringBuilder();
            MetaData metadatas = this.Client.GetMetaData( sharingFileDir );
            List<SharingInfo> sharingInfoList = new List<SharingInfo>();
            if ( metadatas != null )
            {
                foreach ( MetaData sharingFile in metadatas.Contents )
                {
                    if ( sharingFile.Name == PKM.PKFileName )
                        continue;
                    sharingFilePath.Clear();
                    String sharingFileDropboxPath = sharingFilePath.AppendFormat( "{0}/{1}" , sharingFileDir , sharingFile.Name ).ToString();
                    try
                    {
                        Byte[] sharingInfoBytes;// = this.Client.Download( sharingFileDropboxPath );
                        MetaData files = this.Client.GetMetaData( sharingFileDropboxPath );
                        if ( files == null || files.Is_Dir == true ||
                                files.Is_Deleted || files.Bytes == 0 )
                        {
                            sharingInfoBytes = null;
                        }
                        else
                        {
                            sharingInfoBytes = Client.GetFile( sharingFileDropboxPath );
                            MemoryStream ms = new MemoryStream( sharingInfoBytes );
                            BinaryFormatter bfer = new BinaryFormatter();
                            sharingInfoList.Add( (SecuruStik.Protocal.SharingInfo)bfer.Deserialize( ms ) );

                            Client.Delete( sharingFileDropboxPath );
                        }
                    }
                    catch ( System.Exception ) { }
                }
            }
            return sharingInfoList;
        }
    }
}
