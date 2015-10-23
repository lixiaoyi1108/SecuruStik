using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace SecuruStik.BaseExtension
{
    public class ComRegisterHelper
    {
        private static String CmdRegisterParam
        {
            get
            {
                    String _cmd_RegisterParam = String.Format( "{0} /register /codebase" ,//\"{1}\"
                        Path.Combine( RuntimeEnvironment.GetRuntimeDirectory() , "RegAsm.exe" ) );
                    Boolean Is64BitRunTime = _cmd_RegisterParam.Contains( "Framework64" ) ? true : false;
                    if ( Environment.Is64BitOperatingSystem && !Is64BitRunTime )
                    {
                        _cmd_RegisterParam = _cmd_RegisterParam.Replace( "Framework" , "Framework64" );
                    }
                    else if ( !Environment.Is64BitOperatingSystem && Is64BitRunTime )
                    {
                        _cmd_RegisterParam = _cmd_RegisterParam.Replace( "Framework64" , "Framework" );
                    }
                return _cmd_RegisterParam;
            }
        }
        private static String CmdUnRegisterParam
        {
            get
            {
               return ComRegisterHelper.CmdRegisterParam.Replace( "/register" , "/u" );
            }
        }

        private static void ReStartExplorer()
        {
            //Restart the explorer
            Process[] process = Process.GetProcesses();
            var p = ( from proc in process
                      where proc.ProcessName.Equals( "explorer" )
                      select proc ).FirstOrDefault();
            p.Kill();
        }

        public static void Register( String pathName )
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
            p.StandardInput.WriteLine( String.Format( "{0} \"{1}\"" , ComRegisterHelper.CmdRegisterParam , Path.GetFullPath( pathName ) ) );
            p.StandardInput.WriteLine( "exit" );
            p.Close();
        }
        public static void UnRegister( String pathName )
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
            p.StandardInput.WriteLine( String.Format("{0} \"{1}\"",ComRegisterHelper.CmdUnRegisterParam,Path.GetFullPath(pathName)) );
            p.StandardInput.WriteLine( "exit" );
            p.Close();
        }
    }
}
