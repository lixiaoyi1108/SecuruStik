using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecuruStik.Exception
{
    [Serializable]
    public class SecuruStikException:ApplicationException
    {
        #region 1. Constructor

        public SecuruStikException() { }
        public SecuruStikException( String message ) : base( message ) { }
        public SecuruStikException( String message , System.Exception inner ) : base( message , inner ) { }

        public SecuruStikException( SecuruStikExceptionType exType , String message )
            : base( message )
        {
            Tracer.Log( exType.ToString() , message );
        }
        public SecuruStikException( SecuruStikExceptionType exType , String message , System.Exception inner )
            : base( message , inner )
        {
            Tracer.Log( inner, exType.ToString() , message , inner.Message );
        }

        #endregion Constructor
        
    }

    public enum SecuruStikExceptionType:long
    {
        Init_Database,

        Init_EncryptionController,
        Init_PrekeyFileController,
        Init_DropBoxController,
        Init_ProxyServerController,
        Init_UserKey ,
        Init_Environment ,

        DropBoxControl_GetToken,
        DropBoxControl_AccessToken,
        DropBoxControl_Login,
        DropBoxControl_Download,
        DropBoxControl_Upload,
        DropBoxControl_GetCopyRef,
        DropBoxControl_Copy,
        DropBoxControl_Delete,

        PRE_Setup,

        ProxyServerControl_Login,

        Velosti_NotPlugin,
    }
}
