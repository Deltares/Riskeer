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

using System;
using System.Collections.Generic;
using System.Linq;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.StabilityStoneCover.Data;

namespace Ringtoets.StabilityStoneCover.Forms
{
    /// <summary>
    /// Class holds methods to help views when dealing with <see cref="StabilityStoneCoverWaveConditionsCalculation"/>
    /// </summary>
    public static class StabilityStoneCoverCalculationConfigurationHelper
    {
        /// <summary>
        /// Adds <see cref="StabilityStoneCoverWaveConditionsCalculation"/> based on the <paramref name="locations"/> 
        /// in the <paramref name="calculations"/>.
        /// </summary>
        /// <param name="locations">Locations to base the calculation upon.</param>
        /// <param name="calculations">The list to update.</param>
        /// <exception cref="ArgumentNullException">Throw when any input parameter is <c>null</c>.</exception>
        public static void AddCalculationsFromLocations(
            IEnumerable<HydraulicBoundaryLocation> locations,
            IList<ICalculationBase> calculations
            )
        {
            if (locations == null)
            {
                throw new ArgumentNullException("locations");
            }
            if (calculations == null)
            {
                throw new ArgumentNullException("calculations");
            }
            foreach (var calculation in locations.Select(location => CreateStabilityStoneCoverWaveConditionsCalculation(location, calculations)))
            {
                calculations.Add(calculation);
            }
        }

        private static ICalculationBase CreateStabilityStoneCoverWaveConditionsCalculation(
            HydraulicBoundaryLocation hydraulicBoundaryLocation,
            IEnumerable<ICalculationBase> calculations)
        {
            var nameBase = hydraulicBoundaryLocation.Name;
            var name = NamingHelper.GetUniqueName(calculations, nameBase, c => c.Name);

            return new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
        }
    }
}