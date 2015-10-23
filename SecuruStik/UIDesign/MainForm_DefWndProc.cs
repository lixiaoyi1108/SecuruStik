using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Metro;
using SecuruStik.BaseExtension;
using SecuruStik.Opt;
using System.Text;
using SecuruStik.DropBox;
using System.Collections.Generic;
using System.Threading;
using SecuruStik.DB;
using SecuruStik.PRE;
using Microsoft;
using SecuruStik.MessageQueue;

namespace SecuruStik.UIDesign
{
    public partial class SecuruStik_MainForm : MetroForm
    {
        private delegate void CustomDataDelegate(String[] param); 
        private Dictionary<String, CustomDataDelegate> lookup;
        public void CreateDelegateTable()
        {
            CustomDataDelegate MoveToSecuruStik = (param) => { this.MoveToSecuruStikSecFolder(param); };
            CustomDataDelegate CopyToSecuruStik = (param) => { this.CopyToSecuruStikSecFolder(param); };
            CustomDataDelegate Check_Begin = (param) => { this.IsChecking = true; };
            CustomDataDelegate Check_End = (param) => { this.IsChecking = false; };

            CustomDataDelegate Sync_Begin = (param) => { this.Notify_Sync_Begin(); };
            CustomDataDelegate Sync_End = (param) => { this.Notify_Sync_End(); };
            CustomDataDelegate Share = (param) => { this.ShareFile(param); };
            CustomDataDelegate Share_End = (param) => { this.Notify_Share_End(param[0], param[1]); };
            CustomDataDelegate Download_Begin = (param) => { this.Download(param[0]); };
            CustomDataDelegate Download_Failed = (param) => { this.Notify_Download_Failed(); };

            CustomDataDelegate Show = (param) => { this.OpenSecuruSticSecFolder(); };
            CustomDataDelegate Hiden = (param) => { this.Visible = false; };
            CustomDataDelegate Notify_Eject = (param) => { this.Notify_Eject(); };
            CustomDataDelegate Notify_Plug = (param) => { this.Notify_Plug(); };
            CustomDataDelegate ReceivePK = ( param ) => { this.DropBoxUser.ReceivePK( param[ 0 ] , param[ 1 ] , param[ 2 ] ); };
            CustomDataDelegate Disconnect = ( param ) => { this.Notify_Disconnect(); };
            
            lookup = new Dictionary<String, CustomDataDelegate>
            {
                { SecuruStikMessageType.MoveToSecuruStik ,  MoveToSecuruStik },
                { SecuruStikMessageType.CopyToSecuruStik ,  CopyToSecuruStik },
                { SecuruStikMessageType.Check_Begin ,       Check_Begin },
                { SecuruStikMessageType.Check_End ,         Check_End },

                { SecuruStikMessageType.Sync_Begin ,        Sync_Begin },
                { SecuruStikMessageType.Sync_End ,          Sync_End },
                { SecuruStikMessageType.Share ,             Share },
                {SecuruStikMessageType.Share_End        ,   Share_End },
                { SecuruStikMessageType.Download_Begin ,    Download_Begin },
                { SecuruStikMessageType.Download_Failed ,   Download_Failed },

                { SecuruStikMessageType.Show ,              Show },
                { SecuruStikMessageType.Hiden ,             Hiden },
                { SecuruStikMessageType.Eject ,             Notify_Eject },
                { SecuruStikMessageType.Plug ,              Notify_Plug },
                { SecuruStikMessageType.ReceivePK,          ReceivePK },
                { SecuruStikMessageType.Disconnect,         Disconnect }
            };
        }

        #region DefWndProc
        private Char[] SplitArray = new Char[] { Char.Parse( SecuruStikMessageType.SplitChar ) };
        private COPYDATASTRUCT mystr = new COPYDATASTRUCT();
        protected override void DefWndProc( ref System.Windows.Forms.Message m )
        {
            switch ( m.Msg )
            {
                case Win32API.WM_SYSCOMMAND://prevent the form from being moved
                    int command = m.WParam.ToInt32() & 0xfff0;
                    if ( command == Win32API.SC_MOVE )
                        return;
                    break;
                case Win32API.WM_COPYDATA:
                    mystr = (COPYDATASTRUCT)m.GetLParam( mystr.GetType() );
                    String[] type_params = mystr.lpData.Split( this.SplitArray , StringSplitOptions.RemoveEmptyEntries );

                    String Type = type_params[ 0 ];
                    String[] parameters = type_params.Skip( 1 ).ToArray<String>();

                    CustomDataDelegate customDeal = lookup[ Type ];
                    customDeal( parameters );
                    break;
                default:
                    base.DefWndProc( ref m );
                    break;
            }
        }
        #endregion DefWndProc
    }

        
}
