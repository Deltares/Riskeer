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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;

namespace Riskeer.Piping.Data.SemiProbabilistic
{
    /// <summary>
    /// Strategy to calculate probabilities for <see cref="SemiProbabilisticPipingCalculationScenario"/>.
    /// </summary>
    public class SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy : IFailureMechanismSectionResultCalculateProbabilityStrategy
    {
        private readonly AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult;
        private readonly IEnumerable<SemiProbabilisticPipingCalculationScenario> calculations;
        private readonly PipingFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResult"/> to get the probabilities for.</param>
        /// <param name="calculations">All the <see cref="SemiProbabilisticPipingCalculationScenario"/> of the failure mechanism. </param>
        /// <param name="failureMechanism">The failure mechanism the calculation scenarios belong to.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult,
                                                                                                IEnumerable<SemiProbabilisticPipingCalculationScenario> calculations,
                                                                                                PipingFailureMechanism failureMechanism,
                                                                                                IAssessmentSection assessmentSection)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.sectionResult = sectionResult;
            this.calculations = calculations;
            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;
        }

        public double CalculateProfileProbability()
        {
            return sectionResult.GetInitialFailureMechanismResultProbability(calculations, assessmentSection.FailureMechanismContribution.Norm);
        }

        public double CalculateSectionProbability()
        {
            return CalculateProfileProbability() * failureMechanism.PipingProbabilityAssessmentInput.GetN(
                       sectionResult.Section.Length);
        }
    }
}