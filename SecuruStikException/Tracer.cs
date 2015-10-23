/***********************************************************************
 * CLR Version :      $civersion$
 * Class Name  :      Logger
 * Name Space  :      SecuruStik.Opt
 * File Name   :      Logger
 * Create Time :      2014/12/27 23:56:22
 * Author      :      Jian Ye
 * Brief       :      Log for debug
 * Modification history : 
 ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace SecuruStik.Exception
{
    public class Tracer
    {
        private readonly static String logfile = "./ErrMsg.log";
        /// <summary>
        /// Log event
        /// </summary>
        public static void Log( params String[] messages )
        {
            lock ( logfile )
            {
                FileStream fs = new FileStream( Tracer.logfile , FileMode.Append , FileAccess.Write );
                StreamWriter sw = new StreamWriter( fs );
                sw.WriteLine( DateTime.Now );

                StackTrace stackTrace = new StackTrace( true );
                StackFrame[] sts = stackTrace.GetFrames();

                sw.WriteLine( "Content:" );
                foreach ( String msg in messages )
                    sw.WriteLine( msg );

                String stackMsg = Environment.StackTrace;

                sw.WriteLine( Environment.StackTrace );
                sw.WriteLine( "==============================" );
                sw.Flush();
                sw.Close();
            }
        }
        public static void Log( System.Exception inner , params String[] messages )
        {
            lock ( logfile )
            {
                FileStream fs = new FileStream( Tracer.logfile , FileMode.Append , FileAccess.Write );
                StreamWriter sw = new StreamWriter( fs );
                sw.WriteLine( DateTime.Now );

                StackTrace stackTrace = new StackTrace( true );
                StackFrame[] sts = stackTrace.GetFrames();

                sw.WriteLine( "Content:" );
                foreach ( String msg in messages )
                    sw.WriteLine( msg );
                    
                sw.WriteLine( inner.Source+" : "+inner.StackTrace );

                String stackMsg = Environment.StackTrace;

                sw.WriteLine( Environment.StackTrace );
                sw.WriteLine( "==============================" );
                sw.Flush();
                sw.Close();
            }
        }
        public static void DeleteLogFile()
        {
            try
            {
                File.Delete( Tracer.logfile );
            } catch ( System.Exception ) { }
        }
    }
}
