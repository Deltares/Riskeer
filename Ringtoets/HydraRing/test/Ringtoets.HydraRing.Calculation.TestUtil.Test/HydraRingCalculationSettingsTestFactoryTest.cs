using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Data.Input;

namespace Riskeer.HydraRing.Calculation.TestUtil.Test
{
    [TestFixture]
    public class HydraRingCalculationSettingsTestFactoryTest
    {
        [Test]
        public void CreateSettings_Always_ReturnsHydraRingCalculationSettings()
        {
            // Call
            HydraRingCalculationSettings settings = HydraRingCalculationSettingsTestFactory.CreateSettings();

            // Assert
            Assert.IsEmpty(settings.HlcdFilePath);
            Assert.IsEmpty(settings.PreprocessorDirectory);
        }
    }
}