using Ringtoets.HydraRing.Calculation.Data.Input;

namespace Ringtoets.HydraRing.Calculation.TestUtil
{
    /// <summary>
    /// Factory that creates simple instances of <see cref="HydraRingCalculationSettings"/>
    /// which can be used for testing.
    /// </summary>
    public static class HydraRingCalculationSettingsTestFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="HydraRingCalculationSettings"/>
        /// with empty values.
        /// </summary>
        /// <returns>A <see cref="HydraRingCalculationSettings"/>.</returns>
        public static HydraRingCalculationSettings CreateSettings()
        {
            return new HydraRingCalculationSettings(string.Empty, string.Empty);
        }
    }
}