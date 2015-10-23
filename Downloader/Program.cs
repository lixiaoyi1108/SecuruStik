using System;
using SecuruStik.BaseExtension;
using System.Windows.Forms;
using System.IO;

namespace Downloader
{
    class Program
    {
        static void Main( String[] args )
        {
            if ( ( args != null ) && ( args.Length > 0 ) )
            {
                String filePath = String.Empty;
                for ( int i = 0 ; i < args.Length ; i++ )
                {
                    filePath += Path.GetFullPath( args[i] ) + SecuruStikMessageType.SplitChar;
                }

                SecuruStikMessageQueue.SendMessage_Download( filePath );
            }
        }
    }
}
