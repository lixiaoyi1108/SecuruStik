using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft;
using System.Runtime.InteropServices;

namespace SecuruStik.MessageQueue
{
    public class SecuruStikMessageQueue
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(SecuruStikMessageQueue));

        public static String MainFormText = @"SecuruStik_UIDesign_MainForm";
        public static String SplashFormText = @"SecuruStikSplashScreen";

        private static Boolean SendMessageToSecuruStikForm( String msg )
        {
#if WIN32_MQ
            return Win32API.SendMessage( msg , MainFormText );
#else
            log.WarnFormat("MQ ignored {0} to {1}", msg, MainFormText);
            return true;
#endif
        }
        private static Boolean PostMessageToSecuruStikForm( String msg )
        {
#if WIN32_MQ
            return Win32API.PostMessage( msg , MainFormText );
#else
            log.WarnFormat("MQ ignored {0} to {1}", msg, MainFormText);
            return true;
#endif
        }
        private static Boolean SendMessageToSplashScreen( String msg )
        {
#if WIN32_MQ
            return Win32API.SendMessage( msg , SplashFormText );
#else
            log.WarnFormat("MQ ignored {0} to {1}", msg, SplashFormText);
            return true;
#endif
        }

#region Send messages to MainForm
        public static Boolean SendMessage_MoveToSecuruStik( String filePath )
        {
            return SendMessageToSecuruStikForm(
                String.Format( "{0}{1}{2}" ,
                SecuruStikMessageType.MoveToSecuruStik ,
                SecuruStikMessageType.SplitChar ,
                filePath ) );
        }
        public static Boolean SendMessage_CopyToSecuruStik( String filePath )
        {
            return SendMessageToSecuruStikForm(
                String.Format( "{0}{1}{2}" ,
                SecuruStikMessageType.CopyToSecuruStik ,
                SecuruStikMessageType.SplitChar ,
                filePath ) );
        }
        public static Boolean SendMessage_Checking_Begin()
        {
            return SendMessageToSecuruStikForm( SecuruStikMessageType.Check_Begin );
        }
        public static Boolean SendMessage_Checking_End()
        {
            return SendMessageToSecuruStikForm( SecuruStikMessageType.Check_End );
        }

        public static Boolean SendMessage_Encryption( String filePath )
        {
            return SendMessageToSecuruStikForm(
                String.Format( "{0}{1}{2}" ,
                SecuruStikMessageType.Encryption ,
                SecuruStikMessageType.SplitChar ,
                filePath ) );
        }
        public static Boolean SendMessage_Decryption( String filePath )
        {
            return SendMessageToSecuruStikForm(
                String.Format( "{0}{1}{2}" ,
                SecuruStikMessageType.Decryption ,
                SecuruStikMessageType.SplitChar ,
                filePath ) );
        }

        public static Boolean SendMessage_Sync_Begin()
        {
            return SendMessageToSecuruStikForm( SecuruStikMessageType.Sync_Begin );
        }
        public static Boolean SendMessage_Sync_End()
        {
            return SendMessageToSecuruStikForm( SecuruStikMessageType.Sync_End );
        }
        public static Boolean SendMessage_Share( String filePath )
        {
            return SendMessageToSecuruStikForm(
                String.Format( "{0}{1}{2}" ,
                SecuruStikMessageType.Share ,
                SecuruStikMessageType.SplitChar ,
                filePath ) );
        }
        public static Boolean SendMessage_Share_End( String filePath , String id_To )
        {
            return SendMessageToSecuruStikForm(
                String.Format( "{0}{1}{2}{3}{4}" ,
                SecuruStikMessageType.Share_End ,
                SecuruStikMessageType.SplitChar ,
                filePath ,
                SecuruStikMessageType.SplitChar ,
                id_To
            ) );
        }
        public static Boolean SendMessage_Download_Failed()
        {
            return SendMessageToSecuruStikForm(
                String.Format( "{0}" ,
                SecuruStikMessageType.Download_Failed
            ) );
        }

        public static Boolean PostMessage_Show()
        {
            return PostMessageToSecuruStikForm( SecuruStikMessageType.Show );
        }
        public static Boolean SendMessage_Notify_Eject()
        {
            return SendMessageToSecuruStikForm( SecuruStikMessageType.Eject );
        }
        public static Boolean SendMessage_Notify_Plug()
        {
            return SendMessageToSecuruStikForm( SecuruStikMessageType.Plug );
        }
        public static Boolean SendMessage_ReceivePK( String ID , String PK1 , String PK2 )
        {
            return SendMessageToSecuruStikForm(
                String.Format( "{0}{1}{2}{3}{4}{5}{6}" ,
                SecuruStikMessageType.ReceivePK , SecuruStikMessageType.SplitChar ,
                ID , SecuruStikMessageType.SplitChar ,
                PK1 , SecuruStikMessageType.SplitChar ,
                PK2 )
                );
        }
        public static Boolean SendMEssage_Disconnect()
        {
            return SendMessageToSecuruStikForm(SecuruStikMessageType.Disconnect);
        }

#endregion

#region Send message to SplashForm
        public static Boolean SendMessage_Splash_Show()
        {
            return SendMessageToSplashScreen( SecuruStikMessageType.Show );
        }
        public static Boolean SendMessage_Splash_Hiden()
        {
            return SendMessageToSplashScreen( SecuruStikMessageType.Hiden );
        }
#endregion
    }

    public class SecuruStikMessageType
    {
        //File opt
        public const String MoveToSecuruStik = "M";
        public const String CopyToSecuruStik = "C";
        public const String Encryption = "E";
        public const String Decryption = "D";

        public const String Sync_Begin = "ScB";
        public const String Sync_End = "ScE";

        public const String Share = "Se";
        public const String Share_Begin = "SeB";
        public const String Share_End = "SeE";
        public const String Download_Begin = "DlB";
        public const String Download_End = "DlE";
        public const String Download_Failed = "DlF";

        public const String SplitChar = "|";

        //UI
        public const String Show = "Sw";
        public const String Hiden = "H";
        public const String Check_Begin = "CkB";
        public const String Check_End = "CkE";

        //USB
        public const String Eject = "Ej";
        public const String Plug = "Pl";

        //Proxy Server
        public const String ReceivePK = "RPK";
        public const String Disconnect = "Discon";
    }
}
