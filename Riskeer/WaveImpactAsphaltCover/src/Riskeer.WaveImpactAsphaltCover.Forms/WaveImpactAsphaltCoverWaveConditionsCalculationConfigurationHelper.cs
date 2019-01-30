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
using System.ComponentModel;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Revetment.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.WaveImpactAsphaltCover.Forms
{
    /// <summary>
    /// Class that holds methods to help views when dealing with <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/>
    /// </summary>
    public static class WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationHelper
    {
        /// <summary>
        /// Adds <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/> in the <paramref name="calculations"/>
        /// based on the <paramref name="locations"/> and the <paramref name="normType"/>.
        /// </summary>
        /// <param name="locations">Locations to base the calculation upon.</param>
        /// <param name="calculations">The list to update.</param>
        /// <param name="normType">The <see cref="NormType"/> to set the category type input for.</param>
        /// <exception cref="ArgumentNullException">Throw when any input parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
        /// but unsupported.</exception>
        public static void AddCalculationsFromLocations(IEnumerable<HydraulicBoundaryLocation> locations,
                                                        List<ICalculationBase> calculations,
                                                        NormType normType)
        {
            if (locations == null)
            {
                throw new ArgumentNullException(nameof(locations));
            }

            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in locations)
            {
                calculations.Add(CreateWaveImpactAsphaltCoverWaveConditionsCalculation(hydraulicBoundaryLocation,
                                                                                       calculations,
                                                                                       normType));
            }
        }

        /// <summary>
        /// Creates a calculation and sets the <paramref name="hydraulicBoundaryLocation"/>
        /// and the category type on its input.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> to set.</param>
        /// <param name="calculations">The list of calculations to base the calculation name from.</param>
        /// <param name="normType">The <see cref="NormType"/> to base the category type input on.</param>
        /// <returns>An <see cref="ICalculationBase"/> representing a wave impact asphalt cover calculation.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
        /// but unsupported.</exception>
        private static ICalculationBase CreateWaveImpactAsphaltCoverWaveConditionsCalculation(
            HydraulicBoundaryLocation hydraulicBoundaryLocation,
            IEnumerable<ICalculationBase> calculations,
            NormType normType)
        {
            string nameBase = hydraulicBoundaryLocation.Name;
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = NamingHelper.GetUniqueName(calculations, nameBase, c => c.Name),
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            WaveConditionsInputHelper.SetCategoryType(calculation.InputParameters, normType);
            return calculation;
        }
    }
}