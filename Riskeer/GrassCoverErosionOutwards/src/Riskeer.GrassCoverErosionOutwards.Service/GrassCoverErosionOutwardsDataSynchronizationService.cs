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
using System.Linq;
using Core.Common.Base;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Service;
using Riskeer.GrassCoverErosionOutwards.Data;

namespace Riskeer.GrassCoverErosionOutwards.Service
{
    /// <summary>
    /// Service for synchronizing grass cover erosion outwards data.
    /// </summary>
    public static class GrassCoverErosionOutwardsDataSynchronizationService
    {
        /// <summary>
        /// Clears the <see cref="HydraulicBoundaryLocation"/> and output for the wave conditions calculations
        /// in the <see cref="GrassCoverErosionOutwardsFailureMechanism"/> that uses an <see cref="HydraulicBoundaryLocation"/>
        /// from <paramref name="hydraulicBoundaryLocations"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionOutwardsFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations to clear for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects which are affected by removal of data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(
            GrassCoverErosionOutwardsFailureMechanism failureMechanism,
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }

            var affectedItems = new List<IObservable>();
            foreach (GrassCoverErosionOutwardsWaveConditionsCalculation calculation in failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                                                                       .Where(c => hydraulicBoundaryLocations.Contains(
                                                                                                                  c.InputParameters.HydraulicBoundaryLocation)))
            {
                affectedItems.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation));
                affectedItems.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocation(calculation.InputParameters));
            }

            return affectedItems;
        }

        /// <summary>
        /// Clears the output for all calculations in the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionOutwardsFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by
        /// clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllWaveConditionsCalculationOutput(
            GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return failureMechanism.Calculations
                                   .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                   .SelectMany(RiskeerCommonDataSynchronizationService.ClearCalculationOutput)
                                   .ToArray();
        }

        /// <summary>
        /// Clears all data dependent, either directly or indirectly, on the parent reference line.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to be cleared.</param>
        /// <returns>The results of the clear action.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static ClearResults ClearReferenceLineDependentData(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var changedObjects = new List<IObservable>();
            object[] removedObjects = failureMechanism.Sections.OfType<object>()
                                                      .Concat(failureMechanism.SectionResults)
                                                      .Concat(failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
                                                      .Concat(failureMechanism.ForeshoreProfiles)
                                                      .ToArray();

            failureMechanism.ClearAllSections();
            changedObjects.Add(failureMechanism);
            changedObjects.Add(failureMechanism.SectionResults);

            failureMechanism.CalculationsGroup.Children.Clear();
            changedObjects.Add(failureMechanism.CalculationsGroup);

            failureMechanism.ForeshoreProfiles.Clear();
            changedObjects.Add(failureMechanism.ForeshoreProfiles);

            return new ClearResults(changedObjects, removedObjects);
        }
    }
}