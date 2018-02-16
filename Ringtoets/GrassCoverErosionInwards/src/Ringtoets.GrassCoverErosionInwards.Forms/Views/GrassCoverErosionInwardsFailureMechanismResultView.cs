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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Util;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.GrassCoverErosionInwards.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>.
    /// </summary>
    public class GrassCoverErosionInwardsFailureMechanismResultView
        : FailureMechanismResultView<GrassCoverErosionInwardsFailureMechanismSectionResult, GrassCoverErosionInwardsFailureMechanism>
    {
        private const int detailedAssessmentIndex = 2;
        private readonly IAssessmentSection assessmentSection;
        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationOutput> calculationOutputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsFailureMechanismResultView"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section the failure mechanism section results belongs to.</param>
        /// <inheritdoc />
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

            DataGridViewControl.CellFormatting += ShowAssessmentLayerErrors;
            DataGridViewControl.CellFormatting += DisableIrrelevantFieldsFormatting;

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

            UpdateDataGridViewDataSource();
        }

        protected override void Dispose(bool disposing)
        {
            DataGridViewControl.CellFormatting -= ShowAssessmentLayerErrors;
            DataGridViewControl.CellFormatting -= DisableIrrelevantFieldsFormatting;

            calculationInputObserver.Dispose();
            calculationOutputObserver.Dispose();
            calculationGroupObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override object CreateFailureMechanismSectionResultRow(GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult)
        {
            return new GrassCoverErosionInwardsFailureMechanismSectionResultRow(sectionResult,
                                                                                FailureMechanism,
                                                                                assessmentSection);
        }

        protected override void AddDataGridColumns()
        {
            base.AddDataGridColumns();

            EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>[] simpleAssessmentDataSource =
                Enum.GetValues(typeof(SimpleAssessmentResultValidityOnlyType))
                    .OfType<SimpleAssessmentResultValidityOnlyType>()
                    .Select(sa => new EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>(sa))
                    .ToArray();

            DataGridViewControl.AddComboBoxColumn(
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.SimpleAssessmentResult),
                RingtoetsCommonFormsResources.FailureMechanismResultView_SimpleAssessmentResult_ColumnHeader,
                simpleAssessmentDataSource,
                nameof(EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>.Value),
                nameof(EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>.DisplayName));

            DataGridViewControl.AddTextBoxColumn(
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.DetailedAssessmentProbability),
                RingtoetsCommonFormsResources.FailureMechanismResultView_DetailedAssessment_ColumnHeader,
                true);
            DataGridViewControl.AddTextBoxColumn(
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.AssessmentLayerThree),
                RingtoetsCommonFormsResources.FailureMechanismResultView_TailorMadeAssessment_ColumnHeader);
        }

        private void DisableIrrelevantFieldsFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex > SimpleAssessmentColumnIndex)
            {
                var simpleAssessmentResult = (SimpleAssessmentResultValidityOnlyType) DataGridViewControl.GetCell(eventArgs.RowIndex,
                                                                                                                  SimpleAssessmentColumnIndex).Value;
                if (FailureMechanismResultViewHelper.HasPassedSimpleAssessment(simpleAssessmentResult))
                {
                    DataGridViewControl.DisableCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
                else
                {
                    DataGridViewControl.RestoreCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
            }
        }

        private void ShowAssessmentLayerErrors(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != detailedAssessmentIndex)
            {
                return;
            }

            var resultRow = (GrassCoverErosionInwardsFailureMechanismSectionResultRow) GetDataAtRow(e.RowIndex);
            DataGridViewCell currentDataGridViewCell = DataGridViewControl.GetCell(e.RowIndex, e.ColumnIndex);
            GrassCoverErosionInwardsCalculation normativeCalculation = resultRow.GetSectionResultCalculation();

            FailureMechanismSectionResultRowHelper.SetDetailedAssessmentError(currentDataGridViewCell,
                                                                              resultRow.SimpleAssessmentResult,
                                                                              resultRow.DetailedAssessmentProbability,
                                                                              normativeCalculation);
        }
    }
}