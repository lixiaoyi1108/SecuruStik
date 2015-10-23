/***********************************************************************
 * CLR Version :      4.0.30319.18444
 * Class Name  :      Config
 * Name Space  :      Model
 * File Name   :      Config
 * Create Time :      2014/10/17 10:37:05
 * Author      :      JianYe,Huang
 * Brief       :      Configure for database
 * Modification history : NULL
 ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SecuruStik.DB
{
    public class Config
    {
        #region Local/Dropbox file path
        public static string PrekeyringFile_Dir = "./bini";
        public static string PrekeyringFile_Name = "prekeyring.db";
        public static string PrekeyringFile_Path = Path.Combine( PrekeyringFile_Dir , PrekeyringFile_Name );
        public static string PrekeyringFile_Password = "lus2cykvur5g4iy";
        #endregion Local/Dropbox file path
    }
}
