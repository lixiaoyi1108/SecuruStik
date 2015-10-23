using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevComponents.DotNetBar.Metro;

namespace Server
{
    public partial class MainForm : MetroForm
    {

        Char[] separator = new Char[] { '\t' , '\n' };
        private void AddLog( String message )
        {
            this.listBox_Log.Invoke(
                new Action( () =>
                {
                    this.listBox_Log.Items.Add( String.Format( "{0} : {1}" , DateTime.Now , message ) );
                    this.listBox_Log.SelectedIndex = this.listBox_Log.Items.Count-1;
                } )
                    );
        }
        private void AddLogs( params String[] message )
        {
            this.listBox_Log.Invoke(
                new Action( () =>
                {
                    this.listBox_Log.Items.Add( String.Format( "{0} : {1}" , DateTime.Now , message[ 0 ] ) );
                    for ( int i = 1 ; i != message.Length ; i++ )
                    {
                        String[] msgSections = message[ i ].Split( separator );
                            foreach(String msg in msgSections)
                                this.listBox_Log.Items.Add( String.Format( "\t\t\t{0}" , msg ));
                    }
                    this.listBox_Log.SelectedIndex = this.listBox_Log.Items.Count-1;
                } )
                );
        }

        //Clear
        private void clearToolStripMenuItem_Click( object sender , EventArgs e )
        {
            this.listBox_Log.Items.Clear();
        }
    }
}
