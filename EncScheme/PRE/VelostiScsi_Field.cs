/***********************************************************************
 * CLR Version :      $civersion$
 * Class Name  :      TaskWorker
 * Name Space  :      SecuruStik.PRE
 * File Name   :      TaskWorker
 * Create Time :      2015/4/9 19:09:41
 * Author      :
 * Brief       :
 * Modification history : 
 ***********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using SecuruStik.MessageQueue;

namespace SecuruStik.PRE
{
    public partial class VelostiScsi
    {
        #region Loopbacktool

        private static String _PubDrive = String.Empty;
        private static String _CurrDrive = String.Empty;
        private static String _LoopBackTool = String.Empty;
        private static String _EncBatFile = String.Empty;
        private static String _DecBatFile = String.Empty;
        private static String _KeyFile = String.Empty;

        public static Boolean IsPlugin_PubDrive
        {
            get
            {
                DriveInfo[] diss = System.IO.DriveInfo.GetDrives();
                foreach (DriveInfo dis in diss)
                {
                    if (!String.IsNullOrWhiteSpace(VelostiScsi.PubDrive) &&
                                     dis.RootDirectory.ToString().Contains(VelostiScsi.PubDrive))
                        return true;
                }
                return false;
            }
        }
        public static Boolean IsPlugin_CurrDrive
        {
            get
            {
                DriveInfo[] diss = System.IO.DriveInfo.GetDrives();
                foreach (DriveInfo dis in diss)
                {
                    if (!String.IsNullOrWhiteSpace(VelostiScsi.CurrDrive) &&
                                     dis.RootDirectory.ToString().Contains(VelostiScsi.CurrDrive))
                        return true;
                }
                return false;
            }
        }
        public static String PubDrive
        {
            get
            {
                if ( VelostiScsi._PubDrive == String.Empty )
                {
                    DriveInfo[] diss = System.IO.DriveInfo.GetDrives();
                    //.Single( di =>di.DriveType == DriveType.Removable &&di.VolumeLabel.Equals("PUBLIC"));
                    foreach (DriveInfo dis in diss)
                    {
                        if (dis.DriveType == DriveType.Removable || dis.DriveType == DriveType.CDRom)
                        {
                            try
                            {
                                if (dis.VolumeLabel.ToUpper() == "PUBLIC")
                                    VelostiScsi._PubDrive = dis.Name.Substring(0, 2);
                            }
                            catch (System.Exception ex)
                            { }
                        }
                    }
                }
                return VelostiScsi._PubDrive;
            }
        }
        public static String CurrDrive
        {
            get
            {
                if (VelostiScsi._CurrDrive == String.Empty)
                {
                    VelostiScsi._CurrDrive = Path.GetPathRoot(Environment.CurrentDirectory);
                }
                return VelostiScsi._CurrDrive;
            }
        }
        public static String LoopBackTool
        {
            get
            {
                if ( VelostiScsi._LoopBackTool == String.Empty )
                {
                    VelostiScsi._LoopBackTool = Path.GetFullPath( "./loopback/loopbacktool.exe" );
                }
                return VelostiScsi._LoopBackTool;
            }
        }
        public static String EncBatFile
        {
            get
            {
                if ( VelostiScsi._EncBatFile == String.Empty )
                {
                    VelostiScsi._EncBatFile = Path.GetFullPath( "./loopback/enc.bat" );
                }
                return VelostiScsi._EncBatFile;
            }
        }
        public static String DecBatFile
        {
            get
            {
                if ( VelostiScsi._DecBatFile == String.Empty )
                {
                    VelostiScsi._DecBatFile = Path.GetFullPath("./loopback/dec.bat");
                }
                return VelostiScsi._DecBatFile;
            }
        }
        public static String KeyFile
        {
            get
            {
                if ( VelostiScsi._KeyFile == String.Empty )
                {
                    VelostiScsi._KeyFile = Path.GetFullPath("./loopback/loopback.ck");
                }
                return VelostiScsi._KeyFile;
            }
        }

        private static ProcessStartInfo processInfo;
        private static Process process;
        public static Boolean ExecuteCommand( String file , String command )
        {
            processInfo = new ProcessStartInfo( file , command );
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start( processInfo );
            process.WaitForExit();

            // *** Read the streams ***
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if ( process.ExitCode != 0 )
                return false;
            return true;
        }
        #endregion Loopbacktool

        private Aes Aes;
        private Boolean IsBusy;
        private BackgroundWorker Worker;
        private Stopwatch StopWatch = new Stopwatch();

        private static List<TaskUnit> EncryptWaitingList = new List<TaskUnit>();
        private static List<TaskUnit> DecryptWaitingList = new List<TaskUnit>();
        private static List<TaskUnit> ErrorList = new List<TaskUnit>();
        private static long EncryptWaitingCount
        {
            get
            {
                return ( VelostiScsi.EncryptWaitingList == null ) ? 0 : VelostiScsi.EncryptWaitingList.Count;
            }
        }
        private static long DecryptWaitingCount
        {
            get
            {
                return ( VelostiScsi.DecryptWaitingList == null ) ? 0 : VelostiScsi.DecryptWaitingList.Count;
            }
        }
        private static TaskUnit CurrentTask = null;
        public static long TaskCount
        {
            get
            {
                return VelostiScsi.EncryptWaitingCount +
                    VelostiScsi.DecryptWaitingCount +
                    ( ( VelostiScsi.CurrentTask == null ) ? 0 : 1 );
            }
        }
        private static Double _speed = 30;
        public static Double Speed 
        {
            get
            {
                return _speed;
            }
            set { 
                _speed = value;
            }
        }
        public static String CurrentTaskName
        {
            get
            {
                if ( VelostiScsi.CurrentTask == null )
                    return String.Empty;
                else
                   return ( VelostiScsi.CurrentTask.TaskType == TaskType.Encrypt ) ?
                        VelostiScsi.CurrentTask.PlaintextFilePath :
                        VelostiScsi.CurrentTask.EncryptedFilePath;
            }
        }

        public VelostiScsi()
        {
            this.Aes = AesManaged.Create();
            this.Aes.KeySize = 256;
            this.IsBusy = false;

            this.Worker = new BackgroundWorker();
            this.Worker.WorkerReportsProgress = true;
            this.Worker.WorkerSupportsCancellation = true;
            this.Worker.DoWork += new DoWorkEventHandler(TaskWorker_DoWork);
            this.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(TaskWorker_RunWorkerCompleted);

            this.timer = new Thread(timer_Elapsed);
            this.timer.IsBackground = true;
            this.timer.Start();
        }
        //Detect the Public Drive
        private Thread timer ;
        private void timer_Elapsed()
        {
            while (true)
            {
                if( VelostiScsi.IsPlugin_CurrDrive == false )
                { 
                    Process.GetCurrentProcess().Kill();
                }
                else if ( VelostiScsi.IsPlugin_PubDrive == false )
                {
                    SecuruStikMessageQueue.SendMessage_Notify_Eject();
                    WaitForPlugin();
                    break;
                }
                Thread.Sleep(1000);
            }
        }
        private void WaitForPlugin()
        {
            while (true)
            {
                if (VelostiScsi.IsPlugin_CurrDrive == false)
                {
                    Process.GetCurrentProcess().Kill();
                }
                else if (VelostiScsi.IsPlugin_PubDrive == true)
                {
                    SecuruStikMessageQueue.SendMessage_Notify_Plug();
                    this.StartWork();
                    break;
                }
            }
        }
        protected virtual Boolean IsFileLocked( String file )
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream( file , FileMode.Open , FileAccess.Read , FileShare.None );
            }
            catch ( IOException )
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if ( stream != null )
                    stream.Close();
            }
            //file is not locked
            return false;
        }


    }
    
}
