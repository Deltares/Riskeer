using System;
using Ringtoets.Common.Data.Hydraulics;
using Riskeer.HydraRing.Calculation.Data.Input;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Factory to create instances of <see cref="HydraRingCalculationSettings"/>.
    /// </summary>
    public static class HydraRingCalculationSettingsFactory
    {
        /// <summary>
        /// Creates a new instance of a <see cref="HydraRingCalculationSettings"/>
        /// based on a <see cref="HydraulicBoundaryCalculationSettings"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryCalculationSettings">The <see cref="HydraulicBoundaryCalculationSettings"/>
        /// to create a <see cref="HydraRingCalculationSettings"/> for.</param>
        /// <returns>A <see cref="HydraRingCalculationSettings"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryCalculationSettings"/>
        /// is <c>null</c>.</exception>
        public static HydraRingCalculationSettings CreateSettings(HydraulicBoundaryCalculationSettings hydraulicBoundaryCalculationSettings)
        {
            if (hydraulicBoundaryCalculationSettings == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryCalculationSettings));
            }

            return new HydraRingCalculationSettings(hydraulicBoundaryCalculationSettings.HlcdFilePath,
                                                    hydraulicBoundaryCalculationSettings.PreprocessorDirectory);
        }
    }
}