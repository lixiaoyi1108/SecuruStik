using NUnit.Framework;
using SecuruStik.PRE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SecuruStik.Tests
{
    [TestFixture]
    class DeviceTest
    {
        [Test]
        public void FindDevice()
        {
            var v = new VelostiScsi();
            while (true) Thread.Sleep(12*1000);
        }
    }
}
