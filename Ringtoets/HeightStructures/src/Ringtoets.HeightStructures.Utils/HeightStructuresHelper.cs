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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Utils;
using Ringtoets.HeightStructures.Data;

namespace Ringtoets.HeightStructures.Utils
{
    /// <summary>
    /// Class holds helper methods to match <see cref="FailureMechanismSection"/> objects 
    /// with <see cref="HeightStructuresCalculation"/> objects. 
    /// </summary>
    public static class HeightStructuresHelper
    {
        /// <summary>
        /// Determine which <see cref="HeightStructuresCalculation"/> objects are available for a
        /// <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="sections">The <see cref="FailureMechanismSection"/> objects.</param>
        /// <param name="calculations">The <see cref="CalculationWithLocation"/> objects.</param>
        /// <returns>A <see cref="Dictionary{K, V}"/> containing a <see cref="IList{T}"/> 
        /// of <see cref="FailureMechanismSectionResult"/> objects 
        /// for each section name which has calculations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>
        /// or when an element in <paramref name="calculations"/> is <c>null</c></exception>
        /// <exception cref="ArgumentException">Thrown when an element in <paramref name="sections"/> is 
        /// <c>null</c>.</exception>
        public static Dictionary<string, IList<ICalculation>> CollectCalculationsPerSection(IEnumerable<FailureMechanismSection> sections, IEnumerable<HeightStructuresCalculation> calculations)
        {
            return AssignUnassignCalculations.CollectCalculationsPerSection(sections, AsCalculationsWithLocations(calculations));
        }

        /// <summary>
        /// Determine which <see cref="FailureMechanismSection"/> geometrically contains the <see cref="HeightStructuresCalculation"/>.
        /// </summary>
        /// <param name="sections">The <see cref="FailureMechanismSection"/> objects 
        /// whose <see cref="FailureMechanismSection"/> are considered.</param>
        /// <param name="calculation">The <see cref="HeightStructuresCalculation"/>.</param>
        /// <returns>The containing <see cref="FailureMechanismSection"/>, or <c>null</c> if none found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sections"/> is <c>null</c></exception>
        /// <exception cref="ArgumentException">Thrown when  an element in <paramref name="sections"/> is <c>null</c>.
        /// </exception>
        public static FailureMechanismSection FailureMechanismSectionForCalculation(IEnumerable<FailureMechanismSection> sections,
                                                                                    HeightStructuresCalculation calculation)
        {
            var asCalculationWithLocation = AsCalculationWithLocation(calculation);
            if (asCalculationWithLocation != null)
            {
                return AssignUnassignCalculations.FailureMechanismSectionForCalculation(sections, asCalculationWithLocation);
            }
            return null;
        }

        /// <summary>
        /// Updates the <see cref="HeightStructuresFailureMechanismSectionResult.Calculation"/> for each element of <see cref="sectionResults"/> that have the
        /// <paramref name="calculation"/> assigned, or should have the <paramref name="calculation"/> assigned.
        /// </summary>
        /// <param name="sectionResults">The <see cref="IEnumerable{T}"/> of <see cref="HeightStructuresFailureMechanismSectionResult"/> to iterate while 
        /// possibly updating the <see cref="HeightStructuresCalculation"/> assigned to it.</param>
        /// <param name="calculation">The <see cref="HeightStructuresCalculation"/> which has a location that has been updated.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="sectionResults"/> is <c>null</c></exception>
        /// <exception cref="ArgumentException">Thrown when element in <paramref name="sectionResults"/> is 
        /// <c>null</c>.</exception>
        public static void Update(IEnumerable<HeightStructuresFailureMechanismSectionResult> sectionResults, HeightStructuresCalculation calculation)
        {
            ValidateSectionResults(sectionResults);

            var asCalculationWithLocation = AsCalculationWithLocation(calculation);
            if (asCalculationWithLocation != null)
            {
                AssignUnassignCalculations.Update(sectionResults.Select(AsCalculationAssignment), asCalculationWithLocation);
            }
        }

        /// <summary>
        /// Removed the <see cref="HeightStructuresFailureMechanismSectionResult.Calculation"/> for each 
        /// element of <see cref="sectionResults"/> that have the <paramref name="calculation"/> assigned.
        /// </summary>
        /// <param name="sectionResults">The <see cref="IEnumerable{T}"/> of <see cref="HeightStructuresFailureMechanismSectionResult"/> to iterate while 
        /// removing the reference to the <paramref name="calculation"/> if present.</param>
        /// <param name="calculation">The <see cref="HeightStructuresCalculation"/> which has a location that has been updated.</param>
        /// <param name="calculations">The <see cref="IEnumerable{T}"/> of <see cref="HeightStructuresCalculation"/> that were left after removing
        /// <paramref name="calculation"/>.</param>
        /// <exception cref="ArgumentNullException">When any input parameter is <c>null</c> or when an element 
        /// in <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when element in <paramref name="sectionResults"/> is 
        /// <c>null</c>.</exception>
        public static void Delete(IEnumerable<HeightStructuresFailureMechanismSectionResult> sectionResults, HeightStructuresCalculation calculation, IEnumerable<HeightStructuresCalculation> calculations)
        {
            ValidateSectionResults(sectionResults);

            AssignUnassignCalculations.Delete(
                sectionResults.Select(AsCalculationAssignment),
                calculation,
                AsCalculationsWithLocations(calculations));
        }

        /// <summary>
        /// Transforms the <paramref name="calculations"/> into <see cref="CalculationWithLocation"/> and filter out the calculations
        /// for which a <see cref="CalculationWithLocation"/> could not be made.
        /// </summary>
        /// <param name="calculations">The <see cref="HeightStructuresCalculation"/> collection to transform.</param>
        /// <returns>A collection of <see cref="CalculationWithLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c> or when
        /// an element in <paramref name="calculations"/> is <c>null</c>.</exception>
        private static IEnumerable<CalculationWithLocation> AsCalculationsWithLocations(IEnumerable<HeightStructuresCalculation> calculations)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException("calculations");
            }
            return calculations.Select(AsCalculationWithLocation).Where(c => c != null);
        }

        private static void ValidateSectionResults(IEnumerable<HeightStructuresFailureMechanismSectionResult> sectionResults)
        {
            if (sectionResults == null)
            {
                throw new ArgumentNullException("sectionResults");
            }
            if (sectionResults.Any(sr => sr == null))
            {
                throw new ArgumentException("SectionResults contains an entry without value.", "sectionResults");
            }
        }

        private static CalculationWithLocation AsCalculationWithLocation(HeightStructuresCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }
            if (calculation.InputParameters.Structure == null)
            {
                return null;
            }
            return new CalculationWithLocation(calculation, calculation.InputParameters.Structure.Location);
        }

        private static SectionResultWithCalculationAssignment AsCalculationAssignment(HeightStructuresFailureMechanismSectionResult failureMechanismSectionResult)
        {
            return new SectionResultWithCalculationAssignment(failureMechanismSectionResult,
                                                              result => ((HeightStructuresFailureMechanismSectionResult)result).Calculation,
                                                              (result, calculation) => ((HeightStructuresFailureMechanismSectionResult)result).Calculation = (HeightStructuresCalculation)calculation);
        }
    }
}