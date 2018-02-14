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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Util;

namespace Ringtoets.ClosingStructures.Util
{
    /// <summary>
    /// Class holds helper methods to match <see cref="ClosingStructuresFailureMechanismSectionResult"/> objects 
    /// with <see cref="StructuresCalculation{T}"/> objects. 
    /// </summary>
    public static class ClosingStructuresHelper
    {
        /// <summary>
        /// Updates the <see cref="ClosingStructuresFailureMechanismSectionResult.Calculation"/> for each element
        /// of <see cref="sectionResults"/> if required due to a change.
        /// </summary>
        /// <param name="sectionResults">The <see cref="IEnumerable{T}"/> of <see cref="ClosingStructuresFailureMechanismSectionResult"/>
        /// to possibly reassign a calculation to.</param>
        /// <param name="calculations">The <see cref="IEnumerable{T}"/> of <see cref="StructuresCalculation{T}"/> to try 
        /// and match with the <paramref name="sectionResults"/>.</param>
        /// <returns>All affected objects by the update.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c> or when an element 
        /// in <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when element in <paramref name="sectionResults"/> is 
        /// <c>null</c>.</exception>
        public static IEnumerable<ClosingStructuresFailureMechanismSectionResult> UpdateCalculationToSectionResultAssignments(
            IEnumerable<ClosingStructuresFailureMechanismSectionResult> sectionResults,
            IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculations)
        {
            ValidateSectionResults(sectionResults);

            return AssignUnassignCalculations.Update(sectionResults.Select(AsCalculationAssignment),
                                                     AsCalculationsWithLocations(calculations))
                                             .Cast<ClosingStructuresFailureMechanismSectionResult>()
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
        private static IEnumerable<CalculationWithLocation> AsCalculationsWithLocations(
            IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculations)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            return calculations.Select(AsCalculationWithLocation).Where(c => c != null);
        }

        /// <summary>
        /// Validates the section results.
        /// </summary>
        /// <param name="sectionResults">The <see cref="ClosingStructuresFailureMechanismSectionResult"/> to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResults"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="sectionResults"/> contains <c>null</c> items.</exception>
        private static void ValidateSectionResults(IEnumerable<ClosingStructuresFailureMechanismSectionResult> sectionResults)
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

        private static CalculationWithLocation AsCalculationWithLocation(StructuresCalculation<ClosingStructuresInput> calculation)
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

        private static SectionResultWithCalculationAssignment AsCalculationAssignment(
            ClosingStructuresFailureMechanismSectionResult failureMechanismSectionResult)
        {
            return new SectionResultWithCalculationAssignment(
                failureMechanismSectionResult,
                result => ((ClosingStructuresFailureMechanismSectionResult) result).Calculation,
                (result, calculation) => ((ClosingStructuresFailureMechanismSectionResult) result).Calculation = (StructuresCalculation<ClosingStructuresInput>) calculation);
        }
    }
}