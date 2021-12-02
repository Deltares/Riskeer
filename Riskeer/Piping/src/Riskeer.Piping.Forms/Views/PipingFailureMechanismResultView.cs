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
using System.Linq;
using Core.Common.Base;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Forms.Builders;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="PipingFailureMechanismSectionResult"/>.
    /// </summary>
    public class PipingFailureMechanismResultView : FailureMechanismResultView<PipingFailureMechanismSectionResult,
        PipingFailureMechanismSectionResultRow, PipingFailureMechanism>
    {
        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;
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
                                                   .OfType<SemiProbabilisticPipingCalculationScenario>()
                                                   .Select(c => c.InputParameters)));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, ICalculationBase>(
                UpdateView,
                c => c.Children);

            CalculationGroup observableGroup = failureMechanism.CalculationsGroup;
            calculationInputObserver.Observable = observableGroup;
            calculationGroupObserver.Observable = observableGroup;
        }

        protected override void Dispose(bool disposing)
        {
            calculationInputObserver.Dispose();
            calculationGroupObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override PipingFailureMechanismSectionResultRow CreateFailureMechanismSectionResultRow(PipingFailureMechanismSectionResult sectionResult)
        {
            return new PipingFailureMechanismSectionResultRow(
                sectionResult,
                FailureMechanism.Calculations.OfType<SemiProbabilisticPipingCalculationScenario>(),
                FailureMechanism,
                assessmentSection);
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
        }
    }
}