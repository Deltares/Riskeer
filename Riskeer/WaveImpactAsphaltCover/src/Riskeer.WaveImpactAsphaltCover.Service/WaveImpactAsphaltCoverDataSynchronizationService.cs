﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.WaveImpactAsphaltCover.Service
{
    /// <summary>
    /// Service for synchronizing wave impact asphalt cover data.
    /// </summary>
    public static class WaveImpactAsphaltCoverDataSynchronizationService
    {
        /// <summary>
        /// Clears the <see cref="HydraulicBoundaryLocation"/> and output for all the wave conditions calculations
        /// in the <see cref="WaveImpactAsphaltCoverFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="WaveImpactAsphaltCoverFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects which are affected by removing data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(
            WaveImpactAsphaltCoverFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var affectedItems = new List<IObservable>();
            foreach (WaveImpactAsphaltCoverWaveConditionsCalculation calculation in failureMechanism.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>())
            {
                affectedItems.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation));
                affectedItems.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocation(calculation.InputParameters));
            }

            return affectedItems;
        }

        /// <summary>
        /// Clears the output for all calculations in the <see cref="WaveImpactAsphaltCoverFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="WaveImpactAsphaltCoverFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by
        /// clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllWaveConditionsCalculationOutput(WaveImpactAsphaltCoverFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return failureMechanism.Calculations
                                   .Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                   .SelectMany(RiskeerCommonDataSynchronizationService.ClearCalculationOutput)
                                   .ToArray();
        }

        /// <summary>
        /// Clears all data dependent, either directly or indirectly, on the parent reference line.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to be cleared.</param>
        /// <returns>The results of the clear action.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static ClearResults ClearReferenceLineDependentData(WaveImpactAsphaltCoverFailureMechanism failureMechanism)
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