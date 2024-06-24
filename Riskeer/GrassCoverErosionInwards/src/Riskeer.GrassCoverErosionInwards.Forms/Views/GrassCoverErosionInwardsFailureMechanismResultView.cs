// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Providers;
using Riskeer.Common.Forms.Views;
using Riskeer.GrassCoverErosionInwards.Data;

namespace Riskeer.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="AdoptableFailureMechanismSectionResult"/>
    /// in the <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionInwardsFailureMechanismResultView : AdoptableFailureMechanismResultView<GrassCoverErosionInwardsFailureMechanism,
        GrassCoverErosionInwardsCalculationScenario, GrassCoverErosionInwardsInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsFailureMechanismResultView"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of
        /// <see cref="AdoptableFailureMechanismSectionResult"/> to show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the results belong to.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism results belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GrassCoverErosionInwardsFailureMechanismResultView(IObservableEnumerable<AdoptableFailureMechanismSectionResult> failureMechanismSectionResults,
                                                                  GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                                  IAssessmentSection assessmentSection)
            : base(failureMechanismSectionResults, failureMechanism, assessmentSection,
                   GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                   GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSection) {}

        protected override IFailureMechanismSectionResultCalculateProbabilityStrategy CreateCalculateStrategy(AdoptableFailureMechanismSectionResult sectionResult,
                                                                                                              IEnumerable<GrassCoverErosionInwardsCalculationScenario> calculationScenarios)
        {
            return new GrassCoverErosionInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(sectionResult, calculationScenarios);
        }

        protected override IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider CreateErrorProvider(AdoptableFailureMechanismSectionResult sectionResult,
                                                                                                                       IEnumerable<GrassCoverErosionInwardsCalculationScenario> calculationScenarios)
        {
            return new FailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider<GrassCoverErosionInwardsCalculationScenario>(
                sectionResult, calculationScenarios,
                (scenario, lineSegments) => scenario.IsDikeProfileIntersectionWithReferenceLineInSection(lineSegments));
        }

        protected override IEnumerable<GrassCoverErosionInwardsCalculationScenario> GetCalculationScenarios(AdoptableFailureMechanismSectionResult sectionResult)
        {
            return FailureMechanism.Calculations
                                   .OfType<GrassCoverErosionInwardsCalculationScenario>()
                                   .ToArray();
        }
    }
}