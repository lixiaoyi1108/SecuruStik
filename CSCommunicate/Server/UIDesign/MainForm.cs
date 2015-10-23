using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Metro;
using SecuruStik.Protocal;

namespace Server
{
    public partial class MainForm : MetroForm
    {
        #region 0. Fields

        private TcpListener serverListener;
        private Boolean IsExit = false;

        public String IP = Properties.Settings.Default.REMOTE_IP;//CommunicatingProtocal.ServerIP;
        public Int16 Port = Properties.Settings.Default.PORT;//CommunicatingProtocal.ServerPort;

        public List<User> ClientList { get; set; }
        /// <summary>
        /// My communication protocal
        /// Convert between bytes and packet
        /// </summary>
        private ServerControler DropBoxServerControler;

        #endregion Fields

        #region 1. Construction
        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load( object sender , EventArgs e )
        {
            try
            {
                this.serverListener = new TcpListener( IPAddress.Parse( this.IP ) , this.Port );

                this.ClientList = new List<User>();

                this.DropBoxServerControler = new ServerControler();

                this.AddLog( "Init server successfully..." );
            } catch ( Exception ex )
            {
                this.button_Listenning.Enabled = false;
                this.AddLogs("Failed to init server...",
                                String.Format("error msg : {0}",ex.Message));
            }
            this.MouseDown+=new MouseEventHandler( MainForm_MouseDown );
            this.MouseMove+=new MouseEventHandler( MainForm_MouseMove );

            this.groupBox_Client.MouseDown +=new MouseEventHandler( MainForm_MouseDown );
            this.groupBox_Client.MouseMove +=new MouseEventHandler( MainForm_MouseMove );

            this.groupBox_Log.MouseDown +=new MouseEventHandler( MainForm_MouseDown );
            this.groupBox_Log.MouseMove +=new MouseEventHandler( MainForm_MouseMove );
        }

        private Point offset;
        private void MainForm_MouseDown( object sender , MouseEventArgs e )
        {
            if ( MouseButtons.Left != e.Button ) return;

            Control control = sender as Control;

            Point cur = control.PointToScreen( e.Location );
            offset = new Point( cur.X - this.Left , cur.Y - this.Top );

        }
        private void MainForm_MouseMove( object sender , MouseEventArgs e )
        {
            if ( MouseButtons.Left != e.Button ) return;

            Point cur = MousePosition;
            this.Location = new Point( cur.X - offset.X , cur.Y - offset.Y );
        }
        #endregion Constructions

        #region 2. listen
        private void BeginListen()
        {
            serverListener.Start();

            this.IsExit = false;
            Thread listenThread = new Thread( ListenClientConnect );
            listenThread.IsBackground = true;
            listenThread.Start();
            AddLog( "Start listening..." );
        }
        private void StopListen()
        {
            try
            {
                IsExit = true;
                foreach ( User u in ClientList )
                {
                    u.Close();
                }
                this.listBox_ClientList.Items.Clear();
                this.ClientList.Clear();

                serverListener.Stop();

                this.IsExit = true;
                AddLog( "Stop listening..." );
            } catch ( System.Exception ex )
            {
                MessageBoxEx.Show( new Form { TopMost = true } , ex.Message );
            }
        }

        private void button_Begin_Stop_Listening_Click( object sender , EventArgs e )
        {
            ButtonX button = sender as ButtonX;
            if ( button.Text == "Start" )
            {
                this.BeginListen();
                button.Text = "Stop";
                button.Symbol = "\uf04d";
            }
            else
            {
                this.StopListen();
                button.Text = "Start";
                button.Symbol = "\uf04b";
            }
        }
        /// <summary> Listen thread </summary>
        private void ListenClientConnect()
        {
            TcpClient newClient = null;

            while ( this.IsExit == false )
            {
                try
                {
                    newClient = this.serverListener.AcceptTcpClient();
                    User user = new User( newClient );
                    Thread receivedThread = new Thread( ReceiveThread );
                    receivedThread.IsBackground = true;
                    receivedThread.Start( user );
                }
                catch ( System.Exception )
                {
                    break;
                }
            }
        }
        #endregion listen

