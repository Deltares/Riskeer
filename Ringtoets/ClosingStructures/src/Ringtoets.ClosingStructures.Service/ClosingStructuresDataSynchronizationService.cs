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
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Utils.Extensions;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Structures;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.ClosingStructures.Service
{
    /// <summary>
    /// Service for synchronizing closing structures data.
    /// </summary>
    public static class ClosingStructuresDataSynchronizationService
    {
        /// <summary>
        /// Clears the output for all calculations in the <see cref="ClosingStructuresFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="ClosingStructuresFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by
        /// clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<StructuresCalculation<ClosingStructuresInput>> ClearAllCalculationOutput(ClosingStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            var affectedItems = failureMechanism.Calculations
                                                .Cast<StructuresCalculation<ClosingStructuresInput>>()
                                                .Where(c => c.HasOutput)
                                                .ToArray();

            affectedItems.ForEachElementDo(item => item.ClearOutput());

            return affectedItems;
        }
        
        /// <summary>
        /// Clears the <see cref="HydraulicBoundaryLocation"/> and output for all the calculations
        /// in the <see cref="StructuresCalculation{T}"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="StructuresCalculation{T}"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by
        /// removing data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<StructuresCalculation<ClosingStructuresInput>> ClearAllCalculationOutputAndHydraulicBoundaryLocations(ClosingStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            var affectedItems = new Collection<StructuresCalculation<ClosingStructuresInput>>();

            foreach (var calculation in failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>())
            {
                var calculationChanged = false;

                if (calculation.HasOutput)
                {
                    calculation.ClearOutput();
                    calculationChanged = true;
                }

                if (calculation.InputParameters.HydraulicBoundaryLocation != null)
                {
                    ClearHydraulicBoundaryLocation(calculation);
                    calculationChanged = true;
                }

                if (calculationChanged)
                {
                    affectedItems.Add(calculation);
                }
            }

            return affectedItems;
        }

        private static void ClearHydraulicBoundaryLocation(StructuresCalculation<ClosingStructuresInput> calculation)
        {
            calculation.InputParameters.HydraulicBoundaryLocation = null;
        }
    }
}