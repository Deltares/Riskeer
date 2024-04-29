﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Builders;
using Riskeer.Common.Forms.Providers;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResult"/>
    /// in the <see cref="PipingFailureMechanism"/>.
    /// </summary>
    public class PipingFailureMechanismResultView : FailureMechanismResultView<AdoptableWithProfileProbabilityFailureMechanismSectionResult,
        AdoptableWithProfileProbabilityFailureMechanismSectionResultRow, PipingFailureMechanism>
    {
        private const int initialFailureMechanismResultTypeIndex = 2;
        private const int initialFailureMechanismResultSectionProbabilityIndex = 3;
        private const int furtherAnalysisTypeIndex = 4;
        private const int refinedSectionProbabilityIndex = 5;
        private const int sectionProbabilityIndex = 6;
        private const int assemblyGroupIndex = 7;

        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputsObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;
        private readonly RecursiveObserver<IObservableEnumerable<PipingScenarioConfigurationPerFailureMechanismSection>, PipingScenarioConfigurationPerFailureMechanismSection> scenarioConfigurationsPerSectionObserver;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismResultView"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResult"/> to
        /// show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the results belong to.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism results belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingFailureMechanismResultView(IObservableEnumerable<AdoptableWithProfileProbabilityFailureMechanismSectionResult> failureMechanismSectionResults,
                                                PipingFailureMechanism failureMechanism,
                                                IAssessmentSection assessmentSection)
            : base(failureMechanismSectionResults, failureMechanism, assessmentSection, PipingFailureMechanismAssemblyFactory.AssembleFailureMechanism)
        {
            // The concat is needed to observe the input of calculations in child groups.
            calculationInputsObserver = new RecursiveObserver<CalculationGroup, ICalculationInput>(
                UpdateInternalViewData,
                cg => cg.Children.Concat<object>(cg.Children
                                                   .OfType<IPipingCalculationScenario<PipingInput>>()
                                                   .Select(c => c.InputParameters)))
            {
                Observable = failureMechanism.CalculationsGroup
            };
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, ICalculationBase>(
                UpdateInternalViewData,
                c => c.Children)
            {
                Observable = failureMechanism.CalculationsGroup
            };

            scenarioConfigurationsPerSectionObserver = new RecursiveObserver<IObservableEnumerable<PipingScenarioConfigurationPerFailureMechanismSection>, PipingScenarioConfigurationPerFailureMechanismSection>(
                UpdateInternalViewData,
                sc => sc)
            {
                Observable = failureMechanism.ScenarioConfigurationsPerFailureMechanismSection
            };
        }

        protected override void Dispose(bool disposing)
        {
            calculationInputsObserver.Dispose();
            calculationGroupObserver.Dispose();
            scenarioConfigurationsPerSectionObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override AdoptableWithProfileProbabilityFailureMechanismSectionResultRow CreateFailureMechanismSectionResultRow(AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult)
        {
            return new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(
                sectionResult,
                PipingFailureMechanismSectionResultCalculateProbabilityStrategyFactory.CreateCalculateStrategy(sectionResult, FailureMechanism, AssessmentSection),
                CreateErrorProvider(sectionResult),
                () => PipingFailureMechanismAssemblyFactory.AssembleSection(sectionResult, FailureMechanism, AssessmentSection),
                new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.ConstructionProperties
                {
                    InitialFailureMechanismResultTypeIndex = initialFailureMechanismResultTypeIndex,
                    InitialFailureMechanismResultSectionProbabilityIndex = initialFailureMechanismResultSectionProbabilityIndex,
                    FurtherAnalysisTypeIndex = furtherAnalysisTypeIndex,
                    RefinedSectionProbabilityIndex = refinedSectionProbabilityIndex,
                    SectionProbabilityIndex = sectionProbabilityIndex,
                    AssemblyGroupIndex = assemblyGroupIndex
                });
        }

        protected override void AddDataGridColumns()
        {
            FailureMechanismSectionResultViewColumnBuilder.AddSectionNameColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.Name));

            FailureMechanismSectionResultViewColumnBuilder.AddIsRelevantColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.IsRelevant));

            FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultTypeColumn<AdoptableInitialFailureMechanismResultType>(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.InitialFailureMechanismResultType));

            FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultSectionProbabilityColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.InitialFailureMechanismResultSectionProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddFurtherAnalysisTypeColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.FurtherAnalysisType));

            FailureMechanismSectionResultViewColumnBuilder.AddRefinedSectionProbabilityColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.RefinedSectionProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddAssemblySectionProbabilityColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.SectionProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddAssemblyGroupColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.AssemblyGroup));
        }

        private IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider CreateErrorProvider(FailureMechanismSectionResult sectionResult)
        {
            PipingScenarioConfigurationPerFailureMechanismSection scenarioConfigurationForSection = FailureMechanism.GetScenarioConfigurationForSection(sectionResult);
            IEnumerable<IPipingCalculationScenario<PipingInput>> calculationScenarios = FailureMechanism.ScenarioConfigurationTypeIsSemiProbabilistic(scenarioConfigurationForSection)
                                                                                            ? (IEnumerable<IPipingCalculationScenario<PipingInput>>) FailureMechanism.Calculations.OfType<SemiProbabilisticPipingCalculationScenario>()
                                                                                            : FailureMechanism.Calculations.OfType<ProbabilisticPipingCalculationScenario>();

            return new FailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider<IPipingCalculationScenario<PipingInput>>(
                sectionResult, calculationScenarios,
                (scenario, lineSegments) => scenario.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));
        }
    }
}