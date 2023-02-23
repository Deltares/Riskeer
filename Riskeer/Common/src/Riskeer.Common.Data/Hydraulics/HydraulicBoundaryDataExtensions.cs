// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

namespace Riskeer.Common.Data.Hydraulics
{
    /// <summary>
    /// Extension methods for <see cref="HydraulicBoundaryData"/>.
    /// </summary>
    public static class HydraulicBoundaryDataExtensions
    {
        /// <summary>
        /// Checks whether the hydraulic boundary data is linked to a hydraulic location configuration database.
        /// </summary>
        /// <param name="hydraulicBoundaryData">The hydraulic boundary data to check for being linked.</param>
        /// <returns><c>true</c> if the hydraulic boundary data is linked; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryData"/> is <c>null</c>.</exception>
        public static bool IsLinked(this HydraulicBoundaryData hydraulicBoundaryData)
        {
            if (hydraulicBoundaryData == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryData));
            }

            return !string.IsNullOrEmpty(hydraulicBoundaryData.HydraulicLocationConfigurationSettings.FilePath)
                   || !string.IsNullOrEmpty(hydraulicBoundaryData.FilePath);
        }
    }
}