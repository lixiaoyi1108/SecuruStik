using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserSetting.Key
{
    #region 1.1 PRE(C#)
    public struct PRE_PK
    {
        public String PK1;
        public String PK2;
    }
    public struct PRE_SK
    {
        public String X1;
        public String X2;
    }
    public struct PRE_KEY
    {
        public PRE_PK PK;
        public PRE_SK SK;
    }
    [Serializable]
    public struct PRE_Cipher
    {
        public String E;
        public String F;
        public String U;
        public String W;
    }
    #endregion Structure for C#


}
