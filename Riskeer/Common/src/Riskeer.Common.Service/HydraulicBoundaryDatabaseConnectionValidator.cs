// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.HydraRing;
using Riskeer.Common.Service.Properties;

namespace Riskeer.Common.Service
{
    /// <summary>
    /// Class responsible for validating the connection of all relevant <see cref="HydraulicBoundaryDatabase"/>
    /// instances (as they are used within one or more calculations).
    /// </summary>
    public static class HydraulicBoundaryDatabaseConnectionValidator
    {
        /// <summary>
        /// Validates all relevant <paramref name="hydraulicBoundaryDatabases"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabases">The hydraulic boundary databases to validate.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations to perform calculations for.</param>
        /// <returns>An error message if a problem was found; <c>null</c> in case no problems were found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryDatabases"/> or
        /// <paramref name="hydraulicBoundaryLocations"/> is <c>null</c>.</exception>
        public static string Validate(IEnumerable<HydraulicBoundaryDatabase> hydraulicBoundaryDatabases,
                                      IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
        {
            if (hydraulicBoundaryDatabases == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabases));
            }

            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }

            if (hydraulicBoundaryDatabases.Any(hbd => !hbd.IsLinked()))
            {
                return Resources.HydraulicBoundaryDatabaseConnectionValidator_No_hydraulic_boundary_database_imported;
            }

            string validationProblem = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(hydraulicBoundaryDatabases,
                                                                                                   hydraulicBoundaryLocations);

            if (!string.IsNullOrEmpty(validationProblem))
            {
                return string.Format(Resources.Hydraulic_boundary_database_connection_failed_0_,
                                     validationProblem);
            }

            return null;
        }

        /// <summary>
        /// Validates the connection of the provided hydraulic boundary database.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database to validate.</param>
        /// <returns>An error message if a problem was found; <c>null</c> in case no problems were found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryDatabase"/> is <c>null</c>.</exception>
        public static string Validate(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            if (!hydraulicBoundaryDatabase.IsLinked())
            {
                return Resources.HydraulicBoundaryDatabaseConnectionValidator_No_hydraulic_boundary_database_imported;
            }

            string validationProblem = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(
                hydraulicBoundaryDatabase.FilePath,
                hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.FilePath,
                hydraulicBoundaryDatabase.EffectivePreprocessorDirectory(),
                hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessorClosure);

            if (!string.IsNullOrEmpty(validationProblem))
            {
                return string.Format(Resources.Hydraulic_boundary_database_connection_failed_0_,
                                     validationProblem);
            }

            return null;
        }
    }
}