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
using System.Linq;
using Core.Common.Base.Geometry;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Piping.Data.Probabilistic;

namespace Riskeer.Piping.Data
{
    /// <summary>
    /// Factory for creating <see cref="ProbabilityAssessmentOutput"/> for piping.
    /// </summary>
    public static class PipingProbabilityAssessmentOutputFactory
    {
        /// <summary>
        /// Creates <see cref="ProbabilityAssessmentOutput"/> based on the provided parameters.
        /// </summary>
        /// <param name="output">The output to get the reliability from.</param>
        /// <param name="calculation">The calculation the output belongs to.</param>
        /// <param name="failureMechanism">The failure mechanism the output belongs to.</param>
        /// <param name="assessmentSection">The assessment section the output belongs to.</param>
        /// <returns>The calculated <see cref="ProbabilityAssessmentOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static ProbabilityAssessmentOutput Create(IPartialProbabilisticPipingOutput output,
                                                         ProbabilisticPipingCalculationScenario calculation,
                                                         PipingFailureMechanism failureMechanism,
                                                         IAssessmentSection assessmentSection)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return ProbabilityAssessmentOutputFactory.Create(assessmentSection.FailureMechanismContribution.Norm,
                                                             failureMechanism.Contribution,
                                                             GetSectionLength(calculation, failureMechanism),
                                                             output.Reliability);
        }

        private static double GetSectionLength(ProbabilisticPipingCalculationScenario calculation, PipingFailureMechanism failureMechanism)
        {
            FailureMechanismSection failureMechanismSection = failureMechanism
                                                              .Sections
                                                              .First(section => calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(Math2D.ConvertPointsToLineSegments(section.Points)));

            return failureMechanismSection.Length;
        }
    }
}