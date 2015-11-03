/***********************************************************************
 * CLR Version :      $civersion$
 * Class Name  :      SerializeHelper
 * Name Space  :      SecuruStik.BaseExtension
 * File Name   :      SerializeHelper
 * Create Time :      2015/3/2 14:32:16
 * Author      :
 * Brief       :
 * Modification history : 
 ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SecuruStik.BaseExtension
{
    public class SerializeHelper
    {

        // TODO: this seems to be unused but DeSerialize does occur in the code
        public static byte[] Serialize( Object r )
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bfer = new BinaryFormatter();
                bfer.Serialize( ms , r );
                return ms.ToArray();
            }
            catch ( System.Exception ) { return null; }
        }
        public static Object DeSerialize( byte[] bytes )
        {
            try
            {
                MemoryStream ms = new MemoryStream( bytes );
                BinaryFormatter bfer = new BinaryFormatter();
                return bfer.Deserialize( ms );
            }
            catch ( System.Exception ) { return null; }
        }
    }
}
