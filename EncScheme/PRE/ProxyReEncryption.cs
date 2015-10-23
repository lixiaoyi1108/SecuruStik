/***********************************************************************
 * CLR Version :      4.0.30319.18444
 * Class Name  :      ProxyReEncryption
 * Name Space  :      PRE
 * File Name   :      ProxyReEncryption
 * Create Time :      2014/10/28 15:13:13
 * Author      :      JianYe,Huang
 * Brief       :      Cryptographic model( PRE & File encrypt/decrypt )
 ***********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using SecuruStik.PRE.Base;
using UserSetting.Key;
using SecuruStik.Exception;

namespace SecuruStik.PRE
{
    public class ProxyReEncryption
    {
        #region 0. Invoke & Fields

        #region 0.1. Key(C Invoke)
        private struct PRE_ptrPK
        {
            public IntPtr pk1;
            public IntPtr pk2;
        }
        private struct PRE_ptrSK
        {
            public IntPtr x1;
            public IntPtr x2;
        }
        private struct PRE_ptrKEY
        {
            public IntPtr pk;
            public IntPtr sk;
        }
        private struct PRE_ptrCipher
        {
            public IntPtr E;
            public IntPtr F;
            public IntPtr U;
            public IntPtr W;
        }
        #endregion Key(C Invoke)

        #region 0.2 Fields

        private const int KeySize = 64;
        private static int CipherBlockSize = 128;
        private const string PREDllPath = "PREC.dll";
        #endregion Fields

        #region 0.3 Init
        [DllImport( PREDllPath )]
        private extern static void PRE_Setup();
        [DllImport( PREDllPath )]
        private extern static void PRE_UnSetup();
        #endregion Init

        #region 0.4 key Enc & Dec
        [DllImport( PREDllPath , CharSet = CharSet.Ansi , CallingConvention = CallingConvention.Cdecl )]
        private extern static IntPtr PRE_KEYstr_Create();
        [DllImport( PREDllPath , CharSet = CharSet.Ansi , CallingConvention = CallingConvention.Cdecl )]
        private extern static IntPtr PRE_Encrypt( IntPtr key1 , IntPtr key2_pk , string m );
        [DllImport( PREDllPath , CharSet = CharSet.Ansi , CallingConvention = CallingConvention.Cdecl )]
        private extern static IntPtr PRE_Decrypt( IntPtr key2 , IntPtr C );
        #endregion key Enc & Dec

        #endregion Invoke & Fields

        #region 1. Initialization
        static ProxyReEncryption()
        {
            try
            {
                PRE_Setup();
            } catch ( System.Exception ex )
            {
                throw new SecuruStikException( SecuruStikExceptionType.PRE_Setup , "Failed to Setup PRE_C" , ex );
            }
        }
        public static Boolean UnSetup()
        {
            PRE_UnSetup();
            return true;
        }
        #endregion Initialization

        #region 2. key Opeartions

        //Create Key ptr/str
        public static PRE_KEY GenRandomKey()
        {
            return KeyPtr2KeyStr( CreateKeyPtr() );
        }
        public static PRE_KEY GenKey( String pk1 , String pk2 , String sk1 , String sk2 )
        {
            PRE_KEY key = new PRE_KEY();
            key.PK = new PRE_PK();
            key.SK = new PRE_SK();
            key.PK.PK1 = pk1;
            key.PK.PK2 = pk2;
            key.SK.X1 = sk1;
            key.SK.X2 = sk2;
            return key;
        }
        public static PRE_PK GenPK(String pk1,String pk2)
        {
            PRE_PK pk = new PRE_PK();
            pk.PK1 = pk1;
            pk.PK2 = pk2;
            return pk;
        }

        private static IntPtr CreateKeyPtr()
        {
            return PRE_KEYstr_Create();
        }
        private static IntPtr PKStr2PKPtr( PRE_PK pk )
        {
            PRE_ptrPK ptrPK = new PRE_ptrPK
            {
                pk1 = Marshal.StringToHGlobalAnsi( pk.PK1 ) ,
                pk2 = Marshal.StringToHGlobalAnsi( pk.PK2 )
            };
            IntPtr pPk = Marshal.AllocHGlobal( Marshal.SizeOf( ptrPK ) );
            Marshal.StructureToPtr( ptrPK , pPk , true );
            return pPk;
        }
        private static IntPtr KeyStr2KeyPtr( PRE_KEY key )
        {
            PRE_ptrPK ptrPK = new PRE_ptrPK { pk1 = Marshal.StringToHGlobalAnsi( key.PK.PK1 ) , pk2 = Marshal.StringToHGlobalAnsi( key.PK.PK2 ) };
            IntPtr pPk = Marshal.AllocHGlobal( Marshal.SizeOf( ptrPK ) );
            Marshal.StructureToPtr( ptrPK , pPk , true );

            PRE_ptrSK ptrSK = new PRE_ptrSK { x1 = Marshal.StringToHGlobalAnsi( key.SK.X1 ) , x2 = Marshal.StringToHGlobalAnsi( key.SK.X2 ) };
            IntPtr pSk = Marshal.AllocHGlobal( Marshal.SizeOf( ptrSK ) );
            Marshal.StructureToPtr( ptrSK , pSk , true );

            PRE_ptrKEY keyPtr = new PRE_ptrKEY { pk = pPk , sk = pSk };
            IntPtr pKey = Marshal.AllocHGlobal( Marshal.SizeOf( keyPtr ) );
            Marshal.StructureToPtr( keyPtr , pKey , true );
            return pKey;
        }
        private static PRE_KEY KeyPtr2KeyStr( IntPtr keyPtr )
        {
            PRE_ptrKEY ptrKey = (PRE_ptrKEY)Marshal.PtrToStructure( keyPtr , typeof( PRE_ptrKEY ) );
            PRE_PK pkey = (PRE_PK)Marshal.PtrToStructure( ptrKey.pk , typeof( PRE_PK ) );
            PRE_SK skey = (PRE_SK)Marshal.PtrToStructure( ptrKey.sk , typeof( PRE_SK ) );

            return new PRE_KEY { PK = pkey , SK = skey };
        }
        private static IntPtr GetPkPtrFromKey( IntPtr KeyPtr )
        {
            PRE_ptrKEY pKey = (PRE_ptrKEY)Marshal.PtrToStructure( KeyPtr , typeof( PRE_ptrKEY ) );
            return pKey.pk;
        }
        private static PRE_PK GetPkStrFromKey( IntPtr KeyPtr )
        {
            IntPtr ptrPK = GetPkPtrFromKey( KeyPtr );
            return (PRE_PK)Marshal.PtrToStructure( ptrPK , typeof( PRE_PK ) );
        }
        private static IntPtr GetSkPtrFromKey( IntPtr Keyptr )
        {
            PRE_ptrKEY pKey = (PRE_ptrKEY)Marshal.PtrToStructure( Keyptr , typeof( PRE_ptrKEY ) );
            return pKey.sk;
        }
        private static PRE_SK GetSkStrFromKey( IntPtr Keyptr )
        {
            IntPtr ptrSK = GetSkPtrFromKey( Keyptr );
            return (PRE_SK)Marshal.PtrToStructure( ptrSK , typeof( PRE_SK ) );
        }

        #endregion key Operations

        #region 3. Enc && Dec

        #region 3.1 key Enc & Dec

        public static PRE_Cipher KeyEncrypt( PRE_KEY key1 , PRE_PK key2_pk , String m )
        {
            IntPtr pKey1 = ProxyReEncryption.KeyStr2KeyPtr( key1 );
            IntPtr pPk2 = ProxyReEncryption.PKStr2PKPtr( key2_pk );

            IntPtr C = KeyEncrypt( pKey1 , pPk2 , m );
            PRE_Cipher CC = CipherPtr2CipherStr( C );

            return CC;
        }
        public static String KeyDecrypt( PRE_KEY key2 , PRE_Cipher CC )
        {
            IntPtr pKey2 = KeyStr2KeyPtr( key2 );
            IntPtr C = CipherStr2CipherPtr( CC );
            return KeyDecrypt( pKey2 , C );
        }

        private static IntPtr KeyEncrypt( IntPtr key1 , IntPtr key2_pk , String m )
        {
            return PRE_Encrypt( key1 , key2_pk , m );
        }
        private static String KeyDecrypt( IntPtr key2 , IntPtr C )
        {
            IntPtr pD = PRE_Decrypt( key2 , C );
            return Marshal.PtrToStringAnsi( pD , ProxyReEncryption.KeySize );
        }
        private static PRE_Cipher CipherPtr2CipherStr( IntPtr C )
        {
            PRE_ptrCipher CC = (PRE_ptrCipher)Marshal.PtrToStructure( C , typeof( PRE_ptrCipher ) );
            String e = Marshal.PtrToStringAuto( CC.E , CipherBlockSize );
            String f = Marshal.PtrToStringAnsi( CC.F );
            String u = Marshal.PtrToStringAnsi( CC.U );
            String w = Marshal.PtrToStringAnsi( CC.W );
            return new PRE_Cipher { E = e , F = f , U = u , W = w };
        }
        private static IntPtr CipherStr2CipherPtr( PRE_Cipher CC )
        {
            PRE_ptrCipher ptrC = new PRE_ptrCipher
            {
                E = Marshal.StringToHGlobalAuto( CC.E ) ,
                F = Marshal.StringToHGlobalAnsi( CC.F ) ,
                U = Marshal.StringToHGlobalAnsi( CC.U ) ,
                W = Marshal.StringToHGlobalAnsi( CC.W ) ,
            };
            IntPtr C = Marshal.AllocHGlobal( Marshal.SizeOf( ptrC ) );
            Marshal.StructureToPtr( ptrC , C , true );

            return C;
        }

        #endregion key Enc & Dec

        #region 3.2 Hash
        public static Byte[] GetHashCode( String filePath )
        {
            Byte[] hashBytes = null;
            //Get the plaintextHash value (SHA-256) of the file
            try
            {
                using ( FileStream fs = new FileStream( filePath , FileMode.Open , FileAccess.Read ) )
                {
                    hashBytes = SHA256Managed.Create().ComputeHash( fs );
                }
                return hashBytes;
            }
            catch ( System.Exception  )
            {
                return null;
            }
        }
        public static Boolean GetHashCode( String filePath , ref String hashValue )
        {
            //Get the plaintextHash value (SHA-256) of the file
            try
            {
                Byte[] hashBytes = null;
                using ( FileStream fs = new FileStream( filePath , FileMode.Open , FileAccess.Read ) )
                {
                    hashBytes = SHA256Managed.Create().ComputeHash( fs );
                }
                hashValue = BinaryStream.ByteArrayToHexString( hashBytes );
                return true;
            }
            catch ( System.Exception )
            {
                hashValue = string.Empty;
                return false;
            }

        }

        #endregion Hash

        #endregion Enc && Dec
    }
}