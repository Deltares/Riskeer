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
using Riskeer.Common.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Service;
using Riskeer.HeightStructures.Data;

namespace Riskeer.HeightStructures.Service
{
    /// <summary>
    /// Service for synchronizing height structures data.
    /// </summary>
    public static class HeightStructuresDataSynchronizationService
    {
        /// <summary>
        /// Removes the <paramref name="structure"/> and clears all dependent data, either directly or indirectly.
        /// </summary>
        /// <param name="structure">The structure to be removed.</param>
        /// <param name="failureMechanism">The <see cref="HeightStructuresFailureMechanism"/>
        /// to clear the data from.</param>
        /// <returns>All objects affected by the removal.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveStructure(HeightStructure structure,
                                                               HeightStructuresFailureMechanism failureMechanism)
        {
            if (structure == null)
            {
                throw new ArgumentNullException(nameof(structure));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            IEnumerable<StructuresCalculation<HeightStructuresInput>> calculations =
                failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>();

            StructuresCalculation<HeightStructuresInput>[] calculationWithRemovedStructure = calculations
                                                                                             .Where(c => ReferenceEquals(c.InputParameters.Structure, structure))
                                                                                             .ToArray();

            List<IObservable> changedObservables = ClearStructureDependentData(calculationWithRemovedStructure);

            StructureCollection<HeightStructure> structures = failureMechanism.HeightStructures;
            structures.Remove(structure);
            changedObservables.Add(structures);

            return changedObservables;
        }

        /// <summary>
        /// Removes all structures and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="HeightStructuresFailureMechanism"/> to 
        /// clear the structures from.</param>
        /// <returns>All objects that are affected by this operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveAllStructures(HeightStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            IEnumerable<StructuresCalculation<HeightStructuresInput>> calculations =
                failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>();
            StructuresCalculation<HeightStructuresInput>[] calculationWithRemovedStructure = calculations
                                                                                             .Where(c => c.InputParameters.Structure != null)
                                                                                             .ToArray();

            List<IObservable> changedObservables = ClearStructureDependentData(calculationWithRemovedStructure);

            StructureCollection<HeightStructure> structures = failureMechanism.HeightStructures;
            structures.Clear();
            changedObservables.Add(structures);

            return changedObservables;
        }

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
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return failureMechanism.Calculations
                                   .Cast<StructuresCalculation<HeightStructuresInput>>()
                                   .SelectMany(RiskeerCommonDataSynchronizationService.ClearCalculationOutput)
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
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var affectedItems = new List<IObservable>();
            foreach (StructuresCalculation<HeightStructuresInput> calculation in failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>())
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
        public static ClearResults ClearReferenceLineDependentData(HeightStructuresFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var changedObjects = new List<IObservable>();
            object[] removedObjects = failureMechanism.CalculationsGroup.GetAllChildrenRecursive().OfType<object>()
                                                      .Concat(failureMechanism.ForeshoreProfiles)
                                                      .Concat(failureMechanism.HeightStructures)
                                                      .ToArray();

            changedObjects.Add(failureMechanism);

            failureMechanism.CalculationsGroup.Children.Clear();
            changedObjects.Add(failureMechanism.CalculationsGroup);

            failureMechanism.ForeshoreProfiles.Clear();
            changedObjects.Add(failureMechanism.ForeshoreProfiles);

            failureMechanism.HeightStructures.Clear();
            changedObjects.Add(failureMechanism.HeightStructures);

            return new ClearResults(changedObjects, removedObjects);
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

        private static List<IObservable> ClearStructureDependentData(IEnumerable<StructuresCalculation<HeightStructuresInput>> calculationWithRemovedStructure)
        {
            var changedObservables = new List<IObservable>();
            foreach (StructuresCalculation<HeightStructuresInput> calculation in calculationWithRemovedStructure)
            {
                changedObservables.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation));

                calculation.InputParameters.ClearStructure();
                changedObservables.Add(calculation.InputParameters);
            }

            return changedObservables;
        }
    }
}