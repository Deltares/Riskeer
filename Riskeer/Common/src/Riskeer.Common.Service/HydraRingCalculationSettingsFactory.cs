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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.HydraRing.Calculation.Data.Input;

namespace Riskeer.Common.Service
{
    /// <summary>
    /// Factory to create instances of <see cref="HydraRingCalculationSettings"/>.
    /// </summary>
    public static class HydraRingCalculationSettingsFactory
    {
        /// <summary>
        /// Creates a new instance of a <see cref="HydraRingCalculationSettings"/>
        /// based on a <see cref="HydraulicBoundaryCalculationSettings"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryCalculationSettings">The <see cref="HydraulicBoundaryCalculationSettings"/>
        /// to create a <see cref="HydraRingCalculationSettings"/> for.</param>
        /// <returns>A <see cref="HydraRingCalculationSettings"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryCalculationSettings"/>
        /// is <c>null</c>.</exception>
        public static HydraRingCalculationSettings CreateSettings(HydraulicBoundaryCalculationSettings hydraulicBoundaryCalculationSettings)
        {
            if (hydraulicBoundaryCalculationSettings == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryCalculationSettings));
            }

            return new HydraRingCalculationSettings(hydraulicBoundaryCalculationSettings.HlcdFilePath,
                                                    hydraulicBoundaryCalculationSettings.PreprocessorDirectory,
                                                    hydraulicBoundaryCalculationSettings.UsePreprocessorClosure);
        }
    }
}