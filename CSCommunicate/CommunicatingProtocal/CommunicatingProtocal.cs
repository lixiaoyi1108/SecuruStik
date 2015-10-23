/***********************************************************************
 * CLR Version :      $civersion$
 * Class Name  :      CommunicatingProtocal
 * Name Space  :      CommunicatingProtocal
 * File Name   :      CommunicatingProtocal
 * Create Time :      2014/11/23 8:26:25
 * Author      :      JianYe Huang
 * Brief       :      Encapsulation protocal between client and proxy server
 ***********************************************************************/
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SecuruStik.Protocal
{
    #region 1. Enum & Class definition

    public enum MessageType : byte
    {
        Request_Login ,
        Request_Logout ,
        Request_GetPK ,
        Request_SetPK ,
        Request_GetSharingInfo ,
        Request_SetSharingInfo ,

        Response_Login ,
        Response_GetPK ,
        Response_SetPK ,
        Response_GetSharingInfo ,
        Response_SetSharingInfo
    }
    [Serializable]
    public class RequestContent { }
    [Serializable]
    public class PublicKey : RequestContent
    {
        public String ID;
        public String PK1;
        public String PK2;
        public PublicKey( String id , String pk1 , String pk2 )
        {
            this.ID = id;
            this.PK1 = pk1;
            this.PK2 = pk2;
        }
    }
    [Serializable]
    public class SharingInfo : RequestContent
    {
        public String ID_From;
        public String ID_TO;
        public String FileName;
        public String Reference;
        public String CKey_E;
        public String CKey_F;
        public String CKey_U;
        public String CKey_W;
        public SharingInfo( String id_from , String id_to , String filename , String reference , String ckey_E , String ckey_F , String ckey_U , String ckey_W )
        {
            this.ID_From = id_from;
            this.ID_TO = id_to;
            this.FileName = filename;
            this.Reference = reference;
            this.CKey_E = ckey_E;
            this.CKey_F= ckey_F;
            this.CKey_U= ckey_U;
            this.CKey_W= ckey_W;
        }
    }
    #endregion Enum & Structure

    #region 2. Message denifition
    /// <summary> Communication Type </summary>
    [Serializable]
    public class Packet
    {
        public MessageType Type;
    }
    [Serializable]
    public class Request : Packet
    {
        public String UserID;
        public RequestContent RequestContent;
        private Request() { }
        public Request( MessageType type , String userID )
        {
            this.Type = type;
            this.UserID = userID;
            this.RequestContent = null;
        }
        public Request( MessageType type , String userID , RequestContent requestContent )
        {
            this.Type = type;
            this.UserID = userID;
            this.RequestContent = requestContent;
        }
    }
    [Serializable]
    public class Response : Packet
    {
        public Boolean IsSuccess;
        public RequestContent ResponseContent;
        private Response() { }
        public Response( MessageType messageType , Boolean isSuccess , RequestContent response )
            : this()
        {
            this.Type = messageType;
            this.IsSuccess = isSuccess;
            this.ResponseContent = response;
        }
    }
    #endregion Message denifition

    public static class CommunicatingProtocal
    {
        public static readonly int ServerMaxConnectNum = 1024;

        public static byte[] Serialize( Object r )
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bfer = new BinaryFormatter();
                bfer.Serialize( ms , r );
                return ms.ToArray();
            }
            catch ( System.Exception ){return null;}
        }
        public static Object ExplainPacket( byte[] packet )
        {
            try
            {
                MemoryStream ms = new MemoryStream( packet );
                BinaryFormatter bfer = new BinaryFormatter();
                return bfer.Deserialize( ms );
            }
            catch ( System.Exception ){return null;}
        }
    }
}