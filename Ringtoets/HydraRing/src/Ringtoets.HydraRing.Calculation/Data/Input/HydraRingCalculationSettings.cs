using System;
using Core.Common.Util;

namespace Ringtoets.HydraRing.Calculation.Data.Input
{
    /// <summary>
    /// Class which holds all the general information to run a Hydra-Ring calculation
    /// </summary>
    public class HydraRingCalculationSettings
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraRingCalculationSettings"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary
        /// database file.</param>
        /// <param name="hydraulicBoundaryLocationsConfigurationFilePath">The path which points to
        /// the hydraulic boundary locations configuration file.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory to be used for the calculations.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="preprocessorDirectory"/>
        /// is <c>null</c></exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="hydraulicBoundaryDatabaseFilePath"/>
        /// or <paramref name="hydraulicBoundaryLocationsConfigurationFilePath"/>of the input parameters contains invalid
        /// characters.</exception>
        public HydraRingCalculationSettings(string hydraulicBoundaryDatabaseFilePath,
                                            string hydraulicBoundaryLocationsConfigurationFilePath,
                                            string preprocessorDirectory)
        {
            if (preprocessorDirectory == null)
            {
                throw new ArgumentNullException(nameof(preprocessorDirectory));
            }

            IOUtils.ValidateFilePath(hydraulicBoundaryDatabaseFilePath);
            IOUtils.ValidateFilePath(hydraulicBoundaryLocationsConfigurationFilePath);

            HydraulicBoundaryDatabaseFilePath = hydraulicBoundaryDatabaseFilePath;
            HydraulicBoundaryLocationsConfigurationFilePath = hydraulicBoundaryLocationsConfigurationFilePath;
            PreprocessorDirectory = preprocessorDirectory;
        }

        /// <summary>
        /// Gets the hydraulic boundary database filepath.
        /// </summary>
        public string HydraulicBoundaryDatabaseFilePath { get; }

        /// <summary>
        /// Gets the hydraulic locations configurations filepath.
        /// </summary>
        public string HydraulicBoundaryLocationsConfigurationFilePath { get; }

        /// <summary>
        /// Gets the preprocessor director.
        /// </summary>
        public string PreprocessorDirectory { get; }
    }
}