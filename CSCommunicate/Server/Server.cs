/***********************************************************************
 * CLR Version :      $civersion$
 * Class Name  :      Server
 * Name Space  :      Server
 * File Name   :      Server
 * Create Time :      2014/11/22 15:17:54
 * Author      :
 * Brief       :      
 * Modification history : 
 ***********************************************************************/
using System;
using System.Diagnostics;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DropNet;
using DropNet.Models;
using SecuruStik.BaseExtension;
using Server.DropBox;

namespace Server
{
    public class AccessToken
    {
        public String UserToken;
        public String UserSecret;
    }
    
    public class ServerControler
    {
        public PKM PublicKey;
        public SIM SharingInfo;

        private DropNetClient Client;
        private static String apiKey = "adqaukzw3605k8u";
        private static String appSecret = "lus2cykvur5g4iy";
        private AccessToken AccessToken = new AccessToken { UserToken = Properties.Settings.Default.UserToken , UserSecret = Properties.Settings.Default.UserSecret };

        public ServerControler()
        {
            this.Login();

            this.PublicKey = new PKM( this.Client );
            this.SharingInfo = new SIM( this.Client );
        }
        public void Login()
        {
            AccessToken at = this.AccessToken;

            this.Client = new DropNetClient(
                        ServerControler.apiKey ,
                        ServerControler.appSecret ,
                        at.UserToken , at.UserSecret , null );
        }
#if Login_UI
        private static String Node_AccessToken_UserToken = "AccessToken_UserToken";
        private static String Node_AccessToken_UserSecret = "AccessToken_UserSecret";
        public String AuthorizeUrl
        {
            get { return this.Client.BuildAuthorizeUrl(); }
        }
        private AccessToken _accessToken = null;
        public AccessToken AccessToken_UI
        {
            get
            {
                if ( _accessToken == null )
                {
                    String _accessToken_UserToken = ConfigHelper.GetAppConfig( ServerControler.Node_AccessToken_UserToken );
                    String _accessToken_UserSecret = ConfigHelper.GetAppConfig( ServerControler.Node_AccessToken_UserSecret );
                    if ( _accessToken_UserToken == null ||_accessToken_UserSecret == null )
                        return null;
                    else
                    {
                        _accessToken = new AccessToken();
                        _accessToken.UserToken = _accessToken_UserToken;
                        _accessToken.UserSecret = _accessToken_UserSecret;
                    }
                }
                return _accessToken;
            }
            set
            {
                if ( _accessToken == null )
                    _accessToken = new AccessToken();
                _accessToken.UserToken = value.UserToken;
                _accessToken.UserSecret = value.UserSecret;
                ConfigHelper.UpdateAppConfig( ServerControler.Node_AccessToken_UserToken , value.UserToken );
                ConfigHelper.UpdateAppConfig( ServerControler.Node_AccessToken_UserSecret , value.UserSecret );
            }
        }
        public Boolean Login_UI()
        {
            try
            {
                //Check prekeyring.xml to see if the Dropbox credential exists.
                AccessToken at = this.AccessToken;
                //If not,pop up a login window asking the user to enter his/her account && password
                if ( at == null )
                {
                    this.Client = new DropNet.DropNetClient( ServerControler.apiKey , ServerControler.appSecret );
                    this.Client.GetToken();
                    //Ask user to enter DropBox account and password
                    //SecuruStikMessageQueue.SendMessage_Splash_Hiden();
                    AuthorizeForm af = new AuthorizeForm( this.AuthorizeUrl );
                    af.ShowDialog();

                    if ( af.DialogResult == DialogResult.OK )
                    {
                        //return this.GetAccessToken();
                        try
                        {
                            UserLogin userLogin = this.Client.GetAccessToken();
                            this.AccessToken =
                                new AccessToken
                                {
                                    UserToken = userLogin.Token ,
                                    UserSecret = userLogin.Secret
                                };
                            return true;
                        } catch ( DropNet.Exceptions.DropboxException ex )
                        {
                            //SecuruStikException.ThrowSecuruStik( SecuruStikExceptionType.DropBoxControl_AccessToken ,
                            //    ex.Message + "\r\n" + ex.Message , ex );
                            return false;
                        }
                    }
                    else if ( af.DialogResult == DialogResult.Cancel )
                        return false;
                    else
                    {
                        MessageBoxEx.Show( "Login Dropbox failed." );
                        return false;
                    }
                }
                else
                {
                    this.Client = new DropNet.DropNetClient(
                                ServerControler.apiKey ,
                                ServerControler.appSecret ,
                                at.UserToken , at.UserSecret , null );
                }
            } catch ( System.Exception ex )
            {
                //SecuruStikException.ThrowSecuruStik(
                //    SecuruStikExceptionType.Initialization_DropBoxController ,
                //    ex.Message );
            }
            return true;
        }
#endif
    }
}
