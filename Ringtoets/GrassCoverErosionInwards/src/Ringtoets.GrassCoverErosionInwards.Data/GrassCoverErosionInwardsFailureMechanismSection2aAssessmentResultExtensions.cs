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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Extension methods for obtaining level 2a results from output for an assessment of the 
    /// grass cover erosion inwards failure mechanism.
    /// </summary>
    public static class GrassCoverErosionInwardsFailureMechanismSection2aAssessmentResultExtensions
    {
        /// <summary>
        /// Gets the value for the detailed assessment of safety per failure mechanism section as a probability.
        /// </summary>
        /// <param name="sectionResult">The section result to get the assessment layer 2A for.</param>
        /// <param name="failureMechanism">The failure mechanism the calculations belong to.</param>
        /// <param name="assessmentSection">The assessment section the calculations belong to.</param>
        /// <returns>The calculated assessment layer 2A; or <see cref="double.NaN"/> when there are no
        /// is no calculation assigned to the section result or the calculation is not performed.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static double GetAssessmentLayerTwoA(this GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult,
                                                    GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                    IAssessmentSection assessmentSection)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (sectionResult.Calculation == null || !sectionResult.Calculation.HasOutput)
            {
                return double.NaN;
            }

            ProbabilityAssessmentOutput derivedOutput = GrassCoverErosionInwardsProbabilityAssessmentOutputFactory.Create(sectionResult.Calculation.Output.OvertoppingOutput,
                                                                                                                          failureMechanism, assessmentSection);

            return derivedOutput.Probability;
        }
    }
}