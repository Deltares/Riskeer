// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Service;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Util;

namespace Ringtoets.StabilityPointStructures.Service
{
    /// <summary>
    /// Service for synchronizing stability point structures data.
    /// </summary>
    public static class StabilityPointStructuresDataSynchronizationService
    {
        /// <summary>
        /// Clears all structures, unassigns them from the calculations in the <paramref name="failureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="StabilityPointStructuresFailureMechanism"/> to 
        /// clear the structures from.</param>
        /// <returns>All objects that are affected by this operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveAllStructures(StabilityPointStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculations =
                failureMechanism.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>();
            StructuresCalculation<StabilityPointStructuresInput>[] calculationWithRemovedStructure = calculations
                                                                                                     .Where(c => c.InputParameters.Structure != null)
                                                                                                     .ToArray();

            List<IObservable> changedObservables = ClearStructureDependentData(failureMechanism,
                                                                               calculationWithRemovedStructure);

            StructureCollection<StabilityPointStructure> structures = failureMechanism.StabilityPointStructures;
            structures.Clear();
            changedObservables.Add(structures);

            return changedObservables;
        }

        /// <summary>
        /// Removes the given stability point structure and all dependent data, either
        /// directly or indirectly, from the failure mechanism.
        /// </summary>
        /// <param name="structure">The structure to be removed.</param>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="structure"/>.</param>
        /// <returns>All objects affected by the removal.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveStructure(StabilityPointStructure structure, StabilityPointStructuresFailureMechanism failureMechanism)
        {
            if (structure == null)
            {
                throw new ArgumentNullException(nameof(structure));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> stabilityPointStructureCalculations =
                failureMechanism.Calculations
                                .Cast<StructuresCalculation<StabilityPointStructuresInput>>();
            IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculationWithRemovedStabilityPointStructure =
                stabilityPointStructureCalculations
                    .Where(c => ReferenceEquals(c.InputParameters.Structure, structure))
                    .ToArray();

            List<IObservable> changedObservables = ClearStructureDependentData(failureMechanism, calculationWithRemovedStabilityPointStructure);

            failureMechanism.StabilityPointStructures.Remove(structure);
            changedObservables.Add(failureMechanism.StabilityPointStructures);

            return changedObservables;
        }

        /// <summary>
        /// Clears the output for all calculations in the <see cref="StabilityPointStructuresFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="StabilityPointStructuresFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by
        /// clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllCalculationOutput(
            StabilityPointStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return failureMechanism.Calculations
                                   .Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                   .SelectMany(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput)
                                   .ToArray();
        }

        /// <summary>
        /// Clears the <see cref="HydraulicBoundaryLocation"/> and output for all the calculations
        /// in the <see cref="StabilityPointStructuresFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="StabilityPointStructuresFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects which are affected by removing data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllCalculationOutputAndHydraulicBoundaryLocations(
            StabilityPointStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var affectedItems = new List<IObservable>();
            foreach (StructuresCalculation<StabilityPointStructuresInput> calculation in failureMechanism.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>())
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
        public static ClearResults ClearReferenceLineDependentData(StabilityPointStructuresFailureMechanism failureMechanism)
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
                                                      .Concat(failureMechanism.StabilityPointStructures)
                                                      .ToArray();

            failureMechanism.ClearAllSections();
            changedObjects.Add(failureMechanism);
            changedObjects.Add(failureMechanism.SectionResults);

            failureMechanism.CalculationsGroup.Children.Clear();
            changedObjects.Add(failureMechanism.CalculationsGroup);

            failureMechanism.ForeshoreProfiles.Clear();
            changedObjects.Add(failureMechanism.ForeshoreProfiles);

            failureMechanism.StabilityPointStructures.Clear();
            changedObjects.Add(failureMechanism.StabilityPointStructures);

            return new ClearResults(changedObjects, removedObjects);
        }

        private static IEnumerable<IObservable> ClearHydraulicBoundaryLocation(StabilityPointStructuresInput input)
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

        private static List<IObservable> ClearStructureDependentData(StabilityPointStructuresFailureMechanism failureMechanism,
                                                                     IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculationWithRemovedStructure)
        {
            var changedObservables = new List<IObservable>();
            foreach (StructuresCalculation<StabilityPointStructuresInput> calculation in calculationWithRemovedStructure)
            {
                changedObservables.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation));

                calculation.InputParameters.ClearStructure();
                changedObservables.Add(calculation.InputParameters);
            }

            IEnumerable<StabilityPointStructuresFailureMechanismSectionResult> affectedSectionResults =
                StabilityPointStructuresHelper.UpdateCalculationToSectionResultAssignments(failureMechanism);

            changedObservables.AddRange(affectedSectionResults);
            return changedObservables;
        }
    }
}