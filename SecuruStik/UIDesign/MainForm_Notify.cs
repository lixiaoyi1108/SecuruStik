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

namespace SecuruStik.UIDesign
{
    public enum NotifyType : int
    {
        Eject,
        Plug,

        ReceiveFile,
        ShareBegin,
        ShareEnd,
        DownloadBegin,
        DownloadEnd,
        DownloadFailed,

        SyncBegin,
        SyncEnd,
    }
    public class NotifyUnit
    {
        public NotifyType Type;
        public String TipTitle;
        public String TipText;
        public Int16 TypeNum;

        public static Int16 RECEIVEFILE = 0;
        public static Int16 SHAREFILE = 1;
        public static Int16 DOWNLOAD = 2;
        public static Int16 SYNC = 3;

        public NotifyUnit( NotifyType type,String tipTitle,String tipText )
        {
            this.Type = type;
            switch (this.Type)
            {
                case NotifyType.Eject:
                case NotifyType.Plug: this.TypeNum = -1; break;
                case NotifyType.ReceiveFile: this.TypeNum = NotifyUnit.RECEIVEFILE; break;
                case NotifyType.ShareBegin:
                case NotifyType.ShareEnd:this.TypeNum = NotifyUnit.SHAREFILE; break;
                case NotifyType.SyncBegin:
                case NotifyType.SyncEnd:this.TypeNum = NotifyUnit.SYNC; break;
            }
            this.TipTitle = tipTitle;
            this.TipText = tipText;
        }
    }
    public partial class SecuruStik_MainForm : MetroForm
    {
        private NotifyUnit[] NotifyArr = new NotifyUnit[]{null,null,null,null};
        private String[] NotifyTag = new String[] { 
            DropBoxController.Local_ShareInfoFolder,
            String.Empty,
            DropBoxController.Local_DownloadFolder,
            DropBoxController.Local_SecFolder
        };
        private readonly int NotifyTipShowTime = 500;

        //Notify
        private Boolean NotifyOn = true;
        private void Notify_Timer_Tick(object sender, EventArgs e)
        {
            if (NotifyOn)
            {
                NotifyUnit notifyUnit = NotifyArr.FirstOrDefault(ntf => ntf != null);
                if (notifyUnit != null)
                {
                    this.notifyIcon.ShowBalloonTip(
                        this.NotifyTipShowTime,
                        notifyUnit.TipTitle,
                        notifyUnit.TipText,
                       ToolTipIcon.Info
                        );
                    this.notifyIcon.Tag = NotifyTag[notifyUnit.TypeNum];
                    NotifyArr[notifyUnit.TypeNum] = null;
                }
                else
                    this.Notify_Timer.Stop();
            }
            else
            {
                Boolean hasNewNotify = false;
                for (int i = 0; i != NotifyArr.Length; i++)
                {
                    NotifyUnit ntu = NotifyArr[i];
                    if (ntu != null)
                    {
                        hasNewNotify = true;
                        ntu = null;
                    }
                }
                if (hasNewNotify)
                    Notify_Eject();
            }
        }

        private void AddNotify(NotifyUnit notifyUnit)
        {
            if(!this.Notify_Timer.Enabled)
                this.Notify_Timer.Start();
            this.NotifyArr[notifyUnit.TypeNum] = notifyUnit;
        }

