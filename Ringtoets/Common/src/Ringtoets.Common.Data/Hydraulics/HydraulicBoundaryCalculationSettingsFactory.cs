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
        /// <returns>A <see cref="HydraulicBoundaryCalculationSettings"/>.</returns>
        public static HydraulicBoundaryCalculationSettings CreateSettings(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            string hydraulicBoundaryDatabaseFilePath = hydraulicBoundaryDatabase.FilePath;
            string effectivePreprocessorDirectory = hydraulicBoundaryDatabase.EffectivePreprocessorDirectory();
            if (string.IsNullOrWhiteSpace(hydraulicBoundaryDatabaseFilePath))
            {
                return new HydraulicBoundaryCalculationSettings(hydraulicBoundaryDatabaseFilePath, 
                    effectivePreprocessorDirectory);
            }

            string directory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
            string hlcdFilePath = Path.Combine(directory, "HLCD.sqlite");
            return new HydraulicBoundaryCalculationSettings(hydraulicBoundaryDatabaseFilePath,
                                                            hlcdFilePath,
                                                            effectivePreprocessorDirectory);
        }
    }
}