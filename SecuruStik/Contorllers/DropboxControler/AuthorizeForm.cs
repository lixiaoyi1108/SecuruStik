using System;
using System.Linq;
using System.Windows.Forms;

using DevComponents.DotNetBar.Metro;
using System.Drawing;

namespace SecuruStik.DropBox
{
    public partial class AuthorizeForm : MetroForm
    {
        public AuthorizeForm( String authorizeURL )
        {
            InitializeComponent();
            this.circularProgress1.IsRunning = true;

            this.DialogResult = DialogResult.No;
            this.webBrowser_login.Navigate( authorizeURL );

            this.MouseDown+=new MouseEventHandler(Form_MouseDown);
            this.MouseMove+=new MouseEventHandler(Form_MouseMove);
            this.Shown +=new EventHandler(Form_Shown);
        }

        private void webBrowser_Login_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.webBrowser_login.Visible = true;
            this.circularProgress1.IsRunning = false;
            this.circularProgress1.Visible = false;

            //Callback url reached, user has completed authorization
            if (e.Url.Host.Contains("dropbox") && e.Url.Segments.Contains("authorize_submit"))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            this.TopLevel = true;
            this.TopMost = true;
            this.Focus();
            this.BringToFront();
            this.TopMost = false;
        }

        #region Drag window
        private Point offset;
        private void Form_MouseDown( object sender , MouseEventArgs e )
        {
            if ( MouseButtons.Left != e.Button ) return;

            Point cur = this.PointToScreen( e.Location );
            offset = new Point( cur.X - this.Left , cur.Y - this.Top );
        }
        private void Form_MouseMove( object sender , MouseEventArgs e )
        {
            if ( MouseButtons.Left != e.Button ) return;

            Point cur = MousePosition;
            this.Location = new Point( cur.X - offset.X , cur.Y - offset.Y );
        }

        #endregion 
    }
    
}
