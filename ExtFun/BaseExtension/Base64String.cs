using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecuruStik.BaseExtension
{
    public static class Base64String
    {
        public static string StringToBase64String( String value )
        {
            byte[] binBuffer = ( new UnicodeEncoding() ).GetBytes( value );
            int base64ArraySize = (int)Math.Ceiling( binBuffer.Length / 3d ) * 4;
            char[] charBuffer = new char[base64ArraySize];
            Convert.ToBase64CharArray( binBuffer , 0 , binBuffer.Length , charBuffer , 0 );
            string s = new string( charBuffer );
            return s;
        }

        public static string Base64StringToString( string base64 )
        {
            char[] charBuffer = base64.ToCharArray();
            byte[] bytes = Convert.FromBase64CharArray( charBuffer , 0 , charBuffer.Length );
            return ( new UnicodeEncoding() ).GetString( bytes );
        }

        public static string BytesToBase64String( byte[] binBuffer )
        {
            int base64ArraySize = (int)Math.Ceiling( binBuffer.Length / 3d ) * 4;
            char[] charBuffer = new char[base64ArraySize];
            Convert.ToBase64CharArray( binBuffer , 0 , binBuffer.Length , charBuffer , 0 );
            string s = new string( charBuffer );
            return s;
        }

        public static Byte[] Base64ToBytes( string base64 )
        {
            char[] charBuffer = base64.ToCharArray();
            byte[] bytes = Convert.FromBase64CharArray( charBuffer , 0 , charBuffer.Length );
            return bytes;
        }
    }
}
