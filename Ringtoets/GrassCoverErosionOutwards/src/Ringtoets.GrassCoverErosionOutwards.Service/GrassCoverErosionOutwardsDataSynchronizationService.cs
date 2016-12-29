﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.Revetment.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Service
{
    /// <summary>
    /// Service for synchronizing grass cover erosion outwards data.
    /// </summary>
    public static class GrassCoverErosionOutwardsDataSynchronizationService
    {
        /// <summary>
        /// Clears the output of the given <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>
        /// to clear the output for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>
        /// is <c>null</c>.</exception>
        /// <returns>All objects that have been changed.</returns>
        public static IEnumerable<IObservable> ClearWaveConditionsCalculationOutput(GrassCoverErosionOutwardsWaveConditionsCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }

            if (calculation.HasOutput)
            {
                calculation.ClearOutput();
                return new[]
                {
                    calculation
                };
            }
            return Enumerable.Empty<IObservable>();
        }

        /// <summary>
        /// Clears the <see cref="HydraulicBoundaryLocation"/> and output for all the wave conditions calculations
        /// in the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionOutwardsFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects which are affected by removal of data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(
            GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            var affectedItems = new List<IObservable>();
            foreach (var calculation in failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>())
            {
                affectedItems.AddRange(ClearWaveConditionsCalculationOutput(calculation)
                                           .Concat(ClearHydraulicBoundaryLocation(calculation.InputParameters)));
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
                throw new ArgumentNullException("failureMechanism");
            }

            return failureMechanism.Calculations
                                   .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                   .SelectMany(ClearWaveConditionsCalculationOutput)
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
                throw new ArgumentNullException("failureMechanism");
            }

            var changedObjects = new List<IObservable>();
            var removedObjects = failureMechanism.Sections.OfType<object>()
                                                 .Concat(failureMechanism.SectionResults)
                                                 .Concat(failureMechanism.WaveConditionsCalculationGroup.GetAllChildrenRecursive())
                                                 .Concat(failureMechanism.ForeshoreProfiles)
                                                 .ToArray();

            failureMechanism.ClearAllSections();
            changedObjects.Add(failureMechanism);

            failureMechanism.WaveConditionsCalculationGroup.Children.Clear();
            changedObjects.Add(failureMechanism.WaveConditionsCalculationGroup);

            failureMechanism.ForeshoreProfiles.Clear();
            changedObjects.Add(failureMechanism.ForeshoreProfiles);

            return new ClearResults(changedObjects, removedObjects);
        }

        private static IEnumerable<IObservable> ClearHydraulicBoundaryLocation(WaveConditionsInput input)
        {
            if (input.HydraulicBoundaryLocation != null)
            {
                input.HydraulicBoundaryLocation = null;
                return new[]
                {
                    input
                };
            }
            return Enumerable.Empty<IObservable>();
        }
    }
}