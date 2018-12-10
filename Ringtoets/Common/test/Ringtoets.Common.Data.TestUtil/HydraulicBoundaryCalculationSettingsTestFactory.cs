using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Factory which creates valid instances of <see cref="HydraulicBoundaryCalculationSettings"/>
    /// which can be used for testing.
    /// </summary>
    public static class HydraulicBoundaryCalculationSettingsTestFactory
    {
        /// <summary>
        /// Creates a <see cref="HydraulicBoundaryCalculationSettings"/> with valid values and
        /// without a preprocessor directory.
        /// </summary>
        /// <returns>A <see cref="HydraulicBoundaryCalculationSettings"/>.</returns>
        public static HydraulicBoundaryCalculationSettings CreateSettings()
        {
            const string directory = "D:\\HydraulicBoundaryLocationDataBase\\";
            return new HydraulicBoundaryCalculationSettings($"{directory}HBL.sqlite",
                                                            $"{directory}HLCD.sqlite",
                                                            string.Empty);
        }
    }
}