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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.ComponentModel;
using System.IO;

namespace Microsoft
{
    public class Win32API
    {
        #region 1. Msg

        public const int SW_RESTORE = 9;
        public const int USER = 0x0400;
        public const int UM_2007 = USER + 2007;
        public const int WM_COPYDATA = 0x004A;

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;

        #endregion Msg

        #region 2. API_User32
        [DllImport( "user32.dll" )]
        public static extern void PostMessage( IntPtr hWnd , int wMsg , IntPtr wParam , IntPtr lParam );
        [DllImport( "User32.dll" , EntryPoint = "SendMessage" )]
        public static extern int PostMessage(
        IntPtr hWnd , // handle to destination window
        int Msg , // message
        IntPtr wParam , // first message parameter
        ref COPYDATASTRUCT lParam // second message parameter
        );

        [DllImport( "user32.dll" , EntryPoint = "SendMessageA" )]
        public static extern int SendMessage( IntPtr hWnd , int wMsg , IntPtr wParam , IntPtr lParam );

        [DllImport( "User32.dll" , EntryPoint = "SendMessage" )]
        public static extern int SendMessage(
        IntPtr hWnd , // handle to destination window
        int Msg , // message
        IntPtr wParam , // first message parameter
        ref COPYDATASTRUCT lParam // second message parameter
        );

        [DllImport( "User32.dll" , EntryPoint = "FindWindow" )]
        public static extern IntPtr FindWindow( string lpClassName , string lpWindowName );

        [DllImport( "user32.dll" , SetLastError = true )]
        public static extern IntPtr FindWindowEx( IntPtr parentHandle , IntPtr childAfter , string className , IntPtr windowTitle );

        [DllImport( "User32.dll" )]
        private static extern bool ShowWindow( IntPtr handle , int nCmdShow );
        [DllImport( "User32.dll" )]
        private static extern bool IsIconic( IntPtr handle );
        [DllImport( "User32.dll" )]
        private static extern bool SetForegroundWindow( IntPtr hWnd );
        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        static extern bool GetWindowRect( IntPtr hWnd , out RECT lpRect );

        [System.Runtime.InteropServices.DllImport( "user32.dll" , CharSet = System.Runtime.InteropServices.CharSet.Auto , ExactSpelling = true )]
        public static extern IntPtr GetForegroundWindow();

        #endregion

        #region 3. API_Shell32
        [DllImport( "Shell32.dll" , CharSet = CharSet.Auto )]
        public static extern UInt32 SHGetSetFolderCustomSettings( ref LPSHFOLDERCUSTOMSETTINGS pfcs , string pszPath , UInt32 dwReadWrite );
        #endregion

        #region SendMessage
        /// <summary>
        /// Send a message to specific window
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="windowName">window's name</param>
        /// <returns></returns>
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
        public static Boolean PostMessage( String msg , String windowName )
        {
            IntPtr WINDOW_HANDLER = Win32API.FindWindow( null , windowName );
            if ( WINDOW_HANDLER == IntPtr.Zero ) { return false; }
            else
            {
                COPYDATASTRUCT cds;
                cds.dwData = (IntPtr)100;
                cds.lpData = msg;
                cds.cbData = Encoding.Unicode.GetBytes( msg ).Length + 1;
                Win32API.PostMessage( WINDOW_HANDLER , Win32API.WM_COPYDATA , IntPtr.Zero , ref cds );
            }
            return true;
        }
        #endregion SendMessage

