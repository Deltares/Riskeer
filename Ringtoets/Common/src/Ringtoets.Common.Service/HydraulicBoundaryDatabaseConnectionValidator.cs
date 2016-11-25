// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.Service.Properties;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Class responsible for validating the connection of a <see cref="HydraulicBoundaryDatabase"/>.
    /// </summary>
    public static class HydraulicBoundaryDatabaseConnectionValidator
    {
        /// <summary>
        /// Validates the connection of the specified database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>An error message if a problem was found. Returns null in case no problems were found.</returns>
        public static string Validate(HydraulicBoundaryDatabase database)
        {
            if (database == null)
            {
                return Resources.HydraulicBoundaryDatabaseConnectionValidator_No_hydraulic_boundary_database_imported;
            }

            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(database.FilePath);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                return string.Format(Resources.Hydraulic_boundary_database_connection_failed_0_,
                                     validationProblem);
            }

            return null;
        }
    }
}