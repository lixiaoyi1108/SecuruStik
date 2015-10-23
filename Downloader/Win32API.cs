/***********************************************************************
 * CLR Version :      $civersion$
 * Class Name  :      Win32API
 * Name Space  :      SecuruStik.BaseExtension
 * File Name   :      Win32API
 * Create Time :      2014/12/31 0:19:57
 * Author      :
 * Brief       :
 * Modification history : 
 ***********************************************************************/
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SecuruStik.BaseExtension
{
    public class Win32API
    {
        #region 1. Msg
        public const int USER = 0x0400;
        public const int UM_2007 = USER + 2007;
        public const int WM_COPYDATA = 0x004A;

        #endregion

        #region 2. API_User32

        [DllImport( "User32.dll" , EntryPoint = "SendMessage" )]
        public static extern int SendMessage(
        IntPtr hWnd , // handle to destination window
        int Msg , // message
        IntPtr wParam , // first message parameter
        ref COPYDATASTRUCT lParam // second message parameter
        );

        [DllImport( "User32.dll" , EntryPoint = "FindWindow" )]
        public static extern IntPtr FindWindow( string lpClassName , string lpWindowName );


        #endregion

        #region SendMessage
        public static Boolean SendMessage( String msg , String windowName )
        {
            IntPtr WINDOW_HANDLER = Win32API.FindWindow( null , windowName );
            if ( WINDOW_HANDLER == IntPtr.Zero ) { return false; }
            else
            {
                COPYDATASTRUCT cds;
                cds.dwData = (IntPtr)100;
                cds.lpData = msg;
                cds.cbData = Encoding.Unicode.GetBytes( msg ).Length + 1;
                Win32API.SendMessage( WINDOW_HANDLER , Win32API.WM_COPYDATA , IntPtr.Zero , ref cds );
            }
            return true;
        }
        #endregion SendMessage
    }
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        [MarshalAs( UnmanagedType.LPWStr )]
        public string lpData;
    }
}
