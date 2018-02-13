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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Util;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Views;
using Ringtoets.GrassCoverErosionInwards.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>.
    /// </summary>
    public class GrassCoverErosionInwardsFailureMechanismResultView
        : FailureMechanismResultView<GrassCoverErosionInwardsFailureMechanism, GrassCoverErosionInwardsFailureMechanismSectionResult>
    {
        private const int assessmentLayerTwoAIndex = 2;
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
            IAssessmentSection assessmentSection,
            GrassCoverErosionInwardsFailureMechanism failureMechanism,
            IObservableEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> failureMechanismSectionResults)
            : base(failureMechanism, failureMechanismSectionResults)
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

            EnumDisplayWrapper<AssessmentLayerOneState>[] layerOneDataSource =
                Enum.GetValues(typeof(AssessmentLayerOneState))
                    .OfType<AssessmentLayerOneState>()
                    .Select(sa => new EnumDisplayWrapper<AssessmentLayerOneState>(sa))
                    .ToArray();

            DataGridViewControl.AddComboBoxColumn(
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.AssessmentLayerOne),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_one,
                layerOneDataSource,
                nameof(EnumDisplayWrapper<SimpleAssessmentResultType>.Value),
                nameof(EnumDisplayWrapper<SimpleAssessmentResultType>.DisplayName));

            DataGridViewControl.AddTextBoxColumn(
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.AssessmentLayerTwoA),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a,
                true);
            DataGridViewControl.AddTextBoxColumn(
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.AssessmentLayerThree),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three);
        }

        private void DisableIrrelevantFieldsFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex > AssessmentLayerOneColumnIndex)
            {
                if (HasPassedSimpleAssessment(eventArgs.RowIndex))
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
            if (e.ColumnIndex != assessmentLayerTwoAIndex)
            {
                return;
            }

            var resultRow = (GrassCoverErosionInwardsFailureMechanismSectionResultRow) GetDataAtRow(e.RowIndex);
            DataGridViewCell currentDataGridViewCell = DataGridViewControl.GetCell(e.RowIndex, e.ColumnIndex);
            GrassCoverErosionInwardsCalculation normativeCalculation = resultRow.GetSectionResultCalculation();

            FailureMechanismSectionResultRowHelper.SetAssessmentLayerTwoAError(currentDataGridViewCell,
                                                                               resultRow.AssessmentLayerOne,
                                                                               resultRow.AssessmentLayerTwoA,
                                                                               normativeCalculation);
        }
    }
}