using Core.Common.Utils.Reflection;

using NUnit.Framework;

namespace Core.Common.Gui.Test
{
    [TestFixture]
    public class SettingsHelperTest
    {
        [Test]
        public void ApplicationName_ReturnProductNameOfExecutingAssebly()
        {
            // Call
            var settings = SettingsHelper.ApplicationName;

            // Assert
            Assert.AreEqual(AssemblyUtils.GetExecutingAssemblyInfo().Product, settings);
        }

        [Test]
        public void ApplicationVersion_ReturnVersionOfExecutingAssebly()
        {
            // Call
            var settings = SettingsHelper.ApplicationVersion;

            // Assert
            Assert.AreEqual(AssemblyUtils.GetExecutingAssemblyInfo().Version, settings);
        }

        [Test]
        public void ApplicationCompany_ReturnCompanyOfExecutingAssebly()
        {
            // Call
            var settings = SettingsHelper.ApplicationCompany;

            // Assert
            Assert.AreEqual(AssemblyUtils.GetExecutingAssemblyInfo().Company, settings);
        }
    }
}