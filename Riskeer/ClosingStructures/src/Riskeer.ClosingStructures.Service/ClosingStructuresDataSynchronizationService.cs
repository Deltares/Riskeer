// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Service;

namespace Riskeer.ClosingStructures.Service
{
    /// <summary>
    /// Service for synchronizing closing structures data.
    /// </summary>
    public static class ClosingStructuresDataSynchronizationService
    {
        /// <summary>
        /// Removes the <paramref name="structure"/>, unassigns it from the calculations in
        /// <paramref name="failureMechanism"/> and clears all dependent data, either directly or indirectly.
        /// </summary>
        /// <param name="structure">The structure to be removed.</param>
        /// <param name="failureMechanism">The <see cref="ClosingStructuresFailureMechanism"/>
        /// to clear the data from.</param>
        /// <returns>All objects affected by the removal.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveStructure(ClosingStructure structure,
                                                               ClosingStructuresFailureMechanism failureMechanism)
        {
            if (structure == null)
            {
                throw new ArgumentNullException(nameof(structure));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculations =
                failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>();

            StructuresCalculation<ClosingStructuresInput>[] calculationWithRemovedStructure = calculations
                                                                                              .Where(c => ReferenceEquals(c.InputParameters.Structure, structure))
                                                                                              .ToArray();

            List<IObservable> changedObservables = ClearStructureDependentData(failureMechanism,
                                                                               calculationWithRemovedStructure);

            StructureCollection<ClosingStructure> structures = failureMechanism.ClosingStructures;
            structures.Remove(structure);
            changedObservables.Add(structures);

            return changedObservables;
        }

        /// <summary>
        /// Clears all structures, unassigns them from the calculations in the <paramref name="failureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="ClosingStructuresFailureMechanism"/> to 
        /// clear the structures from.</param>
        /// <returns>All objects that are affected by this operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveAllStructures(ClosingStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculations =
                failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>();
            StructuresCalculation<ClosingStructuresInput>[] calculationWithRemovedStructure = calculations
                                                                                              .Where(c => c.InputParameters.Structure != null)
                                                                                              .ToArray();

            List<IObservable> changedObservables = ClearStructureDependentData(failureMechanism,
                                                                               calculationWithRemovedStructure);

            StructureCollection<ClosingStructure> structures = failureMechanism.ClosingStructures;
            structures.Clear();
            changedObservables.Add(structures);

            return changedObservables;
        }

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
                                   .SelectMany(RiskeerCommonDataSynchronizationService.ClearCalculationOutput)
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
                affectedItems.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation)
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
            changedObjects.Add(failureMechanism.SectionResults);

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

        private static List<IObservable> ClearStructureDependentData(ClosingStructuresFailureMechanism failureMechanism,
                                                                     IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculationWithRemovedStructure)
        {
            var changedObservables = new List<IObservable>();
            foreach (StructuresCalculation<ClosingStructuresInput> calculation in calculationWithRemovedStructure)
            {
                changedObservables.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation));

                calculation.InputParameters.ClearStructure();
                changedObservables.Add(calculation.InputParameters);
            }

            return changedObservables;
        }
    }
}