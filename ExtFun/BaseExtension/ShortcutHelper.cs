using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IWshRuntimeLibrary;

namespace SecuruStik.BaseExtension
{
    public class ShortcutUnit
    {
        public String LinkName;
        public String TargetPath;
        public String IconLocation;
        public String Description;
        public ShortcutUnit(String LinkName,String TargetPath,String IconLocation,String Description)
        {
            this.LinkName = LinkName;
            this.TargetPath = TargetPath;
            this.IconLocation = IconLocation;
            this.Description = Description;
        }
    }
    /// <summary>
    /// Create a shortcut(It needs ref the "Windows Script Host Object Model" COM)
    /// </summary>
    public class ShortcutHelper
    {
        public static void CreateShortcut(ShortcutUnit Shortcut)
        {
            String IconPath = Path.GetFullPath( Shortcut.IconLocation );
            String Target = Path.GetFullPath( Shortcut.TargetPath );

            WshShell shell = new WshShell();
            IWshShortcut shortcut  = (IWshShortcut)shell.CreateShortcut( Shortcut.LinkName );
            shortcut.TargetPath = Shortcut.TargetPath;
            shortcut.WindowStyle = 1;
            shortcut.IconLocation = Shortcut.IconLocation;
            shortcut.Description = Shortcut.Description;
            //Set up this propertise, the "current location" would be the application folder,
            //rather than the location of shortcut.
            shortcut.WorkingDirectory = Path.GetDirectoryName( Target );

            shortcut.Save();
        }
        
    }
}
