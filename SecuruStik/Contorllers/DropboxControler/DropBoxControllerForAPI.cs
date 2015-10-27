using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DropNet.Exceptions;
using DropNet.Models;
using SecuruStik.DB;
using SecuruStik;
using SecuruStik.MessageQueue;
using SecuruStikSettings;

namespace SecuruStik.DropBox
{
    public partial class DropBoxController
    {
        #region 0. Fields

        private DropNet.DropNetClient client;
        public String AuthorizeUrl
        {
            get { return this.client.BuildAuthorizeUrl(); }
        }

        private AccountInfo _accountInfo = null;
        public AccountInfo DropboxAccountInfo
        {
            get
            {
                if (this._accountInfo == null)
                    this._accountInfo = this.client.AccountInfo();
                return _accountInfo;
            }
        }

        private String _email = null;
        public String Email
        {
            get
            {
                if (_email == null)
                    _email = this.DropboxAccountInfo.email;
                return _email;
            }
        }

        private readonly int RetryTimes = 2;

        #endregion

        #region 1. Authorization && Login

        private Boolean GetAccessToken()
        {
            try
            {
                UserLogin userLogin = this.client.GetAccessToken();
                PreKeyring.AccessToken =
                    new SecuruStik.DB.AccessToken
                    {
                        UserToken = userLogin.Token ,
                        UserSecret = userLogin.Secret
                    };
                return true;

            } catch ( DropNet.Exceptions.DropboxException ex )
            {
                throw new SecuruStikException(
                    SecuruStikExceptionType.DropBoxControl_AccessToken ,
                    "Failed to Get the Access token" ,
                    ex );
            }
        }

        // TODO: this is displaying a Form which is not so good for unit testing or anything
        public Boolean Login()
        {
            try
            {
                //Check prekeyring.xml to see if the Dropbox credential exists.
                SecuruStik.DB.AccessToken at = PreKeyring.AccessToken;
                //If not,pop up a login window asking the user to enter his/her account && password
                if ( at == null )
                {
                    this.client = new DropNet.DropNetClient( AppSetting.apiKey , AppSetting.appSecret );
                    this.client.GetToken();

                    //Ask user to enter DropBox account and password
                    SecuruStikMessageQueue.SendMessage_Splash_Hiden();
                    AuthorizeForm af = new AuthorizeForm( this.AuthorizeUrl );
                    af.ShowDialog();

                    if ( af.DialogResult == DialogResult.OK )
                    {
                        return this.GetAccessToken();
                    }
                    else if ( af.DialogResult == DialogResult.Cancel )
                        return false;
                }
                else
                {
                    this.client = new DropNet.DropNetClient(
                                AppSetting.apiKey ,
                                AppSetting.appSecret ,
                                at.UserToken , at.UserSecret , null );
                }
            } catch ( System.Exception ex )
            {
                throw new SecuruStikException(
                     SecuruStikExceptionType.DropBoxControl_Login ,
                     "Failed to Login the dropbox" ,
                     ex );
            }
            return true;
        }

        #endregion Authorization && Login

        #region 2. File operations(DropBox API)

        public MetaData GetMetaData(String path)
        {
            try
            {
                return this.client.GetMetaData(path, null);
            }
            catch (DropboxException) { }
            return null;
        }

