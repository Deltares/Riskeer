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
        public static IEnumerable<ClosingStructuresCalculation> ClearAllCalculationOutput(ClosingStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            var affectedItems = failureMechanism.Calculations
                                                .Cast<ClosingStructuresCalculation>()
                                                .Where(c => c.HasOutput)
                                                .ToArray();

            affectedItems.ForEachElementDo(ClearCalculationOutput);

            return affectedItems;
        }

        /// <summary>
        /// Clears the output of the given <see cref="ClosingStructuresCalculation"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="ClosingStructuresCalculation"/> to clear
        /// the output for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>
        /// is <c>null</c>.</exception>
        public static void ClearCalculationOutput(ClosingStructuresCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }

            calculation.Output = null;
        }

        /// <summary>
        /// Clears the <see cref="HydraulicBoundaryLocation"/> for all the calculations in
        /// the <see cref="ClosingStructuresCalculation"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="ClosingStructuresFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by
        /// removing <see cref="HydraulicBoundaryLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<ClosingStructuresCalculation> ClearHydraulicBoundaryLocations(ClosingStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            var affectedItems = failureMechanism.Calculations
                                                .Cast<ClosingStructuresCalculation>()
                                                .Where(c => c.InputParameters.HydraulicBoundaryLocation != null)
                                                .ToArray();

            affectedItems.ForEachElementDo(ClearHydraulicBoundaryLocation);

            return affectedItems;
        }

        /// <summary>
        /// Clears the <see cref="HydraulicBoundaryLocation"/> and output for all the calculations
        /// in the <see cref="ClosingStructuresCalculation"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="ClosingStructuresCalculation"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by
        /// removing data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<ClosingStructuresCalculation> ClearAllCalculationOutputAndHydraulicBoundaryLocations(ClosingStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            Collection<ClosingStructuresCalculation> affectedItems = new Collection<ClosingStructuresCalculation>();

            foreach (var calculation in failureMechanism.Calculations.Cast<ClosingStructuresCalculation>())
            {
                var calculationChanged = false;

                if (calculation.HasOutput)
                {
                    ClearCalculationOutput(calculation);
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

        private static void ClearHydraulicBoundaryLocation(ClosingStructuresCalculation calculation)
        {
            calculation.InputParameters.HydraulicBoundaryLocation = null;
        }
    }
}