        private void Notify_Eject()
        {
            this.notifyIcon.ShowBalloonTip(
                       this.NotifyTipShowTime, "Sync error", "The Velosti has been ejected.",
                       ToolTipIcon.Warning);
        }
        private void Notify_Plug()
        {
            this.notifyIcon.ShowBalloonTip(
                       this.NotifyTipShowTime, "", "The Velosti has been plugged.",
                       ToolTipIcon.Warning);
        }
        private void Notify_Disconnect()
        {
            this.notifyIcon.ShowBalloonTip(
                       this.NotifyTipShowTime , "Sync error" , "Disconnect with server.\r\nPlease check your network setting or firewall..." ,
                       ToolTipIcon.Warning );
        }
        private void Notify_ReceiveShareFile()
        {
            this.AddNotify(new NotifyUnit(
                NotifyType.ReceiveFile,
                "",
                "A new file is shared to you(click to view)."));
            //this.notifyIcon.Tag = DropBoxController.Local_ShareInfoFolder;
        }
        private void Notify_Share_End(String fileName, String id_To)
        {
            String sFileName = CompressToShortString(fileName);
            this.AddNotify(new NotifyUnit(
                NotifyType.ShareEnd, 
                String.Format("Sharing '{0}'", sFileName), 
                String.Format("The link of '{0}' has been sent to '{1}'.", sFileName, id_To)));
            
            //this.notifyIcon.Tag = String.Empty;
        }
        private void Notify_Download_Begin(String fileName, String id_From)
        {
            this.AddNotify(new NotifyUnit(
                NotifyType.DownloadBegin,
                "Downloading...", 
                String.Format("Downloading the file '{0}' from '{1}'.",
                        this.CompressToShortString(fileName),
                        this.CompressToShortString(id_From))
                        ));
            //this.notifyIcon.Tag = String.Empty;
        }
        private void Notify_Download_End(String filePath)
        {
            this.AddNotify(new NotifyUnit(
                NotifyType.DownloadEnd,
                "Decrypte successfully.",
                String.Format("Downloading the file '{0}' from '{1}'.",
                        String.Format("'{0}' is downloaded(click to view).", this.CompressToShortString(filePath)))
                        ));
        }
        private void Notify_Download_Failed()
        {
            this.AddNotify(new NotifyUnit(
                NotifyType.DownloadFailed,
                "Download Error...",
                String.Format("This file is invaild or is being uploaded to dropbox."))
                        );
        }
        private void Notify_LocalSecuruStikFolder_Created(String filePath)
        {
            if (filePath.Contains(DropBoxController.Local_SecFolder))
                this.Notify_Sync_Begin();
            else if (filePath.Contains(DropBoxController.Local_ShareInfoFolder))
                this.Notify_ReceiveShareFile();
            else if (filePath.Contains(DropBoxController.Local_DownloadFolder))
                this.Notify_Download_End(filePath);
        }
        private void Notify_Sync_Begin()
        {
            long TaskCount = VelostiScsi.TaskCount;

            if (TaskCount != 0)
            {
                this.AddNotify(
                    new NotifyUnit(
                    NotifyType.SyncBegin,
                    "Syncing",
                    String.Format("{0} files remaining...\r\nSpeed : {1:F2} M/s", VelostiScsi.TaskCount.ToString(), VelostiScsi.Speed)
                       ));
                //this.notifyIcon.Tag = DropBoxController.Local_SecFolder;
            }
        }
        private void Notify_Sync_End()
        {
            this.AddNotify(new NotifyUnit(
                    NotifyType.SyncBegin,
                    "",
                    "Up to date"
                       ));
        }

        private String CompressToShortString(String file)
        {
            if (file.Length > 23)
            {
                return String.Format("{0}...{1}", file.Substring(0, 11), file.Substring(file.Length - 11));
            }
            return file;
        }

        #region Action
        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int x = (Cursor.Position.X < Screen.PrimaryScreen.WorkingArea.Width - this.Width) ?
                    Cursor.Position.X : (Screen.PrimaryScreen.WorkingArea.Width - this.Width);
                int y = (Cursor.Position.Y < this.Height) ?
                    0 : Cursor.Position.Y - this.Height;
                this.Location = new Point(x, y);

                this.Visible = true;
                if (this.Visible) { this.Activate(); }
            }
            else if (e.Button == MouseButtons.Left)
            {
                this.OpenSecuruSticSecFolder();
            }

        }
        private void NotifyIcon_MouseMove(object sender, MouseEventArgs e)
        {
            long TaskCount = VelostiScsi.TaskCount;
            String currentFile = this.CompressToShortString(Path.GetFileName(VelostiScsi.CurrentTaskName));
            if (TaskCount == 0)
            {
                if (this.IsChecking)
                    this.notifyIcon.Text = "Indexing...";
                else
                {
                    this.notifyIcon.Text = "SecuruStik\r\nUp to date.";
                }
            }
            else
            {
                this.notifyIcon.Text = String.Format("Syncing : \r\n{0} files remaining...\r\nSpeed : {1:F2} M/s", VelostiScsi.TaskCount.ToString(), VelostiScsi.Speed);
            }
        }
        private void NotifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            NotifyIcon notify = sender as NotifyIcon;
            if (notify == null) return;
            String natigative = notify.Tag as String;
            if (natigative == String.Empty || !Directory.Exists(natigative))
                return;
            Win32API.BringWindowToTopOrCreate(natigative);
        }
        #endregion

    }
}