        public Boolean Download( String dropbox_filePath, String local_savePath, String fileName )
        {
            try
            {
                if ( Directory.Exists( local_savePath ) == false )
                    Directory.CreateDirectory( local_savePath );
                MetaData file = this.client.GetMetaData( dropbox_filePath, null );
                if ( file == null )
                    return false;
                if ( file.Is_Dir == true )
                {
                    foreach ( MetaData subFile in file.Contents )
                    {
                        Download( subFile.Path , local_savePath + "\\" + fileName , subFile.Name );
                    }
                }
                else
                {
                    client.GetFileAsync( dropbox_filePath ,
                       ( response ) =>
                       {
                           byte[] download_file = response.RawBytes;
                           using ( FileStream fs = new FileStream( local_savePath + "\\" + fileName , FileMode.CreateNew , FileAccess.Write ) )
                           {
                               fs.Write( download_file , 0 , download_file.Length );
                           }
                       } ,
                       ( error ) =>
                       {
                           throw new SecuruStikException(
                               SecuruStikExceptionType.DropBoxControl_Download ,
                               "DownLoadError" ,
                               error );
                       } );
                }
            } catch ( SecuruStikException ex )
            {
                throw ex;
            } catch ( System.Exception ex )
            {
                throw new SecuruStikException( SecuruStikExceptionType.DropBoxControl_Download , "DownLoadError" , ex );
            }
            return true;
        }
        public Byte[] Download( String dropbox_filePath )
        {
            try
            {
                MetaData file = this.client.GetMetaData(dropbox_filePath, null);
                if (file == null ||
                    file.Is_Dir == true ||
                    file.Is_Deleted ||
                    file.Bytes == 0)
                {
                    return null;
                }
                else
                {
                    return client.GetFile(dropbox_filePath);
                }
            }
            catch (DropboxException)
            {
                return null;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        public Boolean Upload( String dropbox_path, String local_file )
        {
            FileInfo f = new FileInfo(local_file);
            if (f.Exists == false)
                return false;
            try
            {
                byte[] fileData;
                using (FileStream fs = f.OpenRead())
                {
                    fileData = new byte[fs.Length];
                    fs.Read(fileData, 0, (int)fs.Length);
                    fs.Close();
                }
                client.UploadFileAsync(dropbox_path, f.Name, fileData,
                            (response) =>
                            {
                            },
                            (error) =>
                            {

                            });
            }
            catch ( DropboxException ex )
            {
                throw new SecuruStikException(
                                  SecuruStikExceptionType.DropBoxControl_Upload , "UpLoadError" , ex );
            }
            catch (System.Exception ex)
            {
                throw new SecuruStikException(
                                  SecuruStikExceptionType.DropBoxControl_Upload, "UpLoadError", ex);
            }

            return true;

        }
        public Boolean Upload( String dropbox_path, String fileName, Byte[] rawData )
        {
            try
            {
                client.UploadFileAsync(dropbox_path, fileName, rawData,
                            (response) =>
                            {
                            },
                            (error) =>
                            {
                                throw new SecuruStikException(
                                                  SecuruStikExceptionType.DropBoxControl_Upload, "UpLoadError", error);
                            });
            }
            catch (System.Exception ex)
            {
                throw new SecuruStikException(
                                  SecuruStikExceptionType.DropBoxControl_Upload, "UpLoadError", ex);
            }

            return true;

        }
        public String GetCopyRef(String file)
        {
            int haveTryCounter = 0;
            while (true)
            {
                try
                {
                    CopyRefResponse cpy = this.client.GetCopyRef(file);
                    return cpy.Copy_Ref;
                }
                catch (DropboxException e)
                {
                    haveTryCounter++;
                    if (haveTryCounter >= this.RetryTimes)
                        return String.Empty;
                }
            }

        }
        public void CopyAsync( String copyRef, String downloadPath )
        {
            this.client.CopyFromCopyRefAsync(copyRef, downloadPath,
                (response) =>
                {
                },
                       (error) =>
                       {
                           PreKeyring.SharingFile_Delete(copyRef);
                           SecuruStikMessageQueue.SendMessage_Download_Failed();
                           
                       }
            );
        }
        public MetaData Copy( String copyRef, String downloadPath )
        {
            try
            {
                MetaData ret = this.client.CopyFromCopyRef(copyRef, downloadPath);
                return ret;
            }
            catch (DropNet.Exceptions.DropboxException ex)
            {
                throw new SecuruStikException(
                       SecuruStikExceptionType.DropBoxControl_Copy, "GetCopyError", ex);
            }

        }
        public Boolean Delete(string dropbox_path)
        {
            try
            {
                client.Delete(dropbox_path);
            }
            catch (DropboxException) { }
            return true;
        }

        #endregion File operations

        #region 3. Others

        public String Share( String dropbox_path )
        {
            try
            {
                ShareResponse share = client.GetShare(dropbox_path);
                return share.Url;
            }
            catch (DropboxException ex)
            {
                MessageBox.Show(new Form { TopMost = true }, ex.Message);
            }
            return String.Empty;
        }
        public Boolean Move(string file, string newPath) { return true; }
        public Image GetThumbnail(string file) { return null; }
        public string GetMediaLink(string mediaLink) { return null; }
        public Boolean CreateFolder(string path) { return true; }
        public void GetDelta(String cursor)
        {
            try
            {
                DeltaPage delta = this.client.GetDelta(cursor);
                List<DeltaEntry> deltaEntries = delta.Entries;
                foreach (DeltaEntry deltaEntry in deltaEntries)
                {
                    MessageBox.Show(new Form { TopMost = true }, deltaEntry.MetaData.Path + " : " + client.GetMetaData(deltaEntry.MetaData.Path, null).Hash);//
                }
                if (delta.Has_More == true)
                {
                    GetDelta(delta.Cursor);
                }
            }

            catch (DropNet.Exceptions.DropboxException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        #endregion Others
    }
}
