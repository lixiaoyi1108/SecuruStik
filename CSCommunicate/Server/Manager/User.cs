/***********************************************************************
 * CLR Version :      4.0.30319.18444
 * Class Name  :      User
 * Name Space  :      Server
 * File Name   :      User
 * Create Time :      2014/10/13 19:16:01
 * Author      :      JianYe,Huang
 * Brief       :
 * Modification history : NULL
 ***********************************************************************/
using System;
using System.IO;
using System.Net.Sockets;

namespace Server
{
    public class User
    {
        public TcpClient tc { get; private set; }
        public BinaryReader br { get; private set; }
        public BinaryWriter bw { get; private set; }
        public String userName { get; set; }
        public Boolean IsExit { get; set; }
        public User( TcpClient client )
        {
            this.tc = client;
            NetworkStream networkStrem = client.GetStream();
            br = new BinaryReader( networkStrem );
            bw = new BinaryWriter( networkStrem );
            this.IsExit = false;
        }
        public User( TcpClient client , String userName )
            : this( client )
        {
            this.userName = userName;
        }
        ~User()
        {
            this.Close();
        }
        public void Close()
        {
            if(br!= null)br.Close();
            if(bw!= null)bw.Close();
            if(tc!= null)tc.Close();
            this.IsExit = true;
        }
    }
}
