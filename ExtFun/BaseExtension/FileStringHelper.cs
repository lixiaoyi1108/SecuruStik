using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SecuruStik.BaseExtension
{
    public class FileStringHelper
    {
        public static String GetNonConflictFileName( String fullPath )
        {
            String destPath;
            int index = 2;
            String oldFileNameWithoutExtension = Path.GetFileNameWithoutExtension( fullPath );

            String oldFileName = Path.GetFileName( fullPath );

            destPath = fullPath;

            while ( File.Exists( destPath ) == true )
            {
                destPath = fullPath.Replace( oldFileNameWithoutExtension , String.Format( "{0}({1})" , oldFileNameWithoutExtension , index++ ) );
            }
            return destPath;
        }
    }
}
