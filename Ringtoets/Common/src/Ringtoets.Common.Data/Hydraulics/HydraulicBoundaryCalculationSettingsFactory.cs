using System;
using System.IO;

namespace Ringtoets.Common.Data.Hydraulics
{
    /// <summary>
    /// Factory to create instances of <see cref="HydraulicBoundaryCalculationSettings"/>.
    /// </summary>
    public static class HydraulicBoundaryCalculationSettingsFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryCalculationSettings"/>
        /// based on a <see cref="HydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="HydraulicBoundaryDatabase"/>
        /// to create a <see cref="HydraulicBoundaryCalculationSettings"/> for.</param>
        /// <returns>The <see cref="HydraulicBoundaryCalculationSettings"/>.</returns>
        public static HydraulicBoundaryCalculationSettings CreateSettings(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            string hydraulicBoundaryDatabaseFilePath = hydraulicBoundaryDatabase.FilePath;
            string hlcdFilePath = null;

            if (!string.IsNullOrWhiteSpace(hydraulicBoundaryDatabaseFilePath))
            {
                string directory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
                hlcdFilePath = Path.Combine(directory, "HLCD.sqlite");
            }

            return new HydraulicBoundaryCalculationSettings(hydraulicBoundaryDatabaseFilePath,
                                                            hlcdFilePath,
                                                            hydraulicBoundaryDatabase.EffectivePreprocessorDirectory());
        }
    }
}