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
        [Test]
        [STAThread]
        public void Init()
        {
            ConfigHelper.CustomConfigFile = "SecuruStik.exe.config";
            var c = PreKeyring.AccessToken;
            DBox_User dx = new DBox_User();
        }
    }
}
