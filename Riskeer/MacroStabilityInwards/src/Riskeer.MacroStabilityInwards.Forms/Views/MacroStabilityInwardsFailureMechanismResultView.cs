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
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="AdoptableFailureMechanismSectionResult"/>
    /// in the <see cref="MacroStabilityInwardsFailureMechanism"/>.
    /// </summary>
    public class MacroStabilityInwardsFailureMechanismResultView : FailureMechanismResultView<AdoptableFailureMechanismSectionResult,
        AdoptableFailureMechanismSectionResultRow, MacroStabilityInwardsFailureMechanism>
    {
        private const int initialFailureMechanismResultTypeIndex = 2;
        private const int initialFailureMechanismResultSectionProbabilityIndex = 3;
        private const int furtherAnalysisTypeIndex = 4;
        private const int refinedSectionProbabilityIndex = 5;
        private const int sectionProbabilityIndex = 6;
        private const int assemblyGroupIndex = 7;

        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputsObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFailureMechanismResultView"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of
        /// <see cref="AdoptableFailureMechanismSectionResult"/> to show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the results belong to.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism results belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsFailureMechanismResultView(IObservableEnumerable<AdoptableFailureMechanismSectionResult> failureMechanismSectionResults,
                                                               MacroStabilityInwardsFailureMechanism failureMechanism,
                                                               IAssessmentSection assessmentSection)
            : base(failureMechanismSectionResults, failureMechanism, assessmentSection, MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism)
        {
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

        protected override AdoptableFailureMechanismSectionResultRow CreateFailureMechanismSectionResultRow(AdoptableFailureMechanismSectionResult sectionResult)
        {
            MacroStabilityInwardsCalculationScenario[] calculationScenarios = FailureMechanism.Calculations
                                                                                              .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                              .ToArray();
            return new AdoptableFailureMechanismSectionResultRow(
                sectionResult,
                CreateCalculateStrategy(sectionResult, calculationScenarios),
                CreateErrorProvider(sectionResult, calculationScenarios),
                () => MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleSection(sectionResult, FailureMechanism, AssessmentSection),
                new AdoptableFailureMechanismSectionResultRow.ConstructionProperties
                {
                    InitialFailureMechanismResultTypeIndex = initialFailureMechanismResultTypeIndex,
                    InitialFailureMechanismResultSectionProbabilityIndex = initialFailureMechanismResultSectionProbabilityIndex,
                    FurtherAnalysisTypeIndex = furtherAnalysisTypeIndex,
                    RefinedSectionProbabilityIndex = refinedSectionProbabilityIndex,
                    SectionProbabilityIndex = sectionProbabilityIndex,
                    AssemblyGroupIndex = assemblyGroupIndex
                });
        }

        protected override void Dispose(bool disposing)
        {
            calculationInputsObserver.Dispose();
            calculationGroupObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override void AddDataGridColumns()
        {
            FailureMechanismSectionResultViewColumnBuilder.AddSectionNameColumn(
                DataGridViewControl,
                nameof(AdoptableFailureMechanismSectionResultRow.Name));

            FailureMechanismSectionResultViewColumnBuilder.AddIsRelevantColumn(
                DataGridViewControl,
                nameof(AdoptableFailureMechanismSectionResultRow.IsRelevant));

            FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultTypeColumn<AdoptableInitialFailureMechanismResultType>(
                DataGridViewControl,
                nameof(AdoptableFailureMechanismSectionResultRow.InitialFailureMechanismResultType));

            FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultSectionProbabilityColumn(
                DataGridViewControl,
                nameof(AdoptableFailureMechanismSectionResultRow.InitialFailureMechanismResultSectionProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddFurtherAnalysisTypeColumn(
                DataGridViewControl,
                nameof(AdoptableFailureMechanismSectionResultRow.FurtherAnalysisType));

            FailureMechanismSectionResultViewColumnBuilder.AddRefinedSectionProbabilityColumn(
                DataGridViewControl,
                nameof(AdoptableFailureMechanismSectionResultRow.RefinedSectionProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddAssemblySectionProbabilityColumn(
                DataGridViewControl,
                nameof(AdoptableFailureMechanismSectionResultRow.SectionProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddAssemblyGroupColumn(
                DataGridViewControl,
                nameof(AdoptableFailureMechanismSectionResultRow.AssemblyGroup));
        }

        private MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy CreateCalculateStrategy(
            AdoptableFailureMechanismSectionResult sectionResult,
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
    }
}