/***********************************************************************
 * CLR Version :      4.0.30319.18444
 * Class Name  :      StructOpt
 * Name Space  :      Model.Base
 * File Name   :      StructOpt
 * Create Time :      2014/10/17 16:12:37
 * Author      :      JianYe,Huang
 * Brief       :      Operations for the structure
 * Modification history : NULL
 ***********************************************************************/
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace SecuruStik.DB.Base
{
    internal class StructOpt
    {
        #region 1. Base
        private static FieldInfo[] GetFieldInfosOfStructure( Type structType )
        {
            return structType.GetFields( System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance );
        }
        private static Object[] GetAttributesOfField( FieldInfo info )
        {
            return info.GetCustomAttributes( false );
        }
        public static String GetStructureName( Type structType )
        {
            String structName = structType.ToString();
            return structName.Substring( structName.LastIndexOf( '.' ) + 1 );
        }
        #endregion Base

        private static int GetMaxSizeOfField(FieldInfo info)
        {
            return ((Field)info.GetCustomAttributes( false )[0]).MaxSize;
        }
        public static String[] GetFieldNamesOfStructure( Type structType )
        {
            FieldInfo[] infos = StructOpt.GetFieldInfosOfStructure( structType );
            String[] fieldNames = new String[infos.Length];
            int index = 0;
            foreach ( FieldInfo info in infos )
            {
                fieldNames[index++] = info.Name;
            }
            return fieldNames;
        }
        public static int[] GetMaxSizeOfStructure( Type structType )
        {
            FieldInfo[] infos = StructOpt.GetFieldInfosOfStructure( structType );
            int[] maxSize = new int[infos.Length];
            int index = 0;
            foreach ( FieldInfo info in infos )
            {
                maxSize[index++] = StructOpt.GetMaxSizeOfField( info );
            }
            return maxSize;
        }
        public static Dictionary<String,int> GetFieldInfo( Type structType )
        {
            FieldInfo[] infos = StructOpt.GetFieldInfosOfStructure( structType );
            Dictionary<String , int> fields = new Dictionary<String , int>();
            foreach ( FieldInfo info in infos )
            {
                fields.Add( info.Name , StructOpt.GetMaxSizeOfField(info) );
            }
            return fields;
        }
    }
}
