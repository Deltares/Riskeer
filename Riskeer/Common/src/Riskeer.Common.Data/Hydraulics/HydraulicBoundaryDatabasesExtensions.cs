using System;
using System.Collections.Generic;
using System.Linq;

namespace Riskeer.Common.Data.Hydraulics
{
    public static class HydraulicBoundaryDatabasesExtensions
    {
        /// <summary>
        /// Checks whether the hydraulic boundary database is linked to a database file.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database to check
        /// for being linked.</param>
        /// <returns><c>true</c> if the hydraulic boundary database is linked;
        /// <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when
        /// <paramref name="hydraulicBoundaryDatabase"/> is <c>null</c>.</exception>
        public static bool IsLinked(this HydraulicBoundaryDatabases hydraulicBoundaryDatabases)
        {
            if (hydraulicBoundaryDatabases == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabases));
            }

            return hydraulicBoundaryDatabases.HydraulicBoundaryDatabaseInstances.Any(
                hydraulicBoundaryDatabase => !string.IsNullOrEmpty(hydraulicBoundaryDatabase.FilePath));
        }

        /// <summary>
        /// Gets the preprocessor directory to be used during Hydra-Ring calculations.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database to get the
        /// effective preprocessor directory from.</param>
        /// <returns>A preprocessor directory, which is <see cref="string.Empty"/> when
        /// <see cref="HydraulicLocationConfigurationSettings.CanUsePreprocessor"/> or
        /// <see cref="HydraulicLocationConfigurationSettings.UsePreprocessor"/> is <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when
        /// <paramref name="hydraulicBoundaryDatabase"/> is <c>null</c>.</exception>
        public static string EffectivePreprocessorDirectory(this HydraulicBoundaryDatabases hydraulicBoundaryDatabases)
        {
            if (hydraulicBoundaryDatabases == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabases));
            }

            return hydraulicBoundaryDatabases.HydraulicLocationConfigurationSettings.CanUsePreprocessor
                   && hydraulicBoundaryDatabases.HydraulicLocationConfigurationSettings.UsePreprocessor
                       ? hydraulicBoundaryDatabases.HydraulicLocationConfigurationSettings.PreprocessorDirectory
                       : string.Empty;
        }

        public static IEnumerable<HydraulicBoundaryLocation> GetAllLocations(this HydraulicBoundaryDatabases hydraulicBoundaryDatabases)
        {
            return hydraulicBoundaryDatabases.HydraulicBoundaryDatabaseInstances.SelectMany(hbd => hbd.Locations);
        }

        public static HydraulicBoundaryDatabase GetHydraulicBoundaryDatabase(this HydraulicBoundaryDatabases hydraulicBoundaryDatabases,
                                                                             HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return hydraulicBoundaryDatabases.HydraulicBoundaryDatabaseInstances.First(
                hbd => hbd.Locations.Contains(hydraulicBoundaryLocation));
        }
    }
}