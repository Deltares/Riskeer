// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Builders;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>.
    /// </summary>
    public class GrassCoverErosionInwardsFailureMechanismResultView
        : FailureMechanismResultView<GrassCoverErosionInwardsFailureMechanismSectionResult, GrassCoverErosionInwardsFailureMechanismSectionResultRow, GrassCoverErosionInwardsFailureMechanism>
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
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsFailureMechanismResultView"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section the failure mechanism section results belongs to.</param>
        public GrassCoverErosionInwardsFailureMechanismResultView(
            IObservableEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> failureMechanismSectionResults,
            GrassCoverErosionInwardsFailureMechanism failureMechanism,
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
                                                   .OfType<GrassCoverErosionInwardsCalculation>()
                                                   .Select(c => c.InputParameters)));
            calculationOutputObserver = new RecursiveObserver<CalculationGroup, ICalculationOutput>(
                UpdateDataGridViewDataSource,
                cg => cg.Children.Concat<object>(cg.Children
                                                   .OfType<GrassCoverErosionInwardsCalculation>()
                                                   .Select(c => c.Output)));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, ICalculationBase>(
                UpdateDataGridViewDataSource,
                c => c.Children);

            CalculationGroup observableGroup = failureMechanism.CalculationsGroup;
            calculationInputObserver.Observable = observableGroup;
            calculationOutputObserver.Observable = observableGroup;
            calculationGroupObserver.Observable = observableGroup;

            FormattingRules = CreateFormattingRules();
        }

        protected override IEnumerable<DataGridViewColumnFormattingRule<GrassCoverErosionInwardsFailureMechanismSectionResultRow>> FormattingRules { get; }

        protected override void Dispose(bool disposing)
        {
            DataGridViewControl.CellFormatting -= ShowAssessmentLayerErrors;

            calculationInputObserver.Dispose();
            calculationOutputObserver.Dispose();
            calculationGroupObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override GrassCoverErosionInwardsFailureMechanismSectionResultRow CreateFailureMechanismSectionResultRow(GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult)
        {
            return new GrassCoverErosionInwardsFailureMechanismSectionResultRow(sectionResult,
                                                                                FailureMechanism,
                                                                                assessmentSection);
        }

        protected override void AddDataGridColumns()
        {
            FailureMechanismSectionResultColumnBuilder.AddSectionNameColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.Name));

            FailureMechanismSectionResultColumnBuilder.AddSimpleAssessmentResultValidityOnlyColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.SimpleAssessmentResult));

            FailureMechanismSectionResultColumnBuilder.AddDetailedAssessmentResultColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.DetailedAssessmentResult));

            FailureMechanismSectionResultColumnBuilder.AddDetailedAssessmentProbabilityColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.DetailedAssessmentProbability));

            FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentProbabilityCalculationResultColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.TailorMadeAssessmentResult));

            FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentProbabilityColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.TailorMadeAssessmentProbability));

            FailureMechanismSectionResultColumnBuilder.AddSimpleAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.SimpleAssemblyCategoryGroup));

            FailureMechanismSectionResultColumnBuilder.AddDetailedAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.DetailedAssemblyCategoryGroup));

            FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.TailorMadeAssemblyCategoryGroup));

            FailureMechanismSectionResultColumnBuilder.AddCombinedAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.CombinedAssemblyCategoryGroup));

            FailureMechanismSectionResultColumnBuilder.AddCombinedAssemblyProbabilityColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.CombinedAssemblyProbability));

            FailureMechanismSectionResultColumnBuilder.AddUseManualAssemblyCategoryGroupColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.UseManualAssemblyProbability));

            FailureMechanismSectionResultColumnBuilder.AddManualAssemblyProbabilityColumn(
                DataGridViewControl,
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.ManualAssemblyProbability));
        }

        protected override void BindEvents()
        {
            base.BindEvents();

            DataGridViewControl.CellFormatting += ShowAssessmentLayerErrors;
        }

        private void ShowAssessmentLayerErrors(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != detailedAssessmentProbabilityIndex)
            {
                return;
            }

            GrassCoverErosionInwardsFailureMechanismSectionResultRow resultRow = GetDataAtRow(e.RowIndex);
            DataGridViewCell currentDataGridViewCell = DataGridViewControl.GetCell(e.RowIndex, e.ColumnIndex);
            GrassCoverErosionInwardsCalculation normativeCalculation = resultRow.GetSectionResultCalculation();

            if (resultRow.UseManualAssemblyProbability || resultRow.DetailedAssessmentResult == DetailedAssessmentResultType.NotAssessed)
            {
                currentDataGridViewCell.ErrorText = string.Empty;
                return;
            }

            FailureMechanismSectionResultRowHelper.SetDetailedAssessmentError(currentDataGridViewCell,
                                                                              resultRow.SimpleAssessmentResult,
                                                                              resultRow.DetailedAssessmentProbability,
                                                                              normativeCalculation);
        }

        private static IEnumerable<DataGridViewColumnFormattingRule<GrassCoverErosionInwardsFailureMechanismSectionResultRow>> CreateFormattingRules()
        {
            return new[]
            {
                new DataGridViewColumnFormattingRule<GrassCoverErosionInwardsFailureMechanismSectionResultRow>(
                    new[]
                    {
                        detailedAssessmentResultIndex,
                        detailedAssessmentProbabilityIndex,
                        tailorMadeAssessmentResultIndex,
                        tailorMadeAssessmentProbabilityIndex
                    },
                    row => FailureMechanismResultViewHelper.SimpleAssessmentIsSufficient(row.SimpleAssessmentResult)),
                new DataGridViewColumnFormattingRule<GrassCoverErosionInwardsFailureMechanismSectionResultRow>(
                    new[]
                    {
                        detailedAssessmentProbabilityIndex
                    },
                    row => row.DetailedAssessmentResult != DetailedAssessmentResultType.Probability),
                new DataGridViewColumnFormattingRule<GrassCoverErosionInwardsFailureMechanismSectionResultRow>(
                    new[]
                    {
                        tailorMadeAssessmentProbabilityIndex
                    },
                    row => row.TailorMadeAssessmentResult != TailorMadeAssessmentProbabilityCalculationResultType.Probability),
                new DataGridViewColumnFormattingRule<GrassCoverErosionInwardsFailureMechanismSectionResultRow>(
                    new[]
                    {
                        manualAssemblyProbabilityIndex
                    },
                    row => !row.UseManualAssemblyProbability),
                new DataGridViewColumnFormattingRule<GrassCoverErosionInwardsFailureMechanismSectionResultRow>(
                    new[]
                    {
                        simpleAssessmentResultIndex,
                        detailedAssessmentResultIndex,
                        detailedAssessmentProbabilityIndex,
                        tailorMadeAssessmentResultIndex,
                        tailorMadeAssessmentProbabilityIndex,
                        simpleAssemblyCategoryGroupIndex,
                        detailedAssemblyCategoryGroupIndex,
                        tailorMadeAssemblyCategoryGroupIndex,
                        combinedAssemblyCategoryGroupIndex,
                        combinedAssemblyProbabilityIndex
                    },
                    row => row.UseManualAssemblyProbability)
            };
        }
    }
}