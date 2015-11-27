// Log4net.cs created with MonoDevelop
// User: baart_f at 2:18 PM 8/8/2007
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using log4net;
using NUnit.Framework;

namespace Core.Common.Tests.Gui
{
    [TestFixture]
    public class Log4net
    {
        [Test]
        public void CreateLogger()
        {
            ILog log = LogManager.GetLogger("Core.Common.tests.log4net");
            Assert.AreEqual(log.Logger.Name, "Core.Common.tests.log4net");
        }

        [Test]
        public void LoggingTypes()
        {
            ILog log = LogManager.GetLogger("Core.Common.tests.log4net");
            log.Info("LoggingTypes");
            log.Debug("LoggingTypes");
            log.Warn("LoggingTypes");
            log.Error("LoggingTypes");
            log.Fatal("LoggingTypes");
        }

        [Test]
        public void LoggingFormat()
        {
            ILog log = LogManager.GetLogger("Core.Common.tests.log4net");
            //The following methods somehow dont work under mono.....
            //TODO: find the cause of this
            log.InfoFormat("LoggingTypes {0}", 1);
            log.DebugFormat("LoggingTypes {0}", 1);
            log.WarnFormat("LoggingTypes {0}", 1);
            log.ErrorFormat("LoggingTypes {0}", 1);
            log.FatalFormat("LoggingTypes {0}", 1);
        }
    }
}