// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Forms;
using Riskeer.Common.Forms.Builders;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="PipingFailureMechanismSectionResult"/>.
    /// </summary>
    public class PipingFailureMechanismResultView : FailureMechanismResultView<PipingFailureMechanismSectionResult,
        PipingFailureMechanismSectionResultRow, PipingFailureMechanism>
    {
        private const int initialFailureMechanismResultIndex = 2;
        private const int initialFailureMechanismResultProfileProbabilityIndex = 3;
        private const int initialFailureMechanismResultSectionProbabilityIndex = 4;
        private const int furtherAnalysisNeededIndex = 5;
        private const int probabilityRefinementTypeIndex = 6;
        private const int refinedProfileProbabilityIndex = 7;
        private const int refinedSectionProbabilityIndex = 8;
        private const int profileProbabilityIndex = 9;
        private const int sectionProbabilityIndex = 10;
        private const int sectionNIndex = 11;
        private const int assemblyGroupIndex = 12;

        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;
        private readonly RecursiveObserver<IObservableEnumerable<PipingScenarioConfigurationPerFailureMechanismSection>, PipingScenarioConfigurationPerFailureMechanismSection> scenarioConfigurationsPerSectionObserver;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismResultView"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of <see cref="PipingFailureMechanismSectionResult"/> to
        /// show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the results belong to.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism results belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingFailureMechanismResultView(IObservableEnumerable<PipingFailureMechanismSectionResult> failureMechanismSectionResults,
                                                PipingFailureMechanism failureMechanism,
                                                IAssessmentSection assessmentSection)
            : base(failureMechanismSectionResults, failureMechanism)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.assessmentSection = assessmentSection;

            // The concat is needed to observe the input of calculations in child groups.
            calculationInputObserver = new RecursiveObserver<CalculationGroup, ICalculationInput>(
                UpdateView,
                cg => cg.Children.Concat<object>(cg.Children
                                                   .OfType<IPipingCalculationScenario<PipingInput>>()
                                                   .Select(c => c.InputParameters)))
            {
                Observable = failureMechanism.CalculationsGroup
            };
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, ICalculationBase>(
                UpdateView,
                c => c.Children)
            {
                Observable = failureMechanism.CalculationsGroup
            };

            scenarioConfigurationsPerSectionObserver = new RecursiveObserver<IObservableEnumerable<PipingScenarioConfigurationPerFailureMechanismSection>, PipingScenarioConfigurationPerFailureMechanismSection>(
                UpdateView,
                sc => sc)
            {
                Observable = failureMechanism.ScenarioConfigurationsPerFailureMechanismSection
            };
        }

        protected override void Dispose(bool disposing)
        {
            calculationInputObserver.Dispose();
            calculationGroupObserver.Dispose();
            scenarioConfigurationsPerSectionObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override PipingFailureMechanismSectionResultRow CreateFailureMechanismSectionResultRow(PipingFailureMechanismSectionResult sectionResult)
        {
            PipingScenarioConfigurationPerFailureMechanismSection scenarioConfigurationForSection = GetScenarioConfigurationForSection(sectionResult);
            
            return new PipingFailureMechanismSectionResultRow(
                sectionResult, CreateCalculateStrategy(sectionResult, scenarioConfigurationForSection),
                CreateErrorProvider(sectionResult, scenarioConfigurationForSection),
                FailureMechanism, assessmentSection,
                new PipingFailureMechanismSectionResultRow.ConstructionProperties
                {
                    InitialFailureMechanismResultIndex = initialFailureMechanismResultIndex,
                    InitialFailureMechanismResultProfileProbabilityIndex = initialFailureMechanismResultProfileProbabilityIndex,
                    InitialFailureMechanismResultSectionProbabilityIndex = initialFailureMechanismResultSectionProbabilityIndex,
                    FurtherAnalysisNeededIndex = furtherAnalysisNeededIndex,
                    ProbabilityRefinementTypeIndex = probabilityRefinementTypeIndex,
                    RefinedProfileProbabilityIndex = refinedProfileProbabilityIndex,
                    RefinedSectionProbabilityIndex = refinedSectionProbabilityIndex,
                    ProfileProbabilityIndex = profileProbabilityIndex,
                    SectionProbabilityIndex = sectionProbabilityIndex,
                    SectionNIndex = sectionNIndex,
                    AssemblyGroupIndex = assemblyGroupIndex
                });
        }

        protected override void AddDataGridColumns()
        {
            FailureMechanismSectionResultViewColumnBuilder.AddSectionNameColumn(
                DataGridViewControl,
                nameof(PipingFailureMechanismSectionResultRow.Name));

            FailureMechanismSectionResultViewColumnBuilder.AddIsRelevantColumn(
                DataGridViewControl,
                nameof(PipingFailureMechanismSectionResultRow.IsRelevant));

            FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultColumn(
                DataGridViewControl,
                nameof(PipingFailureMechanismSectionResultRow.InitialFailureMechanismResult));

            FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultProfileProbabilityColumn(
                DataGridViewControl,
                nameof(PipingFailureMechanismSectionResultRow.InitialFailureMechanismResultProfileProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultSectionProbabilityColumn(
                DataGridViewControl,
                nameof(PipingFailureMechanismSectionResultRow.InitialFailureMechanismResultSectionProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddFurtherAnalysisNeededColumn(
                DataGridViewControl,
                nameof(PipingFailureMechanismSectionResultRow.FurtherAnalysisNeeded));

            FailureMechanismSectionResultViewColumnBuilder.AddProbabilityRefinementTypeColumn(
                DataGridViewControl,
                nameof(PipingFailureMechanismSectionResultRow.ProbabilityRefinementType));

            FailureMechanismSectionResultViewColumnBuilder.AddRefinedProfileProbabilityColumn(
                DataGridViewControl,
                nameof(PipingFailureMechanismSectionResultRow.RefinedProfileProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddRefinedSectionProbabilityColumn(
                DataGridViewControl,
                nameof(PipingFailureMechanismSectionResultRow.RefinedSectionProbability));
            
            FailureMechanismSectionResultViewColumnBuilder.AddProfileProbabilityColumn(
                DataGridViewControl,
                nameof(PipingFailureMechanismSectionResultRow.ProfileProbability));
            
            FailureMechanismSectionResultViewColumnBuilder.AddSectionProbabilityColumn(
                DataGridViewControl,
                nameof(PipingFailureMechanismSectionResultRow.SectionProbability));
            
            FailureMechanismSectionResultViewColumnBuilder.AddSectionNColumn(
                DataGridViewControl,
                nameof(PipingFailureMechanismSectionResultRow.SectionN));
            
            FailureMechanismSectionResultViewColumnBuilder.AddAssemblyGroupColumn(
                DataGridViewControl,
                nameof(PipingFailureMechanismSectionResultRow.AssemblyGroup));
        }
        
        private IInitialFailureMechanismResultErrorProvider CreateErrorProvider(FailureMechanismSectionResult sectionResult,
                                                                                PipingScenarioConfigurationPerFailureMechanismSection scenarioConfigurationForSection)
        {
            IEnumerable<IPipingCalculationScenario<PipingInput>> calculationScenarios;
            if (FailureMechanism.ScenarioConfigurationType == PipingScenarioConfigurationType.SemiProbabilistic
                || FailureMechanism.ScenarioConfigurationType == PipingScenarioConfigurationType.PerFailureMechanismSection
                && scenarioConfigurationForSection.ScenarioConfigurationType == PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic)
            {
                calculationScenarios = FailureMechanism.Calculations.OfType<SemiProbabilisticPipingCalculationScenario>();
            }
            else
            {
                calculationScenarios = FailureMechanism.Calculations.OfType<ProbabilisticPipingCalculationScenario>();
            }

            return new InitialFailureMechanismResultErrorProvider<IPipingCalculationScenario<PipingInput>>(
                sectionResult, calculationScenarios, (scenario, lineSegments) => scenario.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));
        }

        private IPipingFailureMechanismSectionResultCalculateProbabilityStrategy CreateCalculateStrategy(PipingFailureMechanismSectionResult sectionResult,
                                                                                                         PipingScenarioConfigurationPerFailureMechanismSection scenarioConfigurationForSection)
        {
            if (FailureMechanism.ScenarioConfigurationType == PipingScenarioConfigurationType.SemiProbabilistic
                || FailureMechanism.ScenarioConfigurationType == PipingScenarioConfigurationType.PerFailureMechanismSection
                && scenarioConfigurationForSection.ScenarioConfigurationType == PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic)
            {
                return CreateSemiProbabilisticCalculateStrategy(sectionResult);
            }

            return CreateProbabilisticCalculateStrategy(sectionResult);
        }

        private ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy CreateProbabilisticCalculateStrategy(PipingFailureMechanismSectionResult sectionResult)
        {
            return new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, FailureMechanism.Calculations.OfType<ProbabilisticPipingCalculationScenario>());
        }

        private SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy CreateSemiProbabilisticCalculateStrategy(PipingFailureMechanismSectionResult sectionResult)
        {
            return new SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, FailureMechanism.Calculations.OfType<SemiProbabilisticPipingCalculationScenario>(),
                FailureMechanism, assessmentSection);
        }

        private PipingScenarioConfigurationPerFailureMechanismSection GetScenarioConfigurationForSection(IFailureMechanismSectionResult sectionResult)
        {
            return FailureMechanism.ScenarioConfigurationsPerFailureMechanismSection
                                   .Single(sc => sc.Section.StartPoint.Equals(sectionResult.Section.StartPoint)
                                                 && sc.Section.EndPoint.Equals(sectionResult.Section.EndPoint));
        }
    }
}