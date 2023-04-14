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
using System.Linq;

namespace Riskeer.Common.Data.Hydraulics
{
    /// <summary>
    /// Factory to create instances of <see cref="HydraulicBoundaryCalculationSettings"/>.
    /// </summary>
    public static class HydraulicBoundaryCalculationSettingsFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryCalculationSettings"/> based on the provided
        /// <paramref name="hydraulicBoundaryData"/> and <paramref name="hydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryData">The <see cref="HydraulicBoundaryData"/> to create the
        /// <see cref="HydraulicBoundaryCalculationSettings"/> for.</param>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> to create the
        /// <see cref="HydraulicBoundaryCalculationSettings"/> for.</param>
        /// <returns>A <see cref="HydraulicBoundaryCalculationSettings"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="hydraulicBoundaryLocation"/> is not part of <paramref name="hydraulicBoundaryData"/>;</item>
        /// <item>the hydraulic boundary database file path is <c>null</c>, is empty or consists of whitespaces;</item>
        /// <item>the location configuration database file path is <c>null</c>, is empty or consists of whitespaces.</item>
        /// </list>
        /// </exception>
        public static HydraulicBoundaryCalculationSettings CreateSettings(HydraulicBoundaryData hydraulicBoundaryData,
                                                                          HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (hydraulicBoundaryData == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryData));
            }

            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation));
            }

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = hydraulicBoundaryData.HydraulicBoundaryDatabases.FirstOrDefault(hbd => hbd.Locations.Contains(hydraulicBoundaryLocation));
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentException($"'{nameof(hydraulicBoundaryLocation)}' is not part of '{nameof(hydraulicBoundaryData)}'.");
            }

            return new HydraulicBoundaryCalculationSettings(hydraulicBoundaryData.HydraulicLocationConfigurationDatabase.FilePath,
                                                            hydraulicBoundaryDatabase.FilePath,
                                                            hydraulicBoundaryDatabase.UsePreprocessorClosure);
        }
    }
}