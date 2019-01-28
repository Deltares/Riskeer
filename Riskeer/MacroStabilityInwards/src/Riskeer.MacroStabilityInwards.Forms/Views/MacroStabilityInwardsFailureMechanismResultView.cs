// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Forms.Builders;
using Ringtoets.Common.Forms.Controls;
using Ringtoets.Common.Forms.Views;
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="MacroStabilityInwardsFailureMechanismSectionResult"/>.
    /// </summary>
    public class MacroStabilityInwardsFailureMechanismResultView : FailureMechanismResultView<MacroStabilityInwardsFailureMechanismSectionResult,
        MacroStabilityInwardsFailureMechanismSectionResultRow,
        MacroStabilityInwardsFailureMechanism,
        FailureMechanismAssemblyControl>
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

        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFailureMechanismResultView"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of <see cref="MacroStabilityInwardsFailureMechanismSectionResult"/> to
        /// show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the results belong to.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism results belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsFailureMechanismResultView(IObservableEnumerable<MacroStabilityInwardsFailureMechanismSectionResult> failureMechanismSectionResults,
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
            calculationInputObserver = new RecursiveObserver<CalculationGroup, ICalculationInput>(
                UpdateView,
                cg => cg.Children.Concat<object>(cg.Children
                                                   .OfType<MacroStabilityInwardsCalculationScenario>()
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

        protected override MacroStabilityInwardsFailureMechanismSectionResultRow CreateFailureMechanismSectionResultRow(
            MacroStabilityInwardsFailureMechanismSectionResult sectionResult)
        {
            return new MacroStabilityInwardsFailureMechanismSectionResultRow(
                sectionResult,
                FailureMechanism.Calculations.Cast<MacroStabilityInwardsCalculationScenario>(),
                FailureMechanism,
                assessmentSection,
                new MacroStabilityInwardsFailureMechanismSectionResultRow.ConstructionProperties
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

        protected override void AddDataGridColumns()
        {
            FailureMechanismSectionResultViewColumnBuilder.AddSectionNameColumn(
                DataGridViewControl,
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.Name));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssessmentResultColumn(
                DataGridViewControl,
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.SimpleAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentProbabilityOnlyResultColumn(
                DataGridViewControl,
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.DetailedAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentProbabilityColumn(
                DataGridViewControl,
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.DetailedAssessmentProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentProbabilityCalculationResultColumn(
                DataGridViewControl,
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.TailorMadeAssessmentResult));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentProbabilityColumn(
                DataGridViewControl,
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.TailorMadeAssessmentProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.SimpleAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.DetailedAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.TailorMadeAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddCombinedAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.CombinedAssemblyCategoryGroup));

            FailureMechanismSectionResultViewColumnBuilder.AddCombinedAssemblyProbabilityColumn(
                DataGridViewControl,
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.CombinedAssemblyProbability));

            FailureMechanismSectionResultViewColumnBuilder.AddUseManualAssemblyColumn(
                DataGridViewControl,
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.UseManualAssembly));

            FailureMechanismSectionResultViewColumnBuilder.AddManualAssemblyProbabilityColumn(
                DataGridViewControl,
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.ManualAssemblyProbability));
        }

        protected override void RefreshDataGrid()
        {
            base.RefreshDataGrid();
            DataGridViewControl.AutoResizeColumn(combinedAssemblyProbabilityIndex);
        }

        protected override void UpdateAssemblyResultControl()
        {
            FailureMechanismAssemblyResultControl.SetAssemblyResult(MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(FailureMechanism, assessmentSection, true));
        }
    }
}