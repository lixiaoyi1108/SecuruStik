/***********************************************************************
 * CLR Version :      $civersion$
 * Class Name  :      TaskUnit
 * Name Space  :      SecuruStik.PRE
 * File Name   :      TaskUnit
 * Create Time :      2015/4/9 18:34:08
 * Author      :
 * Brief       :
 * Modification history : 
 ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecuruStik.PRE
{
    public enum TaskType:byte
    {
        Encrypt,
        Decrypt
    }
    public class TaskUnit:IComparable
    {
        public TaskType TaskType;
        public String PlaintextFilePath;
        public String EncryptedFilePath;
        public String Key;
        public UInt64 Size;
        public String HashValueOfPlaintext;
        public String HashValueOfCryptotext;
        public TaskUnit( String plaintextFilePath,String encryptedFilePath,String key = "",UInt64 size = 0 ,String hashValueOfPlaintext = "" ,String hashValueOfCryptotext = "" )
        {
            this.PlaintextFilePath = plaintextFilePath;
            this.EncryptedFilePath = encryptedFilePath;
            this.Size = size;
            this.Key = key;
            this.HashValueOfPlaintext = hashValueOfPlaintext;
            this.HashValueOfCryptotext = hashValueOfCryptotext;
        }

        public int CompareTo( object obj )
        {
            TaskUnit tu2 = obj as TaskUnit;
            if ( tu2 == null ) return 1;
            if ( this.Size < tu2.Size ) return -1;
            else if ( this.Size == tu2.Size ) return 0;
            else return 1;
        }
    }
}
