/***********************************************************************
 * CLR Version :      $civersion$
 * Class Name  :      ProxyServerControler
 * Name Space  :      DBoxPRE.Opt
 * File Name   :      ProxyServerControler
 * Create Time :      2014/11/24 10:08:55
 * Author      :      JianYe Huang
 * Brief       :      Communicate with proxy server.
 * Modification history : 
 ***********************************************************************/
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using SecuruStik.Contorllers;
using SecuruStik.DB;
using SecuruStik.DropBox;
using SecuruStik.MessageQueue;
using SecuruStik.Protocal;
using SecuruStikSettings;

namespace SecuruStik.Opt
{
    public class ProxyServerController
    {
        private Boolean IsInited;
        private Boolean IsLogin;
        private TcpClient client;
        private BinaryReader br;
        private BinaryWriter bw;

        private String UserEmail;
        public PublicKeyList PKList;
        
        public ProxyServerController(String userEmail)
        {
            this.IsInited = false;
            this.IsLogin = false;
            this.UserEmail = userEmail;
            this.PKList = new PublicKeyList();    
        }
        delegate void ReceiveThread();
        static ManualResetEvent signal_sharefile = new ManualResetEvent( true );
        ReceiveThread Supend_ReceiveThread = delegate() { signal_sharefile.WaitOne(); signal_sharefile.Reset(); };
        ReceiveThread Resume_ReceiveThread = delegate() { signal_sharefile.Set(); };

        private void Init()
        {
            this.IsInited = false;

            client = new TcpClient();
            client.Connect( new IPEndPoint(
                    IPAddress.Parse( Properties.Settings.Default.REMOTE_IP ) ,
                                     Properties.Settings.Default.PORT )
                    );
            NetworkStream ns = client.GetStream();
            this.br = new BinaryReader( ns );
            this.bw = new BinaryWriter( ns );
            Thread receiveThread = new Thread( Receive );
            receiveThread.IsBackground = true;
            receiveThread.Start();
            Resume_ReceiveThread();

            this.IsInited = true;
        }
        public void UnInit()
        {
            this.Logout();
        }
        private void Login()
        {
            SecuruStik.Protocal.Request r = new SecuruStik.Protocal.Request(
                MessageType.Request_Login , this.UserEmail );

            byte[] packetBytes = CommunicatingProtocal.Serialize( r );
            this.bw.Write( packetBytes.Length );
            this.bw.Write( packetBytes );
            this.bw.Flush();

            this.IsLogin = true;
        }
        private void Logout()
        {
            SecuruStik.Protocal.Request r = new SecuruStik.Protocal.Request(
                                MessageType.Request_Logout ,
                                this.UserEmail );
            this.Send( r );
            this.IsInited = false;
        }
        public Boolean Send( SecuruStik.Protocal.Packet packet )
        {
            try
            {
                if ( this.IsInited == false )
                {
                    this.Init();
                }
                if ( this.IsLogin == false )
                {
                    this.Login();
                }
                byte[] packetBytes = CommunicatingProtocal.Serialize(packet);
                this.bw.Write(packetBytes.Length);
                this.bw.Write(packetBytes);
                this.bw.Flush();
                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show( new Form { TopMost = true , StartPosition = FormStartPosition.CenterScreen } , 
                    "Disconnect with server.\r\nPlease check your network setting or firewall." , ""
                    , MessageBoxButtons.OK , MessageBoxIcon.Error ); 
                return false;
            }
        }
        public Boolean Request_GetPK( String UserID )
        {
            try
            {
                if (this.PKList.Contains(UserID) == false)
                {
                    this.PKList.Add(UserID);
                    SecuruStik.Protocal.Request r = new SecuruStik.Protocal.Request(MessageType.Request_GetPK, UserID);
                    return this.Send(r);
                }
                else
                    return true;
            }
            catch (System.Exception) { return false; }
        }
        public Boolean Request_GetSharingInfo()
        {
            SecuruStik.Protocal.Request r = new SecuruStik.Protocal.Request( MessageType.Request_GetSharingInfo , this.UserEmail );
            return this.Send( r );
        }
        public Boolean Request_SetPK( SecuruStik.Protocal.PublicKey pk )
        {
            SecuruStik.Protocal.Request r = new SecuruStik.Protocal.Request( MessageType.Request_SetPK , this.UserEmail , pk );
            return this.Send( r );
        }
        public Boolean Request_SetSharingInfo( SecuruStik.Protocal.SharingInfo sharingInfo )
        {
            SecuruStik.Protocal.Request r = new SecuruStik.Protocal.Request(MessageType.Response_SetSharingInfo,this.UserEmail,sharingInfo);
            return this.Send( r );
        }

