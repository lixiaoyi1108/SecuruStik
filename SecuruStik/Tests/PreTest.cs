using NUnit.Framework;
using SecuruStik.BaseExtension;
using SecuruStik.DB;
using SecuruStik.Opt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecuruStik.Tests
{
    [TestFixture]
    class PreTest
    {

        const string DROPBOX_USER_SECRET = "60e6dmakhbyz9bc";
        const string DROPBOX_USER_TOKEN = "5f3ckqfwnhoxyxqy";

        [Test]
        [STAThread]
        public void Init()
        {
            // unable to read application's config from VS Test runner (different app/process name)
            ConfigHelper.UserSettings.DropboxAuthUserSecret = DROPBOX_USER_SECRET;
            ConfigHelper.UserSettings.DropboxAuthUserToken = DROPBOX_USER_TOKEN;

            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

            DBox_User dx = new DBox_User();
        }
    }
}
