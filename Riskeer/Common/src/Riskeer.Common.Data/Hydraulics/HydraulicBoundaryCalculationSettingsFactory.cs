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
    /// Factory to create instances of <see cref="HydraulicBoundaryCalculationSettings"/>.
    /// </summary>
    public static class HydraulicBoundaryCalculationSettingsFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryCalculationSettings"/> based on the provided
        /// <paramref name="hydraulicBoundaryData"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryData">The <see cref="HydraulicBoundaryData"/> to create the
        /// <see cref="HydraulicBoundaryCalculationSettings"/> for.</param>
        /// <returns>A <see cref="HydraulicBoundaryCalculationSettings"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when either the hydraulic boundary database file path or the hydraulic
        /// location configuration database file path is <c>null</c>, is empty or consists of whitespaces.</exception>
        public static HydraulicBoundaryCalculationSettings CreateSettings(HydraulicBoundaryData hydraulicBoundaryData)
        {
            if (hydraulicBoundaryData == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryData));
            }

            return new HydraulicBoundaryCalculationSettings(hydraulicBoundaryData.FilePath,
                                                            hydraulicBoundaryData.HydraulicLocationConfigurationSettings.FilePath,
                                                            hydraulicBoundaryData.HydraulicLocationConfigurationSettings.UsePreprocessorClosure);
        }
    }
}