        public void Receive()
        {
            while ( true )
            {
                try
                {
                    int packetSize = this.br.ReadInt32();
                    byte[] packetBytes = this.br.ReadBytes( packetSize );
                    DealReceivedData( packetBytes );
                    if ( this.IsInited == false )
                    {
                        Supend_ReceiveThread();
                    }

                }
                catch ( System.Exception ex)
                {
                    if ( this.IsLogin == true )
                    {
                        SecuruStikMessageQueue.SendMEssage_Disconnect();
                        this.IsInited = false;
                        this.IsLogin = false;
                        Supend_ReceiveThread();
                    }
                    else
                        break;
                }
            }
        }
        private void DealReceivedData( Byte[] packetBytes )
        {
            Packet packet = (Packet)CommunicatingProtocal.ExplainPacket( packetBytes );
            Response response = (Response)packet;
            if ( response.IsSuccess == false )
                this.DealReceiveError( response );
            else
            {
                switch ( response.Type )
                {
                    case MessageType.Request_Login:
                        this.IsInited = true;Resume_ReceiveThread();break;
                    case MessageType.Response_GetPK:
                        this.DealReceivePK( response.ResponseContent ); break;
                    case MessageType.Response_GetSharingInfo:
                        this.DealReceiveSharingInfo( response.ResponseContent ); break;
                }
            }
        }
        private void DealReceiveError( Response response )
        {
            String ErrorStr = null;
            switch ( response.Type )
            {
                case MessageType.Response_GetPK:
                    String id = ( (SecuruStik.Protocal.PublicKey)response.ResponseContent ).ID;
                    ErrorStr = String.Format( "Failed to get the 【{0}】's public key." , id );
                    this.PKList.Remove(id);
                    break;
                case MessageType.Response_SetPK: ErrorStr = "Failed to Update the public key."; break;
                case MessageType.Response_GetSharingInfo: ErrorStr = "Remote server is busy.Please try again."; break;
                case MessageType.Response_SetSharingInfo: ErrorStr = "Failed to Share the file."; break;
            }
            MessageBoxEx.Show(
                new Form { TopMost = true },
                ErrorStr , "Sorry!" ,
                System.Windows.Forms.MessageBoxButtons.OK ,
                System.Windows.Forms.MessageBoxIcon.Error );
            return;
        }
        private void DealReceivePK( RequestContent ResponseContent )
        {
            SecuruStik.Protocal.PublicKey receivepk = ResponseContent as SecuruStik.Protocal.PublicKey;
            if ( receivepk != null )
            {
                PublicKeyUnit pku = PKList[receivepk.ID];
                if ( pku == null )
                {
                    this.PKList.Add(new PublicKeyUnit(receivepk.ID, receivepk));
                }
                else
                {
                    PublicKey pk = pku.PK;
                    if ( pk == null || pk.PK1 != receivepk.PK1 || pk.PK2 != receivepk.PK2 )
                    {
                        pku.PK = receivepk;
                    }
                }
                SecuruStikMessageQueue.SendMessage_ReceivePK( pku.ID , pku.PK.PK1 , pku.PK.PK2 );
            }
            return;
        }
        private void DealReceiveSharingInfo( RequestContent ResponseContent )
        {
            SecuruStik.Protocal.SharingInfo si = ResponseContent as SecuruStik.Protocal.SharingInfo;
            if ( si != null )
            {
                try
                {
                    PreKeyring.SharingFile_Update(
                             new DB.SharingInfo
                             {
                                 CopyRef = si.Reference ,
                                 FileName = si.FileName ,
                                 ID_From = si.ID_From ,
                                 CKEY_E = si.CKey_E ,
                                 CKEY_F = si.CKey_F ,
                                 CKEY_U = si.CKey_U ,
                                 CKEY_W = si.CKey_W
                             } );

                    String savePath = Path.Combine( DropBoxController.Local_ShareInfoFolder ,
                        String.Format( "[{1}]{0}.{2}" , si.FileName , si.ID_From  , AppSetting.Name_ShareInfoFileExtension )
                        );
                    Byte[] sharingInfoBytes = CommunicatingProtocal.Serialize( si );
                    File.WriteAllBytes( BaseExtension.FileStringHelper.GetNonConflictFileName(savePath) , sharingInfoBytes );
                }
                catch (System.Exception ex){}
            }
        }
    }
}
