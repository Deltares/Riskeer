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
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.Builders;
using Riskeer.Common.Forms.Providers;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="AdoptableFailureMechanismSectionResult"/> in structures.
    /// </summary>
    /// <typeparam name="TFailureMechanism">The type of failure mechanism.</typeparam>
    /// <typeparam name="TStructuresInput">The type of input.</typeparam>
    public class StructuresFailureMechanismResultView<TFailureMechanism, TStructuresInput> : FailureMechanismResultView<AdoptableFailureMechanismSectionResult, AdoptableFailureMechanismSectionResultRow, TFailureMechanism>
        where TFailureMechanism : IHasSectionResults<FailureMechanismSectionResultOld, AdoptableFailureMechanismSectionResult>, ICalculatableFailureMechanism
        where TStructuresInput : IStructuresCalculationInput<StructureBase>, new()
    {
        private const int initialFailureMechanismResultIndex = 2;
        private const int initialFailureMechanismResultSectionProbabilityIndex = 3;
        private const int furtherAnalysisNeededIndex = 4;
        private const int refinedSectionProbabilityIndex = 5;
        private const int sectionProbabilityIndex = 6;
        private const int assemblyGroupIndex = 7;

        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputsObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;

        private readonly IAssessmentSection assessmentSection;
        private readonly Func<TFailureMechanism, double> getNFunc;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresFailureMechanismResultView{TFailureMechanism,TStructuresInput}"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of <see cref="AdoptableFailureMechanismSectionResult"/> to
        /// show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the results belong to.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism results belong to.</param>
        /// <param name="getNFunc">The <see cref="Func{T1,TResult}"/> to get the N.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StructuresFailureMechanismResultView(IObservableEnumerable<AdoptableFailureMechanismSectionResult> failureMechanismSectionResults,
                                                    TFailureMechanism failureMechanism,
                                                    IAssessmentSection assessmentSection,
                                                    Func<TFailureMechanism, double> getNFunc)
            : base(failureMechanismSectionResults, failureMechanism)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (getNFunc == null)
            {
                throw new ArgumentNullException(nameof(getNFunc));
            }

            this.assessmentSection = assessmentSection;
            this.getNFunc = getNFunc;

            // The concat is needed to observe the input of calculations in child groups.
            calculationInputsObserver = new RecursiveObserver<CalculationGroup, ICalculationInput>(
                UpdateInternalViewData,
                cg => cg.Children.Concat(cg.Children
                                           .OfType<StructuresCalculationScenario<TStructuresInput>>()
                                           .Select(scenario => scenario.InputParameters)
                                           .Cast<object>()))
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
            StructuresCalculationScenario<TStructuresInput>[] calculationScenarios = FailureMechanism.Calculations
                                                                                                     .OfType<StructuresCalculationScenario<TStructuresInput>>()
                                                                                                     .ToArray();

            return new AdoptableFailureMechanismSectionResultRow(
                sectionResult,
                () => sectionResult.GetInitialFailureMechanismResultProbability(calculationScenarios),
                CreateErrorProvider(sectionResult, calculationScenarios),
                assessmentSection,
                new AdoptableFailureMechanismSectionResultRow.ConstructionProperties
                {
                    InitialFailureMechanismResultIndex = initialFailureMechanismResultIndex,
                    InitialFailureMechanismResultSectionProbabilityIndex = initialFailureMechanismResultSectionProbabilityIndex,
                    FurtherAnalysisNeededIndex = furtherAnalysisNeededIndex,
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

        protected override double GetN()
        {
            return getNFunc(FailureMechanism);
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

            FailureMechanismSectionResultViewColumnBuilder.AddFurtherAnalysisNeededColumn(
                DataGridViewControl,
                nameof(AdoptableFailureMechanismSectionResultRow.FurtherAnalysisNeeded));

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

        private static InitialFailureMechanismResultErrorProvider<StructuresCalculationScenario<TStructuresInput>> CreateErrorProvider(
            FailureMechanismSectionResult sectionResult, IEnumerable<StructuresCalculationScenario<TStructuresInput>> calculationScenarios)
        {
            return new InitialFailureMechanismResultErrorProvider<StructuresCalculationScenario<TStructuresInput>>(
                sectionResult, calculationScenarios,
                (scenario, lineSegments) => scenario.IsStructureIntersectionWithReferenceLineInSection(lineSegments));
        }
    }
}