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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Util;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Util
{
    /// <summary>
    /// Class which defines helper methods to match <see cref="GrassCoverErosionInwardsCalculation"/> objects 
    /// with <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects. 
    /// </summary>
    public static class GrassCoverErosionInwardsHelper
    {
        /// <summary>
        /// Determine which <see cref="GrassCoverErosionInwardsCalculation"/> objects are available for a
        /// <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="sections">The <see cref="FailureMechanismSection"/> objects.</param>
        /// <param name="calculations">The <see cref="CalculationWithLocation"/> objects.</param>
        /// <returns>A <see cref="IDictionary{K, V}"/> containing a <see cref="List{T}"/> 
        /// of <see cref="GrassCoverErosionInwardsCalculation"/> objects 
        /// for each section which has calculations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>
        /// or when an element in <paramref name="calculations"/> is <c>null</c></exception>
        /// <exception cref="ArgumentException">Thrown when an element in <paramref name="sections"/> is 
        /// <c>null</c>.</exception>
        public static IDictionary<string, List<ICalculation>> CollectCalculationsPerSection(IEnumerable<FailureMechanismSection> sections,
                                                                                            IEnumerable<GrassCoverErosionInwardsCalculation> calculations)
        {
            return AssignUnassignCalculations.CollectCalculationsPerSection(sections, CalculationsToCalculationsWithLocations(calculations));
        }

        /// <summary>
        /// Removed the <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult.Calculation"/> for each 
        /// element of <see cref="sectionResults"/> if required due to a change.
        /// </summary>
        /// <param name="sectionResults">The <see cref="IEnumerable{T}"/> of <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>
        /// to possibly reassign a calculation to.</param>
        /// <param name="calculations">The <see cref="IEnumerable{T}"/> of <see cref="GrassCoverErosionInwardsCalculation"/> to try 
        /// and match with the <paramref name="sectionResults"/>.</param>
        /// <returns>All section results affected by the deletion.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c> or when an element 
        /// in <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when element in <paramref name="sectionResults"/> is 
        /// <c>null</c>.</exception>
        public static IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> UpdateCalculationToSectionResultAssignments(IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults,
                                                                                                                                     IEnumerable<GrassCoverErosionInwardsCalculation> calculations)
        {
            ValidateSectionResults(sectionResults);

            return AssignUnassignCalculations.Update(
                                                 sectionResults.Select(AsCalculationAssignment),
                                                 CalculationsToCalculationsWithLocations(calculations))
                                             .Cast<GrassCoverErosionInwardsFailureMechanismSectionResult>()
                                             .ToArray();
        }

        /// <summary>
        /// Transforms the <paramref name="calculations"/> into <see cref="CalculationWithLocation"/> and filter out the calculations
        /// for which a <see cref="CalculationWithLocation"/> could not be made.
        /// </summary>
        /// <param name="calculations">The <see cref="GrassCoverErosionInwardsCalculation"/> collection to transform.</param>
        /// <returns>A collection of <see cref="CalculationWithLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c> or when
        /// an element in <paramref name="calculations"/> is <c>null</c>.</exception>
        private static IEnumerable<CalculationWithLocation> CalculationsToCalculationsWithLocations(
            IEnumerable<GrassCoverErosionInwardsCalculation> calculations)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            return calculations.Select(AsCalculationWithLocation).Where(c => c != null);
        }

        private static void ValidateSectionResults(IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults)
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

        /// <summary>
        /// Transforms the <paramref name="calculation"/> into a <see cref="CalculationWithLocation"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/> to transform.</param>
        /// <returns>A new <see cref="CalculationWithLocation"/> or <c>null</c> if the calculation had no dike profile assigned.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        private static CalculationWithLocation AsCalculationWithLocation(GrassCoverErosionInwardsCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (calculation.InputParameters.DikeProfile == null)
            {
                return null;
            }

            return new CalculationWithLocation(calculation, calculation.InputParameters.DikeProfile.WorldReferencePoint);
        }

        private static SectionResultWithCalculationAssignment AsCalculationAssignment(
            GrassCoverErosionInwardsFailureMechanismSectionResult failureMechanismSectionResult)
        {
            return new SectionResultWithCalculationAssignment(failureMechanismSectionResult,
                                                              result => ((GrassCoverErosionInwardsFailureMechanismSectionResult) result).Calculation,
                                                              (result, calculation) => ((GrassCoverErosionInwardsFailureMechanismSectionResult) result).Calculation = (GrassCoverErosionInwardsCalculation) calculation);
        }
    }
}