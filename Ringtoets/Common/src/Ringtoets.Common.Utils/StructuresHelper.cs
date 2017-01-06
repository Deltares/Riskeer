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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;

namespace Ringtoets.Common.Utils
{
    /// <summary>
    /// Class holds helper methods to match <see cref="FailureMechanismSection"/> objects 
    /// with <see cref="StructuresCalculation{T}"/> objects. 
    /// </summary>
    public static class StructuresHelper
    {
        /// <summary>
        /// Determine which <see cref="StructuresCalculation{T}"/> objects are available for a
        /// <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="sections">The <see cref="FailureMechanismSection"/> objects.</param>
        /// <param name="calculations">The <see cref="CalculationWithLocation"/> objects.</param>
        /// <returns>A <see cref="Dictionary{K, V}"/> containing a <see cref="IList{T}"/> 
        /// of <see cref="FailureMechanismSectionResult"/> objects 
        /// for each section which has calculations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>
        /// or when an element in <paramref name="calculations"/> is <c>null</c></exception>
        /// <exception cref="ArgumentException">Thrown when an element in <paramref name="sections"/> is 
        /// <c>null</c>.</exception>
        public static Dictionary<string, IList<ICalculation>> CollectCalculationsPerSection<T>(IEnumerable<FailureMechanismSection> sections,
                                                                                               IEnumerable<StructuresCalculation<T>> calculations)
            where T : IStructuresCalculationInput<StructureBase>, new()
        {
            return AssignUnassignCalculations.CollectCalculationsPerSection(sections, AsCalculationsWithLocations(calculations));
        }

        /// <summary>
        /// Determine which <see cref="FailureMechanismSection"/> geometrically contains the <see cref="StructuresCalculation{T}"/>.
        /// </summary>
        /// <param name="sections">The <see cref="FailureMechanismSection"/> objects 
        /// whose <see cref="FailureMechanismSection"/> are considered.</param>
        /// <param name="calculation">The <see cref="StructuresCalculation{T}"/>.</param>
        /// <returns>The containing <see cref="FailureMechanismSection"/>, or <c>null</c> if none found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sections"/> is <c>null</c></exception>
        /// <exception cref="ArgumentException">Thrown when an element in <paramref name="sections"/> is <c>null</c>.
        /// </exception>
        public static FailureMechanismSection GetFailureMechanismSectionForCalculation<T>(IEnumerable<FailureMechanismSection> sections,
                                                                                       StructuresCalculation<T> calculation)
            where T : IStructuresCalculationInput<StructureBase>, new()
        {
            CalculationWithLocation calculationWithLocation = AsCalculationWithLocation(calculation);
            if (calculationWithLocation != null)
            {
                return AssignUnassignCalculations.FailureMechanismSectionForCalculation(sections, calculationWithLocation);
            }
            return null;
        }

        /// <summary>
        /// Updates the <see cref="StructuresFailureMechanismSectionResult{T}.Calculation"/> for each element
        /// of <see cref="sectionResults"/> if required due to a change.
        /// </summary>
        /// <param name="sectionResults">The <see cref="IEnumerable{T}"/> of <see cref="StructuresFailureMechanismSectionResult{T}"/>
        /// to possibly reassign a calculation to.</param>
        /// <param name="calculations">The <see cref="IEnumerable{T}"/> of <see cref="StructuresCalculation{T}"/> to try 
        /// and match with the <paramref name="sectionResults"/>.</param>
        /// <returns>All affected objects by the deletion.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c> or when an element 
        /// in <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when element in <paramref name="sectionResults"/> is 
        /// <c>null</c>.</exception>
        public static IEnumerable<StructuresFailureMechanismSectionResult<T>> UpdateCalculationToSectionResultAssignments<T>(IEnumerable<StructuresFailureMechanismSectionResult<T>> sectionResults,
                                                                                                                             IEnumerable<StructuresCalculation<T>> calculations)
            where T : IStructuresCalculationInput<StructureBase>, new()
        {
            ValidateSectionResults(sectionResults);

            return AssignUnassignCalculations.Update(
                sectionResults.Select(AsCalculationAssignment),
                AsCalculationsWithLocations(calculations))
                                             .Cast<StructuresFailureMechanismSectionResult<T>>()
                                             .ToArray();
        }

        /// <summary>
        /// Transforms the <paramref name="calculations"/> into <see cref="CalculationWithLocation"/> and filter out the calculations
        /// for which a <see cref="CalculationWithLocation"/> could not be made.
        /// </summary>
        /// <param name="calculations">The <see cref="StructuresCalculation{T}"/> collection to transform.</param>
        /// <returns>A collection of <see cref="CalculationWithLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c> or when
        /// an element in <paramref name="calculations"/> is <c>null</c>.</exception>
        private static IEnumerable<CalculationWithLocation> AsCalculationsWithLocations<T>(IEnumerable<StructuresCalculation<T>> calculations)
            where T : IStructuresCalculationInput<StructureBase>, new()
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }
            return calculations.Select(AsCalculationWithLocation).Where(c => c != null);
        }

        private static void ValidateSectionResults<T>(IEnumerable<StructuresFailureMechanismSectionResult<T>> sectionResults)
            where T : IStructuresCalculationInput<StructureBase>, new()
        {
            if (sectionResults == null)
            {
                throw new ArgumentNullException(nameof(sectionResults));
            }
            if (sectionResults.Any(sr => sr == null))
            {
                throw new ArgumentException(@"SectionResults contains an entry without value.", nameof(sectionResults));
            }
        }

        private static CalculationWithLocation AsCalculationWithLocation<T>(StructuresCalculation<T> calculation)
            where T : IStructuresCalculationInput<StructureBase>, new()
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }
            if (calculation.InputParameters.Structure == null)
            {
                return null;
            }
            return new CalculationWithLocation(calculation, calculation.InputParameters.Structure.Location);
        }

        private static SectionResultWithCalculationAssignment AsCalculationAssignment<T>(StructuresFailureMechanismSectionResult<T> failureMechanismSectionResult)
            where T : IStructuresCalculationInput<StructureBase>, new()
        {
            return new SectionResultWithCalculationAssignment(
                failureMechanismSectionResult,
                result => ((StructuresFailureMechanismSectionResult<T>) result).Calculation,
                (result, calculation) => ((StructuresFailureMechanismSectionResult<T>) result).Calculation = (StructuresCalculation<T>) calculation);
        }
    }
}