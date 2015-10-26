using System;
using System.Windows.Forms;
using Microsoft;
using SecuruStik.DropBox;
using SecuruStik.MessageQueue;


namespace SecuruStik.UIDesign
{
    static class Program
    {
        static private log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            log.Info("Starting Program");

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
                log.Info("App already detected to be running.");
                Win32API.BringWindowToTopOrCreate(DropBoxController.Local_SecFolder); 
            }
        }
    }}
