using DeltaShell.Core;
using DeltaShell.Tests.TestObjects;
using NUnit.Framework;

namespace DeltaShell.Tests.Core
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
            Assert.AreEqual(1,callCount);
        }
    
    }
}