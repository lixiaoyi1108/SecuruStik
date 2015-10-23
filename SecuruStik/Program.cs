using System;
using System.Windows.Forms;
using Microsoft;
using SecuruStik.DropBox;
using SecuruStik.MessageQueue;

namespace SecuruStik.UIDesign
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            IntPtr WINDOW_HANDLER = Win32API.FindWindow( null , SecuruStikMessageQueue.MainFormText );
            if ( WINDOW_HANDLER == IntPtr.Zero )
            {
                SecuruStikSplashScreen splashScreen = new SecuruStikSplashScreen();
                Application.Run( splashScreen );

                Application.Run( new SecuruStik_MainForm( SecuruStikSplashScreen.DropBoxUser ) );
            }
            else
            {
                Win32API.BringWindowToTopOrCreate(DropBoxController.Local_SecFolder); 
            }
        }
    }}
