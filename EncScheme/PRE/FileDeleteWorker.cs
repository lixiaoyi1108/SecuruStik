using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SecuruStik.DB;
using System.IO;
using System.Threading;

namespace SecuruStik.PRE
{
    public class FileDeleteWorker
    {
        private Queue<String> deleteFileQueue = new Queue<String>();
        private List<String> deleteDirQueue = new List<String>();
        private BackgroundWorker Worker = new BackgroundWorker();
        public FileDeleteWorker()
        {
            this.Worker.DoWork += DeleteDoWork;
        }
        private void DeleteDoWork( object sender , DoWorkEventArgs e )
        {
            while ( this.deleteFileQueue.Count != 0 || this.deleteDirQueue.Count != 0 )
            {
                while ( this.deleteFileQueue.Count != 0 )
                {
                    String dropboxpath = this.deleteFileQueue.Peek();
                    try
                    {
                        File.Delete(dropboxpath);
                        this.deleteFileQueue.Dequeue();
                        PreKeyring.FileInfo_Delete(dropboxpath);
                    }
                    catch (System.Exception) { }
                }
                for(int i = deleteDirQueue.Count - 1 ; i >=0 ; i-- )
                {
                    String path = deleteDirQueue[i];
                    try
                    {
                        Directory.Delete(path,true);
                        deleteDirQueue.RemoveAt(i);
                    }
                    catch (System.Exception) { }
                }
            }
        }
        public void DeleteFileOrDir( String dropboxpath )
        {
            if ( File.Exists( dropboxpath ) )
                this.deleteFileQueue.Enqueue( dropboxpath );
            else if(Directory.Exists(dropboxpath))
            {
                Queue<String> dirQueue = new Queue<string>();
                dirQueue.Enqueue(dropboxpath);
                while(dirQueue.Count != 0 )
                {
                    String dir = dirQueue.Dequeue();
                    String[] subDirs = Directory.GetDirectories(dir);
                        String[] fs = Directory.GetFiles(dropboxpath);
                        foreach (String subDir in subDirs)
                        {
                            dirQueue.Enqueue(subDir);
                        }
                        foreach (String f in fs)
                        {
                            this.deleteFileQueue.Enqueue(f);
                        }
                }
                this.deleteDirQueue.Add(dropboxpath);
            }
            if(!this.Worker.IsBusy)
                this.Worker.RunWorkerAsync();
        }
    }
}
