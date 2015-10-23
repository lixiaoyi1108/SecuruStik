using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using System.Net.Mail;

namespace SecuruStik.UIDesign
{
    public partial class ShareForm : DevComponents.DotNetBar.Metro.MetroForm
    {
        private MailAddress emailAddress ;
        public String EmailAddrStr
        {
            get {
                try
                {
                    if ( this.textBoxX_Email.Text == String.Empty && this.emailAddress != null )
                        return String.Empty;
                    return this.emailAddress.Address.ToLower();
                }
                catch ( System.Exception )
                {
                    return String.Empty;
                }
            }
        }
        public ShareForm()
        {
            InitializeComponent();

            this.ShowInTaskbar = false;
        }

        private void ShareForm_Load( object sender , EventArgs e )
        {
            this.StartPosition = FormStartPosition.CenterScreen; 
        }
        private void button_OK_Click( object sender , EventArgs e )
        {
            try
            {
                this.emailAddress = new MailAddress( this.textBoxX_Email.Text );
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (System.Exception)
            {
                this.errorProvider1.SetError( this.textBoxX_Email , "Invalid email address." );
            }
            
        }
        private void button_Cancel_Click( object sender , EventArgs e )
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}