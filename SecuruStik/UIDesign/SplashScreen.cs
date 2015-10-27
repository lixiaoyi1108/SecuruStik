using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using SecuruStik.Opt;
using SecuruStik.BaseExtension;
using SecuruStik;
using System.Diagnostics;
using SecuruStik.MessageQueue;
using Microsoft;

namespace SecuruStik.UIDesign
{
    public partial class SecuruStikSplashScreen : Form
    {
        #region 0. Fields
        public static DBox_User DropBoxUser;
        Bitmap bitmap;
        #endregion 

        #region 1. Setup
        private delegate void InitializationDelegate();
        public SecuruStikSplashScreen()
        {
            InitializeComponent();

            bitmap = new Bitmap( Properties.Resources.appicon );
            ClientSize = new Size(500,500);
            this.BackgroundImageLayout = ImageLayout.Zoom;
            this.BackgroundImage = bitmap;
            this.Icon = Icon.FromHandle( bitmap.GetHicon() );

            //Begin to initialize.
            Thread t = new Thread( Initialization );
            t.IsBackground = true;
            t.SetApartmentState( ApartmentState.STA );
            t.Start();
        }
        private void Initialization()
        {
            //Do the initial work
            try
            {
                DropBoxUser = new DBox_User();
                this.BeginInvoke(new MethodInvoker(this.Close));
            }
            catch (System.Exception ex)
            {
                SecuruStikMessageQueue.SendMessage_Splash_Hiden();
                MessageBox.Show(
                    new Form { TopMost = true, StartPosition = FormStartPosition.CenterScreen },
                    ex.Message, "Sorry, our program meets some mistakes...",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();

            }
        }
        #endregion 

        #region 2. Drag window
        private Point offset;
        private void SplashScreen_MouseDown( object sender , MouseEventArgs e )
        {
            if ( MouseButtons.Left != e.Button ) return;

            Point cur = this.PointToScreen( e.Location );
            offset = new Point( cur.X - this.Left , cur.Y - this.Top );
        }
        private void SplashScreen_MouseMove( object sender , MouseEventArgs e )
        {
            if ( MouseButtons.Left != e.Button ) return;

            Point cur = MousePosition;
            this.Location = new Point( cur.X - offset.X , cur.Y - offset.Y );
        }
        #endregion
        
        protected override void DefWndProc( ref System.Windows.Forms.Message m )
        {
            switch ( m.Msg )
            {
                case Win32API.WM_COPYDATA:
                    COPYDATASTRUCT mystr = new COPYDATASTRUCT();
                    Type mytype = mystr.GetType();
                    mystr = (COPYDATASTRUCT)m.GetLParam( mytype );
                    String op = mystr.lpData;
                    switch ( op )
                    {
                        case SecuruStikMessageType.Show:
                            this.Visible = true;break;
                        case SecuruStikMessageType.Hiden:
                            this.Visible = false;break;
                    }
                    break;
                default:
                    base.DefWndProc( ref m );
                    break;
            }
        }
    }
}