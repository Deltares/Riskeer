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
using System.Collections.Generic;
using System.ComponentModel;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Util.Helpers;
using Riskeer.Revetment.Data;
using Riskeer.StabilityStoneCover.Data;

namespace Riskeer.StabilityStoneCover.Forms
{
    /// <summary>
    /// Class that holds methods to help views when dealing with <see cref="StabilityStoneCoverWaveConditionsCalculation"/>
    /// </summary>
    public static class StabilityStoneCoverCalculationConfigurationHelper
    {
        /// <summary>
        /// Adds <see cref="StabilityStoneCoverWaveConditionsCalculation"/> in the <paramref name="calculations"/>
        /// based on the <paramref name="locations"/> and the <paramref name="normativeProbabilityType"/>.
        /// </summary>
        /// <param name="locations">Locations to base the calculation upon.</param>
        /// <param name="calculations">The list to update.</param>
        /// <param name="normativeProbabilityType">The <see cref="NormativeProbabilityType"/> to set the water level type input for.</param>
        /// <exception cref="ArgumentNullException">Throw when any <paramref name="locations"/>
        /// or <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normativeProbabilityType"/> is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="normativeProbabilityType"/> is a valid value,
        /// but unsupported.</exception>
        public static void AddCalculationsFromLocations(IEnumerable<HydraulicBoundaryLocation> locations,
                                                        List<ICalculationBase> calculations,
                                                        NormativeProbabilityType normativeProbabilityType)
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
                calculations.Add(CreateStabilityStoneCoverWaveConditionsCalculation(hydraulicBoundaryLocation,
                                                                                    calculations,
                                                                                    normativeProbabilityType));
            }
        }

        /// <summary>
        /// Creates a calculation and sets the <paramref name="hydraulicBoundaryLocation"/>
        /// and the water level type on its input.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> to set.</param>
        /// <param name="calculations">The list of calculations to base the calculation name from.</param>
        /// <param name="normativeProbabilityType">The <see cref="NormativeProbabilityType"/> to base the water level type input on.</param>
        /// <returns>An <see cref="ICalculationBase"/> representing a stability stone cover calculation.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normativeProbabilityType"/> is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="normativeProbabilityType"/> is a valid value,
        /// but unsupported.</exception>
        private static ICalculationBase CreateStabilityStoneCoverWaveConditionsCalculation(
            HydraulicBoundaryLocation hydraulicBoundaryLocation,
            IEnumerable<ICalculationBase> calculations,
            NormativeProbabilityType normativeProbabilityType)
        {
            string nameBase = hydraulicBoundaryLocation.Name;
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = NamingHelper.GetUniqueName(calculations, nameBase, c => c.Name),
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            WaveConditionsInputHelper.SetWaterLevelType(calculation.InputParameters, normativeProbabilityType);
            return calculation;
        }
    }
}