        #region 3. Recive date
        /// <summary> Recive data from client </summary>
        private void ReceiveThread( object userState )
        {
            User client = userState as User;
            if ( client == null ) return;
            TcpClient tc = client.tc;

            while ( this.IsExit == false && client.IsExit == false )
            {
                string receiveString = String.Empty;
                try
                {
                    int packetSize = client.br.ReadInt32();
                    byte[] packet = client.br.ReadBytes( packetSize );
                    DealReceivedData( client , packet );
                }
                catch ( Exception)
                {
                    if ( IsExit == false )
                    {
                        //RemoveClient( client );
                        //AddLog( String.Format( "【{0}】 logout." , client.userName ) );
                        Logout( client );
                    }
                    break;
                }
            }
        }
        private void DealReceivedData( User client , byte[] packetBytes )
        {
            SecuruStik.Protocal.Packet packet = (Packet)CommunicatingProtocal.ExplainPacket( packetBytes );
            SecuruStik.Protocal.Request request = packet as Request;
            switch ( packet.Type )
            {
                case MessageType.Request_Login:
                    Login( client , request );
                    break;
                case MessageType.Request_Logout:
                    Logout(client);
                    break;
                case MessageType.Request_GetPK:
                    Request_GetPK( client , request);
                    break;
                case MessageType.Request_SetPK:
                    Request_SetPK( client , request );
                    break;
                case MessageType.Request_GetSharingInfo:
                    Request_GetSharingInfo( client , request.UserID ); 
                    break;
                case MessageType.Request_SetSharingInfo:
                    Request_SetSharingInfo( client , request ); 
                    break;
            }
        }
        private void Login( User client,Request request )
        {
            client.userName = request.UserID;
            AddClient( client );

            SecuruStik.Protocal.Response response = new Response( MessageType.Request_Login , true , null );
            Send( client , response );

            AddLog( String.Format( "【{0}】 login." , client.userName ) );
            Request_GetSharingInfo( client , client.userName );
        }
        private void Logout( User client )
        {
            RemoveClient( client );

            client.Close();

            AddLog( String.Format( "【{0}】 logout." , client.userName ) );
        }
        private Boolean Request_GetPK( User client , Request request )
        {
            AddLog( string.Format( "【{0}】 request 【{1}】's public key." , client.userName , request.UserID ) );
            PublicKey pk = null;
            SecuruStik.Protocal.Response response = null;
            this.DropBoxServerControler.PublicKey.Download( request.UserID , ref pk );
            if ( pk == null )
            {
                pk = new PublicKey( request.UserID , null , null );
                response = new Response( MessageType.Response_GetPK , false , pk );
                AddLog( string.Format( "Failed to get 【{0}】's public key." , request.UserID ) );
            }
            else
            {
                response = new Response( MessageType.Response_GetPK , true , pk );
                AddLogs( new String[] 
                            {
                                    String.Format( "Return【{1}】's public key to【{0}】." , client.userName , request.UserID ) ,
                                    String.Format( "PK1 : {0}" , pk.PK1 ) ,
                                    String.Format( "PK2 : {0}" , pk.PK2 ) 
                            }
                        );
            }
            this.Send( client , response );
            return true;
        }
        private Boolean Request_SetPK( User client , Request request )
        {
            PublicKey pk = (SecuruStik.Protocal.PublicKey)request.RequestContent;
            Boolean ret = this.DropBoxServerControler.PublicKey.Update( request.UserID , pk );
            AddLogs( new String[]
                    {
                        String.Format( "【{0}】 updates public key." , client.userName ) ,
                        String.Format( "PK1:{0}",pk.PK1) ,
                        String.Format( "PK2:{0}",pk.PK2),
                        String.Format( "{1} to update【{0}】's  public key." , client.userName,ret?"Success":"Failed" )
                    }
                );
            return ret;
        }
        private Boolean Request_GetSharingInfo( User client , String userID )
        {
            List<String> logList = new List<string>();
            logList.Add( string.Format( "【{0}】 requests sharing info." , client.userName ) );

            List<SharingInfo> sharingInfoList =
                        this.DropBoxServerControler.SharingInfo.Download( userID );
            if ( sharingInfoList.Count == 0 )
            {
                logList.Add( string.Format( "【{0}】's sharing box is empty." , client.userName ) );
                return false;
            }
            else
            {
                logList.Add( String.Format( "Return {0} sharing infos to【{1}】." , sharingInfoList.Count,client.userName ) );
                foreach ( SharingInfo sharingInfo in sharingInfoList )
                {
                    SecuruStik.Protocal.Response response = new Response(
                        MessageType.Response_GetSharingInfo ,
                        true ,
                        sharingInfo );
                    this.Send( client , response );

                    logList.Add( String.Format("{0}",sharingInfo.FileName ));
                }
                AddLogs( logList.ToArray() );
            }
            return true;
        }
        private void Request_SetSharingInfo( User client,Request request )
        {
            String retStr = String.Empty;
            SecuruStik.Protocal.SharingInfo si = request.RequestContent as SharingInfo;
            if ( si == null )
            {
                retStr = String.Format( "Failed to upload sharing info from 【{0}】 to 【{1}】." , client.userName , (request.RequestContent as SharingInfo).ID_TO );
            }
            else
            {
                User u = this.ClientList.Find( usr => usr.userName == si.ID_TO );
                if ( u != null )
                {
                    SecuruStik.Protocal.Response response = new Response(
                        MessageType.Response_GetSharingInfo ,
                        true ,
                        si );
                    this.Send( u , response );
                    retStr = String.Format( "Success : Forward sharing info from 【{0}】 to 【{1}】." , client.userName , u.userName );
                    
                }
                else
                {
                    Boolean ret = this.DropBoxServerControler.SharingInfo.Upload( si );
                    
                    retStr =  String.Format( "{3} : Upload sharing info from 【{0}】 to 【{1}】." , 
                        client.userName , 
                        (request.RequestContent as SharingInfo).ID_TO ,
                        ret?"Success":"Failed")  ;

                }
            }
            AddLogs( 
                new String[] {
                String.Format( "【{0}】 upload sharing info to 【{1}】." , client.userName , request.UserID ) ,
                retStr}
                );
            
        }
        #endregion Receive data

