using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SecuruStik.MessageQueue;
using SecuruStikSettings;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using Microsoft;

namespace SecuruStik.ShellExtensions
{
    [ComVisible(true)]
    [COMServerAssociation( AssociationType.AllFiles )]
    public class SecuruStik : SharpContextMenu
    {
        protected override Boolean CanShowMenu()
        {
            IntPtr WINDOW_HANDLER = Win32API.FindWindow( null , SecuruStikMessageQueue.MainFormText );
            if ( WINDOW_HANDLER == IntPtr.Zero )
            {
                return false;
            }
            return true;
        }
        protected override ContextMenuStrip CreateMenu()
        {
            String SecuruStikSecFolder = AppSetting.Path_Local_SecuruStikSecFolder;
            ContextMenuStrip menu = new ContextMenuStrip();

            foreach ( String path in SelectedItemPaths )
            {
                if ( Path.GetFullPath( path ).Contains( SecuruStikSecFolder ) == false )
                {
                    var ToolStrip_Move = new ToolStripMenuItem( Image.FromHbitmap( Properties.Resources.Icon16X16.ToBitmap().GetHbitmap() ) );
                    var ToolStrip_Copy = new ToolStripMenuItem( Image.FromHbitmap( Properties.Resources.Icon16X16.ToBitmap().GetHbitmap() ) );
                    ToolStrip_Move.Text = "Move to SecuruStik";
                    ToolStrip_Copy.Text = "Copy to SecuruStik";
                    ToolStrip_Move.Click += ( sender , args ) => MoveToSecuruStik(); 
                    ToolStrip_Copy.Click += ( sender , args ) => CopyToSecuruStik();

                    menu.Items.Add( ToolStrip_Move );
                    menu.Items.Add( ToolStrip_Copy );
                }
                else
                {
                    var ToolStrip_Share = new ToolStripMenuItem( Image.FromHbitmap( Properties.Resources.Icon16X16.ToBitmap().GetHbitmap() ) );
                    ToolStrip_Share.Text = "Share link...";
                    ToolStrip_Share.Click += ( sender , args ) => Share();
                    menu.Items.Add( ToolStrip_Share );
                }
                break;
            }
            return menu;
        }
        
        private void MoveToSecuruStik()
        {
            StringBuilder sb = new StringBuilder();
            foreach ( string SelectedItemPath in SelectedItemPaths )
            {
                sb.AppendFormat( "{0}|" , SelectedItemPath );
            }
            SecuruStikMessageQueue.SendMessage_MoveToSecuruStik( sb.ToString() );
        }
        private void CopyToSecuruStik()
        {
            StringBuilder sb = new StringBuilder();
            foreach ( string SelectedItemPath in SelectedItemPaths )
            {
                sb.AppendFormat( "{0}|" , SelectedItemPath );
            }
            SecuruStikMessageQueue.SendMessage_CopyToSecuruStik( sb.ToString() );
        }
        private void Share()
        {
            StringBuilder sb = new StringBuilder();
            foreach ( String SelectedItemPath in SelectedItemPaths )
            {
                sb.AppendFormat("{0}|", SelectedItemPath );
            }
            SecuruStikMessageQueue.SendMessage_Share( sb.ToString() );
        }
    }
}
