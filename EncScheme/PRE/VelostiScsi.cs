using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using SecuruStik.DB;
using SecuruStik.PRE.Base;
using SecuruStik.MessageQueue;

namespace SecuruStik.PRE
{
    public partial class VelostiScsi
    {
        private log4net.ILog log = Tracer.GetClassLogger();

        private void TaskWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while ( VelostiScsi.TaskCount != 0 && VelostiScsi.IsPlugin_PubDrive)
            {
                try
                {
                    this.GetNextTask();
                    this.WriteKeyFile();
                    this.RunLoopBackTool();
                    VelostiScsi.CurrentTask = null;
                }
                catch (System.Exception ex)
                {
                    log.Error("Worker encountered an error", ex);
                    this.Error();
                }
            }
        }
        private void TaskWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.IsBusy = false;
            SecuruStikMessageQueue.SendMessage_Sync_End();
        }

        private String GenerateKey()
        {
            this.Aes.GenerateKey();
            return BinaryStream.ByteArrayToHexString( this.Aes.Key );
        }

        private void WriteKeyFile()
        {
            String key = Regex.Replace( VelostiScsi.CurrentTask.Key , @"(.{2})" , "$1 " );
            File.WriteAllText( VelostiScsi.KeyFile , key , Encoding.ASCII );
        }

        private void GetNextTask()
        {
            if ( VelostiScsi.EncryptWaitingCount == 0 && VelostiScsi.DecryptWaitingCount == 0) return;
            else if ( VelostiScsi.EncryptWaitingCount == 0 )
                this.GetNextDecryptTask();
            else if ( VelostiScsi.DecryptWaitingCount == 0 )
                this.GetNextEncryptTask();
            else
            {
                if ( VelostiScsi.EncryptWaitingList[0].Size < VelostiScsi.DecryptWaitingList[0].Size )
                    this.GetNextEncryptTask();
                else
                    this.GetNextDecryptTask();
            }
        }

        private void GetNextEncryptTask()
        {
            VelostiScsi.CurrentTask = VelostiScsi.EncryptWaitingList[0];
            VelostiScsi.EncryptWaitingList.Remove( VelostiScsi.CurrentTask );
            while ( this.IsFileLocked( VelostiScsi.CurrentTask.PlaintextFilePath ) )
            {
                VelostiScsi.EncryptWaitingList.Add( VelostiScsi.CurrentTask );
                VelostiScsi.CurrentTask = VelostiScsi.EncryptWaitingList[0];
                VelostiScsi.EncryptWaitingList.Remove( VelostiScsi.CurrentTask );
            }
            VelostiScsi.CurrentTask.Key = this.GenerateKey();
        }

        private void GetNextDecryptTask()
        {
            VelostiScsi.CurrentTask = VelostiScsi.DecryptWaitingList[0];
            VelostiScsi.DecryptWaitingList.Remove( VelostiScsi.CurrentTask );
            while ( this.IsFileLocked( VelostiScsi.CurrentTask.EncryptedFilePath ) )
            {
                VelostiScsi.DecryptWaitingList.Add( VelostiScsi.CurrentTask );
                VelostiScsi.CurrentTask = VelostiScsi.DecryptWaitingList[0];
                VelostiScsi.DecryptWaitingList.Remove( VelostiScsi.CurrentTask );
            }
        }

        private void RecordSpeed()
        {
            VelostiScsi.Speed = 0.1 * VelostiScsi.Speed + 
                0.9 * CurrentTask.Size / (1024*1024) / ( this.StopWatch.ElapsedMilliseconds / 1000.0 );
            this.StopWatch.Reset();
        }

        private void RunLoopBackTool()
        {
            if ( VelostiScsi.CurrentTask.TaskType == TaskType.Encrypt )
                RunLoopBackTool_Enc();
            else
                RunLoopBackTool_Dec();
        }

        private void RunLoopBackTool_Enc()
        {
            File.Copy(CurrentTask.PlaintextFilePath, CurrentTask.EncryptedFilePath);

            // this doesn't work so let's disable it to be able to use the app anyway
            /*
            String batArguments = String.Format("{0} \"{1}\" \"{2}\" \"{3}\" \"{4}\" ",
                        VelostiScsi.PubDrive,
                        VelostiScsi.LoopBackTool,
                        VelostiScsi.KeyFile,
                        VelostiScsi.CurrentTask.PlaintextFilePath ,
                        VelostiScsi.CurrentTask.EncryptedFilePath
                        );
            this.StopWatch.Start();
            Boolean isSuccess = VelostiScsi.ExecuteCommand( VelostiScsi.EncBatFile , batArguments );
            this.StopWatch.Stop();
            this.RecordSpeed();
            if ( isSuccess && File.Exists( VelostiScsi.CurrentTask.EncryptedFilePath ) )
            {
                String hashValueOfCryptotext = String.Empty;
                ProxyReEncryption.GetHashCode( VelostiScsi.CurrentTask.EncryptedFilePath , ref hashValueOfCryptotext );
                VelostiScsi.CurrentTask.HashValueOfCryptotext = hashValueOfCryptotext;
                this.UpdateTaskInfo();
            }
            else
            {
                this.EncryptError();
            }
            */

        }

        private void RunLoopBackTool_Dec()
        {
            File.Copy(CurrentTask.EncryptedFilePath, CurrentTask.PlaintextFilePath);
            /*
            String batArguments = String.Format("{0} \"{1}\" \"{2}\" \"{3}\" \"{4}\" ",
                           VelostiScsi.PubDrive,
                           VelostiScsi.LoopBackTool,
                           VelostiScsi.KeyFile,
                           VelostiScsi.CurrentTask.EncryptedFilePath ,
                           VelostiScsi.CurrentTask.PlaintextFilePath
                           );
            Boolean isSuccess = VelostiScsi.ExecuteCommand(VelostiScsi.DecBatFile, batArguments);
            if ( isSuccess && File.Exists( VelostiScsi.CurrentTask.PlaintextFilePath ) )
            {
                String hashValueOfPlaintext = String.Empty;
                ProxyReEncryption.GetHashCode( VelostiScsi.CurrentTask.PlaintextFilePath , ref hashValueOfPlaintext );
                VelostiScsi.CurrentTask.HashValueOfCryptotext = hashValueOfPlaintext;
                this.UpdateTaskInfo();
            }
            else
            {
                this.DecryptError();
            }
            */
        }

        public void AddDecryptTask(String fileFullPath_Dropbox, String fileFullPath_Local, String cryptTextHash,UInt64 fileSize, String Key)
        {
            TaskUnit task = new TaskUnit(fileFullPath_Local, fileFullPath_Dropbox, Key, size:fileSize,hashValueOfCryptotext: cryptTextHash);
            task.TaskType = TaskType.Decrypt;
            VelostiScsi.DecryptWaitingList.Add( task );
            VelostiScsi.DecryptWaitingList.Sort();
            this.StartWork();
        }

        public void AddEncryptTask( String fileFullPath_Local , String fileFullPath_Dropbox , String hashValue_Local , UInt64 fileSize )
        {
            TaskUnit task = new TaskUnit(fileFullPath_Local, fileFullPath_Dropbox, size:fileSize , hashValueOfPlaintext: hashValue_Local);
            task.TaskType = TaskType.Encrypt;
            VelostiScsi.EncryptWaitingList.Add( task );
            VelostiScsi.EncryptWaitingList.Sort();
            this.StartWork();
        }

        private void StartWork()
        {
            if (!this.IsBusy)
            {
                if ( String.IsNullOrWhiteSpace( VelostiScsi.PubDrive ) )
                {
                    SecuruStikMessageQueue.SendMessage_Notify_Eject();
                    return;
                }
                SecuruStikMessageQueue.SendMessage_Sync_Begin();
                this.IsBusy = true;
                this.Worker.RunWorkerAsync();
            }
        }

        public Boolean IsWaitingOrEncrypting(String filePath_Dropbox)
        {
            if ( VelostiScsi.CurrentTask == null )
                return false;
            else if ( VelostiScsi.CurrentTask.EncryptedFilePath == filePath_Dropbox )
            {
                return true;
            }
            else if ( VelostiScsi.EncryptWaitingList.Count == 0 )
            {
                return false;
            }
            else
                return VelostiScsi.EncryptWaitingList.Exists( etw => etw.EncryptedFilePath == filePath_Dropbox );
        }

        public Boolean IsWaitingOrDecrypting(String filePath_Local)
        {
            if ( VelostiScsi.CurrentTask == null )
                return false;
            else if ( VelostiScsi.CurrentTask.PlaintextFilePath == filePath_Local )
            {
                return true;
            }
            else if ( VelostiScsi.DecryptWaitingList.Count == 0 )
            {
                return false;
            }
            else
                return VelostiScsi.DecryptWaitingList.Exists( dtw => dtw.PlaintextFilePath == filePath_Local );
        }

        private void UpdateTaskInfo()
        {
            PreKeyring.FileInfo_Update
                                        (
                                             new SecuruStik.DB.FileMetaData
                                             {
                                                 FileName = Path.GetFileName( VelostiScsi.CurrentTask.PlaintextFilePath ) ,
                                                 FilePath = VelostiScsi.CurrentTask.PlaintextFilePath ,
                                                 Key = VelostiScsi.CurrentTask.Key ,
                                                 PlainTextHash = VelostiScsi.CurrentTask.HashValueOfPlaintext ,
                                                 CryptTextHash = VelostiScsi.CurrentTask.HashValueOfCryptotext
                                             }
                                        );
        }

        #region Error
        private void Error()
        {
            if ( VelostiScsi.CurrentTask == null ) return;

            if ( VelostiScsi.CurrentTask.TaskType == TaskType.Encrypt )
                this.EncryptError();
            else
                this.DecryptError();
            VelostiScsi.CurrentTask = null;
        }

        private void EncryptError()
        {
            try
            {
                if ( VelostiScsi.CurrentTask != null &&
                    File.Exists( VelostiScsi.CurrentTask.EncryptedFilePath ) )
                    File.Delete( VelostiScsi.CurrentTask.EncryptedFilePath );
                //VelostiScsi.ErrorList.Add( VelostiScsi.CurrentTask );
                VelostiScsi.EncryptWaitingList.Add(VelostiScsi.CurrentTask);
            } catch (System.Exception ex) {
                // TODO: why catch?
                log.Error("EncryptError", ex);
            }
        }

        private void DecryptError()
        {
            try
            {
                if ( VelostiScsi.CurrentTask != null &&
                    File.Exists( VelostiScsi.CurrentTask.PlaintextFilePath ) )
                    File.Delete( VelostiScsi.CurrentTask.PlaintextFilePath );
                //VelostiScsi.ErrorList.Add( VelostiScsi.CurrentTask );
                VelostiScsi.DecryptWaitingList.Add(VelostiScsi.CurrentTask);
            } catch (System.Exception ex) {
                // TODO: why catch?
                log.Error("DecryptError", ex);
            }
        }
        #endregion 
    }
}