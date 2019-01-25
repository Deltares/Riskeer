using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Riskeer.HydraRing.Calculation.Data.Input;

namespace Ringtoets.Common.Service.TestUtil
{
    /// <summary>
    /// A test helper which can be used to assert instances of <see cref="HydraRingCalculationSettings"/>.
    /// </summary>
    public static class HydraRingCalculationSettingsTestHelper
    {
        /// <summary>
        /// Asserts whether the <see cref="HydraRingCalculationSettings"/> contains the correct
        /// data from <see cref="HydraulicBoundaryCalculationSettings"/>.
        /// </summary>
        /// <param name="expectedSettings">The <see cref="HydraulicBoundaryCalculationSettings"/>
        /// to assert against.</param>
        /// <param name="actualSettings">The <see cref="HydraRingCalculationSettings"/> to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The HLCD file paths do not match.</item>
        /// <item>The preprocessor directories do not match.</item>
        /// </list>
        /// </exception>
        public static void AssertHydraRingCalculationSettings(HydraulicBoundaryCalculationSettings expectedSettings,
                                                              HydraRingCalculationSettings actualSettings)
        {
            Assert.AreEqual(expectedSettings.HlcdFilePath, actualSettings.HlcdFilePath);
            Assert.AreEqual(expectedSettings.PreprocessorDirectory, actualSettings.PreprocessorDirectory);
        }
    }
}