        #region 4. Send data
        private void Send( User user , SecuruStik.Protocal.Packet packet )
        {
            try
            {
                byte[] packetBytes = CommunicatingProtocal.Serialize( packet );
                user.bw.Write( packetBytes.Length );
                user.bw.Write( packetBytes );
                user.bw.Flush();
            }
            catch ( System.Exception  )
            {
            }
        }
        #endregion Send data

        #region 5. UserList
        private delegate void ClientDelegate( User client );
        /// <summary>Add new client(list/log/new thread)</summary>
        public void AddClient( User client )
        {
            if ( this.listBox_ClientList.InvokeRequired )
            {
                ClientDelegate addClient = new ClientDelegate( AddClient );
                this.listBox_ClientList.Invoke( addClient , client );
            }
            else
            {
                this.ClientList.Add( client );
                this.listBox_ClientList.Items.Add( client.userName );
            }
        }
        public void RemoveClient( User user )
        {
            if ( this.listBox_ClientList.InvokeRequired )
            {
                ClientDelegate removeClient = new ClientDelegate( RemoveClient );
                this.listBox_ClientList.Invoke( removeClient , user );
            }
            else
            {
                int index = this.ClientList.IndexOf( user );
                if ( index >= 0 && index <= this.listBox_ClientList.Items.Count )
                {
                    this.listBox_ClientList.Items.RemoveAt( index );
                    this.ClientList.Remove( user );
                }
            }

        }
        #endregion UserList

        #region 6. Others
        private void ServerForm_FormClosing( object sender , FormClosingEventArgs e )
        {
            if ( this.serverListener != null )
            {
                this.StopListen();
            }
        }
        #endregion Others

    }
}