using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using SecuruStik.DB;
using SecuruStik.MessageQueue;
using SecuruStik.PRE;
using SecuruStikSettings;

namespace SecuruStik.DropBox
{
    public partial class DropBoxController
    {
        // TODO: WTF this seems to be used from different threads. Needs locks
        private List<String> LostKeyFile = new List<String>();
        private List<String> EncryptTaskList = new List<String>();
        private List<String> DecryptTaskList = new List<String>();
        private Thread SynchronizeAsyncThread = null;
        private VelostiScsi LoopBackTool;
        private FileDeleteWorker DeleteWorker = new FileDeleteWorker();

        public void SynchronizeAsync()
        {
            if (this.SynchronizeAsyncThread == null || this.SynchronizeAsyncThread.IsAlive == false)
            {
                this.SynchronizeAsyncThread = new Thread(Synchronize);
                this.SynchronizeAsyncThread.IsBackground = true;
            }
            this.SynchronizeAsyncThread.Start();
        }

        public void Synchronize()
        {
            SecuruStikMessageQueue.SendMessage_Checking_Begin();
            Synchronize_Local2DropBox(AppSetting.Path_Local_SecuruStikFolder);
            Synchronize_DropBox2Local(DropBoxController.DropBox_SecFolder);
            SecuruStikMessageQueue.SendMessage_Checking_End();
        }

        private void Synchronize_DropBox2Local(string localPath)
        {
            String dirFullPath_Local = this.DropboxSecuruStikFolder2SecuruStikFolder(localPath);
            if (!Directory.Exists(dirFullPath_Local))
                Directory.CreateDirectory(dirFullPath_Local);
            try
            {
                string[] files = Directory.GetFiles(localPath);
                string[] dirs = Directory.GetDirectories(localPath);

                String lostKeyFile = String.Empty;
                foreach (string file in files)
                {
                    this.AddDecryptTask(file);
                }
                foreach (string dir in dirs)
                {
                    Synchronize_DropBox2Local(dir);
                }

            } catch (System.Exception ex) {
                log.Error("Synchronizing Dropbox -> Local", ex);
            }
        }

        public void Synchronize_Local2DropBox(string localPath)
        {
            String dirFullPath_Dropbox = this.SecuruStikFolder2DropboxSecuruStikFolder(localPath);
            if (Directory.Exists(dirFullPath_Dropbox) == false)
                Directory.CreateDirectory(dirFullPath_Dropbox);

            try
            {
                string[] files = Directory.GetFiles(localPath);
                string[] dirs = Directory.GetDirectories(localPath);

                foreach (String file in files)
                {
                    this.AddEncryptTask(file);
                }

                foreach (string dir in dirs)
                {
                    Synchronize_Local2DropBox(dir);
                }
            }
            catch (System.Exception ex)
            {
                log.Error("Synchronizing Local -> Dropbox", ex);
            }
        }

        public void AddDecryptTask(String file)
        {
            FileInfo f = new FileInfo( file );
            String fileFullPath_Dropbox = f.FullName;

            //If the file is being encrypted,skip it .
            if (this.LoopBackTool.IsWaitingOrDecrypting(fileFullPath_Dropbox))
                return;

            String fileFullPath_Local = this.DropboxSecuruStikFolder2SecuruStikFolder(fileFullPath_Dropbox);
            String hashValue_Local = String.Empty;

            // TODO: error handling (it's returning bool (which is ignored) but should perhaps throw  exception)
            ProxyReEncryption.GetHashCode(fileFullPath_Local, ref hashValue_Local);

            FileMetaData fmd = PreKeyring.FileInfo_Query(fileFullPath_Local);
            if (fmd == null)
            {
                this.LostKeyFile.Add(fileFullPath_Dropbox);
            }
            else if (! File.Exists(fileFullPath_Local) || fmd.PlainTextHash != hashValue_Local)
            {
                // If the local file does not exist, redecrypt the file
                string hashValue_Dropbox = "";
                ProxyReEncryption.GetHashCode( fileFullPath_Local , ref hashValue_Dropbox );
                this.LoopBackTool.AddDecryptTask(fileFullPath_Dropbox, fileFullPath_Local, hashValue_Dropbox,(UInt64)f.Length, fmd.Key);
            }
        }

        public void AddEncryptTask(string file)
        {
            FileInfo f = new FileInfo( file );

            String fileFullPath_Local = f.FullName;

            //if the file is beeing decrypted, skip it.
            if (this.LoopBackTool.IsWaitingOrEncrypting(file))
                return;

            string fileFullPath_Dropbox = this.SecuruStikFolder2DropboxSecuruStikFolder(fileFullPath_Local);

            string hashValue_DropBox = String.Empty;
            ProxyReEncryption.GetHashCode(fileFullPath_Dropbox, ref hashValue_DropBox);

            FileMetaData fmd = PreKeyring.FileInfo_Query(fileFullPath_Local);

            if (File.Exists(fileFullPath_Dropbox) == false || fmd == null ||
                hashValue_DropBox != fmd.CryptTextHash)
            {
                try
                {
                    String hashValue_Local = String.Empty;
                    ProxyReEncryption.GetHashCode( fileFullPath_Local , ref hashValue_Local );
                    this.LoopBackTool.AddEncryptTask( fileFullPath_Local , fileFullPath_Dropbox , hashValue_Local , (UInt64)f.Length );
                }
                catch (System.Exception ex)
                {
                    log.ErrorFormat("Add Encrypt Task for {0}", file, ex);
                }
            }
        }

        public void DeleteDropboxFile(string dropboxpath )
        {
            this.DeleteWorker.DeleteFileOrDir(dropboxpath);
        }
    }
}
