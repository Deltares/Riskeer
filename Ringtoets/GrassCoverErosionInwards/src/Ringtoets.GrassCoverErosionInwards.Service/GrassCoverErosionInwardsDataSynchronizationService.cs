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
using Core.Common.Base;
using Core.Common.Utils.Extensions;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionInwards.Service
{
    /// <summary>
    /// Service for synchronizing grass cover erosion inwards data.
    /// </summary>
    public static class GrassCoverErosionInwardsDataSynchronizationService
    {
        /// <summary>
        /// Clears the output for all calculations in the <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionInwardsFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by
        /// clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<GrassCoverErosionInwardsCalculation> ClearAllCalculationOutput(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            var affectedItems = failureMechanism.Calculations
                                                .Cast<GrassCoverErosionInwardsCalculation>()
                                                .Where(c => c.HasOutput)
                                                .ToArray();

            affectedItems.ForEachElementDo(ClearCalculationOutput);

            return affectedItems;
        }

        /// <summary>
        /// Clears the output of the given <see cref="GrassCoverErosionInwardsCalculation"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/>
        /// to clear the output for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>
        /// is <c>null</c>.</exception>
        public static void ClearCalculationOutput(GrassCoverErosionInwardsCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }

            calculation.Output = null;
        }

        /// <summary>
        /// Clears the <see cref="HydraulicBoundaryLocation"/> and output for all the calculations
        /// in the <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionInwardsFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by
        /// removal of data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<GrassCoverErosionInwardsCalculation> ClearAllCalculationOutputAndHydraulicBoundaryLocations(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            Collection<GrassCoverErosionInwardsCalculation> affectedItems = new Collection<GrassCoverErosionInwardsCalculation>();
            foreach (var calculation in failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>())
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

        /// <summary>
        /// Clears all data dependent, either directly or indirectly, on the parent reference line.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to be cleared.</param>
        /// <returns>All objects that have been changed.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearReferenceLineDependentData(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            var observables = new List<IObservable>();

            failureMechanism.ClearAllSections();
            observables.Add(failureMechanism);

            failureMechanism.CalculationsGroup.Children.Clear();
            observables.Add(failureMechanism.CalculationsGroup);

            failureMechanism.DikeProfiles.Clear();
            observables.Add(failureMechanism.DikeProfiles);

            return observables;
        }

        private static void ClearHydraulicBoundaryLocation(GrassCoverErosionInwardsCalculation calculation)
        {
            calculation.InputParameters.HydraulicBoundaryLocation = null;
        }
    }
}