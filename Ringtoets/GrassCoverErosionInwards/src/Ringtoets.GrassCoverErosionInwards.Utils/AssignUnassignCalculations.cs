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
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionInwardsFailureMechanism"/> containing the 
        /// <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects.</param>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/>.</param>
        /// <exception cref="ArgumentNullException">When any input parameter is <c>null</c>.</exception>
        public static void Update(GrassCoverErosionInwardsFailureMechanism failureMechanism, GrassCoverErosionInwardsCalculation calculation)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }

            // The FailureMechanismSection which contains the calculation whose dikeprofile has been updated.
            FailureMechanismSection failureMechanismSection = GrassCoverErosionInwardsHelper.FailureMechanismSectionForCalculation(failureMechanism.SectionResults, calculation);

            // All SectionResults (0 or 1) which don't contain the calculation, but do have it assigned, have their calculation set to null.
            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResultsUsingCalculation = 
                failureMechanism.SectionResults.Where(sr => sr.Calculation != null && sr.Calculation.Equals(calculation));
            foreach (var sectionResult in sectionResultsUsingCalculation)
            {
                if (!sectionResult.Section.Equals(failureMechanismSection))
                {
                    sectionResult.Calculation = null;
                }
            }

            // Assign the calculation to the SectionResult which contains it, if that SectionResult currently has no calculation set.
            foreach (var sectionResult in failureMechanism.SectionResults)
            {
                if (sectionResult.Section.Equals(failureMechanismSection) && sectionResult.Calculation == null)
                {
                    sectionResult.Calculation = calculation;
                }
            }
        }

        /// <summary>
        /// Update <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects which use the deleted <see cref="GrassCoverErosionInwardsCalculation"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionInwardsFailureMechanism"/> containing the 
        /// <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects.</param>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/>.</param>
        /// <exception cref="ArgumentNullException">When any input parameter is <c>null</c>.</exception>
        public static void Delete(GrassCoverErosionInwardsFailureMechanism failureMechanism, GrassCoverErosionInwardsCalculation calculation)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }

            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResultsUsingCalculation =
                failureMechanism.SectionResults.Where(sr => sr.Calculation != null && sr.Calculation.Equals(calculation));
            foreach (var sectionResult in sectionResultsUsingCalculation)
            {
                sectionResult.Calculation = null;
            }
        }
    }
}