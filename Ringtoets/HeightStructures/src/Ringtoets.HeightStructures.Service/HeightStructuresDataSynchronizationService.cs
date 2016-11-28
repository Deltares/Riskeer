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
using Core.Common.Base;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Service;
using Ringtoets.Common.Utils;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.HeightStructures.Service
{
    /// <summary>
    /// Service for synchronizing height structures data.
    /// </summary>
    public static class HeightStructuresDataSynchronizationService
    {
        /// <summary>
        /// Clears the output for all calculations in the <see cref="HeightStructuresFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="HeightStructuresFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by
        /// clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllCalculationOutput(HeightStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            return failureMechanism.Calculations
                                   .Cast<StructuresCalculation<HeightStructuresInput>>()
                                   .SelectMany(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput)
                                   .ToArray();
        }

        /// <summary>
        /// Clears the <see cref="HydraulicBoundaryLocation"/> and output for all the calculations
        /// in the <see cref="HeightStructuresFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="HeightStructuresFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects which are affected by removing data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllCalculationOutputAndHydraulicBoundaryLocations(HeightStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            var affectedItems = new List<IObservable>();
            foreach (var calculation in failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>())
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
        /// <returns>All objects that have been changed.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearReferenceLineDependentData(HeightStructuresFailureMechanism failureMechanism)
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

            failureMechanism.ForeshoreProfiles.Clear();
            observables.Add(failureMechanism.ForeshoreProfiles);

            failureMechanism.HeightStructures.Clear();
            observables.Add(failureMechanism.HeightStructures);

            return observables;
        }

        /// <summary>
        /// Removes the given height structure and all dependent data, either directly or indirectly,
        /// from the failure mechanism.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="structure"/>.</param>
        /// <param name="structure">The structure to be removed.</param>
        /// <returns>All objects affected by the removal.</returns>
        public static IEnumerable<IObservable> RemoveStructure(HeightStructuresFailureMechanism failureMechanism, HeightStructure structure)
        {
            var changedObservables = new HashSet<IObservable>();
            StructuresCalculation<HeightStructuresInput>[] heightStructureCalculations = failureMechanism.Calculations
                                                                                                         .Cast<StructuresCalculation<HeightStructuresInput>>()
                                                                                                         .ToArray();
            StructuresCalculation<HeightStructuresInput>[] calculationWithRemovedHeightStructure = heightStructureCalculations
                .Where(c => ReferenceEquals(c.InputParameters.Structure, structure))
                .ToArray();
            foreach (StructuresCalculation<HeightStructuresInput> calculation in calculationWithRemovedHeightStructure)
            {
                calculation.InputParameters.Structure = null;

                IEnumerable<StructuresFailureMechanismSectionResult<HeightStructuresInput>> affectedSectionResults =
                    StructuresHelper.Delete(failureMechanism.SectionResults, calculation, heightStructureCalculations);
                foreach (StructuresFailureMechanismSectionResult<HeightStructuresInput> result in affectedSectionResults)
                {
                    changedObservables.Add(result);
                }
                changedObservables.Add(calculation.InputParameters);
            }

            failureMechanism.HeightStructures.Remove(structure);
            changedObservables.Add(failureMechanism.HeightStructures);

            return changedObservables;
        }

        private static IEnumerable<IObservable> ClearHydraulicBoundaryLocation(HeightStructuresInput input)
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