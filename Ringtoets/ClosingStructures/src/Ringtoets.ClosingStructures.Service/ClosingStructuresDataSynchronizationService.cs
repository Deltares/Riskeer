// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Service;
using Ringtoets.Common.Utils;

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
        public static IEnumerable<IObservable> ClearAllCalculationOutput(ClosingStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return failureMechanism.Calculations
                                   .Cast<StructuresCalculation<ClosingStructuresInput>>()
                                   .SelectMany(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput)
                                   .ToArray();
        }

        /// <summary>
        /// Clears the <see cref="HydraulicBoundaryLocation"/> and output for all the calculations
        /// in the <see cref="StructuresCalculation{T}"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="ClosingStructuresFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects which are affected by removing data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllCalculationOutputAndHydraulicBoundaryLocations(ClosingStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var affectedItems = new List<IObservable>();
            foreach (StructuresCalculation<ClosingStructuresInput> calculation in failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>())
            {
                affectedItems.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation)
                                                                                .Concat(ClearHydraulicBoundaryLocation(calculation.InputParameters)));
            }

            return affectedItems;
        }

        /// <summary>
        /// Clears all data dependent, either directly or indirectly, on the parent reference line.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to be cleared.</param>
        /// <returns>The results of the clear action.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static ClearResults ClearReferenceLineDependentData(ClosingStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var changedObjects = new Collection<IObservable>();
            object[] removedObjects = failureMechanism.Sections.OfType<object>()
                                                      .Concat(failureMechanism.SectionResults)
                                                      .Concat(failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
                                                      .Concat(failureMechanism.ForeshoreProfiles)
                                                      .Concat(failureMechanism.ClosingStructures)
                                                      .ToArray();

            failureMechanism.ClearAllSections();
            changedObjects.Add(failureMechanism);

            failureMechanism.CalculationsGroup.Children.Clear();
            changedObjects.Add(failureMechanism.CalculationsGroup);

            failureMechanism.ForeshoreProfiles.Clear();
            changedObjects.Add(failureMechanism.ForeshoreProfiles);

            failureMechanism.ClosingStructures.Clear();
            changedObjects.Add(failureMechanism.ClosingStructures);

            return new ClearResults(changedObjects, removedObjects);
        }

        private static IEnumerable<IObservable> ClearHydraulicBoundaryLocation(ClosingStructuresInput input)
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