        #region Get System Icon
        [StructLayout( LayoutKind.Sequential )]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs( UnmanagedType.ByValTStr , SizeConst = 260 )]
            public string szDisplayName;
            [MarshalAs( UnmanagedType.ByValTStr , SizeConst = 80 )]
            public string szTypeName;
        }
        /// <summary>   
        /// Get System icon  
        /// </summary>   
        /// <param pathFullName="pszPath">File path</param>   
        /// <param pathFullName="dwFileAttributes">0</param>   
        /// <param pathFullName="psfi">structure</param>   
        /// <param pathFullName="cbSizeFileInfo">the size of structure</param>   
        /// <param pathFullName="uFlags">enum tpyes</param>   
        /// <returns>-1:faile</returns>   
        [DllImport( "shell32.dll" )]
        public static extern IntPtr SHGetFileInfo( string pszPath , uint dwFileAttributes , ref   SHFILEINFO psfi , uint cbSizeFileInfo , uint uFlags );
        public enum SHGFI
        {
            SHGFI_ICON = 0x100 ,
            SHGFI_LARGEICON = 0x0 ,
            SHGFI_USEFILEATTRIBUTES = 0x10
        }


        /// <summary>   
        /// Get the icon of file
        /// </summary>   
        /// <param pathFullName="p_Path">the path of file</param>   
        /// <returns>icon</returns>   
        public static Icon GetFileIcon( string p_Path )
        {
            SHFILEINFO _SHFILEINFO = new SHFILEINFO();
            IntPtr _IconIntPtr = SHGetFileInfo( p_Path , 0 , ref _SHFILEINFO , (uint)Marshal.SizeOf( _SHFILEINFO ) , (uint)( SHGFI.SHGFI_ICON | SHGFI.SHGFI_LARGEICON | SHGFI.SHGFI_USEFILEATTRIBUTES ) );
            if ( _IconIntPtr.Equals( IntPtr.Zero ) ) return null;
            Icon _Icon = System.Drawing.Icon.FromHandle( _SHFILEINFO.hIcon );
            return _Icon;
        }
        /// <summary>   
        /// Get the icon of folder
        /// </summary>   
        /// <returns>icon</returns>   
        public static Icon GetDirectoryIcon()
        {
            SHFILEINFO _SHFILEINFO = new SHFILEINFO();
            IntPtr _IconIntPtr = SHGetFileInfo( @"" , 0 , ref _SHFILEINFO , (uint)Marshal.SizeOf( _SHFILEINFO ) , (uint)( SHGFI.SHGFI_ICON | SHGFI.SHGFI_LARGEICON ) );
            if ( _IconIntPtr.Equals( IntPtr.Zero ) ) return null;
            Icon _Icon = System.Drawing.Icon.FromHandle( _SHFILEINFO.hIcon );
            return _Icon;
        }
        #endregion Get System Icon

        public static void BringWindowToTopOrCreate( String driectoryPath )
        {
            DirectoryInfo dir = new DirectoryInfo( driectoryPath );
            IntPtr hWnd = FindWindow( null , dir.Name );

            if ( hWnd != IntPtr.Zero )
            {
                hWnd = IntPtr.Zero;
                var t = Type.GetTypeFromProgID( "Shell.Application" );
                dynamic o = Activator.CreateInstance( t );
                try
                {
                    var ws = o.Windows();
                    for ( int i = 0 ; i < ws.Count ; i++ )
                    {
                        var ie = ws.Item( i );
                        if ( ie == null ) continue;
                        var path = System.IO.Path.GetFileName( (string)ie.FullName );
                        if ( path.ToLower() == "explorer.exe" )
                        {
                            String explorepath = null;
                            try
                            {
                                explorepath = Directory.GetParent( ie.document.focuseditem.path ).ToString();
                            } catch ( System.Exception ) { continue; }
                            if ( explorepath == dir.FullName )
                                hWnd = (IntPtr)ie.hwnd;
                        }
                    }

                } finally
                {
                    Marshal.FinalReleaseComObject( o );
                }
                if ( hWnd == IntPtr.Zero )
                {
                    System.Diagnostics.Process.Start( "explorer.exe" , dir.FullName );
                }
                else
                {
                    if ( IsIconic( hWnd ) )
                    {
                        ShowWindow( hWnd , SW_RESTORE );
                    }
                    SetForegroundWindow( hWnd );
                }
            }
            else
            {
                System.Diagnostics.Process.Start( "explorer.exe" , dir.FullName );
            }
        }
    }
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        [MarshalAs( UnmanagedType.LPWStr )]
        public string lpData;
    }

    [StructLayout( LayoutKind.Sequential , CharSet = CharSet.Auto )]
    public struct LPSHFOLDERCUSTOMSETTINGS
    {
        public UInt32 dwSize;
        public UInt32 dwMask;
        public IntPtr pvid;
        public string pszWebViewTemplate;
        public UInt32 cchWebViewTemplate;
        public string pszWebViewTemplateVersion;
        public string pszInfoTip;
        public UInt32 cchInfoTip;
        public IntPtr pclsid;
        public UInt32 dwFlags;
        public string pszIconFile;
        public UInt32 cchIconFile;
        public int iIconIndex;
        public string pszLogo;
        public UInt32 cchLogo;
    }
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public override string ToString()
        {
            return "(" + left + ", " + top + ") --> (" + right + ", " + bottom + ")";
        }
    }
}
