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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Utils
{
    /// <summary>
    /// Utility class for data synchronization of the <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult.Calculation"/> 
    /// of <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects.
    /// </summary>
    public static class AssignUnassignCalculations
    {
        /// <summary>
        /// Update <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects which used or can use the <see cref="GrassCoverErosionInwardsCalculation"/>.
        /// </summary>
        /// <param name="sectionResults">The <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects.</param>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/>.</param>
        /// <exception cref="ArgumentNullException">When any input parameter is <c>null</c>.</exception>
        public static void Update(IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults, GrassCoverErosionInwardsCalculation calculation)
        {
            if (sectionResults == null)
            {
                throw new ArgumentNullException("sectionResults");
            }
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }

            var sectionResultsArray = sectionResults.ToArray();

            FailureMechanismSection failureMechanismSectionContainingCalculation =
                GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(sectionResultsArray, calculation);

            UnassignCalculationInSectionResultsNotContainingCalculation(calculation, sectionResultsArray, failureMechanismSectionContainingCalculation);

            AssignCalculationIfContainingSectionResultHasNoCalculationAssigned(calculation, sectionResultsArray, failureMechanismSectionContainingCalculation);
        }

        /// <summary>
        /// Update <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects which use the deleted <see cref="GrassCoverErosionInwardsCalculation"/>.
        /// </summary>
        /// <param name="sectionResults">The <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects.</param>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/>.</param>
        /// <param name="calculations"></param>
        /// <exception cref="ArgumentNullException">When any input parameter is <c>null</c>.</exception>
        public static void Delete(
            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults, 
            GrassCoverErosionInwardsCalculation calculation, 
            IEnumerable<GrassCoverErosionInwardsCalculation> calculations)
        {
            if (sectionResults == null)
            {
                throw new ArgumentNullException("sectionResults");
            }
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }

            var sectionResultsArray = sectionResults.ToArray();

            Dictionary<string, IList<GrassCoverErosionInwardsCalculation>> calculationsPerSegmentName =
                GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(sectionResultsArray, calculations);

            UnassignCalculationInAllSectionResultsAndAssignSingleRemainingCalculation(sectionResultsArray, calculation, calculationsPerSegmentName);
        }

        private static void UnassignCalculationInAllSectionResultsAndAssignSingleRemainingCalculation(
            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults, 
            GrassCoverErosionInwardsCalculation calculation, Dictionary<string, 
            IList<GrassCoverErosionInwardsCalculation>> calculationsPerSegmentName)
        {
            var sectionsContainingOneCalculation = calculationsPerSegmentName.Where(kvp => kvp.Value.Count == 1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResultsUsingCalculation =
                sectionResults.Where(sr => sr.Calculation != null && sr.Calculation.Equals(calculation));
            foreach (var sectionResult in sectionResultsUsingCalculation)
            {
                string sectionName = sectionResult.Section.Name;
                if (sectionsContainingOneCalculation.ContainsKey(sectionName))
                {
                    sectionResult.Calculation = sectionsContainingOneCalculation[sectionName][0];
                    continue;
                }
                sectionResult.Calculation = null;
            }
        }

        private static void AssignCalculationIfContainingSectionResultHasNoCalculationAssigned(
            GrassCoverErosionInwardsCalculation calculation,
            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults,
            FailureMechanismSection failureMechanismSection)
        {
            foreach (var sectionResult in sectionResults)
            {
                if (sectionResult.Section.Equals(failureMechanismSection) && sectionResult.Calculation == null)
                {
                    sectionResult.Calculation = calculation;
                }
            }
        }

        private static void UnassignCalculationInSectionResultsNotContainingCalculation(
            GrassCoverErosionInwardsCalculation calculation,
            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults,
            FailureMechanismSection failureMechanismSection)
        {
            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResultsUsingCalculation =
                sectionResults.Where(sr => sr.Calculation != null && sr.Calculation.Equals(calculation));
            foreach (var sectionResult in sectionResultsUsingCalculation)
            {
                if (!sectionResult.Section.Equals(failureMechanismSection))
                {
                    sectionResult.Calculation = null;
                }
            }
        }
    }
}