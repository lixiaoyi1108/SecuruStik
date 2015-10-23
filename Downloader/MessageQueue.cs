/***********************************************************************
 * CLR Version :      $civersion$
 * Class Name  :      SecuruStikMessageQueue
 * Name Space  :      SecuruStik.BaseExtension
 * File Name   :      SecuruStikMessageQueue
 * Create Time :      2014/12/31 0:18:55
 * Author      :      Jian ye
 * Brief       :      Message Queue
 * Modification history : 
 ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SecuruStik.BaseExtension
{
    public class SecuruStikMessageQueue
    {
        public static String MainFormText = @"SecuruStik_UIDesign_MainForm";

        private static Boolean SendMessageToSecuruStikForm( String msg )
        {
            return Win32API.SendMessage( msg , MainFormText );
        }

        public static Boolean SendMessage_Download( String filePath )
        {
            return SendMessageToSecuruStikForm(
                String.Format( "{0}{1}{2}" ,
                SecuruStikMessageType.Download_Begin ,
                SecuruStikMessageType.SplitChar ,
                filePath )
                );
        }
    }
    public class SecuruStikMessageType
    {
        //Send to mainform
        public const String Download_Begin = "DlB";
        public const String Download_End = "DlE";

        public const String SplitChar = "|";

    }
}
