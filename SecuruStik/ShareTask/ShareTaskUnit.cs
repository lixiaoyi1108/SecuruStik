using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SecuruStik.Protocal;

namespace SecuruStik.Contorllers
{
    public class ShareTaskUnit
    {
        public String ID_TO;
        public String FilePath;
        public String Key;

        public PublicKey PK;
        public String CpyRef;
        public ShareTaskUnit( String id_to , String filePath , String key )
        {
            this.ID_TO = id_to;
            this.FilePath = filePath;
            this.Key = key;

            this.PK = null;
            this.CpyRef = String.Empty;
        }
    }
}
