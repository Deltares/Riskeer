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
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Forms.Builders;
using Riskeer.Common.Forms.Providers;
using Riskeer.Common.Forms.Views;
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResult"/>
    /// in the <see cref="MacroStabilityInwardsFailureMechanism"/>.
    /// </summary>
    public class MacroStabilityInwardsFailureMechanismResultView : FailureMechanismResultView<AdoptableWithProfileProbabilityFailureMechanismSectionResult,
        AdoptableWithProfileProbabilityFailureMechanismSectionResultRow, MacroStabilityInwardsFailureMechanism>
    {
        private const int initialFailureMechanismResultTypeIndex = 2;
        private const int initialFailureMechanismResultProfileProbabilityIndex = 3;
        private const int initialFailureMechanismResultSectionProbabilityIndex = 4;
        private const int furtherAnalysisTypeIndex = 5;
        private const int probabilityRefinementTypeIndex = 6;
        private const int refinedProfileProbabilityIndex = 7;
        private const int refinedSectionProbabilityIndex = 8;
        private const int profileProbabilityIndex = 9;
        private const int sectionProbabilityIndex = 10;
        private const int sectionNIndex = 11;
        private const int assemblyGroupIndex = 12;

        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputsObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFailureMechanismResultView"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of
        /// <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResult"/> to show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the results belong to.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism results belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsFailureMechanismResultView(IObservableEnumerable<AdoptableWithProfileProbabilityFailureMechanismSectionResult> failureMechanismSectionResults,
                                                               MacroStabilityInwardsFailureMechanism failureMechanism,
                                                               IAssessmentSection assessmentSection)
            : base(failureMechanismSectionResults, failureMechanism)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.assessmentSection = assessmentSection;

            // The concat is needed to observe the input of calculations in child groups.
            calculationInputsObserver = new RecursiveObserver<CalculationGroup, ICalculationInput>(
                UpdateInternalViewData,
                cg => cg.Children.Concat<object>(cg.Children
                                                   .OfType<MacroStabilityInwardsCalculationScenario>()
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
        }

        protected override AdoptableWithProfileProbabilityFailureMechanismSectionResultRow CreateFailureMechanismSectionResultRow(AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult)
        {
            MacroStabilityInwardsCalculationScenario[] calculationScenarios = FailureMechanism.Calculations
                                                                                              .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                              .ToArray();
            return new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(
                sectionResult,
                CreateCalculateStrategy(sectionResult, calculationScenarios),
                CreateErrorProvider(sectionResult, calculationScenarios),
                CreateLengthEffectProvider(sectionResult),
                assessmentSection,
                new AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.ConstructionProperties
                {
                    InitialFailureMechanismResultTypeIndex = initialFailureMechanismResultTypeIndex,
                    InitialFailureMechanismResultProfileProbabilityIndex = initialFailureMechanismResultProfileProbabilityIndex,
                    InitialFailureMechanismResultSectionProbabilityIndex = initialFailureMechanismResultSectionProbabilityIndex,
                    FurtherAnalysisNeededIndex = furtherAnalysisTypeIndex,
                    ProbabilityRefinementTypeIndex = probabilityRefinementTypeIndex,
                    RefinedProfileProbabilityIndex = refinedProfileProbabilityIndex,
                    RefinedSectionProbabilityIndex = refinedSectionProbabilityIndex,
                    ProfileProbabilityIndex = profileProbabilityIndex,
                    SectionProbabilityIndex = sectionProbabilityIndex,
                    SectionNIndex = sectionNIndex,
                    AssemblyGroupIndex = assemblyGroupIndex
                });
        }

        protected override void Dispose(bool disposing)
        {
            calculationInputsObserver.Dispose();
            calculationGroupObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override double GetN()
        {
            return FailureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.GetN(assessmentSection.ReferenceLine.Length);
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

            FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultProfileProbabilityColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.InitialFailureMechanismResultProfileProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultSectionProbabilityColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.InitialFailureMechanismResultSectionProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddFurtherAnalysisTypeColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.FurtherAnalysisType));

            FailureMechanismSectionResultViewColumnBuilder.AddProbabilityRefinementTypeColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.ProbabilityRefinementType));

            FailureMechanismSectionResultViewColumnBuilder.AddRefinedProfileProbabilityColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.RefinedProfileProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddRefinedSectionProbabilityColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.RefinedSectionProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddAssemblyProfileProbabilityColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.ProfileProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddAssemblySectionProbabilityColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.SectionProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddAssemblySectionNColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.SectionN));

            FailureMechanismSectionResultViewColumnBuilder.AddAssemblyGroupColumn(
                DataGridViewControl,
                nameof(AdoptableWithProfileProbabilityFailureMechanismSectionResultRow.AssemblyGroup));
        }

        private MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy CreateCalculateStrategy(
            AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult,
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculationScenarios)
        {
            return new MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, calculationScenarios, FailureMechanism);
        }

        private static FailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider<MacroStabilityInwardsCalculationScenario> CreateErrorProvider(
            FailureMechanismSectionResult sectionResult, IEnumerable<MacroStabilityInwardsCalculationScenario> calculationScenarios)
        {
            return new FailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider<MacroStabilityInwardsCalculationScenario>(
                sectionResult, calculationScenarios, (scenario, lineSegments) => scenario.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));
        }

        private ILengthEffectProvider CreateLengthEffectProvider(FailureMechanismSectionResult sectionResult)
        {
            return new LengthEffectProvider(
                () => FailureMechanism.GeneralInput.ApplyLengthEffectInSection,
                () => FailureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.GetN(sectionResult.Section.Length));
        }
    }
}