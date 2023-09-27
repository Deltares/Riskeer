// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.HydraRing;
using Riskeer.Common.Service.Properties;

namespace Riskeer.Common.Service
{
    /// <summary>
    /// Class responsible for validating the connection of a <see cref="HydraulicBoundaryData"/> instance.
    /// </summary>
    public static class HydraulicBoundaryDataConnectionValidator
    {
        /// <summary>
        /// Validates the connection of the provided <paramref name="hydraulicBoundaryData"/> in relation to the provided
        /// <paramref name="hydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryData">The hydraulic boundary data to validate.</param>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location to validate for.</param>
        /// <returns>An error message if a problem was found; <c>null</c> in case no problems were found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="hydraulicBoundaryLocation"/> is not part of
        /// <paramref name="hydraulicBoundaryData"/>.</exception>
        public static string Validate(HydraulicBoundaryData hydraulicBoundaryData, HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (hydraulicBoundaryData == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryData));
            }

            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation));
            }

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = hydraulicBoundaryData.GetHydraulicBoundaryDatabaseForLocation(hydraulicBoundaryLocation);

            string validationProblem = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(
                hydraulicBoundaryData.HydraulicLocationConfigurationDatabase.FilePath,
                hydraulicBoundaryDatabase.FilePath,
                hydraulicBoundaryDatabase.UsePreprocessorClosure);

            if (!string.IsNullOrEmpty(validationProblem))
            {
                return string.Format(Resources.Hydraulic_boundary_database_connection_failed_0_, validationProblem);
            }

            if (!HydraulicBoundaryDataHelper.IsCorrectVersion(hydraulicBoundaryDatabase.Version, hydraulicBoundaryDatabase.FilePath))
            {
                return string.Format(Resources.Hydraulic_boundary_database_mismatching_version_in_file_0_, hydraulicBoundaryDatabase.FilePath);
            }

            if (!HydraulicBoundaryDataHelper.IsCorrectHrdFile(hydraulicBoundaryData.HydraulicLocationConfigurationDatabase.FilePath, hydraulicBoundaryDatabase.FilePath))
            {
                return string.Format(Resources.Hydraulic_location_configuration_database_refers_to_hydraulic_boundary_database_that_does_not_correspond_with_0, hydraulicBoundaryDatabase.FilePath);
            }

            return null;
        }
    }
}