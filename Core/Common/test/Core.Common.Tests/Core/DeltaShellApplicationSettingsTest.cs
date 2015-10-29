using Core.Common.Base;
using Core.Common.Tests.TestObjects;
using NUnit.Framework;

namespace Core.Common.Tests.Core
{
    [TestFixture]
    public class DeltaShellApplicationSettingsTest
    {
        [Test]
        public void PropertyChangedWorksForWrappedSettings()
        {
            var settings = new TestSettings();

            var wrappedSettings = new DeltaShellApplicationSettings(settings);
            int callCount = 0;
            wrappedSettings.PropertyChanged += (s, e) => callCount++;
            wrappedSettings["Name"] = "kees";
            Assert.AreEqual(1, callCount);
        }
    }
}