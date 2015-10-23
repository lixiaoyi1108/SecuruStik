/***********************************************************************
 * CLR Version :      4.0.30319.18444
 * Class Name  :      BinaryStream
 * Name Space  :      DBoxPRE.DBoxPRE_Base
 * File Name   :      BinaryStream
 * Create Time :      2014/10/5 15:25:01
 * Author      :      JianYe,Huang
 * Brief       :      Convertions between byte array and string
 * Modification history : NULL
 ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SecuruStik.PRE.Base
{
    class BinaryStream
    {
        public static string ByteArrayToString( Byte[] byteArray )
        {
            return Encoding.Default.GetString( byteArray );
        }
        public static Byte[] StringToByteArray( string str )
        {
            return Encoding.Default.GetBytes( str );
        }

        public static string ByteArrayToHexString( Byte[] byteArray )
        {
            return BitConverter.ToString( byteArray ).Replace( "-" , "" );
        }
        public static Byte[] HexStringToByteArray( string hex )
        {
            int NumberChars = hex.Length / 2;
            byte[] bytes = new byte[NumberChars];
            using ( var sr = new StringReader( hex ) )
            {
                for ( int i = 0 ; i < NumberChars ; i++ )
                    bytes[i] =
                      Convert.ToByte( new string( new char[2] { (char)sr.Read() , (char)sr.Read() } ) , 16 );
            }
            return bytes;
        }
        public static string StringToHexString( string str )
        {
            return ByteArrayToHexString(StringToByteArray( str ));
        }
        public static string HexStringToString( string str )
        {
            return ByteArrayToString(HexStringToByteArray( str ));
        }
    }
}
