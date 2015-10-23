using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SecuruStik.Protocal;
using SecuruStik.PRE;
using SecuruStik.DB;
using System.IO;
using SecuruStik.BaseExtension;
using SecuruStik.DropBox;
using SecuruStik.Opt;
using SecuruStik.MessageQueue;
using UserSetting.Key;

namespace SecuruStik.Contorllers
{
    public class ShareTaskWorker
    {
        public List<ShareTaskUnit> ShareTaskWaitingList = new List<ShareTaskUnit>();
        public List<ShareTaskUnit> ShareTaskSuspendList = new List<ShareTaskUnit>();
        private BackgroundWorker ShareWorker ;
        private String _Email = String.Empty;
        private String Email
        {
            get
            {
                if ( String.IsNullOrEmpty( this._Email ) )
                {
                    this._Email = this.DropboxController.Email;
                }
                return this._Email;
            }
            set
            {
                this._Email = value;
            }
        }
        private DropBoxController DropboxController;
        private ProxyServerController ProxyServerController;
        public ShareTaskWorker( DropBoxController dropboxController,ProxyServerController proxyServerController)
        {
            this.DropboxController = dropboxController;
            this.ProxyServerController = proxyServerController;

            this.ShareWorker = new BackgroundWorker();
            this.ShareWorker.DoWork += new DoWorkEventHandler( ShareWorker_DoWork );
            this.ShareWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler( ShareWorker_RunWorkerCompleted );
            this.ShareWorker.WorkerReportsProgress = true;
            this.ShareWorker.WorkerSupportsCancellation = true;
        }
        private void ShareWorker_DoWork( object sender , DoWorkEventArgs e )
        {
            while( this.ShareTaskWaitingList.Count != 0 )
            {
                for ( int i = this.ShareTaskWaitingList.Count - 1 ; i >= 0 ; i-- )
                {
                    ShareTaskUnit worker = this.ShareTaskWaitingList[i];
                    PublicKey pubkey = worker.PK;
                    //Key1
                    PRE_KEY key = ProxyReEncryption.GenKey(
                        PreKeyring.UserKey.PK1 , 
                        PreKeyring.UserKey.PK2 , 
                        PreKeyring.UserKey.SK1 , 
                        PreKeyring.UserKey.SK2 );

                    //Key_pk2
                    PRE_PK key2_pk = ProxyReEncryption.GenPK(pubkey.PK1,pubkey.PK2);

                    //ReKey
                    PRE_Cipher reKey = ProxyReEncryption.KeyEncrypt( key , key2_pk , worker.Key );

                    String cpyRef = this.DropboxController.GetCopyRef(
                        this.DropboxController.SecuruStikFolder2RemoteDropboxPath( worker.FilePath ) );
                    if ( String.IsNullOrEmpty( cpyRef ) )
                        continue;//获取copyref失败处理
                    else
                    {
                        worker.CpyRef = cpyRef;
                    }

                    SecuruStik.Protocal.SharingInfo si = new SecuruStik.Protocal.SharingInfo(
                        this.Email ,
                        pubkey.ID ,
                        Path.GetFileName( worker.FilePath ) ,
                        worker.CpyRef ,
                        reKey.E,reKey.F,reKey.U,reKey.W );

                    Request r = new Request( MessageType.Request_SetSharingInfo , this.DropboxController.Email , si );
                    this.ProxyServerController.Send( r );
                    this.ShareTaskWaitingList.Remove( worker );
                    SecuruStikMessageQueue.SendMessage_Share_End( Path.GetFileName( worker.FilePath ) , worker.ID_TO );
                }
            }
        }
        private void ShareWorker_RunWorkerCompleted( object sender , RunWorkerCompletedEventArgs e )
        {
        }

        private Boolean IsPKExist(String ID_TO)
        {
            PublicKeyUnit pku = this.ProxyServerController.PKList[ ID_TO ];
            if ( pku == null )
            {
                this.ProxyServerController.Request_GetPK( ID_TO );
                return false;
            }
            else if ( pku.PK == null )
                return false;
            else
                return true;
        }
        private PublicKey GetRequestedPK( String ID_TO )
        {
            PublicKeyUnit pku = this.ProxyServerController.PKList[ID_TO];

            if (pku != null)
                return pku.PK;

            return null;
        }

        public Boolean AddShareTask( String id_to , String filePath )
        {
            if ( System.IO.File.Exists( filePath ) == false )
                return false;
            FileMetaData fmd = PreKeyring.FileInfo_Query( filePath );
            if ( fmd == null )
                return false;

            ShareTaskUnit shareUnit = new ShareTaskUnit( id_to , filePath , fmd.Key );

            if ( !this.IsPKExist( shareUnit.ID_TO ) )
                this.ShareTaskSuspendList.Add( shareUnit );
            else
            {
                shareUnit.PK = GetRequestedPK( shareUnit.ID_TO );
                this.ShareTaskWaitingList.Add( shareUnit );
            }

            if ( !this.ShareWorker.IsBusy )
            {
                this.ShareWorker.RunWorkerAsync();
            }
            return true;
        }
        public void ReceivePKInfo( PublicKey pubkey )
        {
            for ( int i = this.ShareTaskSuspendList.Count - 1 ; i >= 0 ; i-- )
            {
                ShareTaskUnit shareTaskWorker = this.ShareTaskSuspendList[ i ];
                if(shareTaskWorker.ID_TO == pubkey.ID)
                {
                    shareTaskWorker.PK = pubkey;
                    this.ShareTaskSuspendList.Remove( shareTaskWorker );
                    this.ShareTaskWaitingList.Add( shareTaskWorker );
                }
            }
            if ( !this.ShareWorker.IsBusy )
            {
                this.ShareWorker.RunWorkerAsync();
            }
        }
    }
}