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
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Utils;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Service for synchronizing common data 
    /// </summary>
    public static class RingtoetsCommonDataSynchronizationService
    {
        /// <summary>
        /// Clears the output of the hydraulic boundary locations within the collection.
        /// </summary>
        /// <param name="locations">The locations for which the output needs to be cleared.</param>
        /// <returns>All objects changed during the clear.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="locations"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearHydraulicBoundaryLocationOutput(IEnumerable<HydraulicBoundaryLocation> locations)
        {
            if (locations == null)
            {
                throw new ArgumentNullException(nameof(locations));
            }

            return locations.SelectMany(ClearHydraulicBoundaryLocationOutput)
                            .ToArray();
        }

        /// <summary>
        /// Clears the output of the given <see cref="StructuresCalculation{T}"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="StructuresCalculation{T}"/> to clear the output for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        /// <returns>All objects that have been changed.</returns>
        public static IEnumerable<IObservable> ClearCalculationOutput(ICalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
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
        /// Clears the given foreshore profile from a <see cref="StructuresCalculation{T}"/> collection.
        /// </summary>
        /// <typeparam name="TStructureInput">Object type of the structure calculation input.</typeparam>
        /// <typeparam name="TStructure">Object type of the structure property of <typeparamref name="TStructureInput"/>.</typeparam>
        /// <param name="calculations">The calculations.</param>
        /// <param name="profile">The profile to be cleared.</param>
        /// <returns>All affected objects by the clear.</returns>
        public static IEnumerable<IObservable> ClearForeshoreProfile<TStructureInput, TStructure>(IEnumerable<StructuresCalculation<TStructureInput>> calculations, ForeshoreProfile profile)
            where TStructureInput : StructuresInputBase<TStructure>, new()
            where TStructure : StructureBase
        {
            var affectedObjects = new List<IObservable>();
            foreach (StructuresCalculation<TStructureInput> calculation in calculations.Where(c => ReferenceEquals(c.InputParameters.ForeshoreProfile, profile)))
            {
                affectedObjects.AddRange(ClearCalculationOutput(calculation));

                calculation.InputParameters.ForeshoreProfile = null;
                affectedObjects.Add(calculation.InputParameters);
            }
            return affectedObjects;
        }

        /// <summary>
        /// Removes the <paramref name="structure"/>, unassigns it from the <paramref name="calculations"/>
        /// and clears all dependent data, either directly or indirectly,
        /// from the failure mechanism.
        /// </summary>
        /// <param name="structure">The structure to be removed.</param>
        /// <param name="calculations">The calculations that may have <paramref name="structure"/> assigned.</param>
        /// <param name="structures">The collection of structures in which <paramref name="structure"/> is 
        /// contained.</param>
        /// <param name="sectionResults">The section results that may have an assignment to a calculation 
        /// based on the <paramref name="structure"/>.</param>
        /// <returns>All objects affected by the removal.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveStructure<TStructure, TInput>(
            TStructure structure,
            IEnumerable<StructuresCalculation<TInput>> calculations,
            StructureCollection<TStructure> structures,
            IEnumerable<StructuresFailureMechanismSectionResult<TInput>> sectionResults)
            where TInput : IStructuresCalculationInput<TStructure>, new()
            where TStructure : StructureBase
        {
            if (structure == null)
            {
                throw new ArgumentNullException(nameof(structure));
            }
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }
            if (structures == null)
            {
                throw new ArgumentNullException(nameof(structures));
            }
            if (sectionResults == null)
            {
                throw new ArgumentNullException(nameof(sectionResults));
            }
            StructuresCalculation<TInput>[] calculationWithRemovedStructure = calculations
                .Where(c => ReferenceEquals(c.InputParameters.Structure, structure))
                .ToArray();

            ICollection<IObservable> changedObservables = ClearStructureDependentData(
                sectionResults,
                calculationWithRemovedStructure,
                calculations);

            structures.Remove(structure);
            changedObservables.Add(structures);

            return changedObservables;
        }

        /// <summary>
        /// Clears the <paramref name="structures"/>, unassigns them from the 
        /// <paramref name="calculations"/> and clears all data that depends on it,
        /// either directly or indirectly.
        /// </summary>
        /// <param name="calculations">The calculations that may have assigned an element in
        /// <paramref name="structures"/>.</param>
        /// <param name="structures">The collection of structures to clear.</param>
        /// <param name="sectionResults">The section results that may have an assignment to a calculation 
        /// based on elements of <paramref name="structures"/>.</param>
        /// <returns>All objects that are affected by this operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveAllStructures<TStructure, TInput>(
            IEnumerable<StructuresCalculation<TInput>> calculations,
            StructureCollection<TStructure> structures,
            IEnumerable<StructuresFailureMechanismSectionResult<TInput>> sectionResults)
            where TInput : IStructuresCalculationInput<TStructure>, new()
            where TStructure : StructureBase
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }
            if (structures == null)
            {
                throw new ArgumentNullException(nameof(structures));
            }
            if (sectionResults == null)
            {
                throw new ArgumentNullException(nameof(sectionResults));
            }
            StructuresCalculation<TInput>[] calculationWithRemovedStructure = calculations
                .Where(c => c.InputParameters.Structure != null)
                .ToArray();

            ICollection<IObservable> changedObservables = ClearStructureDependentData(
                sectionResults,
                calculationWithRemovedStructure,
                calculations);

            structures.Clear();
            changedObservables.Add(structures);

            return changedObservables;
        }

        private static ICollection<IObservable> ClearStructureDependentData<T>(IEnumerable<StructuresFailureMechanismSectionResult<T>> sectionResults,
                                                                               IEnumerable<StructuresCalculation<T>> calculationWithRemovedStructure,
                                                                               IEnumerable<StructuresCalculation<T>> structureCalculations)
            where T : IStructuresCalculationInput<StructureBase>, new()
        {
            var changedObservables = new List<IObservable>();
            foreach (StructuresCalculation<T> calculation in calculationWithRemovedStructure)
            {
                changedObservables.AddRange(ClearCalculationOutput(calculation));

                calculation.InputParameters.ClearStructure();
                changedObservables.Add(calculation.InputParameters);
            }

            IEnumerable<StructuresFailureMechanismSectionResult<T>> affectedSectionResults =
                StructuresHelper.UpdateCalculationToSectionResultAssignments(sectionResults, structureCalculations);

            changedObservables.AddRange(affectedSectionResults);
            return changedObservables;
        }

        private static IEnumerable<IObservable> ClearHydraulicBoundaryLocationOutput(HydraulicBoundaryLocation location)
        {
            if (location.DesignWaterLevelOutput != null || location.WaveHeightOutput != null)
            {
                location.DesignWaterLevelOutput = null;
                location.WaveHeightOutput = null;

                return new[]
                {
                    location
                };
            }
            return Enumerable.Empty<IObservable>();
        }
    }
}