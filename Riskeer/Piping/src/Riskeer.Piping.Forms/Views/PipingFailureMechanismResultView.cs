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
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="AdoptableFailureMechanismSectionResult"/>
    /// in the <see cref="PipingFailureMechanism"/>.
    /// </summary>
    public class PipingFailureMechanismResultView : AdoptableFailureMechanismResultView<PipingFailureMechanism, IPipingCalculationScenario<PipingInput>, PipingInput>
    {
        private readonly RecursiveObserver<IObservableEnumerable<PipingFailureMechanismSectionConfiguration>, PipingFailureMechanismSectionConfiguration> sectionConfigurationsObserver;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismResultView"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of <see cref="AdoptableFailureMechanismSectionResult"/> to
        /// show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the results belong to.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism results belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingFailureMechanismResultView(IObservableEnumerable<AdoptableFailureMechanismSectionResult> failureMechanismSectionResults,
                                                PipingFailureMechanism failureMechanism,
                                                IAssessmentSection assessmentSection)
            : base(failureMechanismSectionResults, failureMechanism, assessmentSection, PipingFailureMechanismAssemblyFactory.AssembleFailureMechanism, PipingFailureMechanismAssemblyFactory.AssembleSection)
        {
            sectionConfigurationsObserver = new RecursiveObserver<IObservableEnumerable<PipingFailureMechanismSectionConfiguration>, PipingFailureMechanismSectionConfiguration>(
                UpdateInternalViewData,
                sc => sc)
            {
                Observable = failureMechanism.SectionConfigurations
            };
        }

        protected override void Dispose(bool disposing)
        {
            sectionConfigurationsObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override IFailureMechanismSectionResultCalculateProbabilityStrategy CreateCalculateStrategy(AdoptableFailureMechanismSectionResult sectionResult,
                                                                                                              IEnumerable<IPipingCalculationScenario<PipingInput>> calculationScenarios)
        {
            return PipingFailureMechanismSectionResultCalculateProbabilityStrategyFactory.CreateCalculateStrategy(sectionResult, FailureMechanism, AssessmentSection);
        }

        protected override IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider CreateErrorProvider(AdoptableFailureMechanismSectionResult sectionResult,
                                                                                                                       IEnumerable<IPipingCalculationScenario<PipingInput>> calculationScenarios)
        {
            return new FailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider<IPipingCalculationScenario<PipingInput>>(
                sectionResult, calculationScenarios,
                (scenario, lineSegments) => scenario.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));
        }

        protected override IEnumerable<IPipingCalculationScenario<PipingInput>> GetCalculationScenarios(AdoptableFailureMechanismSectionResult sectionResult)
        {
            PipingFailureMechanismSectionConfiguration failureMechanismSectionConfigurationForSection = FailureMechanism.GetSectionConfiguration(sectionResult);
            return FailureMechanism.ScenarioConfigurationTypeIsSemiProbabilistic(failureMechanismSectionConfigurationForSection)
                       ? (IEnumerable<IPipingCalculationScenario<PipingInput>>) FailureMechanism.Calculations.OfType<SemiProbabilisticPipingCalculationScenario>().ToArray()
                       : FailureMechanism.Calculations.OfType<ProbabilisticPipingCalculationScenario>().ToArray();
        }
    }
}