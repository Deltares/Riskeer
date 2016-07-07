﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Linq;
using System.Windows.Forms;

using Core.Common.Base;
using Core.Common.Utils.Reflection;

using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Views;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

using CoreCommonResources = Core.Common.Base.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>.
    /// </summary>
    public class GrassCoverErosionInwardsFailureMechanismResultView : FailureMechanismResultView<GrassCoverErosionInwardsFailureMechanismSectionResult>
    {
        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationOutput> calculationOutputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;
        private const int assessmentLayerTwoAIndex = 2;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsFailureMechanismResultView"/>.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanismResultView()
        {
            DataGridViewControl.AddCellFormattingHandler(ShowAssementLayerTwoAErrors);
            DataGridViewControl.AddCellFormattingHandler(DisableIrrelevantFieldsFormatting);

            // The concat is needed to observe the input of calculations in child groups.
            calculationInputObserver = new RecursiveObserver<CalculationGroup, ICalculationInput>(
                UpdateDataGridViewDataSource,
                cg => cg.Children.Concat<object>(
                    cg.Children
                      .OfType<ICalculation>()
                      .Select(c => c.GetObservableInput())
                          )
                );
            calculationOutputObserver = new RecursiveObserver<CalculationGroup, ICalculationOutput>(
                UpdateDataGridViewDataSource,
                cg => cg.Children.Concat<object>(
                    cg.Children
                      .OfType<ICalculation>()
                      .Select(c => c.GetObservableOutput())
                          )
                );
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, ICalculationBase>(
                UpdateDataGridViewDataSource,
                c => c.Children
                );

            AddDataGridColumns();
        }

        public override IFailureMechanism FailureMechanism
        {
            set
            {
                base.FailureMechanism = value;

                var calculatableFailureMechanism = value as ICalculatableFailureMechanism;
                CalculationGroup observableGroup = calculatableFailureMechanism != null ? calculatableFailureMechanism.CalculationsGroup : null;

                calculationInputObserver.Observable = observableGroup;
                calculationOutputObserver.Observable = observableGroup;
                calculationGroupObserver.Observable = observableGroup;
            }
        }

        protected override void Dispose(bool disposing)
        {
            DataGridViewControl.RemoveCellFormattingHandler(ShowAssementLayerTwoAErrors);
            DataGridViewControl.RemoveCellFormattingHandler(DisableIrrelevantFieldsFormatting);

            calculationInputObserver.Dispose();
            calculationOutputObserver.Dispose();
            calculationGroupObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override object CreateFailureMechanismSectionResultRow(GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult)
        {
            return new GrassCoverErosionInwardsFailureMechanismSectionResultRow(sectionResult);
        }

        private void AddDataGridColumns()
        {
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<GrassCoverErosionInwardsFailureMechanismSectionResultRow>(sr => sr.Name),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Section_name,
                true);
            DataGridViewControl.AddCheckBoxColumn(
                TypeUtils.GetMemberName<GrassCoverErosionInwardsFailureMechanismSectionResultRow>(sr => sr.AssessmentLayerOne),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_one);
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<GrassCoverErosionInwardsFailureMechanismSectionResultRow>(sr => sr.AssessmentLayerTwoA),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a,
                true);
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<GrassCoverErosionInwardsFailureMechanismSectionResultRow>(sr => sr.AssessmentLayerThree),
                RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three);
        }

        private void DisableIrrelevantFieldsFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex > 1)
            {
                if (HasPassedLevelOne(eventArgs.RowIndex))
                {
                    DataGridViewControl.DisableCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
                else
                {
                    DataGridViewControl.RestoreCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
            }
        }

        private void ShowAssementLayerTwoAErrors(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex <= 0)
            {
                return;
            }

            DataGridViewCell currentDataGridViewCell = DataGridViewControl.GetCell(e.RowIndex, e.ColumnIndex);

            var resultRow = (GrassCoverErosionInwardsFailureMechanismSectionResultRow)GetDataAtRow(e.RowIndex);
            if (resultRow != null && e.ColumnIndex == assessmentLayerTwoAIndex)
            {
                GrassCoverErosionInwardsCalculation normativeCalculation = resultRow.GetSectionResultCalculation();

                if (resultRow.AssessmentLayerOne || normativeCalculation == null)
                {
                    currentDataGridViewCell.ErrorText = string.Empty;
                    return;
                }

                CalculationScenarioStatus calculationScenarioStatus = GetCalculationStatus(normativeCalculation);
                if (calculationScenarioStatus == CalculationScenarioStatus.NotCalculated)
                {
                    currentDataGridViewCell.ErrorText = Resources.GrassCoverErosionInwardsFailureMechanismResultView_Calculation_not_calculated;
                    return;
                }
                if (calculationScenarioStatus == CalculationScenarioStatus.Failed)
                {
                    currentDataGridViewCell.ErrorText = Resources.GrassCoverErosionInwardsFailureMechanismResultView_Calculation_not_successful;
                    return;
                }
                currentDataGridViewCell.ErrorText = string.Empty;
            }
        }

        private static CalculationScenarioStatus GetCalculationStatus(GrassCoverErosionInwardsCalculation calculation)
        {
            if (calculation.HasOutput)
            {
                if (double.IsNaN(calculation.Output.Probability))
                {
                    return CalculationScenarioStatus.Failed;
                }
                return CalculationScenarioStatus.Done;
            }
            return CalculationScenarioStatus.NotCalculated;
        }
    }
}