﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.Builders;
using Ringtoets.Common.Forms.Views;
using Ringtoets.StabilityPointStructures.Data;

namespace Ringtoets.StabilityPointStructures.Forms.Views
{
    /// <summary>
    /// The view for a collection of <see cref="StabilityPointStructuresFailureMechanismSectionResult"/>
    /// for stability point structures.
    /// </summary>
    public class StabilityPointStructuresFailureMechanismResultView : FailureMechanismResultView<StabilityPointStructuresFailureMechanismSectionResult,
        StabilityPointStructuresFailureMechanismSectionResultRow, StabilityPointStructuresFailureMechanism>
    {
        private const int simpleAssessmentResultIndex = 1;
        private const int detailedAssessmentResultIndex = 2;
        private const int detailedAssessmentProbabilityIndex = 3;
        private const int tailorMadeAssessmentResultIndex = 4;
        private const int tailorMadeAssessmentProbabilityIndex = 5;
        private const int simpleAssemblyCategoryGroupIndex = 6;
        private const int detailedAssemblyCategoryGroupIndex = 7;
        private const int tailorMadeAssemblyCategoryGroupIndex = 8;
        private const int combinedAssemblyCategoryGroupIndex = 9;
        private const int combinedAssemblyProbabilityIndex = 10;
        private const int manualAssemblyProbabilityIndex = 12;

        private readonly IAssessmentSection assessmentSection;
        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationOutput> calculationOutputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;

        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresFailureMechanismResultView"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section the failure mechanism results belong to.</param>
        public StabilityPointStructuresFailureMechanismResultView(
            IObservableEnumerable<StabilityPointStructuresFailureMechanismSectionResult> failureMechanismSectionResults,
            StabilityPointStructuresFailureMechanism failureMechanism,
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
                UpdateDataGridViewDataSource,
                cg => cg.Children.Concat<object>(cg.Children
                                                   .OfType<StructuresCalculation<StabilityPointStructuresInput>>()
                                                   .Select(c => c.InputParameters)));
            calculationOutputObserver = new RecursiveObserver<CalculationGroup, ICalculationOutput>(
                UpdateDataGridViewDataSource,
                cg => cg.Children.Concat<object>(cg.Children
                                                   .OfType<StructuresCalculation<StabilityPointStructuresInput>>()
                                                   .Select(c => c.Output)));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, ICalculationBase>(
                UpdateDataGridViewDataSource,
                c => c.Children);

            CalculationGroup observableGroup = failureMechanism.CalculationsGroup;

            calculationInputObserver.Observable = observableGroup;
            calculationOutputObserver.Observable = observableGroup;
            calculationGroupObserver.Observable = observableGroup;
        }

        protected override StabilityPointStructuresFailureMechanismSectionResultRow CreateFailureMechanismSectionResultRow(StabilityPointStructuresFailureMechanismSectionResult sectionResult)
        {
            return new StabilityPointStructuresFailureMechanismSectionResultRow(
                sectionResult,
                FailureMechanism,
                assessmentSection,
                new StabilityPointStructuresFailureMechanismSectionResultRow.ConstructionProperties
                {
                    SimpleAssessmentResultIndex = simpleAssessmentResultIndex,
                    DetailedAssessmentResultIndex = detailedAssessmentResultIndex,
                    DetailedAssessmentProbabilityIndex = detailedAssessmentProbabilityIndex,
                    TailorMadeAssessmentResultIndex = tailorMadeAssessmentResultIndex,
                    TailorMadeAssessmentProbabilityIndex = tailorMadeAssessmentProbabilityIndex,
                    SimpleAssemblyCategoryGroupIndex = simpleAssemblyCategoryGroupIndex,
                    DetailedAssemblyCategoryGroupIndex = detailedAssemblyCategoryGroupIndex,
                    TailorMadeAssemblyCategoryGroupIndex = tailorMadeAssemblyCategoryGroupIndex,
                    CombinedAssemblyCategoryGroupIndex = combinedAssemblyCategoryGroupIndex,
                    CombinedAssemblyProbabilityIndex = combinedAssemblyProbabilityIndex,
                    ManualAssemblyProbabilityIndex = manualAssemblyProbabilityIndex
                });
        }

        protected override void Dispose(bool disposing)
        {
            calculationInputObserver.Dispose();
            calculationOutputObserver.Dispose();
            calculationGroupObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override void AddDataGridColumns()
        {
            FailureMechanismSectionResultViewColumnBuilder.AddSectionNameColumn(
                DataGridViewControl,
                nameof(StabilityPointStructuresFailureMechanismSectionResultRow.Name));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssessmentValidityOnlyResultColumn(
                DataGridViewControl,
                nameof(StabilityPointStructuresFailureMechanismSectionResultRow.SimpleAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentProbabilityOnlyResultColumn(
                DataGridViewControl,
                nameof(StabilityPointStructuresFailureMechanismSectionResultRow.DetailedAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentProbabilityColumn(
                DataGridViewControl,
                nameof(StabilityPointStructuresFailureMechanismSectionResultRow.DetailedAssessmentProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentProbabilityCalculationResultColumn(
                DataGridViewControl,
                nameof(StabilityPointStructuresFailureMechanismSectionResultRow.TailorMadeAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentProbabilityColumn(
                DataGridViewControl,
                nameof(StabilityPointStructuresFailureMechanismSectionResultRow.TailorMadeAssessmentProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(StabilityPointStructuresFailureMechanismSectionResultRow.SimpleAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(StabilityPointStructuresFailureMechanismSectionResultRow.DetailedAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(StabilityPointStructuresFailureMechanismSectionResultRow.TailorMadeAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddCombinedAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(StabilityPointStructuresFailureMechanismSectionResultRow.CombinedAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddCombinedAssemblyProbabilityColumn(
                DataGridViewControl,
                nameof(StabilityPointStructuresFailureMechanismSectionResultRow.CombinedAssemblyProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddUseManualAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(StabilityPointStructuresFailureMechanismSectionResultRow.UseManualAssemblyProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddManualAssemblyProbabilityColumn(
                DataGridViewControl,
                nameof(StabilityPointStructuresFailureMechanismSectionResultRow.ManualAssemblyProbability));
        }
    }
}