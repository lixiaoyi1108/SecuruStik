using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters;


using SecuruStik.PRE;
using System.Security.Cryptography;
using SecuruStik.PRE.Base;
using System.IO;
using UserSetting.Key;
namespace SecuruStik.PRE
{
    class Program
    {

        static void Main( string[] args )
        {
            PRETEST();

            //VelostiScsi loopback = new VelostiScsi();
        }
        static void PRETEST()
        {
            String m = "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f";
            PRE_KEY key1 = ProxyReEncryption.GenRandomKey();
            PRE_KEY key2 = ProxyReEncryption.GenRandomKey();
            PRE_Cipher keyC = ProxyReEncryption.KeyEncrypt( key1 , key2.PK , m );

            String mm = ProxyReEncryption.KeyDecrypt( key2 , keyC );
            Console.WriteLine( mm + mm.Length );
            Console.Read();
        }
    }
}