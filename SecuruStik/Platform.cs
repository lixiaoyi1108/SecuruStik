using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SecuruStik
{
    class Platform
    {

        public enum Name
        {
            Windows,
            Mac,
            Linux
        }

        /// <summary> For folder handling (dropbox, app paths, ...) which platform are we running on? </summary>
        public static Name Folders
        {
            get
            { return Name.Mac; }
        }

        public static Name Shell { get { return Folders;  } }
    }
}
