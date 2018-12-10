using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class HydraulicBoundaryCalculationSettingsTestFactoryTest
    {
        [Test]
        public void CreateSettings_Always_ReturnsExpectedValues()
        {
            // Call
            HydraulicBoundaryCalculationSettings settings = HydraulicBoundaryCalculationSettingsTestFactory.CreateSettings();

            // Assert
            Assert.AreEqual("D:\\HydraulicBoundaryLocationDataBase\\HBL.sqlite", settings.HydraulicBoundaryDatabaseFilePath);
            Assert.AreEqual("D:\\HydraulicBoundaryLocationDataBase\\HLCD.sqlite", settings.HlcdFilePath);
            Assert.IsEmpty(settings.PreprocessorDirectory);
        }
    }
}